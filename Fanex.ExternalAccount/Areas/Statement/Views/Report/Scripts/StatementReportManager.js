
var StatementReportManager = function () {
    var self = this;
    self.Enums = {
        RoleId: {
            Member: 1
        },
        TransactionType: {
            WinLoss: 1,
            VoidCancelled: 2,
            Outstanding: 3,
            GrandPrize: 4
        }
    };
    self.Constants = {
        Format: {
            Number: {
                TwoDecimal: "0,0.00"
            }
        }
    };
    self.eventFunction = {
        completeRenderStatementReport: null,
        openPopup: null
    };

    self.cachedData = {
        statementReportCriteria: null,
        statementReport: null,
        viewBetListFunction: null
    };
    self.initFunction = function (completeRenderStatementReport, openPopup, viewBetListFunction) {
        self.eventFunction.completeRenderStatementReport = completeRenderStatementReport;
        self.eventFunction.openPopup = openPopup;
        self.eventFunction.viewBetListFunction = viewBetListFunction;
    };

    self.checkCriteria = function (criteria) {
        var newLayoutFromDate = new Date(newLayoutDate);
        if (new Date(criteria.fromDate) < newLayoutFromDate &&
            newLayoutFromDate <= new Date(criteria.toDate)) {
            alert(statementResources.Invalid_Date_Range);
            return false;
        }

        return true;
    };
    self.callAjax = function (url, criteria, successFunction) {
        jQuery.ajax({
            type: "POST",
            url: url,
            data: jQuery.param(criteria),
            responseType: "json",
            success: function (rs) {
                successFunction(rs);
            },
            error: function (jqXhr, event, ajaxSettings) {
                if (jqXhr.status && jqXhr.status == 500) {
                    var r = jQuery.parseJSON(jqXhr.responseText);
                    alert(r.Message);
                }

                if (self.eventFunction.unblockUI) {
                    self.eventFunction.unblockUI();
                }
            }
        });
    }
    self.IsUseNewLayout = function (criteria) {
        var newLayoutFromDate = new Date(newLayoutDate);
        return newLayoutFromDate <= new Date(criteria.fromDate);
    };
    self.getStatementReport = function (criteria) {
        self.cachedData.statementReportCriteria = criteria;
        var url = domain + 'NapStatement/StatementReport/GetCustomerInfo';
        if (criteria.custId == 0) {
            self.callAjax(url, criteria, self.completeLoadCustomerInfo);
            return;
        }

        if (criteria.roleId === self.Enums.RoleId.Member) {
            self.getMemberStatementReport(criteria);
        } else {
            self.getSMAStatementReport(criteria);
        }
    };

    self.completeLoadCustomerInfo = function (rs) {
        self.cachedData.statementReportCriteria.custId = rs.CustomerId;
        self.cachedData.statementReportCriteria.roleId = rs.RoleId;

        var criteria = self.cachedData.statementReportCriteria;
        if (criteria.roleId === self.Enums.RoleId.Member) {
            self.getMemberStatementReport(criteria);
        } else {
            self.getSMAStatementReport(criteria);
        }
    };

    self.getSMAStatementReport = function (criteria) {
        var url = domain + 'NapStatement/StatementReport/SMAStatement';
        self.callAjax(url, criteria, self.renderSMAStatementReport);
    };
    self.renderSMAStatementReport = function (rs) {
        self.cachedData.statementReport = rs;
        var headerContent = self.renderHeader_SMA(rs);
        var bodyContent = self.renderBody_SMA(rs);
        var footerContent = self.renderFooter(7);

        var tableReport = $("#tbl-report-new");
        tableReport.find("thead").empty().append(headerContent);
        tableReport.find("tbody").empty().append(bodyContent);
        tableReport.find("tfoot").empty().append(footerContent);
        tableReport.removeClass('hide');

        self.renderPager();

        self.bindGridEvent();
        if (self.eventFunction.completeRenderStatementReport) {
            self.eventFunction.completeRenderStatementReport();
        }
    };
    self.renderHeader_SMA = function () {
        var html = '<tr>\
                        <th class="col-150">' + statementResources.Date + '</th>\
                        <th class="col-120">' + statementResources.Opening_Balance + '</th>\
                        <th class="col-120">' + statementResources.Turnover + '</th>\
                        <th class="col-120">' + statementResources.Win_Loss + '</th>\
                        <th class="col-120">' + statementResources.Commission + '</th>\
                        <th class="col-120">' + statementResources.Transfer_Amt + '</th>\
                        <th class="col-120">' + statementResources.Accumulated_Total + '</th>\
                    </tr>';

        return html;
    },
    self.renderBody_SMA = function (rs) {
        var html = '';

        var statementListLength = rs && rs.StatementList ? rs.StatementList.length : 0;
        self.totalRows = statementListLength;

        if (statementListLength === 0) {
            html = '<tr><td colspan="7" class="no-data">There is no data</td></tr>';

            return html;
        }

        var previousStatement = null;
        for (var i = 0; i < statementListLength; i++) {
            var dataRowIndex = ' data-rowindex="' + i + '"';
            var statement = rs.StatementList[i];
            var isNotedOpeningBalance = previousStatement &&
                previousStatement.AccumulatedTotal.Value !== statement.OpeningBalance.Value;

            var balance = statement.OpeningBalance.HasData
                ? self.renderNegativeNumber(statement.OpeningBalance.Value)
                : statementResources.Symbol_NoData;

            var turnOver = statement.Turnover.HasData ? numeral(statement.Turnover.Value).format('0,0.00') : statementResources.Symbol_NoData;

            var winLoss = self.renderNegativeCell(statement.WinLoss.HasData, dataRowIndex);
            var commission = statement.Commission.HasData
                ? self.renderNegativeNumber(statement.Commission.Value.toFixed(2))
                : statementResources.Symbol_NoData;

            var accumulatedTotal = statement.AccumulatedTotal.HasData
                ? self.renderNegativeNumber(statement.AccumulatedTotal.Value)
                : statementResources.Symbol_NoData;

            html += '<tr class="hide">\
                        <td class="align-center">' + statement.StatementDateString + '</td>\
                        <td class="align-right' + (isNotedOpeningBalance ? " note-opening-balance" : "") + '">' +
                            balance +
                        '</td>\
                         <td class="align-right">' + turnOver + '</td>\
                         <td class="align-right">' + winLoss + '</td>\
                         <td class="align-right">' + commission + '</td>\
                         <td class="align-right">' + self.getColumnTransferAmount(statement, dataRowIndex) + '</td>\
                         <td class="align-right">' + accumulatedTotal + '</td>\
                     </tr>';

            previousStatement = statement;
        }

        return html;
    };
    self.renderNegativeNumber = function (value) {
        var html = numeral(value).format('0,0.00');

        return value < 0 ? '<span class="format-negative">' + html + '</span>' : html;
    };
    self.getColumnTransferAmount = function (statement, dataRowIndex) {
        var isNegative = statement.TransferAmount.HasData && parseFloat(statement.TransferAmount.Value) < 0;
        return '<div class="' +
                    (statement.TransferAmount.HasData ? 'column-transfer-amount' : '') +
                    (isNegative ? ' format-negative' : '') + '" ' + dataRowIndex + '>' +
                    (statement.TransferAmount.HasData ?
                        numeral(statement.TransferAmount.Value).format(self.Constants.Format.Number.TwoDecimal) :
                        statementResources.Symbol_NoData) +
                '</div>';
    };
    self.renderFooter = function (noColumn) {
        var html = self.totalRows ? '<tr><td colspan="' + noColumn + '"><div id="report-pager"></div></td></tr>' : '';

        return html;
    };

    self.pageSize = 50;
    self.pageIndex = 1;
    self.bodyRows = null;
    self.totalRows = 0;
    self.renderPager = function () {
        self.bodyRows = $("#tbl-report-new > tbody > tr");

        $("#report-pager").pagination({
            pageList: [10, 50, 100, 500],
            total: self.totalRows,
            pageSize: self.pageSize,
            pageNumber: self.pageIndex,

            showRefresh: false,
            onSelectPage: self.onSelectPage,
            beforePageText: "Page",
            afterPageTex: "of {pages}",
            displayMsg: "Displaying {from} to {to} of {total} items"
        });

        self.showBodyRows();
    };
    self.onSelectPage = function (pageNumber, pageSize) {
        self.pageSize = pageSize;
        self.pageIndex = pageNumber;
        self.showBodyRows();
    };
    self.showBodyRows = function () {
        if (!self.totalRows) {
            return;
        }

        var startIndex = self.pageSize * (self.pageIndex - 1);
        var endIndex = startIndex + self.pageSize;

        self.bodyRows.addClass("hide");
        for (var i = startIndex; i < endIndex && i < self.totalRows; i++) {
            $(self.bodyRows[i]).removeClass("hide");
        }
    };

    self.bindGridEvent = function () {
        var reportGrid = $('#tbl-report-new');
        reportGrid.find(".column-transfer-amount").off("click").on("click", function () {
            var rowIndex = parseInt(jQuery(this).attr("data-rowindex"));
            self.viewTransferDetails(rowIndex);
        });
    };

    self.getMemberStatementReport = function (criteria) {
        self.cachedData.statementReportCriteria = criteria;
        var url = domain + 'NapStatement/StatementReport/MemberStatement';
        self.callAjax(url, criteria, self.renderMemberStatementReport);
    };
    self.renderMemberStatementReport = function (rs) {
        self.cachedData.statementReport = rs;
        var headerContent = self.renderHeader_Member(rs);
        var bodyContent = self.renderBody_Member(rs);
        var footerContent = self.renderFooter(10);

        var tableReport = $("#tbl-report-new");
        tableReport.find("thead").empty().append(headerContent);
        tableReport.find("tbody").empty().append(bodyContent);
        tableReport.find("tfoot").empty().append(footerContent);
        tableReport.removeClass('hide');

        self.renderPager();

        self.bindGridEvent();
        if (self.eventFunction.completeRenderStatementReport) {
            self.eventFunction.completeRenderStatementReport();
        }
    };
    self.renderHeader_Member = function () {
        var html = '<tr>\
                        <th class="col-150">' + statementResources.Date + '</th>\
                        <th class="col-120">' + statementResources.Opening_Balance + '</th>\
                        <th class="col-120">' + statementResources.Turnover + '</th>\
                        <th class="col-120">' + statementResources.Void_Cancelled + '</th>\
                        <th class="col-120">' + statementResources.Outstanding + '</th>\
                        <th class="col-120">' + statementResources.Win_Loss + '</th>\
                        <th class="col-120">' + statementResources.Commission + '</th>\
                        <th class="col-120">' + statementResources.GrandPrize + '</th>\
                        <th class="col-120">' + statementResources.Transfer_Amt + '</th>\
                        <th class="col-120">' + statementResources.Accumulated_Total + '</th>\
                    </tr>';

        return html;
    },
    self.renderBody_Member = function (rs) {
        var html = '';

        var statementListLength = rs && rs.StatementList ? rs.StatementList.length : 0;
        self.totalRows = statementListLength;

        if (statementListLength === 0) {
            html = '<tr><td colspan="10" class="no-data">There is no data</td></tr>';

            return html;
        }

        var previousStatement = null;
        for (var i = 0; i < statementListLength; i++) {
            var dataRowIndex = ' data-rowindex="' + i + '"';
            var statement = rs.StatementList[i];
            var isNotedOpeningBalance = previousStatement &&
                previousStatement.AccumulatedTotal.Value !== statement.OpeningBalance.Value;

            var balance = statement.OpeningBalance.HasData
                ? self.renderNegativeNumber(statement.OpeningBalance.Value)
                : statementResources.Symbol_NoData;

            var turnOver = statement.Turnover.HasData ? numeral(statement.Turnover.Value).format('0,0.00') : statementResources.Symbol_NoData;
            var voidCancelled = self.renderNegativeCell(statement.VoidCancelled.HasData, dataRowIndex);
            var outstanding = self.renderNegativeCell(statement.Outstanding.HasData, dataRowIndex);
            var winLoss = self.renderNegativeCell(statement.WinLoss.HasData, dataRowIndex);

            var commission = statement.Commission.HasData
                ? self.renderNegativeNumber(statement.Commission.Value.toFixed(2))
                : statementResources.Symbol_NoData;
            var grandPrize = self.renderNegativeCell(statement.GrandPrize.HasData, dataRowIndex);

            var accumulatedTotal = statement.AccumulatedTotal.HasData
                ? self.renderNegativeNumber(statement.AccumulatedTotal.Value)
                : statementResources.Symbol_NoData;

            html += '<tr class="hide">\
                        <td class="align-center">' + statement.StatementDateString + '</td>\
                        <td class="align-right' + (isNotedOpeningBalance ? " note-opening-balance" : "") + '">' +
                            balance +
                        '</td>\
                         <td class="align-right">' + turnOver + '</td>\
                         <td class="align-right">' + voidCancelled + '</td>\
                         <td class="align-right">' + outstanding + '</td>\
                         <td class="align-right">' + winLoss + '</td>\
                         <td class="align-right">' + commission + '</td>\
                         <td class="align-right">' + grandPrize + '</td>\
                         <td class="align-right">' + self.getColumnTransferAmount(statement, dataRowIndex) + '</td>\
                         <td class="align-right">' + accumulatedTotal + '</td>\
                     </tr>';

            previousStatement = statement;
        }

        return html;
    };
    self.renderNegativeCell = function (value, dataRowIndex) {
        var result = value.HasData
            ? ('<div class="' + (value.Value < 0 ? 'format-negative' : '') +
                '"' + dataRowIndex + '>' +
                numeral(value.Value).format(self.Constants.Format.Number.TwoDecimal) +
                '</div>')
            : statementResources.Symbol_NoData;

        return result;
    };
    self.viewTransferDetails = function (rowIndex) {
        var statement = self.cachedData.statementReport.StatementList[rowIndex];
        var url = domain + 'NapStatement/StatementReport/TransferDetails';
        var criteria = {
            CustId: self.cachedData.statementReportCriteria.custId,
            RoleId: self.cachedData.statementReportCriteria.roleId,
            TransferDate: statement.StatementDateString,
            Username: self.cachedData.statementReportCriteria.username,
            BalanceUpDate: balanceUpDay,
            SiteName: ''
        };

        if (self.eventFunction.openPopup) {
            self.eventFunction.openPopup({
                url: url + "?" + jQuery.param(criteria),
                title: statementResources.Transfer_Details + " (" + self.cachedData.statementReportCriteria.username + ")",
                left: 400,
                top: 100,
                width: 800,
                height: 600
            });
        }
    };
    self.viewBetList = function (rowIndex, transactionType) {
        var statement = self.cachedData.statementReport.StatementList[rowIndex];
        if (self.eventFunction.viewBetListFunction) {
            self.eventFunction.viewBetListFunction({
                title: statementResources.Bet_List,
                left: 400,
                top: 100,
                width: 900,
                height: 600,
                custId: self.cachedData.statementReportCriteria.custId,
                statementDate: statement.StatementDateString,
                transactionType: transactionType
            });
        }
    };

    self.exportStatementReport = function (criteria) {
        var url = domain;
        if (criteria.roleId === self.Enums.RoleId.Member) {
            url += 'NapStatement/StatementReport/ExportMemberStatement?';
        } else {
            url += 'NapStatement/StatementReport/ExportSMAStatement?';
        }

        url += jQuery.param(criteria);
        jQuery('#frm').attr('src', url);
    };
};
var statementReportManager = new StatementReportManager();