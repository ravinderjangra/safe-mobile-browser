using SafeMobileBrowser.ViewModels;
using Xamarin.Forms;

namespace SafeMobileBrowser.Views
{
    public partial class BookmarksModalPage : BaseContentPage<BookmarksModalPageViewModel>
    {
        BookmarksModalPageViewModel _viewModel;

        public BookmarksModalPage()
        {
            InitializeComponent();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            if (_viewModel == null)
            {
                _viewModel = (BookmarksModalPageViewModel)BindingContext;
            }

            await _viewModel.GetBookmarks();
        }
    }
}
