using System;
using System.Threading.Tasks;

namespace FarmerAndPartners.Commands.AsyncCommands
{
    public class AsyncCommand<T> : AsyncCommand
    {
        private bool _isExecuting;
        private Func<T, Task> _executeAsync;
        private Func<T, bool> _canExecute;

        public AsyncCommand(Func<T, Task> executeAsync, Func<T, bool> canExecute = null)
        {
            _executeAsync = executeAsync ?? throw new ArgumentNullException(nameof(executeAsync));
            _canExecute = canExecute;
        }

        public override bool CanExecute(object parameter) => (!_isExecuting && _canExecute == null) || (!_isExecuting && _canExecute((T)parameter));
        public override async void Execute(object parameter) => await ExecuteAsync(parameter);
        public override async Task ExecuteAsync(object parameter)
        {
            try
            {
                _isExecuting = true;
                await _executeAsync((T)parameter);
            }
            finally
            {
                _isExecuting = false;
            }
        }
    }
}
