using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace SolidConvert_Pro.Services
{
    public class DialogService
    {
        public List<string> SelectFiles()
        {
            var fileDialog = new OpenFileDialog
            {
                Title = "Please select files",
                Multiselect = true,
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
                Filter = "SolidWorks Drawing | *.slddrw"
            };

            if (fileDialog.ShowDialog() == DialogResult.OK)
            {
                return fileDialog.FileNames.ToList();
            }

            return new List<string>();
        }

        public string SelectFolder()
        {
            var folderDialog = new FolderBrowserDialog
            {
                Description = "Please select the desired folder"
            };

            if (folderDialog.ShowDialog() == DialogResult.OK)
            {
                return folderDialog.SelectedPath;
            }

            return null;
        }
    }
}


