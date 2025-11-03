!function(a){var b=/iPhone/i,c=/iPod/i,d=/iPad/i,e=/(?=.*\bAndroid\b)(?=.*\bMobile\b)/i,f=/Android/i,g=/(?=.*\bAndroid\b)(?=.*\bSD4930UR\b)/i,h=/(?=.*\bAndroid\b)(?=.*\b(?:KFOT|KFTT|KFJWI|KFJWA|KFSOWI|KFTHWI|KFTHWA|KFAPWI|KFAPWA|KFARWI|KFASWI|KFSAWI|KFSAWA)\b)/i,i=/Windows Phone/i,j=/(?=.*\bWindows\b)(?=.*\bARM\b)/i,k=/BlackBerry/i,l=/BB10/i,m=/Opera Mini/i,n=/(CriOS|Chrome)(?=.*\bMobile\b)/i,o=/(?=.*\bFirefox\b)(?=.*\bMobile\b)/i,p=new RegExp("(?:Nexus 7|BNTV250|Kindle Fire|Silk|GT-P1000)","i"),q=function(a,b){return a.test(b)},r=function(a){var r=a||navigator.userAgent,s=r.split("[FBAN");if("undefined"!=typeof s[1]&&(r=s[0]),s=r.split("Twitter"),"undefined"!=typeof s[1]&&(r=s[0]),this.apple={phone:q(b,r),ipod:q(c,r),tablet:!q(b,r)&&q(d,r),device:q(b,r)||q(c,r)||q(d,r)},this.amazon={phone:q(g,r),tablet:!q(g,r)&&q(h,r),device:q(g,r)||q(h,r)},this.android={phone:q(g,r)||q(e,r),tablet:!q(g,r)&&!q(e,r)&&(q(h,r)||q(f,r)),device:q(g,r)||q(h,r)||q(e,r)||q(f,r)},this.windows={phone:q(i,r),tablet:q(j,r),device:q(i,r)||q(j,r)},this.other={blackberry:q(k,r),blackberry10:q(l,r),opera:q(m,r),firefox:q(o,r),chrome:q(n,r),device:q(k,r)||q(l,r)||q(m,r)||q(o,r)||q(n,r)},this.seven_inch=q(p,r),this.any=this.apple.device||this.android.device||this.windows.device||this.other.device||this.seven_inch,this.phone=this.apple.phone||this.android.phone||this.windows.phone,this.tablet=this.apple.tablet||this.android.tablet||this.windows.tablet,"undefined"==typeof window)return this},s=function(){var a=new r;return a.Class=r,a};"undefined"!=typeof module&&module.exports&&"undefined"==typeof window?module.exports=r:"undefined"!=typeof module&&module.exports&&"undefined"!=typeof window?module.exports=s():"function"==typeof define&&define.amd?define("isMobile",[],a.isMobile=s()):a.isMobile=s()}(this);

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
/// <reference path="../_references.js" />
if (typeof Fanex == 'undefined' || typeof Fanex.ExtAcc == 'undefined') {
    alert("Fanex.ExtAcc.js module is required!");
}

