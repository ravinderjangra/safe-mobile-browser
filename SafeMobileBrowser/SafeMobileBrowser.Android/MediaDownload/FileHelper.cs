using System;
using System.IO;
using System.Linq;
using Android.Graphics;
using Android.Widget;
using Plugin.CurrentActivity;
using SafeMobileBrowser.Helpers;
using Xamarin.Forms;
using Path = System.IO.Path;

namespace SafeMobileBrowser.Droid.MediaDownload
{
    public static class FileHelper
    {
        private static string DownloadPath => Path.Combine(Android.OS.Environment.ExternalStorageDirectory.AbsolutePath, Android.OS.Environment.DirectoryDownloads);

        public static void ExportBitmapAsFile(Bitmap image, string fileType, string fileName)
        {
            try
            {
                var filePath = Path.Combine(DownloadPath, fileName);
                SaveMedia(image, filePath);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                Device.BeginInvokeOnMainThread(() =>
                {
                    Toast.MakeText(
                        CrossCurrentActivity.Current.AppContext,
                        "Failed to download image",
                        ToastLength.Long).Show();
                });
            }
        }

        public static bool MediaExists(string fileName)
        {
            var filePath = Path.Combine(DownloadPath, fileName);
            return File.Exists(filePath);
        }

        public static void SaveImageAtDownloads(byte[] byteImageData, string fileName)
        {
            var filePath = Path.Combine(DownloadPath, fileName);
            File.WriteAllBytes(filePath, byteImageData.Concat(new byte[] { (byte)0xD9 }).ToArray());
        }

        public static void SaveMedia(Bitmap image, string filePath)
        {
            var stream = new FileStream(filePath, FileMode.Create);
            image.Compress(Bitmap.CompressFormat.Png, 100, stream);
            stream.Close();
            Device.BeginInvokeOnMainThread(() =>
            {
                Toast.MakeText(CrossCurrentActivity.Current.AppContext, "Image downloaded", ToastLength.Long)
                     .Show();
            });
        }
    }
}
