/// <reference path="../_references.js" />
if (typeof Fanex == 'undefined' || typeof Fanex.ExtAcc == 'undefined') {
    alert("Fanex.ExtAcc.js module is required!")
}

Fanex.ExtAcc.User = {
    Adapter: null,
    DataFields: null,
    DataSource: null,
    gridHeight: 200,
    UserId: null,
    leftMenuWidth: Fanex.ExtAcc.leftMenuWidth,
    IsFirstLoad: true,
    URL: '',
    gHeight: 200,
    gSize: { width: 500, height: 300 },
    totalRows: 0,

    Init: function () {
        $("#GroupIDs").combobox({
        });

        $("#userGroup input").val($("#GroupIDs")[0].options[0].text);
        $("#userGroup input").attr("readonly", "true");
        $("#userGroup input").bind("click", function () {
            $(this).autocomplete("search", '');
        });

        var titles = $.parseJSON(menuTitles);

        this.DataFields = [
            {
                name: 'Username', align: 'left', width: 120, sortable: false, resizable: false,
                formatter: function (value, model, row) {
                    var userId = row.UserID;

                    var func = "javascript:Fanex.ExtAcc.User.ShowEdit(" + userId + ", '" + titles.EditUser + "')";

                    return '<div style="margin: 3px"><a href="' + func + '">' + value + '</a></div>';
                }
            },
            { name: 'FirstName', align: 'left', width: 120, sortable: false, resizable: false },
            { name: 'LastName', align: 'left', width: 120, sortable: false, resizable: false },
            { name: 'GroupName', align: 'left', width: 120, sortable: false, resizable: false },
            { name: 'Creator', align: 'left', width: 120, sortable: false, resizable: false },
            { name: 'CreatedDate', align: 'center', width: 150, sortable: false, resizable: false },
            { name: '', align: 'center', width: 75, fixed: true, sortable: false, resizable: false,
                formatter: function (value, model, row) {
                    var userId = row.UserID;
                    var username = row.Username;
                    var groupName = row.GroupName;
                    var onclick = "javascript:Fanex.ExtAcc.User.ShowPermission(" + userId + ",'" + username + "','" + groupName + "')";

                    return '<div class="edit" onclick="' + onclick + '"></div>';
                }
            },
            { name: '', align: 'center', cellsalign: 'center', width: 50, fixed: true, sortable: false, resizable: false,
                formatter: function (value, model, row) {
                    var style = row.Enabled == true ? "menuActive" : "menuInactive";
                    var userId = row.UserID;
                    var userName = row.Username;
                    var onclick = "javascript:Fanex.ExtAcc.User.ShowActivate(" + userId + ", '" + userName + "')";

                    return '<div id="' + userId + '" class="' + style + '" onclick="' + onclick + '"></div>';
                }
            },
            { name: '', align: 'center', cellsalign: 'center', width: 70, fixed: true, sortable: false, resizable: false,
                formatter: function (value, model, row) {
                    var style = "menuResetPwd";
                    var userId = row.UserID;
                    var onclick = "javascript:Fanex.ExtAcc.User.ShowResetPwd(" + userId + ")";

                    return '<div class="' + style + '" onclick="' + onclick + '"></div>';
                }
            }
        ];

        $('#Usernames').bind('keypress', function (e) {
            var code = e.keyCode ? e.keyCode : e.which;

            if (code == 13) {
                Fanex.ExtAcc.User.IsFirstLoad = false;
                Fanex.ExtAcc.User.Search();
            }
        });

        this.Search(this.IsFirstLoad);

        $('#jqbuttonAddUser').bind('click', function () {
            Fanex.ExtAcc.Form.Show(resolveClientUrl("User/AddUser"), $.parseJSON(menuTitles).CreateUser, 400, 300, false);
        });

        $('#jqbuttonAddExistUser').bind('click', function () {
            Fanex.ExtAcc.Form.Show(resolveClientUrl("User/AddExistUser"), $.parseJSON(menuTitles).CreateExistUser, 400, 200, false);
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
        if ($('#gbox_jqgrid').length == 0) {
            $('#jqgrid').jqGrid('GridUnload');
        }

        $("#jqgrid").jqGrid({
            autowidth: true,
            url: Fanex.ExtAcc.User.URL,
            datatype: "json",
            colNames: columns,
            colModel: Fanex.ExtAcc.User.DataFields,
            height: '100%',
            jsonReader: { repeatitems: false },
            hoverrows: true,
            beforeSelectRow: function (rowid, e) {
                return false;
            },
            loadComplete: function (data) {
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

                $("tr.jqgrow").css("background", "#ffffff");
            }
        });

        $("#jqgrid").jqGrid('setGridParam', { url: Fanex.ExtAcc.User.URL, datatype: "json", page: 1 }).trigger("reloadGrid");
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
            this.RenderGrid(this.dataTexts);
        }
    }
}