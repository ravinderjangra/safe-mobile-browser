using System;
using System.Threading.Tasks;
using Xamarin.Essentials;

namespace SafeMobileBrowser.Services
{
    internal class CredentialCacheService
    {
        private const string AuthRspKey = "UnregisteredAuthResponse";

        public static void Delete()
        {
            try
            {
                SecureStorage.RemoveAll();
            }
            catch (Exception)
            {
                // ignore acct not existing
            }
        }

        public static async Task<string> Retrieve()
        {
            return await SecureStorage.GetAsync(AuthRspKey);
        }

        public static async Task Store(string authRsp)
        {
            await SecureStorage.SetAsync(AuthRspKey, authRsp);
        }
    }
}
