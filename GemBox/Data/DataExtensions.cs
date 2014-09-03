using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Reflection;

namespace GemBox.Data
{
    public static class DataExtensions
    {
        #region Extension methods for IDataRecord

        public static bool GetBoolean(this IDataRecord record, string name)
        {
            if (record == null) throw new ArgumentNullException("record");
            if (name == null) throw new ArgumentNullException("name");
            int i = record.GetOrdinal(name);
            return record.GetBoolean(i);
        }

        public static bool GetBooleanOrDefault(this IDataRecord record, string name, bool defaultValue)
        {
            if (record == null) throw new ArgumentNullException("record");
            if (name == null) throw new ArgumentNullException("name");
            int i = record.GetOrdinal(name);
            return record.IsDBNull(i) ? defaultValue : record.GetBoolean(i);
        }

        public static byte GetByte(this IDataRecord record, string name)
        {
            if (record == null) throw new ArgumentNullException("record");
            if (name == null) throw new ArgumentNullException("name");
            int i = record.GetOrdinal(name);
            return record.GetByte(i);
        }

        public static byte GetByteOrDefault(this IDataRecord record, string name, byte defaultValue)
        {
            if (record == null) throw new ArgumentNullException("record");
            if (name == null) throw new ArgumentNullException("name");
            int i = record.GetOrdinal(name);
            return record.IsDBNull(i) ? defaultValue : record.GetByte(i);
        }

        public static char GetChar(this IDataRecord record, string name)
        {
            if (record == null) throw new ArgumentNullException("record");
            if (name == null) throw new ArgumentNullException("name");
            int i = record.GetOrdinal(name);
            return record.GetChar(i);
        }

        public static char GetCharOrDefault(this IDataRecord record, string name, char defaultValue)
        {
            if (record == null) throw new ArgumentNullException("record");
            if (name == null) throw new ArgumentNullException("name");
            int i = record.GetOrdinal(name);
            return record.IsDBNull(i) ? defaultValue : record.GetChar(i);
        }

        public static DateTime GetDateTime(this IDataRecord record, string name)
        {
            if (record == null) throw new ArgumentNullException("record");
            if (name == null) throw new ArgumentNullException("name");
            int i = record.GetOrdinal(name);
            return record.GetDateTime(i);
        }

        public static DateTime GetDateTimeOrDefault(this IDataRecord record, string name, DateTime defaultValue)
        {
            if (record == null) throw new ArgumentNullException("record");
            if (name == null) throw new ArgumentNullException("name");
            int i = record.GetOrdinal(name);
            return record.IsDBNull(i) ? defaultValue : record.GetDateTime(i);
        }

        public static decimal GetDecimal(this IDataRecord record, string name)
        {
            if (record == null) throw new ArgumentNullException("record");
            if (name == null) throw new ArgumentNullException("name");
            int i = record.GetOrdinal(name);
            return record.GetDecimal(i);
        }

        public static decimal GetDecimalOrDefault(this IDataRecord record, string name, decimal defaultValue)
        {
            if (record == null) throw new ArgumentNullException("record");
            if (name == null) throw new ArgumentNullException("name");
            int i = record.GetOrdinal(name);
            return record.IsDBNull(i) ? defaultValue : record.GetDecimal(i);
        }

        public static double GetDouble(this IDataRecord record, string name)
        {
            if (record == null) throw new ArgumentNullException("record");
            if (name == null) throw new ArgumentNullException("name");
            int i = record.GetOrdinal(name);
            return record.GetDouble(i);
        }

        public static double GetDoubleOrDefault(this IDataRecord record, string name, double defaultValue)
        {
            if (record == null) throw new ArgumentNullException("record");
            if (name == null) throw new ArgumentNullException("name");
            int i = record.GetOrdinal(name);
            return record.IsDBNull(i) ? defaultValue : record.GetDouble(i);
        }

        public static float GetFloat(this IDataRecord record, string name)
        {
            if (record == null) throw new ArgumentNullException("record");
            if (name == null) throw new ArgumentNullException("name");
            int i = record.GetOrdinal(name);
            return record.GetFloat(i);
        }

