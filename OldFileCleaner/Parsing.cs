using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OldFileCleaner
{
    static public class Parsing
    {
        /// <summary>
        /// Парсинг и сферка содержимого с шаблоном
        /// </summary>
        static public bool Run(string path, string template)
        {
            string text;
            using (FileStream file = new FileStream(path, FileMode.Open))
            {
                using (StreamReader reader = new StreamReader(file, Encoding.Default))
                {
                    text = reader.ReadToEnd();
                }
                return text.Contains(template);
            }
        }
    }
}
