using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Asp.netCoreRevsion.Repositories
{
    public class FileManager : IFileManager
    {
        private readonly IHostingEnvironment _environment;

        public FileManager(IHostingEnvironment environment)
        {
            _environment = environment;
        }
        public async Task<string> ProcessImage(IFormFile file)
        {
            var directory = Path.Combine(_environment.WebRootPath, "Images");
            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);

            var extension = new FileInfo(file.FileName).Extension;
            var fileName = $"img_{DateTime.Now:dd.MM.yyyy HH.mm.ss}{extension}";

            using (var fileStream = new FileStream(Path.Combine(directory, fileName), FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }

            return fileName;
        }

        public FileStream ImageStream(string image)
        {
            var directory = Path.Combine(_environment.WebRootPath, "Images");
            return new FileStream(Path.Combine(directory, image), FileMode.Open, FileAccess.Read);
        }
    }
}
