using System.IO;
using System.Linq;
using System.Text;

namespace GemBox.IO
{
    internal class TeeTextWriter : TextWriter
    {
        private readonly TextWriter[] _outputWriters;

        public TeeTextWriter(params TextWriter[] outputWriters)
        {
            _outputWriters = outputWriters;
        }

        #region Overrides of TextWriter

        public override Encoding Encoding
        {
            get { return _outputWriters.Select(w => w.Encoding).FirstOrDefault() ?? Encoding.UTF8; }
        }

        public override void Write(char value)
        {
            foreach (var writer in _outputWriters)
                writer.Write(value);
        }

        public override void Flush()
        {
            base.Flush();
            foreach (var writer in _outputWriters)
                writer.Flush();
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            foreach (var writer in _outputWriters)
                writer.Dispose();
        }

        #endregion
    }
}
