using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Input;
using Acr.UserDialogs;
using SafeMobileBrowser.Helpers;
using SafeMobileBrowser.Models;
using Xamarin.Forms;

namespace SafeMobileBrowser.ViewModels
{
    public class BookmarksModalPageViewModel : BaseNavigationViewModel
    {
        public AsyncCommand GoBackCommand { get; set; }

        public AsyncCommand DeleteBookmarkCommand { get; set; }

        private string _selectedBookmarkItem;

        public string SelectedBookmarkItem
        {
            get => _selectedBookmarkItem;

            set
            {
                RaiseAndUpdate(ref _selectedBookmarkItem, value);
                OpenBookmarkedPage();
            }
        }

        private ObservableCollection<string> _bookmarks;

        public ObservableCollection<string> Bookmarks
        {
            get => _bookmarks;
            set => RaiseAndUpdate(ref _bookmarks, value);
        }

        public BookmarksModalPageViewModel()
        {
            Bookmarks = new ObservableCollection<string>();
            GoBackCommand = new AsyncCommand(GoBackToHomePage);
            DeleteBookmarkCommand = new AsyncCommand(RemoveBookmark);
        }

        private async Task RemoveBookmark(object bookmark)
        {
            try
            {
                if (!App.IsConnectedToInternet)
                {
                    await App.Current.MainPage.DisplayAlert("No internet connection", "Please connect to the internet", "Ok");
                    return;
                }

                using (UserDialogs.Instance.Toast("Bookmark removed successfully"))
                {
                    await BookmarkManager.DeleteBookmarks(bookmark.ToString());
                    Bookmarks.Remove((string)bookmark);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                UserDialogs.Instance.Toast("Failed to remove bookmark");
            }
        }

        public async void OpenBookmarkedPage()
        {
            await Navigation.PopModalAsync();
            MessagingCenter.Send<BookmarksModalPageViewModel, string>(
                this,
                MessageCenterConstants.BookmarkUrl,
                SelectedBookmarkItem.Replace("safe://", string.Empty));
        }

        public async Task GetBookmarks()
        {
            if (!AppService.IsAccessContainerMDataInfoAvailable)
            {
                var mdInfo = await AppService.GetAccessContainerMdataInfoAsync();
                BookmarkService.SetMdInfo(mdInfo);
            }
            if (App.IsConnectedToInternet)
            {
                await BookmarkManager.FetchBookmarks();
            }
            else
            {
                await App.Current.MainPage.DisplayAlert("No internet Connection", "Showing previously fetched bookmarks", "Ok");
            }
            Bookmarks = new ObservableCollection<string>(BookmarkManager.RetrieveBookmarks());
        }

        private async Task GoBackToHomePage()
        {
            await Navigation.PopModalAsync();
        }
    }
}
