﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;

namespace SpaceOverflow
{
    public partial class BrowserOverlay : Form
    {
        public BrowserOverlay() {
            InitializeComponent();
            this.toolStrip1.BackColor = Color.FromArgb(238, 238, 238);
        }

        private void BackButton_Click(object sender, EventArgs e) {
            this.WebBrowser.GoBack();
        }

        private void ForwardButton_Click(object sender, EventArgs e) {
            this.WebBrowser.GoForward();
        }

        private void CloseButton_Click(object sender, EventArgs e) {
            this.Hide();
            this.WebBrowser.Navigate("about:blank");
        }

        private void WebBrowser_Navigated(object sender, WebBrowserNavigatedEventArgs e) {
            this.BackButton.Enabled = this.WebBrowser.CanGoBack;
            this.ForwardButton.Enabled = this.WebBrowser.CanGoForward;
        }

        private void OpenInBrowserButton_Click(object sender, EventArgs e) {
            Process.Start(this.WebBrowser.Url.ToString());
        }
    }
}
