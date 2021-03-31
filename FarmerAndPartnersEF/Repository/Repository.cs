using FarmerAndPartnersEF.EF;
using FarmerAndPartnersModels;
using Microsoft.EntityFrameworkCore;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FarmerAndPartnersEF.Repository
{
    public class Repository : IAsyncDisposable, IAsyncRepository
    {
        private readonly FarmerAndPartnersContext _db;
        private readonly ILogger _log;

        public Repository(ILogger log) : this(new FarmerAndPartnersContext(), log) { }
        public Repository(FarmerAndPartnersContext context, ILogger log)
        {
            _db = context;
            _log = log;
        }

        protected FarmerAndPartnersContext Context => _db;

        public async ValueTask DisposeAsync() => await _db.DisposeAsync();

        public async Task<int> AddCompanyAsync(Company company)
        {
            await _db.Companies.AddAsync(company);
            return await SaveChangesAsync();
        }

        public async Task<int> AddUserAsync(User user)
        {
            await _db.Users.AddAsync(user);
            return await SaveChangesAsync();
        }

        public async Task<int> DeleteCompanyAsync(Company company)
        {
            _db.Companies.Remove(company);
            return await SaveChangesAsync();
        }

        public async Task<int> DeleteUserAsync(User user)
        {
            _db.Users.Remove(user);
            return await SaveChangesAsync();
        }

        public async Task<int> UpdateCompanyAsync(Company company)
        {
            _db.Companies.Update(company);
            return await SaveChangesAsync();
        }

        public async Task<int> UpdateUserAsync(User user)
        {
            _db.Users.Update(user);
            return await SaveChangesAsync();
        }

        public int GetCompaniesCount() => Context.Companies.Include(c => c.ContractStatus).Include(c => c.Users).Count();
        public int GetUsersCount() => Context.Users.Count();

        public async Task<int> ExecuteQueryAsync(string sql, params object[] sqlParameters)
        {
            using (var transaction = await Context.Database.BeginTransactionAsync())
            {
                try
                {
                    var result = await Context.Database.ExecuteSqlRawAsync(sql, sqlParameters);
                    await transaction.CommitAsync();
                    return result;
                }
                catch (Exception ex)
                {
                    _log.Error($"Ошибка при выполнении метода ExecuteQueryAsync{Environment.NewLine}{ex}");
                    await transaction.RollbackAsync();
                    return -1;
                }
            }
        }

        public async Task<List<Company>> GetCompaniesAsync() => await Context.Companies.Include(c => c.ContractStatus).Include(c => c.Users).ToListAsync();
        public List<ContractStatus> GetContractStatuses() => Context.ContractStatuses.ToList();
        public IEnumerable<Company> GetCompanies() => Context.Companies.Include(c => c.ContractStatus).Include(c => c.Users);
        public IEnumerable<User> GetUsers() => Context.Users.Include(u => u.Company);

        protected async Task<int> SaveChangesAsync()
        {
            try
            {
                return await _db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                _log.Error($"Ошибка параллелизма{Environment.NewLine}{ex}");
                return -1;
            }
            catch (DbUpdateException ex)
            {
                _log.Error($"Ошибка при обновлении базы данных{Environment.NewLine}{ex}");
                return -1;
            }
            catch (Exception ex)
            {
                _log.Error($"Ошибка при выполнении метода SaveChanges{Environment.NewLine}{ex}");
                return -1;
            }
        }
    }
}
