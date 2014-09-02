using System;
using System.Collections.Generic;
using System.IO;

namespace GemBox.IO
{
    public static class TextReaderExtensions
    {
        public static IEnumerable<string> Lines(this TextReader reader)
        {
            if (reader == null) throw new ArgumentNullException("reader");
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                yield return line;
            }
        }

        public static IEnumerable<char> Chars(this TextReader reader)
        {
            if (reader == null) throw new ArgumentNullException("reader");
            int c;
            while ((c = reader.Read()) != -1)
            {
                yield return (char)c;
            }
        }
    }
}
