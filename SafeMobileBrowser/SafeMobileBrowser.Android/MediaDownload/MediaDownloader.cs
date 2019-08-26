using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Support.V4.App;
using Android.Widget;
using Nito.AsyncEx.Synchronous;
using Plugin.CurrentActivity;
using SafeMobileBrowser.WebFetchImplementation;
using Xamarin.Forms;
using Object = Java.Lang.Object;

namespace SafeMobileBrowser.Droid.MediaDownload
{
    public class MediaDownloader : AsyncTask
    {
        private const string ChannelId = "browser_notification_channel";
        private const string ChannelName = "browser_channel";
        private const string ChannelDescription = "Notification channel used by browser app";
        private const int NotificationId = 123456789;
        private const NotificationImportance ChannelNotificationImportance = NotificationImportance.Low;
        private NotificationManager _notificationManager;
        private NotificationCompat.Builder _builder;

        protected override void OnPreExecute()
        {
            base.OnPreExecute();

            if (_notificationManager == null)
            {
                _notificationManager =
                    (NotificationManager)CrossCurrentActivity.Current.AppContext.GetSystemService(
                        Context.NotificationService);

                if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
                {
                    var mChannel = new NotificationChannel(ChannelId, ChannelName, ChannelNotificationImportance)
                    {
                        Description = ChannelDescription,
                    };
                    mChannel.EnableLights(false);
                    mChannel.SetShowBadge(false);
                    _notificationManager.CreateNotificationChannel(mChannel);
                }
            }

            _builder = new NotificationCompat.Builder(CrossCurrentActivity.Current.AppContext, ChannelId);
            _builder.SetContentTitle("Downloading Files");
            _builder.SetContentText("Download in progress...");
            _builder.SetSmallIcon(Resource.Drawable.safenetworklogo);
            _notificationManager.Notify(NotificationId, _builder.Build());
        }

        protected override Object DoInBackground(params Object[] @params)
        {
            var dataItem = @params[0].ToString();
            if (dataItem.StartsWith("data:image"))
            {
                var image = DataImage.TryParse(dataItem);
                if (image == null)
                    return false;

                FileHelper.ExportBitmapAsFile(image.Image, image.MimeType);
                return true;
            }

            if (!dataItem.StartsWith("https"))
                return false;

            var task = WebFetchService.FetchResourceAsync(dataItem);
            var webFetchResponse = task.WaitAndUnwrapException();
            var bitmap = BitmapFactory.DecodeByteArray(webFetchResponse.Data, 0, webFetchResponse.Data.Length);

            if (bitmap == null)
                return false;

            FileHelper.ExportBitmapAsFile(bitmap, webFetchResponse.MimeType);
            return true;
        }

        protected override void OnPostExecute(Object result)
        {
            base.OnPostExecute(result);

            if ((bool)result)
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    Toast.MakeText(
                        CrossCurrentActivity.Current.AppContext,
                        "Image downloaded",
                        ToastLength.Long).Show();
                });
                _builder.SetContentText("Download completed");
                _notificationManager.Notify(NotificationId, _builder.Build());
            }
            else
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    Toast.MakeText(
                        CrossCurrentActivity.Current.AppContext,
                        "Failed to download image",
                        ToastLength.Long).Show();
                });
                _builder.SetContentText("Download failed");
                _notificationManager.Notify(NotificationId, _builder.Build());
            }
        }
    }
}
