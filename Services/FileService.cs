using System;
using System.IO;

namespace SolidConvert_Pro.Services
{
    public class FileService
    {
        public bool ValidateFile(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
                return false;

            if (!File.Exists(filePath))
                return false;

            string extension = Path.GetExtension(filePath);
            return extension.Equals(".slddrw", StringComparison.OrdinalIgnoreCase);
        }

        public bool ValidateFolder(string folderPath)
        {
            if (string.IsNullOrWhiteSpace(folderPath))
                return false;

            try
            {
                return Directory.Exists(folderPath);
            }
            catch
            {
                return false;
            }
        }
    }
}


