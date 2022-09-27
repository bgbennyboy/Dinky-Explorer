using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThimbleweedLibrary
{
    /// <summary>
    /// Class to Read GGDict files
    /// </summary>
    public class GGDict
    {
        private bool shortKeyIndices;
        private Stream stream;
        private BinaryReader reader;

        private List<string> Strings = new List<string>();

        private enum TypeCode
        {
            NULL = 1,
            DICTIONARY = 2,
            ARRAY = 3,
            STRING = 4,
            INTEGER = 5,
            FLOAT = 6,
            OFFSETS = 7,
            // ??
            VECTOR2D = 9,
            VECTOR2DPAIR = 10,
            VECTOR2DTRIPLET = 11,
        }

        public readonly Dictionary<string, object> Root;

        public GGDict(Stream file, bool shortKeyIndices)
        {
            this.stream = file;
            this.shortKeyIndices = shortKeyIndices;
            this.reader = new BinaryReader(stream);

            stream.Position = 0;
            uint signature = reader.ReadUInt32();
            if (signature != 0x04030201) throw new Exception("Invalid File Signature");
            reader.ReadUInt32(); // Always 0x00000001? unknown purpose.
            UInt32 StringListOffset = reader.ReadUInt32();
            stream.Position = StringListOffset + 1;
            uint stringAddress;

            do
            {
                stringAddress = reader.ReadUInt32();
                if (stringAddress < 0xFFFFFFFF)
                {
                    var position = stream.Position;
                    stream.Position = stringAddress;
                    Strings.Add(readNullTerminatedString());
                    stream.Position = position;
                }
            } while (stringAddress < 0xFFFFFFFF);

            stream.Position = 12;

            Root = ReadValue() as Dictionary<string, object>;
            if (Root == null) throw new Exception("Root of file is not a dictionary.");
        }

        #region reading
        object ReadValue()
        {
            byte typeMarkerByte = reader.ReadByte();
            if (!Enum.IsDefined(typeof(TypeCode), (int)typeMarkerByte)) throw new InvalidDataException($"Unknown Type Marker {typeMarkerByte}");
            TypeCode typeMarker = (TypeCode)typeMarkerByte;
            switch (typeMarker)
            {
                case TypeCode.NULL: return null;
                case TypeCode.DICTIONARY: return ReadDictionary();
                case TypeCode.ARRAY: return ReadArray();
                case TypeCode.STRING: return ReadString();
                case TypeCode.INTEGER: return ReadInteger();
                case TypeCode.FLOAT: return ReadFloat();
                case TypeCode.OFFSETS: return null;
                case TypeCode.VECTOR2D: return ReadString();
                case TypeCode.VECTOR2DPAIR: return ReadString();
                case TypeCode.VECTOR2DTRIPLET: return ReadString();
                default: return null; // should be unreachable
            }
        }

        private Dictionary<string, object> ReadDictionary()
        {
            Dictionary<string, object> result = new Dictionary<string, object>();
            uint numItems = reader.ReadUInt32();
            for (uint i = 0; i < numItems; ++i)
            {
                string key = ReadString();
                object value = ReadValue();
                result.Add(key, value);
            }

            var endTypeMarker = reader.ReadByte();
            if (endTypeMarker != (int)TypeCode.DICTIONARY) throw new InvalidDataException("Dictionary not terminated by correct code.");

            return result;
        }

        private object[] ReadArray()
        {
            uint numItems = reader.ReadUInt32();
            List<object> result = new List<object>();
            for (uint i = 0; i < numItems; ++i)
            {
                object value = ReadValue();
                result.Add(value);
            }
            var endTypeMarker = reader.ReadByte();
            if (endTypeMarker != (int)TypeCode.ARRAY) throw new InvalidDataException("Dictionary not terminated by correct code.");
            return result.ToArray();
        }

        private string ReadString()
        {
            uint StringIndex = 0;
            if (shortKeyIndices) StringIndex = reader.ReadUInt16();
            else StringIndex = reader.ReadUInt32();
            if (Strings.Count <= StringIndex) throw new InvalidDataException($"Invalid string index {StringIndex}");
            return Strings[(int)StringIndex];
        }

        private int ReadInteger() => int.Parse(ReadString());
        private double ReadFloat() => double.Parse(ReadString());

        private string readNullTerminatedString()
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

        #endregion

        object CreateJson(object o)
        {
            if (o is Dictionary<string, object> dict)
            {
                return new JObject(dict.Select(kv => new JProperty(kv.Key, CreateJson(kv.Value))).ToArray());
            }
            else if (o is object[] list)
            {
                return new JArray(list.Select(obj => CreateJson(obj)).ToArray());
            }
            else if (o is int i) return i;
            else if (o is string s) return s;
            else if (o is double d) return d;
            else return null;
        }

        public JObject ToJson()
        {
            return CreateJson(Root) as JObject;
        }

        public string ToJsonString()
        {
            return ToJson().ToString();
        }
    }
}
