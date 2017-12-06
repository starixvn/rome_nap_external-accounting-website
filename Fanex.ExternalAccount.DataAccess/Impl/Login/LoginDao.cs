// --------------------------------------------------------------------------
// <copyright file="LoginDao.cs" company="">
// Copyright (c) . All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Andy.Nguyen" date="2013.9.17">Created</change>
// </history>

namespace Fanex.ExternalAccount.DataAccess.Impl.Login
{
    using System;
    using System.Collections.Generic;
    using Fanex.Common;
    using Fanex.Common.Extensions;
    using Fanex.Data;
    using Fanex.Data.DNAide;
    using Fanex.ExternalAccount.DataAccess.Impl.Base;
    using Fanex.ExternalAccount.DataAccess.Interface.Login;
    using Fanex.ExternalAccount.DTO.UserControl;
    using Logging.Extensions.Database;

    /// <summary>
    /// LoginDao object.
    /// </summary>
    public class LoginDao : DAOBase, ILoginDao
    {
        #region ILoginDao Members

        /// <summary>
        /// Logins the specified username.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <param name="password">The password.</param>
        /// <param name="browser">The browser.</param>
        /// <param name="ip">The IP address.</param>
        /// <param name="domain">The domain.</param>
        /// <param name="countryCode">The country code.</param>
        /// <param name="platForm">The platform.</param>
        /// <param name="custId">The customer id.</param>
        /// <returns>Result object.</returns>
        public Fanex.ExternalAccount.DTO.Result Login(
            string username, 
            string password, 
            string browser, 
            string ip, 
            string domain,
            string countryCode, 
            string platForm, 
            out int custId)
        {
            var result = new DTO.Result();

            custId = 0;

            using (var db = new ObjectDb("EAS_UserLogin"))
            {
                db.AddInParameter("Username", System.Data.DbType.String, username);
                db.AddInParameter("userpwd", System.Data.DbType.String, Cryptography.CfsEnCode(password));
                db.AddInParameter("BrowserName", System.Data.DbType.String, browser);
                db.AddInParameter("LoginIP", System.Data.DbType.String, ip);
                db.AddInParameter("LoginDomain", System.Data.DbType.String, domain);
                db.AddInParameter("CountryCode", System.Data.DbType.String, countryCode);
                db.AddInParameter("Platform", System.Data.DbType.String, platForm);
                db.AddOutParameter("Custid", System.Data.DbType.Int32, sizeof(int));
                db.AddOutParameter("ErrCode", System.Data.DbType.Int32, sizeof(int));
                db.AddOutParameter("ErrMsg", System.Data.DbType.String, 200);
                db.AddOutParameter("GroupID", System.Data.DbType.Int32, sizeof(int));

                try
                {
                    db.ExecuteNonQuery();

                    custId = (int)db.GetParameterValue("Custid");
                    result.ErrorCode = db.GetParameterValue("ErrCode").ToString();
                    result.ErrorMessage = db.GetParameterValue("ErrMsg").ToString();
                    result.ResultData = db.GetParameterValue("GroupID");
                }
                catch (Exception ex)
                {
                    throw new DatabaseException(ex.Message, ex, db.DbSetting);
                }
            }

            return result;
        }

        /// <summary>
        /// Logouts the specified customer id.
        /// </summary>
        /// <param name="custId">The customer id.</param>
        /// <param name="username">The username.</param>
        /// <returns>DTO.Result object.</returns>
        public DTO.Result Logout(int custId, string username)
        {
            using (var db = new ObjectDb("EAS_UserLogout"))
            {
                db.AddInParameter("on_userid", System.Data.DbType.Int32, custId);
                db.AddInParameter("username", System.Data.DbType.String, username);

                try
                {
                    db.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    throw new DatabaseException(ex.Message, ex, db.DbSetting);
                }

                return new DTO.Result();
            }
        }

