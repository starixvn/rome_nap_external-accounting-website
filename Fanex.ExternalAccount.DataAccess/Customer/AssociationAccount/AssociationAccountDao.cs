namespace Fanex.ExternalAccount.DataAccess.Interface.Customer
{
    using System.Collections.Generic;
    using Criteria.Customer;
    using DTO.Customer;
    using Fanex.Data;
    using Fanex.ExternalAccount.Utilities;

    public interface IAssociationAccountDao
    {
        AssociationAccountViewModel Get(GetAssociationAccountCriteria criteria);
    }

    public class AssociationAccountDao : IAssociationAccountDao
    {
        public AssociationAccountViewModel Get(GetAssociationAccountCriteria criteria)
        {
            var accounts = new AssociationAccountViewModel();
            using (var objDb = new ObjectDb(criteria.GetSettingKey()))
            {
                objDb.AssignParamValues(criteria.GetSpParams());
                var reader = objDb.ExecuteReader();
                while (reader.Read())
                {
                    accounts.Accounts.Add(new AssociationAccountModel
                    {
                        CustId = Utils.ToInt32(reader["CustId"]),
                        Username = Utils.ToString(reader["UserName"]),
                        RoleId = Utils.ToInt32(reader["RoleId"]),
                        Date = Utils.ToDateTime(reader["Date"]),
                        Closed = Utils.ToBoolean(reader["Closed"]),
                        Enabled = Utils.ToBoolean(reader["Enabled"]),
                        Suspended = Utils.ToBoolean(reader["Suspended"]),
                        AgentId = Utils.ToInt32(reader["AgentId"])
                    });
                }

                reader.Close();
                accounts.PageIndex = criteria.PageIndex;
                accounts.TotalRows = Utils.ToInt32(objDb.GetOuputParams()["totalrows"]);
                accounts.TotalAccount = Utils.ToInt32(objDb.GetOuputParams()["totalaccount"]);
            }

            return accounts;
        }
    }
}