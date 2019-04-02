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
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            if (_viewModel == null)
                _viewModel = new HomePageViewModel();

            BindingContext = _viewModel;

            InitilizeTapGestures();
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

        private void InitilizeTapGestures()
        {
            //var tabCountFrameTapGestureRecognizer = new TapGestureRecognizer() { NumberOfTapsRequired = 1 };
            //tabCountFrameTapGestureRecognizer.Tapped += (s, e) =>
            //{
            //    if (this.SlideMenu.IsShown)
            //    {
            //        this.HideMenu();
            //    }
            //    else
            //    {
            //        this.ShowMenu();
            //    }
            //};
            //TabCountFrame.GestureRecognizers.Add(tabCountFrameTapGestureRecognizer);

            //var refreshTapGestureRecognizer = new TapGestureRecognizer() { NumberOfTapsRequired = 1 };
            //refreshTapGestureRecognizer.Tapped += (s, e) =>
            //{

            //};
            //AddressBarButton.GestureRecognizers.Add(refreshTapGestureRecognizer);

            //var menuTapGestureRecognizer = new TapGestureRecognizer() { NumberOfTapsRequired = 1 };
            //menuTapGestureRecognizer.Tapped += (s, e) =>
            //{

            //};
            //SettingsButton.GestureRecognizers.Add(menuTapGestureRecognizer);
        }
    }
}
