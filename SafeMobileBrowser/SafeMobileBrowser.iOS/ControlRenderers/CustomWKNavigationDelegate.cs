using System;

using Foundation;
using UIKit;
using WebKit;

namespace SafeMobileBrowser.iOS.ControlRenderers
{
    public class CustomWKNavigationDelegate : WKNavigationDelegate
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
