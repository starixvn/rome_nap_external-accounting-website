// --------------------------------------------------------------------------
// <copyright file="BalanceUp.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Louis.Nguyen" date="2012.10.18">Created</change>
// </history>

using System;
using System.Diagnostics.CodeAnalysis;

namespace Fanex.Puffin.Core.Models
{
    /// <summary>
    /// BalanceUp: getting balance up day and move bet day
    /// </summary>
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented")]
    public class BalanceUp 
    {
        public DateTime BalanceUpDay { get; set; }

        public DateTime MoveBetsDay { get; set; }
    }
}
