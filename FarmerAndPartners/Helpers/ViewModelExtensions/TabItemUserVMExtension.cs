using FarmerAndPartners.Helpers.SerializableObjects;
using FarmerAndPartners.ViewModels;
using FarmerAndPartnersModels;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Threading.Tasks;

namespace FarmerAndPartners.Helpers.ViewModelExtensions
{
    public static class TabItemUserVMExtension
    {
        public static SqlParameter CreateSqlParameter(this TabItemUserViewModel tabItemUserViewModel, DataTable dataTable)
        {
            if (tabItemUserViewModel is null)
                throw new ArgumentNullException(nameof(tabItemUserViewModel));

            if (dataTable is null)
                throw new ArgumentNullException(nameof(dataTable));

            return new SqlParameter()
            {
                SqlDbType = SqlDbType.Structured,
                Direction = ParameterDirection.Input,
                ParameterName = "@UsersType",
                TypeName = "[dbo].[UsersType]",
                Value = dataTable
            };
        }

        public static async Task<List<User>> GetUsers(this TabItemUserViewModel tabItemUserViewModel, ObservableCollection<UserViewModel> userViewModels)
        {
            if (tabItemUserViewModel is null)
                throw new ArgumentNullException(nameof(tabItemUserViewModel));

            if (userViewModels is null)
                throw new ArgumentNullException(nameof(userViewModels));

            return await Task.Run(() =>
            {
                var users = new List<User>();

                foreach (var item in userViewModels)
                {
                    users.Add(item.User);
                }

                return users;
            });
        }

        public static async Task<List<SerializableUser>> GetSerialisableUsers(this TabItemUserViewModel tabItemUserViewModel, List<User> users)
        {
            if (tabItemUserViewModel is null)
                throw new ArgumentNullException(nameof(tabItemUserViewModel));

            if (users is null)
                throw new ArgumentNullException(nameof(users));

            return await Task.Run(() =>
            {
                var serializableUsers = new List<SerializableUser>();

                foreach (var user in users)
                {
                    serializableUsers.Add(new SerializableUser(user.Id, user.Name, user.Login, user.Password, user.CompanyId, user.Company.Name));
                }

                return serializableUsers;
            });
        }
    }
}
