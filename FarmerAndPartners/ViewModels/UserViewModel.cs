using FarmerAndPartners.Helpers;
using FarmerAndPartnersModels;
using System;

namespace FarmerAndPartners.ViewModels
{
    public class UserViewModel : Notifier, ICloneable
    {
        private readonly User _user;
        private CompanyViewModel _companyViewModel;

        public UserViewModel() { }

        public UserViewModel(User user)
        {
            _user = user;
            _companyViewModel = new CompanyViewModel(_user.Company);
        }

        public User User => _user;

        public int Id
        {
            get => _user.Id;
            set
            {
                if (_user.Id != value)
                {
                    _user.Id = value;
                    OnPropertyChanged(nameof(Id));
                }
            }
        }

        public string Name
        {
            get => _user.Name;
            set
            {
                if (_user.Name != value)
                {
                    _user.Name = value;
                    OnPropertyChanged(nameof(Name));
                }
            }
        }

        public byte[] TimeStamp
        {
            get => _user.TimeStamp;
            set
            {
                if (_user.TimeStamp != value)
                {
                    _user.TimeStamp = value;
                    OnPropertyChanged(nameof(TimeStamp));
                }
            }
        }

        public string Login
        {
            get => _user.Login;
            set
            {
                if (_user.Login != value)
                {
                    _user.Login = value;
                    OnPropertyChanged(nameof(Login));
                }
            }
        }
        public string Password
        {
            get => _user.Password;
            set
            {
                if (_user.Password != value)
                {
                    _user.Password = value;
                    OnPropertyChanged(nameof(Password));
                }
            }
        }

        public int CompanyId
        {
            get => _user.CompanyId;
            set
            {
                if (_user.CompanyId != value)
                {
                    _user.CompanyId = value;
                    OnPropertyChanged(nameof(CompanyId));
                }
            }
        }

        public CompanyViewModel CompanyViewModel
        {
            get => _companyViewModel;
            set
            {
                if (_companyViewModel != value)
                {
                    _companyViewModel = value;

                    if (value != null)
                    {
                        _user.Company = value.Company;
                        _user.CompanyId = (int)(_user.Company?.Id);
                    }

                    OnPropertyChanged(nameof(CompanyViewModel));
                }
            }
        }

        public object Clone()
        {
            var user = new User()
            {
                Id = this.User.Id,
                Name = this.User.Name,
                TimeStamp = this.User.TimeStamp,
                Login = this.User.Login,
                Password = this.User.Password,
                CompanyId = this.User.CompanyId,
                Company = this.User.Company
            };

            return new UserViewModel(user);
        }

        public override string ToString() => $" {nameof(Id)}: {Id}" +
                                             $" {nameof(Name)}: {Name}" +
                                             $" {nameof(Login)}: {Login}," +
                                             $" {nameof(Password)}: {Password}" +
                                             $" {nameof(CompanyId)}: {CompanyId}" +
                                             $" {nameof(CompanyViewModel)}: {CompanyViewModel?.Name}";
    }
}
