using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThimbleweedLibrary
{
    public class DinkDisassembler
    {
        public DinkDisassembler(Stream file)
        {
            var dink = new BinaryReader(file);
            dink.BaseStream.Position = 0;

            List<byte[]> functionSegments = new List<byte[]>();

            while (dink.BaseStream.Position < dink.BaseStream.Length)
            {
                uint functionMagic = dink.ReadUInt32();
                if (functionMagic != 0x3441789C) throw new InvalidDataException("Unknown magic number for function.");

                uint functionLength = dink.ReadUInt32();

                byte[] functionSegment = dink.ReadBytes((int)functionLength);
                functionSegments.Add(functionSegment);
            }

            foreach (var script in functionSegments)
            {
                Functions.Add(new ParsedFunction(script));
            }
        }

        /// <summary>
        /// Save the disassembled structure back into its binary representation.
        /// This should result in an identical File if no changes have been made to the functions.
        /// </summary>
        /// <returns>byte array of dinky data</returns>
        public byte[] SaveDink()
        {
            using (var ms = new MemoryStream())
            {
                var bw = new BinaryWriter(ms);

                foreach (var func in Functions)
                {
                    byte[] funcBlock = func.SaveFunctionBlock();
                    bw.Write((uint)0x3441789C);
                    bw.Write((uint)funcBlock.Length);
                    bw.Write(funcBlock);
                }

                return ms.ToArray();
            }
        }

        private readonly List<ParsedFunction> Functions = new List<ParsedFunction>();

        public override string ToString() => String.Join("\n\n", Functions);
        public string[] ScriptFiles() => Functions.Select(f => f.ScriptName).Distinct().ToArray();
        public ParsedFunction[] FunctionsInScript(string ScriptName) => Functions.Where(f => f.ScriptName == ScriptName).ToArray();

        public class ParsedFunction
        {
            public class DataBlock
            {
                public uint typeCode;
                public byte[] data;
            }

            public struct DinkyInstruction
            {
                public enum DinkyOpCode
                {
                    OP_NOP = 0x00,
                    OP_PUSH_CONST = 0x01,
                    OP_PUSH_NULL = 0x02,
                    OP_PUSH_LOCAL = 0x03,
                    OP_PUSH_UPVAR = 0x04,
                    OP_PUSH_GLOBAL = 0x05,
                    OP_PUSH_FUNCTION = 0x06,
                    OP_PUSH_VAR = 0x07,
                    OP_PUSH_GLOBALREF = 0x08,
                    OP_PUSH_LOCALREF = 0x09,
                    OP_PUSH_UPVARREF = 0x0A,
                    OP_PUSH_VARREF = 0x0B,
                    OP_PUSH_INDEXREF = 0x0C,
                    OP_DUP_TOP = 0x0D,
                    OP_UNOT = 0x0E,
                    OP_UMINUS = 0x0F,
                    OP_UONECOMP = 0x10,
                    OP_MATH = 0x11,
                    OP_LAND = 0x12,
                    OP_LOR = 0x13,
                    OP_INDEX = 0x14,
                    OP_ITERATE = 0x15,
                    OP_ITERATEKV = 0x16,
                    OP_CALL = 0x17,
                    OP_FCALL = 0x18,
                    OP_CALLINDEXED = 0x19,
                    OP_CALL_NATIVE = 0x1A,
                    OP_FCALL_NATIVE = 0x1B,
                    OP_POP = 0x1C,
                    OP_STORE_LOCAL = 0x1D,
                    OP_STORE_UPVAR = 0x1E,
                    OP_STORE_ROOT = 0x1F,
                    OP_STORE_VAR = 0x20,
                    OP_STORE_INDEXED = 0x21,
                    OP_SET_LOCAL = 0x22,
                    OP_NULL_LOCAL = 0x23,
                    OP_MATH_REF = 0x24,
                    OP_INC_REF = 0x25,
                    OP_DEC_REF = 0x26,
                    OP_ADD_LOCAL = 0x27,
                    OP_JUMP = 0x28,
                    OP_JUMP_TRUE = 0x29,
                    OP_JUMP_FALSE = 0x2A,
                    OP_JUMP_TOPTRUE = 0x2B,
                    OP_JUMP_TOPFALSE = 0x2C,
                    OP_TERNARY = 0x2D,
                    OP_NEW_TABLE = 0x2E,
                    OP_NEW_ARRAY = 0x2F,
                    OP_NEW_SLOT = 0x30,
                    OP_NEW_THIS_SLOT = 0x31,
                    OP_DELETE_SLOT = 0x32,
                    OP_RETURN = 0x33,
                    OP_CLONE = 0x34,
                    OP_BREAKPOINT = 0x35,
                    OP_REMOVED = 0x36,
                    __OP_LAST__ = 0x37,
                    _OP_LABEL_ = 0x38,
                    UNKNOWN,
                }

                public readonly uint instruction;
                public readonly uint OpcodeRaw;
                public DinkyOpCode Opcode;
                public uint PotentialParameter1;
                public byte PotentialParameter2;
                public uint PotentialParameter3;

                public DinkyInstruction(uint instruction)
                {
                    this.instruction = instruction;
                    OpcodeRaw = instruction & 0x3F;
                    PotentialParameter1 = instruction >> 7;
                    PotentialParameter2 = (byte)(instruction >> 0x0F);
                    PotentialParameter3 = instruction >> 0x17;
                    Opcode = DinkyOpCode.UNKNOWN;
                    if (Enum.IsDefined(typeof(DinkyOpCode), (int)OpcodeRaw)) Opcode = (DinkyOpCode)OpcodeRaw;
                }

                public DinkyInstruction(DinkyOpCode OpCode, uint Argument3 = 0, uint Argument2 = 0)
                {
                    this.Opcode = OpCode;
                    this.OpcodeRaw = (uint)OpCode;
                    this.PotentialParameter1 = 0;
                    this.PotentialParameter2 = (byte)Argument2;
                    this.PotentialParameter3 = Argument3;
                    this.instruction = OpcodeRaw | ((uint)Argument2 << 0x0F) | (Argument3 << 0x17);
                }

                public DinkyInstruction(DinkyOpCode OpCode, ushort longArgument)
                {
                    this.Opcode = OpCode;
                    this.OpcodeRaw = (uint)OpCode;
                    this.PotentialParameter1 = longArgument;
                    this.PotentialParameter2 = 0;
                    this.PotentialParameter3 = 0;
                    this.instruction = OpcodeRaw | ((uint)longArgument << 7);
                }

                public string ToString(ParsedFunction function)
                {
                    string opcodeName = Opcode.ToString().Replace("OP_", "");
                    switch (Opcode)
                    {
                        case DinkyOpCode.UNKNOWN:               // the game would terminate execution of the script if an unknown opcode was reached
                            return $"// Unknown opcode {OpcodeRaw:X2} - {instruction:X8}.";
                        case DinkyOpCode.OP_JUMP:               // jumps always
                        case DinkyOpCode.OP_JUMP_FALSE:         // jumps if stack.pop == false
                        case DinkyOpCode.OP_JUMP_TOPFALSE:      // jumps if stack.top == false
                        case DinkyOpCode.OP_JUMP_TOPTRUE:       // jumps if stack.top == true
                        case DinkyOpCode.OP_JUMP_TRUE:          // jumps if stack.pop == true
                            {
                                int jumpAmount = (int)((PotentialParameter1 & 0xFFFF) - 0x7FFF);
                                return $"{opcodeName} {jumpAmount}";
                            }
                        case DinkyOpCode.OP_REMOVED:
                            return $"{opcodeName} // This opcode is no longer used and treated as No-Op"; // Seems to have been in use at one point.
                        case DinkyOpCode.OP_NOP:                // No Operation
                        case DinkyOpCode.OP_RETURN:             // Returns from function
                        case DinkyOpCode.OP_PUSH_NULL:          // pushes null to the stack 
                            return opcodeName;
                        case DinkyOpCode.OP_PUSH_CONST:         // push a constant to the stack
                        case DinkyOpCode.OP_PUSH_LOCAL:         // push a local variable to the stack
                            return $"{opcodeName} {function.GetLocalAsString((int)PotentialParameter3, true)}";
                        case DinkyOpCode.OP_PUSH_GLOBAL:        // push the global variable with this name to the stack
                            return $"{opcodeName} ::{function.GetLocalAsString((int)PotentialParameter3, false)}";
                        case DinkyOpCode.OP_PUSH_FUNCTION:      // push the function with this GUID to the stack
                        case DinkyOpCode.OP_PUSH_VAR:           // push a script-local variable with this name to the stack (?)
                            return $"{opcodeName} {function.GetLocalAsString((int)PotentialParameter3, false)}";
                        case DinkyOpCode.OP_PUSH_GLOBALREF:
                            return $"{opcodeName} ::&{function.GetLocalAsString((int)PotentialParameter3, false)}";
                        case DinkyOpCode.OP_PUSH_LOCALREF:
                            return $"{opcodeName} &{function.GetLocalAsString((int)PotentialParameter3, false)}";
                        case DinkyOpCode.OP_PUSH_UPVAR:         // push a variable from an upper closure to the stack
                            {
                                if (PotentialParameter2 != 0)
                                {
                                    return $"{opcodeName} ( local {PotentialParameter3} in Closure {PotentialParameter2})";
                                }
                                return $"{opcodeName} {function.GetLocalAsString((int)PotentialParameter3, false)}";
                            }
                        case DinkyOpCode.OP_PUSH_UPVARREF:         // push a variable from an upper closure to the stack
                            {
                                if (PotentialParameter2 != 0)
                                {
                                    return $"{opcodeName} &( local {PotentialParameter3} in Closure {PotentialParameter2})";
                                }
                                return $"{opcodeName} &{function.GetLocalAsString((int)PotentialParameter3, false)}";
                            }
                        case DinkyOpCode.OP_PUSH_VARREF:           // push a script-local variable with this name to the stack (?)
                            return $"{opcodeName} &{function.GetLocalAsString((int)PotentialParameter3, false)}";
                        case DinkyOpCode.OP_PUSH_INDEXREF:
                            return opcodeName;
                        case DinkyOpCode.OP_DUP_TOP:                // stack.push(stack.top)
                            return opcodeName;
                        case DinkyOpCode.OP_UNOT:                   // stack.push(!stack.pop)
                            return opcodeName;
                        case DinkyOpCode.OP_UMINUS:                 // stack.push(-stack.pop)
                            return opcodeName;
                        case DinkyOpCode.OP_UONECOMP:               // stack.push(stack.pop ^ 0xFFFFFFFF)
                            return opcodeName;
                        case DinkyOpCode.OP_MATH:                   // various math operations on the top two elements on the stack
                            return $"{opcodeName} {PotentialParameter3:X2} //(Todo: find out which operation this is.)";
                        case DinkyOpCode.OP_LAND:
                        case DinkyOpCode.OP_LOR:
                            return $"{opcodeName} //(possibly unused - I couldn't find it in the game.)";
                        case DinkyOpCode.OP_INDEX:
                            if (((PotentialParameter1 >> 1) & 1) == 0)
                            {
                                return $"{opcodeName} (stack)";
                            }
                            else return $"{opcodeName} {function.GetLocalAsString((int)PotentialParameter3, true)}";
                        case DinkyOpCode.OP_ITERATE:
                            return $"{opcodeName} {function.GetLocalAsString((int)PotentialParameter3, true)}";
                        case DinkyOpCode.OP_ITERATEKV:
                            return $"{opcodeName} {function.GetLocalAsString((int)PotentialParameter3, true)}";
                        case DinkyOpCode.OP_CALL:
                        case DinkyOpCode.OP_FCALL:
                            return $"{opcodeName} {PotentialParameter3}";
                        case DinkyOpCode.OP_CALLINDEXED:
                            return $"{opcodeName} // (possibly unused)";
                        case DinkyOpCode.OP_CALL_NATIVE:
                        case DinkyOpCode.OP_FCALL_NATIVE:
                            return $"{opcodeName} {function.GetLocalAsString((int)(PotentialParameter1 & 0xFFFF), true)} ({PotentialParameter3} parameters)";
                        case DinkyOpCode.OP_POP:
                            return opcodeName;
                        case DinkyOpCode.OP_STORE_LOCAL:
                            return $"{opcodeName} slot {function.GetLocalAsString((int)PotentialParameter3, false)}";
                        case DinkyOpCode.OP_STORE_UPVAR:
                            return $"{opcodeName} slot {function.GetLocalAsString((int)PotentialParameter3, false)} in Closure {PotentialParameter2}";
                        case DinkyOpCode.OP_STORE_ROOT:
                            return $"{opcodeName} slot {function.GetLocalAsString((int)PotentialParameter3, false)}";
                        case DinkyOpCode.OP_STORE_VAR:
                            return $"{opcodeName} {function.GetLocalAsString((int)PotentialParameter3, false)}";
                        case DinkyOpCode.OP_SET_LOCAL:
                            return $"{opcodeName} {function.GetLocalAsString((int)PotentialParameter3, false)} <- {function.GetLocalAsString((int)PotentialParameter2 & 0xFF, false)}";
                        case DinkyOpCode.OP_NULL_LOCAL:
                            return $"{opcodeName} {function.GetLocalAsString((int)PotentialParameter3, false)} <- null";
                        case DinkyOpCode.OP_MATH_REF:
                        case DinkyOpCode.OP_INC_REF:
                        case DinkyOpCode.OP_DEC_REF:
                            return $"{opcodeName} {PotentialParameter3:X3}";
                        case DinkyOpCode.OP_ADD_LOCAL:
                            var one = function.GetLocalAsString((int)PotentialParameter3, true);
                            var two = function.GetLocalAsString(PotentialParameter2, true);
                            return $"{opcodeName} {one} {two}";
                        case DinkyOpCode.OP_TERNARY:
                            int onFalse = (int)PotentialParameter2 - 0x80;
                            int onTrue = (int)PotentialParameter3 - 0x80;
                            return $"{opcodeName} true -> {onTrue}, false -> {onFalse}";
                        case DinkyOpCode.OP_BREAKPOINT:
                            return opcodeName;
                        case DinkyOpCode.OP_NEW_SLOT:
                            return $"{opcodeName} {function.GetLocalAsString((int)PotentialParameter3, false)}";
                        default:
                            return $"{instruction:X8} -- {opcodeName} ({OpcodeRaw:X2}) {PotentialParameter1:X8} {PotentialParameter2:X2} {PotentialParameter3:X2}";
                    }
                }
            }

            /// <summary>
            /// Possibly used for debug info.
            /// </summary>
            public struct DinkyInstructionSegment
            {
                public uint LineNumber;
                public uint fromInstruction;
                public uint toInstruction;
            }

            public struct DinkyLocal
            {
                public uint DataType;
                public uint Value;
            }

            public readonly List<DataBlock> blocks = new List<DataBlock>();

            public readonly string ScriptName;
            public readonly string UID;
            public readonly string FunctionName;


            public DinkyInstructionSegment[] InstructionSegments;
            public DinkyLocal[] Locals;
            public DinkyInstruction[] Instructions;
            public DataBlock StringDataBlock;

            uint unk1, unk2, unk3;

            public uint unknownInfo1;
            public uint NumberOfConstants;
            byte[] InfoBlockExtraBytes;

            public byte[] SaveFunctionBlock()
            {
                using (var ms = new MemoryStream())
                {
                    BinaryWriter bw = new BinaryWriter(ms);

                    void WriteString(string s)
                    {
                        byte[] data = Encoding.UTF8.GetBytes(s);
                        bw.Write(data);
                        bw.Write((byte)0);
                    }

                    uint requiredLength(string s) => (uint)Encoding.UTF8.GetByteCount(s) + (uint)1;

                    // Write unknown block
                    bw.Write((uint)0x7F46a125);
                    bw.Write(unk1);
                    bw.Write(unk2);
                    bw.Write((ushort)unk3);

                    // Write info block
                    bw.Write((uint)0x16F94B62);
                    bw.Write((uint)(requiredLength(UID) + requiredLength(FunctionName) + requiredLength(ScriptName) + 4 + 4 + (uint)InfoBlockExtraBytes.Length));
                    WriteString(UID);
                    WriteString(FunctionName);
                    WriteString(ScriptName);
                    bw.Write(unknownInfo1);
                    bw.Write(NumberOfConstants);
                    bw.Write(InfoBlockExtraBytes);

                    // Write string data
                    bw.Write((uint)0x983f1cfa);
                    bw.Write((uint)StringDataBlock.data.Length);
                    bw.Write(StringDataBlock.data);

                    // Write locals
                    bw.Write((uint)0xFD4BC33A);
                    bw.Write((uint)Locals.Length * 4 * 2);
                    foreach (var l in Locals)
                    {
                        bw.Write((uint)l.DataType);
                        bw.Write((uint)l.Value);
                    }

                    // Write instructions
                    bw.Write((uint)0x55ED4D1D);
                    bw.Write((uint)Instructions.Length * 4);
                    foreach (var i in Instructions)
                    {
                        bw.Write((uint)i.instruction);
                    }

                    // Write instruction segments
                    bw.Write((uint)0x62D34042);
                    bw.Write((uint)InstructionSegments.Length * 4 * 3);
                    foreach (var i in InstructionSegments)
                    {
                        bw.Write(i.LineNumber);
                        bw.Write(i.fromInstruction);
                        bw.Write(i.toInstruction);
                    }

                    // write other (unknown) blocks
                    uint[] knownBlocks = new uint[] { 0x7F46a125, 0x16F94B62, 0x62D34042, 0xFD4BC33A, 0x55ED4D1D, 0x983f1cfa };
                    foreach (var block in blocks.Where(b => !knownBlocks.Contains(b.typeCode)))
                    {
                        bw.Write((uint)block.typeCode);
                        bw.Write((uint)block.data.Length);
                        bw.Write(block.data);
                    }

                    return ms.ToArray();
                }
            }

            public ParsedFunction(byte[] functionBlock)
            {
                // Extract the compiled function's subblocks
                using (var ms = new MemoryStream(functionBlock))
                {
                    BinaryReader br = new BinaryReader(ms);

                    while (br.BaseStream.Position < br.BaseStream.Length)
                    {
                        uint code = br.ReadUInt32();
                        switch (code)
                        {
                            case 0x7F46a125:
                                {
                                    unk1 = br.ReadUInt32();
                                    unk2 = br.ReadUInt32();
                                    unk3 = br.ReadUInt16();
                                }
                                break;
                            default:
                                {
                                    uint length = br.ReadUInt32();
                                    byte[] data = br.ReadBytes((int)length);
                                    blocks.Add(new DataBlock() { data = data, typeCode = code });
                                }
                                break;
                        }
                    }

                    // General information is stored in Block 0x16F94B62
                    var infoBlock = blocks.Where(b => b.typeCode == 0x16F94B62).FirstOrDefault();
                    if (infoBlock == null) throw new Exception("Dinky function without info block.");

                    {
                        using (var infoBlockStream = new MemoryStream(infoBlock.data))
                        {
                            var functionInfo = new BinaryReader(infoBlockStream);
                            UID = functionInfo.ReadNullTerminatedString();
                            FunctionName = functionInfo.ReadNullTerminatedString();
                            ScriptName = functionInfo.ReadNullTerminatedString();
                            unknownInfo1 = functionInfo.ReadUInt32();
                            NumberOfConstants = functionInfo.ReadUInt32();
                            List<byte> extraBytes = new List<byte>();
                            while (functionInfo.BaseStream.Position < functionInfo.BaseStream.Length)
                            {
                                extraBytes.Add(functionInfo.ReadByte());
                            }
                            InfoBlockExtraBytes = extraBytes.ToArray();
                        }
                    }
                }

                // Parse instruction Segments:
                {
                    var instructionSegmentsBlock = blocks.Where(b => b.typeCode == 0x62D34042).FirstOrDefault();
                    if (instructionSegmentsBlock == null) throw new Exception("Missing instruction segment block?");

                    int numInstructionSegments = instructionSegmentsBlock.data.Length / (4 * 3);
                    this.InstructionSegments = new DinkyInstructionSegment[numInstructionSegments];

                    var blockreader = new BinaryReader(new MemoryStream(instructionSegmentsBlock.data));
                    for (int i = 0; i < numInstructionSegments; ++i)
                    {

                        uint LineNumber = blockreader.ReadUInt32();
                        uint from = blockreader.ReadUInt32();
                        uint to = blockreader.ReadUInt32();
                        InstructionSegments[i] = new DinkyInstructionSegment() { LineNumber = LineNumber, fromInstruction = from, toInstruction = to };
                    }
                }

                // Parse Locals
                {
                    var localsBlock = blocks.Where(b => b.typeCode == 0xFD4BC33A).FirstOrDefault();
                    StringDataBlock = blocks.Where(b => b.typeCode == 0x983f1cfa).FirstOrDefault();
                    if (localsBlock == null) throw new Exception("Missing variables block?");
                    if (localsBlock == null) throw new Exception("Missing string data block?");

                    var locals = new BinaryReader(new MemoryStream(localsBlock.data));


                    int numLocals = localsBlock.data.Length / (4 * 2);
                    Locals = new DinkyLocal[numLocals];

                    for (int i = 0; i < numLocals; ++i)
                    {
                        uint var_type = locals.ReadUInt32();
                        uint var_value = locals.ReadUInt32();
                        Locals[i] = new DinkyLocal() { DataType = var_type, Value = var_value };
                    }
                }

                // Parse Instructions
                {
                    var instructionBlock = blocks.Where(b => b.typeCode == 0x55ED4D1D).FirstOrDefault();
                    if (instructionBlock == null) throw new Exception("Missing instruction block?");

                    int numInstructions = instructionBlock.data.Length / (4 * 1);
                    Instructions = new DinkyInstruction[numInstructions];

                    var blockreader = new BinaryReader(new MemoryStream(instructionBlock.data));
                    for (int i = 0; i < numInstructions; ++i)
                    {
                        uint instruction = blockreader.ReadUInt32();
                        Instructions[i] = (new DinkyInstruction(instruction));
                    }
                }
            }

            public string LocalToString(DinkyLocal local, bool quotedString)
            {
                switch (local.DataType)
                {
                    case 0x102:
                        // int
                        return ((int)local.Value).ToString();
                    case 0x103:
                        // float
                        return $"{BitConverter.ToSingle(BitConverter.GetBytes(local.Value), 0).ToString()}f";
                    case 0x204:
                        using (var ms = new MemoryStream(StringDataBlock.data))
                        {
                            var br = new BinaryReader(ms);
                            ms.Position = local.Value;
                            if (quotedString) return $"\"{br.ReadNullTerminatedString()}\"";
                            else return br.ReadNullTerminatedString();
                        }
                    default:
                        return $"(Unknown type {local.DataType:X3}: {local.Value:X8})";
                }
            }

            public string GetLocalAsString(int iLocal, bool quotedString)
            {
                if (iLocal < 0 || iLocal >= Locals.Length) return $"Invalid local: {iLocal}";
                return $"{LocalToString(Locals[iLocal], quotedString)} [{iLocal}]";
            }

            public override string ToString()
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine($"{UID}: {ScriptName}::{FunctionName}()");
                sb.AppendLine($"[{Locals.Length} locals]");

                int iInstruction = 0;
                foreach (var instruction in Instructions)
                {
                    string marker = "";
                    if (InstructionSegments.Any(i => i.fromInstruction == iInstruction))
                    {
                        marker = InstructionSegments.Where(i => i.fromInstruction == iInstruction).FirstOrDefault().LineNumber.ToString("D4");
                    }
                    else marker = "    ";

                    sb.AppendLine($"{iInstruction:D4}: {marker} {instruction.ToString(this)}");

                    ++iInstruction;
                }

                return sb.ToString();
            }

            #region Patching
            // Functions related to patching the script data.

            private Dictionary<uint, string> stringsFromStringBlock()
            {
                Dictionary<uint, string> Strings = new Dictionary<uint, string>();
                using (var ms = new MemoryStream(StringDataBlock.data))
                {
                    var reader = new BinaryReader(ms);
                    while (ms.Position < ms.Length)
                    {
                        uint offset = (uint)ms.Position;
                        Strings[offset] = reader.ReadNullTerminatedString();
                    }
                }
                return Strings;
            }

            private void stringsToStringBlock(Dictionary<uint, string> Strings)
            {
                // write strings and adjust offsets
                using (var ms = new MemoryStream())
                {
                    BinaryWriter bw = new BinaryWriter(ms);

                    Dictionary<uint, List<int>> LocalsUsingThisString = new Dictionary<uint, List<int>>();
                    for (int i = 0; i < Locals.Length; ++i)
                    {
                        if (Locals[i].DataType == 0x204)
                        {
                            if (!LocalsUsingThisString.ContainsKey(Locals[i].Value)) LocalsUsingThisString[Locals[i].Value] = new List<int>();
                            LocalsUsingThisString[Locals[i].Value].Add(i);
                        }
                    }

                    foreach (var kvp in Strings)
                    {
                        uint newOffset = (uint)ms.Position;
                        uint oldOffset = kvp.Key;
                        bw.Write(Encoding.UTF8.GetBytes(kvp.Value));
                        bw.Write((byte)0);

                        if (LocalsUsingThisString.ContainsKey(oldOffset))
                        {
                            foreach (var i in LocalsUsingThisString[oldOffset])
                            {
                                Locals[i].Value = newOffset;
                            }
                        }
                    }

                    StringDataBlock.data = ms.ToArray();
                }
            }

            private void CheckLocalValue(int iLocal, object value)
            {
                if (value == null) return;
                if (value is int i)
                {
                    if (Locals[iLocal].DataType != 0x102) throw new ArgumentException($"Local {iLocal} mismatch: not an integer");
                    if (Locals[iLocal].Value != (uint)i) throw new ArgumentException($"Local {iLocal} mismatch: actual value {(int)Locals[iLocal].Value} does not match new value {i}");
                }
                else if (value is float f)
                {
                    if (Locals[iLocal].DataType != 0x103) throw new ArgumentException($"Local {iLocal} mismatch: not a float");
                    float actualValue = BitConverter.ToSingle(BitConverter.GetBytes(Locals[iLocal].Value), 0);
                    if (actualValue != f) throw new ArgumentException($"Local {iLocal} mismatch: actual value {actualValue} does not match new value {f}");
                }
                else if (value is string s)
                {
                    if (Locals[iLocal].DataType != 0x204) throw new ArgumentException($"Local {iLocal} mismatch: not a string");
                    string actualValue;
                    using (var ms = new MemoryStream(StringDataBlock.data))
                    {
                        var br = new BinaryReader(ms);
                        ms.Position = Locals[iLocal].Value;
                        actualValue = br.ReadNullTerminatedString();
                    }
                    if (actualValue != s) throw new ArgumentException($"Local {iLocal} mismatch: actual value {actualValue} does not match new value {s}");
                }
            }

            public void Patch_SetLocal(int iLocal, string newValue, object sanityCheck_oldValue = null)
            {
                if (iLocal < 0 || iLocal >= Locals.Length) throw new ArgumentOutOfRangeException($"invalid local index {iLocal}");
                CheckLocalValue(iLocal, sanityCheck_oldValue);

                // Read strings and offsets
                Dictionary<uint, string> Strings = stringsFromStringBlock();

                // adjust local
                if (Locals[iLocal].DataType != 0x204)
                {
                    Locals[iLocal].DataType = 0x204;
                    Strings[0xFFFFFFFF] = newValue;
                    Locals[iLocal].Value = 0xFFFFFFFF;
                }
                else
                {
                    Strings[Locals[iLocal].Value] = newValue;
                }

                stringsToStringBlock(Strings);
            }

            public void Patch_SetLocal(int iLocal, int newValue, object sanityCheck_oldValue = null)
            {
                if (iLocal < 0 || iLocal >= Locals.Length) throw new ArgumentOutOfRangeException($"invalid local index {iLocal}");
                CheckLocalValue(iLocal, sanityCheck_oldValue);
                Locals[iLocal].DataType = 0x102;
                Locals[iLocal].Value = (uint)newValue;
            }

            public void Patch_SetLocal(int iLocal, float newValue, object sanityCheck_oldValue = null)
            {
                if (iLocal < 0 || iLocal >= Locals.Length) throw new ArgumentOutOfRangeException($"invalid local index {iLocal}");
                CheckLocalValue(iLocal, sanityCheck_oldValue);
                Locals[iLocal].DataType = 0x103;
                Locals[iLocal].Value = BitConverter.ToUInt32(BitConverter.GetBytes(newValue), 0);
            }

            private int Patch_AddLocalRaw(int sanityCheck_newSlotIndex, uint typeCode, uint value)
            {
                if (sanityCheck_newSlotIndex >= 0 && Locals.Length != sanityCheck_newSlotIndex) throw new ArgumentException("New local would be allocated to unexpected slot.");
                var newLocals = new DinkyLocal[Locals.Length + 1];
                for (int i = 0; i < Locals.Length; ++i) newLocals[i] = Locals[i];
                newLocals[sanityCheck_newSlotIndex].Value = value;
                newLocals[sanityCheck_newSlotIndex].DataType = typeCode;

                Locals = newLocals;
                NumberOfConstants = (uint)newLocals.Length;
                return Locals.Length - 1;
            }

            public int Patch_AddIntLocal(int sanityCheck_newSlotIndex, int value) => Patch_AddLocalRaw(sanityCheck_newSlotIndex, 0x102, (uint)value);
            public int Patch_AddFloatLocal(int sanityCheck_newSlotIndex, float value) => Patch_AddLocalRaw(sanityCheck_newSlotIndex, 0x103, BitConverter.ToUInt32(BitConverter.GetBytes(value), 0));
            public int Patch_AddStringLocal(int sanityCheck_newSlotIndex, string value)
            {
                var result = Patch_AddLocalRaw(sanityCheck_newSlotIndex, 0x204, 0xFFFFFFFF);
                var strings = stringsFromStringBlock();
                strings[0xFFFFFFFF] = value;
                stringsToStringBlock(strings);
                return result;
            }

            public void Patch_ReplaceInstruction(int iInstruction, DinkyInstruction newInstruction, DinkyInstruction? sanityCheck_oldInstruction = null)
            {
                if (iInstruction < 0 || iInstruction >= Instructions.Length) throw new ArgumentException($"Invalid instruction index {iInstruction}");
                if (sanityCheck_oldInstruction != null)
                    if (Instructions[iInstruction].instruction != sanityCheck_oldInstruction.Value.instruction)
                        throw new ArgumentException($"Instruction mismatch: actual instruction {Instructions[iInstruction]} does not match Instruction {sanityCheck_oldInstruction.Value}");

                Instructions[iInstruction] = newInstruction;
            }

            public void Patch_InsertInstruction(int iInstruction, IEnumerable<DinkyInstruction> newInstructions)
            {
                if (iInstruction < 0 || iInstruction > Instructions.Length) throw new ArgumentException($"Invalid instruction index {iInstruction}");
                List<DinkyInstruction> instructions = Instructions.ToList();
                foreach (var newInstruction in newInstructions)
                {
                    instructions.Insert(iInstruction, newInstruction);
                    iInstruction++;
                }
                Instructions = instructions.ToArray();
            }

            public static DinkyInstruction Patch_ParseInstruction(string instString)
            {
                string[] commentMarker = new string[] { ";", "#", "//" };
                foreach (var marker in commentMarker)
                {
                    if (instString.Contains(marker)) instString = instString.Substring(0, instString.IndexOf(marker));
                }

                string[] parts = instString.Split().Where(a => a.Length > 0).ToArray();
                if (parts.Length == 0) throw new ArgumentException($"Could not parse instruction: empty.");
                if (uint.TryParse(parts[0], out uint instructionCodeRaw)) return new DinkyInstruction(instructionCodeRaw);

                string opcodeString = "OP_" + parts[0];
                if (!Enum.TryParse(opcodeString, true, out DinkyInstruction.DinkyOpCode opCode)) throw new ArgumentException($"Could not parse instruction: invalid opcode \"{opcodeString}\"");

                switch (opCode)
                {
                    default:
                    case DinkyInstruction.DinkyOpCode.UNKNOWN:
                        throw new ArgumentException($"Could not parse instruction: unsupported OpCode {opCode}");
                    case DinkyInstruction.DinkyOpCode.OP_NOP:
                    case DinkyInstruction.DinkyOpCode.OP_REMOVED:
                    case DinkyInstruction.DinkyOpCode.OP_RETURN:
                    case DinkyInstruction.DinkyOpCode.OP_PUSH_NULL:
                    case DinkyInstruction.DinkyOpCode.OP_BREAKPOINT:
                    case DinkyInstruction.DinkyOpCode.OP_POP:
                    case DinkyInstruction.DinkyOpCode.OP_DUP_TOP:
                    case DinkyInstruction.DinkyOpCode.OP_UNOT:
                    case DinkyInstruction.DinkyOpCode.OP_UMINUS:
                    case DinkyInstruction.DinkyOpCode.OP_UONECOMP:
                        return new DinkyInstruction(opCode);
                    case DinkyInstruction.DinkyOpCode.OP_JUMP:
                    case DinkyInstruction.DinkyOpCode.OP_JUMP_FALSE:
                    case DinkyInstruction.DinkyOpCode.OP_JUMP_TOPFALSE:
                    case DinkyInstruction.DinkyOpCode.OP_JUMP_TOPTRUE:
                    case DinkyInstruction.DinkyOpCode.OP_JUMP_TRUE:
                        {
                            if (parts.Length < 2 || !int.TryParse(parts[1], out int jmpAmount)) throw new ArgumentException($"Could not parse instruction: invalid jump target.");
                            ushort toJump = (ushort)(jmpAmount + 0x7FFF);
                            return new DinkyInstruction(opCode, toJump);
                        }
                    case DinkyInstruction.DinkyOpCode.OP_PUSH_CONST:
                    case DinkyInstruction.DinkyOpCode.OP_PUSH_LOCAL:
                    case DinkyInstruction.DinkyOpCode.OP_PUSH_GLOBAL:
                    case DinkyInstruction.DinkyOpCode.OP_PUSH_FUNCTION:
                    case DinkyInstruction.DinkyOpCode.OP_PUSH_VAR:
                    case DinkyInstruction.DinkyOpCode.OP_PUSH_GLOBALREF:
                    case DinkyInstruction.DinkyOpCode.OP_PUSH_LOCALREF:
                    case DinkyInstruction.DinkyOpCode.OP_PUSH_VARREF:
                    case DinkyInstruction.DinkyOpCode.OP_NULL_LOCAL:
                        {
                            if (parts.Length < 2 || !int.TryParse(parts[1], out int iLocal)) throw new ArgumentException($"Could not parse instruction: invalid local index.");
                            return new DinkyInstruction(opCode, (uint)iLocal, 0);
                        }
                    case DinkyInstruction.DinkyOpCode.OP_CALL:
                    case DinkyInstruction.DinkyOpCode.OP_FCALL:
                        {
                            if (parts.Length < 2 || !int.TryParse(parts[1], out int numArgs)) throw new ArgumentException($"Could not parse instruction: invalid number of arguments.");
                            return new DinkyInstruction(opCode, (uint)numArgs, 0);
                        }
                    case DinkyInstruction.DinkyOpCode.OP_MATH:
                        {
                            if (parts.Length < 2 || !int.TryParse(parts[1], System.Globalization.NumberStyles.HexNumber, System.Globalization.CultureInfo.InvariantCulture, out int mathOp)) throw new ArgumentException($"Could not parse instruction: invalid math operation.");
                            return new DinkyInstruction(opCode, (uint)mathOp, 0);
                        }
                }
            }



            #endregion
        }

    }
}
