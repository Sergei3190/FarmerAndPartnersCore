using System.Threading.Tasks;
using System.Windows.Input;

namespace FarmerAndPartners.Commands.BaseCommands
{
    public interface IAsyncCommand : ICommand
    {
        Task ExecuteAsync(object parameter);
    }
}
