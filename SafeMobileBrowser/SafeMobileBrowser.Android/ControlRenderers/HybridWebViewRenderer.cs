using System.ComponentModel;
using Android.Content;
using Android.OS;
using Android.Webkit;
using SafeMobileBrowser.Controls;
using SafeMobileBrowser.Droid.ControlRenderers;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using AWeb = Android.Webkit;

[assembly: ExportRenderer(typeof(HybridWebView), typeof(HybridWebViewRenderer))]
namespace SafeMobileBrowser.Droid.ControlRenderers
{
    public class HybridWebViewRenderer : ViewRenderer<HybridWebView, AWeb.WebView>
    {
        public const string AssetBaseUrl = "file:///android_asset/";
        CustomWebViewClient _webViewClient;
        FormsWebChromeClient _webChromeClient;
        //const string JavaScriptFunction = "function invokeCSharpAction(data){jsBridge.invokeAction(data);} ; ";
        private readonly Context _context;


        public HybridWebViewRenderer(Context context) : base(context)
        {
            _context = context;
        }

        protected override void OnElementChanged(ElementChangedEventArgs<HybridWebView> e)
        {
            base.OnElementChanged(e);
            if (Control == null)
            {
                var webView = new AWeb.WebView(_context);

                _webViewClient = GetWebViewClient();
                webView.SetWebViewClient(_webViewClient);

                _webChromeClient = GetFormsWebChromeClient();
                webView.SetWebChromeClient(_webChromeClient);

                webView.Settings.JavaScriptEnabled = true;
                webView.Settings.DomStorageEnabled = true;

                SetNativeControl(webView);

                if ((int)Build.VERSION.SdkInt >= 19)
                {
                    Control.SetLayerType(Android.Views.LayerType.Hardware, null);
                }
                else
                {
                    Control.SetLayerType(Android.Views.LayerType.Software, null);
                }
            }
            if (e.OldElement != null)
            {
                //Control.RemoveJavascriptInterface("jsBridge");
                var hybridWebView = e.OldElement as HybridWebView;
                hybridWebView.Cleanup();
            }
            if (e.NewElement != null)
            {
                //Control.AddJavascriptInterface(new JSBridge(this), "jsBridge");
                Control.LoadUrl("file:///android_asset/startbrowsing.html");
            }
        }

        protected virtual CustomWebViewClient GetWebViewClient()
        {
            return new CustomWebViewClient(this);
        }

        protected virtual FormsWebChromeClient GetFormsWebChromeClient()
        {
            return new FormsWebChromeClient();
        }

        public void LoadingStateChanged(bool state)
        {
            ((HybridWebView)Element).ViewLoadingStatechanged(state);
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);
            switch (e.PropertyName)
            {
                case "Uri":
                    if (!string.IsNullOrEmpty(Element.Uri))
                        Control.LoadUrl(string.Format(Element.Uri));
                    break;
                default:
                    break;
            }
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (Element != null)
            {
                Control?.StopLoading();

                _webViewClient?.Dispose();
            }
        }
    }
}
