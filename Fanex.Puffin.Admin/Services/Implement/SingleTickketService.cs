// -----------------------------------------------------------------------
// <copyright file="SingleTickketService.cs" company="Microsoft">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------
// <history>
//   <change who="Robert.Tran" date="2013.2.05">Created</change>
// </history>

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Fanex.Puffin.Admin.Criteria;
using Fanex.Puffin.Admin.Models.SingleTicket;
using Fanex.Puffin.Admin.Services.Interface;
using Fanex.Puffin.Core.Services.Impl;

namespace Fanex.Puffin.Admin.Services.Implement
{
    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented")]
    public class SingleTickketService : ServiceBase, ISingleTickketService
    {
        public List<Bettype> SelBettype(GetBettypeCriteria criteria)
        {
            List<Bettype> bettypes = new List<Bettype>();
            try
            {
                var rawData = base.GetListData<dynamic>(criteria);
                foreach (var row in rawData)
                {
                    bettypes.Add(new Bettype() { 
                        Id = Convert.ToInt32(row.typeid),
                        Name = Convert.ToString(row.bettypename),
                        LiveIndicator = Convert.ToBoolean(row.liveindicator)
                    });
                }
            }
            catch (Exception ex)
            {
                base.HandleException(ex);
            }
            return bettypes;
        }
    }
}