        public static float GetFloatOrDefault(this IDataRecord record, string name, float defaultValue)
        {
            if (record == null) throw new ArgumentNullException("record");
            if (name == null) throw new ArgumentNullException("name");
            int i = record.GetOrdinal(name);
            return record.IsDBNull(i) ? defaultValue : record.GetFloat(i);
        }

        public static Guid GetGuid(this IDataRecord record, string name)
        {
            if (record == null) throw new ArgumentNullException("record");
            if (name == null) throw new ArgumentNullException("name");
            int i = record.GetOrdinal(name);
            return record.GetGuid(i);
        }

        public static Guid GetGuidOrDefault(this IDataRecord record, string name, Guid defaultValue)
        {
            if (record == null) throw new ArgumentNullException("record");
            if (name == null) throw new ArgumentNullException("name");
            int i = record.GetOrdinal(name);
            return record.IsDBNull(i) ? defaultValue : record.GetGuid(i);
        }

        public static short GetInt16(this IDataRecord record, string name)
        {
            if (record == null) throw new ArgumentNullException("record");
            if (name == null) throw new ArgumentNullException("name");
            int i = record.GetOrdinal(name);
            return record.GetInt16(i);
        }

        public static short GetInt16OrDefault(this IDataRecord record, string name, short defaultValue)
        {
            if (record == null) throw new ArgumentNullException("record");
            if (name == null) throw new ArgumentNullException("name");
            int i = record.GetOrdinal(name);
            return record.IsDBNull(i) ? defaultValue : record.GetInt16(i);
        }

        public static int GetInt32(this IDataRecord record, string name)
        {
            if (record == null) throw new ArgumentNullException("record");
            if (name == null) throw new ArgumentNullException("name");
            int i = record.GetOrdinal(name);
            return record.GetInt32(i);
        }

        public static int GetInt32OrDefault(this IDataRecord record, string name, int defaultValue)
        {
            if (record == null) throw new ArgumentNullException("record");
            if (name == null) throw new ArgumentNullException("name");
            int i = record.GetOrdinal(name);
            return record.IsDBNull(i) ? defaultValue : record.GetInt32(i);
        }

        public static long GetInt64(this IDataRecord record, string name)
        {
            if (record == null) throw new ArgumentNullException("record");
            if (name == null) throw new ArgumentNullException("name");
            int i = record.GetOrdinal(name);
            return record.GetInt64(i);
        }

        public static long GetInt64OrDefault(this IDataRecord record, string name, long defaultValue)
        {
            if (record == null) throw new ArgumentNullException("record");
            if (name == null) throw new ArgumentNullException("name");
            int i = record.GetOrdinal(name);
            return record.IsDBNull(i) ? defaultValue : record.GetInt64(i);
        }

        public static string GetString(this IDataRecord record, string name)
        {
            if (record == null) throw new ArgumentNullException("record");
            if (name == null) throw new ArgumentNullException("name");
            int i = record.GetOrdinal(name);
            return record.GetString(i);
        }

        public static string GetStringOrDefault(this IDataRecord record, string name, string defaultValue)
        {
            if (record == null) throw new ArgumentNullException("record");
            if (name == null) throw new ArgumentNullException("name");
            int i = record.GetOrdinal(name);
            return record.IsDBNull(i) ? defaultValue : record.GetString(i);
        }

        public static object GetValue(this IDataRecord record, string name)
        {
            if (record == null) throw new ArgumentNullException("record");
            if (name == null) throw new ArgumentNullException("name");
            int i = record.GetOrdinal(name);
            return record.GetValue(i);
        }

        public static object GetValueOrDefault(this IDataRecord record, string name, object defaultValue)
        {
            if (record == null) throw new ArgumentNullException("record");
            if (name == null) throw new ArgumentNullException("name");
            int i = record.GetOrdinal(name);
            return record.IsDBNull(i) ? defaultValue : record.GetValue(i);
        }

        public static long GetBytes(this IDataRecord record, string name, long dataOffset, byte[] buffer,
            int bufferOffset, int length)
        {
            if (record == null) throw new ArgumentNullException("record");
            if (name == null) throw new ArgumentNullException("name");
            int i = record.GetOrdinal(name);
            return record.GetBytes(i, dataOffset, buffer, bufferOffset, length);
        }

