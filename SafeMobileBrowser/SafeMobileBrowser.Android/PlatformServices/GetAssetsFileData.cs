using Plugin.CurrentActivity;
using System.IO;

namespace SafeMobileBrowser.Droid.PlatformServices
{
    public class GetAssetsFileData
    {
        public static string ReadHtmlFile(string fileName)
        {
            var assetManager = CrossCurrentActivity.Current.Activity.Assets;
            using (var streamReader = new StreamReader(assetManager.Open(fileName)))
            {
                return streamReader.ReadToEnd();
            }
        }
    }
}
