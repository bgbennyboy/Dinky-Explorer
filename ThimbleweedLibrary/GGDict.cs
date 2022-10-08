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
        private BinaryWriter writer;

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

        public GGDict(Dictionary<string, object> root, bool shortKeyIndices)
        {
            Root = root;
            this.shortKeyIndices = shortKeyIndices;
        }

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
                    Strings.Add(reader.ReadNullTerminatedString());
                    stream.Position = position;
                }
            } while (stringAddress < 0xFFFFFFFF);

            stream.Position = 12;

            Root = ReadValue() as Dictionary<string, object>;
            if (Root == null) throw new Exception("Root of file is not a dictionary.");
        }

        #region writing 

        public void Write(Stream target)
        {
            var start = (uint)target.Position;
            this.stream = target;
            this.writer = new BinaryWriter(stream);
            this.Strings.Clear();

            writer.Write((uint)0x04030201); // signature
            writer.Write((uint)0x00000001); // unknown
            writer.Write((uint)0); // Reserved!
            WriteValue(Root);
            uint stringListOffset = (uint)stream.Position - start;
            writer.Write((byte)TypeCode.OFFSETS);
            // how many bytes of offsets?
            var offsetsLength = (Strings.Count + 1) * 4;

            var stringsBase = stringListOffset + 1 + offsetsLength;
            var stringAddress = stringsBase;
            foreach(var str in Strings)
            {
                writer.Write((uint)stringAddress);
                stringAddress += Encoding.UTF8.GetByteCount(str) + 1;
            }
            writer.Write((uint)0xFFFFFFFF);

            foreach (var str in Strings)
            {
                byte[] data = Encoding.UTF8.GetBytes(str);
                writer.Write(data);
                writer.Write((byte)0);
            }

            stream.Position = 8;
            writer.Write((uint)stringListOffset);
        }

        int write_addString(string s)
        {
            int i = Strings.IndexOf(s);
            if (i >= 0) return i;
            Strings.Add(s);
            return Strings.IndexOf(s);
        }

        void write_writeStringIndex(string s)
        {
            int i = write_addString(s);
            if (shortKeyIndices) writer.Write((ushort)(i));
            else writer.Write((uint)(i));
        }

        void WriteValue(object value)
        {
            if (value is Dictionary<string, object> dictionary)
            {
                writer.Write((byte)TypeCode.DICTIONARY);
                writer.Write((uint)dictionary.Count);
                foreach (var kvp in dictionary)
                {
                    write_writeStringIndex(kvp.Key);
                    WriteValue(kvp.Value);
                }
                writer.Write((byte)TypeCode.DICTIONARY);
            }
            else if (value is object[] array)
            {
                writer.Write((byte)TypeCode.ARRAY);
                writer.Write((uint)array.Length);
                foreach (var val in array)
                {
                    WriteValue(val);
                }
                writer.Write((byte)TypeCode.ARRAY);
            }
            else if (value is string str)
            {
                writer.Write((byte)TypeCode.STRING);
                write_writeStringIndex(str);
            }
            else if (value is int i)
            {
                writer.Write((byte)TypeCode.INTEGER);
                write_writeStringIndex(i.ToString());
            }
            else if (value is double d)
            {
                writer.Write((byte)TypeCode.FLOAT);
                write_writeStringIndex(d.ToString(System.Globalization.CultureInfo.InvariantCulture));
            }
            else
            {
                writer.Write((byte)TypeCode.NULL);
            }
        }

        #endregion

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

        private int ReadInteger()
        {
            string str = ReadString();
            if(str == "files")
            {

            }
            Console.WriteLine(str);
            return int.Parse(str, System.Globalization.CultureInfo.InvariantCulture);
        }
            
        private double ReadFloat() => double.Parse(ReadString(), System.Globalization.CultureInfo.InvariantCulture);



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
