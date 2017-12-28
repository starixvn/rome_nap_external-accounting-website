namespace Fanex.ExternalAccount.Service.Interface.Report
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Data;
    using DTO.Customer;
    using DTO.Report;

    public interface WinLossDetailService
    {
        dynamic DrillDownWinLossDetail(IDrillDownRequest request);
    }
}
