namespace Fanex.ExternalAccount.DataAccess.Impl.Customer
{
    using System.Collections;
    using Base;
    using Common;
    using Common.Extensions;
    using Data.DNAide;
    using DTO;
    using DTO.Customer;
    using Interface.Customer;

    public class CustomerDao : DAOBase, ICustomerDao
    {
        public bool IsValidUsername(string userName, int uplineId, out int custId)
        {
            custId = 0;

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
                        HomePhone = reader.ToString("phone"),
                        Email = reader.ToString("email"),
                        Fax = reader.ToString("fax"),

                        Remark = reader.ToString("remark")
                    };
                }

                return null;
            }
        }

        public Result EditUserInfo(
            int custId,
            string firstName,
            string lastName,
            string homePhone,
            string mobilePhone,
            string email,
            string fax,
            string remark,
            string ip,
            int userId,
            int subAccId,
            string language)
        {
            var param = new Hashtable
            {
                {"custid", custId},
                {"firstname", firstName},
                {"lastname", lastName},
                {"phone", homePhone},
                {"mobilephone", mobilePhone},
                {"email", email},
                {"fax", fax},
                {"discasino", 0},
                {"ip", ip},
                {"userid", userId},
                {"subaccid", subAccId},
                {"language", language},
                {"remark", remark},
                {"errorcode", 0},
                {"errormsg", string.Empty}
            };

            var result = DbHelper.ExecuteDbResult("Age_CustomerUpdUserInfo", param);

            return new Result(result.Outputs["errorcode"], result.Outputs["errormsg"], result.ResultSet);
        }

        public Result ResetPassword(int custId, string password, Request request)
        {
            var param = new Hashtable
            {
                {"custid", custId},
                {"userpwd", Cryptography.CfsEnCode(password)},
                {"userid", request.UserId},
                {"discasino", 0},
                {"language", request.Language},
                {"subaccid", request.SubAccId},
                {"loginip", request.IP},
                {"errcode", 0},
                {"errmsg", string.Empty}
            };

            var result = DbHelper.ExecuteDbResult("Age_CustomerUpdPwd", param);

            return new Result(result.Outputs["errcode"].ToString(), result.Outputs["errmsg"].ToString(),
                result.ResultSet);
        }

        public Result ConfigDoubleComm(int custId, bool allow, Request request)
        {
            var param = new Hashtable
            {
                {"custid", custId},
                {"doublecommlosing", allow},
                {"userid", request.UserId},
                {"language", request.Language},
                {"subaccid", request.SubAccId},
                {"ip", request.IP},
                {"errcode", 0},
                {"errmsg", string.Empty}
            };

            var result = DbHelper.ExecuteDbResult("Acc_CustomerUpdDoubleCommLosing", param);

            return new Result(result.Outputs["errcode"].ToString(), result.Outputs["errmsg"].ToString(),
                result.ResultSet);
        }
    }
}