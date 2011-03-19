namespace CS8803AGAEditor
{
    partial class SpriteSheetPreviewer
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
            this.m_picturePanel = new System.Windows.Forms.Panel();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.m_tbsCursorPos = new System.Windows.Forms.ToolStripStatusLabel();
            this.m_tbsCurRect = new System.Windows.Forms.ToolStripStatusLabel();
            this.m_bColor = new System.Windows.Forms.Button();
            this.m_tbScale = new System.Windows.Forms.TrackBar();
            this.m_bUndo = new System.Windows.Forms.Button();
            this.m_pictureBox = new CS8803AGAEditor.BetterPictureBox();
            this.m_bExport = new System.Windows.Forms.Button();
            this.m_picturePanel.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.m_tbScale)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.m_pictureBox)).BeginInit();
            this.SuspendLayout();
            // 
            // m_picturePanel
            // 
            this.m_picturePanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.m_picturePanel.AutoScroll = true;
            this.m_picturePanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.m_picturePanel.Controls.Add(this.m_pictureBox);
            this.m_picturePanel.Location = new System.Drawing.Point(13, 43);
            this.m_picturePanel.Name = "m_picturePanel";
            this.m_picturePanel.Size = new System.Drawing.Size(259, 187);
            this.m_picturePanel.TabIndex = 3;
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.m_tbsCursorPos,
            this.m_tbsCurRect});
            this.statusStrip1.Location = new System.Drawing.Point(0, 240);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(284, 22);
            this.statusStrip1.TabIndex = 4;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // m_tbsCursorPos
            // 
            this.m_tbsCursorPos.Name = "m_tbsCursorPos";
            this.m_tbsCursorPos.Size = new System.Drawing.Size(47, 17);
            this.m_tbsCursorPos.Text = "Pos: 0,0";
            // 
            // m_tbsCurRect
            // 
            this.m_tbsCurRect.Name = "m_tbsCurRect";
            this.m_tbsCurRect.Size = new System.Drawing.Size(0, 17);
            // 
            // m_bColor
            // 
            this.m_bColor.BackColor = System.Drawing.Color.LimeGreen;
            this.m_bColor.Location = new System.Drawing.Point(12, 12);
            this.m_bColor.Name = "m_bColor";
            this.m_bColor.Size = new System.Drawing.Size(30, 23);
            this.m_bColor.TabIndex = 5;
            this.m_bColor.UseVisualStyleBackColor = false;
            this.m_bColor.Click += new System.EventHandler(this.m_bColor_Click);
            // 
            // m_tbScale
            // 
            this.m_tbScale.AutoSize = false;
            this.m_tbScale.LargeChange = 1;
            this.m_tbScale.Location = new System.Drawing.Point(48, 13);
            this.m_tbScale.Maximum = 4;
            this.m_tbScale.Minimum = -3;
            this.m_tbScale.Name = "m_tbScale";
            this.m_tbScale.Size = new System.Drawing.Size(105, 25);
            this.m_tbScale.TabIndex = 6;
            this.m_tbScale.Scroll += new System.EventHandler(this.m_tbScale_Scroll);
            // 
            // m_bUndo
            // 
            this.m_bUndo.Location = new System.Drawing.Point(159, 11);
            this.m_bUndo.Name = "m_bUndo";
            this.m_bUndo.Size = new System.Drawing.Size(41, 23);
            this.m_bUndo.TabIndex = 7;
            this.m_bUndo.Text = "Undo";
            this.m_bUndo.UseVisualStyleBackColor = true;
            this.m_bUndo.Click += new System.EventHandler(this.m_bUndo_Click);
            // 
            // m_pictureBox
            // 
            this.m_pictureBox.BackColor = System.Drawing.Color.White;
            this.m_pictureBox.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.m_pictureBox.Image = null;
            this.m_pictureBox.ImageScale = 1F;
            this.m_pictureBox.Location = new System.Drawing.Point(0, 0);
            this.m_pictureBox.Name = "m_pictureBox";
            this.m_pictureBox.Size = new System.Drawing.Size(50, 50);
            this.m_pictureBox.TabIndex = 0;
            this.m_pictureBox.TabStop = false;
            this.m_pictureBox.TemporarySpriteBox = null;
            this.m_pictureBox.MouseMove += new System.Windows.Forms.MouseEventHandler(this.m_pictureBox_MouseMove);
            this.m_pictureBox.MouseDown += new System.Windows.Forms.MouseEventHandler(this.m_pictureBox_MouseDown);
            this.m_pictureBox.MouseUp += new System.Windows.Forms.MouseEventHandler(this.m_pictureBox_MouseUp);
            // 
            // m_bExport
            // 
            this.m_bExport.Location = new System.Drawing.Point(208, 11);
            this.m_bExport.Name = "m_bExport";
            this.m_bExport.Size = new System.Drawing.Size(64, 23);
            this.m_bExport.TabIndex = 8;
            this.m_bExport.Text = "Export";
            this.m_bExport.UseVisualStyleBackColor = true;
            this.m_bExport.Click += new System.EventHandler(this.m_bExport_Click);
            // 
            // SpriteSheetPreviewer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 262);
            this.Controls.Add(this.m_bExport);
            this.Controls.Add(this.m_bUndo);
            this.Controls.Add(this.m_tbScale);
            this.Controls.Add(this.m_bColor);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.m_picturePanel);
            this.Name = "SpriteSheetPreviewer";
            this.Text = "SpriteSheetPreviewer";
            this.m_picturePanel.ResumeLayout(false);
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.m_tbScale)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.m_pictureBox)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel m_picturePanel;
        private BetterPictureBox m_pictureBox;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.Button m_bColor;
        private System.Windows.Forms.ToolStripStatusLabel m_tbsCursorPos;
        private System.Windows.Forms.TrackBar m_tbScale;
        private System.Windows.Forms.Button m_bUndo;
        private System.Windows.Forms.ToolStripStatusLabel m_tbsCurRect;
        private System.Windows.Forms.Button m_bExport;
    }
}