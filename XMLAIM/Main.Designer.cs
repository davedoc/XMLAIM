namespace XMLAIM
{
    partial class Main
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
			this.ManifestXMLlabel = new System.Windows.Forms.TextBox();
			this.ManifstXMLbutton = new System.Windows.Forms.Button();
			this.logTextbox = new System.Windows.Forms.TextBox();
			this.ManifesttreeView = new System.Windows.Forms.TreeView();
			this.IsmTextBox = new System.Windows.Forms.TextBox();
			this.LoadIsmButton = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// ManifestXMLlabel
			// 
			this.ManifestXMLlabel.Location = new System.Drawing.Point(12, 12);
			this.ManifestXMLlabel.Name = "ManifestXMLlabel";
			this.ManifestXMLlabel.Size = new System.Drawing.Size(583, 20);
			this.ManifestXMLlabel.TabIndex = 0;
			this.ManifestXMLlabel.Text = "D:\\workarea\\manifest.xml";
			// 
			// ManifstXMLbutton
			// 
			this.ManifstXMLbutton.Location = new System.Drawing.Point(12, 38);
			this.ManifstXMLbutton.Name = "ManifstXMLbutton";
			this.ManifstXMLbutton.Size = new System.Drawing.Size(127, 23);
			this.ManifstXMLbutton.TabIndex = 1;
			this.ManifstXMLbutton.Text = "Load Manifest";
			this.ManifstXMLbutton.UseVisualStyleBackColor = true;
			this.ManifstXMLbutton.Click += new System.EventHandler(this.ManifstXMLbutton_Click);
			// 
			// logTextbox
			// 
			this.logTextbox.Location = new System.Drawing.Point(12, 401);
			this.logTextbox.Multiline = true;
			this.logTextbox.Name = "logTextbox";
			this.logTextbox.ReadOnly = true;
			this.logTextbox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			this.logTextbox.Size = new System.Drawing.Size(583, 139);
			this.logTextbox.TabIndex = 3;
			this.logTextbox.Visible = false;
			// 
			// ManifesttreeView
			// 
			this.ManifesttreeView.Location = new System.Drawing.Point(12, 124);
			this.ManifesttreeView.Name = "ManifesttreeView";
			this.ManifesttreeView.Size = new System.Drawing.Size(459, 258);
			this.ManifesttreeView.TabIndex = 4;
			// 
			// IsmTextBox
			// 
			this.IsmTextBox.Location = new System.Drawing.Point(13, 68);
			this.IsmTextBox.Name = "IsmTextBox";
			this.IsmTextBox.Size = new System.Drawing.Size(582, 20);
			this.IsmTextBox.TabIndex = 5;
			this.IsmTextBox.Text = "D:\\cm\\BM10R1\\Suite\\REG\\MSIInstall\\setupxml.ism";
			// 
			// LoadIsmButton
			// 
			this.LoadIsmButton.Location = new System.Drawing.Point(13, 95);
			this.LoadIsmButton.Name = "LoadIsmButton";
			this.LoadIsmButton.Size = new System.Drawing.Size(75, 23);
			this.LoadIsmButton.TabIndex = 6;
			this.LoadIsmButton.Text = "Update ISM";
			this.LoadIsmButton.UseVisualStyleBackColor = true;
			this.LoadIsmButton.Click += new System.EventHandler(this.LoadIsmButton_Click);
			// 
			// Main
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(607, 552);
			this.Controls.Add(this.LoadIsmButton);
			this.Controls.Add(this.IsmTextBox);
			this.Controls.Add(this.ManifesttreeView);
			this.Controls.Add(this.logTextbox);
			this.Controls.Add(this.ManifstXMLbutton);
			this.Controls.Add(this.ManifestXMLlabel);
			this.Name = "Main";
			this.Text = "Main";
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox ManifestXMLlabel;
        private System.Windows.Forms.Button ManifstXMLbutton;
        private System.Windows.Forms.TextBox logTextbox;
        private System.Windows.Forms.TreeView ManifesttreeView;
        private System.Windows.Forms.TextBox IsmTextBox;
        private System.Windows.Forms.Button LoadIsmButton;
    }
}