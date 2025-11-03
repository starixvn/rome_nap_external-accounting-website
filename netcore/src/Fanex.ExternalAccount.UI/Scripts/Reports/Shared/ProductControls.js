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