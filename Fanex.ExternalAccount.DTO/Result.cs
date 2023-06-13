namespace Fanex.ExternalAccount.DTO
{
    using System;

    public class Result
    {
        public Result(object errCode = null, object errMsg = null, object resultData = null)
        {
            ErrorCode = (errCode ?? string.Empty).ToString();
            ErrorMessage = (errMsg ?? string.Empty).ToString();
            ResultData = resultData ?? string.Empty;
        }

        public string ErrorCode { get; set; }

        public string ErrorMessage { get; set; }

        public object ResultData { get; set; }

        public bool IsSuccess => string.IsNullOrWhiteSpace(ErrorCode) || Convert.ToInt32(ErrorCode) <= 0;
    }
}