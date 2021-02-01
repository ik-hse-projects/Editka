using System.Windows.Forms;
using Timer = System.Timers.Timer;

namespace Editka
{
    public class Autosave
    {
        private readonly MainForm _root;
        private bool _running;
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

        public static void Init(MainForm root)
        {
            var _ = new Autosave(root);
        }

        private void Save()
        {
            if (_running)
            {
                return;
            }

            try
            {
                _running = true;
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