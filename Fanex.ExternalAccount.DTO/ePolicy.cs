// --------------------------------------------------------------------------
// <copyright file="ePolicy.cs" company="">
// Copyright (c) . All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Andy.Nguyen" date="2013.9.17">Created</change>
// </history>

namespace Fanex.ExternalAccount.DTO
{
    public enum EPolicy : int
    {
        /// <summary>
        /// Create policy.
        /// </summary>
        Create = 4,

        /// <summary>
        /// Read policy.
        /// </summary>
        Read = 1,

        /// <summary>
        /// Update policy.
        /// </summary>
        Update = 2,

        /// <summary>
        /// Delete policy.
        /// </summary>
        Delete = 8,

        /// <summary>
        /// None policy.
        /// </summary>
        None = 0
    }
}