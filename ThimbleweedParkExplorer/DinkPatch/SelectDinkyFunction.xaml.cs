using System;
using System.Collections.Generic;
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
    /// <summary>
    /// Interaction logic for SelectDinkyFunction.xaml
    /// </summary>
    public partial class SelectDinkyFunction : Window
    {
        public SelectDinkyFunction(DinkDisassembler dink)
        {
            InitializeComponent();

            foreach (var scriptfile in dink.ScriptFiles())
            {
                TreeViewItem script = new TreeViewItem() { Header = scriptfile };
                tree.Items.Add(script);
                foreach (var func in dink.FunctionsInScript(scriptfile))
                {
                    string function = $"{func.UID}: {func.FunctionName}";
                    TreeViewItem f = new TreeViewItem() { Header = function };
                    script.Items.Add(f);
                    f.MouseDoubleClick += (o, e) =>
                    {
                        Result = func;
                        Close();
                    };
                }
            }
        }

        public DinkDisassembler.ParsedFunction Result { get; set; } = null;
    }
}
