namespace Fanex.ExternalAccount.Service.OTP
{
    using System;
    using System.Collections;
    using Data;
    using DTO;
    using DTO.OTP;
    using NAP.Framework.Data;
    using ONEbook.OTP.Services;
    using Utilities;

    public class CustomerOtpService : ICustomerOtpService
    {
        private const int TimePeriod = 30;
        private const int OtpSize = 6;

        private readonly IDynamicRepository dynamicRepository;
        private readonly OtpService otpService;

        public CustomerOtpService(IDynamicRepository dynamicRepository = null, OtpService otpService = null)
        {
            this.dynamicRepository = dynamicRepository?? new DynamicRepository();
            this.otpService = otpService?? new OtpServiceImpl();
        }

        public Result VerifyOtp(CustomerOtp param)
        {
            if (param == null)
            {
                throw new ArgumentNullException(nameof(param), "Verify Otp Param Parameters cannot be null");
            }

            var customerOtp = GetCustomerOtp(param.CustId, param.SubAccId);

            var isCorrect = otpService.VerifyOtp(customerOtp.SecretKey, param.Otp, TimePeriod, OtpSize);

            return new Result(isCorrect ? -1 : 100, isCorrect ? "Verify okie" : "Verify fail");
        }

        public CustomerOtpDto GetCustomerOtpDto(int custId)
        {
            var param = new Hashtable
            {
                { "custid", custId }
            };

            // Cannot use dynamic mapping here due to old fanex.data
            var CustId = 0;
            var Enabled = false;
            var SecretKey = string.Empty;
            using (var db = new ObjectDb("Age_OTP_CustomerOTPSel"))
            {
                var reader = db.ExecuteReader(param);

                while (reader.Read())
                {
                    CustId = reader.ToInt32("CustID");
                    Enabled= reader.ToBoolean("Enabled");
                    SecretKey = reader.ToString("SecretKey");
                }

                return new CustomerOtpDto
                {
                    CustId = CustId,
                    Enabled = Enabled,
                    SecretKey = SecretKey,
                    SubAccId = 0
                };
            }
        }

        public virtual CustomerOtp GetCustomerOtp(int custId, int subAccId = 0)
        {
            try
            {
                if (custId <= 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(custId), "Cust Id cannot be negative or zero");
                }

                if (subAccId < 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(custId), "Sub account Id cannot be negative");
                }

                var customerOtpDto = GetCustomerOtpDto(custId);

                if (customerOtpDto == null)
                {
                    return new CustomerOtp();
                }

                return new CustomerOtp
                {
                    CustId = customerOtpDto.CustId,
                    Enabled = customerOtpDto.Enabled,
                    SecretKey = customerOtpDto.SecretKey,
                    SubAccId = customerOtpDto.SubAccId
                };
            }
            catch (Exception ex)
            {
                return new CustomerOtp();
            }
        }
    }
}