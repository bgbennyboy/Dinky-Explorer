using BrightIdeasSoftware;
using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using ThimbleweedLibrary;

//TODO
//Try/except handling on opening bundle and dumping files particularlly all files - ggpack2 will raise exception
//Integrated text/image/sound
//Saving of text/image/sound in different formats
//Default image for the right bar like in DF Explorer
//Send to hex editor button as usual
//Decoding of wimpy files - tree files?
//Please wait working while dumping and lock controls

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
                //Use img.Value to get the bitmap
                //log(img.Key.ToString());
                var value = img.Value as Bitmap; //Only get images
                if (value != null)
                {
                    if (value.Width == 16 && value.Height == 16) //Only add the 16x16 images
                        imageList1.Images.Add((string)img.Key, value);
                }
            }

            //Setup image delegates
            InitializeListView();

            //Add info to log box
            richTextBoxLog.Text = Constants.ProgName + " " + Constants.Version + Environment.NewLine + Constants.URL;
            
            objectListView1.SetNativeBackgroundTiledImage(Properties.Resources.listViewBackground);

            EnableDisableControlsContextDependant();
        }

        private void InitializeListView()
        {
            this.columnFilename.ImageGetter = delegate (object rowObject)
            {
                BundleEntry b = (BundleEntry)rowObject;
                //Song s = (Song)rowObject;
                if (b.FileType == BundleEntry.FileTypes.Sound)
                    return Properties.Resources.small_audio;
                else if (b.FileType == BundleEntry.FileTypes.Text)
                    return Properties.Resources.small_text;
                else if (b.FileType == BundleEntry.FileTypes.Image)
                    return Properties.Resources.small_image;
                else
                    return Properties.Resources.small_circle_white;
            };
        }

        private void btnOpen_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() != DialogResult.OK)
                return;


            try
            {
                EnableDisableControls(false);

                if (Thimble != null)
                    Thimble.Dispose();
                Thimble = new BundleReader_ggpack(openFileDialog1.FileName);

                richTextBoxLog.Clear();
                log("Opened " + openFileDialog1.SafeFileName);
                objectListView1.Items.Clear();
                objectListView1.SetObjects(Thimble.BundleFiles);
                objectListView1.AutoResizeColumns();
                AddFiletypeContextEntries();
            }
            finally
            {
                EnableDisableControls(true);
            }
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

            //Add all files and make it checked
            var menuItem = contextMenuView.Items.Add(Constants.AllFiles_ContextMenu);
            ((ToolStripMenuItem)menuItem).Checked = true;

            var entries = Thimble.BundleFiles.Select(x => x.FileExtension).Distinct().ToList();
            entries.Sort();
            entries.Insert(0, "-"); //Add the separator

            //Loop through and add the strings from the list
            for (int i = 0; i < entries.Count; i++)
            {
                contextMenuView.Items.Add(entries[i], Properties.Resources.small_circle_white);
            }

            //Now do the images
            for (int i = 0; i < contextMenuView.Items.Count; i++)
            {
                if (contextMenuView.Items[i].Text == "ogg" || contextMenuView.Items[i].Text == "wav")
                    contextMenuView.Items[i].Image = Properties.Resources.small_audio;
                if (contextMenuView.Items[i].Text == "png")
                    contextMenuView.Items[i].Image = Properties.Resources.small_image;
                if (contextMenuView.Items[i].Text == "txt" || contextMenuView.Items[i].Text == "tsv" ||
                    contextMenuView.Items[i].Text == "nut" || contextMenuView.Items[i].Text == "json" ||
                    contextMenuView.Items[i].Text == "fnt" || contextMenuView.Items[i].Text == "byack" ||
                    contextMenuView.Items[i].Text == "lip")
                    contextMenuView.Items[i].Image = Properties.Resources.small_text;
            }
        }


        private void btnSaveFile_Click(object sender, EventArgs e)
        {
            if (Thimble == null || Thimble.BundleFiles.Count == 0 || objectListView1.SelectedIndex == -1)
                return;

            saveFileDialog1.Filter = "All Files|*.*";
            saveFileDialog1.FileName = ((BundleEntry)objectListView1.SelectedObject).FileName;

            if (saveFileDialog1.ShowDialog() != DialogResult.OK)
                return;

            int index = Thimble.BundleFiles.IndexOf((BundleEntry)objectListView1.SelectedObject);

            log("Saving file " + saveFileDialog1.FileName);
            Thimble.SaveFile(index, saveFileDialog1.FileName);
        }

        private void cueTextBox1_TextChanged(object sender, EventArgs e)
        {
            objectListView1.ModelFilter = TextMatchFilter.Contains(objectListView1, cueTextBox1.Text);
        }

        private void contextMenuView_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            ToolStripItem item = e.ClickedItem;

            //Clear the search box
            cueTextBox1.Clear();

            //Check for 'All Files' first
            if (e.ClickedItem.Text == Constants.AllFiles_ContextMenu)
            {
                objectListView1.ModelFilter = null;
            }
            else
            {
                //Filter the listview by extension
                objectListView1.ModelFilter = new ModelFilter(delegate (object x)
                {
                    return ((BundleEntry)x).FileExtension == item.Text;
                });
            }

            //First remove check mark from any items that have it currently
            for (int i = 0; i < contextMenuView.Items.Count; i++)
            {
                if (contextMenuView.Items[i] is ToolStripMenuItem) //The separator is a different type
                {
                    ((ToolStripMenuItem)contextMenuView.Items[i]).Checked = false;
                }
            }

            //Then make the clicked item checked
            ((ToolStripMenuItem)item).Checked = true;
        }

        private void objectListView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Thimble == null || Thimble.BundleFiles.Count == 0 || objectListView1.SelectedIndex == -1)
                return;

            EnableDisableControlsContextDependant();

            int index = Thimble.BundleFiles.IndexOf((BundleEntry)objectListView1.SelectedObject);

            switch (Thimble.BundleFiles[index].FileType)
            {
                case BundleEntry.FileTypes.Image:
                    panelImage.BringToFront();
                    using (MemoryStream ms = new MemoryStream())
                    {
                        //Release resources from old image
                        var oldImage = pictureBoxPreview.Image as Bitmap;
                        if (oldImage != null)
                            ((IDisposable)oldImage).Dispose();

                        Thimble.SaveFileToStream(index, ms);
                        var image = Image.FromStream(ms); 
                        pictureBoxPreview.Image = image;

                        //Set scaling mode depending on whether image is larger or smaller than the picturebox. From https://stackoverflow.com/questions/41188806/fit-image-to-picturebox-if-picturebox-is-smaller-than-picture
                        var imageSize = pictureBoxPreview.Image.Size;
                        var fitSize = pictureBoxPreview.ClientSize;
                        pictureBoxPreview.SizeMode = imageSize.Width > fitSize.Width || imageSize.Height > fitSize.Height ? PictureBoxSizeMode.Zoom : PictureBoxSizeMode.CenterImage;
                    }
                    break;

                case BundleEntry.FileTypes.Sound:
                    panelAudio.BringToFront();
                    break;

                case BundleEntry.FileTypes.Text:
                    panelText.BringToFront();
                    using (MemoryStream ms = new MemoryStream())
                    {
                        Thimble.SaveFileToStream(index, ms);

                        string[] tempArray = new string[] { }; //initialise an empty array of strings
                        if (Decoders.ExtractText(ms, out tempArray) == true)
                        {
                            textBoxPreview.Lines = tempArray;
                        }
                    }
                    break;

                default:
                    panelBlank.BringToFront();
                    break;
            }
        }

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (components != null)
                {
                    components.Dispose();
                }

                // Dispose stuff here
                if (Thimble != null)
                    Thimble.Dispose();
            }
            base.Dispose(disposing);
        }

        private void btnSaveAllFiles_Click(object sender, EventArgs e)
        {
            if (Thimble == null || Thimble.BundleFiles.Count == 0)
                return;

            using (var openFolder = new CommonOpenFileDialog())
            {
                openFolder.AllowNonFileSystemItems = true;
                openFolder.Multiselect = false;
                openFolder.IsFolderPicker = true;
                openFolder.Title = "Select a folder";

                if (openFolder.ShowDialog() != CommonFileDialogResult.Ok)
                    return;

                try
                {
                    log("Saving all files...");
                    EnableDisableControls(false);

                    progressBar1.Visible = true;
                    progressBar1.Maximum = Thimble.BundleFiles.Count;
                    progressBar1.Step = 1;

                    for (int i = 0; i < Thimble.BundleFiles.Count; i++)
                    {
                        Thimble.SaveFile(i, Path.Combine(openFolder.FileName, Thimble.BundleFiles[i].FileName));
                        progressBar1.PerformStep();
                        Application.DoEvents(); //HACK use backgroundworker or async task in future
                    }

                    log("...done!");
                }
                finally
                {
                    EnableDisableControls(true);
                    progressBar1.Visible = false;
                }
            }

        }

        private void EnableDisableControls(bool Value)
        {
            btnOpen.Enabled = Value;
            btnView.Enabled = Value;
            btnSaveFile.Enabled = Value;
            btnSaveAllFiles.Enabled = Value;
            btnAbout.Enabled = Value;
            cueTextBox1.Enabled = Value;
            objectListView1.Enabled = Value;

            if (Value == true)
                EnableDisableControlsContextDependant();
        }

        private void EnableDisableControlsContextDependant()
        {
            if (objectListView1.GetItemCount() > 0)
                btnSaveAllFiles.Enabled = true;
            else
                btnSaveAllFiles.Enabled = false;

            btnSaveFile.Enabled = objectListView1.SelectedIndex != -1;

            if (objectListView1.SelectedIndex != -1)
            {
                //Different menu items make visible
            }
        }

        private void btnAbout_Click(object sender, EventArgs e)
        {
            formAbout f = new formAbout();
            f.ShowDialog(this);
            f.Dispose();
        }
    }
}