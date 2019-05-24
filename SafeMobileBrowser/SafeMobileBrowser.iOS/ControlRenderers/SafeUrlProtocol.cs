using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using Foundation;
using SafeMobileBrowser.Models;
using SafeMobileBrowser.WebFetchImplementation;
using UIKit;

namespace SafeMobileBrowser.iOS.ControlRenderers
{
    public class SafeUrlProtocol : NSUrlProtocol
    {
        [Export("canInitWithRequest:")]
        public static bool canInitWithRequest(NSUrlRequest request)
        {
            return request.Url.Scheme == "safe";
        }

        [Export("canonicalRequestForRequest:")]
        public static new NSUrlRequest GetCanonicalRequest(NSUrlRequest request)
        {
            return request;
        }

        public override void StartLoading()
        {
            var request = Request.Url;
            var options = new WebFetchOptions();

            var saferesponse = WebFetchService.FetchResourceAsync(request.ToString()).Result;
            Stream stream = new MemoryStream(saferesponse.Data);
            var response = new NSUrlResponse(request, saferesponse.MimeType, -1, null);
            Client.ReceivedResponse(this, response, NSUrlCacheStoragePolicy.NotAllowed);
            this.InvokeOnMainThread(delegate {
                using (var data = NSData.FromArray(saferesponse.Data))
                {
                    Client.DataLoaded(this, data);
                }
                Client.FinishedLoading(this);
            });
        }

        public override void StopLoading()
        {
        }
    }
}