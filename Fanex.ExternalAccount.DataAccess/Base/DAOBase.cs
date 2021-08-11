namespace Fanex.ExternalAccount.DataAccess.Impl.Base
{
    using Data.DNAide;
    using DTO;
    using Fanex.Data;
    using Fanex.NAP.Framework.Data;
    using Microsoft.Practices.EnterpriseLibrary.Data;
    using System;
    using System.Collections;
    using System.Data;
    using System.Data.Common;

    public class DAOBase
    {
        private const int Defaulttimeout = 600;

        public static void AddResultParams(Database db, DbCommand command)
        {
#pragma warning disable S109 // Magic numbers should not be used
            db.AddOutParameter(command, "ErrCode", DbType.String, 300);
            db.AddOutParameter(command, "ErrMsg", DbType.String, 4000);
#pragma warning restore S109 // Magic numbers should not be used
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

        public Result ExecuteNonQuery(ICriteria criteria)
        {
            using (var objDB = new ObjectDb(criteria.GetSettingKey()))
            {
                objDB.AssignParamValues(criteria.GetSpParams());
                objDB.ExecuteNonQuery();
                var outputs = objDB.GetOuputParams();
                return new Result()
                {
                    ErrorCode = outputs["errcode"]?.ToString(),
                    ErrorMessage = outputs["errmsg"]?.ToString()
                };
            }
        }
    }
}