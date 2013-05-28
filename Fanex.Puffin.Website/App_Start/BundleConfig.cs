using System.Web.Optimization;

namespace Fanex.Puffin.Website.App_Start
{
    public class BundleConfig
    {
        // For more information on Bundling, visit http://go.microsoft.com/fwlink/?LinkId=254725
        public static void RegisterBundles(BundleCollection bundles)
        {
            #region Common Scripts

            bundles.Add(new ScriptBundle("~/scripts/common").Include(
                "~/Scripts/_Libs/jQuery/jquery-1.8.2.js",
                "~/Scripts/_Libs/jQuery/jquery-ui-1.9.1.js",
                "~/Scripts/_Libs/jQuery/jquery.jqprint-0.3.js",
                "~/Scripts/_Libs/jQuery/jquery.formattyping.js",
                "~/Scripts/_Libs/Bootstrap/bootstrap-alert.js",
                "~/Scripts/_Libs/Bootstrap/bootstrap-tooltip.js",
                "~/Scripts/_Libs/BlockUI/jquery.blockUI.1.33.js",
                "~/Scripts/_Libs/FanexGrid/Fanex.Grid.js",
                "~/Scripts/_Libs/FanexGrid/Fanex.Pagination.js",
                "~/Scripts/_Libs/FormatLibs/NumberFormat/numeral.js",
                "~/Scripts/NapTicketControl.js",
                "~/Scripts/NapTicketControl.Helper.js",
                "~/Scripts/NapTicketControl.Validator.js",
                "~/Scripts/_Libs/Validation/jquery.validationEngine-en.js",
                "~/Scripts/_Libs/Validation/jquery.validationEngine.js",
                "~/Scripts/_Libs/FanexGrid/Fanex.Pagination.js"
            ));

            #endregion

            #region NAP Recheckin
            //bundles.Add(new ScriptBundle("~/Areas/UserLogin/scripts").Include(
            //    "~/Areas/UserLogin/scripts/ReCheckIn.js"
            //    ));
            #endregion
            #region NAP msgbox
            bundles.Add(new ScriptBundle("~/Areas/UserLogin/scripts/lib/msgbox/JS").Include(
                "~/Scripts/_Libs/msgbox/msgbox-0.0.1.js"));

            bundles.Add(new StyleBundle("~/Areas/UserLogin/scripts/lib/msgbox/CSS").Include(
                 "~/Scripts/_Libs/msgbox/msgbox-0.0.1.css"));
            #endregion

            #region Multiselect Plugin
            bundles.Add(new StyleBundle("~/Scripts/_Libs/MultipleSelect/style").Include(
            "~/Scripts/_Libs/MultipleSelect/jquery.multiselect.css"
            ));
            bundles.Add(new ScriptBundle("~/Scripts/_Libs/multiselect").Include(
                      "~/Scripts/_Libs/MultipleSelect/jquery.multiselect.js"));

            #endregion

            #region Nex Date Picker
            bundles.Add(new StyleBundle("~/Scripts/_Libs/DatePicker/style").Include(
             "~/Scripts/_Libs/DatePicker/jquery.nexDatePicker.css"
             ));
            bundles.Add(new ScriptBundle("~/Scripts/_Libs/nexDatePicker").Include(
                      "~/Scripts/_Libs/DatePicker/jquery.nexDatePicker.js"));

            #endregion

            #region Client Template Render
            bundles.Add(new ScriptBundle("~/Scripts/_Libs/template").Include(
                     "~/Scripts/_Libs/TemplateRender/doT/doT.js"));

            #endregion

            #region Month Picker
            bundles.Add(new StyleBundle("~/Scripts/_Libs/MonthPicker/style").Include(
            "~/Scripts/_Libs/MonthPicker/monPickr-0.0.3.css"
            ));
            bundles.Add(new ScriptBundle("~/Scripts/_Libs/monPickr").Include(
                      "~/Scripts/_Libs/MonthPicker/monPickr-0.0.3.js"));
            #endregion

            #region Knockout
            bundles.Add(new ScriptBundle("~/scripts-bundle/knockout").Include(
                        "~/Scripts/_Libs/knockout*"));
            #endregion
        }
    }
}