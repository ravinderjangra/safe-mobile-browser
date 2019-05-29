using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace SafeMobileBrowser.ViewModels
{
    public class HomePageViewModel : BaseViewModel
    {
        public bool IsSessionAvailable => App.AppSession != null ? true : false;

        public ICommand PageLoadCommand { get; private set; }

        public ICommand ToolbarItemCommand { get; private set; }

        private bool _pageLoading;

        public bool IsPageLoading
        {
            get { return _pageLoading; }
            set { SetProperty(ref _pageLoading, value); }
        }

        private string _url;

        public string Url
        {
            get { return _url; }
            set { SetProperty(ref _url, value); }
        }

        private string _addressbarText;

        public string AddressbarText
        {
            get { return _addressbarText; }
            set { SetProperty(ref _addressbarText, value); }
        }

        public HomePageViewModel()
        {
            PageLoadCommand = new Command(LoadUrl);
            ToolbarItemCommand = new Command<string>(LoadUrl);
            InitilizeSessionAsync();
        }

        private async Task InitilizeSessionAsync()
        {
            //TODO: Connect using hardcoded response, provide option to authenticate using Authenticator
            await AuthService.ConnectUsingHardcodedResponse();
        }

        private void LoadUrl(string url)
        {
            AddressbarText = url;
            LoadUrl();
        }

        private void LoadUrl()
        {
            Url = $"http://{AddressbarText}";
        }
    }
}
