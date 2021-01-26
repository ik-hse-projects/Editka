using System;
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
            Context = new MultiFormContext(new MainForm());
            Application.Run(Context);
        }

        private int openForms;

        public MultiFormContext(params Form[] forms)
        {
            foreach (var form in forms)
            {
                AddForm(form);
            }
        }

        public void AddForm(Form form)
        {
            openForms++;

            form.FormClosed += (s, args) =>
            {
                //When we have closed the last of the "starting" forms, 
                //end the program.
                if (Interlocked.Decrement(ref openForms) == 0)
                    ExitThread();
            };

            form.Show();
        }
    }
}