using System;
using System.IO;

namespace GemBox.IO
{
    internal class TeeStream : Stream
    {
        private readonly Stream[] _outputStreams;

        public TeeStream(params Stream[] outputStreams)
        {
            _outputStreams = outputStreams;
        }

        #region Overrides of Stream

        public override void Flush()
        {
            foreach (var stream in _outputStreams)
                stream.Flush();
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotSupportedException();
        }

        public override void SetLength(long value)
        {
            throw new NotSupportedException();
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            throw new NotSupportedException();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            foreach (var stream in _outputStreams)
                stream.Write(buffer, offset, count);
        }

        public override bool CanRead
        {
            get { return false; }
        }

        public override bool CanSeek
        {
            get { return false; }
        }

        public override bool CanWrite
        {
            get { return true; }
        }

        public override long Length
        {
            get { throw new NotSupportedException(); }
        }

        public override long Position
        {
            get { throw new NotSupportedException(); }
            set { throw new NotSupportedException(); }
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            foreach (var stream in _outputStreams)
                stream.Dispose();
        }

        #endregion
    }
}
