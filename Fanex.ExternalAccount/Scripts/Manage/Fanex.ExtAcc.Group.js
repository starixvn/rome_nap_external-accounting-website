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

Fanex.ExtAcc.Group = {
    DataFields: null,

    DataSource: null,
    GroupID: 0,

    URL: '',

    $this: null,

    Init: function () {
        Fanex.ExtAcc.BlockUI();

        $('#mainGrid').css('visibility', 'hidden');
        $this = this;

        var titles = $.parseJSON(menuTitles);
        $this.URL = resolveClientUrl("Group/GroupListPost");
        $this.DataFields = [
                    {
                        HeaderText: 'Group Name', Name: 'GroupName', Width: 195,
                        CustomCellRender: function (cell) {
                            var func = "javascript:Fanex.ExtAcc.Group.ShowEdit(" + cell.Value.GroupID + ", '" + titles.EditGroup + "')";

                            return '<div class="edit2"><a href="' + func + '">' + cell.Value.GroupName + '</a></div>';
                        }
                    },
                    {
                        HeaderText: 'No. of Users', Name: 'NoUsers', Width: 100, CellAlign: 'center',
                        CustomCellRender: function (cell) {
                            var groupId = cell.Value.GroupID;
                            var onclick = "javascript:Fanex.ExtAcc.Group.SearchUser(" + groupId + ")";

                            return '<div style="text-align: center"><a href="' + onclick + '">' + cell.Value.NoUsers + '</a></div>';
                        }
                    },
                    { HeaderText: 'Description', Name: 'Description', Width: 220 },
                    { HeaderText: 'Creator', Name: 'Creator', Width: 120, CellAlign: 'center' },
                    { HeaderText: 'Created Date', Name: 'CreatedDate', Width: 160, CellAlign: 'center' },
                    {
                        HeaderText: 'Permission', Name: 'Permission', Width: 75, CellAlign: 'center',
                        CustomCellRender: function (cell) {
                            var groupId = cell.Value.GroupID;
                            var groupName = cell.Value.GroupName;
                            var onclick = "javascript:Fanex.ExtAcc.Group.ShowPermission(" + groupId + ", '" + groupName + "')";

                            return '<div id="' + groupId + '" class="icon-clipboard-text" onclick="' + onclick + '"></div>';
                        }
                    },
                    {
                        HeaderText: 'Delete', Name: 'Delete', Width: 50, CellAlign: 'center',
                        CustomCellRender: function (cell) {
                            var style = "icon-delete";
                            var groupId = cell.Value.GroupID;
                            var groupName = cell.Value.GroupName;
                            var onclick = "javascript:Fanex.ExtAcc.Group.ShowDelete(" + groupId + ", '" + groupName + "')";

                            return '<div id="' + groupId + '" class="' + style + '" onclick="' + onclick + '"></div>';
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
            var rows = data.rows;
            var bodyRows = [];

            for (var i = 0; i < rows.length; i++) {
                bodyRows.push({
                    Columns: [
                        { Name: 'GroupName', Value: rows[i] },
                        { Name: 'NoUsers', Value: rows[i] },
                        { Name: 'Description', Value: rows[i].Description },
                        { Name: 'Creator', Value: rows[i].Creator },
                        { Name: 'CreatedDate', Value: rows[i].CreatedDate },
                        { Name: 'Permission', Value: rows[i] },
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
            top.shouldCalculatePopupHeight = false;
            Fanex.ExtAcc.Form.Show(resolveClientUrl('Group/AddGroup'), 'Add New Group', 400, 350, false);
        });

        Fanex.ExtAcc.OnResize = function () {
            try {
                $("#jqgrid-group").fanexGrid("resizeGrid");
            }
            catch (err) {
            }
        }

        $(window).bind('orientationchange', function (e) {

            top.landscapeHeight =  260;

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
            $('#jqgrid-group').fanexGrid(options);
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

    ShowEdit: function (groupId, title) {
        top.shouldCalculatePopupHeight = false;
        Fanex.ExtAcc.Group.GroupId = groupId;
        Fanex.ExtAcc.Form.Show(
            resolveClientUrl('Group/EditGroup/' + groupId),
            title,
            400,
            350,
            false);
    },

    ShowPermission: function (groupId, groupName) {
        Fanex.ExtAcc.Group.GroupId = groupId;

        var params = new Object();

        params.groupId = groupId;
        params.groupName = groupName;
        top.portraitHeight = 500;
        top.shouldCalculatePopupHeight = true;
        Fanex.ExtAcc.Form.Show(
            resolveClientUrl('Group/GrantPermission'),
            'Grant Permission',
            400,
            Fanex.ExtAcc.Group.GetPopupHeight(top.landscapeHeight, top.portraitHeight),
            false,
            null,
            0,
            null,
            params);
    },

    ShowDelete: function (groupId, groupName) {
        Fanex.ExtAcc.Group.GroupId = groupId;

        var titles = $.parseJSON(menuTitles);
        var title = titles.Delete;
        top.shouldCalculatePopupHeight = false;

        Fanex.ExtAcc.Form.ConfirmBox(
            'Delete Group',
            'Are you sure to delete group <font color="#4b91be">' + groupName + '</font> ?',
            400,
            150,
            false,
            function () {
                Fanex.ExtAcc.Group.Delete(groupId);
            });
    },

    Add: function () {
        if (!Validators.validateInput($('#GroupName').val())) {
            $('#GroupName').focus();
            Validators.showDown($('#GroupName').get(0), 'This field is required', 0, 10, 41, -10, 15);
            return false;
        }

        if (!Validators.validateName($('#GroupName').val())) {
            $('#GroupName').focus();
            Validators.showDown($('#GroupName').get(0), 'Please input alphanumeric characters only', 0, 10, 41, -10, 15);
            return false;
        }

        Fanex.ExtAcc.BlockUI();

        var name = $('#GroupName').val();
        var desc = $('#Description').val();
        var parentId = $('#ParentGroupId').val();

        var editUrl = 'Group/Add';
        var params = new Object();

        params.name = Fanex.ExtAcc.EscapeHtml(name);
        params.description = Fanex.ExtAcc.EscapeHtml(desc);
        params.parentId = Fanex.ExtAcc.EscapeHtml(parentId);

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

                    Fanex.ExtAcc.Form.Close(false);

                    Fanex.ExtAcc.LoadMainFrame(resolveClientUrl('Group/GroupList/' + $('#selectedGroupName').html()), 'dll-Administration');
                }, 2000);
            }
        });
    },

    Edit: function () {
        if (!Validators.validateInput($('#GroupName').val())) {
            $('#GroupName').focus();
            Validators.showDown($('#GroupName').get(0), 'This field is required', 0, 10, 41, -10, 15);
            return false;
        }

        if (!Validators.validateName($('#GroupName').val())) {
            $('#GroupName').focus();
            Validators.showDown($('#GroupName').get(0), 'Please input alphanumeric characters only', 0, 10, 41, -10, 15);
            return false;
        }

        Fanex.ExtAcc.BlockUI();

        var name = $('#GroupName').val();
        var desc = $('#Description').val();

        var editUrl = 'Group/Edit';
        var params = new Object();
        params.groupId = $('#GroupID').val();
        params.groupName = Fanex.ExtAcc.EscapeHtml(name);
        params.description = Fanex.ExtAcc.EscapeHtml(desc);

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
                    Fanex.ExtAcc.UnBlockUI();

                    Fanex.ExtAcc.Form.Close(false);

                    Fanex.ExtAcc.LoadMainFrame(resolveClientUrl('Group/GroupList/' + $('#selectedGroupName').html()), 'dll-Administration');
                }, 2000);
            }
        });
    },

    Activate: function (groupId, isActive) {
        $.ajax({ url: resolveClientUrl('Group/UpdateStatus/?groupId=' + groupId + '&isActive=' + isActive), type: 'POST' })
        .done(function (data) {
            if (!data.isSuccess) {
                alert(data.errorMessage);
            }
            else {
                Fanex.ExtAcc.Form.Close(true);
            }
        });
    },

    Delete: function (groupId) {
        Fanex.ExtAcc.BlockUI();

        $.ajax({ url: resolveClientUrl('Group/Delete/?groupId=' + groupId), type: 'POST' })
        .done(function (data) {
            if (!data.isSuccess) {
                Fanex.ExtAcc.UnBlockUI();

                alert(data.errorMessage);
            }
            else {
                var timeout = setTimeout(function () {
                    clearTimeout(timeout);

                    Fanex.ExtAcc.Form.Close(false);

                    Fanex.ExtAcc.LoadMainFrame(resolveClientUrl('Group/GroupList/' + $('#selectedGroupName').html()), 'dll-Administration');
                }, 2000);
            }
        });
    },

    SearchUser: function (groupId) {
        Fanex.ExtAcc.User.URL = resolveClientUrl('User/UserList/?username=&groupId=' + groupId + '&/' + $('#selectedGroupName').html())
        Fanex.ExtAcc.LoadMainFrame(resolveClientUrl('User/UserList/?username=&groupId=' + groupId + '&'));
    }
}

$(document).ready(function () {
    Fanex.ExtAcc.Group.Init();
});