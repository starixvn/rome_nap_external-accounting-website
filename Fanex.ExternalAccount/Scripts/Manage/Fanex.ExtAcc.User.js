/// <reference path="../_references.js" />
if (typeof Fanex == 'undefined' || typeof Fanex.ExtAcc == 'undefined') {
    alert("Fanex.ExtAcc.js module is required!")
}

Fanex.ExtAcc.User = {
    DataFields: null,
    DataSource: null,

    UserId: null,

    IsFirstLoad: true,
    URL: '',

    $this: null,
    options: null,

    Init: function () {
        //$('#mainGrid').css('visibility', 'hidden');
        $this = this;

        $("#GroupIDs").combobox({
        });

        $("#userGroup input").val($("#GroupIDs")[0].options[0].text);
        $("#userGroup input").attr("readonly", "true");
        $("#userGroup input").bind("click", function () {
            $(this).autocomplete("search", '');
        });

        var titles = $.parseJSON(menuTitles);

        $this.DataFields = [
            {
                HeaderText: 'Username', Name: 'Username', Width: 120,
                CustomCellRender: function (cell) {
                    var userId = cell.Value.UserID;

                    var func = "javascript:Fanex.ExtAcc.User.ShowEdit(" + userId + ", '" + titles.EditUser + "')";

                    return '<div style="margin: 3px"><a href="' + func + '">' + cell.Value.Username + '</a></div>';
                }
            },
            { HeaderText: 'First Name', Name: 'FirstName', Width: 100 },
            { HeaderText: 'Last Name', Name: 'LastName', Width: 100 },
            { HeaderText: 'Group Name', Name: 'GroupName', Width: 115 },
            { HeaderText: 'Creator', Name: 'Creator', Width: 120 },
            { HeaderText: 'Created Date', Name: 'CreatedDate', CellAlign: 'center', Width: 160 },
            { HeaderText: 'Permission', Name: 'Permission', CellAlign: 'center', Width: 75,
                CustomCellRender: function (cell) {
                    var userId = cell.Value.UserID;
                    var username = cell.Value.Username;
                    var groupName = cell.Value.GroupName;
                    var onclick = "javascript:Fanex.ExtAcc.User.ShowPermission(" + userId + ",'" + username + "','" + groupName + "')";

                    return '<div class="edit" onclick="' + onclick + '"></div>';
                }
            },
            { HeaderText: 'Active', Name: 'Enabled', CellAlign: 'center', Width: 50,
                CustomCellRender: function (cell) {
                    var style = cell.Value.Enabled == true ? "menuActive" : "menuInactive";
                    var userId = cell.Value.UserID;
                    var userName = cell.Value.Username;
                    var onclick = "javascript:Fanex.ExtAcc.User.ShowActivate(" + userId + ", '" + userName + "')";

                    return '<div id="' + userId + '" class="' + style + '" onclick="' + onclick + '"></div>';
                }
            },
            { HeaderText: 'Password', Name: 'ResetPwd', CellAlign: 'center', Width: 70,
                CustomCellRender: function (cell) {
                    var style = "menuResetPwd";
                    var userId = cell.Value.UserID;
                    var onclick = "javascript:Fanex.ExtAcc.User.ShowResetPwd(" + userId + ", false, '" + cell.Value.Username + "')";

                    return '<div class="' + style + '" onclick="' + onclick + '"></div>';
                }
            }
        ];

        $('#Usernames').bind('keypress', function (e) {
            var code = e.keyCode ? e.keyCode : e.which;

            if (code == 13) {
                $this.IsFirstLoad = false;
                $this.Search($this.IsFirstLoad);
            }
        });

        $this.Search($this.IsFirstLoad);

        $('#jqbuttonAddUser').bind('click', function () {
            Fanex.ExtAcc.Form.Show(resolveClientUrl("User/AddUser"), $.parseJSON(menuTitles).CreateUser, 400, 300, false);
        });

        $('#jqbuttonAddExistUser').bind('click', function () {
            Fanex.ExtAcc.Form.Show(resolveClientUrl("User/AddExistUser"), $.parseJSON(menuTitles).CreateExistUser, 400, 200, false);
        });

        Fanex.ExtAcc.OnResize = function () {
            try {
                $('#jqgrid').fanexGrid('resizeGrid');
            }
            catch (err) {
            }
        }
    },

    RenderGrid: function (params) {
        try {
            $.ajax({
                url: Fanex.ExtAcc.User.URL,
                dataType: 'json',
                type: 'post',
                data: params
            })
            .success(function (data) {
                Fanex.ExtAcc.User.DataSource = data;

                var rows = data.rows;
                var bodyRows = [];

                for (var i = 0; i < rows.length; i++) {
                    bodyRows.push({
                        Columns: [
                        { Name: 'Username', Value: rows[i] },
                        { Name: 'FirstName', Value: rows[i].FirstName },
                        { Name: 'LastName', Value: rows[i].LastName },
                        { Name: 'GroupName', Value: rows[i].GroupName },
                        { Name: 'Creator', Value: rows[i].Creator },
                        { Name: 'CreatedDate', Value: rows[i].CreatedDate },
                        { Name: 'Permission', Value: rows[i] },
                        { Name: 'Enabled', Value: rows[i] },
                        { Name: 'ResetPwd', Value: rows[i] }
                    ]
                    });
                }

                Fanex.ExtAcc.User.options = {
                    columns: Fanex.ExtAcc.User.DataFields,
                    bodyRows: bodyRows,
                    gridExpandHeight: 130,
                    marginRight: 2,
                    showDistinctRow: false,
                    allowTruncate: true
                };

                if (bodyRows.length == 0) {
                    $('#msgNoinfo').css('display', 'block');
                    $('#jqgrid').css('display', 'none');

                    $('#activeUsers').html('0');
                    $('#inActiveUsers').html('0');

                    $('#mainGrid').css('visibility', 'visible');
                }
                else {
                    $('#msgNoinfo').css('display', 'none');
                    $('#jqgrid').css('display', 'block');
                    // Init Grid jqgrid
                    $('#jqgrid').fanexGrid(Fanex.ExtAcc.User.options);

                    var divs = $('div');
                    var activeUsers = 0;
                    var inActiveUsers = 0;

                    for (var i = 0; i < divs.length; i++) {
                        if (divs[i].className == 'menuActive') {
                            activeUsers++;
                        }

                        if (divs[i].className == 'menuInactive') {
                            inActiveUsers++;
                        }
                    }

                    $('#activeUsers').html(activeUsers);
                    $('#inActiveUsers').html(inActiveUsers);

                    $('#mainGrid').css('visibility', 'visible');
                }

                Fanex.ExtAcc.UnBlockUI();
            });
        }
        catch (e) {
            if (typeof console != 'undefined') {
                console.log(e);
            }

            Fanex.ExtAcc.UnBlockUI();
        }
    },

    ShowEdit: function (userId, title) {
        Fanex.ExtAcc.User.UserId = userId;
        Fanex.ExtAcc.Form.Show(
            resolveClientUrl('User/EditUser/' + userId),
            title,
            400,
            265,
            false);
    },

    ShowActivate: function (userId, userName) {
        var titles = $.parseJSON(menuTitles);
        var title = '';

        title = $('#' + userId).attr('class') == 'menuActive' ? 'Deactivate User' : 'Activate User';

        Fanex.ExtAcc.User.UserId = userId;
        Fanex.ExtAcc.Form.ConfirmBox(
            title,
            'Are you sure to ' + title.toLowerCase() + ' <font color="#4b91be">' + userName + '</font> ?',
            400,
            150,
            false,
            function () {
                Fanex.ExtAcc.User.Activate(userId, !($('#' + userId).attr('class') == 'menuActive'));
            });
    },

    ShowResetPwd: function (userId, isOwner, username) {
        Fanex.ExtAcc.User.UserId = userId;

        var titles = $.parseJSON(menuTitles);
        var title = titles.ResetPwd;

        Fanex.ExtAcc.Form.Show(
            resolveClientUrl('User/' + (isOwner ? 'ResetPasswordOwner' : 'ResetPassword') + '/' + userId),
            title + ': <font color="#6dcff6">' + username + '</font>',
            415,
            190,
            false
            );
    },

    ShowPermission: function (userId, username, groupName) {
        Fanex.ExtAcc.User.UserId = userId;

        var params = new Object();

        params.userId = userId;
        params.username = username;
        params.groupName = groupName;

        Fanex.ExtAcc.Form.Show(
            resolveClientUrl('User/GrantPermission'),
            'Grant Permission',
            520,
            525,
            false,
            null,
            3,
            null,
            params);
    },

    Add: function () {
        if (!Validators.validateInput($('#Username').val())) {
            $('#Username').focus();
            Validators.showDown($('#Username').get(0), 'This field is required', 0, 10, 41, -10, 15);
            return false;
        }

        if (!Validators.validateKey($('#Username').val())) {
            $('#Username').focus();
            Validators.showDown($('#Username').get(0), 'Please input alphanumeric characters only', 0, 10, 41, -10, 15);
            return false;
        }

        if (!Validators.validateInput($('#Password').val())) {
            $('#Password').focus();
            Validators.show($('#Password').get(0), 'This field is required');
            return false;
        }

        Fanex.ExtAcc.BlockUI();

        var name = $('#Username').val();
        var pass = $('#Password').val();
        var firstName = $('#FirstName').val();
        var lastName = $('#LastName').val();
        var groupId = $('#GroupID').val();

        var editUrl = 'User/Add';
        var params = new Object();

        params.username = name;
        params.pwd = pass;
        params.firstName = Fanex.ExtAcc.EscapeHtml(firstName);
        params.lastName = Fanex.ExtAcc.EscapeHtml(lastName);
        params.groupId = groupId;

        $.ajax({ url: resolveClientUrl(editUrl), type: 'POST', data: params })
        .done(function (data) {
            if (!data.IsSuccess) {
                Fanex.ExtAcc.UnBlockUI();

                $('#errMsg').css('display', 'block');
                $('#errMsg').html(data.ErrorMessage.formatErrorMessage());
            }
            else {
                var timeout = setTimeout(function () {
                    clearTimeout(timeout);
                    Fanex.ExtAcc.Form.Close(false);

                    Fanex.ExtAcc.User.Search();
                }, 2000);
            }
        })
        .error(function (data) {
            if (typeof console != 'undefined') console.log(data.responseText);

            Fanex.ExtAcc.UnBlockUI();
        });
    },

    AddExist: function () {
        if (!Validators.validateInput($('#Username').val())) {
            $('#Username').focus();
            Validators.showDown($('#Username').get(0), 'This field is required', 0, 10, 41, -10, 15);
            return false;
        }

        Fanex.ExtAcc.BlockUI();

        var name = $('#Username').val();
        var groupId = $('#GroupID').val();

        var editUrl = 'User/AddExist/?';
        var params = new Object();

        params.username = Fanex.ExtAcc.EscapeHtml(name);
        params.groupId = groupId;

        $.ajax({ url: resolveClientUrl(editUrl), type: 'POST', data: params })
        .done(function (data) {
            if (!data.IsSuccess) {
                Fanex.ExtAcc.UnBlockUI();

                $('#errMsg').css('display', 'block');
                $('#errMsg').html(data.ErrorMessage.formatErrorMessage());
            }
            else {
                Fanex.ExtAcc.Form.Close(false);

                var timeout = setTimeout(function () {
                    clearTimeout(timeout);
                    Fanex.ExtAcc.Form.Close(false);

                    Fanex.ExtAcc.User.Search();
                }, 2000);
            }
        })
        .error(function (data) {
            if (typeof console != 'undefined') console.log(data.responseText);

            Fanex.ExtAcc.UnBlockUI();
        });
    },

    Edit: function () {
        Fanex.ExtAcc.BlockUI();

        var firstName = $('#FirstName').val();
        var lastName = $('#LastName').val();
        var groupId = $('#GroupID').val();

        var editUrl = 'User/Edit';
        var params = new Object();
        params.custId = $('#UserID').val();
        params.firstName = Fanex.ExtAcc.EscapeHtml(firstName);
        params.lastName = Fanex.ExtAcc.EscapeHtml(lastName);
        params.groupId = groupId;

        $.ajax({ url: resolveClientUrl(editUrl), data: params, type: 'POST' })
        .done(function (data) {
            if (!data.IsSuccess) {
                Fanex.ExtAcc.UnBlockUI();

                $('#errMsg').css('display', 'block');
                $('#errMsg').html(data.ErrorMessage.formatErrorMessage());
            }
            else {
                Fanex.ExtAcc.Form.Close(false);

                var timeout = setTimeout(function () {
                    clearTimeout(timeout);
                    Fanex.ExtAcc.Form.Close(false);

                    Fanex.ExtAcc.User.Search();
                }, 2000);
            }
        })
        .error(function (data) {
            if (typeof console != 'undefined') console.log(data.responseText);

            Fanex.ExtAcc.UnBlockUI();
        });
    },

    Activate: function (userId, isActive) {
        Fanex.ExtAcc.BlockUI();

        $.ajax({ url: resolveClientUrl('User/UpdateStatus/?userId=' + userId + '&isActive=' + isActive), type: 'POST' })
        .done(function (data) {
            if (!data.IsSuccess) {
                Fanex.ExtAcc.UnBlockUI();

                alert(data.ErrorMessage);
            }
            else {
                Fanex.ExtAcc.Form.Close(false);

                var timeout = setTimeout(function () {
                    clearTimeout(timeout);
                    Fanex.ExtAcc.Form.Close(false);

                    Fanex.ExtAcc.User.Search();
                }, 2000);
            }
        })
        .error(function (data) {
            if (typeof console != 'undefined') console.log(data.responseText);
        });
    },

    ResetPwd: function (isOwner) {
        if (!Validators.validateInput($('#Password').val())) {
            $('#Password').focus();
            Validators.showDown($('#Password').get(0), 'This field is required', 0, 10, 41, -10, 15);
            return false;
        }

        if (!Validators.validateInput($('#confirmPwd').val())) {
            $('#confirmPwd').focus();
            Validators.show($('#confirmPwd').get(0), 'This field is required');
            return false;
        }

        if ($('#confirmPwd').val() !== $('#Password').val()) {
            $('#confirmPwd').focus();
            Validators.show($('#confirmPwd').get(0), 'Confirm password is not matched');
            return false;
        }

        var params = new Object();
        params.userId = $('#UserID').val();
        params.pwd = $('#Password').val();

        $.ajax({ url: resolveClientUrl('User/' + (isOwner ? 'ResetPwdOwner' : 'ResetPwd')), data: params, type: 'POST' })
        .done(function (data) {
            if (!data.IsSuccess) {
                console.log($('#errMsg'));
                $('table tr td div#errMsg').css('display', 'block');
                $('table tr td div#errMsg').html(data.ErrorMessage.formatErrorMessage());

            }
            else {
                Fanex.ExtAcc.Form.Close(false);
            }
        })
        .error(function (data) {
            if (typeof console != 'undefined') console.log(data.responseText);
        });
    },

    Search: function (IsFirstLoad) {
        Fanex.ExtAcc.BlockUI();

        var username = '';
        var groupId = null;

        if (Fanex.ExtAcc.User.URL != '/User/UserListPost' && Fanex.ExtAcc.User.URL != '') {
            var tmp = Fanex.ExtAcc.User.URL.split('?');

            if (tmp != null) {
                tmp = tmp[1];
                if (tmp != null) {
                    tmp = tmp.split('&');
                    username = tmp[0].split('=')[1];
                    groupId = tmp[1].split('=')[1];
                }
            }

            if (username != $('#Usernames').val()) {
                username = $('#Usernames').val();
            }

            if (!IsFirstLoad && groupId != $('#GroupIDs').val()) {
                groupId = $('#GroupIDs').val() == '-1' ? null : $('#GroupIDs').val();
            }
            else {
                var select = document.getElementById("GroupIDs");

                $('#GroupIDs').val(groupId);
                $('#userGroup input').val(select.options[select.selectedIndex].text);
            }
        }
        else {
            username = $('#Usernames').val();
            groupId = $('#GroupIDs').val() == '-1' ? null : $('#GroupIDs').val();
        }

        var params = { username: username.encodeHtml(), groupId: groupId };

        Fanex.ExtAcc.User.URL = resolveClientUrl("User/UserListPost");

        if (this.dataTexts) {
            this.RenderGrid(params);
        }
    }
};