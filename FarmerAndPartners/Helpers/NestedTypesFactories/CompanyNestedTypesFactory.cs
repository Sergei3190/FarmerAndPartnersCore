using FarmerAndPartners.Helpers.FileServices;
using FarmerAndPartners.Helpers.Interfaces;
using FarmerAndPartners.Helpers.NavigationServices;
using FarmerAndPartners.Helpers.NestedTypesFactories.BaseFactory;
using FarmerAndPartners.Helpers.SerializableObjects;
using FarmerAndPartnersModels;
using FarmerAndPartners.ViewModels;

namespace FarmerAndPartners.NestedTypesFactories
{
    public class CompanyNestedTypesFactory : BaseNestedTypesFactory
    {
        public INavigationService<CompanyViewModel, ContractStatus> CreateNavigationService() => new CompanyNavigationService();
        public IAsyncFileService<SerializableCompany> CreateJsonCompanyFileService() => new JsonCompanyFileService();
        public IAsyncFileService<SerializableCompany> CreateXmlCompanyFileService() => new XmlCompanyFileService();
    }
}
