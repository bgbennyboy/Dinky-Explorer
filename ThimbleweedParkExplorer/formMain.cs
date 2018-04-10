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
        public BundleReader_ggpack Thimble;

        public formMain()
        {
            InitializeComponent();
        }

        private void btnOpen_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() != DialogResult.OK)
                return;
            /*
                        listView1.Items.Clear();

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
                        */
            Thimble = new BundleReader_ggpack(openFileDialog1.FileName);
            objectListView1.Items.Clear();
            objectListView1.SetObjects(Thimble.BundleFiles);
            objectListView1.AutoResizeColumns();
            AddFiletypeContextEntries();
        }

        private void log(string logText)
        {
            richTextBoxLog.AppendText(logText + Environment.NewLine);
        }

        private void btnView_Click(object sender, EventArgs e)
        {
            contextMenuView.Show(btnView, new Point(0, btnView.Height));
        }

        private void AddFiletypeContextEntries()
        {
            contextMenuView.Items.Clear();
            
            var entries = Thimble.BundleFiles.Select(x => x.FileExtension).Distinct().ToList();
            entries.Sort();
            entries.Insert(0, "All files"); //Add all files at the top
            entries.Insert(1, "-"); //Add the separator next
            for (int i= 0; i < entries.Count; i++)
            {
                contextMenuView.Items.Add(entries[i]);
            }
           
         
        }
    }
}
