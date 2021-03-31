using FarmerAndPartners.Helpers.Interfaces;
using FarmerAndPartners.Helpers.SerializableObjects;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace FarmerAndPartners.Helpers.FileServices
{
    public class JsonCompanyFileService : IAsyncFileService<SerializableCompany>
    {
        public async Task<List<SerializableCompany>> OpenAsync(string fileName)
        {
            using (StreamReader sr = new StreamReader(fileName, new UTF8Encoding(false)))
            {
                var jsonString = await sr.ReadToEndAsync();

                return await Task.Run(() =>
                {
                    return JsonConvert.DeserializeObject<List<SerializableCompany>>(jsonString);
                });
            }
        }

        public async Task SaveAsync(string fileName, List<SerializableCompany> objects)
        {
            using (StreamWriter sw = new StreamWriter(fileName, false, new UTF8Encoding(false)))
            {
                var serializedObjects = await Task.Run(() => JsonConvert.SerializeObject(objects, Formatting.Indented));

                await sw.WriteAsync(serializedObjects);
            }
        }
    }
}
