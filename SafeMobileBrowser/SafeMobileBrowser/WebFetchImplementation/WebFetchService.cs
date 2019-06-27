using System;
using System.Threading.Tasks;
using SafeMobileBrowser.Helpers;
using SafeMobileBrowser.Models;

namespace SafeMobileBrowser.WebFetchImplementation
{
    public class WebFetchService
    {
        private static readonly WebFetch webFetch = new WebFetch();

        public static async Task<WebFetchResponse> FetchResourceAsync(string url, WebFetchOptions options = null)
        {
            try
            {
                return await webFetch.FetchAsync(url, options);
            }
            catch (WebFetchException ex)
            {
                Logger.Error(ex);
                throw ex;
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                throw ex;
            }
        }
    }
}
