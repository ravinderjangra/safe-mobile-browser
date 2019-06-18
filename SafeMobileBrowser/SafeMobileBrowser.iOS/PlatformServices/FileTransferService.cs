using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using SafeMobileBrowser.iOS.PlatformServices;
using SafeMobileBrowser.Models;
using SafeMobileBrowser.Services;
using Xamarin.Forms;

[assembly: Dependency(typeof(FileTransferService))]

namespace SafeMobileBrowser.iOS.PlatformServices
{
    public class FileTransferService : IFileTransferService
    {
        public string ConfigFilesPath
        {
            get
            {
                // Resources -> /Library
                return Environment.GetFolderPath(Environment.SpecialFolder.Resources);
            }
        }

        public async Task TransferAssetsAsync(List<AssetFileTransferModel> fileList)
        {
            foreach (var file in fileList)
            {
                using (var reader = new StreamReader(Path.Combine(".", file.FileName)))
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
