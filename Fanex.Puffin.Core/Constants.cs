// --------------------------------------------------------------------------
// <copyright file="Constants.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Ricky.Nguyen" date="2012.11.12">Created</change>
// </history>

using System.Diagnostics.CodeAnalysis;

namespace Fanex.Puffin.Core
{
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented")]
    public static class Constants
    {
        public const string SHORT_DATE_FORMAT = "MMM dd, yyyy";
        public const string MONTH_YEAR_DATE_FORMAT = "MMM yyyy";
        public const string SHORT_DATE_FORMAT_WITH_SLASH = "MM/dd/yyyy";
        public const string LONG_DATE_FORMAT_WITH_SLASH = "MM/dd/yyyy hh:mm:ss tt";
        public const string MONTH_YEAR_DATE_FORMAT_WITH_SLASH = "MM/yyyy";
        public const string LONG_DATE_FORMAT = "dd MMM, yyyy hh:mm:ss tt";
        public const string CURRENT_LOG_IN_USER = "Fanex.Notus.Core.Contants.AppSession.CurrentLogInUser";
        public const string CURRENT_USER_MENU = "Fanex.Notus.Core.Contants.AppSession.CurrentUserMenu";
        public const string CURRENT_USER_ROLE_ACCESS = "Fanex.Notus.Core.Contants.AppSession.CurrentUserRoleAccess";
        public const string CURRENT_WORKING_ON_MENU = "Fanex.Notus.Core.Contants.AppSession.CurrentWorkingOnMenu";
        public const string CURRENT_ACTION_NAME = "Fanex.Notus.Core.Contants.AppSession.CurrentActionName";
        public const string TWO_DECIMAL_DIGIT_FORMAT = ".00";
        public const string FOUR_DECIMAL_DIGIT_FORMAT = ".0000";
        public const string SP_PARAMS_SEPERATOR = ",";
        public const string VIEW_IP_VN = "ViewIPVN";
        public const string ARES_DATE_FORMAT = "dd-MMM-yy";
        public const string ARES_MONTH_YEAR_FORMAT = "MMM-yy";
    }
}
