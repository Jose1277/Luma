using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luma.Entities
{
    internal class Calculos
    {
        public static long DecodeTimestamp(string encodedTimestamp, string encodingType)
        {
            switch (encodingType)
            {
                case "Iso8601":
                    return DateTimeOffset.Parse(encodedTimestamp).Ticks;
                case "Ticks":
                    return long.Parse(encodedTimestamp);
                case "TicksBinary":
                    {
                        byte[] bytes = Convert.FromBase64String(encodedTimestamp);
                        if (BitConverter.IsLittleEndian == false)
                        {
                            Array.Reverse(bytes);
                        }
                        return BitConverter.ToInt64(bytes, 0);
                    }
                case "TicksBinaryBigEndian":
                    {
                        byte[] bytes = Convert.FromBase64String(encodedTimestamp);
                        if (BitConverter.IsLittleEndian)
                        {
                            Array.Reverse(bytes);
                        }
                        return BitConverter.ToInt64(bytes, 0);
                    }
                default:
                    throw new ArgumentException("Unknown encoding type.");
            }
        }
        public static string EncodeTimestamp(long ticks, string encodingType)
        {
            switch (encodingType)
            {
                case "Iso8601":
                    return new DateTimeOffset(ticks, TimeSpan.Zero).ToString("o");

                case "Ticks":
                    return ticks.ToString();

                case "TicksBinary":
                    byte[] bytesLittleEndian = BitConverter.GetBytes(ticks);
                    if (BitConverter.IsLittleEndian == false)
                    {
                        Array.Reverse(bytesLittleEndian);
                    }
                    return Convert.ToBase64String(bytesLittleEndian);

                case "TicksBinaryBigEndian":
                    byte[] bytesBigEndian = BitConverter.GetBytes(ticks);
                    if (BitConverter.IsLittleEndian)
                    {
                        Array.Reverse(bytesBigEndian);
                    }
                    return Convert.ToBase64String(bytesBigEndian);

                default:
                    throw new ArgumentException("Unknown encoding type.");
            }
        }
    }
}
