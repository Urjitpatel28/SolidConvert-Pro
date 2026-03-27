using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace SolidConvert_Pro.Models
{
    public enum FileStatus
    {
        Pending,
        Processing,
        Completed,
        Error
    }

    public class FileItem : INotifyPropertyChanged
    {
        private string _filePath;
        private FileStatus _status;
        private string _errorMessage;

        public string FilePath
        {
            get => _filePath;
            set
            {
                _filePath = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(FileName));
            }
        }

        public string FileName => System.IO.Path.GetFileName(_filePath);

        public FileStatus Status
        {
            get => _status;
            set
            {
                _status = value;
                OnPropertyChanged();
            }
        }

        public string ErrorMessage
        {
            get => _errorMessage;
            set
            {
                _errorMessage = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}


