// -----------------------------------------------------------------------
// <copyright file="CasinoDao.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Fanex.ExternalAccount.DataAccess.Impl.Customer.Casino
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Collections;
    using Fanex.Data.DNAide;
    using Fanex.ExternalAccount.Utilities;
    using CasinoLibrary.SBAgent;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class CasinoDao
    {
        public bool GetCasinoStatus(int custid)
        {
            Hashtable param = new Hashtable();
            param.Add("custid", custid);

            DbResult drCustomer = DNAide.Data.DbHelper.Execute("Age_GetCSStatus", param, 300);
            return Utils.ToBoolean(drCustomer.Outputs["status"]);
        }

        public bool UpdateCSStatus(int custid, bool bSyncCS, bool bDisabledCS, int userId, string ip, int subAccId, string lang, string username, string subUsername)
        {
            Hashtable param = new Hashtable();
            param.Add("custid", custid);
            param.Add("disabledcs", bDisabledCS);
            param.Add("synccs", bSyncCS);
            param.Add("enabledownline", false);

            param.Add("userid", userId);
            param.Add("subaccid", subAccId);
            param.Add("loginip", ip);
            param.Add("language", lang);

            DbResult drReturn = DNAide.Data.DbHelper.Execute("Acc_UpdCSStatus", param, 300);

            bool status = Utils.ToBoolean(drReturn.Outputs["status"]);

            if (!status) return false;

            Customer cust = new Customer();
            return cust.UpdateCustEnableCasino(custid, !bDisabledCS, true, ip, custid, subAccId == 0 ? username : subUsername, 2);
        }

        public bool DoUpdateCSStatusWithoutSync(int custid, bool bSyncCS, bool bDisabledCS, bool isUpdDownline, int userId, string ip, int subAccId, string lang, out string errMsg)
        {
            Hashtable param = new Hashtable();
            param.Add("custid", custid);
            param.Add("disabledcs", bDisabledCS);
            param.Add("synccs", bSyncCS);
            param.Add("enabledownline", isUpdDownline);

            param.Add("userid", userId);
            param.Add("subaccid", subAccId);
            param.Add("loginip", ip);
            param.Add("language", lang);

            DbResult drReturn = DNAide.Data.DbHelper.Execute("Acc_UpdCSStatus", param, 300);

            errMsg = drReturn.Outputs["errmsg"].ToString();
            return Utils.ToBoolean(drReturn.Outputs["status"]);
        }
    }
}
