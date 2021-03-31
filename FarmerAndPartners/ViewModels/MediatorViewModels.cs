using FarmerAndPartners.Helpers;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FarmerAndPartners.ViewModels
{
    public class MediatorViewModels
    {
        private readonly Logger _log;
        private readonly TabItemCompanyViewModel _tabItemCompanyViewModel;
        private readonly TabItemUserViewModel _tabItemUserViewModel;

        public MediatorViewModels(TabItemCompanyViewModel tabItemCompanyViewModel, TabItemUserViewModel tabItemUserViewModel)
        {
            _log = LogManager.GetLogger(nameof(MediatorViewModels));
            _tabItemCompanyViewModel = tabItemCompanyViewModel;
            _tabItemUserViewModel = tabItemUserViewModel;

            _tabItemCompanyViewModel.ViewModelChanged += OnCompanyViewModelChanged;
            _tabItemCompanyViewModel.ViewModelDeleted += OnCompanyViewModelDeleted;
            _tabItemCompanyViewModel.ViewModelAdded += OnCompanyViewModelAdded;
            _tabItemCompanyViewModel.CompanyViewModelsUpdated += OnCompanyViewModelsUpdated;

            _tabItemUserViewModel.ViewModelChanged += OnUserViewModelChanged;
            _tabItemUserViewModel.ViewModelDeleted += OnUserViewModelDeleted;
            _tabItemUserViewModel.ViewModelAdded += OnUserViewModelAdded;
            _tabItemUserViewModel.UserViewModelsUpdated += OnUserViewModelsUpdated;
        }

        private void OnUserViewModelsUpdated(object sender, EventArgs e) => _tabItemCompanyViewModel.UpdateCompanyViewModels();
        private void OnCompanyViewModelsUpdated(object sender, EventArgs e) => _tabItemUserViewModel.UpdateUserViewModels();
        private void OnCompanyViewModelAdded(object sender, ViewModelEventArgs e) => _log.Info(e.message);

        private void OnUserViewModelAdded(object sender, ViewModelEventArgs e)
        {
            _log.Info(e.message);

            var addedUser = (sender as TabItemUserViewModel).UserViewModels.Last();

            var editedCompanyViewModel = _tabItemCompanyViewModel.CompanyViewModels.Where(c => c.Id == addedUser.CompanyViewModel.Id).FirstOrDefault();

            editedCompanyViewModel.Users.Add(addedUser.User);

            editedCompanyViewModel.UpdateUsers();
        }

        private void OnUserViewModelDeleted(object sender, ViewModelEventArgs e)
        {
            _log.Info(e.message);

            var editedCompanyViewModel = _tabItemCompanyViewModel.CompanyViewModels.Where(c => c.Id == _tabItemUserViewModel.SelectedUserViewModel.CompanyId).FirstOrDefault();

            editedCompanyViewModel.Users.Remove(_tabItemUserViewModel.SelectedUserViewModel.User);

            editedCompanyViewModel.UpdateUsers();
        }

        private void OnCompanyViewModelDeleted(object sender, ViewModelEventArgs e)
        {
            _log.Info(e.message);

            var userViewModels = _tabItemUserViewModel.UserViewModels.Where(u => u.CompanyId == _tabItemCompanyViewModel.SelectedCompanyViewModel.Id);

            var userViewModelsOnDeletion = new List<UserViewModel>();

            userViewModelsOnDeletion.AddRange(userViewModels);

            foreach (var item in userViewModelsOnDeletion)
            {
                _tabItemUserViewModel.UserViewModels.Remove(item);
            }
        }

        private void OnUserViewModelChanged(object sender, ViewModelEventArgs e)
        {
            _log.Info(e.message);

            var editedCompanyViewModel = _tabItemCompanyViewModel.CompanyViewModels.Where(c => c.Id == _tabItemUserViewModel.SelectedUserViewModel.CompanyId).FirstOrDefault();

            editedCompanyViewModel.UpdateUsers();

            if (editedCompanyViewModel.Id != e.oldViewModelId)
            {
                var oldcompanyviewmodel = _tabItemCompanyViewModel.CompanyViewModels.Where(c => c.Id == e.oldViewModelId).FirstOrDefault();

                oldcompanyviewmodel.Users.Remove(_tabItemUserViewModel.SelectedUserViewModel.User);

                oldcompanyviewmodel.UpdateUsers();
            }
        }

        private void OnCompanyViewModelChanged(object sender, ViewModelEventArgs e)
        {
            _log.Info(e.message);

            var editeddUserViewModels = _tabItemUserViewModel.UserViewModels.Where(u => u.CompanyId == _tabItemCompanyViewModel.SelectedCompanyViewModel.Id);

            foreach (var item in editeddUserViewModels)
            {
                if (!item.CompanyViewModel.Equals(_tabItemCompanyViewModel.SelectedCompanyViewModel))
                    item.CompanyViewModel = _tabItemCompanyViewModel.SelectedCompanyViewModel;
            }
        }
    }
}
