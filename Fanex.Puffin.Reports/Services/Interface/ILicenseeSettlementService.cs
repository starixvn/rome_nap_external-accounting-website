using System.Collections.Generic;
using System.Web.Mvc;
using Fanex.Puffin.Core.Criteria;
using Fanex.Puffin.Report.Criteria.LicenseeSettlement;
using Fanex.Puffin.Report.ViewModels;
using Fanex.Puffin.Report.ViewModels.LicenseeSettlement;

namespace Fanex.Puffin.Report.Services.Interface
{
    public interface ILicenseeSettlementService
    {
        IEnumerable<SelectListItem> GetAllLicensees();
        IEnumerable<SelectListItem> GetProductsByLicensee(IMS_LicenseeProductSelCriteria criteria);
        IEnumerable<SelectListItem> GetCurrencyByLicensee(IMS_LicenseeCurrencySelCriteria criteria);
        IEnumerable<SelectListItem> GetBaseCurrency(IMS_BaseCurrencySel criteria);
        LicenseeInformation GetLicenseeInformation(IMS_LicenseeBillingInfoSelCriteria criteria);
        void ExportReportToExcel(string exchange, string product, bool isSaveFile = false);
        ViewModels.LicenseeSettlement.Report GetLicenseeReport(IMS_Rpt_LicenseeSettlementCriteria criteria);
        IEnumerable<ExchangeRate> GetLicenseeExchangeRate(IMS_ExchangeRateBillingSelCriteria criteria);
        LicenseeSettlementModel GenerateReport(IMS_Rpt_LicenseeSettlementCriteria criteria);
        string GetFileName();
    }
}