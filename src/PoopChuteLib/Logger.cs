using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace PoopChuteLib
{
    public class Logger
    {
        private StreamWriter writer;

        public Logger(string logFilePath)
        {
            writer = new StreamWriter(File.Open(logFilePath, FileMode.OpenOrCreate, FileAccess.ReadWrite));
            writer.AutoFlush = true;
        }

        public void WriteLine(object o)
        {
            string s = $"[{DateTime.Now}] : {o?.ToString()}";
            Console.WriteLine(s);
            writer.WriteLine(s);
        }

        public Task WriteLineAsync(object o)
        {
            string s = $"[{DateTime.Now}] : {o?.ToString()}";
            Console.WriteLine(s);
            return writer.WriteLineAsync(s);
        }
    }
}
