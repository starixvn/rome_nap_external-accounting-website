namespace Fanex.ExternalAccount.ProblemAccount.Extensions
{
    using System;

    public static class DateTimeExtension
    {
        private const string DateFormatWithSlash = "MM/dd/yyyy";
        private const string DateFormatWithHyphen = "yyyy-MM-dd";

        private const string DateTimeFormatInMinute = "MM/dd/yyyy HH:mm";
        private const string DateTimeFormatInSecond = "MM/dd/yyyy HH:mm:ss";

        public static string ToDateWithSlash(this DateTime dateTime)
            => dateTime.ToString(DateFormatWithSlash);

        public static string ToDateWithHyphen(this DateTime dateTime)
            => dateTime.ToString(DateFormatWithHyphen);

        public static string ToDateTimeInMinuteString(this DateTime dateTime)
            => dateTime.ToString(DateTimeFormatInMinute);

        public static string ToDateTimeInSecondString(this DateTime dateTime)
            => dateTime.ToString(DateTimeFormatInSecond);
    }
}
