using BrightIdeasSoftware;
using Microsoft.WindowsAPICodePack.Dialogs;
using NAudio.Vorbis;
using NAudio.Wave;
using System;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using ThimbleweedLibrary;

//TODO
//Decoding of wimpy files - seem to be 'room' or directories of files + other stuff like walkboxes. Json animation files look like the same format too.


namespace ThimbleweedParkExplorer
{
    public partial class formMain : Form
    {
        private WaveOutEvent outputDevice;
        private WaveStream audioReader;
        private MemoryStream audioDataStream;
        public BundleReader_ggpack Thimble;

        public formMain()
        {
            InitializeComponent();
            RtMIKeyReader.OnSearchForMonkeyIsland += RtMIKeyReader_OnSearchForMonkeyIsland;
            //Get icon from exe and use for form icon
            Icon = Icon.ExtractAssociatedIcon(System.Reflection.Assembly.GetExecutingAssembly().Location);

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
            richTextBoxLog.Text = Constants.ProgName + " " + Constants.Version + Environment.NewLine + Constants.URL + Environment.NewLine;

            //Set listview background
            objectListView1.SetNativeBackgroundTiledImage(Properties.Resources.listViewBackground);

            EnableDisableControlsContextDependant();

            panelBlank.BringToFront();
        }

        private string RtMIKeyReader_OnSearchForMonkeyIsland()
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "Executable File|*.exe";
            if (dialog.ShowDialog() != DialogResult.OK) return "";
            return dialog.FileName;
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

                if (outputDevice != null)
                    outputDevice.Dispose();

                if (audioReader != null)
                    audioReader.Dispose();

