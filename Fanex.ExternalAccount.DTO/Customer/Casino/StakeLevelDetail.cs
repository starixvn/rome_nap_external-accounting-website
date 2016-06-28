// --------------------------------------------------------------------------
// <copyright file="StakeLevelDetail.cs" company="">
// Copyright (c) . All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Andy.Nguyen" date="2013.4.11">Created</change>
// </history>

namespace Fanex.ExternalAccount.DTO.Customer.Casino
{
    /// <summary>
    /// StakeLevelDetail object.
    /// </summary>
    public class StakeLevelDetail
    {
        public int StakeLevelID
        {
            get;
            set;
        }

        public string StakeLevelName
        {
            get;
            set;
        }

        public bool IsChecked
        {
            get;
            set;
        }
    }
}
