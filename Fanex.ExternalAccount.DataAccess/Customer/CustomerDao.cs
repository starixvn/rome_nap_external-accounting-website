namespace Fanex.ExternalAccount.DataAccess.Impl.Customer
{
    using System;
    using System.Collections;
    using Base;
    using Common;
    using Common.Extensions;
    using Criteria.Customer;
    using Data.DNAide;
    using DTO;
    using DTO.Customer;
    using Interface.Customer;
    using Data;

    public class CustomerDao : DAOBase, ICustomerDao
    {
        public bool IsValidUsername(string userName, int uplineId)
        {
            if (string.IsNullOrEmpty(userName))
            {
                return false;
            }

            var parameters = new Hashtable { { "username", userName }, { "isexists", false } };

            var result = DbHelper.ExecuteDbResult("Age_CustomerCheckExists", parameters);

            return !(bool)result.Outputs["isexists"];
        }

        public Customer UserInfo(int custId)
        {
            var param = new Hashtable { { "custid", custId } };

            using (var reader = ExecDataReader("Age_CustomerSelUserInfo", param))
            {
                if (reader?.Read() == true)
                {
                    return new Customer
                    {
                        CustId = reader.ToInt("custid"),
                        RoleId = reader.ToInt("roleid"),
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

            return new Result(result.Outputs["errcode"].ToString(), result.Outputs["errmsg"].ToString(),
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
    }
}