                if (audioDataStream != null)
                    audioDataStream.Dispose();
            }
            base.Dispose(disposing);
        }

        //Form closing event
        private void formMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            DisposeAudio();
        }

        //Setup image delegates for the listview
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
                else if (b.FileType == BundleEntry.FileTypes.Bnut)
                    return Properties.Resources.small_code;
                else if (b.FileType == BundleEntry.FileTypes.GGDict)
                    return Properties.Resources.gg;
                else
                    return Properties.Resources.small_circle_white;
            };
        }


        //************************************************Button click events*********************************************************************

        //Button open clicked
        private void btnOpen_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() != DialogResult.OK)
                return;

            try
            {
                EnableDisableControls(false);

                //objectListView1.Items.Clear();

                if (Thimble != null)
                {
                    objectListView1.RemoveObjects(Thimble.BundleFiles);
                    Thimble.Dispose();
                }

                //Clear any existing filters
                objectListView1.ModelFilter = null;

                Thimble = new BundleReader_ggpack(openFileDialog1.FileName);
                Thimble.LogEvent += this.HandleLogEvent;

                richTextBoxLog.Clear();
                log("Opened " + openFileDialog1.SafeFileName);
                objectListView1.SetObjects(Thimble.BundleFiles);
                objectListView1.AutoResizeColumns();
                panelBlank.BringToFront();
                AddFiletypeContextEntries();
                UpdateSaveAllMenu();
                log(Thimble.BundleFiles.Count + " files in bundle.");
            }
            catch (ArgumentException ex)
            {
                log(ex.Message);
                contextMenuView.Items.Clear();
            }
            finally
            {
                EnableDisableControls(true);
            }
        }

        //Button view clicked
        private void btnView_Click(object sender, EventArgs e)
        {
            contextMenuView.Show(btnView, new Point(0, btnView.Height));
        }

        //Button save single file clicked
        private void btnSaveFile_Click(object sender, EventArgs e)
        {
            contextMenuSaveFile.Show(btnSaveFile, new Point(0, btnSaveFile.Height));
        }

        //Button SaveAllFiles CLicked
        private void btnSaveAllFiles_Click(object sender, EventArgs e)
        {
            contextMenuSaveAll.Show(btnSaveAllFiles, new Point(0, btnSaveAllFiles.Height));
        }


        //Button about clicked
        private void btnAbout_Click(object sender, EventArgs e)
        {
            formAbout f = new formAbout();
            f.ShowDialog(this);
            f.Dispose();
        }






        //************************************************Save handlers*********************************************************************

        //Save all files handler
        private void SaveAllHandler(object sender, EventArgs e)
        {
            if (Thimble == null || Thimble.BundleFiles.Count == 0)
                return;

            //BundleEntry.FileTypes TargetFileType = BundleEntry.FileTypes.None;
            BundleEntry.FileTypes TargetFileType;
            string SavingMessage = "";

            if (sender.Equals(toolStripSaveAllRaw))
            {
                TargetFileType = BundleEntry.FileTypes.None;
                SavingMessage = "Saving all files...";
            }
            else if (sender.Equals(toolStripSaveAllVisible))
            {
                TargetFileType = BundleEntry.FileTypes.None;
                SavingMessage = "Saving all visible files...";
            }
            else if (sender.Equals(toolStripSaveAllAudio))
            {
                TargetFileType = BundleEntry.FileTypes.Sound;
                SavingMessage = "Saving all audio files...";
            }
            else if (sender.Equals(toolStripSaveAllImages))
            {
                TargetFileType = BundleEntry.FileTypes.Image;
                SavingMessage = "Saving all images...";
            }
            else if (sender.Equals(toolStripSaveAllText))
            {
                TargetFileType = BundleEntry.FileTypes.Text;
                SavingMessage = "Saving all text files...";
            }
            else if (sender.Equals(toolStripSaveAllBnut))
            {
                TargetFileType = BundleEntry.FileTypes.Bnut;
                SavingMessage = "Saving all bnut script files...";
            }
            else
            {
                log("Unknown sender in SaveAllHandler !");
                return;
            }

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
                    log(SavingMessage);
                    EnableDisableControls(false);
                    panelProgress.Visible = true;
                    panelProgress.BringToFront();

                    progressBar1.Visible = true;
                    progressBar1.Maximum = Thimble.BundleFiles.Count;
                    progressBar1.Step = 1;
                    progressBar1.Value = 0;

                    //Save visible uses different technique for dumping
                    if (sender.Equals(toolStripSaveAllVisible))
                    {
                        progressBar1.Maximum = objectListView1.Items.Count; //Count of filtered items
                        foreach (var item in objectListView1.FilteredObjects)
                        {
                            int index = Thimble.BundleFiles.IndexOf((BundleEntry)item);
                            Thimble.SaveFile(index, Path.Combine(openFolder.FileName, Thimble.BundleFiles[index].FileName));
                            progressBar1.PerformStep();
                            Application.DoEvents(); //HACK use backgroundworker or async task in future
                        }
                    }
                    else
                    {
                        for (int i = 0; i < Thimble.BundleFiles.Count; i++)
                        {
                            if (TargetFileType == BundleEntry.FileTypes.None) //Dump all raw files
                                Thimble.SaveFile(i, Path.Combine(openFolder.FileName, Thimble.BundleFiles[i].FileName));
                            else if (TargetFileType == Thimble.BundleFiles[i].FileType) //Sound/image/text
                                Thimble.SaveFile(i, Path.Combine(openFolder.FileName, Thimble.BundleFiles[i].FileName));

                            progressBar1.PerformStep();
                            Application.DoEvents(); //HACK use backgroundworker or async task in future
                        }
                    }

                    log("...done!");
                }
                finally
                {
                    EnableDisableControls(true);
                    progressBar1.Visible = false;
                    panelProgress.Visible = false;
                }
            }
        }

        //Save single file handler
        private void SaveFileAsHandler(object sender, EventArgs e)
        {
            if (Thimble == null || Thimble.BundleFiles.Count == 0 || objectListView1.SelectedIndex == -1)
                return;

            if (sender.Equals(toolStripSaveFileAsAudio))
            {
                saveFileDialog1.Filter = "Audio Files|*.ogg*";
                saveFileDialog1.FileName = ((BundleEntry)objectListView1.SelectedObject).FileName;
            }
            else if (sender.Equals(toolStripSaveFileAsImage))
            {
                saveFileDialog1.Filter = "Image Files|*.png;*.ktx";
                saveFileDialog1.FileName = ((BundleEntry)objectListView1.SelectedObject).FileName;
                if (saveFileDialog1.FileName.EndsWith(".ktxbz")) saveFileDialog1.FileName = saveFileDialog1.FileName.Replace(".ktxbz", ".ktx");
            }
            else if (sender.Equals(toolStripSaveFileAsText))
            {
                saveFileDialog1.Filter = "Text Files|*.txt*";
                saveFileDialog1.FileName = ((BundleEntry)objectListView1.SelectedObject).FileName + ".txt";
            }
            else if (sender.Equals(toolStripSaveFileRaw))
            {
                saveFileDialog1.Filter = "All Files|*.**";
                saveFileDialog1.FileName = ((BundleEntry)objectListView1.SelectedObject).FileName;
            }
            else
            {
                log("Unknown sender in SaveFileAs !");
                return;
            }

            if (saveFileDialog1.ShowDialog() != DialogResult.OK)
                return;

            int index = Thimble.BundleFiles.IndexOf((BundleEntry)objectListView1.SelectedObject);

            try
            {
                if (Thimble.BundleFiles[index].FileExtension == "ktxbz")
                {
                    byte[] ktxData;
                    using (var memstr = new MemoryStream())
                    {
                        Thimble.SaveFileToStream(index, memstr);
                        ktxData = DecompressStream(memstr);
                    }

                    if (saveFileDialog1.FileName.ToLowerInvariant().EndsWith(".png"))
                    {
                        ktxData = KtxToPng(new MemoryStream(ktxData));
                    }

                    File.WriteAllBytes(saveFileDialog1.FileName, ktxData);
                }
                else
                {
                    log("Saving file " + saveFileDialog1.FileName);
                    EnableDisableControls(false);
                    Thimble.SaveFile(index, saveFileDialog1.FileName);
                }
            }
            finally
            {
                EnableDisableControls(true);
            }
        }



        //************************************************Audio stuff*********************************************************************

        //Button sound play clicked
        private void btnSoundPlay_Click(object sender, EventArgs e)
        {
            if (Thimble == null || Thimble.BundleFiles.Count == 0 || objectListView1.SelectedIndex == -1)
                return;

            int index = Thimble.BundleFiles.IndexOf((BundleEntry)objectListView1.SelectedObject);

            if (Thimble.BundleFiles[index].FileType != BundleEntry.FileTypes.Sound)
                return;

            DisposeAudio();

            audioDataStream = new MemoryStream();
            Thimble.SaveFileToStream(index, audioDataStream);
            outputDevice = new WaveOutEvent();

            if (Thimble.BundleFiles[index].FileExtension == "ogg")
            {
                audioReader = new VorbisWaveReader(audioDataStream);
            }
            else if (Thimble.BundleFiles[index].FileExtension == "wav")
            {
                audioReader = new WaveFileReader(audioDataStream);
            }
            else throw new InvalidOperationException("Not a correct audio file type.");

            outputDevice.Init(audioReader);
            labelSoundProgress.Text = String.Format("{0:00}:{1:00} / {2:00}:{3:00}", 0, 0, (int)audioReader.TotalTime.TotalMinutes, audioReader.TotalTime.Seconds);
            outputDevice.Play();
        }

        //Button sound stop clicked
        private void btnSoundStop_Click(object sender, EventArgs e)
        {
            DisposeAudio();
        }

        //Button sound pause clicked
        private void btnSoundPause_Click(object sender, EventArgs e)
        {
            if (outputDevice != null)
            {
                if (outputDevice.PlaybackState == PlaybackState.Playing) outputDevice.Pause();
                else if (outputDevice.PlaybackState == PlaybackState.Paused) outputDevice.Play();
            }
        }

        //Dispose of all audio resources
        private void DisposeAudio()
        {
            if (outputDevice != null)
            {
                if (outputDevice.PlaybackState == PlaybackState.Playing) outputDevice.Stop();
                outputDevice.Dispose();
                outputDevice = null;
            }
            if (audioReader != null)
            {
                audioReader.Dispose();
                audioReader = null;
            }
            if (audioDataStream != null)
            {
                audioDataStream.Dispose();
                audioDataStream = null;
            }

            labelSoundProgress.Text = "";
        }

        //Timer tick event
        private void timer1_Tick(object sender, EventArgs e)
        {
            if (outputDevice != null && audioReader != null)
            {
                TimeSpan currentTime = (outputDevice.PlaybackState == PlaybackState.Stopped) ? TimeSpan.Zero : audioReader.CurrentTime;
                trackBarSound.Value = Math.Min(trackBarSound.Maximum, (int)(100 * currentTime.TotalSeconds / audioReader.TotalTime.TotalSeconds));
                labelSoundProgress.Text = String.Format("{0:00}:{1:00} / {2:00}:{3:00}", (int)currentTime.TotalMinutes, currentTime.Seconds, (int)audioReader.TotalTime.TotalMinutes, audioReader.TotalTime.Seconds);
            }
            else
            {
                trackBarSound.Value = 0;
            }
        }

        //Sound trackbar scroll event
        private void trackBarSound_Scroll(object sender, EventArgs e)
        {
            if (outputDevice != null && audioReader != null)
            {
                TimeSpan temp = TimeSpan.FromSeconds(audioReader.TotalTime.TotalSeconds * trackBarSound.Value / 100.0);
                if (temp != null && temp < audioReader.TotalTime)
                    audioReader.CurrentTime = temp;
            }
        }





        //************************************************Everything else*********************************************************************


        //Handle log event from other class
        private void HandleLogEvent(object sender, StringEventArgs e)
        {
            log(e.Message);
        }

        //Log string to the textbox
        private void log(string logText)
        {
            richTextBoxLog.AppendText(logText + Environment.NewLine);
        }

        //Enable/disable controls depending on param
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

        //Enable/disable controls depending on the context - itemcount, filetypes in file etc
        private void EnableDisableControlsContextDependant()
        {
            if (objectListView1.GetItemCount() > 0)
                btnSaveAllFiles.Enabled = true;
            else
                btnSaveAllFiles.Enabled = false;

            btnSaveFile.Enabled = objectListView1.SelectedIndex != -1;

            if (objectListView1.SelectedIndex != -1)
            {
                //Different save context menu items make visible
                toolStripSaveFileAsAudio.Visible = ((BundleEntry)objectListView1.SelectedObject).FileType == BundleEntry.FileTypes.Sound;
                toolStripSaveFileAsImage.Visible = ((BundleEntry)objectListView1.SelectedObject).FileType == BundleEntry.FileTypes.Image;
                toolStripSaveFileAsText.Visible = ((BundleEntry)objectListView1.SelectedObject).FileType == BundleEntry.FileTypes.Text || ((BundleEntry)objectListView1.SelectedObject).FileType == BundleEntry.FileTypes.Bnut;
            }
        }

        //Add the appropriate filetypes to the view contextmenu
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
            foreach (string entry in entries)
                contextMenuView.Items.Add(entry, Properties.Resources.small_circle_white);

            //Now do the images
            foreach (ToolStripItem item in contextMenuView.Items)
            {
                var temp = Thimble.BundleFiles.FirstOrDefault(x => x.FileExtension == item.Text); //get first item that has the same file extension so we can look at its filetype]
                if (temp != null)
                {
                    switch (temp.FileType)
                    {
                        case BundleEntry.FileTypes.Bnut:
                            item.Image = Properties.Resources.small_code;
                            break;
                        case BundleEntry.FileTypes.Image:
                            item.Image = Properties.Resources.small_image;
                            break;
                        case BundleEntry.FileTypes.Sound:
                            item.Image = Properties.Resources.small_audio;
                            break;
                        case BundleEntry.FileTypes.Text:
                            item.Image = Properties.Resources.small_text;
                            break;
                        case BundleEntry.FileTypes.GGDict:
                            item.Image = Properties.Resources.gg;
                            break;
                    }
                }
            }
        }

        //Parse through all files and enable the appropriate menu if it finds a matching filetype
        private void UpdateSaveAllMenu()
        {
            if (Thimble == null || Thimble.BundleFiles.Count == 0)
            {
                btnSaveAllFiles.Enabled = false;
                return;
            }

            toolStripSaveAllAudio.Visible = false;
            toolStripSaveAllImages.Visible = false;
            toolStripSaveAllText.Visible = false;
            toolStripSaveAllBnut.Visible = false;

            for (int i = 0; i < Thimble.BundleFiles.Count; i++)
            {
                if (Thimble.BundleFiles[i].FileType == BundleEntry.FileTypes.Sound)
                    toolStripSaveAllAudio.Visible = true;
                if (Thimble.BundleFiles[i].FileType == BundleEntry.FileTypes.Image)
                    toolStripSaveAllImages.Visible = true;
                if (Thimble.BundleFiles[i].FileType == BundleEntry.FileTypes.Text)
                    toolStripSaveAllText.Visible = true;
                if (Thimble.BundleFiles[i].FileType == BundleEntry.FileTypes.Bnut)
                    toolStripSaveAllBnut.Visible = true;
            }
        }

        //Search textbox changed
        private void cueTextBox1_TextChanged(object sender, EventArgs e)
        {
            objectListView1.ModelFilter = TextMatchFilter.Contains(objectListView1, cueTextBox1.Text);
        }

        //Item in contextMenuView clicked
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

        System.Diagnostics.Process ViewerProcess = null;

        private byte[] KtxToPng(Stream ktxStream)
        {
            BCnEncoder.Decoder.BcDecoder decoder = new BCnEncoder.Decoder.BcDecoder();
            var image = decoder.Decode(ktxStream);

            for (int i = 0; i < image.Width * image.Height; ++i)
            {
                int base_addr = 4 * i;
                //BGRA to 
                //RGBA
                var temp = image.data[base_addr];
                image.data[base_addr] = image.data[base_addr + 2];
                image.data[base_addr + 2] = temp;
            }

            GCHandle pinnedArray = GCHandle.Alloc(image.data, GCHandleType.Pinned);
            IntPtr pointer = pinnedArray.AddrOfPinnedObject();

            System.Drawing.Bitmap bmp = new Bitmap(image.Width, image.Height, image.Width * 4, System.Drawing.Imaging.PixelFormat.Format32bppArgb, pointer);
            var ms = new MemoryStream();
            bmp.Save(ms, ImageFormat.Png);

            pinnedArray.Free();
            return ms.ToArray();
        }

        private byte[] DecompressStream(MemoryStream ms)
        {
            ms.Position = 2;
            var Decompressed = new MemoryStream();

            using (var d = new DeflateStream(ms, CompressionMode.Decompress))
            {
                d.CopyTo(Decompressed);
                return Decompressed.ToArray();
            }
        }

        //Listview selection changed
        private void objectListView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Thimble == null || Thimble.BundleFiles.Count == 0 || objectListView1.SelectedIndex == -1)
                return;

            EnableDisableControlsContextDependant();

            int index = Thimble.BundleFiles.IndexOf((BundleEntry)objectListView1.SelectedObject);

            if (ViewerProcess != null && !ViewerProcess.HasExited) ViewerProcess.CloseMainWindow();

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

                        Stream imageStream = ms;

                        if (Thimble.BundleFiles[index].FileExtension == "ktxbz")
                        {
                            try
                            {
                                imageStream = new MemoryStream(KtxToPng(new MemoryStream(DecompressStream(ms))));
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show("Could not show file: " + ex.Message);
                                imageStream = null;
                            }
                        }

                        if (imageStream != null)
                        {
                            var image = Image.FromStream(imageStream);
                            pictureBoxPreview.Image = image;

                            //Set scaling mode depending on whether image is larger or smaller than the picturebox. From https://stackoverflow.com/questions/41188806/fit-image-to-picturebox-if-picturebox-is-smaller-than-picture
                            var imageSize = pictureBoxPreview.Image.Size;
                            var fitSize = pictureBoxPreview.ClientSize;
                            pictureBoxPreview.SizeMode = imageSize.Width > fitSize.Width || imageSize.Height > fitSize.Height ? PictureBoxSizeMode.Zoom : PictureBoxSizeMode.CenterImage;
                        }
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

                        if (Thimble.BundleFiles[index].FileExtension == "yack" && Thimble.FileVersion == BundleFileVersion.Version_RtMI)
                        {
                            try
                            {
                                ms.Position = 0;
                                YackDecompiler yack = new YackDecompiler(ms);
                                textBoxPreview.Lines = ("Note: This file was decompiled. Not all opcodes are understood!" + yack.ToString()).Split('\n');
                            }
                            catch (Exception ex)
                            {
                                textBoxPreview.Lines = new string[] { "could not decompile .yack:", ex.Message };
                            }
                        }
                        else
                        {
                            string[] tempArray = new string[] { }; //initialise an empty array of strings
                            if (Decoders.ExtractText(ms, out tempArray) == true)
                            {
                                textBoxPreview.Lines = tempArray;
                            }
                        }

                    }
                    break;

                case BundleEntry.FileTypes.GGDict:
                    panelText.BringToFront();
                    using (MemoryStream ms = new MemoryStream())
                    {
                        Thimble.SaveFileToStream(index, ms);

                        GGDict dict = new GGDict(ms, Thimble.FileVersion == BundleFileVersion.Version_RtMI);
                        string[] lines = dict.ToJsonString().Split('\n').Select(s => s.Trim('\r')).ToArray();
                        textBoxPreview.Lines = lines;
                    }
                    break;

                case BundleEntry.FileTypes.Bnut:
                    panelText.BringToFront();
                    using (MemoryStream ms = new MemoryStream())
                    {
                        Thimble.SaveFileToStream(index, ms);
                        //Decoders.DecodeBnut(ms);

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

        //Listview double clicked
        private void objectListView1_DoubleClick(object sender, EventArgs e)
        {
            if (Thimble == null || Thimble.BundleFiles.Count == 0 || objectListView1.SelectedIndex == -1)
                return;

            int index = Thimble.BundleFiles.IndexOf((BundleEntry)objectListView1.SelectedObject);

            if (Thimble.BundleFiles[index].FileType == BundleEntry.FileTypes.Sound)
            {
                btnSoundPlay.PerformClick();
            }
        }

    }
}