/// <reference path="../_references.js" />
if (typeof Fanex == 'undefined' || typeof Fanex.ExtAcc == 'undefined') {
    alert("Fanex.ExtAcc.js module is required!")
}

Fanex.ExtAcc.Menus = {
    Adapter: null,
    DataFields: null,
    DataSource: null,
    gridHeight: 200,
    MenuId: null,
    leftMenuWidth: Fanex.ExtAcc.leftMenuWidth,
    URL: '',
    gHeight: 200,
    gSize: { width: 500, height: 300 },
    totalRows: 0,

    Init: function () {
        this.URL = resolveClientUrl("Menu/MenuListPost");

        var titles = $.parseJSON(menuTitles);
        var index = 0;

        Fanex.ExtAcc.Menus.DataFields = [
            {
                name: 'MenuName', align: 'left', width: 150, sortable: false, resizable: false,
                formatter: function (value, model, row) {
                    var menuId = row.MenuID;
                    var isParent = row.ParentID == 0;
                    var hasChild = false;

                    if (Fanex.ExtAcc.Menus.DataSource.rows[index + 1]) {
                        hasChild = Fanex.ExtAcc.Menus.DataSource.rows[index + 1].ParentID != 0;
                    }

                    var func = "javascript:Fanex.ExtAcc.Menus.ShowEdit(" + row.MenuID + ", '" + titles.Edit + "')";
                    var style = isParent && !hasChild ? "menuItemParentOnly" : (!isParent ? "menuItemChild" : "menuItemParent");
                    var div = '';

                    if (isParent && hasChild) {
                        var ids = [];
                        var length = Fanex.ExtAcc.Menus.DataSource.rows.length;

                        for (var i = 0; i < length; i++) {
                            if (Fanex.ExtAcc.Menus.DataSource.rows[i].ParentID == menuId) {
                                ids.push(i);
                            }
                        }

                        var onclickParent = 'Fanex.ExtAcc.TogglejqRows(' + menuId + ',[' + ids + '])';
                        div = '<div id="fn-' + menuId + '" onclick="' + onclickParent + '" style="float: left; cursor: pointer" class="itemParent">&nbsp;</div>';
                    }

                    index++;

                    return div + '<div class="' + style + '"><a href="' + func + '">' + value + '</a></div>';
                }
            },
            { name: 'MenuDesc', align: 'left', width: 150, sortable: false, resizable: false },
        //                  { name: 'ParentName', align: 'left', width: 120, sortable: false, resizable: false },
            {name: 'Url', align: 'left', width: 200, sortable: false, resizable: false },
            { name: 'FuncName', align: 'left', width: 150, sortable: false, resizable: false },
            { name: 'DisplayOrder', align: 'center', width: 50, sortable: false, resizable: false },
            { name: 'ModifiedDate', align: 'center', width: 150, sortable: false, resizable: false },
            { name: '', align: 'center', width: 50, sortable: false, resizable: false,
                formatter: function (value, model, row) {
                    var style = row.IsActive == true ? "menuActive" : "menuInactive";
                    var menuId = row.MenuID;
                    var menuName = row.MenuName;
                    var onclick = "javascript:Fanex.ExtAcc.Menus.ShowActivate(" + menuId + ", '" + menuName + "')";

                    return '<div id="menu-' + menuId + '" class="' + style + '" onclick="' + onclick + '"></div>';
                }
            },
            { name: '', align: 'center', width: 50, sortable: false, resizable: false,
                formatter: function (value, model, row) {
                    var style = "menuDelete";
                    var menuId = row.MenuID;
                    var menuName = row.MenuName;
                    var onclick = "javascript:Fanex.ExtAcc.Menus.ShowDelete(" + menuId + ", '" + menuName + "')";

                    return '<div id="' + menuId + '" class="' + style + '" onclick="' + onclick + '"></div>';
                }
            }
        ];

        Fanex.ExtAcc.Menus.RenderGrid(Fanex.ExtAcc.Menus.dataTexts);

        $('#jqbutton').bind('click', function () {
            Fanex.ExtAcc.Form.Show(resolveClientUrl("Menu/AddMenu"), $.parseJSON(menuTitles).Create, 400, 460, false);
        });

        $this = this;

        Fanex.ExtAcc.OnResize = function () {
            $this.leftMenuWidth = Fanex.ExtAcc.leftMenuWidth;
            try {
                $this.Resize(true);
            }
            catch (err) {
            }
        }
    },

    CalculateHeight: function () {
        this.gridHeight = Math.round($("#footer").position().top - $("#jqgrid").position().top);
        var oHeight = 103;
        var mHeight = this.totalRows * 25 + oHeight;
        if (mHeight < this.gridHeight) {
            this.gridHeight = mHeight
        }
    },

    Resize: function (autoResizeGrid) {
        try {
            this.CalculateHeight();

            // Calculate totalWidth
            this.totalWidth = 0;
            for (var i = 0; i < this.DataFields.length; i++) {
                this.totalWidth += this.DataFields[i].width;
            }

            var width = this.width = $("#footer").position().left - this.leftMenuWidth - 8;
            var height = this.gHeight - (width > this.totalWidth ? 16 : 0);

            if (this.gSize && this.gSize.width == width && this.gSize.height == height) {
                return;
            }
            else {
                if (autoResizeGrid && this.gSize.width != width) {
                    $("#jqgrid").setGridWidth(width, true);
                }
                this.gSize = { width: width, height: height };
            }
        } catch (e) { }
    },

    RenderGrid: function (columns) {
        var titles = $.parseJSON(menuTitles);

        $("#jqgrid").jqGrid(
            {
                autowidth: true,
                url: Fanex.ExtAcc.Menus.URL,
                datatype: "json",
                colNames: columns,
                colModel: Fanex.ExtAcc.Menus.DataFields,
                height: '100%',
                jsonReader: { repeatitems: false },
                hoverrows: true,
                beforeSelectRow: function (rowid, e) {
                    return false;
                },
                beforeProcessing: function (data, status, xhr) {
                    Fanex.ExtAcc.Menus.DataSource = data;
                },
                loadComplete: function () {
                    $("tr.jqgrow").css("background", "#ffffff");
                }
            });
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

        var name = $('#MenuName').val();
        var desc = $('#MenuDesc').val();
        var parentId = $('#ParentName').val() != '-1' ? $('#ParentName').val() : '0';
        var url = $('#Url').val();
        var funcId = $('#FuncID').val();
        var order = $('#DisplayOrder').val();

        var editUrl = 'Menu/Add/?';
        var params = '';

        params += '&name=' + name;
        params += '&description=' + desc;
        params += '&url=' + url;
        params += '&functionId=' + funcId;
        params += '&parentId=' + parentId;
        params += '&parentName=' + ($('#ParentName').val() != '-1' ? $('#ParentName option:selected').text() : '');
        params += '&displayOrder=' + order;

        $.ajax({ url: resolveClientUrl(editUrl + params), type: 'POST' })
        .done(function (data) {
            if (!data.IsSuccess) {
                $('#errMsg').css('display', 'block');
                $('#errMsg').html(data.ErrorMessage);
            }
            else {
                Fanex.ExtAcc.Form.Close(true);
            }
        })
        .error(function (data) {
            if (typeof console != 'undefined') console.log(data.responseText);
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

        var name = $('#MenuName').val();
        var desc = $('#MenuDesc').val();
        var parentId = $('#ParentName').val() != '-1' ? $('#ParentName').val() : '0';
        var url = $('#Url').val();
        var funcId = $('#FuncID').val();
        var order = $('#DisplayOrder').val();

        var editUrl = 'Menu/Edit/?';
        var params = 'menuId=' + $('#MenuID').val();

        params += '&name=' + name;
        params += '&description=' + desc;
        params += '&url=' + url;
        params += '&functionId=' + funcId;
        params += '&parentId=' + parentId;
        params += '&parentName=' + ($('#ParentName').val() != '-1' ? $('#ParentName option:selected').text() : '');
        params += '&displayOrder=' + order;

        $.ajax({ url: resolveClientUrl(editUrl + params), type: 'POST' })
        .done(function (data) {
            if (!data.IsSuccess) {
                $('#errMsg').css('display', 'block');
                $('#errMsg').html(data.ErrorMessage);
            }
            else {
                Fanex.ExtAcc.Form.Close(true);
            }
        })
        .error(function (data) {
            if (typeof console != 'undefined') console.log(data.responseText);
        });
    },

    Activate: function (menuId, isActive) {
        $.ajax({ url: resolveClientUrl('Menu/UpdateStatus/?menuId=' + menuId + '&isActive=' + isActive), type: 'POST' })
        .done(function (data) {
            if (!data.IsSuccess) {
                alert(data.ErrorMessage);
            }
            else {
                Fanex.ExtAcc.Form.Close(true);
            }
        })
        .error(function (data) {
            if (typeof console != 'undefined') console.log(data.responseText);
        });
    },

    Delete: function (menuId) {
        $.ajax({ url: resolveClientUrl('Menu/Delete/?menuId=' + menuId), type: 'POST' })
        .done(function (data) {
            if (!data.IsSuccess) {
                alert(data.ErrorMessage);
            }
            else {
                Fanex.ExtAcc.Form.Close(true);
            }
        })
        .error(function (data) {
            if (typeof console != 'undefined') console.log(data.responseText);
        });
    }
}