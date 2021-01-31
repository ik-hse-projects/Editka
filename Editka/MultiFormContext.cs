using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows.Forms;

namespace Editka
{
    // https://stackoverflow.com/a/15301089
    public class MultiFormContext : ApplicationContext
    {
        public static MultiFormContext Context;

        [STAThread]
        public static void Main()
        {
            var initialState = State.DeserializeLatest() ?? new State();
            Context = new MultiFormContext(new MainForm(initialState));
            Application.EnableVisualStyles();
            Application.Run(Context);
        }

        private int openForms;

        public MultiFormContext(params MainForm[] forms)
        {
            foreach (var form in forms)
            {
                AddForm(form);
            }
        }

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