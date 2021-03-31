using System;
using System.ComponentModel;

namespace FarmerAndPartners.ViewModels
{
    public partial class AddEditCompanyViewModel : IDataErrorInfo
    {
        public string Error => throw new NotImplementedException();
        public string this[string columnName]
        {
            get
            {
                var hasError = false;

                switch (columnName)
                {
                    case nameof(Name):
                        hasError = CheckObjectInitializer(nameof(Name), Name, "Компания с таким именем уже существует");
                        if (!hasError)
                            ClearErrors(nameof(Name));
                        else
                            return GetErrors(nameof(Name)).ToString();
                        AddErrors(nameof(Name), GetErrorFromAnnotations(nameof(Name), Name));
                        break;
                }

                return string.Empty;
            }
        }
    }
}
