using System;
using System.Windows.Forms;

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

    public static class NotifyChangedExtensions
    {
        public static NumericUpDown GetControl(this NotifyChanged<int> self)
        {
            var result = new NumericUpDown
            {
                Value = self.Value,
            };
            result.ValueChanged += (sender, args) => self.Value = (int) result.Value;
            self.Changed += (value, newValue) =>
            {
                if (newValue != result.Value)
                {
                    result.Value = newValue;
                }
            };
            return result;
        }
        
        public static CheckBox GetControl(this NotifyChanged<bool> self)
        {
            var result = new CheckBox
            {
                Checked = self.Value,
            };
            result.CheckedChanged += (sender, args) => self.Value = result.Checked;
            self.Changed += (value, newValue) =>
            {
                if (newValue != result.Checked)
                {
                    result.Checked = newValue;
                }
            };
            return result;
        }
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