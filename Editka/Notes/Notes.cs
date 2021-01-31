using System.Windows.Forms;

namespace Editka
{
    public class Notes : TabControl
    {
        public BuildLog BuildLog { get; }

        public Notes(MainForm root)
        {
            BuildLog = new BuildLog(root);
            TabPages.Add(BuildLog);
        }
    }
}