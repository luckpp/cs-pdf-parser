using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace FordParser.Model.Raw
{
    public class RawFordPage : IRawFordPage
    {
        public RawFordPage(int number, string text)
        {
            Number = number;
            Lines = Regex.Split(text, "\r\n|\r|\n").Select(line => line.Trim()).ToArray();
        }

        public int Number { get; }

        public string[] Lines { get; }
    }
}
