namespace Fanex.ExternalAccount.HeadlineBonus.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Fanex.ExternalAccount.DTO;
    using Fanex.ExternalAccount.HeadlineBonus.Criteria;
    using Fanex.ExternalAccount.HeadlineBonus.DataAccesses;
    using Fanex.ExternalAccount.HeadlineBonus.Models;
    using Fanex.ExternalAccount.HeadlineBonus.Resources;

    internal interface IBonusDetailService
    {
        BonusDetailReport GetBonusDetailReport(
            string groupName,
            string username,
            DateTime fromDate,
            DateTime toDate,
            byte? statusId,
            bool? isComplained,
            int pageIndex,
            int pageSize);

        Result CancelPending(string groupName, int custId, long transId, int userId, string ip);

        Result AddNew(string groupName, string usernames, decimal amount, int userId, string ip);
    }

    internal class BonusDetailService : IBonusDetailService
    {
        private readonly IBonusDetailDataAccess dataAccess;

        public BonusDetailService(IBonusDetailDataAccess dataAccess = null)
        {
            this.dataAccess = dataAccess ?? new BonusDetailDataAccess();
        }

        public BonusDetailReport GetBonusDetailReport(
            string groupName,
            string username,
            DateTime fromDate,
            DateTime toDate,
            byte? statusId,
            bool? isComplained,
            int pageIndex,
            int pageSize)
        {
            var criteria = new GetMemberBonusCriteria(groupName, username, fromDate, toDate, statusId, isComplained, pageIndex, pageSize);
            var rawData = dataAccess.GetMemberBonus(criteria);

            return GetBonusDetailReport(rawData, criteria.TotalRows);
        }

        private static BonusDetailReport GetBonusDetailReport(IEnumerable<MemberBonusEntity> rawData, int totalItems)
        {
            var totalAmount = 0M;
            var superBonusList = new List<SuperBonus>();
            SuperBonus superBonus;

            rawData
                .GroupBy(b => new { b.CreatedDate.Date, b.SuperName })
                .ToList()
                .ForEach(grp =>
                {
                    superBonus = GetSuperBonus(grp.Key.SuperName, grp);
                    superBonusList.Add(superBonus);

                    totalAmount += superBonus.TotalAmount;
                });

            return new BonusDetailReport(superBonusList, totalAmount, totalItems);
        }

        private static SuperBonus GetSuperBonus(string superName, IEnumerable<MemberBonusEntity> entities)
        {
            var totalAmount = 0M;
            var memberBonusList = new List<MemberBonus>();
            MemberBonus memberBonus;

            entities
                .ToList()
                .ForEach(item =>
                {
                    memberBonus = new MemberBonus(item);
                    memberBonusList.Add(memberBonus);

                    if (!memberBonus.IsCancelled)
                    {
                        totalAmount += item.Amount;
                    }
                });

            return new SuperBonus(superName, totalAmount, memberBonusList);
        }

        public Result CancelPending(string groupName, int custId, long transId, int userId, string ip)
        { 
            var result = dataAccess.CancelPendingBonus(new CancelPendingBonusCriteria(groupName, custId, transId, userId, ip));

            if (result.IsSuccess)
            {
                result.ErrorMessage = ViewLang.CancelBonusSuccessfully;
            }

            return result;
        }

        public Result AddNew(string groupName, string usernames, decimal amount, int userId, string ip)
        {
            var result = ValidateUsernames(usernames);

            if (result.IsSuccess)
            {
                result = dataAccess.AddNewBonus(new AddNewBonusCriteria(groupName, result.ErrorMessage, amount, userId, ip));

                if (result.IsSuccess)
                {
                    result.ErrorMessage = ViewLang.AddNewBonusSuccessfully;
                }
            }

            return result;
        }

        private static Result ValidateUsernames(string usernames)
        {
            const int errorCode = 1001;
            const int maxUsernamesLength = 100;

            var builder = new StringBuilder(usernames);
            builder
                .Replace("\n", ",")
                .Replace("\r", ",")
                .Replace("\t", ",");
            var newUsernames = builder.ToString();

            if (newUsernames.IndexOf('\\') > -1)
            {
                return new Result(errorCode, ViewLang.UsernamesContainBackslashError);
            }

            var newUsernameArray = newUsernames
                .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                .Where(u => u.Trim().Length > 0)
                .Select(u => u.Trim())
                .ToList();

            if (newUsernameArray.Count > maxUsernamesLength)
            {
                return new Result(errorCode, string.Format(ViewLang.UsernamesTooManyError, maxUsernamesLength));
            }

            var duplicateUsernames = newUsernameArray
                .GroupBy(u => u)
                .Where(group => group.Count() > 1)
                .Select(group => group.Key)
                .ToList();

            if (duplicateUsernames.Count > 0)
            {
                return new Result(errorCode, string.Format(ViewLang.UsernameDuplicateError, string.Join(", ", duplicateUsernames)));
            }

            return new Result(0, string.Join(",", newUsernameArray));
        }
    }
}
