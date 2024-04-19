using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using DevExpress.Images;
using DevExpress.Utils;
using DevExpress.Utils.Svg;
using DevExpress.XtraBars.Ribbon;

namespace GreenStem.Std
{
    public partial class frmIcon : DevExpress.XtraEditors.XtraForm
    {
        public event EventHandler<GalleryItemClickEventArgs> GalleryItemClick;
        public frmIcon()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            backgroundWorker1.RunWorkerAsync();
        }
        protected virtual void OnGalleryItemClick(GalleryItemClickEventArgs e)
        {
            GalleryItemClick?.Invoke(this, e);
        }
        private void BeginLoadingImages(object sender, DoWorkEventArgs e)
        {
            string[] imageKeys = ImageResourceCache.Default.GetAllResourceKeys();

            List<GalleryItem> svgGalleryItems = new List<GalleryItem>();

            foreach (string imageKey in imageKeys)
            {
                if (imageKey.EndsWith(".svg")) // Check if the image key ends with ".svg"
                {
                    int index = imageKey.LastIndexOf("/") + 1;
                    string nameicon = imageKey.Substring(index, imageKey.Length - index);

                    GalleryItem item = new GalleryItem();
                    item.ImageOptions.SvgImage = ImageResourceCache.Default.GetSvgImage(imageKey);
                    item.Caption = imageKey;
                    item.Description = imageKey;
                    item.Hint = nameicon;

                    svgGalleryItems.Add(item);
                }
            }

            e.Result = svgGalleryItems.ToArray();
        }
        private void ImageLoadCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            galleryControl1.Gallery.BeginUpdate();
            galleryControl1.Gallery.Groups[0].Items.AddRange(e.Result as GalleryItem[]);
            galleryControl1.Gallery.EndUpdate();
        }

        private void galleryControl1_Gallery_ItemClick(object sender, GalleryItemClickEventArgs e)
        {
            lblname.Text = e.Item.Description;
            // Raise the custom event with the clicked item's information
            OnGalleryItemClick(e);

        }
    }
}
