namespace ThimbleweedParkExplorer
{
    partial class formAbout
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(formAbout));
            this.pictureBoxTop = new System.Windows.Forms.PictureBox();
            this.pictureBoxBottom = new System.Windows.Forms.PictureBox();
            this.labelAboutText = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxTop)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxBottom)).BeginInit();
            this.SuspendLayout();
            // 
            // pictureBoxTop
            // 
            this.pictureBoxTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.pictureBoxTop.Image = ((System.Drawing.Image)(resources.GetObject("pictureBoxTop.Image")));
            this.pictureBoxTop.Location = new System.Drawing.Point(0, 0);
            this.pictureBoxTop.Name = "pictureBoxTop";
            this.pictureBoxTop.Size = new System.Drawing.Size(397, 141);
            this.pictureBoxTop.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.pictureBoxTop.TabIndex = 0;
            this.pictureBoxTop.TabStop = false;
            this.pictureBoxTop.Click += new System.EventHandler(this.pictureBoxTop_Click);
            // 
            // pictureBoxBottom
            // 
            this.pictureBoxBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pictureBoxBottom.Image = ((System.Drawing.Image)(resources.GetObject("pictureBoxBottom.Image")));
            this.pictureBoxBottom.Location = new System.Drawing.Point(0, 342);
            this.pictureBoxBottom.Name = "pictureBoxBottom";
            this.pictureBoxBottom.Size = new System.Drawing.Size(397, 81);
            this.pictureBoxBottom.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.pictureBoxBottom.TabIndex = 1;
            this.pictureBoxBottom.TabStop = false;
            this.pictureBoxBottom.Click += new System.EventHandler(this.pictureBoxBottom_Click);
            // 
            // labelAboutText
            // 
            this.labelAboutText.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelAboutText.ForeColor = System.Drawing.Color.White;
            this.labelAboutText.Location = new System.Drawing.Point(0, 141);
            this.labelAboutText.Name = "labelAboutText";
            this.labelAboutText.Size = new System.Drawing.Size(397, 201);
            this.labelAboutText.TabIndex = 2;
            this.labelAboutText.Text = "Dinky Explorer\r\n\r\nVersion\r\n\r\nBy Bennyboy and Jan Frederick\r\n\r\nhttp://quickandeasy" +
    "software.net\r\n";
            this.labelAboutText.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.labelAboutText.Click += new System.EventHandler(this.labelAboutText_Click);
            // 
            // formAbout
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(158)))), ((int)(((byte)(208)))), ((int)(((byte)(99)))));
            this.ClientSize = new System.Drawing.Size(397, 423);
            this.Controls.Add(this.labelAboutText);
            this.Controls.Add(this.pictureBoxBottom);
            this.Controls.Add(this.pictureBoxTop);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "formAbout";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "formAbout";
            this.Load += new System.EventHandler(this.formAbout_Load);
            this.Click += new System.EventHandler(this.formAbout_Click);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxTop)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxBottom)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBoxTop;
        private System.Windows.Forms.PictureBox pictureBoxBottom;
        private System.Windows.Forms.Label labelAboutText;
    }
}