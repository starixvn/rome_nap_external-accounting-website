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