using FarmerAndPartnersEF.Datalnitialization;
using Microsoft.Extensions.Configuration;
using System.IO;
using System.Threading.Tasks;
using System.Windows;

namespace FarmerAndPartners
{
    /// <summary>
    /// Логика взаимодействия для App.xaml
    /// </summary>
    public partial class App : Application
    {
        public IConfiguration AppConfiguration { get; private set; }

        private void OnStartup(object sender, StartupEventArgs e)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("AppSettings.json");

            AppConfiguration = builder.Build();

#if DEBUG
            #region Readme for Test
            // Для тестирования загрузки файлов xml и json во время работы приложения необходимо:
            // 1. Запустить приложение с пересозданием БД с её последующей инициализацией
            // 2. Выполнить в БД скрипт StoredProcedures.sql во время работы приложения
            #endregion
            var testContext = DataInitializer.GetTestContext();
            Task.Run(async () => await DataInitializer.RecreateDatabaseAsync(testContext)).Wait();
            Task.Run(async () => await DataInitializer.InitializeData(testContext)).Wait();
#endif
        }
    }
}
