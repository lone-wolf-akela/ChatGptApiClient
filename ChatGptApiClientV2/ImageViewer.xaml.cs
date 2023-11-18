using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ChatGptApiClientV2
{
    /// <summary>
    /// ImageViewer.xaml 的交互逻辑
    /// </summary>
    public partial class ImageViewer : Window
    {
        public ImageViewer()
        {
            InitializeComponent();
        }
        public void ShowImage(BitmapImage img)
        {
            imgbox.Source = img;
        }
        private void btn_saveimg_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new SaveFileDialog
            {
                FileName = "image",
                DefaultExt = ".png",
                Filter = "PNG images|*.png",
                ClientGuid = new Guid("A1DF5042-ADCE-48EA-9275-32A4E877B2F9"),
            };
            if (dlg.ShowDialog() == true)
            {
                var encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create((BitmapSource)imgbox.Source));
                using var stream = dlg.OpenFile();
                encoder.Save(stream);
            }
        }
    }
}
