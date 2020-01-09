using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FordParser.Model.Raw
{
    public class UnifiedRawFordPage : IRawFordPage
    {

        public UnifiedRawFordPage(string[] allLines)
        {
            Lines = allLines;
        }

        public string[] Lines { get; }
    }
}
