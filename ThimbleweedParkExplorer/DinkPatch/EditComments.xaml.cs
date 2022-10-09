using System;
using System.Collections.Generic;
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

namespace ThimbleweedParkExplorer.DinkPatch
{
    /// <summary>
    /// Interaction logic for EditComments.xaml
    /// </summary>
    public partial class EditComments : Window
    {
        public EditComments(string precomment, string linecomment)
        {
            InitializeComponent();
            LineComment.Text = linecomment;
            PreComment.Text = precomment;

            ElementHost.EnableModelessKeyboardInterop(this);
        }

        private void save_click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
