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

                public string ToString(ParsedFunction function)
                {
                    string opcodeName = Opcode.ToString().Replace("OP_", "");
                    switch (Opcode)
                    {
                        case DinkyOpCode.UNKNOWN:               // the game would terminate execution of the script if an unknown opcode was reached
                            return $"Unknown opcode {OpcodeRaw:X2} - {instruction:X8}.";
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
                            return $"{opcodeName}: This opcode is no longer used and treated as No-Op"; // Seems to have been in use at one point.
                        case DinkyOpCode.OP_NOP:                // No Operation
                        case DinkyOpCode.OP_RETURN:             // Returns from function
                        case DinkyOpCode.OP_PUSH_NULL:          // pushes null to the stack 
                            return opcodeName;
                        case DinkyOpCode.OP_PUSH_CONST:         // push a constant to the stack
                        case DinkyOpCode.OP_PUSH_LOCAL:         // push a local variable to the stack
                            return $"{opcodeName} {function.GetLocalAsString((int)PotentialParameter3)}";
                        case DinkyOpCode.OP_PUSH_GLOBAL:        // push the global variable with this name to the stack
                            return $"{opcodeName} ::{function.GetLocalAsString((int)PotentialParameter3).Trim('"')}";
                        case DinkyOpCode.OP_PUSH_FUNCTION:      // push the function with this GUID to the stack
                        case DinkyOpCode.OP_PUSH_VAR:           // push a script-local variable with this name to the stack (?)
                            return $"{opcodeName} {function.GetLocalAsString((int)PotentialParameter3).Trim('"')}";
                        case DinkyOpCode.OP_PUSH_GLOBALREF:
                            return $"{opcodeName} ::&{function.GetLocalAsString((int)PotentialParameter3).Trim('"')}";
                        case DinkyOpCode.OP_PUSH_LOCALREF:
                            return $"{opcodeName} &{function.GetLocalAsString((int)PotentialParameter3).Trim('"')}";

                        case DinkyOpCode.OP_PUSH_UPVAR:         // push a variable from an upper closure to the stack
                            {
                                if (PotentialParameter2 != 0)
                                {
                                    return $"{opcodeName} ( local {PotentialParameter3} in Closure {PotentialParameter2})";
                                }
                                return $"{opcodeName} {function.GetLocalAsString((int)PotentialParameter3).Trim('"')}";
                            }
                        case DinkyOpCode.OP_PUSH_UPVARREF:         // push a variable from an upper closure to the stack
                            {
                                if (PotentialParameter2 != 0)
                                {
                                    return $"{opcodeName} &( local {PotentialParameter3} in Closure {PotentialParameter2})";
                                }
                                return $"{opcodeName} &{function.GetLocalAsString((int)PotentialParameter3).Trim('"')}";
                            }
                        case DinkyOpCode.OP_PUSH_VARREF:           // push a script-local variable with this name to the stack (?)
                            return $"{opcodeName} &{function.GetLocalAsString((int)PotentialParameter3).Trim('"')}";
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
                            return $"{opcodeName} {PotentialParameter3:X2} (Todo: find out which operation this is.)";
                        case DinkyOpCode.OP_LAND:
                        case DinkyOpCode.OP_LOR:
                            return $"{opcodeName} (possibly unused - I couldn't find it in the game.)";
                        case DinkyOpCode.OP_INDEX:
                            if (((PotentialParameter1 >> 1) & 1) == 0)
                            {
                                return $"{opcodeName} (stack)";
                            }
                            else return $"{opcodeName} {function.GetLocalAsString((int)PotentialParameter3)}";
                        case DinkyOpCode.OP_ITERATE:
                            return $"{opcodeName} {function.GetLocalAsString((int)PotentialParameter3)}";
                        case DinkyOpCode.OP_ITERATEKV:
                            return $"{opcodeName} {function.GetLocalAsString((int)PotentialParameter3)}";
                        case DinkyOpCode.OP_CALL:
                        case DinkyOpCode.OP_FCALL:
                            return $"{opcodeName}";
                        case DinkyOpCode.OP_CALLINDEXED:
                            return $"{opcodeName} (possibly unused)";
                        case DinkyOpCode.OP_CALL_NATIVE:
                        case DinkyOpCode.OP_FCALL_NATIVE:
                            return $"{opcodeName} {function.GetLocalAsString((int)(PotentialParameter1 & 0xFFFF))}({PotentialParameter3} parameters)";
                        case DinkyOpCode.OP_POP:
                            return opcodeName;
                        case DinkyOpCode.OP_STORE_LOCAL:
                            return $"{opcodeName} slot {PotentialParameter3}";
                        case DinkyOpCode.OP_STORE_UPVAR:
                            return $"{opcodeName} slot {PotentialParameter3} in Closure {PotentialParameter2}";
                        case DinkyOpCode.OP_STORE_ROOT:
                            return $"{opcodeName} slot {PotentialParameter3}";
                        case DinkyOpCode.OP_STORE_VAR:
                            return $"{opcodeName} {function.GetLocalAsString((int)PotentialParameter3).Trim('"')}";
                        case DinkyOpCode.OP_SET_LOCAL:
                            return $"{opcodeName} {function.GetLocalAsString((int)PotentialParameter3).Trim('"')} <- {function.GetLocalAsString((int)PotentialParameter2 & 0xFF).Trim('"')}";
                        case DinkyOpCode.OP_NULL_LOCAL:
                            return $"{opcodeName} {function.GetLocalAsString((int)PotentialParameter3).Trim('"')} <- null";
                        case DinkyOpCode.OP_MATH_REF:
                        case DinkyOpCode.OP_INC_REF:
                        case DinkyOpCode.OP_DEC_REF:
                            return $"{opcodeName} {PotentialParameter3:X3}";
                        case DinkyOpCode.OP_ADD_LOCAL:
                            var one = function.GetLocalAsString((int)PotentialParameter3);
                            var two = function.GetLocalAsString(PotentialParameter2);
                            return $"{opcodeName} {one} {two}";
                        case DinkyOpCode.OP_TERNARY:
                            int onFalse = (int)PotentialParameter2 - 0x80;
                            int onTrue = (int)PotentialParameter3 - 0x80;
                            return $"{opcodeName} true -> {onTrue}, false -> {onFalse}";
                        case DinkyOpCode.OP_BREAKPOINT:
                            return opcodeName;
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

            public readonly uint unknownInfo1;
            public readonly uint unknownInfo2;

            public readonly DinkyInstructionSegment[] InstructionSegments;
            public readonly DinkyLocal[] Locals;
            public readonly DinkyInstruction[] Instructions;
            public readonly DataBlock StringDataBlock;

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
                                    uint unk1 = br.ReadUInt32();
                                    uint unk2 = br.ReadUInt32();
                                    uint unk3 = br.ReadUInt16();
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
                            unknownInfo2 = functionInfo.ReadUInt32();
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

            public string LocalToString(DinkyLocal local)
            {
                switch (local.DataType)
                {
                    case 0x102:
                        // int
                        return local.Value.ToString();
                    case 0x103:
                        // float
                        return BitConverter.ToSingle(BitConverter.GetBytes(local.Value), 0).ToString();
                    case 0x204:
                        using (var ms = new MemoryStream(StringDataBlock.data))
                        {
                            var br = new BinaryReader(ms);
                            ms.Position = local.Value;
                            return $"\"{br.ReadNullTerminatedString()}\"";
                        }
                    default:
                        return $"(Unknown type {local.DataType:X3}: {local.Value:X8})";
                }
            }

            public string GetLocalAsString(int iLocal)
            {
                if (iLocal < 0 || iLocal >= Locals.Length) return $"Invalid local: {iLocal}";
                return LocalToString(Locals[iLocal]);
            }

            public override string ToString()
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine($"{UID}: {ScriptName}::{FunctionName}()");

                foreach (var segment in InstructionSegments)
                {
                    sb.AppendLine($"\t{segment.LineNumber:X4}:");
                    uint i1 = segment.fromInstruction;
                    do
                    {
                        sb.AppendLine($"\t\t{Instructions[(int)i1].ToString(this)}");
                        ++i1;
                    } while (i1 < segment.toInstruction);
                }

                // The last "return" opcode is not always part of the last segment.
                if (InstructionSegments.Length == 0 || InstructionSegments.Last().toInstruction == Instructions.Length - 1 && InstructionSegments.Last().fromInstruction < InstructionSegments.Last().toInstruction)
                {
                    sb.AppendLine("\t");
                    for (int i3 = (int)(InstructionSegments.LastOrDefault().toInstruction); i3 < Instructions.Length; ++i3) sb.AppendLine($"\t\t{Instructions[i3].ToString(this)}");
                }

                return sb.ToString();
            }
        }

    }
}
