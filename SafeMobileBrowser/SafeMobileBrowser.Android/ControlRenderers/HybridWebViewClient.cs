using System;
using System.Collections.Generic;
using System.IO;
using Android.Webkit;
using Nito.AsyncEx.Synchronous;
using SafeMobileBrowser.Helpers;
using SafeMobileBrowser.Models;
using SafeMobileBrowser.WebFetchImplementation;
using Xamarin.Forms.Platform.Android;

namespace SafeMobileBrowser.Droid.ControlRenderers
{
    public class HybridWebViewClient : FormsWebViewClient
    {
        readonly WeakReference<HybridWebViewRenderer> _renderer;

        public HybridWebViewClient(HybridWebViewRenderer renderer)
            : base(renderer)
        {
            _renderer = new WeakReference<HybridWebViewRenderer>(renderer);
        }

        public override bool ShouldOverrideUrlLoading(WebView view, IWebResourceRequest request)
        {
            if (!request.Url.ToString().Contains("safe"))
                return base.ShouldOverrideUrlLoading(view, request);

            view.LoadUrl(request.Url.ToString().Replace("safe://", "https:"));
            return true;
        }

        public override WebResourceResponse ShouldInterceptRequest(WebView view, IWebResourceRequest request)
        {
            try
            {
                Logger.Info($"Requested Url: {request.Url.ToString()}");
                var urlToFetch = request.Url.ToString();
                var isHttpRequest = request.Url.Scheme == "https";
                if (isHttpRequest && !urlToFetch.Contains("favicon"))
                {
                    var requestHeaders = request.RequestHeaders;

                    if (requestHeaders.ContainsKey("Range"))
                    {
                        Logger.Info("Request Header");
                        foreach (var key in requestHeaders.Keys)
                        {
                            Logger.Info($"Key: {key}, Value: {requestHeaders[key]}");
                        }

                        var webFetchOptions = new WebFetchOptions()
                        {
                            RangeHeader = RequestHelpers.GetRangeFromHeaderRangeValue(requestHeaders["Range"])
                        };

                        var task = WebFetchService.FetchResourceAsync(urlToFetch, webFetchOptions);
                        var safeResponse = task.WaitAndUnwrapException();

                        var stream = new MemoryStream(safeResponse.Data);
                        var response = new WebResourceResponse(safeResponse.MimeType, "UTF-8", stream);

                        response.SetStatusCodeAndReasonPhrase(206, "Partial Content");
                        response.ResponseHeaders = new Dictionary<string, string>
                        {
                            { "Accept-Ranges", "bytes" },
                            { "Content-Type", safeResponse.MimeType },
                            { "Content-Range", safeResponse.Headers["Content-Range"] },
                            { "Content-Length", safeResponse.Headers["Content-Length"] },
                        };

                        var responseHeaders = response?.ResponseHeaders;
                        if (responseHeaders != null)
                        {
                            Logger.Info("Response Header");
                            foreach (var key in responseHeaders?.Keys)
                            {
                                Logger.Info($"Key: {key}, Value: {responseHeaders[key]}");
                            }
                        }
                        return response;
                    }
                    else
                    {
                        var safeResponse = WebFetchService.FetchResourceAsync(urlToFetch).Result;
                        var stream = new MemoryStream(safeResponse.Data);
                        var response = new WebResourceResponse(safeResponse.MimeType, "UTF-8", stream);
                        return response;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);

                if (ex.InnerException == null)
                    return base.ShouldInterceptRequest(view, request);

                using (var stream = new MemoryStream())
                {
                    var response = new WebResourceResponse("text/html", "UTF-8", stream);
                    response.SetStatusCodeAndReasonPhrase(404, "Not Found");
                    return response;
                }
            }
            return base.ShouldInterceptRequest(view, request);
        }
    }
}
