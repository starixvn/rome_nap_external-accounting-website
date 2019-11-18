namespace Fanex.ExternalAccount.Module.LeftPanel.DataAccesses
{
    using Core.Context;
    using Core.Shared;
    using Criteria;
    using Data;
    using Data.DNAide;
    using DTO;
    using Extensions;
    using Fanex.ExternalAccount.Module.LeftPanel.Models;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Data;
    using System.Web;
    using Utilities;
    using ViewModels;
    using FanexCommonConverter = Fanex.Common.Converter;

#pragma warning disable S1192 // String literals should not be duplicated
    public class CustomerDataAccessImpl : CustomerDataAccess
    {
        public DataSet GetPersonalInfo(string username, string groupName, int userId)
        {
            var param = new Hashtable
            {
                { "username", username },
                { "groupname", groupName },
                { "userid", userId }
            };

            return DbHelper.ExecuteDataSet("EAS_CustomerSelAllInfo", param);
        }

        public DataSet GetLoginInfo(int custId, string groupName)
        {
            var param = new Hashtable
            {
                { "custid", custId },
                { "groupname", groupName }
            };

            return DbHelper.ExecuteDataSet("EAS_CustomerSelLoginInfo", param);
        }

        public DataSet GetCustomerStatusInfo(int custId)
        {
            var param = new Hashtable
            {
                { "custid", custId }
            };

            return DbHelper.ExecuteDataSet("EAS_CustomerSelAllStatus", param);
        }

        public DataSet GetBalanceInfo(int custId)
        {
            var param = new Hashtable
            {
                { "custid", custId }
            };

            return DbHelper.ExecuteDataSet("EAS_CustomerSelBalanceInfo", param);
        }

        public Result UpdateGivenCredit(int id, int uplineId, decimal newGivenCredit)
        {
            using (var objDb = new ObjectDb("Acc_CustomerUpdCredit"))
            {
                var param = new Hashtable
                {
                    {"userid", uplineId},
                    {"custid", id},
                    {"newcredit", newGivenCredit},
                    {"ip", CurrentContext.Instance.Ip4Address},
                    {"errcode", 0},
                    {"errmsg", string.Empty}
                };

                objDb.AssignParamValues(param);

                objDb.ExecuteNonQuery();

                return new Result(objDb.GetParameterValue("errcode"), objDb.GetParameterValue("errmsg"));
            }
        }

        public DbResult GetDownlineList(int custid, int pageSize, int pageIndex)
        {
            var param = new Hashtable
            {
                {"custid", custid},
                {"pagesize", pageSize},
                {"pageindex", pageIndex},
                {"totalpages", 0}
            };

            return DbHelper.ExecuteDbResult("Acc_CustomerSelList", param);
        }

        public DataRowCollection GetOnlineList(int custid)
        {
            var param = new Hashtable { { "custid", custid } };
            var rs = DbHelper.ExecuteDbResult("Acc_CustomerSelOnline", param);

            return rs.ResultSet.Tables[0].Rows;
        }

        public IList<MasterAgentUsedCreditItem> MasterAgentUsedCreditItems(int custId, int role)
        {
            if (role.IsSuper())
            {
                return MasterUsedCreditItems(custId);
            }

            return role.IsMaster() ? AgentUsedCreditItems(custId) : null;
        }

        public IList<MemberUsedCreditItem> MemberUsedCreditItems(int custId)
        {
            IList<MemberUsedCreditItem> items = new List<MemberUsedCreditItem>();

            using (var db = new ObjectDb("Acc_CustomerSelUsedCredit_Ag"))
            {
                db.AddInParameter("agentid", DbType.Int32, custId);

                using (var reader = db.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        items.Add(new MemberUsedCreditItem(reader));
                    }
                }
            }

            return items;
        }

        public Result ResetMemberCredit(int custId, int agentId, bool closedOnly)
        {
            using (var objDb = new ObjectDb("Acc_CustomerUpdResetMemberCredit", 600))
            {
                var param = new Hashtable
                {
                    {"userid", custId},
                    {"agentid", agentId},
                    {"closedonly", closedOnly},
                    {"ip", HttpContext.Current.Request.UserHostAddress},
                    {"system", "EXTERNAL"},
                    {"errcode", 0},
                    {"errmsg", string.Empty}
                };

                objDb.AssignParamValues(param);

                objDb.ExecuteNonQuery();

                var errorCode = Utils.ToInt32(objDb.GetParameterValue("errcode"));

                return new Result(errorCode, objDb.GetParameterValue("errmsg"));
            }
        }

        public Result CustomerInsertClearBalance(int custId, string custIds, bool isCheckedAll, string ip)
        {
            using (var db = new ObjectDb("Acc_CustomerInsClearBalance"))
            {
                var param = new Hashtable
                {
                    {"fromcustid", custId},
                    {"tocustids", custIds},
                    {"userid", CurrentSession.Instance.UserId},
                    {"language", "E"},
                    {"errcode", 0},
                    {"errmsg", string.Empty},
                    {"cusstatus", !isCheckedAll ? string.Empty : "All"},
                    {"ip", ip},
                    {"system", "EXTERNAL"}
                };

                db.AssignParamValues(param);
                db.ExecuteNonQuery();

                return new Result
                {
                    ErrorCode = Utils.ToString(db.GetParameterValue("errcode")),
                    ErrorMessage = Utils.ToString(db.GetParameterValue("errmsg"))
                };
            }
        }

        public DataTable CustomerSelectClearBalance(int custId)
        {
            using (var db = new ObjectDb("Acc_CustomerSelClearBalance"))
            {
                var param = new Hashtable { { "custid", custId } };
                var result = db.ExecuteDataSet(param);

                return result?.Tables?[0];
            }
        }

        public Result CloseCutomer(int custId, bool close, bool clearCredit, string custName)
        {
            using (var objDb = new ObjectDb("Acc_CustomerUpdClosed", 600))
            {
                var param = new Hashtable
                {
                    {"userid", CurrentSession.Instance.UserId},
                    {"custid", custId},
                    {"closed", close},
                    {"language", "E"},
                    {"errcode", 0},
                    {"errmsg", string.Empty},
                    {"ip", CurrentContext.Instance.Ip4Address},
                    {"systemid", Config.SystemId}
                };

                objDb.AssignParamValues(param);

                objDb.ExecuteNonQuery();

                if (Utils.ToInt32(objDb.GetParameterValue("errcode")) <= 0 && clearCredit)
                {
                    return ClearCredit(CurrentSession.Instance.UserId, custName);
                }

                return new Result(objDb.GetParameterValue("errcode"), objDb.GetParameterValue("errmsg"));
            }
        }

        public Result AllowOutright(int custId, bool allow)
        {
            using (var objDb = new ObjectDb("Acc_CustomerUpdDisableOutright", 600))
            {
                var param = new Hashtable
                {
                    {"userid", CurrentSession.Instance.UserId},
                    {"custid", custId},
                    {"disableoutright", !allow},
                    {"language", "E"},
                    {"errcode", 0},
                    {"errmsg", string.Empty},
                    {"ip", CurrentContext.Instance.Ip4Address}
                };

                objDb.AssignParamValues(param);
                objDb.ExecuteNonQuery();

                return new Result(objDb.GetParameterValue("errcode"), objDb.GetParameterValue("errmsg"));
            }
        }

        public Result AutoMasterPT(int id, bool allow)
        {
            var param = new Hashtable
            {
                {"userid", CurrentSession.Instance.UserId},
                {"custid", id},
                {"disableautompt", !allow},
                {"language", "E"},
                {"errcode", 0},
                {"errmsg", string.Empty},
                {"ip", CurrentContext.Instance.Ip4Address}
            };

            using (var objDb = new ObjectDb("Acc_CustomerUpdMaster_DisableAutoMPT"))
            {
                objDb.AssignParamValues(param);
                objDb.ExecuteNonQuery();

                return new Result(objDb.GetParameterValue("errcode"), objDb.GetParameterValue("errmsg"));
            }
        }

        public Result ResetSecurityCode(int id, string code, bool force)
        {
            using (var objDb = new ObjectDb("Acc_CustomerUpdSecurityCode"))
            {
                objDb.AddInParameter("Custid", DbType.Int32, id);
                objDb.AddInParameter("Userid", DbType.Int32, CurrentSession.Instance.UserId);
                objDb.AddInParameter("IsForcedChangeSecurityCode", DbType.Boolean, force);
                objDb.AddInParameter("Securitycode", DbType.String, code);
                objDb.AddInParameter("Language", DbType.String, "En");
                objDb.AddInParameter("IP", DbType.String, CurrentContext.Instance.Ip4Address);
                objDb.AddInParameter("system", DbType.String, "EXTERNAL");
#pragma warning disable S109 // Magic numbers should not be used
                objDb.AddOutParameter("ErrCode", DbType.Int32, 8);
                objDb.AddOutParameter("ErrMsg", DbType.String, 1000);
#pragma warning restore S109 // Magic numbers should not be used
                objDb.ExecuteNonQuery();

                var outputParams = objDb.GetOuputParams();

                return new Result(outputParams["ErrCode"], outputParams["ErrMsg"]);
            }
        }

        public Result RemoveOTP(RemoveOtpCriteria criteria)
        {
            if (criteria == null)
            {
                throw new ArgumentNullException(nameof(criteria));
            }

            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                db.ExecuteNonQuery(criteria.GetSpParams());
                var outputParams = db.GetOuputParams();

                return new Result
                {
                    ErrorCode = outputParams["errcode"].ToString(),
                    ErrorMessage = outputParams["errmsg"].ToString()
                };
            }
        }

        private IList<MasterAgentUsedCreditItem> MasterUsedCreditItems(int custId)
        {
            var items = new List<MasterAgentUsedCreditItem>();

            using (var db = new ObjectDb("Acc_CustomerSelUsedCredit_SAg"))
            {
                db.AddInParameter("superid", DbType.Int32, custId);

                using (var reader = db.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        items.Add(new MasterAgentUsedCreditItem
                        {
                            Username = reader.MasterName(),
                            GivenCredit = reader.GivenCredit(),
                            UsedCredit = reader.UsedCredit()
                        });
                    }
                }
            }

            return items;
        }

        private IList<MasterAgentUsedCreditItem> AgentUsedCreditItems(int custId)
        {
            var items = new List<MasterAgentUsedCreditItem>();

            using (var db = new ObjectDb("Acc_CustomerSelUsedCredit_MAg"))
            {
                db.AddInParameter("masterid", DbType.Int32, custId);

                using (var reader = db.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        items.Add(new MasterAgentUsedCreditItem
                        {
                            Username = reader.AgentName(),
                            GivenCredit = reader.GivenCredit(),
                            UsedCredit = reader.UsedCredit()
                        });
                    }
                }
            }

            return items;
        }

        private static Result ClearCredit(int userId, string username = null)
        {
            using (var objDb = new ObjectDb("Acc_CustomerUpdClearCredit", 600))
            {
                var hash = new Hashtable
                {
                    {"username", username},
                    {"userid", userId},
                    {"returnerrcode", 1},
                    {"errcode", 0},
                    {"errmsg", string.Empty},
                    {"ip", CurrentContext.Instance.Ip4Address},
                    {"system", "EXTERNAL"}
                };
                objDb.AssignParamValues(hash);

                objDb.ExecuteNonQuery();

                return new Result(objDb.GetParameterValue("errcode"), objDb.GetParameterValue("errmsg"));
            }
        }

        public DataTable EAS_CustomerSelUserInfo(string groupname, int custId, string userName, bool isGetUplineInfo)
        {
            using (var db = new ObjectDb("EAS_CustomerSelUserInfo"))
            {
                if (custId != 0)
                {
                    db.AddInParameter("custid", DbType.Int32, custId);
                }

                db.AddInParameter("groupname", DbType.String, groupname);
                db.AddInParameter("username", DbType.String, userName);
                db.AddInParameter("isgetuplineinfo", DbType.Boolean, isGetUplineInfo);

                var ds = db.ExecuteDataSet();

                return ds?.Tables.Count > 0 ? ds.Tables[0] : null;
            }
        }

        public MemberBalanceInfoModel GetMemberBalanceModel(int custId)
        {
            using (ObjectDb objdb = new ObjectDb("Acc_BalanceMemberSel", 180))
            {
                var param = new Hashtable();
                param.Add("custid", custId);

                objdb.AssignParamValues(param);
                objdb.ExecuteNonQuery();
                return new MemberBalanceInfoModel(
                    FanexCommonConverter.ToDecimal(objdb.GetParameterValue("balance")),
                    FanexCommonConverter.ToDecimal(objdb.GetParameterValue("netposition")));
            }
        }

        public decimal GetMemberOustanding(int custId)
        {
            using (ObjectDb objdb = new ObjectDb("Acc_OutstandingMemberSel", 180))
            {
                var param = new Hashtable();
                param.Add("custid", custId);

                objdb.AssignParamValues(param);
                objdb.ExecuteNonQuery();
                return FanexCommonConverter.ToDecimal(objdb.GetParameterValue("outstanding"));
            }
        }
    }
#pragma warning restore S1192 // String literals should not be duplicated
}