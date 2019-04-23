using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Android.Graphics;
using Android.Runtime;
using Android.Webkit;
using SafeMobileBrowser.Droid.PlatformServices;
using SafeMobileBrowser.Helpers;
using SafeMobileBrowser.Models;
using SafeMobileBrowser.WebFetchImplementation;
using Xamarin.Forms;
using AWeb = Android.Webkit;

namespace SafeMobileBrowser.Droid.ControlRenderers
{
    public class CustomWebViewClient : WebViewClient
    {
        private HybridWebViewRenderer _renderer;
        public bool IsLoading;
        public bool IsRedirecting;

        public CustomWebViewClient(HybridWebViewRenderer renderer) => _renderer = renderer ?? throw new ArgumentNullException("null renderer");

        protected CustomWebViewClient(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
        {
        }

        public override WebResourceResponse ShouldInterceptRequest(AWeb.WebView view, IWebResourceRequest request)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine(request.Url.ToString());
                var headers = request.RequestHeaders;
                if (request.Url.Scheme.ToLower().Contains("http") && !request.Url.ToString().ToLower().Contains("favicon"))
                {
                    if (headers.ContainsKey("Range"))
                    {
                        var options = new WebFetchOptions
                        {
                            Range = RequestHelpers.RangeStringToArray(headers["Range"])
                        };
                        var saferesponse = WebFetchService.FetchResourceAsync(request.Url.ToString(), options).Result;
                        Stream stream = new MemoryStream(saferesponse.Data);
                        //var contentType = "video/mp4";
                        //var contentLength = saferesponse.Data.Length;
                        //var contentStart = options?.Range[0].Start;
                        //var contentEnd = options?.Range[0].End > 0 ? options?.Range[0].End : contentLength - 1;
                        WebResourceResponse response = new WebResourceResponse(saferesponse.MimeType, "UTF-8", stream);
                        response.SetStatusCodeAndReasonPhrase(206, "Partial Content");
                        response.ResponseHeaders = new Dictionary<string, string>
                        {
                            { "Accept-Ranges", "bytes"},
                            { "content-type", saferesponse.MimeType },
                            { "Content-Range", saferesponse.Headers["Content-Range"]},
                            { "Content-Length", saferesponse.Headers["Content-Length"]},
                        };
                        return response;
                    }
                    else
                    {
                        var saferesponse = WebFetchService.FetchResourceAsync(request.Url.ToString()).Result;
                        Stream stream = new MemoryStream(saferesponse.Data);
                        WebResourceResponse response = new WebResourceResponse(saferesponse.MimeType, "UTF-8", stream);
                        return response;
                    }
                }
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
            //var emptyStream = new MemoryStream(0);
            //return new WebResourceResponse("text/plain", "utf-8", emptyStream);
            return base.ShouldInterceptRequest(view, request);
        }

<<<<<<< HEAD
=======
<<<<<<< Updated upstream
        //public override bool ShouldOverrideUrlLoading(AWeb.WebView view, string url)
        //{
        //    if (!IsRedirecting)
        //        IsRedirecting = true;
        //    UpdateLoadingState(true);
        //    return base.ShouldOverrideUrlLoading(view, url);
        //}
=======
        //public override WebResourceResponse ShouldInterceptRequest(AWeb.WebView view, string url)
        //{
        //    try
        //    {
        //        if (url.Contains("favicon.ico"))
        //        {
        //            return base.ShouldInterceptRequest(view, url);
        //        }

        //        var saferesponse = WebFetchService.FetchResourceAsync(url).Result;
        //        var stream = new MemoryStream(saferesponse.Data);
        //        var response = new WebResourceResponse(saferesponse.MimeType, "UTF-8", stream);
        //        return response;
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
        //    var emptyStream = new MemoryStream(0);
        //    return new WebResourceResponse("text/plain", "utf-8", emptyStream);
        //}

>>>>>>> Stashed changes
>>>>>>> 9e20af1... a

        public override void OnPageStarted(AWeb.WebView view, string url, Bitmap favicon)
        {
            _renderer.Element.IsLoading = true;
            base.OnPageStarted(view, url, favicon);
        }

        public override void OnPageFinished(AWeb.WebView view, string url)
        {
            _renderer.Element.IsLoading = false;
            base.OnPageFinished(view, url);
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