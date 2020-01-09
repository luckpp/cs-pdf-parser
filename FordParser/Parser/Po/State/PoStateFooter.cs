using FordParser.Constants;

namespace FordParser.Parser.Po.State
{
    internal class PoStateFooter : IParserPageState
    {
        private readonly IPoPageContext _context;

        public PoStateFooter(IPoPageContext context)
        {
            _context = context;
        }

        public void Process()
        {
            var purchase = _context.PurchaseOrder;
            var line = _context.Line;

            if (line == PoConstants.Remarks)
            {
                _context.Next();
                line = _context.Line;

                while (line != null)
                {
                    purchase.Misc.Add(line);
                    _context.Next();
                    line = _context.Line;
                }
            }
        }
    }
}
