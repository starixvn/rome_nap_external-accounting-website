// --------------------------------------------------------------------------
// <copyright file="IReportBuilder.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Andy.Nguyen" date="2013.9.17">Created</change>
// </history>

namespace Fanex.ExternalAccount.Excel
{
    using NPOI.HSSF.UserModel;

    public interface IReportBuilder
    {
        HSSFWorkbook Build();
    }
}