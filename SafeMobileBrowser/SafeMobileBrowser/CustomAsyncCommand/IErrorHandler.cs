using System;

namespace SafeMobileBrowser.CustomAsyncCommand
{
    public interface IErrorHandler
    {
        void HandleError(Exception ex);
    }
}
