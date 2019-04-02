using SafeMobileBrowser.Models;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace SafeMobileBrowser.WebFetchImplementation
{
    public class WebFetchService
    {
        public static WebFetch webFetch = new WebFetch();

        public static async Task<WebFetchResponse> FetchResourceAsync(string url, WebFetchOptions options = null)
        {
            try
            {
                return await webFetch.FetchAsync(url, options);
            }
            catch(WebFetchException ex)
            {
                Debug.WriteLine("Error: " + ex.Message);
                throw ex;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error: " + ex.Message);
                throw ex;
            }
        }
    }
}
