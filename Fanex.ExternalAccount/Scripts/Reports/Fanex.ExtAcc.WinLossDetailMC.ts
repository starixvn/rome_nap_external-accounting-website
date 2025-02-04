declare var Fanex: any;
declare var workaroundForGridOnPhone: any;
declare var setPagerOptionOnMobile: any;
declare var isMobile: any;

class WinLossDetailMC {
    private $: JQueryStatic;
    private productControls: ProductControls;

    private groupId = "";
    private groupName = "";
    private username = "";
    private srecommend = 0;
    private mrecommend = 0;
    private recommend = 0;
    private custId = 0;
    private supername = "";
    private mastername = "";
    private agentname = "";
    private cachedProductAndCurParams = "";
    private fromdate = moment(new Date()).format("MM/DD/YYYY");
    private todate = moment(new Date()).format("MM/DD/YYYY");
    private fixedsortdata = new Array();
    private removedsortdata = new Array();
    private fixedsortlist = new Array();
    private url = resolveClientUrl("WinLossDetailMC/GetData");
    private viewlevel = 5;
    private dataSource = [];
    private totalSource = [];
    private totalRows = 0;
    private productDetailsData = [];
    private productDetailsDataMapToProductId: any;
    private $multiSelectProduct: any;
    private $multiSelectProductDetail: any;
    private $multiSelectCurrency: any;
    private dateRangePickerComponent: DateRangePickerJQueryComponent;
    private topWindow: any;
    private hideFilter = false;
    private sorters: any;
    private sortTable: any;

