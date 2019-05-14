using System;
using Foundation;
using WebKit;
using UIKit;
using SafeMobileBrowser.Controls;

namespace SafeMobileBrowser.iOS.ControlRenderers
{
    class HybridNavigationDelegate : WKNavigationDelegate
    {

        readonly WeakReference<HybridWebViewRenderer> Reference;

        public HybridNavigationDelegate(HybridWebViewRenderer renderer)
        {
            Reference = new WeakReference<HybridWebViewRenderer>(renderer);
        }

        public bool AttemptOpenCustomUrlScheme(NSUrl url)
        {
            var app = UIApplication.SharedApplication;

            if (app.CanOpenUrl(url))
                return app.OpenUrl(url);

            return false;
        }

        [Export("webView:decidePolicyForNavigationAction:decisionHandler:")]
        public override void DecidePolicy(WKWebView webView, WKNavigationAction navigationAction, Action<WKNavigationActionPolicy> decisionHandler)
        {
            if (Reference == null || !Reference.TryGetTarget(out HybridWebViewRenderer renderer)) return;
            if (renderer.Element == null) return;

            renderer.Element.IsLoading = true;
        }

        public override void DecidePolicy(WKWebView webView, WKNavigationResponse navigationResponse, Action<WKNavigationResponsePolicy> decisionHandler)
        {
            if (Reference == null || !Reference.TryGetTarget(out HybridWebViewRenderer renderer)) return;
            if (renderer.Element == null) return;

            if (navigationResponse.Response is NSHttpUrlResponse)
            {
                var code = ((NSHttpUrlResponse)navigationResponse.Response).StatusCode;
                if (code >= 400)
                {
                    renderer.Element.IsLoading = false;
                    decisionHandler(WKNavigationResponsePolicy.Cancel);
                    return;
                }
            }

            decisionHandler(WKNavigationResponsePolicy.Allow);
        }

        [Export("webView:didFinishNavigation:")]
        public async override void DidFinishNavigation(WKWebView webView, WKNavigation navigation)
        {
            if (Reference == null || !Reference.TryGetTarget(out HybridWebViewRenderer renderer)) return;
            if (renderer.Element == null) return;

            await renderer.OnJavascriptInjectionRequest(HybridWebView.InjectedFunction);

            if (renderer.Element.EnableGlobalCallbacks)
                foreach (var function in HybridWebView.GlobalRegisteredCallbacks)
                    await renderer.OnJavascriptInjectionRequest(HybridWebView.GenerateFunctionScript(function.Key));

            foreach (var function in renderer.Element.LocalRegisteredCallbacks)
                await renderer.OnJavascriptInjectionRequest(HybridWebView.GenerateFunctionScript(function.Key));
        }
    }
}
