using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using FastColoredTextBoxNS;
using Control = Editka.Compat.Control;

namespace Editka
{
    /// <summary>
    /// Поддерживаемые типы файлов.
    /// </summary>
    public enum FileKind
    {
        Plain,
        Rtf,
        CSharp
    }

    /// <summary>
    /// Объединяет в себе RichTextBox и FastColoredTextBox под единым интерфейсом.
    /// </summary>
    public class TextboxWrapper
    {
        private readonly FastColoredTextBox? fast;
        private readonly RichTextBox? rich;

        /// <summary>
        /// Создаёт текстбокс, соответсвующий типу файла.
        /// </summary>
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
        }

        /// <summary>
        /// Текстбокс внутри.
        /// </summary>
        public Control Control => (rich as System.Windows.Forms.Control ?? fast)!;
        
        /// <summary>
        /// Загружает файл из потока в текстбокс.
        /// </summary>
        public void LoadFile(FileStream file)
        {
            if (rich != null)
            {
                rich.LoadFile(file, RichTextBoxStreamType.RichText);
                return;
            }

            if (fast != null)
            {
                StreamReader reader = new StreamReader(file);
                fast.Text = reader.ReadToEnd();
            }
        }

        /// <summary>
        /// Сохраняет текст из текстбокса в файл.
        /// </summary>
        public void SaveFile(FileStream file)
        {
            if (rich != null)
            {
                rich.SaveFile(file, RichTextBoxStreamType.RichText);
                return;
            }

            if (fast != null)
            {
                StreamWriter writer = new StreamWriter(file);
                writer.Write(fast.Text);
            }
        }

        /// <summary>
        /// Обновляет стиль для выделенного текста.
        /// </summary>
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

        // Далее следует совершенно тривиальная реализация функций, общих для RichTextBox и FastColoredTextBox.

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
    }
}