    private DefaultColumns = [
        {
            HeaderText: "Username", Name: "Username", Width: 130, Sort: true, HeaderAlign: "center",
            CustomCellRender: cell => this.RenderUsernameColumn(cell),
            CustomSortFunction: (cell1Value, cell2Value, sortDirection) => this.CustomUsernameSortFunction(cell1Value, cell2Value, sortDirection)
        },
        { HeaderText: "Currency Name", Name: "Currency", Width: 130, Sort: true, HeaderAlign: "center" },
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
        { HeaderText: "Company", Name: "Company", Width: 130, Sort: true, HeaderAlign: "center", CellAlign: "right", FormatNumber: "0,0.00", FormatNegative: "$" },
        { HeaderText: "Reward", Name: "RewardAmount", Width: 130, Sort: true, HeaderAlign: "center", CellAlign: "right", FormatNumber: "0,0.00", FormatNegative: "$" },
        { HeaderText: "Reward Currency", Name: "RewardCurrency", Width: 130, Sort: true, HeaderAlign: "center", CellAlign: "right" }
    ];
    private SAgColumns = [
        {
            HeaderText: "Username", Name: "Username", Width: 130, Sort: true, HeaderAlign: "center",
            CustomCellRender: cell => this.RenderUsernameColumn(cell),
            CustomSortFunction: (cell1Value, cell2Value, sortDirection) => this.CustomUsernameSortFunction(cell1Value, cell2Value, sortDirection)
        },
        { HeaderText: "Currency Name", Name: "Currency", Width: 130, Sort: true, HeaderAlign: "center" },
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
        { HeaderText: "Company", Name: "Company", Width: 130, Sort: true, HeaderAlign: "center", CellAlign: "right", FormatNumber: "0,0.00", FormatNegative: "$" },
        { HeaderText: "Reward", Name: "RewardAmount", Width: 130, Sort: true, HeaderAlign: "center", CellAlign: "right", FormatNumber: "0,0.00", FormatNegative: "$" },
        { HeaderText: "Reward Currency", Name: "RewardCurrency", Width: 130, Sort: true, HeaderAlign: "center", CellAlign: "right" }
    ];
    private MAgColumns = [
        {
            HeaderText: "Username", Name: "Username", Width: 130,
            Sort: true, HeaderAlign: "center",
            CustomCellRender: cell => this.RenderUsernameColumn(cell),
            CustomSortFunction: (cell1Value, cell2Value, sortDirection) => this.CustomUsernameSortFunction(cell1Value, cell2Value, sortDirection)
        },
        { HeaderText: "Currency Name", Name: "Currency", Width: 130, Sort: true, HeaderAlign: "center" },
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
        { HeaderText: "Company", Name: "Company", Width: 130, Sort: true, HeaderAlign: "center", CellAlign: "right", FormatNumber: "0,0.00", FormatNegative: "$" },
        { HeaderText: "Reward", Name: "RewardAmount", Width: 130, Sort: true, HeaderAlign: "center", CellAlign: "right", FormatNumber: "0,0.00", FormatNegative: "$" },
        { HeaderText: "Reward Currency", Name: "RewardCurrency", Width: 130, Sort: true, HeaderAlign: "center", CellAlign: "right" }
    ];
    private AgColumns = [
        {
            HeaderText: "Username", Name: "Username", Width: 130,
            Sort: true, HeaderAlign: "center",
            CustomCellRender: cell => this.RenderUsernameColumn(cell),
            CustomSortFunction: (cell1Value, cell2Value, sortDirection) => this.CustomUsernameSortFunction(cell1Value, cell2Value, sortDirection)
        },
        { HeaderText: "Currency Name", Name: "Currency", Width: 130, Sort: true, HeaderAlign: "center" },
        { HeaderText: "Turnover", Name: "Turnover", Width: 130, Sort: true, HeaderAlign: "center", CellAlign: "right", FormatNumber: "0,0.00", FormatNegative: "$" },
        { HeaderText: "Net Turnover", Name: "NetTurnover", Width: 130, Sort: true, HeaderAlign: "center", CellAlign: "right", FormatNumber: "0,0.00", FormatNegative: "$" },
        { HeaderText: "GrossComm", Name: "GrossComm", Width: 130, Sort: true, HeaderAlign: "center", CellAlign: "right", FormatNumber: "0,0.00", FormatNegative: "$" },
        { HeaderText: "Win/Loss", Name: "MemberWinlost", Width: 130, Sort: true, HeaderAlign: "center", CellAlign: "right", FormatNumber: "0,0.00", FormatNegative: "$" },
        { HeaderText: "Comm", Name: "PlayerComm", Width: 130, Sort: true, HeaderAlign: "center", CellAlign: "right", FormatNumber: "0,0.00", FormatNegative: "$" },
        { HeaderText: "Total", Name: "MemberTotal", Width: 130, Sort: true, HeaderAlign: "center", CellAlign: "right", FormatNumber: "0,0.00", FormatNegative: "$" },
        { HeaderText: "Win/Loss", Name: "AgWinlost", Width: 130, Sort: true, HeaderAlign: "center", CellAlign: "right", FormatNumber: "0,0.00", FormatNegative: "$" },
        { HeaderText: "Comm", Name: "AgComm", Width: 130, Sort: true, HeaderAlign: "center", CellAlign: "right", FormatNumber: "0,0.00", FormatNegative: "$" },
        { HeaderText: "Total", Name: "AgTotal", Width: 130, Sort: true, HeaderAlign: "center", CellAlign: "right", FormatNumber: "0,0.00", FormatNegative: "$" },
        { HeaderText: "Company", Name: "Company", Width: 130, Sort: true, HeaderAlign: "center", CellAlign: "right", FormatNumber: "0,0.00", FormatNegative: "$" },
        { HeaderText: "Reward", Name: "RewardAmount", Width: 130, Sort: true, HeaderAlign: "center", CellAlign: "right", FormatNumber: "0,0.00", FormatNegative: "$" },
        { HeaderText: "Reward Currency", Name: "RewardCurrency", Width: 130, Sort: true, HeaderAlign: "center", CellAlign: "right" }
    ];
    private DefaultHeaderColumns = [
        { HeaderText: "Username", Name: "Username", MergeRowNumber: 2 },
        { HeaderText: "Currency Name", Name: "Currency", MergeRowNumber: 2 },
        { HeaderText: "Turnover", Name: "Turnover", MergeRowNumber: 2 },
        { HeaderText: "Net Turnover&nbsp;<span id=\"netTurnover\" class=\"bet-type-detail-icon\"></span>", Name: "NetTurnover", MergeRowNumber: 2 },
        { HeaderText: "GrossComm", Name: "GrossComm", MergeRowNumber: 2 },
        { HeaderText: "Member", Name: "MemberWinlost", MergeColNumber: 3 },
        { HeaderText: "Agent", Name: "AgWinlost", MergeColNumber: 3 },
        { HeaderText: "Master", Name: "MAgWinlost", MergeColNumber: 3 },
        { HeaderText: "Super", Name: "SAgWinlost", MergeColNumber: 3 },
        { HeaderText: "Company", Name: "Company", MergeRowNumber: 2 },
        { HeaderText: "Reward", Name: "RewardAmount", MergeRowNumber: 2 },
        { HeaderText: "Reward Currency", Name: "RewardCurrency", MergeRowNumber: 2 }
    ];
    private SAgHeaderColumns = [
        { HeaderText: "Username", Name: "Username", MergeRowNumber: 2 },
        { HeaderText: "Currency Name", Name: "Currency", MergeRowNumber: 2 },
        { HeaderText: "Turnover", Name: "Turnover", MergeRowNumber: 2 },
        { HeaderText: "Net Turnover&nbsp;<span id=\"netTurnover\" class=\"bet-type-detail-icon\"></span>", Name: "NetTurnover", MergeRowNumber: 2 },
        { HeaderText: "GrossComm", Name: "GrossComm", MergeRowNumber: 2 },
        { HeaderText: "Member", Name: "MemberWinlost", MergeColNumber: 3 },
        { HeaderText: "Agent", Name: "AgWinlost", MergeColNumber: 3 },
        { HeaderText: "Master", Name: "MAgWinlost", MergeColNumber: 3 },
        { HeaderText: "Super", Name: "SAgWinlost", MergeColNumber: 3 },
        { HeaderText: "Company", Name: "Company", MergeRowNumber: 2 },
        { HeaderText: "Reward", Name: "RewardAmount", MergeRowNumber: 2 },
        { HeaderText: "Reward Currency", Name: "RewardCurrency", MergeRowNumber: 2 }
    ];
    private MAgHeaderColumns = [
        { HeaderText: "Username", Name: "Username", MergeRowNumber: 2 },
        { HeaderText: "Currency Name", Name: "Currency", MergeRowNumber: 2 },
        { HeaderText: "Turnover", Name: "Turnover", MergeRowNumber: 2 },
        { HeaderText: "Net Turnover&nbsp;<span id=\"netTurnover\" class=\"bet-type-detail-icon\"></span>", Name: "NetTurnover", MergeRowNumber: 2 },
        { HeaderText: "GrossComm", Name: "GrossComm", MergeRowNumber: 2 },
        { HeaderText: "Member", Name: "MemberWinlost", MergeColNumber: 3 },
        { HeaderText: "Agent", Name: "AgWinlost", MergeColNumber: 3 },
        { HeaderText: "Master", Name: "MAgWinlost", MergeColNumber: 3 },
        { HeaderText: "Company", Name: "Company", MergeRowNumber: 2 },
        { HeaderText: "Reward", Name: "RewardAmount", MergeRowNumber: 2 },
        { HeaderText: "Reward Currency", Name: "RewardCurrency", MergeRowNumber: 2 }
    ];
    private AgHeaderColumns = [
        { HeaderText: "Username", Name: "Username", MergeRowNumber: 2 },
        { HeaderText: "Currency Name", Name: "Currency", MergeRowNumber: 2 },
        { HeaderText: "Turnover", Name: "Turnover", MergeRowNumber: 2 },
        { HeaderText: "Net Turnover&nbsp;<span id=\"netTurnover\" class=\"bet-type-detail-icon\"></span>", Name: "NetTurnover", MergeRowNumber: 2 },
        { HeaderText: "GrossComm", Name: "GrossComm", MergeRowNumber: 2 },
        { HeaderText: "Member", Name: "MemberWinlost", MergeColNumber: 3 },
        { HeaderText: "Agent", Name: "AgWinlost", MergeColNumber: 3 },
        { HeaderText: "Company", Name: "Company", MergeRowNumber: 2 },
        { HeaderText: "Reward", Name: "RewardAmount", MergeRowNumber: 2 },
        { HeaderText: "Reward Currency", Name: "RewardCurrency", MergeRowNumber: 2 }
    ];

