namespace AutoQuest
{
    public static class TimeHelper
    {
        private static readonly DateTime Time2022 = new(2022, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        public static long Now()
        {
            return (DateTime.UtcNow - Time2022).Ticks / 10000;
        }

        /// <summary>
        /// 世界标准时间戳
        /// </summary>
        public static long UtcNow()
        {
            return (long)(DateTimeOffset.UtcNow - new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero))
                .TotalMilliseconds;
        }

        /// <summary>
        /// 从世界标准时间戳解析时间信息
        /// </summary>
        public static DateTime GetDateTime(long UtcNow)
        {
            return DateTimeOffset.FromUnixTimeMilliseconds(UtcNow).DateTime;
        }
    }
}