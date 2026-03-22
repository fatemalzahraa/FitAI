using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;

namespace AvciFidancilik.WebUI.Utils
{
    public class FileHelper
    {
        public static async Task<string> FileLoaderAsync(IFormFile formfile, string filePath = "/Img/")
        {
            if (formfile == null || formfile.Length == 0)
                throw new ArgumentException("Dosya boş olamaz.");

            // Klasörü oluştur
            string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", filePath.TrimStart('/'));

            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            // Benzersiz dosya adı oluştur
            string uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(formfile.FileName);
            string fileSavePath = Path.Combine(uploadsFolder, uniqueFileName);

            // Dosyayı kaydet
            using (var fileStream = new FileStream(fileSavePath, FileMode.Create))
            {
                await formfile.CopyToAsync(fileStream);
            }

            return uniqueFileName;
        }

        public static bool FileRemover(string fileName, string filePath = "/Img/")
        {
            if (string.IsNullOrEmpty(fileName))
                return false;

            string fileFullPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", filePath.TrimStart('/'), fileName);

            if (File.Exists(fileFullPath))
            {
                try
                {
                    File.Delete(fileFullPath);
                    return true;
                }
                catch
                {
                    return false;
                }
            }

            return false;
        }
    }
}