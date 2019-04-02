using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Text;
using Android.Content;
using Android.OS;
using Android.Webkit;
using SafeMobileBrowser.Controls;
using SafeMobileBrowser.Droid.ControlRenderers;
using SafeMobileBrowser.Droid.PlatformServices;
using SafeMobileBrowser.Helpers;
using SafeMobileBrowser.Models;
using SafeMobileBrowser.WebFetchImplementation;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using AWeb = Android.Webkit;

[assembly: ExportRenderer(typeof(HybridWebView), typeof(HybridWebViewRenderer))]
namespace SafeMobileBrowser.Droid.ControlRenderers
{
    public class HybridWebViewRenderer : ViewRenderer<HybridWebView, AWeb.WebView>
    {
        //const string JavaScriptFunction = "function invokeCSharpAction(data){jsBridge.invokeAction(data);} ; ";
        private readonly Context _context;
        private readonly bool _ignoreSourceChanges;

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
                webView.Settings.JavaScriptEnabled = false;
                webView.Settings.MediaPlaybackRequiresUserGesture = false;
                webView.Settings.MixedContentMode = MixedContentHandling.AlwaysAllow;
                webView.Settings.CacheMode = CacheModes.NoCache;
                webView.Settings.AllowContentAccess = true;
                webView.Settings.DomStorageEnabled = true;
                webView.SetWebViewClient(new CustomWebViewClient(this));
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
                // Subscribe
                if (_ignoreSourceChanges)
                {
                    return;
                }

                Control.LoadUrl("file:///android_asset/startbrowsing.html");
                //Control.LoadUrl(string.Format(Element.Uri));
            }
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);
            switch (e.PropertyName)
            {
                case "Uri":
                    if (_ignoreSourceChanges)
                    {
                        return;
                    }
                    Control.LoadUrl(string.Format(Element.Uri));
                    break;
                default:
                    break;
            }
        }

        private class CustomWebViewClient : WebViewClient
        {
            private HybridWebViewRenderer _renderer;
            private WebNavigationResult _navigationResult = WebNavigationResult.Success;

            public CustomWebViewClient(HybridWebViewRenderer renderer)
            {
                _renderer = renderer ?? throw new ArgumentNullException("Renderer");
            }

            //public override WebResourceResponse ShouldInterceptRequest(AWeb.WebView view, IWebResourceRequest request)
            //{
            //    try
            //    {
            //        System.Diagnostics.Debug.WriteLine(request.Url.ToString());
            //        var headers = request.RequestHeaders;
            //        if (request.Url.Scheme.ToLower().Contains("http") && !request.Url.ToString().ToLower().Contains("favicon"))
            //        {
            //            if (headers.ContainsKey("Range"))
            //            {
            //                var options = new WebFetchOptions
            //                {
            //                    Range = RequestHelpers.RangeStringToArray(headers["Range"])
            //                };
            //                var saferesponse = WebFetchService.FetchResourceAsync(request.Url.ToString(), options).Result;
            //                Stream stream = new MemoryStream(saferesponse.Data);
            //                //var contentType = "video/mp4";
            //                //var contentLength = saferesponse.Data.Length;
            //                //var contentStart = options?.Range[0].Start;
            //                //var contentEnd = options?.Range[0].End > 0 ? options?.Range[0].End : contentLength - 1;
            //                WebResourceResponse response = new WebResourceResponse(saferesponse.MimeType, "UTF-8", stream);
            //                response.SetStatusCodeAndReasonPhrase(206, "Partial Content");
            //                response.ResponseHeaders = new Dictionary<string, string>
            //                {
            //                    { "Accept-Ranges", "bytes"},
            //                    { "content-type", saferesponse.MimeType },
            //                    { "Content-Range", saferesponse.Headers["Content-Range"]},
            //                    { "Content-Length", saferesponse.Headers["Content-Length"]},
            //                };
            //                return response;
            //            }
            //            else
            //            {
            //                var saferesponse = WebFetchService.FetchResourceAsync(request.Url.ToString()).Result;
            //                Stream stream = new MemoryStream(saferesponse.Data);
            //                WebResourceResponse response = new WebResourceResponse(saferesponse.MimeType, "UTF-8", stream);
            //                return response;
            //            }
            //        }
            //    }
            //    catch (Exception ex)
            //    {
            //        System.Diagnostics.Debug.WriteLine("Error: " + ex.Message);

            //        if (ex.InnerException != null)
            //        {
            //            var exception = ex.InnerException as WebFetchException;
            //            System.Diagnostics.Debug.WriteLine("Error Code: " + exception.ErrorCode);
            //            System.Diagnostics.Debug.WriteLine("Error Message: " + exception.Message);

            //            if (exception.ErrorCode == WebFetchConstants.NoSuchData ||
            //                exception.ErrorCode == WebFetchConstants.NoSuchEntry ||
            //                exception.ErrorCode == WebFetchConstants.NoSuchPublicName ||
            //                exception.ErrorCode == WebFetchConstants.NoSuchServiceName)
            //            {
            //                var htmlString = GetAssetsFileData.ReadHtmlFile("notfound.html");
            //                byte[] byteArray = Encoding.ASCII.GetBytes(htmlString);
            //                MemoryStream stream = new MemoryStream(byteArray);
            //                var response = new WebResourceResponse("text/html", "UTF-8", stream);
            //                return response;
            //            }
            //        }
            //    }
            //    //var emptyStream = new MemoryStream(0);
            //    //return new WebResourceResponse("text/plain", "utf-8", emptyStream);
            //    return base.ShouldInterceptRequest(view, request);
            //}

            public override WebResourceResponse ShouldInterceptRequest(AWeb.WebView view, string url)
            {
                try
                {
                    if (url.Contains("favicon.ico"))
                    {
                        return base.ShouldInterceptRequest(view, url);
                        //var faviconEmptyStream = new MemoryStream(0);
                        //return new WebResourceResponse("image/x-icon", "utf-8", faviconEmptyStream);
                    }

                    var saferesponse = WebFetchService.FetchResourceAsync(url).Result;
                    var stream = new MemoryStream(saferesponse.Data);
                    var response = new WebResourceResponse(saferesponse.MimeType, "UTF-8", stream);
                    return response;
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine("Error: " + ex.Message);

                    if (ex.InnerException != null)
                    {
                        var exception = ex.InnerException as WebFetchException;
                        System.Diagnostics.Debug.WriteLine("Error Code: " + exception.ErrorCode);
                        System.Diagnostics.Debug.WriteLine("Error Message: " + exception.Message);

                        if (exception.ErrorCode == WebFetchConstants.NoSuchData ||
                            exception.ErrorCode == WebFetchConstants.NoSuchEntry ||
                            exception.ErrorCode == WebFetchConstants.NoSuchPublicName ||
                            exception.ErrorCode == WebFetchConstants.NoSuchServiceName)
                        {
                            var htmlString = GetAssetsFileData.ReadHtmlFile("notfound.html");
                            byte[] byteArray = Encoding.ASCII.GetBytes(htmlString);
                            MemoryStream stream = new MemoryStream(byteArray);
                            var response = new WebResourceResponse("text/html", "UTF-8", stream);
                            return response;
                        }
                    }
                }
                var emptyStream = new MemoryStream(0);
                return new WebResourceResponse("text/plain", "utf-8", emptyStream);
            }

            protected override void Dispose(bool disposing)
            {
                base.Dispose(disposing);
                if (disposing)
                {
                    _renderer = null;
                }
            }
        }
    }
}
