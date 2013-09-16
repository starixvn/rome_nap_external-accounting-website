// --------------------------------------------------------------------------
// <copyright file="Config.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Andy.Nguyen" date="2013.9.16">Created</change>
// </history>

namespace Fanex.ExternalAccount.Utilities
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Linq;
    using System.Text;
    using System.Web;
    using Fanex.ExternalAccount.DTO;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public static class Config
    {
        public static DateTime WinlostDate
        {
            get
            {
                if (System.Web.HttpContext.Current.IsDebuggingEnabled)
                {
                    return new DateTime(2012, 11, 6).Date;
                }
                else
                {
                    return DateTime.Now.AddDays(-1).Date;
                }
            }
        }

        public static bool IsTest
        {
            get
            {
                return GetValue("IsTest").Equals("true", StringComparison.OrdinalIgnoreCase);
            }
        }

        public static string LBUrl
        {
            get
            {
                return GetValue("LBUrl");
            }
        }

        public static bool IsUM
        {
            get
            {
                return Utils.ToBoolean(GetValue("IsUM"));
            }
        }

        public static string TimeFrom
        {
            get
            {
                return GetValue("TimeFrom");
            }
        }

        public static string TimeTo
        {
            get
            {
                return GetValue("TimeTo");
            }
        }

        public static bool Recheckin
        {
            get { return Utils.ToBoolean(GetValue("Recheckin")); }
        }

        public static bool IsShowVirtualSports
        {
            get { return Utils.ToBoolean(GetValue("EnableVirtualSports")); }
        }

        public static string CssFiles
        {
            get
            {
                return GetValue("CssFiles");
            }
        }

        public static string JsFiles
        {
            get
            {
                return GetValue("JsFiles");
            }
        }

        public static string JsLogin
        {
            get
            {
                return GetValue("JsLogin");
            }
        }

        public static string CssLogin
        {
            get
            {
                return GetValue("CssLogin");
            }
        }

        public static string JsExtGroup
        {
            get
            {
                return GetValue("JsExtGroup");
            }
        }

        public static string CssExtGroup
        {
            get
            {
                return GetValue("CssExtGroup");
            }
        }

        public static string JsDailyWinLoss
        {
            get
            {
                return GetValue("JsDailyWinLoss");
            }
        }

        public static string CssDailyWinLoss
        {
            get
            {
                return GetValue("CssDailyWinLoss");
            }
        }

        public static string JsWinLossDetail
        {
            get
            {
                return GetValue("JsWinLossDetail");
            }
        }

        public static string CssWinLossDetail
        {
            get
            {
                return GetValue("CssWinLossDetail");
            }
        }

        public static string JsStatement
        {
            get
            {
                return GetValue("JsStatement");
            }
        }

        public static string CssStatement
        {
            get
            {
                return GetValue("CssStatement");
            }
        }

        public static string JsCustomer
        {
            get
            {
                return GetValue("JsCustomer");
            }
        }

        public static string CssCustomer
        {
            get
            {
                return GetValue("CssCustomer");
            }
        }

        public static string InternalLoginUrl
        {
            get
            {
                return GetValue("int_agent_url");
            }
        }

        /// <summary>
        /// Gets maintenance information.
        /// </summary>
        /// <returns>MaintenanceInfo object.</returns>
        public static MaintenanceInfo GetMaintenanceInfo()
        {
            var maintenanceInfo = new MaintenanceInfo { IsDownTime = false };

            if (Config.IsUM)
            {
                maintenanceInfo.IsDownTime = true;
                maintenanceInfo.TimeFrom = Config.TimeFrom;
                maintenanceInfo.TimeTo = Config.TimeTo;
            }

            return maintenanceInfo;
        }

        private static string GetValue(string key)
        {
            return Sunplus.Agent.Configurations.AppSettings.Configuration.Get(key);
        }
    }
}