using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using FastColoredTextBoxNS;

namespace Editka
{
    public enum FileKind
    {
        Plain,
        Rtf,
        CSharp
    }
    
    public class TextboxWrapper
    {
        private readonly RichTextBox? rich;
        private readonly FastColoredTextBox? fast;

        public Control Control => (rich as Control ?? fast)!;
        public event EventHandler? TextChanged;

        public TextboxWrapper(FileKind streamType)
        {
            switch (streamType)
            {
                case FileKind.Plain:
                    fast = new FastColoredTextBox();
                    break;
                case FileKind.Rtf:
                    rich = new RichTextBox();
                    break;
                case FileKind.CSharp:
                    fast = new FastColoredTextBox();
                    fast.Language = Language.CSharp;
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
                rich.LoadFile(file, RichTextBoxStreamType.RichText);
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
                rich.SaveFile(file, RichTextBoxStreamType.RichText);
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