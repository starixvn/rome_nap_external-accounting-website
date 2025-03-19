namespace Fanex.ExternalAccount.DataAccess.Belonging
{
    using System;
    using Fanex.Data.Repository;
    using Fanex.ExternalAccount.DataAccess.Criteria.Belonging;

    public interface IBelongingDataAccess
    {
        bool IsBelonging(CheckBelongingCommand command);

        bool CheckCustomerBelongingCriteria(CheckCustomerBelongingCommand command);
    }

    public class BelongingDataAccess : IBelongingDataAccess
    {
        private readonly IDynamicRepository dynamicRepository;

        public BelongingDataAccess(IDynamicRepository dynamicRepository)
        {
            this.dynamicRepository = dynamicRepository ?? new DynamicRepository();
        }

        public bool IsBelonging(CheckBelongingCommand command)
        {
            dynamicRepository.Execute(command);
            return command.IsBelonging;
        }

        public bool CheckCustomerBelongingCriteria(CheckCustomerBelongingCommand command)
        {
            dynamicRepository.Execute(command);
            return command.IsBelonging;
        }
    }
}
