using FarmerAndPartners.Helpers.SerializableObjects;
using FarmerAndPartnersModels;
using FarmerAndPartners.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Threading.Tasks;
using System.Windows;

namespace FarmerAndPartners.Helpers.ViewModelExtensions
{
    public static class TabItemCompanyVMExtension
    {
        public static SqlParameter[] GetSqlParameters(this TabItemCompanyViewModel tabItemCompanyViewModel, params DataTable[] dataTables)
        {
            if (tabItemCompanyViewModel is null)
                throw new ArgumentNullException(nameof(tabItemCompanyViewModel));

            if (dataTables is null)
                throw new ArgumentNullException(nameof(dataTables));

            var companyTableName = (Application.Current as App).AppConfiguration.GetSection("AppSettings")["CompaniesTableName"];
            var userTableName = (Application.Current as App).AppConfiguration.GetSection("AppSettings")["UsersTableName"];

            var sqlParameters = new SqlParameter[2];

            for (int i = 0; i < dataTables.Length; i++)
            {
                switch (dataTables[i].TableName)
                {
                    case string name when name.Equals(companyTableName, StringComparison.OrdinalIgnoreCase):
                        sqlParameters.SetValue(new SqlParameter
                        {
                            SqlDbType = SqlDbType.Structured,
                            Direction = ParameterDirection.Input,
                            ParameterName = "@CompaniesType",
                            TypeName = "[dbo].[CompaniesType]",
                            Value = dataTables[i]
                        }, 0);
                        break;
                    case string name when name.Equals(userTableName, StringComparison.OrdinalIgnoreCase):
                        sqlParameters.SetValue(new SqlParameter
                        {
                            SqlDbType = SqlDbType.Structured,
                            Direction = ParameterDirection.Input,
                            ParameterName = "@UsersType",
                            TypeName = "[dbo].[UsersType]",
                            Value = dataTables[i]
                        }, 1);
                        break;
                    default:
                        throw new ArgumentException(nameof(dataTables));
                }
            }

            return sqlParameters;
        }

        public static async Task<List<Company>> GetCompanies(this TabItemCompanyViewModel tabItemCompanyViewModel, ObservableCollection<CompanyViewModel> companyViewModels)
        {
            if (tabItemCompanyViewModel is null)
                throw new ArgumentNullException(nameof(tabItemCompanyViewModel));

            if (companyViewModels is null)
                throw new ArgumentNullException(nameof(companyViewModels));

            return await Task.Run(() =>
            {
                var companies = new List<Company>();

                foreach (var item in companyViewModels)
                {
                    companies.Add(item.Company);
                }

                return companies;
            });
        }

        public static async Task<List<SerializableCompany>> GetSerialisableCompanies(this TabItemCompanyViewModel tabItemCompanyViewModel, List<Company> companies)
        {
            if (tabItemCompanyViewModel is null)
                throw new ArgumentNullException(nameof(tabItemCompanyViewModel));

            if (companies is null)
                throw new ArgumentNullException(nameof(companies));

            return await Task.Run(() =>
            {
                var serializableCompanies = new List<SerializableCompany>();

                foreach (var item in companies)
                {
                    var users = new List<SerializableUser>();

                    foreach (var user in item.Users)
                    {
                        users.Add(new SerializableUser(user.Id, user.Name, user.Login, user.Password, user.CompanyId, user.Company.Name));
                    }

                    serializableCompanies.Add(new SerializableCompany(item.Id, item.Name, item.ContractStatusId,
                        item.ContractStatus.Definition, new UsersCollection() { Users = users }));
                }

                return serializableCompanies;
            });
        }

        public static async Task<List<SerializableUser>> GetSerialisableUsers(this TabItemCompanyViewModel tabItemCompanyViewModel, IEnumerable<UsersCollection> usersCollection)
        {
            if (tabItemCompanyViewModel is null)
                throw new ArgumentNullException(nameof(tabItemCompanyViewModel));

            if (usersCollection is null)
                throw new ArgumentNullException(nameof(usersCollection));

            return await Task.Run(() =>
            {
                var serializableUsers = new List<SerializableUser>();

                foreach (var item in usersCollection)
                {
                    if (item.Users.Count > 0)
                    {
                        serializableUsers.AddRange(item.Users);
                    }
                }

                return serializableUsers;
            });
        }
    }
}
