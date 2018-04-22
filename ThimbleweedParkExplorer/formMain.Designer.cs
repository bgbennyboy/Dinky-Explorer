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
            this.testToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.testToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.testToolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.objectListView1 = new BrightIdeasSoftware.ObjectListView();
            this.columnFilename = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.columnFileextension = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.columnSize = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.columnOffset = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.panelText = new System.Windows.Forms.Panel();
            this.textBoxPreview = new System.Windows.Forms.TextBox();
            this.panelAudio = new System.Windows.Forms.Panel();
            this.button1 = new System.Windows.Forms.Button();
            this.panelBlank = new System.Windows.Forms.Panel();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            this.panel1.SuspendLayout();
            this.contextMenuView.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.objectListView1)).BeginInit();
            this.panelText.SuspendLayout();
            this.panelAudio.SuspendLayout();
            this.panelBlank.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.Filter = "Ggpack files|*.ggpack1;*.ggpack2";
            // 
            // richTextBoxLog
            // 
            this.richTextBoxLog.BackColor = System.Drawing.Color.White;
            this.richTextBoxLog.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.richTextBoxLog.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.richTextBoxLog.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.richTextBoxLog.HideSelection = false;
            this.richTextBoxLog.Location = new System.Drawing.Point(0, 452);
            this.richTextBoxLog.Margin = new System.Windows.Forms.Padding(4);
            this.richTextBoxLog.Name = "richTextBoxLog";
            this.richTextBoxLog.ReadOnly = true;
            this.richTextBoxLog.ShowSelectionMargin = true;
            this.richTextBoxLog.Size = new System.Drawing.Size(894, 68);
            this.richTextBoxLog.TabIndex = 4;
            this.richTextBoxLog.Text = "Thimbleweed Park Explorer\nhttp://quickandeasysoftware.net";
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
            this.btnAbout.FlatAppearance.BorderSize = 0;
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
            // 
            // btnSaveAllFiles
            // 
            this.btnSaveAllFiles.AutoSize = true;
            this.btnSaveAllFiles.Dock = System.Windows.Forms.DockStyle.Left;
            this.btnSaveAllFiles.FlatAppearance.BorderSize = 0;
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
            this.btnSaveFile.FlatAppearance.BorderSize = 0;
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
            this.btnView.FlatAppearance.BorderSize = 0;
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
            this.btnOpen.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(164)))), ((int)(((byte)(206)))), ((int)(((byte)(249)))));
            this.btnOpen.FlatAppearance.BorderSize = 0;
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
            this.contextMenuView.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.testToolStripMenuItem,
            this.testToolStripMenuItem1,
            this.testToolStripMenuItem2,
            this.toolStripMenuItem1});
            this.contextMenuView.Name = "contextMenuView";
            this.contextMenuView.Size = new System.Drawing.Size(96, 76);
            this.contextMenuView.ItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.contextMenuView_ItemClicked);
            // 
            // testToolStripMenuItem
            // 
            this.testToolStripMenuItem.Name = "testToolStripMenuItem";
            this.testToolStripMenuItem.Size = new System.Drawing.Size(95, 22);
            this.testToolStripMenuItem.Tag = "";
            this.testToolStripMenuItem.Text = "Test";
            // 
            // testToolStripMenuItem1
            // 
            this.testToolStripMenuItem1.Name = "testToolStripMenuItem1";
            this.testToolStripMenuItem1.Size = new System.Drawing.Size(95, 22);
            this.testToolStripMenuItem1.Text = "test";
            // 
            // testToolStripMenuItem2
            // 
            this.testToolStripMenuItem2.Name = "testToolStripMenuItem2";
            this.testToolStripMenuItem2.Size = new System.Drawing.Size(95, 22);
            this.testToolStripMenuItem2.Text = "test";
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(92, 6);
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
            this.splitContainer1.Panel2.Controls.Add(this.panelText);
            this.splitContainer1.Panel2.Controls.Add(this.panelAudio);
            this.splitContainer1.Panel2.Controls.Add(this.panelBlank);
            this.splitContainer1.Panel2MinSize = 200;
            this.splitContainer1.Size = new System.Drawing.Size(894, 397);
            this.splitContainer1.SplitterDistance = 605;
            this.splitContainer1.SplitterWidth = 5;
            this.splitContainer1.TabIndex = 9;
            // 
            // objectListView1
            // 
            this.objectListView1.AllColumns.Add(this.columnFilename);
            this.objectListView1.AllColumns.Add(this.columnFileextension);
            this.objectListView1.AllColumns.Add(this.columnSize);
            this.objectListView1.AllColumns.Add(this.columnOffset);
            this.objectListView1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.objectListView1.CellEditUseWholeCell = false;
            this.objectListView1.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnFilename,
            this.columnFileextension,
            this.columnSize,
            this.columnOffset});
            this.objectListView1.Cursor = System.Windows.Forms.Cursors.Default;
            this.objectListView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.objectListView1.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.objectListView1.FullRowSelect = true;
            this.objectListView1.HideSelection = false;
            this.objectListView1.Location = new System.Drawing.Point(0, 0);
            this.objectListView1.Margin = new System.Windows.Forms.Padding(4);
            this.objectListView1.Name = "objectListView1";
            this.objectListView1.ShowGroups = false;
            this.objectListView1.Size = new System.Drawing.Size(603, 395);
            this.objectListView1.SmallImageList = this.imageList1;
            this.objectListView1.TabIndex = 6;
            this.objectListView1.UseCompatibleStateImageBehavior = false;
            this.objectListView1.UseFiltering = true;
            this.objectListView1.View = System.Windows.Forms.View.Details;
            this.objectListView1.SelectedIndexChanged += new System.EventHandler(this.objectListView1_SelectedIndexChanged);
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
            // columnSize
            // 
            this.columnSize.AspectName = "Size";
            this.columnSize.DisplayIndex = 3;
            this.columnSize.MaximumWidth = 200;
            this.columnSize.MinimumWidth = 80;
            this.columnSize.Text = "Size";
            this.columnSize.Width = 80;
            // 
            // columnOffset
            // 
            this.columnOffset.AspectName = "Offset";
            this.columnOffset.DisplayIndex = 2;
            this.columnOffset.MinimumWidth = 80;
            this.columnOffset.Text = "Offset";
            this.columnOffset.Width = 80;
            // 
            // imageList1
            // 
            this.imageList1.ColorDepth = System.Windows.Forms.ColorDepth.Depth32Bit;
            this.imageList1.ImageSize = new System.Drawing.Size(16, 16);
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // panelText
            // 
            this.panelText.Controls.Add(this.textBoxPreview);
            this.panelText.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelText.Location = new System.Drawing.Point(0, 0);
            this.panelText.Margin = new System.Windows.Forms.Padding(4);
            this.panelText.Name = "panelText";
            this.panelText.Size = new System.Drawing.Size(282, 395);
            this.panelText.TabIndex = 2;
            // 
            // textBoxPreview
            // 
            this.textBoxPreview.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBoxPreview.Location = new System.Drawing.Point(0, 0);
            this.textBoxPreview.Multiline = true;
            this.textBoxPreview.Name = "textBoxPreview";
            this.textBoxPreview.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.textBoxPreview.Size = new System.Drawing.Size(282, 395);
            this.textBoxPreview.TabIndex = 0;
            // 
            // panelAudio
            // 
            this.panelAudio.Controls.Add(this.button1);
            this.panelAudio.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelAudio.Location = new System.Drawing.Point(0, 0);
            this.panelAudio.Margin = new System.Windows.Forms.Padding(4);
            this.panelAudio.Name = "panelAudio";
            this.panelAudio.Size = new System.Drawing.Size(282, 395);
            this.panelAudio.TabIndex = 1;
            // 
            // button1
            // 
            this.button1.AutoSize = true;
            this.button1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.button1.FlatAppearance.BorderSize = 0;
            this.button1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button1.Image = global::ThimbleweedParkExplorer.Properties.Resources.Play_24x24;
            this.button1.Location = new System.Drawing.Point(61, 165);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(30, 30);
            this.button1.TabIndex = 0;
            this.button1.UseVisualStyleBackColor = true;
            // 
            // panelBlank
            // 
            this.panelBlank.Controls.Add(this.pictureBox1);
            this.panelBlank.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelBlank.Location = new System.Drawing.Point(0, 0);
            this.panelBlank.Margin = new System.Windows.Forms.Padding(4);
            this.panelBlank.Name = "panelBlank";
            this.panelBlank.Size = new System.Drawing.Size(282, 395);
            this.panelBlank.TabIndex = 0;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pictureBox1.Image = global::ThimbleweedParkExplorer.Properties.Resources.ExplorerLogo;
            this.pictureBox1.Location = new System.Drawing.Point(0, 0);
            this.pictureBox1.Margin = new System.Windows.Forms.Padding(4);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(282, 395);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            // 
            // formMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(894, 520);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.richTextBoxLog);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "formMain";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Thimbleweed Park Explorer";
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.contextMenuView.ResumeLayout(false);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.objectListView1)).EndInit();
            this.panelText.ResumeLayout(false);
            this.panelText.PerformLayout();
            this.panelAudio.ResumeLayout(false);
            this.panelAudio.PerformLayout();
            this.panelBlank.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
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
        private System.Windows.Forms.ToolStripMenuItem testToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem testToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem testToolStripMenuItem2;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
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
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Panel panelText;
        private System.Windows.Forms.TextBox textBoxPreview;
    }
}

