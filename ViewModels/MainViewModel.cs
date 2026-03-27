using SolidConvert_Pro.Commands;
using SolidConvert_Pro.Models;
using SolidConvert_Pro.Services;
using SolidWorks.Interop.sldworks;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace SolidConvert_Pro.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        private readonly SolidWorksService _solidWorksService;
        private readonly FileService _fileService;
        private readonly DialogService _dialogService;
        private CancellationTokenSource _cancellationTokenSource;

        private ObservableCollection<FileItem> _selectedFiles;
        private string _outputFolder;
        private bool _exportToPdf;
        private bool _exportToDwg;
        private string _statusMessage;
        private string _currentFile;
        private int _progress;
        private bool _isConverting;
        private int _filesProcessed;
        private int _totalFiles;

        public MainViewModel()
        {
            _solidWorksService = new SolidWorksService();
            _fileService = new FileService();
            _dialogService = new DialogService();

            _selectedFiles = new ObservableCollection<FileItem>();
            _outputFolder = Properties.Settings.Default.LastOutputFolder ?? string.Empty;
            _exportToPdf = true;
            _exportToDwg = false;
            _statusMessage = "Ready";
            _progress = 0;

            InitializeCommands();
        }

        public ObservableCollection<FileItem> SelectedFiles
        {
            get => _selectedFiles;
            set => SetProperty(ref _selectedFiles, value);
        }

        public string OutputFolder
        {
            get => _outputFolder;
            set
            {
                SetProperty(ref _outputFolder, value);
                if (!string.IsNullOrEmpty(value))
                {
                    Properties.Settings.Default.LastOutputFolder = value;
                    Properties.Settings.Default.Save();
                }
            }
        }

        public bool ExportToPdf
        {
            get => _exportToPdf;
            set => SetProperty(ref _exportToPdf, value);
        }

        public bool ExportToDwg
        {
            get => _exportToDwg;
            set => SetProperty(ref _exportToDwg, value);
        }

        public string StatusMessage
        {
            get => _statusMessage;
            set => SetProperty(ref _statusMessage, value);
        }

        public string CurrentFile
        {
            get => _currentFile;
            set => SetProperty(ref _currentFile, value);
        }

        public int Progress
        {
            get => _progress;
            set => SetProperty(ref _progress, value);
        }

        public bool IsConverting
        {
            get => _isConverting;
            set
            {
                SetProperty(ref _isConverting, value);
                System.Windows.Input.CommandManager.InvalidateRequerySuggested();
            }
        }

        public int FilesProcessed
        {
            get => _filesProcessed;
            set => SetProperty(ref _filesProcessed, value);
        }

        public int TotalFiles
        {
            get => _totalFiles;
            set => SetProperty(ref _totalFiles, value);
        }

        // Commands
        public RelayCommand AddFilesCommand { get; private set; }
        public RelayCommand RemoveSelectedFilesCommand { get; private set; }
        public RelayCommand ClearListCommand { get; private set; }
        public RelayCommand SelectOutputFolderCommand { get; private set; }
        public RelayCommand ExportCommand { get; private set; }
        public RelayCommand CancelCommand { get; private set; }

        private void InitializeCommands()
        {
            AddFilesCommand = new RelayCommand(ExecuteAddFiles, _ => !IsConverting);
            RemoveSelectedFilesCommand = new RelayCommand(ExecuteRemoveSelectedFiles, _ => !IsConverting && SelectedFiles.Count > 0);
            ClearListCommand = new RelayCommand(ExecuteClearList, _ => !IsConverting && SelectedFiles.Count > 0);
            SelectOutputFolderCommand = new RelayCommand(ExecuteSelectOutputFolder, _ => !IsConverting);
            ExportCommand = new RelayCommand(ExecuteExport, CanExecuteExport);
            CancelCommand = new RelayCommand(ExecuteCancel, _ => IsConverting);
        }

        private bool CanExecuteExport(object parameter)
        {
            return !IsConverting &&
                   SelectedFiles.Count > 0 &&
                   !string.IsNullOrWhiteSpace(OutputFolder) &&
                   _fileService.ValidateFolder(OutputFolder) &&
                   (ExportToPdf || ExportToDwg);
        }

        private void ExecuteAddFiles(object parameter)
        {
            var files = _dialogService.SelectFiles();
            foreach (var file in files)
            {
                if (_fileService.ValidateFile(file) && !SelectedFiles.Any(f => f.FilePath.Equals(file, StringComparison.OrdinalIgnoreCase)))
                {
                    SelectedFiles.Add(new FileItem { FilePath = file, Status = FileStatus.Pending });
                }
            }
            UpdateFileCount();
        }

        public void AddFilesFromDrop(string[] files)
        {
            foreach (var file in files)
            {
                if (Path.GetExtension(file).Equals(".slddrw", StringComparison.OrdinalIgnoreCase))
                {
                    if (_fileService.ValidateFile(file) && !SelectedFiles.Any(f => f.FilePath.Equals(file, StringComparison.OrdinalIgnoreCase)))
                    {
                        SelectedFiles.Add(new FileItem { FilePath = file, Status = FileStatus.Pending });
                    }
                }
            }
            UpdateFileCount();
        }

        private void ExecuteRemoveSelectedFiles(object parameter)
        {
            if (parameter is System.Collections.IList selectedItems && selectedItems.Count > 0)
            {
                var itemsToRemove = selectedItems.Cast<FileItem>().ToList();
                foreach (var item in itemsToRemove)
                {
                    SelectedFiles.Remove(item);
                }
                UpdateFileCount();
            }
        }

        private void ExecuteClearList(object parameter)
        {
            SelectedFiles.Clear();
            StatusMessage = "Ready";
            CurrentFile = string.Empty;
            Progress = 0;
            FilesProcessed = 0;
            TotalFiles = 0;
        }

        private void ExecuteSelectOutputFolder(object parameter)
        {
            var folder = _dialogService.SelectFolder();
            if (!string.IsNullOrEmpty(folder))
            {
                OutputFolder = folder;
            }
        }

        private async void ExecuteExport(object parameter)
        {
            if (!ValidateExport())
                return;

            IsConverting = true;
            _cancellationTokenSource = new CancellationTokenSource();
            StatusMessage = "Starting Conversion...";
            Progress = 0;
            FilesProcessed = 0;
            TotalFiles = SelectedFiles.Count;

            try
            {
                StatusMessage = "Connecting to SolidWorks...";
                SldWorks swApp = await _solidWorksService.ConnectAsync();

                if (swApp == null)
                {
                    StatusMessage = "Error: Could not connect to SolidWorks";
                    IsConverting = false;
                    MessageBox.Show("Could not connect to SolidWorks. Please ensure SolidWorks is installed and try again.", "Connection Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                StatusMessage = "Connected to SolidWorks";
                await Task.Delay(500);

                int processed = 0;
                foreach (var fileItem in SelectedFiles)
                {
                    if (_cancellationTokenSource.Token.IsCancellationRequested)
                    {
                        StatusMessage = "Conversion cancelled";
                        break;
                    }

                    fileItem.Status = FileStatus.Processing;
                    CurrentFile = fileItem.FileName;
                    StatusMessage = $"Converting: {fileItem.FileName}";

                    try
                    {
                        var result = await _solidWorksService.ConvertDrawingAsync(swApp, fileItem.FilePath, OutputFolder, ExportToPdf, ExportToDwg);
                        
                        if (result.Success)
                        {
                            fileItem.Status = FileStatus.Completed;
                        }
                        else
                        {
                            fileItem.Status = FileStatus.Error;
                            fileItem.ErrorMessage = result.ErrorMessage;
                        }
                    }
                    catch (Exception ex)
                    {
                        fileItem.Status = FileStatus.Error;
                        fileItem.ErrorMessage = ex.Message;
                    }

                    processed++;
                    FilesProcessed = processed;
                    Progress = (int)((double)processed / TotalFiles * 100);
                }

                if (!_cancellationTokenSource.Token.IsCancellationRequested)
                {
                    StatusMessage = "Conversion completed";
                    CurrentFile = string.Empty;
                    MessageBox.Show($"Conversion completed successfully!\n{processed} of {TotalFiles} files processed.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                StatusMessage = $"Error: {ex.Message}";
                MessageBox.Show($"An error occurred during conversion: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                IsConverting = false;
            }
        }

        private void ExecuteCancel(object parameter)
        {
            _cancellationTokenSource?.Cancel();
            StatusMessage = "Cancelling...";
        }

        private bool ValidateExport()
        {
            if (string.IsNullOrWhiteSpace(OutputFolder))
            {
                StatusMessage = "Please select output folder";
                MessageBox.Show("Please select an output folder.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (!_fileService.ValidateFolder(OutputFolder))
            {
                StatusMessage = "Invalid output folder";
                MessageBox.Show("The selected output folder is not valid.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (SelectedFiles.Count == 0)
            {
                StatusMessage = "Please select files to convert";
                MessageBox.Show("Please select at least one file to convert.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (!ExportToPdf && !ExportToDwg)
            {
                StatusMessage = "Please select export format";
                MessageBox.Show("Please select at least one export format (PDF or DWG).", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            return true;
        }

        private void UpdateFileCount()
        {
            TotalFiles = SelectedFiles.Count;
            OnPropertyChanged(nameof(TotalFiles));
        }
    }
}

