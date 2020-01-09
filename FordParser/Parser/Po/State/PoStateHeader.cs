using System;
using FordParser.Constants;
using FordParser.Model.Business.Po;

namespace FordParser.Parser.Po.State
{
    internal class PoStateHeader : IParserPageState
    {
        private readonly IPoPageContext _context;

        public PoStateHeader(IPoPageContext context)
        {
            _context = context;
        }

        public void Process()
        {
            PoPurchaseOrder purchaseOrder = null;
            var line = _context.Line;

            if (line.StartsWith(PoConstants.PurchaseOrder))
            {
                purchaseOrder = new PoPurchaseOrder();
                _context.PurchaseOrder = purchaseOrder;
                _context.Next();
            }
            else
            {
                _context.PurchaseOrder = null;
                return;
            }

            line = _context.Line;
            if (line.StartsWith(PoConstants.DateOfOrder))
            {
                purchaseOrder.DateOfOrder = line;
                _context.Next();
            }

            line = _context.Line;
            if (line.StartsWith(PoConstants.AtpNumber))
            {
                _context.Next();
                line = _context.Line;
                purchaseOrder.AtpNumber = line;
                _context.Next();
            }

            line = _context.Line;
            if (line.StartsWith(PoConstants.BuyerEntity))
            {
                _context.State = new PoStateBuyerEntity(_context);
                _context.Process();
            }
            else
            {
                throw new Exception($"[HeaderState] Unknown format: {line}");
            }
        }
    }
}
