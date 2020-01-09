using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace TazParser.Ui.Business
{
    public static class Extensions
    {
        public static void AddRange<T>(this ObservableCollection<T> collection, IEnumerable<T> values)
        {
            foreach (var value in values)
            {
                if (collection.All(item => !item.Equals(value)))
                {
                    collection.Add(value);
                }
            }
        }
    }
}
