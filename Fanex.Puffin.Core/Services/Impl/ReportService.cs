// --------------------------------------------------------------------------
// <copyright file="ReportUtils.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Louis.Nguyen" date="2012.10.18">Created</change>
// </history>

using System;
using System.Diagnostics.CodeAnalysis;
using Fanex.NAP.Framework.Data;
using Fanex.Puffin.Core.Criteria;
using Fanex.Puffin.Core.Models;
using Fanex.Puffin.Core.Web;

namespace Fanex.Puffin.Core.Services.Impl
{
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented")]
    public static class ReportService
    {
        #region StaticFields

        public const string CACHE_BALANCE_UP = "Fanex.Notus.Core.ReportUtils.BalanceUp";
        public const string CACHE_EXCHANGE_CURRENCIES = "Fanex.Notus.Core.ReportService.ExchangeCurrencies";
        public const string BET_TRANS14_SP_SUFFIX = "_14";
        public const string WARE_HOUSE_SP_SUFFIX = "_WH";
        private static readonly IDynamicRepository _balanceUpRepository;

        #endregion StaticFields

        static ReportService()
        {
            _balanceUpRepository = new DynamicRepository();
        }

        #region StaticProperties

        public static BalanceUp BalanceUp
        {
            get
            {
                var balanceUp = CacheManager.Get<BalanceUp>(CACHE_BALANCE_UP);
                if (balanceUp == null)
                {
                    var criteria = new BalanceUpCriteria();
                    balanceUp = _balanceUpRepository.Get<BalanceUp>(criteria);
                    CacheManager.Set<BalanceUp>(CACHE_BALANCE_UP, balanceUp, new TimeSpan(5,0,0,0));
                }

                return balanceUp;
            }
        }


        #endregion StaticProperties

        #region StaticMethods

        public static string GetStoreMappingSubfix(DateTime reportDate)
        {
            if (reportDate > BalanceUp.MoveBetsDay)
            {
                return string.Empty;
            }
            else if (reportDate >= BalanceUp.BalanceUpDay)
            {
                return BET_TRANS14_SP_SUFFIX;
            }
            else
            {
                return WARE_HOUSE_SP_SUFFIX;
            }
        }

        #endregion StaticMethods
    }
}