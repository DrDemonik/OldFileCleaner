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
        static public bool Run(string path)
        {
            string text;
            using (FileStream file = new FileStream(path, FileMode.Open))
            {
                using (StreamReader reader = new StreamReader(file, Encoding.Default))
                {
                    //text = reader.ReadToEndAsync
                    text = reader.ReadToEnd();
                }
                return text.Contains("A\nB\nC");
            }
        }
    }
}
