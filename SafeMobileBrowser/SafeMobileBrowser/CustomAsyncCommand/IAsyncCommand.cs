using System.Threading.Tasks;
using System.Windows.Input;

namespace SafeMobileBrowser.CustomAsyncCommand
{
    public interface IAsyncCommand : ICommand
    {
        Task ExecuteAsync();

        bool CanExecute();

        bool ForceCanExecute();
    }

    public interface IAsyncCommand<T> : ICommand
    {
        Task ExecuteAsync(T parameter);

        bool CanExecute(T parameter);
    }
}
