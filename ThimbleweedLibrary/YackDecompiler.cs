using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThimbleweedLibrary
{
    public class YackDecompiler
    {

        public class YackStatement
        {
            public enum YackOpcode
            {
                UNKNOWN,
                END_PROGRAM = 0,
                ACTOR_SAY = 0x01,
                EMIT_CODE = 0x08,
                LABEL = 0x09,
                GOTO = 10,
                GOSUB = 0x13,
                DIALOG_CHOICE = 0x64,
            }

            public YackOpcode Opcode;
            public byte RawOpcode;

            public uint LineNumber;
            public uint Unknown;
            public byte ConditionalFlag;

            public int[] parameters;
        }

        private readonly List<string> StringTable = new List<string>();
        private readonly List<YackStatement> Statements = new List<YackStatement>();

        public YackDecompiler(Stream stream)
        {
            BinaryReader yack = new BinaryReader(stream);
            uint magic = yack.ReadUInt32();
            if (magic != 0xDCE67800) throw new InvalidDataException("Invalid file magic");
            uint StringsOffset = yack.ReadUInt32() + 4;

            yack.BaseStream.Position = StringsOffset;
            // Read strings
            uint numStrings = yack.ReadUInt32();

            for (uint i = 0; i < numStrings; ++i) StringTable.Add(yack.ReadNullTerminatedString());

            // Read Lines
            yack.BaseStream.Position = 8;

            while(true)
            {
                byte opcode = yack.ReadByte();
                if (opcode == 0) break;

                YackStatement ys = new YackStatement();
                ys.Opcode = YackStatement.YackOpcode.UNKNOWN;
                ys.RawOpcode = opcode;
                if (Enum.IsDefined(typeof(YackStatement.YackOpcode), (int)opcode)) ys.Opcode = (YackStatement.YackOpcode)opcode;
                
                ys.LineNumber = yack.ReadUInt32();
                ys.Unknown = yack.ReadUInt32();
                ys.ConditionalFlag = yack.ReadByte();

                int numParameter = 2 + ys.ConditionalFlag;
                List<int> parameters = new List<int>();
                for (int i = 0; i < numParameter; ++i) parameters.Add(yack.ReadInt32());
                if(parameters.Last() == -1) parameters.RemoveAt(parameters.Count - 1);

                ys.parameters = parameters.ToArray();
                Statements.Add(ys);
            }

        }

        private string YackStatementToString(YackStatement ys)
        {
            switch(ys.Opcode)
            {
                default:
                case YackStatement.YackOpcode.UNKNOWN:
                    return $"UNKNOWN OPCODE {ys.RawOpcode:X2}: {String.Join(",", ys.parameters)}";
                case YackStatement.YackOpcode.ACTOR_SAY:
                    if(ys.ConditionalFlag > 0) return $"if {stGetString(ys.parameters[0])} then {stGetString(ys.parameters[1])}: {stGetString(ys.parameters[2])}";
                    return $"{stGetString(ys.parameters[0])}: {stGetString(ys.parameters[1])}";
                case YackStatement.YackOpcode.EMIT_CODE:
                    if (ys.ConditionalFlag > 0) return $"if {stGetString(ys.parameters[0])} then {stGetString(ys.parameters[1])}";
                    return $"{stGetString(ys.parameters[0])}";
                case YackStatement.YackOpcode.LABEL:
                    return $"\n:{stGetString(ys.parameters[0])}";
                case YackStatement.YackOpcode.GOTO:
                    if (ys.ConditionalFlag > 0) return $"if {stGetString(ys.parameters[0])} -> {stGetString(ys.parameters[1])}";
                    return $"-> {stGetString(ys.parameters[0])}";
                case YackStatement.YackOpcode.GOSUB:
                    if (ys.ConditionalFlag > 0) return $"if {stGetString(ys.parameters[0])} gosub {stGetString(ys.parameters[1])}";
                    return $"gosub {stGetString(ys.parameters[0])}";
                case YackStatement.YackOpcode.DIALOG_CHOICE:
                    if (ys.ConditionalFlag > 0) return $"if {stGetString(ys.parameters[0])} DialogChoice {stGetString(ys.parameters[1])} -> {stGetString(ys.parameters[2])}";
                    return $"DialogChoice {stGetString(ys.parameters[0])} -> {stGetString(ys.parameters[1])}";
            }
        }

        private string stGetString(int index)
        {
            if (StringTable.Count <= index || index < 0) return "";
            return StringTable[index];
        }

        public override string ToString()
        {
            return string.Join("\n", Statements.Select(s => YackStatementToString(s)));
        }
    }
}
