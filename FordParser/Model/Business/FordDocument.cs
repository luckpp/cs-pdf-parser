using System.Collections.Generic;

namespace FordParser.Model.Business
{
    public class FordDocument
    {
        public string Name { get; set; }

        public IEnumerable<FordPage> Pages { get; set; }
    }
}
