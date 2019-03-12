using System;
using System.Collections.Generic;
using System.Text;

namespace SafeMobileBrowser.ViewModels
{
    public class HomePageViewModel : BaseViewModel
    {
        public bool IsSessionAvailable => App.AppSession != null ? true : false;
    }
}
