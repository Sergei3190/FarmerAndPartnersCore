using FarmerAndPartners.Helpers.Interfaces;
using FarmerAndPartners.Helpers.SerializableObjects;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace FarmerAndPartners.Helpers.FileServices
{
    public class XmlCompanyFileService : IAsyncFileService<SerializableCompany>
    {
        public async Task<List<SerializableCompany>> OpenAsync(string fileName)
        {
            using (StreamReader sr = new StreamReader(fileName, new UTF8Encoding(false)))
            {
                XmlSerializer xmlSerializer = new XmlSerializer(typeof(CompanyCollection));

                return await Task.Run(() =>
                {
                    return ((CompanyCollection)xmlSerializer.Deserialize(sr)).Companies;
                });
            }
        }

        public async Task SaveAsync(string fileName, List<SerializableCompany> objects)
        {
            using (StreamWriter sw = new StreamWriter(fileName, false, new UTF8Encoding(false)))
            {
                XmlSerializer xmlSerializer = new XmlSerializer(typeof(CompanyCollection));

                var ns = new XmlSerializerNamespaces();
                ns.Add("", "");

                await Task.Run(() => xmlSerializer.Serialize(sw, new CompanyCollection() { Companies = objects.ToList() }, ns));
            }
        }
    }
}
