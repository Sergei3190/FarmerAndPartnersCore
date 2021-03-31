using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace FarmerAndPartners.Commands.BaseCommands
{
    public abstract class BaseAsyncCommand : IAsyncCommand
    {
        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        public abstract bool CanExecute(object parameter);
        public abstract void Execute(object parameter);
        public abstract Task ExecuteAsync(object parameter);
    }
}
