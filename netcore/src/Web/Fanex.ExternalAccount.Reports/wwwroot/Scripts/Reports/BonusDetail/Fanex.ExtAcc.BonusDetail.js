var BonusDetail = /** @class */ (function () {
    function BonusDetail($) {
        this.cachedPageIndex = 0;
        this.cachedPageSize = 500;
        this.$ = $;
        this.dateTimeControls = new DateTimeControls($);
    }
    BonusDetail.prototype.Init = function () {
        var _this = this;
        this.cacheElements();
        this.bindEvents();
        this.dateTimeControls.Init();
        this.$("#userGroup input")
            .attr("readonly", "true")
            .bind("click", function (event) { return _this.$(event.currentTarget).autocomplete("search", ""); });
        this.initCombobox("#ddl-bonusStatus");
        this.initCombobox("#ddl-memberFeedback");
    };
    BonusDetail.prototype.cacheElements = function () {
        var _this = this;
        this.iconFilter = this.$("#span-iconFilter");
        this.formBonusDetail = this.$("#form-bonusDetail");
        this.gridContainer = this.$("#freegrid-container");
        this.grid = this.$("#jqgrid");
        this.msgNoInfo = this.$("#msgNoinfo");
        this.getBonusDetailURL = this.formBonusDetail.attr("action") + "?";
        var popupContainer = this.$("#div-popupAddNewBonus");
        this.hasHostPermission = popupContainer.length > 0;
        this.groupName = this.$("#selectedGroupName").html();
        if (this.hasHostPermission) {
            this.popupAddNewBonus = popupContainer.clone();
            popupContainer.remove();
            this.cancelBonus = new CancelBonus(this.$, function () { return _this.formBonusDetail.submit(); });
            this.addNewBonus = new AddNewBonus(this.$, function () { return _this.formBonusDetail.submit(); });
        }
    };
    BonusDetail.prototype.bindEvents = function () {
        var _this = this;
        this.iconFilter.click(function () { return _this.showHideFilter(); });
        this.formBonusDetail.submit(function (event) {
            event.preventDefault();
            _this.cachedPageIndex = 0;
            _this.searchBonusDetailQueryString = _this.formBonusDetail.serialize();
            _this.getBonusDetail();
        });
        this.hasHostPermission && this.addNewBonus.RegisterAddNewBonus(this.$("#btn-addNewBonus"), this.popupAddNewBonus);
        Fanex.ExtAcc.OnResize = function () { return _this.resizeReportGrid; };
    };
    BonusDetail.prototype.initCombobox = function (query) {
        var _this = this;
        var $dropdownList = this.$(query).combobox();
        $dropdownList.next("input.ui-autocomplete-input")
            .val($dropdownList.find("option:selected").text())
            .attr("readonly", "true")
            .css("width", "120px")
            .bind("click", function (event) { return _this.$(event.currentTarget).autocomplete("search", ""); });
    };
    BonusDetail.prototype.showHideFilter = function () {
        if (this.iconFilter.hasClass("active")) {
            this.iconFilter.removeClass("active");
            this.formBonusDetail.addClass("hide");
        }
        else {
            this.iconFilter.addClass("active");
            this.formBonusDetail.removeClass("hide");
        }
        this.resizeReportGrid();
    };
    BonusDetail.prototype.resizeReportGrid = function () {
        try {
            this.gridOptions && this.grid.fanexGrid("resizeGrid");
        }
        catch (err) {
            console.log(err);
        }
    };
    BonusDetail.prototype.getBonusDetail = function () {
        var _this = this;
        Fanex.ExtAcc.BlockUI();
        $.getJSON(this.buildGetBonusDetailURL(), function (data) {
            if (data.TotalItems) {
                _this.renderBonusDetail(data);
            }
            else {
                _this.renderNoData();
            }
        }).done(Fanex.ExtAcc.UnBlockUI);
    };
    BonusDetail.prototype.buildGetBonusDetailURL = function () {
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
    };
    BonusDetail.prototype.renderBonusDetail = function (data) {
        var bodyRows = [];
        for (var i = 0, max = data.Bonus.length; i < max; i++) {
            var superBonus = data.Bonus[i];
            var rowMerge = superBonus.Bonus.length;
            for (var j = 0; j < rowMerge; j++) {
                var memberBonus = superBonus.Bonus[j];
                var columns = [
                    { Name: "colCreatedDate", Value: memberBonus.CreatedDate, },
                    { Name: "colRefNo", Value: memberBonus.TransId }
                ];
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
    };
    BonusDetail.prototype.initGridOptions = function () {
        var _this = this;
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
                pagerSelectCallBack: function (pageIndex, pageSize) {
                    _this.cachedPageIndex = pageIndex;
                    _this.cachedPageSize = pageSize;
                    _this.getBonusDetail();
                }
            },
            showFooter: true
        };
    };
    BonusDetail.prototype.buildBonusStatusCell = function (memberBonus) {
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
    };
    BonusDetail.prototype.renderNoData = function () {
        this.gridContainer.removeAttr("style").addClass("hide");
        this.grid.removeAttr("style").html("");
        this.msgNoInfo.css("display", "block");
    };
    return BonusDetail;
}());
$(function () {
    var bonusDetail = new BonusDetail($);
    bonusDetail.Init();
});
//# sourceMappingURL=Fanex.ExtAcc.BonusDetail.js.map