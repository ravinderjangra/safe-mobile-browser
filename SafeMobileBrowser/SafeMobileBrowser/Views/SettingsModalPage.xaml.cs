using SafeMobileBrowser.ViewModels;

namespace SafeMobileBrowser.Views
{
    public partial class SettingsModalPage : BaseContentPage<SettingsModalPageViewModel>
    {
        public SettingsModalPage()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            if (_viewModel == null)
            {
                _viewModel = new SettingsModalPageViewModel(Navigation);
            }

            BindingContext = _viewModel;
        }

        public void OnDarkThemeToggled(object sender, ToggledEventArgs toggledEventArgs)
        {
            if (toggledEventArgs.Value)
            {
                DisplayAlert("Dark Mode", "Coming soon", "Ok");
            }
        }
    }
}
