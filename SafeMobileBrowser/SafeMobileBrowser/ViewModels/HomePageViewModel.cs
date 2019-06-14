using System;
using System.Diagnostics;
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

        public Command BottomNavbarTapCommand { get; set; }

        public ICommand GoBackCommand { get; set; }

        public ICommand GoForwardCommand { get; set; }

        public ICommand AddressBarFocusCommand { get; set; }

        public ICommand ReloadCommand { get; set; }

        public ICommand WebViewNavigatingCommand { get; set; }

        public ICommand WebViewNavigatedCommand { get; set; }

        private bool _canGoBack;

        public bool CanGoBack
        {
            get { return _canGoBack; }
            set
            {
                _canGoBack = value;
                OnPropertyChanged();
            }
        }

        private bool _canGoForward;

        public bool CanGoForward
        {
            get { return _canGoForward; }
            set
            {
                _canGoForward = value;
                OnPropertyChanged();
            }
        }

        private bool _isRunning;

        public bool IsRunning
        {
            get { return _isRunning; }
            set
            {
                _isRunning = value;
                OnPropertyChanged();
            }
        }

        private bool _isVisible;

        public bool IsVisible
        {
            get { return _isVisible; }
            set
            {
                _isVisible = value;
                OnPropertyChanged();
            }
        }

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
            BottomNavbarTapCommand = new Command<string>(OnTapped);
            WebViewNavigatingCommand = new Command<WebNavigatingEventArgs>(OnNavigating);
            WebViewNavigatedCommand = new Command<WebNavigatedEventArgs>(OnNavigated);
        }

        private void OnNavigated(WebNavigatedEventArgs obj)
        {
            IsRunning = false;
            IsVisible = false;
        }

        private void OnNavigating(WebNavigatingEventArgs obj)
        {
            try
            {
                string urlText = obj.Url.ToString();
                if (urlText.StartsWith("file://"))
                {
                    AddressbarText = string.Empty;
                }
                else if (urlText.StartsWith("https"))
                {
                    IsRunning = true;
                    IsVisible = true;
                    string newurlText = urlText.Remove(0, 8);
                    AddressbarText = newurlText;
                }
                else if (urlText.StartsWith("http"))
                {
                    IsRunning = true;
                    IsVisible = true;
                    string newurlText = urlText.Remove(0, 7);
                    AddressbarText = newurlText;
                }
                else
                {
                    AddressbarText = urlText;
                    IsRunning = true;
                    IsVisible = true;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error: {ex.Message}");
            }
        }

        internal async Task InitilizeSessionAsync()
        {
            //TODO: Connect using hardcoded response, provide option to authenticate using Authenticator
            await AuthService.ConnectUsingHardcodedResponse();
        }

        public void OnTapped(string imageButton)
        {
            switch (imageButton)
            {
                case "LeftImage":
                    if (CanGoBack)
                        this.GoBackCommand.Execute(null);
                    break;
                case "RightImage":
                    if (CanGoForward)
                        this.GoForwardCommand.Execute(null);
                    break;
                case "SearchImage":
                    this.AddressBarFocusCommand.Execute(null);
                    break;
                case "RefreshImage":
                    this.ReloadCommand.Execute(null);
                    break;
                default:
                    break;
            }
        }

        private void LoadUrl(string url)
        {
            AddressbarText = url;
            LoadUrl();
        }

        private void LoadUrl()
        {
            if (Device.RuntimePlatform == Device.iOS)
                Url = $"safe://{AddressbarText}";
            else
                Url = $"https://{AddressbarText}";
        }
    }
}
