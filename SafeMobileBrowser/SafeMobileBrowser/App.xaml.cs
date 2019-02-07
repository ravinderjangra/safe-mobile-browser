using SafeMobileBrowser.Models;
using SafeMobileBrowser.Views;
using Xamarin.Forms;

namespace SafeMobileBrowser
{
    public partial class App : Application
    {
        public static TabPageStore TabPageStore{get;set;}
        public App()
        {
            InitializeComponent();

            TabPageStore = new TabPageStore();

            MainPage = new NavigationPage(new HomePage())
            {
                BarBackgroundColor = Color.White
            };
        }

        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}
