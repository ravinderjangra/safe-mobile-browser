using System.IO;
using Android.Graphics;
using SafeMobileBrowser.Droid.PlatformServices;
using SafeMobileBrowser.Services;

[assembly: Xamarin.Forms.Dependency(typeof(ScreenshotService))]
namespace SafeMobileBrowser.Droid.PlatformServices
{
    public class ScreenshotService : IScreenshotService
    {
        public byte[] Capture()
        {
            var activity = Xamarin.Forms.Forms.Context as MainActivity;
            if (activity == null)
            {
                return null;
            }

            var view = activity.Window.DecorView.RootView;
            view.DrawingCacheEnabled = true;
            Bitmap bitmap = view.GetDrawingCache(true);
            byte[] bitmapData;
            using (var stream = new MemoryStream())
            {
                bitmap.Compress(Bitmap.CompressFormat.Png, 0, stream);
                bitmapData = stream.ToArray();
            }
            return bitmapData;
        }
    }
}