    RenderUsernameColumn(cell) {
        if (cell == null || this.viewlevel < 2) {
            return '<div class="text-truncate" style="line-height: 22px; text-align: left; padding-left: 3px" title="' + cell.Value.Username + '">' + cell.Value.Username + "</div>";
        }
        
        return '<div style="line-height: 22px; text-align: left;">' +
            '<a class="text-truncate report-username" style="text-decoration: none; margin: 0" href="" data-custid="' + cell.Value.CustId + '" title="' + cell.Value.Username + '">' + cell.Value.Username + '</a>' +
            '</div>';
    }

    CustomUsernameSortFunction(cell1Value, cell2Value, sortDirection) {
        if (sortDirection == 'asc') {
            return cell1Value.Username > cell2Value.Username ? 1 : -1;
        }

        return cell1Value.Username < cell2Value.Username ? 1 : -1;
    }

    constructor($: JQueryStatic, topWindow: any, dateRangePickerComponent: DateRangePickerJQueryComponent) {
        this.$ = $;
        this.topWindow = topWindow;

        this.fixedsortdata = [];
        this.fixedsortlist = [];
        this.productControls = new ProductControls($);
        this.dateRangePickerComponent = dateRangePickerComponent;
    }

    Init() {
        this.productControls.Init();
        this.dateRangePickerComponent.render(new DateRangePickerJQueryOption());
        Fanex.ExtAcc.InitSuggestion();


        $("#ddlGroupName").bind("change", (event) => {
            var args = event.data;
            if (args) {
                var item = args.item;
                this.groupName = item.innerHTML;

                this.ResetParameters();
                this.InitShowHideControls("none");
            }
        });

        this.groupId = Fanex.ExtAcc.Customer.GetGroupId() || "";
        this.groupName = $("#selectedGroupName").html();

        this.InitCurrencyList();
        $("#ddl-base-currency").combobox();
        $("#base-currency input").attr('readonly', 'true').val(($("#ddl-base-currency")[0] as HTMLSelectElement).options[0].text).bind("click", function () {
            $(this).autocomplete("search", "");
        });
        $("#ckbBaseCurrency").change(() => {
            $("#base-currency").find("input, button").prop("disabled", !$("#ckbBaseCurrency").is(":checked"));
        })

        $("#Submit").click((e) => {
            this.CallSubmit();
        });

        $("#txtUserName").keyup(function (e) {
            if ($("#txtUserName").val().length > 1) {
                Fanex.ExtAcc.InitSuggestion();
            }
        });
        $("#tabSuper").click((e) => {
            this.viewlevel = 4;
            this.srecommend = 0;
            this.mrecommend = 0;
            this.recommend = 0;

            if (this.custId !== 0) {
                this.custId = 0;
            }

            this.ToggleTabs('none', 'none', 'none', 'none', 4);
            this.Databind(this.supername);
        });

        $("#tabMaster").click((e) => {
            this.viewlevel = 4;
            this.custId = this.mastername === "" ? this.srecommend : 0;
            this.ToggleTabs('block', 'block', 'none', 'none', 3);

            this.Databind(this.mastername);
        });

        $("#tabAgent").click((e) => {
            this.viewlevel = 3;
            this.custId = this.agentname === "" ? this.mrecommend : 0;
            this.ToggleTabs('block', 'block', 'block', 'none', 2);

            this.Databind(this.agentname);
        });
        $("#txtUserName").keypress(e => {
            if (typeof e == "undefined") e = window.event as JQueryEventObject;
            var key = (typeof e.keyCode == "undefined") ? e.which : e.keyCode;
            if (key == 13) {
                this.CallSubmit();
                return true;
            } else {
                return true;
            }
        });
        this.$(".col-sorter").click(event => this.sortTable(this.$(event.currentTarget)));

        Fanex.ExtAcc.OnResize = this.ResizeReportGrid;
        $(".icon-filter").click(() => this.ShowHideFilter());
        $("#notes").click(() => this.ShowInfo());
        Fanex.ExtAcc.OnResize = this.ResizeReportGrid;
    }
    
