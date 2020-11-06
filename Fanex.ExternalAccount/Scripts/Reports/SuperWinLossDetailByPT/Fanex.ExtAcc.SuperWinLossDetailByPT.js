if (typeof Fanex === "undefined" || typeof Fanex.ExtAcc === "undefined") {
    alert("Fanex.ExtAcc.js module is required!");
}

class SuperWinLossDetailByPT {
    constructor() {
        this.gridOption = {};
        this.groupName= "";
        this.fixedsortdata= new Array();
        this.fixedsortlist= new Array();
        this.url= resolveClientUrl("SuperWinLossDetailByPT/GetData");
        this.dataSource= [];
        this.grandTotal= null;
        this.pageIndex= 0;
        this.pageSize= 50;
        this.totalRecords= 0;
        this.productDetailsData= [];
        this.hideFilter= false;
        this.dateTimeControls = new DateTimeControls();
        this.productControls = new ProductControls();

        this.defaultColumns = [
            { HeaderText: "Username", Name: "Username", Width: 130, Sort: true, HeaderAlign: "center", CellAlign: "left", ShowCollapseExpand: true },
            { HeaderText: "Turnover", Name: "Turnover", Width: 130, Sort: true, HeaderAlign: "center", CellAlign: "right", FormatNumber: "0,0.00", FormatNegative: "$" },
            { HeaderText: "Net Turnover", Name: "NetTurnover", Width: 130, Sort: true, HeaderAlign: "center", CellAlign: "right", FormatNumber: "0,0.00", FormatNegative: "$" },
            { HeaderText: "GrossComm", Name: "GrossComm", Width: 130, Sort: true, HeaderAlign: "center", CellAlign: "right", FormatNumber: "0,0.00", FormatNegative: "$" },
            { HeaderText: "Win/Loss", Name: "MemberWinlost", Width: 130, Sort: true, HeaderAlign: "center", CellAlign: "right", FormatNumber: "0,0.00", FormatNegative: "$" },
            { HeaderText: "Comm", Name: "PlayerComm", Width: 130, Sort: true, HeaderAlign: "center", CellAlign: "right", FormatNumber: "0,0.00", FormatNegative: "$" },
            { HeaderText: "Total", Name: "MemberTotal", Width: 130, Sort: true, HeaderAlign: "center", CellAlign: "right", FormatNumber: "0,0.00", FormatNegative: "$" },
            { HeaderText: "Win/Loss", Name: "AgWinlost", Width: 130, Sort: true, HeaderAlign: "center", CellAlign: "right", FormatNumber: "0,0.00", FormatNegative: "$" },
            { HeaderText: "Comm", Name: "AgComm", Width: 130, Sort: true, HeaderAlign: "center", CellAlign: "right", FormatNumber: "0,0.00", FormatNegative: "$" },
            { HeaderText: "Total", Name: "AgTotal", Width: 130, Sort: true, HeaderAlign: "center", CellAlign: "right", FormatNumber: "0,0.00", FormatNegative: "$" },
            { HeaderText: "Win/Loss", Name: "MAgWinlost", Width: 130, Sort: true, HeaderAlign: "center", CellAlign: "right", FormatNumber: "0,0.00", FormatNegative: "$" },
            { HeaderText: "Comm", Name: "MAgComm", Width: 130, Sort: true, HeaderAlign: "center", CellAlign: "right", FormatNumber: "0,0.00", FormatNegative: "$" },
            { HeaderText: "Total", Name: "MAgTotal", Width: 130, Sort: true, HeaderAlign: "center", CellAlign: "right", FormatNumber: "0,0.00", FormatNegative: "$" },
            { HeaderText: "Win/Loss", Name: "SAgWinlost", Width: 130, Sort: true, HeaderAlign: "center", CellAlign: "right", FormatNumber: "0,0.00", FormatNegative: "$" },
            { HeaderText: "Comm", Name: "SAgComm", Width: 130, Sort: true, HeaderAlign: "center", CellAlign: "right", FormatNumber: "0,0.00", FormatNegative: "$" },
            { HeaderText: "Total", Name: "SAgTotal", Width: 130, Sort: true, HeaderAlign: "center", CellAlign: "right", FormatNumber: "0,0.00", FormatNegative: "$" },
            { HeaderText: "Company", Name: "Company", Width: 130, Sort: true, HeaderAlign: "center", CellAlign: "right", FormatNumber: "0,0.00", FormatNegative: "$" }
        ];

        this.defaultHeaderColumns = [
            { HeaderText: "Username", Name: "Username", MergeRowNumber: 2 },
            { HeaderText: "Turnover", Name: "Turnover", MergeRowNumber: 2 },
            { HeaderText: "Net Turnover&nbsp;<span id=\"netTurnover\" class=\"bet-type-detail-icon\"></span>", Name: "NetTurnover", MergeRowNumber: 2 },
            { HeaderText: "GrossComm", Name: "GrossComm", MergeRowNumber: 2 },
            { HeaderText: "Member", Name: "MemberWinlost", MergeColNumber: 3 },
            { HeaderText: "Agent", Name: "AgWinlost", MergeColNumber: 3 },
            { HeaderText: "Master", Name: "MAgWinlost", MergeColNumber: 3 },
            { HeaderText: "Super", Name: "SAgWinlost", MergeColNumber: 3 },
            { HeaderText: "Company", Name: "Company", MergeRowNumber: 2 }
        ];
    }

