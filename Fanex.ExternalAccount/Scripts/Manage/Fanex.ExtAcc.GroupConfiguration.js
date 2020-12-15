/// <reference path="../_references.js" />
if (typeof Fanex == 'undefined' || typeof Fanex.ExtAcc == 'undefined') {
    alert("Fanex.ExtAcc.js module is required!")
}

Fanex.ExtAcc.GroupConfiguration = {

    $this: null,

    Init: function () {
        $this = this;
        $('#tabs').tabs();

        $("#groupIds").combobox({
            selected: function (event, ui) {
                $this.RenderDataByGroup();
            }
        });
        $("#grouplist input").val($("#groupIds")[0].options[0].text);
        $("#grouplist input").attr("readonly", "true");
        $("#grouplist input").bind("click", function () {
            $(this).autocomplete("search", '');
        });

        $("#updateByProduct").on('click', function () {
            $this.UpdateDataByProduct();
        });

        $("#updateByGroup").on('click', function () {
            $this.UpdateDataByGroup();
        });

        $("#productIds")
            .change(this.RenderDataByProduct)
            .trigger("change");
    },

    UpdateDataByProduct: function () {
        Fanex.ExtAcc.BlockUI();
        var products = $this.GetDataByProduct();
        $.ajax({
            url: resolveClientUrl('GroupConfiguration/Update'),
            data: {
                GroupDetails: products
            },
            type: 'POST'
        }).success(function () {
            $('#msgProductSection').attr('class', 'successMsg');
            $('#msgProductSection').html('<font color="Blue">Updated successfully</font>');
            $('#msgProductSection').show();
            setTimeout(function () {
                $('#msgProductSection').hide();
            }, 2000);
        }).done(function () {
            Fanex.ExtAcc.UnBlockUI();
        });
    },

    UpdateDataByGroup: function () {
        Fanex.ExtAcc.BlockUI();
        var groups = $this.GetDataByGroup();
        $.ajax({
            url: resolveClientUrl('GroupConfiguration/Update'),
            data: {
                GroupDetails: groups
            },
            type: 'POST'
        }).success(function () {
            $('#msgGroupSection').attr('class', 'successMsg');
            $('#msgGroupSection').html('<font color="Blue">Updated successfully</font>');
            $('#msgGroupSection').show();
            setTimeout(function () {
                $('#msgGroupSection').hide();
            }, 2000);
        }).done(function () {
            Fanex.ExtAcc.UnBlockUI();
        });
    },

    GetDataByProduct: function () {
        var data = [];
        var productId = $("#productIds").val();
        $("#ProductGrid tbody tr").each(function (i, row) {
            var $row = $(row);
            var isShow = $row.find('#IsShow');
            var detail = {
                ProductId: productId,
                GroupId: $row.find('input').val(),
                IsShow: isShow.attr('class') == 'menuActive' ? 1 : 0
            };

            data.push(detail);
        });

        return data;
    },

    GetDataByGroup: function () {
        var data = [];
        var groupId = $("#groupIds").val();
        $("#GroupGrid tbody tr").each(function (i, row) {
            var $row = $(row);
            var isShow = $row.find('#IsShow');
            var detail = {
                ProductId: $row.find('#ProductId').html(),
                GroupId: groupId,
                IsShow: isShow.attr('class') == 'menuActive' ? 1 : 0
            };

            data.push(detail);
        });

        return data;
    },

    RenderDataByProduct: function () {
        Fanex.ExtAcc.BlockUI();
        $.ajax({
            url: resolveClientUrl('GroupConfiguration/Get'),
            data: { ProductId: $("#productIds").val() },
            type: 'POST'
        }).success(function (data) {
            var bodyRows = [];
            for (var i = 0; i < data.length; i++) {
                bodyRows.push({
                    Columns: [
                        { Name: 'GroupName', Value: data[i] },
                        { Name: 'Currency', Value: data[i].FormatedCurrency },
                        { Name: 'CurrencyCode', Value: data[i].FormatedCurrencyCode },
                        { Name: 'Site', Value: data[i].FormatedSite },
                        { Name: 'IsShow', Value: data[i] }
                    ]
                });
            }
            var columns = [
                {
                    HeaderText: 'Group Name', Name: 'GroupName', Width: 150,
                    CustomCellRender: function (cell) {
                        return '<input type="hidden" value ="' + cell.Value.GroupId + '" />' + cell.Value.GroupName
                    }
                },
                { HeaderText: 'Currency', Name: 'Currency', Width: 150 },
                { HeaderText: 'Currency Code', Name: 'CurrencyCode', Width: 150 },
                { HeaderText: 'Site', Name: 'Site', Width: 150 },
                {
                    HeaderText: '<div class="menuInactive" onclick="javascript:Fanex.ExtAcc.GroupConfiguration.CheckAllForProduct(this);">  &nbsp;  Show</div> ', Name: 'IsShow', Width: 150, CellAlign: 'center',
                    CustomCellRender: function (cell) {
                        var style = cell.Value.IsShow == true ? "menuActive" : "menuInactive";

                        var onclick = "javascript:Fanex.ExtAcc.GroupConfiguration.CheckboxEdit(this)";

                        return '<div id="IsShow" class="' + style + '" onclick="' + onclick + '"></div>';
                    }
                }
            ];
            var options = {
                columns: columns,
                bodyRows: bodyRows,
                gridHeight: 650,
                fullHeight: false,
                marginRight: 10,
                showDistinctRow: false,
                allowTruncate: false
            };
            $this.RenderGrid('ProductGrid', options);
        }).done(function () {
            Fanex.ExtAcc.UnBlockUI();
        });
    },

    RenderDataByGroup: function () {
        Fanex.ExtAcc.BlockUI();
        $.ajax({
            url: resolveClientUrl('GroupConfiguration/Get'),
            data: { GroupId: $("#groupIds").val() },
            type: 'POST'
        }).success(function (data) {
            var bodyRows = [];
            for (var i = 0; i < data.length; i++) {
                bodyRows.push({
                    Columns: [
                        { Name: 'ProductId', Value: data[i] },
                        { Name: 'ProductName', Value: data[i].ProductText },
                        { Name: 'IsShow', Value: data[i] }
                    ]
                });
            }
            var columns = [
                {
                    HeaderText: 'Product Id', Name: 'ProductId', Width: 150, CellAlign: "center",
                    CustomCellRender: function (cell) {
                        return '<div id="ProductId"> ' + cell.Value.ProductId + '</div>';
                    }
                },
                { HeaderText: 'Product Name', Name: 'ProductName', Width: 460 },
                {
                    HeaderText: '<div class="menuInactive" onclick="javascript:Fanex.ExtAcc.GroupConfiguration.CheckAllForGroup(this);">  &nbsp;  Show</div> ', Name: 'IsShow', Width: 150, CellAlign: 'center',
                    CustomCellRender: function (cell) {
                        var style = cell.Value.IsShow == true ? "menuActive" : "menuInactive";

                        var onclick = "javascript:Fanex.ExtAcc.GroupConfiguration.CheckboxEdit(this)";

                        return '<div id="IsShow" class="' + style + '" onclick="' + onclick + '"></div>';
                    }
                }
            ];
            var options = {
                columns: columns,
                bodyRows: bodyRows,
                gridHeight: 650,
                fullHeight: false,
                marginRight: 10,
                showDistinctRow: false,
                allowTruncate: false
            };
            $this.RenderGrid('GroupGrid', options);
        }).done(function () {
            Fanex.ExtAcc.UnBlockUI();
        })
    },

    RenderGrid: function (id, options) {
        try {
            $('#' + id).fanexGrid(options);
            Fanex.ExtAcc.UnBlockUI();
        }
        catch (e) {
            if (typeof console != 'undefined') {
                console.log(e);
            }

            Fanex.ExtAcc.UnBlockUI();
        }
    },

    CheckAllForGroup: function (element) {
        $this.CheckAll("GroupGrid", element);
    },
    CheckAllForProduct: function (element) {
        $this.CheckAll("ProductGrid", element);
    },
    CheckAll: function (id, element) {
        $this.CheckboxEdit(element);
        var className = $(element).attr('class');

        $("#" + id + " tbody tr").each(function (i, row) {
            $(row).find('#IsShow').attr('class', className);
        });
    },
    CheckboxEdit: function (element) {
        var className = $(element).attr('class');
        if (className == 'menuActive') {
            $(element).attr('class', 'menuInactive');
        } else {
            $(element).attr('class', 'menuActive')
        }
    }
}

$(document).ready(function () {
    Fanex.ExtAcc.GroupConfiguration.Init();
});