
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
        var footerContent = self.renderFooter(rs);

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

            var winLoss = statement.WinLoss.HasData
                ? ('<div class="' + (statement.WinLoss.Value < 0 ? 'format-negative' : '') + '"' + dataRowIndex + '>' +
                        numeral(statement.WinLoss.Value).format(self.Constants.Format.Number.TwoDecimal) +
                    '</div>')
                : statementResources.Symbol_NoData;
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
    self.renderFooter = function () {
        var html = '<tr><td colspan="7"><div id="report-pager"></div></td></tr>';

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
        self.bindGridEvent();
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
        reportGrid.find(".column-transfer-amount").click(function () {
            var rowIndex = parseInt(jQuery(this).attr("data-rowindex"));
            self.viewTransferDetails(rowIndex);
        });
        reportGrid.find(".column-void-cancelled").click(function () {
            var rowIndex = parseInt(jQuery(this).attr("data-rowindex"));
            self.viewBetList(rowIndex, self.Enums.TransactionType.VoidCancelled);
        });
        reportGrid.find(".column-outstanding").click(function () {
            var rowIndex = parseInt(jQuery(this).attr("data-rowindex"));
            self.viewBetList(rowIndex, self.Enums.TransactionType.Outstanding);
        });
        reportGrid.find(".column-winloss").click(function () {
            var rowIndex = parseInt(jQuery(this).attr("data-rowindex"));
            self.viewBetList(rowIndex, self.Enums.TransactionType.WinLoss);
        });
        reportGrid.find(".column-grandprize").click(function () {
            var rowIndex = parseInt(jQuery(this).attr("data-rowindex"));
            self.viewBetList(rowIndex, self.Enums.TransactionType.GrandPrize);
        });
    };

    self.getMemberStatementReport = function (criteria) {
        self.cachedData.statementReportCriteria = criteria;
        var url = domain + 'NapStatement/StatementReport/MemberStatement';
        self.callAjax(url, criteria, self.renderMemberStatementReport);
    };
    self.renderMemberStatementReport = function (rs) {
        self.cachedData.statementReport = rs;
        var statementListLength = rs && rs.StatementList ? rs.StatementList.length : 0;
        var bodyData = [];
        if (statementListLength) {
            var previousStatement = null;
            var dataRowIndex;
            for (var i = 0; i < statementListLength; i++) {
                var dataRowIndex = ' data-rowindex="' + i + '"';
                var statement = rs.StatementList[i];
                var currentRow = {
                    Columns: [
                        {
                            Name: "ColDate",
                            Value: statement.StatementDateString
                        },
                        {
                            Name: "ColOpeningBalance",
                            Value: statement.OpeningBalance.HasData ?
                                statement.OpeningBalance.Value :
                                statementResources.Symbol_NoData,
                            TdClass: previousStatement &&
                                previousStatement.AccumulatedTotal.Value !== statement.OpeningBalance.Value ?
                                "note-opening-balance" :
                                ""
                        },
                        {
                            Name: "ColTurnover",
                            Value: statement.Turnover.HasData ?
                                statement.Turnover.Value :
                                statementResources.Symbol_NoData
                        },
                        {
                            Name: "ColVoid_Cancelled",
                            Value: statement.VoidCancelled.HasData ?
                               ('<div class="' + (statement.VoidCancelled.Value < 0 ? 'format-negative' : '') + '"' + dataRowIndex + '>' +
                                    numeral(statement.VoidCancelled.Value).format(self.Constants.Format.Number.TwoDecimal) +
                                '</div>') :
                                statementResources.Symbol_NoData
                        },
                        {
                            Name: "ColOutstanding",
                            Value: statement.Outstanding.HasData ?
                               ('<div class="' + (statement.Outstanding.Value < 0 ? 'format-negative' : '') + '"' + dataRowIndex + '>' +
                                    numeral(statement.Outstanding.Value).format(self.Constants.Format.Number.TwoDecimal) +
                                '</div>') :
                                statementResources.Symbol_NoData
                        },
                        {
                            Name: "ColWinLoss",
                            Value: statement.WinLoss.HasData ?
                               ('<div class="' + (statement.WinLoss.Value < 0 ? 'format-negative' : '') + '"' + dataRowIndex + '>' +
                                    numeral(statement.WinLoss.Value).format(self.Constants.Format.Number.TwoDecimal) +
                                '</div>') :
                                statementResources.Symbol_NoData
                        },
                        {
                            Name: "ColCommission",
                            Value: statement.Commission.HasData ?
                                statement.Commission.Value.toFixed(2) :
                                statementResources.Symbol_NoData
                        },
                        {
                            Name: "ColGrandPrize",
                            Value: statement.GrandPrize.HasData ?
                                 ('<div class="column-grandprize ' + (statement.GrandPrize.Value < 0 ? 'format-negative' : '') + '"' + dataRowIndex + '>' +
                                 numeral(statement.GrandPrize.Value).format(self.Constants.Format.Number.TwoDecimal) +
                                '</div>') :
                                statementResources.Symbol_NoData
                        },
                        {
                            Name: "ColTransferAmount",
                            Value: self.getColumnTransferAmount(statement, i)
                        },
                        {
                            Name: "ColAccumulatedTotal",
                            Value: statement.AccumulatedTotal.HasData ?
                                statement.AccumulatedTotal.Value :
                                statementResources.Symbol_NoData
                        }]
                };

                bodyData.push(currentRow);
                previousStatement = statement;
            }
        }

        var headerColumns = [
            {
                HeaderText: statementResources.Date,
                Width: 150,
                Name: "ColDate",
                HeaderAlign: "center",
                CellAlign: "center"
            },
            {
                HeaderText: statementResources.Opening_Balance,
                Width: 120,
                Name: "ColOpeningBalance",
                HeaderAlign: "center",
                CellAlign: "right",
                FormatNumber: self.Constants.Format.Number.TwoDecimal,
                FormatNegative: '$'
            },
            {
                HeaderText: statementResources.Turnover,
                Width: 120,
                Name: "ColTurnover",
                HeaderAlign: "center",
                CellAlign: "right",
                FormatNumber: self.Constants.Format.Number.TwoDecimal
            },
            {
                HeaderText: statementResources.Void_Cancelled,
                Width: 120,
                Name: "ColVoid_Cancelled",
                HeaderAlign: "center",
                CellAlign: "right",
                FormatNumber: self.Constants.Format.Number.TwoDecimal
            },
            {
                HeaderText: statementResources.Outstanding,
                Width: 120,
                Name: "ColOutstanding",
                HeaderAlign: "center",
                CellAlign: "right",
                FormatNumber: self.Constants.Format.Number.TwoDecimal
            },
            {
                HeaderText: statementResources.Win_Loss,
                Width: 120,
                Name: "ColWinLoss",
                HeaderAlign: "center",
                CellAlign: "right",
                FormatNumber: self.Constants.Format.Number.TwoDecimal,
                FormatNegative: '$'
            },
            {
                HeaderText: statementResources.Commission,
                Width: 120,
                Name: "ColCommission",
                HeaderAlign: "center",
                CellAlign: "right",
                FormatNumber: self.Constants.Format.Number.TwoDecimal,
                FormatNegative: '$'
            },
            {
                HeaderText: statementResources.GrandPrize,
                Width: 120,
                Name: "ColGrandPrize",
                HeaderAlign: "center",
                CellAlign: "right",
                FormatNumber: self.Constants.Format.Number.TwoDecimal,
                FormatNegative: '$'
            },
            {
                HeaderText: statementResources.Transfer_Amt,
                Width: 120,
                Name: "ColTransferAmount",
                HeaderAlign: "center",
                CellAlign: "right"
            },
            {
                HeaderText: statementResources.Accumulated_Total,
                Width: 120,
                Name: "ColAccumulatedTotal",
                HeaderAlign: "center",
                CellAlign: "right",
                FormatNumber: self.Constants.Format.Number.TwoDecimal,
                FormatNegative: '$'
            }];

        gridOptions = {
            columns: headerColumns,
            bodyRows: bodyData,
            headerColumnHeight: 24,
            bodyRowHeight: 25,
            numberOfFrozenColumn: 0,
            gridExpandHeight: 100,
            showHighlightRow: false,
            showPager: true,
            customPager: false,
            pagerOption: {
                itemsPerPage: 50,
                maxItemsPerPage: 500,
                itemsPerPageArray: [10, 50, 100, 500],
                currentPage: 0,
                startPerPage: 50,
                pageStep: 50,
                afterPagedFunction: function () {
                    self.bindGridEvent();
                }
            }
        };

        jQuery('#GridContainer').removeClass("hidden").fanexGrid(gridOptions);
        self.bindGridEvent();
        if (self.eventFunction.completeRenderStatementReport) {
            self.eventFunction.completeRenderStatementReport();
        }
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