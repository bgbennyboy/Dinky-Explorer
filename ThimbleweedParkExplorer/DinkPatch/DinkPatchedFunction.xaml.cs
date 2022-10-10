using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ThimbleweedLibrary;

namespace ThimbleweedParkExplorer.DinkPatch
{
    public class BoolToVisibility : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) => ((value as bool?) == true) ? Visibility.Visible : Visibility.Collapsed;
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => null;
    }

    /// <summary>
    /// Interaction logic for DinkPatchedFunction.xaml
    /// </summary>
    public partial class DinkPatchedFunction : UserControl
    {
        public DinkyPatchFile.DinkyPatch function { get; private set; }
        DinkyPatchFile file;
        DinkDisassembler dink;
        DinkDisassembler.ParsedFunction dinkFunction;

        public class EditableInstruction : INotifyPropertyChanged
        {
            public EditableInstruction()
            {
                DeleteCommand = new DinkPatchGui.ActionCommand(() =>
                {
                    if (!wasAdded) return;
                    RemoveMe?.Invoke(this, new EventArgs());
                });

                RemoveModificationCommand = new DinkPatchGui.ActionCommand(() =>
                {
                    IsModified = false;
                });

                ModifyCommand = new DinkPatchGui.ActionCommand(() =>
                {
                    IsModified = true;
                });

                EditCommentsCommand = new DinkPatchGui.ActionCommand(() =>
                {
                    EditComments ec = new EditComments(string.Join("\n", PreComment.Split('\n').Select(s => s.TrimStart(';', ' '))), LineComment.TrimStart(';', ' '));
                    ec.Closing += (o, e) =>
                    {

                        if (!String.IsNullOrWhiteSpace(ec.LineComment.Text))
                        {
                            LineComment = $"; {ec.LineComment.Text}";
                        }
                        else LineComment = "";

                        if (!String.IsNullOrWhiteSpace(ec.PreComment.Text))
                        {
                            PreComment = string.Join("\n", ec.PreComment.Text.Split('\n').Select(s => $"; {s}"));
                        }
                        else PreComment = "";
                    };
                    ec.Show();
                });
            }

            private int _instructionIndex = 0;
            public int InstructionIndex { get => _instructionIndex; set { _instructionIndex = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(InstructionIndex))); } }
            public string originalInstructionString { get; set; }
            public bool wasAdded { get; set; } = false;

            private bool _highlighted = false;
            public bool IsHighlighted { get => _highlighted; set { _highlighted = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsHighlighted))); } }

            private bool _modified = false;
            public bool IsModified
            {
                get => _modified; set
                {
                    RemoveMyHighlights?.Invoke(this, new EventArgs());
                    _modified = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsModified)));
                }
            }

            private string _newValue = "";
            public string NewValue
            {
                get => _newValue;
                set
                {
                    RemoveMyHighlights?.Invoke(this, new EventArgs());
                    _newValue = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(NewValue)));
                }
            }

            private string _preComment = "";
            public string PreComment
            {
                get => _preComment;
                set
                {
                    _preComment = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(PreComment)));
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(HasPreComment)));
                }
            }

            private string _lineComment = "";
            public string LineComment
            {
                get => _lineComment;
                set
                {
                    _lineComment = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(LineComment)));
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(HasLineComment)));
                }
            }

            public bool HasPreComment { get => !String.IsNullOrWhiteSpace(_preComment); set { } }
            public bool HasLineComment { get => !String.IsNullOrWhiteSpace(_lineComment); set { } }

            public DinkDisassembler.ParsedFunction.DinkyInstruction? originalInstruction { get; set; } = null;

            public DinkDisassembler.ParsedFunction.DinkyInstruction? parsedInstruction
            {
                get
                {
                    if (IsModified)
                    {
                        try
                        {
                            return DinkDisassembler.ParsedFunction.Patch_ParseInstruction(NewValue);
                        }
                        catch (Exception)
                        {
                            return null;
                        }
                    }
                    else return originalInstruction;
                }
            }

            public int? getReferencedConstant() => parsedInstruction?.getReferencedLocals();

            public List<int> getJumpTargets() => parsedInstruction?.getPotentialJumpTargets() ?? new List<int>();

            public void setReferencedConstant(int newConstant)
            {
                var instruction = parsedInstruction;
                if (instruction == null) return;
                IsModified = true;
                NewValue = $"{instruction.Value.Opcode.ToString().Replace("OP_", "")} {newConstant}";
            }

            public void setReferencedJumpTarget(int amount)
            {
                var instruction = parsedInstruction;
                if (instruction == null) return; IsModified = true;
                NewValue = $"{instruction.Value.Opcode.ToString().Replace("OP_", "")} {amount}";
            }

            public ICommand DeleteCommand { get; set; }
            public ICommand RemoveModificationCommand { get; set; }
            public ICommand ModifyCommand { get; set; }
            public ICommand EditCommentsCommand { get; set; }

            public event PropertyChangedEventHandler PropertyChanged;

            public event EventHandler RemoveMe;
            public event EventHandler RemoveMyHighlights;
        }

        public class EditableConstant : INotifyPropertyChanged
        {
            public EditableConstant()
            {
                DeleteCommand = new DinkPatchGui.ActionCommand(() =>
                {
                    if (!wasAdded) return;
                    RemoveMe?.Invoke(this, new EventArgs());
                });

                RemoveModificationCommand = new DinkPatchGui.ActionCommand(() =>
                {
                    IsModified = false;
                });

                ModifyCommand = new DinkPatchGui.ActionCommand(() =>
                {
                    IsModified = true;
                });
            }
            private int _constantIndex = 0;
            public int constantIndex { get => _constantIndex; set { _constantIndex = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(constantIndex))); } }
            public string originalValue { get; set; } = "";
            public string originalType { get; set; } = "";
            public bool wasAdded { get; set; } = false;

            private bool _highlighted = false;
            public bool IsHighlighted { get => _highlighted; set { _highlighted = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsHighlighted))); } }

            private bool _modified = false;
            public bool IsModified
            {
                get => _modified; set
                {
                    RemoveMyHighlights?.Invoke(this, new EventArgs());
                    _modified = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsModified)));
                }
            }

            private string _newValue = "";
            public string NewValue
            {
                get => _newValue; set
                {
                    RemoveMyHighlights?.Invoke(this, new EventArgs());
                    _newValue = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(NewValue)));
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ComputedType)));
                }
            }

            public string ComputedType
            {
                get
                {
                    if (String.IsNullOrWhiteSpace(NewValue)) return "string";

                    if (NewValue.Count(c => char.IsLetter(c)) == 1 && NewValue.Trim().Length > 1 && char.ToLower(NewValue.Last()) == 'f')
                    {
                        if (double.TryParse(NewValue.TrimEnd('f'), NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out _)) return "float";
                        else return "string";
                    }

                    if (int.TryParse(NewValue, out _)) return "int";
                    if (double.TryParse(NewValue, NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out _)) return "float";
                    return "string";
                }
            }

            public ICommand DeleteCommand { get; set; }
            public ICommand RemoveModificationCommand { get; set; }
            public ICommand ModifyCommand { get; set; }

            public event PropertyChangedEventHandler PropertyChanged;

            public event EventHandler RemoveMe;
            public event EventHandler RemoveMyHighlights;
        }

        class ModificationItem
        {
            public int oldIndex;
            public int newIndex;
        }

        public ObservableCollection<EditableConstant> constants { get; set; } = new ObservableCollection<EditableConstant>();
        public ObservableCollection<EditableInstruction> instructions { get; set; } = new ObservableCollection<EditableInstruction>();

        public DinkPatchedFunction(DinkyPatchFile.DinkyPatch function, DinkyPatchFile file, DinkDisassembler dink)
        {
            InitializeComponent();
            this.function = function;
            this.file = file;
            this.dink = dink;
            this.DataContext = this;

            if (!String.IsNullOrWhiteSpace(function.function_id)) dinkFunction = dink.FunctionByUid(function.function_id);
            else dinkFunction = dink.FunctionsInScript(function.script).Where(f => f.FunctionName == function.function).FirstOrDefault();

            function.function_id = dinkFunction.UID;

            // function name
            FunctionName.Text = $"{dinkFunction.UID}: {dinkFunction.ScriptName}::{dinkFunction.FunctionName}()";
            Comment.Text = function.comment;

            // add "real" constants
            int i = 0;
            foreach (var constant in dinkFunction.Locals)
            {
                string typeString = "";
                switch (constant.DataType)
                {
                    case 0x102:
                        typeString = "[int]";
                        break;
                    case 0x103:
                        typeString = "[float]";
                        break;
                    case 0x204:
                        typeString = "[string]";
                        break;
                    default:
                        typeString = $"[0x{constant.DataType:X3}]";
                        break;
                }

                EditableConstant ec = new EditableConstant()
                {
                    constantIndex = i,
                    originalValue = dinkFunction.LocalToString(constant, false),
                    originalType = typeString,
                };
                ec.RemoveMyHighlights += RemoveConstantHighlights;

                var patch = function.patches.Where(p => p.type.ToLowerInvariant() == "set_local" && p.index != null && p.index.Value == i).LastOrDefault();
                if (patch != null)
                {
                    ec.IsModified = true;
                    if (patch.value is string s) ec.NewValue = s;
                    else if (patch.value is long l) ec.NewValue = l.ToString();
                    else if (patch.value is int pi) ec.NewValue = pi.ToString();
                    else if (patch.value is float f) ec.NewValue = f.ToString(System.Globalization.CultureInfo.InvariantCulture) + "f";
                    else if (patch.value is double d) ec.NewValue = d.ToString(System.Globalization.CultureInfo.InvariantCulture) + "f";
                }

                constants.Add(ec);
                ++i;
            }

            // add constants defined in patch file
            foreach (var added in function.patches.Where(p => p.type.ToLowerInvariant() == "add_local"))
            {
                EditableConstant ec = new EditableConstant()
                {
                    constantIndex = added.index ?? -1,
                    wasAdded = true,
                    IsModified = true,
                    originalType = "",
                    originalValue = "",
                };

                ec.RemoveMe += RemoveConstant;
                ec.RemoveMyHighlights += RemoveConstantHighlights;

                if (added.value is string s) ec.NewValue = s;
                else if (added.value is long l) ec.NewValue = l.ToString();
                else if (added.value is int pi) ec.NewValue = pi.ToString();
                else if (added.value is float f) ec.NewValue = f.ToString(System.Globalization.CultureInfo.InvariantCulture) + "f";
                else if (added.value is double d) ec.NewValue = d.ToString(System.Globalization.CultureInfo.InvariantCulture) + "f";
                constants.Add(ec);
            }

            // add "real" instructions
            i = 0;
            foreach (var instruction in dinkFunction.Instructions)
            {
                EditableInstruction ei = new EditableInstruction()
                {
                    InstructionIndex = i,
                    originalInstructionString = instruction.ToString(),
                    originalInstruction = instruction,
                };
                ei.RemoveMyHighlights += RemoveFunctionHighlights;
                instructions.Add(ei);
                ++i;
            }

            // apply instruction patches
            foreach (var patch in function.patches)
            {
                switch (patch.type.ToLowerInvariant())
                {
                    case "replace_instruction":
                        {
                            var toReplace = instructions.Where(ii => ii.InstructionIndex == (patch.index ?? -1)).FirstOrDefault();
                            if (toReplace != null)
                            {
                                toReplace.IsModified = true;
                                toReplace.NewValue = patch.value as string ?? "";
                            }
                        }
                        break;
                    case "insert_instruction":
                    case "insert_instructions":
                        {
                            i = patch.index ?? 0;
                            foreach (var instructionString in (patch.value as string ?? "").Split('\n').Select(s => s.Trim()).Where(s => s.Length > 0))
                            {
                                EditableInstruction ei = new EditableInstruction()
                                {
                                    InstructionIndex = i,
                                    wasAdded = true,
                                    IsModified = true,
                                    NewValue = instructionString,
                                };
                                ei.RemoveMyHighlights += RemoveFunctionHighlights;
                                ei.RemoveMe += RemoveInstruction;
                                instructions.Insert(i, ei);

                                ++i;
                            }

                            List<ModificationItem> modifiedInstructions = new List<ModificationItem>();

                            for (int inst = i; inst < instructions.Count; ++inst)
                            {
                                modifiedInstructions.Add(new ModificationItem() { newIndex = inst, oldIndex = instructions[inst].InstructionIndex });
                                instructions[inst].InstructionIndex = inst;
                            }

                            OnInstructionsUpdated(modifiedInstructions);
                        }
                        break;
                }
            }

            // add comments
            foreach (var comment in file.function_comments?.Where(f => f.function_id == dinkFunction.UID))
            {
                if (comment.instruction_index >= 0 && comment.instruction_index < instructions.Count)
                {
                    switch (comment.comment_type.ToLowerInvariant())
                    {
                        case "pre_comment":
                            instructions[comment.instruction_index].PreComment = String.Join("\n", comment.comment.Split('\n').Select(s => $"; {s}"));
                            break;
                        case "line_comment":
                            instructions[comment.instruction_index].LineComment = $"; {comment.comment}";
                            break;
                    }
                }
            }
        }

        private void AutoComment(object sender, RoutedEventArgs e)
        {
            foreach (var inst in instructions)
            {
                var idx = inst.InstructionIndex;
                var instruction = inst.parsedInstruction;
                if (instruction == null) continue;

                switch (instruction.Value.Opcode)
                {
                    case DinkDisassembler.ParsedFunction.DinkyInstruction.DinkyOpCode.OP_CALL:
                    case DinkDisassembler.ParsedFunction.DinkyInstruction.DinkyOpCode.OP_FCALL:
                    case DinkDisassembler.ParsedFunction.DinkyInstruction.DinkyOpCode.OP_STORE_INDEXED:
                    case DinkDisassembler.ParsedFunction.DinkyInstruction.DinkyOpCode.OP_JUMP:
                    case DinkDisassembler.ParsedFunction.DinkyInstruction.DinkyOpCode.OP_JUMP_TRUE:
                    case DinkDisassembler.ParsedFunction.DinkyInstruction.DinkyOpCode.OP_JUMP_TOPTRUE:
                    case DinkDisassembler.ParsedFunction.DinkyInstruction.DinkyOpCode.OP_JUMP_FALSE:
                    case DinkDisassembler.ParsedFunction.DinkyInstruction.DinkyOpCode.OP_JUMP_TOPFALSE:
                    case DinkDisassembler.ParsedFunction.DinkyInstruction.DinkyOpCode.OP_STORE_LOCAL:
                    case DinkDisassembler.ParsedFunction.DinkyInstruction.DinkyOpCode.OP_NULL_LOCAL:
                    case DinkDisassembler.ParsedFunction.DinkyInstruction.DinkyOpCode.OP_SET_LOCAL:
                        inst.LineComment = $"; {GetAutoCommentForInstruction(ref idx)};";
                        break;
                }
            }
        }

        public void SavePatch()
        {
            // clear
            function.comment = Comment.Text;
            if (String.IsNullOrWhiteSpace(function.comment)) function.comment = null;

            if (function.patches == null) function.patches = new List<DinkyPatchFile.DinkyPatch.DinkyPatchItem>();
            if (file.function_comments == null) file.function_comments = new List<DinkyPatchFile.Comment>();

            function.patches.Clear();

            // first add the modification to constants
            foreach (var constant in constants)
            {
                if (constant.wasAdded || constant.IsModified)
                {
                    void parse_value(string val, out object value, out string valuetype)
                    {
                        valuetype = null;
                        if (String.IsNullOrWhiteSpace(val))
                        {
                            value = val;
                            return;
                        }

                        if (val.Count(c => char.IsLetter(c)) == 1 && val.Trim().Length > 1 && char.ToLower(val.Last()) == 'f')
                        {
                            if (double.TryParse(val.TrimEnd('f'), NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out double parsedFloat))
                            {
                                value = parsedFloat;
                            }
                            else
                            {
                                value = val;
                            }
                            return;
                        }

                        if (int.TryParse(val, out int i))
                        {
                            valuetype = "int";
                            value = i;
                            return;
                        }
                        if (double.TryParse(val, NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out double pf2))
                        {
                            value = pf2;
                            return;
                        }
                        value = val;
                    }

                    parse_value(constant.NewValue, out object new_value, out string value_type);
                    parse_value(constant.originalValue, out object original_value, out string original_value_type);

                    if (constant.wasAdded)
                    {
                        var patch = new DinkyPatchFile.DinkyPatch.DinkyPatchItem()
                        {
                            type = "add_local",
                            index = constant.constantIndex,
                            value = new_value,
                            valuetype = value_type,
                        };
                        function.patches.Add(patch);
                    }
                    else if (constant.IsModified)
                    {
                        var patch = new DinkyPatchFile.DinkyPatch.DinkyPatchItem()
                        {
                            type = "set_local",
                            index = constant.constantIndex,
                            value = new_value,
                            valuetype = value_type,
                            old_value = original_value,
                            oldvaluetype = original_value_type,
                        };
                        function.patches.Add(patch);
                    }
                }
            }

            // patched instructions: added instructions
            for (int i = 0; i < instructions.Count; ++i)
            {
                if (instructions[i].wasAdded)
                {
                    List<EditableInstruction> addedBlock = new List<EditableInstruction>();
                    int i2 = i;
                    while (instructions[i2].wasAdded)
                    {
                        addedBlock.Add(instructions[i2]);
                        i2++;
                    }

                    string newValue = string.Join("\n", addedBlock.Select(s => s.NewValue));

                    var patch = new DinkyPatchFile.DinkyPatch.DinkyPatchItem()
                    {
                        type = "insert_instructions",
                        index = i,
                        value = newValue,
                    };
                    function.patches.Add(patch);
                    i = i2 - 1;
                }
            }

            // patched instructions: changed instructions
            for (int i = 0; i < instructions.Count; ++i)
            {
                if (!instructions[i].wasAdded && instructions[i].IsModified)
                {
                    var newValue = instructions[i].NewValue;
                    var oldInstruction = instructions[i].originalInstruction;
                    string oldValue = null;
                    if (oldInstruction != null) oldValue = $"{oldInstruction.Value.instruction:X8}";


                    var patch = new DinkyPatchFile.DinkyPatch.DinkyPatchItem()
                    {
                        type = "replace_instruction",
                        index = i,
                        value = newValue,
                        old_value = oldValue,
                    };
                    function.patches.Add(patch);
                }
            }

            // comments
            for (int i = 0; i < instructions.Count; ++i)
            {
                if (instructions[i].HasPreComment)
                {
                    DinkyPatchFile.Comment comment = new DinkyPatchFile.Comment()
                    {
                        function_id = function.function_id,
                        comment_type = "pre_comment",
                        comment = String.Join("\n", instructions[i].PreComment.Split('\n').Select(s => s.TrimStart(';', ' '))),
                        instruction_index = i,
                    };
                    file.function_comments.Add(comment);
                }
                if (instructions[i].HasLineComment)
                {
                    DinkyPatchFile.Comment comment = new DinkyPatchFile.Comment()
                    {
                        function_id = function.function_id,
                        comment_type = "line_comment",
                        comment = instructions[i].LineComment.TrimStart(';', ' '),
                        instruction_index = i,
                    };
                    file.function_comments.Add(comment);
                }
            }
        }


        private void AddConstant_Click(object sender, RoutedEventArgs e)
        {
            EditableConstant ec = new EditableConstant()
            {
                constantIndex = constants.Count,
                wasAdded = true,
                IsModified = true,
                originalType = "",
                originalValue = "",
                NewValue = "",
            };
            ec.RemoveMe += RemoveConstant;
            constants.Add(ec);

            constants_scroller.ScrollToBottom();
        }

        private void RemoveConstant(object sender, EventArgs e)
        {
            int index = constants.IndexOf(sender as EditableConstant);
            if (index < 0) return;

            RemoveConstantHighlights(sender, e);

            List<ModificationItem> constantModifications = new List<ModificationItem>();
            constants.RemoveAt(index);
            for (int i = index; i < constants.Count; ++i)
            {
                constants[i].constantIndex--;
                constantModifications.Add(new ModificationItem() { newIndex = constants[i].constantIndex, oldIndex = constants[i].constantIndex + 1 });
            }

            OnConstantsUpdated(constantModifications);
        }


        private void RemoveInstruction(object sender, EventArgs e)
        {
            int index = instructions.IndexOf(sender as EditableInstruction);
            if (index < 0) return;

            RemoveFunctionHighlights(sender, e);

            List<ModificationItem> instructionModifications = new List<ModificationItem>();

            for (int i = index + 1; i < instructions.Count; ++i)
            {
                instructionModifications.Add(new ModificationItem() { newIndex = i - 1, oldIndex = i });
                instructions[i].InstructionIndex--;
            }

            instructions.RemoveAt(index);

            OnInstructionsUpdated(instructionModifications);
        }

        void OnConstantsUpdated(List<ModificationItem> modifications)
        {
            // update the instructions referencing the modified constants.
            foreach (var instruction in instructions)
            {
                var referencedConstant = instruction.getReferencedConstant();
                if (referencedConstant != null)
                {
                    var modified = modifications.Where(m => m.oldIndex == referencedConstant.Value).FirstOrDefault();
                    if (modified != null)
                    {
                        instruction.setReferencedConstant(modified.newIndex);
                    }
                }
            }
        }

        private void OnInstructionsUpdated(List<ModificationItem> modifiedInstructions)
        {
            foreach (var instruction in instructions)
            {
                var jump_targets = instruction.getJumpTargets();
                if (jump_targets.Count == 0) continue;
                if (jump_targets.Count > 1)
                {
                    MessageBox.Show($"WARNING: this operation may have resulted in the jump target of instructions {instruction.InstructionIndex} ({instruction.parsedInstruction?.ToString()}) to become invalid (it could not be manually adjusted).", "Warning");
                    continue;
                }

                int delta = 0;
                int jumpTarget = jump_targets[0];

                /*
                 * If both the source and the target instruction move in the same direction, the change in the source instruction is weighted as -1, 
                 * the change in the target instruction is weighted as +1.
                 * They both cancel out, the jump amount does not change.
                 * If the jump instruction jumps backwards:
                 *  - the source moves +1, the target +0 -> delta = -(+1) + +0 = -1 -> The function jumps back one more.
                 *  - the source moves -1, target moves +0 -> -(-1) + 0 = 1 -> the function jumps back one less
                 *  - both source and target move +1 -> -(+1) + +1 = 0 -> no change
                 *  - both source and target move -1 -> -(-1) + -1 = 0 -> no change
                 *  
                 * If the jump instruction jumps forwards:
                 *  - the source moves +0, the target moves +1 = -(+0) + +1 = 1 -> The function jumps forward one more.
                 *  - the source moves +0, the target moves -1 = -(+0) + -1 = -1 -> The function jumps forwards one less.
                 */

                int instruction_index = instruction.InstructionIndex;
                var current_instruction_modifier = modifiedInstructions.Where(w => w.newIndex == instruction_index).FirstOrDefault();
                if (current_instruction_modifier != null)
                {
                    instruction_index = current_instruction_modifier.oldIndex;
                    delta -= (current_instruction_modifier.newIndex - current_instruction_modifier.oldIndex);
                }

                int target_instruction_index = instruction_index + jumpTarget + 1;

                var target_instruction_modifier = modifiedInstructions.Where(w => w.oldIndex == target_instruction_index).FirstOrDefault();
                if (target_instruction_modifier != null)
                {
                    delta += (target_instruction_modifier.newIndex - target_instruction_modifier.oldIndex);
                }

                // avoid unnecessary re-formatting of instructions that do not change. 
                if (delta != 0)
                {
                    jumpTarget += delta;
                    instruction.setReferencedJumpTarget(jumpTarget);
                }
            }
        }

        private void MouseEnterInstruction(object sender, MouseEventArgs e)
        {
            if (!((sender as FrameworkElement)?.DataContext is EditableInstruction inst)) return;

            int? referenced = inst.getReferencedConstant();
            if (referenced != null)
            {
                foreach (var c in constants.Where(cc => cc.constantIndex == referenced.Value))
                {
                    c.IsHighlighted = true;
                    if (Constants.ItemContainerGenerator.ContainerFromItem(c) is FrameworkElement uielement)
                        uielement.BringIntoView();
                }
            }

            var targets = inst.getJumpTargets();
            foreach (var target in targets)
            {
                int targetIndex = inst.InstructionIndex + target + 1;
                var targetedInstruction = instructions.Where(i => i.InstructionIndex == targetIndex).FirstOrDefault();
                if (targetedInstruction != null) targetedInstruction.IsHighlighted = true;
            }
        }

        private void MouseLeaveInstruction(object sender, MouseEventArgs e)
        {
            if (!((sender as FrameworkElement)?.DataContext is EditableInstruction inst)) inst = sender as EditableInstruction;
            if (inst == null) return;

            int? referenced = inst.getReferencedConstant();
            if (referenced != null)
            {
                foreach (var c in constants.Where(cc => cc.constantIndex == referenced.Value)) c.IsHighlighted = false;
            }

            var targets = inst.getJumpTargets();
            foreach (var target in targets)
            {
                int targetIndex = inst.InstructionIndex + target + 1;
                var targetedInstruction = instructions.Where(i => i.InstructionIndex == targetIndex).FirstOrDefault();
                if (targetedInstruction != null) targetedInstruction.IsHighlighted = false;
            }
        }

        private void RemoveFunctionHighlights(object sender, EventArgs e) => MouseLeaveInstruction(sender, null);
        private void RemoveConstantHighlights(object sender, EventArgs e) => MouseLeaveConstant(sender, null);

        private void MouseEnterConstant(object sender, MouseEventArgs e)
        {
            if (!((sender as FrameworkElement)?.DataContext is EditableConstant constant)) constant = sender as EditableConstant;
            if (constant == null) return;

            foreach (var inst in instructions)
            {
                int? referenced = inst.getReferencedConstant();
                if (referenced != null && referenced.Value == constant.constantIndex)
                {
                    inst.IsHighlighted = true;
                }
            }
        }

        private void MouseLeaveConstant(object sender, MouseEventArgs e)
        {
            if (!((sender as FrameworkElement)?.DataContext is EditableConstant constant)) constant = sender as EditableConstant;
            if (constant == null) return;

            foreach (var inst in instructions)
            {
                int? referenced = inst.getReferencedConstant();
                if (referenced != null && referenced.Value == constant.constantIndex)
                {
                    inst.IsHighlighted = false;
                }
            }
        }

        private void InsertInstruction(object sender, MouseButtonEventArgs e)
        {
            if (!((sender as FrameworkElement)?.DataContext is EditableInstruction after)) return;

            int i = after.InstructionIndex;
            EditableInstruction ei = new EditableInstruction()
            {
                InstructionIndex = i,
                wasAdded = true,
                IsModified = true,
                NewValue = "",
            };
            ei.RemoveMyHighlights += RemoveFunctionHighlights;
            ei.RemoveMe += RemoveInstruction;
            instructions.Insert(i, ei);

            List<ModificationItem> modifiedInstructions = new List<ModificationItem>();

            for (int inst = i + 1; inst < instructions.Count; ++inst)
            {
                modifiedInstructions.Add(new ModificationItem() { newIndex = inst, oldIndex = instructions[inst].InstructionIndex });
                instructions[inst].InstructionIndex = inst;
            }

            OnInstructionsUpdated(modifiedInstructions);
        }

        private void Instructions_scroller_ScrollChanged(object sender, ScrollChangedEventArgs e) => SetScrollMarker();
        private void Instructions_scroller_SizeChanged(object sender, SizeChangedEventArgs e) => SetScrollMarker();

        private void SetScrollMarker()
        {
            double totalHeight = Instructions.ActualHeight;
            double shownHeight = instructions_scroller.ActualHeight;
            double offset = instructions_scroller.VerticalOffset;

            if (totalHeight == 0) return;

            double ratioShown = shownHeight / totalHeight;
            double ratioOffset = offset / totalHeight;

            double indicatorPanelHeight = ScrollIndicator_parent.ActualHeight;
            double indicatorHeight = indicatorPanelHeight * ratioShown;
            double indicatorOffset = indicatorPanelHeight * ratioOffset;

            ScrollIndicator_marker.Height = indicatorHeight;
            ScrollIndicator_marker.Margin = new Thickness(0, indicatorOffset, 0, 0);
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            SetScrollMarker();
        }

        private void ScrollMarker_DragDelta(object sender, System.Windows.Controls.Primitives.DragDeltaEventArgs e)
        {
            double totalHeight = Instructions.ActualHeight;
            double indicatorPanelHeight = ScrollIndicator_parent.ActualHeight;

            if (totalHeight == 0 || indicatorPanelHeight == 0) return;
            double ratio = e.VerticalChange / indicatorPanelHeight;
            double delta = totalHeight * ratio;

            instructions_scroller.ScrollToVerticalOffset(instructions_scroller.VerticalOffset + delta);
        }

        private string GetAutoCommentForInstruction(ref int index)
        {
            string GetConstantAsString(int constantIndex, bool quoted = false)
            {
                if (constantIndex >= constants.Count || constantIndex < 0) return "";
                var c = constants[constantIndex];

                if (c.IsModified)
                {
                    if (c.ComputedType == "string" && quoted) return $"\"{c.NewValue}\"";
                    return c.NewValue;
                }
                else
                {
                    if (c.originalType == "[string]" && quoted) return $"\"{c.originalValue}\"";
                    return c.originalValue;
                }
            }
            if (index < 0 || index >= instructions.Count) return "";
            var instruction = instructions[index].parsedInstruction;
            if (instruction == null) return "";
            switch (instruction.Value.Opcode)
            {
                default:
                    return "?";
                case DinkDisassembler.ParsedFunction.DinkyInstruction.DinkyOpCode.OP_PUSH_NULL:
                    return "null";
                case DinkDisassembler.ParsedFunction.DinkyInstruction.DinkyOpCode.OP_PUSH_CONST:
                    return GetConstantAsString((int)instruction.Value.PotentialParameter3, true);
                case DinkDisassembler.ParsedFunction.DinkyInstruction.DinkyOpCode.OP_PUSH_VAR:
                case DinkDisassembler.ParsedFunction.DinkyInstruction.DinkyOpCode.OP_PUSH_GLOBAL:
                    return GetConstantAsString((int)instruction.Value.PotentialParameter3);
                case DinkDisassembler.ParsedFunction.DinkyInstruction.DinkyOpCode.OP_PUSH_LOCAL:
                    return $"local{instruction.Value.PotentialParameter3}";
                case DinkDisassembler.ParsedFunction.DinkyInstruction.DinkyOpCode.OP_PUSH_LOCALREF:
                    return $"&local{instruction.Value.PotentialParameter3}";
                case DinkDisassembler.ParsedFunction.DinkyInstruction.DinkyOpCode.OP_PUSH_VARREF:
                case DinkDisassembler.ParsedFunction.DinkyInstruction.DinkyOpCode.OP_PUSH_GLOBALREF:
                    return $"&{GetConstantAsString((int)instruction.Value.PotentialParameter3)}";
                case DinkDisassembler.ParsedFunction.DinkyInstruction.DinkyOpCode.OP_CALL:
                case DinkDisassembler.ParsedFunction.DinkyInstruction.DinkyOpCode.OP_FCALL:
                    {
                        index -= 1;
                        string functionName = GetAutoCommentForInstruction(ref index);
                        List<string> arguments = new List<string>();
                        for (int i = 0; i < instruction.Value.PotentialParameter3; ++i)
                        {
                            index -= 1;
                            arguments.Add(GetAutoCommentForInstruction(ref index));
                        }
                        arguments.Reverse();
                        return $"{functionName}({String.Join(", ", arguments)})";
                    }
                case DinkDisassembler.ParsedFunction.DinkyInstruction.DinkyOpCode.OP_INDEX:
                    {
                        string member = GetConstantAsString((int)instruction.Value.PotentialParameter3);
                        index -= 1;
                        return $"{GetAutoCommentForInstruction(ref index)}.{member}";
                    }
                case DinkDisassembler.ParsedFunction.DinkyInstruction.DinkyOpCode.OP_STORE_INDEXED:
                    {
                        index -= 1;
                        string member = GetAutoCommentForInstruction(ref index);
                        index -= 1;
                        string before = GetAutoCommentForInstruction(ref index);
                        index -= 1;
                        string value = GetAutoCommentForInstruction(ref index);
                        return $"{before}[{member}] = {value}";
                    }
                case DinkDisassembler.ParsedFunction.DinkyInstruction.DinkyOpCode.OP_MATH:
                    {
                        index -= 1;
                        string rhs = GetAutoCommentForInstruction(ref index);
                        index -= 1;
                        string lhs = GetAutoCommentForInstruction(ref index);
                        switch (instruction.Value.PotentialParameter3)
                        {
                            case 0x3F: return $"{lhs} == {rhs}";
                            case 0x40: return $"{lhs} != {rhs}";
                            default: return $"{lhs} (math 0x{instruction.Value.PotentialParameter3:X2}) {rhs}";
                        }
                    }
                case DinkDisassembler.ParsedFunction.DinkyInstruction.DinkyOpCode.OP_JUMP:
                    {
                        int jumpAmount = (int)((instruction.Value.PotentialParameter1 & 0xFFFF) - 0x7FFF);
                        return $"GoTo {(index + jumpAmount + 1)}";
                    }
                case DinkDisassembler.ParsedFunction.DinkyInstruction.DinkyOpCode.OP_JUMP_TRUE:
                case DinkDisassembler.ParsedFunction.DinkyInstruction.DinkyOpCode.OP_JUMP_TOPTRUE:
                    {
                        int jumpAmount = (int)((instruction.Value.PotentialParameter1 & 0xFFFF) - 0x7FFF);
                        int iMe = index;

                        index -= 1;
                        string condition = GetAutoCommentForInstruction(ref index);
                        return $"if({condition}) GoTo {(iMe + jumpAmount + 1)}";
                    }
                case DinkDisassembler.ParsedFunction.DinkyInstruction.DinkyOpCode.OP_JUMP_FALSE:
                case DinkDisassembler.ParsedFunction.DinkyInstruction.DinkyOpCode.OP_JUMP_TOPFALSE:
                    {
                        int jumpAmount = (int)((instruction.Value.PotentialParameter1 & 0xFFFF) - 0x7FFF);
                        int iMe = index;

                        index -= 1;
                        string condition = GetAutoCommentForInstruction(ref index);
                        return $"if(!({condition})) GoTo {(iMe + jumpAmount + 1)}";
                    }
                case DinkDisassembler.ParsedFunction.DinkyInstruction.DinkyOpCode.OP_NEW_ARRAY:
                    {
                        string myParam = GetConstantAsString((int)instruction.Value.PotentialParameter3);
                        if (!int.TryParse(myParam, out int numItems)) return "";

                        List<string> items = new List<string>();
                        for (int i = 0; i < numItems; ++i)
                        {
                            index -= 1;
                            items.Add(GetAutoCommentForInstruction(ref index));
                        }

                        items.Reverse();
                        return $"[{String.Join(", ", items)}]";
                    }
                case DinkDisassembler.ParsedFunction.DinkyInstruction.DinkyOpCode.OP_STORE_LOCAL:
                    {
                        index -= 1;
                        string value = GetAutoCommentForInstruction(ref index);
                        return $"local{instruction.Value.PotentialParameter3} = {value}";
                    }
                case DinkDisassembler.ParsedFunction.DinkyInstruction.DinkyOpCode.OP_NULL_LOCAL:
                    return $"local{instruction.Value.PotentialParameter3} = null";
                case DinkDisassembler.ParsedFunction.DinkyInstruction.DinkyOpCode.OP_SET_LOCAL:
                    return $"local{instruction.Value.PotentialParameter3} = {GetConstantAsString(instruction.Value.PotentialParameter2 & 0xFF, true)}";
                case DinkDisassembler.ParsedFunction.DinkyInstruction.DinkyOpCode.OP_UNOT:
                    {
                        index -= 1;
                        string value = GetAutoCommentForInstruction(ref index);
                        return $"!({value})";
                    }
                case DinkDisassembler.ParsedFunction.DinkyInstruction.DinkyOpCode.OP_UMINUS:
                    {
                        index -= 1;
                        string value = GetAutoCommentForInstruction(ref index);
                        return $"-({value})";
                    }
                case DinkDisassembler.ParsedFunction.DinkyInstruction.DinkyOpCode.OP_UONECOMP:
                    {
                        index -= 1;
                        string value = GetAutoCommentForInstruction(ref index);
                        return $"~({value})";
                    }

            }
        }

    }
}
