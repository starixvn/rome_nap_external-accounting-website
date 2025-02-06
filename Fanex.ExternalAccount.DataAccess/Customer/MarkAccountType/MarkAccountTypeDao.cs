namespace Fanex.ExternalAccount.DataAccess.Customer.MarkAccountType
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Fanex.Data;
    using Fanex.ExternalAccount.DataAccess.Criteria.Customer.MarkAccountType;
    using Fanex.ExternalAccount.DTO;
    using Fanex.ExternalAccount.DTO.Customer;
    using Fanex.ExternalAccount.DTO.Customer.MarkAccount;
    using Fanex.ExternalAccount.Utilities;
    using Fanex.ExternalAccount.ViewModel.Customer.MarkAccountType;

    public class MarkAccountTypeDao : IMarkAccountTypeDao
    {
        private const string ErrorUsernameFormat = "{0}: {1}";
        private const string CommonErrorCode = "1";
        public Result Add(AddMarkAccountCriteria criteria)
        {
            var errorUsernames = new List<string> { };
            var result = new Result();
            using (var objDb = new ObjectDb(criteria.GetSettingKey()))
            {
                var reader = objDb.ExecuteReader(criteria.GetSpParams());
                while (reader.Read())
                {
                    if (!Utils.ToBoolean(reader["Isfinished"]))
                    {
                        errorUsernames.Add(Utils.ToString(reader["Username"]));
                    }
                }

                reader.Close();

                if(errorUsernames.Count == 0)
                {
                    return result;
                }

                result.ErrorCode = CommonErrorCode;
                result.ErrorMessage = string.Join(", ", errorUsernames);
            }

            return result;
        }

        public Result Check(CheckMarkAccountCriteria criteria)
        {
            var errorUsernames = new List<ErrorMarkAccount>();
            var result = new Result();
            using (var objDb = new ObjectDb(criteria.GetSettingKey()))
            {
                var reader = objDb.ExecuteReader(criteria.GetSpParams());
                while (reader.Read())
                {
                    errorUsernames.Add(new ErrorMarkAccount
                    {
                        CustId = Utils.ToInt32(reader["CustId"]),
                        Username = Utils.ToString(reader["Username"]),
                        ErrCode = Utils.ToInt32(reader["ErrCode"]),
                        ErrMsg = Utils.ToString(reader["ErrMsg"]),
                    });
                }

                reader.Close();
                errorUsernames.RemoveAll(err => err.ErrCode <= 0);
                if (errorUsernames.Count == 0)
                {
                    return result;
                }

                var errors = errorUsernames.Where(err => err.ErrCode > 0).GroupBy(err => err.ErrMsg);
                result.ErrorCode = CommonErrorCode;
                var errorMsg = new StringBuilder();
                foreach (var error in errors)
                {
                    var usernames = string.Join(", ", error.Select(cust => cust.Username)) + "</br>";

                    errorMsg.AppendFormat(ErrorUsernameFormat, error.Key, usernames);
                }

                result.ErrorMessage = errorMsg.ToString();
            }

            return result;
        }

        public MarkAccountViewModel SearchMarkAccount(SearchMarkAccountCriteria criteria)
        {
            var result = new MarkAccountViewModel();
            var customers = new List<MarkAccount>();
            using (var objDb = new ObjectDb(criteria.GetSettingKey()))
            {
                var reader = objDb.ExecuteReader(criteria.GetSpParams());
                while (reader.Read())
                {
                    customers.Add(new MarkAccount
                    {
                        CustId = Utils.ToInt32(reader["CustID"]),
                        Username = Utils.ToString(reader["Username"]),
                        Level = Utils.ToInt32(reader["RoleID"]),
                        TypeId = Utils.ToInt32(reader["TypeID"]),
                        MarkDateTime = Utils.ToStringExt(Utils.HasColumn(reader, "MarkedDate") ? Utils.ToDateTime(reader["MarkedDate"]) : DateTime.MinValue),
                        Remark = Utils.ToString(reader["Remark"]),
                        AgentId = Utils.ToInt32(reader["AgentId"])

                    });
                }

                result.Customers = customers;
                reader.Close();
                result.PageIndex = criteria.PageIndex;
                result.TotalRows = Utils.ToInt32(objDb.GetOuputParams()["totalrows"]);
            }

            return result;
        }

        public Result Unmark(UnmarkMarkAccountCriteria criteria)
        {
            var result = new Result();
            using (var objDb = new ObjectDb(criteria.GetSettingKey()))
            {
                objDb.ExecuteNonQuery(criteria.GetSpParams());
                var outputs = objDb.GetOuputParams();
                result.ErrorCode = outputs["errcode"].ToString();
                result.ErrorMessage = outputs["errmsg"].ToString();
            }

            return result;
        }

        public Result Update(UpdateMarkAccountCriteria criteria)
        {
            var result = new Result();
            using (var objDb = new ObjectDb(criteria.GetSettingKey()))
            {
                objDb.ExecuteNonQuery(criteria.GetSpParams());
                var outputs = objDb.GetOuputParams();
                result.ErrorCode = outputs["errcode"].ToString();
                result.ErrorMessage = outputs["errmsg"].ToString();
            }

            return result;
        }
    }
}
