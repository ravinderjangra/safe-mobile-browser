using Foundation;
using SafeMobileBrowser.Controls;
using SafeMobileBrowser.iOS.ControlRenderers;
using SafeMobileBrowser.WebFetchImplementation;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using UIKit;
using WebKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(HybridWebView), typeof(HybridWebViewRenderer))]
namespace SafeMobileBrowser.iOS.ControlRenderers
{
    public class HybridWebViewRenderer : ViewRenderer<HybridWebView, WKWebView>, IWKScriptMessageHandler, IWKUIDelegate
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
            element.OnJavascriptInjectionRequest += OnJavascriptInjectionRequest;
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
            element.OnJavascriptInjectionRequest -= OnJavascriptInjectionRequest;

            element.Dispose();
        }

        void SetupControl()
        {
            _navigationDelegate = new HybridNavigationDelegate(this);
            _contentController = new WKUserContentController();
            _contentController.AddScriptMessageHandler(this, "safe");
            _configuration = new WKWebViewConfiguration
            {
                UserContentController = _contentController
            };

            var wkWebView = new WKWebView(Frame, _configuration)
            {
                Opaque = false,
                UIDelegate = this,
                NavigationDelegate = _navigationDelegate
            };

            HybridWebView.CallbackAdded += OnCallbackAdded;

            SetNativeControl(wkWebView);
        }

        async void OnCallbackAdded(object sender, string e)
        {
            if (Element == null || string.IsNullOrWhiteSpace(e)) return;

            if ((sender == null && Element.EnableGlobalCallbacks) || sender != null)
                await OnJavascriptInjectionRequest(HybridWebView.GenerateFunctionScript(e));
        }

        internal async Task<string> OnJavascriptInjectionRequest(string js)
        {
            if (Control == null || Element == null) return string.Empty;

            var response = string.Empty;

            try
            {
                var obj = await Control.EvaluateJavaScriptAsync(js).ConfigureAwait(true);
                if (obj != null)
                    response = obj.ToString();
            }

            catch (Exception) { /* The Webview might not be ready... */ }
            return response;
        }

        public void DidReceiveScriptMessage(WKUserContentController userContentController, WKScriptMessage message)
        {
            if (Element == null || message == null || message.Body == null) return;
            Element.HandleScriptReceived(message.Body.ToString());
        }

        [Export("webView:runJavaScriptAlertPanelWithMessage:initiatedByFrame:completionHandler:")]
        public void RunJavaScriptAlertPanel(WebKit.WKWebView webView, string message, WKFrameInfo frame, Action completionHandler)
        {
            var alertController = UIAlertController.Create(null, message, UIAlertControllerStyle.Alert);
            alertController.AddAction(UIAlertAction.Create("Ok", UIAlertActionStyle.Default, null));
            UIApplication.SharedApplication.KeyWindow.RootViewController.PresentViewController(alertController, true, null);

            completionHandler();
        }

        [Export("webView:runJavaScriptConfirmPanelWithMessage:initiatedByFrame:completionHandler:")]
        public void RunJavaScriptConfirmPanel(WKWebView webView, string message, WKFrameInfo frame, Action<bool> completionHandler)
        {
            var alertController = UIAlertController.Create(null, message, UIAlertControllerStyle.Alert);

            alertController.AddAction(UIAlertAction.Create("Ok", UIAlertActionStyle.Default, okAction =>
            {

                completionHandler(true);

            }));

            alertController.AddAction(UIAlertAction.Create("Cancel", UIAlertActionStyle.Default, cancelAction =>
            {

                completionHandler(false);

            }));

            UIApplication.SharedApplication.KeyWindow.RootViewController.PresentViewController(alertController, true, null);
        }



        [Export("webView:runJavaScriptTextInputPanelWithPrompt:defaultText:initiatedByFrame:completionHandler:")]
        public void RunJavaScriptTextInputPanel(WKWebView webView, string prompt, string defaultText, WebKit.WKFrameInfo frame, System.Action<string> completionHandler)
        {
            var alertController = UIAlertController.Create(null, prompt, UIAlertControllerStyle.Alert);

            UITextField alertTextField = null;
            alertController.AddTextField(textField =>
            {
                textField.Placeholder = defaultText;
                alertTextField = textField;
            });

            alertController.AddAction(UIAlertAction.Create("Ok", UIAlertActionStyle.Default, okAction =>
            {

                completionHandler(alertTextField.Text);

            }));

            alertController.AddAction(UIAlertAction.Create("Cancel", UIAlertActionStyle.Default, cancelAction =>
            {

                completionHandler(null);

            }));

            UIApplication.SharedApplication.KeyWindow.RootViewController.PresentViewController(alertController, true, null);
        }


        protected override void OnElementPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);
            if (e.PropertyName == nameof(Element.Uri))
            {
                //await LoadHtmlFromSafe();
                //Control.Url = new NSUrl(Element.Uri);
                Control.LoadRequest(new NSUrlRequest(new NSUrl(Element.Uri)));
            }
        }

        private async Task LoadHtmlFromSafe()
        {
            var safeResponse = await WebFetchService.FetchResourceAsync(Element.Uri.ToString());
            string decodedString = Encoding.UTF8.GetString(safeResponse.Data);
            Control.LoadHtmlString(decodedString, new NSUrl(Element.Uri));
            //var imageLinks = LinkFinder.FindImages(decodedString);
            //if (imageLinks.Count > 0)
            //{
            //    foreach (var item in imageLinks)
            //    {
            //        var td = await WebFetchService.FetchResourceAsync(Element.Uri + "/" + item.Href.ToString());
            //        var st = Convert.ToBase64String(td.Data);
            //        var data = $"{td.MimeType};base64,{st}";
            //        Control.LoadData(new NSData(data, NSDataBase64DecodingOptions.None),
            //            td.MimeType,
            //            "UTF-8", new NSUrl(Element.Uri + "/" + item.Href));
            //    };
            //}
        }
    }
}
