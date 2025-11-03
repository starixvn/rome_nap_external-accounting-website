namespace Fanex.ExternalAccount.Service.Customer.MarkAccountType
{
    using Fanex.ExternalAccount.DataAccess.Criteria.Customer.MarkAccountType;
    using Fanex.ExternalAccount.DataAccess.Customer.MarkAccountType;
    using Fanex.ExternalAccount.DTO;
    using Fanex.ExternalAccount.Service.Customer.MarkAccountType.Params;
    using Fanex.ExternalAccount.ViewModel.Customer.MarkAccountType;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.RegularExpressions;
    using System.Web;

    public class MarkAccountTypeService : IMarkAccountTypeService
    {
        private readonly IMarkAccountTypeDao markAccountTypeDao;

        private const int MaxUsernames = 500;
        private const string UmarkUpdateError = "The following usernames are being unmarked: ";
        private const string FailUpdateError = "</br>The following usernames are failed to update: ";

        public MarkAccountTypeService() : this(null)
        {
        }

        public MarkAccountTypeService(MarkAccountTypeDao markAccountTypeDao)
        {
            this.markAccountTypeDao = markAccountTypeDao ?? new MarkAccountTypeDao();
        }

        public Result Add(NewMarkAccountTypeParams newMarkAccountTypeParams)
        {
            var usernames = newMarkAccountTypeParams.Usernames.Replace(" ", "").Split(',').Distinct();

            if(usernames.Count() > MaxUsernames)
            {
                return new Result(1, "The system supports up to 500 usernames at one time.");
            }

            var result = markAccountTypeDao.Check(new CheckMarkAccountCriteria
            {
                GroupName = newMarkAccountTypeParams.GroupName,
                Usernames = string.Join(",", usernames)
            });

            return !result.IsSuccess
                ? result
                : markAccountTypeDao.Add(new AddMarkAccountCommand
            {
                GroupName = newMarkAccountTypeParams.GroupName,
                Usernames = string.Join(",", usernames),
                TypeId = newMarkAccountTypeParams.TypeId,
                Remark = HttpUtility.HtmlDecode(newMarkAccountTypeParams.Remark),
                UserId = newMarkAccountTypeParams.UserId,
                IP = newMarkAccountTypeParams.IP
            });
        }

        public Result MultipleUpdate(IEnumerable<UpdateMarkAccountTypeParams> updateAccountTypes)
        {
            var unmarksUsername = new List<string>();
            var failedUsername = new List<string>();
            foreach (var request in updateAccountTypes)
            {
                var result = SingleUpdate(request);
                if (!result.IsSuccess)
                {
                    if (result.ErrorCode == "99") {
                        unmarksUsername.Add(request.Username);
                    } else {
                        failedUsername.Add(request.Username);
                    }
                } 
            }

            var errorMsg = unmarksUsername.Count > 0 ? UmarkUpdateError + string.Join(", ", unmarksUsername) : "";

            errorMsg += failedUsername.Count > 0 ? FailUpdateError + string.Join(", ", failedUsername) : "";

            if (string.IsNullOrEmpty(errorMsg))
            {
                return new Result();
            }

            return new Result(1, errorMsg);
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
            var result = markAccountTypeDao.Update(new UpdateMarkAccountCommand
            {
                CustID = updateMarkAccountTypeParams.CustId,
                TypeID = updateMarkAccountTypeParams.TypeId,
                Remark = HttpUtility.HtmlDecode(updateMarkAccountTypeParams.Remark),
                IP = updateMarkAccountTypeParams.IP,
                UserID = updateMarkAccountTypeParams.UserId,
            });

            return result;
        }

        public Result Unmark(UnmarkAccountTypeParams parameters)
        {
            var result = markAccountTypeDao.Unmark(new UnmarkMarkAccountCommand
            {
                CustID = parameters.CustId,
                IP = parameters.IP,
                UserID = parameters.UserId,
            });

            return result;
        }

        public Result UnmarkAll(UnmarkAccountTypeParams unmarkAccountTypeParams)
        {
            var errorDeleteUsernames = new List<string>();
            foreach (var account in unmarkAccountTypeParams.Accounts)
            {
                var result = markAccountTypeDao.Unmark(new UnmarkMarkAccountCommand
                {
                    CustID = account.CustId,
                    IP = unmarkAccountTypeParams.IP,
                    UserID = unmarkAccountTypeParams.UserId,
                });

                if (!result.IsSuccess)
                {
                    errorDeleteUsernames.Add(account.Username);
                }
            }

            if(errorDeleteUsernames.Count > 0)
            {
                return new Result("1", string.Join(", ", errorDeleteUsernames));
            }

            return new Result();
        }
    }
}
