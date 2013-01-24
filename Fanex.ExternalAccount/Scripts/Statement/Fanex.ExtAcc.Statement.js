/// <reference path="../_references.js" />
if (typeof Fanex == 'undefined' || typeof Fanex.ExtAcc == 'undefined') {
    alert("Fanex.ExtAcc.js module is required!")
}

Fanex.ExtAcc.Statement = {
    groupName: '',
    custId: 1539009,
    fromdate: new Date(),
    todate: new Date(),
    url: resolveClientUrl('Statement/GetStatement'),
    dataSource: [],
    $this: null,
    SetRange: function () {
        return {
            minDate: new Date('01/01/2010'),
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
            onClose: function (date) {
            }, showOn: 'both', buttonImageOnly: true, buttonImage: '/Images/calendar.png', buttonText: '', beforeShow: $this.SetRange, dateFormat: 'M dd, yy'
        });
        $("#dtpFromDate").val($.datepicker.formatDate('M dd, yy', new Date($('#MaxDate').val())));

        $("#dtpToDate").datepicker({
            onClose: function (date) {
            }, showOn: 'both', buttonImageOnly: true, buttonImage: '/Images/calendar.png', buttonText: '', beforeShow: $this.SetRange, dateFormat: 'M dd, yy'
        });
        $("#dtpToDate").val($.datepicker.formatDate('M dd, yy', new Date($('#MaxDate').val())));

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

        $('#exporttoexcel').click(function () {
            var params = "fromdate=" + $this.fromdate + "&todate=" + $this.todate + "&viewlevel=" + $this.viewlevel + '&custid=' + $this.custId + '&groupname=' + $this.groupName + '&username=' + $this.username;
            document.location.href = resolveClientUrl("Statement/GetExportToExcel?") + params;
        });

        $this.groupName = $('#selectedGroupName').html();

        Fanex.ExtAcc.OnResize = function () {
            try {
                $("#jqgrid").fanexGrid("resizeGrid");
            }
            catch (err) { }
        }
    },
    RenderUsernameColumn: function (cell) {
        if (cell == null || cell.Value.Username == 'Grand Total' || $this.viewlevel < 2) {
            return '<div style="line-height: 22px; text-align: left; padding-left: 3px">' + cell.Value.Username + '</div>';
        }
        else {
            var func = "javascript: Fanex.ExtAcc.Statement.GetDownlineStatement('" + cell.Value.CustId + "')";
            return html = '<div style="line-height: 22px; text-align: left;"><a style="text-decoration: none;margin:0" href="' + func + '">' + cell.Value.Username + '</a></div>';
        }
    },
    defaultColumns: [
        { HeaderText: 'Date', Name: 'Date', Width: 100, HeaderAlign: "center", CellAlign: 'center' },
        { HeaderText: 'Remark', Name: 'Remark', Width: 500, HeaderAlign: "center" },
        { HeaderText: 'Total', Name: 'TotalAmt', Width: 140, HeaderAlign: "center", CellAlign: 'right', FormatNumber: '0,0.00', FormatNegative: '$' },
        { HeaderText: 'Accumulated Total', Name: 'AccumulatedTotal', Width: 140, HeaderAlign: "center", CellAlign: 'right', FormatNumber: '0,0.00', FormatNegative: '$', AllowTruncate: false }
        ],
    Render: function () {
        var gridOption = {
            columns: this.defaultColumns,
            numberOfFrozenColumn: 3,
            showFooter: true,
            gridExpandHeight: 120,
            showPager: true,
            pagerOption: {
                itemsPerPageArray: [100, 300, 500],
                currentPage: 0,
                totalItem: $this.totalRows,
                itemsPerPage: 100
            },
            marginRight: 2,
            bottomBorderWidth: 0,
            showDistinctRow: false
        };

        //// Assing BodyData
        var bodyData = [];
        var accumulatedTotal = 0;
        for (var i = 0; i < this.totalRows; i++) {
            if (this.dataSource[i].TransType.toUpperCase() == 'BAL' && this.dataSource[i].TransDesc.toUpperCase() != 'ACCUMULATED BALANCE') {
                if (accumulatedTotal - this.dataSource[i].TotalAmt < 0.00001 || i == 0) {
                    accumulatedTotal = "<span class='StatementOK'>OK</span>";
                }
                else {
                    accumulatedTotal = accumulatedTotal - this.dataSource[i].TotalAmt;
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
                { Name: "AccumulatedTotal", Value: accumulatedTotal }
            ]
            })

            if (isNaN(accumulatedTotal)) {
                accumulatedTotal = this.dataSource[i].TotalAmt;
            }
        }

        gridOption.bodyRows = bodyData;
        gridOption.pagerOption.totalItem = this.totalRows;
        return gridOption;
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

        this.GetFromDateToDate();
        this.groupName = $('#selectedGroupName').html();
        var parameterUrl = "?groupname=" + this.groupName
                            + "&custid=" + this.custId
                            + "&fromdate=" + this.fromdate
                            + "&todate=" + this.todate;

        $.blockUI({ message: '<img src="../../Images/Loading.gif" />' });

        $.ajax({
            context: $this,
            url: this.url + parameterUrl,
            type: 'GET',
            dataType: 'json',
            cache: false,
            beforesend: function (xhr) { },
            success: function (data) {
                $this.dataSource = data.rows;
                $this.totalRows = data.rows.length;

                $('#mainGrid').css('visibility', $this.totalRows < 1 ? 'hidden' : 'visible');
                $('#headlineDaily').css('display', $this.totalRows < 1 ? 'none' : 'block');
                $('#exporttoexcel').css('display', $this.totalRows < 1 ? 'none' : 'block');
                $('#msgNoinfo').css('display', $this.totalRows < 1 ? 'block' : 'none');

                if ($this.totalRows > 0) {
                    $("#jqgrid").fanexGrid($this.Render());
                }

                $(document).ajaxStop($.unblockUI);
            },
            error: function () {
                $(document).ajaxStop($.unblockUI);
            }
        })
    }
};