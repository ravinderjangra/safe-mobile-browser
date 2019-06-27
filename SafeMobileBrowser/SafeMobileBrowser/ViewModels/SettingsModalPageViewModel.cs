using System.Threading.Tasks;
using SafeMobileBrowser.Models;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace SafeMobileBrowser.ViewModels
{
    public class SettingsModalPageViewModel : BaseViewModel
    {
        private INavigation _navigation;

        public string ApplicationVersion => AppInfo.VersionString;

        public AsyncCommand FaqCommand { get; }

        public AsyncCommand PrivacyInfoCommand { get; }

        public AsyncCommand GoBackCommand { get; }

        public SettingsModalPageViewModel(INavigation navigation)
        {
            _navigation = navigation;
            GoBackCommand = new AsyncCommand(GoBackToHomePageAsync);
            FaqCommand = new AsyncCommand(ShowNotImplementedDialogAsync);
            PrivacyInfoCommand = new AsyncCommand(ShowNotImplementedDialogAsync);
        }

        private async Task ShowNotImplementedDialogAsync()
        {
            await Application.Current.MainPage.DisplayAlert("Feature", "This feature not available yet.", "OK");
        }

        private async Task GoBackToHomePageAsync()
        {
            await _navigation.PopModalAsync();
        }
    }
}
