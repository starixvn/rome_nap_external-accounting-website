// --------------------------------------------------------------------------
// <copyright file="BingoDao.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Andy.Nguyen" date="2013.4.12">Created</change>
// </history>

namespace Fanex.ExternalAccount.DataAccess.Impl.Customer.Bingo
{
    using System;
    using System.Collections;
    using Fanex.Data.DNAide;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class BingoDao
    {
        public int UpdateBingoStatus(int custId, int userId, int subAccId, bool isSynBingo, bool disabledBingo, bool isEnableDownline, string ip, string languageKey, ref string errMsg)
        {
            Hashtable parameters = new Hashtable();

            parameters.Add("custid", custId);
            parameters.Add("disabledbingo", disabledBingo);
            parameters.Add("syncbg", isSynBingo);
            parameters.Add("enabledownline", isEnableDownline);
            parameters.Add("userid", userId);
            parameters.Add("subaccid", subAccId);
            parameters.Add("loginip", ip);
            parameters.Add("language", languageKey);
            parameters.Add("errcode", 0);
            parameters.Add("errmsg", string.Empty);

            DbResult result = DNAide.Data.DbHelper.Execute("Acc_UpdBingoStatus", parameters);
            errMsg = result.Outputs["errmsg"].ToString();

            return Convert.ToInt32(result.Outputs["errcode"]);
        }
    }
}
