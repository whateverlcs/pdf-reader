using Caliburn.Micro;
using Microsoft.WindowsAPICodePack.Dialogs;
using PDFReader.Logging;
using PDFReader.Models;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;

namespace PDFReader.ViewModels
{
    public class IndexViewModel : Screen
    {
        #region Bindings

        private ObservableCollection<PDF> _pdfFiles = new ObservableCollection<PDF>();

        public ObservableCollection<PDF> PdfFiles
        {
            get => _pdfFiles;
            set
            {
                _pdfFiles = value;
                NotifyOfPropertyChange(() => PdfFiles);
            }
        }

        private PDF _selectedPdf;

        public PDF SelectedPdf
        {
            get => _selectedPdf;
            set
            {
                if (Equals(value, _selectedPdf)) return;
                _selectedPdf = value;
                NotifyOfPropertyChange(() => SelectedPdf);

                if (_selectedPdf != null)
                {
                    SelectedPdfUri = new Uri(_selectedPdf.FullPath);
                }
                else
                {
                    SelectedPdfUri = new Uri("about:blank");
                }
            }
        }

        private Uri _selectedPdfUri = new Uri("about:blank");

        public Uri SelectedPdfUri
        {
            get => _selectedPdfUri;
            set
            {
                _selectedPdfUri = value ?? new Uri("about:blank");
                NotifyOfPropertyChange(() => SelectedPdfUri);
            }
        }

        private bool _isSidebarVisible = true;

        public bool IsSidebarVisible
        {
            get => _isSidebarVisible;
            set
            {
                _isSidebarVisible = value;
                NotifyOfPropertyChange(() => IsSidebarVisible);
            }
        }

        private bool _isTitleItemsVisible;

        public bool IsTitleItemsVisible
        {
            get => _isTitleItemsVisible;
            set
            {
                _isTitleItemsVisible = value;
                NotifyOfPropertyChange(() => IsTitleItemsVisible);
            }
        }

        private bool _isInitialState = true;

        public bool IsInitialState
        {
            get => _isInitialState;
            set
            {
                _isInitialState = value;
                NotifyOfPropertyChange(() => IsInitialState);
            }
        }

        private bool _isNotFound = false;

        public bool IsNotFound
        {
            get => _isNotFound;
            set
            {
                _isNotFound = value;
                NotifyOfPropertyChange(() => IsNotFound);
            }
        }

        private string _emptyStateText = "Select a folder to load all PDF's founded";

        public string EmptyStateText
        {
            get => _emptyStateText;
            set
            {
                _emptyStateText = value;
                NotifyOfPropertyChange(() => EmptyStateText);
            }
        }

        #endregion Bindings

        private readonly ILoggingService _logger;

        public IndexViewModel(ILoggingService logger)
        {
            _logger = logger;
        }

        public void MarkAsRead()
        {
            var pathMainFolder = SelectFolderToSaveReadFiles();

            var nameFolder = Path.GetFileName(BackDirectories(SelectedPdf.FullPath, 2));

            var nameToSave = $"{nameFolder} - {SelectedPdf.FileName}";

            var pathTxt = Path.Combine(pathMainFolder, "assistidos.txt");

            if (!File.Exists(pathTxt))
            {
                using (var stream = File.Create(pathTxt)) { }
            }

            File.AppendAllTextAsync(pathTxt, nameToSave + System.Environment.NewLine);

            MessageBox.Show("Marcado como visto com sucesso", "Sucesso", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        public static string BackDirectories(string path, int level)
        {
            if (level <= 0) return path;

            string result = path;
            for (int i = 0; i < level; i++)
            {
                var parent = Directory.GetParent(result);
                if (parent == null) break;
                result = parent.FullName;
            }

            return result;
        }

        public string SelectFolderToSaveReadFiles()
        {
            var dlg = new CommonOpenFileDialog
            {
                IsFolderPicker = true,
                Title = "Selecione a pasta que deseja salvar os PDFs lidos",
                Multiselect = false,
                EnsurePathExists = true
            };

            if (dlg.ShowDialog() == CommonFileDialogResult.Ok)
            {
                return dlg.FileName;
            }

            return string.Empty;
        }

        public void SelectFolder()
        {
            var dlg = new CommonOpenFileDialog
            {
                IsFolderPicker = true,
                Title = "Selecione a pasta com PDFs",
                Multiselect = false,
                EnsurePathExists = true
            };

            if (dlg.ShowDialog() == CommonFileDialogResult.Ok)
            {
                LoadPdfsFromFolder(dlg.FileName);
            }
        }

        private void LoadPdfsFromFolder(string folder)
        {
            PdfFiles.Clear();
            try
            {
                var files = Directory.GetFiles(folder, "*.pdf", SearchOption.AllDirectories)
                                     .Select(f => new PDF
                                     {
                                         FullPath = f,
                                         FileName = Path.GetFileName(f)
                                     })
                                     .DistinctBy(f => f.FileName)
                                     .OrderBy(f =>
                                     {
                                         var digits = new string(f.FileName.Where(char.IsDigit).ToArray());
                                         return int.TryParse(digits, out var number) ? number : 0;
                                     })
                                     .ToList();

                foreach (var f in files)
                {
                    PdfFiles.Add(new PDF
                    {
                        FileName = Path.GetFileNameWithoutExtension(f.FileName),
                        FullPath = f.FullPath
                    });
                }

                if (PdfFiles.Count > 0)
                {
                    SelectedPdf = PdfFiles.First();
                    IsNotFound = false;
                    IsTitleItemsVisible = true;
                    EmptyStateText = "";

                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        var shellView = Application.Current.MainWindow as PDFReader.Views.ShellView;
                        shellView?.MaximizarJanela();
                    });
                }
                else
                {
                    SelectedPdf = null;
                    IsNotFound = true;
                    IsTitleItemsVisible = false;
                    EmptyStateText = "No PDFs found in the selected folder";
                }

                IsInitialState = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred while searching/loading the files. Please try again, if the error persists, contact support.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                _logger.LogError(ex, "LoadPdfsFromFolder(string folder)");
            }
        }

        public void ToggleSidebar()
        {
            IsSidebarVisible = !IsSidebarVisible;
            IsTitleItemsVisible = IsSidebarVisible && PdfFiles.Count > 0;
        }
    }
}