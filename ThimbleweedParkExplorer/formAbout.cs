using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ThimbleweedParkExplorer
{
    public partial class formAbout : Form
    {
        public formAbout()
        {
            InitializeComponent();
        }

        private void formAbout_Load(object sender, EventArgs e)
        {
            labelAboutText.Text = labelAboutText.Text.Replace("Version", "Version " + Constants.Version);
            labelAboutText.Font = new Font(labelAboutText.Font.Name, 14, labelAboutText.Font.Style, labelAboutText.Font.Unit);
        }

        private void pictureBoxTop_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void pictureBoxBottom_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void formAbout_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void labelAboutText_Click(object sender, EventArgs e)
        {
            this.Close();
        }

    }
}
