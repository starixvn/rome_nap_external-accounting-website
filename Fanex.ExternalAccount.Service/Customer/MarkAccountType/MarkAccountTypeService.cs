namespace Fanex.ExternalAccount.Service.Customer.MarkAccountType
{
    using Fanex.ExternalAccount.DataAccess.Criteria.Customer.MarkAccountType;
    using Fanex.ExternalAccount.DataAccess.Customer.MarkAccountType;
    using Fanex.ExternalAccount.DTO;
    using Fanex.ExternalAccount.Service.Customer.MarkAccountType.Params;
    using Fanex.ExternalAccount.ViewModel.Customer.MarkAccountType;
    using System.Collections.Generic;
    using System.Linq;

    public class MarkAccountTypeService : IMarkAccountTypeService
    {
        private readonly IMarkAccountTypeDao markAccountTypeDao;

        public MarkAccountTypeService() : this(null)
        {
        }

        public MarkAccountTypeService(MarkAccountTypeDao markAccountTypeDao)
        {
            this.markAccountTypeDao = markAccountTypeDao ?? new MarkAccountTypeDao();
        }

        public Result Add(NewMarkAccountTypeParams newMarkAccountTypeParams)
        {
            var usernames = string.Join(",", newMarkAccountTypeParams.Usernames.Trim(' ').Split(',').Distinct());

            var result = markAccountTypeDao.Check(new CheckMarkAccountCriteria
            {
                GroupName = newMarkAccountTypeParams.GroupName,
                Usernames = usernames
            });

            return !result.IsSuccess
                ? result
                : markAccountTypeDao.Add(new AddMarkAccountCriteria
            {
                GroupName = newMarkAccountTypeParams.GroupName,
                Usernames = usernames,
                TypeId = newMarkAccountTypeParams.TypeId,
                Remark = newMarkAccountTypeParams.Remark,
                UserId = newMarkAccountTypeParams.UserId,
                IP = newMarkAccountTypeParams.IP
            });
        }

        public Result MultipleUpdate(IEnumerable<UpdateMarkAccountTypeParams> updateAccountTypes)
        {
            throw new System.NotImplementedException();
        }

        public MarkAccountViewModel Search(SearchMarkAccountTypeParams parameters)
        {
            var result = markAccountTypeDao.SearchMarkAccount(new SearchMarkAccountCriteria
            {
                GroupId = parameters.GroupId,
                GroupName = parameters.GroupName,
                RoleId = parameters.Level,
                PageIndex = parameters.PageIndex,
                PageSize = parameters.PageSize,
                TypeId = parameters.Type,
                UserId = parameters.UserId,
                UserName = parameters.UserName
            });

            return result;
        }

        public Result SingleUpdate(UpdateMarkAccountTypeParams updateMarkAccountTypeParams)
        {
            var result = markAccountTypeDao.Update(new UpdateMarkAccountCriteria
            {
                CustID = updateMarkAccountTypeParams.CustId,
                TypeID = updateMarkAccountTypeParams.TypeId,
                Remark = updateMarkAccountTypeParams.Remark,
                IP = updateMarkAccountTypeParams.IP,
                UserID = updateMarkAccountTypeParams.UserId,
            });

            return result;
        }

        public Result Unmark(UnmarkAccountTypeParams parameters)
        {
            var result = markAccountTypeDao.Unmark(new UnmarkMarkAccountCriteria
            {
                CustID = parameters.CustId,
                IP = parameters.IP,
                UserID = parameters.UserId,
            });

            return result;
        }

        public Result UnmarkAll(UnmarkAccountTypeParams unmarkAccountTypeParams)
        {
            var errorDeleteCustIds = new List<int>();
            foreach (var custId in unmarkAccountTypeParams.CustIds)
            {
                var result = markAccountTypeDao.Unmark(new UnmarkMarkAccountCriteria
                {
                    CustID = custId,
                    IP = unmarkAccountTypeParams.IP,
                    UserID = unmarkAccountTypeParams.UserId,
                });

                if (!result.IsSuccess)
                {
                    errorDeleteCustIds.Add(custId);
                }
            }

            if(errorDeleteCustIds.Count > 0)
            {
                return new Result("1", string.Join(", ", errorDeleteCustIds));
            }

            return new Result();
        }
    }
}