    ResetParameters() {
        this.srecommend = 0;
        this.mrecommend = 0;
        this.recommend = 0;
        this.custId = 0;
        this.username = '';
        this.viewlevel = 4;
    }

    InitShowHideControls(status) {
        $("#headlineDaily").css("display", status);
        $("#mainGrid").css("display", status);
    }

    ShowHideFilter() {
        if (this.hideFilter) {
            $(".icon-filter").addClass("active");

            $(".filterbox").removeClass("hide");
        } else {
            $(".icon-filter").removeClass("active");
            $(".filterbox").addClass("hide");
        }

        this.ResizeReportGrid();
        this.hideFilter = !this.hideFilter;
    }

    ResizeReportGrid(){
        try {
            $("#jqgrid").fanexGrid("resizeGrid");
        }
        catch (err) { }
    }

    InitCurrencyList() {
        $.ajax({
            context: this,
            url: resolveClientUrl("Filter/CurrencyList"),
            type: "GET",
            data: { groupName: this.groupName },
            dataType: "json",
            cache: false,
            success: (result) => {
                var dataList = [];
                $.each(result, function (i, currency) {
                    dataList.push({
                        id: "rpt_cur_" + currency.CurrencyId,
                        value: currency.CurrencyId,
                        text: currency.CurrencyName,
                        checked: true,
                        enable: true
                    });
                });
                this.$multiSelectCurrency = $("#ddl-currency").fanexMultiSelect({
                    minWidth: 140,
                    checkAllText: "Select all",
                    noneSelectedText: "Select Currencies",
                    useCustomCheckAllText: true,
                    customCheckAllText: "All Currencies",
                    isCheckAll: true,
                    listData: dataList
                });
            }
        });
    }

    ToggleTabs(levelLine, statusSAg, statusMAg, statusAg, status) {
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
    }

    CallSubmit() {
        this.groupName = $("#selectedGroupName").html();
        this.ResetParameters();
        this.username = Fanex.encodeHtml($("#txtUserName").val());
        this.Databind(this.username);
    }

    GetParameterCurrency() {
        var currencyNames = this.$multiSelectCurrency.fanexMultiSelect("getSelectedTexts", true);
        return currencyNames.length ? "&currencies=" + currencyNames.join(",") : "";
    }

    GetParameterBaseCurrency() {
        if ($("#ckbBaseCurrency").prop("checked")) {
            return "&baseCurrency=" + $("#ddl-base-currency").val() + "&baseCurrencyName=" + $("#ddl-base-currency option:selected").text();
        } else {
            return "";
        }
    }