        /// <summary>
        /// Check-in the specified customer id.
        /// </summary>
        /// <param name="custId">The customer id.</param>
        /// <param name="netSessionID">The net session ID.</param>
        /// <param name="ip">The IP address.</param>
        /// <param name="onuserId">The on user id.</param>
        /// <returns>DTO.Result object.</returns>
        public DTO.Result Checkin(int custId, string netSessionID, string ip, out int onuserId)
        {
            var result = new DTO.Result();

            onuserId = 0;

            using (var db = new ObjectDb("EAS_UserCheckIn"))
            {
                db.AddInParameter("custid", System.Data.DbType.Int32, custId);
                db.AddInParameter("NetSessionID", System.Data.DbType.String, netSessionID);
                db.AddInParameter("IP", System.Data.DbType.String, ip);
                db.AddOutParameter("on_userid", System.Data.DbType.Int32, sizeof(int));

                try
                {
                    db.ExecuteNonQuery();
                    onuserId = (int)db.GetParameterValue("on_userid");
                }
                catch (Exception ex)
                {
                    throw new DatabaseException(ex.Message, ex, db.DbSetting);
                }
            }

            return result;
        }

        /// <summary>
        /// Recheck-in the specified customer id .
        /// </summary>
        /// <param name="custId">The customer id .</param>
        /// <param name="isKickOut">If set to <c>true</c> [is kick out].</param>
        /// <returns>DTO.Result object.</returns>
        public bool IsKickedOut(int custId)
        {
            using (var db = new ObjectDb("EAS_UserRecheckIn"))
            {
                db.AddInParameter("On_UserID", System.Data.DbType.Int32, custId);
                db.AddOutParameter("isKickedOut", System.Data.DbType.Boolean, sizeof(bool));

                try
                {
                    db.ExecuteNonQuery();
                    return (bool)db.GetParameterValue("isKickedOut");
                }
                catch (Exception ex)
                {
                    throw new DatabaseException(ex.Message, ex, db.DbSetting);
                }
                
            }
        }

        /// <summary>
        /// Groups the list.
        /// </summary>
        /// <returns>ICollection object.</returns>
        public ICollection<Group> GroupList()
        {
            var list = new List<Group>();

            using (var db = new ObjectDb("EAS_GroupInfoSelExternalGroup"))
            {
                try
                {
                    using (var reader = db.ExecuteReader())
                    {
                        while (reader != null && reader.Read())
                        {
                            list.Add(new Group
                            {
                                GroupID = reader.ToInt("GroupID"),
                                GroupName = reader.To<string>("GroupName")
                            });
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new DatabaseException(ex.Message, ex, db.DbSetting);
                }
            }

            return list;
        }

        /// <summary>
        /// Checks the external group.
        /// </summary>
        /// <param name="custId">The customer id .</param>
        /// <param name="funcId">The function id.</param>
        /// <param name="funcKey">The function key.</param>
        /// <returns>Boolean object.</returns>
        public bool CheckExternalGroup(int custId, int? funcId, string funcKey)
        {
            using (var db = new ObjectDb("EAS_UserCheckPermission"))
            {
                db.AddInParameter("UserID", System.Data.DbType.Int32, custId);
                db.AddInParameter("FuncID", System.Data.DbType.Int32, funcId);
                db.AddInParameter("FuncKey", System.Data.DbType.String, funcKey);

                this.AddResultParams(db.Database, db.DbCommand);

                try
                {
                    db.ExecuteNonQuery();
                    var result = this.GetResultParams(db.Database, db.DbCommand);

                    return result.IsSuccess;
                }
                catch (Exception ex)
                {
                    throw new DatabaseException(ex.Message, ex, db.DbSetting);
                }
            }
        }

        /// <summary>
        /// Gets DB time.
        /// </summary>
        /// <returns>DateTime object.</returns>
        public DateTime GetDBTime()
        {
            DbResult dr = null;
            DateTime loginTime;
            var p = new System.Collections.Hashtable();

            p.Add("logintime", null);

            dr = Fanex.Data.DNAide.DbHelper.ExecuteDbResult("Age_LoginAdminVerifyTicket", p);
            loginTime = Fanex.Common.Converter.ToDateTime(dr.Outputs["logintime"]);

            return loginTime;
        }

        #endregion ILoginDao Members
    }
}