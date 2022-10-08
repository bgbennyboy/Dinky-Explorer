//using Syroot.BinaryData;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ThimbleweedLibrary
{
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