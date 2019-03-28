using SafeApp;
using SafeMobileBrowser.Views;
using Xamarin.Forms;

namespace SafeMobileBrowser
{
    public partial class App : Application
    {
        public static Session AppSession;
        public App()
        {
            InitializeComponent();

            MainPage = new AuthenticationPage();
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
