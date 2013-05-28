// -----------------------------------------------------------------------
// <copyright file="GetMatchInfoCriteria.cs" company="Microsoft">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

using System;
using Fanex.NAP.Framework.Data;

namespace Fanex.Puffin.Core.Criteria
{
    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class GetMatchInfoCriteria : CriteriaBase
    {

        public int MatchId { get; set; }

        public Int64 TransId { get; set; }

        public override string GetSettingKey()
        {
            return "Core_MatchSelByID";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
