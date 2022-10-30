namespace ThimbleweedParkExplorer
{
    partial class formMain
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;



        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(formMain));
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.richTextBoxLog = new System.Windows.Forms.RichTextBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.cueTextBox1 = new CueTextBox();
            this.btnAbout = new System.Windows.Forms.Button();
            this.btnSaveAllFiles = new System.Windows.Forms.Button();
            this.btnSaveFile = new System.Windows.Forms.Button();
            this.btnView = new System.Windows.Forms.Button();
            this.btnOpen = new System.Windows.Forms.Button();
            this.contextMenuView = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.objectListView1 = new BrightIdeasSoftware.ObjectListView();
            this.columnFilename = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.columnFileextension = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.columnOffset = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.columnSize = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.DinkOptionsHost = new System.Windows.Forms.Integration.ElementHost();
            this.panelAudio = new System.Windows.Forms.Panel();
            this.labelSoundProgress = new System.Windows.Forms.Label();
            this.trackBarSound = new System.Windows.Forms.TrackBar();
            this.btnSoundStop = new System.Windows.Forms.Button();
            this.btnSoundPause = new System.Windows.Forms.Button();
            this.btnSoundPlay = new System.Windows.Forms.Button();
            this.bankAudioListHost = new System.Windows.Forms.Integration.ElementHost();
            this.panelBlank = new System.Windows.Forms.Panel();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.panelProgress = new System.Windows.Forms.Panel();
            this.pictureBoxProgress = new System.Windows.Forms.PictureBox();
            this.panelImage = new System.Windows.Forms.Panel();
            this.pictureBoxPreview = new System.Windows.Forms.PictureBox();
            this.panelText = new System.Windows.Forms.Panel();
            this.textBoxPreview = new System.Windows.Forms.TextBox();
            this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.contextMenuSaveAll = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.toolStripSaveAllRaw = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSaveAllVisible = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSaveAllAudio = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSaveAllImages = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSaveAllText = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSaveAllBnut = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenuSaveFile = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.toolStripSaveFileRaw = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSaveFileAsText = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSaveFileAsImage = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSaveFileAsAudio = new System.Windows.Forms.ToolStripMenuItem();
            this.progressBar1 = new ThimbleweedParkExplorer.CustomProgressBar();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.objectListView1)).BeginInit();
            this.panelAudio.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarSound)).BeginInit();
            this.panelBlank.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.panelProgress.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxProgress)).BeginInit();
            this.panelImage.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxPreview)).BeginInit();
            this.panelText.SuspendLayout();
            this.contextMenuSaveAll.SuspendLayout();
            this.contextMenuSaveFile.SuspendLayout();
            this.SuspendLayout();
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.Filter = "Ggpack files|*.ggpack?;*.ggpack??";
            // 
            // richTextBoxLog
            // 
            this.richTextBoxLog.BackColor = System.Drawing.Color.White;
            this.richTextBoxLog.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.richTextBoxLog.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.richTextBoxLog.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.richTextBoxLog.HideSelection = false;
            this.richTextBoxLog.Location = new System.Drawing.Point(0, 436);
            this.richTextBoxLog.Margin = new System.Windows.Forms.Padding(4);
            this.richTextBoxLog.Name = "richTextBoxLog";
            this.richTextBoxLog.ReadOnly = true;
            this.richTextBoxLog.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
            this.richTextBoxLog.ShowSelectionMargin = true;
            this.richTextBoxLog.Size = new System.Drawing.Size(894, 68);
            this.richTextBoxLog.TabIndex = 4;
            this.richTextBoxLog.Text = "Dinky Explorer\nhttp://quickandeasysoftware.net\n";
            // 
            // panel1
            // 
            this.panel1.AutoSize = true;
            this.panel1.Controls.Add(this.cueTextBox1);
            this.panel1.Controls.Add(this.btnAbout);
            this.panel1.Controls.Add(this.btnSaveAllFiles);
            this.panel1.Controls.Add(this.btnSaveFile);
            this.panel1.Controls.Add(this.btnView);
            this.panel1.Controls.Add(this.btnOpen);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Margin = new System.Windows.Forms.Padding(4);
            this.panel1.MinimumSize = new System.Drawing.Size(0, 55);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(894, 55);
            this.panel1.TabIndex = 0;
            // 
            // cueTextBox1
            // 
            this.cueTextBox1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.cueTextBox1.Cue = "Search";
            this.cueTextBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cueTextBox1.Font = new System.Drawing.Font("Segoe UI", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cueTextBox1.Location = new System.Drawing.Point(434, 0);
            this.cueTextBox1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 2);
            this.cueTextBox1.Name = "cueTextBox1";
            this.cueTextBox1.Size = new System.Drawing.Size(460, 43);
            this.cueTextBox1.TabIndex = 13;
            this.cueTextBox1.TextChanged += new System.EventHandler(this.cueTextBox1_TextChanged);
            // 
            // btnAbout
            // 
            this.btnAbout.AutoSize = true;
            this.btnAbout.Dock = System.Windows.Forms.DockStyle.Left;
            this.btnAbout.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(226)))), ((int)(((byte)(227)))), ((int)(((byte)(228)))));
            this.btnAbout.FlatAppearance.BorderSize = 0;
            this.btnAbout.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(201)))), ((int)(((byte)(224)))), ((int)(((byte)(247)))));
            this.btnAbout.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(232)))), ((int)(((byte)(239)))), ((int)(((byte)(247)))));
            this.btnAbout.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAbout.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnAbout.Image = global::ThimbleweedParkExplorer.Properties.Resources.info;
            this.btnAbout.Location = new System.Drawing.Point(346, 0);
            this.btnAbout.Margin = new System.Windows.Forms.Padding(4);
            this.btnAbout.Name = "btnAbout";
            this.btnAbout.Size = new System.Drawing.Size(88, 55);
            this.btnAbout.TabIndex = 12;
            this.btnAbout.Text = "About";
            this.btnAbout.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.btnAbout.UseVisualStyleBackColor = true;
            this.btnAbout.Click += new System.EventHandler(this.btnAbout_Click);
            // 
            // btnSaveAllFiles
            // 
            this.btnSaveAllFiles.AutoSize = true;
            this.btnSaveAllFiles.Dock = System.Windows.Forms.DockStyle.Left;
            this.btnSaveAllFiles.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(226)))), ((int)(((byte)(227)))), ((int)(((byte)(228)))));
            this.btnSaveAllFiles.FlatAppearance.BorderSize = 0;
            this.btnSaveAllFiles.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(201)))), ((int)(((byte)(224)))), ((int)(((byte)(247)))));
            this.btnSaveAllFiles.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(232)))), ((int)(((byte)(239)))), ((int)(((byte)(247)))));
            this.btnSaveAllFiles.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSaveAllFiles.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSaveAllFiles.Image = global::ThimbleweedParkExplorer.Properties.Resources.save_red_32;
            this.btnSaveAllFiles.Location = new System.Drawing.Point(264, 0);
            this.btnSaveAllFiles.Margin = new System.Windows.Forms.Padding(4);
            this.btnSaveAllFiles.Name = "btnSaveAllFiles";
            this.btnSaveAllFiles.Size = new System.Drawing.Size(82, 55);
            this.btnSaveAllFiles.TabIndex = 11;
            this.btnSaveAllFiles.Text = "Save All Files";
            this.btnSaveAllFiles.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.btnSaveAllFiles.UseVisualStyleBackColor = true;
            this.btnSaveAllFiles.Click += new System.EventHandler(this.btnSaveAllFiles_Click);
            // 
            // btnSaveFile
            // 
            this.btnSaveFile.AutoSize = true;
            this.btnSaveFile.Dock = System.Windows.Forms.DockStyle.Left;
            this.btnSaveFile.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(226)))), ((int)(((byte)(227)))), ((int)(((byte)(228)))));
            this.btnSaveFile.FlatAppearance.BorderSize = 0;
            this.btnSaveFile.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(201)))), ((int)(((byte)(224)))), ((int)(((byte)(247)))));
            this.btnSaveFile.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(232)))), ((int)(((byte)(239)))), ((int)(((byte)(247)))));
            this.btnSaveFile.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSaveFile.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSaveFile.Image = global::ThimbleweedParkExplorer.Properties.Resources.save_32;
            this.btnSaveFile.Location = new System.Drawing.Point(176, 0);
            this.btnSaveFile.Margin = new System.Windows.Forms.Padding(4);
            this.btnSaveFile.Name = "btnSaveFile";
            this.btnSaveFile.Size = new System.Drawing.Size(88, 55);
            this.btnSaveFile.TabIndex = 10;
            this.btnSaveFile.Text = "Save File";
            this.btnSaveFile.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.btnSaveFile.UseVisualStyleBackColor = true;
            this.btnSaveFile.Click += new System.EventHandler(this.btnSaveFile_Click);
            // 
            // btnView
            // 
            this.btnView.AutoSize = true;
            this.btnView.Dock = System.Windows.Forms.DockStyle.Left;
            this.btnView.Enabled = false;
            this.btnView.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(226)))), ((int)(((byte)(227)))), ((int)(((byte)(228)))));
            this.btnView.FlatAppearance.BorderSize = 0;
            this.btnView.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(201)))), ((int)(((byte)(224)))), ((int)(((byte)(247)))));
            this.btnView.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(232)))), ((int)(((byte)(239)))), ((int)(((byte)(247)))));
            this.btnView.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnView.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnView.Image = global::ThimbleweedParkExplorer.Properties.Resources.views_32;
            this.btnView.Location = new System.Drawing.Point(88, 0);
            this.btnView.Margin = new System.Windows.Forms.Padding(4);
            this.btnView.Name = "btnView";
            this.btnView.Size = new System.Drawing.Size(88, 55);
            this.btnView.TabIndex = 3;
            this.btnView.Text = "View";
            this.btnView.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.btnView.UseVisualStyleBackColor = true;
            this.btnView.Click += new System.EventHandler(this.btnView_Click);
            // 
            // btnOpen
            // 
            this.btnOpen.AutoSize = true;
            this.btnOpen.Dock = System.Windows.Forms.DockStyle.Left;
            this.btnOpen.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(226)))), ((int)(((byte)(227)))), ((int)(((byte)(228)))));
            this.btnOpen.FlatAppearance.BorderSize = 0;
            this.btnOpen.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(201)))), ((int)(((byte)(224)))), ((int)(((byte)(247)))));
            this.btnOpen.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(232)))), ((int)(((byte)(239)))), ((int)(((byte)(247)))));
            this.btnOpen.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnOpen.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnOpen.Image = global::ThimbleweedParkExplorer.Properties.Resources.Open_32;
            this.btnOpen.Location = new System.Drawing.Point(0, 0);
            this.btnOpen.Margin = new System.Windows.Forms.Padding(4);
            this.btnOpen.Name = "btnOpen";
            this.btnOpen.Size = new System.Drawing.Size(88, 55);
            this.btnOpen.TabIndex = 2;
            this.btnOpen.Text = "Open";
            this.btnOpen.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.btnOpen.UseVisualStyleBackColor = true;
            this.btnOpen.Click += new System.EventHandler(this.btnOpen_Click);
            // 
            // contextMenuView
            // 
            this.contextMenuView.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.contextMenuView.Name = "contextMenuView";
            this.contextMenuView.Size = new System.Drawing.Size(61, 4);
            this.contextMenuView.ItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.contextMenuView_ItemClicked);
            // 
            // splitContainer1
            // 
            this.splitContainer1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 55);
            this.splitContainer1.Margin = new System.Windows.Forms.Padding(4);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.objectListView1);
            this.splitContainer1.Panel1MinSize = 400;
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.DinkOptionsHost);
            this.splitContainer1.Panel2.Controls.Add(this.panelAudio);
            this.splitContainer1.Panel2.Controls.Add(this.bankAudioListHost);
            this.splitContainer1.Panel2.Controls.Add(this.panelBlank);
            this.splitContainer1.Panel2.Controls.Add(this.panelProgress);
            this.splitContainer1.Panel2.Controls.Add(this.panelImage);
            this.splitContainer1.Panel2.Controls.Add(this.panelText);
            this.splitContainer1.Panel2MinSize = 200;
            this.splitContainer1.Size = new System.Drawing.Size(894, 381);
            this.splitContainer1.SplitterDistance = 605;
            this.splitContainer1.SplitterWidth = 5;
            this.splitContainer1.TabIndex = 9;
            // 
            // objectListView1
            // 
            this.objectListView1.AllColumns.Add(this.columnFilename);
            this.objectListView1.AllColumns.Add(this.columnFileextension);
            this.objectListView1.AllColumns.Add(this.columnOffset);
            this.objectListView1.AllColumns.Add(this.columnSize);
            this.objectListView1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.objectListView1.CellEditUseWholeCell = false;
            this.objectListView1.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnFilename,
            this.columnFileextension,
            this.columnOffset,
            this.columnSize});
            this.objectListView1.Cursor = System.Windows.Forms.Cursors.Default;
            this.objectListView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.objectListView1.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.objectListView1.FullRowSelect = true;
            this.objectListView1.HideSelection = false;
            this.objectListView1.Location = new System.Drawing.Point(0, 0);
            this.objectListView1.Margin = new System.Windows.Forms.Padding(4);
            this.objectListView1.Name = "objectListView1";
            this.objectListView1.OverlayImage.Transparency = 180;
            this.objectListView1.ShowGroups = false;
            this.objectListView1.Size = new System.Drawing.Size(603, 379);
            this.objectListView1.SmallImageList = this.imageList1;
            this.objectListView1.TabIndex = 6;
            this.objectListView1.UseCompatibleStateImageBehavior = false;
            this.objectListView1.UseFiltering = true;
            this.objectListView1.View = System.Windows.Forms.View.Details;
            this.objectListView1.SelectedIndexChanged += new System.EventHandler(this.objectListView1_SelectedIndexChanged);
            this.objectListView1.DoubleClick += new System.EventHandler(this.objectListView1_DoubleClick);
            // 
            // columnFilename
            // 
            this.columnFilename.AspectName = "FileName";
            this.columnFilename.Groupable = false;
            this.columnFilename.MinimumWidth = 80;
            this.columnFilename.Text = "Name";
            this.columnFilename.Width = 100;
            // 
            // columnFileextension
            // 
            this.columnFileextension.AspectName = "FileExtension";
            this.columnFileextension.MinimumWidth = 120;
            this.columnFileextension.Text = "Type";
            this.columnFileextension.Width = 120;
            // 
            // columnOffset
            // 
            this.columnOffset.AspectName = "Offset";
            this.columnOffset.MinimumWidth = 50;
            this.columnOffset.Text = "Offset";
            this.columnOffset.Width = 80;
            // 
            // columnSize
            // 
            this.columnSize.AspectName = "Size";
            this.columnSize.FillsFreeSpace = true;
            this.columnSize.MinimumWidth = 50;
            this.columnSize.Text = "Size";
            this.columnSize.Width = 80;
            // 
            // imageList1
            // 
            this.imageList1.ColorDepth = System.Windows.Forms.ColorDepth.Depth32Bit;
            this.imageList1.ImageSize = new System.Drawing.Size(16, 16);
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // DinkOptionsHost
            // 
            this.DinkOptionsHost.Dock = System.Windows.Forms.DockStyle.Fill;
            this.DinkOptionsHost.Location = new System.Drawing.Point(0, 0);
            this.DinkOptionsHost.Margin = new System.Windows.Forms.Padding(0);
            this.DinkOptionsHost.Name = "DinkOptionsHost";
            this.DinkOptionsHost.Size = new System.Drawing.Size(282, 379);
            this.DinkOptionsHost.TabIndex = 15;
            this.DinkOptionsHost.Text = "elementHost1";
            this.DinkOptionsHost.Child = null;
            // 
            // panelAudio
            // 
            this.panelAudio.Controls.Add(this.labelSoundProgress);
            this.panelAudio.Controls.Add(this.trackBarSound);
            this.panelAudio.Controls.Add(this.btnSoundStop);
            this.panelAudio.Controls.Add(this.btnSoundPause);
            this.panelAudio.Controls.Add(this.btnSoundPlay);
            this.panelAudio.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelAudio.Location = new System.Drawing.Point(0, 0);
            this.panelAudio.Margin = new System.Windows.Forms.Padding(4);
            this.panelAudio.Name = "panelAudio";
            this.panelAudio.Size = new System.Drawing.Size(282, 379);
            this.panelAudio.TabIndex = 1;
            // 
            // labelSoundProgress
            // 
            this.labelSoundProgress.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.labelSoundProgress.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelSoundProgress.Location = new System.Drawing.Point(0, 112);
            this.labelSoundProgress.Name = "labelSoundProgress";
            this.labelSoundProgress.Size = new System.Drawing.Size(279, 31);
            this.labelSoundProgress.TabIndex = 14;
            this.labelSoundProgress.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // trackBarSound
            // 
            this.trackBarSound.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.trackBarSound.AutoSize = false;
            this.trackBarSound.Location = new System.Drawing.Point(6, 151);
            this.trackBarSound.Maximum = 100;
            this.trackBarSound.Name = "trackBarSound";
            this.trackBarSound.Size = new System.Drawing.Size(276, 33);
            this.trackBarSound.TabIndex = 13;
            this.trackBarSound.TickStyle = System.Windows.Forms.TickStyle.None;
            this.trackBarSound.Scroll += new System.EventHandler(this.trackBarSound_Scroll);
            // 
            // btnSoundStop
            // 
            this.btnSoundStop.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.btnSoundStop.AutoSize = true;
            this.btnSoundStop.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnSoundStop.FlatAppearance.BorderSize = 0;
            this.btnSoundStop.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSoundStop.Image = global::ThimbleweedParkExplorer.Properties.Resources.stop;
            this.btnSoundStop.Location = new System.Drawing.Point(171, 181);
            this.btnSoundStop.Name = "btnSoundStop";
            this.btnSoundStop.Size = new System.Drawing.Size(30, 30);
            this.btnSoundStop.TabIndex = 12;
            this.btnSoundStop.UseVisualStyleBackColor = true;
            this.btnSoundStop.Click += new System.EventHandler(this.btnSoundStop_Click);
            // 
            // btnSoundPause
            // 
            this.btnSoundPause.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.btnSoundPause.AutoSize = true;
            this.btnSoundPause.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnSoundPause.FlatAppearance.BorderSize = 0;
            this.btnSoundPause.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSoundPause.Image = global::ThimbleweedParkExplorer.Properties.Resources.pause;
            this.btnSoundPause.Location = new System.Drawing.Point(119, 181);
            this.btnSoundPause.Name = "btnSoundPause";
            this.btnSoundPause.Size = new System.Drawing.Size(30, 30);
            this.btnSoundPause.TabIndex = 11;
            this.btnSoundPause.UseVisualStyleBackColor = true;
            this.btnSoundPause.Click += new System.EventHandler(this.btnSoundPause_Click);
            // 
            // btnSoundPlay
            // 
            this.btnSoundPlay.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.btnSoundPlay.AutoSize = true;
            this.btnSoundPlay.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnSoundPlay.FlatAppearance.BorderSize = 0;
            this.btnSoundPlay.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSoundPlay.Image = global::ThimbleweedParkExplorer.Properties.Resources.Play_24x24;
            this.btnSoundPlay.Location = new System.Drawing.Point(69, 181);
            this.btnSoundPlay.Name = "btnSoundPlay";
            this.btnSoundPlay.Size = new System.Drawing.Size(30, 30);
            this.btnSoundPlay.TabIndex = 0;
            this.btnSoundPlay.UseVisualStyleBackColor = true;
            this.btnSoundPlay.Click += new System.EventHandler(this.btnSoundPlay_Click);
            // 
            // bankAudioListHost
            // 
            this.bankAudioListHost.Dock = System.Windows.Forms.DockStyle.Fill;
            this.bankAudioListHost.Location = new System.Drawing.Point(0, 0);
            this.bankAudioListHost.Name = "bankAudioListHost";
            this.bankAudioListHost.Size = new System.Drawing.Size(282, 379);
            this.bankAudioListHost.TabIndex = 15;
            this.bankAudioListHost.Text = "elementHost1";
            this.bankAudioListHost.Child = null;
            // 
            // panelBlank
            // 
            this.panelBlank.Controls.Add(this.pictureBox1);
            this.panelBlank.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelBlank.Location = new System.Drawing.Point(0, 0);
            this.panelBlank.Margin = new System.Windows.Forms.Padding(4);
            this.panelBlank.Name = "panelBlank";
            this.panelBlank.Size = new System.Drawing.Size(282, 379);
            this.panelBlank.TabIndex = 0;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
            this.pictureBox1.InitialImage = ((System.Drawing.Image)(resources.GetObject("pictureBox1.InitialImage")));
            this.pictureBox1.Location = new System.Drawing.Point(0, 0);
            this.pictureBox1.Margin = new System.Windows.Forms.Padding(4);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(282, 379);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            // 
            // panelProgress
            // 
            this.panelProgress.Controls.Add(this.pictureBoxProgress);
            this.panelProgress.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelProgress.Location = new System.Drawing.Point(0, 0);
            this.panelProgress.Margin = new System.Windows.Forms.Padding(4);
            this.panelProgress.Name = "panelProgress";
            this.panelProgress.Size = new System.Drawing.Size(282, 379);
            this.panelProgress.TabIndex = 4;
            // 
            // pictureBoxProgress
            // 
            this.pictureBoxProgress.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pictureBoxProgress.Image = ((System.Drawing.Image)(resources.GetObject("pictureBoxProgress.Image")));
            this.pictureBoxProgress.Location = new System.Drawing.Point(0, 0);
            this.pictureBoxProgress.Margin = new System.Windows.Forms.Padding(4);
            this.pictureBoxProgress.Name = "pictureBoxProgress";
            this.pictureBoxProgress.Size = new System.Drawing.Size(282, 379);
            this.pictureBoxProgress.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.pictureBoxProgress.TabIndex = 0;
            this.pictureBoxProgress.TabStop = false;
            // 
            // panelImage
            // 
            this.panelImage.Controls.Add(this.pictureBoxPreview);
            this.panelImage.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelImage.Location = new System.Drawing.Point(0, 0);
            this.panelImage.Margin = new System.Windows.Forms.Padding(4);
            this.panelImage.Name = "panelImage";
            this.panelImage.Size = new System.Drawing.Size(282, 379);
            this.panelImage.TabIndex = 3;
            // 
            // pictureBoxPreview
            // 
            this.pictureBoxPreview.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pictureBoxPreview.Location = new System.Drawing.Point(0, 0);
            this.pictureBoxPreview.Name = "pictureBoxPreview";
            this.pictureBoxPreview.Size = new System.Drawing.Size(282, 379);
            this.pictureBoxPreview.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.pictureBoxPreview.TabIndex = 0;
            this.pictureBoxPreview.TabStop = false;
            // 
            // panelText
            // 
            this.panelText.Controls.Add(this.textBoxPreview);
            this.panelText.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelText.Location = new System.Drawing.Point(0, 0);
            this.panelText.Margin = new System.Windows.Forms.Padding(4);
            this.panelText.Name = "panelText";
            this.panelText.Size = new System.Drawing.Size(282, 379);
            this.panelText.TabIndex = 2;
            // 
            // textBoxPreview
            // 
            this.textBoxPreview.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBoxPreview.Location = new System.Drawing.Point(0, 0);
            this.textBoxPreview.Multiline = true;
            this.textBoxPreview.Name = "textBoxPreview";
            this.textBoxPreview.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.textBoxPreview.Size = new System.Drawing.Size(282, 379);
            this.textBoxPreview.TabIndex = 0;
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Interval = 500;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // contextMenuSaveAll
            // 
            this.contextMenuSaveAll.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.contextMenuSaveAll.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripSaveAllRaw,
            this.toolStripSaveAllVisible,
            this.toolStripSaveAllAudio,
            this.toolStripSaveAllImages,
            this.toolStripSaveAllText,
            this.toolStripSaveAllBnut});
            this.contextMenuSaveAll.Name = "contextMenuView";
            this.contextMenuSaveAll.Size = new System.Drawing.Size(239, 136);
            // 
            // toolStripSaveAllRaw
            // 
            this.toolStripSaveAllRaw.Image = global::ThimbleweedParkExplorer.Properties.Resources.small_savefile;
            this.toolStripSaveAllRaw.Name = "toolStripSaveAllRaw";
            this.toolStripSaveAllRaw.Size = new System.Drawing.Size(238, 22);
            this.toolStripSaveAllRaw.Text = "Save all files (raw dump)";
            this.toolStripSaveAllRaw.Click += new System.EventHandler(this.SaveAllHandler);
            // 
            // toolStripSaveAllVisible
            // 
            this.toolStripSaveAllVisible.Image = global::ThimbleweedParkExplorer.Properties.Resources.small_savefile;
            this.toolStripSaveAllVisible.Name = "toolStripSaveAllVisible";
            this.toolStripSaveAllVisible.Size = new System.Drawing.Size(238, 22);
            this.toolStripSaveAllVisible.Text = "Save all visible files (raw dump)";
            this.toolStripSaveAllVisible.Click += new System.EventHandler(this.SaveAllHandler);
            // 
            // toolStripSaveAllAudio
            // 
            this.toolStripSaveAllAudio.Image = global::ThimbleweedParkExplorer.Properties.Resources.small_audio;
            this.toolStripSaveAllAudio.Name = "toolStripSaveAllAudio";
            this.toolStripSaveAllAudio.Size = new System.Drawing.Size(238, 22);
            this.toolStripSaveAllAudio.Text = "Save all audio";
            this.toolStripSaveAllAudio.Click += new System.EventHandler(this.SaveAllHandler);
            // 
            // toolStripSaveAllImages
            // 
            this.toolStripSaveAllImages.Image = global::ThimbleweedParkExplorer.Properties.Resources.small_image;
            this.toolStripSaveAllImages.Name = "toolStripSaveAllImages";
            this.toolStripSaveAllImages.Size = new System.Drawing.Size(238, 22);
            this.toolStripSaveAllImages.Text = "Save all images";
            this.toolStripSaveAllImages.Click += new System.EventHandler(this.SaveAllHandler);
            // 
            // toolStripSaveAllText
            // 
            this.toolStripSaveAllText.Image = global::ThimbleweedParkExplorer.Properties.Resources.small_text;
            this.toolStripSaveAllText.Name = "toolStripSaveAllText";
            this.toolStripSaveAllText.Size = new System.Drawing.Size(238, 22);
            this.toolStripSaveAllText.Tag = "";
            this.toolStripSaveAllText.Text = "Save all text";
            this.toolStripSaveAllText.Click += new System.EventHandler(this.SaveAllHandler);
            // 
            // toolStripSaveAllBnut
            // 
            this.toolStripSaveAllBnut.Image = global::ThimbleweedParkExplorer.Properties.Resources.small_code;
            this.toolStripSaveAllBnut.Name = "toolStripSaveAllBnut";
            this.toolStripSaveAllBnut.Size = new System.Drawing.Size(238, 22);
            this.toolStripSaveAllBnut.Tag = "";
            this.toolStripSaveAllBnut.Text = "Save all bnut scripts";
            this.toolStripSaveAllBnut.Click += new System.EventHandler(this.SaveAllHandler);
            // 
            // contextMenuSaveFile
            // 
            this.contextMenuSaveFile.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.contextMenuSaveFile.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripSaveFileRaw,
            this.toolStripSaveFileAsText,
            this.toolStripSaveFileAsImage,
            this.toolStripSaveFileAsAudio});
            this.contextMenuSaveFile.Name = "contextMenuView";
            this.contextMenuSaveFile.Size = new System.Drawing.Size(164, 92);
            // 
            // toolStripSaveFileRaw
            // 
            this.toolStripSaveFileRaw.Image = global::ThimbleweedParkExplorer.Properties.Resources.small_savefile;
            this.toolStripSaveFileRaw.Name = "toolStripSaveFileRaw";
            this.toolStripSaveFileRaw.Size = new System.Drawing.Size(163, 22);
            this.toolStripSaveFileRaw.Text = "As is (raw dump)";
            this.toolStripSaveFileRaw.Click += new System.EventHandler(this.SaveFileAsHandler);
            // 
            // toolStripSaveFileAsText
            // 
            this.toolStripSaveFileAsText.Image = global::ThimbleweedParkExplorer.Properties.Resources.small_text;
            this.toolStripSaveFileAsText.Name = "toolStripSaveFileAsText";
            this.toolStripSaveFileAsText.Size = new System.Drawing.Size(163, 22);
            this.toolStripSaveFileAsText.Text = "As text";
            this.toolStripSaveFileAsText.Click += new System.EventHandler(this.SaveFileAsHandler);
            // 
            // toolStripSaveFileAsImage
            // 
            this.toolStripSaveFileAsImage.Image = global::ThimbleweedParkExplorer.Properties.Resources.small_image;
            this.toolStripSaveFileAsImage.Name = "toolStripSaveFileAsImage";
            this.toolStripSaveFileAsImage.Size = new System.Drawing.Size(163, 22);
            this.toolStripSaveFileAsImage.Text = "As Image";
            this.toolStripSaveFileAsImage.Click += new System.EventHandler(this.SaveFileAsHandler);
            // 
            // toolStripSaveFileAsAudio
            // 
            this.toolStripSaveFileAsAudio.Image = global::ThimbleweedParkExplorer.Properties.Resources.small_audio;
            this.toolStripSaveFileAsAudio.Name = "toolStripSaveFileAsAudio";
            this.toolStripSaveFileAsAudio.Size = new System.Drawing.Size(163, 22);
            this.toolStripSaveFileAsAudio.Text = "As audio";
            this.toolStripSaveFileAsAudio.Click += new System.EventHandler(this.SaveFileAsHandler);
            // 
            // progressBar1
            // 
            this.progressBar1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.progressBar1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(184)))), ((int)(((byte)(39)))));
            this.progressBar1.Location = new System.Drawing.Point(0, 504);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(894, 19);
            this.progressBar1.TabIndex = 10;
            this.progressBar1.Visible = false;
            // 
            // formMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.AutoSize = true;
            this.ClientSize = new System.Drawing.Size(894, 523);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.richTextBoxLog);
            this.Controls.Add(this.progressBar1);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Margin = new System.Windows.Forms.Padding(4);
            this.MinimumSize = new System.Drawing.Size(470, 400);
            this.Name = "formMain";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Dinky Explorer";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.formMain_FormClosing);
            this.DragDrop += new System.Windows.Forms.DragEventHandler(this.formMain_DragDrop);
            this.DragEnter += new System.Windows.Forms.DragEventHandler(this.objectListView1_DragEnter);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.objectListView1)).EndInit();
            this.panelAudio.ResumeLayout(false);
            this.panelAudio.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarSound)).EndInit();
            this.panelBlank.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.panelProgress.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxProgress)).EndInit();
            this.panelImage.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxPreview)).EndInit();
            this.panelText.ResumeLayout(false);
            this.panelText.PerformLayout();
            this.contextMenuSaveAll.ResumeLayout(false);
            this.contextMenuSaveFile.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.RichTextBox richTextBoxLog;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button btnOpen;
        private System.Windows.Forms.Button btnView;
        private System.Windows.Forms.ContextMenuStrip contextMenuView;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private BrightIdeasSoftware.ObjectListView objectListView1;
        private BrightIdeasSoftware.OLVColumn columnFilename;
        private BrightIdeasSoftware.OLVColumn columnFileextension;
        private BrightIdeasSoftware.OLVColumn columnOffset;
        private BrightIdeasSoftware.OLVColumn columnSize;
        private System.Windows.Forms.Button btnSaveFile;
        private System.Windows.Forms.Button btnSaveAllFiles;
        private System.Windows.Forms.Button btnAbout;
        private System.Windows.Forms.ImageList imageList1;
        private CueTextBox cueTextBox1;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Panel panelBlank;
        private System.Windows.Forms.SaveFileDialog saveFileDialog1;
        private System.Windows.Forms.Panel panelAudio;
        private System.Windows.Forms.Button btnSoundPlay;
        private System.Windows.Forms.Panel panelText;
        private System.Windows.Forms.TextBox textBoxPreview;
        private CustomProgressBar progressBar1;
        private System.Windows.Forms.Panel panelImage;
        private System.Windows.Forms.PictureBox pictureBoxPreview;
        private System.Windows.Forms.Panel panelProgress;
        private System.Windows.Forms.PictureBox pictureBoxProgress;
        private System.Windows.Forms.Button btnSoundPause;
        private System.Windows.Forms.TrackBar trackBarSound;
        private System.Windows.Forms.Button btnSoundStop;
        private System.Windows.Forms.Label labelSoundProgress;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.ContextMenuStrip contextMenuSaveAll;
        private System.Windows.Forms.ToolStripMenuItem toolStripSaveAllRaw;
        private System.Windows.Forms.ToolStripMenuItem toolStripSaveAllVisible;
        private System.Windows.Forms.ToolStripMenuItem toolStripSaveAllText;
        private System.Windows.Forms.ToolStripMenuItem toolStripSaveAllImages;
        private System.Windows.Forms.ToolStripMenuItem toolStripSaveAllAudio;
        private System.Windows.Forms.ContextMenuStrip contextMenuSaveFile;
        private System.Windows.Forms.ToolStripMenuItem toolStripSaveFileRaw;
        private System.Windows.Forms.ToolStripMenuItem toolStripSaveFileAsText;
        private System.Windows.Forms.ToolStripMenuItem toolStripSaveFileAsImage;
        private System.Windows.Forms.ToolStripMenuItem toolStripSaveFileAsAudio;
        private System.Windows.Forms.ToolStripMenuItem toolStripSaveAllBnut;
        private System.Windows.Forms.Integration.ElementHost bankAudioListHost;
        private System.Windows.Forms.Integration.ElementHost DinkOptionsHost;
    }
}

