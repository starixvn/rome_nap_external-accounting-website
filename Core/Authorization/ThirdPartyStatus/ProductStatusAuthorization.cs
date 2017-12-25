using System;
using Fanex.CustomerSetting.Security.Authenticate;
using Fanex.ExternalAccount.Core.Context;

namespace Fanex.ExternalAccount.Core.Authorization.ThirdPartyStatus
{
    public class ProductStatusAuthorization
    {
        private readonly ExternalAccountAuthenticateAttribute _authenticationService;

        public ProductStatusAuthorization() : this(null)
        {
        }

        public ProductStatusAuthorization(ExternalAccountAuthenticateAttribute authenticationService)
        {
            _authenticationService = authenticationService ?? new ExternalAccountAuthenticateAttribute();
        }

        public bool HasPermission(AuthenticateAction.AuthenticationType role, int productId)
        {
            var enableFunction = MappingEnableStatus(productId);
            return role == AuthenticateAction.AuthenticationType.Read ? 
                _authenticationService.AllowReadByAuthenticationParam(enableFunction) : 
                _authenticationService.AllowUpdateByAuthenticationParam(enableFunction);
        }

        private string MappingEnableStatus(int productId)
        {
            var enumName = Enum.GetName(typeof(EnableStatusProductFunction), productId);

            return enumName;
        }
    }
}
