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
    url: resolveClientUrl('Statement/GetStatement'),
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

        // $("#dtpFromDate").datepicker("option", "minDate", new Date(newLayoutDate));
        // $("#dtpToDate").datepicker("option", "minDate", new Date(newLayoutDate));

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
                fromDate: moment(new Date($this.fromdate)).format("MM DD, YYYY"),
                toDate: moment(new Date($this.todate)).format("MM DD, YYYY"),
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
                    var url = domain + 'NapStatement/StatementReport/GetCustomerInfo';
                    statementReportManager.callAjax(url, criteria, function (rs) {
                        statementReportCriteria.custId = rs.CustomerId;
                        statementReportCriteria.roleId = rs.RoleId;
                        statementReportManager.exportStatementReport(statementReportCriteria);
                    });
                    return;
                }
            }

            var params = "fromdate=" + $this.fromdate + "&todate=" + $this.todate + '&custid=' + $this.custId + '&groupname=' + $this.groupName + '&username=' + $this.username;
            document.location.href = resolveClientUrl("Statement/GetExportToExcel?") + params;
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
                    $this.renderReport();
                }

                Fanex.ExtAcc.UnBlockUI();
            },
            error: function () {
                Fanex.ExtAcc.UnBlockUI();
            }
        });
    },

    renderReport: function () {
        var bodyContent = $this.renderBody();
        var footerContent = $this.renderFooter();

        $("#tbl-report-old tbody").empty().append(bodyContent);
        $("#tbl-report-old tfoot").empty().append(footerContent);
        $("#tbl-report-old").removeClass('hide');

        $this.renderPager();
    },
    renderBody: function () {
        var html = '';

        var accumulatedTotal = 0;
        var balanceResult;

        for (var i = 0; i < this.totalRows; i++) {
            var row = this.dataSource[i];

            if (row.TransType.toUpperCase() === 'BAL' &&
                row.TransDesc.toUpperCase() !== 'ACCUMULATED BALANCE') {
                balanceResult = $this.getBalanceResult(row, accumulatedTotal, i);
                accumulatedTotal = row.TotalAmt;
            }
            else {
                accumulatedTotal += row.TotalAmt;
                balanceResult = "";
            }

            html += '<tr class="hide">\
                        <td class="align-center">' + row.AccDate + '</td>\
                        <td>' +
                            '<b>' +
                                row.TransType +
                                '<br/> - ' + row.TransDesc +
                            '</b>' +
                        '</td>\
                        <td class="align-right">' + $this.renderNegativeNumber(row.TotalAmt) + '</td>\
                        <td class="align-right">' +
                            (balanceResult !== "" ? balanceResult : $this.renderNegativeNumber(accumulatedTotal)) +
                        '</td>\
                     </tr>';
        }

        return html;
    },
    getBalanceResult: function (row, accumulatedTotal, i) {
        if (Math.abs(accumulatedTotal - row.TotalAmt) < 0.00001 || i === 0) {
            return "<span class='StatementOK'>OK</span>";
        }

        return "NOT EQUAL<br/><b>Diff: " + numeral(accumulatedTotal - row.TotalAmt).format('0,0.00') + "</b>";
    },
    renderNegativeNumber: function (value) {
        var html = numeral(value).format('0,0.00');

        return value < 0 ? '<span class="format-negative">' + html + '</span>' : html;
    },
    renderFooter: function () {
        var html = '<tr><td colspan="4"><div id="report-pager"></div></td></tr>';

        return html;
    },
    pageSize: 100,
    pageIndex: 1,
    bodyRows: null,
    renderPager: function () {
        $this.bodyRows = $("#tbl-report-old > tbody > tr");

        $("#report-pager").pagination({
            pageList: [100, 300, 500],
            total: $this.totalRows,
            pageSize: $this.pageSize,
            pageNumber: $this.pageIndex,

            showRefresh: false,
            onSelectPage: $this.onSelectPage,
            beforePageText: "Page",
            afterPageTex: "of {pages}",
            displayMsg: "Displaying {from} to {to} of {total} items"
        });

        $this.showBodyRows();
    },
    onSelectPage: function (pageNumber, pageSize) {
        $this.pageSize = pageSize;
        $this.pageIndex = pageNumber;
        $this.showBodyRows();
    },
    showBodyRows: function () {
        if (!$this.totalRows) {
            return;
        }

        var startIndex = $this.pageSize * ($this.pageIndex - 1);
        var endIndex = startIndex + $this.pageSize;

        $this.bodyRows.addClass("hide");
        for (var i = startIndex; i < endIndex && i < $this.totalRows; i++) {
            $($this.bodyRows[i]).removeClass("hide");
        }
    }
};

$(document).ready(function () {
    Fanex.ExtAcc.Statement.Init();
});