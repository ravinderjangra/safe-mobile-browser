using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using SafeMobileBrowser.Helpers;
using SafeMobileBrowser.Models;
using Xamarin.Forms;

namespace SafeMobileBrowser.ViewModels
{
    public class BookmarksModalPageViewModel : BaseViewModel
    {
        public AsyncCommand GoBackCommand { get; set; }

        public AsyncCommand DeleteBookmarkCommand { get; set; }

        public INavigation Navigation { get; set; }

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

        public BookmarksModalPageViewModel(INavigation navigation)
        {
            Navigation = navigation;
            Bookmarks = new ObservableCollection<string>();
            GoBackCommand = new AsyncCommand(GoBackToHomePage);
            DeleteBookmarkCommand = new AsyncCommand(RemoveBookmark);
        }

        private async Task RemoveBookmark(object bookmark)
        {
            try
            {
                await BookmarkService.DeleteBookmarks(bookmark.ToString());
                Bookmarks.Remove((string)bookmark);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                throw;
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
            await BookmarkService.FetchBookmarks();
            Bookmarks = new ObservableCollection<string>(BookmarkService.RetrieveBookmarks());
        }

        private async Task GoBackToHomePage()
        {
            await Navigation.PopModalAsync();
        }
    }
}
