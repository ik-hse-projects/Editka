using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Linq;

namespace Editka
{
    public delegate void PropertyChanged<in T>(T oldValue, T newValue);

    public class NotifyChanged<T>
    {
        private T _value;

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

        public TVal this[TKey key]
        {
            get => _dictionary[key].Value;
            set
            {
                if (_dictionary.TryGetValue(key, out NotifyChanged<TVal> existing))
                {
                    if ((existing.Value == null && value == null) || (value != null && value.Equals(existing.Value)))
                    {
                        return;
                    }
                }
                else
                {
                    existing = new NotifyChanged<TVal>();
                    _dictionary[key] = existing;
                }

                Changed?.Invoke(new KeyValuePair<TKey, TVal>(key, existing.Value),
                    new KeyValuePair<TKey, TVal>(key, value));
                existing.Value = value;
            }
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

        public IEnumerator<KeyValuePair<TKey, TVal>> GetEnumerator()
        {
            return _dictionary
                .Select(x => new KeyValuePair<TKey, TVal>(x.Key, x.Value.Value))
                .GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public IEnumerable<KeyValuePair<TKey, NotifyChanged<TVal>>> Notifiable() => _dictionary;
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