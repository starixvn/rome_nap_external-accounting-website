// --------------------------------------------------------------------------
// <copyright file="CasinoDao.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Andy.Nguyen" date="2013.4.11">Created</change>
// </history>

namespace Fanex.ExternalAccount.DataAccess.Impl.Customer.Casino
{
    using System.Collections;
    using CasinoLibrary.SBAgent;
    using Fanex.Data.DNAide;
    using Fanex.ExternalAccount.Utilities;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class CasinoDao
    {
        public bool GetCasinoStatus(int custid)
        {
            Hashtable param = new Hashtable();
            param.Add("custid", custid);

            DbResult dr = DNAide.Data.DbHelper.Execute("Age_GetCSStatus", param, 300);
            return Utils.ToBoolean(dr.Outputs["status"]);
        }

        public bool UpdateCSStatus(int custid, bool syncCS, bool disabledCS, int userId, string ip, int subAccId, string lang, string username, string subUsername)
        {
            Hashtable param = new Hashtable();
            param.Add("custid", custid);
            param.Add("disabledcs", disabledCS);
            param.Add("synccs", syncCS);
            param.Add("enabledownline", false);

            param.Add("userid", userId);
            param.Add("subaccid", subAccId);
            param.Add("loginip", ip);
            param.Add("language", lang);

            DbResult dr = DNAide.Data.DbHelper.Execute("Acc_UpdCSStatus", param, 300);

            bool status = Utils.ToBoolean(dr.Outputs["status"]);

            if (!status)
            {
                return false;
            }

            Customer cust = new Customer();
            return cust.UpdateCustEnableCasino(custid, !disabledCS, true, ip, custid, subAccId == 0 ? username : subUsername, 2);
        }

        public bool DoUpdateCSStatusWithoutSync(int custid, bool syncCS, bool disabledCS, bool isUpdDownline, int userId, string ip, int subAccId, string lang, out string errMsg)
        {
            Hashtable param = new Hashtable();
            param.Add("custid", custid);
            param.Add("disabledcs", disabledCS);
            param.Add("synccs", syncCS);
            param.Add("enabledownline", isUpdDownline);

            param.Add("userid", userId);
            param.Add("subaccid", subAccId);
            param.Add("loginip", ip);
            param.Add("language", lang);

            DbResult dr = DNAide.Data.DbHelper.Execute("Acc_UpdCSStatus", param, 300);

            errMsg = dr.Outputs["errmsg"].ToString();
            return Utils.ToBoolean(dr.Outputs["status"]);
        }
    }
}
