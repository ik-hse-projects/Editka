using System;
using System.Windows.Forms;

namespace Editka
{
    public class Actions
    {
        private readonly MainForm _root;

        public Actions(MainForm root)
        {
            _root = root;
        }

        public void TODO(object sender, EventArgs eventArgs)
        {
            MessageBox.Show("Not yet implemented!");
        }

        public void Exit(object sender, EventArgs eventArgs)
        {
            // TODO: Save everything and show pretty dialog
            Environment.Exit(0);
        }
    }
}