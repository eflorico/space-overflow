using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Imaging;
using Microsoft.Xna.Framework.Graphics;

namespace SpaceOverflow
{
    class XNABrowser
    {
        public XNABrowser(GraphicsDevice device, Microsoft.Xna.Framework.Vector2 size)
        {
            this.WinFormsBrowser = new WebBrowser();
            this.WinFormsBrowser.Size = new Size((int)size.X, (int)size.Y);

            this.WinFormsBrowserBuffer = new System.Drawing.Bitmap(this.WinFormsBrowser.Width, this.WinFormsBrowser.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            this.XNABrowserBuffer = new Texture2D(device, this.WinFormsBrowser.Width, this.WinFormsBrowser.Height, 0, TextureUsage.AutoGenerateMipMap, SurfaceFormat.Color);

            this.State = WebBrowserReadyState.Loading;

            this.WinFormsBrowser.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler((sender, e) =>
                this.State = WebBrowserReadyState.Complete);
        }

        public WebBrowserReadyState State { get; private set; }
        private WebBrowser WinFormsBrowser;
        private Bitmap WinFormsBrowserBuffer;
        public Texture2D XNABrowserBuffer { get; private set; }

        public void Navigate(Uri to)
        {
            this.WinFormsBrowser.Url = to;
            
        }

        public void UpdateTexture()
        {
            this.WinFormsBrowser.DrawToBitmap(this.WinFormsBrowserBuffer, new System.Drawing.Rectangle(0, 0, this.WinFormsBrowser.Width, this.WinFormsBrowser.Height));
            this.CopyBitmapToTexture();
        }

        private void CopyBitmapToTexture()
        {
            byte[] textureData = new byte[4 * this.WinFormsBrowser.Width * this.WinFormsBrowser.Height];

            BitmapData bmpData = this.WinFormsBrowserBuffer.LockBits(new System.Drawing.Rectangle(0, 0, this.WinFormsBrowser.Width, this.WinFormsBrowser.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            var safePtr = bmpData.Scan0;
            System.Runtime.InteropServices.Marshal.Copy(safePtr, textureData, 0, textureData.Length);
            this.WinFormsBrowserBuffer.UnlockBits(bmpData);

            this.XNABrowserBuffer.SetData<byte>(textureData);
        }
    }
}
