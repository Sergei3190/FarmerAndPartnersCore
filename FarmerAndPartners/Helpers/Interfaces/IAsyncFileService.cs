using System.Collections.Generic;
using System.Threading.Tasks;

namespace FarmerAndPartners.Helpers.Interfaces
{
    public interface IAsyncFileService<T>
    {
        Task<List<T>> OpenAsync(string fileName);
        Task SaveAsync(string fileName, List<T> objects);
    }
}
