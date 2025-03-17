using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Forms;

namespace lab
{
    internal static class TextChangeHandler
    {
        public static List<double> FontSizes = new List<double>
        {
            8, 10, 12, 14, 16, 18, 20, 22, 24, 26, 28, 36, 48, 72
        };

        public static (TextPointer startPos, TextPointer endPos) FindText(string searchText, TextPointer start, TextPointer end)
        {
            TextRange textRange = new TextRange(start, end);
            string documentText = textRange.Text;

            int index = documentText.IndexOf(searchText, StringComparison.OrdinalIgnoreCase);
            if (index != -1)
            {
                TextPointer startPos = start.GetPositionAtOffset(index);
                TextPointer endPos = startPos.GetPositionAtOffset(searchText.Length);
                return (startPos, endPos);
            }
            return (null, null);
        }

        public static bool ReplaceText(string searchText, string replaceText, System.Windows.Controls.RichTextBox rich)
        {
            TextPointer start = rich.Document.ContentStart;
            TextPointer end = rich.Document.ContentEnd;

            if (start == null || start.CompareTo(end) >= 0)
            {
                return false;
            }

            while (start != null && start.CompareTo(end) < 0)
            {
                string textRun = start.GetTextInRun(LogicalDirection.Forward);

                int index = textRun.IndexOf(searchText, StringComparison.OrdinalIgnoreCase);

                if (index != -1)
                {
                    TextPointer startPos = start.GetPositionAtOffset(index);
                    TextPointer endPos = startPos.GetPositionAtOffset(searchText.Length);

                    TextRange selectedText = new TextRange(startPos, endPos);
                    selectedText.Text = replaceText;

                    break;
                }

                start = start.GetNextContextPosition(LogicalDirection.Forward);
            }

            return true;
        }

        public static bool SortText(TextPointer start, TextPointer end, System.Windows.Controls.RichTextBox rich)
        {
            string text = new TextRange(start, end).Text;

            string[] lines = text.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);

            Array.Sort(lines);

            string sortedText = string.Join(Environment.NewLine, lines);

            rich.Document.Blocks.Clear();
            rich.Document.Blocks.Add(new Paragraph(new Run(sortedText)));

            return true;
        }

        public static void AddList(int count, System.Windows.Controls.RichTextBox richTextBox)
        {
            for (int i = 0; i < count; i++)
            {
                richTextBox.Document.Blocks.Add(new Paragraph(new Run((i + 1) + ": ")));
            }
        }
    }
}
