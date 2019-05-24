﻿using Foundation;
using SafeMobileBrowser.iOS.ControlRenderers;
using SafeMobileBrowser.Services;
using System;
using System.Diagnostics;
using UIKit;
using Xamarin.Forms;

namespace SafeMobileBrowser.iOS
{
    [Register("AppDelegate")]
    public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
    {
        readonly AuthenticationService authenticationService = new AuthenticationService();

        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            // Register custom SAFE protocol
            NSUrlProtocol.RegisterClass(new ObjCRuntime.Class(typeof(SafeUrlProtocol)));

            global::Xamarin.Forms.Forms.Init();
            global::Xamarin.Forms.FormsMaterial.Init();

            LoadApplication(new App());

            return base.FinishedLaunching(app, options);
        }

        public override bool OpenUrl(UIApplication app, NSUrl url, NSDictionary options)
        {
            Device.BeginInvokeOnMainThread(
              async () =>
              {
                  try
                  {
                      await authenticationService.ProcessAuthenticationResponseAsync(url.ToString());
                      Debug.WriteLine("IPC Msg Handling Completed");
                  }
                  catch (Exception ex)
                  {
                      Debug.WriteLine($"Error: {ex.Message}");
                  }
              });
            return true;
        }
    }
}
