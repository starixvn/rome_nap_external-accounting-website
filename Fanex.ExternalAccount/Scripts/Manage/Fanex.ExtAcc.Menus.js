/// <reference path="../_references.js" />
if (typeof Fanex == 'undefined' || typeof Fanex.ExtAcc == 'undefined') {
    alert("Fanex.ExtAcc.js module is required!")
}

Fanex.ExtAcc.Menus = {
    DataFields: null,
    DataSource: null,

    MenuId: null,

    URL: '',

    $this: null,

    Init: function () {
        Fanex.ExtAcc.BlockUI();

        $('#mainGrid').css('visibility', 'hidden');
        $this = this;

        $this.URL = resolveClientUrl("Menu/MenuListPost");

        var titles = $.parseJSON(menuTitles);
        var index = 0;

        $this.DataFields = [
            {
                HeaderText: 'Menu Item', Name: 'MenuName', Width: 160,
                CustomCellRender: function (cell) {
                    var menuId = cell.Value.MenuID;
                    var isParent = cell.Value.ParentID == 0;
                    var hasChild = false;

                    if ($this.DataSource.rows[index + 1]) {
                        hasChild = $this.DataSource.rows[index + 1].ParentID != 0;
                    }

                    var func = "javascript:Fanex.ExtAcc.Menus.ShowEdit(" + cell.Value.MenuID + ", '" + titles.Edit + "')";
                    var style = isParent && !hasChild ? "menuItemParentOnly" : (!isParent ? "menuItemChild" : "menuItemParent");
                    var div = '';

                    if (isParent && hasChild) {
                        var ids = [];
                        var length = $this.DataSource.rows.length;

                        for (var i = 0; i < length; i++) {
                            if ($this.DataSource.rows[i].ParentID == menuId) {
                                ids.push(i);
                            }
                        }

                        var onclickParent = 'Fanex.ExtAcc.TogglejqRows(' + menuId + ',[' + ids + '])';
                        div = '<div id="fn-' + menuId + '" onclick="' + onclickParent + '" style="float: left; cursor: pointer" class="itemParent">&nbsp;</div>';
                    }

                    index++;

                    return div + '<div class="' + style + '"><a href="' + func + '">' + cell.Value.MenuName + '</a></div>';
                }
            },
            { HeaderText: 'Description', Name: 'MenuDesc', Width: 150 },
            { HeaderText: 'URL', Name: 'Url', Width: 145 },
            { HeaderText: 'Function', Name: 'FuncName', Width: 150 },
            { HeaderText: 'Order', Name: 'DisplayOrder', Width: 50, CellAlign: 'center' },
            { HeaderText: 'Last Modified', Name: 'ModifiedDate', Width: 160 },
            { HeaderText: 'Active', Name: 'IsActive', CellAlign: 'center', Width: 50,
                CustomCellRender: function (cell) {
                    var style = cell.Value.IsActive == true ? "menuActive" : "menuInactive";
                    var menuId = cell.Value.MenuID;
                    var menuName = cell.Value.MenuName;
                    var onclick = "javascript:Fanex.ExtAcc.Menus.ShowActivate(" + menuId + ", '" + menuName + "')";

                    return '<div id="menu-' + menuId + '" class="' + style + '" onclick="' + onclick + '"></div>';
                }
            },
            { HeaderText: 'Delete', Name: 'Delete', CellAlign: 'center', Width: 50,
                CustomCellRender: function (cell) {
                    var style = "menuDelete";
                    var menuId = cell.Value.MenuID;
                    var menuName = cell.Value.MenuName;
                    var onclick = "javascript:Fanex.ExtAcc.Menus.ShowDelete(" + menuId + ", '" + menuName + "')";

                    return '<div id="' + menuId + '" class="' + style + '" onclick="' + onclick + '"></div>';
                }
            }
        ];

        $.ajax({
            url: $this.URL,
            dataType: 'json'
        })
        .success(function (data) {
            $this.DataSource = data;

            var options = null;
            var bodyRows = [];
            var rows = data.rows;

            for (var i = 0; i < rows.length; i++) {
                bodyRows.push({
                    Columns: [
                        { Name: 'MenuName', Value: rows[i] },
                        { Name: 'MenuDesc', Value: rows[i].MenuDesc },
                        { Name: 'Url', Value: rows[i].Url },
                        { Name: 'FuncName', Value: rows[i].FuncName },
                        { Name: 'DisplayOrder', Value: rows[i].DisplayOrder },
                        { Name: 'ModifiedDate', Value: rows[i].ModifiedDate },
                        { Name: 'IsActive', Value: rows[i] },
                        { Name: 'Delete', Value: rows[i] }
                    ]
                });
            }

            options = {
                columns: $this.DataFields,
                bodyRows: bodyRows,
                gridExpandHeight: 100,
                marginRight: 2,
                showDistinctRow: false,
                allowTruncate: true
            };

            $this.RenderGrid(options);
        })
        .error(function (xhr) {
            $.unblockUI(document);
        });

        $('#jqbutton').bind('click', function () {
            Fanex.ExtAcc.Form.Show(resolveClientUrl("Menu/AddMenu"), $.parseJSON(menuTitles).Create, 400, 460, false);
        });

        Fanex.ExtAcc.OnResize = function () {
            try {
                $('#jqgrid').fanexGrid('resizeGrid');
            }
            catch (err) {
            }
        }
    },

    RenderGrid: function (options) {
        try {
            // Init Grid jqgrid
            $('#jqgrid').fanexGrid(options);
            $.unblockUI(document);

            $('#mainGrid').css('visibility', 'visible');
        }
        catch (e) {
            if (typeof console != 'undefined') {
                console.log(e);
            }

            $.unblockUI(document);
        }
    },

    ShowEdit: function (menuId, title) {
        Fanex.ExtAcc.Menus.MenuId = menuId;
        Fanex.ExtAcc.Form.Show(
            resolveClientUrl('Menu/EditMenu/' + menuId),
            title,
            400,
            460,
            false);
    },

    ShowActivate: function (menuId, menuName) {
        var titles = $.parseJSON(menuTitles);
        var title = '';

        title = $('#menu-' + menuId).attr('class') == 'menuActive' ? 'Deactivate Menu' : 'Activate Menu';

        Fanex.ExtAcc.Menus.MenuId = menuId;
        Fanex.ExtAcc.Form.ConfirmBox(
            title,
            'Are you sure to ' + title.toLowerCase() + ' <font color="#4b91be">' + menuName + '</font> ?',
            400,
            150,
            false,
            function () {
                Fanex.ExtAcc.Menus.Activate(menuId, !($('#menu-' + menuId).attr('class') == 'menuActive'));
            });
    },

    ShowDelete: function (menuId, menuName) {
        Fanex.ExtAcc.Menus.MenuId = menuId;

        var titles = $.parseJSON(menuTitles);
        var title = titles.Delete;

        Fanex.ExtAcc.Form.ConfirmBox(
            'Delete Menu',
            'Are you sure to delete menu <font color="#4b91be">' + menuName + '</font> ?',
            400,
            150,
            false,
            function () {
                Fanex.ExtAcc.Menus.Delete(menuId);
            });
    },

    Add: function () {
        if (!Validators.validateInput($('#MenuName').val())) {
            $('#MenuName').focus();
            Validators.showDown($('#MenuName').get(0), 'This field is required', 0, 10, 41, -10, 15);
            return false;
        }

        if (!Validators.validateInput($('#DisplayOrder').val())) {
            $('#DisplayOrder').focus();
            Validators.show($('#DisplayOrder').get(0), 'This field is required');
            return false;
        }

        if (!Validators.validateNumber($('#DisplayOrder').val())) {
            $('#DisplayOrder').focus();
            Validators.show($('#DisplayOrder').get(0), 'Please input a number');
            return false;
        }

        Fanex.ExtAcc.BlockUI();

        var name = $('#MenuName').val();
        var desc = $('#MenuDesc').val();
        var parentId = $('#ParentName').val() != '-1' ? $('#ParentName').val() : '0';
        var url = $('#Url').val();
        var funcId = $('#FuncID').val();
        var order = $('#DisplayOrder').val();

        var editUrl = 'Menu/Add';
        var params = new Object();

        params.name = name;
        params.description = desc;
        params.url = url;
        params.functionId = funcId;
        params.parentId = parentId;
        params.parentName = ($('#ParentName').val() != '-1' ? $('#ParentName option:selected').text() : '');
        params.displayOrder = order;

        $.ajax({ url: resolveClientUrl(editUrl), data: params, type: 'POST' })
        .done(function (data) {
            if (!data.IsSuccess) {
                $.unblockUI(document);

                $('#errMsg').css('display', 'block');
                $('#errMsg').html(data.ErrorMessage);
            }
            else {
                var timeout = setTimeout(function () {
                    clearTimeout(timeout);

                    Fanex.ExtAcc.Form.Close(true);
                }, 2000);
            }
        })
        .error(function (data) {
            if (typeof console != 'undefined') console.log(data.responseText);

            $.unblockUI(document);
        });
    },

    Edit: function () {
        if (!Validators.validateInput($('#MenuName').val())) {
            $('#MenuName').focus();
            Validators.showDown($('#MenuName').get(0), 'This field is required', 0, 10, 41, -10, 15);
            return false;
        }

        if (!Validators.validateInput($('#DisplayOrder').val())) {
            $('#DisplayOrder').focus();
            Validators.show($('#DisplayOrder').get(0), 'This field is required');
            return false;
        }

        if (!Validators.validateNumber($('#DisplayOrder').val())) {
            $('#DisplayOrder').focus();
            Validators.show($('#DisplayOrder').get(0), 'Please input a number');
            return false;
        }

        Fanex.ExtAcc.BlockUI();

        var name = $('#MenuName').val();
        var desc = $('#MenuDesc').val();
        var parentId = $('#ParentName').val() != '-1' ? $('#ParentName').val() : '0';
        var url = $('#Url').val();
        var funcId = $('#FuncID').val();
        var order = $('#DisplayOrder').val();

        var editUrl = 'Menu/Edit/?';
        var params = new Object();

        params.menuId = $('#MenuID').val();
        params.name = name;
        params.description = desc;
        params.url = url;
        params.functionId = funcId;
        params.parentId = parentId;
        params.parentName = ($('#ParentName').val() != '-1' ? $('#ParentName option:selected').text() : '');
        params.displayOrder = order;

        $.ajax({ url: resolveClientUrl(editUrl), data: params, type: 'POST' })
        .done(function (data) {
            if (!data.IsSuccess) {
                $.unblockUI(document);

                $('#errMsg').css('display', 'block');
                $('#errMsg').html(data.ErrorMessage);
            }
            else {
                var timeout = setTimeout(function () {
                    clearTimeout(timeout);

                    Fanex.ExtAcc.Form.Close(true);
                }, 2000);
            }
        })
        .error(function (data) {
            if (typeof console != 'undefined') console.log(data.responseText);

            $.unblockUI(document);
        });
    },

    Activate: function (menuId, isActive) {
        Fanex.ExtAcc.BlockUI();

        $.ajax({ url: resolveClientUrl('Menu/UpdateStatus/?menuId=' + menuId + '&isActive=' + isActive), type: 'POST' })
        .done(function (data) {
            if (!data.IsSuccess) {
                $.unblockUI(document);

                alert(data.ErrorMessage);
            }
            else {
                var timeout = setTimeout(function () {
                    clearTimeout(timeout);

                    Fanex.ExtAcc.Form.Close(true);
                }, 2000);
            }
        })
        .error(function (data) {
            if (typeof console != 'undefined') console.log(data.responseText);

            $.unblockUI(document);
        });
    },

    Delete: function (menuId) {
        Fanex.ExtAcc.BlockUI();

        $.ajax({ url: resolveClientUrl('Menu/Delete/?menuId=' + menuId), type: 'POST' })
        .done(function (data) {
            if (!data.IsSuccess) {
                $.unblockUI(document);

                alert(data.ErrorMessage);
            }
            else {
                var timeout = setTimeout(function () {
                    clearTimeout(timeout);

                    Fanex.ExtAcc.Form.Close(true);
                }, 2000);
            }
        })
        .error(function (data) {
            if (typeof console != 'undefined') console.log(data.responseText);

            $.unblockUI(document);
        });
    }
}