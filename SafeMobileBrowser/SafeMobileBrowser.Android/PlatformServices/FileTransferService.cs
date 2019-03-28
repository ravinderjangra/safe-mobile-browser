using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using SafeMobileBrowser.Droid.PlatformServices;
using SafeMobileBrowser.Models;
using SafeMobileBrowser.Services;
using Xamarin.Forms;
using Application = Android.App.Application;

[assembly: Dependency(typeof(FileTransferService))]
namespace SafeMobileBrowser.Droid.PlatformServices
{
    public class FileTransferService : IFileTransferService
    {
        public string ConfigFilesPath
        {
            get
            {
                return Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            }
        }

        public async Task TransferAssetsAsync(List<AssetFileTransferModel> fileList)
        {
            foreach (var file in fileList)
            {
                using (var reader = new StreamReader(Application.Context.Assets.Open(file.FileName)))
                {
                    using (var writer = new StreamWriter(Path.Combine(file.TransferLocation, file.FileName)))
                    {
                        await writer.WriteAsync(await reader.ReadToEndAsync());
                        writer.Close();
                    }
                    reader.Close();
                }
            }
        }
    }
}
