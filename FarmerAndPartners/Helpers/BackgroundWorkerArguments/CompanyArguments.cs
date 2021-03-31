using FarmerAndPartnersModels;
using System.Collections.Generic;

namespace FarmerAndPartners.Helpers.BackgroundWorkerArguments
{
    public class CompanyArguments
    {
        public CompanyArguments(IEnumerable<Company> companies, int companiesCount)
        {
            Companies = companies;
            CompaniesCount = companiesCount;
        }

        public IEnumerable<Company> Companies { get; }
        public int CompaniesCount { get; }
    }
}
