using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ThimbleweedLibrary;

namespace ThimbleweedParkExplorer
{
    public partial class formMain : Form
    {
        public formMain()
        {
            InitializeComponent();
        }

        private void btnOpen_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() != DialogResult.OK)
                return;

            BundleReader_ggpack Thimble = new BundleReader_ggpack(openFileDialog1.FileName);
            //listView1.Items.Add(Thimble.BundleFiles[0].FileName);

            foreach(BundleEntry bundleEntry in Thimble.BundleFiles)
            {
                ListViewItem item = new ListViewItem();
                item.Text = bundleEntry.FileName;
                item.SubItems.Add(bundleEntry.FileExtension);
                item.SubItems.Add(bundleEntry.Offset.ToString());
                item.SubItems.Add(bundleEntry.Size.ToString());
                listView1.Items.Add(item);
            }
        }

        private void log(string logText)
        {
            richTextBoxLog.AppendText(logText + Environment.NewLine);
        }
    }
}
