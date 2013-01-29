// --------------------------------------------------------------------------
// <copyright file="ICustomerService.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Andy.Nguyen" date="2012.12.28">Created</change>
// </history>

namespace Fanex.ExternalAccount.Service.Interface.Customer
{
    using Fanex.ExternalAccount.DTO.Customer;
    using Fanex.ExternalAccount.DTO;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public interface ICustomerService
    {
        Customer UserInfo(int custId);

        Result EditUserInfo(int custId, string firstName, string lastName, string homePhone, string mobilePhone,
                                    string email, string fax, string ip, int userId, int subAccId, string language);


        Result EditTransferOption(int custId, Transfer transferOption, Request request);

        Result ResetPassword(int custId, string password, Request request);

        Result ConfigDoubleComm(int custId, bool allow, Request request);
    }
}