// --------------------------------------------------------------------------
// <copyright file="DeleteSingleBetController.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Ricky.Nguyen" date="2013.1.25">Created</change>
// </history>


using System.Web.Mvc;
using System.Diagnostics.CodeAnalysis;
using Fanex.NAP.Core.Web.Attribute;
using Fanex.NAP.Core.Business;
using Fanex.Puffin.Admin.Criteria;
using Fanex.Puffin.Admin.Services.Implement;
using Fanex.Puffin.Admin.Services.Interface;
using Fanex.Puffin.Core.Web;

namespace Fanex.Puffin.Admin.Controllers
{
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented")]
    [NapSuperAuth]
    public class DeleteSingleTicketController : Controller
    {
        ISingleTickketService _service;

        public DeleteSingleTicketController()
        {
            _service = new SingleTickketService();
        }

        [HttpGet]
        [NapAuth(FunctionKey = NAPSpecialDeleteFunctionKeys.SpecialDelete, Policy = Constant.Policy.Read)]
        public ActionResult Index()
        {
            return View();
        }
               

        [HttpPost]
        public JsonResult SelBettype(GetBettypeCriteria criteria)
        {
            var bettypes = _service.SelBettype(criteria);
            return base.Json(bettypes, JsonRequestBehavior.AllowGet); 
        }
    }
}