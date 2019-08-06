using Android.Content;
using SafeMobileBrowser.Controls;
using SafeMobileBrowser.Droid.ControlRenderers;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using AWebkit = Android.Webkit;
using Image = System.Drawing.Image;

[assembly: ExportRenderer(typeof(HybridWebView), typeof(HybridWebViewRenderer))]

namespace SafeMobileBrowser.Droid.ControlRenderers
{
    public class HybridWebViewRenderer : WebViewRenderer
    {
        private HybridWebViewClient _webViewClient;
        private HybridWebViewChromeClient _webViewChromeClient;

        public HybridWebViewRenderer(Context context)
            : base(context)
        {
        }

        protected override void OnElementChanged(ElementChangedEventArgs<WebView> e)
        {
            base.OnElementChanged(e);

            if (Control != null)
            {
                _webViewClient = GetHybridWebViewClient();
                Control.SetWebViewClient(_webViewClient);
                _webViewChromeClient = GetHybridWebViewChromeClient();
                Control.SetWebChromeClient(_webViewChromeClient);
                Control.LoadUrl($"{AssetBaseUrl}index.html");
                CrossCurrentActivity.Current.Activity.RegisterForContextMenu(Control);
            }
        }

        protected override void OnCreateContextMenu(IContextMenu menu)
        {
            base.OnCreateContextMenu(menu);

            var webViewHitTestResult = Control.GetHitTestResult().Type;

            if (webViewHitTestResult == AWebkit.HitTestResult.ImageType ||
                webViewHitTestResult == AWebkit.HitTestResult.SrcAnchorType)
            {
                menu.SetHeaderTitle("Download Image...");
                menu.Add(0, 1, 0, "click to download")
                    .SetOnMenuItemClickListener(new ImageDownloadMenuItemListener(Control.GetHitTestResult().Extra));
            }
        }

        private HybridWebViewClient GetHybridWebViewClient()
        {
            return new HybridWebViewClient(this);
        }

        private HybridWebViewChromeClient GetHybridWebViewChromeClient()
        {
            return new HybridWebViewChromeClient(this);
        }

        protected override void Dispose(bool disposing)
        {
            _webViewClient?.Dispose();
            _webViewChromeClient?.Dispose();
            base.Dispose(disposing);
        }

        public class ImageDownloadMenuItemListener : Java.Lang.Object, IMenuItemOnMenuItemClickListener
        {
            private readonly string _itemExtra;

            public ImageDownloadMenuItemListener(string extra)
            {
                _itemExtra = extra;
            }

            public bool OnMenuItemClick(IMenuItem item)
            {
                if (_itemExtra.StartsWith("data:image"))
                {
                    var image = DataImage.TryParse(_itemExtra);
                    if (image == null)
                    {
                        Toast.MakeText(
                            CrossCurrentActivity.Current.AppContext,
                            "Failed to download image",
                            ToastLength.Long).Show();
                        return false;
                    }

                    ExportBitmapAsFile(image.Image, image.MimeType);
                    return true;
                }

                if (_itemExtra.StartsWith("https"))
                {
                    Device.BeginInvokeOnMainThread(async () =>
                    {
                        var webFetchResponse = await WebFetchService.FetchResourceAsync(_itemExtra);
                        var ms = new MemoryStream(webFetchResponse.Data);
                        var bmp = Image.FromStream(ms);
                        ExportBitmapAsFile(bmp, webFetchResponse.MimeType);
                    });
                    return true;
                }

                return false;
            }

            private void ExportBitmapAsFile(Image image, string fileType, [Optional] string fileName)
            {
                try
                {
                    var extKey = MimeMapping.MimeUtility.TypeMap.FirstOrDefault(t => t.Value == fileType).Key;
                    var downloadPath = Path.Combine(Android.OS.Environment.ExternalStorageDirectory.AbsolutePath, Android.OS.Environment.DirectoryDownloads);
                    var filePath = Path.Combine(downloadPath, fileName ?? $"image.{extKey}");
                    var stream = new FileStream(filePath, FileMode.Create);
                    image.Save(stream, ImageFormat.Png);
                    stream.Close();
                    Toast.MakeText(CrossCurrentActivity.Current.AppContext, "Image downloaded", ToastLength.Long)
                        .Show();
                }
                catch (Exception ex)
                {
                    Logger.Error(ex);
                    Toast.MakeText(
                        CrossCurrentActivity.Current.AppContext,
                        "Failed to download image",
                        ToastLength.Long).Show();
                }
            }

            public class DataImage
            {
                private static readonly Regex DataUriPattern = new Regex(
                    @"^data\:(?<type>image\/(png|tiff|jpg|gif));base64,(?<data>[A-Z0-9\+\/\=]+)$",
                    RegexOptions.Compiled | RegexOptions.ExplicitCapture | RegexOptions.IgnoreCase);

                private DataImage(string mimeType, byte[] rawData)
                {
                    MimeType = mimeType;
                    RawData = rawData;
                }

                public string MimeType { get; }

                public byte[] RawData { get; }

                public Bitmap Image => new Bitmap(new MemoryStream(RawData));

                public static DataImage TryParse(string dataUri)
                {
                    if (string.IsNullOrWhiteSpace(dataUri))
                        return null;

                    var match = DataUriPattern.Match(dataUri);
                    if (!match.Success)
                        return null;

                    var mimeType = match.Groups["type"].Value;
                    var base64Data = match.Groups["data"].Value;

                    try
                    {
                        var rawData = Convert.FromBase64String(base64Data);
                        return rawData.Length == 0 ? null : new DataImage(mimeType, rawData);
                    }
                    catch (FormatException)
                    {
                        return null;
                    }
                }
            }
        }
    }
}
