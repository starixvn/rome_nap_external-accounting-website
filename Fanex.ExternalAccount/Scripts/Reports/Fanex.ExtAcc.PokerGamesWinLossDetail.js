/// <reference path="../_references.js" />
if (typeof Fanex == "undefined" || typeof Fanex.ExtAcc == "undefined") {
    alert("Fanex.ExtAcc.js module is required!")
}

Fanex.ExtAcc.PokerGamesWinLossDetail = {
    groupName: "",
    username: "",
    srecommend: 0,
    mrecommend: 0,
    recommend: 0,
    custId: 0,
    supername: "",
    mastername: "",
    agentname: "",
    fromdate: new Date(),
    todate: new Date(),
    fixedsortdata: new Array(),
    removedsortdata: new Array(),
    fixedsortlist: new Array(),
    url: resolveClientUrl("PokerGamesWinLossDetail/GetData"),
    viewlevel: 5,
    dataSource: [],
    $this: null,

    showPokerGames: false,

    SetRange: function () {
        return {
            minDate: new Date($("#MinDate").val()),
            maxDate: new Date($("#MaxDate").val())
        }
    },
    SwitchCalendarType: function (showType) {
        if (showType == 1) {
            $("#GroupViewHistory").css("display", "none");
            $("#GroupBackToCurrent").css("display", "block");
            $('.from-to-date').css('display', 'none');
            $('.group-month-day').css('display', 'inline-block');
        }
        else {
            $("#GroupViewHistory").css("display", "block");
            $("#GroupBackToCurrent").css("display", "none");
            $('.from-to-date').css('display', 'inline-block');
            $('.group-month-day').css('display', 'none');
        }
    },
    CallSubmit: function () {
        this.groupName = $("#selectedGroupName").html();
        this.ResetParameters();
        this.username = Fanex.encodeHtml($("#txtUserName").val());
        this.Databind(this.username);
    },
    initMonthlyCalendar: function () {
        $("#monPickr1").monPickr({
            today: $("#MaxDate").val(),
            minDate: "2010/1/1",
            showDateRange: true,
            fromDayId: "fromDay1",
            toDayId: "toDay1",
            log: function (msg) {
                if (typeof console != "undefined")
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
        var totalDays = day + (2 * month) + parseInt(3 * (month + 1) / 5) + year + parseInt(year / 4) - parseInt(year / 100) + parseInt(year / 400) + 2;
        var dayNo = (totalDays % 7);
        if (dayNo == 0) {
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
                if (itemIndex == 1) {
                    fdate = new Date(tdate.getFullYear(), tdate.getMonth(), tdate.getDate() - 1);
                    tdate = fdate;
                } else if (itemIndex == 2) {
                    fdate = new Date(tdate.getFullYear(), tdate.getMonth(), tdate.getDate() - $this.getDayOfYear(tdate) + 1);
                } else if (itemIndex == 3) {
                    fdate = new Date(tdate.getFullYear(), tdate.getMonth(), tdate.getDate() - $this.getDayOfYear(tdate) - 6);
                    tdate = new Date(tdate.getFullYear(), tdate.getMonth(), tdate.getDate() - $this.getDayOfYear(tdate));
                } else if (itemIndex == 4) {
                    fdate = new Date(tdate.getFullYear(), tdate.getMonth(), 1);
                    tdate = new Date(tdate.getFullYear(), tdate.getMonth(), tdate.getDate());
                } else if (itemIndex == 5) {
                    fdate = new Date(tdate.getFullYear(), tdate.getMonth() - 1, 1);
                    tdate = new Date(tdate.getFullYear(), tdate.getMonth(), 0);
                } else if (itemIndex == 6) {
                    fdate = new Date(tdate.getFullYear(), tdate.getMonth() - 1, 1);
                } else if (itemIndex == 7) {
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
            }, showOn: "both", buttonImageOnly: true, buttonImage: resolveClientUrl("Content/Images/calendar.png"), buttonText: "", beforeShow: $this.SetRange, dateFormat: "M dd, yy"
        });
        $("#dtpFromDate").val($.datepicker.formatDate("M dd, yy", new Date($("#MaxDate").val())));

        $("#dtpToDate").datepicker({
            onClose: function (date) {
                $("#userGroup input").val($("#DateList")[0].options[7].value);
            }, showOn: "both", buttonImageOnly: true, buttonImage: resolveClientUrl("Content/Images/calendar.png"), buttonText: "", beforeShow: $this.SetRange, dateFormat: "M dd, yy"
        });
        $("#dtpToDate").val($.datepicker.formatDate("M dd, yy", new Date($("#MaxDate").val())));

        $(".ui-datepicker").draggable();

        $this.initMonthlyCalendar();

        $("#ddlGroupName").bind("change", function (event) {
            var args = event.args;
            if (args) {
                var item = args.item;
                $this.groupName = item.innerHTML;

                $this.ResetParameters();
                $this.InitShowHideControls("none");
            }
        });

        $("#Submit").click(function (e) {
            $this.CallSubmit();
        });

        $("#txtUserName").keyup(function (e) {
            if ($("#txtUserName").val().length > 1) {
                Fanex.ExtAcc.InitSuggestion();
            }
        });

        $("#exporttoexcel").click(function () {
            var params = "fromdate=" + $this.fromdate + "&todate=" + $this.todate + "&viewlevel=" + $this.viewlevel +
            "&custid=" + $this.custId + "&groupname=" + $this.groupName + "&username=" + Fanex.ExtAcc.PokerGamesWinLossDetail.username;
            var url = resolveClientUrl("PokerGamesWinLossDetail/ExportPokerGamesWinLossDetailToExcel?") + params;

            top.exportExcel(url, document);
        });

        $("#ViewHistoryReport").click(function () {
            $this.SwitchCalendarType(1);
        });
        $("#BackToCurrentReport").click(function () {
            $this.SwitchCalendarType(0);
        });

        $("#tabSuper").click(function (e) {
            $this.viewlevel = 4;
            $this.srecommend = 0;
            $this.mrecommend = 0;
            $this.recommend = 0;

            if ($this.custId !== 0) {
                $this.custId = 0;
            }

            $this.ToggleTabs('none', 'none', 'none', 'none', 4);
            $this.Databind($this.supername);
        });

        $("#tabMaster").click(function (e) {
            $this.viewlevel = 4;
            $this.custId = $this.mastername === "" ? $this.srecommend : 0;
            $this.ToggleTabs('block', 'block', 'none', 'none', 3);

            $this.Databind($this.mastername);
        });

        $("#tabAgent").click(function (e) {
            $this.viewlevel = 3;
            $this.custId = $this.agentname === "" ? $this.mrecommend : 0;
            $this.ToggleTabs('block', 'block', 'block', 'none', 2);

            $this.Databind($this.agentname);
        });

        $this.groupName = $("#selectedGroupName").html();

        $("#txtUserName").keypress(function (e) {
            if (typeof e == "undefined") e = window.event;
            var key = (typeof e.keyCode == "undefined") ? e.which : e.keyCode;
            if (key == 13) {
                $this.CallSubmit();
                return true;
            } else {
                return true;
            }
        });

        $this.sorters = $(".col-sorter");
        $this.sorters.click(function () {
            var elem = $(this);
            $this.sortTable(elem);
        });

        Fanex.ExtAcc.OnResize = $this.resizeReportGrid;
        $(".icon-filter").click($this.showHideFilter);

        $("#report-notes").bind("click",
            function () {
                var message = "- Turnover of Poker Games are the buy-in/top up amount on table.<br /><br />";
                message += "- Net Turnover of Poker Games are actual amount member played on game table and Gems bought.<br /><br />";
                message += "- Gross Tax Comm is commission for whole line.<br /><br />";
                message += "- Member Win Loss excludes Insurance and Gems bought.<br /><br />";
                message += "- Member Insurance and Gems Win Loss is the win loss on Insurance and Gems bought only.";

                var height = 320;

                Fanex.ExtAcc.Form.Alert("Report Information", message, 430, height);
                return false;
            });
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
    ToggleTabs: function (levelLine, statusSAg, statusMAg, statusAg, status) {
        $("#tabSuper").css("display", statusSAg);
        $("#tabMaster").css("display", statusMAg);
        $("#tabAgent").css("display", statusAg);

        $('#levelLine').css('display', levelLine);
        if (status == 1) {
            $("#SelectedLevel").html("Member");
        }
        else if (status == 2) {
            $("#SelectedLevel").html("Agent");
        }
        else if (status == 3) {
            $("#SelectedLevel").html("Master");
        }
        else if (status == 4) {
            $("#SelectedLevel").html("Super");
        }
    },

    RenderUsernameColumn: function (cell) {
        if (cell == null || cell.Value.Username == "Grand Total" || $this.viewlevel < 2) {
            return '<div class="text-truncate" style="line-height: 22px; text-align: left; padding-left: 3px" title="' + cell.Value.Username + '">' + cell.Value.Username + "</div>";
        }

        var func = "javascript: Fanex.ExtAcc.PokerGamesWinLossDetail.GetDownlineWinLossDetail('" + cell.Value.CustId + "')";
        return html = '<div style="line-height: 22px; text-align: left;">' +
                            '<a class="text-truncate" style="text-decoration: none; margin: 0" href="' + func + '" title="' + cell.Value.Username + '">' + cell.Value.Username + '</a>' +
                        '</div>';
    },
    CustomUsernameSortFunction: function (cell1Value, cell2Value, sortDirection) {
        if (sortDirection == 'asc') {
            return cell1Value.Username > cell2Value.Username ? 1 : -1;
        }

        return cell1Value.Username < cell2Value.Username ? 1 : -1;
    },
    defaultColumns: [
        {
            HeaderText: "Username", Name: "Username", Width: 130, Sort: true, HeaderAlign: "center",
            CustomCellRender: function (cell) { return $this.RenderUsernameColumn(cell); }, CustomSortFunction: function (cell1Value, cell2Value, sortDirection) { return $this.CustomUsernameSortFunction(cell1Value, cell2Value, sortDirection) }
        },
        { HeaderText: "Turnover", Name: "Turnover", Width: 130, Sort: true, HeaderAlign: "center", CellAlign: "right", FormatNumber: "0,0.00", FormatNegative: "$" },
        { HeaderText: "Net Turnover", Name: "NetTurnover", Width: 130, Sort: true, HeaderAlign: "center", CellAlign: "right", FormatNumber: "0,0.00", FormatNegative: "$" },
        { HeaderText: "Gross Tax Comm", Name: "TaxComm", Width: 130, Sort: true, HeaderAlign: "center", CellAlign: "right", FormatNumber: "0,0.00", FormatNegative: "$" },
        { HeaderText: "Win/Loss", Name: "MemberWinLoss", Width: 130, Sort: true, HeaderAlign: "center", CellAlign: "right", FormatNumber: "0,0.00", FormatNegative: "$" },
        { HeaderText: "Insurance & Gems Win Loss", Name: "MemberInsuranceGemsWinLoss", Width: 130, Sort: true, HeaderAlign: "center", CellAlign: "right", FormatNumber: "0,0.00", FormatNegative: "$", CssClass: "col-insurance-games-win-loss" },
        { HeaderText: "Comm", Name: "MemberComm", Width: 130, Sort: true, HeaderAlign: "center", CellAlign: "right", FormatNumber: "0,0.00", FormatNegative: "$" },
        { HeaderText: "Total", Name: "MemberTotal", Width: 130, Sort: true, HeaderAlign: "center", CellAlign: "right", FormatNumber: "0,0.00", FormatNegative: "$" },
        { HeaderText: "Insurance & Gems Win Loss", Name: "AgInsuranceGemsWinLoss", Width: 130, Sort: true, HeaderAlign: "center", CellAlign: "right", FormatNumber: "0,0.00", FormatNegative: "$", CssClass: "col-insurance-games-win-loss" },
        { HeaderText: "Comm", Name: "AgComm", Width: 130, Sort: true, HeaderAlign: "center", CellAlign: "right", FormatNumber: "0,0.00", FormatNegative: "$" },
        { HeaderText: "Total", Name: "AgTotal", Width: 130, Sort: true, HeaderAlign: "center", CellAlign: "right", FormatNumber: "0,0.00", FormatNegative: "$" },
        { HeaderText: "Insurance & Gems Win Loss", Name: "MAgInsuranceGemsWinLoss", Width: 130, Sort: true, HeaderAlign: "center", CellAlign: "right", FormatNumber: "0,0.00", FormatNegative: "$", CssClass: "col-insurance-games-win-loss" },
        { HeaderText: "Comm", Name: "MAgComm", Width: 130, Sort: true, HeaderAlign: "center", CellAlign: "right", FormatNumber: "0,0.00", FormatNegative: "$" },
        { HeaderText: "Total", Name: "MAgTotal", Width: 130, Sort: true, HeaderAlign: "center", CellAlign: "right", FormatNumber: "0,0.00", FormatNegative: "$" },
        { HeaderText: "Insurance & Gems Win Loss", Name: "SAgInsuranceGemsWinLoss", Width: 130, Sort: true, HeaderAlign: "center", CellAlign: "right", FormatNumber: "0,0.00", FormatNegative: "$", CssClass: "col-insurance-games-win-loss" },
        { HeaderText: "Comm", Name: "SAgComm", Width: 130, Sort: true, HeaderAlign: "center", CellAlign: "right", FormatNumber: "0,0.00", FormatNegative: "$" },
        { HeaderText: "Total", Name: "SAgTotal", Width: 130, Sort: true, HeaderAlign: "center", CellAlign: "right", FormatNumber: "0,0.00", FormatNegative: "$" },
        { HeaderText: "Company", Name: "Company", Width: 130, Sort: true, HeaderAlign: "center", CellAlign: "right", FormatNumber: "0,0.00", FormatNegative: "$" }
    ],
    MAgColumns: [
        {
            HeaderText: "Username", Name: "Username", Width: 130,
            Sort: true, HeaderAlign: "center", CustomCellRender: function (cell) { return $this.RenderUsernameColumn(cell); }, CustomSortFunction: function (cell1Value, cell2Value, sortDirection) { return $this.CustomUsernameSortFunction(cell1Value, cell2Value, sortDirection) }
        },
        { HeaderText: "Turnover", Name: "Turnover", Width: 130, Sort: true, HeaderAlign: "center", CellAlign: "right", FormatNumber: "0,0.00", FormatNegative: "$" },
        { HeaderText: "Net Turnover", Name: "NetTurnover", Width: 130, Sort: true, HeaderAlign: "center", CellAlign: "right", FormatNumber: "0,0.00", FormatNegative: "$" },
        { HeaderText: "Gross Tax Comm", Name: "TaxComm", Width: 130, Sort: true, HeaderAlign: "center", CellAlign: "right", FormatNumber: "0,0.00", FormatNegative: "$" },
        { HeaderText: "Win/Loss", Name: "MemberWinLoss", Width: 130, Sort: true, HeaderAlign: "center", CellAlign: "right", FormatNumber: "0,0.00", FormatNegative: "$" },
        { HeaderText: "Insurance & Gems Win Loss", Name: "MemberInsuranceGemsWinLoss", Width: 130, Sort: true, HeaderAlign: "center", CellAlign: "right", FormatNumber: "0,0.00", FormatNegative: "$", CssClass: "col-insurance-games-win-loss" },
        { HeaderText: "Comm", Name: "MemberComm", Width: 130, Sort: true, HeaderAlign: "center", CellAlign: "right", FormatNumber: "0,0.00", FormatNegative: "$" },
        { HeaderText: "Total", Name: "MemberTotal", Width: 130, Sort: true, HeaderAlign: "center", CellAlign: "right", FormatNumber: "0,0.00", FormatNegative: "$" },
        { HeaderText: "Insurance & Gems Win Loss", Name: "AgInsuranceGemsWinLoss", Width: 130, Sort: true, HeaderAlign: "center", CellAlign: "right", FormatNumber: "0,0.00", FormatNegative: "$", CssClass: "col-insurance-games-win-loss" },
        { HeaderText: "Comm", Name: "AgComm", Width: 130, Sort: true, HeaderAlign: "center", CellAlign: "right", FormatNumber: "0,0.00", FormatNegative: "$" },
        { HeaderText: "Total", Name: "AgTotal", Width: 130, Sort: true, HeaderAlign: "center", CellAlign: "right", FormatNumber: "0,0.00", FormatNegative: "$" },
        { HeaderText: "Insurance & Gems Win Loss", Name: "MAgInsuranceGemsWinLoss", Width: 130, Sort: true, HeaderAlign: "center", CellAlign: "right", FormatNumber: "0,0.00", FormatNegative: "$", CssClass: "col-insurance-games-win-loss" },
        { HeaderText: "Comm", Name: "MAgComm", Width: 130, Sort: true, HeaderAlign: "center", CellAlign: "right", FormatNumber: "0,0.00", FormatNegative: "$" },
        { HeaderText: "Total", Name: "MAgTotal", Width: 130, Sort: true, HeaderAlign: "center", CellAlign: "right", FormatNumber: "0,0.00", FormatNegative: "$" },
        { HeaderText: "Company", Name: "Company", Width: 130, Sort: true, HeaderAlign: "center", CellAlign: "right", FormatNumber: "0,0.00", FormatNegative: "$" }
    ],
    AgColumns: [
        {
            HeaderText: "Username", Name: "Username", Width: 130,
            Sort: true, HeaderAlign: "center", CustomCellRender: function (cell) { return $this.RenderUsernameColumn(cell); }, CustomSortFunction: function (cell1Value, cell2Value, sortDirection) { return $this.CustomUsernameSortFunction(cell1Value, cell2Value, sortDirection) }
        },
        { HeaderText: "Turnover", Name: "Turnover", Width: 130, Sort: true, HeaderAlign: "center", CellAlign: "right", FormatNumber: "0,0.00", FormatNegative: "$" },
        { HeaderText: "Net Turnover", Name: "NetTurnover", Width: 130, Sort: true, HeaderAlign: "center", CellAlign: "right", FormatNumber: "0,0.00", FormatNegative: "$" },
        { HeaderText: "Gross Tax Comm", Name: "TaxComm", Width: 130, Sort: true, HeaderAlign: "center", CellAlign: "right", FormatNumber: "0,0.00", FormatNegative: "$" },
        { HeaderText: "Win/Loss", Name: "MemberWinLoss", Width: 130, Sort: true, HeaderAlign: "center", CellAlign: "right", FormatNumber: "0,0.00", FormatNegative: "$" },
        { HeaderText: "Insurance & Gems Win Loss", Name: "MemberInsuranceGemsWinLoss", Width: 130, Sort: true, HeaderAlign: "center", CellAlign: "right", FormatNumber: "0,0.00", FormatNegative: "$", CssClass: "col-insurance-games-win-loss" },
        { HeaderText: "Comm", Name: "MemberComm", Width: 130, Sort: true, HeaderAlign: "center", CellAlign: "right", FormatNumber: "0,0.00", FormatNegative: "$" },
        { HeaderText: "Total", Name: "MemberTotal", Width: 130, Sort: true, HeaderAlign: "center", CellAlign: "right", FormatNumber: "0,0.00", FormatNegative: "$" },
        { HeaderText: "Insurance & Gems Win Loss", Name: "AgInsuranceGemsWinLoss", Width: 130, Sort: true, HeaderAlign: "center", CellAlign: "right", FormatNumber: "0,0.00", FormatNegative: "$", CssClass: "col-insurance-games-win-loss" },
        { HeaderText: "Comm", Name: "AgComm", Width: 130, Sort: true, HeaderAlign: "center", CellAlign: "right", FormatNumber: "0,0.00", FormatNegative: "$" },
        { HeaderText: "Total", Name: "AgTotal", Width: 130, Sort: true, HeaderAlign: "center", CellAlign: "right", FormatNumber: "0,0.00", FormatNegative: "$" },
        { HeaderText: "Company", Name: "Company", Width: 130, Sort: true, HeaderAlign: "center", CellAlign: "right", FormatNumber: "0,0.00", FormatNegative: "$" }
    ],
    SAgHeaderColumns: [
        { HeaderText: "Username", Name: "Username", MergeRowNumber: 3 },
        { HeaderText: "Turnover", Name: "Turnover", MergeRowNumber: 3 },
        { HeaderText: "Net Turnover", Name: "NetTurnover", MergeRowNumber: 3 },
        { HeaderText: "Gross Tax Comm", Name: "TaxComm", MergeRowNumber: 3 },
        { HeaderText: "Member", Name: "MemberWinLoss", MergeColNumber: 4 },
        { HeaderText: "Agent", Name: "AgInsuranceGemsWinLoss", MergeColNumber: 3 },
        { HeaderText: "Master", Name: "MAgInsuranceGemsWinLoss", MergeColNumber: 3 },
        { HeaderText: "Super", Name: "SAgInsuranceGemsWinLoss", MergeColNumber: 3 },
        { HeaderText: "Company", Name: "Company", MergeRowNumber: 2 }
    ],
    MAgHeaderColumns: [
        { HeaderText: "Username", Name: "Username", MergeRowNumber: 3 },
        { HeaderText: "Turnover", Name: "Turnover", MergeRowNumber: 3 },
        { HeaderText: "Net Turnover", Name: "NetTurnover", MergeRowNumber: 3 },
        { HeaderText: "Gross Tax Comm", Name: "TaxComm", MergeRowNumber: 3 },
        { HeaderText: "Member", Name: "MemberWinLoss", MergeColNumber: 4 },
        { HeaderText: "Agent", Name: "AgInsuranceGemsWinLoss", MergeColNumber: 3 },
        { HeaderText: "Master", Name: "MAgInsuranceGemsWinLoss", MergeColNumber: 3 },
        { HeaderText: "Company", Name: "Company", MergeRowNumber: 2 }
    ],
    AgHeaderColumns: [
        { HeaderText: "Username", Name: "Username", MergeRowNumber: 3 },
        { HeaderText: "Turnover", Name: "Turnover", MergeRowNumber: 3 },
        { HeaderText: "Net Turnover", Name: "NetTurnover", MergeRowNumber: 3 },
        { HeaderText: "Gross Tax Comm", Name: "TaxComm", MergeRowNumber: 3 },
        { HeaderText: "Member", Name: "MemberWinLoss", MergeColNumber: 4 },
        { HeaderText: "Agent", Name: "AgInsuranceGemsWinLoss", MergeColNumber: 3 },
        { HeaderText: "Company", Name: "Company", MergeRowNumber: 2 }
    ],
    Render: function (dateInfo) {
        var arrColData = new Array();
        var arrGroupHeaders = new Array();
        var arrGroupFooters = new Array();

        if (this.viewlevel == 1) {
            arrColData = this.AgColumns;
            arrGroupHeaders = this.AgHeaderColumns;
            arrGroupFooters = this.AgFooterColumns;
        } else if (this.viewlevel == 2) {
            arrColData = this.MAgColumns;
            arrGroupHeaders = this.MAgHeaderColumns;
            arrGroupFooters = this.MAgFooterColumns;
        } else {
            arrColData = this.defaultColumns;
            arrGroupHeaders = this.SAgHeaderColumns;
            arrGroupFooters = this.SAgFooterColumns;
        }

        var level = $this.viewlevel == 4 ? "Super" :
             $this.viewlevel == 3 ? "Master" :
              $this.viewlevel == 2 ? "Agent" :
              "Member";

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
                text: level + " Poker Games Win Loss " + dateInfo
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
        var sumTurnover = 0;
        var sumNetTurnover = 0;
        var sumTaxComm = 0;
        var sumMemberWinLoss = 0;
        var sumMemberInsuranceGemsWinLoss = 0;
        var sumMemberComm = 0;
        var sumMemberTotal = 0;
        var sumAgInsuranceGemsWinLoss = 0;
        var sumAgComm = 0;
        var sumAgTotal = 0;
        var sumMAgInsuranceGemsWinLoss = 0;
        var sumMAgComm = 0;
        var sumMAgTotal = 0;
        var sumSAgInsuranceGemsWinLoss = 0;
        var sumSAgComm = 0;
        var sumSAgTotal = 0;
        var sumCompany = 0;

        //// Assing BodyData
        $this.InitSort();
        var bodyData = [];
        for (var i = 0; i < this.totalRows; i++) {
            if (this.viewlevel == 1) {
                bodyData.push({
                    Columns: [
                    { Name: "Username", Value: this.dataSource[i] },
                    { Name: "Turnover", Value: this.dataSource[i].Turnover },
                    { Name: "NetTurnover", Value: this.dataSource[i].NetTurnover },
                    { Name: "TaxComm", Value: this.dataSource[i].TaxComm },
                    { Name: "MemberWinLoss", Value: this.dataSource[i].MemberWinLoss },
                    { Name: "MemberInsuranceGemsWinLoss", Value: this.dataSource[i].MemberInsuranceGemsWinLoss },
                    { Name: "MemberComm", Value: this.dataSource[i].MemberComm },
                    { Name: "MemberTotal", Value: this.dataSource[i].MemberTotal },
                    { Name: "AgInsuranceGemsWinLoss", Value: this.dataSource[i].AgInsuranceGemsWinLoss },
                    { Name: "AgComm", Value: this.dataSource[i].AgComm },
                    { Name: "AgTotal", Value: this.dataSource[i].AgTotal },
                    { Name: "Company", Value: this.dataSource[i].Company }
                    ]
                })
            } else if (this.viewlevel == 2) {
                bodyData.push({
                    Columns: [
                    { Name: "Username", Value: this.dataSource[i] },
                    { Name: "Turnover", Value: this.dataSource[i].Turnover },
                    { Name: "NetTurnover", Value: this.dataSource[i].NetTurnover },
                    { Name: "TaxComm", Value: this.dataSource[i].TaxComm },
                    { Name: "MemberWinLoss", Value: this.dataSource[i].MemberWinLoss },
                    { Name: "MemberInsuranceGemsWinLoss", Value: this.dataSource[i].MemberInsuranceGemsWinLoss },
                    { Name: "MemberComm", Value: this.dataSource[i].MemberComm },
                    { Name: "MemberTotal", Value: this.dataSource[i].MemberTotal },
                    { Name: "AgInsuranceGemsWinLoss", Value: this.dataSource[i].AgInsuranceGemsWinLoss },
                    { Name: "AgComm", Value: this.dataSource[i].AgComm },
                    { Name: "AgTotal", Value: this.dataSource[i].AgTotal },
                    { Name: "MAgInsuranceGemsWinLoss", Value: this.dataSource[i].MAgInsuranceGemsWinLoss },
                    { Name: "MAgComm", Value: this.dataSource[i].MAgComm },
                    { Name: "MAgTotal", Value: this.dataSource[i].MAgTotal },
                    { Name: "Company", Value: this.dataSource[i].Company }
                    ]
                })
            } else {
                bodyData.push({
                    Columns: [
                    { Name: "Username", Value: this.dataSource[i] },
                    { Name: "Turnover", Value: this.dataSource[i].Turnover },
                    { Name: "NetTurnover", Value: this.dataSource[i].NetTurnover },
                    { Name: "TaxComm", Value: this.dataSource[i].TaxComm },
                    { Name: "MemberWinLoss", Value: this.dataSource[i].MemberWinLoss },
                    { Name: "MemberInsuranceGemsWinLoss", Value: this.dataSource[i].MemberInsuranceGemsWinLoss },
                    { Name: "MemberComm", Value: this.dataSource[i].MemberComm },
                    { Name: "MemberTotal", Value: this.dataSource[i].MemberTotal },
                    { Name: "AgInsuranceGemsWinLoss", Value: this.dataSource[i].AgInsuranceGemsWinLoss },
                    { Name: "AgComm", Value: this.dataSource[i].AgComm },
                    { Name: "AgTotal", Value: this.dataSource[i].AgTotal },
                    { Name: "MAgInsuranceGemsWinLoss", Value: this.dataSource[i].MAgInsuranceGemsWinLoss },
                    { Name: "MAgComm", Value: this.dataSource[i].MAgComm },
                    { Name: "MAgTotal", Value: this.dataSource[i].MAgTotal },
                    { Name: "SAgInsuranceGemsWinLoss", Value: this.dataSource[i].SAgInsuranceGemsWinLoss },
                    { Name: "SAgComm", Value: this.dataSource[i].SAgComm },
                    { Name: "SAgTotal", Value: this.dataSource[i].SAgTotal },
                    { Name: "Company", Value: this.dataSource[i].Company }
                    ]
                })
            }

            sumTurnover += this.dataSource[i].Turnover;
            sumNetTurnover += this.dataSource[i].NetTurnover;
            sumTaxComm += this.dataSource[i].TaxComm;
            sumMemberWinLoss += this.dataSource[i].MemberWinLoss;
            sumMemberInsuranceGemsWinLoss += this.dataSource[i].MemberInsuranceGemsWinLoss;
            sumMemberComm += this.dataSource[i].MemberComm;
            sumMemberTotal += this.dataSource[i].MemberTotal;
            sumAgInsuranceGemsWinLoss += this.dataSource[i].AgInsuranceGemsWinLoss;
            sumAgComm += this.dataSource[i].AgComm;
            sumAgTotal += this.dataSource[i].AgTotal;
            if (this.viewlevel > 1) {
                sumMAgInsuranceGemsWinLoss += this.dataSource[i].MAgInsuranceGemsWinLoss;
                sumMAgComm += this.dataSource[i].MAgComm;
                sumMAgTotal += this.dataSource[i].MAgTotal;
            }
            if (this.viewlevel > 2) {
                sumSAgInsuranceGemsWinLoss += this.dataSource[i].SAgInsuranceGemsWinLoss;
                sumSAgComm += this.dataSource[i].SAgComm;
                sumSAgTotal += this.dataSource[i].SAgTotal;
            }
            sumCompany += this.dataSource[i].Company;
        }

        gridOption.bodyRows = bodyData;
        gridOption.pagerOption.totalItem = this.totalRows;

        //// Asign Footer Data
        var footerData = [];
        if (this.viewlevel == 1) {
            footerData.push({
                Columns: [
                    { Name: "Username", Value: "Grand Total" },
                    { Name: "Turnover", Value: sumTurnover },
                    { Name: "NetTurnover", Value: sumNetTurnover },
                    { Name: "TaxComm", Value: sumTaxComm },
                    { Name: "MemberWinLoss", Value: sumMemberWinLoss },
                    { Name: "MemberInsuranceGemsWinLoss", Value: sumMemberInsuranceGemsWinLoss },
                    { Name: "MemberComm", Value: sumMemberComm },
                    { Name: "MemberTotal", Value: sumMemberTotal },
                    { Name: "AgInsuranceGemsWinLoss", Value: sumAgInsuranceGemsWinLoss },
                    { Name: "AgComm", Value: sumAgComm },
                    { Name: "AgTotal", Value: sumAgTotal },
                    { Name: "Company", Value: sumCompany }
                ]
            });
        } else if (this.viewlevel == 2) {
            footerData.push({
                Columns: [
                    { Name: "Username", Value: "Grand Total" },
                    { Name: "Turnover", Value: sumTurnover },
                    { Name: "NetTurnover", Value: sumNetTurnover },
                    { Name: "TaxComm", Value: sumTaxComm },
                    { Name: "MemberWinLoss", Value: sumMemberWinLoss },
                    { Name: "MemberInsuranceGemsWinLoss", Value: sumMemberInsuranceGemsWinLoss },
                    { Name: "MemberComm", Value: sumMemberComm },
                    { Name: "MemberTotal", Value: sumMemberTotal },
                    { Name: "AgInsuranceGemsWinLoss", Value: sumAgInsuranceGemsWinLoss },
                    { Name: "AgComm", Value: sumAgComm },
                    { Name: "AgTotal", Value: sumAgTotal },
                    { Name: "MAgInsuranceGemsWinLoss", Value: sumMAgInsuranceGemsWinLoss },
                    { Name: "MAgComm", Value: sumMAgComm },
                    { Name: "MAgTotal", Value: sumMAgTotal },
                    { Name: "Company", Value: sumCompany }
                ]
            });
        } else {
            footerData.push({
                Columns: [
                    { Name: "Username", Value: "Grand Total" },
                    { Name: "Turnover", Value: sumTurnover },
                    { Name: "NetTurnover", Value: sumNetTurnover },
                    { Name: "TaxComm", Value: sumTaxComm },
                    { Name: "MemberWinLoss", Value: sumMemberWinLoss },
                    { Name: "MemberInsuranceGemsWinLoss", Value: sumMemberInsuranceGemsWinLoss },
                    { Name: "MemberComm", Value: sumMemberComm },
                    { Name: "MemberTotal", Value: sumMemberTotal },
                    { Name: "AgInsuranceGemsWinLoss", Value: sumAgInsuranceGemsWinLoss },
                    { Name: "AgComm", Value: sumAgComm },
                    { Name: "AgTotal", Value: sumAgTotal },
                    { Name: "MAgInsuranceGemsWinLoss", Value: sumMAgInsuranceGemsWinLoss },
                    { Name: "MAgComm", Value: sumMAgComm },
                    { Name: "MAgTotal", Value: sumMAgTotal },
                    { Name: "SAgInsuranceGemsWinLoss", Value: sumSAgInsuranceGemsWinLoss },
                    { Name: "SAgComm", Value: sumSAgComm },
                    { Name: "SAgTotal", Value: sumSAgTotal },
                    { Name: "Company", Value: sumCompany }
                ]
            });
        }

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
                    if ($this.fixedsortlist[i] == rows[j].Columns[0].Value.CustId) {
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
                    if ($this.fixedsortlist[i] == $this.dataSource[j].CustId) {
                        $this.fixedsortdata.push($this.dataSource.slice(j, j + 1)[0]);
                        $this.dataSource.unshift($this.dataSource.splice(j, 1)[0]);
                        break;
                    }
                }
            }
        }
    },
    ChangeHeadlineTab: function () {
        if (this.viewlevel >= 4) {
            this.srecommend = this.custId;
            this.ToggleTabs('none', 'none', 'none', 'none', 4);
        } else if (this.viewlevel == 3) {
            this.mrecommend = this.custId;
            this.ToggleTabs('block', 'block', 'none', 'none', 3);
            $("#tabSuper").attr("class", "tabActiveSuper");
        } else if (this.viewlevel == 2) {
            this.recommend = this.custId;
            this.ToggleTabs('block', 'block', 'block', 'none', 2);
            $("#tabSuper").attr("class", "tabDisableSuper");
            $("#tabMaster").attr("class", "tabActive");
        } else {
            this.ToggleTabs('block', 'block', 'block', 'block', 1);
            $("#tabSuper").attr("class", "tabDisableSuper");
            $("#tabMaster").attr("class", "tabDisable");
            $("#tabAgent").attr("class", "tabActive");
        };
        $("#headlineDaily").css("visibility", "visible");
    },
    GetDownlineWinLossDetail: function (custId) {
        this.custId = custId;
        this.username = "";
        this.Databind(this.username);
    },
    InitShowHideControls: function (status) {
        $("#headlineDaily").css("display", status);
        $("#mainGrid").css("display", status);
    },
    ResetParameters: function () {
        this.srecommend = 0;
        this.mrecommend = 0;
        this.recommend = 0;
        this.custId = 0;
        this.username = '';
        this.viewlevel = 4;
    },
    GetFromDateToDate: function () {
        if ($(".from-to-date").css('display') != 'none') {
            this.fromdate = $("#dtpFromDate").val();
            this.todate = $("#dtpToDate").val();
        }
        else {
            this.fromdate = $("#monPickr1").monPickr("getFromDate", "MM/dd/yyyy");
            this.todate = $("#monPickr1").monPickr("getToDate", "MM/dd/yyyy");
        }
    },
    Databind: function (username) {
        this.username = !username ? "" : username;
        this.GetFromDateToDate();
        this.groupName = $("#selectedGroupName").html();

        var dateInfo;
        if (this.fromdate == this.todate) {
            dateInfo = 'on ' + this.fromdate;
        }
        else {
            dateInfo = 'from ' + (new Date(Date.parse(this.fromdate))).toStringFormat('MMM dd, yyyy') + ' to ' + (new Date(Date.parse(this.todate))).toStringFormat('MMM dd, yyyy');
        }

        var parameterUrl = "?groupname=" + this.groupName
                            + "&srecommend=" + this.srecommend
                            + "&mrecommend=" + this.mrecommend
                            + "&recommend=" + this.recommend
                            + "&custid=" + this.custId
                            + "&username=" + Fanex.ExtAcc.ReplaceAll(this.viewlevel == 5 ? "" : this.username, "&", "")
                            + "&fromdate=" + this.fromdate
                            + "&todate=" + this.todate
                            + "&viewlevel=" + this.viewlevel;

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

                $this.viewlevel = data.viewlevel;
                if ($this.viewlevel == 3) {
                    $this.srecommend = data.srecommend;
                } else if ($this.viewlevel == 2) {
                    $this.srecommend = data.srecommend;
                    $this.mrecommend = data.mrecommend;
                } else if ($this.viewlevel == 1) {
                    $this.srecommend = data.srecommend;
                    $this.mrecommend = data.mrecommend;
                    $this.recommend = data.recommend;
                }

                if ($this.custId == 0) {
                    $this.supername = data.supername;
                    $this.mastername = data.mastername;
                    $this.agentname = data.agentname;
                }

                $this.ChangeHeadlineTab();

                $("#mainGrid").css("display", $this.totalRows < 1 ? "none" : "block");
                $("#mainGrid").css("visibility", $this.totalRows < 1 ? "hidden" : "visible");
                $("#headlineDaily").css("display", $this.totalRows < 1 ? "none" : "block");
                $("#headlineDaily").css("visibility", $this.totalRows < 1 ? "hidden" : "visible");
                $("#msgNoinfo").css("display", $this.totalRows < 1 ? "block" : "none");

                if ($this.totalRows > 0) {
                    $this.Render(dateInfo);
                    $(".export-to-excel-container").removeClass("hide");
                    $(".groupBack").removeClass("hide");
                } else {
                    $(".export-to-excel-container").addClass("hide");
                    $(".groupBack").addClass("hide");
                }

                Fanex.ExtAcc.UnBlockUI();
            }
        });
    }
};

$(document).ready(function () {
    Fanex.ExtAcc.PokerGamesWinLossDetail.Init();
});