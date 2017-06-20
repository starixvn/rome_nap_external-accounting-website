using Fanex.CustomerSetting.Security.Authenticate;
using Fanex.ExternalAccount.Core.Context;

namespace Fanex.ExternalAccount.Core.Authorization.ThirdPartyStatus
{
    public class ProductStatusAuthorization
    {
        private readonly ExternalAccountAuthenticate _authenticationService;

        public ProductStatusAuthorization() : this(null)
        {
        }

        public ProductStatusAuthorization(ExternalAccountAuthenticate authenticationService)
        {
            _authenticationService = authenticationService ?? new ExternalAccountAuthenticate();
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
            switch (productId)
            {
                case 2:
                    return EnableStatusProduct.EnableRacing.ToString();
                case 5:
                    return EnableStatusProduct.EnableVirtualSports.ToString();
                case 6:
                    return EnableStatusProduct.EnableCasino.ToString();
                case 8:
                    return EnableStatusProduct.EnableKeno.ToString();
                case 15:
                    return EnableStatusProduct.EnableColossusBets.ToString();
                case 21:
                    return EnableStatusProduct.EnableAllbet.ToString();
                case 22:
                    return EnableStatusProduct.EnableVoidbridge.ToString();
                case 23:
                    return EnableStatusProduct.EnableNG2.ToString();
                case 24:
                    return EnableStatusProduct.EnableFishingWorld.ToString();
                default:
                    return string.Empty;
            }
        }
    }
}
