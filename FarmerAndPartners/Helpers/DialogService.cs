using FarmerAndPartners.Helpers.Interfaces;
using Microsoft.Win32;
using System.Windows;

namespace FarmerAndPartners.Helpers
{
    public class DialogService : IDialogService
    {
        private const string Filter = "Xml|*.xml|Json|*.json";

        public string FilePath { get; set; }
        public int FilterIndex { get; set; }

        public bool OpenFileDialog()
        {
            var openFileDialog = new OpenFileDialog();

            openFileDialog.Filter = Filter;

            if (openFileDialog.ShowDialog() == true)
            {
                FilterIndex = openFileDialog.FilterIndex;
                FilePath = openFileDialog.FileName;
                return true;
            }

            return false;
        }

        public bool SaveFileDialog()
        {
            var saveFileDialog = new SaveFileDialog();

            saveFileDialog.Filter = Filter;

            if (saveFileDialog.ShowDialog() == true)
            {
                FilterIndex = saveFileDialog.FilterIndex;
                FilePath = saveFileDialog.FileName;
                return true;
            }

            return false;
        }

        public void ShowMessage(string message) => MessageBox.Show(message);
    }
}
