using FarmerAndPartners.Helpers.Interfaces;
using FarmerAndPartners.ViewModels;
using FarmerAndPartners.Views;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;

namespace FarmerAndPartners.Helpers.NavigationServices
{
    public class UserNavigationService : INavigationService<UserViewModel, CompanyViewModel>
    {
        private Window _window;
        private UserViewModel _userViewModel;

        public bool IsOriginalViewModelChanged { get; set; }

        public void OpenWindow(in UserViewModel viewModel, ObservableCollection<CompanyViewModel> nestedObjects, IList<string> displayMemberPaths)
        {
            if (_userViewModel != null)
                _userViewModel = null;

            var cloneViewModel = GetCloneViewModel(in viewModel);

            if (cloneViewModel != null)
            {
                if (IsOriginalViewModelChanged)
                    IsOriginalViewModelChanged = false;
            }

            _window = new AddEditUser();
            _window.DataContext = new AddEditUserViewModel(this, nestedObjects, displayMemberPaths, cloneViewModel);
            _window.Owner = Application.Current.MainWindow;
            _window.ShowDialog();
        }

        public void AssertResultAddInWindow(UserViewModel cloneViewModel) => AddViewModel(cloneViewModel);
        public void AssertResultEditInWindow(UserViewModel cloneViewModel) => EditViewModel(cloneViewModel);
        public UserViewModel GetAddedViewModel() => _userViewModel;
        public void CloseEditWindow() => _window.Close();

        private UserViewModel GetCloneViewModel(in UserViewModel viewModel)
        {
            if (viewModel is null)
                return null;

            _userViewModel = viewModel;

            return viewModel.Clone() as UserViewModel;
        }

        private void EditViewModel(UserViewModel cloneViewModel)
        {
            _userViewModel.Name = cloneViewModel.Name;
            _userViewModel.Login = cloneViewModel.Login;
            _userViewModel.Password = cloneViewModel.Password;
            _userViewModel.CompanyViewModel = cloneViewModel.CompanyViewModel;

            IsOriginalViewModelChanged = true;
        }

        private void AddViewModel(UserViewModel cloneViewModel) => _userViewModel = cloneViewModel;
    }
}
