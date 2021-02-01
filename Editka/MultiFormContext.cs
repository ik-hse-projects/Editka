using System;
using System.Threading;
using System.Windows.Forms;

namespace Editka
{
    /// <summary>
    /// Класс помогает открывать сразу несколько независимых окон в одном процессе.
    /// Подробнее: https://stackoverflow.com/a/15301089
    /// </summary>
    public class MultiFormContext : ApplicationContext
    {
        /// <summary>
        /// Этот класс — синглтон в этой программе.
        /// </summary>
        public static MultiFormContext Context;

        /// <summary>
        /// Число открытых на данный момент форм.
        /// </summary>
        private int openForms;

        /// <summary>
        /// Точка входа!
        /// </summary>
        [STAThread]
        public static void Main()
        {
            var initialState = State.DeserializeLatest() ?? new State();
            Context = new MultiFormContext();
            Context.AddForm(new MainForm(initialState));
            Application.EnableVisualStyles();
            Application.Run(Context);
        }

        /// <summary>
        /// Добавляет и открывает формочку.
        /// </summary>
        public void AddForm(MainForm form)
        {
            openForms++;

            form.FormClosed += (s, args) =>
            {
                if (Interlocked.Decrement(ref openForms) == 0)
                {
                    ExitThread();
                }
            };

            form.Show();
        }
    }
}