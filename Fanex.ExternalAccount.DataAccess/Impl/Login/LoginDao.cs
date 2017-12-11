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

    public class LoginDao : DAOBase, ILoginDao
    {
        #region ILoginDao Members

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