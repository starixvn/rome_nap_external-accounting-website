namespace Fanex.ExternalAccount.DataAccess.Impl.Customer
{
    using System.Collections;
    using System.Data;
    using Data.DNAide;
    using Interface.Customer;

    public class SuperDao : CustomerDao, ISuperDao
    {
        public DataTable GetData(
            int? userId,
            string groupName,
            int? custId = null,
            bool? enabled = null,
            bool? closed = null,
            bool? suspended = null
        )
        {
            var param = new Hashtable
            {
                {"groupname", groupName},
                {"custid", custId},
                {"enabled", enabled},
                {"closed", closed},
                {"suspended", suspended},
                {"userid", userId}
            };

            return DbHelper.ExecuteDataTable("EAS_CustomerSelByCom", param);
        }
    }
}