//using Syroot.BinaryData;
using System;
using System.Collections.Generic;
using System.IO;

namespace ThimbleweedLibrary
{
    public static class XorCryptors
    {
        private static readonly byte[] magic_bytes_thimbleweed = new byte[] { 0x4F, 0xD0, 0xA0, 0xAC, 0x4A, 0x5B, 0xB9, 0xE5, 0x93, 0x79, 0x45, 0xA5, 0xC1, 0xCB, 0x31, 0x93 };
        private static readonly byte[] magic_bytes_delores = new byte[] { 0x3F, 0x41, 0x41, 0x60, 0x95, 0x87, 0x4A, 0xE6, 0x34, 0xC6, 0x3A, 0x86, 0x29, 0x27, 0x77, 0x8D, 0x38, 0xB4, 0x96, 0xC9, 0x38, 0xB4, 0x96, 0xC9, 0x00, 0xE0, 0x0A, 0xC6, 0x00, 0xE0, 0x0A, 0xC6, 0x00, 0x3C, 0x1C, 0xC6, 0x00, 0x3C, 0x1C, 0xC6, 0x00, 0xE4, 0x40, 0xC6, 0x00, 0xE4, 0x40, 0xC6 };

        public static IXorCryptor Cryptor_RTMI = new RtmiXorCryptor();
        public static IXorCryptor Cryptor_Delores = new TwpXorCryptor(BundleFileVersion.Version_Delores, magic_bytes_delores);
        public static IXorCryptor Cryptor_849 = new TwpXorCryptor(BundleFileVersion.Version_849, magic_bytes_thimbleweed);
        public static IXorCryptor Cryptor_918 = new TwpXorCryptor(BundleFileVersion.Version_918, magic_bytes_thimbleweed);
        public static IXorCryptor Cryptor_957 = new TwpXorCryptor(BundleFileVersion.Version_957, magic_bytes_thimbleweed);

        public static readonly Dictionary<BundleFileVersion, IXorCryptor> Cryptors = new Dictionary<BundleFileVersion, IXorCryptor>()
        {
            { BundleFileVersion.Version_RtMI, Cryptor_RTMI },
            { BundleFileVersion.Version_Delores, Cryptor_Delores },
            { BundleFileVersion.Version_918, Cryptor_918 },
            { BundleFileVersion.Version_957, Cryptor_957 },
            { BundleFileVersion.Version_849, Cryptor_849 },
        };
    }

    public interface IXorCryptor
    {
        BundleFileVersion FileVersion { get; }
        bool Decrypt(Stream source, Stream target, string packFileLocation, int length = -1);
        bool Encrypt(Stream source, Stream target, string packFileLocation, int length = -1);

    }

    public class RtmiXorCryptor : IXorCryptor
    {
        public BundleFileVersion FileVersion => BundleFileVersion.Version_RtMI;

        public bool Decrypt(Stream source, Stream target, string packFileLocation, int length = -1)
        {
            if (length < 0) length = (int)(source.Length - source.Position);
            if (length == 0) return false;
            byte[] data = new byte[length];
            source.Read(data, 0, data.Length);
            RtMIKeyReader.ComputeXOR(ref data, packFileLocation);
            target.Write(data, 0, data.Length);
            target.Position = 0;
            return true;
        }

        public bool Encrypt(Stream source, Stream target, string packFileLocation, int length = -1)
        {
            return Decrypt(source, target, packFileLocation, length); // Encryption and decryption are the same for this algorithm
        }
    }

    public class TwpXorCryptor : IXorCryptor
    {
        private byte[] magic_bytes;
        private int magicNumber = 109;

        public TwpXorCryptor(BundleFileVersion version, byte[] magicBytes)
        {
            if (version == BundleFileVersion.Version_957) magicNumber = -83;
            magic_bytes = magicBytes;
            FileVersion = version;
        }

        public BundleFileVersion FileVersion { get; private set; }

        public bool Decrypt(Stream source, Stream target, string packFileLocation, int length = -1)
        {
            if (length < 0) length = (int)(source.Length - source.Position);
            if (length == 0) return false;
            byte[] buffer = new byte[length];
            source.Read(buffer, 0, buffer.Length);

            var buf_len = buffer.Length;
            var eax = buf_len;
            var var4 = buf_len & 255;
            var ebx = 0;

            while (ebx < buf_len)
            {
                eax = ebx & 255;
                eax = eax * magicNumber;
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

            target.Write(buffer, 0, buffer.Length);
            target.Position = 0;
            return true;
        }

        public bool Encrypt(Stream source, Stream target, string packFileLocation, int length = -1)
        {
            // Todo
            throw new NotImplementedException();
        }
    }

}