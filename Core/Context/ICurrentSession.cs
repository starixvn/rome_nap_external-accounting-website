// --------------------------------------------------------------------------
// <copyright file="ICurrentSession.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// <summary>
// For current session info
// </summary>
// --------------------------------------------------------------------------
// <history>
//   <change who="Andy.Nguyen" date="2012.12.14">Created</change>
// </history>
namespace Fanex.ExternalAccount.Core.Context
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    public interface ICurrentSession
    {
        string UserName { get; set; }

        int CustId { get; set; }

        int UserId { get; set; }

        bool IsLogin { get; }

        int OnUserId { get; set; }

        string NetSessionId { get; }

        string Captcha { get; set; }

        string CurrencyGroup { get; set; }

        bool IsInternalAdmin { get; set; }

        bool IsITAdmin { get; set; }
    }
}