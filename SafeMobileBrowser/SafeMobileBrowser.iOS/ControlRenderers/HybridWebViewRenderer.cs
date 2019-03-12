using Foundation;
using SafeMobileBrowser.Controls;
using SafeMobileBrowser.iOS.ControlRenderers;
using SafeMobileBrowser.Services;
using System;
using System.Text;
using System.Threading.Tasks;
using UIKit;
using WebKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(HybridWebView), typeof(HybridWebViewRenderer))]
namespace SafeMobileBrowser.iOS.ControlRenderers
{
    public class HybridWebViewRenderer : ViewRenderer<HybridWebView, WKWebView>
    {
        private WKUserContentController _userController;
        private bool _ignoreSourceChanges;

        protected override void OnElementChanged(ElementChangedEventArgs<HybridWebView> e)
        {
            base.OnElementChanged(e);

            if (Control == null && e.NewElement != null)
            {
                _userController = new WKUserContentController();

                var config = new WKWebViewConfiguration { UserContentController = _userController };
                var webView = new WKWebView(Frame, config)
                {
                    NavigationDelegate = new CustomWKNavigationDelegate()
                };
                SetNativeControl(webView);
            }
        }

        protected override async void OnElementPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);
            if (e.PropertyName == nameof(Element.Uri))
            {
                await LoadHtmlFromSafe();
            }
        }

        private async Task LoadHtmlFromSafe()
        {
            if (_ignoreSourceChanges)
            {
                return;
            }

            var safeResponse = await WebFetchService.FetchResourceAsync(Element.Uri.ToString());
            var base64String = Convert.ToBase64String(safeResponse.Data);
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

        private class CustomWKNavigationDelegate : WKNavigationDelegate
        {
            public override void DecidePolicy(WKWebView webView, WKNavigationAction navigationAction, Action<WKNavigationActionPolicy> decisionHandler)
            {
                try
                {
                    var policy = WKNavigationActionPolicy.Allow;
                    if (navigationAction.NavigationType == WKNavigationType.LinkActivated)
                    {
                        var url = new NSUrl(navigationAction.Request.Url.ToString());
                        if (UIApplication.SharedApplication.CanOpenUrl(url))
                        {
                            UIApplication.SharedApplication.OpenUrl(url);
                            policy = WKNavigationActionPolicy.Cancel;
                        }
                    }
                    decisionHandler?.Invoke(policy);
                }
                catch
                {
                    base.DecidePolicy(webView, navigationAction, decisionHandler);
                }
            }

            public override void DidReceiveAuthenticationChallenge(WKWebView webView, NSUrlAuthenticationChallenge challenge, Action<NSUrlSessionAuthChallengeDisposition, NSUrlCredential> completionHandler)
            {
                completionHandler?.Invoke(NSUrlSessionAuthChallengeDisposition.PerformDefaultHandling, null);
            }
        }
    }
}