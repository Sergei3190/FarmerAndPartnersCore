using FarmerAndPartners.Helpers.Interfaces;
using FarmerAndPartnersModels;
using FarmerAndPartners.ViewModels;
using FarmerAndPartners.Views;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;

namespace FarmerAndPartners.Helpers.NavigationServices
{
    public class CompanyNavigationService : INavigationService<CompanyViewModel, ContractStatus>
    {
        private Window _window;
        private CompanyViewModel _companyViewModel;

        public bool IsOriginalViewModelChanged { get; set; }

        public void OpenWindow(in CompanyViewModel viewModel, ObservableCollection<ContractStatus> nestedObjects, IList<string> displayMemberPaths)
        {
            if (_companyViewModel != null)
                _companyViewModel = null;

            var cloneViewModel = GetCloneViewModel(in viewModel);

            if (cloneViewModel != null)
            {
                if (IsOriginalViewModelChanged)
                    IsOriginalViewModelChanged = false;
            }

            _window = new AddEditCompany();
            _window.DataContext = new AddEditCompanyViewModel(this, nestedObjects, displayMemberPaths, cloneViewModel);
            _window.Owner = Application.Current.MainWindow;
            _window.ShowDialog();
        }

        public void AssertResultAddInWindow(CompanyViewModel cloneViewModel) => AddViewModel(cloneViewModel);
        public void AssertResultEditInWindow(CompanyViewModel cloneViewModel) => EditViewModel(cloneViewModel);
        public CompanyViewModel GetAddedViewModel() => _companyViewModel;
        public void CloseEditWindow() => _window.Close();

        private CompanyViewModel GetCloneViewModel(in CompanyViewModel viewModel)
        {
            if (viewModel is null)
                return null;

            _companyViewModel = viewModel;

            return viewModel.Clone() as CompanyViewModel;
        }

        private void EditViewModel(CompanyViewModel cloneViewModel)
        {
            _companyViewModel.Name = cloneViewModel.Name;
            _companyViewModel.ContractStatus = cloneViewModel.ContractStatus;

            IsOriginalViewModelChanged = true;
        }

        private void AddViewModel(CompanyViewModel cloneViewModel) => _companyViewModel = cloneViewModel;
    }
}
