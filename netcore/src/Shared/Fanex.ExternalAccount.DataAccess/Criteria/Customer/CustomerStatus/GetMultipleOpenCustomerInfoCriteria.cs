namespace Fanex.ExternalAccount.DataAccess.Criteria.Customer.CustomerStatus
{
    using Fanex.Data.Repository;

    public class GetMultipleOpenCustomerInfoCriteria : CriteriaBase
    {
        public GetMultipleOpenCustomerInfoCriteria(string custIdList, int roleId)
        {
            CustIdList = custIdList;
            RoleId = roleId;
        }

        public string CustIdList { get; }

        public int RoleId { get; }

        public override string GetSettingKey() => "Acc_CustomerList_CloseInfoSel";

        public override bool IsValid() => !string.IsNullOrWhiteSpace(CustIdList);
    }
}