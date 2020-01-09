using FordParser.Constants;
using FordParser.Model.Business.Po;

namespace FordParser.Parser.Po.State
{
    class PoStateEconomicalInfo : IParserPageState
    {
        private readonly IPoPageContext _context;

        public PoStateEconomicalInfo(IPoPageContext context)
        {
            _context = context;
        }

        public void Process()
        {
            var purchase = _context.PurchaseOrder;
            
            purchase.EconomicalInfo = new PoEconomicalInfo();

            var line = _context.Line; //BuyerRepresentative
            _context.Next(); //BuyerRepresentativeCode
            _context.Next(); //PartNumber
            _context.Next(); //PartName

            _context.Next();
            line = _context.Line;
            purchase.EconomicalInfo.BuyerRepresentative = line;

            _context.Next();
            line = _context.Line;
            purchase.EconomicalInfo.BuyerRepresentativeCode = line;

            _context.Next();
            line = _context.Line;
            purchase.EconomicalInfo.PartNumber = line;

            _context.Next();
            line = _context.Line;
            purchase.EconomicalInfo.PartName = line;


            _context.Next(); // AtpNumber
            _context.Next(); // EconomicLevelMaterial
            _context.Next(); // SupersededPartNumber
            _context.Next(); // EngineeringLevel

            _context.Next();
            line = _context.Line;
            purchase.EconomicalInfo.AtpNumber = line;

            _context.Next();
            line = _context.Line;
            purchase.EconomicalInfo.EconomicLevelMaterial = line;

            _context.Next();
            line = _context.Line;
            purchase.EconomicalInfo.SupersededPartNumber = line;

            _context.Next();
            line = _context.Line;
            purchase.EconomicalInfo.EngineeringLevel = line;

            _context.Next(); // Currency
            _context.Next(); // EconomicLevelLo

            _context.Next();
            line = _context.Line;
            purchase.EconomicalInfo.Currency = line;

            _context.Next();
            line = _context.Line;
            purchase.EconomicalInfo.EconomicLevelLo = line;


            _context.Next();
            line = _context.Line;

            if (line.StartsWith(PoConstants.PlantCodeName))
            {
                _context.State = new PoStateMaterial(_context);
                _context.Process();
            }
        }
    }
}
