using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImageParser;

namespace TazParser.Ui.Business
{
    internal class ImageExtractor
    {
        private readonly string[] _pdfFilePaths;
        private readonly string _destinationPath;

        public ImageExtractor(string[] pdfFilePaths, string destinationPath)
        {
            _pdfFilePaths    = pdfFilePaths;
            _destinationPath = destinationPath;
        }

        public void Extract()
        {
            foreach (var pdfFilePath in _pdfFilePaths)
            {
                var images = PdfImageParser.ExtractImages(pdfFilePath);
                var pdfFolder = PrepareFolder(_destinationPath, pdfFilePath);

                Util.WriteImageFiles(images, pdfFolder);
            }
        }

        private static string PrepareFolder(string destinationPath, string pdfFilePath)
        {
            var pdfFileName = Path.GetFileNameWithoutExtension(pdfFilePath);

            var pdfFolder = Path.Combine(destinationPath, pdfFileName);

            if (Directory.Exists(pdfFolder))
            {
                Util.ClearFolder(pdfFolder);
            }
            else
            {
                Util.CreateFolder(pdfFolder);
            }

            return pdfFolder;
        }


        
        
    }
}
