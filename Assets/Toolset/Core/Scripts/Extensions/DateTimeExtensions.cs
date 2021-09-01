using System;

namespace Toolset.Core
{
    /// <summary>
    /// Extensions for the System.DateTime class.
    /// </summary>
    public static class DateTimeExtensions
    {
        /// <summary>
        /// DateTime representing the start of the unix epoch.
        /// </summary>
        public static DateTime EpochTime { get; } = new DateTime(1970, 1, 1);

        /// <summary>
        /// Gets the number of milliseconds that have passed between the DateTime and the start
        /// of the unix epoch.
        /// </summary>
        /// <param name="dateTime">The DateTime to compare against the unix epoch.</param>
        /// <returns>Milliseconds between the passed DateTime and the unix epoch.</returns>
        public static long MillisecondsSinceUnixEpoch(this DateTime dateTime)
        {
            return (long)dateTime.Subtract(EpochTime).TotalMilliseconds;
        }
    }
}
