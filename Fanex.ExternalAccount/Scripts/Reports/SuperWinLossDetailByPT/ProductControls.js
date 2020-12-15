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
            data: { groupName: $("#selectedGroupName").html(), groupId: Fanex.ExtAcc.Customer.GetGroupId() || '' },
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
        var dataList = [];
        this.$.each(productDetails, function (i, item) {
            dataList.push({
                id: item.ResourceId,
                value: item.ResourceId,
                text: item.DisplayName,
                checked: true,
                enable: true
            });
        });
        this.$multiSelectProductDetail = this.$("#ddl-product-detail").fanexMultiSelect({
            minWidth: 180,
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
        this.$.each(filteredProductDetails, function (index, p) {
            optionsElement
                .find('input[type="checkbox"][value="' + p.ResourceId + '"]')
                .prop("checked", checked)
                .prop("disabled", !checked);
        });
        var enabledCheckboxes = optionsElement.find('input[type="checkbox"]:enabled').length - 1;
        if (checked) {
            if (enabledCheckboxes === this.productDetailsData.length) {
                $ddlProductDetail.fanexMultiSelect("setCheckAll", checked);
            }
            $ddlProductDetail.fanexMultiSelect("setEnable", checked);
        }
        else if (!enabledCheckboxes) {
            $ddlProductDetail.fanexMultiSelect("setCheckAll", checked);
            $ddlProductDetail.fanexMultiSelect("setEnable", checked);
        }
    };
    ProductControls.prototype.GetParameterProducts = function () {
        var productIds = this.$multiSelectProduct.fanexMultiSelect("getItems", true);
        return productIds.length ? "&productIds=" + productIds.join(",") : "";
    };
    ProductControls.prototype.GetParameterProductDetails = function () {
        if (this.$multiSelectProductDetail) {
            var resourceIds = this.$multiSelectProductDetail.fanexMultiSelect("getItems", true);
            return resourceIds.length ? "&resourceIds=" + resourceIds.join(",") : "";
        }
        return "";
    };
    return ProductControls;
}());
//# sourceMappingURL=ProductControls.js.map