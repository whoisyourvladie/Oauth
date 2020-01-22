using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SaaS.Common.Utils
{
    public static class TimeHelper
    {
        const int SecondsInMinute = 60;
        const long TicksInSecond = 10000000;
        const long TicksInMinute = TicksInSecond * SecondsInMinute;
        static readonly DateTime UnixTimeBegining = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        /// <summary>
        /// get Epoch time;
        /// Epoch time is a Unix convention - the number of seconds elapsed since Jan 1, 1970
        /// </summary>
        /// <returns></returns>
        public static int GetEpochTime()
        {
            return (int)(DateTime.UtcNow - UnixTimeBegining).TotalSeconds;
        }

        /// <summary>
        /// get Epoch time;
        /// Epoch time is a Unix convention - the number of seconds elapsed since Jan 1, 1970
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public static int GetEpochTime(this DateTime time)
        {
            return (int)(time.ToUniversalTime() - UnixTimeBegining).TotalSeconds;
        }

        /// <summary>
        /// convert unix time to utc datetime
        /// </summary>
        /// <param name="unixTime">Epoch time is a Unix convention - the number of seconds elapsed since Jan 1, 1970</param>
        /// <returns>return utc datetime</returns>
        public static DateTime FromEpochTime(this long unixTime)
        {
            return UnixTimeBegining.AddSeconds(unixTime);
        }

        /// <summary>
        /// convert UTC to relative time
        /// </summary>
        /// <param name="dateTimeUtc">utc time</param>
        /// <param name="timeZoneOffset">time zone offset in minutes</param>
        /// <returns>return point in time relative to UTC</returns>
        public static DateTimeOffset UtcToLocalTime(this DateTime dateTimeUtc, int timeZoneOffset)
        {
            if (dateTimeUtc.Kind != DateTimeKind.Utc)
                throw new Exception("Utc datetime is expected.");

            var offsetTimespan = new TimeSpan(timeZoneOffset * TicksInMinute);
            return new DateTimeOffset(dateTimeUtc).ToOffset(-offsetTimespan);
        }
    }
}
