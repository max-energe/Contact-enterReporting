using System.Collections.Generic;

namespace ContactСenterReporting.Model
{
    /// <summary>
    /// Интерфейс по работе с деревом диапазонов
    /// </summary>
    /// <typeparam name="T">Тип диапазона</typeparam>
    public interface IRangeTree<T>:
        IEnumerable<RangeValuePair<T>>, 
        ICollection<RangeValuePair<T>>
    {
        IEnumerable<RangeValuePair<T>> this[T value] { get; }

        void Add(T From, T To);
        void Rebuild();
    }
}
