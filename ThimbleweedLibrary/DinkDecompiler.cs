using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThimbleweedLibrary
{
    public class DinkDecompiler
    {
        BinaryReader dink;
        public DinkDecompiler(Stream file)
        {
            dink = new BinaryReader(file);
            dink.BaseStream.Position = 0;

            List<byte[]> ScriptFiles = new List<byte[]>();

            while (dink.BaseStream.Position < dink.BaseStream.Length)
            {
                uint FileMagic = dink.ReadUInt32();
                if (FileMagic != 0x3441789C) throw new InvalidDataException("Unknown file magic.");

                uint ScriptLength = dink.ReadUInt32();

                byte[] ScriptFile = dink.ReadBytes((int)ScriptLength);
                ScriptFiles.Add(ScriptFile);
            }

            foreach (var script in ScriptFiles)
            {
                Functions.Add(ParseScript(script));
            }

        }

        public List<ParsedFunction> Functions = new List<ParsedFunction>();

        public class Block
        {
            public uint typeCode;
            public byte[] data;
        }

        public class ParsedFunction
        {
            public List<Block> blocks = new List<Block>();

            public string ScriptName;
            public string UID;
            public string FunctionName;

            public uint unknownInfo1;
            public uint unknownInfo2;

            public List<string> StringData = new List<string>();

            public override string ToString()
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine($"{UID}: {ScriptName}::{FunctionName}()");
                sb.AppendLine();
                sb.AppendLine("Strings:");
                int i = 0;
                foreach (var s in StringData) sb.AppendLine($"{(i++).ToString("X3")}: \"{s}\"");
                sb.AppendLine();
                sb.AppendLine("Other blocks:");
                foreach (var b in blocks)
                {
                    if (b.typeCode != 0x16F94B62 && b.typeCode != 0x983f1cfa)
                    {
                        sb.AppendLine();
                        sb.AppendLine(b.typeCode.ToString("X8"));

                        sb.Append("[");
                        for (int x = 0; x < b.data.Length / 4; ++x)
                        {
                            for (int y = 0; y < 4; ++y)
                            {
                                sb.Append(b.data[x * 4 + y].ToString("X2"));
                            }
                            sb.Append(", ");
                        }
                        sb.AppendLine("]");
                    }
                }


                return sb.ToString();
            }
        }

        ParsedFunction ParseScript(byte[] script)
        {
            using (var ms = new MemoryStream(script))
            {
                BinaryReader br = new BinaryReader(ms);


                ParsedFunction pf = new ParsedFunction();

                while (br.BaseStream.Position < br.BaseStream.Length)
                {
                    uint code = br.ReadUInt32();
                    switch (code)
                    {
                        case 0x7F46a125:
                            {
                                uint unk1 = br.ReadUInt32();
                                uint unk2 = br.ReadUInt32();
                                uint unk3 = br.ReadUInt16();
                            }
                            break;
                        default:
                            {
                                uint length = br.ReadUInt32();
                                byte[] data = br.ReadBytes((int)length);
                                pf.blocks.Add(new Block() { data = data, typeCode = code });
                            }
                            break;
                    }
                }

                foreach (var block in pf.blocks)
                {
                    switch (block.typeCode)
                    {
                        case 0x16F94B62:
                            {
                                using (var ms2 = new MemoryStream(block.data))
                                {
                                    var br2 = new BinaryReader(ms2);
                                    pf.UID = br2.ReadNullTerminatedString();
                                    pf.FunctionName = br2.ReadNullTerminatedString();
                                    pf.ScriptName = br2.ReadNullTerminatedString();
                                    pf.unknownInfo1 = br2.ReadUInt32();
                                    pf.unknownInfo2 = br2.ReadUInt32();
                                }
                            }
                            break;
                        case 0x983f1cfa:
                            {
                                using (var ms2 = new MemoryStream(block.data))
                                {
                                    var br2 = new BinaryReader(ms2);

                                    while (ms2.Position < ms2.Length)
                                    {
                                        pf.StringData.Add(br2.ReadNullTerminatedString());
                                    }
                                }
                            }
                            break;
                    }
                }

                return pf;

            }
        }

        public override string ToString()
        {
            return String.Join("\n\n\n", Functions.Select(s => s.ToString()));
        }
    }
}