    Databind(username) {
        this.username = !username ? "" : username;
        this.groupName = $("#selectedGroupName").html();
        const dateRange = this.dateRangePickerComponent.getDateRangeValues();
        this.fromdate = dateRange.fromDate;
        this.todate = dateRange.toDate;
        var dateInfo;

        if (dateRange.fromDate == dateRange.toDate) {
            dateInfo = 'on ' + moment(dateRange.fromDate).format('MMM DD, yyyy');
        }
        else {
            dateInfo = 'from ' + moment(dateRange.fromDate).format('MMM DD, yyyy') + ' to ' + moment(dateRange.toDate).format('MMM DD, yyyy');
        }
        var currencyInfo = "";
        if ($("#ckbBaseCurrency").prop("checked")) {
            currencyInfo = " in " + $("#ddl-base-currency option:selected").text();
        } else {
            currencyInfo = " in customer currency";
        }

        var selectedCurrency = this.GetParameterCurrency();
        if (selectedCurrency === '') {
            this.ShowNoData();
            return;
        }
        this.cachedProductAndCurParams = this.productControls.GetParameterProducts()
            + this.productControls.GetParameterProductDetails()
            + selectedCurrency
            + this.GetParameterBaseCurrency();

        var parameterUrl = "?groupid=" + this.groupId
            + "&groupname=" + this.groupName
            + "&srecommend=" + this.srecommend
            + "&mrecommend=" + this.mrecommend
            + "&recommend=" + this.recommend
            + "&custid=" + this.custId
            + "&username=" + Fanex.ExtAcc.ReplaceAll(this.viewlevel == 5 ? "" : this.username, "&", "")
            + "&fromdate=" + this.fromdate
            + "&todate=" + this.todate
            + "&viewlevel=" + this.viewlevel
            + this.cachedProductAndCurParams

        Fanex.ExtAcc.BlockUI();

        this.$.ajax({
            url: this.url + parameterUrl,
            type: "post",
            dataType: "json",
            cache: false,
            success: data => {
                if (data.errorCode) {
                    Fanex.ExtAcc.UnBlockUI();
                    Fanex.ExtAcc.Form.Alert("Error", data.errorMessage.formatErrorMessage(), 350, 150, "alertWLD");
                    return;
                }

                this.fixedsortdata = new Array();
                this.removedsortdata = new Array();
                if (data.rows && data.rows.WinLossDetails) {
                    this.dataSource = data.rows.WinLossDetails;
                    this.totalSource = data.rows.WinLossTotals;
                    this.totalRows = data.rows.WinLossDetails.length;
                } else {
                    this.dataSource = new Array();
                    this.totalSource = new Array();
                    this.totalRows = 0;
                }
                this.fixedsortlist = data.sortingorderlist;

                this.viewlevel = data.viewlevel;
                if (this.viewlevel == 3) {
                    this.srecommend = data.srecommend;
                } else if (this.viewlevel == 2) {
                    this.srecommend = data.srecommend;
                    this.mrecommend = data.mrecommend;
                } else if (this.viewlevel == 1) {
                    this.srecommend = data.srecommend;
                    this.mrecommend = data.mrecommend;
                    this.recommend = data.recommend;
                }

                if (this.custId == 0) {
                    this.supername = data.supername;
                    this.mastername = data.mastername;
                    this.agentname = data.agentname;
                }

                this.ChangeHeadlineTab();

                $("#mainGrid").css("display", this.totalRows < 1 ? "none" : "block");
                $("#mainGrid").css("visibility", this.totalRows < 1 ? "hidden" : "visible");
                $("#headlineDaily").css("display", this.totalRows < 1 ? "none" : "block");
                $("#headlineDaily").css("visibility", this.totalRows < 1 ? "hidden" : "visible");
                $("#msgNoinfo").css("display", this.totalRows < 1 ? "block" : "none");

                if (this.totalRows > 0) {
                    this.Render(dateInfo, currencyInfo);
                    $(".groupBack").removeClass("hide");
                } else {
                    $(".groupBack").addClass("hide");
                }

                $("#exporttoexcelByCur").click(() => {

                    var params = "fromdate=" + this.fromdate +
                        "&todate=" + this.todate +
                        "&viewlevel=" + this.viewlevel +
                        "&custid=" + this.custId +
                        "&groupId=" + this.groupId +
                        "&groupname=" + this.groupName +
                        "&username=" + this.username;
                    var url = resolveClientUrl("WinLossDetailMC/ExportWinLossDetailToExcel?") +
                        params + this.cachedProductAndCurParams;

                    this.topWindow.exportExcel(url, document);
                });

                $("#netTurnover").bind("click",
                    function () {
                        var message =
                            "- For Sportsbook, Net Turnover is Turnover which excludes the draw tickets and halves of the turnover of the half won, half lost tickets.";
                        var height = 170;

                        if (data.tooltips.liveCasino != "") {
                            message +=
                                "<br/>" +
                                "<br/>" +
                                data.tooltips.liveCasino;
                            height += 50;
                        }

                        if (data.tooltips.rngCasino != "") {
                            message +=
                                "<br/>" +
                                "<br/>" +
                                data.tooltips.rngCasino;
                            height += 70;
                        }
                        Fanex.ExtAcc.Form.Alert("Net Turnover Information", message, 430, height);
                        return false;
                    });
                $(".report-username").unbind('click').click((event) => {
                    event.preventDefault();
                    var custID = $(event.currentTarget).data("custid");
                    this.GetDownlineWinLossDetail(custID);
                });
                Fanex.ExtAcc.UnBlockUI();
            }
    });
    }

