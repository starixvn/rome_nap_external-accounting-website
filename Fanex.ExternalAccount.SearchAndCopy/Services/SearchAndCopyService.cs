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
        private readonly ICustomerRepository customerRepository;      
        private readonly IServiceBase serviceBase;

        public SearchAndCopyService()
            : this(null, null, null)
        {
        }

        public SearchAndCopyService(
            ISearchAndCopyRepository searchAndCopyRepository,
            ICustomerRepository customerRepository,
            IServiceBase serviceBase)
        {
            this.searchAndCopyRepository = searchAndCopyRepository ?? new SearchAndCopyRepository();
            this.customerRepository = customerRepository ?? new CustomerRepository();
            this.serviceBase = serviceBase ?? new ServiceBase();
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
            var rawPassword = Encryptor.Decrypt(context.Request.Password, Encryptor.RandomCaptcha);

            var cloneSettingComCriteria = new CloneSettingComCriteria
            {
                Username = context.Request.Username,
                UserPwd = Encryption.CfsEnCode(rawPassword),
                FirstName = context.Request.FirstName,
                LastName = context.Request.LastName,
                Phone = context.Request.Phone,
                MobilePhone = context.Request.MobilePhone,
                Fax = context.Request.Fax,
                Credit = context.Request.Credit,
                ExistingCustId = context.ExistingCustId,
                GroupAdmin = context.Request.GroupName,
                Creator = context.CurrentSession.CustId,
                IP = context.CurrentContext.Ip4Address,
                SettingStatus = Config.SettingStatus
            };

            int Case = 0;
            int HeadlineID_Old = 0;
            int HeadlineID_New = 0;
            context.Result = searchAndCopyRepository.CloneSettingComm(cloneSettingComCriteria, ref Case, ref HeadlineID_Old, ref HeadlineID_New);

            if (!context.Result.IsSuccess)
            {
                return context.Result;
            }
            
            context.CustId = Convert.ToInt32(context.Result.ResultData);
            serviceBase.AddMsgAfterCreateNewSMA(new AddMsgAfterCreateNewSMACriteria(context.CustId, 0, Role.Super));

            var cloneSettingExtraPTCriteria = new CloneSettingExtraPTCriteria
            {
                CustId = context.CustId,
                ExistingCustId = context.ExistingCustId,
                Creator = context.CurrentSession.CustId,
                IP = context.CurrentContext.Ip4Address
            };

            context.Result = searchAndCopyRepository.CloneSettingExtraPT(cloneSettingExtraPTCriteria);
            if (!context.Result.IsSuccess)
            {
                return context.Result;
            }

            if (Case != 0)
            {
                var updateCRMHeadlineInfoCriteria = new UpdateCrmHeadlineInfoCriteria
                {
                    CustId = context.CustId,
                    UserName = string.Empty,
                    LoginIP = context.CurrentContext.Ip4Address,
                    UserId = context.CurrentSession.CustId,
                    Case = Case,
                    Old_Headline = HeadlineID_Old,
                    New_Headline = HeadlineID_New
                };

                context.Result = searchAndCopyRepository.UpdateCRMHeadlineInfo(updateCRMHeadlineInfoCriteria);

                if (!context.Result.IsSuccess)
                {
                    return context.Result;
                }
            }

            return context.Result;
        }       
    }
}