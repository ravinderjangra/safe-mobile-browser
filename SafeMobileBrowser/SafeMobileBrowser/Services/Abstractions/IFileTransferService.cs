using System.Collections.Generic;
using System.Threading.Tasks;
using SafeMobileBrowser.Models;

namespace SafeMobileBrowser.Services
{
    public interface IFileTransferService
    {
        string ConfigFilesPath { get; }

        Task TransferAssetsAsync(List<AssetFileTransferModel> fileList);
    }
}
