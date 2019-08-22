using Acr.UserDialogs;
using Android.Views;
using Android.Webkit;

namespace SafeMobileBrowser.Droid.MediaDownload
{
    public class ImageDownloadMenuItemListener : Java.Lang.Object, IMenuItemOnMenuItemClickListener
    {
        private readonly string _itemExtra;

        public ImageDownloadMenuItemListener(string extra)
        {
            _itemExtra = extra;
        }

        public bool OnMenuItemClick(IMenuItem item)
        {
            if (string.IsNullOrEmpty(_itemExtra))
                return false;
            var guessedFileName = URLUtil.GuessFileName(_itemExtra, null, null);

            if (FileHelper.MediaExists(guessedFileName))
            {
                UserDialogs.Instance.ActionSheet(new ActionSheetConfig()
                .SetTitle("Media already Exists")
                .SetMessage($"Do you want replace the existing {guessedFileName} in Download")
                .Add("Replace File", () => new MediaDownloader().Execute(_itemExtra, guessedFileName), null)
                .SetCancel()
                .SetUseBottomSheet(true));
            }
            else
            {
                _ = new MediaDownloader().Execute(_itemExtra, guessedFileName);
            }

            return true;
        }
    }
}
