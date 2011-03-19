namespace CS8803AGAEditor
{
    partial class ContentTypeSelector
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
            this.m_lbTypes = new System.Windows.Forms.ListBox();
            this.m_tcTabs = new System.Windows.Forms.TabControl();
            this.m_bCreateNew = new System.Windows.Forms.Button();
            this.m_bLoad = new System.Windows.Forms.Button();
            this.m_bSave = new System.Windows.Forms.Button();
            this.m_bPicEditor = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // m_lbTypes
            // 
            this.m_lbTypes.FormattingEnabled = true;
            this.m_lbTypes.Location = new System.Drawing.Point(13, 13);
            this.m_lbTypes.Name = "m_lbTypes";
            this.m_lbTypes.Size = new System.Drawing.Size(259, 199);
            this.m_lbTypes.TabIndex = 0;
            this.m_lbTypes.SelectedIndexChanged += new System.EventHandler(this.m_lbTypes_SelectedIndexChanged);
            // 
            // m_tcTabs
            // 
            this.m_tcTabs.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.m_tcTabs.Location = new System.Drawing.Point(278, 13);
            this.m_tcTabs.Name = "m_tcTabs";
            this.m_tcTabs.SelectedIndex = 0;
            this.m_tcTabs.Size = new System.Drawing.Size(370, 442);
            this.m_tcTabs.TabIndex = 1;
            this.m_tcTabs.Selected += new System.Windows.Forms.TabControlEventHandler(this.m_tcTabs_Selected);
            // 
            // m_bCreateNew
            // 
            this.m_bCreateNew.Enabled = false;
            this.m_bCreateNew.Location = new System.Drawing.Point(13, 219);
            this.m_bCreateNew.Name = "m_bCreateNew";
            this.m_bCreateNew.Size = new System.Drawing.Size(75, 23);
            this.m_bCreateNew.TabIndex = 2;
            this.m_bCreateNew.Text = "Create New";
            this.m_bCreateNew.UseVisualStyleBackColor = true;
            this.m_bCreateNew.Click += new System.EventHandler(this.m_bCreateNew_Click);
            // 
            // m_bLoad
            // 
            this.m_bLoad.Location = new System.Drawing.Point(13, 249);
            this.m_bLoad.Name = "m_bLoad";
            this.m_bLoad.Size = new System.Drawing.Size(75, 23);
            this.m_bLoad.TabIndex = 3;
            this.m_bLoad.Text = "Load";
            this.m_bLoad.UseVisualStyleBackColor = true;
            this.m_bLoad.Click += new System.EventHandler(this.m_bLoad_Click);
            // 
            // m_bSave
            // 
            this.m_bSave.Location = new System.Drawing.Point(13, 279);
            this.m_bSave.Name = "m_bSave";
            this.m_bSave.Size = new System.Drawing.Size(75, 23);
            this.m_bSave.TabIndex = 4;
            this.m_bSave.Text = "Save";
            this.m_bSave.UseVisualStyleBackColor = true;
            this.m_bSave.Click += new System.EventHandler(this.m_bSave_Click);
            // 
            // m_bPicEditor
            // 
            this.m_bPicEditor.Location = new System.Drawing.Point(13, 309);
            this.m_bPicEditor.Name = "m_bPicEditor";
            this.m_bPicEditor.Size = new System.Drawing.Size(75, 23);
            this.m_bPicEditor.TabIndex = 5;
            this.m_bPicEditor.Text = "Pic Editor";
            this.m_bPicEditor.UseVisualStyleBackColor = true;
            this.m_bPicEditor.Click += new System.EventHandler(this.m_bPicEditor_Click);
            // 
            // ContentTypeSelector
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(660, 467);
            this.Controls.Add(this.m_bPicEditor);
            this.Controls.Add(this.m_bSave);
            this.Controls.Add(this.m_bLoad);
            this.Controls.Add(this.m_bCreateNew);
            this.Controls.Add(this.m_tcTabs);
            this.Controls.Add(this.m_lbTypes);
            this.Name = "ContentTypeSelector";
            this.Text = "ContentTypeSelector";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListBox m_lbTypes;
        private System.Windows.Forms.TabControl m_tcTabs;
        private System.Windows.Forms.Button m_bCreateNew;
        private System.Windows.Forms.Button m_bLoad;
        private System.Windows.Forms.Button m_bSave;
        private System.Windows.Forms.Button m_bPicEditor;
    }
}