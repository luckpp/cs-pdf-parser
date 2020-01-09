using System.Text;
using FordParser.Constants;
using FordParser.Model.Business.Po;

namespace FordParser.Parser.Po.State
{
    internal class PoStateSupplier : IParserPageState
    {
        private readonly IPoPageContext _context;

        public PoStateSupplier(IPoPageContext context)
        {
            _context = context;
        }

        public void Process()
        {
            var purchase = _context.PurchaseOrder;
            var line = _context.Line;

            if (line.StartsWith(PoConstants.Supplier))
            {
                purchase.Supplier = new PoSupplier();

                _context.Next();
                line = _context.Line;
                purchase.Supplier.Name = line;

                _context.Next();
                line = _context.Line;

                if (line.StartsWith(PoConstants.PoAddress))
                {
                    _context.Next();
                    line = _context.Line;

                    var poAddress = new StringBuilder();
                    while (line != null && !line.StartsWith(PoConstants.Manufacturing))
                    {
                        poAddress.AppendLine(line);
                        _context.Next();
                        line = _context.Line;
                    }
                    purchase.Supplier.PoAddress = poAddress.ToString();
                }
            }

            _context.State = new PoStateManufacturing(_context);
            _context.Process();
        }
    }
}
