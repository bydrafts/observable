using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine;

namespace Drafts
{
    using Action = NotifyCollectionChangedAction;
    using ChangedArgs = NotifyCollectionChangedEventArgs;

    public enum ListEvent
    {
        Change,
        Add,
        Remove,
        Clear,
    }

    public delegate void ListEventHandler<TValue>(ListEvent action, int index, TValue value);

    public interface IObservableList<out T> : IReadOnlyList<T>, INotifyCollectionChanged { }

    /// <summary>
    /// Made this cause ObservableCollection inst serializable
    /// </summary>
    [Serializable]
    public class ObservableList<T> : IList<T>, IObservableList<T>, IList
    {
        [SerializeField] private List<T> list;

        public event ListEventHandler<T> OnChanged;
        public event NotifyCollectionChangedEventHandler CollectionChanged;

        public T this[int index]
        {
            get => list[index];
            set {
                var old = list[index];
                list[index] = value;
                var args = new ChangedArgs(Action.Replace, value, old, index);
                CollectionChanged?.Invoke(this, args);
                OnChanged?.Invoke(ListEvent.Change, index, value);
            }
        }

        public int Count => list.Count;

        public bool Contains(T item) => list.Contains(item);
        public int IndexOf(T item) => list.IndexOf(item);
        public int FindIndex(Predicate<T> predicate) => list.FindIndex(predicate);
        public void CopyTo(T[] array, int arrayIndex) => list.CopyTo(array, arrayIndex);
        public void Add(T item) => Insert(Count, item);

        public void AddRange(IEnumerable<T> items)
        {
            foreach (var item in items) Add(item);
        }

        public IEnumerator<T> GetEnumerator() => list.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)list).GetEnumerator();

        public ObservableList() => list = new();
        public ObservableList(IEnumerable<T> items) => list = new(items);

        public void Insert(int index, T item)
        {
            list.Insert(index, item);
            var args = new ChangedArgs(Action.Add, item, index);
            CollectionChanged?.Invoke(this, args);
            OnChanged?.Invoke(ListEvent.Add, index, item);
        }

        public void Clear()
        {
            list.Clear();
            var args = new ChangedArgs(Action.Reset);
            CollectionChanged?.Invoke(this, args);
            OnChanged?.Invoke(ListEvent.Clear, 0, default);
        }

        public bool Remove(T item)
        {
            var index = list.IndexOf(item);
            if (index < 0) return false;
            RemoveAt(index);
            return true;
        }

        public void RemoveAt(int index)
        {
            var item = list[index];
            list.RemoveAt(index);
            var args = new ChangedArgs(Action.Remove, item, index);
            CollectionChanged?.Invoke(this, args);
            OnChanged?.Invoke(ListEvent.Remove, index, item);
        }

        public void RemoveAll(Func<T, bool> predicate)
        {
            for (var i = list.Count - 1; i >= 0; i--)
                if (predicate(list[i]))
                    RemoveAt(i);
        }

        public bool IsReadOnly => ((IList<T>)list).IsReadOnly;
        void ICollection.CopyTo(Array array, int index) => ((ICollection)list).CopyTo(array, index);
        bool ICollection.IsSynchronized => ((ICollection)list).IsSynchronized;
        object ICollection.SyncRoot => ((ICollection)list).SyncRoot;
        object IList.this[int index] { get => ((IList)list)[index]; set => ((IList)list)[index] = value; }
        int IList.Add(object value) => ((IList)list).Add(value);
        bool IList.Contains(object value) => ((IList)list).Contains(value);
        int IList.IndexOf(object value) => ((IList)list).IndexOf(value);
        void IList.Insert(int index, object value) => ((IList)list).Insert(index, value);
        void IList.Remove(object value) => ((IList)list).Remove(value);
        bool IList.IsFixedSize => ((IList)list).IsFixedSize;
    }
}