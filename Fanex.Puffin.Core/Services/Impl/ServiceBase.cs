// --------------------------------------------------------------------------
// <copyright file="ServiceBase.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Anders.Le" date="2012.10.9">Created</change>
//   <change who="Ricky.Nguyen" date="2012.11.6">Move</change>
// </history>

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Fanex.Common.Helper;
using Fanex.NAP.Framework.Data;
using Fanex.Caching;
using System.Web.Mvc;
using System.Data;
using Fanex.NAP.Core.Web.Context;
using Fanex.NAP.Core.Web.Session;
using Fanex.Puffin.Core.Criteria;
using Fanex.Puffin.Core.Repository.Impl;
using Fanex.Puffin.Core.Services.Interface;

namespace Fanex.Puffin.Core.Services.Impl
{
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented")]
    public class ServiceBase : IServiceBase
    {
        protected readonly IDynamicRepository DynamicRepository;

        protected readonly ICache _cacheManager;

        protected readonly BaseRepository _repository;

        protected readonly ICurrentContext _context;

        protected readonly ICurrentSession _session;

        public ServiceBase(IDynamicRepository repository = null)
        {
            this.DynamicRepository = repository ?? new DynamicRepository();
            this._cacheManager = CacheService.CreateInstance();
            this._repository = new BaseRepository();
            this._context = this._context ?? new CurrentContext();
            this._session = this._session ?? new CurrentSession();
        }

        /// <summary>
        ///  Get dynamic repository
        /// </summary>
        protected IDynamicRepository Repository
        {
            get { return this.DynamicRepository; }
        }

        /// <summary>
        /// Handler Exception
        /// </summary>
        /// <param name="ex"></param>
        public void HandleException(Exception ex)
        {
            //LogService.Error(ex.Message, ex);
            LoggingHandler.Log(ex);
        }

        /// <summary>
        /// Get Report Data with Criteria
        /// </summary>
        /// <typeparam name="T">Type of Object return</typeparam>
        /// <param name="criteria">Criteria to Query DB</param>
        /// <returns>Enumerable of Type T</returns>
        public virtual IEnumerable<T> GetListData<T>(ICriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");

            try
            {
                if (criteria.IsValid())
                {
                    return DynamicRepository.Fetch<T>(criteria);
                }

                return new List<T>();
            }
            catch (Exception ex)
            {
                HandleException(ex);
                return null;
            }
        }

        #region Get data for combobox
        public IEnumerable<SelectListItem> GetAllLicensees()
        {
            IList<SelectListItem> listLicensees = new List<SelectListItem>();
            try
            {
                Func<IDataReader, SelectListItem> mappingFunc = r =>
                {
                    return new SelectListItem
                    {
                        Value = Convert.ToString(r["LicenseeID"]),
                        Text = Convert.ToString(r["LicenseeName"])
                    };
                };

                listLicensees = _repository.Fetch<SelectListItem>(new IMS_LicenseeSelCriteria(), mappingFunc);
            }
            catch (Exception ex)
            {
                LoggingHandler.Log(ex);
            }
            return listLicensees;
        }


        public IEnumerable<SelectListItem> GetProductsByLicensee(IMS_LicenseeProductSelCriteria criteria)
        {
            IList<SelectListItem> listProducts = new List<SelectListItem>();
            try
            {
                Func<IDataReader, SelectListItem> mappingFunc = r =>
                {
                    return new SelectListItem
                    {
                        Value = Convert.ToString(r["productid"]),
                        Text = Convert.ToString(r["productname"])
                    };
                };

                listProducts = _repository.Fetch<SelectListItem>(criteria, mappingFunc);
            }
            catch (Exception ex)
            {
                LoggingHandler.Log(ex);
            }
            return listProducts;
        }

        public IEnumerable<SelectListItem> GetCurrencyByLicensee(IMS_LicenseeCurrencySelCriteria criteria)
        {
            IList<SelectListItem> listProducts = new List<SelectListItem>();
            try
            {
                Func<IDataReader, SelectListItem> mappingFunc = r =>
                {
                    return new SelectListItem
                    {
                        Value = Convert.ToString(r["currency"]),
                        Text = Convert.ToString(r["currencyname"])
                    };
                };

                listProducts = _repository.Fetch<SelectListItem>(criteria, mappingFunc);
            }
            catch (Exception ex)
            {
                LoggingHandler.Log(ex);
            }

            return listProducts;
        }

        public IEnumerable<SelectListItem> GetBaseCurrency(IMS_BaseCurrencySel criteria)
        {
            IList<SelectListItem> listProducts = new List<SelectListItem>();
            try
            {
                Func<IDataReader, SelectListItem> mappingFunc = r =>
                {
                    return new SelectListItem
                    {
                        Value = Convert.ToString(r["currency"]),
                        Text = Convert.ToString(r["currencyname"])
                    };
                };

                listProducts = _repository.Fetch<SelectListItem>(criteria, mappingFunc);
            }
            catch (Exception ex)
            {
                LoggingHandler.Log(ex);
            }

            return listProducts;
        }
        #endregion
    }
}