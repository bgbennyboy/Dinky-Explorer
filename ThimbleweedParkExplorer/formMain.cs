using System;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
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

            //Set search box height
            cueTextBox1.AutoSize = false;
            cueTextBox1.Height = panel1.Height;
            
            //Add 16x16 resources to the imagelist
            var list = ThimbleweedParkExplorer.Properties.Resources.ResourceManager.GetResourceSet(new System.Globalization.CultureInfo("en-us"), true, true);
            foreach (System.Collections.DictionaryEntry img in list)
            {
                //log(img.Key.ToString());
                //Use img.Value to get the bitmap
                var value = img.Value as Bitmap; //Only get images
                if (value != null)
                {
                    if (value.Width == 16 && value.Height == 16) //Only add the 16x16 images
                        imageList1.Images.Add((string)img.Key, value);
                }
            }

            //Setup image delegates
            InitializeListView();
        }

        private void InitializeListView()
        {
            this.columnFilename.ImageGetter = delegate (object rowObject) {
                BundleEntry b = (BundleEntry)rowObject;
                //Song s = (Song)rowObject;
                if (b.FileExtension == "ogg" || b.FileExtension == "wav")
                    return "small_audio";
                else
                    return "small_circle_white";
            };

        }
        private void btnOpen_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() != DialogResult.OK)
                return;

            Thimble = new BundleReader_ggpack(openFileDialog1.FileName);
            objectListView1.Items.Clear();
            objectListView1.SetObjects(Thimble.BundleFiles);
            objectListView1.AutoResizeColumns();
            AddFiletypeContextEntries();
        }

        private void log(string logText)
        {
            richTextBoxLog.AppendText(Environment.NewLine + logText);
        }

        private void btnView_Click(object sender, EventArgs e)
        {
            contextMenuView.Show(btnView, new Point(0, btnView.Height));
        }

        private void AddFiletypeContextEntries()
        {
            contextMenuView.Items.Clear();

            //Add all files and make it checked
            var menuItem = contextMenuView.Items.Add("All files");
            ((ToolStripMenuItem)menuItem).Checked = true;

            var entries = Thimble.BundleFiles.Select(x => x.FileExtension).Distinct().ToList();
            entries.Sort();
            entries.Insert(0, "-"); //Add the separator

            //Loop through and add the strings from the list
            for (int i= 0; i < entries.Count; i++)
            {
                contextMenuView.Items.Add(entries[i], Properties.Resources.small_circle_white);
            }


            //Now do the images
            for (int i = 0; i < contextMenuView.Items.Count; i++)
            {
                if (contextMenuView.Items[i].Text == "ogg" || contextMenuView.Items[i].Text == "wav")
                    contextMenuView.Items[i].Image = Properties.Resources.small_audio;
            }
        }

        private void formMain_Load(object sender, EventArgs e)
        {
     
        }

        private void btnSaveFile_Click(object sender, EventArgs e)
        {
            if (Thimble == null || Thimble.BundleFiles.Count == 0 || objectListView1.SelectedIndex == -1)
                return;

            saveFileDialog1.Filter = "All Files|*.*";
            saveFileDialog1.FileName = Thimble.BundleFiles[objectListView1.SelectedIndex].FileName;

            if (saveFileDialog1.ShowDialog() != DialogResult.OK)
                return;

            log("Saving file " + saveFileDialog1.FileName);
            Thimble.SaveFile(objectListView1.SelectedIndex, saveFileDialog1.FileName);
        }
    }



}
