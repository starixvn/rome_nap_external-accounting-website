class BonusDetail {
    private $: JQueryStatic;
    private dateTimeControls: DateTimeControls;
    private iconFilter: JQuery;
    private formBonusDetail: JQuery;
    private gridContainer: JQuery;
    private grid: JQuery;
    private msgNoInfo: JQuery;
    private gridOptions: any;
    private getBonusDetailURL: string;
    private searchBonusDetailQueryString: string;
    private cachedPageIndex = 0;
    private cachedPageSize = 500;
    private groupName: string;
    private popupAddNewBonus: JQuery;
    private hasHostPermission: boolean;
    private cancelBonus: CancelBonus;
    private addNewBonus: AddNewBonus;

    constructor($: JQueryStatic) {
        this.$ = $;
        this.dateTimeControls = new DateTimeControls($);
    }

    Init() {
        this.cacheElements();
        this.bindEvents();

        this.dateTimeControls.Init();
        this.$("#userGroup input")
            .attr("readonly", "true")
            .bind("click", event => this.$(event.currentTarget).autocomplete("search", ""));

        this.initCombobox("#ddl-bonusStatus");
        this.initCombobox("#ddl-memberFeedback");
    }

    private cacheElements() {
        this.iconFilter = this.$("#span-iconFilter");
        this.formBonusDetail = this.$("#form-bonusDetail");
        this.gridContainer = this.$("#freegrid-container");
        this.grid = this.$("#jqgrid");
        this.msgNoInfo = this.$("#msgNoinfo");
        this.getBonusDetailURL = this.formBonusDetail.attr("action") + "?";

        const popupContainer = this.$("#div-popupAddNewBonus");
        this.hasHostPermission = popupContainer.length > 0;
        this.groupName = this.$("#selectedGroupName").html();

        if (this.hasHostPermission) {
            this.popupAddNewBonus = popupContainer.clone();
            popupContainer.remove();

            this.cancelBonus = new CancelBonus(this.$, () => this.formBonusDetail.submit());
            this.addNewBonus = new AddNewBonus(this.$, () => this.formBonusDetail.submit());
        }
    }

    private bindEvents() {
        this.iconFilter.click(() => this.showHideFilter());
        this.formBonusDetail.submit(event => {
            event.preventDefault();

            this.cachedPageIndex = 0;
            this.searchBonusDetailQueryString = this.formBonusDetail.serialize();
            this.getBonusDetail();
        });
        this.hasHostPermission && this.addNewBonus.RegisterAddNewBonus(this.$("#btn-addNewBonus"), this.popupAddNewBonus);
        Fanex.ExtAcc.OnResize = () => this.resizeReportGrid;
    }

    private initCombobox(query: string) {
        const $dropdownList = this.$(query).combobox();
        $dropdownList.next("input.ui-autocomplete-input")
            .val($dropdownList.find("option:selected").text())
            .attr("readonly", "true")
            .css("width", "120px")
            .bind("click", event => this.$(event.currentTarget).autocomplete("search", ""));
    }

    private showHideFilter() {
        if (this.iconFilter.hasClass("active")) {
            this.iconFilter.removeClass("active");
            this.formBonusDetail.addClass("hide");
        } else {
            this.iconFilter.addClass("active");
            this.formBonusDetail.removeClass("hide");
        }

        this.resizeReportGrid();
    }

    private resizeReportGrid() {
        try {
            this.gridOptions && this.grid.fanexGrid("resizeGrid");
        }
        catch (err) {
            console.log(err);
        }
    }

    private getBonusDetail() {
        Fanex.ExtAcc.BlockUI();
        $.getJSON(this.buildGetBonusDetailURL(), data => {
            if (data.TotalItems) {
                this.renderBonusDetail(data);
            } else {
                this.renderNoData();
            }
        }).done(Fanex.ExtAcc.UnBlockUI);
    }

    private buildGetBonusDetailURL(): string {
        return [
            this.getBonusDetailURL,
            this.searchBonusDetailQueryString,
            "&groupName=",
            this.groupName,
            "&pageIndex=",
            this.cachedPageIndex,
            "&pageSize=",
            this.cachedPageSize
        ].join("");
    }

    private renderBonusDetail(data) {
        let bodyRows = [];
        for (let i = 0, max = data.Bonus.length; i < max; i++) {
            const superBonus = data.Bonus[i];
            let rowMerge = superBonus.Bonus.length;

            for (let j = 0; j < rowMerge; j++) {
                const memberBonus = superBonus.Bonus[j];
                let columns: any = [
                    { Name: "colCreatedDate", Value: memberBonus.CreatedDate, },
                    { Name: "colRefNo", Value: memberBonus.TransId }];

                if (j === 0) {
                    columns.push({ Name: "colSuper", Value: superBonus.SuperName, MergeRowNumber: rowMerge });
                    columns.push({ Name: "colSuperAmount", Value: superBonus.TotalAmount, MergeRowNumber: rowMerge });
                }

                columns.push({ Name: "colMember", Value: memberBonus.Username });
                columns.push({ Name: "colMemberAmount", Value: memberBonus.Amount, CssClass: memberBonus.IsCancelled ? "red" : "" });
                columns.push({ Name: "colPaymentDate", Value: memberBonus.PaymentDate, });
                columns.push({ Name: "colBonusStatus", Value: this.buildBonusStatusCell(memberBonus), CssClass: memberBonus.IsCancelled ? "red" : "" });
                columns.push({ Name: "colMemberFeedback", Value: memberBonus.MemberFeedback, CssClass: memberBonus.IsNotReceived ? "red" : "" });

                bodyRows.push({ Columns: columns });
            }
        }

        this.gridOptions = this.gridOptions || this.initGridOptions();
        this.gridOptions.bodyRows = bodyRows;
        this.gridOptions.footerRows = [{
            Columns: [
                { Name: "colCreatedDate", Value: "Grand Total", CssClass: "text-left" },
                { Name: "colRefNo", Value: "" },
                { Name: "colSuper", Value: "" },
                { Name: "colSuperAmount", Value: data.TotalAmount },
                { Name: "colMember", Value: "" },
                { Name: "colMemberAmount", Value: data.TotalAmount },
                { Name: "colPaymentDate", Value: "" },
                { Name: "colBonusStatus", Value: "" },
                { Name: "colMemberFeedback", Value: "" }
            ]
        }];

        this.gridOptions.pagerOption.currentPage = this.cachedPageIndex;
        this.gridOptions.pagerOption.itemsPerPage = this.cachedPageSize;
        this.gridOptions.pagerOption.totalItem = data.TotalItems;

        this.gridContainer.removeClass("hide");
        this.msgNoInfo.removeAttr("style");

        this.grid.fanexGrid(this.gridOptions);
        this.hasHostPermission && this.cancelBonus.RegisterCancelPending(this.grid.find(".cancel-pending"));
    }

    private initGridOptions() {
        return {
            customizeHeader: true,
            groupHeaderRows: [
                {
                    Columns: [
                        { HeaderText: "Created Date", Name: "colCreatedDate", MergeRowNumber: 2 },
                        { HeaderText: "Ref. No.", Name: "colRefNo", MergeRowNumber: 2 },
                        { HeaderText: "Super", Name: "colSuper", MergeColNumber: 2 },
                        { HeaderText: "Member", Name: "colMember", MergeColNumber: 3 },
                        { HeaderText: "Bonus Status", Name: "colBonusStatus", MergeRowNumber: 2 },
                        { HeaderText: "Member Feedback", Name: "colMemberFeedback", MergeRowNumber: 2 }
                    ]
                }
            ],
            columns: [
                { HeaderText: "Created Date", Width: 140, Name: "colCreatedDate", CellAlign: "center" },
                { HeaderText: "Ref. No.", Width: 100, Name: "colRefNo", HeaderAlign: "center", CellAlign: "right" },
                { HeaderText: "Username", Width: 160, Name: "colSuper", AllowTruncate: true },
                { HeaderText: "Amount", Width: 120, Name: "colSuperAmount", AllowTruncate: true, HeaderAlign: "center", CellAlign: "right", FormatNumber: "0,0.00" },
                { HeaderText: "Username", Width: 160, Name: "colMember", AllowTruncate: true },
                { HeaderText: "Amount", Width: 120, Name: "colMemberAmount", AllowTruncate: true, HeaderAlign: "center", CellAlign: "right", FormatNumber: "0,0.00" },
                { HeaderText: "Payment Date", Width: 100, Name: "colPaymentDate", CellAlign: "center" },
                { HeaderText: "Bonus Status", Width: 100, Name: "colBonusStatus", CellAlign: "center" },
                { HeaderText: "Member Feedback", Width: 120, Name: "colMemberFeedback", CellAlign: "center" }
            ],
            autoCalculateGridExpandHeight: true,
            fullHeight: !isMobile.phone,
            gridHeight: isMobile.phone ? $(window).height() - 70 : 0,
            gridExpandHeight: this.hasHostPermission ? 255 : 200,
            showDistinctRow: false,
            showPager: true,
            customPager: true,
            pagerOption: {
                itemsPerPageArray: [50, 100, 500],
                pagerSelectCallBack: (pageIndex, pageSize) => {
                    this.cachedPageIndex = pageIndex;
                    this.cachedPageSize = pageSize;
                    this.getBonusDetail();
                }
            },
            showFooter: true
        };
    }

    private buildBonusStatusCell(memberBonus) {
        if (this.hasHostPermission && memberBonus.CanCancelPending) {
            return [
                '<a href="javascript:;" class="cancel-pending" data-custid="',
                memberBonus.CustId,
                '" data-transid="',
                memberBonus.TransId,
                '">',
                memberBonus.BonusStatus,
                '</a>'
            ].join("");
        }

        return memberBonus.BonusStatus;
    }

    private renderNoData() {
        this.gridContainer.removeAttr("style").addClass("hide");
        this.grid.removeAttr("style").html("");
        this.msgNoInfo.css("display", "block");
    }
}

$(function () {
    const bonusDetail = new BonusDetail($);
    bonusDetail.Init();
});
