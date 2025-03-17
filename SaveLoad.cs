using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Documents;

namespace lab
{
    public static class SaveLoad
    {
        public static void LoadDocumentFromFile(string fileName, TextRange textRange)
        {
                FileStream file = new FileStream(fileName, FileMode.Open);
                textRange.Load(file, System.Windows.DataFormats.Rtf);
            
        }

        public static (string fileName, bool isSaved) SaveDocumentToFile(string fileName, TextRange textRange)
        {
            bool isSaved = false;
            using (FileStream fileStream = new FileStream(fileName, FileMode.Create))
            {
                textRange.Save(fileStream, System.Windows.DataFormats.Rtf);
                isSaved = true;
            }

            return (fileName, isSaved);
        }
    }
}
