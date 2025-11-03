(function ($) {
    $.widget("ui.combobox", {
        _create: function () {
            var self = this;
            var select = this.element.hide();
            var wasOpen = true; 
            var input = $("<input>")
				.insertAfter(select)
				.autocomplete({
                    source: function (request, response) {
                        var matcher;

                        try {
                             matcher = new RegExp(request.term, "i");
                        } catch (e) {
                            matcher = new RegExp(request.term.replace(/[-\/\\^$*+?.()|[\]{}]/g, '\\$&'), "i");
                        }

				        response(select.children("option").map(function () {
				            var text = $(this).text();
				            if (this.value && (!request.term || matcher.test(text)))
				                return {
				                    id: this.value,
				                    label: text, //text.replace(new RegExp("(?![^&;]+;)(?!<[^<>]*)(" + $.ui.autocomplete.escapeRegex(request.term) + ")(?![^<>]*>)(?![^&;]+;)", "gi"), "<strong>$1</strong>"),
				                    value: text
				                };
				        }));
				    },
				    delay: 0,
				    select: function (event, ui) {
				        if (!ui.item) {
				            // remove invalid value, as it didn't match anything
				            $(this).val("");
				            return false;
				        }
				        select.val(ui.item.id);
				        self._trigger("selected", event, {
				            item: select.find("[value='" + ui.item.id + "']")
				        });
				    },
				    minLength: 0
				})
				.addClass("ui-widget ui-widget-content ui-corner-left");
            $("<button type='button'>▾</button>")
			.attr("tabIndex", -1)
			.attr("title", "Show All Items")
			.insertAfter(input)
			.button({
			    icons: {
			        primary: "ui-icon-triangle-1-s"
			    },
			    text: false
			}).removeClass("ui-corner-all")
			.addClass("ui-corner-right ui-button-icon")
			.mousedown(function () {
			    wasOpen = input.autocomplete("widget").is(":visible");
			})
            .mouseenter(function () {
                wasOpen = input.autocomplete("widget").is(":visible");
            })
            .click(function () {
                input.focus();
                // Close if already visible
                if (wasOpen) {
                    return;
                }
                // Pass empty string as value to search for, displaying all results
                input.autocomplete("search", "");
            });
        }
    });

})(jQuery);
var ProductControls = /** @class */ (function () {
    function ProductControls($) {
        this.$ = $;
        this.productDetailsData = [];
    }
    ProductControls.prototype.Init = function () {
        var _this = this;
        this.$.ajax({
            url: resolveClientUrl("Filter/ProductsList"),
            type: "GET",
            data: { groupName: $("#selectedGroupName").html(), groupId: Fanex.ExtAcc.Customer.GetGroupId() || '', isWinLossDetail: false },
            dataType: "json",
            cache: false,
            success: function (data) { return _this.OnSuccessLoadProduct(data); }
        });
    };
    ProductControls.prototype.OnSuccessLoadProduct = function (data) {
        var productDetails = data.productDetails;
        if (productDetails.length) {
            this.InitProductDetailsList(productDetails);
        }
        this.InitProductsList(data.products);
    };
    ProductControls.prototype.InitProductDetailsList = function (productDetails) {
        var self = this;
        var dataList = [];
        this.productDetailsDataMapToProductId = {};
        this.$.each(productDetails, function (i, item) {
            dataList.push({
                id: item.ResourceId,
                value: item.ResourceId,
                text: item.DisplayName,
                checked: true,
                enable: true
            });
            if (self.productDetailsDataMapToProductId.hasOwnProperty(item.ProductId)) {
                self.productDetailsDataMapToProductId[item.ProductId].push(item.ResourceId);
            }
            else {
                self.productDetailsDataMapToProductId[item.ProductId] = [item.ResourceId];
            }
        });
        this.$multiSelectProductDetail = this.$("#ddl-product-detail").fanexMultiSelect({
            minWidth: 220,
            checkAllText: "Select all",
            noneSelectedText: "Select none",
            useCustomCheckAllText: true,
            customCheckAllText: "Select all",
            isCheckAll: true,
            listData: dataList
        });
        $("#ddl-product-detail-container").removeClass("hidden");
        this.productDetailsData = productDetails;
    };
    ProductControls.prototype.InitProductsList = function (products) {
        var self = this;
        var $ddlProductDetail = this.$("#ddl-product-detail");
        //const $checkAllProductDetail = $("#ddl-product-detailHeaderCheckBox");
        var buildProductData = function (productList) {
            var dataList = [];
            self.$.each(productList, function (i, product) {
                dataList.push({
                    id: product.ProductId,
                    value: product.ProductId,
                    text: product.ProductName,
                    checked: true,
                    enable: true,
                    items: product.IsParent ? buildProductData(product.Products) : []
                });
            });
            return dataList;
        };
        this.$multiSelectProduct = this.$("#ddl-product").fanexMultiSelect({
            checkAllText: "Select all",
            noneSelectedText: "Select Products",
            useCustomCheckAllText: true,
            customCheckAllText: "All Products",
            isCheckAll: true,
            listData: buildProductData(products),
            onCheck: function () {
                if ($ddlProductDetail.length) {
                    var checked = this.checked;
                    var productId = parseInt(this.value);
                    if (isNaN(productId)) { // Select all
                        self.ProductsListOnCheckAll(checked, $ddlProductDetail);
                    }
                    else {
                        var $groupCheckbox = $(this);
                        if ($groupCheckbox.data("is-group")) {
                            self.ProductsListOnCheckGroup(checked, $ddlProductDetail, $groupCheckbox);
                        }
                        else {
                            self.ProductsListOnCheck(checked, productId, $ddlProductDetail);
                        }
                    }
                }
            }
        });
    };
    ProductControls.prototype.ProductsListOnCheckAll = function (checked, $ddlProductDetail) {
        $ddlProductDetail.fanexMultiSelect("setCheckAll", checked);
        $ddlProductDetail.fanexMultiSelect("setEnable", checked);
        var optionsElement = $ddlProductDetail.next(".fanex-multiselect-options");
        $.each(optionsElement.find('input[type="checkbox"].show-item'), function () {
            if ($(this).val) {
                $(this).prop("checked", checked);
                $(this).prop("disabled", !checked);
                $(this).trigger('change');
            }
            if ($(this).is("[disabled]")) {
                $(this).parent().addClass("hide-item");
            }
            else {
                $(this).parent().removeClass("hide-item");
            }
        });
    };
    ProductControls.prototype.ProductsListOnCheckGroup = function (checked, $ddlProductDetail, $groupCheckbox) {
        var self = this;
        $groupCheckbox
            .closest(".fanex-multiselect-div")
            .next(".fanex-multiselect-ul-group")
            .find('input[type="checkbox"]')
            .each(function () {
            self.ProductsListOnCheck(checked, parseInt(this.value), $ddlProductDetail);
        });
    };
    ProductControls.prototype.ProductsListOnCheck = function (checked, productId, $ddlProductDetail) {
        var filteredProductDetails = this.productDetailsData.filter(function (p) {
            if (p.ProductId === productId) {
                return p;
            }
        });
        if (!filteredProductDetails.length) {
            return;
        }
        var optionsElement = $ddlProductDetail.next(".fanex-multiselect-options");
        $.each(filteredProductDetails, function (index, product) {
            optionsElement
                .find('input[type="checkbox"][value="' + product.ResourceId + '"]')
                .prop("checked", checked)
                .prop("disabled", !checked)
                .trigger('change');
            $.each(optionsElement.find('input[type="checkbox"].show-item'), function () {
                if ($(this).is("[disabled]")) {
                    $(this).parent().addClass("hide-item");
                }
                else {
                    $(this).parent().removeClass("hide-item");
                }
            });
        });
        var maxAnabledItem = optionsElement.find('input[type="checkbox"].show-item:enabled').length;
        var checkedItem = optionsElement.find('input[type="checkbox"].show-item:enabled:checked').length;
        if (maxAnabledItem === checkedItem) {
            $("#ddl-product-detailHeaderCheckBox").prop("checked", true);
        }
        var enabledCheckboxes = optionsElement.find('input[type="checkbox"]:enabled').length - 1;
        if (checked) {
            $ddlProductDetail.fanexMultiSelect("setEnable", checked);
        }
        else if (!enabledCheckboxes) {
            $ddlProductDetail.fanexMultiSelect("setEnable", checked);
        }
    };
    ProductControls.prototype.GetParameterProducts = function () {
        var productIds = this.GetProductIds();
        return productIds.length ? "&productIds=" + productIds.join(",") : "";
    };
    ProductControls.prototype.GetParameterProductDetails = function () {
        if (this.$multiSelectProductDetail) {
            var resourceIds = this.$multiSelectProductDetail.fanexMultiSelect("getItems", true);
            return resourceIds.length ? "&resourceIds=" + resourceIds.join(",") : "";
        }
        return "";
    };
    ProductControls.prototype.GetProductIds = function () {
        this.UpdateUncheckedProductsDropdown();
        return this.$multiSelectProduct.fanexMultiSelect("getItems", true);
    };
    ProductControls.prototype.GetResourceIds = function () {
        return this.$multiSelectProductDetail
            ? this.$multiSelectProductDetail.fanexMultiSelect("getItems", true)
            : [];
    };
    ProductControls.prototype.UpdateUncheckedProductsDropdown = function () {
        if (this.productDetailsDataMapToProductId) {
            var self_1 = this;
            Object.keys(self_1.productDetailsDataMapToProductId).forEach(function (productId) {
                if (self_1.IsProductDetailsUnChecked(self_1.productDetailsDataMapToProductId[productId])) {
                    $('input[id^="fanex-multiselect-ddl-product-' + productId + '-"]').prop("checked", false).trigger("change");
                }
            });
        }
    };
    ProductControls.prototype.IsProductDetailsUnChecked = function (resourceIds) {
        for (var i = 0, max = resourceIds.length; i < max; i++) {
            if ($('input[id^="fanex-multiselect-ddl-product-detail-' + resourceIds[i] + '-"]').is(":checked")) {
                return false;
            }
        }
        return true;
    };
    return ProductControls;
}());
//# sourceMappingURL=ProductControls.js.map
"use strict";
var DateRangePickerJQueryOption = /** @class */ (function () {
    function DateRangePickerJQueryOption(onClosed, language, selectorId) {
        if (onClosed === void 0) { onClosed = Function(); }
        if (language === void 0) { language = "default"; }
        if (selectorId === void 0) { selectorId = "daterange-picker"; }
        this.onClosed = onClosed;
        this.language = language;
        this.selectorId = selectorId;
        this.onClosed = onClosed;
        this.language = language;
        this.selectorId = selectorId;
    }
    return DateRangePickerJQueryOption;
}());
var DateRangePickerJQueryComponent = /** @class */ (function () {
    function DateRangePickerJQueryComponent() {
    }
    DateRangePickerJQueryComponent.prototype.render = function (options) {
        var _this = this;
        var dateRangePicker = $("#" + options.selectorId);
        this.historicPivotDateText = dateRangePicker.data("historicpivotdate");
        var historicPivotDate = moment(this.historicPivotDateText, "MM/dd/yyyy");
        this.fromDate = dateRangePicker.data("fromdate");
        this.toDate = dateRangePicker.data("todate");
        this.historyMaxDateRange = dateRangePicker.data("historymaxdaterange");
        this.historyMaxMonthRange = dateRangePicker.data("historymaxmonthrange");
        dateRangePicker
            .dateRangePicker({
            format: "MM/DD/YYYY",
            separator: " - ",
            language: options.language,
            startDate: dateRangePicker.data("mindate"),
            endDate: dateRangePicker.data("maxdate"),
            showShortcuts: true,
            shortcuts: null,
            customShortcuts: this.getPredefinedRanges(dateRangePicker),
            duration: 0,
            singleMonth: true,
            monthSelect: true,
            yearSelect: true
        }).bind("datepicker-change", function (_, obj) {
            var startDate = moment(obj.date1, "MM/dd/yyyy");
            var endDate = moment(obj.date2, "MM/dd/yyyy");
            if (endDate.isBefore(historicPivotDate)
                && (_this.historyMaxDateRange != undefined && endDate.diff(startDate, "d") > _this.historyMaxDateRange
                    || _this.historyMaxMonthRange != undefined && endDate.diff(moment(obj.date1, "MM/dd/yyyy").add(_this.historyMaxMonthRange, "months"), "d") > 0)) {
                try {
                    Fanex.ExtAcc.Form.Alert('Date Range Error', dateRangePicker.data("label-maxdataretrievehistoryerror"), 400, 150, null, null, 0);
                }
                catch (e) {
                    alert(dateRangePicker.data("label-maxdataretrievehistoryerror"));
                }
                dateRangePicker.data("dateRangePicker").setDateRange(_this.fromDate, _this.toDate, false);
                return;
            }
            if (startDate.isBefore(historicPivotDate) && endDate.isSameOrAfter(historicPivotDate)
                && (_this.historyMaxDateRange != undefined && endDate.diff(startDate, "d") > _this.historyMaxDateRange
                    || _this.historyMaxMonthRange != undefined && endDate.diff(moment(obj.date1, "MM/dd/yyyy").add(_this.historyMaxMonthRange, "months"), "d") > 0)) {
                try {
                    Fanex.ExtAcc.Form.Alert('Date Range Error', dateRangePicker.data("label-maxdataretrievehistoryerror"), 400, 150, null, null, 0);
                }
                catch (e) {
                    alert(dateRangePicker.data("label-maxdataretrievehistoryerror"));
                }
                dateRangePicker.data("dateRangePicker").setDateRange(_this.fromDate, _this.toDate, false);
                return;
            }
            _this.fromDate = startDate.format("MM/DD/YYYY");
            _this.toDate = endDate.format("MM/DD/YYYY");
        })
            .bind("datepicker-closed", function () { return options.onClosed(_this.getDateRangeValues()); })
            .data("dateRangePicker").setDateRange(this.fromDate, this.toDate, false);
    };
    DateRangePickerJQueryComponent.prototype.getPredefinedRanges = function (dateRangePicker) {
        var minDate = moment(dateRangePicker.data("mindate"));
        var today = moment(dateRangePicker.data("currentdate"));
        var todayValue = today.toDate();
        var yesterday = moment(today).subtract(1, "d");
        var thisWeekStartOfDay = moment(today).startOf("isoWeek");
        var lastMonthStartOfDay = moment(today).subtract(1, "M").startOf("M");
        return [
            {
                name: dateRangePicker.data("label-today"),
                dates: function () {
                    return [todayValue, todayValue];
                }
            },
            {
                name: dateRangePicker.data("label-yesterday"),
                dates: function () {
                    var yesterdayValue = yesterday.toDate();
                    return [yesterdayValue, yesterdayValue];
                }
            },
            {
                name: dateRangePicker.data("label-thisweek"),
                dates: function () {
                    return [
                        thisWeekStartOfDay.diff(minDate) >= 0 ? thisWeekStartOfDay.toDate() : minDate.toDate(),
                        todayValue
                    ];
                }
            },
            {
                name: dateRangePicker.data("label-lastweek"),
                dates: function () {
                    var lastWeekStartOfDay = moment(thisWeekStartOfDay).subtract(1, "w");
                    return [
                        lastWeekStartOfDay.diff(minDate) >= 0 ? lastWeekStartOfDay.toDate() : minDate.toDate(),
                        moment(thisWeekStartOfDay).subtract(1, "d").toDate()
                    ];
                }
            },
            {
                name: dateRangePicker.data("label-thismonth"),
                dates: function () {
                    var thisMonthStartOfDay = moment(today).startOf("M");
                    return [
                        thisMonthStartOfDay.diff(minDate) >= 0 ? thisMonthStartOfDay.toDate() : minDate.toDate(),
                        todayValue
                    ];
                }
            },
            {
                name: dateRangePicker.data("label-lastmonth"),
                dates: function () {
                    return [
                        lastMonthStartOfDay.diff(minDate) >= 0 ? lastMonthStartOfDay.toDate() : minDate.toDate(),
                        moment(lastMonthStartOfDay).endOf("M").toDate()
                    ];
                }
            },
            {
                name: dateRangePicker.data("label-sincelastmonth"),
                dates: function () {
                    return [
                        lastMonthStartOfDay.diff(minDate) >= 0 ? lastMonthStartOfDay.toDate() : minDate.toDate(),
                        todayValue
                    ];
                }
            }
        ];
    };
    DateRangePickerJQueryComponent.prototype.getDateRangeValues = function () {
        return { fromDate: this.fromDate, toDate: this.toDate };
    };
    DateRangePickerJQueryComponent.prototype.getHistoricPivotDate = function () {
        return this.historicPivotDateText;
    };
    return DateRangePickerJQueryComponent;
}());
//# sourceMappingURL=daterangepicker-jquery.js.map
var SuperWinLossMCDetailByPT = /** @class */ (function () {
    function SuperWinLossMCDetailByPT($, topWindow, dateRangePickerComponent) {
        this.fromdate = moment(new Date()).format("MM/DD/YYYY");
        this.todate = moment(new Date()).format("MM/DD/YYYY");
        this.cachedProductAndCurParams = "";
        this.totalRows = 0;
        this.$ = $;
        this.topWindow = topWindow;
        this.fixedsortdata = [];
        this.fixedsortlist = [];
        this.hideFilter = false;
        this.productControls = new ProductControls($);
        this.dateRangePickerComponent = dateRangePickerComponent;
    }
    SuperWinLossMCDetailByPT.prototype.Init = function () {
        var _this = this;
        this.dateRangePickerComponent.render(new DateRangePickerJQueryOption());
        this.productControls.Init();
        this.InitCurrencyList();
        $("#ddl-base-currency").combobox();
        $("#base-currency input").attr('readonly', 'true').val($("#ddl-base-currency")[0].options[0].text).bind("click", function () {
            $(this).autocomplete("search", "");
        });
        $("#ckbBaseCurrency").change(function () {
            $("#base-currency").find("input, button").prop("disabled", !$("#ckbBaseCurrency").is(":checked"));
        });
        this.$(".icon-filter").click(function () { return _this.ShowHideFilter(); });
        this.$("#userGroup input").attr("readonly", "true");
        this.$("#userGroup input").bind("click", function (event) { return _this.$(event.currentTarget).autocomplete("search", ""); });
        this.$("#Submit").click(function () { return _this.CallSubmit(); });
        $("#notes").click(function () { return _this.ShowInfo(); });
        this.$(".col-sorter").click(function (event) { return _this.sortTable(_this.$(event.currentTarget)); });
        Fanex.ExtAcc.OnResize = this.ResizeReportGrid;
    };
    SuperWinLossMCDetailByPT.prototype.InitCurrencyList = function () {
        var _this = this;
        $.ajax({
            context: this,
            url: resolveClientUrl("Filter/CurrencyList"),
            type: "GET",
            data: { groupName: $("#selectedGroupName").html() },
            dataType: "json",
            cache: false,
            success: function (result) {
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
                _this.$multiSelectCurrency = $("#ddl-currency").fanexMultiSelect({
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
    };
    SuperWinLossMCDetailByPT.prototype.BuildDefaultColumns = function () {
        return [
            { HeaderText: "Username", Name: "Username", Width: 130, Sort: true, HeaderAlign: "center", CellAlign: "left", ShowCollapseExpand: true },
            { HeaderText: "Currency Name", Name: "Currency", Width: 130, Sort: true, HeaderAlign: "center", CellAlign: "left" },
            { HeaderText: "Turnover", Name: "Turnover", Width: 130, Sort: true, SortDirection: 'desc', HeaderAlign: "center", CellAlign: "right", FormatNumber: "0,0.00", FormatNegative: "$" },
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
    };
    SuperWinLossMCDetailByPT.prototype.BuildDefaultHeaderColumns = function () {
        return [
            { HeaderText: "Username", Name: "Username", MergeRowNumber: 2 },
            { HeaderText: "Currency Name", Name: "Currency", MergeRowNumber: 2 },
            { HeaderText: "Turnover", Name: "Turnover", MergeRowNumber: 2 },
            { HeaderText: "Net Turnover&nbsp;<span id=\"netTurnover\" class=\"bet-type-detail-icon\"></span>", Name: "NetTurnover", MergeRowNumber: 2 },
            { HeaderText: "GrossComm", Name: "GrossComm", MergeRowNumber: 2 },
            { HeaderText: "Member", Name: "MemberWinlost", MergeColNumber: 3 },
            { HeaderText: "Agent", Name: "AgWinlost", MergeColNumber: 3 },
            { HeaderText: "Master", Name: "MAgWinlost", MergeColNumber: 3 },
            { HeaderText: "Super", Name: "SAgWinlost", MergeColNumber: 3 },
            { HeaderText: "Company", Name: "Company", MergeRowNumber: 2 }
        ];
    };
    SuperWinLossMCDetailByPT.prototype.BuildSubRowData = function (data) {
        var _this = this;
        return {
            Columns: [
                {
                    HeaderText: "Username", Name: "Username",
                    Value: data,
                    CustomCellRender: function (cell) { return _this.RenderPositonTakingColumn(cell); },
                    MergeColNumber: 2
                },
                { Name: "Turnover", Value: data.Turnover },
                { Name: "NetTurnover", Value: data.NetTurnover },
                { Name: "GrossComm", Value: data.GrossComm },
                { Name: "MemberWinlost", Value: data.MemberWinlost },
                { Name: "PlayerComm", Value: data.MemberComm },
                { Name: "MemberTotal", Value: data.MemberTotal },
                { Name: "AgWinlost", Value: data.AgWinlost },
                { Name: "AgComm", Value: data.AgComm },
                { Name: "AgTotal", Value: data.AgTotal },
                { Name: "MAgWinlost", Value: data.MAgWinlost },
                { Name: "MAgComm", Value: data.MAgComm },
                { Name: "MAgTotal", Value: data.MAgTotal },
                { Name: "SAgWinlost", Value: data.SAgWinlost },
                { Name: "SAgComm", Value: data.SAgComm },
                { Name: "SAgTotal", Value: data.SAgTotal },
                { Name: "Company", Value: data.Company }
            ]
        };
    };
    SuperWinLossMCDetailByPT.prototype.BuildBodyData = function (dataSource) {
        var bodyData = [];
        this.totalRows = dataSource.length;
        for (var i = 0; i < dataSource.length; i++) {
            var subRows = [];
            for (var j = 0; j < dataSource[i].WinLossByProducts.length; j++) {
                subRows.push(this.BuildSubRowData(dataSource[i].WinLossByProducts[j]));
            }
            var totalRow = dataSource[i].Total;
            bodyData.push({
                Columns: [
                    { Name: "Username", Value: totalRow.Username },
                    { Name: "Currency", Value: totalRow.CurrencyName, CellAlign: "center" },
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
                SubRows: subRows,
                CssClass: 'sub-row-class'
            });
        }
        return bodyData;
    };
    SuperWinLossMCDetailByPT.prototype.BuildFooterData = function (dataSource) {
        var footerData = [];
        for (var i = 0; i < dataSource.length; i++) {
            var grandTotal = dataSource[i];
            footerData.push({
                Columns: [
                    { Name: "Username", Value: grandTotal.TotalName, MergeColNumber: 2, CellAlign: "right" },
                    { Name: "Turnover", Value: grandTotal.Turnover },
                    { Name: "NetTurnover", Value: grandTotal.NetTurnover },
                    { Name: "GrossComm", Value: grandTotal.GrossComm },
                    { Name: "MemberWinlost", Value: grandTotal.MemberWinlost },
                    { Name: "PlayerComm", Value: grandTotal.MemberComm },
                    { Name: "MemberTotal", Value: grandTotal.MemberTotal },
                    { Name: "AgWinlost", Value: grandTotal.AgWinlost },
                    { Name: "AgComm", Value: grandTotal.AgComm },
                    { Name: "AgTotal", Value: grandTotal.AgTotal },
                    { Name: "MAgWinlost", Value: grandTotal.MAgWinlost },
                    { Name: "MAgComm", Value: grandTotal.MAgComm },
                    { Name: "MAgTotal", Value: grandTotal.MAgTotal },
                    { Name: "SAgWinlost", Value: grandTotal.SAgWinlost },
                    { Name: "SAgComm", Value: grandTotal.SAgComm },
                    { Name: "SAgTotal", Value: grandTotal.SAgTotal },
                    { Name: "Company", Value: grandTotal.Company }
                ]
            });
        }
        return footerData;
    };
    SuperWinLossMCDetailByPT.prototype.RenderPositonTakingColumn = function (cell) {
        var tooltip = '';
        this.$.each(cell.Value.ProductNames, function (index, value) {
            tooltip += "<div>" + value + "</div>";
        });
        return "<span class=\"product-tooltip\" title=\"" + tooltip + "\">" + cell.Value.PositionTakingText + "<a> (" + cell.Value.ProductNames.length + ")</a></span>";
    };
    SuperWinLossMCDetailByPT.prototype.Render = function (data) {
        var _this = this;
        var dateInfo = this.fromdate === this.todate ?
            "on " + moment(this.fromdate).format('MMM DD, yyyy') :
            "from " + moment(this.fromdate).format('MMM DD, yyyy') + " to " + moment(this.todate).format('MMM DD, yyyy');
        var currencyInfo = "";
        if ($("#ckbBaseCurrency").prop("checked")) {
            currencyInfo = " in " + $("#ddl-base-currency option:selected").text();
        }
        else {
            currencyInfo = " in customer currency";
        }
        var gridOption = {
            bodyRows: this.BuildBodyData(data.WinLossByUsernames),
            footerRows: this.BuildFooterData(data.TotalByCurrencies),
            customizeHeader: true,
            groupHeaderRows: [{ Columns: this.BuildDefaultHeaderColumns() }],
            columns: this.BuildDefaultColumns(),
            numberOfFrozenColumn: this.topWindow.$(top.window).width() < 768 ? 1 : 4,
            autoCalculateGridExpandHeight: true,
            fullHeight: !isMobile.phone,
            gridHeight: isMobile.phone ? $(window).height() - 70 : 0,
            showTitle: true,
            titleInfo: {
                text: "Super Win Loss Detail By PT " + dateInfo + currencyInfo,
                customButtons: '<span id="exporttoexcelByCur" title="Export Excel" class="icon-excel"></span>'
            },
            showFooter: true,
            showTree: true,
            gridExpandHeight: 256,
            showPager: true,
            pagerOption: {
                itemsPerPageArray: [50, 100, 500],
                currentPage: 0,
                totalItem: this.totalRows,
                itemsPerPage: 50
            },
            marginRight: 10,
            bottomBorderWidth: 0,
            showDistinctRow: false,
            allowTruncate: true,
            sortOption: {
                afterGridSorted: function (rows) { return _this.Customsortfunc(rows); }
            },
            numOfPaddingForGridExpandHeight: 40
        };
        this.InitSort(data.WinLossByUsernames);
        setPagerOptionOnMobile(gridOption);
        this.$("#jqgrid").fanexGrid(gridOption);
        this.$(".product-tooltip").tooltip();
        this.$("#netTurnover").unbind().bind("click", function () { return _this.ShowNetTurnoverInfo(data); });
        $("#exporttoexcelByCur").click(function () { return _this.ExportExcel(); });
        workaroundForGridOnPhone();
    };
    SuperWinLossMCDetailByPT.prototype.CustomUsernameSortFunction = function (cell1Value, cell2Value, sortDirection) {
        return sortDirection === 'asc' ?
            cell1Value.Username > cell2Value.Username ? 1 : -1 :
            cell1Value.Username < cell2Value.Username ? 1 : -1;
    };
    SuperWinLossMCDetailByPT.prototype.Customsortfunc = function (rows) {
        var countFixedSort = this.fixedsortlist.length;
        if (countFixedSort > 0) {
            for (var i = 0; i < countFixedSort; i++) {
                for (var j = 0; j < rows.length; j++) {
                    if (this.fixedsortlist[i] === rows[j].Columns[0].Value.Turnover) {
                        this.fixedsortdata.push(rows.slice(j, j + 1)[0]);
                        rows.unshift(rows.splice(j, 1)[0]);
                        break;
                    }
                }
            }
        }
        this.$(".product-tooltip").tooltip();
        return rows;
    };
    SuperWinLossMCDetailByPT.prototype.InitSort = function (dataSource) {
        var countFixedSort = this.fixedsortlist.length;
        if (countFixedSort > 0) {
            for (var i = 0; i < countFixedSort; i++) {
                for (var j = 0; j < dataSource.length; j++) {
                    if (this.fixedsortlist[i] === dataSource[j].Turnover) {
                        this.fixedsortdata.push(dataSource.slice(j, j + 1)[0]);
                        dataSource.unshift(dataSource.splice(j, 1)[0]);
                        break;
                    }
                }
            }
        }
    };
    SuperWinLossMCDetailByPT.prototype.ShowNetTurnoverInfo = function (data) {
        var message = "- For Sportsbook, Net Turnover is Turnover which excludes the draw tickets and halves of the turnover of the half won, half lost tickets.";
        var height = 170;
        if (data.TooltipsForLiveCasino !== "") {
            message += "<br/><br/>" + data.TooltipsForLiveCasino;
            height += 50;
        }
        if (data.TooltipsForRNGCasino !== "") {
            message += "<br/><br/>" + data.TooltipsForRNGCasino;
            height += 70;
        }
        Fanex.ExtAcc.Form.Alert("Net Turnover Information", message, 430, height);
    };
    SuperWinLossMCDetailByPT.prototype.GetParameterCurrency = function () {
        var currencyNames = this.$multiSelectCurrency.fanexMultiSelect("getSelectedTexts", true);
        return currencyNames.length ? "&currencies=" + currencyNames.join(",") : "";
    };
    SuperWinLossMCDetailByPT.prototype.GetParameterBaseCurrency = function () {
        if ($("#ckbBaseCurrency").prop("checked")) {
            return "&baseCurrency=" + $("#ddl-base-currency").val() + "&baseCurrencyName=" + $("#ddl-base-currency option:selected").text();
        }
        else {
            return "";
        }
    };
    SuperWinLossMCDetailByPT.prototype.CallSubmit = function () {
        var _this = this;
        var dateRange = this.dateRangePickerComponent.getDateRangeValues();
        this.fromdate = dateRange.fromDate;
        this.todate = dateRange.toDate;
        var selectedCurrency = this.GetParameterCurrency();
        if (selectedCurrency === '') {
            this.ShowNoData();
            return;
        }
        this.cachedProductAndCurParams = this.productControls.GetParameterProducts()
            + this.productControls.GetParameterProductDetails()
            + selectedCurrency
            + this.GetParameterBaseCurrency();
        var parameterUrl = "?Groupname=" + $("#selectedGroupName").html() + "&GroupId=" + (Fanex.ExtAcc.Customer.GetGroupId() || '')
            + ("&Fromdate=" + this.fromdate + "&Todate=" + this.todate)
            + this.cachedProductAndCurParams;
        Fanex.ExtAcc.BlockUI();
        this.$.ajax({
            url: resolveClientUrl("SuperWinLossDetailMCByPT/GetData") + parameterUrl,
            type: "post",
            dataType: "json",
            cache: false,
            success: function (data) { return _this.OnSuccess(data); }
        });
    };
    SuperWinLossMCDetailByPT.prototype.OnSuccess = function (data) {
        if (data.errorCode) {
            Fanex.ExtAcc.UnBlockUI();
            Fanex.ExtAcc.Form.Alert("Error", data.errorMessage.formatErrorMessage(), 350, 150, "alertWLD");
            return;
        }
        var existedData = data.WinLossByUsernames !== null && data.WinLossByUsernames.length > 0;
        if (existedData) {
            this.$("#mainGrid").css("display", "block");
            this.$("#mainGrid").css("visibility", "visible");
            this.$("#msgNoinfo").css("display", "none");
            this.Render(data);
        }
        else {
            this.ShowNoData();
        }
        Fanex.ExtAcc.UnBlockUI();
    };
    SuperWinLossMCDetailByPT.prototype.ShowNoData = function () {
        this.$("#mainGrid").css("display", "none");
        this.$("#mainGrid").css("visibility", "hidden");
        this.$("#msgNoinfo").css("display", "block");
    };
    SuperWinLossMCDetailByPT.prototype.ExportExcel = function () {
        var params = "fromdate=" + this.fromdate + "&todate=" + this.todate + "&groupname=" + this.$("#selectedGroupName").html() + "&groupId=" + (Fanex.ExtAcc.Customer.GetGroupId() || ''), url = "" + resolveClientUrl("SuperWinLossDetailMCByPT/ExportWinLossDetailToExcel?") + params + this.cachedProductAndCurParams;
        this.topWindow.exportExcel(url, document);
    };
    SuperWinLossMCDetailByPT.prototype.ResizeReportGrid = function () {
        try {
            $("#jqgrid").fanexGrid("resizeGrid");
        }
        catch (err) {
            console.log(err);
        }
    };
    SuperWinLossMCDetailByPT.prototype.ShowHideFilter = function () {
        if (this.hideFilter) {
            $(".icon-filter").addClass("active");
            $(".filterbox").removeClass("hide");
        }
        else {
            $(".icon-filter").removeClass("active");
            $(".filterbox").addClass("hide");
        }
        this.ResizeReportGrid();
        this.hideFilter = !this.hideFilter;
    };
    SuperWinLossMCDetailByPT.prototype.ShowInfo = function () {
        var message = $("#notes").data("info");
        Fanex.ExtAcc.Form.Alert('Note', message, 500, 220, null, null, 0);
    };
    return SuperWinLossMCDetailByPT;
}());
$(document).ready(function () {
    var superWinLossMCDetailByPT = new SuperWinLossMCDetailByPT($, window.top, new DateRangePickerJQueryComponent());
    superWinLossMCDetailByPT.Init();
});
//# sourceMappingURL=SuperWinLossMCDetailByPT.js.map