using System.Linq;

namespace Fanex.ExternalAccount.HeadlineList.DataAccesses
{
    using System.Collections.Generic;
    using System.Data;
    using Fanex.CustomerSetting.Module.Core;
    using Criteria;
    using Data;
    using Models;
    using Fanex.Data.Repository;
    public interface IHeadlinesDataAccess
    {
        GroupsHeadline GetGroupHeadlines(GetHeadlinesCriteria criteria);
    }

    public class HeadlinesDataAccess : IHeadlinesDataAccess
    {
        private readonly IDynamicRepository repository;
        public HeadlinesDataAccess(IDynamicRepository repository) => this.repository = repository ?? new DynamicRepository();
        public GroupsHeadline GetGroupHeadlines(GetHeadlinesCriteria criteria)
        {
            IDictionary<int, GroupHeadline> groupHeadlinesHash = new Dictionary<int, GroupHeadline>();
            var countHeadline = 0;
            // using (var db = new ObjectDb(criteria.GetSettingKey()))
            // {
            //     using (var reader = db.ExecuteReader(criteria.GetSpParams()))
            //     {
            //         HeadlineEntity entity;
            //         GroupHeadline groupHeadline;
            //
            //         while (reader.Read())
            //         {
            //             entity = ParseHeadlineEntity(reader);
            //             countHeadline++;
            //             if (!groupHeadlinesHash.TryGetValue(entity.GroupId, out groupHeadline))
            //             {
            //                 groupHeadline = new GroupHeadline
            //                 {
            //                     GroupId = entity.GroupId,
            //                     GroupName = entity.GroupName,
            //                     HeadlineEntities = new List<HeadlineEntity>()
            //                 };
            //
            //                 groupHeadlinesHash.Add(entity.GroupId, groupHeadline);
            //             }
            //
            //             groupHeadline.HeadlineEntities.Add(entity);
            //         }
            //     }
            // }

            // return new GroupsHeadline
            // {
            //     Groups = groupHeadlinesHash.Values,
            //     TotalHeadlines = countHeadline
            // };

            var dbResult = repository.Fetch<HeadlineEntity>(criteria);
            var headlineGroups = dbResult
                .GroupBy(x => x.GroupId)
                .Select(x => new GroupHeadline{GroupId = x.Key, GroupName = x.First().GroupName, HeadlineEntities = x.ToList()}).ToList();
            
            return new GroupsHeadline
            {
                Groups = headlineGroups,
                TotalHeadlines = countHeadline
            };
        }


        // private HeadlineEntity ParseHeadlineEntity(IDataReader reader)
        // {
        //     var isSuspend = reader.ToBoolean("issuspend");
        //     var isClosed = reader.ToBoolean("isclosed");
        //     var isEnable = reader.ToBoolean("isenable");
        //     var statusMapping = !isEnable ? "Disabled" : isClosed ? "Closed" : isSuspend ? "Suspended" : "Open";
        //
        //     var entity = new HeadlineEntity
        //     {
        //         GroupId = reader.ToInt32("custheadlineid"),
        //         GroupName = reader.ToString("groupheadline"),
        //         CustId = reader.ToInt32("custid"),
        //         Username = reader.ToString("username"),
        //         Status = statusMapping,
        //         IsSuspend = isSuspend,
        //         IsClosed = isClosed,
        //         IsEnable = isEnable,
        //         ClosedReason = reader.ToByte("closedreason"),
        //     };
        //     return entity;
        // }
    }
}