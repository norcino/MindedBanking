using System;

namespace MB.Common
{
    /// <summary>
    /// Use SystemTime.Now() to get the 'current' time. By default this is DateTime.Now
    /// Can be overridden by setting the property manually SystemTime.Now = () => new DateTime(xxxxxx);
    /// This should only be overridden in Unit Tests, never in production code.
    /// </summary>
    public static class SystemTime
    {
        public static Func<DateTime> Now = () => DateTime.Now;
        public static Func<DateTime> UtcNow = () => DateTime.UtcNow;
        public static Func<DateTimeOffset> NowOffset = () => DateTimeOffset.Now;
        public static Func<DateTimeOffset> UtcNowOffset = () => DateTimeOffset.UtcNow;

        /// <summary>
        /// Reset the SystemTime to it's default behaviour using DateTime
        /// </summary>
        public static void Reset()
        {
            Now = () => DateTime.Now;
            UtcNow = () => DateTime.UtcNow;
            NowOffset = () => DateTimeOffset.Now;
            UtcNowOffset = () => DateTimeOffset.UtcNow;
        }

        /// <summary>
        /// Set a predefined DateTime to be used for each request to Noe and UtcNow
        /// </summary>
        /// <param name="dateTime">DateTime instance to be returned at each request</param>
        public static void Set(DateTime dateTime)
        {
            Now = () => dateTime;
            UtcNow = () => dateTime;
        }

        /// <summary>
        /// Set a predefined DateTimeOffset to be used for each request to NowOffset and UtcNowOffset
        /// </summary>
        /// <param name="dateTimeOffset">DateTimeOffset instance to be returned at each request</param>
        public static void SetOffset(DateTimeOffset dateTimeOffset)
        {
            NowOffset = () => dateTimeOffset;
            UtcNowOffset = () => dateTimeOffset;
        }
    }
}
