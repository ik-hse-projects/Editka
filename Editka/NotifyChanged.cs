using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Xml.Serialization;

namespace Editka
{
    /// <summary>
    /// Альтернативная реализация стандратоного KeyValuePair, которая нормально сериалзуется в xml.
    /// <seealso cref="System.Collections.Generic.KeyValuePair{TKey,TValue}"/>
    /// </summary>
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

    /// <summary>
    /// Обработчик события изменения значения.
    /// </summary>
    /// <param name="oldValue">Старое значение.</param>
    /// <param name="newValue">Новое значение.</param>
    public delegate void PropertyChanged<in T>(T oldValue, T newValue);

    /// <summary>
    /// Тип-обёртка, который вызывает событие, когда хранимое внутри значение меняется.
    /// </summary>
    public class NotifyChanged<T>
    {
        [XmlIgnore] private T _value;

        public NotifyChanged() : this(default)
        {
        }

        public NotifyChanged(T value)
        {
            _value = value;
        }

        /// <summary>
        /// Хранимое внутри значение.
        /// </summary>
        public T Value
        {
            get => _value;
            set
            {
                if (_value != null && _value.Equals(value) || _value == null && value == null)
                {
                    return;
                }

                var old = _value;
                _value = value;
                Changed?.Invoke(old, value);
            }
        }

        /// <summary>
        /// Событие, которое вызывается, когда <see cref="Value"/> меняется.
        /// </summary>
        public event PropertyChanged<T>? Changed;
    }

    /// <summary>
    /// Аналогично <see cref="NotifyChanged{T}"/>, но словарь. Все значение внутри — NotifyChanged<TVal>
    /// </summary>
    public class NotifiableDictionary<TKey, TVal> : IEnumerable<KeyValuePair<TKey, TVal>>
    {
        [XmlIgnore] private readonly Dictionary<TKey, NotifyChanged<TVal>>
            _dictionary = new Dictionary<TKey, NotifyChanged<TVal>>();

        /// <summary>
        /// Этот метод используется для сериализации.
        /// См. замечание про IEnumerable:
        /// https://docs.microsoft.com/en-US/dotnet/api/system.xml.serialization.xmlserializer?view=net-5.0#overriding-default-serialization
        /// </summary>
        public IEnumerator<KeyValuePair<TKey, TVal>> GetEnumerator()
        {
            return _dictionary
                .Select(x => new KeyValuePair<TKey, TVal>(x.Key, x.Value.Value))
                .ToList()
                .GetEnumerator();
        }

        /// <summary>
        /// Этот метод используется для сериализации.
        /// </summary>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// Этот метод тоже используется для сериализации.
        /// </summary>
        [SuppressMessage("ReSharper", "UnusedMember.Global")]
        public void Add(KeyValuePair<TKey, TVal> pair)
        {
            Get(pair.Key).Value = pair.Value;
        }

        /// <summary>
        /// Никогда не выкидывает ошибку. Если такого ключа нет, то создаёт пустое значение и сохраняет его в словарь.
        /// Очень удобно, чтобы подписаться на изменение (добавление) этого значения.
        /// </summary>
        public NotifyChanged<TVal> Get(TKey key)
        {
            return GetOrDefault(key, default);
        }

        /// <summary>
        /// Возвращает значение из словаря, а если такого нет, то сохраняет fallback в словарь и возвращает его. 
        /// </summary>
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

        /// <summary>
        /// Интерпретирует словарь как IEnumerable of NotifyChanged.
        /// </summary>
        public IEnumerable<KeyValuePair<TKey, NotifyChanged<TVal>>> Notifiable()
        {
            return _dictionary
                .Select(x => new KeyValuePair<TKey, NotifyChanged<TVal>>(x.Key, x.Value));
        }
    }

    /// <summary>
    /// Значение, которое вычисляется какой-то функцией.
    /// Если считать NotifyChanged за property, то это что-то вроде get-only property с лямбдой.
    /// Аланлогично NotifyChanged есть событие об изменении.
    /// Главное не забывать уведомлять, что нужно пересчитать значение (<see cref="Update"/>).
    /// </summary>
    public class Computed<T>
    {
        /// <summary>
        /// Функция, вычисляющая значение.
        /// </summary>
        private readonly Func<T> _lambda;
        
        /// <summary>
        /// Само значение.
        /// </summary>
        private readonly NotifyChanged<T> notifyChanged;

        public Computed(Func<T> lambda)
        {
            _lambda = lambda;
            notifyChanged = new NotifyChanged<T>(lambda());
        }

        public T Value => notifyChanged.Value;

        /// <summary>
        /// Уведомляет, что необходимо обновить значение внутри.
        /// </summary>
        public void Update()
        {
            notifyChanged.Value = _lambda();
        }

        public event PropertyChanged<T>? Changed
        {
            add => notifyChanged.Changed += value;
            remove => notifyChanged.Changed -= value;
        }
    }
}