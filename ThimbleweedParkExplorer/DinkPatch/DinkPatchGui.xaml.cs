using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms.Integration;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ThimbleweedLibrary;
using ThimbleweedParkExplorer.DinkPatch;

namespace ThimbleweedParkExplorer
{
    /// <summary>
    /// Interaction logic for DinkPatchGui.xaml
    /// </summary>
    public partial class DinkPatchGui : Window
    {
        public class ActionCommand : ICommand
        {
            public event EventHandler CanExecuteChanged;
            private Action<object> action = null;
            public bool CanExecute(object parameter) => true;
            public void Execute(object parameter) => action?.Invoke(parameter);
            public ActionCommand(Action a) => action = o => a();
            public ActionCommand(Action<object> a) => action = a;
        }

        public ICommand CmdSave { get; set; }

        public DinkPatchGui(DinkDisassembler dink)
        {
            InitializeComponent();
            this.dink = dink;
            CmdSave = new ActionCommand(() => SaveFile(this, null));
            DataContext = this;
            ElementHost.EnableModelessKeyboardInterop(this);
            tabs.Visibility = Visibility.Collapsed;
        }

        private void NewFile(object sender, RoutedEventArgs e)
        {
            PatchFile = new DinkyPatchFile();
            FileName = null;
            loadFile();
        }

        private void OpenFile(object sender, RoutedEventArgs e)
        {
            using (var sfd = new System.Windows.Forms.OpenFileDialog())
            {
                sfd.Filter = "Dinky patch files|*.dinkypatch";
                sfd.Title = "Select patch";
                if (sfd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    try
                    {
                        string dinkyfile = File.ReadAllText(sfd.FileName);
                        PatchFile = Newtonsoft.Json.JsonConvert.DeserializeObject(dinkyfile, typeof(DinkyPatchFile)) as DinkyPatchFile;
                        FileName = sfd.FileName;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Could not open patch file: {ex.Message}", "Failed opening patch file.");
                    }
                }
            }

            loadFile();
        }

        private void SaveFile(object sender, RoutedEventArgs e)
        {
            if (PatchFile == null) return;


            saveFile();
            string serialized = Newtonsoft.Json.JsonConvert.SerializeObject(PatchFile, Newtonsoft.Json.Formatting.Indented, new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore });

            if (String.IsNullOrEmpty(FileName))
            {
                using (var sfd = new System.Windows.Forms.SaveFileDialog())
                {
                    sfd.Filter = "Dinky patch files|*.dinkypatch";
                    sfd.FileName = "patch.dinkypatch";
                    sfd.Title = "Select patch";
                    if (sfd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    {
                        FileName = sfd.FileName;
                    }
                }
            }

            File.WriteAllText(FileName, serialized);
        }

        private void ExitGui(object sender, RoutedEventArgs e)
        {
            Close();
        }

        public DinkyPatchFile PatchFile { get; set; } = null;
        DinkDisassembler dink { get; set; }
        public string FileName { get; set; } = null;

        void loadFile()
        {
            tabs.Visibility = Visibility.Visible;
            Author.Text = PatchFile.author;
            PatchTitle.Text = PatchFile.title;
            Description.Text = PatchFile.description;

            Title = "DinkPatch";
            if (!String.IsNullOrEmpty(FileName)) Title = $"{FileName} - DinkPatch";

            while (tabs.Items.Count > 1) tabs.Items.RemoveAt(1);
            foreach (var func in PatchFile.function_patches)
            {
                CreateTab(func);
            }
        }

        void saveFile()
        {
            PatchFile.author = Author.Text;
            PatchFile.title = PatchTitle.Text;
            PatchFile.description = Description.Text;
            if (PatchFile.function_comments != null) PatchFile.function_comments.Clear();
            for (int i = 0; i < tabs.Items.Count; ++i)
            {
                var tab = tabs.Items[i] as TabItem;
                if (tab != null && tab.Content is DinkPatchedFunction func)
                {
                    func.SavePatch();
                }
            }
        }

        private void AddNewPatchedFunction(object sender, RoutedEventArgs e)
        {
            if (PatchFile == null) return;
            SelectDinkyFunction sdf = new SelectDinkyFunction(dink);
            sdf.ShowDialog();
            if (sdf.Result != null)
            {
                TabItem exisiting_tab = null;
                foreach (var ti in tabs.Items)
                {
                    if (ti is TabItem item && item.Content is DinkPatchedFunction tab && ((!String.IsNullOrEmpty(tab.function.function_id) && tab.function.function_id == sdf.Result.UID) ||
                        (tab.function.script == sdf.Result.ScriptName && tab.function.function == sdf.Result.FunctionName)))
                    {
                        exisiting_tab = item;
                        break;
                    }
                }

                if (exisiting_tab != null)
                {
                    tabs.SelectedItem = exisiting_tab;
                    return;
                }

                var func = new DinkyPatchFile.DinkyPatch() { function_id = sdf.Result.UID, script = sdf.Result.ScriptName, function = sdf.Result.FunctionName };
                if (PatchFile.function_patches == null) PatchFile.function_patches = new List<DinkyPatchFile.DinkyPatch>();
                PatchFile.function_patches.Add(func);
                CreateTab(func);
            }
        }

        private void CreateTab(DinkyPatchFile.DinkyPatch func)
        {
            DinkPatchedFunction dpf = new DinkPatchedFunction(func, PatchFile, dink);
            TextBlock header = new TextBlock() { Text = $"{func.function_id}: {func.script}::{func.function}".Trim(), FontSize = 16, ContextMenu = new ContextMenu() };
            var ti = new TabItem()
            {
                Header = header,
                Content = dpf,
                Style = Resources["tabItemStyle"] as Style,
            };
            header.ContextMenu.Items.Add(new MenuItem() { Header = "delete" });
            (header.ContextMenu.Items[0] as MenuItem).Click += (o, e) =>
            {
                if (PatchFile == null) return;
                PatchFile.function_patches.Remove(func);
                tabs.Items.Remove(ti);
            };
            tabs.Items.Add(ti);
        }

    }
}
