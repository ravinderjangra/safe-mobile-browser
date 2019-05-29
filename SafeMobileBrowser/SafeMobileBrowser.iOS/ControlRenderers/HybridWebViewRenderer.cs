using Foundation;
using SafeMobileBrowser.Controls;
using SafeMobileBrowser.iOS.ControlRenderers;
using System.IO;
using WebKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(HybridWebView), typeof(HybridWebViewRenderer))]
namespace SafeMobileBrowser.iOS.ControlRenderers
{
    public class HybridWebViewRenderer : WkWebViewRenderer
    {
        public static string BaseUrl { get; set; } = NSBundle.MainBundle.BundlePath;

        protected override void OnElementChanged(VisualElementChangedEventArgs e)
        {
            base.OnElementChanged(e);

            if (NativeView != null)
            {
                var control = (WKWebView)NativeView;
                control.Configuration.SetUrlSchemeHandler(new SafeSchemaHandler(), "safe");
                SetSource();
            }
        }

        private void SetSource()
        {
            if (NativeView == null || Element == null) return;

            var path = Path.Combine(BaseUrl, "startbrowsing.html");
            var nsFileUri = new NSUrl($"file://{path}");
            var nsBaseUri = new NSUrl($"file://{BaseUrl}");

            ((WKWebView)NativeView).LoadFileUrl(nsFileUri, nsBaseUri);
        }
    }
}
