using System;
using FordParser.Constants;

namespace FordParser.Parser.Po.State
{
    internal class PoStateHeaderNote : IParserPageState
    {
        private readonly IPoPageContext _context;

        public PoStateHeaderNote(IPoPageContext context)
        {
            _context = context;
        }

        public void Process()
        {
            var purchase = _context.PurchaseOrder;
            var line = _context.Line;

            if (!line.StartsWith(PoConstants.HeaderNote))
            {
                throw new Exception($"[HeaderNoteState] Unknown format: {line}");
            }

            while (line != null && !line.StartsWith(PoConstants.Supplier))
            {
                purchase.HeaderNotes.Add(line);
                _context.Next();
                line = _context.Line;
            }

            if (line != null && line.StartsWith(PoConstants.Supplier))
            {
                _context.State = new PoStateSupplier(_context);
                _context.Process();
            }
            else
            {
                throw new Exception($"[HeaderNoteState] Unknown format: {line}");
            }
        }
    }
}
