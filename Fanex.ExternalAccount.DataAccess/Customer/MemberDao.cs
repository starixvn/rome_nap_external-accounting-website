namespace Fanex.ExternalAccount.DataAccess.Impl.Customer
{
    using System.Collections;
    using System.Data;
    using Fanex.Data.DNAide;
    using Fanex.ExternalAccount.DataAccess.Interface.Customer;

    public class MemberDao : CustomerDao, IMemberDao
    {
        #region IData Members

        public DataTable GetData(
            int? userId,
            string groupName,
            int? recommend = null, 
            int? custId = null, 
            bool? enabled = null, 
            bool? closed = null, 
            bool? suspended = null)
        {
            var param = new Hashtable();
            param.Add("recommend", recommend);
            param.Add("custid", custId);
            param.Add("enabled", enabled);
            param.Add("closed", closed);
            param.Add("suspended", suspended);
            param.Add("userid", userId);

            return DbHelper.ExecuteDataTable("EAS_CustomerSelByAg", param);
        }

        #endregion IData Members
    }
}