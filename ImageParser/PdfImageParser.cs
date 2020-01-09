using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;

namespace ImageParser
{
    public static class PdfImageParser
    {
        /// <summary>
        ///  Extract Image from PDF file and Store in Image Object
        /// </summary>
        /// <param name="pdfPath">Specify PDF Source Path</param>
        /// <returns>List</returns>
        public static List<Image> ExtractImages(string pdfPath)
        {
            var images    = new List<Image>();
            var rafObj    = new RandomAccessFileOrArray(pdfPath);
            var pdfReader = new PdfReader(rafObj, null);

            for (int i = 0; i < pdfReader.XrefSize; i++)
            {
                var pdfObject = pdfReader.GetPdfObject(i);

                if ((pdfObject != null) && pdfObject.IsStream())
                {
                    var pdfStream = (PdfStream)pdfObject;
                    var subtype = pdfStream.Get(PdfName.SUBTYPE);

                    if ((subtype != null) && subtype.ToString() == PdfName.IMAGE.ToString())
                    {
                        var pdfImageObj = new PdfImageObject((PRStream)pdfStream);
                        var image = pdfImageObj.GetDrawingImage();
                        images.Add(image);
                    }
                }
            }

            pdfReader.Close();

            return images;
        }
    }
}
