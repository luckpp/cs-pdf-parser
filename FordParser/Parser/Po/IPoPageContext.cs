using FordParser.Model.Business.Po;

namespace FordParser.Parser.Po
{
    interface IPoPageContext
    {
        PoPurchaseOrder PurchaseOrder { get; set; }

        IParserPageState State { get; set; }

        string Line { get; }

        void Process();

        void Next();
    }
}
