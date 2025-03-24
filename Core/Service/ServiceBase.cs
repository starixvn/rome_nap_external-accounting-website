using Fanex.ExternalAccount.Core.Criteria;
namespace Fanex.ExternalAccount.Core.Service
{
    using Fanex.Data.Repository;
    using Fanex.ExternalAccount.Core.Model;


    public interface IServiceBase
    {
        CheckCustomerBelongToSpecialSiteResult CheckCustomerBelongToSpecialSite(CheckCustomerIsSpecialSiteCommand command);

    }

    public class ServiceBase: IServiceBase
    {
        private readonly IDynamicRepository _dynamicRepository;

        public ServiceBase() : this(null)
        {
        }

        public ServiceBase(IDynamicRepository dynamicRepository)
        {
            _dynamicRepository = dynamicRepository ?? new DynamicRepository();
        }

        public CheckCustomerBelongToSpecialSiteResult CheckCustomerBelongToSpecialSite(CheckCustomerIsSpecialSiteCommand command)
        {
            _dynamicRepository.Execute(command);
            return new CheckCustomerBelongToSpecialSiteResult(command.IsSpecialSite, command.IsSpecialHeadline, command.Site);
        }
    }
}
