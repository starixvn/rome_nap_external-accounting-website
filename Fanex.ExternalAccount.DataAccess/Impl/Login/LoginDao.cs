namespace Fanex.ExternalAccount.DataAccess.Impl.Login
{
    using System.Collections.Generic;
    using Fanex.Common;
    using Fanex.Common.Extensions;
    using Fanex.Data;
    using Fanex.ExternalAccount.DataAccess.Impl.Base;
    using Fanex.ExternalAccount.DataAccess.Interface.Login;
    using Fanex.ExternalAccount.DTO.UserControl;

    /// <summary>
    /// LoginDao
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
        /// <param name="ip">The ip.</param>
        /// <param name="domain">The domain.</param>
        /// <param name="countryCode">The country code.</param>
        /// <param name="platForm">The plat form.</param>
        /// <param name="custId">The cust id.</param>
        /// <returns>Fanex.ExternalAccount.DTO.Result</returns>
        public Fanex.ExternalAccount.DTO.Result Login(string username, string password, string browser, string ip, string domain, string countryCode, string platForm, out int custId)
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

                db.ExecuteNonQuery();

                custId = (int)db.GetParameterValue("Custid");
                result.ErrorCode = db.GetParameterValue("ErrCode").ToString();
                result.ErrorMessage = db.GetParameterValue("ErrMsg").ToString();
                result.ResultData = db.GetParameterValue("GroupID");
            }

            return result;
        }

        /// <summary>
        /// Logouts the specified cust id.
        /// </summary>
        /// <param name="custId">The cust id.</param>
        /// <param name="username">The username.</param>
        /// <returns>DTO.Result</returns>
        public DTO.Result Logout(int custId, string username)
        {
            using (var db = new ObjectDb("EAS_UserLogout"))
            {
                db.AddInParameter("on_userid", System.Data.DbType.Int32, custId);
                db.AddInParameter("username", System.Data.DbType.String, username);
                db.ExecuteNonQuery();

                return new DTO.Result();
            }
        }

        /// <summary>
        /// Checkins the specified cust id.
        /// </summary>
        /// <param name="custId">The cust id.</param>
        /// <param name="netSessionID">The net session ID.</param>
        /// <param name="ip">The ip.</param>
        /// <param name="onUserId">The on user id.</param>
        /// <returns>DTO.Result</returns>
        public DTO.Result Checkin(int custId, string netSessionID, string ip, out int onUserId)
        {
            var result = new DTO.Result();

            onUserId = 0;

            using (var db = new ObjectDb("EAS_UserCheckIn"))
            {
                db.AddInParameter("custid", System.Data.DbType.Int32, custId);
                db.AddInParameter("NetSessionID", System.Data.DbType.String, netSessionID);
                db.AddInParameter("IP", System.Data.DbType.String, ip);
                db.AddOutParameter("on_userid", System.Data.DbType.Int32, sizeof(int));
                db.ExecuteNonQuery();

                onUserId = (int)db.GetParameterValue("on_userid");
            }

            return result;
        }

        /// <summary>
        /// Recheckins the specified cust id.
        /// </summary>
        /// <param name="custId">The cust id.</param>
        /// <param name="isKickOut">if set to <c>true</c> [is kick out].</param>
        /// <returns>DTO.Result</returns>
        public DTO.Result Recheckin(int custId, out bool isKickOut)
        {
            isKickOut = false;

            using (var db = new ObjectDb("EAS_UserRecheckIn"))
            {
                db.AddInParameter("On_UserID", System.Data.DbType.Int32, custId);
                db.AddOutParameter("isKickedOut", System.Data.DbType.Boolean, sizeof(bool));
                db.ExecuteNonQuery();

                isKickOut = (bool)db.GetParameterValue("isKickedOut");

                return new DTO.Result();
            }
        }

        /// <summary>
        /// Groups the list.
        /// </summary>
        /// <returns>ICollection<DTO.Group></returns>
        public ICollection<Group> GroupList()
        {
            var list = new List<Group>();

            using (var db = new ObjectDb("EAS_GroupInfoSelExternalGroup"))
            {
                using (var reader = db.ExecuteReader())
                {
                    while (reader != null && reader.Read())
                    {
                        list.Add(new Group()
                        {
                            GroupID = reader.ToInt("GroupID"),
                            GroupName = reader.To<string>("GroupName")
                        });
                    }
                }
            }

            return list;
        }

        /// <summary>
        /// Checks the external group.
        /// </summary>
        /// <param name="custId">The cust id.</param>
        /// <param name="funcId">The func id.</param>
        /// <param name="funcKey">The func key.</param>
        /// <returns>bool</returns>
        public bool CheckExternalGroup(int custId, int funcId, string funcKey)
        {
            using (var db = new ObjectDb("EAS_UserCheckPermission"))
            {
                db.AddInParameter("UserID", System.Data.DbType.Int32, custId);
                db.AddInParameter("FuncID", System.Data.DbType.Int32, funcId);
                db.AddInParameter("FuncKey", System.Data.DbType.String, funcKey);

                this.AddResultParams(db.Database, db.DbCommand);

                db.ExecuteNonQuery();

                var result = this.GetResultParams(db.Database, db.DbCommand);

                return result.IsSuccess;
            }
        }

        #endregion ILoginDao Members
    }
}