using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using FordParser.Parser;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using Org.BouncyCastle.Crypto.Generators;

namespace FordParser
{
    class Program
    {
        //private const string FilePath = @"C:\Projects\VS\Pdf2Excel\Ford\FordParser\Input\PO JM2T-10849-ACA.pdf";
        private const string FilePath = @"C:\Projects\VS\Pdf2Excel\Ford\FordParser\Input\Preturi Ford 26.04.pdf";

        static void Main(string[] args)
        {
            var document = Parser.FordParser.GetFordDocument(FilePath);
        }
    }
}
