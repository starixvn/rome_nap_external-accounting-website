// --------------------------------------------------------------------------
// <copyright file="CustomerDao.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
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
                            Username = reader.ToString("username"),
                            FirstName = reader.ToString("firstname"),
                            LastName = reader.ToString("lastname"),
                            MobilePhone = reader.ToString("mobilephone"),
                            HomePhone = reader.ToString("phone"),
                            Email = reader.ToString("email"),
                            Fax = reader.ToString("fax")
                        };
                    }
                }

                return null;
            }
        }

        public Result EditUserInfo(int custId, string firstName, string lastName, string homePhone, string mobilePhone,
                                    string email, string fax, string ip, int userId, int subAccId, string language)
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
            param.Add("errorcode", 0);
            param.Add("errormsg", string.Empty);

            var result = DbHelper.ExecuteDbResult("Age_CustomerUpdUserInfo", param);

            return new Result(result.Outputs["errorcode"], result.Outputs["errormsg"], result.ResultSet);
        }

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