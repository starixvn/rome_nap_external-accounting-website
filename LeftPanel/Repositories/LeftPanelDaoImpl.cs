namespace Fanex.ExternalAccount.Module.LeftPanel.Repositories
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Data;
    using System.Web;
    using Core.Context;
    using Criteria;
    using CustomerSetting.Security.Authenticate;
    using Data;
    using Data.DNAide;
    using DTO;
    using Service.Impl.Customer.Casino;
    using Service.Impl.Report;
    using Fanex.ExternalAccount.Core.Shared;
    using Service.Interface.Report;
    using Utilities;
    using ViewModels;

    public class LeftPanelDaoImpl : LeftPanelDao
    {
        private readonly IReportService reportService = new ReportService();

        public DataSet GetCustomerInfo(string username, string groupName = "ACCDDD")
        {
            var customerInfo = reportService.GetCustomerInfo(groupName, 0, username, true);

            if (customerInfo != null && customerInfo.Rows.Count != 0)
            {
                var param = new Hashtable {{"username", username}};

                return DbHelper.ExecuteDataSet("Acc_CustomerSelAllInfo", param);
            }

            return null;
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
            var param = new Hashtable {{"custid", custid}};
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
                        items.Add(new MemberUsedCreditItem
                        {
                            Username = reader.ToString("username"),
                            FirstName = reader.ToString("firstname"),
                            LastName = reader.ToString("lastname"),
                            Amount = reader.ToDecimal("credit"),
                            IsClosed = reader.ToBoolean("closed")
                        });
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
                var param = new Hashtable {{"custid", custId}};
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

        public Result SuspendCustomer(int custId, bool suspend)
        {
            using (var objDb = new ObjectDb("Acc_CustomerUpdSuspended"))
            {
                var param = new Hashtable
                {
                    {"userid", CurrentSession.Instance.UserId},
                    {"custid", custId},
                    {"suspended", suspend},
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

        public Result EnableCasino(int id, bool enable, bool enableDownline, bool sync, string custName)
        {
            var result = CasinoManager.Instance.UpdateCSStatus(
                id,
                sync,
                !enable,
                enableDownline,
                CurrentSession.Instance.UserId,
                CurrentContext.Instance.Ip4Address,
                0,
                "En",
                CurrentSession.Instance.UserName,
                string.Empty);

            return new Result(result ? 0 : 1, result ? string.Empty : "Update casino status failed");
        }

        public Result EnableNumberGame(int id, bool enable, bool enableDownline)
        {
            using (var objDb = new ObjectDb("Acc_CustomerUpdDisableNumber", 600))
            {
                var param = new Hashtable
                {
                    {"userid", CurrentSession.Instance.UserId},
                    {"custid", id},
                    {"disablenumber", !enable},
                    {"language", "E"},
                    {"errcode", 0},
                    {"errmsg", string.Empty},
                    {"ip", CurrentContext.Instance.Ip4Address},
                    {"disabledownline", !enableDownline}
                };

                objDb.AssignParamValues(param);
                objDb.ExecuteNonQuery();

                return new Result(objDb.GetParameterValue("errcode"), objDb.GetParameterValue("errmsg"));
            }
        }

        public Result EnableLiveCasino(int id, bool enable, bool enableDownline)
        {
            using (var db = new ObjectDb("Acc_CustomerUpdDisableLiveCS", 300))
            {
                var param = new Hashtable
                {
                    {"userid", CurrentSession.Instance.UserId},
                    {"custid", id},
                    {"disabledlivecs", !enable},
                    {"language", "E"},
                    {"ip", CurrentContext.Instance.Ip4Address},
                    {"enabledownlineslivecs", enableDownline},
                    {"errcode", 0},
                    {"errmsg", string.Empty}
                };

                db.AssignParamValues(param);
                db.ExecuteNonQuery();

                return new Result(db.GetParameterValue("errcode"), db.GetParameterValue("errmsg"));
            }
        }

        public Result EnableESports(int id, bool enable, bool enableDownline)
        {
            var param = new Hashtable
            {
                {"userid", CurrentSession.Instance.UserId},
                {"custid", id},
                {"disable", !enable},
                {"language", "E"},
                {"ip", CurrentContext.Instance.Ip4Address},
                {"disabledownlines", !enableDownline},
                {"errcode", 0},
                {"errmsg", string.Empty}
            };

            using (var db = new ObjectDb("Acc_CustomerUpdDisableESports", 120))
            {
                db.AssignParamValues(param);
                db.ExecuteNonQuery();

                return new Result(db.GetParameterValue("errcode"), db.GetParameterValue("errmsg"));
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
                objDb.AddOutParameter("ErrCode", DbType.Int32, 8);
                objDb.AddOutParameter("ErrMsg", DbType.String, 1000);
                objDb.ExecuteNonQuery();

                var outputParams = objDb.GetOuputParams();

                return new Result(outputParams["ErrCode"], outputParams["ErrMsg"]);
            }
        }

        public Result EnableHRFixedOdds(int id, bool enable, bool enableDownline)
        {
            var param = new Hashtable
            {
                {"userid", CurrentSession.Instance.UserId},
                {"custid", id},
                {"disablehrfixedodds", !enable},
                {"enabledownlineshrfixedodds", enableDownline},
                {"language", "E"},
                {"ip", CurrentContext.Instance.Ip4Address},
                {"errcode", 0},
                {"errmsg", string.Empty}
            };

            using (var db = new ObjectDb("Acc_CustomerUpdDisableHRFixedOdds", 120))
            {
                db.AssignParamValues(param);
                db.ExecuteNonQuery();
                var errorCode = Utils.ToString(db.GetParameterValue("errcode"));
                var errorMessage = Utils.ToString(db.GetParameterValue("errmsg"));

                return new Result {ErrorCode = errorCode, ErrorMessage = errorMessage};
            }
        }

        public Result UpdateGoldDeluxe(int id, bool enable, bool enableDownline, string custName, int userId)
        {
            using (var objDb = new ObjectDb("Acc_CustomerUpdDisableGoldDeluxe", 600))
            {
                var hash = new Hashtable
                {
                    {"custid", id},
                    {"userid", userId},
                    {"disable", !enable},
                    {"language", "E"},
                    {"ip", CurrentContext.Instance.Ip4Address},
                    {"enabledownlines", enableDownline},
                    {"errcode", 0},
                    {"errmsg", string.Empty}
                };
                objDb.AssignParamValues(hash);

                objDb.ExecuteNonQuery();

                return new Result(objDb.GetParameterValue("errcode"), objDb.GetParameterValue("errmsg"));
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
                            Username = reader.ToString("mastername"),
                            GivenCredit = reader.ToDecimal("givencredit"),
                            UsedCredit = reader.ToDecimal("usedcredit")
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
                            Username = reader.ToString("agentname"),
                            GivenCredit = reader.ToDecimal("givencredit"),
                            UsedCredit = reader.ToDecimal("usedcredit")
                        });
                    }
                }
            }

            return items;
        }

        [ExternalAccountAuthenticate(Function = eFunctions.ClearCredit,
            Policy = AuthenticateAction.AuthenticationType.Host)]
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
    }
}