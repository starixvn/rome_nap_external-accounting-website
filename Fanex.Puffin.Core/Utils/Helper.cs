// -----------------------------------------------------------------------
// <copyright file="Helper.cs" company="Microsoft">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Fanex.Puffin.Core.Models;
using Fanex.Puffin.Core.Services.Impl;
using Fanex.Puffin.Core.Services.Interface;
using Fanex.Puffin.Core.Web;

namespace Fanex.Puffin.Core.Utils
{
    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class Helper
    {
        public enum SportType
        {
            Sportsbook = 0,
            NumberGame = 1,
            VirtualSports = 2,
            LiveCasino = 3
        }

        public const string AtlanticTime_Canada = "Atlantic Standard Time";

        public static string CurrencyById(int Id)
        {
            string currencyName = string.Empty;
            ICoreService _coreService;
            try
            {
               IList<Currency> _currencies = CacheManager.Get<IList<Currency>>(CacheServiceKey.KeyCurrency);
               if (_currencies == null)
               {
                   _coreService = new CoreService();
                   _currencies = _coreService.GetCurrency();
                   CacheManager.Set<IList<Currency>>(CacheServiceKey.KeyCurrency, _currencies);
               }

                currencyName = _currencies.Where(s => s.Id == Id.ToString()).Select(s => s.Name).FirstOrDefault();
                if (string.IsNullOrEmpty(currencyName))
                {
                     _coreService = new CoreService();
                     currencyName = _coreService.GetCurrency().Where(s => s.Id == Id.ToString()).Select(s => s.Name).FirstOrDefault();
                }

            }
            catch (Exception ex)
            {
                LoggingHandler.Log(ex);
            }

            return currencyName;
        }

        public static string GetClientIP()
        {
            string ip = string.Empty;
            try
            {
                if (HttpContext.Current.Request.IsLocal)
                    ip = "127.0.0.1";
                else 
                    ip = Convert.ToString(HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"]);
            }
            catch (Exception ex)
            {
                LoggingHandler.Log(ex);
            }
            return ip;
        }

        public static string GetBetTypeName(int bettype, bool liveIndicator)
        {
            var bettypeName = CacheRef.GetBettype(bettype);

            switch (bettype)
            {
                case 86:
                    bettypeName = liveIndicator ? "Next Odd/Even" : "FT. Odd/Even";
                    break;
                case 85:
                    bettypeName = "Next Over/Under";
                    break;
                case 90:
                    bettypeName += liveIndicator ? " (Live Game)" : " (Pre Game)";
                    break;
                default:
                    break;
            }

            return bettypeName;
        }

        /// get all time zones Info
        //ReadOnlyCollection<TimeZoneInfo> zones = TimeZoneInfo.GetSystemTimeZones();

        public static DateTime ConvertDatetimeZone(DateTime datetime, string distinationTimeZoneId)
        {
            var utcTime = TimeZoneInfo.ConvertTimeBySystemTimeZoneId(datetime, distinationTimeZoneId); 
            return utcTime;
        }

        /// <summary>
        /// Convert Datetime Zone GMT-4 (Canada)
        /// </summary>
        /// <param name="datetime"></param>
        /// <returns></returns>
        public static DateTime ConvertDatetimeZoneCanada(DateTime datetime)
        {
            return ConvertDatetimeZone(datetime, AtlanticTime_Canada);
        }
    }
}
