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
    public class HybridWebViewRenderer : ViewRenderer<HybridWebView, WKWebView>, IWKUIDelegate
    {
        public static string BaseUrl { get; set; } = NSBundle.MainBundle.BundlePath;

        HybridNavigationDelegate _navigationDelegate;
        WKWebViewConfiguration _configuration;
        WKUserContentController _contentController;

        protected override void OnElementChanged(ElementChangedEventArgs<HybridWebView> e)
        {
            base.OnElementChanged(e);

            if (Control == null && Element != null)
                SetupControl();

            if (e.NewElement != null)
                SetupElement(e.NewElement);

            if (e.OldElement != null)
                DestroyElement(e.OldElement);
        }

        void SetupElement(HybridWebView element)
        {
            SetSource();
        }

        private void SetSource()
        {
            if (Control == null || Element == null) return;

            var path = Path.Combine(BaseUrl, "startbrowsing.html");
            var nsFileUri = new NSUrl($"file://{path}");
            var nsBaseUri = new NSUrl($"file://{BaseUrl}");

            Control.LoadFileUrl(nsFileUri, nsBaseUri);
        }

        void DestroyElement(HybridWebView element)
        {
        }

        void SetupControl()
        {
            _navigationDelegate = new HybridNavigationDelegate(this);
            _contentController = new WKUserContentController();

            _configuration = new WKWebViewConfiguration
            {
                UserContentController = _contentController
            };
            _configuration.SetUrlSchemeHandler(new SafeSchemaHandler(), "safe");

            var wkWebView = new WKWebView(Frame, _configuration)
            {
                Opaque = false,
                UIDelegate = this,
                NavigationDelegate = _navigationDelegate
            };

            SetNativeControl(wkWebView);
        }

        protected override void OnElementPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);
            if (e.PropertyName == nameof(Element.Uri))
            {
                Control.LoadRequest(NSUrlRequest.FromUrl(NSUrl.FromString(Element.Uri)));
            }
        }
    }
}
