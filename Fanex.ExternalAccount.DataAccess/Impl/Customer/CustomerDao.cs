// --------------------------------------------------------------------------
// <copyright file="CustomerDao.cs" company="">
// Copyright (c) . All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Andy.Nguyen" date="2012.12.25">Created</change>
// </history>

namespace Fanex.ExternalAccount.DataAccess.Impl.Customer
{
    using System;
    using System.Collections;
    using Fanex.Common;
    using Fanex.Common.Extensions;
    using Fanex.Data;
    using Fanex.Data.DNAide;
    using Fanex.ExternalAccount.DataAccess.Impl.Base;
    using Fanex.ExternalAccount.DataAccess.Interface.Customer;
    using Fanex.ExternalAccount.DTO;
    using Fanex.ExternalAccount.DTO.Customer;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class CustomerDao : DAOBase, ICustomerDao
    {
        #region ICustomerDao Members

        #region Search&Copy

        /// <summary>
        /// Checks belonging username.
        /// </summary>
        /// <param name="userName">Username to check.</param>
        /// <param name="uplineId">Id of upper user.</param>
        /// <param name="custId">Id of customer.</param>
        /// <param name="isSubAcc">Output to identify whether or not this customer is a sub-account.</param>
        /// <param name="roleId">Role id of customer.</param>
        /// <returns>Boolean object.</returns>
        public bool IsValidUsername(string userName, int uplineId, out int custId, out bool isSubAcc, out int roleId)
        {
            custId = 0;
            isSubAcc = false;
            roleId = 0;
            if (string.IsNullOrEmpty(userName))
            {
                return false;
            }

            Hashtable parameters = new Hashtable();
            parameters.Add("username", userName);
            parameters.Add("isexists", false);

            var result = DbHelper.ExecuteDbResult("Age_CustomerCheckExists", parameters);

            return !(bool)result.Outputs["isexists"];
        }
        #endregion

        /// <summary>
        /// Gets user information.
        /// </summary>
        /// <param name="custId">Customer id.</param>
        /// <returns>Customer object.</returns>
        public Customer UserInfo(int custId)
        {
            var param = new Hashtable();
            param.Add("custid", custId);

            using (var reader = this.ExecDataReader("Age_CustomerSelUserInfo", param))
            {
                if (reader != null)
                {
                    while (reader.Read())
                    {
                        return new Customer()
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
                }

                return null;
            }
        }

        /// <summary>
        /// Edits user information.
        /// </summary>
        /// <param name="custId">Customer id.</param>
        /// <param name="firstName">First name.</param>
        /// <param name="lastName">Last name.</param>
        /// <param name="homePhone">Home phone.</param>
        /// <param name="mobilePhone">Mobile phone.</param>
        /// <param name="email">Email address.</param>
        /// <param name="fax">Fax number.</param>
        /// <param name="remark">Customer remark.</param>
        /// <param name="ip">IP information.</param>
        /// <param name="userId">Admin user id.</param>
        /// <param name="subAccId">Sub-account id.</param>
        /// <param name="language">Language to get data.</param>
        /// <returns>Result object.</returns>
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
            var param = new Hashtable();
            param.Add("custid", custId);
            param.Add("firstname", firstName);
            param.Add("lastname", lastName);
            param.Add("phone", homePhone);
            param.Add("mobilephone", mobilePhone);
            param.Add("email", email);
            param.Add("fax", fax);
            param.Add("discasino", 0);
            param.Add("ip", ip);
            param.Add("userid", userId);
            param.Add("subaccid", subAccId);
            param.Add("language", language);
            param.Add("remark", remark);
            param.Add("errorcode", 0);
            param.Add("errormsg", string.Empty);

            var result = DbHelper.ExecuteDbResult("Age_CustomerUpdUserInfo", param);

            return new Result(result.Outputs["errorcode"], result.Outputs["errormsg"], result.ResultSet);
        }

        /// <summary>
        /// Resets password.
        /// </summary>
        /// <param name="custId">Customer id.</param>
        /// <param name="password">Password characters.</param>
        /// <param name="request">Request object.</param>
        /// <returns>Result object.</returns>
        public Result ResetPassword(int custId, string password, DTO.Request request)
        {
            var param = new Hashtable();
            param.Add("custid", custId);
            param.Add("userpwd", Cryptography.CfsEnCode(password));
            param.Add("userid", request.UserId);
            param.Add("discasino", 0);
            param.Add("language", request.Language);
            param.Add("subaccid", request.SubAccId);
            param.Add("loginip", request.IP);
            param.Add("errcode", 0);
            param.Add("errmsg", string.Empty);

            var result = DbHelper.ExecuteDbResult("Age_CustomerUpdPwd", param);

            return new Result(result.Outputs["errcode"].ToString(), result.Outputs["errmsg"].ToString(), result.ResultSet);
        }

        /// <summary>
        /// Config double commission.
        /// </summary>
        /// <param name="custId">Customer id.</param>
        /// <param name="allow">Allow double commission or not.</param>
        /// <param name="request">Request object.</param>
        /// <returns>Result object.</returns>
        public Result ConfigDoubleComm(int custId, bool allow, DTO.Request request)
        {
            var param = new Hashtable();
            param.Add("custid", custId);
            param.Add("doublecommlosing", allow);
            param.Add("userid", request.UserId);
            param.Add("language", request.Language);
            param.Add("subaccid", request.SubAccId);
            param.Add("ip", request.IP);
            param.Add("errcode", 0);
            param.Add("errmsg", string.Empty);

            var result = DbHelper.ExecuteDbResult("Acc_CustomerUpdDoubleCommLosing", param);

            return new Result(result.Outputs["errcode"].ToString(), result.Outputs["errmsg"].ToString(), result.ResultSet);
        }

        #endregion ICustomerDao Members
    }
}