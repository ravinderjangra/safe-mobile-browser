using SafeApp;
using SafeMobileBrowser.CustomAsyncCommand;
using SafeMobileBrowser.Services;
using System;
using System.Threading.Tasks;

namespace SafeMobileBrowser.ViewModels
{
    public class AuthenticationPageViewMode : BaseViewModel
    {
        public bool IsMock { get; set; }

        private string _progressText;

        public string ProgressText
        {
            get { return _progressText; }
            set { SetProperty(ref _progressText, value); }
        }

        public IAsyncCommand AuthenticateCommand { get; private set; }

        public AuthenticationPageViewMode()
        {
            CheckIfMock();
            AuthenticateCommand = new AsyncCommand(AuthenticateAppAsync, CanExecute);
        }

        private bool CanExecute()
        {
            return !IsBusy;
        }

        private async Task AuthenticateAppAsync()
        {
            try
            {
                IsBusy = true;
#if SAFE_APP_MOCK
                ProgressText = "Creating mock account";
                await AuthenticationService.CreateMockAccount();
                ProgressText = "Mock authentication in progress";
                await AuthenticationService.RequestMockNetworkAuthenticationAsync();
                MessagingCenter.Send(this, MessageCenterConstants.Authenticated);
                IsBusy = false;
#else
                ProgressText = "Requesting authentication for authenticator app";
                await AuthenticationService.RequestLiveNetworkAuthenticationAsync();
#endif
            }
            catch (Exception ex)
            {
                await App.Current.MainPage.DisplayAlert("Authentication Failed", ex.Message, "OK");
                IsBusy = false;
            }
        }

        private void CheckIfMock()
        {
            IsBusy = true;
            IsMock = Session.IsMockBuild();
            IsBusy = false;
        }
    }
}
