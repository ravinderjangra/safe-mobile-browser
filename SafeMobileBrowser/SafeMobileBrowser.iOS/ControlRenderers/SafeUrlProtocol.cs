using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        private NSUrlConnection connection;
        private SafeConnectionDelegate connDelegate;


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


        [Export("initWithRequest:cachedResponse:client:")]
        public SafeUrlProtocol(NSUrlRequest request, NSCachedUrlResponse cachedResponse, INSUrlProtocolClient client)
            : base(request, cachedResponse, client)
        {
        }

        public override void StartLoading()
        {
            if (null == connDelegate)
            {
                connDelegate = new SafeConnectionDelegate(this);
            }
            this.connection = new NSUrlConnection(Request, connDelegate, true);

            var request = Request.Url;
            var saferesponse = WebFetchService.FetchResourceAsync(request.ToString()).Result;
            Stream stream = new MemoryStream(saferesponse.Data);
            var response = new NSUrlResponse(request, saferesponse.MimeType, -1, null);
            Client.ReceivedResponse(this, response, NSUrlCacheStoragePolicy.NotAllowed);
            this.InvokeOnMainThread(delegate
            {
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

        ~SafeUrlProtocol()
        {
            Debug.WriteLine("Log: Destroyed SafeUrlProtocol");
        }

        private class SafeConnectionDelegate : NSUrlConnectionDataDelegate
        {
            private SafeUrlProtocol handler;

            public SafeConnectionDelegate(SafeUrlProtocol handler)
            {
                this.handler = handler;
            }
            public override void ReceivedData(NSUrlConnection connection, NSData data)
            {
                handler.Client.DataLoaded(handler, data);
            }
            public override void FailedWithError(NSUrlConnection connection, NSError error)
            {
                handler.Client.FailedWithError(handler, error);
                connection.Cancel();
            }
            public override void ReceivedResponse(NSUrlConnection connection, NSUrlResponse response)
            {
                handler.Client.ReceivedResponse(handler, response, NSUrlCacheStoragePolicy.NotAllowed);
            }
            public override void FinishedLoading(NSUrlConnection connection)
            {
                handler.Client.FinishedLoading(handler);
                connection.Cancel();
            }
        }
    }
}