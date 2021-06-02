using System;
using System.Collections.Generic;

namespace ContactСenterReporting.Model
{
    /// <summary>
    /// Характеризует диапазон значений
    /// </summary>
    /// <typeparam name="T">Тип значений диапазона</typeparam>
    public struct RangeValuePair<T> : IEquatable<RangeValuePair<T>>
    {
        public T From { get; }
        public T To { get; }

        /// <summary>
        /// Инициализация нового диапазона значений
        /// </summary>
        /// <param name="from">Начальное значение диапазона</param>
        /// <param name="to">Конечное значение диапазона</param>
        public RangeValuePair(T from, T to) : this()
        {
            From = from;
            To = to; 
        }

        /// <summary>
        /// Возвращает <see cref="System.String"/>, который представляет этот экземпляр.
        /// </summary>
        public override string ToString()
        {
            return string.Format("[{0} - {1}]", From, To);
        }

        public override int GetHashCode()
        {
            var hash = 25;
            if (From != null)
                hash = hash * 43 + From.GetHashCode();
            if (To != null)
                hash = hash * 43 + To.GetHashCode();

            return hash;
        }

        public bool Equals(RangeValuePair<T> other)
        {
            return EqualityComparer<T>.Default.Equals(From, other.From)
                && EqualityComparer<T>.Default.Equals(To, other.To);
        }

        public override bool Equals(object obj)
        {
            if (!(obj is RangeValuePair<T>))
                return false;

            return Equals((RangeValuePair<T>)obj);
        }
    }
}
