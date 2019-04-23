using Plugin.CurrentActivity;
using System.IO;
using Xamarin.Forms;

namespace SafeMobileBrowser.Droid.PlatformServices
{
    public class GetAssetsFileData
    {
        public static string ReadHtmlFile(string fileName)
        {
            var assetManager = CrossCurrentActivity.Current.AppContext.Assets;
            using (var streamReader = new StreamReader(assetManager.Open(fileName)))
            {
                return streamReader.ReadToEnd();
            }
        }
    }
}
