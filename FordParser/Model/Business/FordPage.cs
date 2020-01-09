using FordParser.Model.Business.Pc;
using FordParser.Model.Business.Po;

namespace FordParser.Model.Business
{
    public class FordPage
    {
        public int PageNumber { get; set; }

        public PoPurchaseOrder PurchaseOrder { get; set; }

        public PcPriceChange PriceChange { get; set; }
    }
}
