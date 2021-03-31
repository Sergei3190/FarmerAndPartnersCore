using FarmerAndPartners.Helpers.Interfaces;
using FarmerAndPartners.Helpers.SerializableObjects;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using System.Windows;

namespace FarmerAndPartners.Helpers
{
    public class CustomTablesBuilder : IAsyncCustomTablesBuilder
    {
        public async Task<DataTable> GetCompaniesTableAsync(IList<SerializableCompany> companies)
        {
            if (companies is null || companies.Count == 0)
                return null;

            return await Task.Run(() =>
            {
                var dataTable = CreateCompaniesTable();

                DataRow row = null;

                foreach (var item in companies)
                {
                    row = dataTable.NewRow();

                    row["Id"] = item.Id;
                    row["Name"] = item.Name;
                    row["ContractStatusId"] = item.ContractStatusId;

                    dataTable.Rows.Add(row);
                }

                return dataTable;
            });
        }

        public async Task<DataTable> GetUsersTableAsync(IList<SerializableUser> users)
        {
            if (users is null || users.Count == 0)
                return null;

            return await Task.Run(() =>
            {
                var dataTable = CreateUsersTable();

                DataRow row = null;

                foreach (var item in users)
                {
                    row = dataTable.NewRow();

                    row["Id"] = item.Id;
                    row["Name"] = item.Name;
                    row["Login"] = item.Login;
                    row["Password"] = item.Password;
                    row["CompanyId"] = item.CompanyId;

                    dataTable.Rows.Add(row);
                }

                return dataTable;
            });
        }

        private DataTable CreateCompaniesTable()
        {
            var dataTable = new DataTable((Application.Current as App).AppConfiguration.GetSection("AppSettings")["CompaniesTableName"]);

            dataTable.Columns.AddRange(new DataColumn[]
            {
                new DataColumn("Id", typeof(int)),
                new DataColumn("Name", typeof(string)),
                new DataColumn("ContractStatusId", typeof(int))
            });

            return dataTable;
        }

        private DataTable CreateUsersTable()
        {
            var dataTable = new DataTable((Application.Current as App).AppConfiguration.GetSection("AppSettings")["UsersTableName"]);

            dataTable.Columns.AddRange(new DataColumn[]
            {
                new DataColumn("Id", typeof(int)),
                new DataColumn("Name", typeof(string)),
                new DataColumn("Login", typeof(string)),
                new DataColumn("Password", typeof(string)),
                new DataColumn("CompanyId", typeof(int))
            });

            return dataTable;
        }
    }
}
