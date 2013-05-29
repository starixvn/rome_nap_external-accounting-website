// --------------------------------------------------------------------------
// <copyright file="LicenseeSettlementController.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="" date="">Created</change>
// </history>


using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.Hosting;
using System.Web.Mvc;
using Fanex.Ares.Core.Extensions;
using Fanex.Ares.Core.Utils;
using Fanex.Ares.Core.Web.Controllers;
using Fanex.Ares.Report.Criteria;
using Fanex.Ares.Report.Criteria.LicenseeSettlement;
using Fanex.Ares.Report.Services.Interface;
using Fanex.Ares.Report.ViewModels.LicenseeSettlement;
using Fanex.Common.Extensions;
using Fanex.Core.PdfHelper;
using Fanex.Ares.Core.Criteria;

namespace Fanex.Ares.Report.Controllers
{
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented")]
    public class LicenseeSettlementController : BaseController
    {
        ILicenseeSettlementService _service;
        private readonly HtmlViewRenderer htmlViewRenderer;
        private readonly StandardPdfRenderer standardPdfRenderer;

        private static readonly string PdfCssFilePath = HostingEnvironment.MapPath(@"~\Areas\Report\Views\LicenseeSettlement\Styles\LicenseeSettlementPDF.css");

        

        #region View path

        protected string SettlementView = string.Empty;

        #endregion

        public LicenseeSettlementController()
        {
            _service = new LicenseeSettlementService();
            this.htmlViewRenderer = new HtmlViewRenderer();
            this.standardPdfRenderer = new StandardPdfRenderer();
        }

        public ActionResult Index()
        {
            IMS_Rpt_LicenseeSettlementCriteria model = new IMS_Rpt_LicenseeSettlementCriteria();
            model.Year = DateTime.Now.Year;
            ViewBag.ListLicensee = _service.GetAllLicensees();
            var licenseeID = int.Parse(((List<SelectListItem>)ViewBag.ListLicensee)[0].Value);
            ViewBag.ListProduct = _service.GetProductsByLicensee(new IMS_LicenseeProductSelCriteria() { LicenseeId = licenseeID });
            var listCurrency = _service.GetCurrencyByLicensee(new IMS_LicenseeCurrencySelCriteria() { LicenseeId = licenseeID }).ToList<SelectListItem>();
            listCurrency.Add(new SelectListItem() { Selected = true, Text = "All", Value = "0" });
            ViewBag.ListCurrency = listCurrency;
            ViewBag.ListExchangeRate = _service.GetBaseCurrency(new IMS_BaseCurrencySel());

            return View(model);
        }

        public PartialViewResult GetReport(IMS_Rpt_LicenseeSettlementCriteria criteria)
        {
            var model = _service.GenerateReport(criteria);
            model.LicenseeInfor.FromDate = criteria.FromDate;
            model.LicenseeInfor.ToDate = criteria.ToDate;
            SettlementView = "_" + criteria.ProductName;
            SessionExtensions.AddSession(System.Web.HttpContext.Current.Session, CacheServiceKey.SessionKeyLicenseeSettlementMonth, criteria.Month);
            SessionExtensions.AddSession(System.Web.HttpContext.Current.Session, CacheServiceKey.SessionKeyLicenseeSettlementYear, criteria.Year);
            SessionExtensions.AddSession(System.Web.HttpContext.Current.Session, CacheServiceKey.SessionKeyLicenseeSettlementViewName, SettlementView);
            return PartialView(SettlementView, model);
        }

        protected ActionResult ViewPdf(string pageTitle, string viewName, object model, int numericRows, string fileName, bool isSavePDF)
        {
            // Render the view html to a string.
            string htmlText = this.htmlViewRenderer.RenderViewToString(this, viewName, model);

            // Let the html be rendered into a PDF document through iTextSharp.

            byte[] buffer = standardPdfRenderer.Render(htmlText, pageTitle, numericRows, PdfCssFilePath);

            // Return the PDF as a binary stream to the client.
            //return new BinaryContentResult(buffer, "application/pdf");
            if (isSavePDF)
            {
                Response.Clear();
                MemoryStream ms = new MemoryStream(buffer);
                Response.ContentType = "application/pdf";
                Response.AddHeader("content-disposition", "attachment;filename= " + fileName + ".pdf");
                Response.Buffer = true;
                ms.WriteTo(Response.OutputStream);
                Response.End();

                // Return the PDF as a binary stream to the client.
                return new EmptyResult();
            }
            else
            {
                return new BinaryContentResult(buffer, "application/pdf");
            }
        }

