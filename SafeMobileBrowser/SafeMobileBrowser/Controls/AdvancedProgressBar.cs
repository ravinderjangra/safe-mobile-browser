using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace SafeMobileBrowser.Controls
{
    public class AdvancedProgressBar : ProgressBar
    {
        public static readonly BindableProperty IsRunningProperty = BindableProperty.Create(
            nameof(Uri),
            typeof(bool),
            typeof(AdvancedProgressBar),
            default(bool));

        public bool IsRunning
        {
            get { return (bool)GetValue(IsRunningProperty); }
            set { SetValue(IsRunningProperty, value); }
        }
    }
}
