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

        public Result EditUserInfo(Customer custInfo, DTO.Request request)
        {
            var param = new Hashtable();
            param.Add("custid", custInfo.CustId);
            param.Add("firstname", custInfo.FirstName);
            param.Add("lastname", custInfo.LastName);
            param.Add("phone", custInfo.HomePhone);
            param.Add("mobilephone", custInfo.MobilePhone);
            param.Add("email", custInfo.Email);
            param.Add("fax", custInfo.Fax);
            param.Add("discasino", 0);
            param.Add("ip", request.IP);
            param.Add("userid", request.UserId);
            param.Add("subaccid", request.SubAccId);
            param.Add("language", request.Language);
            param.Add("errorcode", string.Empty);
            param.Add("errormsg", string.Empty);

            var result = DbHelper.ExecuteDbResult("Age_CustomerUpdUserInfo", param);

            return new Result(result.Outputs["errorcode"].ToString(), result.Outputs["errormsg"].ToString(), result.ResultSet);
        }

        public Transfer TransferOption(int custId)
        {
            var param = new Hashtable();
            param.Add("custid", custId);
            param.Add("transferoption", string.Empty);

            var result = DbHelper.ExecuteDbResult("Acc_CustomerSelTransferOption", param);
            var output = result.Outputs["transferoption"] as string;

            if (!string.IsNullOrWhiteSpace(output))
            {
                if (output.Length == 8)
                {
                    return new Transfer()
                    {
                        Daily = Convert.ToBoolean(output[0]),
                        Mon = Convert.ToBoolean(output[1]),
                        Tue = Convert.ToBoolean(output[2]),
                        Wed = Convert.ToBoolean(output[3]),
                        Thu = Convert.ToBoolean(output[4]),
                        Fri = Convert.ToBoolean(output[5]),
                        Sat = Convert.ToBoolean(output[6]),
                        Sun = Convert.ToBoolean(output[7])
                    };
                }
                else
                {
                    return new Transfer()
                    {
                        Daily = Convert.ToBoolean(output[0])
                    };
                }
            }

            return null;
        }

        public Result EditTransferOption(int custId, Transfer transferOption, Request request)
        {
            var transfer = string.Empty;
            var param = new Hashtable();

            transfer = (transferOption.Daily ? "1" : "0") +
                (transferOption.Mon ? "1" : "0") +
                (transferOption.Tue ? "1" : "0") +
                (transferOption.Wed ? "1" : "0") +
                (transferOption.Thu ? "1" : "0") +
                (transferOption.Fri ? "1" : "0") +
                (transferOption.Sat ? "1" : "0") +
                (transferOption.Sun ? "1" : "0");

            param.Add("custid", custId);
            param.Add("transferoption", transfer);
            param.Add("userid", request.UserId);
            param.Add("ip", request.IP);
            param.Add("subaccid", request.SubAccId);
            param.Add("language", request.Language);
            param.Add("returnerrcode", false);
            param.Add("errcode", string.Empty);
            param.Add("errmsg", string.Empty);

            var result = DbHelper.ExecuteDbResult("Acc_CustomerUpdTransferOption", param);

            return new Result(result.Outputs["errcode"].ToString(), result.Outputs["errmsg"].ToString(), result.ResultSet);
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
            param.Add("errcode", string.Empty);
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