using System.Globalization;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using SafeMobileBrowser.Controls;
using SafeMobileBrowser.Droid.ControlRenderers;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using AWeb = Android.Webkit;
using Android.Content;
using System.ComponentModel;

[assembly: Xamarin.Forms.ExportRenderer(typeof(HybridWebView), typeof(HybridWebViewRenderer))]
namespace SafeMobileBrowser.Droid.ControlRenderers
{
    public class HybridWebViewRenderer : ViewRenderer<HybridWebView, AWeb.WebView>
    {
        public const string AssetBaseUrl = "file:///android_asset/";
        JavascriptValueCallback _callback;
        private readonly Context _context;


        public HybridWebViewRenderer(Context context) : base(context)
        {
            _context = context;
        }

        protected override void OnElementChanged(ElementChangedEventArgs<HybridWebView> e)
        {
            base.OnElementChanged(e);
            if (Control == null && Element != null)
                SetupControl();

            if (e.NewElement != null)
                SetupElement(e.NewElement);

            if (e.OldElement != null)
                DestroyElement(e.NewElement);
        }

        void SetupElement(HybridWebView element)
        {
            element.OnJavascriptInjectionRequest += OnJavascriptInjectionRequest;
            Control.LoadUrl("file:///android_asset/startbrowsing.html");
        }

        void DestroyElement(HybridWebView element)
        {
            element.OnJavascriptInjectionRequest -= OnJavascriptInjectionRequest;
            element.Dispose();
        }

        void SetupControl()
        {
            var webView = new AWeb.WebView(_context);
            _callback = new JavascriptValueCallback(this);

            AWeb.WebView.SetWebContentsDebuggingEnabled(true);

            // Defaults
            webView.Settings.JavaScriptEnabled = true;
            webView.Settings.DomStorageEnabled = true;
            webView.AddJavascriptInterface(new HybridWebViewBridge(this), "safe");
            webView.SetWebViewClient(new CustomWebViewClient(this));
            webView.SetWebChromeClient(new HybridWebViewChromeClient(this));
            webView.SetBackgroundColor(Android.Graphics.Color.Transparent);

            HybridWebView.CallbackAdded += OnCallbackAdded;

            SetNativeControl(webView);
        }

        async void OnCallbackAdded(object sender, string e)
        {
            if (Element == null || string.IsNullOrWhiteSpace(e)) return;

            if ((sender == null && Element.EnableGlobalCallbacks) || sender != null)
                await OnJavascriptInjectionRequest(HybridWebView.GenerateFunctionScript(e));
        }

        internal async Task<string> OnJavascriptInjectionRequest(string js)
        {
            if (Element == null || Control == null) return string.Empty;

            // fire!
            _callback.Reset();

            var response = string.Empty;

            Device.BeginInvokeOnMainThread(() => Control.EvaluateJavascript(js, _callback));

            // wait!
            await Task.Run(() =>
            {
                while (_callback.Value == null) { }

                // Get the string and strip off the quotes
                if (_callback.Value is Java.Lang.String)
                {
                    // Unescape that damn Unicode Java bull.
                    response = Regex.Replace(_callback.Value.ToString(),
                        @"\\[Uu]([0-9A-Fa-f]{4})",
                        m => char.ToString((char)ushort.Parse(m.Groups[1].Value,
                        NumberStyles.AllowHexSpecifier)));
                    response = Regex.Unescape(response);

                    if (response.Equals("\"null\""))
                        response = null;

                    else if (response.StartsWith("\"") && response.EndsWith("\""))
                        response = response.Substring(1, response.Length - 2);
                }

            });

            // return
            return response;
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            if (e.PropertyName == HybridWebView.UriProperty.PropertyName)
                if (!string.IsNullOrEmpty(Element.Uri))
                    Control.LoadUrl(string.Format(Element.Uri));
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (Element != null)
            {
                Control?.StopLoading();
            }
        }
    }
}
