using System;
using SlideOverKit;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SafeMobileBrowser.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class HomePage : MenuContainerPage
    {
        Random randonGen = new Random();
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
            RandomColorChangeButton.Clicked += (s, e) =>
            {
                BackgroundColor = GenerateRandomColor();
            };
        }

        public Color GenerateRandomColor()
        {
            Color randomColor = Color.FromRgb(randonGen.Next(255), randonGen.Next(255), randonGen.Next(255));
            return randomColor;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
        }

        private void InitilizeTapGestures()
        {
            var tabCountFrameTapGestureRecognizer = new TapGestureRecognizer() { NumberOfTapsRequired = 1 };
            tabCountFrameTapGestureRecognizer.Tapped += (s, e) =>
            {
                if(this.SlideMenu.IsShown)
                {
                    this.HideMenu();
                }
                else
                {
                    // var screenshotData = DependencyService.Get<IScreenshotService>().Capture();
                    // App.TabPageStore.AddPage(new Models.TabPage { PageTitle = "Hello" + randonGen.Next(100)});
                    this.ShowMenu();
                }
            };
            TabCountFrame.GestureRecognizers.Add(tabCountFrameTapGestureRecognizer);
        }
    }
}