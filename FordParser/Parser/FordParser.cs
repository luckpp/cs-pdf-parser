using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using FordParser.Constants;
using FordParser.Model.Business;
using FordParser.Model.Business.Pc;
using FordParser.Model.Business.Po;
using FordParser.Model.Raw;
using FordParser.Parser.Pc;
using FordParser.Parser.Po;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using Path = System.IO.Path;

namespace FordParser.Parser
{
    public static class FordParser
    {
        #region -------------------------------------------------- Public --------------------------------------------------

        public static FordDocument GetFordDocument(string filePath)
        {
            var document = ReadRawDocument(filePath);

            var pages = new List<FordPage>();
            var purchaseOrder = default(PoPurchaseOrder);
            var priceChange = default(PcPriceChange);

            var unifiedPage = document.UnifiedPage;

            if (IsPurchaseOrderPage(unifiedPage))
            {
                purchaseOrder = ExtractPurchaseOrder(unifiedPage);
            }
            else if (IsPriceChangePage(unifiedPage))
            {
                priceChange = ExtractPriceChange(unifiedPage);
            }

            pages.Add(new FordPage
            {
                PageNumber = 1,
                PurchaseOrder = purchaseOrder,
                PriceChange = priceChange
            });

            return new FordDocument
            {
                Name  = Path.GetFileName(filePath),
                Pages = pages
            };
        }


        public static FordDocument GetFordDocumentMultiPage(string filePath)
        {
            var document = ReadRawDocument(filePath);

            var pages = new List<FordPage>();

            foreach (var fordPage in document.Pages)
            {
                var pageNumber = fordPage.Number;
                var purchaseOrder = default(PoPurchaseOrder);
                var priceChange = default(PcPriceChange);

                if (IsPurchaseOrderPage(fordPage))
                {
                    purchaseOrder = ExtractPurchaseOrder(fordPage);
                }
                else if (IsPriceChangePage(fordPage))
                {
                    priceChange = ExtractPriceChange(fordPage);
                }

                pages.Add(new FordPage
                {
                    PageNumber = pageNumber,
                    PurchaseOrder = purchaseOrder,
                    PriceChange = priceChange
                });
            }

            return new FordDocument
            {
                Name = Path.GetFileName(filePath),
                Pages = pages
            };
        }

        #endregion

        #region -------------------------------------------------- Private --------------------------------------------------

        #region ----- Identify -----

        private static bool IsPurchaseOrderPage(IRawFordPage fordPage)
        {
            var lines = fordPage.Lines;

            if (lines != null && lines.Any())
            {
                var firstLine = lines.First();
                return firstLine.StartsWith(PoConstants.PurchaseOrder);
            }

            return false;
        }

        private static bool IsPriceChangePage(IRawFordPage fordPage)
        {
            var lines = fordPage.Lines;

            if (lines != null && lines.Any())
            {
                var firstLine = lines.First();
                return firstLine.StartsWith(PcConstants.PriceChange);
            }

            return false;
        }

        #endregion

        #region ----- Extract -----

        private static PoPurchaseOrder ExtractPurchaseOrder(IRawFordPage fordPage)
        {
            var context = new PoPageContext(fordPage);
            context.Process();
            return context.PurchaseOrder;
        }

        private static PcPriceChange ExtractPriceChange(IRawFordPage fordPage)
        {
            var context = new PcPageContext(fordPage);
            context.Process();
            return context.PriceChange;
        }

        #endregion

        #region ----- Read Raw Document -----

        private static RawFordDocument ReadRawDocument(string filePath)
        {
            if (!File.Exists(filePath))
            {
                throw new ArgumentException($"Unavailable file {filePath}");
            }

            var fileName = Path.GetFileNameWithoutExtension(filePath);
            var pages = ReadRawPages(filePath);

            var document = new RawFordDocument(fileName, pages);

            return document;
        }

        private static IEnumerable<RawFordPage> ReadRawPages(string filePath)
        {
            var pdfReader = new PdfReader(filePath);

            for (var i = 0; i < pdfReader.NumberOfPages; i++)
            {
                var strategy = new FordExtractionCustomStrategy();
                var textFromPage = PdfTextExtractor.GetTextFromPage(pdfReader, i + 1, strategy);

                textFromPage = Encoding.UTF8.GetString(
                    Encoding.Convert(Encoding.Default, Encoding.UTF8, Encoding.Default.GetBytes(textFromPage))
                );

                var page = new RawFordPage(i + 1, textFromPage);

                yield return page;
            }
        }

        #endregion

        #endregion
    }
}
