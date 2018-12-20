/// <reference path="../_references.js" />
if (typeof Fanex == 'undefined' || typeof Fanex.ExtAcc == 'undefined') {
    alert("Fanex.ExtAcc.js module is required!")
}

Fanex.ExtAcc.Permission = {
    Init: function () {
        this.RenderPermissionGrid();

        //$.ajax({
        //    url: this.URL,
        //    dataType: 'json',
        //    type: 'post'
        //})
        //.success(function (data) {
        //    $this.DataSource = data;

        Fanex.ExtAcc.OnResize = function () {
            try {
                $("#jqgrid-permission").fanexGrid("resizeGrid");
            }
            catch (err) {
            }
        }

        $(window).bind("orientationchange", function (e) {
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

            var dialog = top.$(".ui-dialog");
            dialog.height((height + 34) + "px");

            top.$("#windowPopup").css("height", height + "px");
        });
    },

    RenderPermissionGrid: function () {
        Fanex.ExtAcc.BlockUI();

        var url = resolveClientUrl("Function/FunctionListPost");
        var mainGrid = $("#mainGrid").css("visibility", "hidden");

        var data = [
            { Username: "starix1", GroupName: "ACCUUS", SourcePermission: 1, DestinationPermission: 1, PermissionKey: 1 },
            { Username: "starix2", GroupName: "ACCUUS", SourcePermission: 3, DestinationPermission: 3, PermissionKey: 2 },
            { Username: "starix3", GroupName: "ACCUUS", SourcePermission: 1, DestinationPermission: 1, PermissionKey: 3 },
            { Username: "starix4", GroupName: "ACCUUS", SourcePermission: 3, DestinationPermission: 3, PermissionKey: 4 },
            { Username: "starix5", GroupName: "ACCUUS", SourcePermission: 0, DestinationPermission: 0, PermissionKey: 5 }
        ];

        var bodyRows = [];
        var datum;

        for (var i = 0, max = data.length; i < max; i++) {
            datum = data[i];
            bodyRows.push({
                Columns: [
                    { Name: "colUsername", Value: datum.Username, CssClass: "colUsername" },
                    { Name: "colGroupName", Value: datum.GroupName },
                    { Name: "colSourcePermissionRead", Value: datum.SourcePermission },
                    { Name: "colSourcePermissionWrite", Value: datum.SourcePermission },
                    { Name: "colDestinationPermissionRead", Value: datum },
                    { Name: "colDestinationPermissionWrite", Value: datum }
                ]
            });
        }

        var gridOptions = this.GetGridOptions(bodyRows);
        setTimeout(function () {
            $("#jqgrid-permission").fanexGrid(gridOptions);
            Fanex.ExtAcc.UnBlockUI();
            mainGrid.css("visibility", "visible");
        }, 1000);
    },

    GetGridOptions: function (bodyRows) {
        return {
            customizeHeader: true,
            groupHeaderRows: [
                {
                    Columns: [
                        { HeaderText: "Username", Name: "colUsername", MergeRowNumber: 2 },
                        { HeaderText: "Group Name", Name: "colGroupName", MergeRowNumber: 2 },
                        { HeaderText: "Source Permission", Name: "colSourcePermissionRead", MergeColNumber: 2 },
                        { HeaderText: "Destination Permission", Name: "colDestinationPermissionRead", MergeColNumber: 2 }
                    ]
                }
            ],
            columns: [
                { HeaderText: "Username", Name: "colUsername", Width: 200 },
                { HeaderText: "Group Name", Name: "colGroupName", Width: 200 },
                {
                    HeaderText: "Read",
                    Name: "colSourcePermissionRead",
                    Width: 70,
                    CellAlign: "center",
                    TdClass: "bg-grey",
                    CustomCellRender: function (sourcePermission) {
                        if (sourcePermission.Value === 0) {
                            return '<div class="permInactive" style="cursor:default"></div>';
                        }

                        return sourcePermission.Value > 0
                            ? '<div class="permActive" style="cursor:default"></div>'
                            : "";
                    }
                },
                {
                    HeaderText: "Write",
                    Name: "colSourcePermissionWrite",
                    Width: 70,
                    CellAlign: "center",
                    TdClass: "bg-grey",
                    CustomCellRender: function (sourcePermission) {
                        return sourcePermission.Value === 3
                            ? '<div class="permActive" style="cursor:default"></div>'
                            : '<div class="permInactive" style="cursor:default"></div>';
                    }
                },
                {
                    HeaderText: "Read",
                    Name: "colDestinationPermissionRead",
                    Width: 70,
                    CellAlign: "center",
                    CustomCellRender: function (cell) {
                        return [
                            '<div class="',
                            cell.Value.DestinationPermission === 0 ? "permInactive" : "permActive",
                            '" onclick="javascript:Fanex.ExtAcc.Permission.OnReadChanged(this);" id="read',
                            cell.Value.Username,
                            '" data-username="',
                            cell.Value.Username,
                            '" data-permissionkey="',
                            cell.Value.PermissionKey,
                            '"></div>'
                        ].join("");
                    }
                },
                {
                    HeaderText: "Write",
                    Name: "colDestinationPermissionWrite",
                    Width: 70,
                    CellAlign: "center",
                    CustomCellRender: function (cell) {
                        return [
                            '<div class="',
                            cell.Value.DestinationPermission === 3 ? "permActive" : "permInactive",
                            '" onclick="javascript:Fanex.ExtAcc.Permission.OnWriteChanged(this);" id="write',
                            cell.Value.Username,
                            '" data-username="',
                            cell.Value.Username,
                            '" data-permissionkey="',
                            cell.Value.PermissionKey,
                            '"></div>'
                        ].join("");
                    }
                }
            ],
            bodyRows: bodyRows || [],
            autoCalculateGridExpandHeight: true,
            marginRight: 10,
            showDistinctRow: false,
            allowTruncate: true
        };
    },

    OnReadChanged: function (target) {
        var $target = this.OnPermissionChanged(target);
        if ($target.hasClass("permInactive")) {
            $("#write" + $target.data("username"))
                .removeClass()
                .addClass("permInactive");
        }
    },

    OnWriteChanged: function (target) {
        var $target = this.OnPermissionChanged(target);
        if ($target.hasClass("permActive")) {
            $("#read" + $target.data("username"))
                .removeClass()
                .addClass("permActive");
        }
    },

    OnPermissionChanged: function (target) {
        var $target = $(target);
        var checked = $target.hasClass("permActive");
        return $target
            .removeClass()
            .addClass(checked ? "permInactive" : "permActive");
    },

    Submit: function () {
        Fanex.ExtAcc.BlockUI();

        var formData = this.GetFormData();
        var jqxhr = $.ajax({
            type: "POST",
            url: resolveClientUrl("Function/FunctionListPost"),
            headers: this.SetupAntiXSRF(),
            data: JSON.stringify(formData),
            dataType: "json",
            contentType: "application/json; charset=utf-8"
        });

        jqxhr
            .done(function (data) { })
            .fail(function () { })
            .always(Fanex.ExtAcc.UnBlockUI);
    },

    GetFormData: function () {
        var formData = [];
        var tableContent = $("#tblContent_jqgrid-permission");
        var usernameArray = tableContent.find(".colUsername");

        for (var i = 0, max = usernameArray.length; i < max; i++) {
            formData.push(this.GetUserPermissionPolicy(tableContent, usernameArray[i].innerHTML));
        }

        return formData;
    },

    GetUserPermissionPolicy: function (tableContent, username) {
        var policy = 0;
        var readElement = tableContent.find("#read" + username);
        var permissionKey = readElement.data("permissionkey");

        if (readElement.hasClass("permActive")) {
            policy = 1;

            var writeElement = $("#write" + username);

            if (writeElement.hasClass("permActive")) {
                policy = 3;
            }
        }

        return { Username: username, PermissionKey: permissionKey, Policy: policy };
    },

    SetupAntiXSRF: function () {
        var headers = {};
        var token = $("[name=__RequestVerificationToken]").val();
        headers["__RequestVerificationToken"] = token;

        return headers;
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
            $('#jqgrid-permission').fanexGrid(options);
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
    Fanex.ExtAcc.Permission.Init();
});