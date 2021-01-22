using System;
using System.IO;
using System.Windows.Forms;

namespace Editka.Files
{
    public class Plain : OpenedFile
    {
        public Plain(string path) : base(path)
        {
        }

        public Plain()
        {
        }

        protected override RichTextBoxStreamType StreamType => RichTextBoxStreamType.UnicodePlainText;
    }
}