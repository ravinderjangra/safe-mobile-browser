using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Net;
using Android.OS;
using Android.Support.V4.App;
using Android.Webkit;
using Android.Widget;
using Java.IO;
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
        private string guessedFileName;
        private bool fileAlreadyExists = false;

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
            guessedFileName = URLUtil.GuessFileName(dataItem, null, null);

            if (dataItem.StartsWith("data:image"))
            {
                var image = DataImage.TryParse(dataItem);
                if (image == null)
                    return false;

                FileHelper.ExportBitmapAsFile(image.Image, image.MimeType, guessedFileName);
                return true;
            }

            if (!dataItem.StartsWith("https"))
                return false;

            if (FileHelper.MediaExists(guessedFileName))
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    Toast.MakeText(
                        CrossCurrentActivity.Current.AppContext,
                        "File Already Exists",
                        ToastLength.Long).Show();
                });
                fileAlreadyExists = true;
                return true;
            }

            var task = WebFetchService.FetchResourceAsync(dataItem);
            var webFetchResponse = task.WaitAndUnwrapException();

            var bitmap = BitmapFactory.DecodeByteArray(webFetchResponse.Data, 0, webFetchResponse.Data.Length);
            if (bitmap == null)
            {
                // work around as some jpg images are encoded incorrectly
                FileHelper.SaveImageAtDownloads(webFetchResponse.Data, guessedFileName);
            }
            else
            {
                FileHelper.ExportBitmapAsFile(bitmap, webFetchResponse.MimeType, guessedFileName);
            }

            return true;
        }

        protected override void OnPostExecute(Object result)
        {
            base.OnPostExecute(result);

            if ((bool)result)
            {
                if (fileAlreadyExists)
                {
                    _notificationManager.Cancel(NotificationId);
                    return;
                }

                Device.BeginInvokeOnMainThread(() =>
                {
                    Toast.MakeText(
                        CrossCurrentActivity.Current.AppContext,
                        "Image downloaded",
                        ToastLength.Long).Show();
                });

                var downloadPath = System.IO.Path.Combine(Environment.ExternalStorageDirectory.AbsolutePath, Environment.DirectoryDownloads);
                var filePath = System.IO.Path.Combine(downloadPath, guessedFileName);
                File file = new File(filePath);

                var mimeTypeMap = MimeTypeMap.Singleton;
                var fileExtension = MimeTypeMap.GetFileExtensionFromUrl(filePath);
                var mimeType = MimeTypeMap.Singleton.GetMimeTypeFromExtension(fileExtension.ToLower());

                var intent = new Intent(Intent.ActionView, Android.Net.Uri.Parse(filePath));
                intent.SetDataAndType(Uri.FromFile(file), mimeType);
                intent.SetFlags(ActivityFlags.ClearTop | ActivityFlags.SingleTop | ActivityFlags.NewTask);
                var pendingIntent = PendingIntent.GetActivity(CrossCurrentActivity.Current.Activity, 0, intent, 0);

                _builder.SetContentTitle("Download completed");
                _builder.SetContentText(guessedFileName);
                _builder.SetContentIntent(pendingIntent);
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
