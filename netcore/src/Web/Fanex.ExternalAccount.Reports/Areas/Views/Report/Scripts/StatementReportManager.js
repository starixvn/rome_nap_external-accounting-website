
var StatementReportManager = function () {
    var self = this;
    self.landscapeHeight = 260;
    self.portraitHeight = 500;
    self.shouldCalculatePopupHeight = true;
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
        viewBetListFunction: null,
        hasPermission: true
    };
    self.initFunction = function (completeRenderStatementReport, openPopup, viewBetListFunction) {
        self.eventFunction.completeRenderStatementReport = completeRenderStatementReport;
        self.eventFunction.openPopup = openPopup;
        self.eventFunction.viewBetListFunction = viewBetListFunction;
    };

    self.getPopupHeight = function () {
        var height = isMobile.phone && window.matchMedia("(orientation: landscape)").matches ?
            self.landscapeHeight :
            self.portraitHeight;

        return height;
    }
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

        if (self.cachedData.statementReportCriteria.custId == 0) {
            self.cachedData.hasPermission = false;
            self.showAccessDenied();
            return;
        }

        var criteria = self.cachedData.statementReportCriteria;
        self.cachedData.hasPermission = true;
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
        var statementListLength = rs && rs.StatementList ? rs.StatementList.length : 0;
        var bodyData = [];
        if (statementListLength) {
            var previousStatement = null;
            for (var i = 0; i < statementListLength; i++) {
                var dataRowIndex = ' data-rowindex="' + i + '"';
                var statement = rs.StatementList[i];
                var currentRow = {
                    Columns: [{ Name: "ColDate", Value: statement.StatementDateString },
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
                HeaderText: statementResources.Transfer_Amt,
                Width: 120,
                Name: "ColTransferAmount",
                HeaderAlign: "center",
                CellAlign: "right"
            },
            {
                HeaderText: statementResources.Accumulated_Total,
                Width: 140,
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
            autoCalculateGridExpandHeight: true,
            showHighlightRow: false,
            showPager: true,
            customPager: false,
            iOSLastColumnPaddingRight: iOSversion() === 12 ? 0 : 15,
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

        self.removeAccessDenied();
    };

    self.showAccessDenied = function () {
        var elem = $('<div class="errMsg"/>')
            .html('You don\'t have permission to access.')
            .width($('#GridContainer').width())
            .css('margin-top', 5);
        $('.errMsg').remove();
        $('#mainGrid').append(elem);
        $('#exporttoexcel').hide();
        $.unblockUI();
    };

    self.removeAccessDenied = function () {
        $('.errMsg').remove();
        $('#exporttoexcel').show();
        $.unblockUI();
    };

    self.getColumnTransferAmount = function (statement, rowIndex) {
        var isNegative = statement.TransferAmount.HasData && parseFloat(statement.TransferAmount.Value) < 0;
        return '<div class="' +
            (statement.TransferAmount.HasData ? 'column-transfer-amount' : '') +
            (isNegative ? ' format-negative' : '') + '" ' +
            (' data-rowindex="' + rowIndex + '"') + '>' +
            (statement.TransferAmount.HasData ?
                numeral(statement.TransferAmount.Value).format(self.Constants.Format.Number.TwoDecimal) :
                statementResources.Symbol_NoData) +
            '</div>';
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
            autoCalculateGridExpandHeight: true,
            showHighlightRow: false,
            showPager: true,
            customPager: false,
            iOSLastColumnPaddingRight: iOSversion() === 12 ? 0 : 15,
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

        self.removeAccessDenied();
    };

    self.bindGridEvent = function () {
        var reportGrid = jQuery('#GridContainer');
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
        self.portraitHeight = 600;
        if (self.eventFunction.openPopup) {
            self.eventFunction.openPopup({
                url: url + "?" + jQuery.param(criteria),
                title: statementResources.Transfer_Details + " (" + self.cachedData.statementReportCriteria.username + ")",
                left: 400,
                top: 100,
                width: 800,
                height: self.getPopupHeight()
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
        if (!self.cachedData.hasPermission) {
            self.showAccessDenied();
            return;
        }

        var url = domain;
        if (criteria.roleId === self.Enums.RoleId.Member) {
            url += 'NapStatement/StatementReport/ExportMemberStatement?';
        } else {
            url += 'NapStatement/StatementReport/ExportSMAStatement?';
        }

        url += jQuery.param(criteria);

        return url;
    };

    function iOSversion() {
        if (/iP(hone|od|ad)/.test(navigator.platform)) {
            // supports iOS 2.0 and later: <http://bit.ly/TJjs1V>
            var v = (navigator.appVersion).match(/OS (\d+)_(\d+)_?(\d+)?/);
            return [parseInt(v[1], 10), parseInt(v[2], 10), parseInt(v[3] || 0, 10)];
        }
    }
};
var statementReportManager = new StatementReportManager();