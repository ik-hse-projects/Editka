using System.Windows.Forms;
using Timer = System.Timers.Timer;

namespace Editka
{
    /// <summary>
    /// Реализация автосохранения файлов. Каждые N секунд проходит по изменённым файлам и, возможно, сохраняет их.
    /// </summary>
    public class Autosave
    {
        private readonly MainForm _root;
        
        /// <summary>
        /// Происходит ли сохранение в этот момент.
        /// </summary>
        private bool _running;
        
        /// <summary>
        /// Таймер, который вызывает сохранение.
        /// </summary>
        private readonly Timer _timer;

        private Autosave(MainForm root)
        {
            _root = root;
            _timer = new Timer
            {
                AutoReset = true
            };
            UpdateTimer(_root.Settings.AutosaveSeconds.Value);
            _root.Settings.AutosaveSeconds.Changed += (_, newValue) => UpdateTimer(newValue);
            _timer.Elapsed += (sender, args) => Save();
        }

        /// <summary>
        /// Перенастраивает таймер, когда меняется соответствующий параметр в настройках.
        /// </summary>
        /// <param name="interval">Новый интервал автосохранения.</param>
        private void UpdateTimer(int interval)
        {
            if (interval <= 0)
            {
                interval = int.MaxValue;
            }
            else
            {
                interval = interval * 1000;
            }

            _timer.Interval = interval;
            _timer.Enabled = interval != int.MaxValue;
        }

        /// <summary>
        /// Инициализирует автосохранение в данном окне программы.
        /// </summary>
        public static void Init(MainForm root)
        {
            var _ = new Autosave(root);
        }

        /// <summary>
        /// Сохраняет все файлы в окне, к которому относится этот объект.
        /// </summary>
        private void Save()
        {
            if (_running)
            {
                return;
            }

            _running = true;
            try
            {
                if (_root.IsHandleCreated)
                {
                    _root.Invoke((MethodInvoker) delegate { _root.Actions.SaveAll(false); });
                }
            }
            finally
            {
                _running = false;
            }
        }
    }
}