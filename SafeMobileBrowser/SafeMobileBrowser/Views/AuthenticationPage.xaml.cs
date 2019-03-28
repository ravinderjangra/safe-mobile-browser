using SafeApp;
using SafeMobileBrowser.Helpers;
using SafeMobileBrowser.Services;
using SafeMobileBrowser.ViewModels;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SafeMobileBrowser.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AuthenticationPage : ContentPage
    {
        AuthenticationPageViewMode _viewModel;
        public AuthenticationPage()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            if (_viewModel == null)
                _viewModel = new AuthenticationPageViewMode();

            BindingContext = _viewModel;

            MessagingCenter.Subscribe<AuthenticationService>(this, MessageCenterConstants.Authenticated,
                (sender) =>
                {
                    _viewModel.ProgressText = "Authentication successful";
                    Application.Current.MainPage = new NavigationPage(new HomePage()) { BarBackgroundColor = Color.White };
                });
            MessagingCenter.Subscribe<AuthenticationPageViewMode>(this, MessageCenterConstants.Authenticated,
                (sender) => Application.Current.MainPage = new NavigationPage(new HomePage()) { BarBackgroundColor = Color.White });
            MessagingCenter.Subscribe<AuthenticationService>(this, MessageCenterConstants.ProcessingAuthResponse,
                (sender) => _viewModel.ProgressText = "Process authentication response");
            MessagingCenter.Subscribe<AuthenticationService>(this, MessageCenterConstants.AuthenticationFailed,
                async (sender) =>
                {
                    await Application.Current.MainPage.DisplayAlert("Authentication Failed", "Unable to authenticate", "OK");
                    _viewModel.IsBusy = false;
                    _viewModel.AuthenticateCommand.ForceCanExecute();
                });

        }
    }
}