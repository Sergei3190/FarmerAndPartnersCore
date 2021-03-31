using FarmerAndPartners.Commands;
using FarmerAndPartners.Commands.AsyncCommands;
using FarmerAndPartners.Helpers.BackgroundWorkerArguments;
using FarmerAndPartners.Helpers.Interfaces;
using FarmerAndPartners.Helpers.SerializableObjects;
using FarmerAndPartners.Helpers.ViewModelExtensions;
using FarmerAndPartners.NestedTypesFactories;
using FarmerAndPartners.ViewModels.BaseViewModels;
using FarmerAndPartnersModels;
using NLog;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;

namespace FarmerAndPartners.ViewModels
{
    public class TabItemCompanyViewModel : BaseTabItemViewModel<CompanyViewModel>
    {
        private readonly ILogger _log;
        private readonly INavigationService<CompanyViewModel, ContractStatus> _navigationService;
        private readonly IAsyncFileService<SerializableCompany> _jsonFileService;
        private readonly IAsyncFileService<SerializableCompany> _xmlFileService;
        private readonly BackgroundWorker _worker;
        private readonly ObservableCollection<ContractStatus> _contractStatuses;

        public event EventHandler CompanyViewModelsUpdated;

        public TabItemCompanyViewModel(IAsyncRepositoryService repositoryService, IDialogService dialogService, IAsyncCustomTablesBuilder tablesBuilder,
            StringBuilder completeExMessag, CompanyNestedTypesFactory nestedTypesFactory) : base(repositoryService, dialogService, tablesBuilder, completeExMessag)
        {
            _log = nestedTypesFactory.CreateLogger(nameof(TabItemCompanyViewModel));
            _navigationService = nestedTypesFactory.CreateNavigationService();
            _jsonFileService = nestedTypesFactory.CreateJsonCompanyFileService();
            _xmlFileService = nestedTypesFactory.CreateXmlCompanyFileService();
            _worker = nestedTypesFactory.CreateBackgroundWorker();
            _contractStatuses = _repositoryService.GetContractStatuses();

            _worker.WorkerReportsProgress = true;
            _worker.DoWork += OnDoWork;
            _worker.ProgressChanged += OnProgressChanged;
            _worker.RunWorkerCompleted += OnRunWorkerCompleted;
            _worker.RunWorkerAsync(new CompanyArguments(_repositoryService.GetCompanies(), _repositoryService.GetCompaniesCount()));
        }

        public override int CurrentProgress
        {
            get => _currentProgress;
            set
            {
                if (_currentProgress != value)
                {
                    _currentProgress = value;
                    OnPropertyChanged(nameof(CurrentProgress));
                }
            }
        }

        public CompanyViewModel SelectedCompanyViewModel
        {
            get => _selectedEntity;
            set
            {
                if (_selectedEntity != value)
                {
                    _selectedEntity = value;
                    OnPropertyChanged(nameof(SelectedCompanyViewModel));
                }
            }
        }

        public ObservableCollection<CompanyViewModel> CompanyViewModels
        {
            get => _entities;
            set
            {
                if (_entities != value)
                {
                    _entities = value;
                    OnPropertyChanged(nameof(CompanyViewModels));
                }
            }
        }

