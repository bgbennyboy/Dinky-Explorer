//using Syroot.BinaryData;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ThimbleweedLibrary
{
    public enum BundleFileVersion
    {
        Unknown,

        Version_849,
        Version_918,
        Version_957,
        Version_Delores,
        Version_RtMI
    }

    //For the logging event
    public class StringEventArgs : EventArgs
    {
        public string Message { get; private set; }

        public StringEventArgs(string message)
        {
            this.Message = message;
        }
    }

    public class BundleEntry
    {
        public enum FileTypes
        {
            None,
            Image,
            Sound,
            Text,
            Bnut,
            GGDict,
        };

        public string FileName;
        public string FileExtension;
        public Int64 Offset = -1;
        public int Size = -1;
        public FileTypes FileType = FileTypes.None;

        public override string ToString()
        {
            return $"Offset: 0x{Offset:X8} Size: {Size,8} Name: {FileName}";
        }
    }

    public class BundleReader_ggpack : IDisposable
    {
        private static readonly byte[] magic_bytes_thimbleweed = new byte[] { 0x4F, 0xD0, 0xA0, 0xAC, 0x4A, 0x5B, 0xB9, 0xE5, 0x93, 0x79, 0x45, 0xA5, 0xC1, 0xCB, 0x31, 0x93 };
        private static readonly byte[] magic_bytes_delores = new byte[] { 0x3F, 0x41, 0x41, 0x60, 0x95, 0x87, 0x4A, 0xE6, 0x34, 0xC6, 0x3A, 0x86, 0x29, 0x27, 0x77, 0x8D, 0x38, 0xB4, 0x96, 0xC9, 0x38, 0xB4, 0x96, 0xC9, 0x00, 0xE0, 0x0A, 0xC6, 0x00, 0xE0, 0x0A, 0xC6, 0x00, 0x3C, 0x1C, 0xC6, 0x00, 0x3C, 0x1C, 0xC6, 0x00, 0xE4, 0x40, 0xC6, 0x00, 0xE4, 0x40, 0xC6 };

        public BundleFileVersion FileVersion { get; private set; }
        public List<BundleEntry> BundleFiles;
        public event EventHandler<StringEventArgs> LogEvent;
        private string BundleFilename;
        private BinaryReader fileReader;
        private bool _disposed = false;

        //Constructor
        public BundleReader_ggpack(string ResourceFile)
        {
            try
            {
                BundleFilename = ResourceFile;

                fileReader = new BinaryReader(File.Open(BundleFilename, FileMode.Open));

                if (DetectBundle() == false)
                {
                    throw new ArgumentException("Invalid ggpack file!");
                }

                BundleFiles = new List<BundleEntry>();
                ParseFiles();
                UpdateFileTypes();
            }
            catch (Exception ex)
            {
                try
                {
                    Dispose();
                }
                catch { }
                throw;
            }
        }

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


        //Decrypt the file records and see if its a valid bundle
        private bool DetectBundle()
        {
            bool isValid = false;

            uint DataOffset = fileReader.ReadUInt32();
            uint DataSize = fileReader.ReadUInt32();

            using (BinaryReader decReader = new BinaryReader(new MemoryStream())) //Frees when done + underlying stream
            {
                //Try to decode all versions
                var fileVersions = Enum.GetValues(typeof(BundleFileVersion)).Cast<BundleFileVersion>().Where(v => v != BundleFileVersion.Unknown); //Build an array of enums but exclude the unknown enum
                foreach (var currentFileVersion in fileVersions)
                {
                    FileVersion = currentFileVersion;

                    fileReader.BaseStream.Position = DataOffset;
                    decReader.BaseStream.Position = 0;
                    CopyStream(fileReader.BaseStream, decReader.BaseStream, Convert.ToInt32(DataSize));
                    decReader.BaseStream.Position = 0;

                    //Decode data records
                    if (isValid = DecodeUnbreakableXor((MemoryStream)decReader.BaseStream))
                    {
                        //Check header is valid. First dword is 01,02,03,04
                        int header = decReader.ReadInt32();
                        //Seek past 4 unknown bytes = 0x00000001
                        int unknown = decReader.ReadInt32();
                        isValid = header == 0x04030201 && unknown == 0x00000001;
                    }

                    if (isValid)
                        //Valid bundle found
                        break;
                }
            }

            if (!isValid)
                FileVersion = BundleFileVersion.Unknown;
            return isValid;
        }

        /// <summary>
        /// Takes a stream and decodes it, overwriting the contents. Doesnt actually check if valid data produced.
        /// Originally converted from code by mstr- https://github.com/mstr-/twp-ggdump 
        /// </summary>
        /// <param name="DecodeStream"></param>
        /// <returns></returns>
        private bool DecodeUnbreakableXor(MemoryStream DecodeStream)
        {
            if (FileVersion == BundleFileVersion.Version_RtMI)
            {
                var buffer = DecodeStream.ToArray(); //Put the stream data into a buffer

                RtMIKeyReader.ComputeXOR(ref buffer, BundleFilename);

                DecodeStream.SetLength(0); //empty the stream
                DecodeStream.Write(buffer, 0, buffer.Length); //write the decoded bytes back
                DecodeStream.Position = 0;

                return true;
            }
            else
            {
                //Quick hack for Delores
                byte[] magic_bytes;
                if (FileVersion == BundleFileVersion.Version_Delores)
                    magic_bytes = magic_bytes_delores;
                else
                    magic_bytes = magic_bytes_thimbleweed;


                var buffer = DecodeStream.ToArray(); //Put the stream data into a buffer

                var buf_len = buffer.Length;
                var eax = buf_len;
                var var4 = buf_len & 255;
                var ebx = 0;
                int f = FileVersion == BundleFileVersion.Version_957 ? -83 : 109; //Latest version of TP uses -83 here. All the others use 109
                while (ebx < buf_len)
                {
                    eax = ebx & 255;
                    eax = eax * f;
                    var ecx = ebx & 15;
                    eax = (eax ^ magic_bytes[ecx]) & 255;
                    ecx = var4;
                    eax = (eax ^ ecx) & 255;
                    buffer[ebx] = Convert.ToByte(buffer[ebx] ^ eax);
                    ecx = ecx ^ buffer[ebx];
                    ebx = ebx + 1;
                    var4 = ecx;
                }

                if (FileVersion != BundleFileVersion.Version_849 && FileVersion != BundleFileVersion.Version_Delores)
                {
                    //Loop through in blocks of 16 and xor the 6th and 7th bytes
                    int i = 5;
                    while (i + 1 < buf_len)
                    {
                        buffer[i] = Convert.ToByte(buffer[i] ^ 0x0D);
                        buffer[i + 1] = Convert.ToByte(buffer[i + 1] ^ 0x0D);
                        i += 16;
                    }
                }

                //Check everything has decoded
                if (buffer != null && buffer.Length > 0)
                {
                    DecodeStream.SetLength(0); //empty the stream
                    DecodeStream.Write(buffer, 0, buffer.Length); //write the decoded bytes back
                    DecodeStream.Position = 0;

                    return true;
                }
                return false;
            }
        }

        //Parse the bundle, extracting information about the files and adding BundleEntry objects for each
        public void ParseFiles()
        {
            fileReader.BaseStream.Position = 0;
            uint DataOffset = fileReader.ReadUInt32();
            uint DataSize = fileReader.ReadUInt32();
            fileReader.BaseStream.Position = DataOffset;

            using (BinaryReader decReader = new BinaryReader(new MemoryStream())) //Frees when done + underlying stream
            {
                CopyStream(fileReader.BaseStream, decReader.BaseStream, Convert.ToInt32(DataSize));
                decReader.BaseStream.Position = 0;

                //Decode all data records
                if (DecodeUnbreakableXor((MemoryStream)decReader.BaseStream) == false)
                    throw new ArgumentException("Error parsing the packfile. Decode failed!");

                decReader.BaseStream.Position = 0;

                GGDict dict = new GGDict(decReader.BaseStream, FileVersion == BundleFileVersion.Version_RtMI);
                if (!dict.Root.ContainsKey("files") || dict.Root["files"].GetType() != typeof(object).MakeArrayType()) throw new Exception("Dictionary does not contain files or files is not an array.");
                Dictionary<string, object>[] files = (dict.Root["files"] as object[]).Select(s => s as Dictionary<string, object>).Where(o => o != null).ToArray();

                foreach(var fileinfo in files)
                {
                    BundleEntry entry = new BundleEntry();
                    entry.FileName = fileinfo["filename"] as string ?? "";
                    entry.Offset = (int)fileinfo["offset"];
                    entry.Size = (int)fileinfo["size"];
                    entry.FileExtension = Path.GetExtension(entry.FileName).TrimStart('.');

                    BundleFiles.Add(entry);
                }

                if (dict.Root.ContainsKey("guid")) Console.WriteLine($"Opened archive {dict.Root["guid"]}");
            }
        }

        //Assign filetypes to particular file extensions
        public void UpdateFileTypes()
        {
            for (int i = 0; i < BundleFiles.Count; i++)
            {
                switch (BundleFiles[i].FileExtension)
                {
                    case "ogg":
                    case "wav":
                        BundleFiles[i].FileType = BundleEntry.FileTypes.Sound;
                        break;
                    case "ktxbz":
                    case "png":
                        BundleFiles[i].FileType = BundleEntry.FileTypes.Image;
                        break;

                    case "bnut":
                        BundleFiles[i].FileType = BundleEntry.FileTypes.Bnut;
                        break;

                    case "json":
                        if(FileVersion == BundleFileVersion.Version_RtMI)
                        {
                            BundleFiles[i].FileType = BundleEntry.FileTypes.GGDict;
                        } else
                        {
                            BundleFiles[i].FileType = BundleEntry.FileTypes.Text;
                        }
                        break;
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
                        BundleFiles[i].FileType = BundleEntry.FileTypes.Text;
                        break;

                    case "emitter":
                    case "wimpy":
                        BundleFiles[i].FileType = BundleEntry.FileTypes.GGDict;
                        break;
                }
            }
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

        public void SaveFileToStream(int FileNo, Stream DestStream, Boolean Autodecode = true)
        {
            if (FileNo < 0 || FileNo > BundleFiles.Count)
                throw new ArgumentException(FileNo.ToString() + " Invalid file number! Save cancelled.");
            if (BundleFiles[FileNo].Size == 0)
                //throw new ArgumentException(BundleFiles[FileNo].FileName + " File num " + FileNo.ToString() + " Filesize = 0 Skipping this file.");
                Log(BundleFiles[FileNo].FileName + " File num " + FileNo.ToString() + " Filesize = 0 Skipping this file.");

            using (MemoryStream ms = new MemoryStream())
            {
                fileReader.BaseStream.Position = BundleFiles[FileNo].Offset;
                CopyStream(fileReader.BaseStream, ms, BundleFiles[FileNo].Size);
                ms.Position = 0;

                bool skipDecode = false;
                
                // In Rtmi, the FMOD .bank files used for audio do not seem to be Xor'd. 
                // Although it should be noted that the game uses FMOD's built-in encryption for the files. (The password is easily extracted from the .exe as well)
                if ((FileVersion == BundleFileVersion.Version_RtMI && BundleFiles[FileNo].FileName.ToLowerInvariant().EndsWith(".bank"))) skipDecode = true;

                //Decode data records
                if (skipDecode || DecodeUnbreakableXor(ms) == true)
                {
                    ms.Position = 0;

                    if ((Autodecode == true) && (BundleFiles[FileNo].FileType == BundleEntry.FileTypes.Bnut))
                    {
                        Decoders.DecodeBnut(ms);
                    }

                    if (FileVersion == BundleFileVersion.Version_RtMI && BundleFiles[FileNo].FileName.ToLowerInvariant().EndsWith(".yack"))
                    {
                        // yack files seem to be encrypted twice.
                        var yack_bytes = ms.ToArray();
                        RtMIKeyReader.ComputeXORYack(ref yack_bytes, BundleFiles[FileNo].FileName.Length - ".yack".Length, "");
                        ms.Position = 0;
                        ms.SetLength(0);
                        ms.Write(yack_bytes, 0, yack_bytes.Length);
                        ms.Position = 0;
                    }

                    ms.CopyTo(DestStream);
                }

                DestStream.Position = 0;
            }
        }

        //Copy between streams
        public static void CopyStream(Stream input, Stream output, int bytes)
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

        //Used for the log event
        protected virtual void Log(string e)
        {
            this.LogEvent?.Invoke(this, new StringEventArgs(e));
        }
    }

    /// <summary>
    /// Helps decrypt Return to Monkey Island's resource files.
    /// 
    /// Return to Monkey Island seems to use longer keys than previous titles.
    /// To prevent inclusion of the keys in this project (as the keys themselves might be considered part of the games data and therefore protected by copyright),
    /// we only distribute the MD5-Checksum of the two data arrays (as well as the first byte of each key)
    /// 
    /// Two keys are used, a short 256 byte key beginning with 0x5D and a longer 65536 byte key starting with 0xF7
    /// 
    /// There is a third key used to decrypt the yack files. It is 1024 bytes in length and begins with 0x1F.
    /// To decrypt the .yacks (after being decrypted normally!), it is necessary to know this number. 
    /// By experiment I have discovered that the extra parameter for Carla.yack from Weird.ggpack1a appears to be 5.
    /// </summary>
    public class RtMIKeyReader
    {
        public static byte[] Key1 = null;
        public static byte[] Key2 = null;

        public static byte[] KeyYack = null;

        // short key - checksum
        private static readonly string Key1Checksum = "B190C421FE7FEAFC77C517A232ABBB4C";
        // long key - checksum
        private static readonly string Key2Checksum = "7FAAF6574F27EBD9D2744CC68E4115C8";

        private static readonly string YackKeyChecksum = "506925BB6A72B6ED50C95094275485E6";

        /// <summary>
        /// Decrypts the files in the fashion used by Return to Monkey Island
        /// </summary>
        /// <param name="data">data to en/decrypt</param>
        /// <param name="fileLocation">used to search for the game's executable if the keys have not yet been loaded.</param>
        public static void ComputeXOR(ref byte[] data, string fileLocation)
        {
            EnsureKeys(fileLocation);

            ushort var = (ushort)(((ushort)data.Length) + (ushort)0x78);

            for (int i = 0; i < data.Length; ++i)
            {
                data[i] = (byte)(data[i] ^ (Key1[(byte)((byte)var + (byte)0x78)]) ^ Key2[var]);
                var = (ushort)(var + (Key1[(byte)var]));
            }
        }

        /// <summary>
        /// Decrypts the .yack files. 
        /// Currently unknown where the extra parameter comes from.
        /// 
        /// Working under the assumption that the first byte in the file is 0, I try to guess the correct offset.
        /// The chosen offset the index of the first key-byte to equal the first data byte.
        /// 
        /// This seems to work and the largest offset I've seen was 20.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="fileLocation"></param>
        public static void ComputeXORYack(ref byte[] data, int keyOffset, string fileLocation)
        {
            EnsureKeys(fileLocation);

            // find possible value for extra parameter

            if (keyOffset < 0)
            {
                for (int i = 0; i < KeyYack.Length; ++i)
                {
                    if (KeyYack[i] == data[0])
                    {
                        keyOffset = i;
                        Console.WriteLine($"possible contestant: {i}");
                        break;
                    }
                }
            }

            for (int i = 0; i < data.Length; ++i)
            {
                data[i] = (byte)(data[i] ^ KeyYack[(i + keyOffset) & 0x3ff]);
            }
        }

        /// <summary>
        /// Ensure we have the keys.
        /// </summary>
        /// <param name="fileLocation">If this directory contains a file called "Return to Monkey Island.exe", use this.</param>
        /// <exception cref="KeyNotFoundException">thrown if one or both keys could not be extracted</exception>
        private static void EnsureKeys(string fileLocation)
        {

            if (Key1 == null || Key2 == null || KeyYack == null)
            {
                // We need to get the Keys from the game executable.
                // is there an appopriate file in the resource file's directory?
                string executableName = Path.Combine(Path.GetDirectoryName(fileLocation), "Return to Monkey Island.exe");
                if (!File.Exists(executableName))
                {
                    if (OnSearchForMonkeyIsland != null) executableName = OnSearchForMonkeyIsland();
                    if (String.IsNullOrWhiteSpace(executableName) || !File.Exists(executableName))
                    {
                        throw new KeyNotFoundException("To extract files from Return To Monkey Island, the XOR Keys need to be extracted from the game's executable File. This file was not found.");
                    }
                }

                var miexe = File.ReadAllBytes(executableName);
                Key1 = SearchForKey(miexe, Key1Checksum, 256, 0xD5);
                Key2 = SearchForKey(miexe, Key2Checksum, 65536, 0xF7);
                KeyYack = SearchForKey(miexe, YackKeyChecksum, 1024, 0x1F);

                if (Key1 == null || Key2 == null || KeyYack == null)
                {
                    string WholeFileChecksum = Checksum(miexe, 0, miexe.Length);
                    throw new KeyNotFoundException($"The XOR-Key for Return to Monkey Island could not be extracted from the File \"{executableName}\": Checksum {WholeFileChecksum}");
                }
            }
        }

        private static byte[] SearchForKey(byte[] data, string keyChecksum, int keyLengthBytes, byte firstByte)
        {
            for (int i = 0; i < data.Length - keyLengthBytes; ++i)
            {
                // optimization - including a single byte should be fine.
                if (data[i] == firstByte)
                {
                    string checksum = Checksum(data, i, keyLengthBytes);
                    if (checksum == keyChecksum)
                    {
                        byte[] key = new byte[keyLengthBytes];
                        for (int x = 0; x < keyLengthBytes; ++x) key[x] = data[i + x];
                        return key;
                    }
                }
            }
            return null;
        }

        private static string Checksum(byte[] buffer, int start, int offset)
        {
            using (var MD5 = System.Security.Cryptography.MD5.Create())
            {
                return String.Join("", MD5.ComputeHash(buffer, start, offset).Select(s => s.ToString("X2")));
            }
        }

        public delegate string SearchForMonkeyIslandExeEvent();
        public static event SearchForMonkeyIslandExeEvent OnSearchForMonkeyIsland;
    }
}