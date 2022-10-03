using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fmod5Sharp;
using Fmod5Sharp.FmodTypes;

namespace ThimbleweedLibrary
{
    public class FMODBankExtractor
    {
        private static readonly byte[] remi_fsb_key = new byte[] { 0x73, 0x6B, 0x6B, 0x70, 0x79, 0x63, 0x77, 0x74, 0x78, 0x7A, 0x78, 0x6E, 0x62, 0x6F, 0x7A, 0x64, 0x30, 0x68, 0x62, 0x31, 0x69, 0x61, 0x6C, 0x30, 0x68, 0x78, 0x6E, 0x72, 0x62, 0x75, 0x6F, 0x30 };

        public event EventHandler<StringEventArgs> LogEvent;

        //Used for the log event
        protected virtual void Log(string e)
        {
            this.LogEvent?.Invoke(this, new StringEventArgs(e));
        }

        private static byte Reverse(byte b)
        {
            int rev = (b >> 4) | ((b & 0xf) << 4);
            rev = ((rev & 0xcc) >> 2) | ((rev & 0x33) << 2);
            rev = ((rev & 0xaa) >> 1) | ((rev & 0x55) << 1);

            return (byte)rev;
        }

        // Reverses bits in each byte in the array 
        private static void Reverse(byte[] bytes)
        {
            // Precompute the value of each reversed byte 
            byte[] reversed = new byte[256];
            for (int i = 0; i < 256; i++) reversed[i] = Reverse((byte)i);

            // Reverse each byte in the input 
            for (int i = 0; i < bytes.Length; i++) bytes[i] = reversed[bytes[i]];
        }

        private int Search(byte[] src, byte[] pattern)
        {
            int maxFirstCharSlot = src.Length - pattern.Length + 1;
            for (int i = 0; i < maxFirstCharSlot; i++)
            {
                if (src[i] != pattern[0]) // compare only first byte
                    continue;

                // found a match on first byte, now try to match rest of the pattern
                for (int j = pattern.Length - 1; j >= 1; j--)
                {
                    if (src[i + j] != pattern[j]) break;
                    if (j == 1) return i;
                }
            }
            return -1;
        }

        private static void DecryptFSB5(ref byte[] SourceBytes, byte[] Key)
        {
            var j = 0;
            Reverse(SourceBytes);
            for (int i = 0; i < SourceBytes.Length; i++)
            {
                SourceBytes[i] = (byte)(SourceBytes[i] ^ Key[j]);
                j++;
                if (j == Key.Length)
                {
                    j = 0;
                }
            }
        }

        private byte[] FSBarray;
        public FMODBankExtractor(MemoryStream stream)
        {
            var buffer = stream.ToArray();
            DecryptFSB5(ref buffer, remi_fsb_key);
            var FSBIndex = Search(buffer, new byte[] { 0x46, 0x53, 0x42, 0x35 }); //"FSB5"
            if (FSBIndex > -1) //Search for FSB5 header
            {
                FSBarray = new byte[buffer.Length - FSBIndex];
                Array.Copy(buffer, FSBIndex, FSBarray, 0, buffer.Length - FSBIndex);
            }
            else
            {
                throw new Exception("No FSB header found");
            }
        }

        public void SaveAllToDir(string DestDir)
        {
            FmodSoundBank bank = FsbLoader.LoadFsbFromByteArray(FSBarray);
            if (bank.Header.NumSamples == 0)
                throw new Exception("No samples found in FSB");

            var i = 0;
            foreach (var bankSample in bank.Samples)
            {
                i++;
                var name = bankSample.Name ?? $"sample-{i}";

                if (!bankSample.RebuildAsStandardFileFormat(out var data, out var extension))
                {
                    Log($"Failed to extract sample {name}");
                    continue;
                }

                var filePath = Path.Combine(DestDir, $"{name}.{extension}");
                File.WriteAllBytes(filePath, data);
                Log($"Extracted sample {name}");
            }
        }

        public string[] EnumerateFiles()
        {
            FmodSoundBank bank = FsbLoader.LoadFsbFromByteArray(FSBarray);
            return bank.Samples.Select(s => s.Name).ToArray();
        }

        public byte[] ExtractSingleFile(string filename, out string extension)
        {
            FmodSoundBank bank = FsbLoader.LoadFsbFromByteArray(FSBarray);
            var sample = bank.Samples.Where(b => b.Name == filename).FirstOrDefault();
            if (sample == null) throw new FileNotFoundException($"File {filename} not found in bank.");
            if (!sample.RebuildAsStandardFileFormat(out byte[] data, out extension))
            {
                throw new Exception($"Failed to extract sample {filename}");
            }
            return data;
        }

    }
}
