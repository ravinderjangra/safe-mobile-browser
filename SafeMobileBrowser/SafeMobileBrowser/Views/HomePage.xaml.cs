using SafeMobileBrowser.Models;
using SafeMobileBrowser.ViewModels;
using System;
using System.Collections.Generic;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SafeMobileBrowser.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class HomePage : ContentPage
    {
        List<string> _websiteList;
        HomePageViewModel _viewModel;
        public HomePage()
        {
            InitializeComponent();
            BindingContext = new HomePageViewModel();
            HybridWebViewControl.Navigating += (s, e) =>
            {
                _viewModel.WebViewNavigatingCommand.Execute(e);
            };

            HybridWebViewControl.Navigated += (s, e) =>
            {
                _viewModel.WebViewNavigatedCommand.Execute(e);
            };
        }               

        protected override async  void OnAppearing()
        {
            base.OnAppearing();

            if (_viewModel == null)
            {
                _viewModel = new HomePageViewModel();
                await _viewModel.InitilizeSessionAsync();
            }

            BindingContext = _viewModel;

            if (Device.RuntimePlatform == Device.Android)
                AddWebsiteList();

            AddressBarEntry.Completed += (s, e) =>
            {
                _viewModel.PageLoadCommand.Execute(null);
            };
        }

        private void AddWebsiteList()
        {
            if (_websiteList == null)
                _websiteList = WebsiteList.GetWebsiteList();

            if (ToolbarItems.Count == 0)
                foreach (var url in _websiteList)
                {
                    var item = new ToolbarItem
                    {
                        Order = ToolbarItemOrder.Secondary,
                        Text = url,
                        CommandParameter = url
                    };
                    item.SetBinding(MenuItem.CommandProperty, new Binding("ToolbarItemCommand"));
                    ToolbarItems.Add(item);
                }
        }

        public static string GetRGBFill(Xamarin.Forms.Color color)
        {
            var red = (int)(color.R * 255);
            var green = (int)(color.G * 255);
            var blue = (int)(color.B * 255);
            var rgbFill = $"fill: rgb({red},{green},{blue});";
            return rgbFill;
        }

        private void HybridWebViewControl_Navigating(object sender, WebNavigatingEventArgs e)
        {

        }
    }
}
