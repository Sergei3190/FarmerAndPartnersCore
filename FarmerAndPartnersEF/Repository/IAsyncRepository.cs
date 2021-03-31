using FarmerAndPartnersModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FarmerAndPartnersEF.Repository
{
    public interface IAsyncRepository
    {
        Task<int> AddCompanyAsync(Company company);
        Task<int> DeleteCompanyAsync(Company company);
        Task<int> UpdateCompanyAsync(Company company);
        Task<int> AddUserAsync(User user);
        Task<int> DeleteUserAsync(User user);
        Task<int> UpdateUserAsync(User user);
        Task<int> ExecuteQueryAsync(string sql, params object[] sqlParameters);
        Task<List<Company>> GetCompaniesAsync();
        List<ContractStatus> GetContractStatuses();
        IEnumerable<Company> GetCompanies();
        IEnumerable<User> GetUsers();
        int GetCompaniesCount();
        int GetUsersCount();
    }
}