    CallSubmit() {
        this.groupName = $("#selectedGroupName").html();
        this.Databind();
    }
    Init() {
        var $this = this;

        Fanex.ExtAcc.InitSuggestion();

        $("#DateList").combobox({
            selected: function (event, ui) {
                var itemIndex = event.target.selectedIndex;
                $("#DateList").prop("selectedIndex", itemIndex);

                var fdate = new Date($("#MaxDate").val());
                var tdate = new Date($("#MaxDate").val());
                if (itemIndex === 1) {
                    fdate = new Date(tdate.getFullYear(), tdate.getMonth(), tdate.getDate() - 1);
                    tdate = fdate;
                } else if (itemIndex === 2) {
                    fdate = new Date(tdate.getFullYear(), tdate.getMonth(), tdate.getDate() - $this.dateTimeControls.getDayOfYear(tdate) + 1);
                } else if (itemIndex === 3) {
                    fdate = new Date(tdate.getFullYear(), tdate.getMonth(), tdate.getDate() - $this.dateTimeControls.getDayOfYear(tdate) - 6);
                    tdate = new Date(tdate.getFullYear(), tdate.getMonth(), tdate.getDate() - $this.dateTimeControls.getDayOfYear(tdate));
                } else if (itemIndex === 4) {
                    fdate = new Date(tdate.getFullYear(), tdate.getMonth(), 1);
                    tdate = new Date(tdate.getFullYear(), tdate.getMonth(), tdate.getDate());
                } else if (itemIndex === 5) {
                    fdate = new Date(tdate.getFullYear(), tdate.getMonth() - 1, 1);
                    tdate = new Date(tdate.getFullYear(), tdate.getMonth(), 0);
                } else if (itemIndex === 6) {
                    fdate = new Date(tdate.getFullYear(), tdate.getMonth() - 1, 1);
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
            beforeShow: $this.dateTimeControls.SetRange,
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
            beforeShow: $this.dateTimeControls.SetRange,
            dateFormat: "M dd, yy"
        });
        $("#dtpToDate").val($.datepicker.formatDate("M dd, yy", new Date($("#MaxDate").val())));

        $(".ui-datepicker").draggable();

        $this.dateTimeControls.initMonthlyCalendar();

        $("#ddlGroupName").bind("change", function (event) {
            var args = event.args;
            if (args) {
                var item = args.item;
                $this.groupName = item.innerHTML;

                $this.InitShowHideControls("none");
            }
        });

        $this.groupName = $("#selectedGroupName").html();

        this.productControls.ProductsList(this.groupName);

        $("#Submit").click(function (e) {
            $this.CallSubmit();
        });

        $("#exporttoexcel").click(function () {
            var dateTimeParams = $this.dateTimeControls.GetFromDateToDate();
            var params = "fromdate=" + dateTimeParams.fromdate +
                "&todate=" + dateTimeParams.todate +
                "&groupname=" + $this.groupName;

            var url = resolveClientUrl("SuperWinLossDetailByPT/ExportWinLossDetailToExcel?") + params +
                $this.productControls.getParameterProducts() +
                $this.productControls.getParameterProductDetails();

            top.exportExcel(url, document);
        });

        $this.sorters = $(".col-sorter");
        $this.sorters.click(function () {
            var elem = $(this);
            $this.sortTable(elem);
        });

        Fanex.ExtAcc.OnResize = $this.resizeReportGrid;
        $(".icon-filter").click($this.showHideFilter);
    }
    resizeReportGrid() {
        try {
            $("#jqgrid").fanexGrid("resizeGrid");
        }
        catch (err) {
            console.log(err);
        }
    }
    showHideFilter() {
        if ($this.hideFilter) {
            $(".icon-filter").addClass("active");

            $(".filterbox").removeClass("hide");
        } else {
            $(".icon-filter").removeClass("active");
            $(".filterbox").addClass("hide");
        }

        $this.resizeReportGrid();
        $this.hideFilter = !$this.hideFilter;
    }
    CustomUsernameSortFunction(cell1Value, cell2Value, sortDirection) {
        if (sortDirection === 'asc') {
            return cell1Value.Username > cell2Value.Username ? 1 : -1;
        }

        return cell1Value.Username < cell2Value.Username ? 1 : -1;
    }
    
    Render(dateInfo) {
        var arrColData = new Array();
        var arrGroupHeaders = new Array();

        var numberOfFrozenColumn = 3;
        var widthDevice = top.$(top.window).width();
        if (widthDevice <= 768) {
            numberOfFrozenColumn = 1;
        }

        var titleInfo = {
            text: "Super Win Loss Detail By PT " + dateInfo
        };

        arrColData = this.defaultColumns;
        arrGroupHeaders = this.defaultHeaderColumns;

        this.gridOption = {
            customizeHeader: true,
            groupHeaderRows: [{ Columns: arrGroupHeaders }],
            columns: arrColData,
            numberOfFrozenColumn: numberOfFrozenColumn,
            autoCalculateGridExpandHeight: true,
            fullHeight: !isMobile.phone,
            gridHeight: isMobile.phone ? $(window).height() - 70 : 0,
            showTitle: true,
            titleInfo,
            showFooter: true,
            showTree: true,
            gridExpandHeight: 256,
            showPager: true,
            customPager: true,
            pagerOption: {
                itemsPerPageArray: [50, 100, 200],
                currentPage: $this.pageIndex - 1,
                totalItem: $this.totalRecords,
                itemsPerPage: $this.pageSize,
                pagerSelectCallBack: function (currentPageIndex, itemsPerPage) {
                    $this.Databind(itemsPerPage, currentPageIndex + 1);
                    $("#jqgrid").fanexGrid($this.gridOption); 
                    $(".product-tooltip").tooltip();
                },
                showDetail: true
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

        //// Assing BodyData
        $this.InitSort();
        var bodyData = [];
        for (var i = 0; i < this.totalRows; i++) {
            var subRow = [];
            for (var j = 0; j < this.dataSource[i].WinLossByProducts.length; j++) {
                var winLossByProductModel = this.dataSource[i].WinLossByProducts[j];
                subRow.push({
                    Columns: [
                        {
                            HeaderText: "Username", Name: "Username",
                            Value: winLossByProductModel,
                            CustomCellRender: function (cell) { return $this.RenderPositonTakingColumn(cell); }
                        },
                        { Name: "Turnover", Value: winLossByProductModel.Turnover },
                        { Name: "NetTurnover", Value: winLossByProductModel.NetTurnover },
                        { Name: "GrossComm", Value: winLossByProductModel.GrossComm },
                        { Name: "MemberWinlost", Value: winLossByProductModel.MemberWinlost },
                        { Name: "PlayerComm", Value: winLossByProductModel.MemberComm },
                        { Name: "MemberTotal", Value: winLossByProductModel.MemberTotal },
                        { Name: "AgWinlost", Value: winLossByProductModel.AgWinlost },
                        { Name: "AgComm", Value: winLossByProductModel.AgComm },
                        { Name: "AgTotal", Value: winLossByProductModel.AgTotal },
                        { Name: "MAgWinlost", Value: winLossByProductModel.MAgWinlost },
                        { Name: "MAgComm", Value: winLossByProductModel.MAgComm },
                        { Name: "MAgTotal", Value: winLossByProductModel.MAgTotal },
                        { Name: "SAgWinlost", Value: winLossByProductModel.SAgWinlost },
                        { Name: "SAgComm", Value: winLossByProductModel.SAgComm },
                        { Name: "SAgTotal", Value: winLossByProductModel.SAgTotal },
                        { Name: "Company", Value: winLossByProductModel.Company }
                    ]
                });
            }

            var totalRow = this.dataSource[i].Total;
            bodyData.push({
                Columns: [
                    { Name: "Username", Value: totalRow.Username },
                    { Name: "Turnover", Value: totalRow.Turnover },
                    { Name: "NetTurnover", Value: totalRow.NetTurnover },
                    { Name: "GrossComm", Value: totalRow.GrossComm },
                    { Name: "MemberWinlost", Value: totalRow.MemberWinlost },
                    { Name: "PlayerComm", Value: totalRow.MemberComm },
                    { Name: "MemberTotal", Value: totalRow.MemberTotal },
                    { Name: "AgWinlost", Value: totalRow.AgWinlost },
                    { Name: "AgComm", Value: totalRow.AgComm },
                    { Name: "AgTotal", Value: totalRow.AgTotal },
                    { Name: "MAgWinlost", Value: totalRow.MAgWinlost },
                    { Name: "MAgComm", Value: totalRow.MAgComm },
                    { Name: "MAgTotal", Value: totalRow.MAgTotal },
                    { Name: "SAgWinlost", Value: totalRow.SAgWinlost },
                    { Name: "SAgComm", Value: totalRow.SAgComm },
                    { Name: "SAgTotal", Value: totalRow.SAgTotal },
                    { Name: "Company", Value: totalRow.Company }
                ],
                SubRows: subRow,
                CssClass: 'sub-row-class'
            });

           
        }

        this.gridOption.bodyRows = bodyData;
        this.gridOption.pagerOption.totalItem = this.totalRecords;

        //// Asign Footer Data
        var footerData = [];
        footerData.push({
            Columns: [
                { Name: "Username", Value: "Grand Total" },
                { Name: "Turnover", Value: this.grandTotal.Turnover },
                { Name: "NetTurnover", Value: this.grandTotal.NetTurnover },
                { Name: "GrossComm", Value: this.grandTotal.GrossComm },
                { Name: "MemberWinlost", Value: this.grandTotal.MemberWinlost },
                { Name: "PlayerComm", Value: this.grandTotal.MemberComm },
                { Name: "MemberTotal", Value: this.grandTotal.MemberTotal },
                { Name: "AgWinlost", Value: this.grandTotal.AgWinlost },
                { Name: "AgComm", Value: this.grandTotal.AgComm },
                { Name: "AgTotal", Value: this.grandTotal.AgTotal },
                { Name: "MAgWinlost", Value: this.grandTotal.MAgWinlost },
                { Name: "MAgComm", Value: this.grandTotal.MAgComm },
                { Name: "MAgTotal", Value: this.grandTotal.MAgTotal },
                { Name: "SAgWinlost", Value: this.grandTotal.SAgWinlost },
                { Name: "SAgComm", Value: this.grandTotal.SAgComm },
                { Name: "SAgTotal", Value: this.grandTotal.SAgTotal },
                { Name: "Company", Value: this.grandTotal.Company }
            ]
        });

        this.gridOption.footerRows = footerData;

        setPagerOptionOnMobile(this.gridOption);
        $("#jqgrid").fanexGrid(this.gridOption);
        $(".product-tooltip").tooltip();

        workaroundForGridOnPhone();
    }
    RenderPositonTakingColumn(cell) {
        var tooltip = '';

        $.each(cell.Value.ProductNames, function (index, value) {
            tooltip += '<div>' + value + '</div>';
        });

        return '<span class="product-tooltip" title="' + tooltip + '">' + cell.Value.PositionTakingText + ' <a>(' + cell.Value.ProductNames.length + ')</a>' + '</span>';
    }

    customsortfunc(rows) {
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
    }
    InitSort() {
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
    }
    InitShowHideControls(status) {
        $("#headlineDaily").css("display", status);
        $("#mainGrid").css("display", status);
    }

    Databind(pageSize = 50, pageIndex = 1) {
        var dateTimeParams = $this.dateTimeControls.GetFromDateToDate();
        this.groupName = $("#selectedGroupName").html();

        var dateInfo;
        if (dateTimeParams.fromdate === dateTimeParams.todate) {
            dateInfo = 'on ' + dateTimeParams.fromdate;
        }
        else {
            dateInfo = 'from ' + (new Date(Date.parse(dateTimeParams.fromdate))).toStringFormat('MMM dd, yyyy') +
                ' to ' + (new Date(Date.parse(dateTimeParams.todate))).toStringFormat('MMM dd, yyyy');
        }
      
        var parameterUrl = "?Groupname=" + this.groupName
            + "&Fromdate=" + dateTimeParams.fromdate
            + "&Todate=" + dateTimeParams.todate
            + "&PageSize=" + pageSize
            + "&PageIndex=" + pageIndex
            + $this.productControls.getParameterProducts()
            + $this.productControls.getParameterProductDetails();

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

                $this.dataSource = data.WinLossByUsernames;
                $this.grandTotal = data.GrandTotal;
                $this.totalRows = data.WinLossByUsernames !== null ? data.WinLossByUsernames.length : 0;
                $this.totalRecords = data.TotalRecords;
                $this.pageSize = data.PageSize;
                $this.pageIndex = data.PageIndex;
                $this.fixedsortlist = [];

                $("#mainGrid").css("display", $this.totalRows < 1 ? "none" : "block");
                $("#mainGrid").css("visibility", $this.totalRows < 1 ? "hidden" : "visible");
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

                        if (data.TooltipsForLiveCasino !== "") {
                            message +=
                                "<br/>" +
                                "<br/>" +
                            data.TooltipsForLiveCasino;
                            height += 50;
                        }

                        Fanex.ExtAcc.Form.Alert("Net Turnover Information", message, 430, height);
                        return false;
                    });

                Fanex.ExtAcc.UnBlockUI();
            }
        });
    }
}

$(document).ready(function () {
    var superWinLossDetailByPT = new SuperWinLossDetailByPT();
    superWinLossDetailByPT.Init();
});