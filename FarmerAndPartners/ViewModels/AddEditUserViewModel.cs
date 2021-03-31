using FarmerAndPartners.Commands;
using FarmerAndPartners.Helpers.Interfaces;
using FarmerAndPartnersModels;
using FarmerAndPartners.ViewModels.BaseViewModels;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Windows;

namespace FarmerAndPartners.ViewModels
{
    public partial class AddEditUserViewModel : BaseAddEditViewModel
    {
        private readonly INavigationService<UserViewModel, CompanyViewModel> _navigationService;
        private readonly ObservableCollection<CompanyViewModel> _companyViewModels;
        private UserViewModel _userViewModel;

        public AddEditUserViewModel(INavigationService<UserViewModel, CompanyViewModel> navigationService, ObservableCollection<CompanyViewModel> companyViewModels,
            IList<string> userLogins, UserViewModel cloneUserViewModel = null) : base(userLogins, cloneUserViewModel?.Login)
        {
            _navigationService = navigationService;
            _companyViewModels = companyViewModels;
            _userViewModel = cloneUserViewModel;
        }

        public UserViewModel UserViewModel => _userViewModel ?? (_userViewModel = new UserViewModel(new User() { Company = new Company() { Users = new List<User>() } }));
        public ObservableCollection<CompanyViewModel> CompanyViewModels => _companyViewModels;

        [Required(ErrorMessage = "Введите имя пользователя, не более 50 символов")]
        [StringLength(50)]
        [RegularExpression(@"^[A-я]*$", ErrorMessage = "Имя пользователя должно состоять только из букв")]
        public string Name
        {
            get => UserViewModel.Name;
            set
            {
                if (UserViewModel.Name != value)
                {
                    UserViewModel.Name = value;
                    OnPropertyChanged(nameof(Name));
                }
            }
        }

        [Required(ErrorMessage = "Введите логин, не более 50 символов")]
        [StringLength(50)]
        [RegularExpression(@"^[A-z]\w*\W*.*$", ErrorMessage = "Логин должен начинаться с буквы")]
        public string Login
        {
            get => UserViewModel.Login;
            set
            {
                if (UserViewModel.Login != value)
                {
                    UserViewModel.Login = value;
                    OnPropertyChanged(nameof(Login));
                }
            }
        }

        [Required(ErrorMessage = "Введите пароль, не более 50 символов и не менее 5")]
        [StringLength(50, MinimumLength = 5)]
        [RegularExpression(@"^[A-z]+[0-9]+.*\W+$|[0-9]+[A-z]+.*\W+$",
            ErrorMessage = "Слишком простой пароль. Следуйте следующему шаблону:\r\n\"комбинация букв и цифр или цифр и букв ->\r\nлюбое количество букв или цифр " +
                           "или знаков или их сочетание ->\r\nлюбое кол-во знаков\"")]
        public string Password
        {
            get => UserViewModel.Password;
            set
            {
                if (UserViewModel.Password != value)
                {
                    UserViewModel.Password = value;
                    OnPropertyChanged(nameof(Password));
                }
            }
        }

        public int CompanyViewModelId
        {
            get => SelectedCompanyViewModel.Id;
            set
            {
                if (SelectedCompanyViewModel.Id != value)
                {
                    SelectedCompanyViewModel.Id = value;
                    OnPropertyChanged(nameof(CompanyViewModelId));
                }
            }
        }

        public CompanyViewModel SelectedCompanyViewModel
        {
            get
            {
                if (UserViewModel.CompanyViewModel?.Name is null)
                {
                    UserViewModel.CompanyViewModel = CompanyViewModels[0];
                    return UserViewModel.CompanyViewModel;
                }
                else
                    return CompanyViewModels.FirstOrDefault(c => c.Id == UserViewModel.CompanyViewModel.Id);
            }
            set
            {
                if (UserViewModel.CompanyViewModel != value)
                {
                    UserViewModel.CompanyViewModel = value;
                    OnPropertyChanged(nameof(SelectedCompanyViewModel));
                }
            }
        }

        public override RelayCommand Cancel
        {
            get => new RelayCommand(() =>
            {
                _navigationService.CloseEditWindow();
            },
                () => _navigationService != null);
        }

        public override RelayCommandT<CancelEventArgs> Closing
        {
            get => new RelayCommandT<CancelEventArgs>(e =>
            {
                var message = MessageBox.Show("Сохранить внесенные изменения?", "Пользователь", MessageBoxButton.YesNoCancel, MessageBoxImage.Exclamation);

                if (message == MessageBoxResult.Yes)
                {
                    var isDataSaved = SaveUser();

                    if (IsDataSaved)
                        e.Cancel = false;
                }

                if (message == MessageBoxResult.Cancel)
                    e.Cancel = true;
            },
                e => e != null && !IsDataSaved);
        }

        public override RelayCommand Save
        {
            get => new RelayCommand(() =>
            {
                var isDataSaved = SaveUser();

                if (isDataSaved)
                    _navigationService.CloseEditWindow();
            });
        }

        private bool SaveUser()
        {
            if (HasErrors)
            {
                MessageBox.Show($"Сохранение невозможно, пока не будут корректно заполнены все поля", "Пользователь", MessageBoxButton.OK, MessageBoxImage.Error);
                return IsDataSaved;
            }

            if (UserViewModel.Id == 0)
            {
                _navigationService.AssertResultAddInWindow(UserViewModel);
                MessageBox.Show($"Пользователь \"{UserViewModel.Name}\" добавлен", "Пользователь", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                _navigationService.AssertResultEditInWindow(UserViewModel);
                MessageBox.Show($"Изменения для пользователя \"{UserViewModel.Name}\" сохранены", "Пользователя", MessageBoxButton.OK, MessageBoxImage.Information);
            }

            return IsDataSaved = true;
        }
    }
}
