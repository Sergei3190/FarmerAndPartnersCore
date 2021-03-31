using FarmerAndPartners.Commands.AsyncCommands;
using FarmerAndPartners.Helpers.Interfaces;
using FarmerAndPartners.Helpers.NestedTypesFactories.BaseFactory;
using FarmerAndPartners.NestedTypesFactories;
using NLog;
using System.Text;

namespace FarmerAndPartners.ViewModels
{
    public class MainWindowViewModel
    {
        private readonly IAsyncRepositoryService _repositoryService;
        private readonly IDialogService _dialogService;
        private readonly IAsyncCustomTablesBuilder _tablesBuilder;
        private readonly StringBuilder _completeExMessage;
        private readonly MediatorViewModels _mediatorViewModels;

        public MainWindowViewModel(BaseNestedTypesFactory baseNestedTypesFactory)
        {
            _repositoryService = baseNestedTypesFactory.CreateRepositoryService(LogManager.GetLogger(nameof(MainWindowViewModel)));
            _dialogService = baseNestedTypesFactory.CreateDialogService();
            _tablesBuilder = baseNestedTypesFactory.CreateCustomTablesBuilder();
            _completeExMessage = baseNestedTypesFactory.CreateStringBuilder();
            TabItemCompanyViewModel = new TabItemCompanyViewModel(_repositoryService, _dialogService, _tablesBuilder, _completeExMessage, new CompanyNestedTypesFactory());
            TabItemUserViewModel = new TabItemUserViewModel(_repositoryService, _dialogService, _tablesBuilder, _completeExMessage, new UserNestedTypesFactory());
            _mediatorViewModels = new MediatorViewModels(TabItemCompanyViewModel, TabItemUserViewModel);
        }

        public TabItemCompanyViewModel TabItemCompanyViewModel { get; }
        public TabItemUserViewModel TabItemUserViewModel { get; }

        public AsyncCommand Closing => new AsyncCommand(async () => await _repositoryService.DisposeDbContextAsync());
    }
}
