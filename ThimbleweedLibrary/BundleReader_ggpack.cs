using Syroot.BinaryData;
using System;
using System.Collections.Generic;
using System.IO;

namespace ThimbleweedLibrary
{
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
            Text
        };

        public string FileName;
        public string FileExtension;
        public Int64 Offset = -1;
        public int Size = -1;
        public FileTypes FileType = FileTypes.None;
    }

    public class BundleReader_ggpack : IDisposable
    {
        public List<BundleEntry> BundleFiles;
        public event EventHandler<StringEventArgs> LogEvent;
        private string BundleFilename;
        private BinaryStream fileReader;
        private bool _disposed = false;
        
        //Constructor
        public BundleReader_ggpack(string ResourceFile)
        {
            BundleFilename = ResourceFile;

            fileReader = new BinaryStream(File.Open(BundleFilename, FileMode.Open));

            if (DetectBundle() == false)
            {
                throw new ArgumentException("Invalid ggpack file!");
            }

            BundleFiles = new List<BundleEntry>();
            ParseFiles();
            UpdateFileTypes();
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
                    fileReader.Dispose();
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
            uint DataOffset = fileReader.ReadUInt32();
            uint DataSize = fileReader.ReadUInt32();
            fileReader.BaseStream.Position = DataOffset;

            using (BinaryStream decReader = new BinaryStream(new MemoryStream())) //Frees when done + underlying stream
            {
                CopyStream(fileReader.BaseStream, decReader.BaseStream, Convert.ToInt32(DataSize));
                decReader.Position = 0;

                //Decode data records
                if (DecodeUnbreakableXor((MemoryStream)decReader.BaseStream) == false)
                    return false;

                //using (FileStream file = new FileStream("c:/users/ben/desktop/file.bin", FileMode.Create, System.IO.FileAccess.Write))
                //    fileReader.CopyTo(file);
                //    fileReader.Position = 0;

                //Check header is valid. First dword is 01,02,03,04
                if (decReader.ReadInt32() == 67305985)
                    return true;
                else
                    return false;
            }
        }

        /// <summary>
        /// Takes a stream and decodes it, overwriting the contents. Doesnt actually check if valid data produced.
        /// </summary>
        /// <param name="DecodeStream"></param>
        /// <returns></returns>
        private bool DecodeUnbreakableXor(MemoryStream DecodeStream)
        {
            var magic_bytes = new byte[] { 0x4F, 0xD0, 0xA0, 0xAC, 0x4A, 0x5B, 0xB9, 0xE5, 0x93, 0x79, 0x45, 0xA5, 0xC1, 0xCB, 0x31, 0x93 };  //0x5B - possibly 0x56?
            var buffer = DecodeStream.ToArray();

            //alt way to get data from generic stream to array buffer
            //byte[] buffer = new byte[DecodeStream.Length];
            //DecodeStream.Read(buffer, 0, (int)DecodeStream.Length);

            var buf_len = buffer.Length;
            var eax = buf_len;
            var var4 = buf_len & 255;
            var ebx = 0;
            while (ebx < buf_len)
            {
                eax = ebx & 255;
                eax = eax * -83; // 109;
                var ecx = ebx & 15;
                eax = (eax ^ magic_bytes[ecx]) & 255;
                ecx = var4;
                eax = (eax ^ ecx) & 255;
                buffer[ebx] = Convert.ToByte(buffer[ebx] ^ eax);
                ecx = ecx ^ buffer[ebx];
                ebx = ebx + 1;
                var4 = ecx;
            }

            //Loop through in blocks of 16 and xor the 6th and 7th bytes
            int i = 5;
            while (i + 1 < buf_len)
            {
                buffer[i] = Convert.ToByte(buffer[i] ^ 0x0D);
                buffer[i + 1] = Convert.ToByte(buffer[i + 1] ^ 0x0D);
                i += 16;
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

        //Parse the bundle, extracting information about the files and adding BundleEntry objects for each
        public void ParseFiles()
        {
            fileReader.Position = 0;
            uint DataOffset = fileReader.ReadUInt32();
            uint DataSize = fileReader.ReadUInt32();
            fileReader.BaseStream.Position = DataOffset;

            using (BinaryStream decReader = new BinaryStream(new MemoryStream())) //Frees when done + underlying stream
            {
                CopyStream(fileReader.BaseStream, decReader.BaseStream, Convert.ToInt32(DataSize));
                decReader.Position = 0;

                //Decode all data records
                if (DecodeUnbreakableXor((MemoryStream)decReader.BaseStream) == false)
                    throw new ArgumentException("Error parsing the packfile. Decode failed!");

                //Check header is valid. First dword is 01,02,03,04
                if (decReader.ReadInt32() != 67305985)
                    throw new ArgumentException("Error parsing the packfile. Header invalid!");

                //Seek past 4 unknown bytes = 01000000
                decReader.Seek(4, SeekOrigin.Current);

                //Read and go to the start of records offset
                int RecordsStartOffset = decReader.ReadInt32() + 1;
                decReader.Position = RecordsStartOffset;

                /*
                File records offsets section contains a series of offsets to the different information.
                Normal entries are 'Filename offset', 'DataOffset offset' and 'Size offset' - Seek to each offset and the value is stored as a string.
                However some filesize entries are missing and for 1 file in .ggpack2 the offset entry is missing.
                So you dont know until you seek to the offset if it is what you expect or if you're looking at a filename rather than the size/offset value you were expecting.
                This code works on the following basis:
                    Filename entry is always there so just get that.
                    If you read the offset value and it cant be converted to an integer - its a filename and there isnt an offset value. So add the file record you've got, seek back and start a new record.
                    If you read the size value and it cant be converted to an integer - its a filename and there isnt an size value. So add the file record you've got, seek back and start a new record.
                    If everything is fine and you've got a filename, offset and size then add the record and start a new one.
                    After we've read everything - correct the sizes and offsets by looking at the difference between current and previous files.
                */
                BundleEntry bundleEntry = new BundleEntry();
                while (true)
                {
                    //Read the offset of the next string, go there, read it and come back.
                    uint NextOffset = decReader.ReadUInt32();
                    if (NextOffset == 4294967295) //0xFFFFFFFF - marker for end of all offset entries
                        break;

                    long OldPos = decReader.Position;
                    decReader.Position = NextOffset;
                    string TmpString = decReader.ReadString(StringCoding.ZeroTerminated); //.ReadStringASCIINullTerminated();
                    decReader.Position = OldPos;

                    //Swallow these string markers
                    if (TmpString == "files" || TmpString == "filename" || TmpString == "offset" || TmpString == "size")
                        continue;

                    //Check which one we are up to by seeing which is 'empty' file/offset/size
                    if (bundleEntry.FileName == null)
                    {
                        bundleEntry.FileName = TmpString;
                        bundleEntry.FileExtension = Path.GetExtension(TmpString).TrimStart('.');
                    }
                    else if (bundleEntry.Offset == -1)
                    {
                        int TmpInt = -1;
                        if (Int32.TryParse(TmpString, out TmpInt) == false) //No offset value
                        {
                            bundleEntry.Offset = 0;
                            bundleEntry.Size = 0;
                            decReader.Seek(-4, SeekOrigin.Current); //Seek back as we are probably looking at a filename now
                            BundleFiles.Add(bundleEntry); //Add the completed entry to the list
                            bundleEntry = new BundleEntry(); //Make a new one
                            continue;
                        }
                        else
                        {
                            bundleEntry.Offset = TmpInt;
                        }
                    }
                    else if (bundleEntry.Size == -1)
                    {
                        int TmpInt = -1;
                        if (Int32.TryParse(TmpString, out TmpInt) == false) //No size value
                        {
                            bundleEntry.Size = 0;
                            decReader.Seek(-4, SeekOrigin.Current); //seek back as we are probably looking at a filename now
                        }
                        else
                        {
                            bundleEntry.Size = TmpInt;
                        }

                        BundleFiles.Add(bundleEntry); //Add the completed entry to the list
                        bundleEntry = new BundleEntry(); //Make a new one
                        continue;
                    }
                }

                //Now correct missing sizes / offsets
                for (int i = 0; i < BundleFiles.Count; i++)
                {
                    if (BundleFiles[i].Offset == 0) //So far only seen in 1 file in .ggpack2
                    {
                        BundleFiles[i].Offset = BundleFiles[i - 1].Offset + BundleFiles[i - 1].Size; //BundleFiles[i + 1].Offset - BundleFiles[i - 1].Offset;
                    }
                    if (BundleFiles[i].Size == 0)
                    {
                        if (i == BundleFiles.Count - 1) //Last entry - look at difference between data offset and its offset
                        {
                            BundleFiles[i].Size = Convert.ToInt32(DataOffset - BundleFiles[i].Offset);
                        }
                        else
                            if (BundleFiles[i + 1].Offset > 0) //1 file in ggpack2 has no size and the file after has no offset
                            BundleFiles[i].Size = Convert.ToInt32(BundleFiles[i + 1].Offset - BundleFiles[i].Offset); //Look at difference between this and next offset
                    }
                }
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

                    case "png":
                        BundleFiles[i].FileType = BundleEntry.FileTypes.Image;
                        break;

                    case "txt":
                    case "tsv":
                    case "nut":
                    case "json":
                    case "fnt":
                    case "byack":
                    case "lip":
                        BundleFiles[i].FileType = BundleEntry.FileTypes.Text;
                        break;
                }
            }
        }

        //Save a single file
        public void SaveFile(int FileNo, string PathAndFileName)
        {
            if (FileNo < 0 || FileNo > BundleFiles.Count)
                throw new ArgumentException(FileNo.ToString() + " Invalid file number! Save cancelled.");

            using (Stream file = File.Create(PathAndFileName))
            {
                SaveFileToStream(FileNo, file);
            }
        }

        //Save a file to a stream
        public void SaveFileToStream(int FileNo, Stream DestStream)
        {
            if (FileNo < 0 || FileNo > BundleFiles.Count)
                throw new ArgumentException(FileNo.ToString() + " Invalid file number! Save cancelled.");
            if (BundleFiles[FileNo].Size == 0)
                //throw new ArgumentException(BundleFiles[FileNo].FileName + " File num " + FileNo.ToString() + " Filesize = 0 Skipping this file.");
                Log( BundleFiles[FileNo].FileName + " File num " + FileNo.ToString() + " Filesize = 0 Skipping this file.");


            using (MemoryStream ms = new MemoryStream())
            {
                fileReader.Position = BundleFiles[FileNo].Offset;
                CopyStream(fileReader.BaseStream, ms, BundleFiles[FileNo].Size);
                ms.Position = 0;

                //Decode data records
                if (DecodeUnbreakableXor(ms) == true)
                {
                    ms.Position = 0;
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
}