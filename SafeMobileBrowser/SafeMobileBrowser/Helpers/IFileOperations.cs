using System.Collections.Generic;
using System.Threading.Tasks;

namespace SafeMobileBrowser.Helpers
{
    public interface IFileOperations
    {
        string ConfigFilesPath { get; }

        Task<string> TransferAssetsAsync(List<(string, string)> fileList);
    }
}
