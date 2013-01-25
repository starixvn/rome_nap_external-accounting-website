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
        $('#mainGrid').css('visibility', 'hidden');
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
                    var onclick = "javascript:Fanex.ExtAcc.User.ShowResetPwd(" + userId + ")";

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

    RenderGrid: function () {
        try {
            $.ajax({
                url: Fanex.ExtAcc.User.URL,
                dataType: 'json'
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
                    showDistinctRow: false
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

                $(document).ajaxStop($.unblockUI);
            });
        }
        catch (e) {
            if (typeof console != 'undefined') {
                console.log(e);
            }

            $(document).ajaxStop($.unblockUI);
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

    ShowResetPwd: function (userId) {
        Fanex.ExtAcc.User.UserId = userId;

        var titles = $.parseJSON(menuTitles);
        var title = titles.ResetPwd;

        Fanex.ExtAcc.Form.Show(
            resolveClientUrl('User/ResetPassword/' + userId),
            title,
            420,
            230,
            false,
            null,
            20
            );
    },

    ShowPermission: function (userId, username, groupName) {
        Fanex.ExtAcc.User.UserId = userId;

        Fanex.ExtAcc.Form.Show(
            resolveClientUrl('User/GrantPermission/?userId=' + userId + '&username=' + username + '&groupName=' + groupName),
            'Grant Permission',
            520,
            525,
            false,
            null,
            3
            );
    },

    Add: function () {
        if (!Validators.validateInput($('#Username').val())) {
            $('#Username').focus();
            Validators.showDown($('#Username').get(0), 'This field is required', 0, 10, 41, -10, 15);
            return false;
        }

        if (!Validators.validateInput($('#Password').val())) {
            $('#Password').focus();
            Validators.show($('#Password').get(0), 'This field is required');
            return false;
        }

        var name = $('#Username').val();
        var pass = $('#Password').val();
        var firstName = $('#FirstName').val();
        var lastName = $('#LastName').val();
        var groupId = $('#GroupID').val();

        var editUrl = 'User/Add/?';
        var params = '';

        params += '&username=' + name;
        params += '&pwd=' + pass;
        params += '&firstName=' + firstName;
        params += '&lastName=' + lastName;
        params += '&groupId=' + groupId;

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

    AddExist: function () {
        if (!Validators.validateInput($('#Username').val())) {
            $('#Username').focus();
            Validators.showDown($('#Username').get(0), 'This field is required', 0, 10, 41, -10, 15);
            return false;
        }

        var name = $('#Username').val();
        var groupId = $('#GroupID').val();

        var editUrl = 'User/AddExist/?';
        var params = '';

        params += '&username=' + name;
        params += '&groupId=' + groupId;

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
        var firstName = $('#FirstName').val();
        var lastName = $('#LastName').val();
        var groupId = $('#GroupID').val();

        var editUrl = 'User/Edit/?';
        var params = 'custId=' + $('#UserID').val();

        params += '&firstName=' + firstName;
        params += '&lastName=' + lastName;
        params += '&groupId=' + groupId;

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

    Activate: function (userId, isActive) {
        $.ajax({ url: resolveClientUrl('User/UpdateStatus/?userId=' + userId + '&isActive=' + isActive), type: 'POST' })
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

    ResetPwd: function () {
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

        $.ajax({ url: resolveClientUrl('User/ResetPwd/?userId=' + $('#UserID').val() + '&pwd=' + $('#Password').val()), type: 'POST' })
        .done(function (data) {
            if (!data.IsSuccess) {
                $('#errMsg').css('display', 'block');
                $('#errMsg').html(data.ErrorMessage);
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
        $.blockUI({ message: '<img src="../../Images/Loading.gif" />' });
        var username = '';
        var groupId = null;

        if (location.href.indexOf('?username=') != -1) {
            var tmp = location.href.split('?')[1].split('&');
            username = tmp[0].split('=')[1];
            groupId = tmp[1].split('=')[1];

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

        Fanex.ExtAcc.User.URL = resolveClientUrl("User/UserListPost/?username=" + username + "&groupId=" + groupId);

        if (this.dataTexts) {
            this.RenderGrid();
        }
    }
}