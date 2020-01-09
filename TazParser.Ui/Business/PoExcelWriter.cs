using System;
using System.Drawing;
using System.Globalization;
using System.IO;
using FordParser.Constants;
using FordParser.Model;
using FordParser.Model.Business;
using FordParser.Model.Business.Pc;
using FordParser.Model.Business.Po;
using OfficeOpenXml;
using OfficeOpenXml.Style;

namespace TazParser.Ui.Business
{
    public class PoExcelWriter
    {
        private readonly string _filePath;
        private readonly FordDocument[] _documents;
        private ExcelWorksheet _worksheetPo;
        private ExcelWorksheet _worksheetPc;
        private int _indexPo;
        private int _indexPc;

        public PoExcelWriter(string filePath, FordDocument[] documents)
        {
            _filePath  = filePath;
            _documents = documents;
            _indexPo = 1;
            _indexPc = 1;
        }

        //private static void 
        public void Write()
        {
            if (File.Exists(_filePath))
            {
                File.Delete(_filePath);
            }

            var fileInfo = new FileInfo(_filePath);

            var excel = new ExcelPackage(fileInfo);

            _worksheetPo = excel.Workbook.Worksheets.Add("Purchase Order");
            _worksheetPo.View.ShowGridLines = true;

            _worksheetPc = excel.Workbook.Worksheets.Add("Price Change");
            _worksheetPc.View.ShowGridLines = true;

            AddPoHeader();
            AddPcHeader();

            AddContent();

            _worksheetPo.Cells.AutoFitColumns();
            _worksheetPc.Cells.AutoFitColumns();

            excel.Save();
        }

        private void AddContent()
        {
            foreach (var document in _documents)
            {
                var documentName = document.Name;

                foreach (var page in document.Pages)
                {
                    if (page.PurchaseOrder != null)
                    {
                        AddPoContent(documentName, page.PageNumber, page.PurchaseOrder);
                    }
                    else if (page.PriceChange != null)
                    {
                        AddPcContent(documentName, page.PageNumber, page.PriceChange);
                    }
                }
            }
        }

        private void AddPcContent(string documentName, int pageNumber, PcPriceChange priceChange)
        {
            var curency       = priceChange.EconomicalInfo.Currency;
            var effectiveDate = priceChange.EconomicalInfo.EffectiveDate;
            var materials     = priceChange.Materials;

            if (materials != null)
            {
                foreach (var material in materials)
                {
                    AddContent(_worksheetPc, 'A', _indexPc, material.PartNumber);
                    AddContent(_worksheetPc, 'B', _indexPc, curency);
                    AddContent(_worksheetPc, 'C', _indexPc, material.InvoicePrice);
                    AddContent(_worksheetPc, 'D', _indexPc, material.PreviousInvoicePrice);
                    AddContent(_worksheetPc, 'E', _indexPc, effectiveDate);
                    AddContent(_worksheetPc, 'F', _indexPc, documentName, true);
                    AddContent(_worksheetPc, 'G', _indexPc, pageNumber, true);

                    _indexPc++;
                }
            }
        }

        private void AddPoContent(string documentName, int pageNumber, PoPurchaseOrder purchaseOrder)
        {
            if (purchaseOrder == null) return;

            var partNumber           = purchaseOrder.EconomicalInfo.PartNumber;
            var supersededPartNumber = purchaseOrder.EconomicalInfo.SupersededPartNumber;
            var currency             = purchaseOrder.EconomicalInfo.Currency;
            var materials            = purchaseOrder.Materials;

            if (materials != null)
            {
                foreach (var material in materials)
                {
                    var effectiveDate = GetDate(material.EffectiveDate);
                    var price         = GetDecimal(material.Price);
                    var plantCodeName = material.PlantCodeName;

                    AddContent(_worksheetPo, 'a', _indexPo, plantCodeName);
                    AddContent(_worksheetPo, 'B', _indexPo, partNumber);
                    AddContent(_worksheetPo, 'C', _indexPo, supersededPartNumber);
                    AddContent(_worksheetPo, 'D', _indexPo, currency);
                    AddContent(_worksheetPo, 'E', _indexPo, price);
                    AddContent(_worksheetPo, 'F', _indexPo, effectiveDate);
                    AddContent(_worksheetPo, 'G', _indexPo, documentName, true);
                    AddContent(_worksheetPo, 'H', _indexPo, pageNumber, true);

                    _indexPo++;
                }
            }
        }

        private void AddPoHeader()
        {
            
            AddHeader(_worksheetPo, 'A', _indexPo, PoConstants.PlantCodeName);
            AddHeader(_worksheetPo, 'B', _indexPo, PoConstants.PartNumber);
            AddHeader(_worksheetPo, 'C', _indexPo, PoConstants.SupersededPartNumber);
            AddHeader(_worksheetPo, 'D', _indexPo, PoConstants.Currency);
            AddHeader(_worksheetPo, 'E', _indexPo, PoConstants.Price);
            AddHeader(_worksheetPo, 'F', _indexPo, PoConstants.EffectiveDate);
            AddHeader(_worksheetPo, 'G', _indexPo, "File Name", true);
            AddHeader(_worksheetPo, 'H', _indexPo, "Page Number", true);

            _indexPo++;
        }

        private void AddPcHeader()
        {
            AddHeader(_worksheetPc, 'A', _indexPc, PcConstants.PartNumber);
            AddHeader(_worksheetPc, 'B', _indexPc, PcConstants.Currency);
            AddHeader(_worksheetPc, 'C', _indexPc, PcConstants.InvoicePrice);
            AddHeader(_worksheetPc, 'D', _indexPc, PcConstants.PreviousInvoicePrice);
            AddHeader(_worksheetPc, 'E', _indexPc, PcConstants.EffectiveDate);
            AddHeader(_worksheetPc, 'F', _indexPc, "File Name", true);
            AddHeader(_worksheetPc, 'G', _indexPc, "Page Number", true);

            _indexPc++;
        }

        private static void AddHeader(ExcelWorksheet worksheet, char column, int row, string value, bool isMisc = false)
        {
            var cellIndex = $"{column}{row}";
            var cell = worksheet.Cells[cellIndex];

            cell.Value = value;
            cell.Style.Fill.PatternType = ExcelFillStyle.Solid;
            cell.Style.Fill.BackgroundColor.SetColor(isMisc ? Color.Wheat : Color.DarkOrange);


            cell.Style.Border.Top.Style = ExcelBorderStyle.Thin;
            cell.Style.Border.Left.Style = ExcelBorderStyle.Thin;
            cell.Style.Border.Right.Style = ExcelBorderStyle.Thin;
            cell.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
        }

        private static void AddContent(ExcelWorksheet worksheet, char column, int row, object value, bool isMisc = false)
        {
            var cellIndex = $"{column}{row}";
            var cell = worksheet.Cells[cellIndex];

            cell.Value = value;

            cell.Style.Border.Top.Style = ExcelBorderStyle.Thin;
            cell.Style.Border.Left.Style = ExcelBorderStyle.Thin;
            cell.Style.Border.Right.Style = ExcelBorderStyle.Thin;
            cell.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

            if (isMisc)
            {
                cell.Style.Font.Color.SetColor(Color.DimGray);
            }
        }




        private decimal GetDecimal(string value)
        {
            var result = decimal.Parse(value);

            return result;
        }

        private string GetDate(string value)
        {
            var tokens = value.Split('-');

            var year = int.Parse(tokens[0]);
            var month = int.Parse(tokens[1]);
            var day = int.Parse(tokens[2]);

            var dateTime = new DateTime(year, month, day);

            return dateTime.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
        }
    }
}
