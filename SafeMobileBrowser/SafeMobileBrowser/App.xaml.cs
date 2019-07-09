﻿using SafeApp;
using SafeMobileBrowser.Services;
using SafeMobileBrowser.ViewModels;
using SafeMobileBrowser.Views;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace SafeMobileBrowser
{
    public partial class App : Application
    {
        public static Session AppSession { get; set; }

        public static bool IsConnectedToInternet { get; set; }

        public App()
        {
            InitializeComponent();
            RegisterServices();
            MainPage = new NavigationPage(new HomePage()) { BarBackgroundColor = Color.White };
            IsConnectedToInternet = Connectivity.NetworkAccess == NetworkAccess.Internet;
            Connectivity.ConnectivityChanged += Connectivity_ConnectivityChanged;
        }

        void Connectivity_ConnectivityChanged(object sender, ConnectivityChangedEventArgs e)
        {
            if (e.NetworkAccess == NetworkAccess.Internet)
            {
                IsConnectedToInternet = true;
                AppSession.ReconnectAsync();
            }
            else
            {
                IsConnectedToInternet = false;
            }
        }

        private void RegisterServices()
        {
            var navigationService = new NavigationService();
            navigationService.RegisterViewModels(GetType().Assembly);
            BaseNavigationViewModel.RegisterService(navigationService);
            ServiceContainer.Register(() => new AppService());
            ServiceContainer.Register(() => new BookmarkService());
        }

        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}
