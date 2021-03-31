using FarmerAndPartners.Commands;
using FarmerAndPartners.Helpers;
using System.Collections.Generic;
using System.ComponentModel;

namespace FarmerAndPartners.ViewModels.BaseViewModels
{
    public abstract class BaseAddEditViewModel : Notifier
    {
        protected readonly IList<string> _objectInitializers;
        protected readonly string _originalObjectInitializer;

        public BaseAddEditViewModel(IList<string> objectInitializers, string originalObjectInitializer = null)
        {
            _objectInitializers = objectInitializers;
            _originalObjectInitializer = originalObjectInitializer;
        }

        public abstract RelayCommand Cancel { get; }
        public abstract RelayCommandT<CancelEventArgs> Closing { get; }
        public abstract RelayCommand Save { get; }
        protected bool IsDataSaved { get; set; }

        protected bool CheckObjectInitializer(string propertyName, string value, string errorMessage)
        {
            if (_originalObjectInitializer != null)
                _objectInitializers.Remove(_originalObjectInitializer);

            if (_objectInitializers.Contains(value))
            {
                AddError(propertyName, errorMessage);
                return true;
            }

            return false;
        }
    }
}
