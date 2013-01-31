// --------------------------------------------------------------------------
// <copyright file="MasterService.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Andy.Nguyen" date="2012.12.28">Created</change>
// </history>

namespace Fanex.ExternalAccount.Service.Impl.Customer
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Fanex.ExternalAccount.DataAccess;
    using Fanex.ExternalAccount.DataAccess.Interface.Customer;
    using Fanex.ExternalAccount.DataAccess.Impl.Customer;
    using Fanex.ExternalAccount.DTO;
    using Fanex.ExternalAccount.DTO.Customer;
    using Fanex.ExternalAccount.Service.Interface.Customer;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class MasterService : CustomerService, IMasterService
    {
        private readonly IMasterDao dao = new MasterDao();

        #region IDataService Members

        public System.Data.DataTable GetData(int? recommend = null, int? custId = null, bool? enabled = null, bool? closed = null, bool? suspended = null)
        {
            try
            {
                return this.dao.GetData(recommend, custId, enabled, closed, suspended);
            }
            catch (Exception ex)
            {
                this.ProcessException(ex);
            }

            return null;
        }

        public Transfer GetTransferOption(int custId)
        {
            try
            {
                return this.dao.GetTransferOption(custId);
            }
            catch (Exception ex)
            {
                this.ProcessException(ex);
            }

            return null;
        }

        public Result EditTransferOption(string transferOption, int custId, int userId, string ip, int subAccId, string language)
        {
            try
            {
                return this.dao.EditTransferOption(transferOption, custId, userId, ip, subAccId, language);
            }
            catch (Exception ex)
            {
                this.ProcessException(ex);
            }

            return null;
        }

        #endregion IDataService Members
    }
}