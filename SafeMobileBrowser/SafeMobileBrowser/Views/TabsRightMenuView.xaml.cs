using SlideOverKit;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SafeMobileBrowser.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TabsRightMenuView : SlideMenuView
    {
        // public ObservableCollection<TabPage> Pages => App.TabPageStore.TabPages;

        public TabsRightMenuView()
        {
            InitializeComponent();
            IsFullScreen = true;
            WidthRequest = 300;
            MenuOrientations = MenuOrientation.LeftToRight;
            BackgroundColor = Color.White;
            BackgroundViewColor = Color.LightSlateGray;
            BindingContext = this;
        }
    }
}
