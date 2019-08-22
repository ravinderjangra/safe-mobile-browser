using Android.Views;

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

            new MediaDownloader(_itemExtra).Execute();
            return true;
        }
    }
}
