namespace Fanex.ExternalAccount.ProblemAccount.Extensions
{
    using System;

    public static class DateTimeExtension
    {
        private const string DateFormat = "MM/dd/yyyy";
        private const string DateTimeInMinuteFormat = "MM/dd/yyyy HH:mm";

        public static string ToDateString(this DateTime dateTime)
            => dateTime.ToString(DateFormat);

        public static string ToDateTimeInMinuteString(this DateTime dateTime)
            => dateTime.ToString(DateTimeInMinuteFormat);
    }
}
