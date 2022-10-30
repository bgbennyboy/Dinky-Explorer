//using Syroot.BinaryData;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace ThimbleweedLibrary
{
    public enum BundleFileVersion
    {
        Unknown,

        Version_849,
        Version_918,
        Version_957,
        Version_Delores,
        Version_RtMI,
    }

    public class BundleEntry
    {
        private static string GetMultipleExtension(string path)
        {
            var ret = "";
            for (; ; )
            {
                var ext = Path.GetExtension(path);
                if (String.IsNullOrEmpty(ext))
                    break;
                path = path.Substring(0, path.Length - ext.Length);
                ret = ext + ret;
            }
            return ret;
        }

        private static FileTypes FileTypeFromFileName(string fileName, BundleFileVersion bundle)
        {
            string extension = GetMultipleExtension(fileName).TrimStart('.').ToLower();
            switch (extension)
            {
                case "ogg":
                case "wav":
                    return FileTypes.Sound;
                case "ktxbz":
                case "png":
                    return FileTypes.Image;
                case "bnut":
                    return FileTypes.Bnut;
                case "json":
                    if (bundle == BundleFileVersion.Version_RtMI) return FileTypes.GGDict;
                    return FileTypes.Text;
                case "txt":
                case "tsv":
                case "nut":
                case "fnt":
                case "byack":
                case "lip":
                case "yack":
                case "dinky":
                case "atlas":
                case "anim":
                case "attach":
                case "blend":
                    return FileTypes.Text;
                case "emitter":
                case "wimpy":
                    return FileTypes.GGDict;
                case "assets.bank":
                    return FileTypes.Soundbank;
                case "dink":
                case "vanilla.dink": // added by this tool to preserve the original script file.
                    return FileTypes.CompiledScript;
                default:
                    return FileTypes.None;
            }
        }

        public enum FileTypes
        {
            None,
            Image,
            Sound,
            Text,
            Bnut,
            GGDict,
            Soundbank,
            CompiledScript,
        };

        public string FileName;
        public string FileExtension;
        public long Offset = -1;
        public int Size = -1;
        public FileTypes FileType = FileTypes.None;

        private IXorCryptor Cryptor;
        private Stream sourceStream;
        private bool EncryptedInSource;

        public BundleEntry(Dictionary<string, object> fileinfo, IXorCryptor cryptor, Stream sourceStream)
        {
            FileName = fileinfo["filename"] as string ?? "";
            Offset = (int)fileinfo["offset"];
            Size = (int)fileinfo["size"];
            Cryptor = cryptor;
            this.sourceStream = sourceStream;
            EncryptedInSource = true;

            FileExtension = Path.GetExtension(FileName).TrimStart('.');
            FileType = BundleEntry.FileTypeFromFileName(FileName, Cryptor.FileVersion);
        }

        public BundleEntry(Stream dataStream, IXorCryptor cryptor, string FileNameInBundle)
        {
            FileName = FileNameInBundle;
            Offset = 0;
            Size = (int)dataStream.Length;

            Cryptor = cryptor;
            EncryptedInSource = false;
            {
                sourceStream = new MemoryStream();
                dataStream.Position = 0;
                dataStream.CopyTo(sourceStream);
            }

            FileExtension = Path.GetExtension(FileName).TrimStart('.');
            FileType = BundleEntry.FileTypeFromFileName(FileName, Cryptor.FileVersion);
        }

        public uint Write(Stream target)
        {
            sourceStream.Position = Offset;
            if (EncryptedInSource || FileExtension == "bank")
            {
                CopyStream(sourceStream, target, Size);
                return (uint)Size;
            }

            using (var data = new MemoryStream())
            {
                CopyStream(sourceStream, data, Size);
                data.Position = 0;
                if (Cryptor.FileVersion == BundleFileVersion.Version_RtMI && FileExtension == "yack")
                {
                    byte[] b = data.ToArray();
                    RtMIKeyReader.ComputeXORYack(ref b, FileName.Length - ".yack".Length, "");
                    data.Position = 0;
                    data.SetLength(0);
                    data.Write(b, 0, b.Length);
                    data.Position = 0;
                }

                var pos = target.Position;
                Cryptor.Encrypt(data, target, "");
                target.Position = pos + data.Length;
                return (uint)data.Length;
            }
        }

        public void Extract(Stream target, bool autodecode = true)
        {
            sourceStream.Position = Offset;

            // Raw file (unencrypted)?
            {
                bool skipDecode = false;
                // if the file was not in encrypted form to begin with, we do not need to decrypt it.
                if (!EncryptedInSource) skipDecode = true;
                // In Rtmi, the FMOD .bank files used for audio do not seem to be Xor'd. 
                if ((Cryptor.FileVersion == BundleFileVersion.Version_RtMI && FileExtension == "bank")) skipDecode = true;

                if (skipDecode)
                {
                    var pos = target.Position;
                    CopyStream(sourceStream, target, Size);
                    target.Position = pos;
                    return;
                }
            }

            using (var unpacked = new MemoryStream())
            {
                if (!Cryptor.Decrypt(sourceStream, unpacked, "", Size)) throw new InvalidDataException("Could not decrypt file.");

                // Decode BNUT?
                if ((autodecode == true) && (FileType == FileTypes.Bnut)) Decoders.DecodeBnut(unpacked);
                // Decode YACK?
                if (Cryptor.FileVersion == BundleFileVersion.Version_RtMI && FileExtension == "yack")
                {
                    var yack_bytes = unpacked.ToArray();
                    RtMIKeyReader.ComputeXORYack(ref yack_bytes, FileName.Length - ".yack".Length, "");
                    unpacked.Position = 0;
                    unpacked.SetLength(0);
                    unpacked.Write(yack_bytes, 0, yack_bytes.Length);
                    unpacked.Position = 0;
                }
                //Decode GGDict
                if ((autodecode == true) && (FileType == FileTypes.GGDict))
                {
                    GGDict dict = new GGDict(unpacked, Cryptor.FileVersion == BundleFileVersion.Version_RtMI);
                    string line = dict.ToJsonString();
                    unpacked.Position = 0;
                    unpacked.SetLength(0);
                    var data = Encoding.UTF8.GetBytes(line);
                    unpacked.Write(data, 0, data.Length);
                    unpacked.Position = 0;
                }

                var pos = target.Position;
                unpacked.CopyTo(target);
                target.Position = pos;
            }
        }

        public override string ToString()
        {
            return $"Offset: 0x{Offset:X8} Size: {Size,8} Name: {FileName}";
        }


        //Copy between streams
        private static void CopyStream(Stream input, Stream output, int bytes)
        {
            byte[] buffer = new byte[32768];
            int read;
            while (bytes > 0 &&
                   (read = input.Read(buffer, 0, Math.Min(buffer.Length, bytes))) > 0)
            {
                output.Write(buffer, 0, read);
                bytes -= read;
            }
        }
    }

    public class BundleReader_ggpack : IDisposable
    {
        public delegate void LogEventType(string message);
        public event LogEventType LogEvent;

        public IXorCryptor Cryptor { get; private set; }
        public List<BundleEntry> BundleFiles = new List<BundleEntry>();

        private string BundleFilename;
        private BinaryReader fileReader;
        private bool _disposed = false;
        private string guid = null;

        //Constructor
        public BundleReader_ggpack(string ResourceFile)
        {
            try
            {
                BundleFilename = ResourceFile;
                using (var originalFile = File.OpenRead(BundleFilename))
                {
                    // Use a copy of the file to avoid keeping the original file open.
                    fileReader = new BinaryReader(new FileSystemBackedStream(originalFile));
                }
                DetectBundle();

                // parse files
                {
                    uint DictOffset = fileReader.ReadUInt32();
                    uint DictSize = fileReader.ReadUInt32();

                    fileReader.BaseStream.Position = DictOffset;
                    using (var FileDict = new MemoryStream())
                    {
                        if (!Cryptor.Decrypt(fileReader.BaseStream, FileDict, BundleFilename, (int)DictSize)) throw new ArgumentException("Error parsing the packfile. Decode failed!");
                        GGDict bundleDict = new GGDict(FileDict, Cryptor.FileVersion == BundleFileVersion.Version_RtMI);
                        if (bundleDict.Root.ContainsKey("guid"))
                        {
                            Console.WriteLine($"Opened archive {bundleDict.Root["guid"]}");
                            guid = bundleDict.Root["guid"] as string;
                        }

                        if (!bundleDict.Root.ContainsKey("files") || bundleDict.Root["files"].GetType() != typeof(object).MakeArrayType()) throw new Exception("Dictionary does not contain files or files is not an array.");
                        Dictionary<string, object>[] files = (bundleDict.Root["files"] as object[]).Select(s => s as Dictionary<string, object>).Where(o => o != null).ToArray();

                        foreach (var fileinfo in files)
                        {
                            string filename = fileinfo["filename"] as string ?? "";
                            BundleFiles.Add(new BundleEntry(fileinfo, Cryptor, fileReader.BaseStream));
                        }
                    }
                }
            }
            catch (Exception)
            {
                try
                {
                    Dispose();
                }
                catch { }

                throw;
            }
        }

        protected virtual void Log(string e) => LogEvent?.Invoke(e);

        private void DetectBundle()
        {
            uint DataOffset = fileReader.ReadUInt32();
            uint DataSize = fileReader.ReadUInt32();

            Cryptor = DetectFileVersion(fileReader.BaseStream, (int)DataOffset, (int)DataSize);
            if (Cryptor == null) throw new ArgumentException("Invalid ggpack file!");

            fileReader.BaseStream.Position = 0;
        }

        private IXorCryptor DetectFileVersion(Stream fileStream, int DataOffset, int DataSize)
        {
            using (BinaryReader decReader = new BinaryReader(new MemoryStream())) //Frees when done + underlying stream
            {
                foreach (var cryptor in XorCryptors.Cryptors.Values)
                {
                    decReader.BaseStream.Position = 0;
                    decReader.BaseStream.SetLength(0);

                    fileStream.Position = DataOffset;

                    if (cryptor.Decrypt(fileStream, decReader.BaseStream, BundleFilename, DataSize))
                    {
                        int header = decReader.ReadInt32();
                        //Seek past 4 unknown bytes = 0x00000001
                        int unknown = decReader.ReadInt32();
                        if (header == 0x04030201 && unknown == 0x00000001)
                        {
                            return cryptor;
                        }
                    }
                }
            }
            return null;
        }

        public void SaveFileToStream(int FileNo, Stream DestStream, bool Autodecode = true)
        {
            if (FileNo < 0 || FileNo > BundleFiles.Count)
                throw new ArgumentException(FileNo.ToString() + " Invalid file number! Save cancelled.");
            BundleFiles[FileNo].Extract(DestStream, Autodecode);
        }

        public void SaveFile(int FileNo, string PathAndFileName, Boolean Autodecode = true)
        {
            if (FileNo < 0 || FileNo > BundleFiles.Count)
                throw new ArgumentException(FileNo.ToString() + " Invalid file number! Save cancelled.");

            using (Stream file = File.Create(PathAndFileName))
            {
                SaveFileToStream(FileNo, file, Autodecode);
            }
        }

        public BundleEntry AddFile(Stream file, string filenameInPack)
        {
            var existingFile = BundleFiles.Where(f => f.FileName.ToLowerInvariant() == filenameInPack.ToLowerInvariant()).FirstOrDefault();
            if (existingFile != null) BundleFiles.Remove(existingFile);

            BundleEntry entry = new BundleEntry(file, Cryptor, filenameInPack);
            BundleFiles.Add(entry);
            return entry;
        }

        public BundleEntry AddFile(string filenameOnDisk)
        {
            string filename = Path.GetFileName(filenameOnDisk);
            using (var file = File.OpenRead(filenameOnDisk))
            {
                return AddFile(file, filename);
            }
        }

        public void Save()
        {
            string backupName = BundleFilename + ".backup";
            int iBackup = 0;
            while(File.Exists(backupName))
            {
                ++iBackup;
                backupName = BundleFilename + $".backup{iBackup}";
            }

            Log($"The previous version of the file will be copied to \"{backupName}\".");
            File.Copy(BundleFilename, backupName);
            using (var filestream = File.Create(BundleFilename))
            {
                var bw = new BinaryWriter(filestream);
                bw.Write((uint)0); // Reserved
                bw.Write((uint)0); // Reserved

                List<Dictionary<string, object>> files = new List<Dictionary<string, object>>();

                foreach(var bundleFile in BundleFiles)
                {
                    int offset = (int)filestream.Position;
                    int length = (int)bundleFile.Write(filestream);
                    files.Add(new Dictionary<string, object>() { { "filename", bundleFile.FileName }, { "offset", offset }, { "size", length } });
                }

                Dictionary<string, object> root = new Dictionary<string, object>()
                {
                    { "files", files.Select(f => (object)f).ToArray() }
                };

                if (!String.IsNullOrWhiteSpace(guid)) root["guid"] = guid;

                uint ggdictOffset = (uint)filestream.Position;

                GGDict dict = new GGDict(root, Cryptor.FileVersion == BundleFileVersion.Version_RtMI);
                using (var ms = new MemoryStream())
                {
                    dict.Write(ms);
                    ms.Position = 0;
                    Cryptor.Encrypt(ms, filestream, "");
                }

                uint ggdictLength = (uint)(filestream.Length - ggdictOffset);

                filestream.Position = 0;
                bw.Write(ggdictOffset);
                bw.Write(ggdictLength);

                filestream.Flush();
            }
        }


        #region IDisposable
        //Destructor
        ~BundleReader_ggpack()
        {
            Dispose(false);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    // Dispose any managed objects
                    // ...
                    if (fileReader != null) fileReader.Dispose();
                }

                // Now disposed of any unmanaged objects
                // ...

                _disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }

}