        public ActionResult ExportPDF(bool isSavePDF)
        {
            //var model = GenerateReport(criteria);
            var model = new LicenseeSettlementModel();
            SettlementView = (string)SessionExtensions.GetSession(System.Web.HttpContext.Current.Session, CacheServiceKey.SessionKeyLicenseeSettlementViewName);
            model = (LicenseeSettlementModel)SessionExtensions.GetSession(System.Web.HttpContext.Current.Session, CacheServiceKey.SessionKeyLicenseeSettlementReport);
            int month = (int)SessionExtensions.GetSession(System.Web.HttpContext.Current.Session, CacheServiceKey.SessionKeyLicenseeSettlementMonth);
            int year = (int)SessionExtensions.GetSession(System.Web.HttpContext.Current.Session, CacheServiceKey.SessionKeyLicenseeSettlementYear);
            string fileName = model.LicenseeInfor.LicenseeName + "_" + model.LicenseeInfor.ProductName + "_" + "Settlement_For_" + month + "-" + year;
            //FillImageUrl(customerList, "report.jpg");
            return this.ViewPdf("", SettlementView, model, model.Report.EntityList.Count + 1, fileName, isSavePDF);
        }

        public ActionResult ExportSettlementReportToExcel(string exchange, string product)
        {
            _service.ExportReportToExcel(exchange, product);
            return new EmptyResult();
        }




        //testing
        public ActionResult ExportDocument(WorkBookExtension.DocumentType documentType )
        {
            var model = new LicenseeSettlementModel();


            

            model = (LicenseeSettlementModel)SessionExtensions.GetSession(System.Web.HttpContext.Current.Session, CacheServiceKey.SessionKeyLicenseeSettlementReport);
            SettlementView = (string)SessionExtensions.GetSession(System.Web.HttpContext.Current.Session, CacheServiceKey.SessionKeyLicenseeSettlementViewName);


            

            string html = this.htmlViewRenderer.RenderViewToString(this, SettlementView, model);


            
            var excelCssFilePath = new List<string>();
            excelCssFilePath.Add(HostingEnvironment.MapPath(@"~\Areas\Report\Views\LicenseeSettlement\Styles\LicenseeSettlement.css"));
           
            var excelExtension = new WorkBookExtension();
            var ms = excelExtension.CreateDocument(html, excelCssFilePath, documentType, WorkBookExtension.PageOrientation.Landscape);
            if (ms != null)
            {
                // return the filestream
                // Rewind the memory stream to the beginning
                ms.Seek(0, SeekOrigin.Begin);

                int month = (int)SessionExtensions.GetSession(System.Web.HttpContext.Current.Session, CacheServiceKey.SessionKeyLicenseeSettlementMonth);
                int year = (int)SessionExtensions.GetSession(System.Web.HttpContext.Current.Session, CacheServiceKey.SessionKeyLicenseeSettlementYear);
                string fileName = model.LicenseeInfor.LicenseeName + "_" + model.LicenseeInfor.ProductName + "_" + "Settlement_For_" + month + "-" + year + "." + documentType.ToString().ToLower();

                return File(ms, @"application/vnd.openxmlformats-officedocument.spreadsheetml.sheet.main+xml", fileName);
            }
            return new EmptyResult();

            
        }

        protected string RenderPartialViewToString(string viewName, object model)
        {
            if (string.IsNullOrEmpty(viewName))
                viewName = ControllerContext.RouteData.GetRequiredString("action");

            ViewData.Model = model;

            using (StringWriter sw = new StringWriter())
            {
                
                ViewEngineResult viewResult = ViewEngines.Engines.FindPartialView(ControllerContext, viewName);// ViewEngines.Engines.FindPartialView(ControllerContext, viewName);
                ViewContext viewContext = new ViewContext(ControllerContext, viewResult.View, ViewData, TempData, sw);
                viewResult.View.Render(viewContext, sw);
                return sw.GetStringBuilder().ToString();


            }
        }

        public JsonResult GetProductByLicenseeID(int licenseeID)
        {
            return Json(_service.GetProductsByLicensee(new IMS_LicenseeProductSelCriteria() { LicenseeId = licenseeID }));
        }

        public JsonResult GetCurencyByLicenseeID(int licenseeID)
        {
            return Json(_service.GetCurrencyByLicensee(new IMS_LicenseeCurrencySelCriteria() { LicenseeId = licenseeID }));
        }

        public ActionResult PrintScreen()
        {
            var model = (LicenseeSettlementModel)SessionExtensions.GetSession(System.Web.HttpContext.Current.Session, CacheServiceKey.SessionKeyLicenseeSettlementReport);
            return View("PrintReport", model);
        }
    }
}