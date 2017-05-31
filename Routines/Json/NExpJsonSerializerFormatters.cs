using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Vse.Routines.Json
{
    public static class NExpJsonSerializerFormatters
    {
        /// <summary>
        /// ISO 8601 without "second fractions"
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static bool SerializeToIso8601WithSecUtc(StringBuilder sb, DateTime dateTime)
        {
            var notEmpty = SerializeToIso8601WithSec(sb, dateTime.ToUniversalTime());
            return notEmpty;
        }

        /// <summary>
        /// ISO 8601 with "second fractions", there with milliseconds
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static bool SerializeToIso8601WithMsUtc(StringBuilder sb, DateTime dateTime)
        {
            var notEmpty = SerializeToIso8601WithMs(sb, dateTime.ToUniversalTime());
            return notEmpty;
        }

        /// <summary>
        /// ISO 8601 without "second fractions"
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static bool SerializeToIso8601WithSec(StringBuilder sb, DateTime dateTime)
        {
            sb.Append('"').Append(dateTime.ToString("yyyy-MM-ddTHH:mm:ssK")).Append('"');
            return true;
        }

        /// <summary>
        /// ISO 8601 with "second fractions", there with milliseconds
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static bool SerializeToIso8601WithMs(StringBuilder sb, DateTime dateTime)
        {
            sb.Append('"').Append(dateTime.ToString("yyyy-MM-ddTHH:mm:ss.fffK")).Append('"');
            return true;
        }

        // <summary>
        /// ISO 8601 with "second fractions", there with milliseconds
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static bool SerializeToIso8601WithNs(StringBuilder sb, DateTime dateTime)
        {
            sb.Append('"').Append(dateTime.ToString("yyyy-MM-ddTHH:mm:ss.fffffffK")).Append('"');
            return true;
        }

        /// <summary>
        /// Epoch time like Date(1494012786765+0300)
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static bool SerializeToUnixTimeMs(StringBuilder sb, DateTime dateTime)
        {
            var dateTimeOffset = new DateTimeOffset(dateTime);
            var unixTime = dateTimeOffset.ToUnixTimeMilliseconds();
            var offset = dateTimeOffset.Offset;
            var hours = offset.Hours;
            var minutes = offset.Minutes;
            sb.Append(@"""\/Date(");
            sb.Append(unixTime);
            if (dateTime.Kind != DateTimeKind.Utc)
            {
                sb.Append(offset > TimeSpan.Zero ? "+" : "-");
                if (hours < 10)
                    sb.Append("0");
                sb.Append(hours.ToString(CultureInfo.InvariantCulture));
                if (minutes < 10)
                    sb.Append("0");
                sb.Append(minutes.ToString(CultureInfo.InvariantCulture));
            }
            sb.Append(@")\/""");
            return true;
        }

        /// <summary>
        /// Epoch time like Date(1494012786765)
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static bool SerializeToUnixTimeMsUtc(StringBuilder sb, DateTime dateTime)
        {
            var notEmpty = SerializeToUnixTimeMs(sb, dateTime.ToUniversalTime());
            return notEmpty;
        }

        public static bool SerializeBytesToJsonArray(StringBuilder sb, byte[] bytes)
        {
            sb.Append('[');
            var length = bytes.Length;
            for (var i = 0; i < length; i++) // better the foreach because we need to reuse length
            {
                var chars = bytes[i].ToString();
                sb.Append(chars).Append(',');
            }
            if (length > 0)
                sb.Length--;
            sb.Append(']');
            return true;
        }

        public static bool SerializeBase64(StringBuilder sb, byte[] bytes)
        {
            sb.Append('"').Append(Convert.ToBase64String(bytes)).Append('"');
            return true; 
        }

        

        // This will be always a little bit slower than Convert.ToBase64String because of cost of "safe array indexers"
        // but I left it there because it can be used in future projects on stream (after some modifications) 
        static char[] base64chars = Encoding.ASCII.GetChars(Encoding.ASCII.GetBytes("ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/"));
        public static bool SerializeBase64Custom(StringBuilder sb, byte[] bytes)
        {
            unchecked
            {
                
                var size = bytes.Length;
                int wholeTripletsReminder = (size % 3);
                int wholeTripletsSize = size - wholeTripletsReminder;
                int bufferSize = (4 * ((size + 2) / 3))+2 /* for quotation marks */;
                var buffer = new char[bufferSize];
                buffer[0] = '"';
                var bufferPosition = 1;
                UInt32 octet_a, octet_b, octet_c, triple;
                int i = 0;
                while (i < wholeTripletsSize)
                {
                    // NOTE: migratio to BitConverter make it even slower because (I guess) of boxing|unboxing
                    // Int32 dword = BitConverter.ToInt32(bytes, i);
                    // byte[] octets = BitConverter.GetBytes(dword);
                    //i = i + 3;
                    //octet_a = (UInt32)octets[0];
                    //octet_b = (UInt32)octets[1];
                    //octet_c = (UInt32)octets[2];

                    octet_a = bytes[i++];
                    octet_b = bytes[i++];
                    octet_c = bytes[i++];

                    triple = (octet_a << 0x10) + (octet_b << 0x08) + octet_c;

                    buffer[bufferPosition++] = base64chars[(triple >> 3 * 6) & 0x3F];
                    buffer[bufferPosition++] = base64chars[(triple >> 2 * 6) & 0x3F];
                    buffer[bufferPosition++] = base64chars[(triple >> 1 * 6) & 0x3F];
                    buffer[bufferPosition++] = base64chars[(triple >> 0 * 6) & 0x3F];
                }

                switch (wholeTripletsReminder)
                {
                    case 0:
                        break;
                    case 1:
                        octet_a = bytes[i++];
                        triple = (octet_a << 0x10); 
                        buffer[bufferPosition++] = base64chars[(triple >> 3 * 6) & 0x3F];
                        buffer[bufferPosition++] = base64chars[(triple >> 2 * 6) & 0x3F];
                        buffer[bufferPosition++] = '=';
                        buffer[bufferPosition++] = '=';
                        break;
                    case 2:
                        octet_a = bytes[i++];
                        octet_b = bytes[i++];
                        triple = (octet_a << 0x10) + (octet_b << 0x08);
                        buffer[bufferPosition++] = base64chars[(triple >> 3 * 6) & 0x3F];
                        buffer[bufferPosition++] = base64chars[(triple >> 2 * 6) & 0x3F];
                        buffer[bufferPosition++] = base64chars[(triple >> 1 * 6) & 0x3F];
                        buffer[bufferPosition++] = '=';
                        break;
                }
                buffer[bufferPosition] = '"';
                sb.Append(buffer);
            }
            return true;
        }

        public static bool SerializeJson(StringBuilder sb, byte[] bytes)
        {
            sb.Append('[');
            var e = bytes.GetEnumerator();
            bool moveNext = e.MoveNext();
            while (moveNext)
            {
                sb.Append(e.Current);
                moveNext = e.MoveNext();
                if (moveNext)
                    sb.Append(",");
            }
            sb.Append(']');
            return true;
        }

        
    }
}
