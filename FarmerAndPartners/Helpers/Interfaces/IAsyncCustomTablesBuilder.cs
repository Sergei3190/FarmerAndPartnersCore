using FarmerAndPartners.Helpers.SerializableObjects;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace FarmerAndPartners.Helpers.Interfaces
{
    public interface IAsyncCustomTablesBuilder
    {
        Task<DataTable> GetCompaniesTableAsync(IList<SerializableCompany> companies);
        Task<DataTable> GetUsersTableAsync(IList<SerializableUser> users);
    }
}
