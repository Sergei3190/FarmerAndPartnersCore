using FarmerAndPartners.Commands;
using FarmerAndPartners.Commands.AsyncCommands;
using FarmerAndPartners.Helpers;
using FarmerAndPartners.Helpers.Interfaces;
using System;
using System.Collections.ObjectModel;
using System.Text;

namespace FarmerAndPartners.ViewModels.BaseViewModels
{
    public abstract class BaseTabItemViewModel<T> : Notifier where T : class
    {
        protected static object locker = new object();

        protected readonly IAsyncRepositoryService _repositoryService;
        protected readonly IDialogService _dialogService;
        protected readonly IAsyncCustomTablesBuilder _tablesBuilder;
        protected readonly StringBuilder _completeExMessage;

        protected int _currentProgress;
        protected T _selectedEntity;
        protected ObservableCollection<T> _entities;

        public event EventHandler<ViewModelEventArgs> ViewModelChanged;
        public event EventHandler<ViewModelEventArgs> ViewModelDeleted;
        public event EventHandler<ViewModelEventArgs> ViewModelAdded;

        public BaseTabItemViewModel(IAsyncRepositoryService repositoryService, IDialogService dialogService, IAsyncCustomTablesBuilder tablesBuilder, StringBuilder completeExMessage)
        {
            _repositoryService = repositoryService;
            _dialogService = dialogService;
            _tablesBuilder = tablesBuilder;
            _completeExMessage = completeExMessage;
        }

        public virtual int CurrentProgress { get; set; }
        public virtual AsyncCommand Open { get; }
        public virtual AsyncCommand Save { get; }
        public abstract RelayCommand Add { get; }
        public abstract RelayCommandT<T> Edit { get; }
        public abstract RelayCommandT<T> Delete { get; }

        protected void OnViewModelChanged(string message, int oldViewModelId = 0) => ViewModelChanged?.Invoke(this, new ViewModelEventArgs(message, oldViewModelId));
        protected void OnViewModelDeleted(string message, int oldViewModelId = 0) => ViewModelDeleted?.Invoke(this, new ViewModelEventArgs(message, oldViewModelId));
        protected void OnViewModelAdded(string message, int oldViewModelId = 0) => ViewModelAdded?.Invoke(this, new ViewModelEventArgs(message, oldViewModelId));
    }
}
