using System.Collections.Generic;
using System.Linq;

namespace ContactСenterReporting.Model
{
    /// <summary>
    /// Узел дерева диапазонов 
    /// </summary>
    internal class RangeTreeNode<T> : IComparer<RangeValuePair<T>>
    {
        private T center;

        private RangeTreeNode<T> leftNode;
        private RangeTreeNode<T> rightNode;
        private RangeValuePair<T>[] items;

        private readonly IComparer<T> comparer;

        /// <summary>
        /// Инициализация пустого узла
        /// </summary>
        /// <param name="comparer">Компаратор для сравнивания двух элементов</param>
        public RangeTreeNode(IComparer<T> comparer)
        {
            this.comparer = comparer ?? Comparer<T>.Default;

            center = default;
            leftNode = null;
            rightNode = null;
            items = null;
        }

        /// <summary>
        /// Инициализирует узел, наполняя его элементами.
        /// Производит построение поддеревьев.
        /// </summary>
        /// <param name="comparer">The comparer used to compare two items.</param>
        public RangeTreeNode(IList<RangeValuePair<T>> items, IComparer<T> comparer)
        {
            this.comparer = comparer ?? Comparer<T>.Default;

            var endPoints = new List<T>(items.Count * 2);
            foreach (var item in items)
            {
                endPoints.Add(item.From);
                endPoints.Add(item.To);
            }
            endPoints.Sort(this.comparer);
            endPoints = endPoints.Distinct().ToList();
            // Определяем центральное значение коллекции
            center = endPoints[endPoints.Count / 2];

            var inner = new List<RangeValuePair<T>>();
            var left = new List<RangeValuePair<T>>();
            var right = new List<RangeValuePair<T>>();

            // Распределение элементов по коллекциям
            foreach (var item in items)
            {
                if (this.comparer.Compare(item.To, center) < 0)
                    left.Add(item);
                else if (this.comparer.Compare(item.From, center) > 0)
                    right.Add(item);
                else
                    inner.Add(item);
            }


            if (inner.Count > 0)
            {
                if (inner.Count > 1)
                    inner.Sort(this);
                this.items = inner.ToArray();
            }
            else
            {
                this.items = null;
            }

            // Создаем узлы слева и справа 
            if (left.Count > 0)
                leftNode = new RangeTreeNode<T>(left, this.comparer);
            if (right.Count > 0)
                rightNode = new RangeTreeNode<T>(right, this.comparer);
        }


        /// <summary>
        /// Возвращает все элементы, которые были перекрыты указанным значеним.
        /// </summary>
        public IEnumerable<RangeValuePair<T>> Query(T value)
        {
            var results = new List<RangeValuePair<T>>();

            if (items != null)
            {
                // Проверяем наличие листьев, соедаржащих значение
                foreach (var o in items)
                {
                    if (comparer.Compare(o.From, value) > 0)
                        break;
                    else if (comparer.Compare(value, o.From) >= 0 && comparer.Compare(value, o.To) <= 0)
                    {
                        results.Add(o);
                    }
                }
            }

            // Сравниваем значение запроса с центром, двигаемся влево/вправо
            var centerComp = comparer.Compare(value, center);
            if (leftNode != null && centerComp < 0)
                results.AddRange(leftNode.Query(value));
            else if (rightNode != null && centerComp > 0)
                results.AddRange(rightNode.Query(value));

            return results;
        }

        /// <summary>
        /// Возвращает меньше 0, если начальное значение первого диапазона меньше, чем второго. Возвращает 0, если больше.
        /// Если начальные значения диапазонов точки равны, то возвращает результат сравнения значений  конечных точек.
        /// 0 если все равны.
        /// </summary>
        /// <param name="x">Первый диапазон</param>
        /// <param name="y">Второй диапазон</param>
        /// <returns></returns>
        int IComparer<RangeValuePair<T>>.Compare(RangeValuePair<T> x, RangeValuePair<T> y)
        {
            var fromComp = comparer.Compare(x.From, y.From);
            if (fromComp == 0)
                return comparer.Compare(x.To, y.To);
            return fromComp;
        }
    }
}