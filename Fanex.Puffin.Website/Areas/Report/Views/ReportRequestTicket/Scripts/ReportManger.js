$(document).ready(function () {
    reportManager.InitPage();
});

ReportManager = function () {
    var self = this;
    self.Enums = {
        ReportStatus: {
            Pending: { Id: 1, Name: "Pending" },
            In_Progress: { Id: 2, Name: "In Progress" },
            Completed: { Id: 3, Name: "Completed" },
            All: { Id: 0, Name: "All" }
        },
        SortDirection: {
            Desc: "desc",
            Asc: "asc",
            None: ""
        },

        SortDataType: {
            Number: "number",
            String: "string",
            DateTime: "dateTime"
        },

        FieldToSort: {
            RequestNo: "RequestNo",
            Requestor: "Requestor",
            DateOfRequest: "DateOfRequest",
            ReportName: "ReportName",
            Dateline: "Dateline",
            Assignee: "Assignee",
            StatusOfReport: "StatusOfReport",
            LastModifier: "LastModifier"
        }
    };

    self.Tables = {
        btnGenerate: null,
        report: null,
        report_Body: null,

        freezeGridReportContainer: null,
        freezeGridReport: null,

        addNewRequestTicketPopupContainer: null,
        addNewRequestTicketPopup: null,
        ddlStatusOfReport: null,
        txtRequestNo: null,
        txtRequestor: null,
        txtDateOfRequest: null,
        txtReportName: null,
        txtRemark: null,
        txtDateline: null,
        txtAssignee: null,
        ddlStatusOfReport_AddNew: null,
        txtRemarkByPreparer: null,
        btnSubmit: null,
        resultMessage_AddNew: null,

        editRequestTicketPopupContainer: null,
        editRequestTicketPopup: null,
        ddlStatusOfReport_Edit: null,
        txtRequestNo_Edit: null,
        txtRequestor_Edit: null,
        txtDateOfRequest_Edit: null,
        txtReportName_Edit: null,
        txtRemark_Edit: null,
        txtDateline_Edit: null,
        txtAssignee_Edit: null,
        ddlStatusOfReport_Edit: null,
        txtRemarkByPreparer_Edit: null,
        btnSubmit_Edit: null,
        resultMessage_Edit: null,
        CheckDate_Edit: null
    };

    self.Params = {
        reportParams: {
            Status: null,
            StatusName: null
        },

        addNewRequestTicketParams: {
            RequestNo: null,
            Requestor: null,
            ReqDate: null,
            ReportName: null,
            Remark: null,
            Dateline: null,
            Assignee: null,
            Status: null,
            RemarkByPreparer: null
        },

        getMaxRequestNo: {
        },
        editRequestTicketParams: {
            RequestNo: null,
            Requestor: null,
            DateOfRequest: null,
            ReportName: null,
            Remark: null,
            Dateline: null,
            Assignee: null,
            StatusOfReport: null,
            RemarkByPreparer: null
        },
        deleteRequestTicketParams: {
            ReqNo: null,
            Requestor: null,
            DateOfRequest: null,
            ReportName: null,
            Remark: null,
            Dateline: null,
            Assignee: null,
            StatusOfReport: null,
            RemarkByPreparer: null,
            LastModifiedBy: null
        }
    };

    self.CacheData = {
        addNewRequestTicketPopupHtml: '',
        editRequestTicketPopupHtml: '',

        isSelectDateline_AddNew: false,
        isSelectDateline_Edit: false,
        maxRequestNo: 0,
        currentDialog: null,
        maxRemarkDisplayLength: 21,
        maxRemarkDisplayLength_FreezeGrid: 19,
        requestTicketReport: null,
        sorterList: null,
        currentSorter: null,

        sortDataType: null,
        sortField: null,
        sortFactor: null,

        hasHostPermission: false
    };
};
ReportManager.prototype.InitPage = function () {
    this.CachedDOMElements();
    this.BindingEvent();

    this.CacheData.addNewRequestTicketPopupHtml = this.Tables.addNewRequestTicketPopupContainer.html();
    this.CacheData.editRequestTicketPopupHtml = this.Tables.editRequestTicketPopupContainer.html();
    this.RenderDatelineCalendar();
    this.RenderDatelineCalendar_Edit();
    this.CacheData.hasHostPermission = hasHostPermission == "true";
    //    this.CacheData.hasHostPermission = false;
    if (!this.CacheData.hasHostPermission) {
        this.Tables.btnAddNew.attr("disabled", "disabled");
    }

    this.RenderDropdownReportStatus();
    reportManager.btnReportClick();
};
ReportManager.prototype.CachedDOMElements = function () {
    var self = this;
    self.Tables.btnAddNew = $("#btnAddNew");
    self.Tables.ddlStatusOfReport = $("#ddlStatusOfReport");
    self.Tables.btnGenerate = $("#btnGenerate");

    self.Tables.report = $('#reportRequestTicketReport');
    self.Tables.report_Body = self.Tables.report.find('tbody');
    self.Tables.freezeGridReportContainer = $('#freezeGrid-reportRequestTicketReportContainer');
    self.Tables.freezeGridReport = self.Tables.freezeGridReportContainer.find('#freezeGrid-reportRequestTicketReport');

    self.Tables.addNewRequestTicketPopupContainer = $("#addNewRequestTicketPopupContainer");
    self.Tables.addNewRequestTicketPopup = self.Tables.addNewRequestTicketPopupContainer.find("#addNewRequestTicketPopup");
    self.Tables.txtRequestNo = self.Tables.addNewRequestTicketPopup.find("#txtRequestNo");
    self.Tables.txtRequestor = self.Tables.addNewRequestTicketPopup.find("#txtRequestor");
    self.Tables.txtDateOfRequest = self.Tables.addNewRequestTicketPopup.find("#txtDateOfRequest");
    self.Tables.txtReportName = self.Tables.addNewRequestTicketPopup.find("#txtReportName");
    self.Tables.txtRemark = self.Tables.addNewRequestTicketPopup.find("#txtRemark");
    self.Tables.txtDateline = self.Tables.addNewRequestTicketPopup.find("#txtDateline");
    self.Tables.txtAssignee = self.Tables.addNewRequestTicketPopup.find("#txtAssignee");
    self.Tables.ddlStatusOfReport_AddNew = self.Tables.addNewRequestTicketPopup.find("#ddlStatusOfReport_AddNew");
    self.Tables.txtRemarkByPreparer = self.Tables.addNewRequestTicketPopup.find("#txtRemarkByPreparer");
    self.Tables.btnSubmit = self.Tables.addNewRequestTicketPopup.find("#btnSubmit");
    self.Tables.resultMessage_AddNew = self.Tables.addNewRequestTicketPopup.find("#resultMessage_AddNew");

    self.Tables.editRequestTicketPopupContainer = $("#editRequestTicketPopupContainer");
    self.Tables.editRequestTicketPopup = self.Tables.editRequestTicketPopupContainer.find("#editRequestTicketPopup");
    self.Tables.txtRequestNo_Edit = self.Tables.editRequestTicketPopup.find("#txtRequestNo_Edit");
    self.Tables.txtRequestor_Edit = self.Tables.editRequestTicketPopup.find("#txtRequestor_Edit");
    self.Tables.txtDateOfRequest_Edit = self.Tables.editRequestTicketPopup.find("#txtDateOfRequest_Edit");
    self.Tables.txtReportName_Edit = self.Tables.editRequestTicketPopup.find("#txtReportName_Edit");
    self.Tables.txtRemark_Edit = self.Tables.editRequestTicketPopup.find("#txtRemark_Edit");
    self.Tables.txtDateline_Edit = self.Tables.editRequestTicketPopup.find("#txtDateline_Edit");
    self.Tables.txtAssignee_Edit = self.Tables.editRequestTicketPopup.find("#txtAssignee_Edit");
    self.Tables.ddlStatusOfReport_Edit = self.Tables.editRequestTicketPopup.find("#ddlStatusOfReport_Edit");
    self.Tables.txtRemarkByPreparer_Edit = self.Tables.editRequestTicketPopup.find("#txtRemarkByPreparer_Edit");
    self.Tables.btnSubmit_Edit = self.Tables.editRequestTicketPopup.find("#btnSubmit_Edit");
    self.Tables.resultMessage_Edit = self.Tables.editRequestTicketPopup.find("#resultMessage_Edit");
};
ReportManager.prototype.BindingEvent = function () {
    var self = this;

    self.Tables.btnGenerate.unbind("click");
    self.Tables.btnGenerate.click(function () {
        reportManager.btnReportClick();
    });

    self.Tables.btnAddNew.unbind("click");
    self.Tables.btnAddNew.click(function () {
        reportManager.btnAddNewClick();
    });

    self.Tables.btnSubmit.unbind("click");
    self.Tables.btnSubmit.click(function () {
        reportManager.btnSubmitClick();
    });

    self.Tables.btnSubmit_Edit.unbind("click");
    self.Tables.btnSubmit_Edit.click(function () {
        reportManager.btnSubmit_EditClick();
    });
};
ReportManager.prototype.btnReportClick = function () {
    var self = this;
    if (self.CacheData.currentDialog) {
        self.CacheData.currentDialog.dialog("close");
    }

    NapTicketControl.popUp.removeHighLight();
    self.Params.reportParams.StatusName = self.Tables.ddlStatusOfReport.find("option:selected").text();
    self.Params.reportParams.Status = parseInt(self.Tables.ddlStatusOfReport.find("option:selected").val());
    self.GetReport();
};
ReportManager.prototype.GetReport = function () {
    var self = this;
    NapTicketControl.disableInput();

    self.ResetSorter();
    $.ajax({
        type: "POST",
        url: window.rootUrl + "ReportRequestTicket/GetRequestTicketReport",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        data: JSON.stringify(self.Params.reportParams),
        beforeSend: function () {
        },
        success: function (rs) {
            self.RenderRequestTicketReport_FreezeGrid(rs);
            //            self.RenderRequestTicketReport(rs);
        },
        error: function (event, jqXHR, ajaxSettings) {
            self.RenderRequestTicketReport_FreezeGrid(null);
            //            self.RenderRequestTicketReport(null);
        },
        complete: function () {
            NapTicketControl.enableInput();
        }
    });
};
ReportManager.prototype.RenderRequestTicketReport_FreezeGrid = function (report, isSorting) {
    var self = this;
    self.CacheData.requestTicketReport = report;
    self.Tables.freezeGridReportContainer.removeClass("display-none");
    self.Tables.freezeGridReport.addClass("display-none");
    var bodyData = [];
    var footerData = [];
    if (report && report.ChildInfo && report.ChildInfo.length > 0) {
        var requestLength = report.ChildInfo.length;
        for (var i = 0; i < requestLength; i++) {
            var info = report.ChildInfo[i];
            var infoStr = "'" + info.RequestNo + "'";
            var displayInfo = {
                ColRequestNo: info.RequestNo,
                ColRequestor: self.EscapeHTML(self.FormatRemark(info.Requestor, 12, true)),
                ColDateOfRequest: info.DateOfRequest,
                ColReportName: self.EscapeHTML(self.FormatRemark(info.ReportName, 15, true)),
                ColRemark: self.EscapeHTML(self.FormatRemark(info.Remark, self.CacheData.maxRemarkDisplayLength_FreezeGrid, true)),
                ColDateline: info.Dateline,
                ColAssignee: self.EscapeHTML(self.FormatRemark(info.Assignee, 12, true)),
                ColStatusOfReport: info.StatusOfReport,
                ColReportByPreparer: self.EscapeHTML(self.FormatRemark(info.RemarkByPreparer, 16, true)),
                ColLastModifier: self.EscapeHTML(self.FormatRemark(info.LastModifier, 17, true)),
                ColEdit: '<div class="icon-edit" title="Edit" onclick="reportManager.EditRequestTicket(' + infoStr + ')"></div>',
                ColDelete: '<div class="align-center icon-delete" title="Delete" onclick="reportManager.DeleteRequestTicket(' + infoStr + ')">'
            };

            bodyData.push({
                Columns: [{ Name: "ColRequestNo", Value: displayInfo.ColRequestNo },
                          { Name: "ColRequestor", Value: displayInfo.ColRequestor, CustomAttr: 'title="' + self.EscapeHTML(info.Requestor) + '"', CssClass: 'reportTooltip' },
                          { Name: "ColDateOfRequest", Value: displayInfo.ColDateOfRequest },
                          { Name: "ColReportName", Value: displayInfo.ColReportName, CustomAttr: 'title="' + self.EscapeHTML(info.ReportName) + '"', CssClass: 'reportTooltip' },
                          { Name: "ColRemark", Value: displayInfo.ColRemark, CssClass: "text-overflow-ellipsis reportTooltip remark", CustomAttr: 'title="' + self.EscapeHTML(info.Remark) + '"' },
                          { Name: "ColDateline", Value: displayInfo.ColDateline },
                          { Name: "ColAssignee", Value: displayInfo.ColAssignee, CustomAttr: 'title="' + self.EscapeHTML(info.Assignee) + '"', CssClass: 'reportTooltip' },
                          { Name: "ColStatusOfReport", Value: displayInfo.ColStatusOfReport },
                          { Name: "ColReportByPreparer", Value: displayInfo.ColReportByPreparer, CssClass: "text-overflow-ellipsis reportTooltip", CustomAttr: 'title="' + self.EscapeHTML(info.RemarkByPreparer) + '"' },
                          { Name: "ColLastModifier", Value: displayInfo.ColLastModifier },
                          { Name: "ColEdit", Value: displayInfo.ColEdit },
                          { Name: "ColDelete", Value: displayInfo.ColDelete}]
            });
        }
    }

    var gridOptions = {
        customizeHeader: false,
        columns: [{ HeaderText: "Request No.", Width: 120, Name: "ColRequestNo", HeaderAlign: "center", CellAlign: "center", Sort: true },
                  { HeaderText: "Requestor", Width: 120, Name: "ColRequestor", HeaderAlign: "center", CellAlign: "align-left", Sort: true },
                  { HeaderText: "Date of Request", Width: 140, Name: "ColDateOfRequest", HeaderAlign: "center", CellAlign: "center", Sort: true },
                  { HeaderText: "Report Name", Width: 120, Name: "ColReportName", HeaderAlign: "center", CellAlign: "align-left", Sort: true },
                  { HeaderText: "Remark", Width: 140, Name: "ColRemark", HeaderAlign: "center", CellAlign: "align-left" },
                  { HeaderText: "Dateline", Width: 100, Name: "ColDateline", HeaderAlign: "center", CellAlign: "center", Sort: true },
                  { HeaderText: "Assigned to", Width: 100, Name: "ColAssignee", HeaderAlign: "center", CellAlign: "align-left", Sort: true },
                  { HeaderText: "Status of Report", Width: 140, Name: "ColStatusOfReport", HeaderAlign: "center", CellAlign: "center", Sort: true },
                  { HeaderText: "Remark by Preparer", Width: 140, Name: "ColReportByPreparer", HeaderAlign: "center", CellAlign: "align-left" },
                  { HeaderText: "Last Modified by", Width: 140, Name: "ColLastModifier", HeaderAlign: "center", CellAlign: "align-left", Sort: true },
                  { HeaderText: "Edit", Width: 60, Name: "ColEdit", HeaderAlign: "center", CellAlign: "center" },
                  { HeaderText: "Delete", Width: 60, Name: "ColDelete", HeaderAlign: "center", CellAlign: "center" }],
        showTitle: false,
        showFooter: false,
        showTree: false,
        bodyRows: bodyData,
        footerRows: footerData,
        gridExpandHeight: 250,
        gridWidth: 600,
        numberOfFrozenColumn: 1,
        showDistinctRow: true,
        sortOption: {
            customGridSort: function (rows, sortColumnName, sortDirection) {
                self.MappingSortInfo_FreezeGrid(sortDirection, sortColumnName);
                rows.sort(function (rowA, rowB) {
                    var elementA = {};
                    var elementB = {};
                    var rowALength = rowA.Columns.length;
                    for (var i = 0; i < rowALength; i++) {
                        var infoRowA = rowA.Columns[i];
                        elementA[infoRowA.Name] = infoRowA.Value;
                    }

                    var rowBLength = rowB.Columns.length;
                    for (var i = 0; i < rowBLength; i++) {
                        var infoRowB = rowB.Columns[i];
                        elementB[infoRowB.Name] = infoRowB.Value;
                    }

                    var valueA;
                    var valueB;
                    var lessThanFactor;
                    switch (self.CacheData.sortDataType) {
                        case reportManager.Enums.SortDataType.Number:
                            valueA = parseFloat(elementA[self.CacheData.sortField].replace(/R/g, ''));
                            valueB = parseFloat(elementB[self.CacheData.sortField].replace(/R/g, ''));
                            lessThanFactor = (valueA - valueB) < 0 ? -1 : 1;
                            break;
                        case reportManager.Enums.SortDataType.String:
                            valueA = elementA[self.CacheData.sortField];
                            valueB = elementB[self.CacheData.sortField];
                            lessThanFactor = valueA < valueB ? -1 : 1;
                            break;
                        case reportManager.Enums.SortDataType.DateTime:
                            valueA = elementA[self.CacheData.sortField];
                            valueB = elementB[self.CacheData.sortField];
                            if (valueA == '') {
                                lessThanFactor = -1;
                                break;
                            }

                            if (valueB == '') {
                                lessThanFactor = 1;
                                break;
                            }

                            lessThanFactor = new Date(valueA) < new Date(valueB) ? -1 : 1;
                            break;
                    }

                    if (valueA == valueB) {
                        return 0;
                    }

                    if (lessThanFactor * self.CacheData.sortFactor < 0) {
                        return -1;
                    }

                    return 1;
                });

                return rows;
            }
        }
    };

    self.Tables.freezeGridReport.removeClass("display-none").fanexGrid(gridOptions);
    //    self.DisplaySorter(report);

    self.Tables.freezeGridReport.find(".reportTooltip").each(function () {
        var tooltipTitle = self.EscapeHTML($(this).attr('title')).replace(/\n/g, '<br />');
        $(this).attr('title', tooltipTitle);

        $(this).tipsy({
            html: true,
            opacity: 0.9,
            gravity: 'n'
        });
    });
};
ReportManager.prototype.RenderRequestTicketReport = function (report, isSorting) {
    var self = this;
    self.CacheData.requestTicketReport = report;
    var bodyHtml = '';
    if (report && report.ChildInfo && report.ChildInfo.length > 0) {
        var requestLength = report.ChildInfo.length;
        for (var i = 0; i < requestLength; i++) {
            var info = report.ChildInfo[i];
            var requestNo = info.RequestNo;
            var infoStr = "'" + info.RequestNo + "','" +
                                self.EscapeHTML(info.Requestor) + "','" +
                                info.DateOfRequest + "','" +
                                self.EscapeHTML(info.ReportName) + "','" +
                                self.EscapeHTML(info.Remark) + "','" +
                                info.Dateline + "','" +
                                self.EscapeHTML(info.Assignee) + "','" +
                                info.StatusOfReport + "','" +
                                self.EscapeHTML(info.RemarkByPreparer) + "','" +
                                self.EscapeHTML(info.LastModifier) + "'";

            var htmlContent = '<tr>\
                                     <td>\
                                          <div class="col100 align-center">' + requestNo + '</div>\
                                     </td>\
                                     <td>\
                                          <div class="col100 reportTooltip" title="' + self.EscapeHTML(info.Requestor) + '">' + self.FormatRemark(info.Requestor, 13, true) + '</div>\
                                     </td>\
                                     <td>\
                                          <div class="col140 align-center">' + info.DateOfRequest + '</div>\
                                     </td>\
                                     <td>\
                                          <div class="col120 reportTooltip" title="' + self.EscapeHTML(info.ReportName) + '">' + self.FormatRemark(info.ReportName, 19, true) + '</div>\
                                     </td>\
                                     <td>\
                                          <div class="col140 text-overflow-ellipsis remark reportTooltip" title="' + self.EscapeHTML(info.Remark) + '"></div>\
                                     </td>\
                                     <td>\
                                          <div class="col100 align-center">' + (info.Dateline ? info.Dateline : '') + '</div>\
                                     </td>\
                                     <td>\
                                          <div class="col100 reportTooltip" title="' + self.EscapeHTML(info.Assignee) + '">' + self.FormatRemark(info.Assignee, 13, true) + '</div>\
                                     </td>\
                                     <td>\
                                          <div class="col140 align-center">' + info.StatusOfReport + '</div>\
                                     </td>\
                                     <td>\
                                          <div class="col140 text-overflow-ellipsis remark remarkByPreparer reportTooltip" title="' + self.EscapeHTML(info.RemarkByPreparer) + '"></div>\
                                     </td>\
                                     <td>\
                                          <div class="col140">' + self.FormatRemark(info.LastModifier, 20, true) + '</div>\
                                     </td>\
                                     <td>\
                                          <div class="col60 align-center"><div class="icon-edit"' + (this.CacheData.hasHostPermission ? '  title="Edit" onclick="reportManager.EditRequestTicket(' + infoStr.replace(/\n/g, "\\n") + '); return;"' : ' title="disabled" disabled="disabled"') + '></div></div>\
                                     </td>\
                                     <td>\
                                           <div class="col60 align-center"><div class="align-center icon-delete"' + (this.CacheData.hasHostPermission ? ' title="Delete" onclick="reportManager.DeleteRequestTicket(' + infoStr.replace(/\n/g, "\\n") + '); return;"' : ' title="disabled" disabled="disabled"') + '></div></div>\
                                     </td>\
                                 </tr>';
            var bodyDiv = $('<table>' + htmlContent + '</table>');
            bodyDiv.find(".remark").html(self.EscapeHTML(self.FormatRemark(info.Remark, self.CacheData.maxRemarkDisplayLength, true)));
            bodyDiv.find(".remarkByPreparer").html(self.EscapeHTML(self.FormatRemark(info.RemarkByPreparer, self.CacheData.maxRemarkDisplayLength, true)));
            bodyHtml += bodyDiv.find("tbody").html();
        }
    } else {
        bodyHtml = '<tr>\
                        <td colspan="12" class="no-data-not-frozen">There is no data available.</td>\
                    </tr>';
    }

    bodyHtml = fixBugsBrowser.removeSpaceIE9(bodyHtml);
    self.Tables.report_Body.empty().append(bodyHtml);
    self.DisplaySorter(report);

    self.Tables.report.find(".reportTooltip").each(function () {
        //        $(this).tipsy({
        //            html: true,
        //            opacity: 0.9,
        //            gravity: 'n'
        //        });

        if ($(this).hasClass("remark")) {
            $(this).html(self.FormatRemark($(this).html(), self.CacheData.maxRemarkDisplayLength, true));
        }
    });

    self.Tables.report.removeClass("display-none");
};
ReportManager.prototype.btnAddNewClick = function () {
    var self = this;
    if (!self.CacheData.hasHostPermission) {
        return;
    }

    //    NapTicketControl.disableInput();
    NapTicketControl.popUp.removeHighLight();
    //    $.ajax({
    //        type: "POST",
    //        url: window.rootUrl + "Report/ReportRequestTicket/GetMaxRequestNo",
    //        contentType: "application/json; charset=utf-8",
    //        dataType: "json",
    //        data: JSON.stringify(self.Params.getMaxRequestNo),
    //        beforeSend: function () {
    //        },
    //        success: function (rs) {
    //            self.CacheData.maxRequestNo = rs;
    //            self.ShowAddNewRequestTicketPopup();
    //        },
    //        error: function (event, jqXHR, ajaxSettings) {
    //        },
    //        complete: function () {
    //            NapTicketControl.enableInput();
    //        }
    //    });

    self.ShowAddNewRequestTicketPopup();
};
ReportManager.prototype.ShowAddNewRequestTicketPopup = function () {
    var self = this;

    self.CacheData.isSelectDateline_AddNew = false;
    self.InitAddNewRequestTicketPopup();
    self.Tables.addNewRequestTicketPopup.removeClass("hidden");
    self.Tables.addNewRequestTicketPopup.dialog({
        title: "Add new report request ticket",
        autoOpen: false,
        height: 650,
        width: 600,
        modal: false,
        resizable: false,
        position: "center",
        close: function (event, ui) {
            self.ClosePopup();
        }
    });

    self.Tables.addNewRequestTicketPopup.dialog("open");
    self.CacheData.currentDialog = self.Tables.addNewRequestTicketPopup;
};
ReportManager.prototype.InitAddNewRequestTicketPopup = function () {
    var self = this;

    //    var requestNo = self.CacheData.maxRequestNo;
    //    self.Tables.txtRequestNo.val(requestNo);
    self.Tables.txtRequestor.val(requestor);
    self.Tables.txtDateOfRequest.val(toDay);
};
ReportManager.prototype.RenderDatelineCalendar = function () {
    var self = this;
    $('#txtDateline').nexDatePicker({
        nexDatePickerType: "single",
        firstDatePicker: {
            Id: "CheckDate",
            Label: "Dateline",
            InitDate: '',
            LabelCss: "lbl-field col_5 alpha",
            TextboxCss: "edit-field col_15 omega"
        },

        datePickerFormat: {
            dateFormat: TicketControlConstants.FormatConstant.M_DD_YY_STRING,
            showOtherMonths: true,
            selectOtherMonths: true,
            changeMonth: true,
            changeYear: true,
            minDate: new Date(toDay)
        },
        CustomFunction: self.CalendarSelect
    });

    self.Tables.txtDateline = self.Tables.addNewRequestTicketPopup.find("#txtDateline");
    self.Tables.addNewRequestTicketPopup.find("#CheckDate").val("");
};
ReportManager.prototype.CalendarSelect = function () {
    reportManager.CacheData.isSelectDateline_AddNew = true;
};
ReportManager.prototype.CalendarSelect_Edit = function () {
    reportManager.CacheData.isSelectDateline_Edit = true;
};
ReportManager.prototype.RenderDatelineCalendar_Edit = function () {
    var self = this;
    self.Tables.txtDateline_Edit = self.Tables.editRequestTicketPopup.find("#txtDateline_Edit");
    self.Tables.txtDateline_Edit.nexDatePicker({
        nexDatePickerType: "single",
        firstDatePicker: {
            Id: "CheckDate_Edit",
            Label: "Dateline",
            InitDate: toDay,
            LabelCss: "lbl-field col_5 alpha",
            TextboxCss: "edit-field col_15 omega"
        },

        datePickerFormat: {
            dateFormat: TicketControlConstants.FormatConstant.M_DD_YY_STRING,
            showOtherMonths: true,
            selectOtherMonths: true,
            changeMonth: true,
            changeYear: true,
            minDate: new Date(toDay)
        },

        CustomFunction: self.CalendarSelect_Edit
    });

    self.Tables.txtDateline_Edit = self.Tables.editRequestTicketPopup.find("#txtDateline_Edit");
    self.Tables.CheckDate_Edit = self.Tables.editRequestTicketPopup.find("#CheckDate_Edit");
    self.Tables.CheckDate_Edit.val("");
};
ReportManager.prototype.btnSubmitClick = function () {
    var self = this;
    if (!self.CacheData.hasHostPermission) {
        return;
    }

    NapTicketControl.disableInput();
    self.GetAddNewRequestTicketParams();

    if (!self.ValidRequestTicketParams_AddNew()) {
        NapTicketControl.enableInput();
        return;
    }

    $.ajax({
        type: "POST",
        url: window.rootUrl + "Report/ReportRequestTicket/AddNewRequestTicket",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        data: JSON.stringify(self.Params.addNewRequestTicketParams),
        beforeSend: function () {
        },
        success: function (rs) {
            self.InformAddNewResult(rs);
        },
        error: function (event, jqXHR, ajaxSettings) {
            self.InformAddNewResult(null);
        },
        complete: function () {
            NapTicketControl.enableInput();
        }
    });
};

