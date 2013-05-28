using System;
using System.Data;
using System.IO;
using System.Text;
using System.Threading;
using System.Web.Hosting;
using System.Web.Mvc;
using ExcelUtil.Spreadsheet;
using Fanex.Puffin.Website.Model.Test;
using Microsoft.Office.Interop.Excel;

namespace Fanex.Puffin.Website.Controllers
{
    public class TestController : Controller
    {
        //
        // GET: /Test/

        public ActionResult Index()
        {
            return View();
        }


        public ActionResult SaveExcel()
        {
          
            string html = RenderPartialViewToString("Employee", new IssueDTO());
            // if your view don't have any model then you can pass as null

            html = html.Replace("\n", "");
            html = html.Replace("\r", "");
            html = html.Replace("  ", "");

            

            //return  new DownloadFileActionResult(html, "test13413", DownloadFileActionResult.ExcelFormat.Xlsx);


            var excelExtension = new ExcelExtension(ExcelExtension.ExcelFormat.Xls);
            var ms = excelExtension.CreateExcel(html);
            if (ms != null)
            {
                // return the filestream
                // Rewind the memory stream to the beginning
                ms.Seek(0, SeekOrigin.Begin);

                return File(ms, @"application/vnd.openxmlformats-officedocument.spreadsheetml.sheet.main+xml", "tetsa.xls");
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
                ViewEngineResult viewResult = ViewEngines.Engines.FindPartialView(ControllerContext, viewName);
                ViewContext viewContext = new ViewContext(ControllerContext, viewResult.View, ViewData, TempData, sw);
                viewResult.View.Render(viewContext, sw);
                return sw.GetStringBuilder().ToString();


            }
        }


    }

    

    public class ExcelExtension
    {
        

        private readonly string _fileDirectory;

        private readonly string _xlsFileFormat;

        public enum ExcelFormat
        {
            Xls
        }

       
        public ExcelExtension(ExcelFormat format)
        {
            _fileDirectory = HostingEnvironment.MapPath("~/") + "App_Data/";

            var isExists = System.IO.Directory.Exists(_fileDirectory);

            if (!isExists)
                System.IO.Directory.CreateDirectory(_fileDirectory);

            _xlsFileFormat = "." + format.ToString().ToLower();
        }


        public MemoryStream CreateExcel(string htmlContent)
        {

            var excelFile = CreateExcelFromHtml(htmlContent);


            if (string.IsNullOrEmpty(excelFile)) return null;


            Thread.Sleep(100);

            //readsheetInfo.SetLicense("FREE-LIMITED-KEY");
            //var excel = ExcelFile.Load("D:/test5.xls");

           // var excel = ExcelFile.Load(excelFile);

            var ms = new MemoryStream();

            //excel.Save(ms, SaveOptions.XlsDefault);



            FileStream file = new FileStream(excelFile,FileMode.Open);
            byte[] bytes = new byte[file.Length];
            file.Read(bytes, 0, (int)file.Length);
            ms.Write(bytes, 0, (int)file.Length);
            file.Close();
            //ms.Close();


            Thread.Sleep(100);
            if (File.Exists(excelFile))
            {
                File.Delete(excelFile);
            }

           
            return ms;


        }




        private string CreateExcelFromHtml(string htmlContent)
        {

            var htmlFile = CreateHtmlFile(htmlContent);

            if (string.IsNullOrEmpty(htmlFile)) return string.Empty;

            var app = new Microsoft.Office.Interop.Excel.Application();
            // the reference to the workbook,
            // which is the xls document to read from.
            Workbook book = null;
            // the reference to the worksheet,
            // we'll assume the first sheet in the book.
            Worksheet sheet = null;
            Range range = null;
            // the range object is used to hold the data
            // we'll be reading from and to find the range of data.
            //var workbooks = new Workbooks();

            //Binding html file to workbook
            Thread.Sleep(100);
            Microsoft.Office.Interop.Excel.Workbook workBook = app.Workbooks.Open(htmlFile, 0, true, 5, "", "", true, Microsoft.Office.Interop.Excel.XlPlatform.xlWindows, "\t", false, false, 0, true, 1, 0);

            //save workbook to new real Excel file
            app.DisplayAlerts = false;
            var excelFile = _fileDirectory + Guid.NewGuid();
            workBook.SaveAs(excelFile, Microsoft.Office.Interop.Excel.XlFileFormat.xlWorkbookNormal, Type.Missing, Type.Missing,
            false, false, Microsoft.Office.Interop.Excel.XlSaveAsAccessMode.xlShared,
            Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing);


            workBook.Close();
            app.Workbooks.Close();
           

            //Delete temp html file
            Thread.Sleep(100);
            if (File.Exists(htmlFile))
            {
                File.Delete(htmlFile);
            }

            return excelFile + _xlsFileFormat;

        }
        private string CreateHtmlFile(string htmlContent)
        {
            //Open a memory stream that you can use to write back to the response
            byte[] byteArray = Encoding.ASCII.GetBytes(htmlContent);
            var stream = new MemoryStream(byteArray);
            
            if (stream.Length == 0) return string.Empty;


            var fileName = Guid.NewGuid() + _xlsFileFormat;


            using (FileStream fileStream = System.IO.File.Create(_fileDirectory + fileName, (int)stream.Length))
            {
                // Fill the bytes[] array with the stream data
                var bytesInStream = new byte[stream.Length];
                stream.Read(bytesInStream, 0, (int)bytesInStream.Length);

                // Use FileStream object to write to the specified file
                fileStream.Write(bytesInStream, 0, bytesInStream.Length);
            }
            return _fileDirectory + fileName;
        }


        
    }
}
