using FarmerAndPartners.Commands;
using FarmerAndPartners.Commands.AsyncCommands;
using FarmerAndPartners.Helpers.BackgroundWorkerArguments;
using FarmerAndPartners.Helpers.Interfaces;
using FarmerAndPartners.Helpers.SerializableObjects;
using FarmerAndPartners.Helpers.ViewModelExtensions;
using FarmerAndPartners.NestedTypesFactories;
using FarmerAndPartners.ViewModels.BaseViewModels;
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
    public class TabItemUserViewModel : BaseTabItemViewModel<UserViewModel>
    {
        private readonly ILogger _log;
        private readonly INavigationService<UserViewModel, CompanyViewModel> _navigationService;
        private readonly IAsyncFileService<SerializableUser> _jsonFileService;
        private readonly IAsyncFileService<SerializableUser> _xmlFileService;
        private readonly BackgroundWorker _worker;

        public event EventHandler UserViewModelsUpdated;

        public TabItemUserViewModel(IAsyncRepositoryService repositoryService, IDialogService dialogService, IAsyncCustomTablesBuilder tablesBuilder,
            StringBuilder completeExMessag, UserNestedTypesFactory nestedTypesFactory) : base(repositoryService, dialogService, tablesBuilder, completeExMessag)
        {
            _log = nestedTypesFactory.CreateLogger(nameof(TabItemUserViewModel));
            _navigationService = nestedTypesFactory.CreateUserNavigationService();
            _jsonFileService = nestedTypesFactory.CreateJsonUserFileService();
            _xmlFileService = nestedTypesFactory.CreateXmlUserFileService();
            _worker = nestedTypesFactory.CreateBackgroundWorker();

            _worker.WorkerReportsProgress = true;
            _worker.DoWork += OnDoWork;
            _worker.ProgressChanged += OnProgressChanged;
            _worker.RunWorkerCompleted += OnRunWorkerCompleted;
            _worker.RunWorkerAsync(new UserArguments(_repositoryService.GetUsers(), _repositoryService.GetUsersCount()));
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

        public UserViewModel SelectedUserViewModel
        {
            get => _selectedEntity;
            set
            {
                if (_selectedEntity != value)
                {
                    _selectedEntity = value;
                    OnPropertyChanged(nameof(SelectedUserViewModel));
                }
            }
        }

        public ObservableCollection<UserViewModel> UserViewModels
        {
            get => _entities;
            set
            {
                if (_entities != value)
                {
                    _entities = value;
                    OnPropertyChanged(nameof(UserViewModels));
                }
            }
        }

        public override RelayCommand Add
        {
            get => new RelayCommand(async () =>
            {
                var companyViewModels = await _repositoryService.GetCompanyViewModelsAsync();

                _navigationService.OpenWindow(null, companyViewModels, UserViewModels.Select(uvm => uvm.Login).ToList());

                var userViewModel = _navigationService.GetAddedViewModel();

                if (userViewModel != null)
                {
                    userViewModel.User.Id = UserViewModels.Last().Id + 1;

                    var result = await _repositoryService.AddUserAsync(userViewModel.User);

                    if (result < 0)
                    {
                        MessageBox.Show($"Не удалось добавить в базу данных добавленного пользователя \"{userViewModel.Name}\"", "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    UserViewModels.Add(userViewModel);

                    OnViewModelAdded($"Пользователь {userViewModel.Login} id = {userViewModel.Id} добавлен");
                }
            },
                 () => _entities != null);
        }

        public override RelayCommandT<UserViewModel> Edit
        {
            get => new RelayCommandT<UserViewModel>(async u =>
            {
                var oldIdCompanyViewModel = u.CompanyId;

                var companyViewModels = await _repositoryService.GetCompanyViewModelsAsync();

                _navigationService.OpenWindow(in u, companyViewModels, UserViewModels.Select(uvm => uvm.Login).ToList());

                if (_navigationService.IsOriginalViewModelChanged)
                {
                    var result = await _repositoryService.UpdateUserAsync(u.User);

                    if (result < 0)
                    {
                        MessageBox.Show($"Не удалось добавить в базу данных изменения для пользователя \"{u.Name}\"", "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    OnViewModelChanged($"Пользователь {u.Login} id = {u.Id} изменен", oldIdCompanyViewModel);
                }
            },
                u => u != null);
        }

        public override RelayCommandT<UserViewModel> Delete
        {
            get => new RelayCommandT<UserViewModel>(async u =>
            {
                var result = await _repositoryService.DeleteUserAsync(u.User);

                if (result < 0)
                {
                    MessageBox.Show($"Не удалось удалить пользователя \"{u.Name}\", возможно он был удален или изменен другим пользователем с момента её загрузки"
                        , "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                OnViewModelDeleted($"Пользователь {u.Login} id = {u.Id} удален");

                UserViewModels.Remove(u);
            },
                u => u != null);
        }

        public override AsyncCommand Open
        {
            get => new AsyncCommand(async () =>
            {
                try
                {
                    if (_dialogService.OpenFileDialog())
                    {
                        List<SerializableUser> serializableUsers = null;

                        if (_dialogService.FilterIndex == 1)
                            serializableUsers = await _xmlFileService.OpenAsync(_dialogService.FilePath);

                        if (_dialogService.FilterIndex == 2)
                            serializableUsers = await _jsonFileService.OpenAsync(_dialogService.FilePath);

                        var dataTable = await _tablesBuilder.GetUsersTableAsync(serializableUsers);

                        var usersType = this.CreateSqlParameter(dataTable);

                        var result = await _repositoryService.ExecuteSqlQueryAsync("Exec UsersMerge_proc @UsersType", usersType);

                        if (result < 0)
                        {
                            _dialogService.ShowMessage($"Не удалось добавить данные в БД из файла {_dialogService.FilePath} " +
                                $"Возможно вы пытаетесь добавить пользователя несуществующей компании. Проверьте список загруженных компаний.");
                            return;
                        }

                        _log.Info($"файл {_dialogService.FilePath} открыт, список пользователей в БД обновлен");

                        await _repositoryService.UpdateDbContaextAsync();

                        UserViewModels.Clear();

                        _worker.RunWorkerAsync(new UserArguments(_repositoryService.GetUsers(), _repositoryService.GetUsersCount()));

                        _dialogService.ShowMessage("Список пользователей загружен, запущено обновление списка компаний");

                        UserViewModelsUpdated?.Invoke(this, new EventArgs());
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
                        var users = await this.GetUsers(UserViewModels);

                        var serializableUsers = await this.GetSerialisableUsers(users);

                        if (_dialogService.FilterIndex == 1)
                            await _xmlFileService.SaveAsync(_dialogService.FilePath, serializableUsers);

                        if (_dialogService.FilterIndex == 2)
                            await _jsonFileService.SaveAsync(_dialogService.FilePath, serializableUsers);

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

        public void UpdateUserViewModels()
        {
            UserViewModels.Clear();
            _worker.RunWorkerAsync(new UserArguments(_repositoryService.GetUsers(), _repositoryService.GetUsersCount()));
        }

        private void OnDoWork(object sender, DoWorkEventArgs e)
        {
            var arguments = e.Argument as UserArguments;
            var result = _repositoryService.GetUserViewModels(_worker, arguments.Users, arguments.UsersCount);
            e.Result = result;
        }

        private void OnProgressChanged(object sender, ProgressChangedEventArgs e) => CurrentProgress = e.ProgressPercentage;

        private void OnRunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                _log.Error($"Ошибка при выполнении метода OnRunWorkerCompleted во время загрузки списка пользователей\r\n{e.Error}");
                MessageBox.Show($"Не удалось загрузить список пользователей", "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                if (UserViewModels is null)
                    UserViewModels = e.Result as ObservableCollection<UserViewModel>;
                else
                {
                    foreach (var item in e.Result as ObservableCollection<UserViewModel>)
                    {
                        UserViewModels.Add(item);
                    }
                }

                SelectedUserViewModel = _entities[0];
            }
        }
    }
}
