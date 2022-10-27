using System;
using System.Collections.Generic;
using System.Linq;

namespace ThimbleweedLibrary
{
    /// <summary>
    /// Custom structure to patch instructions in the compiled dink file.
    /// </summary>
    public class DinkyPatchFile
    {
        public string title;
        public string description;
        public string author;

        public List<DinkyPatch> function_patches = new List<DinkyPatch>();
        public List<Comment> function_comments = new List<Comment>();

        public void ApplyPatch(DinkDisassembler dinkFile)
        {
            foreach (var funcToPatch in function_patches)
            {
                DinkDisassembler.ParsedFunction function = null;
                if (String.IsNullOrWhiteSpace(funcToPatch.function_id)) function = dinkFile.FunctionsInScript(funcToPatch.script ?? "").Where(f => f.FunctionName == funcToPatch.function).FirstOrDefault();
                else function = dinkFile.FunctionByUid(funcToPatch.function_id);
                
                if (function == null) throw new ArgumentException($"Could not find function to patch: {funcToPatch.script}::{funcToPatch.function}().");
                try
                {
                    funcToPatch.ApplyTo(function);
                }
                catch (Exception ex)
                {
                    throw new Exception($"{funcToPatch.script}::{funcToPatch.function}(): {ex.Message}", ex);
                }
            }
        }

        public class Comment
        {
            public string comment_type;
            public string comment;
            public string function_id;
            public int instruction_index = -1;
        }

        public class DinkyPatch
        {
            public string script;
            public string function;
            public string function_id;
            public string comment;

            public List<DinkyPatchItem> patches = new List<DinkyPatchItem>();
            public class DinkyPatchItem
            {
                public string type;
                public int? index = null;
                public object value = null;
                public string valuetype = null;
                public object old_value = null;
                public string oldvaluetype = null;
            }

            public void ApplyTo(DinkDisassembler.ParsedFunction function)
            {
                if (function.FunctionName.ToLowerInvariant() != this.function?.ToLowerInvariant()) throw new ArgumentException($"Tried to apply patch to wrong function");
                if (function.ScriptName.ToLowerInvariant() != script?.ToLowerInvariant()) throw new ArgumentException($"Tried to apply patch to wrong script");

                foreach (var item in patches)
                {
                    switch (item.type?.ToLowerInvariant())
                    {
                        default:
                            throw new ArgumentException($"Unknown patch type \"{item.type}\"");
                        case "add_local":
                            {
                                if (item.value == null)
                                {
                                    throw new ArgumentException($"add_local without value");
                                }
                                else if (item.value is string stringValue)
                                {
                                    function.Patch_AddStringLocal(item.index ?? -1, stringValue);
                                }
                                else
                                {

                                    double number = 0;
                                    if (item.value is float f) number = f;
                                    else if (item.value is double d) number = d;
                                    else if (item.value is int i) number = i;
                                    else if (item.value is long l) number = l;
                                    else throw new ArgumentException($"Unknown value: {item.value}");

                                    if (item.valuetype is string valuetype && valuetype.ToLowerInvariant() == "int")
                                    {
                                        function.Patch_AddIntLocal(item.index ?? -1, (int)number);
                                    }
                                    else
                                    {
                                        function.Patch_AddFloatLocal(item.index ?? -1, (float)number);
                                    }
                                }
                            }
                            break;
                        case "set_local":
                            {
                                object oldvalue = null;
                                if (item.old_value != null)
                                {
                                    if (item.old_value is string oldstring) oldvalue = oldstring;
                                    else
                                    {
                                        if (item.old_value is float ovf) oldvalue = (float)ovf;
                                        else if (item.old_value is double ovd) oldvalue = (float)ovd;
                                        else if (item.old_value is int ovi) oldvalue = (float)ovi;
                                        else if (item.old_value is long ovl) oldvalue = (float)ovl;
                                        if (item.oldvaluetype?.ToLowerInvariant() == "int")
                                        {
                                            oldvalue = (int)(float)oldvalue;
                                        }
                                    }
                                }

                                if (item.value == null)
                                {
                                    throw new ArgumentException($"set_local without value");
                                }
                                else if (item.value is string stringValue)
                                {
                                    function.Patch_SetLocal(item.index ?? -1, stringValue, oldvalue);
                                }
                                else
                                {

                                    double number = 0;
                                    if (item.value is float f) number = f;
                                    else if (item.value is double d) number = d;
                                    else if (item.value is int i) number = i;
                                    else throw new ArgumentException($"Unknown value: {item.value}");

                                    if (item.valuetype is string valuetype && valuetype.ToLowerInvariant() == "int")
                                    {
                                        function.Patch_SetLocal(item.index ?? -1, (int)number, oldvalue);
                                    }
                                    else
                                    {
                                        function.Patch_SetLocal(item.index ?? -1, (float)number, oldvalue);
                                    }
                                }
                            }
                            break;
                        case "replace_instruction":
                            {
                                var instruction = DinkDisassembler.ParsedFunction.Patch_ParseInstruction(item.value as string ?? "");
                                DinkDisassembler.ParsedFunction.DinkyInstruction? oldInstruction = null;
                                if (item.old_value != null && item.old_value is string s) oldInstruction = DinkDisassembler.ParsedFunction.Patch_ParseInstruction(s);

                                function.Patch_ReplaceInstruction(item.index ?? -1, instruction, oldInstruction);
                            }
                            break;
                        case "insert_instruction":
                        case "insert_instructions":
                            {
                                string[] lines = (item.value as string ?? "").Split('\n').Select(s => s.Trim('\r').Trim()).Where(s => s.Length > 0).ToArray();
                                var instructions = lines.Select(s => DinkDisassembler.ParsedFunction.Patch_ParseInstruction(s)).ToArray();
                                function.Patch_InsertInstruction(item.index ?? -1, instructions);
                            }
                            break;
                    }
                }
            }
        }
    }
}
