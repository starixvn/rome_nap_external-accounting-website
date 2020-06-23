if (typeof Fanex === "undefined" || typeof Fanex.ExtAcc === "undefined") {
    alert("Fanex.ExtAcc.js module is required!");
}

Fanex.ExtAcc.WinLossDetailIBCBetLiveCasino = {
    groupName: "",
    fromdate: new Date(),
    todate: new Date(),
    fixedsortdata: new Array(),
    removedsortdata: new Array(),
    fixedsortlist: new Array(),
    url: resolveClientUrl("WinLossDetailIBCBetLiveCasino/GetData"),
    dataSource: [],
    $this: null,
    SetRange: function () {
        return {
            minDate: new Date($("#MinDate").val()),
            maxDate: new Date($("#MaxDate").val())
        };
    },
    SwitchCalendarType: function (showType) {
        if (showType === 1) {
            $('.from-to-date').css('display', 'none');
            $('.group-month-day').css('display', 'inline-block');
        }
        else {
            $('.from-to-date').css('display', 'inline-block');
            $('.group-month-day').css('display', 'none');
        }
    },
    CallSubmit: function () {
        this.groupName = $("#selectedGroupName").html();
        this.Databind();
    },
    initMonthlyCalendar: function () {
        $("#monPickr1").monPickr({
            today: $("#MaxDate").val(),
            minDate: "2010/1/1",
            showDateRange: true,
            fromDayId: "fromDay1",
            toDayId: "toDay1",
            log: function (msg) {
                if (typeof console !== "undefined")
                    console.warn(msg);
            }
        });

        var today = new Date($("#MaxDate").val());
        $("#monPickr1").monPickr("setMonth", today);
        $("#monPickr1").monPickr("setToDay", today.getDate());
        $("#monPickr1").monPickr("setFromDay", 1);
    },
    getDayOfYear: function (date) {
        var day = date.getDate();
        var month = date.getMonth() + 1;
        var year = date.getFullYear();
        var totalDays = day + 2 * month
            + parseInt(3 * (month + 1) / 5)
            + year + parseInt(year / 4)
            - parseInt(year / 100)
            + parseInt(year / 400) + 2;
        var dayNo = totalDays % 7;
        if (dayNo === 0) {
            dayNo = 7;
        }
        return dayNo;
    },
    Init: function () {
        $this = this;
        Fanex.ExtAcc.InitSuggestion();

        $("#DateList").combobox({
            selected: function (event, ui) {
                var itemIndex = event.target.selectedIndex;
                $("#DateList").prop("selectedIndex", itemIndex);

                var fdate = new Date($("#MaxDate").val());
                var tdate = new Date($("#MaxDate").val());
                if (itemIndex === 1) {
                    fdate = new Date(
                        tdate.getFullYear(),
                        tdate.getMonth(),
                        tdate.getDate() - 1);
                    tdate = fdate;
                } else if (itemIndex === 2) {
                    fdate = new Date(
                        tdate.getFullYear(),
                        tdate.getMonth(),
                        tdate.getDate() - $this.getDayOfYear(tdate) + 1);
                } else if (itemIndex === 3) {
                    fdate = new Date(
                        tdate.getFullYear(),
                        tdate.getMonth(),
                        tdate.getDate() - $this.getDayOfYear(tdate) - 6);
                    tdate = new Date(
                        tdate.getFullYear(),
                        tdate.getMonth(),
                        tdate.getDate() - $this.getDayOfYear(tdate));
                } else if (itemIndex === 4) {
                    fdate = new Date(
                        tdate.getFullYear(),
                        tdate.getMonth(),
                        1);
                    tdate = new Date(
                        tdate.getFullYear(),
                        tdate.getMonth(),
                        tdate.getDate());
                } else if (itemIndex === 5) {
                    fdate = new Date(
                        tdate.getFullYear(),
                        tdate.getMonth() - 1,
                        1);
                    tdate = new Date(
                        tdate.getFullYear(),
                        tdate.getMonth(),
                        0);
                } else if (itemIndex === 6) {
                    fdate = new Date(
                        tdate.getFullYear(),
                        tdate.getMonth() - 1,
                        1);
                } else if (itemIndex === 7) {
                    return;
                }

                $("#dtpFromDate").val($.datepicker.formatDate("M dd, yy", fdate));
                $("#dtpToDate").val($.datepicker.formatDate("M dd, yy", tdate));
            }
        });

        $("#userGroup input").attr("readonly", "true");
        $("#userGroup input").bind("click", function () {
            $(this).autocomplete("search", "");
        });

        $("#userGroup input").val($("#DateList")[0].options[0].value);

        $("#dtpFromDate").datepicker({
            onClose: function (date) {
                $("#userGroup input").val($("#DateList")[0].options[7].value);
            },
            showOn: "both",
            buttonImageOnly: true,
            buttonImage: resolveClientUrl("Content/Images/calendar.png"),
            buttonText: "",
            beforeShow: $this.SetRange,
            dateFormat: "M dd, yy"
        });
        $("#dtpFromDate").val($.datepicker.formatDate("M dd, yy", new Date($("#MaxDate").val())));

        $("#dtpToDate").datepicker({
            onClose: function (date) {
                $("#userGroup input").val($("#DateList")[0].options[7].value);
            },
            showOn: "both",
            buttonImageOnly: true,
            buttonImage: resolveClientUrl("Content/Images/calendar.png"),
            buttonText: "",
            beforeShow: $this.SetRange,
            dateFormat: "M dd, yy"
        });
        $("#dtpToDate").val($.datepicker.formatDate("M dd, yy", new Date($("#MaxDate").val())));

        $(".ui-datepicker").draggable();

        $this.initMonthlyCalendar();

        $("#ddlGroupName").bind("change", function (event) {
            var args = event.args;
            if (args) {
                var item = args.item;
                $this.groupName = item.innerHTML;
                $this.InitShowHideControls("none");
            }
        });

        $this.groupName = $("#selectedGroupName").html();

        $("#Submit").click(function (e) {
            $this.CallSubmit();
        });

        $("#exporttoexcel").click(function () {
            var params =
                "fromdate=" + $this.fromdate +
                "&todate=" + $this.todate +
                "&groupname=" + $this.groupName;
            var url =
                resolveClientUrl("WinLossDetailIBCBetLiveCasino/ExportWinLossDetailIBCBetLiveCasinoToExcel?")
                + params;

            top.exportExcel(url, document);
        });

        $this.sorters = $(".col-sorter");
        $this.sorters.click(function () {
            var elem = $(this);
            $this.sortTable(elem);
        });

        Fanex.ExtAcc.OnResize = $this.resizeReportGrid;
        $(".icon-filter").click($this.showHideFilter);
    },

    resizeReportGrid: function () {
        try {
            $("#jqgrid").fanexGrid("resizeGrid");
        }
        catch (err) {
            console.log(err);
        }
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
        if (cell === null || cell.Value.Username === "Grand Total") {
            return '<div class="text-truncate" style="line-height: 22px; text-align: left; padding-left: 3px" title="'
                + cell.Value.Username + '">'
                + cell.Value.Username + "</div>";
        }

        return '<div class="text-truncate" style="line-height: 22px; text-align: left; text-decoration: none; margin: 0;" title="'
            + cell.Value.Username + '">'
            + cell.Value.Username + ' </div>';
    },
    CustomUsernameSortFunction: function (cell1Value, cell2Value, sortDirection) {
        if (sortDirection === 'asc') {
            return cell1Value.Username > cell2Value.Username ? 1 : -1;
        }

        return cell1Value.Username < cell2Value.Username ? 1 : -1;
    },
    defaultColumns: [
        {
            HeaderText: "Username",
            Name: "Username",
            Width: 130,
            Sort: true,
            HeaderAlign: "center",
            CustomCellRender: function (cell) {
                return $this.RenderUsernameColumn(cell);
            },
            CustomSortFunction: function (cell1Value, cell2Value, sortDirection) {
                return $this.CustomUsernameSortFunction(cell1Value, cell2Value, sortDirection);
            }
        },
        {
            HeaderText: "Bet Count",
            Name: "BetCount",
            Width: 130,
            Sort: true,
            HeaderAlign: "center",
            CellAlign: "right"
        },
        {
            HeaderText: "Turnover",
            Name: "Turnover",
            Width: 130,
            Sort: true,
            HeaderAlign: "center",
            CellAlign: "right",
            FormatNumber: "0,0.00",
            FormatNegative: "$"
        },
        {
            HeaderText: "Net Turnover",
            Name: "NetTurnover",
            Width: 130,
            Sort: true,
            HeaderAlign: "center",
            CellAlign: "right",
            FormatNumber: "0,0.00",
            FormatNegative: "$"
        },
        {
            HeaderText: "GrossComm",
            Name: "GrossComm",
            Width: 130,
            Sort: true,
            HeaderAlign: "center",
            CellAlign: "right",
            FormatNumber: "0,0.00",
            FormatNegative: "$"
        },
        {
            HeaderText: "Win/Loss",
            Name: "MemberWinlost",
            Width: 130,
            Sort: true,
            HeaderAlign: "center",
            CellAlign: "right",
            FormatNumber: "0,0.00",
            FormatNegative: "$"
        },
        {
            HeaderText: "Comm",
            Name: "PlayerComm",
            Width: 130,
            Sort: true,
            HeaderAlign: "center",
            CellAlign: "right",
            FormatNumber: "0,0.00",
            FormatNegative: "$"
        },
        {
            HeaderText: "Total",
            Name: "MemberTotal",
            Width: 130,
            Sort: true,
            HeaderAlign: "center",
            CellAlign: "right",
            FormatNumber: "0,0.00",
            FormatNegative: "$"
        },
        {
            HeaderText: "Win/Loss",
            Name: "SAgWinlost",
            Width: 130,
            Sort: true,
            HeaderAlign: "center",
            CellAlign: "right",
            FormatNumber: "0,0.00",
            FormatNegative: "$"
        },
        {
            HeaderText: "Comm",
            Name: "SAgComm",
            Width: 130,
            Sort: true,
            HeaderAlign: "center",
            CellAlign: "right",
            FormatNumber: "0,0.00",
            FormatNegative: "$"
        },
        {
            HeaderText: "Total",
            Name: "SAgTotal",
            Width: 130,
            Sort: true,
            HeaderAlign: "center",
            CellAlign: "right",
            FormatNumber: "0,0.00",
            FormatNegative: "$"
        },
        {
            HeaderText: "Win/Loss",
            Name: "ExtWinlost",
            Width: 130,
            Sort: true,
            HeaderAlign: "center",
            CellAlign: "right",
            FormatNumber: "0,0.00",
            FormatNegative: "$"
        },
        {
            HeaderText: "Comm",
            Name: "ExtComm",
            Width: 130,
            Sort: true,
            HeaderAlign: "center",
            CellAlign: "right",
            FormatNumber: "0,0.00",
            FormatNegative: "$"
        },
        {
            HeaderText: "Total",
            Name: "ExtTotal",
            Width: 130,
            Sort: true,
            HeaderAlign: "center",
            CellAlign: "right",
            FormatNumber: "0,0.00",
            FormatNegative: "$"
        },
        {
            HeaderText: "Net Company",
            Name: "NetCompany",
            Width: 130,
            Sort: true,
            HeaderAlign: "center",
            CellAlign: "right",
            FormatNumber: "0,0.00",
            FormatNegative: "$"
        }
    ],
    SAgHeaderColumns: [
        {
            HeaderText: "Username",
            Name: "Username",
            MergeRowNumber: 2
        },
        {
            HeaderText: "Bet Count",
            Name: "BetCount",
            MergeRowNumber: 2
        },
        {
            HeaderText: "Turnover",
            Name: "Turnover",
            MergeRowNumber: 2
        },
        {
            HeaderText: "Net Turnover&nbsp;<span id=\"netTurnover\" class=\"bet-type-detail-icon\"></span>",
            Name: "NetTurnover",
            MergeRowNumber: 2
        },
        {
            HeaderText: "GrossComm",
            Name: "GrossComm",
            MergeRowNumber: 2
        },
        {
            HeaderText: "Member",
            Name: "MemberWinlost",
            MergeColNumber: 3
        },
        {
            HeaderText: "Super",
            Name: "SAgWinlost",
            MergeColNumber: 3
        },
        {
            HeaderText: "Extra",
            Name: "ExtWinlost",
            MergeColNumber: 3
        },
        {
            HeaderText: "NetCompany",
            Name: "NetCompany",
            MergeRowNumber: 2
        }
    ],
    Render: function (dateInfo) {
        var arrColData = new Array();
        var arrGroupHeaders = new Array();

        arrColData = this.defaultColumns;
        arrGroupHeaders = this.SAgHeaderColumns;

        var numberOfFrozenColumn = 3;
        var widthDevice = top.$(top.window).width();
        if (widthDevice <= 768) {
            numberOfFrozenColumn = 1;
        }

        var gridOption = {
            customizeHeader: true,
            groupHeaderRows: [{ Columns: arrGroupHeaders }],
            columns: arrColData,
            numberOfFrozenColumn: numberOfFrozenColumn,
            autoCalculateGridExpandHeight: true,
            fullHeight: !isMobile.phone,
            gridHeight: isMobile.phone ? $(window).height() - 70 : 0,
            showTitle: true,
            titleInfo: {
                text: " Win Loss Detail for IBCBet Live Casino " + dateInfo
            },
            showFooter: true,
            gridExpandHeight: 256,
            showPager: true,
            pagerOption: {
                itemsPerPageArray: [50, 100, 500],
                currentPage: 0,
                totalItem: $this.totalRows,
                itemsPerPage: 50
            },
            marginRight: 10,
            bottomBorderWidth: 0,
            showDistinctRow: false,
            allowTruncate: true,
            sortOption:
            {
                afterGridSorted: function (rows) {
                    return $this.customsortfunc(rows);
                }
            },
            numOfPaddingForGridExpandHeight: 40
        };

        // For Summary Footer
        var sumBetCount = 0;
        var sumTurnover = 0;
        var sumNetTurnover = 0;
        var sumGrossComm = 0;
        var sumMemberWinlost = 0;
        var sumPlayerComm = 0;
        var sumMemberTotal = 0;
        var sumSAgWinlost = 0;
        var sumSAgComm = 0;
        var sumSAgTotal = 0;
        var sumExtWinlost = 0;
        var sumExtComm = 0;
        var sumExtTotal = 0;
        var sumNetCompany = 0;

        //// Assing BodyData
        $this.InitSort();
        var bodyData = [];
        for (var i = 0; i < this.totalRows; i++) {
            bodyData.push({
                Columns: [
                    { Name: "Username", Value: this.dataSource[i] },
                    { Name: "BetCount", Value: this.dataSource[i].BetCount },
                    { Name: "Turnover", Value: this.dataSource[i].Turnover },
                    { Name: "NetTurnover", Value: this.dataSource[i].NetTurnover },
                    { Name: "GrossComm", Value: this.dataSource[i].GrossComm },
                    { Name: "MemberWinlost", Value: this.dataSource[i].MemberWinlost },
                    { Name: "PlayerComm", Value: this.dataSource[i].PlayerComm },
                    { Name: "MemberTotal", Value: this.dataSource[i].MemberTotal },
                    { Name: "SAgWinlost", Value: this.dataSource[i].SAgWinlost },
                    { Name: "SAgComm", Value: this.dataSource[i].SAgComm },
                    { Name: "SAgTotal", Value: this.dataSource[i].SAgTotal },
                    { Name: "ExtWinlost", Value: this.dataSource[i].ExtWinlost },
                    { Name: "ExtComm", Value: this.dataSource[i].ExtComm },
                    { Name: "ExtTotal", Value: this.dataSource[i].ExtTotal },
                    { Name: "NetCompany", Value: this.dataSource[i].NetCompany }
                ]
            });

            sumBetCount += this.dataSource[i].BetCount;
            sumTurnover += this.dataSource[i].Turnover;
            sumNetTurnover += this.dataSource[i].NetTurnover;
            sumGrossComm += this.dataSource[i].GrossComm;
            sumMemberWinlost += this.dataSource[i].MemberWinlost;
            sumPlayerComm += this.dataSource[i].PlayerComm;
            sumMemberTotal += this.dataSource[i].MemberTotal;
            sumSAgWinlost += this.dataSource[i].SAgWinlost;
            sumSAgComm += this.dataSource[i].SAgComm;
            sumSAgTotal += this.dataSource[i].SAgTotal;
            sumExtWinlost += this.dataSource[i].ExtWinlost;
            sumExtComm += this.dataSource[i].ExtComm;
            sumExtTotal += this.dataSource[i].ExtTotal;
            sumNetCompany += this.dataSource[i].NetCompany;
        }

        gridOption.bodyRows = bodyData;
        gridOption.pagerOption.totalItem = this.totalRows;

        //// Asign Footer Data
        var footerData = [];
        footerData.push({
            Columns: [
                { Name: "Username", Value: "Grand Total" },
                { Name: "BetCount", Value: sumBetCount },
                { Name: "Turnover", Value: sumTurnover },
                { Name: "NetTurnover", Value: sumNetTurnover },
                { Name: "GrossComm", Value: sumGrossComm },
                { Name: "MemberWinlost", Value: sumMemberWinlost },
                { Name: "PlayerComm", Value: sumPlayerComm },
                { Name: "MemberTotal", Value: sumMemberTotal },
                { Name: "SAgWinlost", Value: sumSAgWinlost },
                { Name: "SAgComm", Value: sumSAgComm },
                { Name: "SAgTotal", Value: sumSAgTotal },
                { Name: "ExtWinlost", Value: sumExtWinlost },
                { Name: "ExtComm", Value: sumExtComm },
                { Name: "ExtTotal", Value: sumExtTotal },
                { Name: "NetCompany", Value: sumNetCompany }
            ]
        });

        gridOption.footerRows = footerData;

        setPagerOptionOnMobile(gridOption);

        $("#jqgrid").fanexGrid(gridOption);

        workaroundForGridOnPhone();
    },
    customsortfunc: function (rows) {
        var countFixedSort = $this.fixedsortlist.length;
        if (countFixedSort > 0) {
            for (var i = 0; i < countFixedSort; i++) {
                for (var j = 0; j < rows.length; j++) {
                    if ($this.fixedsortlist[i] === rows[j].Columns[0].Value.CustId) {
                        $this.fixedsortdata.push(rows.slice(j, j + 1)[0]);
                        rows.unshift(rows.splice(j, 1)[0]);
                        break;
                    }
                }
            }
        }
        return rows;
    },
    InitSort: function () {
        var countFixedSort = $this.fixedsortlist.length;
        if (countFixedSort > 0) {
            for (var i = 0; i < countFixedSort; i++) {
                for (var j = 0; j < $this.dataSource.length; j++) {
                    if ($this.fixedsortlist[i] === $this.dataSource[j].CustId) {
                        $this.fixedsortdata.push($this.dataSource.slice(j, j + 1)[0]);
                        $this.dataSource.unshift($this.dataSource.splice(j, 1)[0]);
                        break;
                    }
                }
            }
        }
    },
    ChangeHeadlineTab: function () {
        $("#headlineDaily").css("visibility", "visible");
    },
    InitShowHideControls: function (status) {
        $("#headlineDaily").css("display", status);
        $("#mainGrid").css("display", status);
    },
    GetFromDateToDate: function () {
        if ($(".from-to-date").css('display') !== 'none') {
            this.fromdate = $("#dtpFromDate").val();
            this.todate = $("#dtpToDate").val();
        }
        else {
            this.fromdate = $("#monPickr1").monPickr("getFromDate", "MM/dd/yyyy");
            this.todate = $("#monPickr1").monPickr("getToDate", "MM/dd/yyyy");
        }
    },
    Databind: function () {
        this.GetFromDateToDate();

        this.groupName = $("#selectedGroupName").html();

        var dateInfo;
        if (this.fromdate === this.todate) {
            dateInfo = 'on ' + this.fromdate;
        }
        else {
            dateInfo =
                'from '
                + (new Date(Date.parse(this.fromdate))).toStringFormat('MMM dd, yyyy')
                + ' to '
                + (new Date(Date.parse(this.todate))).toStringFormat('MMM dd, yyyy');
        }

        var parameterUrl =
            "?groupname=" + this.groupName
            + "&fromdate=" + this.fromdate
            + "&todate=" + this.todate;

        Fanex.ExtAcc.BlockUI();

        $.ajax({
            context: $this,
            url: this.url + parameterUrl,
            type: "post",
            dataType: "json",
            cache: false,
            beforesend: function (xhr) { },
            success: function (data) {
                if (data.errorCode) {
                    Fanex.ExtAcc.UnBlockUI();
                    Fanex.ExtAcc.Form.Alert("Error", data.errorMessage.formatErrorMessage(), 350, 150, "alertWLD");
                    return;
                }

                $this.fixedsortdata = new Array();
                $this.removedsortdata = new Array();

                $this.dataSource = data.rows;
                $this.totalRows = data.rows.length;
                $this.fixedsortlist = data.sortingorderlist;

                $this.ChangeHeadlineTab();

                $("#mainGrid").css("display", $this.totalRows < 1 ? "none" : "block");
                $("#mainGrid").css("visibility", $this.totalRows < 1 ? "hidden" : "visible");
                $("#headlineDaily").css("display", $this.totalRows < 1 ? "none" : "block");
                $("#headlineDaily").css("visibility", $this.totalRows < 1 ? "hidden" : "visible");
                $("#exporttoexcel").css("display", $this.totalRows < 1 ? "none" : "block");
                $("#msgNoinfo").css("display", $this.totalRows < 1 ? "block" : "none");

                if ($this.totalRows > 0) {
                    $this.Render(dateInfo);
                    $("#exporttoexcel").removeClass("hide");
                } else {
                    $("#exporttoexcel").addClass("hide");
                }

                $("#netTurnover").bind("click",
                    function () {
                        var message =
                            "- For Sportbook, Net Turnover is Turnover which excludes the draw tickets and halves of the turnover of the half won, half lost tickets.";
                        var height = 170;

                        if (data.tooltips.liveCasino !== "") {
                            message +=
                                "<br/>" +
                                "<br/>" +
                                data.tooltips.liveCasino;
                            height += 50;
                        }

                        Fanex.ExtAcc.Form.Alert("Net Turnover Information", message, 430, height);
                        return false;
                    });

                Fanex.ExtAcc.UnBlockUI();
            }
        });
    }
};

$(document).ready(function () {
    Fanex.ExtAcc.WinLossDetailIBCBetLiveCasino.Init();
});