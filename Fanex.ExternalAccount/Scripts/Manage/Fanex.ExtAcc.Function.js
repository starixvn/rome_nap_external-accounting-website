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

Fanex.ExtAcc.Function = {
    DataFields: null,
    DataSource: null,

    FunctionId: null,

    URL: '',

    $this: null,

    Init: function () {
        Fanex.ExtAcc.BlockUI();

        $('#mainGrid').css('visibility', 'hidden');

        $this = this;

        this.URL = resolveClientUrl("Function/FunctionListPost");

        var titles = $.parseJSON(menuTitles);
        var index = 0;
        var options = null;

        $this.DataFields = [
            {
                HeaderText: 'Function', Name: 'FuncName', Width: 220,
                CustomCellRender: function (cell) {
                    var funcId = cell.Value.FuncID;
                    var isParent = cell.Value.ParentID == 0;
                    var hasChild = false;

                    if ($this.DataSource.rows[index + 1]) {
                        hasChild = $this.DataSource.rows[index + 1].ParentID != 0;
                    }

                    var func = "javascript:Fanex.ExtAcc.Function.ShowEdit(" + funcId + ", '" + titles.EditFunction + "')";
                    var style = isParent && !hasChild ? "menuItemParentOnly" : (!isParent ? "menuItemChild" : "menuItemParent");
                    var div = '';

                    if (isParent && hasChild) {
                        var ids = [];
                        var length = $this.DataSource.rows.length;

                        for (var i = 0; i < length; i++) {
                            if ($this.DataSource.rows[i].ParentID == funcId) {
                                ids.push(i);
                            }
                        }

                        var onclickParent = 'Fanex.ExtAcc.TogglejqRows(' + funcId + ',[' + ids + '])';
                        div = '<div id="fn-' + funcId + '" onclick="' + onclickParent + '" style="float: left; cursor: pointer" class="itemParent">&nbsp;</div>';
                    }

                    index++;

                    return div + '<div class="' + style + '"><a href="' + func + '">' + cell.Value.FuncName + '</a></div>';
                }
            },
            { HeaderText: 'Key', Name: 'FuncKey', Width: 200 },
            { HeaderText: 'Description', Name: 'FuncDesc', Width: 245, AllowTruncate: true },
            { HeaderText: 'Order', Name: 'DisplayOrder', Width: 50, CellAlign: 'center' },
            { HeaderText: 'Modified Date', Name: 'ModifiedDate', Width: 160 },
            {
                HeaderText: 'Is Test', Name: 'IsTest', Width: 50, CellAlign: 'center',
                CustomCellRender: function (cell) {
                    var style = cell.Value.IsTest == true ? "menuActive" : "menuInactive";
                    var funcId = cell.Value.FuncID;
                    var funcName = cell.Value.FuncName;
                    var onclick = "javascript:Fanex.ExtAcc.Function.ShowActivate(" + funcId + ", '" + funcName + "')";

                    return '<div id="test-' + funcId + '" class="' + style + '" onclick="' + onclick + '"></div>';
                }
            },
        ];

        $.ajax({
            url: this.URL,
            dataType: 'json',
            type: 'post'
        })
        .success(function (data) {
            $this.DataSource = data;

            var rows = data.rows;
            var bodyRows = [];

            for (var i = 0; i < rows.length; i++) {
                bodyRows.push({
                    Columns: [
                        { Name: 'FuncName', Value: rows[i] },
                        { Name: 'FuncKey', Value: rows[i].FuncKey },
                        { Name: 'FuncDesc', Value: rows[i].FuncDesc },
                        { Name: 'DisplayOrder', Value: rows[i].DisplayOrder },
                        { Name: 'ModifiedDate', Value: rows[i].ModifiedDate },
                        { Name: 'IsTest', Value: rows[i] }
                    ]
                });
            }
            var widthDevice = $(window).width();
            var heightExpand = 0;
            if (widthDevice < 480) {
                heightExpand = 140;
            }
            else {
                heightExpand = 180;
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
            $this.AddNew();
        });

        Fanex.ExtAcc.OnResize = function () {
            try {
                $("#jqgrid-function").fanexGrid("resizeGrid");
                //$("#jqgrid").fanexGrid("resizeHeight", 160);
            }
            catch (err) {
            }
        }

        $(window).bind('orientationchange', function (e) {

            top.landscapeHeight = 260;

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
            $('#jqgrid-function').fanexGrid(options);
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

    ShowEdit: function (funcId, title) {
        Fanex.ExtAcc.Function.FunctionId = funcId;
        top.portraitHeight = 500;
        top.shouldCalculatePopupHeight = true;

        Fanex.ExtAcc.Form.Show(
            resolveClientUrl('Function/EditFunction/' + funcId),
            title,
            410,
            Fanex.ExtAcc.Function.GetPopupHeight(top.landscapeHeight, top.portraitHeight),
            false,
            function () {
                var url = resolveClientUrl("Function/PolicyList/?funcId=" + $('#FuncID').val());

                $.ajax(url).success(function (result) {
                    var groups = '';

                    $.each(result, function (i) {
                        groups += '<option value="' + result[i].GroupID + '"' + (result[i].HasPermission ? ' selected="selected">' : '>') + result[i].GroupName + '</option>';
                    });

                    $('#groups').html(groups);

                    $('#groups').css('display', 'block');
                    $("#groups").multiSelect({
                        minWidth: 255,
                        showHeader: false,
                        onCheck: function () {
                            if (this.value == '-1') {
                                var ids = $(".ui-multiselect-checkboxes :input");
                                for (var i = 0; i < ids.length; i++) {
                                    ids[i].checked = this.checked;
                                }
                            }
                            else {
                                if (!this.checked) {
                                    $('#ui-multiselect-groups-option-0')[0].checked = false;
                                }
                            }
                        },
                        noneSelectedText: '',
                        selectedText: function (numChecked, numTotal, checkedItems) {
                            var tmp = '';
                            var itemsChecked = '';

                            if (checkedItems[0].value != '-1') {
                                numChecked++;
                            }

                            if (numChecked != numTotal) {
                                var mainTitle = '';

                                $.each(checkedItems, function (i) {
                                    mainTitle += checkedItems[i].title + ', ';

                                    itemsChecked += checkedItems[i].value + ',';

                                    if (itemsChecked.indexOf(',') != -1) {
                                        tmp = itemsChecked.substr(0, itemsChecked.length - 1);
                                    }
                                });

                                mainTitle = mainTitle.substring(0, mainTitle.length - 2);
                                $("#groups").attr("title", mainTitle);
                                $('#newGroups').val(tmp.replace('-1,', ''));

                                return mainTitle;
                            }
                            else {
                                $('#ui-multiselect-groups-option-0')[0].checked = true;
                                $("#groups").attr("title", "All");

                                $.each(checkedItems, function (i) {
                                    itemsChecked += checkedItems[i].value + ',';

                                    if (itemsChecked.indexOf(',') != -1) {
                                        tmp = itemsChecked.substr(0, itemsChecked.length - 1);
                                    }
                                });
                                $('#newGroups').val(tmp.replace('-1,', ''));

                                return 'All';
                            }
                        }
                    });

                    $('#groups input').css('width', '235px');
                });
            });
    },

    ShowActivate: function (funcId, funcName) {
        var titles = $.parseJSON(menuTitles);
        var title = '';
        var warning = 'This function will be <font style="color: #ff0000">{0}</font> on grant permission form<br> on PRODUCTION environment';
        top.shouldCalculatePopupHeight = false;

        title = $('#test-' + funcId).attr('class') == 'menuActive' ? titles.Deactivate : titles.Activate;
        warning = $('#test-' + funcId).attr('class') == 'menuActive' ?
            warning.replace('{0}', 'shown') : warning.replace('{0}', 'hidden');

        Fanex.ExtAcc.Function.FunctionId = funcId;
        Fanex.ExtAcc.Form.ConfirmBox(
            title,
            'Are you sure to ' + title.toLowerCase() + ' [Is Test] of function <font color="#4b91be">' + funcName + '</font>?<br>' +
            warning,
            400,
            190,
            false,
            function () {
                Fanex.ExtAcc.Function.Activate(funcId, !($('#test-' + funcId).attr('class') == 'menuActive'));
            });
    },

    ShowDelete: function (funcId, funcName) {
        Fanex.ExtAcc.Function.FunctionId = funcId;
        top.shouldCalculatePopupHeight = false;

        var titles = $.parseJSON(menuTitles);
        var title = titles.Delete;

        Fanex.ExtAcc.Form.ConfirmBox(
            title,
            'Are you sure to delete func <font color="#4b91be">' + funcName + '</font> ?',
            400,
            150,
            false,
            function () {
                Fanex.ExtAcc.Function.Delete(funcId);
            });
    },

    AddNew: function () {
        top.portraitHeight = 500;
        top.shouldCalculatePopupHeight = true;
        Fanex.ExtAcc.Form.Show(
            resolveClientUrl("Function/AddFunction"), $.parseJSON(menuTitles).CreateFunction,
            400,
            Fanex.ExtAcc.Function.GetPopupHeight(top.landscapeHeight, top.portraitHeight),
            false,
            function () {
                var url = resolveClientUrl("Function/PolicyList/?funcId=1&addNew=true");
                var itemsChecked = '';

                $.ajax(url).success(function (result) {
                    var groups = '';

                    $.each(result, function (i) {
                        groups += '<option value="' + result[i].GroupID + '"' + (result[i].HasPermission ? ' selected="selected">' : '>') + result[i].GroupName + '</option>';
                    });

                    $('#groupsAdd').html(groups);

                    $(".groupCboMenu input").val($("#ParentID")[0].options[0].text);

                    $('#groupsAdd').css('display', 'block');
                    $("#groupsAdd").multiSelect({
                        minWidth: 255,
                        showHeader: false,
                        onCheck: function () {
                            if (this.value == '-1') {
                                var ids = $(".ui-multiselect-checkboxes :input");
                                for (var i = 0; i < ids.length; i++) {
                                    ids[i].checked = this.checked;
                                }
                            }
                            else {
                                if (!this.checked) {
                                    $('#ui-multiselect-groupsAdd-option-0')[0].checked = false;
                                }
                            }
                        },
                        noneSelectedText: function () {
                            $("#groupsAdd").attr("title", "");
                            return '';
                        },
                        selectedText: function (numChecked, numTotal, checkedItems) {
                            var tmp = '';
                            var itemsChecked = '';

                            if (checkedItems[0].value != '-1') {
                                numChecked++;
                            }

                            if (numChecked != numTotal) {
                                var mainTitle = '';
                                $.each(checkedItems, function (i) {
                                    mainTitle += checkedItems[i].title + ', ';

                                    itemsChecked += checkedItems[i].value + ',';

                                    if (itemsChecked.indexOf(',') != -1) {
                                        tmp = itemsChecked.substr(0, itemsChecked.length - 1);
                                    }
                                });

                                mainTitle = mainTitle.substring(0, mainTitle.length - 2);
                                $("#groupsAdd").attr("title", mainTitle);
                                $('#newGroups').val(tmp.replace('-1,', ''));

                                return mainTitle;
                            }
                            else {
                                $('#ui-multiselect-groupsAdd-option-0')[0].checked = true;
                                $("#groupsAdd").attr("title", "All");

                                $.each(checkedItems, function (i) {
                                    itemsChecked += checkedItems[i].value + ',';

                                    if (itemsChecked.indexOf(',') != -1) {
                                        tmp = itemsChecked.substr(0, itemsChecked.length - 1);
                                    }
                                });
                                $('#newGroups').val(tmp.replace('-1,', ''));

                                return 'All';
                            }
                        }
                    });

                    $('#groupsAdd input').css('width', '235px');
                });
            });
    },

    Add: function () {
        if (!Validators.validateInput($('#FuncName').val())) {
            $('#FuncName').focus();
            Validators.showDown($('#FuncName').get(0), 'This field is required', 0, 10, 41, -10, 15);
            return false;
        }

        if (!Validators.validateName($('#FuncName').val())) {
            $('#FuncName').focus();
            Validators.showDown($('#FuncName').get(0), 'Please input alphanumeric characters only', 0, 10, 41, -10, 15);
            return false;
        }

        if (!Validators.validateInput($('#FuncKey').val())) {
            $('#FuncKey').focus();
            Validators.show($('#FuncKey').get(0), 'This field is required');
            return false;
        }

        if (!Validators.validateKey($('#FuncKey').val())) {
            $('#FuncKey').focus();
            Validators.show($('#FuncKey').get(0), 'Please input alphanumeric characters only');
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

        var name = $('#FuncName').val();
        var desc = $('#FuncDesc').val();
        var parentId = $('#ParentID').val() != '-1' ? $('#ParentID').val() : '0';
        var parent = $('#ParentID option:selected').val() != '-1' ? $('#ParentID option:selected').text() : '';
        var order = $('#DisplayOrder').val();
        var newGroups = $('#newGroups').val();
        var key = $('#FuncKey').val();

        var editUrl = 'Function/Add';
        var params = new Object();

        params.funcname = Fanex.ExtAcc.EscapeHtml(name);
        params.funckey = Fanex.ExtAcc.EscapeHtml(key);
        params.funcdesc = Fanex.ExtAcc.EscapeHtml(desc);
        params.parentId = parentId;
        params.parentname = parent;
        params.displayorder = order;
        params.groups = newGroups;

        $.ajax({ url: resolveClientUrl(editUrl), data: params, type: 'POST' })
        .done(function (data) {
            if (!data.isSuccess) {
                Fanex.ExtAcc.UnBlockUI();

                $('#errMsg').css('display', 'block');
                $('#errMsg').html(data.errorMessage.formatErrorMessage());
            }
            else {
                var timeout = setTimeout(function () {
                    Fanex.ExtAcc.Form.Close(false);

                    Fanex.ExtAcc.LoadMainFrame(resolveClientUrl('Function/FunctionList/' + $('#selectedGroupName').html()), 'dll-Administration');
                }, 2000);
            }
        });
    },

    Edit: function () {
        if (!Validators.validateInput($('#FuncName').val())) {
            $('#FuncName').focus();
            Validators.showDown($('#FuncName').get(0), 'This field is required', 0, 10, 41, -10, 15);
            return false;
        }

        if (!Validators.validateName($('#FuncName').val())) {
            $('#FuncName').focus();
            Validators.showDown($('#FuncName').get(0), 'Please input alphanumeric characters only', 0, 10, 41, -10, 15);
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

        var name = $('#FuncName').val();
        var desc = $('#FuncDesc').val();
        var parentId = $('#ParentID').val() != '-1' ? $('#ParentID').val() : '0';
        var parent = $('#ParentID option:selected').val() != '-1' ? $('#ParentID option:selected').text() : '';
        var funcId = $('#FuncID').val();
        var order = $('#DisplayOrder').val();
        var oldGroups = $('#oldIDs').val();
        var newGroups = $('#newGroups').val();

        if (newGroups == '') {
            newGroups = oldGroups;
        }

        var editUrl = 'Function/Edit';
        var params = new Object();
        params.funcId = funcId;
        params.funcname = Fanex.ExtAcc.EscapeHtml(name);
        params.funcdesc = Fanex.ExtAcc.EscapeHtml(desc);
        params.parentId = parentId;
        params.parentname = parent;
        params.displayorder = order;
        params.oldGroups = oldGroups;
        params.groups = newGroups;

        $.ajax({ url: resolveClientUrl(editUrl), data: params, type: 'POST' })
        .done(function (data) {
            if (!data.isSuccess) {
                Fanex.ExtAcc.UnBlockUI();

                $('#errMsg').css('display', 'block');
                $('#errMsg').html(data.errorMessage.formatErrorMessage());
            }
            else {
                var timeout = setTimeout(function () {
                    Fanex.ExtAcc.Form.Close(false);

                    Fanex.ExtAcc.LoadMainFrame(resolveClientUrl('Function/FunctionList/' + $('#selectedGroupName').html()), 'dll-Administration');
                }, 2000);
            }
        });
    },

    Activate: function (funcId, isActive) {
        Fanex.ExtAcc.BlockUI();

        $.ajax({ url: resolveClientUrl('Function/UpdateStatus/?funcId=' + funcId + '&isTest=' + isActive), type: 'POST' })
        .done(function (data) {
            
            if (!data.isSuccess) { 
                alert(data.errorMessage);
            }
            else {
                Fanex.ExtAcc.Form.Close(false);
                if (!isActive) {
                    setTimeout(function () {
                        Fanex.ExtAcc.UnBlockUI();
                        Fanex.ExtAcc.Form.Alert(
                            'Warning',
                            'Please consider to <b>grant existing accounts permission</b> of new function in Ext NAP',
                            400,
                            160,
                            null,
                            function () {
                                var timeout = setTimeout(function () {
                                    Fanex.ExtAcc.Form.Show(
                                        resolveClientUrl("Permission/Index?funcId=" + funcId),
                                        "Grant Permisssion",
                                        860,
                                        760,
                                        false,
                                        null,
                                        null,
                                        "popupGrantPermission");
                                    Fanex.ExtAcc.LoadMainFrame(resolveClientUrl('Function/FunctionList/' + $('#selectedGroupName').html()), 'dll-Administration');
                                }, 500);
                            });
                    }, 300);
                }
                else {
                    var timeout = setTimeout(function () {
                        Fanex.ExtAcc.UnBlockUI();
                        Fanex.ExtAcc.LoadMainFrame(resolveClientUrl('Function/FunctionList/' + $('#selectedGroupName').html()), 'dll-Administration');
                    }, 2000);
                }
            }
        });
    },

    Delete: function (funcId) {
        $.ajax({ url: resolveClientUrl('Function/Delete/?funcId=' + funcId), type: 'POST' })
        .done(function (data) {
            if (!data.isSuccess) {
                $('#errMsg').css('display', 'block');
                $('#errMsg').html(data.errorMessage.formatErrorMessage());
            }
            else {
                Fanex.ExtAcc.Form.Close(true);
            }
        });
    }
}

$(document).ready(function () {
    Fanex.ExtAcc.Function.Init();
});