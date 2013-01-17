// -----------------------------------------------------------------------
// <copyright file="DAOBase.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Fanex.ExternalAccount.DataAccess.Impl.Base
{
    using System;
    using System.Collections;
    using System.Data;
    using System.Data.Common;
    using Fanex.Data.DNAide;
    using Fanex.ExternalAccount.DTO;
    using Microsoft.Practices.EnterpriseLibrary.Data;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class DAOBase
    {
        private const int DEFAULTTIMEOUT = 600;

        /// <summary>
        /// Add out put param for Execute Nonquery
        /// Error Code and Error Messages
        /// </summary>
        /// <param name="db"></param>
        /// <param name="command"></param>
        public void AddResultParams(Database db, DbCommand command)
        {
            db.AddOutParameter(command, "ErrCode", DbType.String, 300);
            db.AddOutParameter(command, "ErrMsg", DbType.String, 4000);
        }

        public Result GetResultParams(Database db, DbCommand command)
        {
            var errorCode = db.GetParameterValue(command, "ErrCode");
            errorCode = (errorCode is DBNull) ? string.Empty : Convert.ToString(errorCode);

            var errorMessage = db.GetParameterValue(command, "ErrMsg");
            errorMessage = (errorMessage is DBNull) ? string.Empty : Convert.ToString(errorMessage);

            ////get result message
            var result = new Result((string)errorCode, (string)errorMessage);

            return result;
        }

        #region ExecSp To DataSet

        public virtual IDataReader ExecDataReader(string name, Hashtable parameters)
        {
            ////Data.IDbSetting dtbSetting = DbSettingProviderManager.Current.GetSetting(name);
            ////string spName = dtbSetting.SpName;
            ////string asType = dtbSetting.DbName;

            string spname = name;
            return DbHelper.ExecuteReader(spname, parameters, DEFAULTTIMEOUT);
        }

        public virtual IDataReader ExecDataReader(string name, Hashtable parameters, int timeOut)
        {
            IDataReader reader = this.ExecDataReader(name, parameters);
            if (reader != null)
            {
                return reader;
            }
            else
            {
                return null;
            }
        }

        #endregion ExecSp To DataSet
    }
}