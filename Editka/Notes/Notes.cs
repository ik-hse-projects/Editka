using System.Windows.Forms;

namespace Editka
{
    public class Notes : TabControl
    {
        public Notes(MainForm root)
        {
            TabPages.Add(new TabPage("Помощь")
            {
                Controls =
                {
                    new Label
                    {
                        Dock = DockStyle.Fill,
                        Text = "Чтобы закрывать файлы, надо кликать правой кнопокй мыши."
                    }
                }
            });
            BuildLog = new BuildLog(root);
            TabPages.Add(BuildLog);
        }

        public BuildLog BuildLog { get; }
    }
}