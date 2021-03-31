using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace FarmerAndPartners.Helpers.Interfaces
{
    public interface INavigationService<T, K>
    {
        bool IsOriginalViewModelChanged { get; set; }
        void OpenWindow(in T viewModel, ObservableCollection<K> nestedObjects, IList<string> displayMemberPaths);
        void AssertResultEditInWindow(T cloneViewModel);
        void AssertResultAddInWindow(T cloneViewModel);
        T GetAddedViewModel();
        void CloseEditWindow();
    }
}
