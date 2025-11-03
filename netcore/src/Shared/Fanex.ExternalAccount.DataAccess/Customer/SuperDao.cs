namespace Fanex.ExternalAccount.DataAccess.Impl.Customer
{
    using Criteria.Customer;
    using DTO;
    using Fanex.Data.Repository;
    using Interface.Customer;

    public class SuperDao : CustomerDao, ISuperDao
    {

        private readonly IDynamicRepository _dynamicRepository;

        public SuperDao() : this(null)
        {
        }

        public SuperDao(IDynamicRepository dynamicRepository)
        {
            _dynamicRepository = dynamicRepository ?? new DynamicRepository();
        }

        public Result UpdateSuperInfo(UpdateSuperInfoCommand command)
        {
            _dynamicRepository.Execute(command);
            return new Result { ErrorCode = command.ErrCode.ToString(), ErrorMessage = command.ErrMsg };
        }
    }
}