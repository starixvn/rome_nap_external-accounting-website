// -----------------------------------------------------------------------
// <copyright file="CommonService.cs" company="Microsoft">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------
// <history>
//   <change who="Robert.Tran" date="2013.2.05">Created</change>
// </history>


using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics.CodeAnalysis;
using System.Data;
using Fanex.Common.Extensions;
using Fanex.Data;
using Fanex.Puffin.Core.Criteria;
using Fanex.Puffin.Core.Models;
using Fanex.Puffin.Core.Repository.Impl;
using Fanex.Puffin.Core.Services.Interface;
using Fanex.Puffin.Core.Utils;

namespace Fanex.Puffin.Core.Services.Impl
{
    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented")]
    public class CoreService : ServiceBase, ICoreService
    {
        private readonly BaseRepository _repository;

        public CoreService()
        {
            _repository = new BaseRepository();
        }

        public IList<Bettypes> GetBettypes(GetBettypeCriteria criteria)
        {
            Func<IDataReader, Bettypes> mappingFunc = r => {
                return new Bettypes { 
                    Id = r.ToString("typeId"),
                    Name = r.ToString("BetTypeName"),
                    Type = r.ToInt("type"),
                    LiveIndicator = r.ToBool("LiveIndicator")
                };
            };

            IList<Bettypes> bettypes = _repository.Fetch<Bettypes>(criteria, mappingFunc);
            return bettypes;
        }

        public IList<Licensee> GetAllLicensees()
        {
            Func<IDataReader, Licensee> mappingFunc = r =>
            {
                return new Licensee
                {
                    Id = r.ToString("LicenseeID"),
                    Name = r.ToString("LicenseeName")
                };
            };

            IList<Licensee> licensees = _repository.Fetch<Licensee>(new GetAllLicenseeCriteria(), mappingFunc);
            return licensees;
        }

        public MatchInfo GetMatchInfo(GetMatchInfoCriteria criteria)
        {
            Func<IDataReader, MatchInfo>  mappingFunc = r =>
            {
                return new MatchInfo 
                {
                    MatchId = r.ToInt("MatchId"),
                    HomeId = r.ToInt("HomeId"),
                    AwayId = r.ToInt("AwayId"),
                    MatchCode = r.ToString("MatchCode"),
                    EventDate = r.ToDateTime("EventDate").ToString(FormatKey.SHORT_DATE_TIME_FORMAT)
                };
            };

            var matchInfo = _repository.Fetch<MatchInfo>(criteria, mappingFunc).FirstOrDefault();
            return matchInfo;
        }

        public IList<Currency> GetCurrency()
        {
            Func<IDataReader, Currency> mappingFunc = r =>
            {
                return new Currency
                {
                    Id = r.ToString("exchangeid"),
                    Name = r.ToString("currency")
                };
            };

            var obj = _repository.Fetch<Currency>(new GetCurrencyCriteria(), mappingFunc);
            return obj;
        }


        public void Test()
        {
            DataTable dt = _repository.Fetch(new TestCriteria());
            int c = dt.Rows.Count;
        }


        public Result CheckMatchIdWithRefno(CheckMatchIdWithRefNoCriteria criteria)
        {
            Result result = new Result();
            try
            {
                using (ObjectDb objectDb = new ObjectDb(criteria.GetSettingKey()))
                {
                    var ds = objectDb.ExecuteDataSet(criteria.GetSpParams());
                    var hashOutputParams = objectDb.GetOuputParams();

                    result = new Result()
                    {
                        ErrorCode = Convert.ToInt32(hashOutputParams["errcode"]),
                        ErrorMessage = Convert.ToString(hashOutputParams["errmsg"])
                    };
                }
            }
            catch (Exception ex)
            {
                LoggingHandler.Log(ex);
            }
            return result;
        }
    }
}
