using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using Microsoft.Win32;

namespace lab
{
    public class FileHandler
    {
        public static void OpenFile(RichTextBox richTextBox)
        {
            OpenFileDialog dlg = new OpenFileDialog { Filter = "Document files (*.doc)|*.doc" };
            var result = dlg.ShowDialog();
            if (result == true)
            {
                TextRange textRange = new TextRange(richTextBox.Document.ContentStart, richTextBox.Document.ContentEnd);
                SaveLoad.LoadDocumentFromFile(dlg.FileName, textRange);
            }
        }

        public static void SaveFile(RichTextBox richTextBox, Window window, ref bool isSaved)
        {
            SaveFileDialog savefile = new SaveFileDialog { FileName = DateTime.Now.ToString("yyyyMMdd_HHmmss"), Filter = "Document files (*.doc)|*.doc" };
            if (savefile.ShowDialog() == true)
            {
                TextRange textRange = new TextRange(richTextBox.Document.ContentStart, richTextBox.Document.ContentEnd);
                var result = SaveLoad.SaveDocumentToFile(savefile.FileName, textRange);
                window.Title = Utilily.GetFileNameFromPath(result.fileName);
                isSaved = result.isSaved;
            }
        }
    }
}
