using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FordParser.Model.Business.Pc
{
    public class PcPriceChange
    {
        public PcPriceChange()
        {
            Materials = new List<PcMaterial>();
        }

        public PcEconomicalInfo EconomicalInfo { get; set; }

        public List<PcMaterial> Materials { get; }
    }
}
