using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace GemBox.IO
{
    public static class StreamExtensions
    {
        private const int DefaultBufferSize = 0x14000;

        public static long CopyToFile(this Stream stream, string path, int bufferSize = DefaultBufferSize)
        {
            if (stream == null) throw new ArgumentNullException("stream");
            if (path == null) throw new ArgumentNullException("path");
            using (var fileStream = File.OpenWrite(path))
            {
                fileStream.SetLength(0);
                stream.CopyTo(fileStream, bufferSize);
                return fileStream.Length;
            }
        }

        public static byte[] ReadAllBytes(this Stream stream)
        {
            if (stream == null) throw new ArgumentNullException("stream");
            using (var tmp = new MemoryStream())
            {
                stream.CopyTo(tmp);
                return tmp.ToArray();
            }
        }

        public static byte[] ReadBytes(this Stream stream, int count)
        {
            if (stream == null) throw new ArgumentNullException("stream");
            if (count < 0)
                throw new ArgumentOutOfRangeException("count");
            byte[] buffer = new byte[count];
            int nRead, totalRead = 0;
            while (totalRead < count && (nRead = stream.Read(buffer, totalRead, count - totalRead)) != 0)
            {
                totalRead += nRead;
            }

            if (count > totalRead)
            {
                Array.Resize(ref buffer, totalRead);
            }
            return buffer;
        }

        #if false
        public static Stream SuppressClose(this Stream stream)
        {
            if (stream == null) throw new ArgumentNullException("stream");
            return new NonClosingStreamWrapper(stream);
        }
        #endif

        public static Stream Tee(this Stream stream, Stream other, params Stream[] others)
        {
            var outputStreams = new[] { stream, other }.Concat(others).ToArray();
            if (outputStreams.Any(w => w == null))
                throw new ArgumentNullException();
            if (outputStreams.Any(w => !w.CanWrite))
                throw new ArgumentException("All output streams must be writable");

            return new TeeStream(outputStreams);
        }

        public static IEnumerable<byte> Bytes(this Stream stream, int bufferSize = DefaultBufferSize)
        {
            if (stream == null) throw new ArgumentNullException("stream");
            byte[] buffer = new byte[bufferSize];
            int nRead;
            while ((nRead = stream.Read(buffer, 0, bufferSize)) > 0)
            {
                for (int i = 0; i < nRead; i++)
                {
                    yield return buffer[i];
                }
            }
        }

        public static IEnumerable<byte[]> Blocks(this Stream stream, int blockSize)
        {
            if (stream == null) throw new ArgumentNullException("stream");
            byte[] buffer = new byte[blockSize];
            int nRead;
            while ((nRead = stream.Read(buffer, 0, blockSize)) > 0)
            {
                byte[] buf2 = new byte[nRead];
                Array.Copy(buffer, buf2, nRead);
                yield return buf2;
            }
        }
    }
}
