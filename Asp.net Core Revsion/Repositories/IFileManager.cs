using Microsoft.AspNetCore.Http;
using System.IO;
using System.Threading.Tasks;

namespace Asp.netCoreRevsion.Repositories
{
    public interface IFileManager
    {
        Task<string> ProcessImage(IFormFile file);
        FileStream ImageStream(string image);
    }
}