using System.Linq;
using FordParser.Model.Business.Po;
using FordParser.Model.Raw;
using FordParser.Parser.Po.State;

namespace FordParser.Parser.Po
{
    internal class PoPageContext : IPoPageContext
    {
        private readonly string[] _lines;
        private int _index;

        public PoPageContext(IRawFordPage rawFordPage)
        {
            _lines = rawFordPage.Lines.ToArray();
            _index = 0;

            State = new PoStateHeader(this);
        }

        public PoPurchaseOrder PurchaseOrder { get; set; }

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
