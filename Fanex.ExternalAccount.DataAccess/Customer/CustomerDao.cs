namespace Fanex.ExternalAccount.DataAccess.Impl.Customer
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using Base;
    using Common;
    using Common.Extensions;
    using Core.Shared;
    using Criteria.Customer;
    using Criteria.Customer.CustomerStatus;
    using Data;
    using Data.DNAide;
    using DTO;
    using DTO.Customer;
    using DTO.Customer.Status;
    using Fanex.ExternalAccount.Utilities;
    using Fanex.ExternalAccount.ViewModel.Customer.Suspend;
    using Interface.Customer;

    public class CustomerDao : DAOBase, ICustomerDao
    {
        public Customer GetCustomerByUserName(string userName, string groupName, int userId)
        {
            var param = new Hashtable
            {
                { "username", userName },
                { "groupname", groupName },
                { "userid", userId }
            };

            using (var reader = ExecDataReader("EAS_CustomerSelAllInfo", param))
            {
                if (reader?.Read() == true)
                {
                    return new Customer
                    {
                        CustId = reader.ToInt("custid")
                    };
                }

                return null;
            }
        }

        public bool IsArchivedCustId(int custId)
        {
            if (custId <= 0)
            {
                return false;
            }

#pragma warning disable S1192 // String literals should not be duplicated
            var parameters = new Hashtable { { "custid", custId } };
#pragma warning restore S1192 // String literals should not be duplicated

            var result = DbHelper.ExecuteDbResult("Acc_GetCustArchiveStatus", parameters);

            return (bool)result.Outputs["isarchived"];
        }

        public Customer UserInfo(int custId, byte settingStatus)
        {
#pragma warning disable S1192 // String literals should not be duplicated
            var param = new Hashtable { { "custid", custId }, { "settingstatus", settingStatus } };
#pragma warning restore S1192 // String literals should not be duplicated

            using (var reader = ExecDataReader("Age_CustomerSelUserInfo", param))
            {
                if (reader?.Read() == true)
                {
                    return new Customer
                    {
                        CustId = reader.ToInt("custid"),
#pragma warning disable S1192 // String literals should not be duplicated
                        RoleId = reader.ToInt("roleid"),
#pragma warning restore S1192 // String literals should not be duplicated
                        Username = Utils.ToString(reader["username"]),
                        FirstName = Utils.ToString(reader["firstname"]),
                        LastName = Utils.ToString(reader["lastname"]),
                        MobilePhone = Utils.ToString(reader["mobilephone"]),
                        Phone = Utils.ToString(reader["phone"]),
                        Email = Utils.ToString(reader["email"]),
                        Fax = Utils.ToString(reader["fax"]),

                        Remark = Utils.ToString(reader["remark"])
                    };
                }

                return null;
            }
        }

        public Result EditUserInfo(Customer customer, string ip, int userId, int subAccId, string language)
        {
            var param = new Hashtable
            {
                {"custid", customer.CustId},
                {"firstname", customer.FirstName??string.Empty},
                {"lastname", customer.LastName??string.Empty},
                {"phone", customer.Phone??string.Empty},
                {"mobilephone", customer.MobilePhone??string.Empty},
                {"email", customer.Email??string.Empty},
                {"fax", customer.Fax??string.Empty},
                {"discasino", 0},
                {"ip", ip},
                {"userid", userId},
                {"subaccid", subAccId},
                {"language", language},
                {"remark", customer.Remark??string.Empty},
                {"errorcode", 0},
                {"errormsg", string.Empty}
            };

            var result = DbHelper.ExecuteDbResult("Age_CustomerUpdUserInfo", param);

            return new Result(result.Outputs["errorcode"], result.Outputs["errormsg"], result.ResultSet);
        }

        public Result ResetPassword(ResetPasswordCriteria criteria)
        {
            var result = DbHelper.ExecuteDbResult(criteria.GetSettingKey(), criteria.GetSpParams());

#pragma warning disable S1192 // String literals should not be duplicated
            return new Result(result.Outputs["errcode"].ToString(), result.Outputs["errmsg"].ToString(),
#pragma warning restore S1192 // String literals should not be duplicated
                result.ResultSet);
        }

        public Result ConfigDoubleComm(UpdateDoubleCommCriteria criteria)
        {
            var result = DbHelper.ExecuteDbResult(criteria.GetSettingKey(), criteria.GetSpParams());

            return new Result(result.Outputs["errcode"].ToString(), result.Outputs["errmsg"].ToString(),
                result.ResultSet);
        }

        public List<SingleUpdateAffectedDownline> GetSingleUpdateSuspendAffectedDownlines(GetSingleSuspendCustomerInfoCriteria criteria)
        {
            using (ObjectDb objDb = new ObjectDb(criteria.GetSettingKey()))
            {
                var listAffectedDownline = new List<SingleUpdateAffectedDownline>();
                objDb.AssignParamValues(criteria.GetSpParams());
                var reader = objDb.ExecuteReader();
                while (reader.Read())
                {
                    listAffectedDownline.Add(new SingleUpdateAffectedDownline(
                        Utilities.Utils.ToInt32(reader["roleid"]),
#pragma warning disable S1192 // String literals should not be duplicated
                        Utilities.Utils.ToInt32(reader["cntAffected"]),
                        Utilities.Utils.ToInt32(reader["cntUnchanged"])));
#pragma warning restore S1192 // String literals should not be duplicated
                }

                return listAffectedDownline;
            }
        }

        public SuspendedCustomerDetailViewModel GetSuspendDownlinesDetail(GetSuspendCustomerDetailInfoCriteria criteria)
        {
            var listDownlines = new List<UnsuspendDownlineDetail>();
            using (var objDb = new ObjectDb(criteria.GetSettingKey()))
            {
                var reader = objDb.ExecuteReader(criteria.GetSpParams());
                while (reader.Read())
                {
                    listDownlines.Add(new UnsuspendDownlineDetail(Utilities.Utils.ToInt32(reader["roleid"]), Utilities.Utils.ToString(reader["username"])));
                }

                reader.Close();
                var outputs = objDb.GetOuputParams();
                return new SuspendedCustomerDetailViewModel(Utilities.Utils.ToInt32(outputs["totalrows"]), listDownlines);
            }
        }

        public Result SuspendCustomer(UpdateSuspendCustomerCriteria criteria)
        {
            using (var objDb = new ObjectDb(criteria.GetSettingKey()))
            {
                objDb.ExecuteNonQuery(criteria.GetSpParams());
                var outputs = objDb.GetOuputParams();
                return new Result(outputs["errcode"].ToString(), outputs["errmsg"].ToString());
            }
        }

        public List<CustomerSuspendStatusRow> GetMultipleSuspendCustomerViewModel(GetMultipleSuspendCustomerInfoCriteria criteria)
        {
            var result = new List<CustomerSuspendStatusRow>();
            using (var objDb = new ObjectDb(criteria.GetSettingKey()))
            {
                var reader = objDb.ExecuteReader(criteria.GetSpParams());
                while (reader.Read())
                {
                    result.Add(new CustomerSuspendStatusRow
                    {
                        CustId = Utilities.Utils.ToInt32(reader["custid"]),
                        RoleId = criteria.RoleId,
                        UserName = Utilities.Utils.ToString(reader["username"]),
                        Suspend = Utilities.Utils.ToBoolean(reader["suspended"])
                    });
                }

                reader.NextResult();
                while (reader.Read())
                {
                    var custid = Utilities.Utils.ToInt32(reader["custid"]);
                    var row = result.FirstOrDefault(m => m.CustId == custid);
                    if (row != null)
                    {
                        var roleid = Utilities.Utils.ToInt32(reader["roleid"]);
                        switch (roleid)
                        {
                            case Role.Master:
                                row.AffectedMaster = Utilities.Utils.ToInt32(reader["cntAffected"]);
                                row.RemainUnchangedMaster = Utilities.Utils.ToInt32(reader["cntUnchanged"]);
                                break;

                            case Role.Agent:
                                row.AffectedAgent = Utilities.Utils.ToInt32(reader["cntAffected"]);
                                row.RemainUnchangedAgent = Utilities.Utils.ToInt32(reader["cntUnchanged"]);
                                break;

                            default:
                                row.AffectedMember = Utilities.Utils.ToInt32(reader["cntAffected"]);
                                row.RemainUnchangedMember = Utilities.Utils.ToInt32(reader["cntUnchanged"]);
                                break;
                        }
                    }
                }
            }

            return result;
        }

        public void KickoutCustomer(KickoutCustomerCriteria criteria)
        {
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                db.AssignParamValues(criteria.GetSpParams());
                db.ExecuteNonQuery();
            }
        }

        public CustomerStatusDTO GetCustomerAllStatus(GetCustomerAllStatusCriteria criteria)
        {
            using (var objDb = new ObjectDb(criteria.GetSettingKey()))
            {
                var result = new CustomerStatusDTO();
                var reader = objDb.ExecuteReader(criteria.GetSpParams());
                if (reader.Read())
                {
                    result.HasSecurityCode = Utils.ToBoolean(reader["HasSecurityCode"]);
                    result.UplineHasSecurityCode = Utils.ToBoolean(reader["UplineHasSecurityCode"]);
                    result.DisableTransfer = Utils.ToBoolean(reader["DisableTransfer"]);
                }

                reader.NextResult();
                while (reader.Read())
                {
                    if (!reader.IsDBNull(reader.GetOrdinal("GroupId")))
                    {
                        var isHidden = Utils.ToBoolean(reader["isHidden"]);
                        var groupId = reader.ToInt32("GroupId");
                        if (!isHidden && !result.ProductGroups.Any(x => x.Id == groupId))
                        {
                            result.ProductGroups.Add(new Product(groupId, groupId, CacheRef.GetProductGroupName(groupId)));
                        }
                    }
                }

                return result;
            }
        }

        public bool CheckLimitNewDownline(CheckLimitNewDownlineCriteria criteria)
        {
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                db.ExecuteNonQuery(criteria.GetSpParams());
                var outputs = db.GetOuputParams();

                return Utils.ToBoolean(outputs["isallowed"]);
            }
        }

        public IEnumerable<SingleOpenAffectedDownline> GetSingleOpenAffectedDownlines(GetSingleOpenCustomerInfoCriteria criteria)
        {
            if (criteria.IsValid())
            {
                using (var db = new ObjectDb(criteria.GetSettingKey()))
                {
                    using (var reader = db.ExecuteReader(criteria.GetSpParams()))
                    {
                        while (reader.Read())
                        {
                            yield return BuildSingleOpenAffectedDownline(reader);
                        }
                    }
                }
            }
        }

        public IEnumerable<CustomerClosedStatusRow> GetCustomerClosedStatusList(GetMultipleOpenCustomerInfoCriteria criteria)
        {
            var result = new List<CustomerClosedStatusRow>();

            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                using (var reader = db.ExecuteReader(criteria.GetSpParams()))
                {
                    while (reader.Read())
                    {
                        result.Add(new CustomerClosedStatusRow
                        {
                            CustId = Utils.ToInt32(reader["custid"]),
                            RoleId = criteria.RoleId,
                            UserName = Utils.ToString(reader["username"]),
                            IsClosed = Utils.ToBoolean(reader["closed"])
                        });
                    }

                    reader.NextResult();

                    while (reader.Read())
                    {
                        var custid = Utils.ToInt32(reader["custid"]);
                        var row = result.FirstOrDefault(m => m.CustId == custid);

                        if (row != null)
                        {
                            var roleid = Utils.ToInt32(reader["roleid"]);
                            switch (roleid)
                            {
                                case Role.Master:
                                    row.AffectedMaster = Utils.ToInt32(reader["cntAffected"]);
                                    row.RemainUnchangedMaster = Utils.ToInt32(reader["cntUnchanged"]);
                                    break;

                                case Role.Agent:
                                    row.AffectedAgent = Utils.ToInt32(reader["cntAffected"]);
                                    row.RemainUnchangedAgent = Utils.ToInt32(reader["cntUnchanged"]);
                                    break;

                                default:
                                    row.AffectedMember = Utils.ToInt32(reader["cntAffected"]);
                                    row.RemainUnchangedMember = Utils.ToInt32(reader["cntUnchanged"]);
                                    break;
                            }
                        }
                    }
                }
            }

            return result;
        }

        public Tuple<int, List<SingleOpenDownline>> GetSingleOpenDownlines(GetSingleOpenDownlinesInfoCriteria criteria)
        {
            var singleOpenDownlines = new List<SingleOpenDownline>();
            var totalRows = 0;

            if (criteria.IsValid())
            {
                using (var db = new ObjectDb(criteria.GetSettingKey()))
                {
                    using (var reader = db.ExecuteReader(criteria.GetSpParams()))
                    {
                        while (reader.Read())
                        {
                            singleOpenDownlines.Add(BuildSingleOpenDownline(reader));
                        }
                    }

                    totalRows = Converter.ToInt(db.GetOuputParams()["totalrows"]);
                }
            }

            return Tuple.Create(totalRows, singleOpenDownlines);
        }

        public Result UpdateCustomerClosedStatus(UpdateClosedStatusCriteria criteria)
        {
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                db.ExecuteNonQuery(criteria.GetSpParams());

                var output = db.GetOuputParams();
                return new Result(
                    output["errcode"].ToString(),
                    output["errmsg"].ToString());
            }
        }

        private static SingleOpenAffectedDownline BuildSingleOpenAffectedDownline(IDataReader reader)
            => new SingleOpenAffectedDownline(
                reader.ToInt("roleid"),
                reader.ToInt("cntAffected"),
                reader.ToInt("cntUnchanged"));

        private static SingleOpenDownline BuildSingleOpenDownline(IDataReader reader)
            => new SingleOpenDownline(
                reader.ToInt("roleid"),
               Utils.ToString(reader["username"]));

        public IEnumerable<CustomerProtectedStatus> GetMultipleProtectedStatus(GetMultipleProtectedStatusCriteria criteria)
        {
            using (var reader = ExecDataReader(criteria.GetSettingKey(), criteria.GetSpParams()))
            {
                while (reader.Read())
                {
                    yield return new CustomerProtectedStatus(
                        Convert.ToInt32(reader["CustId"]),
                        reader["Username"].ToString(),
                        Convert.ToByte(reader["roleid"]),
                        Convert.ToBoolean(reader["IsProtectedMode"]));
                }
            }
        }

        public Result UpdateProtectedStatus(UpdateProtectedStatusCriteria criteria)
            => ExecuteNonQuery(criteria);

        public Result CheckMultipleSupersBetSetting(CheckMultipleSupersBetSettingCriteria criteria)
        {
            if (!criteria.IsValid())
            {
                return new Result(1, "Please input all required fields.");
            }

            var result = new Result();
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                using (var reader = db.ExecuteReader(criteria.GetSpParams()))
                {
                    while (reader != null && reader.Read())
                    {
                        result.ResultData += reader.ToInt32("CustID") + ",";
                    }
                }

                var output = db.GetOuputParams();
                result.ErrorCode = output["errcode"].ToString();
                result.ErrorMessage = output["errmsg"].ToString();
                result.ResultData = result.ResultData.ToString().TrimEnd(new[] { ',' });
            }

            return result;
        }
    }
}