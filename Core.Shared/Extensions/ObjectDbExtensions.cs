//using System.Collections;
//using System.Data;
//using System.Data.SqlClient;
//using Fanex.Data;

//namespace Fanex.ExternalAccount.Core.Shared.Extensions
//{
//    public static class ObjectDbExtensions
//    {
//        public static DataSet ExecuteDataSetScaleoutDB(this ObjectDb objdb, Hashtable parameters, string AGIP)
//        {
//            string oldConnectionString = objdb.DbSetting.ConnectionString;
//            objdb.DbSetting.ConnectionString = BuildConnectionString(AGIP, oldConnectionString);
//            objdb.AssignParamValues(parameters);
//            DataSet result;

//            try
//            {
//                result = objdb.ExecuteDataSet();
//            }
//            finally
//            {
//                objdb.DbSetting.ConnectionString = oldConnectionString;
//            }

//            return result;
//        }

//        public static int ExecuteNonQueryScaleoutDB(this ObjectDb objdb, Hashtable parameters, string AGIP)
//        {
//            string oldConnectionString = objdb.DbSetting.ConnectionString;
//            objdb.DbSetting.ConnectionString = BuildConnectionString(AGIP, oldConnectionString);
//            objdb.AssignParamValues(parameters);
//            int result;
//            try
//            {
//                result = objdb.ExecuteNonQuery();
//            }
//            finally
//            {
//                objdb.DbSetting.ConnectionString = oldConnectionString;
//            }

//            return result;
//        }

//        private static string BuildConnectionString(string serverIP, string connectionString)
//        {
//            var sqlConnectionStringBuilder = new SqlConnectionStringBuilder(connectionString);
//            sqlConnectionStringBuilder.DataSource = serverIP;
//            return sqlConnectionStringBuilder.ToString();
//        }
//    }
//}