Fanex.ExtAcc.Statement = {
    groupName: '',
    custId: $('#CustId').val(),
    username: $('#username').val(),
    fromdate: new Date(),
    todate: new Date(),
    url: '',
    dataSource: [],
    $this: null,
    SetRange: function () {
        return {
            minDate: new Date(newLayoutDate),
            maxDate: new Date($('#MaxDate').val())
        }
    },
    CallSubmit: function () {
        this.groupName = $('#selectedGroupName').html();
        this.Databind();
    },
    getDayOfYear: function (date) {
        var day = date.getDate();
        var month = date.getMonth() + 1;
        var year = date.getFullYear();
        var totalDays = day + (2 * month) + parseInt(3 * (month + 1) / 5) + year + parseInt(year / 4) - parseInt(year / 100) + parseInt(year / 400) + 2;
        var dayNo = (totalDays % 7);
        if (dayNo == 0) {
            dayNo = 7;
        }
        return dayNo;
    },
    Init: function () {
        $this = this;

        $("#dtpFromDate").datepicker({
            changeYear: true,
            onClose: function (date) { },
            showOn: 'both',
            buttonImageOnly: true,
            buttonImage: resolveClientUrl('Content/Images/calendar.png'),
            buttonText: '', dateFormat: 'M dd, yy'
        });

        $("#dtpFromDate").val($.datepicker.formatDate('M dd, yy', new Date($('#MaxDate').val())));

        $("#dtpToDate").datepicker({
            changeYear: true,
            onClose: function (date) { },
            showOn: 'both',
            buttonImageOnly: true,
            buttonImage: resolveClientUrl('Content/Images/calendar.png'),
            buttonText: '', dateFormat: 'M dd, yy'
        });
        $("#dtpToDate").val($.datepicker.formatDate('M dd, yy', new Date($('#MaxDate').val())));

        $(".ui-datepicker").draggable();

        $("#dtpFromDate").datepicker("option", "minDate", new Date(newLayoutDate));
        $("#dtpToDate").datepicker("option", "minDate", new Date(newLayoutDate));

        $('#ddlGroupName').bind('change', function (event) {
            var args = event.args;
            if (args) {
                var item = args.item;
                $this.groupName = item.innerHTML;
                $this.InitShowHideControls('none');
            }
        });

        $('#Submit').click(function (e) {
            $this.CallSubmit();
        });

        $('#exporttoexcel').hide();

        $('#exporttoexcel').click(function () {
            var criteria = {
                fromDate: $this.fromdate,
                toDate: $this.todate,
                custId: 0,
                roleId: 0,
                username: _username
            };

            if (!isInternalCustomer) {
                if (!statementReportManager.checkCriteria(criteria)) {
                    parent.jQuery.unblockUI();
                    return;
                }

                if (statementReportManager.IsUseNewLayout(criteria)) {
                    statementReportCriteria = criteria;
                    statementReportCriteria.fromDate = moment(new Date($this.fromdate)).format("MM/DD/YYYY");
                    statementReportCriteria.toDate = moment(new Date($this.todate)).format("MM/DD/YYYY");

                    var url = domain + 'NapStatement/StatementReport/GetCustomerInfo';

                    var success = false;
                    var result;

                    jQuery.ajax({
                        type: "POST",
                        url: url,
                        data: jQuery.param(criteria),
                        responseType: "json",
                        async: false,
                        success: function (rs) {
                            success = true;
                            result = rs;
                        },
                        error: function (jqXhr, event, ajaxSettings) {
                            if (jqXhr.status && jqXhr.status == 500) {
                                var r = jQuery.parseJSON(jqXhr.responseText);
                                alert(r.Message);
                            }

                            if (statementReportManager.eventFunction.unblockUI) {
                                statementReportManager.eventFunction.unblockUI();
                            }
                        }
                    });

                    if (success) {
                        statementReportCriteria.custId = result.CustomerId;
                        statementReportCriteria.roleId = result.RoleId;
                        var url = statementReportManager.exportStatementReport(statementReportCriteria);
                        top.exportExcel(url, document);
                    }

                    return;
                }
            }

            var params = "fromdate=" + $this.fromdate + "&todate=" + $this.todate +
                '&custid=' + $this.custId + '&groupname=' + $this.groupName + '&username=' + $this.username;

            top.exportExcel(resolveClientUrl("Statement/GetExportToExcel?") + params, document);
        });

        var indexes = location.href.split('/');
        $this.groupName = indexes[indexes.length - 1];
        $this.Databind();

        $("#view-guidelines-icon").click(function () {
            var criteria = {
                username: _username
            };
            var url = domain + 'NapStatement/StatementReport/GetCustomerInfo';
            statementReportManager.callAjax(url, criteria, function (rs) {
                Fanex.ExtAcc.Form.ShowFrame(
                    domain + "NapStatement/StatementReport/Guideline?" +
                    $.param({
                        roleId: rs.RoleId,
                        canViewBetListStatement: false
                    }),
                    statementResources.Statement_Guidelines,
                    900,
                    400,
                    false);
            });
        });

        Fanex.ExtAcc.OnResize = $this.resizeReportGrid;
        $(".icon-filter").click($this.showHideFilter);
    },

    resizeReportGrid: function () {
        try {
            $("#jqgrid").fanexGrid("resizeGrid");
        }
        catch (err) { }
    },
    hideFilter: false,
    showHideFilter: function () {
        if ($this.hideFilter) {
            $(".icon-filter").addClass("active");
            $(".filterbox").removeClass("hide");
        } else {
            $(".icon-filter").removeClass("active");
            $(".filterbox").addClass("hide");
        }

        $this.resizeReportGrid();
        $this.hideFilter = !$this.hideFilter;
    },

    RenderUsernameColumn: function (cell) {
        if (cell == null || cell.Value.Username == 'Grand Total' || $this.viewlevel < 2) {
            return '<div style="line-height: 22px; text-align: left; padding-left: 3px">' + cell.Value.Username + '</div>';
        }

        var func = "javascript: Fanex.ExtAcc.Statement.GetDownlineStatement('" + cell.Value.CustId + "')";
        return html = '<div style="line-height: 22px; text-align: left;"><a style="text-decoration: none;margin:0" href="' + func + '">' + cell.Value.Username + '</a></div>';
    },
    defaultColumns: [
        { HeaderText: 'Date', Name: 'Date', Width: 110, HeaderAlign: "center", CellAlign: 'center' },
        { HeaderText: 'Remark', Name: 'Remark', Width: 502, HeaderAlign: "center" },
        { HeaderText: 'Total', Name: 'TotalAmt', Width: 160, HeaderAlign: "center", CellAlign: 'right', FormatNumber: '0,0.00', FormatNegative: '$' },
        { HeaderText: 'Accumulated Total', Name: 'AccumulatedTotal', Width: 160, HeaderAlign: "center", CellAlign: 'right', FormatNumber: '0,0.00', FormatNegative: '$', AllowTruncate: false }
    ],
    Render: function () {
        var gridOption = {
            columns: this.defaultColumns,
            autoCalculateGridExpandHeight: true,
            showPager: true,
            pagerOption: {
                itemsPerPageArray: [100, 300, 500],
                currentPage: 0,
                totalItem: $this.totalRows,
                itemsPerPage: 100
            },
            showDistinctRow: false,
            allowTruncate: true
        };

        //// Assing BodyData
        var bodyData = [];
        var accumulatedTotal = 0;
        var isText = false;
        var ballanceResult = "OK";
        for (var i = 0; i < this.totalRows; i++) {
            isText = false;
            if (this.dataSource[i].TransType.toUpperCase() == 'BAL' && this.dataSource[i].TransDesc.toUpperCase() != 'ACCUMULATED BALANCE') {
                isText = true;
                if (Math.abs(accumulatedTotal - this.dataSource[i].TotalAmt) < 0.00001 || i == 0) {
                    ballanceResult = "<span class='StatementOK'>OK</span>";
                }
                else {
                    accumulatedTotal = accumulatedTotal - this.dataSource[i].TotalAmt;
                    ballanceResult = "NOT EQUAL<br/><b>Diff: " + numeral(accumulatedTotal).format('0,0.00') + "</b>";
                }
            }
            else {
                accumulatedTotal = this.dataSource[i].TotalAmt + accumulatedTotal;
            }

            bodyData.push({
                Columns: [
                { Name: "Date", Value: this.dataSource[i].AccDate },
                { Name: "Remark", Value: '<b>' + this.dataSource[i].TransType + '<br/> - ' + this.dataSource[i].TransDesc + '</b>' },
                { Name: "TotalAmt", Value: this.dataSource[i].TotalAmt },
                { Name: "AccumulatedTotal", Value: isText ? ballanceResult : accumulatedTotal }
                ]
            })

            if (isText) {
                accumulatedTotal = this.dataSource[i].TotalAmt;
            }
        }

        gridOption.bodyRows = bodyData;
        gridOption.pagerOption.totalItem = this.totalRows;
        setPagerOptionOnMobile(gridOption);
        $("#jqgrid").removeClass('hidden').fanexGrid(gridOption);
    },
    GetFromDateToDate: function () {
        if ($("#GroupFromToDate").css('display') != 'none') {
            this.fromdate = $('#dtpFromDate').val();
            this.todate = $('#dtpToDate').val();
        }
        else {
            this.fromdate = $('#monPickr1').monPickr("getFromDate", "MM/dd/yyyy");
            this.todate = $('#monPickr1').monPickr("getToDate", "MM/dd/yyyy");
        }
    },
    Databind: function () {
        if (this.custId <= 0) {
            return;
        }

        Fanex.ExtAcc.BlockUI();
        this.GetFromDateToDate();
        var criteria = {
            fromDate: this.fromdate,
            toDate: this.todate,
            custId: 0,
            roleId: 0,
            balanceUpDay: balanceUpDay,
            moveBetDay: moveBetsDay,
            username: _username
        };
        statementReportManager.initFunction(
            Fanex.ExtAcc.UnBlockUI,
            function (options) {
                Fanex.ExtAcc.Form.ShowFrame(options.url,
                    options.title,
                    options.width,
                    options.height,
                    null,
                    null,
                    1,
                    null,
                    false);
            },
            function (options) {
            });

        if (!isInternalCustomer) {
            if (!statementReportManager.checkCriteria(criteria)) {
                Fanex.ExtAcc.UnBlockUI();
                return;
            }

            if (statementReportManager.IsUseNewLayout(criteria)) {
                $('#mainGrid').removeClass("hide");
                $("#tbl-report-old").addClass('hide');
                statementReportManager.getStatementReport(criteria);
                $('#exporttoexcel').css('display', 'block');
                return;
            }
        }

        $('#mainGrid').css('visibility', 'hidden');
        $("#tbl-report-new").addClass('hide');
        $("#tbl-report-old").addClass('hide');
        $('#exporttoexcel').css('display', 'none');

        var parameterUrl = "?groupname=" + this.groupName
                            + "&custid=" + this.custId
                            + "&fromdate=" + this.fromdate
                            + "&todate=" + this.todate;

        $.ajax({
            context: $this,
            url: this.url + parameterUrl,
            type: 'post',
            dataType: 'json',
            cache: false,
            beforesend: function (xhr) { },
            success: function (data) {
                if (data.errorCode) {
                    Fanex.ExtAcc.UnBlockUI();
                    Fanex.ExtAcc.Form.Alert('Error', data.errorMessage.formatErrorMessage(), 350, 150, 'alertStmt');
                    return;
                }

                $this.dataSource = data.rows;
                $this.totalRows = data.rows.length;
                //$('#StatementUsername').html(data.username);

                $('#mainGrid').css('visibility', $this.totalRows < 1 ? 'hidden' : 'visible');
                $('#mainGrid').css('display', $this.totalRows < 1 ? 'none' : 'block');
                $('#headlineDaily').css('display', $this.totalRows < 1 ? 'none' : 'block');
                $('#exporttoexcel').css('display', $this.totalRows < 1 ? 'none' : 'block');
                $('#msgNoinfo').css('display', $this.totalRows < 1 ? 'block' : 'none');

                if ($this.totalRows > 0) {
                    $this.Render();
                }

                Fanex.ExtAcc.UnBlockUI();
            }
        });
    }
};

$(document).ready(function () {
    Fanex.ExtAcc.Statement.Init();
});