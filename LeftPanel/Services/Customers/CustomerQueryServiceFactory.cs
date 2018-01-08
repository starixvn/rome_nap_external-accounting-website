namespace Fanex.ExternalAccount.Module.LeftPanel.Services.Customers
{
    using Core.Shared;

    public interface CustomerQueryServiceFactory
    {
        CustomerInfoQueryService CreateQueryService(int roleId);
    }

    public class CustomerQueryServiceFactoryImpl : CustomerQueryServiceFactory
    {
        public CustomerInfoQueryService CreateQueryService(int roleId)
        {
            switch (roleId)
            {
                case Role.Super:
                    return new SuperInfoQueryServiceImpl();

                case Role.Master:
                    return new MasterInfoQueryServiceImpl();

                case Role.Agent:
                    return new AgentInfoQueryServiceImpl();

                default:
                    return new MemberInfoQueryServiceImpl();
            }
        }
    }
}