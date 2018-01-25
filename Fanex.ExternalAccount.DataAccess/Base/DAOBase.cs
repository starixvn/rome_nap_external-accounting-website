namespace Fanex.ExternalAccount.DataAccess.Impl.Base
{
    using System;
    using System.Collections;
    using System.Data;
    using System.Data.Common;
    using Data.DNAide;
    using DTO;
    using Microsoft.Practices.EnterpriseLibrary.Data;

    public class DAOBase
    {
        private const int Defaulttimeout = 600;

        public static void AddResultParams(Database db, DbCommand command)
        {
            db.AddOutParameter(command, "ErrCode", DbType.String, 300);
            db.AddOutParameter(command, "ErrMsg", DbType.String, 4000);
        }

        public Result GetResultParams(Database db, DbCommand command)
        {
            var errorCode = db.GetParameterValue(command, "ErrCode");
            errorCode = errorCode is DBNull ? string.Empty : Convert.ToString(errorCode);
            var errorMessage = db.GetParameterValue(command, "ErrMsg");
            errorMessage = errorMessage is DBNull ? string.Empty : Convert.ToString(errorMessage);

            return new Result((string)errorCode, (string)errorMessage);
        }

        public virtual IDataReader ExecDataReader(string name, Hashtable parameters)
            => DbHelper.ExecuteReader(name, parameters, Defaulttimeout);
    }
}