namespace Fanex.ExternalAccount.ProblemAccount.DataAccesses
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using Fanex.Data;
    using Fanex.ExternalAccount.DTO;
    using Fanex.ExternalAccount.ProblemAccount.Criteria;
    using Fanex.ExternalAccount.ProblemAccount.Models;
    using Fanex.NAP.Framework.Data;

    public interface IProblemAccountDataAccess
    {
        IEnumerable<ProblemAccountEntity> GetProblemAccounts(GetProblemAccountCriteria criteria);

        Result SetToReviewed(ICriteria criteria);
    }

    public class ProblemAccountDataAccess : IProblemAccountDataAccess
    {
        public IEnumerable<ProblemAccountEntity> GetProblemAccounts(GetProblemAccountCriteria criteria)
        {
            var problemAccounts = new List<ProblemAccountEntity>();

            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                using (var reader = db.ExecuteReader(criteria.GetSpParams()))
                {
                    while (reader.Read())
                    {
                        problemAccounts.Add(GetProblemAccountEntity(reader));
                    }
                }

                criteria.TotalRows = Convert.ToInt32(db.GetOuputParams()["totalrows"]);
            }

            return problemAccounts;
        }

        private static ProblemAccountEntity GetProblemAccountEntity(IDataReader reader)
            => new ProblemAccountEntity(
                Convert.ToInt32(reader["custid"]),
                reader["username"].ToString(),
                reader["firstname"].ToString(),
                reader["lastname"].ToString(),
                Convert.ToInt32(reader["numberofdownline"]),
                Convert.ToByte(reader["status"]),
                Convert.ToDateTime(reader["createddate"]),
                Convert.ToBoolean(reader["isreviewed"]),
                Convert.ToByte(reader["closedreason"]),
                Convert.ToByte(reader["locked"]),
                Convert.ToInt32(reader["agentid"]));

        public Result SetToReviewed(ICriteria criteria)
        {
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                db.ExecuteNonQuery(criteria.GetSpParams());
                var ouputParams = db.GetOuputParams();

                return new Result(ouputParams["errcode"], ouputParams["errmsg"]);
            }
        }
    }
}
