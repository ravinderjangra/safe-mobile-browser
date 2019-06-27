using System.Threading.Tasks;
using SafeMobileBrowser.Models;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace SafeMobileBrowser.ViewModels
{
    public class SettingsModalPageViewModel : BaseNavigationViewModel
    {
        public string ApplicationVersion => AppInfo.VersionString;

        public AsyncCommand FaqCommand { get; }

        public AsyncCommand PrivacyInfoCommand { get; }

        public AsyncCommand GoBackCommand { get; }

        public SettingsModalPageViewModel()
        {
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
            await Navigation.PopModalAsync();
        }
    }
}
