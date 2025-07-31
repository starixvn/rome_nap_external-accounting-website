declare var resolveClientUrl: any;

class ProductControls {
    private $: JQueryStatic;
    private productDetailsData: any;
    private productDetailsDataMapToProductId: any;
    private $multiSelectProduct: any;
    private $multiSelectProductDetail: any;

    constructor($: JQueryStatic) {
        this.$ = $;
        this.productDetailsData = [];
    }

    public Init() {
        this.$.ajax({
            url: resolveClientUrl("Filter/ProductsList"),
            type: "GET",
            data: { groupName: $("#selectedGroupName").html(), groupId: Fanex.ExtAcc.Customer.GetGroupId() || '', isWinLossDetail: false },
            dataType: "json",
            cache: false,
            success: data => this.OnSuccessLoadProduct(data)
        });
    }

    private OnSuccessLoadProduct(data) {
        const productDetails = data.productDetails;
        if (productDetails.length) {
            this.InitProductDetailsList(productDetails);
        }

        this.InitProductsList(data.products);
    }

    private InitProductDetailsList(productDetails) {
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
            } else {
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
    }

    private InitProductsList(products) {
        const self: any = this;
        const $ddlProductDetail = this.$("#ddl-product-detail");
        //const $checkAllProductDetail = $("#ddl-product-detailHeaderCheckBox");
        const buildProductData = function (productList) {
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
                    const checked = this.checked;
                    const productId = parseInt(this.value);

                    if (isNaN(productId)) { // Select all
                        self.ProductsListOnCheckAll(checked, $ddlProductDetail);
                    } else {
                        var $groupCheckbox = $(this);
                        if ($groupCheckbox.data("is-group")) {
                            self.ProductsListOnCheckGroup(checked, $ddlProductDetail, $groupCheckbox);
                        } else {
                            self.ProductsListOnCheck(checked, productId, $ddlProductDetail);
                        }
                    }
                }
            }
        });
    }

    private ProductsListOnCheckAll(checked, $ddlProductDetail) {
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
            } else {
                $(this).parent().removeClass("hide-item");
            }
        });
    }

    private ProductsListOnCheckGroup(checked, $ddlProductDetail, $groupCheckbox) {
        const self: any = this;
        $groupCheckbox
            .closest(".fanex-multiselect-div")
            .next(".fanex-multiselect-ul-group")
            .find('input[type="checkbox"]')
            .each(function () {
                self.ProductsListOnCheck(checked, parseInt(this.value), $ddlProductDetail);
            });
    }

    private ProductsListOnCheck(checked, productId, $ddlProductDetail) {
        const filteredProductDetails = this.productDetailsData.filter(function (p) {
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
                } else {
                    $(this).parent().removeClass("hide-item");
                }
            })
        });

        const maxAnabledItem = optionsElement.find('input[type="checkbox"].show-item:enabled').length;
        const checkedItem = optionsElement.find('input[type="checkbox"].show-item:enabled:checked').length;
        if (maxAnabledItem === checkedItem) {
            $("#ddl-product-detailHeaderCheckBox").prop("checked", true);
        }

        var enabledCheckboxes = optionsElement.find('input[type="checkbox"]:enabled').length - 1;
        if (checked) {
            $ddlProductDetail.fanexMultiSelect("setEnable", checked);
        } else if (!enabledCheckboxes) {
            $ddlProductDetail.fanexMultiSelect("setEnable", checked);
        }
    }

    public GetParameterProducts() {
        var productIds = this.GetProductIds();
        return productIds.length ? "&productIds=" + productIds.join(",") : "";
    }

    public GetParameterProductDetails() {
        if (this.$multiSelectProductDetail) {
            var resourceIds = this.$multiSelectProductDetail.fanexMultiSelect("getItems", true);
            return resourceIds.length ? "&resourceIds=" + resourceIds.join(",") : "";
        }

        return "";
    }

    public GetProductIds() {
        this.UpdateUncheckedProductsDropdown();
        return this.$multiSelectProduct.fanexMultiSelect("getItems", true);
    }

    public GetResourceIds() {
        return this.$multiSelectProductDetail
            ? this.$multiSelectProductDetail.fanexMultiSelect("getItems", true)
            : [];
    }

    private UpdateUncheckedProductsDropdown() {
        if (this.productDetailsDataMapToProductId) {
            const self = this;
            Object.keys(self.productDetailsDataMapToProductId).forEach(function (productId) {
                if (self.IsProductDetailsUnChecked(self.productDetailsDataMapToProductId[productId])) {
                    $('input[id^="fanex-multiselect-ddl-product-' + productId + '-"]').prop("checked", false).trigger("change");
                }
            });
        }
    }

    private IsProductDetailsUnChecked(resourceIds) {
        for (let i = 0, max = resourceIds.length; i < max; i++) {
            if ($('input[id^="fanex-multiselect-ddl-product-detail-' + resourceIds[i] + '-"]').is(":checked")) {
                return false;
            }
        }

        return true;
    }
}