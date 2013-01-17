namespace Fanex.ExternalAccount.DTO
{
    using System;

    public class Result
    {
        public Result(string errCode = null, string errMsg = null, object resultData = null)
        {
            this.ErrorCode = errCode;
            this.ErrorMessage = errMsg;
            this.ResultData = resultData;
        }

        public string ErrorCode { get; set; }

        public string ErrorMessage { get; set; }

        public object ResultData { get; set; }

        public bool IsSuccess
        {
            get
            {
                return string.IsNullOrWhiteSpace(this.ErrorCode) || Convert.ToInt32(this.ErrorCode) == 0;
            }
        }
    }
}