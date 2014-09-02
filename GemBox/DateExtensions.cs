using System;
using System.Globalization;
using System.Linq;

namespace GemBox
{
    public static class DateExtensions
    {
        #region Current time abstraction (for testability)

        private static readonly ITimeProvider DefaultTimeProvider = new DefaultTimeProvider();
        private static ITimeProvider _timeProvider = DefaultTimeProvider;

        internal static ITimeProvider TimeProvider
        {
            get
            {
                return _timeProvider;
            }
            set
            {
                if (value == null) throw new ArgumentNullException("value");
                _timeProvider = value;
            }
        }

        internal static void ResetTimeProvider()
        {
            _timeProvider = DefaultTimeProvider;
        }

        #endregion

        #region Durations

        public static TimeSpan Days(this int value)
        {
            return TimeSpan.FromDays(value);
        }

        public static TimeSpan Hours(this int value)
        {
            return TimeSpan.FromHours(value);
        }

        public static TimeSpan Minutes(this int value)
        {
            return TimeSpan.FromMinutes(value);
        }

        public static TimeSpan Seconds(this int value)
        {
            return TimeSpan.FromSeconds(value);
        }

        public static TimeSpan Milliseconds(this int value)
        {
            return TimeSpan.FromMilliseconds(value);
        }

        #endregion

        #region Relative dates

        public static DateTime Ago(this TimeSpan timeSpan)
        {
            return TimeProvider.Now - timeSpan;
        }

        public static DateTime FromNow(this TimeSpan timeSpan)
        {
            return TimeProvider.Now + timeSpan;
        }

        public static DateTime From(this TimeSpan timeSpan, DateTime startDate)
        {
            return startDate + timeSpan;
        }

        public static DateTime Before(this TimeSpan timeSpan, DateTime endDate)
        {
            return endDate - timeSpan;
        }

        #endregion

        #region Time of day

        public static DateTime At(this DateTime date, TimeSpan time)
        {
            return date.Date.Add(time);
        }

        public static DateTime At(this DateTime date, params TimeSpan[] timeParts)
        {
            return timeParts.Aggregate(date.Date, (d, t) => d.Add(t));
        }

        public static DateTime At(this DateTime date, int hours)
        {
            return date.Date.AddHours(hours);
        }

        public static DateTime At(this DateTime date, int hours, int minutes)
        {
            return date.Date.AddHours(hours).AddMinutes(minutes);
        }

        public static DateTime At(this DateTime date, int hours, int minutes, int seconds)
        {
            return date.Date.AddHours(hours).AddMinutes(minutes).AddSeconds(seconds);
        }

        #endregion

        #region UNIX timestamps

        private static readonly DateTime UnixEpoch = new DateTime(1970, 1, 1);

        public static long ToUnixTimestamp(this DateTime date)
        {
            TimeSpan span = date - UnixEpoch;
            return (long)Math.Truncate(span.TotalSeconds);
        }

        public static DateTime UnixTimestampToDateTime(this long unixTimestamp)
        {
            return UnixEpoch.AddSeconds(unixTimestamp);
        }

        #endregion

        #region Next/previous day of week

        public static DateTime Next(this DateTime from, DayOfWeek dayOfWeek)
        {
            int start = (int)from.DayOfWeek;
            int wanted = (int)dayOfWeek;
            if (wanted <= start)
                wanted += 7;
            return from.AddDays(wanted - start);
        }

        public static DateTime Next(this DayOfWeek dayOfWeek)
        {
            return TimeProvider.Now.Next(dayOfWeek);
        }

        public static DateTime Previous(this DateTime from, DayOfWeek dayOfWeek)
        {
            int end = (int)from.DayOfWeek;
            int wanted = (int)dayOfWeek;
            if (wanted >= end)
                end += 7;
            return from.AddDays(wanted - end);
        }

        public static DateTime Previous(DayOfWeek dayOfWeek)
        {
            return TimeProvider.Now.Previous(dayOfWeek);
        }

        #endregion

        #region Working days

        public static DateTime AddWorkingDays(this DateTime date, int days)
        {
            if (days == 0)
                return date;
            int sign = days < 0 ? -1 : 1;
            while (days % 5 != 0 || !date.IsWorkingDay())
            {
                date = date.AddDays(sign);
                if (!date.IsWorkingDay())
                    continue;
                days -= sign;
            }

            int nWeekEnds = days / 5;
            DateTime result = date.AddDays(days + nWeekEnds * 2);
            return result;
        }

        public static bool IsWorkingDay(this DateTime date)
        {
            return !(date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday);
        }

        #endregion

        #region Start of week

        public static DateTime GetStartOfWeek(int year, int weekOfYear)
        {
            return GetStartOfWeek(
                year,
                weekOfYear,
                CultureInfo.CurrentCulture.DateTimeFormat.CalendarWeekRule,
                CultureInfo.CurrentCulture.DateTimeFormat.FirstDayOfWeek);
        }

        public static DateTime GetStartOfWeek(int year, int weekOfYear, CalendarWeekRule rule)
        {
            return GetStartOfWeek(
                year,
                weekOfYear,
                rule,
                CultureInfo.CurrentCulture.DateTimeFormat.FirstDayOfWeek);
        }

        public static DateTime GetStartOfWeek(int year, int weekOfYear, DayOfWeek firstDayOfWeek)
        {
            return GetStartOfWeek(
                year,
                weekOfYear,
                CultureInfo.CurrentCulture.DateTimeFormat.CalendarWeekRule,
                firstDayOfWeek);
        }

