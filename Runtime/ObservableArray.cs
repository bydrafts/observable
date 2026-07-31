using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine;

namespace Drafts
{
    using Action = NotifyCollectionChangedAction;
    using ChangedArgs = NotifyCollectionChangedEventArgs;

    public delegate void ArrayEventHandler<in T>(int index, T value);

    public interface IObservableArray<out T> : IReadOnlyList<T>, INotifyCollectionChanged
    {
        event ArrayEventHandler<T> OnChanged;
    }

    /// <summary>
    /// Fixed-size observable collection that wraps an array.
    /// </summary>
    [Serializable]
    public class ObservableArray<T> : IObservableArray<T>, IList<T>, IList
    {
        [SerializeField] private T[] array;

        public event NotifyCollectionChangedEventHandler CollectionChanged;
        public event ArrayEventHandler<T> OnChanged;

        public T this[int index]
        {
            get => array[index];
            set {
                var old = array[index];
                array[index] = value;
                var args = new ChangedArgs(Action.Replace, value, old, index);
                CollectionChanged?.Invoke(this, args);
                OnChanged?.Invoke(index, value);
            }
        }

        public int Count => array.Length;
        public bool IsReadOnly => array.IsReadOnly;

        public bool Contains(T item) => Array.IndexOf(array, item) >= 0;
        public int IndexOf(T item) => Array.IndexOf(array, item);
        public void CopyTo(T[] array, int arrayIndex) => this.array.CopyTo(array, arrayIndex);

        public IEnumerator<T> GetEnumerator() => ((IEnumerable<T>)array).GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => array.GetEnumerator();

        public ObservableArray(int size) => array = new T[size];
        public ObservableArray(T[] initialArray) => array = initialArray;

        public void Add(T item) => throw new NotSupportedException("ObservableArray has a fixed size.");
        public void Insert(int index, T item) => throw new NotSupportedException("ObservableArray has a fixed size.");
        public bool Remove(T item) => throw new NotSupportedException("ObservableArray has a fixed size.");
        public void RemoveAt(int index) => throw new NotSupportedException("ObservableArray has a fixed size.");
        public void Clear() => throw new NotSupportedException("ObservableArray has a fixed size.");

        void ICollection.CopyTo(Array array, int index) => this.array.CopyTo(array, index);
        bool ICollection.IsSynchronized => array.IsSynchronized;
        object ICollection.SyncRoot => array.SyncRoot;
        object IList.this[int index] { get => ((IList)array)[index]; set => ((IList)array)[index] = value; }
        void IList.Remove(object value) => ((IList)array).Remove(value);
        bool IList.IsFixedSize => array.IsFixedSize;
        int IList.Add(object value) => ((IList)array).Add(value);
        bool IList.Contains(object value) => ((IList)array).Contains(value);
        int IList.IndexOf(object value) => ((IList)array).IndexOf(value);
        void IList.Insert(int index, object value) => ((IList)array).Insert(index, value);
    }
}