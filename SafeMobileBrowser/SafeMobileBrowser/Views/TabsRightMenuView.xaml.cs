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
            this.IsFullScreen = true;
            this.WidthRequest = 300;
            this.MenuOrientations = MenuOrientation.LeftToRight;
            this.BackgroundColor = Color.White;
            this.BackgroundViewColor = Color.LightSlateGray;
            BindingContext = this;
        }
    }
}