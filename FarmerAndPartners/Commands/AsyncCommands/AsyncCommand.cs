using FarmerAndPartners.Commands.BaseCommands;
using System;
using System.Threading.Tasks;

namespace FarmerAndPartners.Commands.AsyncCommands
{
    public class AsyncCommand : BaseAsyncCommand
    {
        private bool _isExecuting;
        private Func<Task> _executeAsync;
        private Func<bool> _canExecute;

        public AsyncCommand() { }
        public AsyncCommand(Func<Task> executeAsync, Func<bool> canExecute = null)
        {
            _executeAsync = executeAsync ?? throw new ArgumentNullException(nameof(executeAsync));
            _canExecute = canExecute;
        }

        public override bool CanExecute(object parameter) => (!_isExecuting && _canExecute == null) || (!_isExecuting && _canExecute());
        public override async void Execute(object parameter) => await ExecuteAsync(parameter);
        public override async Task ExecuteAsync(object parameter)
        {
            try
            {
                _isExecuting = true;
                await _executeAsync();
            }
            finally
            {
                _isExecuting = false;
            }
        }
    }
}
