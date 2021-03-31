using FarmerAndPartners.Helpers.Interfaces;
using FarmerAndPartners.Helpers.SerializableObjects;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace FarmerAndPartners.Helpers.FileServices
{
    public class JsonUserFileService : IAsyncFileService<SerializableUser>
    {
        public async Task<List<SerializableUser>> OpenAsync(string fileName)
        {
            using (StreamReader sr = new StreamReader(fileName, new UTF8Encoding(false)))
            {
                var jsonString = await sr.ReadToEndAsync();

                return await Task.Run(() =>
                {
                    return JsonConvert.DeserializeObject<List<SerializableUser>>(jsonString);
                });
            }
        }

        public async Task SaveAsync(string fileName, List<SerializableUser> objects)
        {
            using (StreamWriter sw = new StreamWriter(fileName, false, new UTF8Encoding(false)))
            {
                var serializedObjects = await Task.Run(() => JsonConvert.SerializeObject(objects, Formatting.Indented));

                await sw.WriteAsync(serializedObjects);
            }
        }
    }
}
