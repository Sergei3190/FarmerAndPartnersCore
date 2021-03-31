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
    public partial class AddEditCompanyViewModel : BaseAddEditViewModel
    {
        private readonly INavigationService<CompanyViewModel, ContractStatus> _navigationService;
        private readonly ObservableCollection<ContractStatus> _contractStatuses;
        private CompanyViewModel _companyViewModel;

        public AddEditCompanyViewModel(INavigationService<CompanyViewModel, ContractStatus> navigationService, ObservableCollection<ContractStatus> contractStatuses,
            IList<string> companyNames, CompanyViewModel cloneCompanyViewModel = null) : base(companyNames, cloneCompanyViewModel?.Name)
        {
            _navigationService = navigationService;
            _contractStatuses = contractStatuses;
            _companyViewModel = cloneCompanyViewModel;
        }

        public CompanyViewModel CompanyViewModel => _companyViewModel ?? (_companyViewModel = new CompanyViewModel(new Company()));
        public ObservableCollection<ContractStatus> ContractStatuses => _contractStatuses;

        [Required(ErrorMessage = "Введите имя компании, не более 50 символов")]
        [StringLength(50)]
        public string Name
        {
            get => CompanyViewModel.Name;
            set
            {
                if (CompanyViewModel.Name != value)
                {
                    CompanyViewModel.Name = value;
                    OnPropertyChanged(nameof(Name));
                }
            }
        }

        public int ContractStatusId
        {
            get => SelectedContractStatus.Id;
            set
            {
                if (SelectedContractStatus.Id != value)
                {
                    SelectedContractStatus.Id = value;
                    OnPropertyChanged(nameof(ContractStatusId));
                }
            }
        }

        public ContractStatus SelectedContractStatus
        {
            get
            {
                if (CompanyViewModel.ContractStatus is null)
                {
                    CompanyViewModel.ContractStatus = ContractStatuses[1];
                    return CompanyViewModel.ContractStatus;
                }
                else
                    return ContractStatuses.FirstOrDefault(c => c.Id == CompanyViewModel.ContractStatus.Id);
            }
            set
            {
                if (CompanyViewModel.ContractStatus != value)
                {
                    CompanyViewModel.ContractStatus = value;
                    OnPropertyChanged(nameof(SelectedContractStatus));
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
                if (!IsDataSaved)
                {
                    var message = MessageBox.Show("Сохранить внесенные изменения?", "Компания", MessageBoxButton.YesNoCancel, MessageBoxImage.Exclamation);

                    if (message == MessageBoxResult.Yes)
                    {
                        var isDataSaved = SaveCompany();

                        if (isDataSaved)
                            e.Cancel = false;
                    }

                    if (message == MessageBoxResult.Cancel)
                        e.Cancel = true;
                }
            },
                e => e != null && !IsDataSaved);
        }

        public override RelayCommand Save
        {
            get => new RelayCommand(() =>
            {
                var isDataSaved = SaveCompany();

                if (isDataSaved)
                    _navigationService.CloseEditWindow();
            });
        }

        private bool SaveCompany()
        {
            if (HasErrors)
            {
                MessageBox.Show($"Сохранение невозможно, пока не будут корректно заполнены все поля", "Компания", MessageBoxButton.OK, MessageBoxImage.Error);
                return IsDataSaved;
            }

            if (CompanyViewModel.Id == 0)
            {
                _navigationService.AssertResultAddInWindow(CompanyViewModel);
                MessageBox.Show($"Компания \"{CompanyViewModel.Name}\" добавлена", "Компания", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                _navigationService.AssertResultEditInWindow(CompanyViewModel);
                MessageBox.Show($"Изменения для компании \"{CompanyViewModel.Name}\" сохранены", "Компания", MessageBoxButton.OK, MessageBoxImage.Information);
            }

            return IsDataSaved = true;
        }
    }
}