    ChangeHeadlineTab() {
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
        }
        $("#headlineDaily").css("visibility", "visible");
    }

    Render(dateInfo, currencyInfo) {
        var arrColData = new Array();
        var arrGroupHeaders = new Array();

        var u = this.viewlevel === 2 ? "Agent" : "Member";
        var v = this.viewlevel === 3 ? "Master" : u;
        var level = this.viewlevel === 4 ? "Super" : v;

        var numberOfFrozenColumn = 4;
        var widthDevice = this.topWindow.$(top.window).width();
        if (widthDevice <= 768) {
            numberOfFrozenColumn = 1;
        }

        var titleInfo = {
            text: level + " Win Loss Detail " + dateInfo + currencyInfo,
            customButtons: '<span id="exporttoexcelByCur" title="Export Excel" class="icon-excel"></span>'
        };

        if (this.viewlevel === 1) {
            arrColData = this.AgColumns;
            arrGroupHeaders = this.AgHeaderColumns;
        } else if (this.viewlevel === 2) {
            arrColData = this.MAgColumns;
            arrGroupHeaders = this.MAgHeaderColumns;
        } else if (this.viewlevel === 3) {
            arrColData = this.SAgColumns;
            arrGroupHeaders = this.SAgHeaderColumns;
        } else {
            arrColData = this.DefaultColumns;
            arrGroupHeaders = this.DefaultHeaderColumns;
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
            titleInfo,
            showFooter: true,
            gridExpandHeight: 256,
            showPager: true,
            pagerOption: {
                itemsPerPageArray: [50, 100, 500],
                currentPage: 0,
                totalItem: this.totalRows,
                itemsPerPage: 50,
                afterPagedFunction: () => {
                    $(".report-username").unbind('click').click((event) => {
                        event.preventDefault();
                        var custID = $(event.currentTarget).data("custid");
                        this.GetDownlineWinLossDetail(custID);
                    });
                }
            },
            marginRight: 10,
            bottomBorderWidth: 0,
            showDistinctRow: false,
            allowTruncate: true,
            bodyRows: [],
            footerRows: [],
            sortOption:
                {
                    afterGridSorted: rows => {
                        return this.Customsortfunc(rows);
                    }
                },
            numOfPaddingForGridExpandHeight: 40
        };

        //// Assing BodyData
        this.InitSort();
        var bodyData = [];
        for (var i = 0; i < this.totalRows; i++) {
            if (this.viewlevel === 1) {
                bodyData.push({
                    Columns: [
                        { Name: "Username", Value: this.dataSource[i] },
                        { Name: "Currency", Value: this.dataSource[i].Currency },
                        { Name: "Turnover", Value: this.dataSource[i].Turnover },
                        { Name: "NetTurnover", Value: this.dataSource[i].NetTurnover },
                        { Name: "GrossComm", Value: this.dataSource[i].GrossComm },
                        { Name: "MemberWinlost", Value: this.dataSource[i].MemberWinlost },
                        { Name: "PlayerComm", Value: this.dataSource[i].PlayerComm },
                        { Name: "MemberTotal", Value: this.dataSource[i].MemberTotal },
                        { Name: "AgWinlost", Value: this.dataSource[i].AgWinlost },
                        { Name: "AgComm", Value: this.dataSource[i].AgComm },
                        { Name: "AgTotal", Value: this.dataSource[i].AgTotal },
                        { Name: "Company", Value: this.dataSource[i].Company },
                        { Name: "RewardAmount", Value: this.dataSource[i].RewardAmount },
                        { Name: "RewardCurrency", Value: this.dataSource[i].RewardCurrency }
                    ]
                });
            } else if (this.viewlevel === 2) {
                bodyData.push({
                    Columns: [
                        { Name: "Username", Value: this.dataSource[i] },
                        { Name: "Currency", Value: this.dataSource[i].Currency },
                        { Name: "Turnover", Value: this.dataSource[i].Turnover },
                        { Name: "NetTurnover", Value: this.dataSource[i].NetTurnover },
                        { Name: "GrossComm", Value: this.dataSource[i].GrossComm },
                        { Name: "MemberWinlost", Value: this.dataSource[i].MemberWinlost },
                        { Name: "PlayerComm", Value: this.dataSource[i].PlayerComm },
                        { Name: "MemberTotal", Value: this.dataSource[i].MemberTotal },
                        { Name: "AgWinlost", Value: this.dataSource[i].AgWinlost },
                        { Name: "AgComm", Value: this.dataSource[i].AgComm },
                        { Name: "AgTotal", Value: this.dataSource[i].AgTotal },
                        { Name: "MAgWinlost", Value: this.dataSource[i].MAgWinlost },
                        { Name: "MAgComm", Value: this.dataSource[i].MAgComm },
                        { Name: "MAgTotal", Value: this.dataSource[i].MAgTotal },
                        { Name: "Company", Value: this.dataSource[i].Company },
                        { Name: "RewardAmount", Value: this.dataSource[i].RewardAmount },
                        { Name: "RewardCurrency", Value: this.dataSource[i].RewardCurrency }
                    ]
                });
            } else {
                bodyData.push({
                    Columns: [
                        { Name: "Username", Value: this.dataSource[i] },
                        { Name: "Currency", Value: this.dataSource[i].Currency },
                        { Name: "Turnover", Value: this.dataSource[i].Turnover },
                        { Name: "NetTurnover", Value: this.dataSource[i].NetTurnover },
                        { Name: "GrossComm", Value: this.dataSource[i].GrossComm },
                        { Name: "MemberWinlost", Value: this.dataSource[i].MemberWinlost },
                        { Name: "PlayerComm", Value: this.dataSource[i].PlayerComm },
                        { Name: "MemberTotal", Value: this.dataSource[i].MemberTotal },
                        { Name: "AgWinlost", Value: this.dataSource[i].AgWinlost },
                        { Name: "AgComm", Value: this.dataSource[i].AgComm },
                        { Name: "AgTotal", Value: this.dataSource[i].AgTotal },
                        { Name: "MAgWinlost", Value: this.dataSource[i].MAgWinlost },
                        { Name: "MAgComm", Value: this.dataSource[i].MAgComm },
                        { Name: "MAgTotal", Value: this.dataSource[i].MAgTotal },
                        { Name: "SAgWinlost", Value: this.dataSource[i].SAgWinlost },
                        { Name: "SAgComm", Value: this.dataSource[i].SAgComm },
                        { Name: "SAgTotal", Value: this.dataSource[i].SAgTotal },
                        { Name: "Company", Value: this.dataSource[i].Company },
                        { Name: "RewardAmount", Value: this.dataSource[i].RewardAmount },
                        { Name: "RewardCurrency", Value: this.dataSource[i].RewardCurrency }
                    ]
                });
            }
        }

        gridOption.bodyRows = bodyData;
        gridOption.pagerOption.totalItem = this.totalRows;

        //// Asign Footer Data
        var footerData = [];
        for (var i = 0; i < this.totalSource.length; i++) {
            if (this.viewlevel === 1) {
                footerData.push({
                    Columns: [
                        { Name: "Username", Value: this.totalSource[i].Username, MergeColNumber: 2, CellAlign: "right" },
                        { Name: "Turnover", Value: this.totalSource[i].Turnover },
                        { Name: "NetTurnover", Value: this.totalSource[i].NetTurnover },
                        { Name: "GrossComm", Value: this.totalSource[i].GrossComm },
                        { Name: "MemberWinlost", Value: this.totalSource[i].MemberWinlost },
                        { Name: "PlayerComm", Value: this.totalSource[i].PlayerComm },
                        { Name: "MemberTotal", Value: this.totalSource[i].MemberTotal },
                        { Name: "AgWinlost", Value: this.totalSource[i].AgWinlost },
                        { Name: "AgComm", Value: this.totalSource[i].AgComm },
                        { Name: "AgTotal", Value: this.totalSource[i].AgTotal },
                        { Name: "Company", Value: this.totalSource[i].Company },
                        { Name: "RewardAmount", Value: this.totalSource[i].RewardAmount },
                        { Name: "RewardCurrency", Value: this.totalSource[i].RewardCurrency }
                    ]
                });
            } else if (this.viewlevel === 2) {
                footerData.push({
                    Columns: [
                        { Name: "Username", Value: this.totalSource[i].Username, MergeColNumber: 2, CellAlign: "right" },
                        { Name: "Turnover", Value: this.totalSource[i].Turnover },
                        { Name: "NetTurnover", Value: this.totalSource[i].NetTurnover },
                        { Name: "GrossComm", Value: this.totalSource[i].GrossComm },
                        { Name: "MemberWinlost", Value: this.totalSource[i].MemberWinlost },
                        { Name: "PlayerComm", Value: this.totalSource[i].PlayerComm },
                        { Name: "MemberTotal", Value: this.totalSource[i].MemberTotal },
                        { Name: "AgWinlost", Value: this.totalSource[i].AgWinlost },
                        { Name: "AgComm", Value: this.totalSource[i].AgComm },
                        { Name: "AgTotal", Value: this.totalSource[i].AgTotal },
                        { Name: "MAgWinlost", Value: this.totalSource[i].MAgWinlost },
                        { Name: "MAgComm", Value: this.totalSource[i].MAgComm },
                        { Name: "MAgTotal", Value: this.totalSource[i].MAgTotal },
                        { Name: "Company", Value: this.totalSource[i].Company },
                        { Name: "RewardAmount", Value: this.totalSource[i].RewardAmount },
                        { Name: "RewardCurrency", Value: this.totalSource[i].RewardCurrency }
                    ]
                });
            } else {
                footerData.push({
                    Columns: [
                        { Name: "Username", Value: this.totalSource[i].Username, MergeColNumber: 2, CellAlign: "right" },
                        { Name: "Turnover", Value: this.totalSource[i].Turnover },
                        { Name: "NetTurnover", Value: this.totalSource[i].NetTurnover },
                        { Name: "GrossComm", Value: this.totalSource[i].GrossComm },
                        { Name: "MemberWinlost", Value: this.totalSource[i].MemberWinlost },
                        { Name: "PlayerComm", Value: this.totalSource[i].PlayerComm },
                        { Name: "MemberTotal", Value: this.totalSource[i].MemberTotal },
                        { Name: "AgWinlost", Value: this.totalSource[i].AgWinlost },
                        { Name: "AgComm", Value: this.totalSource[i].AgComm },
                        { Name: "AgTotal", Value: this.totalSource[i].AgTotal },
                        { Name: "MAgWinlost", Value: this.totalSource[i].MAgWinlost },
                        { Name: "MAgComm", Value: this.totalSource[i].MAgComm },
                        { Name: "MAgTotal", Value: this.totalSource[i].MAgTotal },
                        { Name: "SAgWinlost", Value: this.totalSource[i].SAgWinlost },
                        { Name: "SAgComm", Value: this.totalSource[i].SAgComm },
                        { Name: "SAgTotal", Value: this.totalSource[i].SAgTotal },
                        { Name: "Company", Value: this.totalSource[i].Company },
                        { Name: "RewardAmount", Value: this.totalSource[i].RewardAmount },
                        { Name: "RewardCurrency", Value: this.totalSource[i].RewardCurrency }
                    ]
                });
            }
        }


        gridOption.footerRows = footerData;

        setPagerOptionOnMobile(gridOption);

        $("#jqgrid").fanexGrid(gridOption);

        workaroundForGridOnPhone();
    }

    Customsortfunc(rows) {
        var countFixedSort = this.fixedsortlist.length;
        if (countFixedSort > 0) {
            for (var i = 0; i < countFixedSort; i++) {
                for (var j = 0; j < rows.length; j++) {
                    if (this.fixedsortlist[i] == rows[j].Columns[0].Value.CustId) {
                        this.fixedsortdata.push(rows.slice(j, j + 1)[0]);
                        rows.unshift(rows.splice(j, 1)[0]);
                        break;
                    }
                }
            }
        }

        $(".report-username").unbind('click').click((event) => {
            event.preventDefault();
            var custID = $(event.currentTarget).data("custid");
            this.GetDownlineWinLossDetail(custID);
        });

        return rows;
    }

    InitSort() {
        var countFixedSort = this.fixedsortlist.length;
        if (countFixedSort > 0) {
            for (var i = 0; i < countFixedSort; i++) {
                for (var j = 0; j < this.dataSource.length; j++) {
                    if (this.fixedsortlist[i] == this.dataSource[j].CustId) {
                        this.fixedsortdata.push(this.dataSource.slice(j, j + 1)[0]);
                        this.dataSource.unshift(this.dataSource.splice(j, 1)[0]);
                        break;
                    }
                }
            }
        }
    }

    ShowNoData() {
        $("#mainGrid").css("display", "none");
        $("#mainGrid").css("visibility", "hidden");
        $("#headlineDaily").css("display", "none");
        $("#headlineDaily").css("visibility", "hidden");
        $("#msgNoinfo").css("display", "block");
        $(".groupBack").addClass("hide");
    }

    GetDownlineWinLossDetail(custId) {
        event.preventDefault();
        this.custId = custId;
        this.username = "";
        this.Databind(this.username);
    }

    ShowInfo() {
        var message = $("#notes").data("info");
        Fanex.ExtAcc.Form.Alert('Note', message, 400, 200, null, null, 0);
    }
}

$(document).ready(function () {
    const winLossDetailMC = new WinLossDetailMC($, window.top, new DateRangePickerJQueryComponent());
    winLossDetailMC.Init();
});