using System.Collections.Generic;
using System.Data;
using Fanex.CustomerSetting.Module.Core;
using Fanex.ExternalAccount.HeadlineList.Criteria;
using Fanex.Data;
using Fanex.ExternalAccount.HeadlineList.Models;

namespace Fanex.ExternalAccount.HeadlineList.DataAccesses
{
    public interface IHeadlinesDataAccess
    {
        GetGroupHeadlinesResult GetGroupHeadlines(GetHeadlinesCriteria criteria);
    }

    public class HeadlinesDataAccess : IHeadlinesDataAccess
    {
        public GetGroupHeadlinesResult GetGroupHeadlines(GetHeadlinesCriteria criteria)
        {
            IDictionary<int, GroupHeadline> groupHeadlinesHash = new Dictionary<int, GroupHeadline>();
            var countHeadline = 0;
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                using (var reader = db.ExecuteReader(criteria.GetSpParams()))
                {
                    HeadlineEntity entity;
                    GroupHeadline groupHeadline;

                    while (reader.Read())
                    {
                        entity = ParseHeadlineEntity(reader);
                        countHeadline++;
                        if (!groupHeadlinesHash.TryGetValue(entity.GroupId, out groupHeadline))
                        {
                            groupHeadline = new GroupHeadline
                            {
                                GroupId = entity.GroupId,
                                GroupName = entity.GroupName,
                                HeadlineEntities = new List<HeadlineEntity>()
                            };

                            groupHeadlinesHash.Add(entity.GroupId, groupHeadline);
                        }

                        groupHeadline.HeadlineEntities.Add(entity);
                    }
                }
            }

            return new GetGroupHeadlinesResult
            {
                GroupHeadlines = groupHeadlinesHash.Values,
                TotalHeadlines = countHeadline
            };
        }


        private HeadlineEntity ParseHeadlineEntity(IDataReader reader)
        {
            var entity = new HeadlineEntity()
            {
                GroupId = reader.ToInt32("custheadlineid"),
                GroupName = reader.ToString("groupheadline"),
                CustId = reader.ToInt32("custid"),
                Username = reader.ToString("username"),
                Status = reader.ToString("status"),
            };
            return entity;
        }
    }
}