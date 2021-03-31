using FarmerAndPartners.Helpers;
using FarmerAndPartnersModels;
using System;
using System.Collections.ObjectModel;

namespace FarmerAndPartners.ViewModels
{
    public class CompanyViewModel : Notifier, ICloneable
    {
        private readonly Company _company;
        private readonly ObservableCollection<User> _users;

        public CompanyViewModel() { }

        public CompanyViewModel(Company company)
        {
            _company = company;
            _users = new ObservableCollection<User>(_company.Users);
        }

        public Company Company => _company;

        public int Id
        {
            get => _company.Id;
            set
            {
                if (_company.Id != value)
                {
                    _company.Id = value;
                    OnPropertyChanged(nameof(Id));
                }
            }
        }

        public string Name
        {
            get => _company.Name;
            set
            {
                if (_company.Name != value)
                {
                    _company.Name = value;
                    OnPropertyChanged(nameof(Name));
                }
            }
        }

        public byte[] TimeStamp
        {
            get => _company.TimeStamp;
            set
            {
                if (_company.TimeStamp != value)
                {
                    _company.TimeStamp = value;
                    OnPropertyChanged(nameof(TimeStamp));
                }
            }
        }

        public int ContractStatusId
        {
            get => _company.ContractStatusId;
            set
            {
                if (_company.ContractStatusId != value)
                {
                    _company.ContractStatusId = value;
                    OnPropertyChanged(nameof(ContractStatusId));
                }
            }
        }

        public ContractStatus ContractStatus
        {
            get => _company.ContractStatus;
            set
            {
                if (_company.ContractStatus != value)
                {
                    _company.ContractStatus = value;
                    OnPropertyChanged(nameof(ContractStatus));
                }
            }
        }

        public ObservableCollection<User> Users
        {
            get => _users;
        }

        public void UpdateUsers()
        {
            _users.Clear();

            foreach (var item in _company.Users)
            {
                _users.Add(item);
            }
        }

        public object Clone()
        {
            var company = new Company()
            {
                Id = this.Company.Id,
                Name = this.Company.Name,
                TimeStamp = this.Company.TimeStamp,
                ContractStatusId = this.Company.ContractStatusId,
                ContractStatus = this.Company.ContractStatus,
                Users = this.Company.Users
            };

            return new CompanyViewModel(company);
        }

        public override string ToString() => $" {nameof(Id)}: {Id}" +
                                             $" {nameof(Name)}: {Name}" +
                                             $" {nameof(ContractStatusId)}: {ContractStatusId}" +
                                             $" {nameof(ContractStatus)}: {ContractStatus.Definition}" +
                                             $" {nameof(Users)}: {Users?.Count}";
    }
}
