using SafeApp;
#if SAFE_APP_MOCK
using SafeApp.MockAuthBindings;
#endif
using SafeApp.Utilities;
using SafeMobileBrowser.Helpers;
using SafeMobileBrowser.Services;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Xamarin.Forms;

[assembly: Dependency(typeof(AuthenticationService))]
namespace SafeMobileBrowser.Services
{
    public class AuthenticationService
    {
#if SAFE_APP_MOCK
        public static Authenticator _authenticator;
        public static async Task RequestMockNetworkAuthenticationAsync()
        {
            var (_, reqMsg) = await Session.EncodeUnregisteredRequestAsync(Constants.AppId);
            var ipcReq = await Authenticator.UnRegisteredDecodeIpcMsgAsync(reqMsg);
            var authIpcReq = ipcReq as UnregisteredIpcReq;
            var resMsg = await Authenticator.EncodeUnregisteredRespAsync(authIpcReq.ReqId, true);
            var ipcResponse = await Session.DecodeIpcMessageAsync(resMsg);
            if (ipcResponse.GetType() == typeof(UnregisteredIpcMsg))
            {
                var authResponse = ipcResponse as UnregisteredIpcMsg;
                App.AppSession = await Session.AppUnregisteredAsync(authResponse.SerialisedCfg);
            }
        }

        public static async Task CreateMockAccount()
        {
            try
            {
                var locator = RandomGenerators.GetRandomString(5);
                var secret = RandomGenerators.GetRandomString(5);
                var invitation = RandomGenerators.GetRandomString(5);
                _authenticator = await Authenticator.CreateAccountAsync(locator, secret, invitation);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw ex;
            }
        }
#else
        public static async Task RequestLiveNetworkAuthenticationAsync()
        {
            try
            {
                var req = await Session.EncodeUnregisteredRequestAsync(Constants.AppId);
                var url = $"safe-auth://{Constants.AppId}/{req.Item2}";
                Device.OpenUri(new Uri(url));
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw ex;
            }
        }
#endif
        public async Task ProcessAuthenticationResponseAsync(string url)
        {
            try
            {
                MessagingCenter.Send(this, MessageCenterConstants.ProcessingAuthResponse);
                var encodedRequest = RequestHelpers.GetRequestData(url);
                var decodeResult = await Session.DecodeIpcMessageAsync(encodedRequest);
                if (decodeResult.GetType() == typeof(UnregisteredIpcMsg))
                {
                    var ipcMsg = decodeResult as UnregisteredIpcMsg;

                    if (ipcMsg != null)
                    {
                        App.AppSession = await Session.AppUnregisteredAsync(ipcMsg.SerialisedCfg);
                        MessagingCenter.Send(this, MessageCenterConstants.Authenticated);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                MessagingCenter.Send(this, MessageCenterConstants.AuthenticationFailed);
            }
        }
    }
}
