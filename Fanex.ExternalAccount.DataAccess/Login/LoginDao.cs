namespace Fanex.ExternalAccount.DataAccess.Impl.Login
{
    using System;
    using System.Collections;
    using System.Data;
    using Common;
    using Data;
    using DTO;
    using Interface.Login;

    public class LoginDao : ILoginDao
    {
        public Result Login(
            string username,
            string password,
            string browser,
            string ip,
            string domain,
            string countryCode,
            string platForm,
            out int custId)
        {
            var result = new Result();

            custId = 0;

            using (var db = new ObjectDb("EAS_UserLogin"))
            {
                db.AddInParameter("Username", DbType.String, username);
                db.AddInParameter("userpwd", DbType.String, Common.Cryptography.CfsEnCode(password));
                db.AddInParameter("BrowserName", DbType.String, browser);
                db.AddInParameter("LoginIP", DbType.String, ip);
                db.AddInParameter("LoginDomain", DbType.String, domain);
                db.AddInParameter("CountryCode", DbType.String, countryCode);
                db.AddInParameter("Platform", DbType.String, platForm);
                db.AddOutParameter("Custid", DbType.Int32, sizeof(int));
                db.AddOutParameter("ErrCode", DbType.Int32, sizeof(int));
#pragma warning disable S109 // Magic numbers should not be used
                db.AddOutParameter("ErrMsg", DbType.String, 200);
#pragma warning restore S109 // Magic numbers should not be used
                db.AddOutParameter("GroupID", DbType.Int32, sizeof(int));

                db.ExecuteNonQuery();

                custId = (int) db.GetParameterValue("Custid");
                result.ErrorCode = db.GetParameterValue("ErrCode").ToString();
                result.ErrorMessage = db.GetParameterValue("ErrMsg").ToString();
                result.ResultData = db.GetParameterValue("GroupID");
            }

            return result;
        }

        public Result Logout(int custId, string username)
        {
            using (var db = new ObjectDb("EAS_UserLogout"))
            {
                db.AddInParameter("on_userid", DbType.Int32, custId);
                db.AddInParameter("username", DbType.String, username);
                db.ExecuteNonQuery();

                return new Result();
            }
        }

        public Result Checkin(int custId, string netSessionId, string ip, out int onuserId)
        {
            var result = new Result();

            onuserId = 0;

            using (var db = new ObjectDb("EAS_UserCheckIn"))
            {
                db.AddInParameter("custid", DbType.Int32, custId);
                db.AddInParameter("NetSessionID", DbType.String, netSessionId);
                db.AddInParameter("IP", DbType.String, ip);
                db.AddOutParameter("on_userid", DbType.Int32, sizeof(int));
                db.ExecuteNonQuery();

                onuserId = (int) db.GetParameterValue("on_userid");
            }

            return result;
        }

        public bool IsKickedOut(int custId)
        {
            using (var db = new ObjectDb("EAS_UserRecheckIn"))
            {
                db.AddInParameter("On_UserID", DbType.Int32, custId);
                db.AddOutParameter("isKickedOut", DbType.Boolean, sizeof(bool));
                db.ExecuteNonQuery();

                return (bool) db.GetParameterValue("isKickedOut");
            }
        }        

        public bool CheckExternalGroup(int custId, int? funcId, string funcKey)
        {
            using (var db = new ObjectDb("EAS_UserCheckPermission"))
            {
                db.AddInParameter("UserID", DbType.Int32, custId);
                db.AddInParameter("FuncId", DbType.Int32, funcId);
                db.AddInParameter("FuncKey", DbType.String, funcKey);

                AddResultParams(db.Database, db.DbCommand);

                db.ExecuteNonQuery();

                var result = GetResultParams(db.Database, db.DbCommand);

                return result.IsSuccess;
            }
        }

        public DateTime GetDbTime()
        {
            var p = new Hashtable {{"logintime", null}};

            var dr = DbHelper.ExecuteDbResult("Age_LoginAdminVerifyTicket", p);

            return Converter.ToDateTime(dr.Outputs["logintime"]);
        }
    }
}