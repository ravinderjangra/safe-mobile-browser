using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Acr.UserDialogs;
using Rg.Plugins.Popup.Extensions;
using SafeMobileBrowser.Helpers;
using SafeMobileBrowser.Models;
using SafeMobileBrowser.Services;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace SafeMobileBrowser.ViewModels
{
    public class MenuPopUpViewModel : BaseNavigationViewModel
    {
        BookmarksModalPageViewModel _bookmarksModalPageViewModel;
        SettingsModalPageViewModel _settingsModalPageViewModel;

        public AsyncCommand RefreshWebViewCommand { get; set; }

        public AsyncCommand ManageBookmarkCommand { get; set; }

        private bool _checkIfBookmarked;

        public bool CheckIfAlreadyAvailableInBookmark
        {
            get => _checkIfBookmarked;
            set => RaiseAndUpdate(ref _checkIfBookmarked, value);
        }

        private ObservableCollection<PopUpMenuItem> _popMenuItems;

        public ObservableCollection<PopUpMenuItem> PopMenuItems
        {
            get => _popMenuItems;
            set => RaiseAndUpdate(ref _popMenuItems, value);
        }

        private PopUpMenuItem _reloadMenuItem;

        public PopUpMenuItem ReloadMenuItem
        {
            get => _reloadMenuItem;
            set => RaiseAndUpdate(ref _reloadMenuItem, value);
        }

        private PopUpMenuItem _bookmarkMenuItem;

        public PopUpMenuItem BookmarkMenuItem
        {
            get => _bookmarkMenuItem;
            set => RaiseAndUpdate(ref _bookmarkMenuItem, value);
        }

        private PopUpMenuItem _selectedPopMenuItem;

        public PopUpMenuItem SelectedPopMenuItem
        {
            get => _selectedPopMenuItem;

            set
            {
                RaiseAndUpdate(ref _selectedPopMenuItem, value);
                if (value != null)
                {
                    OnPopupMenuSelection();
                }
            }
        }

        public MenuPopUpViewModel()
        {
            Navigation = navigation;
            RefreshWebViewCommand = new Command(RefreshWebView);
            ManageBookmarkCommand = new Command(AddOrRemoveBookmark);
            InitaliseMenuItems();
        }

        private async Task AddOrRemoveBookmarkAsync()
        {
            if (!App.IsConnectedToInternet)
            {
                App.Current.MainPage.DisplayAlert("No internet connection", "Please connect to the internet", "Ok");
                return;
            }
            if (CheckIfAlreadyAvailableInBookmark)
                await RemoveBookmarkAsync();
            else
                await AddBookmarkToSAFEAsync();
        }

        private async Task RemoveBookmarkAsync()
        {
            var currentUrl = HomePageViewModel.CurrentUrl.Replace("https", "safe");
            if (!string.IsNullOrWhiteSpace(currentUrl) && AppService.IsSessionAvailable)
            {
                Task.Run(async () =>
                {
                    try
                    {
                        using (UserDialogs.Instance.Toast("Bookmark removed successfully"))
                        {
                            await BookmarkManager.DeleteBookmarks(currentUrl);
                            CheckIsBookmarkAvailable();
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(ex);
                        UserDialogs.Instance.Toast("Failed to remove bookmark");
                    }
                });
            }
            await Navigation.PopPopupAsync(true);
        }

        private async Task AddBookmarkToSAFEAsync()
        {
            var currentUrl = HomePageViewModel.CurrentUrl;
            if (!string.IsNullOrWhiteSpace(currentUrl) && AppService.IsSessionAvailable)
            {
                Task.Run(async () =>
                {
                    try
                    {
                        using (UserDialogs.Instance.Toast("Bookmark added successfully"))
                        {
                            await BookmarkManager.AddBookmark(currentUrl.Replace("https", "safe"));
                            CheckIsBookmarkAvailable();
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(ex);
                        UserDialogs.Instance.Toast("Failed to add bookmark");
                    }
                });
            }
            await Navigation.PopPopupAsync(true);
        }

        internal void UpdateMenuItemsStatus()
        {
            CheckIsBookmarkAvailable();
            UpdatePopMenuItemStates();
        }

        internal void UpdatePopMenuItemStates()
        {
            var shareMenuItem = PopMenuItems.FirstOrDefault(p => string.Equals(p.MenuItemTitle, "Share"));

            if (!string.IsNullOrWhiteSpace(HomePageViewModel.CurrentUrl))
            {
                shareMenuItem.IsEnabled = true;
                ReloadMenuItem.IsEnabled = true;
                BookmarkMenuItem.IsEnabled = true;
            }
            else
            {
                shareMenuItem.IsEnabled = false;
                ReloadMenuItem.IsEnabled = false;
                BookmarkMenuItem.IsEnabled = false;
            }

            BookmarkMenuItem.MenuItemIcon = CheckIfAlreadyAvailableInBookmark ? IconFont.Bookmark : IconFont.BookmarkOutline;
            BookmarkMenuItem.IsEnabled = AppService.IsSessionAvailable;

            var bookmarksMenuItem = PopMenuItems.FirstOrDefault(p => string.Equals(p.MenuItemTitle, "Bookmarks"));
            bookmarksMenuItem.IsEnabled = AppService.IsSessionAvailable;

            var authenticationMenuItem = PopMenuItems.FirstOrDefault(p => string.Equals(p.MenuItemTitle, "Authenticate"));
            authenticationMenuItem.IsEnabled = !AppService.IsSessionAvailable;
        }

        internal void CheckIsBookmarkAvailable()
        {
            if (AppService.IsSessionAvailable && !string.IsNullOrWhiteSpace(HomePageViewModel.CurrentUrl))
            {
                var currentUrl = HomePageViewModel.CurrentUrl.Replace("https", "safe");
                CheckIfAlreadyAvailableInBookmark = BookmarkService.CheckIfBookmarkAvailable(currentUrl);
            }
            else
            {
                CheckIfAlreadyAvailableInBookmark = false;
            }
        }

        private async Task RefreshWebViewAsync(object obj)
        {
            if (!App.IsConnectedToInternet)
            {
                App.Current.MainPage.DisplayAlert("No internet connection", "Please connect to the internet", "Ok");
                return;
            }
            var currentUrl = HomePageViewModel.CurrentUrl;
            if (!string.IsNullOrWhiteSpace(currentUrl))
            {
                MessagingCenter.Send(
                    this,
                    MessageCenterConstants.ReloadMessage);
            }
            await Navigation.PopPopupAsync(true);
        }

        internal void InitaliseMenuItems()
        {
            PopMenuItems = new ObservableCollection<PopUpMenuItem>
            {
                new PopUpMenuItem { MenuItemTitle = "Settings", MenuItemIcon = IconFont.Settings, IsEnabled = true },
                new PopUpMenuItem { MenuItemTitle = "Bookmarks", MenuItemIcon = IconFont.BookmarkPlusOutline, },
                new PopUpMenuItem { MenuItemTitle = "Authenticate", MenuItemIcon = IconFont.Web, IsEnabled = true },
                new PopUpMenuItem { MenuItemTitle = "Share", MenuItemIcon = IconFont.ShareVariant }
            };

            ReloadMenuItem = new PopUpMenuItem { MenuItemTitle = "Reload", MenuItemIcon = IconFont.Reload };
            BookmarkMenuItem = new PopUpMenuItem { MenuItemTitle = "Bookmark", MenuItemIcon = IconFont.BookmarkOutline };
        }

        private async void OnPopupMenuSelection()
        {
            try
            {
                var selectedMenuItemTitle = SelectedPopMenuItem.MenuItemTitle;
                switch (selectedMenuItemTitle)
                {
                    case "Settings":
                        if (_settingsModalPageViewModel == null)
                            _settingsModalPageViewModel = new SettingsModalPageViewModel();
                        await Navigation.PushModalAsync(_settingsModalPageViewModel);
                        break;
                    case "Bookmarks":
                        if (AppService.IsSessionAvailable)
                        {
                            if (_bookmarksModalPageViewModel == null)
                                _bookmarksModalPageViewModel = new BookmarksModalPageViewModel();
                            await Navigation.PushModalAsync(_bookmarksModalPageViewModel);
                        }
                        else
                        {
                            throw new Exception("Please authenticate");
                        }
                        break;
                    case "Authenticate":
                        if (!AppService.IsSessionAvailable)
                        {
                            await AuthenticationService.RequestNonMockAuthenticationAsync();
                        }
                        break;
                    case "Share":
                        if (HomePageViewModel.CurrentTitle != null && !HomePageViewModel.CurrentTitle.StartsWith("file://"))
                        {
                            await Share.RequestAsync(new ShareTextRequest
                            {
                                Title = HomePageViewModel.CurrentTitle,
                                Uri = HomePageViewModel.CurrentUrl
                            });
                        }
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                MessagingCenter.Send(
                    this,
                    MessageCenterConstants.DisplayAlertMessage,
                    ex.Message);
                Debug.WriteLine(ex);
            }

            // TODO: delay in popup. Needs refactoring
            await Navigation.PopPopupAsync(true);
            SelectedPopMenuItem = null;
        }
    }
}
