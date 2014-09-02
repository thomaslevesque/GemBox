using System;
using System.IO;
using System.Linq;

namespace GemBox.IO
{
    public static class TextWriterExtensions
    {
        public static TextWriter Tee(this TextWriter writer, TextWriter other, params TextWriter[] others)
        {
            var outputWriters = new[] { writer, other }.Concat(others).ToArray();
            if (outputWriters.Any(w => w == null))
                throw new ArgumentNullException();

            return new TeeTextWriter(outputWriters);
        }
    }
}
