namespace Fanex.ExternalAccount.DataAccess.Impl.Customer
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using Base;
    using Common.Extensions;
    using Core.Shared;
    using Criteria.Customer;
    using Criteria.Customer.CustomerStatus;
    using Data;
    using Data.DNAide;
    using DTO;
    using DTO.Customer;
    using DTO.Customer.Status;
    using Interface.Customer;
    using ViewModel.Customer.Suspend;

    public class CustomerDao : DAOBase, ICustomerDao
    {
        public bool IsValidUsername(string userName, int uplineId)
        {
            if (string.IsNullOrEmpty(userName))
            {
                return false;
            }

#pragma warning disable S1192 // String literals should not be duplicated
            var parameters = new Hashtable { { "username", userName }, { "isexists", false } };
#pragma warning restore S1192 // String literals should not be duplicated

            var result = DbHelper.ExecuteDbResult("Age_CustomerCheckExists", parameters);

            return !(bool)result.Outputs["isexists"];
        }

        public Customer UserInfo(int custId)
        {
#pragma warning disable S1192 // String literals should not be duplicated
            var param = new Hashtable { { "custid", custId } };
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
                        Username = reader.ToString("username"),
                        FirstName = reader.ToString("firstname"),
                        LastName = reader.ToString("lastname"),
                        MobilePhone = reader.ToString("mobilephone"),
                        Phone = reader.ToString("phone"),
                        Email = reader.ToString("email"),
                        Fax = reader.ToString("fax"),

                        Remark = reader.ToString("remark")
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

        public Result CheckNewCustomerIsReachLimitation(CheckNewCustomerLimitationCriteria criteria)
        {
            var result = new Result();
            using (ObjectDb objDb = new ObjectDb(criteria.GetSettingKey()))
            {
                objDb.AssignParamValues(criteria.GetSpParams());
                objDb.ExecuteNonQuery();
                var outputs = objDb.GetOuputParams();
                result.ErrorCode = outputs["errcode"].ToString();
                result.ErrorMessage = outputs["errmsg"].ToString();
            }

            return result;
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

        public Result SuspendCustomer(UpdateSuspendCustomerCriteria criteria)
        {
            using (var objDb = new ObjectDb(criteria.GetSettingKey()))
            {
                objDb.ExecuteNonQuery(criteria.GetSpParams());
                var outputs = objDb.GetOuputParams();
                return new Result(outputs["errcode"].ToString(), outputs["errmsg"].ToString());
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
                        switch(roleid)
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
    }
}