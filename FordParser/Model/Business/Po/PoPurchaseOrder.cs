using System.Collections.Generic;

namespace FordParser.Model.Business.Po
{
    public class PoPurchaseOrder
    {
        public PoPurchaseOrder()
        {
            BuyerEntities = new List<PoBuyerEntity>();
            HeaderNotes   = new List<string>();
            Materials     = new List<PoMaterial>();
            Misc          = new List<string>();
        }

        public string DateOfOrder { get; set; }

        public string AtpNumber { get; set; }

        public List<PoBuyerEntity> BuyerEntities { get; }

        public List<string> HeaderNotes { get; }

        public PoSupplier Supplier { get; set; }

        public PoManufacturing Manufacturing { get; set; }

        public PoEconomicalInfo EconomicalInfo { get; set; }

        public List<PoMaterial> Materials { get; }

        public List<string> Misc { get; }
    }
}
