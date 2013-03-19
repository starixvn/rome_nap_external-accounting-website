// --------------------------------------------------------------------------
// <copyright file="LiveCasinoController.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Andy.Nguyen" date="2013.3.19">Created</change>
// </history>

namespace Fanex.ExternalAccount.Controllers.Customer
{
    using System.Web.Mvc;
    using Fanex.ExternalAccount.Controllers.Base;

    /// <summary>
    /// Controller for processing live casino setting
    /// </summary>
    public class LiveCasinoController : BaseController
    {
        public ActionResult EditMaster()
        {
            return this.PartialViewBase();
        }

        public ActionResult EditAgent()
        {
            return this.PartialViewBase();
        }

        public ActionResult EditMember()
        {
            return this.PartialViewBase();
        }

        public ActionResult SuperEditMember()
        {
            return this.PartialViewBase();
        }

        public ActionResult MasterEditMember()
        {
            return this.PartialViewBase();
        }

        public ActionResult EditMultipleMasters()
        {
            return this.PartialViewBase();
        }

        public ActionResult EditMultipleAgents()
        {
            return this.PartialViewBase();
        }

        public ActionResult EditMultipleMembers()
        {
            return this.PartialViewBase();
        }

        public ActionResult SuperEditMultipleMembers()
        {
            return this.PartialViewBase();
        }

        public ActionResult MasterEditMultipleMembers()
        {
            return this.PartialViewBase();
        }
    }
}