        public static long GetChars(this IDataRecord record, string name, long dataOffset, char[] buffer,
            int bufferOffset, int length)
        {
            if (record == null) throw new ArgumentNullException("record");
            if (name == null) throw new ArgumentNullException("name");
            int i = record.GetOrdinal(name);
            return record.GetChars(i, dataOffset, buffer, bufferOffset, length);
        }

        public static IDataReader GetData(this IDataRecord record, string name)
        {
            if (record == null) throw new ArgumentNullException("record");
            if (name == null) throw new ArgumentNullException("name");
            int i = record.GetOrdinal(name);
            return record.GetData(i);
        }

        // ReSharper disable once InconsistentNaming
        public static bool IsDBNull(this IDataRecord record, string name)
        {
            if (record == null) throw new ArgumentNullException("record");
            if (name == null) throw new ArgumentNullException("name");
            int i = record.GetOrdinal(name);
            return record.IsDBNull(i);
        }

        public static string GetDataTypeName(this IDataRecord record, string name)
        {
            if (record == null) throw new ArgumentNullException("record");
            if (name == null) throw new ArgumentNullException("name");
            int i = record.GetOrdinal(name);
            return record.GetDataTypeName(i);
        }

        public static Type GetFieldType(this IDataRecord record, string name)
        {
            if (record == null) throw new ArgumentNullException("record");
            if (name == null) throw new ArgumentNullException("name");
            int i = record.GetOrdinal(name);
            return record.GetFieldType(i);
        }

        public static T Field<T>(this IDataRecord record, int ordinal, bool tryConvert)
        {
            if (record == null) throw new ArgumentNullException("record");
            if (tryConvert)
                return ConvertT<T>.Convert(record[ordinal]);
            return UnboxT<T>.Unbox(record[ordinal]);
        }

        public static T Field<T>(this IDataRecord record, int ordinal)
        {
            if (record == null) throw new ArgumentNullException("record");
            return record.Field<T>(ordinal, false);
        }

        public static T Field<T>(this IDataRecord record, string name, bool tryConvert)
        {
            if (record == null) throw new ArgumentNullException("record");
            if (name == null) throw new ArgumentNullException("name");
            int i = record.GetOrdinal(name);
            return record.Field<T>(i, tryConvert);
        }

        public static T Field<T>(this IDataRecord record, string name)
        {
            if (record == null) throw new ArgumentNullException("record");
            if (name == null) throw new ArgumentNullException("name");
            int i = record.GetOrdinal(name);
            return record.Field<T>(i, false);
        }

        #region Utility classes UnboxT<T> and ConvertT<T> to convert field values

        // Taken from System.Data.DataRowExtensions
        private static class UnboxT<T>
        {
            internal static readonly Converter<object, T> Unbox;

            static UnboxT()
            {
                Unbox = Create(typeof (T));
            }

            private static Converter<object, T> Create(Type type)
            {
                if (!type.IsValueType)
                {
                    return ReferenceField;
                }
                if ((type.IsGenericType && !type.IsGenericTypeDefinition) &&
                    (typeof (Nullable<>) == type.GetGenericTypeDefinition()))
                {
                    return (Converter<object, T>) Delegate.CreateDelegate(
                        typeof (Converter<object, T>),
                        typeof (UnboxT<T>)
                            .GetMethod(
                                "NullableField",
                                BindingFlags.NonPublic | BindingFlags.Static)
                            .MakeGenericMethod(type.GetGenericArguments()));
                }
                return ValueField;
            }

            private static T ReferenceField(object value)
            {
                if (DBNull.Value != value)
                {
                    return (T) value;
                }
                return default(T);
            }

            private static T ValueField(object value)
            {
                if (DBNull.Value == value)
                {
                    throw new InvalidCastException(string.Format("Invalid cast from '{0}' to '{1}'.", typeof (DBNull), typeof (T)));
                }
                return (T) value;
            }
        }

        private static class ConvertT<T>
        {
            internal static readonly Converter<object, T> Convert;

