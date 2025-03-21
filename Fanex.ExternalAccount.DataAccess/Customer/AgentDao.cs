//namespace Fanex.ExternalAccount.DataAccess.Impl.Customer
//{
//    using Criteria.Customer;
//    using Data;
//    using DTO;
//    using Fanex.Data.DNAide;
//    using Fanex.ExternalAccount.DataAccess.Interface.Customer;
//    using Fanex.ExternalAccount.DTO.Customer;
//    using System;
//    using System.Collections;
//    using System.Data;

//    public class AgentDao : CustomerDao, IAgentDao
//    {

//        public Transfer GetTransferOption(int custId)
//        {
//            var param = new Hashtable { { "custid", custId }, { "transferoption", "00000000" } };

//            var result = DbHelper.ExecuteDbResult("Acc_CustomerSelTransferOption", param);
//            var transferOption = result.Outputs["transferoption"].ToString();
//            if (transferOption.Length > 0)
//            {
//                return new Transfer
//                {
//#pragma warning disable S109 // Magic numbers should not be used
//                    Weekly = Convert.ToBoolean(int.Parse(transferOption[7].ToString())),
//                    Mon = Convert.ToBoolean(int.Parse(transferOption[1].ToString())),
//                    Wed = Convert.ToBoolean(int.Parse(transferOption[3].ToString())),
//                    Thu = Convert.ToBoolean(int.Parse(transferOption[4].ToString())),
//                    Fri = Convert.ToBoolean(int.Parse(transferOption[5].ToString())),
//                    Sat = Convert.ToBoolean(int.Parse(transferOption[6].ToString())),
//                    Sun = Convert.ToBoolean(int.Parse(transferOption[0].ToString()))
//#pragma warning restore S109 // Magic numbers should not be used
//                };
//            }

//            return null;
//        }

//        public Result IsLimitMemberCreationBySuper(CheckMemberCreationCriteria criteria)
//        {
//            var result = new Result();
//            using (ObjectDb objDb = new ObjectDb(criteria.GetSettingKey()))
//            {
//                objDb.AssignParamValues(criteria.GetSpParams());
//                objDb.ExecuteNonQuery();
//                var outputs = objDb.GetOuputParams();
//                result.ErrorCode = outputs["errcode"].ToString();
//                result.ErrorMessage = outputs["errmsg"].ToString();
//            }

//            return result;
//        }
//    }
//}