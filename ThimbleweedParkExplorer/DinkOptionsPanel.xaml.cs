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
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ThimbleweedLibrary;

namespace ThimbleweedParkExplorer
{
    /// <summary>
    /// Interaction logic for DinkOptionsPanel.xaml
    /// </summary>
    public partial class DinkOptionsPanel : UserControl
    {
        public DinkOptionsPanel()
        {
            InitializeComponent();
        }

        public delegate void StringEvent(string message);
        public event StringEvent Log;
        public event StringEvent OnApplyPatch;

        public DinkDisassembler dink { get; set; } = null;

        private void DumpToFile(object sender, RoutedEventArgs e)
        {
            if (dink == null) return;

            using (var sfd = new System.Windows.Forms.SaveFileDialog())
            {
                sfd.FileName = "Weird.dinkasm";
                sfd.Title = "Select a location to save the script file";
                if (sfd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    File.WriteAllText(sfd.FileName, dink.ToString());
                }
            }
        }

        private void DumpToFiles(object sender, RoutedEventArgs e)
        {
            if (dink == null) return;
            using (var openFolder = new Microsoft.WindowsAPICodePack.Dialogs.CommonOpenFileDialog())
            {
                openFolder.AllowNonFileSystemItems = true;
                openFolder.Multiselect = false;
                openFolder.IsFolderPicker = true;
                openFolder.Title = "Select a folder to save the disassembly into";

                if (openFolder.ShowDialog() != Microsoft.WindowsAPICodePack.Dialogs.CommonFileDialogResult.Ok)
                    return;

                foreach (var scriptname in dink.ScriptFiles())
                {
                    var functions = dink.FunctionsInScript(scriptname);
                    string filename = System.IO.Path.Combine(openFolder.FileName, scriptname + "asm");
                    File.WriteAllText(filename, String.Join("\n\n\n", functions.Select(s => s.ToString())));
                }

                Log?.Invoke($"Dumped all scripts to {openFolder.FileName}");
            }
        }

        private void ApplyPatch(object sender, RoutedEventArgs e)
        {
            if (dink == null) return;

            using (var sfd = new System.Windows.Forms.OpenFileDialog())
            {
                sfd.Filter = "Dinky patch files|*.dinkypatch";
                sfd.Title = "Select patch";
                if (sfd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    OnApplyPatch?.Invoke(sfd.FileName);
                }
            }
        }

        private void EditPatchFile(object sender, RoutedEventArgs e)
        {
            DinkPatchGui gui = new DinkPatchGui(dink);
            gui.Show();
        }
    }
}
