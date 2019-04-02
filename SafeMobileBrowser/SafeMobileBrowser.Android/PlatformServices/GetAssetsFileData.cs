using System.IO;
using Xamarin.Forms;

namespace SafeMobileBrowser.Droid.PlatformServices
{
    public class GetAssetsFileData
    {
        public static string ReadHtmlFile(string fileName)
        {
            var assetManager = Forms.Context.Assets;
            using (var streamReader = new StreamReader(assetManager.Open(fileName)))
            {
                return streamReader.ReadToEnd();
            }
        }
    }
}
