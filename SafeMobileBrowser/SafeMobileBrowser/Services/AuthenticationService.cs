using SafeApp;
using SafeApp.Utilities;
using SafeMobileBrowser.Helpers;
using System;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace SafeMobileBrowser.Services
{
    public class AuthenticationService
    {
        public static async void RequestLiveNetworkAuthenticationAsync()
        {
            var req = await Session.EncodeUnregisteredRequestAsync(Constants.AppId);
            var url = $"safe-auth://{Constants.AppId}/{req.Item2}";
            Device.BeginInvokeOnMainThread(() => { Device.OpenUri(new Uri(url)); });
        }

        public async Task ProcessAuthenticationResponseAsync(string url)
        {
            var encodedRequest = RequestHelpers.GetRequestData(url);
            var decodeResult = await Session.DecodeIpcMessageAsync(encodedRequest);
            if (decodeResult.GetType() == typeof(UnregisteredIpcMsg))
            {
                var ipcMsg = decodeResult as UnregisteredIpcMsg;

                if (ipcMsg != null)
                {
                    App.AppSession = await Session.AppUnregisteredAsync(ipcMsg.SerialisedCfg);
                    MessagingCenter.Send(this, "Authenticated");
                    await App.Current.MainPage.DisplayAlert("Authentication", "Session established", "Ok");
                }
                else
                {
                    Console.WriteLine("Auth Request is not Granted");
                }
            }
        }
    }
}
