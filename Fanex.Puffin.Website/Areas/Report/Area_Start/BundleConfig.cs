using System.Web.Optimization;

namespace Fanex.Puffin.Website.Areas.Report.Area_Start
{
    public class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/areas/Report/DeleteSingleTicket").Include(
                "~/Scripts/_Libs/TimePicker/jquery-ui-timepicker-addon.js",
                "~/Scripts/_Libs/Utils/stdlib.js",
                "~/Scripts/Common/Message.Helper.js",
                "~/Scripts/Common/Message.Validator.js",
                "~/Areas/SportBook/Scripts/VoidSingleTicket/VoidSingleTicket.js",
                "~/Areas/SportBook/Scripts/VoidSingleTicket/VoidSingleTicketHelper.js",
                "~/Areas/SportBook/Scripts/VoidSingleTicket/VoidSingleTicketValidateData.js",
                "~/Areas/SportBook/Scripts/VoidSingleTicket/VoidSingleTicketManager.js"
                
            ));


            bundles.Add(new ScriptBundle("~/areas/Report/VoidMixParlayTicket").Include(
                "~/Scripts/_Libs/TimePicker/jquery-ui-timepicker-addon.js",
                "~/Scripts/_Libs/Utils/stdlib.js",
                "~/Scripts/Common/Message.Helper.js",
                "~/Scripts/Common/Message.Validator.js",
                "~/Areas/SportBook/Scripts/VoidMixParlayTicket/VoidMixParlayTicketHelper.js",
                "~/Areas/SportBook/Scripts/VoidMixParlayTicket/VoidMixParlayTicketValidateData.js",
                "~/Areas/SportBook/Scripts/VoidMixParlayTicket/VoidMixParlayTicket.js",
                "~/Areas/SportBook/Scripts/VoidMixParlayTicket/SubTicketMixParlay.js",
                "~/Areas/SportBook/Scripts/VoidMixParlayTicket/VoidMixParlayTicketManager.js"
            ));

            bundles.Add(new ScriptBundle("~/areas/Report/RecoverTicket").Include(                
                "~/Scripts/_Libs/Utils/stdlib.js",
                "~/Scripts/Common/Message.Helper.js",
                "~/Scripts/Common/Message.Validator.js",
                "~/Areas/SportBook/Scripts/RecoverTicket/RecoverTicket.js"
            ));

            bundles.Add(new ScriptBundle("~/areas/Report/VoidOutrightTicket").Include(
                "~/Scripts/_Libs/Utils/stdlib.js",
                "~/Scripts/Common/Message.Helper.js",
                "~/Scripts/Common/Message.Validator.js",
                "~/Areas/SportBook/Scripts/VoidOutrightTicket/VoidOutrightTicket.js",
                "~/Areas/SportBook/Scripts/VoidOutrightTicket/VoidOutrightTicketValidateData.js"
            ));
        }
    }
}