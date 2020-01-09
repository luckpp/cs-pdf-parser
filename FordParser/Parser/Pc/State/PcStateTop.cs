using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FordParser.Constants;
using FordParser.Model.Business.Pc;
using FordParser.Model.Business.Po;

namespace FordParser.Parser.Pc.State
{
    internal class PcStateTop : IParserPageState
    {
        private readonly IPcPageContext _context;

        public PcStateTop(IPcPageContext context)
        {
            _context = context;
        }

        public void Process()
        {
            PcPriceChange priceChange = null;
            var line = _context.Line;

            if (line.StartsWith(PcConstants.PriceChange))
            {
                priceChange = new PcPriceChange();
                _context.PriceChange = priceChange;
                _context.Next();
            }
            else
            {
                _context.PriceChange = null;
                return;
            }

            while (line != null && !line.StartsWith(PcConstants.TimestampCet))
            {
                _context.Next();
                line = _context.Line;
            }

            _context.Next();
            var currency = _context.Line;

            _context.Next();
            var effectiveDate = _context.Line;

            priceChange.EconomicalInfo = new PcEconomicalInfo
            {
                Currency = currency,
                EffectiveDate = effectiveDate
            };

            line = _context.Line;
            while (line != null && !line.StartsWith(PcConstants.PartNumber))
            {
                _context.Next();
                line = _context.Line;
            }

            if (line != null && line.StartsWith(PcConstants.PartNumber))
            {
                _context.State = new PcStateMaterial(_context);
                _context.Process();
            }
            else
            {
                throw new Exception($"[PcStateTop] Unknown format: {line}");
            }
            
        }
    }
}
