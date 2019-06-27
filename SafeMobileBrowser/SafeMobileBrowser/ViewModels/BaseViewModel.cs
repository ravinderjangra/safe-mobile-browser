using System;
using System.Threading.Tasks;
using SafeMobileBrowser.Models;
using SafeMobileBrowser.Services;
using Xamarin.Forms;

namespace SafeMobileBrowser.ViewModels
{
    public class BaseViewModel : BaseNotifyPropertyChanged, IDisposable
    {
        public static BookmarkManager BookmarkManager { get; set; }

        public AppService AppService => DependencyService.Resolve<AppService>();

        public AuthenticationService AuthService => DependencyService.Get<AuthenticationService>();

        bool _isBusy;

        public bool IsBusy
        {
            get => _isBusy;
            set
            {
                RaiseAndUpdate(ref _isBusy, value);
                Raise(nameof(IsNotBusy));
            }
        }

        public bool IsNotBusy => !IsBusy;

        public virtual Task InitAsync() => Task.FromResult(true);

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
        }
    }
}
