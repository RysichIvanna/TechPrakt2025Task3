using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;

namespace lab
{
    internal static class Utilily
    {
        public static string GetFileNameFromPath(string path)
        {
            string[] parts = path.Split('\\');

            return parts[parts.Length - 1];
        }
        
        public static string GetFileSize(System.Windows.Controls.RichTextBox richTextBox)
        {
            TextRange textRange = new TextRange(richTextBox.Document.ContentStart, richTextBox.Document.ContentEnd);

            string text = textRange.Text;

            int byteCount = System.Text.Encoding.UTF8.GetByteCount(text);

            string sizeString;
            double sizeInBytes = byteCount;

            if (sizeInBytes < 1024)
            {
                sizeString = $"{sizeInBytes} bytes";
            }
            else if (sizeInBytes < 1024 * 1024)
            {
                sizeInBytes /= 1024;
                sizeString = $"{Math.Round(sizeInBytes, 2)} KB";
            }
            else if (sizeInBytes < 1024 * 1024 * 1024)
            {
                sizeInBytes /= (1024 * 1024);
                sizeString = $"{Math.Round(sizeInBytes, 2)} MB";
            }
            else
            {
                sizeInBytes /= (1024 * 1024 * 1024);
                sizeString = $"{Math.Round(sizeInBytes, 2)} GB";
            }

            return sizeString;
        }
    }
}
