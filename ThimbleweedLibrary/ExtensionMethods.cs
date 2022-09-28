using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThimbleweedLibrary
{
    internal static class ExtensionMethods
    {
        public static string ReadNullTerminatedString(this BinaryReader reader)
        {
            List<byte> bytes = new List<byte>();
            byte b;
            do
            {
                b = reader.ReadByte();
                if (b > 0) bytes.Add(b);
            } while (b != 0);

            return Encoding.UTF8.GetString(bytes.ToArray());
        }
    }
}
