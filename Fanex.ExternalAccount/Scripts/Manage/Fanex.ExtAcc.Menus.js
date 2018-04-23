/// <reference path="../_references.js" />
if (typeof Fanex == 'undefined' || typeof Fanex.ExtAcc == 'undefined') {
    alert("Fanex.ExtAcc.js module is required!")
}

Validators.validateName = function (str) {
    var reg = /^[A-z0-9 ]+$/;

    if (!reg.test(str)) {
        return false;
    }

    return true;
};

Validators.validateKey = function (str) {
    var reg = /^[A-z0-9]+$/;

    if (!reg.test(str)) {
        return false;
    }

    return true;
};

Validators.validateNumber = function (value) {
    if (value && value != "") {
        value = value.replace(/^\s\s*/, '').replace(/\s\s*$/, '');
        var intRegex = /^\d+$/;

        if (!intRegex.test(value)) {
            return false;
        }

        return true;
    } else {
        return false;
    }
};

Validators.validateInput = function (value) {
    if (value && value != "") {
        value = value.replace(/^\s\s*/, '').replace(/\s\s*$/, '');

        if (value != "")
            return true;

        return false;
    }

    return false;
};

Validators.showDown = function (elementInput, message, xPos, pointerXPos, yPos, pointerYPos, height, timeout) {
    // <span class="hint"><span>This is the message.<span><span class="hint-pointer">&nbsp;</span></span>
    var hint = document.getElementById('fHintDown');
    var hintPointer = null;
    if (hint == null) {
        var hint = document.createElement("SPAN");
        hint.className = 'hint';
        var hintMsg = document.createElement("SPAN");
        hint.appendChild(hintMsg);
        hintPointer = document.createElement("SPAN");
        hintPointer.className = 'hint-pointer-down';
        hint.appendChild(hintPointer);
        hint.id = 'fHintDown';
        hint.style.top = '10px';
        hint.style.left = '10px';
        hint.style.zIndex = 999999;
        hint.onclick = function () { // Hide onclick
            hint.style.display = 'none';
        };
        document.body.appendChild(hint);
    };
    hintPointer = hint.lastChild;
    hint.firstChild.innerHTML = message;
    if (this.timeOutHideTooltip) {
        clearTimeout(this.timeOutHideTooltip);
        this.timeOutHideTooltip = null;
    };
    if (typeof (timeout) == 'undefined') {
        timeout = 1000;
    }
    this.timeOutHideTooltip = setTimeout(function () {
        if (hint == null) {
            return;
        }
        hint.style.display = 'none';
    }, timeout); // Auto hide in 1s
    if (elementInput != null) {
        if (typeof xPos == 'undefined') {
            xPos = 0;
        }
        if (typeof pointerXPos == 'undefined') {
            pointerXPos = 10;
        }
        if (typeof yPos == 'undefined') {
            yPos = -45;
        }
        if (typeof (pointerYPos) == 'undefined') {
            pointerYPos = 38;
        }
        if (typeof (height) == 'undefined') {
            height = 18;
        }
        hint.style.height = height + 'px';
        hintPointer.style.left = (pointerXPos + 'px');
        hintPointer.style.top = (pointerYPos + 'px');
        var pos = Validators.utils.findPosRelativeToViewport(elementInput);
        hint.style.left = (pos[0] + xPos) + 'px';
        hint.style.top = (pos[1] + yPos) + 'px';
        elementInput.onchange = function () {
            hint.style.display = 'none';
        };
        elementInput.focus();
    };
    hint.style.display = 'block';
};

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
                    var menuId = cell.Value.MenuId;
                    var isParent = cell.Value.ParentId == 0;
                    var hasChild = false;

                    if ($this.DataSource.rows[index + 1]) {
                        hasChild = $this.DataSource.rows[index + 1].ParentId != 0;
                    }

                    var func = "javascript:Fanex.ExtAcc.Menus.ShowEdit(" + cell.Value.MenuId + ", '" + titles.Edit + "')";
                    var style = isParent && !hasChild ? "menuItemParentOnly" : (!isParent ? "menuItemChild" : "menuItemParent");
                    var div = '';

                    if (isParent && hasChild) {
                        var ids = [];
                        var length = $this.DataSource.rows.length;

                        for (var i = 0; i < length; i++) {
                            if ($this.DataSource.rows[i].ParentId == menuId) {
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
            {
                HeaderText: 'Active', Name: 'IsActive', CellAlign: 'center', Width: 50,
                CustomCellRender: function (cell) {
                    var style = cell.Value.IsActive == true ? "menuActive" : "menuInactive";
                    var menuId = cell.Value.MenuId;
                    var menuName = cell.Value.MenuName;
                    var onclick = "javascript:Fanex.ExtAcc.Menus.ShowActivate(" + menuId + ", '" + menuName + "')";

                    return '<div id="menu-' + menuId + '" class="' + style + '" onclick="' + onclick + '"></div>';
                }
            },
            {
                HeaderText: 'Delete', Name: 'Delete', CellAlign: 'center', Width: 50,
                CustomCellRender: function (cell) {
                    var style = "icon-delete";
                    var menuId = cell.Value.MenuId;
                    var menuName = cell.Value.MenuName;
                    var onclick = "javascript:Fanex.ExtAcc.Menus.ShowDelete(" + menuId + ", '" + menuName + "')";

                    return '<div id="' + menuId + '" class="' + style + '" onclick="' + onclick + '"></div>';
                }
            }
        ];

        $.ajax({
            url: $this.URL,
            dataType: 'json',
            type: 'post'
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
                autoCalculateGridExpandHeight: true,
                marginRight: 10,
                showDistinctRow: false,
                allowTruncate: true
            };

            $this.RenderGrid(options);
        });

        $('#jqbutton').bind('click', function () {            
            top.shouldCalculatePopupHeight = true;
            top.portraitHeight = 500;
            top.landscapeHeight = 260;
            Fanex.ExtAcc.Form.Show(resolveClientUrl("Menu/AddMenu"), $.parseJSON(menuTitles).Create, 400, Fanex.ExtAcc.Menus.GetPopupHeight(top.landscapeHeight, top.portraitHeight), false);
        });

        Fanex.ExtAcc.OnResize = function () {
            try {
                $('#jqgrid-menu').fanexGrid('resizeGrid');
            }
            catch (err) {
            }
        }

        $(window).bind('orientationchange', function (e) {
            var calculatePopupheight = top.shouldCalculatePopupHeight && isMobile.phone;
            if (!calculatePopupheight) {
                return;
            }

            var height;
            var isLandscape = window.matchMedia("(orientation: landscape)").matches;
            if (isMobile.android.phone) {
                height = isLandscape ? top.portraitHeight : top.landscapeHeight;
            }
            else {
                height = isLandscape ? top.landscapeHeight : top.portraitHeight;
            }

            var dialog = top.$('.ui-dialog');
            dialog.height((height + 34) + 'px');

            top.$("#windowPopup").css("height", height + "px");
        });
    },

    GetPopupHeight: function (landscapeHeight, portraitHeight) {
        var height = isMobile.phone && window.matchMedia("(orientation: landscape)").matches ?
            landscapeHeight :
             portraitHeight;

        return height;
    },

    RenderGrid: function (options) {
        try {
            // Init Grid jqgrid
            $('#jqgrid-menu').fanexGrid(options);
            Fanex.ExtAcc.UnBlockUI();

            $('#mainGrid').css('visibility', 'visible');
        }
        catch (e) {
            if (typeof console != 'undefined') {
                console.log(e);
            }

            Fanex.ExtAcc.UnBlockUI();
        }
    },

    ShowEdit: function (menuId, title) {
        Fanex.ExtAcc.Menus.MenuId = menuId;
        top.portraitHeight = 500;
        top.landscapeHeight = 260;
        top.shouldCalculatePopupHeight = true;

        Fanex.ExtAcc.Form.Show(
            resolveClientUrl('Menu/EditMenu/' + menuId),
            title,
            400,
            Fanex.ExtAcc.Menus.GetPopupHeight(top.landscapeHeight, top.portraitHeight),
            false);
    },

    ShowActivate: function (menuId, menuName) {
        var titles = $.parseJSON(menuTitles);
        var title = '';
        top.shouldCalculatePopupHeight = false;

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
        top.shouldCalculatePopupHeight = false;

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

        if (!Validators.validateName($('#MenuName').val())) {
            $('#MenuName').focus();
            Validators.showDown($('#MenuName').get(0), 'Please input alphanumeric characters only', 0, 10, 41, -10, 15);
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

        params.menuname = Fanex.ExtAcc.EscapeHtml(name);
        params.menudesc = Fanex.ExtAcc.EscapeHtml(desc);
        params.url = Fanex.ExtAcc.EscapeHtml(url);
        params.funcId = funcId;
        params.parentId = parentId;
        params.parentName = ($('#ParentName').val() != '-1' ? $('#ParentName option:selected').text() : '');
        params.displayOrder = order;

        $.ajax({ url: resolveClientUrl(editUrl), data: params, type: 'POST' })
        .done(function (data) {
            if (!data.isSuccess) {
                Fanex.ExtAcc.UnBlockUI();

                $('#errMsg').css('display', 'block');
                $('#errMsg').html(data.errorMessage.formatErrorMessage());
            }
            else {
                var timeout = setTimeout(function () {
                    clearTimeout(timeout);
                    Fanex.ExtAcc.Form.Close();
                    Fanex.ExtAcc.LoadMainFrame(resolveClientUrl('Menu/MenuList/' + $('#selectedGroupName').html()), 'dll-Administration');
                }, 2000);
            }
        });
    },

    Edit: function () {
        if (!Validators.validateInput($('#MenuName').val())) {
            $('#MenuName').focus();
            Validators.showDown($('#MenuName').get(0), 'This field is required', 0, 10, 41, -10, 15);
            return false;
        }

        if (!Validators.validateName($('#MenuName').val())) {
            $('#MenuName').focus();
            Validators.showDown($('#MenuName').get(0), 'Please input alphanumeric characters only', 0, 10, 41, -10, 15);
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
        params.menuId = $('#MenuId').val();
        params.menuname = Fanex.ExtAcc.EscapeHtml(name);
        params.menudesc = Fanex.ExtAcc.EscapeHtml(desc);
        params.url = Fanex.ExtAcc.EscapeHtml(url);
        params.funcId = funcId;
        params.parentId = parentId;
        params.parentName = ($('#ParentName').val() != '-1' ? $('#ParentName option:selected').text() : '');
        params.displayOrder = order;

        $.ajax({ url: resolveClientUrl(editUrl), data: params, type: 'POST' })
        .done(function (data) {
            if (!data.isSuccess) {
                Fanex.ExtAcc.UnBlockUI();

                $('#errMsg').css('display', 'block');
                $('#errMsg').html(data.errorMessage.formatErrorMessage());
            }
            else {
                var timeout = setTimeout(function () {
                    clearTimeout(timeout);
                    Fanex.ExtAcc.Form.Close();
                    Fanex.ExtAcc.LoadMainFrame(resolveClientUrl('Menu/MenuList/' +$('#selectedGroupName').html()), 'dll-Administration');
                }, 2000);
            }
        });
    },

    Activate: function (menuId, isActive) {
        Fanex.ExtAcc.BlockUI();

        $.ajax({ url: resolveClientUrl('Menu/UpdateStatus/?menuId=' + menuId + '&isActive=' + isActive), type: 'POST' })
        .done(function (data) {
            if (!data.isSuccess) {
                Fanex.ExtAcc.UnBlockUI();

                alert(data.errorMessage);
            }
            else {
                var timeout = setTimeout(function () {
                    clearTimeout(timeout);
                    Fanex.ExtAcc.Form.Close();
                    Fanex.ExtAcc.LoadMainFrame(resolveClientUrl('Menu/MenuList/' + $('#selectedGroupName').html()), 'dll-Administration');
                }, 2000);
            }
        });
    },

    Delete: function (menuId) {
        Fanex.ExtAcc.BlockUI();

        $.ajax({ url: resolveClientUrl('Menu/Delete/?menuId=' + menuId), type: 'POST' })
        .done(function (data) {
            if (!data.isSuccess) {
                Fanex.ExtAcc.UnBlockUI();

                alert(data.errorMessage);
            }
            else {
                var timeout = setTimeout(function () {
                    clearTimeout(timeout);
                    Fanex.ExtAcc.Form.Close();
                    Fanex.ExtAcc.LoadMainFrame(resolveClientUrl('Menu/MenuList/' +$('#selectedGroupName').html()), 'dll-Administration');
                }, 2000);
            }
        });
    }
}

$(document).ready(function () {
    Fanex.ExtAcc.Menus.Init();
});