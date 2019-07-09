using System;
using System.Threading.Tasks;
using SafeMobileBrowser.Helpers;
using SafeMobileBrowser.Services.Abstractions;
using SafeMobileBrowser.ViewModels;
using Xamarin.Forms;

namespace SafeMobileBrowser.Views
{
    public abstract class BaseContentPage<T> : ContentPage, IViewFor<T>
        where T : BaseNavigationViewModel, new()
    {
        private T _viewModel;

        public T ViewModel
        {
            get
            {
                return _viewModel;
            }

            set
            {
                _viewModel = value;

                BindingContext = _viewModel;

                Task.Run(async () =>
                {
                    try
                    {
                        await Init();
                    }
                    catch (Exception ex)
                    {
                        Logger.Error(ex.Message);
                    }
                });
            }
        }

        protected BaseContentPage()
        {
            ViewModel = Activator.CreateInstance(typeof(T)) as T;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
        }

        object IViewFor.ViewModel
        {
            get
            {
                return _viewModel;
            }

            set
            {
                ViewModel = (T)value;
            }
        }

        private async Task Init()
        {
            try
            {
                await ViewModel.InitAsync();
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message);
            }
        }
    }
}
