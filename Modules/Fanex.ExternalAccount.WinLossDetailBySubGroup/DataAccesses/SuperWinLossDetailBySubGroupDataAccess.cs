namespace Fanex.ExternalAccount.SuperWinLossDetailBySubGroup.DataAccesses
{
    using Fanex.ExternalAccount.SuperWinLossDetailBySubGroup.Models;
    using System.Linq;
    using Fanex.Data.Repository;
    using Fanex.ExternalAccount.DataAccess.Products;
    using Fanex.ExternalAccount.SuperWinLossDetailBySubGroup.Criteria;
    
    public interface ISuperWinLossDetailBySubGroupDataAccess
    {
        SuperWinLossDetailBySubGroupReport GetSuperWinLossDetailBySubGroup(GetSuperWinLossDetailBySubGroupCriteria criteria);
        string GetSubGroupList(int groupId, string subGroup);
    }

    public class SuperWinLossDetailBySubGroupDataAccess : ISuperWinLossDetailBySubGroupDataAccess
    {
        private readonly IDynamicRepository repository;
        public SuperWinLossDetailBySubGroupDataAccess(IDynamicRepository repository = null) => this.repository = repository ?? new DynamicRepository();
        public SuperWinLossDetailBySubGroupReport GetSuperWinLossDetailBySubGroup(GetSuperWinLossDetailBySubGroupCriteria criteria)
        {
           var dbResult = repository.FetchMultiple<SuperWinLossDetailBySubGroupModel, SuperWinLossDetailBySubGroupModel>(criteria);
           return new SuperWinLossDetailBySubGroupReport(dbResult.Item1, dbResult.Item2.FirstOrDefault(), criteria.TotalRecords);
        }

        public string GetSubGroupList(int groupId, string subGroup)
        {
            var criteria = new GetSubGroupListCriteria(groupId, subGroup);
            repository.Execute(criteria);
            return criteria.Subgrouplist;
        }
    }
}
