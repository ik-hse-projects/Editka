using System.Windows.Forms;

namespace Editka
{
    public class Notes : TabControl
    {
        public Comments Comments { get; }
        public Notes()
        {
            Comments = new Comments();
            TabPages.Add(Comments);
        }
    }
}