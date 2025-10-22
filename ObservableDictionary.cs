using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using ChangeArgs = System.Collections.Specialized.NotifyCollectionChangedEventArgs;
using ChangeAction = System.Collections.Specialized.NotifyCollectionChangedAction;

namespace Utility
{
    public enum DictionaryEvent
    {
        Change,
        Add,
        Remove,
        Clear,
    }

    public delegate void DictionaryEventHandler<TKey, TValue>
        (DictionaryEvent action, KeyValuePair<TKey, TValue> pair);

    public interface IObservableDictionary<TKey, TValue>
        : IReadOnlyDictionary<TKey, TValue>, INotifyCollectionChanged
    {
        event DictionaryEventHandler<TKey, TValue> OnPairChanged;
    }

    [Serializable]
    public class ObservableDictionary<TKey, TValue> : IObservableDictionary<TKey, TValue>
    {
        private Dictionary<TKey, TValue> _dictionary = new();
        public event DictionaryEventHandler<TKey, TValue> OnPairChanged;
        public event NotifyCollectionChangedEventHandler CollectionChanged;

        public void Add(TKey key, TValue value)
        {
            _dictionary.Add(key, value);
            var pair = new KeyValuePair<TKey, TValue>(key, value);
            OnPairChanged?.Invoke(DictionaryEvent.Add, pair);
            CollectionChanged?.Invoke(this, new ChangeArgs(ChangeAction.Add, pair, -1));
        }

        public bool Remove(TKey key)
        {
            if (!_dictionary.Remove(key)) return false;
            var pair = new KeyValuePair<TKey, TValue>(key, default);
            OnPairChanged?.Invoke(DictionaryEvent.Remove, pair);
            CollectionChanged?.Invoke(this, new ChangeArgs(ChangeAction.Remove, pair, -1));
            return true;
        }

        public void Clear()
        {
            _dictionary.Clear();
            OnPairChanged?.Invoke(DictionaryEvent.Clear, default);
            CollectionChanged?.Invoke(this, new ChangeArgs(ChangeAction.Reset));
        }

        public TValue this[TKey key]
        {
            get => _dictionary[key];
            set
            {
                if (_dictionary.ContainsKey(key))
                {
                    _dictionary[key] = value;
                    OnPairChanged?.Invoke(DictionaryEvent.Change, new(key, value));
                }
                else Add(key, value);
            }
        }

        public int Count => _dictionary.Count;
        public bool TryGetValue(TKey key, out TValue value) => _dictionary.TryGetValue(key, out value);
        public bool ContainsKey(TKey key) => _dictionary.ContainsKey(key);
        public IEnumerable<TKey> Keys => _dictionary.Keys;
        public IEnumerable<TValue> Values => _dictionary.Values;
        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() => _dictionary.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}