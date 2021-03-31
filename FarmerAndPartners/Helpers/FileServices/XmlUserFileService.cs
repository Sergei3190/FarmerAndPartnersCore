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
    public class XmlUserFileService : IAsyncFileService<SerializableUser>
    {
        public async Task<List<SerializableUser>> OpenAsync(string fileName)
        {
            using (StreamReader sr = new StreamReader(fileName, new UTF8Encoding(false)))
            {
                XmlSerializer xmlSerializer = new XmlSerializer(typeof(UsersCollection));

                return await Task.Run(() =>
                {
                    return ((UsersCollection)xmlSerializer.Deserialize(sr)).Users;
                });
            }
        }

        public async Task SaveAsync(string fileName, List<SerializableUser> objects)
        {
            using (StreamWriter sw = new StreamWriter(fileName, false, new UTF8Encoding(false)))
            {
                XmlSerializer xmlSerializer = new XmlSerializer(typeof(UsersCollection));

                var ns = new XmlSerializerNamespaces();
                ns.Add("", "");

                await Task.Run(() => xmlSerializer.Serialize(sw, new UsersCollection() { Users = objects.ToList() }, ns));
            }
        }
    }
}
