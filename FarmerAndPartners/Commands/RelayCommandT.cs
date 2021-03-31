using System;

namespace FarmerAndPartners.Commands
{
    public class RelayCommandT<T> : RelayCommand
    {
        private Action<T> _execute;
        private Func<T, bool> _canExecute;

        public RelayCommandT(Action<T> execute, Func<T, bool> canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        public override bool CanExecute(object parameter) => _canExecute == null || _canExecute((T)parameter);
        public override void Execute(object parameter) => _execute((T)parameter);
    }
}