ReportManager.prototype.ValidRequestTicketParams_AddNew = function () {
    var self = this;
    self.Tables.resultMessage_AddNew.empty()
                                    .addClass("display-none")
                                    .removeClass("resultMessage")
                                    .removeClass("err");

    if (self.Params.addNewRequestTicketParams.ReportName == '') {
        self.Tables.resultMessage_AddNew.empty()
                                      .html("Please input report name!")
                                      .addClass("resultMessage")
                                      .addClass("err")
                                      .removeClass("display-none");
        return false;
    }

    return true;
};

ReportManager.prototype.InformAddNewResult = function (result) {
    var self = this;
    if (result && result.ErrorCode > 0) {
        self.Tables.resultMessage_AddNew.empty()
                                        .html(result.ErrorMessage)
                                        .addClass("resultMessage")
                                        .addClass("err")
                                        .removeClass("display-none");

    } else {
        self.Tables.resultMessage_AddNew.empty()
                                        .addClass("display-none");

        NapTicketControl.popUp.show("Add new report request ticket successfully!", "", TicketControlConstants.MessageType.Success);
        self.ClosePopup();
        self.ReloadReport();
    }
};
ReportManager.prototype.GetAddNewRequestTicketParams = function () {
    var self = this;
    self.Params.addNewRequestTicketParams.RequestNo = self.Tables.txtRequestNo.val();
    self.Params.addNewRequestTicketParams.Requestor = self.Tables.txtRequestor.val();
    self.Params.addNewRequestTicketParams.ReqDate = self.Tables.txtDateOfRequest.val();
    self.Params.addNewRequestTicketParams.ReportName = self.Tables.txtReportName.val();
    self.Params.addNewRequestTicketParams.Remark = self.Tables.txtRemark.val();
    self.Params.addNewRequestTicketParams.Dateline = self.CacheData.isSelectDateline_AddNew ?
        self.Tables.txtDateline.nexDatePicker("getSingleDate") :
        "";
    self.Params.addNewRequestTicketParams.Assignee = self.Tables.txtAssignee.val();
    self.Params.addNewRequestTicketParams.Status = self.Tables.ddlStatusOfReport_AddNew.find("option:selected").text();
    self.Params.addNewRequestTicketParams.RemarkByPreparer = self.Tables.txtRemarkByPreparer.val();
};
ReportManager.prototype.PaddingZero = function (str, max) {
    var self = this;
    return str.length < max ? self.PaddingZero("0" + str, max) : str;
};
ReportManager.prototype.FormatRemark = function (str, max, notUsingSuffix) {
    var result = '';
    if (str && str.length > max && max >= 3) {
        if (notUsingSuffix) {
            maxLength = max;
        } else {
            maxLength = max - 3;
        }

        var wordArr = str.split(' ');
        var wordArrLength = wordArr.length;
        var currentLength = 0;
        for (var i = 0; i < wordArrLength && currentLength <= maxLength; i++) {
            var wordLength = wordArr[i].length;
            if (wordLength == 0) {
                result += ' ';
                currentLength++;
                continue;
            }

            if (currentLength + wordLength <= maxLength) {
                result += wordArr[i];
                currentLength += wordLength;
                if (currentLength < maxLength && i < wordArrLength - 1) {
                    result += ' ';
                    currentLength++;
                }

                continue;
            }

            var remainChar = maxLength - currentLength;
            if (remainChar < wordLength) {
                result += wordArr[i].substring(0, remainChar);
                currentLength += remainChar;
                if (currentLength < maxLength && i < wordArrLength - 1) {
                    result += ' ';
                    currentLength++;
                }

                break;
            }

            if (remainChar == wordLength) {
                result += wordArr[i].substring(0, remainChar);
                currentLength += remainChar;
                break;
            }
        }

        if (!notUsingSuffix) {
            result += "...";
        }
    } else {
        result = str;
    }

    return result;
};
ReportManager.prototype.DeleteRequestTicket = function (requestNo,
    requestor,
    dateOfRequest,
    reportName,
    remark,
    dateline,
    assignee,
    statusOfReport,
    remarkByPreparer,
    lastModifier) {
    var self = this;
    if (!self.CacheData.hasHostPermission) {
        return;
    }

    //    var requestTicketInfo = $.parseJSON(requestTicketStr);
    //    var requestTicketInfo = {
    //        RequestNo: requestNo,
    //        Requestor: self.UnescapeHTML(requestor),
    //        DateOfRequest: dateOfRequest,
    //        ReportName: self.UnescapeHTML(reportName),
    //        Remark: self.UnescapeHTML(remark),
    //        Dateline: dateline,
    //        Assignee: self.UnescapeHTML(assignee),
    //        StatusOfReport: statusOfReport,
    //        RemarkByPreparer: self.UnescapeHTML(remarkByPreparer),
    //        LastModifier: self.UnescapeHTML(lastModifier)
    //    };

    var requestLength = self.CacheData.requestTicketReport.ChildInfo.length;
    var requestTicketInfo;
    for (var i = 0; i < requestLength; i++) {
        requestTicketInfo = self.CacheData.requestTicketReport.ChildInfo[i];
        if (requestTicketInfo.RequestNo == requestNo) {
            break;
        }
    }

    if (requestTicketInfo == null || !self.CacheData.hasHostPermission) {
        return;
    }

    var requestNo = requestTicketInfo.RequestNo;
    self.Params.deleteRequestTicketParams = {
        ReqNo: requestNo,
        Requestor: requestTicketInfo.Requestor,
        DateOfRequest: requestTicketInfo.DateOfRequest,
        ReportName: requestTicketInfo.ReportName,
        Remark: requestTicketInfo.Remark,
        Dateline: requestTicketInfo.Dateline,
        Assignee: requestTicketInfo.Assignee,
        StatusOfReport: requestTicketInfo.StatusOfReport,
        RemarkByPreparer: requestTicketInfo.RemarkByPreparer,
        LastModifiedBy: requestor
    };

    if (confirm('Are you sure you want to delete the selected Report Request Ticket?')) {
        NapTicketControl.disableInput();
        NapTicketControl.popUp.removeHighLight();
        $.ajax({
            type: "POST",
            url: window.rootUrl + "Report/ReportRequestTicket/DeleteRequestTicket",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            data: JSON.stringify(self.Params.deleteRequestTicketParams),
            beforeSend: function () {
            },
            success: function (rs) {
                self.InformDeleteResult(rs);
            },
            error: function (event, jqXHR, ajaxSettings) {
                self.InformDeleteResult(null);
            },
            complete: function () {
                NapTicketControl.enableInput();
            }
        });
    }
};
ReportManager.prototype.InformDeleteResult = function (result) {
    var self = this;
    result = null;
    if (result && result.ErrorCode > 0) {
        NapTicketControl.popUp.show(result.ErrorMessage, "");
    } else {
        NapTicketControl.popUp.show("Delete report request ticket successfully!", "", TicketControlConstants.MessageType.Success);
        self.ReloadReport();
    }
};
ReportManager.prototype.EditRequestTicket = function (requestNo,
    requestor,
    dateOfRequest,
    reportName,
    remark,
    dateline,
    assignee,
    statusOfReport,
    remarkByPreparer,
    lastModifier) {
    var self = this;
    if (!self.CacheData.hasHostPermission) {
        return;
    }

    //    var requestTicketInfo = $.parseJSON(requestTicketStr);
    //    var requestTicketInfo = {
    //        RequestNo: requestNo,
    //        Requestor: self.UnescapeHTML(requestor),
    //        DateOfRequest: dateOfRequest,
    //        ReportName: self.UnescapeHTML(reportName),
    //        Remark: self.UnescapeHTML(remark),
    //        Dateline: dateline,
    //        Assignee: self.UnescapeHTML(assignee),
    //        StatusOfReport: statusOfReport,
    //        RemarkByPreparer: self.UnescapeHTML(remarkByPreparer),
    //        LastModifier: self.UnescapeHTML(lastModifier)
    //    };

    var requestLength = self.CacheData.requestTicketReport.ChildInfo.length;
    var requestTicketInfo;
    for (var i = 0; i < requestLength; i++) {
        requestTicketInfo = self.CacheData.requestTicketReport.ChildInfo[i];
        if (requestTicketInfo.RequestNo == requestNo) {
            break;
        }
    }

    if (requestTicketInfo == null || !self.CacheData.hasHostPermission) {
        return;
    }

    self.CacheData.isSelectDateline_Edit = requestTicketInfo.Dateline ? true : false;
    self.InitEditRequestTicketPopup(requestTicketInfo);
    self.Tables.editRequestTicketPopup.removeClass("hidden");
    self.Tables.editRequestTicketPopup.dialog({
        title: "Edit report request ticket",
        autoOpen: false,
        height: 700,
        width: 600,
        modal: false,
        resizable: false,
        position: "center",
        close: function (event, ui) {
            self.ClosePopup();
        }
    });

    self.Tables.editRequestTicketPopup.dialog("open");
    self.CacheData.currentDialog = self.Tables.editRequestTicketPopup;
};
ReportManager.prototype.btnSubmit_EditClick = function () {
    var self = this;

    NapTicketControl.disableInput();
    self.GetEditRequestTicketParams();

    if (!self.ValidRequestTicketParams_Edit()) {
        NapTicketControl.enableInput();
        return;
    }

    $.ajax({
        type: "POST",
        url: window.rootUrl + "Report/ReportRequestTicket/UpdateRequestTicket",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        data: JSON.stringify(self.Params.editRequestTicketParams),
        beforeSend: function () {
        },
        success: function (rs) {
            self.InformEditResult(rs);
        },
        error: function (event, jqXHR, ajaxSettings) {
            self.InformEditResult(null);
        },
        complete: function () {
            NapTicketControl.enableInput();
        }
    });
};
ReportManager.prototype.ValidRequestTicketParams_Edit = function () {
    var self = this;
    self.Tables.resultMessage_Edit.empty()
                                    .addClass("display-none")
                                    .removeClass("resultMessage")
                                    .removeClass("err");

    if (self.Params.editRequestTicketParams.ReportName == '') {
        self.Tables.resultMessage_Edit.empty()
                                      .html("Please input report name!")
                                      .addClass("resultMessage")
                                      .addClass("err")
                                      .removeClass("display-none");
        return false;
    }

    return true;
};
ReportManager.prototype.GetEditRequestTicketParams = function () {
    var self = this;
    self.Params.editRequestTicketParams.ReqNo = self.Tables.txtRequestNo_Edit.val();
    self.Params.editRequestTicketParams.Requestor = self.Tables.txtRequestor_Edit.val();
    self.Params.editRequestTicketParams.DateOfRequest = self.Tables.txtDateOfRequest_Edit.val();
    self.Params.editRequestTicketParams.ReportName = self.Tables.txtReportName_Edit.val();
    self.Params.editRequestTicketParams.Remark = self.Tables.txtRemark_Edit.val();
    self.Params.editRequestTicketParams.Dateline = self.CacheData.isSelectDateline_Edit ?
        self.Tables.txtDateline_Edit.nexDatePicker("getSingleDate") :
        "";
    self.Params.editRequestTicketParams.Assignee = self.Tables.txtAssignee_Edit.val();
    self.Params.editRequestTicketParams.Status = self.Tables.ddlStatusOfReport_Edit.find("option:selected").text();
    self.Params.editRequestTicketParams.RemarkByPreparer = self.Tables.txtRemarkByPreparer_Edit.val();
    self.Params.editRequestTicketParams.LastModifiedBy = requestor;
};
ReportManager.prototype.InformEditResult = function (result) {
    var self = this;
    if (result && result.ErrorCode > 0) {
        self.Tables.resultMessage_Edit.empty()
                                      .html(result.ErrorMessage)
                                      .addClass("resultMessage")
                                      .addClass("err")
                                      .removeClass("display-none");

    } else {
        self.Tables.resultMessage_Edit.empty()
                                      .addClass("display-none");
        self.ClosePopup();
        self.ReloadReport();
        NapTicketControl.popUp.show("Update report request ticket successfully!", "", TicketControlConstants.MessageType.Success);
    }
};
ReportManager.prototype.ClosePopup = function () {
    var self = this;
    if (self.CacheData.currentDialog) {
        self.CacheData.currentDialog.dialog("close");
        self.Tables.addNewRequestTicketPopup.remove();
        self.Tables.editRequestTicketPopup.remove();
        self.CacheData.currentDialog = null;
    }

    self.Tables.addNewRequestTicketPopupContainer.append(self.CacheData.addNewRequestTicketPopupHtml);
    self.Tables.editRequestTicketPopupContainer.append(self.CacheData.editRequestTicketPopupHtml);
    self.InitPage();
};
ReportManager.prototype.ReloadReport = function () {
    var self = this;
    self.GetReport();
};
ReportManager.prototype.InitEditRequestTicketPopup = function (requestTicketInfo) {
    var self = this;

    var requestNo = requestTicketInfo.RequestNo;
    self.Tables.txtRequestNo_Edit.val(requestNo);
    self.Tables.txtRequestor_Edit.val(requestTicketInfo.Requestor);
    self.Tables.txtDateOfRequest_Edit.val(requestTicketInfo.DateOfRequest);
    self.Tables.txtReportName_Edit.val(requestTicketInfo.ReportName);
    self.Tables.txtRemark_Edit.val(requestTicketInfo.Remark);
    if (requestTicketInfo.Dateline) {
        var realDateline = moment(requestTicketInfo.Dateline, "MM-DD-YYYY");
        self.Tables.CheckDate_Edit.datepicker("setDate", new Date(moment(realDateline).format('YYYY/MM/DD')));
    }

    self.Tables.txtAssignee_Edit.val(requestTicketInfo.Assignee);


    self.Tables.ddlStatusOfReport_Edit.find("option:contains(" + requestTicketInfo.StatusOfReport + ")")
                                      .each(function () {
                                          if ($(this).text() == requestTicketInfo.StatusOfReport) {
                                              $(this).attr('selected', 'selected');
                                          }
                                      });
    self.Tables.txtRemarkByPreparer_Edit.val(requestTicketInfo.RemarkByPreparer);
};
ReportManager.prototype.EscapeHTML = function (str) {
    var chr = { '"': '&quot;',
        '&': '&amp;',
        '<': '&lt;',
        '>': '&gt;',
        "'": '&#39;',
        "/": '&#x2F;',
        "\\": "&#92;"
    };

    return str.replace(/[\"&<>'\/\\]/g, function (a) { return chr[a]; });
};
ReportManager.prototype.UnescapeHTML = function (str) {
    var chr = { '"': '&quot;', '&': '&amp;', '<': '&lt;', '>': '&gt;' };
    return str.replace(new RegExp("\\n", "g"), "\n");
};
ReportManager.prototype.DisplaySorter = function (report) {
    var self = this;
    //    self.CacheData.sorterList = self.Tables.freezeGridReport.find(".sorter");
    self.CacheData.sorterList = self.Tables.report.find(".sorter");
    if (report && report.ChildInfo && report.ChildInfo.length > 0) {
        self.CacheData.sorterList.attr("title", "Sort");
        self.CacheData.sorterList.removeClass("disableSorter");
        self.CacheData.sorterList.each(function () {
            $(this).attr("title", "Sort by " + self.GetDisplaySorterName($(this).attr("fieldToSort")))
            //            $(this).attr("title", "Sort by " + $(this).html())
        });

        self.CacheData.sorterList.unbind("click");
        self.CacheData.sorterList.click(function () {
            self.btnSorterClick($(this));
        });
    } else {
        self.CacheData.sorterList.addClass("disableSorter");
    }
};
ReportManager.prototype.GetDisplaySorterName = function (fieldToSort) {
    var self = this;
    var fieldDisplay = '';
    switch (fieldToSort) {
        case reportManager.Enums.FieldToSort.RequestNo:
            fieldDisplay = 'Request No.';
            break;
        case reportManager.Enums.FieldToSort.Requestor:
            fieldDisplay = 'Requestor';
            break;
        case reportManager.Enums.FieldToSort.DateOfRequest:
            fieldDisplay = ' Date of Request';
            break;
        case reportManager.Enums.FieldToSort.ReportName:
            fieldDisplay = 'ReportName';
            break;
        case reportManager.Enums.FieldToSort.Dateline:
            fieldDisplay = 'Dateline';
            break;
        case reportManager.Enums.FieldToSort.Assignee:
            fieldDisplay = 'Assignee';
            break;
        case reportManager.Enums.FieldToSort.StatusOfReport:
            fieldDisplay = ' Status of Report';
            break;
        case reportManager.Enums.FieldToSort.LastModifier:
            fieldDisplay = 'Last Modifier';
            break;
    }

    return fieldDisplay;
};
ReportManager.prototype.btnSorterClick = function (btn) {
    var self = this;
    var oldSorter = self.CacheData.currentSorter;
    var fieldToSort = btn.attr("fieldToSort");
    if (oldSorter && oldSorter.attr("fieldToSort") != fieldToSort) {
        oldSorter.removeClass(reportManager.Enums.SortDirection.Asc)
                 .removeClass(reportManager.Enums.SortDirection.Desc);
    }

    self.CacheData.currentSorter = btn;
    var sortDirection = self.ChangeSorterStatus(btn);
    self.MappingSortInfo(sortDirection, fieldToSort);
    //    self.MappingSortInfo_FreezeGrid(sortDirection, btn.html());

    var report = self.CacheData.requestTicketReport;
    if (report && report.ChildInfo && report.ChildInfo.length > 0) {
        report.ChildInfo.sort(function (elementA, elementB) {
            var valueA;
            var valueB;
            var lessThanFactor;
            switch (self.CacheData.sortDataType) {
                case reportManager.Enums.SortDataType.Number:
                    valueA = parseFloat(elementA[self.CacheData.sortField].replace(/R/g, ''));
                    valueB = parseFloat(elementB[self.CacheData.sortField].replace(/R/g, ''));
                    lessThanFactor = (valueA - valueB) < 0 ? -1 : 1;
                    break;
                case reportManager.Enums.SortDataType.String:
                    valueA = elementA[self.CacheData.sortField];
                    valueB = elementB[self.CacheData.sortField];
                    lessThanFactor = valueA < valueB ? -1 : 1;
                    break;
                case reportManager.Enums.SortDataType.DateTime:
                    valueA = elementA[self.CacheData.sortField];
                    valueB = elementB[self.CacheData.sortField];
                    if (valueA == '') {
                        lessThanFactor = -1;
                        break;
                    }

                    if (valueB == '') {
                        lessThanFactor = 1;
                        break;
                    }

                    lessThanFactor = new Date(valueA) < new Date(valueB) ? -1 : 1;
                    break;
            }

            if (valueA == valueB) {
                return 0;
            }

            if (lessThanFactor * self.CacheData.sortFactor < 0) {
                return -1;
            }

            return 1;
        });

        //        self.RenderRequestTicketReport_FreezeGrid(report, true);
        self.RenderRequestTicketReport(report, true);
    }
};
ReportManager.prototype.ChangeSorterStatus = function (btn) {
    var sortDirection = "";
    var sortDirectionEnums = reportManager.Enums.SortDirection;
    if (btn.hasClass(sortDirectionEnums.Asc)) {
        sortDirection = sortDirectionEnums.Desc;
        btn.removeClass(sortDirectionEnums.Asc).addClass(sortDirection);
    } else if (btn.hasClass(sortDirectionEnums.Desc)) {
        sortDirection = sortDirectionEnums.Asc;
        btn.removeClass(sortDirectionEnums.Desc).addClass(sortDirection);
    } else {
        sortDirection = sortDirectionEnums.Asc;
        btn.addClass(sortDirection);
    }

    return sortDirection;
};
ReportManager.prototype.MappingSortInfo = function (sortDirection, fieldToSort) {
    var self = this;
    var sortDirectionEnums = reportManager.Enums.SortDirection;
    switch (sortDirection) {
        case sortDirectionEnums.Asc:
            self.CacheData.sortFactor = 1;
            break;
        case sortDirectionEnums.Desc:
            self.CacheData.sortFactor = -1;
            break;
        default:
            self.CacheData.sortFactor = 0;
            break;
    }

    self.CacheData.sortField = fieldToSort;
    switch (fieldToSort) {
        case reportManager.Enums.FieldToSort.RequestNo:
            self.CacheData.sortField = reportManager.Enums.FieldToSort.RequestNo;
            self.CacheData.sortDataType = reportManager.Enums.SortDataType.Number;
            break;
        case reportManager.Enums.FieldToSort.Requestor:
            self.CacheData.sortField = reportManager.Enums.FieldToSort.Requestor;
            self.CacheData.sortDataType = reportManager.Enums.SortDataType.String;
            break;
        case reportManager.Enums.FieldToSort.DateOfRequest:
            self.CacheData.sortField = reportManager.Enums.FieldToSort.DateOfRequest;
            self.CacheData.sortDataType = reportManager.Enums.SortDataType.DateTime;
            break;
        case reportManager.Enums.FieldToSort.ReportName:
            self.CacheData.sortField = reportManager.Enums.FieldToSort.ReportName;
            self.CacheData.sortDataType = reportManager.Enums.SortDataType.String;
            break;
        case reportManager.Enums.FieldToSort.Dateline:
            self.CacheData.sortField = reportManager.Enums.FieldToSort.Dateline;
            self.CacheData.sortDataType = reportManager.Enums.SortDataType.DateTime;
            break;
        case reportManager.Enums.FieldToSort.Assignee:
            self.CacheData.sortField = reportManager.Enums.FieldToSort.Assignee;
            self.CacheData.sortDataType = reportManager.Enums.SortDataType.String;
            break;
        case reportManager.Enums.FieldToSort.StatusOfReport:
            self.CacheData.sortField = reportManager.Enums.FieldToSort.StatusOfReport;
            self.CacheData.sortDataType = reportManager.Enums.SortDataType.String;
            break;
        case reportManager.Enums.FieldToSort.LastModifier:
            self.CacheData.sortField = "LastModifier";
            self.CacheData.sortDataType = reportManager.Enums.SortDataType.String;
            break;
        default:
            self.CacheData.sortDataType = null;
            break;
    }
};
ReportManager.prototype.MappingSortInfo_FreezeGrid = function (sortDirection, fieldToSort) {
    var self = this;
    var sortDirectionEnums = reportManager.Enums.SortDirection;
    switch (sortDirection) {
        case "ASC":
            self.CacheData.sortFactor = 1;
            break;
        case "DESC":
            self.CacheData.sortFactor = -1;
            break;
        default:
            self.CacheData.sortFactor = 0;
            break;
    }

    self.CacheData.sortField = fieldToSort;
    switch (fieldToSort) {
        case "ColRequestNo":
            self.CacheData.sortDataType = reportManager.Enums.SortDataType.Number;
            break;
        case "ColRequestor":
            self.CacheData.sortDataType = reportManager.Enums.SortDataType.String;
            break;
        case "ColDateOfRequest":
            self.CacheData.sortDataType = reportManager.Enums.SortDataType.DateTime;
            break;
        case "ColReportName":
            self.CacheData.sortDataType = reportManager.Enums.SortDataType.String;
            break;
        case "ColDateline":
            self.CacheData.sortDataType = reportManager.Enums.SortDataType.DateTime;
            break;
        case "ColAssignee":
            self.CacheData.sortDataType = reportManager.Enums.SortDataType.String;
            break;
        case "ColStatusOfReport":
            self.CacheData.sortDataType = reportManager.Enums.SortDataType.String;
            break;
        case "ColLastModifier":
            self.CacheData.sortDataType = reportManager.Enums.SortDataType.String;
            break;
        default:
            self.CacheData.sortDataType = null;
            break;
    }
};
ReportManager.prototype.RenderDropdownReportStatus = function () {
    var self = this;

    var reportStatusEnum = reportManager.Enums.ReportStatus;
    var optionsHtml = '';
    //    optionsHtml += '<option value="' + reportStatusEnum.All.Id + '">' + reportStatusEnum.All.Name + '</option>';
    //    optionsHtml += '<option value="' + reportStatusEnum.Pending.Id + '">' + reportStatusEnum.Pending.Name + '</option>';
    //    optionsHtml += '<option value="' + reportStatusEnum.In_Progress.Id + '">' + reportStatusEnum.In_Progress.Name + '</option>';
    //    optionsHtml += '<option value="' + reportStatusEnum.Completed.Id + '">' + reportStatusEnum.Completed.Name + '</option>';
    //    self.Tables.ddlStatusOfReport.empty().append(optionsHtml);

    optionsHtml = '';
    optionsHtml += '<option value="' + reportStatusEnum.Pending.Id + '">' + reportStatusEnum.Pending.Name + '</option>';
    optionsHtml += '<option value="' + reportStatusEnum.In_Progress.Id + '">' + reportStatusEnum.In_Progress.Name + '</option>';
    optionsHtml += '<option value="' + reportStatusEnum.Completed.Id + '">' + reportStatusEnum.Completed.Name + '</option>';
    self.Tables.ddlStatusOfReport_AddNew.empty().append(optionsHtml);

    optionsHtml = '';
    optionsHtml += '<option value="' + reportStatusEnum.Pending.Id + '">' + reportStatusEnum.Pending.Name + '</option>';
    optionsHtml += '<option value="' + reportStatusEnum.In_Progress.Id + '">' + reportStatusEnum.In_Progress.Name + '</option>';
    optionsHtml += '<option value="' + reportStatusEnum.Completed.Id + '">' + reportStatusEnum.Completed.Name + '</option>';
    self.Tables.ddlStatusOfReport_Edit.empty().append(optionsHtml);
};
ReportManager.prototype.ResetSorter = function () {
    var self = this;
    if (self.CacheData.sorterList) {
        self.CacheData.sorterList.removeClass("asc").removeClass("desc");
    }
};
var reportManager = new ReportManager();