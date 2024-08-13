namespace Fanex.ExternalAccount.SearchAndCopy.Services
{
    using Fanex.CustomerSetting.Module.Core.Criteria;
    using Fanex.CustomerSetting.Module.Core.Services.Impl;
    using Fanex.CustomerSetting.Module.Core.Services.Interface;
    using Fanex.ExternalAccount.Core.Context;
    using Fanex.ExternalAccount.Core.Shared;
    using Fanex.ExternalAccount.DTO;
    using Fanex.ExternalAccount.SearchAndCopy.Criteria;
    using Fanex.ExternalAccount.SearchAndCopy.Models;
    using Fanex.ExternalAccount.SearchAndCopy.Repositories;
    using Fanex.ExternalAccount.SearchAndCopy.Resources;
    using Fanex.ExternalAccount.Service.Impl.Customer;
    using Fanex.ExternalAccount.Utilities;
    using IBCUtils;
    using System;
    using System.Linq;

    public interface ISearchAndCopyService
    {
        Result SearchCustomer(SearchCustomerContext context);

        Result CloneSetting(CloneSettingContext context);

        dynamic GetSuperPrefixes(SuperPrefixCriteria criteria);
    }

    public class SearchAndCopyService : ISearchAndCopyService
    {
        private readonly ISearchAndCopyRepository searchAndCopyRepository;
        private readonly IServiceBase serviceBase;
        private readonly ISiteDBConfigService siteDBConfigService;

        public SearchAndCopyService()
            : this(null, null, null)
        {
        }

        public SearchAndCopyService(
            ISearchAndCopyRepository searchAndCopyRepository,
            ISiteDBConfigService siteDBConfigService,
            IServiceBase serviceBase)
        {
            this.searchAndCopyRepository = searchAndCopyRepository ?? new SearchAndCopyRepository();
            this.serviceBase = serviceBase ?? new ServiceBase();
            this.siteDBConfigService = siteDBConfigService ?? new SiteDBConfigService();
        }

        public Result SearchCustomer(SearchCustomerContext context)
        {
            var customer = searchAndCopyRepository.SearchCustomer(context.Request.Username, context.CurrentSession.CurrencyGroup)
                .FirstOrDefault(c => c.RoleId == context.RoleId && context.Request.Username.Equals(c.Username, StringComparison.InvariantCultureIgnoreCase) && c.IsExistGroup);
            if (customer == null)
            {
                return new Result(1, Lang.SearchSuperNotExisted);
            }

            if (!customer.IsValidSuperPrefix)
            {
                return new Result(2, Lang.SuperNotFollowPrefix);
            }

            return new Result(0, string.Empty, customer);
        }

        public Result CloneSetting(CloneSettingContext context)
        {
            return CloneCustomerAndSettings(context);
        }

        public dynamic GetSuperPrefixes(SuperPrefixCriteria criteria)
            => searchAndCopyRepository.GetSuperPrefixes(criteria)
            .GroupBy(item => item.GroupName, item => item.Prefix, (groupName, prefixes) => new
            {
                GroupName = groupName,
                Prefixes = string.Join(", ", prefixes)
            });

        private Result CloneCustomerAndSettings(CloneSettingContext context)
        {
            CopyCustomer(context);

            if (!context.Result.IsSuccess)
            {
                return context.Result;
            }

            var resultData = ((dynamic)context.Result.ResultData);
            var newCustId = resultData.CustId;
            var _case = resultData.Case;
            var headlineID_Old = resultData.HeadlineID_Old;
            var headlineID_New = resultData.HeadlineID_New;

            serviceBase.AddMsgAfterCreateNewSMA(new AddMsgAfterCreateNewSMACriteria(newCustId, 0, Role.Super));

            var cloneSettingExtraPTCriteria = new CloneSettingExtraPTCriteria
            {
                CustId = newCustId,
                ExistingCustId = context.ExistingCustId,
                Creator = context.CurrentSession.CustId,
                IP = context.CurrentContext.Ip4Address
            };

            context.Result = searchAndCopyRepository.CloneSettingExtraPT(cloneSettingExtraPTCriteria);
            if (!context.Result.IsSuccess)
            {
                return context.Result;
            }

            if (_case != 0)
            {
                var updateCRMHeadlineInfoCriteria = new UpdateCrmHeadlineInfoCriteria
                {
                    CustId = newCustId,
                    UserName = string.Empty,
                    LoginIP = context.CurrentContext.Ip4Address,
                    UserId = context.CurrentSession.CustId,
                    Case = _case,
                    Old_Headline = headlineID_Old,
                    New_Headline = headlineID_New
                };

                context.Result = searchAndCopyRepository.UpdateCRMHeadlineInfo(updateCRMHeadlineInfoCriteria);

                if (!context.Result.IsSuccess)
                {
                    return context.Result;
                }
            }

            return context.Result;
        }

        private void CopyCustomer(CloneSettingContext context)
        {
            context.Result = searchAndCopyRepository.ValidateSettingComm(new ValidateCloneSettingComCriteria
            {
                Username = context.Request.Username,
                Credit = context.Request.Credit,
                ExistingCustId = context.ExistingCustId,
                IP = context.CurrentContext.Ip4Address
            });
            if (!context.Result.IsSuccess)
            {
                return;
            }

            context.Result = searchAndCopyRepository.RegisterCustomer(new RegisterCustomerCriteria
            {
                Username = context.Request.Username,
                SiteDbGroupId = siteDBConfigService.GetSiteDBConfigByCustId(context.ExistingCustId).CustDBGroupID,
                RoleId = Role.Super

            });
            if (!context.Result.IsSuccess)
            {
                return;
            }

            var newCustId = ((dynamic)context.Result.ResultData).NewCustId;
            var groupNames = searchAndCopyRepository.GetGetGroupParentNameByGroupName(new GetGroupParentNameByGroupNameCriteria
            {
                GroupAdmin = context.CurrentSession.CurrencyGroup

            });

            context.Result = searchAndCopyRepository.CloneSettingComm(new CloneSettingComCriteria
            {
                CustId = newCustId,
                Username = context.Request.Username,
                UserPwd = Encryption.CfsEnCode(Encryptor.Decrypt(context.Request.Password, Encryptor.RandomCaptcha)),
                FirstName = context.Request.FirstName,
                LastName = context.Request.LastName,
                Phone = context.Request.Phone,
                MobilePhone = context.Request.MobilePhone,
                Fax = context.Request.Fax,
                Credit = context.Request.Credit,
                ExistingCustId = context.ExistingCustId,
                Creator = context.CurrentSession.CustId,
                GroupAdmin = context.Request.GroupName,
                GroupName = groupNames,
                IP = context.CurrentContext.Ip4Address,
                SettingStatus = Config.SettingStatus
            });

            if (!context.Result.IsSuccess)
            {
                searchAndCopyRepository.RollBackCustomer(new RollbackCustomerInsertCriteria
                {
                    CustId = newCustId
                });
            }
        }
    }
}