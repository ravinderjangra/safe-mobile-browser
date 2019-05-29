using System;
using System.IO;
using System.Threading.Tasks;
using Foundation;
using SafeMobileBrowser.WebFetchImplementation;
using WebKit;

namespace SafeMobileBrowser.iOS.ControlRenderers
{
    public class SafeSchemaHandler : NSObject, IWKUrlSchemeHandler
    {
        [Export("webView:startURLSchemeTask:")]
        public void StartUrlSchemeTask(WKWebView webView, IWKUrlSchemeTask urlSchemeTask)
        {
            Task.Run(async () =>
            {
                var saferesponse = await WebFetchService.FetchResourceAsync(urlSchemeTask.Request.Url.ToString());
                Stream stream = new MemoryStream(saferesponse.Data);
                var response = new NSUrlResponse(urlSchemeTask.Request.Url, saferesponse.MimeType, (nint)stream.Length, null);
                urlSchemeTask.DidReceiveResponse(response);
                urlSchemeTask.DidReceiveData(NSData.FromStream(stream));
                urlSchemeTask.DidFinish();
            }).Wait();
        }

        [Export("webView:stopURLSchemeTask:")]
        public void StopUrlSchemeTask(WKWebView webView, IWKUrlSchemeTask urlSchemeTask)
        {
            throw new NotImplementedException();
        }

    }
}
