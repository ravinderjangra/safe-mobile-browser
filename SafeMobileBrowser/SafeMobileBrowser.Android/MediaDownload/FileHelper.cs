using System;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
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
        public static void ExportBitmapAsFile(Bitmap image, string fileType, [Optional] string fileName)
        {
            try
            {
                var extKey = MimeMapping.MimeUtility.TypeMap.FirstOrDefault(t => t.Value == fileType).Key;
                var downloadPath = Path.Combine(Android.OS.Environment.ExternalStorageDirectory.AbsolutePath, Android.OS.Environment.DirectoryDownloads);
                var filePath = Path.Combine(downloadPath, fileName ?? $"image.{extKey}");
                var stream = new FileStream(filePath, FileMode.Create);
                image.Compress(Bitmap.CompressFormat.Png, 100, stream);
                stream.Close();
                Device.BeginInvokeOnMainThread(() =>
                {
                    Toast.MakeText(CrossCurrentActivity.Current.AppContext, "Image downloaded", ToastLength.Long)
                        .Show();
                });
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
    }
}
