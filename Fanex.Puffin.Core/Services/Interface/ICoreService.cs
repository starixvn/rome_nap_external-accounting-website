// -----------------------------------------------------------------------
// <copyright file="ICommonService.cs" company="Microsoft">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

using System.Collections.Generic;
using Fanex.Puffin.Core.Criteria;
using Fanex.Puffin.Core.Models;

namespace Fanex.Puffin.Core.Services.Interface
{
    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public interface ICoreService
    {
        IList<Bettypes> GetBettypes(GetBettypeCriteria criteria);

        IList<Licensee> GetAllLicensees();

        MatchInfo GetMatchInfo(GetMatchInfoCriteria criteria);

        IList<Currency> GetCurrency();

        void Test();

        Result CheckMatchIdWithRefno(CheckMatchIdWithRefNoCriteria criteria);
    }
}
