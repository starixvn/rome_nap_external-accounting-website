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
var WinLossDetailMC = /** @class */ (function () {
    function WinLossDetailMC($, topWindow, dateRangePickerComponent) {
        var _this = this;
        this.groupId = "";
        this.groupName = "";
        this.username = "";
        this.srecommend = 0;
        this.mrecommend = 0;
        this.recommend = 0;
        this.custId = 0;
        this.supername = "";
        this.mastername = "";
        this.agentname = "";
        this.cachedProductAndCurParams = "";
        this.fromdate = moment(new Date()).format("MM/DD/YYYY");
        this.todate = moment(new Date()).format("MM/DD/YYYY");
        this.fixedsortdata = new Array();
        this.removedsortdata = new Array();
        this.fixedsortlist = new Array();
        this.url = resolveClientUrl("WinLossDetailMC/GetData");
        this.viewlevel = 5;
        this.cachedViewLevel = 5;
        this.dataSource = [];
        this.totalSource = [];
        this.totalRows = 0;
        this.productDetailsData = [];
        this.hideFilter = false;
        this.DefaultColumns = [
            {
                HeaderText: "Username", Name: "Username", Width: 130, Sort: true, HeaderAlign: "center",
                CustomCellRender: function (cell) { return _this.RenderUsernameColumn(cell); },
                CustomSortFunction: function (cell1Value, cell2Value, sortDirection) { return _this.CustomUsernameSortFunction(cell1Value, cell2Value, sortDirection); }
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
        this.SAgColumns = [
            {
                HeaderText: "Username", Name: "Username", Width: 130, Sort: true, HeaderAlign: "center",
                CustomCellRender: function (cell) { return _this.RenderUsernameColumn(cell); },
                CustomSortFunction: function (cell1Value, cell2Value, sortDirection) { return _this.CustomUsernameSortFunction(cell1Value, cell2Value, sortDirection); }
            },
            { HeaderText: "Level", Name: "LevelType", Width: 130, Sort: true, HeaderAlign: "center" },
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
        this.MAgColumns = [
            {
                HeaderText: "Username", Name: "Username", Width: 130,
                Sort: true, HeaderAlign: "center",
                CustomCellRender: function (cell) { return _this.RenderUsernameColumn(cell); },
                CustomSortFunction: function (cell1Value, cell2Value, sortDirection) { return _this.CustomUsernameSortFunction(cell1Value, cell2Value, sortDirection); }
            },
            { HeaderText: "Level", Name: "LevelType", Width: 130, Sort: true, HeaderAlign: "center" },
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
        this.AgColumns = [
            {
                HeaderText: "Username", Name: "Username", Width: 130,
                Sort: true, HeaderAlign: "center",
                CustomCellRender: function (cell) { return _this.RenderUsernameColumn(cell); },
                CustomSortFunction: function (cell1Value, cell2Value, sortDirection) { return _this.CustomUsernameSortFunction(cell1Value, cell2Value, sortDirection); }
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
        this.DefaultHeaderColumns = [
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
        this.SAgHeaderColumns = [
            { HeaderText: "Username", Name: "Username", MergeRowNumber: 2 },
            { HeaderText: "Level", Name: "LevelType", MergeRowNumber: 2 },
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
        this.MAgHeaderColumns = [
            { HeaderText: "Username", Name: "Username", MergeRowNumber: 2 },
            { HeaderText: "Level", Name: "LevelType", MergeRowNumber: 2 },
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
        this.AgHeaderColumns = [
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
        this.$ = $;
        this.topWindow = topWindow;
        this.fixedsortdata = [];
        this.fixedsortlist = [];
        this.productControls = new ProductControls($);
        this.dateRangePickerComponent = dateRangePickerComponent;
    }
    WinLossDetailMC.prototype.RenderUsernameColumn = function (cell) {
        if (cell == null || this.viewlevel < 2 || cell.Value.RoleId == 1) {
            return '<div class="text-truncate" style="line-height: 22px; text-align: left; padding-left: 3px" title="' + cell.Value.Username + '">' + cell.Value.Username + "</div>";
        }
        return '<div style="line-height: 22px; text-align: left;">' +
            '<a class="text-truncate report-username" style="text-decoration: none; margin: 0" href="" data-custid="' + cell.Value.CustId + '" title="' + cell.Value.Username + '">' + cell.Value.Username + '</a>' +
            '</div>';
    };
    WinLossDetailMC.prototype.CustomUsernameSortFunction = function (cell1Value, cell2Value, sortDirection) {
        if (sortDirection == 'asc') {
            return cell1Value.Username > cell2Value.Username ? 1 : -1;
        }
        return cell1Value.Username < cell2Value.Username ? 1 : -1;
    };
    WinLossDetailMC.prototype.Init = function () {
        var _this = this;
        this.productControls.Init();
        this.dateRangePickerComponent.render(new DateRangePickerJQueryOption());
        Fanex.ExtAcc.InitSuggestion();
        $("#ddlGroupName").bind("change", function (event) {
            var args = event.data;
            if (args) {
                var item = args.item;
                _this.groupName = item.innerHTML;
                _this.ResetParameters();
                _this.InitShowHideControls("none");
            }
        });
        this.groupId = Fanex.ExtAcc.Customer.GetGroupId() || "";
        this.groupName = $("#selectedGroupName").html();
        this.InitCurrencyList();
        $("#ddl-base-currency").combobox();
        $("#base-currency input").attr('readonly', 'true').val($("#ddl-base-currency")[0].options[0].text).bind("click", function () {
            $(this).autocomplete("search", "");
        });
        $("#ckbBaseCurrency").change(function () {
            $("#base-currency").find("input, button").prop("disabled", !$("#ckbBaseCurrency").is(":checked"));
        });
        $("#Submit").click(function (e) {
            _this.CallSubmit();
        });
        $("#txtUserName").keyup(function (e) {
            if ($("#txtUserName").val().length > 1) {
                Fanex.ExtAcc.InitSuggestion();
            }
        });
        $("#tabSuper").click(function (e) {
            _this.viewlevel = 4;
            _this.srecommend = 0;
            _this.mrecommend = 0;
            _this.recommend = 0;
            if (_this.custId !== 0) {
                _this.custId = 0;
            }
            _this.ToggleTabs('none', 'none', 'none', 'none', 4);
            _this.Databind(_this.supername);
        });
        $("#tabMaster").click(function (e) {
            _this.viewlevel = 4;
            _this.custId = _this.mastername === "" ? _this.srecommend : 0;
            _this.ToggleTabs('block', 'block', 'none', 'none', 3);
            _this.Databind(_this.mastername);
        });
        $("#tabAgent").click(function (e) {
            _this.viewlevel = 3;
            _this.custId = _this.agentname === "" ? _this.mrecommend : 0;
            _this.ToggleTabs('block', 'block', 'block', 'none', 2);
            _this.Databind(_this.agentname);
        });
        $("#txtUserName").keypress(function (e) {
            if (typeof e == "undefined")
                e = window.event;
            var key = (typeof e.keyCode == "undefined") ? e.which : e.keyCode;
            if (key == 13) {
                _this.CallSubmit();
                return true;
            }
            else {
                return true;
            }
        });
        this.$(".col-sorter").click(function (event) { return _this.sortTable(_this.$(event.currentTarget)); });
        Fanex.ExtAcc.OnResize = this.ResizeReportGrid;
        $(".icon-filter").click(function () { return _this.ShowHideFilter(); });
        $("#notes").click(function () { return _this.ShowInfo(); });
        Fanex.ExtAcc.OnResize = this.ResizeReportGrid;
    };
    WinLossDetailMC.prototype.InitLevelSelection = function (options) {
        $("#" + options.dropdownId).empty().select2({
            placeholder: "Select",
            data: options.dropdownData,
            dropdownAutoWidth: true,
            templateSelection: function (data) {
                return data.text;
            },
            sorter: function (data) {
                return data;
            }
        }).bind("select2:select", function () {
            if (options.onClosed) {
                options.onClosed($("#" + options.dropdownId).val());
            }
        });
    };
    WinLossDetailMC.prototype.ResetParameters = function () {
        this.srecommend = 0;
        this.mrecommend = 0;
        this.recommend = 0;
        this.custId = 0;
        this.username = '';
        this.viewlevel = 4;
    };
    WinLossDetailMC.prototype.InitShowHideControls = function (status) {
        $("#headlineDaily").css("display", status);
        $("#mainGrid").css("display", status);
    };
    WinLossDetailMC.prototype.ShowHideFilter = function () {
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
    WinLossDetailMC.prototype.ResizeReportGrid = function () {
        try {
            $("#jqgrid").fanexGrid("resizeGrid");
        }
        catch (err) {
        }
    };
    WinLossDetailMC.prototype.InitCurrencyList = function () {
        var _this = this;
        $.ajax({
            context: this,
            url: resolveClientUrl("Filter/CurrencyList"),
            type: "GET",
            data: { groupName: this.groupName },
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
    WinLossDetailMC.prototype.ToggleTabs = function (levelLine, statusSAg, statusMAg, statusAg, status) {
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
    };
    WinLossDetailMC.prototype.CallSubmit = function () {
        this.groupName = $("#selectedGroupName").html();
        this.ResetParameters();
        this.username = Fanex.encodeHtml($("#txtUserName").val());
        this.Databind(this.username);
    };
    WinLossDetailMC.prototype.GetParameterCurrency = function () {
        var currencyNames = this.$multiSelectCurrency.fanexMultiSelect("getSelectedTexts", true);
        return currencyNames.length ? "&currencies=" + currencyNames.join(",") : "";
    };
    WinLossDetailMC.prototype.GetParameterBaseCurrency = function () {
        if ($("#ckbBaseCurrency").prop("checked")) {
            return "&baseCurrency=" + $("#ddl-base-currency").val() + "&baseCurrencyName=" + $("#ddl-base-currency option:selected").text();
        }
        else {
            return "";
        }
    };
    WinLossDetailMC.prototype.Databind = function (username) {
        var _this = this;
        this.username = !username ? "" : username;
        this.groupName = $("#selectedGroupName").html();
        var dateRange = this.dateRangePickerComponent.getDateRangeValues();
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
        }
        else {
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
            + this.cachedProductAndCurParams;
        Fanex.ExtAcc.BlockUI();
        this.$.ajax({
            url: this.url + parameterUrl,
            type: "post",
            dataType: "json",
            cache: false,
            success: function (data) {
                if (data.errorCode) {
                    Fanex.ExtAcc.UnBlockUI();
                    Fanex.ExtAcc.Form.Alert("Error", data.errorMessage.formatErrorMessage(), 350, 150, "alertWLD");
                    return;
                }
                _this.fixedsortdata = new Array();
                _this.removedsortdata = new Array();
                if (data.rows && data.rows.WinLossDetails) {
                    _this.dataSource = data.rows.WinLossDetails;
                    _this.totalSource = data.rows.WinLossTotals;
                    _this.totalRows = data.rows.WinLossDetails.length;
                }
                else {
                    _this.dataSource = new Array();
                    _this.totalSource = new Array();
                    _this.totalRows = 0;
                }
                _this.fixedsortlist = data.sortingorderlist;
                _this.viewlevel = data.viewlevel;
                if (_this.viewlevel == 3) {
                    _this.srecommend = data.srecommend;
                }
                else if (_this.viewlevel == 2) {
                    _this.srecommend = data.srecommend;
                    _this.mrecommend = data.mrecommend;
                }
                else if (_this.viewlevel == 1) {
                    _this.srecommend = data.srecommend;
                    _this.mrecommend = data.mrecommend;
                    _this.recommend = data.recommend;
                }
                if (_this.custId == 0) {
                    _this.supername = data.supername;
                    _this.mastername = data.mastername;
                    _this.agentname = data.agentname;
                }
                _this.ChangeHeadlineTab();
                $("#mainGrid").css("display", _this.totalRows < 1 ? "none" : "block");
                $("#mainGrid").css("visibility", _this.totalRows < 1 ? "hidden" : "visible");
                $("#headlineDaily").css("display", _this.totalRows < 1 ? "none" : "block");
                $("#headlineDaily").css("visibility", _this.totalRows < 1 ? "hidden" : "visible");
                $("#msgNoinfo").css("display", _this.totalRows < 1 ? "block" : "none");
                if (_this.totalRows > 0) {
                    _this.Render(dateInfo, currencyInfo);
                    $(".groupBack").removeClass("hide");
                }
                else {
                    $(".groupBack").addClass("hide");
                }
                $("#exporttoexcelByCur").click(function () {
                    var params = "fromdate=" + _this.fromdate +
                        "&todate=" + _this.todate +
                        "&viewlevel=" + _this.viewlevel +
                        "&custid=" + _this.custId +
                        "&groupId=" + _this.groupId +
                        "&groupname=" + _this.groupName +
                        "&username=" + _this.username;
                    var url = resolveClientUrl("WinLossDetailMC/ExportWinLossDetailToExcel?") +
                        params + _this.cachedProductAndCurParams;
                    _this.topWindow.exportExcel(url, document);
                });
                $("#netTurnover").bind("click", function () {
                    var message = "- For Sportsbook, Net Turnover is Turnover which excludes the draw tickets and halves of the turnover of the half won, half lost tickets.";
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
                $(".report-username").unbind('click').click(function (event) {
                    event.preventDefault();
                    var custID = $(event.currentTarget).data("custid");
                    _this.GetDownlineWinLossDetail(custID);
                });
                Fanex.ExtAcc.UnBlockUI();
            }
        });
    };
    WinLossDetailMC.prototype.ChangeHeadlineTab = function () {
        if (this.viewlevel >= 4) {
            this.srecommend = this.custId;
            this.ToggleTabs('none', 'none', 'none', 'none', 4);
        }
        else if (this.viewlevel == 3) {
            this.mrecommend = this.custId;
            this.ToggleTabs('block', 'block', 'none', 'none', 3);
            $("#tabSuper").attr("class", "tabActiveSuper");
        }
        else if (this.viewlevel == 2) {
            //case need check
            this.recommend = this.custId;
            this.ToggleTabs('block', 'block', 'block', 'none', 2);
            $("#tabSuper").attr("class", "tabDisableSuper");
            $("#tabMaster").attr("class", "tabActive");
        }
        else {
            this.ToggleTabs('block', 'block', 'block', 'block', 1);
            $("#tabSuper").attr("class", "tabDisableSuper");
            $("#tabMaster").attr("class", "tabDisable");
            $("#tabAgent").attr("class", "tabActive");
        }
        $("#headlineDaily").css("visibility", "visible");
    };
    WinLossDetailMC.prototype.Render = function (dateInfo, currencyInfo) {
        var _this = this;
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
        switch (this.viewlevel) {
            case 1:
                arrColData = this.AgColumns;
                arrGroupHeaders = this.AgHeaderColumns;
                break;
            case 2:
                arrColData = this.MAgColumns;
                arrGroupHeaders = this.MAgHeaderColumns;
                numberOfFrozenColumn = 5;
                break;
            case 3:
                arrColData = this.SAgColumns;
                arrGroupHeaders = this.SAgHeaderColumns;
                numberOfFrozenColumn = 5;
                break;
            default:
                arrColData = this.DefaultColumns;
                arrGroupHeaders = this.DefaultHeaderColumns;
                break;
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
            titleInfo: titleInfo,
            showFooter: true,
            gridExpandHeight: 256,
            showPager: true,
            pagerOption: {
                itemsPerPageArray: [50, 100, 500],
                currentPage: 0,
                totalItem: this.totalRows,
                itemsPerPage: 50,
                afterPagedFunction: function () {
                    $(".report-username").unbind('click').click(function (event) {
                        event.preventDefault();
                        var custID = $(event.currentTarget).data("custid");
                        _this.GetDownlineWinLossDetail(custID);
                    });
                }
            },
            marginRight: 10,
            bottomBorderWidth: 0,
            showDistinctRow: false,
            allowTruncate: true,
            bodyRows: [],
            footerRows: [],
            sortOption: {
                afterGridSorted: function (rows) {
                    return _this.Customsortfunc(rows);
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
            }
            else if (this.viewlevel === 2) {
                bodyData.push({
                    Columns: [
                        { Name: "Username", Value: this.dataSource[i] },
                        { Name: "LevelType", Value: this.dataSource[i].RoleId === 2 ? "Agent" : "Direct Member" },
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
            }
            else if (this.viewlevel === 3) {
                bodyData.push({
                    Columns: [
                        { Name: "Username", Value: this.dataSource[i] },
                        { Name: "LevelType", Value: this.dataSource[i].RoleId === 3 ? "Master Agent" : "Direct Member" },
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
            else {
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
                        { Name: "Username", Value: this.totalSource[i].TotalName, MergeColNumber: 2, CellAlign: "right" },
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
            }
            else if (this.viewlevel === 2) {
                footerData.push({
                    Columns: [
                        { Name: "Username", Value: this.totalSource[i].TotalName, MergeColNumber: 3, CellAlign: "right" },
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
            }
            else if (this.viewlevel === 3) {
                footerData.push({
                    Columns: [
                        { Name: "Username", Value: this.totalSource[i].TotalName, MergeColNumber: 3, CellAlign: "right" },
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
            else {
                footerData.push({
                    Columns: [
                        { Name: "Username", Value: this.totalSource[i].TotalName, MergeColNumber: 2, CellAlign: "right" },
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
    };
    WinLossDetailMC.prototype.Customsortfunc = function (rows) {
        var _this = this;
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
        $(".report-username").unbind('click').click(function (event) {
            event.preventDefault();
            var custID = $(event.currentTarget).data("custid");
            _this.GetDownlineWinLossDetail(custID);
        });
        return rows;
    };
    WinLossDetailMC.prototype.InitSort = function () {
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
    };
    WinLossDetailMC.prototype.ShowNoData = function () {
        $("#mainGrid").css("display", "none");
        $("#mainGrid").css("visibility", "hidden");
        $("#headlineDaily").css("display", "none");
        $("#headlineDaily").css("visibility", "hidden");
        $("#msgNoinfo").css("display", "block");
        $(".groupBack").addClass("hide");
    };
    WinLossDetailMC.prototype.GetDownlineWinLossDetail = function (custId) {
        event.preventDefault();
        this.custId = custId;
        this.username = "";
        this.Databind(this.username);
    };
    WinLossDetailMC.prototype.ShowInfo = function () {
        var message = $("#notes").data("info");
        Fanex.ExtAcc.Form.Alert('Note', message, 400, 200, null, null, 0);
    };
    return WinLossDetailMC;
}());
$(document).ready(function () {
    var winLossDetailMC = new WinLossDetailMC($, window.top, new DateRangePickerJQueryComponent());
    winLossDetailMC.Init();
});
//# sourceMappingURL=Fanex.ExtAcc.WinLossDetailMC.js.map