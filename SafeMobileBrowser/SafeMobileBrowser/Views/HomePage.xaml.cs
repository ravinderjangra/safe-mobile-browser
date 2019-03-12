using SafeMobileBrowser.Services;
using SlideOverKit;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SafeMobileBrowser.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class HomePage : MenuContainerPage
    {
        public HomePage()
        {
            InitializeComponent();
            this.SlideMenu = new TabsRightMenuView();
            InitilizeTapGestures();
            App.TabPageStore.AddPage(new Models.TabPage
            {
                PageTitle = "MainPage",
                PageHtmlContent = "Content Null"
            });
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            MessagingCenter.Subscribe<AuthenticationService>(this, "Authenticated",
                (sender) =>
                {
                    AuthenticateButton.IsVisible = false;
                    HybridWebView.IsVisible = true;
                });
        }

        private void InitilizeTapGestures()
        {
            var tabCountFrameTapGestureRecognizer = new TapGestureRecognizer() { NumberOfTapsRequired = 1 };
            tabCountFrameTapGestureRecognizer.Tapped += (s, e) =>
            {
                if (this.SlideMenu.IsShown)
                {
                    this.HideMenu();
                }
                else
                {
                    this.ShowMenu();
                }
            };
            TabCountFrame.GestureRecognizers.Add(tabCountFrameTapGestureRecognizer);

            var refreshTapGestureRecognizer = new TapGestureRecognizer() { NumberOfTapsRequired = 1 };
            refreshTapGestureRecognizer.Tapped += (s, e) =>
            {

            };
            AddressBarButton.GestureRecognizers.Add(refreshTapGestureRecognizer);

            var menuTapGestureRecognizer = new TapGestureRecognizer() { NumberOfTapsRequired = 1 };
            menuTapGestureRecognizer.Tapped += (s, e) =>
            {

            };
            SettingsButton.GestureRecognizers.Add(menuTapGestureRecognizer);

            AuthenticateButton.Clicked += (s, e) =>
            {
                AuthenticationService.RequestLiveNetworkAuthenticationAsync();
            };

            AddressBarEntry.Completed += (s, e) =>
            {
                HybridWebView.Uri = "http://" + AddressBarEntry.Text;
            };
        }
    }
}