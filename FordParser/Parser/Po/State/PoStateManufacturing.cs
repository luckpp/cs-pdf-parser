using System.Text;
using FordParser.Constants;
using FordParser.Model.Business.Po;

namespace FordParser.Parser.Po.State
{
    internal class PoStateManufacturing : IParserPageState
    {
        private readonly IPoPageContext _context;

        public PoStateManufacturing(IPoPageContext context)
        {
            _context = context;
        }

        public void Process()
        {
            var purchase = _context.PurchaseOrder;
            var line = _context.Line;

            if (line.StartsWith(PoConstants.Manufacturing))
            {
                purchase.Manufacturing = new PoManufacturing();

                _context.Next();
                line = _context.Line;

                var data = new StringBuilder();
                while (line != null && !line.StartsWith(PoConstants.BuyerRepresentative))
                {
                    data.AppendLine(line);
                    _context.Next();
                    line = _context.Line;
                }

                purchase.Manufacturing.Data = data.ToString();
            }

            _context.State = new PoStateEconomicalInfo(_context);
            _context.Process();
        }
    }
}
