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
            data: { groupName: $("#selectedGroupName").html() },
            dataType: "json",
            cache: false,
            success: function (data) { return _this.OnSuccessLoadProduct(data); }
        });
    };
    ProductControls.prototype.GetParameterProducts = function () {
        var parameterProducts = '';
        var ids = $("#productList .ui-multiselect-checkboxes :input");
        for (var i = 0; i < ids.length; i++) {
            parameterProducts += "&" + ids[i].value + "=" + ids[i].checked;
        }
        return parameterProducts;
    };
    ProductControls.prototype.GetParameterProductDetails = function () {
        var parameterProductDetails = [];
        if (!$("#ProductDetails").hasClass("ui-state-disabled")) {
            var ids = $("#productDetailsList .ui-multiselect-checkboxes :input:checked:not(:disabled)");
            for (var i = 0; i < ids.length; i++) {
                this.$(ids[i]).val() !== "showAll" && parameterProductDetails.push(this.$(ids[i]).val());
            }
        }
        return parameterProductDetails.length ? "&resourceIds=" + parameterProductDetails.join(",") : "";
    };
    ProductControls.prototype.InitProductsList = function ($groupProducts) {
        var $this = this;
        $groupProducts.css("display", "block");
        var $productList = $("#productList");
        $("#productList div").remove();
        $groupProducts.multiSelect({
            minWidth: 170,
            showHeader: false,
            onCheck: function () {
                var checkedValue = this.value;
                if (checkedValue === "showAll") {
                    var ids = $productList.find(".ui-multiselect-checkboxes :input");
                    for (var i = 0; i < ids.length; i++) {
                        ids[i].checked = this.checked;
                    }
                }
                else {
                    if (!this.checked) {
                        $("#ui-multiselect-GroupProducts-option-0")[0].checked = false;
                    }
                }
                $this.ConstrainProductDetails(this.checked, checkedValue);
            },
            noneSelectedText: function () {
                $groupProducts.attr("title", "");
                return "";
            },
            selectedText: function (numChecked, numTotal, checkedItems) {
                if (checkedItems[0].value !== "showAll") {
                    numChecked++;
                }
                if (numChecked !== numTotal) {
                    var mainTitle = "";
                    $.each(checkedItems, function (i) {
                        mainTitle += checkedItems[i].title + ", ";
                    });
                    mainTitle = mainTitle.substring(0, mainTitle.length - 2);
                    $groupProducts.attr("title", mainTitle);
                    return mainTitle;
                }
                else {
                    $("#ui-multiselect-GroupProducts-option-0")[0].checked = true;
                    $groupProducts.attr("title", "All");
                    return "All";
                }
            }
        });
        $("#GroupProducts").width(170);
    };
    ProductControls.prototype.ConstrainProductDetails = function (checked, checkedValue) {
        var $productDetails;
        if (checkedValue === "showAll") {
            $('#productDetailsList .ui-multiselect-checkboxes input[type="checkbox"]').prop("checked", checked);
            $productDetails = $("#ProductDetails");
            if (checked) {
                $productDetails.removeClass("ui-state-disabled").find('input[type="button"]').val("All");
            }
            else {
                $productDetails.addClass("ui-state-disabled").find('input[type="button"]').val("");
            }
        }
        else {
            var filteredProductDetails = this.productDetailsData.filter(function (p) {
                if (p.ProductKey === checkedValue) {
                    return p;
                }
            });
            if (filteredProductDetails.length) {
                $.each(filteredProductDetails, function (index, p) {
                    $('#productDetailsList .ui-multiselect-checkboxes input[type="checkbox"]').prop("checked", checked);
                });
                $productDetails = $("#ProductDetails");
                if (checked) {
                    $productDetails.removeClass("ui-state-disabled").find('input[type="button"]').val("All");
                }
                else {
                    $productDetails.addClass("ui-state-disabled").find('input[type="button"]').val("");
                }
            }
        }
    };
    ProductControls.prototype.InitProductDetailsList = function ($productDetails) {
        $productDetails.css("display", "block");
        var $productDetailsList = $("#productDetailsList");
        $("#productDetailsList div").remove();
        $productDetails.multiSelect({
            minWidth: 200,
            maxHeight: 100,
            showHeader: false,
            onCheck: function () {
                if (this.value === "showAll") {
                    var ids = $productDetailsList.find(".ui-multiselect-checkboxes :input");
                    for (var i = 0; i < ids.length; i++) {
                        ids[i].checked = this.checked;
                    }
                }
                else {
                    if (!this.checked) {
                        $("#ui-multiselect-ProductDetails-option-0")[0].checked = false;
                    }
                }
            },
            noneSelectedText: function () {
                $productDetails.attr("title", "");
                return "";
            },
            selectedText: function (numChecked, numTotal, checkedItems) {
                if (checkedItems[0].value !== "showAll") {
                    numChecked++;
                }
                if (numChecked !== numTotal) {
                    var mainTitle = "";
                    $.each(checkedItems, function (i) {
                        mainTitle += checkedItems[i].title + ", ";
                    });
                    mainTitle = mainTitle.substring(0, mainTitle.length - 2);
                    $productDetails.attr("title", mainTitle);
                    return mainTitle;
                }
                else {
                    $("#ui-multiselect-ProductDetails-option-0")[0].checked = true;
                    $productDetails.attr("title", "All");
                    return "All";
                }
            }
        });
        $("#ProductDetails").width(200);
    };
    ProductControls.prototype.OnSuccessLoadProduct = function (data) {
        var products = data.products;
        var productList = "<option value=\"showAll\" selected=\"selected\">All</option>";
        this.$.each(products, function (i) {
            productList += "<option value=\"" + products[i].Key + "\" selected=\"selected\">" + products[i].Value + "</option>";
        });
        var $groupProducts = $("#GroupProducts").html(productList);
        this.InitProductsList($groupProducts);
        this.$("#GroupProducts :input").width($groupProducts.width() - 21);
        var productDetails = data.productDetails;
        if (productDetails.length) {
            var productDetailsList_1 = "<option value=\"showAll\" selected=\"selected\">All</option>";
            this.$.each(productDetails, function (i) {
                productDetailsList_1 += "<option value=\"" + productDetails[i].ResourceId + "\" selected=\"selected\" data-productkey=\"" + productDetails[i].ProductKey + "\">" + productDetails[i].DisplayName + "</option>";
            });
            var $productDetails = $("#ProductDetails").html(productDetailsList_1);
            this.InitProductDetailsList($productDetails);
            this.$("#ProductDetails :input").width($productDetails.width() - 21);
            this.$("#productDetailsContainer").removeClass("hidden");
            this.productDetailsData = productDetails;
        }
    };
    return ProductControls;
}());
//# sourceMappingURL=ProductControls.js.map