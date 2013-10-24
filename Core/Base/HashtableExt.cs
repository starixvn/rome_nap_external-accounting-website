// --------------------------------------------------------------------------
// <copyright file="HashtableExt.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Andy.Nguyen" date="2013.10.24">Created</change>
// </history>

namespace Fanex.ExternalAccount.Core.Base
{
    using System.Collections;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class HashtableExt : Hashtable
    {
        public bool IsExternalAccountingSystem
        {
            get
            {
                return true;
            }
        }

        public string IsExternalAccountingParam
        {
            get
            {
                return "IsExtAcc";
            }
        }
    }
}