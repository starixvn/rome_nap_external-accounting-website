namespace Fanex.ExternalAccount.DataAccess.Criteria.Customer.CustomerStatus
{
    using Fanex.Data.Repository;

    public class GetMultipleSuspendCustomerInfoCriteria : CriteriaBase
    {
        public GetMultipleSuspendCustomerInfoCriteria(string custIdList, int roleId)
        {
            CustIdList = custIdList;
            RoleId = roleId;
        }

        public string CustIdList { get; }

        public int RoleId { get; }

        public override string GetSettingKey()
        {
            return "Acc_CustomerList_SuspendInfoSel";
        }

        public override bool IsValid()
        {
            return string.IsNullOrWhiteSpace(CustIdList);
        }
    }
}
