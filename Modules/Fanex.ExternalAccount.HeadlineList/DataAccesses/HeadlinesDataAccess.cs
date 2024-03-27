using System;
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
        IEnumerable<GroupHeadline> GetGroupHeadlines(GetHeadlinesCriteria criteria);
    }

    public class HeadlinesDataAccess : IHeadlinesDataAccess
    {
        public IEnumerable<GroupHeadline> GetGroupHeadlines(GetHeadlinesCriteria criteria)
        {
            IDictionary<int, GroupHeadline> groupHeadlinesHash = new Dictionary<int, GroupHeadline>(1000);

            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                using (var reader = db.ExecuteReader(criteria.GetSpParams()))
                {
                    HeadlineEntity entity;
                    GroupHeadline groupHeadline;

                    while (reader.Read())
                    {
                        entity = ParseHeadlineEntity(reader);

                        if (!groupHeadlinesHash.TryGetValue(entity.GroupId, out groupHeadline))
                        {
                            groupHeadline = new GroupHeadline
                            {
                                GroupId = entity.GroupId,
                                GroupName = entity.GroupName,
                                HeadlineEntities = new List<HeadlineEntity>(1000)
                            };

                            groupHeadlinesHash.Add(entity.GroupId, groupHeadline);
                        }

                        groupHeadline.HeadlineEntities.Add(entity);
                    }
                }
            }
            return groupHeadlinesHash.Values;
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