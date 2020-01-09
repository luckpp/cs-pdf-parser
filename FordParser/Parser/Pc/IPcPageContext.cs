using FordParser.Model.Business.Pc;

namespace FordParser.Parser.Pc
{
    interface IPcPageContext
    {
        PcPriceChange PriceChange { get; set; }

        IParserPageState State { get; set; }

        string Line { get; }

        void Process();

        void Next();
    }
}
