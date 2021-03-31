using System;
using System.ComponentModel;

namespace FarmerAndPartners.ViewModels
{
    public partial class AddEditUserViewModel : IDataErrorInfo
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
                        AddErrors(nameof(Name), GetErrorFromAnnotations(nameof(Name), Name));
                        break;
                    case nameof(Login):
                        hasError = CheckObjectInitializer(nameof(Login), Login, "Пользователь с таким логином уже существует");
                        if (!hasError)
                            ClearErrors(nameof(Login));
                        else
                            return GetErrors(nameof(Login)).ToString();
                        AddErrors(nameof(Login), GetErrorFromAnnotations(nameof(Login), Login));
                        break;
                    case nameof(Password):
                        AddErrors(nameof(Password), GetErrorFromAnnotations(nameof(Password), Password));
                        break;
                }

                return string.Empty;
            }
        }
    }
}
