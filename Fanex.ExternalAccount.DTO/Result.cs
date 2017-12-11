namespace Fanex.ExternalAccount.DTO
{
    using System;

    public class Result
    {
        public Result(object errCode = null, object errMsg = null, object resultData = null)
        {
            this.ErrorCode = errCode != null ? errCode.ToString() : string.Empty;
            this.ErrorMessage = errMsg != null ? errMsg.ToString() : string.Empty;
            this.ResultData = resultData;
        }

        public string ErrorCode { get; set; }

        public string ErrorMessage { get; set; }

        public object ResultData { get; set; }

        public bool IsSuccess
        {
            get
            {
                return string.IsNullOrWhiteSpace(this.ErrorCode) || Convert.ToInt32(this.ErrorCode) <= 0;
            }
        }
    }
}