        public static DateTime GetStartOfWeek(int year, int weekOfYear, CalendarWeekRule rule, DayOfWeek firstDayOfWeek)
        {
            if (year < 1 || year > 9999)
                throw new ArgumentOutOfRangeException("year");
            if (weekOfYear < 1 || weekOfYear > 54)
                throw new ArgumentOutOfRangeException("weekOfYear");
            if (!Enum.IsDefined(typeof (DayOfWeek), firstDayOfWeek))
                throw new ArgumentOutOfRangeException("firstDayOfWeek");
            switch (rule)
            {
                case CalendarWeekRule.FirstDay:
                    return GetStartOfWeekWithFirstDayRule(year, weekOfYear, firstDayOfWeek);
                case CalendarWeekRule.FirstFullWeek:
                    return GetStartOfWeekWithFirstFullWeekRule(year, weekOfYear, firstDayOfWeek);
                case CalendarWeekRule.FirstFourDayWeek:
                    return GetStartOfWeekWithFirstFourDayWeekRule(year, weekOfYear, firstDayOfWeek);
                default:
                    throw new ArgumentOutOfRangeException("rule");
            }
        }

        static DateTime GetStartOfWeekWithFirstDayRule(int year, int weekOfYear, DayOfWeek firstDayOfWeek)
        {
            DateTime jan1 = new DateTime(year, 1, 1);
            DateTime startOfFirstWeek =
                jan1.DayOfWeek == firstDayOfWeek
                    ? jan1
                    : jan1.Previous(firstDayOfWeek);
            DateTime startOfWeek = startOfFirstWeek.AddDays(7 * (weekOfYear - 1));
            if (startOfWeek.Year < year)
                return jan1;
            if (startOfWeek.Year > year)
                throw new ArgumentOutOfRangeException("weekOfYear");
            return startOfWeek;
        }

        static DateTime GetStartOfWeekWithFirstFullWeekRule(int year, int weekOfYear, DayOfWeek firstDayOfWeek)
        {
            DateTime jan1 = new DateTime(year, 1, 1);
            int diff = ((firstDayOfWeek - jan1.DayOfWeek) + 7) % 7;
            DateTime startOfFirstWeek = jan1.AddDays(diff);
            DateTime startOfWeek = startOfFirstWeek.AddDays(7 * (weekOfYear - 1));
            if (startOfWeek.Year > year)
                throw new ArgumentOutOfRangeException("weekOfYear");
            return startOfWeek;
        }

        static DateTime GetStartOfWeekWithFirstFourDayWeekRule(int year, int weekOfYear, DayOfWeek firstDayOfWeek)
        {
            DateTime jan1 = new DateTime(year, 1, 1);
            DayOfWeek fourthDayOfWeek = (DayOfWeek)(((int)firstDayOfWeek + 3) % 7);
            DateTime firstFourDayOfWeek =
                jan1.DayOfWeek == fourthDayOfWeek
                    ? jan1
                    : jan1.Next(fourthDayOfWeek);

            DateTime startOfFirstWeek = firstFourDayOfWeek.Previous(firstDayOfWeek);
            DateTime startOfWeek = startOfFirstWeek.AddDays(7 * (weekOfYear - 1));
            if (startOfWeek.Year < year)
                return jan1;
            if (startOfWeek.Year > year)
                throw new ArgumentOutOfRangeException("weekOfYear");
            return startOfWeek;
        }

        #endregion

        #region WeekOfYear

        public static int WeekOfYear(this DateTime date)
        {
            return WeekOfYear(date, CalendarWeekRule.FirstDay);
        }

        public static int WeekOfYear(this DateTime date, CalendarWeekRule rule)
        {
            return date.WeekOfYear(rule, CultureInfo.CurrentCulture.DateTimeFormat.FirstDayOfWeek);
        }

        public static int WeekOfYear(this DateTime date, DayOfWeek firstDayOfWeek)
        {
            return date.WeekOfYear(CultureInfo.CurrentCulture.DateTimeFormat.CalendarWeekRule, firstDayOfWeek);
        }

        public static int WeekOfYear(this DateTime date, CalendarWeekRule rule, DayOfWeek firstDayOfWeek)
        {
            return CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(date, rule, firstDayOfWeek);
        }

        #endregion

        #region Misc

        public static DateTime Truncate(this DateTime date, DateTimeComponent component)
        {
            int year = 1;
            int month = 1;
            int day = 1;
            int hour = 0;
            int minute = 0;
            int second = 0;
            int millisecond = 0;

            if (component <= DateTimeComponent.Millisecond)
                millisecond = date.Millisecond;
            if (component <= DateTimeComponent.Second)
                second = date.Second;
            if (component <= DateTimeComponent.Minute)
                minute = date.Minute;
            if (component <= DateTimeComponent.Hour)
                hour = date.Hour;
            if (component <= DateTimeComponent.Day)
                day = date.Day;
            if (component <= DateTimeComponent.Month)
                month = date.Month;
            if (component <= DateTimeComponent.Year)
                year = date.Year;

            return new DateTime(year, month, day, hour, minute, second, millisecond, date.Kind);
        }

        #endregion
    }

    internal interface ITimeProvider
    {
        DateTime Now { get; }
        DateTime UtcNow { get; }
    }

    internal class DefaultTimeProvider : ITimeProvider
    {
        public DateTime Now
        {
            get { return DateTime.Now; }
        }

        public DateTime UtcNow
        {
            get { return DateTime.UtcNow; }
        }
    }

    public enum DateTimeComponent
    {
        Millisecond,
        Second,
        Minute,
        Hour,
        Day,
        Month,
        Year
    }
}
