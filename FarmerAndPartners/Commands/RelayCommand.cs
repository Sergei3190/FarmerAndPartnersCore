using FarmerAndPartners.Commands.BaseCommands;
using System;

namespace FarmerAndPartners.Commands
{
    public class RelayCommand : BaseCommand
    {
        private Action _execute;
        private Func<bool> _canExecute;

        public RelayCommand() { }
        public RelayCommand(Action execute) : this(execute, null) { }
        public RelayCommand(Action execute, Func<bool> canExecute)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        public override bool CanExecute(object parameter) => _canExecute == null || _canExecute();
        public override void Execute(object parameter) => _execute();
    }
}
