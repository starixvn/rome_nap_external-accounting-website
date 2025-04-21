namespace Fanex.ProductStatus.Setting.Models
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Xml.Linq;
    using AppResources;
    using Fanex.ProductStatus.Setting.Services;
    using Fanex.ProductStatus.Setting.Shared;

    public class Result
    {
        private const int InternalErrorCode = 500;
        public const int UMNonDCSErrorCode = 90014;
        private const int DoNothingErrorCode = 90017;
        private const int ParentChildViolated = 92;
        private readonly RefDataService _refDataService;

        public Result(int code, string message, RefDataService refDataService = null)
        {
            Code = code;
            Message = message;
            _refDataService = refDataService ?? new RefDataServiceImpl();
        }

        public Result(Hashtable outputParams, RefDataService refDataService = null)
        {
            CreateResult(outputParams);
            _refDataService = refDataService ?? new RefDataServiceImpl();
        }

        public int Code { get; private set; }

        public string Message { get; private set; }

        public int MsgType { get; private set; }

        public bool IsSuccess()
            => Code == 0;

        public bool IsUMOfNonDCS()
            => Code == UMNonDCSErrorCode;

        public bool IsDoNothing()
            => Code == DoNothingErrorCode;

        public string ParseDCSProductMessage()
        {
            if (IsValidXml())
            {
                if (MsgType == ParentChildViolated)
                {
                    return ParseParentChildViolatedMessages();
                }

                var errorMessages = new List<string>();

                var xmlRoot = XElement.Parse(Message);
                var errCode = xmlRoot.Attributes("MsgID").FirstOrDefault().Value;
                errorMessages.Add(Resources.ResourceManager.GetString("Msg" + errCode));

                Message = string.Join("<br/>", errorMessages);
            }

            return Message;
        }

        private string ParseParentChildViolatedMessages()
        {
            var xmlRoot = XElement.Parse(Message);
            var errCode = xmlRoot.Attributes("MsgID").FirstOrDefault().Value;
            string parentSportType = string.Empty;
            string parentBetType = string.Empty;
            var errMsgTemplate = Resources.ResourceManager.GetString("Msg" + errCode);
            List<string> childSportTypesNames = new List<string>();
            List<string> errorMessages = new List<string>();
            foreach (var xmlElement in xmlRoot.Elements())
            {
                var currentParentSportType = xmlElement.Attribute("ParentSportType").Value;
                var currentParentBetType = xmlElement.Attribute("ParentBetType").Value;
                var currentSportType = xmlElement.Attribute("SportType").Value;
                if (currentParentSportType != parentSportType || currentParentBetType != parentBetType)
                {
                    if (!string.IsNullOrWhiteSpace(parentSportType))
                    {
                        errorMessages.Add(string.Format(errMsgTemplate, string.Join(", ", childSportTypesNames), _refDataService.GetSportTypeName(Convert.ToInt32(parentSportType))));
                    }

                    parentSportType = currentParentSportType;
                    parentBetType = currentParentBetType;
                    childSportTypesNames = new List<string>();
                }

                childSportTypesNames.Add(_refDataService.GetSportTypeName(Convert.ToInt32(currentSportType)));
            }

            errorMessages.Add(string.Format(errMsgTemplate, string.Join(", ", childSportTypesNames), _refDataService.GetSportTypeName(Convert.ToInt32(parentSportType))));
            Message = string.Join("<br/>", errorMessages);
            return Message;
        }

        private void CreateResult(Hashtable outputParams)
        {
            if (outputParams == null
                || !outputParams.ContainsKey("errcode")
                || !outputParams.ContainsKey("errmsg"))
            {
                Code = InternalErrorCode;
                Message = Resources.InternalServerError;
            }
            else
            {
                Code = Convert.ToInt32(outputParams["errcode"]);
                MsgType = Convert.ToInt32(outputParams["msgtype"]);
                Message = IsSuccess()
                    ? Resources.StatusUpdatedSuccessfully
                    : outputParams["errmsg"].ToString();
            }
        }

        private bool IsValidXml()
        {
            var isValidXml = false;

            if (!string.IsNullOrWhiteSpace(Message) && Message.TrimStart().StartsWith("<"))
            {
                try
                {
                    XDocument.Parse(Message);
                    isValidXml = true;
                }
                catch (Exception)
                {
                }
            }

            return isValidXml;
        }
    }
}