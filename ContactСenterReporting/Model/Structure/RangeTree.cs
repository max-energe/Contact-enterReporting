using System;
using System.Collections;
using System.Collections.Generic;

namespace ContactСenterReporting.Model
{
    /// <summary>
    /// Реализация дерева диапазонов
    /// </summary>
    /// <typeparam name="T">Тип диапазона</typeparam>
    public class RangeTree<T> : IRangeTree<T>
    {
        private RangeTreeNode<T> root;
        private List<RangeValuePair<T>> items;
        private bool isInSync;
        private bool autoRebuild;
        private IComparer<T> _comparer;

        /// <summary>
        /// Указывает на то, следует ли перестраивать дерево.
        /// </summary>
        public bool AutoRebuild
        {
            get { return autoRebuild; }
            set { autoRebuild = value; }
        }

        /// <summary>
        /// Получает количество элементов, содержащихся в <see cref="RangeValuePair{T}"/>
        /// </summary>
        public int Count { get { return items.Count; } }

        /// <summary>
        /// Указывает на то, синхронизировано ли дерево или нет.
        /// </summary>
        public bool IsInSync { get { return isInSync; } }

        public bool IsReadOnly { get { return false; } }

        /// <summary>
        /// Возвращает все элементы, которые были перекрыты указанным значением.
        /// </summary>
        public IEnumerable<RangeValuePair<T>> this[T value]
        {
            get
            {
                if (!isInSync && autoRebuild)
                    Rebuild();

                return root.Query(value);
            }
        }

        /// <summary>
        /// Инициализация пустого дерева
        /// </summary>
        public RangeTree() : this(Comparer<T>.Default) { }

        /// <summary>
        /// Инициализация пустого дерева
        /// </summary>
        public RangeTree(IComparer<T> comparer)
        {
            _comparer = comparer;
            autoRebuild = true;
            Clear();
        }

        /// <summary>
        /// Добавление нового диапазона. Дерево не будет синзронизировано.
        /// </summary>
        public void Add(T from, T to)
        {
            if (_comparer.Compare(from, to) == 1)
                throw new ArgumentOutOfRangeException($"{nameof(from)} не может быть больше, чем {nameof(to)}");

            isInSync = false;
            items.Add(new RangeValuePair<T>(from, to));
        }

        /// <summary>
        /// Добавление нового диапазона. Дерево не будет синзронизировано.
        /// </summary>
        public void Add(RangeValuePair<T> item)
        {
            Add(item.From, item.To);
        }

        /// <summary>
        /// Полная очистка дерева
        /// </summary>
        public void Clear()
        {
            root = new RangeTreeNode<T>(_comparer);
            items = new List<RangeValuePair<T>>();
            isInSync = true;
        }

        public bool Contains(RangeValuePair<T> item)
        {
            return items.Contains(item);
        }

        public void CopyTo(RangeValuePair<T>[] array, int arrayIndex)
        {
            items.CopyTo(array, arrayIndex);
        }

        public IEnumerator<RangeValuePair<T>> GetEnumerator()
        {
            if (!isInSync && autoRebuild)
                Rebuild();

            return items.GetEnumerator();
        }

        /// <summary>
        /// Пересобирает дерево, если оно не синхронизировано.
        /// </summary>
        public void Rebuild()
        {
            if (isInSync)
                return;

            if (items.Count > 0)
                root = new RangeTreeNode<T>(items, _comparer);
            else
                root = new RangeTreeNode<T>(_comparer);
            isInSync = true;
            items.TrimExcess();
        }

        /// <summary>
        ///  Удаляет элемент из коллекции. Дерево выйдет из синхронизации.
        /// </summary>
        public bool Remove(RangeValuePair<T> item)
        {
            var removed = items.Remove(item);
            isInSync = isInSync && !removed;
            return removed;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
