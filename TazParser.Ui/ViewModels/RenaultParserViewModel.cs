using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using TazParser.Ui.Business;
using MessageBox = System.Windows.MessageBox;
using OpenFileDialog = Microsoft.Win32.OpenFileDialog;

namespace TazParser.Ui.ViewModels
{
    internal class RenaultParserViewModel : ViewModelBase
    {
        public RenaultParserViewModel()
        {
            PdfFiles = new ObservableCollection<string>();

            OpenFilesCommand = new RelayCommand(
                OpenFilesCommand_Execute);

            ExtractImagesCommand = new RelayCommand(
                ExtractImagesCommand_Execute,
                ExtractImagesCommand_CanExecute);

            ClearFilesCommand = new RelayCommand(
                ClearFilesCommand_Execute,
                ClearFilesCommand_CanExecute);

            PdfFileSelectionCommand = new RelayCommand<IList>(
                PdfFileSelectionCommand_Execute);

            SetBusy(false);
        }

        #region -------------------------------------------------- Properties --------------------------------------------------

        private ObservableCollection<string> _pdfFiles;
        public ObservableCollection<string> PdfFiles
        {
            get { return _pdfFiles; }
            set
            {
                _pdfFiles = value;
                RaisePropertyChanged();
                ClearFilesCommand?.RaiseCanExecuteChanged();
                ExtractImagesCommand?.RaiseCanExecuteChanged();
            }
        }

        private string[] _selectedPdfFiles;
        public string[] SelectedPdfFiles
        {
            get { return _selectedPdfFiles; }
            set
            {
                _selectedPdfFiles = value;
                ClearFilesCommand.RaiseCanExecuteChanged();
            }
        }

        private bool _isIdle;
        public bool IsIdle
        {
            get { return _isIdle; }
            set
            {
                _isIdle = value;
                RaisePropertyChanged();
            }
        }

        private bool _isBusy;
        public bool IsBusy
        {
            get { return _isBusy; }
            set
            {
                _isBusy = value;
                RaisePropertyChanged();
            }
        }

        #endregion

        #region -------------------------------------------------- Commands --------------------------------------------------

        #region ----- OpenFilesCommand -----

        public RelayCommand OpenFilesCommand { get; set; }

        private void OpenFilesCommand_Execute()
        {
            var dialog = new OpenFileDialog
            {
                Multiselect = true,
                Filter = "PDF files (*.pdf) | *.pdf"
            };

            if (dialog.ShowDialog() == true)
            {
                PdfFiles.AddRange(dialog.FileNames);
            }

            ExtractImagesCommand?.RaiseCanExecuteChanged();
            ClearFilesCommand?.RaiseCanExecuteChanged();
        }

        #endregion

        #region ----- ExtractImagesCommand -----

        public RelayCommand ExtractImagesCommand { get; set; }

        private void ExtractImagesCommand_Execute()
        {
            try
            {
                SetBusy(true);
                var dialog = new FolderBrowserDialog();
                var result = dialog.ShowDialog();

                if (result == DialogResult.OK)
                {
                    var pdfFilePaths   = PdfFiles.ToArray();
                    var imageExtractor = new ImageExtractor(pdfFilePaths, dialog.SelectedPath);

                    imageExtractor.Extract();

                    System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                    {
                        FileName        = dialog.SelectedPath,
                        UseShellExecute = true,
                        Verb            = "open"
                    });
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
                Console.WriteLine(exception);
            }
            finally
            {
                SetBusy(false);
            }
        }

        private void SetBusy(bool value)
        {
            IsBusy = value;
            IsIdle = !value;
        }

        private bool ExtractImagesCommand_CanExecute()
        {
            return PdfFiles.Count > 0;
        }

        #endregion

        #region ----- ClearFilesCommand -----

        public RelayCommand ClearFilesCommand { get; set; }

        private void ClearFilesCommand_Execute()
        {
            var files = PdfFiles.Where(
                file => SelectedPdfFiles.All(f => f != file));

            PdfFiles = new ObservableCollection<string>(files);
        }

        private bool ClearFilesCommand_CanExecute()
        {
            return SelectedPdfFiles != null && SelectedPdfFiles.Length > 0;
        }

        #endregion

        #region ----- PdfFileSelectionCommand -----

        public RelayCommand<IList> PdfFileSelectionCommand { get; set; }

        private void PdfFileSelectionCommand_Execute(IList selectedItems)
        {
            SelectedPdfFiles = selectedItems
                .OfType<string>()
                .ToArray();
        }

        #endregion

        #endregion
    }
}
