using System;
using System.Collections.Generic;
using UnityEngine;

namespace Drafts
{
    public delegate void ValueChangedHandler<in T>(T value);

    public delegate void ValueChangedHandler2<in T>(T oldValue, T newValue);

    public interface IReadOnlyObservable
    {
        object Value { get; }
        event ValueChangedHandler<object> OnChanged;
    }

    public interface IObservable : IReadOnlyObservable
    {
        new object Value { get; set; }
        object IReadOnlyObservable.Value => Value;
    }

    public interface IReadOnlyObservable<out T> : IReadOnlyObservable
    {
        object IReadOnlyObservable.Value => Value;
        public new T Value { get; }
        public event ValueChangedHandler<T> OnValueChanged;
        public event ValueChangedHandler2<T> OnValueChanged2;
    }

    [Serializable]
    public class Observable<T> : IObservable, IReadOnlyObservable<T>
    {
        [SerializeField] private T value;

        public Observable() { }

        public Observable(T value) => this.value = value;

        object IObservable.Value { get => value; set => Value = value is T v ? v : default; }

        public T Value
        {
            get => value;
            set
            {
                if (EqualityComparer<T>.Default.Equals(value, this.value))
                    return;

                var old = this.value;
                this.value = value;
                OnChanged?.Invoke(value);
                OnValueChanged?.Invoke(value);
                OnValueChanged2?.Invoke(old, value);
            }
        }

        public void SetSilently(T val) => value = val;
        public event ValueChangedHandler<object> OnChanged;
        public event ValueChangedHandler<T> OnValueChanged;
        public event ValueChangedHandler2<T> OnValueChanged2;
        object IReadOnlyObservable.Value => Value;
        public override string ToString() => $"({typeof(T).Name}) {value}";
    }
}