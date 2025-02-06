namespace Fanex.ExternalAccount.RewardManagement.DataAccesses
{
    using System.Collections.Generic;
    using Fanex.ExternalAccount.RewardManagement.Criteria;
    using Fanex.ExternalAccount.RewardManagement.Models;
    using Fanex.Common.Extensions;
    using Fanex.ExternalAccount.Core.Shared;
    using Fanex.Data;
    using System;

    public interface IRewardDataAccess
    {
        IEnumerable<Program> GetPrograms(GeProgramCriteria criteria);

        RewardReport GetReport(GetRewardRecordCriteria criteria);
    }

    public class RewardDataAccess : IRewardDataAccess
    {
        public IEnumerable<Program> GetPrograms(GeProgramCriteria criteria)
        {
            var betTypes = new List<Program>();
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                using (var reader = db.ExecuteReader(criteria.GetSpParams()))
                {
                    while (reader.Read())
                    {
                        var entity = new Program()
                        {
                            ProgramId = reader.ToString("resourceid"),
                            ProgramName = reader.ToString("resource_en"),
                        };
                        betTypes.Add(entity);
                    }
                }
            }

            return betTypes;
        }

        public RewardReport GetReport(GetRewardRecordCriteria criteria)
        {
            var rewardRecords = new List<RewardRecord>();

            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                using (var reader = db.ExecuteReader(criteria.GetSpParams()))
                {
                    while (reader.Read())
                    {
                        var entity = new RewardRecord
                        {
                            CreatedDate = reader.ToDateTime("CreatedDate"),
                            ProgramId = reader.ToInt("ProgramId"),
                            MemberUsername = reader.ToString("MemberUsername"),
                            MemberAmount = reader.ToDecimal("MemberAmount"),
                            MemberCurrency = reader.ToString("CurrencyCode"),
                            MemberPaymentStatus = reader.ToBool("MemberPaymentStatus"),
                            MemberPaymentDate = reader.ToDateTime2("MemberPaymentDate"),
                            MemberFeedBack = reader.ToString("MemberFeedBack"),
                            AgentUsername = reader.ToString("AgentUsername"),
                            AgentPaymentStatus = reader.ToBool("AgentPaymentStatus"),
                            AgentPaymentDate = reader.ToDateTime2("AgentPaymentDate"),
                            MasterUsername = reader.ToString("MasterUsername"),
                            MasterPaymentStatus = reader.ToBool("MasterPaymentStatus"),
                            MasterPaymentDate = reader.ToDateTime2("MasterPaymentDate"),
                            SuperUsername = reader.ToString("SuperUsername"),
                            SuperPaymentStatus = reader.ToBool("SuperPaymentStatus"),
                            SuperPaymentDate = reader.ToDateTime2("SuperPaymentDate"),
                            IsDirectMember = reader.ToBool("IsDirectMember"),
                        };
                        rewardRecords.Add(entity);
                    }
                }

                var outputParams = db.GetOuputParams();
                var totalRows = outputParams["totalrecords"] is DBNull
                    ? 0
                    : Convert.ToInt16(outputParams["totalrecords"]);

                return new RewardReport
                {
                    Items = rewardRecords,
                    TotalItems = totalRows
                };
            }
        }
    }
}