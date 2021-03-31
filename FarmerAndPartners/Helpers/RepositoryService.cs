using FarmerAndPartners.Helpers.Interfaces;
using FarmerAndPartners.ViewModels;
using FarmerAndPartnersEF.Repository;
using FarmerAndPartnersModels;
using NLog;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading.Tasks;

namespace FarmerAndPartners.Helpers
{
    public class RepositoryService : IAsyncRepositoryService
    {
        private Repository _repository;
        private object _locker;

        public RepositoryService(ILogger logger)
        {
            _repository = new Repository(logger);
            _locker = new object();
            Logger = logger;
        }

        public ILogger Logger { get; }

        public IEnumerable<Company> GetCompanies() => _repository.GetCompanies();
        public IEnumerable<User> GetUsers() => _repository.GetUsers();
        public int GetCompaniesCount() => _repository.GetCompaniesCount();
        public int GetUsersCount() => _repository.GetUsersCount();
        public async Task DisposeDbContextAsync() => await _repository.DisposeAsync();
        public async Task UpdateDbContaextAsync()
        {
            await DisposeDbContextAsync();
            _repository = new Repository(Logger);
        }
        public async Task<int> AddCompanyAsync(Company company) => await _repository.AddCompanyAsync(company);
        public async Task<int> UpdateCompanyAsync(Company company) => await _repository.UpdateCompanyAsync(company);
        public async Task<int> DeleteCompanyAsync(Company company) => await _repository.DeleteCompanyAsync(company);
        public async Task<int> AddUserAsync(User user) => await _repository.AddUserAsync(user);
        public async Task<int> UpdateUserAsync(User user) => await _repository.UpdateUserAsync(user);
        public async Task<int> DeleteUserAsync(User user) => await _repository.DeleteUserAsync(user);

        public async Task<int> ExecuteSqlQueryAsync(string sqlQuery, params object[] sqlQueryParameters)
        {
            return await _repository.ExecuteQueryAsync(sqlQuery, sqlQueryParameters);
        }

        public async Task<ObservableCollection<CompanyViewModel>> GetCompanyViewModelsAsync()
        {
            var result = new ObservableCollection<CompanyViewModel>();

            var companies = await _repository.GetCompaniesAsync();

            foreach (var item in companies)
            {
                result.Add(new CompanyViewModel(item));
            }

            return result;
        }

        public ObservableCollection<ContractStatus> GetContractStatuses() 
        {
            var contractStatuses = _repository.GetContractStatuses();

            return new ObservableCollection<ContractStatus>(contractStatuses);
        } 

        public ObservableCollection<CompanyViewModel> GetCompanyViewModels(BackgroundWorker backgroundWorker, IEnumerable<Company> companies, int count)
        {
            lock (_locker)
            {
                var result = new ObservableCollection<CompanyViewModel>();

                var index = 0;

                while (index < count)
                {
                    foreach (var item in companies)
                    {
                        var companyViewModel = new CompanyViewModel(item);

                        if (!result.Contains(companyViewModel))
                            result.Add(companyViewModel);

                        if (backgroundWorker.WorkerReportsProgress)
                            backgroundWorker.ReportProgress(Convert.ToInt32((float)(index + 1) / count * 100));

                        index++;
                    }
                }

                if (result.Count == count)
                    backgroundWorker.ReportProgress(100);

                return result;
            }
        }

        public ObservableCollection<UserViewModel> GetUserViewModels(BackgroundWorker backgroundWorker, IEnumerable<User> users, int count)
        {
            lock (_locker)
            {
                var result = new ObservableCollection<UserViewModel>();

                var index = 0;

                while (index < count)
                {
                    foreach (var item in users)
                    {
                        var userViewModel = new UserViewModel(item);

                        if (!result.Contains(userViewModel))
                            result.Add(userViewModel);

                        if (backgroundWorker.WorkerReportsProgress)
                            backgroundWorker.ReportProgress(Convert.ToInt32((float)(index + 1) / count * 100));

                        index++;
                    }
                }

                if (result.Count == count)
                    backgroundWorker.ReportProgress(100);

                return result;
            }
        }
    }
}