        public override RelayCommand Add
        {
            get => new RelayCommand(async () =>
            {
                _navigationService.OpenWindow(null, _contractStatuses, CompanyViewModels.Select(cvm => cvm.Name).ToList());

                var companyViewModel = _navigationService.GetAddedViewModel();

                if (companyViewModel != null)
                {
                    companyViewModel.Company.Id = CompanyViewModels.Last().Id + 1;

                    var result = await _repositoryService.AddCompanyAsync(companyViewModel.Company);

                    if (result < 0)
                    {
                        MessageBox.Show($"Не удалось добавить в базу данных добавленную компанию \"{companyViewModel.Name}\"", "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    CompanyViewModels.Add(companyViewModel);

                    OnViewModelAdded($"Компания {companyViewModel.Name} id = {companyViewModel.Id} добавлена");
                }
            },
                () => _entities != null);
        }

        public override RelayCommandT<CompanyViewModel> Edit
        {
            get => new RelayCommandT<CompanyViewModel>(async c =>
            {
                _navigationService.OpenWindow(in c, _contractStatuses, CompanyViewModels.Select(cvm => cvm.Name).ToList());

                if (_navigationService.IsOriginalViewModelChanged)
                {
                    var result = await _repositoryService.UpdateCompanyAsync(c.Company);

                    if (result < 0)
                    {
                        MessageBox.Show($"Не удалось добавить в базу данных изменения для компании \"{c.Name}\" ", "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    OnViewModelChanged($"Компания {c.Name} id = {c.Id} изменена");
                }
            },
                c => c != null);
        }

        public override RelayCommandT<CompanyViewModel> Delete
        {
            get => new RelayCommandT<CompanyViewModel>(async c =>
            {
                var result = await _repositoryService.DeleteCompanyAsync(c.Company);

                if (result < 0)
                {
                    MessageBox.Show($"Не удалось удалить компанию \"{c.Name}\", возможно она была удалена или изменена другим пользователем с момента её загрузки",
                        "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                OnViewModelDeleted($"Компания {c.Name} id = {c.Id} удалена");

                CompanyViewModels.Remove(c);
            },
                c => c != null);
        }

        public override AsyncCommand Open
        {
            get => new AsyncCommand(async () =>
            {
                try
                {
                    if (_dialogService.OpenFileDialog())
                    {
                        List<SerializableCompany> serializableCompanies = null;

                        if (_dialogService.FilterIndex == 1)
                            serializableCompanies = await _xmlFileService.OpenAsync(_dialogService.FilePath);

                        if (_dialogService.FilterIndex == 2)
                            serializableCompanies = await _jsonFileService.OpenAsync(_dialogService.FilePath);

                        var usersCollection = serializableCompanies.Select(c => c.Users);

                        var serializableUsers = await this.GetSerialisableUsers(usersCollection);

                        var companiesDataTable = await _tablesBuilder.GetCompaniesTableAsync(serializableCompanies);

                        var usersDataTable = await _tablesBuilder.GetUsersTableAsync(serializableUsers);

                        var sqlParameters = this.GetSqlParameters(companiesDataTable, usersDataTable);

                        var result = await _repositoryService.ExecuteSqlQueryAsync("Exec CompaniesMerge_proc @CompaniesType, @UsersType", sqlParameters);

                        if (result < 0)
                        {
                            _dialogService.ShowMessage($"Не удалось добавить данные в БД из файла {_dialogService.FilePath}.");
                            return;
                        }

                        _log.Info($"файл {_dialogService.FilePath} открыт, списки компаний и пользователей в БД обновлены");

                        await _repositoryService.UpdateDbContaextAsync();

                        CompanyViewModels.Clear();

                        _worker.RunWorkerAsync(new CompanyArguments(_repositoryService.GetCompanies(), _repositoryService.GetCompaniesCount()));

                        _dialogService.ShowMessage("Список компаний загружен, запущено обновление списка пользователей");

                        CompanyViewModelsUpdated?.Invoke(this, new EventArgs());
                    }
                }
                catch (Exception ex)
                {
                    lock (locker)
                    {
                        _completeExMessage.Clear();
                        _completeExMessage.AppendLine(ex.Message);
                        _completeExMessage.AppendLine(ex.InnerException?.Message);
                    }

                    _log.Error($"Ошибка при считывании файла: {_dialogService.FilePath},\r\n{_completeExMessage}");
                    _dialogService.ShowMessage(_completeExMessage.ToString());
                }
            });
        }

        public override AsyncCommand Save
        {
            get => new AsyncCommand(async () =>
            {
                try
                {
                    if (_dialogService.SaveFileDialog())
                    {
                        var companies = await this.GetCompanies(CompanyViewModels);

                        var serializableCompanies = await this.GetSerialisableCompanies(companies);

                        if (_dialogService.FilterIndex == 1)
                            await _xmlFileService.SaveAsync(_dialogService.FilePath, serializableCompanies);

                        if (_dialogService.FilterIndex == 2)
                            await _jsonFileService.SaveAsync(_dialogService.FilePath, serializableCompanies);

                        _log.Info($"файл {_dialogService.FilePath} сохранен");

                        _dialogService.ShowMessage("файл сохранен");
                    }
                }
                catch (Exception ex)
                {
                    lock (locker)
                    {
                        _completeExMessage.Clear();
                        _completeExMessage.AppendLine(ex.Message);
                        _completeExMessage.AppendLine(ex.InnerException?.Message);
                    }

                    _log.Error($"Ошибка при сохранении файла: {_dialogService.FilePath},\r\n{_completeExMessage}");
                    _dialogService.ShowMessage(_completeExMessage.ToString());
                }
            });
        }

        public void UpdateCompanyViewModels()
        {
            CompanyViewModels.Clear();
            _worker.RunWorkerAsync(new CompanyArguments(_repositoryService.GetCompanies(), _repositoryService.GetCompaniesCount()));
        }

        private void OnDoWork(object sender, DoWorkEventArgs e)
        {
            var arguments = e.Argument as CompanyArguments;
            var result = _repositoryService.GetCompanyViewModels(_worker, arguments.Companies, arguments.CompaniesCount);
            e.Result = result;
        }

        private void OnProgressChanged(object sender, ProgressChangedEventArgs e) => CurrentProgress = e.ProgressPercentage;

        private void OnRunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                _log.Error($"Ошибка при выполнении метода OnRunWorkerCompleted во время загрузки списка компаний\r\n{e.Error}");
                MessageBox.Show($"Не удалось загрузить список компаний", "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                if (CompanyViewModels is null)
                    CompanyViewModels = e.Result as ObservableCollection<CompanyViewModel>;
                else
                {
                    foreach (var item in e.Result as ObservableCollection<CompanyViewModel>)
                    {
                        CompanyViewModels.Add(item);
                    }
                }

                SelectedCompanyViewModel = _entities[0];
            }
        }
    }
}
