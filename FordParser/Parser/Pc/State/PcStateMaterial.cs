using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FordParser.Constants;
using FordParser.Model.Business.Pc;

namespace FordParser.Parser.Pc.State
{
    internal class PcStateMaterial : IParserPageState
    {
        private readonly IPcPageContext _context;

        public PcStateMaterial(IPcPageContext context)
        {
            _context = context;
        }

        public void Process()
        {
            var table = new Table();
            PcPriceChange priceChange = _context.PriceChange;
            var line = _context.Line;

            if (line.StartsWith(PcConstants.PartNumber))
            {
                while (line != null && line != PcConstants.MaterialWeight)
                {
                    // TODO : review for the moment we consider that 'FINIS Code' will not have any values on it so we skip it
                    //if (line != PcConstants.FinisCode)
                    //{
                    //    table.Add(new Column { Header = line });
                    //}
                    table.Add(new Column { Header = line });

                    _context.Next();
                    line = _context.Line;
                }
                table.Add(new Column { Header = line }); // add also the last column MaterialWeight

                _context.Next();
                line = _context.Line;

                var index = 0;
                var columnCount = table.ColumnCount;

                // TODO: see problem with FINIS Code (present / not present)
                bool justAddedPartNumber = false;

                while (line != null && line != PoConstants.Remarks)
                {
                    if (justAddedPartNumber)
                    {
                        if (!IsFinisCode(line)) // TODO: see problem with FINIS Code (present / not present)
                        {
                            var finisCodeColumn = table[index];
                            finisCodeColumn.Values.Add("");

                            index++;
                            index = index % columnCount;
                        }
                    }
                    justAddedPartNumber = IsPartNumber(line);

                    var column = table[index];
                    column.Values.Add(line);

                    index++;
                    index = index % columnCount;

                    _context.Next();
                    line = _context.Line;
                }

                priceChange.Materials.AddRange(table.GetMaterials());

                if (line == PoConstants.Remarks)
                {
                    _context.State = new PcStateBottom(_context);
                    _context.Process();
                }
                else
                {
                    throw new Exception($"[PcStateMaterial] Unkonwon format {line}");
                }
            }
        }

        private bool IsPartNumber(string line)
        {
            line = line.Trim();
            var tokens = line.Split('-');

            return tokens.Length == 3 && tokens.All(token => !token.Contains(' '));
        }

        private bool IsFinisCode(string line)
        {
            line = line.Trim();
            int number;

            return line.Length == 7 && int.TryParse(line, out number);
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

            public IEnumerable<PcMaterial> GetMaterials()
            {
                var result = new List<PcMaterial>();

                for (int i = 0; i < RowCount; i++)
                {
                    var cPartNumber           = GetColumn(PcConstants.PartNumber);
                    var cPlantCodes           = GetColumn(PcConstants.PlantCodes);
                    var cBuyerRepCodeName     = GetColumn(PcConstants.BuyerRepCodeName);
                    var cInvoicePrice         = GetColumn(PcConstants.InvoicePrice);
                    var cPreviousInvoicePrice = GetColumn(PcConstants.PreviousInvoicePrice);
                    var cServicePackPrice     = GetColumn(PcConstants.ServicePackPrice);
                    var cMaterialWeight       = GetColumn(PcConstants.MaterialWeight);

                    var material = new PcMaterial
                    {
                        PartNumber           = cPartNumber.Values[i],
                        PlantCodes           = cPlantCodes.Values[i],
                        BuyerRepCodeName     = cBuyerRepCodeName.Values[i],
                        InvoicePrice         = cInvoicePrice.Values[i],
                        PreviousInvoicePrice = cPreviousInvoicePrice.Values[i],
                        ServicePackPrice     = cServicePackPrice.Values[i],
                        MaterialWeight       = cMaterialWeight.Values[i]
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
