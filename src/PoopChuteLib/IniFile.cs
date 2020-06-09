using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;

namespace PoopChuteLib
{
    public class IniFile
    {
        private Dictionary<string, string> iniData;

        public string FileName { get; private set; }

        public string this[string key] => KeyExists(key) ? iniData[key] : null;

        private IniFile(string fileName)
        {
            this.FileName = fileName;
            this.iniData = new Dictionary<string, string>();
        }

        /// <summary>
        /// Read an INI file from disk and parse it.
        /// </summary>
        /// <param name="fileName">The full path to the ini file.</param>
        /// <returns>An ini file object.</returns>
        public static IniFile Parse(string fileName)
        {
            IniFile f = new IniFile(fileName);

            using StreamReader sr = new StreamReader(File.Open(fileName, FileMode.Open, FileAccess.Read));

            while(true)
            {
                string line = sr.ReadLine();

                if (line == null)
                    break;

                if (line.StartsWith('#') || !line.Contains('=') || line.StartsWith('='))
                    continue;

                string key = line.Substring(0, line.IndexOf('='));
                string value = line.IndexOf('=') + 1 >= line.Length ? string.Empty : line.Substring(line.IndexOf('=') + 1);
                f.Add(key, value);
            }

            return f;
        }

        /// <summary>
        /// Appends or overwrites a key/value pair in the internal dictionary.
        /// </summary>
        /// <param name="key">The name of the key. This is case-sensitive.</param>
        /// <param name="value">The value.</param>
        private void Add(string key, string value)
        {
            if (iniData.ContainsKey(key))
                iniData[key] = value;
            else
                iniData.Add(key, value);
        }

        /// <summary>
        /// Returns <see langword="true"/> if the specified key exists. This is case-sensitive.
        /// </summary>
        /// <param name="key">The name of the key. This is case-sensitive.</param>
        /// <returns>Returns true if the key exists.</returns>
        public bool KeyExists(string key) => iniData != null && iniData.ContainsKey(key);

        /// <summary>
        /// Returns true if the specified key exists and its associated value is not null, empty, or whitespace.
        /// </summary>
        /// <param name="key">The name of the key.</param>
        public bool IsSet(string key) => KeyExists(key) && !string.IsNullOrWhiteSpace(this[key]);

        /// <summary>
        /// Throw an exception if the specified key is not set, null, empty, or whitespace.
        /// </summary>
        /// <param name="key">The key</param>
        public void Assert(string key)
        {
            if (!IsSet(key))
                throw new Exception($"The key \"{key}\" was not set in the configuration file {FileName}.");
        }
    }
}
