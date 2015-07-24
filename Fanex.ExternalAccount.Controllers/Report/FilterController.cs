// --------------------------------------------------------------------------
// <copyright file="FilterController.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// <summary>
// For current session info
// </summary>
// --------------------------------------------------------------------------
// <history>
//   <change who="Andy.Nguyen" date="2012.12.14">Created</change>
// </history>
namespace Fanex.ExternalAccount.Controllers.Report
{
    using System.Collections.Generic;
    using System.Web.Mvc;
    using Fanex.ExternalAccount.Core.Base;
    using Fanex.ExternalAccount.Service.Impl.Report;
    using Fanex.ExternalAccount.Service.Interface.Report;
    using Fanex.ExternalAccount.Utilities;

    /// <summary>
    /// Report controller.
    /// </summary>
    public partial class FilterController : BaseController
    {
        public const string SHOWALL = "all";
        public const string SHOWSB = "showsb";
        public const string SHOWCASINO = "showcasino";
        public const string SHOWRB = "showrb";
        public const string ShowIbcBingo = "showbi";
        public const string ShowNumberGame = "showng";
        public const string ShowP2P = "showp2p";
        public const string ShowLiveCasino = "showlivecasino";
        public const string ShowVirtualSport = "showvs";
        public const string ShowKeno = "showkeno";
        public const string ShowGoldDeluxe = "showgd";

        /// <summary>
        /// Gets list of request from filter condition.
        /// </summary>
        /// <returns>ICollection object.</returns>
        public ICollection<KeyValuePair<string, bool>> AsCollection()
        {
            List<KeyValuePair<string, bool>> list = new List<KeyValuePair<string, bool>>();

            list.Add(new KeyValuePair<string, bool>(SHOWSB, CheckRequest.GetBooleanRequest(SHOWSB, true)));
            list.Add(new KeyValuePair<string, bool>(SHOWCASINO, CheckRequest.GetBooleanRequest(SHOWCASINO, true)));
            list.Add(new KeyValuePair<string, bool>(SHOWRB, CheckRequest.GetBooleanRequest(SHOWRB, true)));
            list.Add(new KeyValuePair<string, bool>(ShowIbcBingo, CheckRequest.GetBooleanRequest(ShowIbcBingo, true)));
            list.Add(new KeyValuePair<string, bool>(ShowNumberGame, CheckRequest.GetBooleanRequest(ShowNumberGame, true)));
            list.Add(new KeyValuePair<string, bool>(ShowP2P, CheckRequest.GetBooleanRequest(ShowP2P, true)));
            list.Add(new KeyValuePair<string, bool>(ShowLiveCasino, CheckRequest.GetBooleanRequest(ShowLiveCasino, true)));
            list.Add(new KeyValuePair<string, bool>(ShowVirtualSport, CheckRequest.GetBooleanRequest(ShowVirtualSport, true)));
            list.Add(new KeyValuePair<string, bool>(ShowKeno, CheckRequest.GetBooleanRequest(ShowKeno, true)));
            list.Add(new KeyValuePair<string, bool>(ShowGoldDeluxe, CheckRequest.GetBooleanRequest(ShowGoldDeluxe, true)));

            return list;
        }

        /// <summary>
        /// Gets product list.
        /// </summary>
        /// <returns>ActionResult object.</returns>
        public ActionResult ProductsList()
        {
            List<KeyValuePair<string, string>> list = new List<KeyValuePair<string, string>>();

            list.Add(new KeyValuePair<string, string>("showAll", ViewLang.All));
            list.Add(new KeyValuePair<string, string>("showSB", ViewLang.Sportsbook));
            list.Add(new KeyValuePair<string, string>("showCasino", ViewLang.Casino));
            list.Add(new KeyValuePair<string, string>("showRB", ViewLang.Racing));
            list.Add(new KeyValuePair<string, string>("showBI", ViewLang.Bingo));
            list.Add(new KeyValuePair<string, string>("showNG", ViewLang.NumberGame));
            list.Add(new KeyValuePair<string, string>("showP2P", ViewLang.Poker));
            list.Add(new KeyValuePair<string, string>("showLiveCasino", ViewLang.LiveCasino));
            if (Config.IsShowVirtualSports)
            {
                list.Add(new KeyValuePair<string, string>("showVS", ViewLang.VirtualSports));
            }

            list.Add(new KeyValuePair<string, string>("showKeno", ViewLang.Keno));

            if (!Config.HideGoldDeluxe)
            {
                list.Add(new KeyValuePair<string, string>("showGD", ViewLang.GoldDeluxe));
            }

            return this.JsonBase(list, new Newtonsoft.Json.JsonSerializerSettings());
        }

        /// <summary>
        /// Gets suggested usernames.
        /// </summary>
        /// <returns>ActionResult object.</returns>
        public ActionResult GetSuggestedUserName()
        {
            string groupName = CheckRequest.GetStringRequest("groupname", string.Empty);

            if (string.IsNullOrEmpty(groupName) || !this.IsBelong(groupName, 0, null, true))
            {
                return this.JsonBase(new { username = string.Empty });
            }

            IReportService reportService = new ReportService();
            var users = reportService.GetSuggestedUserName(
                CurrentSession.CustId, 
                CheckRequest.GetStringRequest("username", string.Empty), 
                CheckRequest.GetStringRequest("groupname", string.Empty));
            
            return this.JsonBase(users, new Newtonsoft.Json.JsonSerializerSettings());
        }
    }
}