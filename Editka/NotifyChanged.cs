using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Drawing.Imaging;
using System.Linq;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace Editka
{
    [Serializable]
    public struct KeyValuePair<TKey, TValue>
    {
        public KeyValuePair(TKey key, TValue value)
        {
            Key = key;
            Value = value;
        }

        public TKey Key { get; set; }
        public TValue Value { get; set; }
    }

    public delegate void PropertyChanged<in T>(T oldValue, T newValue);

    public class NotifyChanged<T>
    {
        [XmlIgnore] private T _value;

        public T Value
        {
            get => _value;
            set
            {
                if ((_value != null && _value.Equals(value)) || (_value == null && value == null))
                {
                    return;
                }

                var old = _value;
                _value = value;
                Changed?.Invoke(old, value);
            }
        }

        public event PropertyChanged<T>? Changed;

        public NotifyChanged() : this(default)
        {
        }

        public NotifyChanged(T value)
        {
            _value = value;
        }
    }

    public class NotifiableDictionary<TKey, TVal> : IEnumerable<KeyValuePair<TKey, TVal>>
    {
        private readonly Dictionary<TKey, NotifyChanged<TVal>>
            _dictionary = new Dictionary<TKey, NotifyChanged<TVal>>();

        public bool TryGetValue(TKey key, out TVal value)
        {
            var exists = _dictionary.TryGetValue(key, out var result);
            value = exists ? result.Value : default;
            return exists;
        }

        public event PropertyChanged<KeyValuePair<TKey, TVal>>? Changed;

        // For xml serialization
        [SuppressMessage("ReSharper", "UnusedMember.Global")]
        public void Add(KeyValuePair<TKey, TVal> pair)
        {
            Get(pair.Key).Value = pair.Value;
        }

        public NotifyChanged<TVal> Get(TKey key) => GetOrDefault(key, default);

        public NotifyChanged<TVal> GetOrDefault(TKey key, TVal fallback)
        {
            if (_dictionary.TryGetValue(key, out var result))
            {
                return result;
            }

            result = new NotifyChanged<TVal>(fallback);
            _dictionary[key] = result;
            return result;
        }

        public IEnumerator<KeyValuePair<TKey, TVal>> GetEnumerator() =>
            _dictionary
                .Select(x => new KeyValuePair<TKey, TVal>(x.Key, x.Value.Value))
                .ToList()
                .GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public IEnumerable<KeyValuePair<TKey, NotifyChanged<TVal>>> Notifiable() => _dictionary
            .Select(x => new KeyValuePair<TKey, NotifyChanged<TVal>>(x.Key, x.Value));
    }

    public class Computed<T>
    {
        private readonly Func<T> _lambda;
        private readonly NotifyChanged<T> notifyChanged;

        public T Value => notifyChanged.Value;

        public void Update()
        {
            notifyChanged.Value = _lambda();
        }

        public Computed(Func<T> lambda)
        {
            _lambda = lambda;
            notifyChanged = new NotifyChanged<T>(lambda());
            notifyChanged.Changed += (oldValue, newValue) => Changed?.Invoke(oldValue, newValue);
        }

        public event PropertyChanged<T>? Changed;
    }
}