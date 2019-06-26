namespace SafeMobileBrowser.Models
{
    public class PopUpMenuItem : BaseNotifyPropertyChanged
    {
        public string MenuItemTitle { get; set; }

        private string _menuItemIcon;

        public string MenuItemIcon
        {
            get => _menuItemIcon;
            set
            {
                RaiseAndUpdate(ref _menuItemIcon, value);
            }
        }

        private bool _isEnabled;

        public bool IsEnabled
        {
            get => _isEnabled;

            set
            {
                RaiseAndUpdate(ref _isEnabled, value);
            }
        }
    }
}
