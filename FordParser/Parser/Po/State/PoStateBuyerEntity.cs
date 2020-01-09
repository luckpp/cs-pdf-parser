using System;
using FordParser.Constants;

namespace FordParser.Parser.Po.State
{
    internal class PoStateBuyerEntity : IParserPageState
    {
        private readonly IPoPageContext _context;

        public PoStateBuyerEntity(IPoPageContext context)
        {
            _context = context;
        }

        public void Process()
        {
            var purchase = _context.PurchaseOrder;

            var line = _context.Line;
            if (line.StartsWith(PoConstants.BuyerEntity))
            {
                _context.Next();
            }
            else
            {
                throw new Exception($"[BuyerEntityState] Unknown format: {line}");
            }

            line = _context.Line;
            if (line.StartsWith(PoConstants.PaymentTerms))
            {
                _context.Next();
            }

            line = _context.Line;
            while (line != null && !line.StartsWith(PoConstants.HeaderNote))
            {
                _context.Next();
                line = _context.Line;
            }

            if (line != null && line.StartsWith(PoConstants.HeaderNote))
            {
                _context.State = new PoStateHeaderNote(_context);
                _context.Process();
            }
            else
            {
                throw new Exception($"[BuyerEntityState] Unknown format: {line}");
            }
            
        }
    }
}
