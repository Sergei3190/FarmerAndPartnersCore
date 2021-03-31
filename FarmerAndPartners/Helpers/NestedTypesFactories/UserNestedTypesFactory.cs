using FarmerAndPartners.Helpers.FileServices;
using FarmerAndPartners.Helpers.Interfaces;
using FarmerAndPartners.Helpers.NavigationServices;
using FarmerAndPartners.Helpers.NestedTypesFactories.BaseFactory;
using FarmerAndPartners.Helpers.SerializableObjects;
using FarmerAndPartners.ViewModels;

namespace FarmerAndPartners.NestedTypesFactories
{
    public class UserNestedTypesFactory : BaseNestedTypesFactory
    {
        public INavigationService<UserViewModel, CompanyViewModel> CreateUserNavigationService() => new UserNavigationService();
        public IAsyncFileService<SerializableUser> CreateJsonUserFileService() => new JsonUserFileService();
        public IAsyncFileService<SerializableUser> CreateXmlUserFileService() => new XmlUserFileService();
    }
}
