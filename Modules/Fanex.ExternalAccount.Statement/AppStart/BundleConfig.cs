namespace Fanex.ExternalAccount.Statement.AppStart
{
    using System.Web.Optimization;

    public class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            RegisterFanexGridBundles(bundles);
        }

        public static void RegisterFanexGridBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/Scripts/_Libs/FanexGrid/FanexGridScripts").Include(
                "~/Scripts/_Libs/FanexGrid-MultiLang/Fanex.Grid.js",
                "~/Scripts/_Libs/FanexGrid-MultiLang/Language/Fanex.Gird.Lang.en-US.js",
                "~/Scripts/_Libs/FanexGrid-MultiLang/Language/Fanex.Gird.Lang.ja-JP.js",
                "~/Scripts/_Libs/FanexGrid-MultiLang/Language/Fanex.Gird.Lang.ko-KR.js",
                "~/Scripts/_Libs/FanexGrid-MultiLang/Language/Fanex.Gird.Lang.th-TH.js",
                "~/Scripts/_Libs/FanexGrid-MultiLang/Language/Fanex.Gird.Lang.vi-VN.js",
                "~/Scripts/_Libs/FanexGrid-MultiLang/Language/Fanex.Gird.Lang.zh-CN.js",
                "~/Scripts/_Libs/FanexGrid-MultiLang/Language/Fanex.Gird.Lang.zh-TW.js",
                "~/Scripts/_Libs/FanexGrid/Fanex.Pagination.js",
                "~/Scripts/_Libs/FanexGrid/moment.js",
                "~/Scripts/_Libs/FanexGrid/numeral.js"));

            bundles.Add(new StyleBundle("~/Scripts/_Libs/FanexGrid/FanexGridStyles").Include(
                "~/Scripts/_Libs/FanexGrid/Fanex.Grid.css"));
        }
    }
}
