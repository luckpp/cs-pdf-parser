using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Windows;
using FordParser.Parser;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Ioc;
using Microsoft.Win32;
using Taz.Services.Interfaces;
using TazParser.Ui.Business;

namespace TazParser.Ui.ViewModels
{
    internal class FordParserViewModel : ViewModelBase
    {
        private readonly IEmailService _emailService;

        public FordParserViewModel()
        {
            PdfFiles = new ObservableCollection<string>();

            OpenFilesCommand = new RelayCommand(
                OpenFilesCommand_Execute);

            GenerateXlsxCommand = new RelayCommand(
                GenerateXlsxCommand_Execute,
                GenerateXlsxCommand_CanExecute);

            ClearFilesCommand = new RelayCommand(
                ClearFilesCommand_Execute,
                ClearFilesCommand_CanExecute);

            PdfFileSelectionCommand = new RelayCommand<IList>(
                PdfFileSelectionCommand_Execute);

            SetBusy(false);

            _emailService = SimpleIoc.Default.GetInstance<IEmailService>();
            _emailService.SendEmail("Hello!");
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
                GenerateXlsxCommand?.RaiseCanExecuteChanged();
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

            GenerateXlsxCommand?.RaiseCanExecuteChanged();
            ClearFilesCommand?.RaiseCanExecuteChanged();
        }

        #endregion

        #region ----- GenerateXlsxCommand -----

        public RelayCommand GenerateXlsxCommand { get; set; }

        private void GenerateXlsxCommand_Execute()
        {
            try
            {
                SetBusy(true);
                var saveFileDialog = new SaveFileDialog
                {
                    Filter = "XLSX Files (*.xlsx) | *.xlsx"
                };
                if (saveFileDialog.ShowDialog() == true)
                {
                    var fileName = saveFileDialog.FileName;
                    var documents = PdfFiles.Select(FordParser.Parser.FordParser.GetFordDocument).ToArray();

                    var writer = new PoExcelWriter(fileName, documents);
                    writer.Write();

                    System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo()
                    {
                        FileName = Path.GetDirectoryName(saveFileDialog.FileName),
                        UseShellExecute = true,
                        Verb = "open"
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

        private bool GenerateXlsxCommand_CanExecute()
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
