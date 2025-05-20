namespace Fanex.ExternalAccount.DataAccess.OTP
{
    using DTO.OTP;
    using Fanex.Data.Repository;
    using Fanex.ExternalAccount.DataAccess.Criteria.OTP;

    public interface ICustomerOtpDataAccess
    {
        CustomerOtp GetCustomerOtpDto(int custId);
    }

    public class CustomerOtpDataAccess : ICustomerOtpDataAccess
    {

        private readonly IDynamicRepository _dynamicRepository;

        public CustomerOtpDataAccess() : this(null)
        {
        }

        public CustomerOtpDataAccess(IDynamicRepository dynamicRepository)
        {
            _dynamicRepository = dynamicRepository ?? new DynamicRepository();
        }

        public CustomerOtp GetCustomerOtpDto(int custId)
            => _dynamicRepository.Get<CustomerOtp>(new GetCustomerOtpDtoCriteria(custId)) ?? new CustomerOtp();
    }
}