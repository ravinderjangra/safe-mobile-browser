using System;
using Xamarin.Forms;

namespace SafeMobileBrowser.Controls
{
    public class HybridWebView : View
    {
        private Action<string> action;

        public static readonly BindableProperty UriProperty = BindableProperty.Create(
            nameof(Uri),
            typeof(string),
            typeof(HybridWebView),
            default(string));

        public static readonly BindableProperty IsLoadingProperty = BindableProperty.Create(
            nameof(Uri),
            typeof(bool),
            typeof(HybridWebView),
            default(bool));

        public event EventHandler LoadingStateChanged;

        public void ViewLoadingStatechanged(bool state)
        {
            IsLoading = state;
        }

        public bool IsLoading
        {
            get { return (bool)GetValue(IsLoadingProperty); }
            set { SetValue(IsLoadingProperty, value); }
        }

        public string Uri
        {
            get { return (string)GetValue(UriProperty); }
            set { SetValue(UriProperty, value); }
        }

        public void RegisterAction(Action<string> callback)
        {
            action = callback;
        }

        public void Cleanup()
        {
            action = null;
        }

        public void InvokeAction(string data)
        {
            if (action == null || data == null)
            {
                return;
            }
            action.Invoke(data);
        }
    }
}
