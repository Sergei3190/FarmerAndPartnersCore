using FarmerAndPartners.Helpers.Interfaces;
using NLog;
using System.ComponentModel;
using System.Text;

namespace FarmerAndPartners.Helpers.NestedTypesFactories.BaseFactory
{
    public class BaseNestedTypesFactory
    {
        public ILogger CreateLogger(string name) => LogManager.GetLogger(name);
        public IAsyncRepositoryService CreateRepositoryService(ILogger logger) => new RepositoryService(logger);
        public IDialogService CreateDialogService() => new DialogService();
        public IAsyncCustomTablesBuilder CreateCustomTablesBuilder() => new CustomTablesBuilder();
        public BackgroundWorker CreateBackgroundWorker() => new BackgroundWorker();
        public StringBuilder CreateStringBuilder() => new StringBuilder();
    }
}
