namespace FordParser.Model.Business.Po
{
    public class PoEconomicalInfo
    {
        //-----

        public string BuyerRepresentative { get; set; }
        public string BuyerRepresentativeCode { get; set; }
        public string AtpNumber { get; set; }
        public string Currency { get; set; }
        public string EconomicLevelMaterial { get; set; }
        public string EconomicLevelLo { get; set; }

        //-----

        public string PartNumber { get; set; }
        public string PartName { get; set; }
        public string SupersededPartNumber { get; set; }
        public string EngineeringLevel { get; set; }
    }
}
