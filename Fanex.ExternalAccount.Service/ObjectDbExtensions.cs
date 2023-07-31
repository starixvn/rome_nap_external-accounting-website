using Fanex.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fanex.ExternalAccount.Service
{

    public static class ObjectDbExtensions
    {        
        public static IDataReader ExecuteReaderScaleoutDB(this ObjectDb objdb, Hashtable parameters, string AGIP)
        {
            string oldConnectionString = objdb.DbSetting.ConnectionString;
            objdb.DbSetting.ConnectionString = BuildConnectionString(AGIP, oldConnectionString);
            objdb.AssignParamValues(parameters);
            IDataReader result;
            try
            {
                result = objdb.ExecuteReader();
            }
            finally
            {
                objdb.DbSetting.ConnectionString = oldConnectionString;
            }

            return result;
        }
        
        public static DataSet ExecuteDataSetScaleoutDB(this ObjectDb objdb, Hashtable parameters, string AGIP)
        {
            string oldConnectionString = objdb.DbSetting.ConnectionString;
            objdb.DbSetting.ConnectionString = BuildConnectionString(AGIP, oldConnectionString);
            objdb.AssignParamValues(parameters);
            DataSet result;
            try
            {
                result = objdb.ExecuteDataSet();
            }
            finally
            {
                objdb.DbSetting.ConnectionString = oldConnectionString;
            }

            return result;
        }
       
        public static int ExecuteNonQueryScaleoutDB(this ObjectDb objdb, Hashtable parameters, string AGIP)
        {
            string oldConnectionString = objdb.DbSetting.ConnectionString;
            objdb.DbSetting.ConnectionString = BuildConnectionString(AGIP, oldConnectionString);
            objdb.AssignParamValues(parameters);
            int result;
            try
            {
                result = objdb.ExecuteNonQuery();
            }
            finally
            {
                objdb.DbSetting.ConnectionString = oldConnectionString;
            }

            return result;
        }
        
        private static string BuildConnectionString(string serverIP, string connectionString)
        {
            var sqlConnectionStringBuilder = new SqlConnectionStringBuilder(connectionString);
            sqlConnectionStringBuilder.DataSource = serverIP;
            return sqlConnectionStringBuilder.ToString();
        }
    }

}