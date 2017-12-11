namespace Fanex.ExternalAccount.DataAccess.Impl.Customer
{
    using System.Collections;
    using Fanex.Data.DNAide;
    using Fanex.ExternalAccount.DataAccess.Interface.Customer;

    public class SuperDao : CustomerDao, ISuperDao
    {
        #region IData Members

        public System.Data.DataTable GetData(
            int? userId,
            int? recommend = null, 
            int? custId = null, 
            bool? enabled = null,
            bool? closed = null,
            bool? suspended = null, 
            string groupName = "ACCDDD"
            )
        {
            var param = new Hashtable();
            param.Add("groupname", groupName);
            param.Add("custid", custId);
            param.Add("enabled", enabled);
            param.Add("closed", closed);
            param.Add("suspended", suspended);
            param.Add("userid", userId);

            return DbHelper.ExecuteDataTable("EAS_CustomerSelByCom", param);
        }

        #endregion IData Members
    }
}