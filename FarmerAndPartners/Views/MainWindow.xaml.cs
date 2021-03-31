using FarmerAndPartners.Helpers.NestedTypesFactories.BaseFactory;
using FarmerAndPartners.ViewModels;
using System.Windows;

namespace FarmerAndPartners.Views
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
      
            tabControlRoot.DataContext = new MainWindowViewModel(new BaseNestedTypesFactory());
        }
    }
}
