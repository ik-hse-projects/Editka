using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using FastColoredTextBoxNS;

namespace Editka
{
    public class TextboxWrapper
    {
        private readonly RichTextBox? rich;
        private readonly FastColoredTextBox? fast;
        private readonly RichTextBoxStreamType streamType;

        public Control Control => (rich as Control ?? fast)!;
        public event EventHandler? TextChanged;

        public TextboxWrapper(RichTextBoxStreamType streamType)
        {
            this.streamType = streamType;
            switch (streamType)
            {
                case RichTextBoxStreamType.RichNoOleObjs:
                case RichTextBoxStreamType.RichText:
                    rich = new RichTextBox();
                    break;
                case RichTextBoxStreamType.PlainText:
                case RichTextBoxStreamType.UnicodePlainText:
                case RichTextBoxStreamType.TextTextOleObjs:
                    fast = new FastColoredTextBox();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(streamType), streamType, null);
            }

            Control.TextChanged += (sender, args) => TextChanged?.Invoke(sender, args);
        }

        public void Paste()
        {
            fast?.Paste();
            rich?.Paste();
        }

        public void Copy()
        {
            fast?.Copy();
            rich?.Copy();
        }

        public void Cut()
        {
            fast?.Cut();
            rich?.Cut();
        }

        public void SelectAll()
        {
            fast?.SelectAll();
            rich?.SelectAll();
        }

        public void Undo()
        {
            fast?.Undo();
            rich?.Undo();
        }

        public void Redo()
        {
            fast?.Redo();
            rich?.Redo();
        }

        public void Clear()
        {
            fast?.Clear();
            rich?.Clear();
        }

        public void LoadFile(FileStream file)
        {
            if (rich != null)
            {
                rich.LoadFile(file, streamType);
                return;
            }

            if (fast != null)
            {
                using StreamReader reader = new StreamReader(file);
                fast.Text = reader.ReadToEnd();
            }
        }

        public void SaveFile(FileStream file)
        {
            if (rich != null)
            {
                rich.SaveFile(file, streamType);
                return;
            }

            if (fast != null)
            {
                using StreamWriter writer = new StreamWriter(file);
                writer.Write(fast.Text);
            }
        }

        public void UpdateStyle(FontStyle mask)
        {
            if (rich == null)
            {
                MessageBox.Show("Форматирование возможно только для rtf файлов.");
                return;
            }

            var style = rich.SelectionFont.Style ^ mask;
            rich.SelectionFont = new Font(rich.SelectionFont, style);
        }
    }
}