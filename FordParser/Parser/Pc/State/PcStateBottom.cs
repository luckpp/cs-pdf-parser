using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FordParser.Model.Business.Pc;

namespace FordParser.Parser.Pc.State
{
    internal class PcStateBottom : IParserPageState
    {
        private readonly IPcPageContext _context;

        public PcStateBottom(IPcPageContext context)
        {
            _context = context;
        }

        public void Process()
        {
            PcPriceChange priceChange = _context.PriceChange;
            var line = _context.Line;

            // TODO : implement if needed
        }
    }
}
