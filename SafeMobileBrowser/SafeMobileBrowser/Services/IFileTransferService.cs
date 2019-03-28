using SafeMobileBrowser.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SafeMobileBrowser.Services
{
    public interface IFileTransferService
    {
        string ConfigFilesPath { get; }
        Task TransferAssetsAsync(List<AssetFileTransferModel> fileList);
    }
}
