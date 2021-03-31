using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace FarmerAndPartners.Helpers
{
    public abstract class Notifier : INotifyPropertyChanged, INotifyDataErrorInfo
    {
        private readonly Dictionary<string, List<string>> _errors = new Dictionary<string, List<string>>();

        public event PropertyChangedEventHandler PropertyChanged;
        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

        public bool HasErrors => _errors.Count != 0;

        public IEnumerable GetErrors(string propertyName = "")
        {
            if (string.IsNullOrEmpty(propertyName))
                return _errors.Values;

            return _errors.ContainsKey(propertyName) ? _errors[propertyName] : null;
        }

        protected void AddError(string propertyName, string error)
        {
            AddErrors(propertyName, new List<string>() { error });
        }

        protected void AddErrors(string propertyName, IList<string> errors)
        {
            if (errors is null)
            {
                ClearErrors(propertyName);
                return;
            }

            var changed = false;

            if (!_errors.ContainsKey(propertyName))
            {
                _errors.Add(propertyName, new List<string>());

                changed = true;
            }

            foreach (var error in errors)
            {
                if (!_errors[propertyName].Contains(error))
                {
                    _errors[propertyName].Add(error);

                    if (!changed)
                        changed = true;
                }
            }

            if (changed)
                OnErrorsChanged(propertyName);
        }

        protected void ClearErrors(string propertyName = null)
        {
            if (string.IsNullOrEmpty(propertyName))
                _errors.Clear();
            else
                _errors.Remove(propertyName);

            OnErrorsChanged(propertyName);
        }

        protected string[] GetErrorFromAnnotations<T>(string propertyName, T value)
        {
            var results = new List<ValidationResult>();
            var vc = new ValidationContext(this) { MemberName = propertyName };
            var isValid = Validator.TryValidateProperty(value, vc, results);
            return isValid ? null : Array.ConvertAll(results.ToArray(), o => o.ErrorMessage);
        }

        protected void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        protected void OnErrorsChanged(string propertyName) => ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
    }
}
