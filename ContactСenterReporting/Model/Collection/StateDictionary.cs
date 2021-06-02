using System;
using System.Collections.Generic;
using System.Linq;

namespace ContactСenterReporting.Model
{
    public class StateDictionary<TKey, TValue> 
    {
        public IEnumerable<KeyValuePair<TKey, TValue>> items;

        public int Count => items.Count();

        public StateDictionary(IEnumerable<KeyValuePair<TKey, TValue>> _items)
            => items = _items;

        public override string ToString()
        {
            string result = string.Empty;
            var ef = items.ToList();

            foreach (var state in Enum.GetNames(typeof(State)))
            {
                if (items.Count(x => x.Key.ToString() == state) != 0)
                    result += $"{state}: {Math.Round(Convert.ToDouble(items.First(x => x.Key.ToString() == state).Value),2)} ";
                else
                    result += $"{state}: 0 ";

            }
             
            return result;
        }
    }
}
