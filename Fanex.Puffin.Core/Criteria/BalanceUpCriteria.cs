// --------------------------------------------------------------------------
// <copyright file="BalanceUpCriteria.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Louis.Nguyen" date="2012.10.18">Created</change>
// </history>

using Fanex.NAP.Framework.Data;

namespace Fanex.Puffin.Core.Criteria
{
    /// <summary>
    /// BalanceUpCriteria: criteria for getting balance up day and move bet day
    /// </summary>
    public class BalanceUpCriteria : CriteriaBase
    {
        /// <summary>
        /// Get the reference SP's Name which is specified in Config file.
        /// </summary>
        /// <returns>Store procedure alias name</returns>
        public override string GetSettingKey()
        {
            return "SelBalanceUpDay";
        }

        /// <summary>
        /// Check data's constrains
        /// </summary>
        /// <returns>True: valid; False: invalid</returns>
        public override bool IsValid()
        {
            return true;
        }
    }
}
