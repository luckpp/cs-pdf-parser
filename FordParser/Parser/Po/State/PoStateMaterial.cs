using System.Collections.Generic;
using System.Linq;
using FordParser.Constants;
using FordParser.Model.Business.Po;

namespace FordParser.Parser.Po.State
{
    internal class PoStateMaterial : IParserPageState
    {
        private readonly IPoPageContext _context;

        public PoStateMaterial(IPoPageContext context)
        {
            _context = context;
        }

        public void Process()
        {
            var table = new Table();

            var purchase = _context.PurchaseOrder;
            var line = _context.Line;

            if (line.Contains(PoConstants.PlantCodeName))
            {
                while (line != null && line != PoConstants.PackagingTerm)
                {
                    table.Add(new Column { Header = line });
                    _context.Next();
                    line = _context.Line;
                }
                table.Add(new Column { Header = line }); // add also the last column

                _context.Next();
                line = _context.Line;

                var index = 0;
                var columnCount = table.ColumnCount;
                while (line != null && line != PoConstants.Remarks)
                {
                    var column = table[index];
                    column.Values.Add(line);
                    index++;
                    index = index % columnCount;
                    _context.Next();
                    line = _context.Line;
                }

                purchase.Materials.AddRange(table.GetMaterials());

                _context.State = new PoStateFooter(_context);
                _context.Process();
            }
        }

        internal class Table
        {
            private readonly List<Column> _columns;

            public Table()
            {
                _columns = new List<Column>();
            }

            public void Add(Column column)
            {
                _columns.Add(column);
            }

            public Column this[int index] => _columns[index];

            public int ColumnCount => _columns.Count;

            public int RowCount
            {
                get
                {
                    var column = _columns.FirstOrDefault();
                    return column?.Values.Count ?? 0;
                }
            }

            public Column GetColumn(string header)
            {
                return _columns.FirstOrDefault(c => c.Header == header);
            }

            public IEnumerable<PoMaterial> GetMaterials()
            {
                var result = new List<PoMaterial>();

                for (int i = 0; i < RowCount; i++)
                {
                    var cPlantCodeName       = GetColumn(PoConstants.PlantCodeName);
                    var cPercent             = GetColumn(PoConstants.Percent);
                    var cUoM                 = GetColumn(PoConstants.UoM);
                    var cPrice               = GetColumn(PoConstants.Price);
                    var cEffectiveDate       = GetColumn(PoConstants.EffectiveDate);
                    var cIor                 = GetColumn(PoConstants.Ior);
                    var cShipFromCodeCountry = GetColumn(PoConstants.ShipFromCodeCountry);
                    var cDeliveryTerm        = GetColumn(PoConstants.DeliveryTerm);
                    var cPackagingTerm       = GetColumn(PoConstants.PackagingTerm);

                    var material = new PoMaterial
                    {
                        PlantCodeName       = cPlantCodeName.Values[i],
                        Percent             = cPercent.Values[i],
                        UoM                 = cUoM.Values[i],
                        Price               = cPrice.Values[i],
                        EffectiveDate       = cEffectiveDate.Values[i],
                        Ior                 = cIor.Values[i],
                        ShipFromCodeCountry = cShipFromCodeCountry.Values[i],
                        DeliveryTerm        = cDeliveryTerm.Values[i],
                        PackagingTerm       = cPackagingTerm.Values[i]
                    };

                    result.Add(material);
                }

                return result;
            }
        }

        internal class Column
        {
            public Column()
            {
                Values = new List<string>();
            }

            public string Header { get; set; }

            public List<string> Values { get; }
        }
    }
}
