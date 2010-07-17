namespace SpaceOverflow
{
    partial class BrowserOverlay
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
            System.Windows.Forms.ToolStripButton CloseButton;
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(BrowserOverlay));
            System.Windows.Forms.ToolStripButton OpenInBrowserButton;
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.BackButton = new System.Windows.Forms.ToolStripButton();
            this.ForwardButton = new System.Windows.Forms.ToolStripButton();
            this.WebBrowser = new System.Windows.Forms.WebBrowser();
            CloseButton = new System.Windows.Forms.ToolStripButton();
            OpenInBrowserButton = new System.Windows.Forms.ToolStripButton();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // CloseButton
            // 
            CloseButton.AccessibleRole = System.Windows.Forms.AccessibleRole.ScrollBar;
            CloseButton.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            CloseButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            CloseButton.Image = ((System.Drawing.Image)(resources.GetObject("CloseButton.Image")));
            CloseButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            CloseButton.Name = "CloseButton";
            CloseButton.Size = new System.Drawing.Size(116, 22);
            CloseButton.Text = "Return to the space!";
            CloseButton.Click += new System.EventHandler(this.CloseButton_Click);
            // 
            // toolStrip1
            // 
            this.toolStrip1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.BackButton,
            this.ForwardButton,
            CloseButton,
            OpenInBrowserButton});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Padding = new System.Windows.Forms.Padding(1, 0, 1, 0);
            this.toolStrip1.Size = new System.Drawing.Size(440, 25);
            this.toolStrip1.TabIndex = 1;
            this.toolStrip1.Text = "ToolBar";
            // 
            // BackButton
            // 
            this.BackButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.BackButton.Image = ((System.Drawing.Image)(resources.GetObject("BackButton.Image")));
            this.BackButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.BackButton.Name = "BackButton";
            this.BackButton.Size = new System.Drawing.Size(36, 22);
            this.BackButton.Text = "Back";
            this.BackButton.Click += new System.EventHandler(this.BackButton_Click);
            // 
            // ForwardButton
            // 
            this.ForwardButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.ForwardButton.Image = ((System.Drawing.Image)(resources.GetObject("ForwardButton.Image")));
            this.ForwardButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.ForwardButton.Name = "ForwardButton";
            this.ForwardButton.Size = new System.Drawing.Size(54, 22);
            this.ForwardButton.Text = "Forward";
            this.ForwardButton.Click += new System.EventHandler(this.ForwardButton_Click);
            // 
            // WebBrowser
            // 
            this.WebBrowser.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.WebBrowser.Location = new System.Drawing.Point(0, 24);
            this.WebBrowser.MinimumSize = new System.Drawing.Size(20, 20);
            this.WebBrowser.Name = "WebBrowser";
            this.WebBrowser.Size = new System.Drawing.Size(440, 404);
            this.WebBrowser.TabIndex = 0;
            this.WebBrowser.Url = new System.Uri("", System.UriKind.Relative);
            this.WebBrowser.Navigated += new System.Windows.Forms.WebBrowserNavigatedEventHandler(this.WebBrowser_Navigated);
            // 
            // OpenInBrowserButton
            // 
            OpenInBrowserButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            OpenInBrowserButton.Image = ((System.Drawing.Image)(resources.GetObject("OpenInBrowserButton.Image")));
            OpenInBrowserButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            OpenInBrowserButton.Name = "OpenInBrowserButton";
            OpenInBrowserButton.Size = new System.Drawing.Size(168, 22);
            OpenInBrowserButton.Text = "Open in your favorite browser";
            OpenInBrowserButton.Click += new System.EventHandler(this.OpenInBrowserButton_Click);
            // 
            // BrowserOverlay
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(440, 429);
            this.ControlBox = false;
            this.Controls.Add(this.WebBrowser);
            this.Controls.Add(this.toolStrip1);
            this.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "BrowserOverlay";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "OverlayForm";
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        public System.Windows.Forms.WebBrowser WebBrowser;
        private System.Windows.Forms.ToolStripButton BackButton;
        private System.Windows.Forms.ToolStripButton ForwardButton;
        private System.Windows.Forms.ToolStrip toolStrip1;

    }
}