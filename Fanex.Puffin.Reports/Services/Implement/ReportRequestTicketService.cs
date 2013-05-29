// --------------------------------------------------------------------------
// <copyright file="ReportRequestTicketService.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Louis.Nguyen" date="2012.12.07">Created</change>
// </history>

using Fanex.Puffin.Core.Extensions;
using Fanex.Puffin.Core.Services.Impl;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Fanex.Common;
using Fanex.Puffin.Report.Criteria.ReportRequestTicket;
using Fanex.Puffin.Report.Entities;
using Fanex.Puffin.Report.Entities.ReportRequestTicket;
using Fanex.Puffin.Report.Repositories.Impl;
using Fanex.Puffin.Report.Repositories.Interface;
using Fanex.Puffin.Report.Services.Interface;
using Resource;

namespace Fanex.Puffin.Report.Services.Implement
{
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented")]
    public class ReportRequestTicketService : ServiceBase, IReportRequestTicketService
    {
        private IReportRequestTicketRepository _reportRepository;

        public ReportRequestTicketService() :
            this(null)
        {
        }

        public ReportRequestTicketService(IReportRequestTicketRepository reportRepository)
        {
            this._reportRepository = reportRepository ?? new ReportRequestTicketRepository();
        }

        public RequestTicketInfo GetRequestTicketReport(GetRequestTicketReportCriteria criteria)
        {
            try
            {
                var rawData = base.GetListData<dynamic>(criteria);
                var report = new RequestTicketInfo
                {
                    ChildInfo = new List<RequestTicketInfo>()
                };

                foreach (var data in rawData)
                {
                    DateTime reqDate = Converter.ToDateTime(data.ReqDate);
                    DateTime reqDateline = Converter.ToDateTime(data.Dateline);
                    var info = new RequestTicketInfo
                    {
                        RequestNo = Converter.ToString2(data.ReqNo),
                        Requestor = Converter.ToString2(data.Requestor),
                        DateOfRequest = reqDate.ToShortFormatedDateWithSlash(),
                        ReportName = Converter.ToString2(data.ReportName),
                        Remark = Converter.ToString2(data.Remark),
                        Dateline = !string.IsNullOrEmpty(Converter.ToString2(data.Dateline)) ?
                            reqDateline.ToShortFormatedDateWithSlash() :
                            string.Empty,
                        Assignee = Converter.ToString2(data.Assignee),
                        StatusOfReport = Converter.ToString2(data.Status),
                        RemarkByPreparer = Converter.ToString2(data.RemarkByPreparer),
                        LastModifier = Converter.ToString2(data.LastModifiedBy)
                    };

                    report.ChildInfo.Add(info);
                }

                return report;
            }
            catch (Exception ex)
            {
                base.HandleException(ex);
            }

            return null;
        }

        public string GetMaxRequestNo(GetMaxRequestNoCriteria criteria)
        {
            try
            {
                Hashtable outputParams = new Hashtable();
                this._reportRepository.GetMaxRequestNo(criteria, ref outputParams);
                return string.Format("R{0}", Converter.ToString2(outputParams["RequestNo"]));
            }
            catch (Exception ex)
            {
                base.HandleException(ex);
            }

            return "R000102";
        }

        public Result AddNewRequestTicket(AddNewRequestTicketCriteria criteria)
        {
            try
            {
                Hashtable outputParams = new Hashtable();
                this._reportRepository.AddNewRequestTicket(criteria, ref outputParams);
                return new Result();

                //return new Result
                //{
                //    ErrorCode = Converter.ToInt(outputParams["ErrorCode"]),
                //    ErrorMessage = Converter.ToString2(outputParams["ErrorMessage"])
                //};
            }
            catch (Exception ex)
            {
                base.HandleException(ex);
            }

            return new Result
            {
                ErrorCode = 1,
                ErrorMessage = Labels.Failed
            };
        }

        public Result DeleteRequestTicket(DeleteRequestTicketCriteria criteria)
        {
            try
            {
                Hashtable outputParams = new Hashtable();
                this._reportRepository.DeleteRequestTicket(criteria, ref outputParams);
                return new Result();

                //return new Result
                //{
                //    ErrorCode = Converter.ToInt(outputParams["ErrorCode"]),
                //    ErrorMessage = Converter.ToString2(outputParams["ErrorMessage"])
                //};
            }
            catch (Exception ex)
            {
                base.HandleException(ex);
            }

            return new Result
            {
                ErrorCode = 1,
                ErrorMessage = Labels.Failed
            };
        }

        public Result UpdateRequestTicket(UpdateRequestTicketCriteria criteria)
        {
            try
            {
                Hashtable outputParams = new Hashtable();
                this._reportRepository.UpdateRequestTicket(criteria, ref outputParams);
                return new Result();
                //return new Result
                //{
                //    ErrorCode = Converter.ToInt(outputParams["ErrorCode"]),
                //    ErrorMessage = Converter.ToString2(outputParams["ErrorMessage"])
                //};
            }
            catch (Exception ex)
            {
                base.HandleException(ex);
            }

            return new Result
            {
                ErrorCode = 1,
                ErrorMessage = Labels.Failed
            };
        }
    }
}
