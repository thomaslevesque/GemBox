using System.Text;

namespace GemBox.Text
{
    public static class EncodingExtensions
    {
        public static bool TryGetString(this Encoding encoding, byte[] bytes, out string result)
        {
            return encoding.TryGetString(bytes, 0, bytes.Length, out result);
        }

        public static bool TryGetString(this Encoding encoding, byte[] bytes, int index, int count, out string result)
        {
            result = null;
            try
            {
                var decoder = encoding.GetDecoder();
                decoder.Fallback = DecoderFallback.ExceptionFallback;
                int charCount = decoder.GetCharCount(bytes, index, count);
                char[] chars = new char[charCount];
                decoder.GetChars(bytes, index, count, chars, 0);
                result = new string(chars);
                return true;
            }
            catch (DecoderFallbackException)
            {
                return false;
            }
        }

        public static bool IsValid(this Encoding encoding, byte[] bytes)
        {
            return encoding.IsValid(bytes, 0, bytes.Length);
        }

        public static bool IsValid(this Encoding encoding, byte[] bytes, int index, int count)
        {
            try
            {
                var decoder = encoding.GetDecoder();
                decoder.Fallback = DecoderFallback.ExceptionFallback;
                decoder.GetCharCount(bytes, index, count);
                return true;
            }
            catch (DecoderFallbackException)
            {
                return false;
            }
        }
    }
}
