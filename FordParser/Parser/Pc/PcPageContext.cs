using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FordParser.Model.Business.Pc;
using FordParser.Model.Business.Po;
using FordParser.Model.Raw;
using FordParser.Parser.Pc.State;
using FordParser.Parser.Po.State;

namespace FordParser.Parser.Pc
{
    internal class PcPageContext : IPcPageContext
    {
        private readonly string[] _lines;
        private int _index;

        public PcPageContext(IRawFordPage rawFordPage)
        {
            _lines = rawFordPage.Lines.ToArray();
            _index = 0;

            State = new PcStateTop(this);
        }

        public PcPriceChange PriceChange { get; set; }

        public IParserPageState State { get; set; }

        public string Line => _index < _lines.Length ? _lines[_index] : null;

        public void Process()
        {
            State.Process();
        }

        public void Next()
        {
            _index++;
        }
    }
}
