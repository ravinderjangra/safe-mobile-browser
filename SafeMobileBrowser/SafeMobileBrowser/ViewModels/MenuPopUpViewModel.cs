using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Rg.Plugins.Popup.Extensions;
using SafeMobileBrowser.Helpers;
using SafeMobileBrowser.Models;
using SafeMobileBrowser.Services;
using SafeMobileBrowser.Views;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace SafeMobileBrowser.ViewModels
{
    public class MenuPopUpViewModel : BaseViewModel
    {
        BookmarksModalPage _bookmarksModalPage;
        SettingsModalPage _settingsModalPage;

        public INavigation Navigation { get; set; }

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

        public MenuPopUpViewModel(INavigation navigation)
        {
            Navigation = navigation;
            BookmarkManager = new BookmarkManager();
            RefreshWebViewCommand = new AsyncCommand(RefreshWebViewAsync);
            ManageBookmarkCommand = new AsyncCommand(AddOrRemoveBookmarkAsync);
            InitaliseMenuItems();
        }

        private async Task AddOrRemoveBookmarkAsync()
        {
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
                await BookmarkManager.DeleteBookmarks(currentUrl);
                CheckIsBookmarkAvailable();
            }
            await Navigation.PopPopupAsync();
        }

        private async Task AddBookmarkToSAFEAsync()
        {
            var currentUrl = HomePageViewModel.CurrentUrl;
            if (!string.IsNullOrWhiteSpace(currentUrl) && AppService.IsSessionAvailable)
            {
                await BookmarkManager.AddBookmark(currentUrl.Replace("https", "safe"));
                CheckIsBookmarkAvailable();
            }
            await Navigation.PopPopupAsync();
        }

        internal void UpdateMenuItemsStatus()
        {
            CheckIsBookmarkAvailable();
            UpdatePopMenuItemStates();
        }

        internal void UpdatePopMenuItemStates()
        {
            var shareMenuItem = PopMenuItems.Where(p => string.Equals(p.MenuItemTitle, "Share")).FirstOrDefault();

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
        }

        internal void CheckIsBookmarkAvailable()
        {
            if (AppService.IsSessionAvailable && !string.IsNullOrWhiteSpace(HomePageViewModel.CurrentUrl))
            {
                var currentUrl = HomePageViewModel.CurrentUrl.Replace("https", "safe");
                CheckIfAlreadyAvailableInBookmark = BookmarkManager.CheckIfBookmarkAvailable(currentUrl);
            }
            else
            {
                CheckIfAlreadyAvailableInBookmark = false;
            }
        }

        private async Task RefreshWebViewAsync(object obj)
        {
            var currentUrl = HomePageViewModel.CurrentUrl;
            if (!string.IsNullOrWhiteSpace(currentUrl))
            {
                MessagingCenter.Send(
                    this,
                    MessageCenterConstants.ReloadMessage);
            }
            await Navigation.PopPopupAsync();
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
                        if (_settingsModalPage == null)
                            _settingsModalPage = new SettingsModalPage();
                        await Navigation.PushModalAsync(_settingsModalPage);
                        break;
                    case "Bookmarks":
                        if (AppService.IsSessionAvailable)
                        {
                            if (_bookmarksModalPage == null)
                                _bookmarksModalPage = new BookmarksModalPage();
                            await Navigation.PushModalAsync(_bookmarksModalPage);
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
                            SelectedPopMenuItem.IsEnabled = false;
                            var bookmarksMenuItem = PopMenuItems.Where(p => string.Equals(p.MenuItemTitle, "Bookmarks")).FirstOrDefault();
                            bookmarksMenuItem.IsEnabled = true;
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

            // Todo: delay in popup. Needs refactoring
            await Navigation.PopPopupAsync();
            SelectedPopMenuItem = null;
        }
    }
}