            static ConvertT()
            {
                Convert = Create(typeof (T));
            }

            private static Converter<object, T> Create(Type type)
            {
                if (!type.IsValueType)
                {
                    return ReferenceField;
                }
                if (type.IsGenericType &&
                    !type.IsGenericTypeDefinition &&
                    typeof (Nullable<>) == type.GetGenericTypeDefinition())
                {
                    return
                        (Converter<object, T>)
                            Delegate.CreateDelegate(typeof (Converter<object, T>),
                                typeof (ConvertT<T>).GetMethod("NullableField",
                                    BindingFlags.NonPublic | BindingFlags.Static)
                                    .MakeGenericMethod(type.GetGenericArguments()));
                }
                return ValueField;
            }

            private static T ReferenceField(object value)
            {
                if (null != value && DBNull.Value != value)
                {
                    return (T) System.Convert.ChangeType(value, typeof (T));
                }
                return default(T);
            }

            private static T ValueField(object value)
            {
                if (DBNull.Value == value)
                {
                    throw new InvalidCastException(string.Format("Invalid cast from '{0}' to '{1}'.", typeof(DBNull), typeof (T)));
                }
                if (null == value)
                {
                    throw new InvalidCastException("Can't cast null to a value type.");
                }
                return (T) System.Convert.ChangeType(value, typeof (T));
            }
        }

        #endregion

        public static Stream GetStream(this IDataRecord record, int ordinal)
        {
            if (record == null) throw new ArgumentNullException("record");
            return new DbBinaryFieldStream(record, ordinal);
        }

        public static Stream GetStream(this IDataRecord record, string name)
        {
            if (record == null) throw new ArgumentNullException("record");
            if (name == null) throw new ArgumentNullException("name");
            int i = record.GetOrdinal(name);
            return record.GetStream(i);
        }

        #region DbBinaryFieldStream class to read a binary field (BLOB) from a IDataRecord

        private class DbBinaryFieldStream : Stream
        {
            private readonly IDataRecord _record;
            private readonly int _fieldIndex;
            private long _position;
            private long _length = -1;

            public DbBinaryFieldStream(IDataRecord record, int fieldIndex)
            {
                _record = record;
                _fieldIndex = fieldIndex;
            }

            public override bool CanRead
            {
                get { return true; }
            }

            public override bool CanSeek
            {
                get { return true; }
            }

            public override bool CanWrite
            {
                get { return false; }
            }

            public override void Flush()
            {
                throw new NotSupportedException();
            }

            public override long Length
            {
                get
                {
                    if (_length < 0)
                    {
                        _length = _record.GetBytes(_fieldIndex, 0, null, 0, 0);
                    }
                    return _length;
                }
            }

            public override long Position
            {
                get { return _position; }
                set { _position = value; }
            }

            public override int Read(byte[] buffer, int offset, int count)
            {
                long nRead = _record.GetBytes(_fieldIndex, _position, buffer, offset, count);
                _position += nRead;
                return (int) nRead;
            }

            public override long Seek(long offset, SeekOrigin origin)
            {
                long newPosition = _position;
                switch (origin)
                {
                    case SeekOrigin.Begin:
                        newPosition = offset;
                        break;
                    case SeekOrigin.Current:
                        newPosition = _position + offset;
                        break;
                    case SeekOrigin.End:
                        newPosition = this.Length - offset;
                        break;
                }
                if (newPosition < 0)
                    throw new ArgumentOutOfRangeException("offset");
                _position = newPosition;
                return _position;
            }

            public override void SetLength(long value)
            {
                throw new NotSupportedException();
            }

            public override void Write(byte[] buffer, int offset, int count)
            {
                throw new NotSupportedException();
            }
        }

        #endregion

        #endregion

        #region Extension methods for IDataReader

        public static IEnumerable<IDataRecord> AsEnumerable(this IDataReader reader)
        {
            if (reader == null) throw new ArgumentNullException("reader");
            return reader.AsEnumerableImpl();
        }

        private static IEnumerable<IDataRecord> AsEnumerableImpl(this IDataReader reader)
        {
            while (reader.Read())
            {
                yield return reader;
            }
        }

        #endregion
    }
}
