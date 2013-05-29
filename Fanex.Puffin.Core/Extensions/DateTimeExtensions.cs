// --------------------------------------------------------------------------
// <copyright file="DateTimeExtensions.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Anders.Le" date="2012.10.11">Created</change>
// </history>

using System;

namespace Fanex.Puffin.Core.Extensions
{
    /// <summary>
    /// Contains date extension methods
    /// </summary>
    public static class DateTimeExtensions
    {
        /// <summary>
        /// Get First Date of Month
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static DateTime FirstDayOfMonth(this DateTime dateTime)
        {
            return new DateTime(dateTime.Year, dateTime.Month, 1);
        }

        /// <summary>
        /// Get Last Date Of Month
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static DateTime LastDayOfMonth(this DateTime dateTime)
        {
            return dateTime.FirstDayOfMonth().AddMonths(1).AddDays(-1);
        }

        /// <summary>
        /// Format Short Date
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static string ToShortFormatedDate(this DateTime? date)
        {
            if (date.HasValue)
            {
                return date.Value.ToShortFormatedDate();
            }

            return string.Empty;
        }

        /// <summary>
        /// format a datetime value to string (MMM dd, yyyy)
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static string ToShortFormatedDate(this DateTime date)
        {
            return date.ToString(Constants.SHORT_DATE_FORMAT);
        }

        /// <summary>
        /// format a nullable datetime value to string (MMM dd, yyyy hh:mm)
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static string ToLongFormatedDate(this DateTime? date)
        {
            if (date.HasValue)
            {
                return date.Value.ToLongFormatedDate();
            }

            return string.Empty;
        }

        /// <summary>
        /// format a datetime value to string (MMM dd, yyyy hh:mm)
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static string ToLongFormatedDate(this DateTime date)
        {
            return date.ToString(Constants.LONG_DATE_FORMAT);
        }

        /// <summary>
        /// Format a nullable datetime value to string (dd-MMM)
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static string ToMonthAndDay(this DateTime? date)
        {
            if (date.HasValue)
            {
                return date.Value.ToMonthAndDay();
            }

            return string.Empty;
        }

        /// <summary>
        /// Foarmat da datatime value to string (dd-MMM)
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static string ToMonthAndDay(this DateTime date)
        {
            return date.ToString("dd-MMM");
        }

        /// <summary>
        /// Format a nullable datetime value to string (MMM yyyy)
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static string ToMonthYear(this DateTime? date)
        {
            if (date.HasValue)
            {
                return date.Value.ToMonthYear();
            }

            return string.Empty;
        }

        /// <summary>
        /// Format datatime value to string
        /// </summary>
        /// <param name="date"></param>
        /// <returns>string with format (MMM yyyy)</returns>
        public static string ToMonthYear(this DateTime date)
        {
            return date.ToString(Constants.MONTH_YEAR_DATE_FORMAT);
        }

        /// <summary>
        /// Format Short Date (MM/dd/yyyy)
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static string ToShortFormatedDateWithSlash(this DateTime? date)
        {
            if (date.HasValue)
            {
                return date.Value.ToShortFormatedDateWithSlash();
            }

            return string.Empty;
        }

        /// <summary>
        /// format a datetime value to string (MM/dd/yyyy)
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static string ToLongFormatedDateWithSlash(this DateTime date)
        {
            return date.ToString(Constants.LONG_DATE_FORMAT_WITH_SLASH);
        }

        /// <summary>
        /// format a datetime value to string (MM/dd/yyyy)
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static string ToShortFormatedDateWithSlash(this DateTime date)
        {
            return date.ToString(Constants.SHORT_DATE_FORMAT_WITH_SLASH);
        }

        /// <summary>
        /// Format a nullable datetime value to string (MM/yyyy)
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static string ToMonthYearWithSlash(this DateTime? date)
        {
            if (date.HasValue)
            {
                return date.Value.ToMonthYearWithSlash();
            }

            return string.Empty;
        }

        /// <summary>
        /// Format datatime value to string
        /// </summary>
        /// <param name="date"></param>
        /// <returns>string with format (MM/yyyy)</returns>
        public static string ToMonthYearWithSlash(this DateTime date)
        {
            return date.ToString(Constants.MONTH_YEAR_DATE_FORMAT_WITH_SLASH);
        }

        /// <summary>
        /// Foarmat da datatime value to string (dd-MMM-yy)
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static string ToAresDayMonthYear(this DateTime date)
        {
            return date.ToString(Constants.ARES_DATE_FORMAT);
        }

        /// <summary>
        /// Format datatime value to string (MMM-yy)
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static string ToAresMonthYear(this DateTime date)
        {
            return date.ToString(Constants.ARES_MONTH_YEAR_FORMAT);
        }
    }
}