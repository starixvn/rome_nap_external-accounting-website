using Fanex.ExternalAccount.DataAccess.Criteria.Customer.EditSuperList;
using Fanex.ExternalAccount.DataAccess.Customer;
using Fanex.ExternalAccount.DTO;
using Fanex.ExternalAccount.Requests.Customer.SuperList;
using Fanex.Security;
using Sunplus.Agent.Utilities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Encryptor = Sunplus.Agent.Utilities.Encryptor;

namespace Fanex.ExternalAccount.Service.Customer
{
    public interface IEditSuperListService
    {
        Result CheckMultipleSupers(string superNames, string groupName, int userId);

        IEnumerable<Result> UpdateMultipleSuperRequest(UpdateMultipleSuperRequest request, int userId, string ip);
    }

    public class EditSuperListService: IEditSuperListService
    {
        private readonly IEditSuperListDao dataAccess;
        private readonly Encryptor encryptor;

        private const string Salt = "1qaw3+{:P)";
        private const int ErrorUpdateInfo = 1;
        private const int ErrorUpdatePassword = 2;
        private const int ErrorUpdateCode = 3;

        public EditSuperListService() : this(null)
        {
        }

        public EditSuperListService(IEditSuperListDao dataAccess, Encryptor encryptor = null)
        {
            this.dataAccess = dataAccess ?? new EditSuperListDao();
            this.encryptor = encryptor ?? new EncryptorImpl();
        }

        public Result CheckMultipleSupers(string superNames, string groupName, int userId)
            => dataAccess.CheckMultipleSupers(new CheckMultipleSupersCriteria(superNames, groupName));

        public IEnumerable<Result> UpdateMultipleSuperRequest(UpdateMultipleSuperRequest request, int userId, string ip)
        {
            var errorList = new List<Result>();
            var superIdNameList = request.SuperIdNameList.Split(',').ToList();
            var custIds = string.Join(",", superIdNameList.Select(idname => idname.Split(';')[0]));
            if (request.IsUpdateInfo)
            {
                var criteriaInfo = new UpdateMultipleSuperInfoCriteria
                {
                    CustIDList = custIds,
                    FirstName = request.IsChangeFirstName && string.IsNullOrEmpty(request.FirstName)? string.Empty : request.FirstName,
                    LastName = request.IsChangeLastName && string.IsNullOrEmpty(request.LastName) ? string.Empty : request.LastName,
                    Phone = request.IsChangePhone && string.IsNullOrEmpty(request.Phone) ? string.Empty : request.Phone,
                    MobilePhone = request.IsChangeMobilePhone && string.IsNullOrEmpty(request.MobilePhone) ? string.Empty : request.MobilePhone,
                    UserId = userId,
                    IP = ip
                };

                var resultInfo = dataAccess.ExecuteUpdate(criteriaInfo);
                if (!string.IsNullOrEmpty(resultInfo))
                {
                    errorList.AddRange(ParseErrMsg(ErrorUpdateInfo, resultInfo, superIdNameList));
                }
            }

            if (request.IsUpdatePassword)
            {
                var criteriaInfo = new UpdateMultipleSuperPasswordCirteria
                {
                    CustIDList = custIds,
                    UserPwd = Cryptography.CfsEnCode(request.Password),
                    UserId = userId,
                    LoginIP = ip
                };

                var resultPwd = dataAccess.ExecuteUpdate(criteriaInfo);
                if (!string.IsNullOrEmpty(resultPwd))
                {
                    errorList.AddRange(ParseErrMsg(ErrorUpdatePassword, resultPwd, superIdNameList));
                }
            }
            
            if (request.IsUpdateSecurityCode)
            {
                var criteriaInfo = new UpdateMultipleSuperSecurityCodeCriteria
                {
                    CustIDList = BuildUpdateCodeXML(custIds, request.SecurityCode),
                    IsForcedChangeSecurityCode = request.IsForceChange,
                    UserId = userId,
                    IP = ip,
                    System = "EXTERNAL"
                };

                var resultCode = dataAccess.ExecuteUpdate(criteriaInfo);
                if (!string.IsNullOrEmpty(resultCode))
                {
                    errorList.AddRange(ParseErrMsg(ErrorUpdateCode, resultCode, superIdNameList));
                }
            }
            return errorList;
        }

        private string BuildUpdateCodeXML(string custIds, string code)
        {
            var custIdList = custIds.Split(',');
            StringBuilder codeUpdateXmlList = new StringBuilder();
            foreach (var custId in custIdList)
            {
                var md5 = encryptor.ComputeHash("MD5", custId, Salt); //DevSkim: ignore DS126858
                var hashCode = encryptor.ComputeHash("SHA1", code, md5); //DevSkim: ignore DS126858
                codeUpdateXmlList.Append(string.Format("<SecurityCode CustID=\"{0}\" Code=\"{1}\" />", custId, hashCode));
            }

            return string.Format("<Root>{0}</Root>", codeUpdateXmlList);
        }

        private static IEnumerable<Result> ParseErrMsg(int errorType, string errMsg, List<string> superIdNameList)
        {
            var errMsgXml = XElement.Parse(errMsg);
            var errors = errMsgXml.Elements("Error");

            var errorList = new List<Result>();
            foreach (var err in errors)
            {
                var custId = (string)err.Element("CustID");
                var supername = superIdNameList.FirstOrDefault(sp => sp.Split(';')[0] == custId ).Split(';')[1];
                errorList.Add(new Result(errorType, err.Element("ErrMsg"), supername));
            }

            return errorList;
        } 
    }
}
