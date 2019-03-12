﻿using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Content;
using Xamarin.Forms;
using System;
using SafeMobileBrowser.Services;
using SafeMobileBrowser.Helpers;

namespace SafeMobileBrowser.Droid
{
    [Activity(
        Label = "SafeMobileBrowser",
        Icon = "@mipmap/icon",
        Theme = "@style/MainTheme",
        MainLauncher = true,
        LaunchMode = LaunchMode.SingleTask,
        ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation),
        IntentFilter(
            new[] { Intent.ActionView },
            Categories = new[] { Intent.CategoryDefault, Intent.CategoryBrowsable },
            DataScheme = Constants.AppId
        )]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        readonly AuthenticationService authenticationService = new AuthenticationService();

        protected override void OnCreate(Bundle savedInstanceState)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(savedInstanceState);

            global::Xamarin.Forms.Forms.SetFlags(
                "Shell_Experimental",
                "Visual_Experimental",
                "CollectionView_Experimental",
                "FastRenderers_Experimental");
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);

            LoadApplication(new App());
        }

        protected override void OnNewIntent(Intent intent)
        {
            base.OnNewIntent(intent);
            if (intent?.Data != null)
            {
                HandleAppLaunch(intent.Data.ToString());
            }
        }

        private void HandleAppLaunch(string url)
        {
            System.Diagnostics.Debug.WriteLine($"Launched via: {url}");
            Device.BeginInvokeOnMainThread(
              async () =>
              {
                  try
                  {
                      await authenticationService.ProcessAuthenticationResponseAsync(url);
                  }
                  catch (Exception ex)
                  {
                      System.Diagnostics.Debug.WriteLine($"Error: {ex.Message}");
                  }
              });
        }
    }
}