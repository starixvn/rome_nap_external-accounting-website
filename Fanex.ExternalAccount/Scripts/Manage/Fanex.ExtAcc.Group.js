/// <reference path="../_references.js" />
if (typeof Fanex == 'undefined' || typeof Fanex.ExtAcc == 'undefined') {
    alert("Fanex.ExtAcc.js module is required!")
}

Fanex.ExtAcc.Group = {
    DataFields: null,

    DataSource: null,
    GroupID: 0,

    URL: '',

    $this: null,

    Init: function () {
        $.blockUI({ message: '<img src="../../Images/Loading.gif" />' });
        $('#mainGrid').css('visibility', 'hidden');
        $this = this;

        var titles = $.parseJSON(menuTitles);
        $this.URL = resolveClientUrl("Group/GroupListPost");
        $this.DataFields = [
                    { HeaderText: 'Group Name', Name: 'GroupName', Width: 195,
                        CustomCellRender: function (cell) {
                            var func = "javascript:Fanex.ExtAcc.Group.ShowEdit(" + cell.Value.GroupID + ", '" + titles.EditGroup + "')";

                            return '<div class="editInfo"><a href="' + func + '">' + cell.Value.GroupName + '</a></div>';
                        }
                    },
                    { HeaderText: 'No. of Users', Name: 'NoUsers', Width: 100, CellAlign: 'center',
                        CustomCellRender: function (cell) {
                            var groupId = cell.Value.GroupID;
                            var onclick = "javascript:Fanex.ExtAcc.Group.SearchUser(" + groupId + ")";

                            return '<div style="text-align: center"><a href="' + onclick + '">' + cell.Value.NoUsers + '</a></div>';
                        }
                    },
                    { HeaderText: 'Description', Name: 'Description', Width: 220 },
                    { HeaderText: 'Creator', Name: 'Creator', Width: 120, CellAlign: 'center' },
                    { HeaderText: 'Created Date', Name: 'CreatedDate', Width: 160, CellAlign: 'center' },
                    { HeaderText: 'Permission', Name: 'Permission', Width: 75, CellAlign: 'center',
                        CustomCellRender: function (cell) {
                            var groupId = cell.Value.GroupID;
                            var groupName = cell.Value.GroupName;
                            var onclick = "javascript:Fanex.ExtAcc.Group.ShowPermission(" + groupId + ", '" + groupName + "')";

                            return '<div id="' + groupId + '" class="edit" onclick="' + onclick + '"></div>';
                        }
                    },
                    { HeaderText: 'Delete', Name: 'Delete', Width: 50, CellAlign: 'center',
                        CustomCellRender: function (cell) {
                            var style = "menuDelete";
                            var groupId = cell.Value.GroupID;
                            var groupName = cell.Value.GroupName;
                            var onclick = "javascript:Fanex.ExtAcc.Group.ShowDelete(" + groupId + ", '" + groupName + "')";

                            return '<div id="' + groupId + '" class="' + style + '" onclick="' + onclick + '"></div>';
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
                gridExpandHeight: 100,
                marginRight: 2,
                showDistinctRow: false,
                allowTruncate: true
            };

            $this.RenderGrid(options);
        })
        .error(function (xhr) {
            $(document).ajaxStop($.unblockUI);
        });

        $('#jqbutton').bind('click', function () {
            Fanex.ExtAcc.Form.Show(resolveClientUrl('Group/AddGroup'), 'Add New Group', 400, 330, false);
        });

        Fanex.ExtAcc.OnResize = function () {
            $this.leftMenuWidth = Fanex.ExtAcc.leftMenuWidth;
            try {
                $("#jqgrid").nexcelGrid("resizeGrid");
            }
            catch (err) {
            }
        }
    },

    RenderGrid: function (options) {
        try {
            // Init Grid jqgrid
            $('#jqgrid').nexcelGrid(options);
            $(document).ajaxStop($.unblockUI);

            $('#mainGrid').css('visibility', 'visible');
        }
        catch (e) {
            if (typeof console != 'undefined') {
                console.log(e);
            }

            $(document).ajaxStop($.unblockUI);
        }
    },

    ShowEdit: function (groupId, title) {
        Fanex.ExtAcc.Group.GroupId = groupId;
        Fanex.ExtAcc.Form.Show(
            resolveClientUrl('Group/EditGroup/' + groupId),
            title,
            400,
            330,
            false);
    },

    ShowPermission: function (groupId, groupName) {
        Fanex.ExtAcc.Group.GroupId = groupId;
        Fanex.ExtAcc.Form.Show(
            resolveClientUrl('Group/GrantPermission/?groupId=' + groupId + '&groupName=' + groupName),
            'Grant Permission',
            400,
            525,
            false,
            null,
            3
            );
    },

    ShowDelete: function (groupId, groupName) {
        Fanex.ExtAcc.Group.GroupId = groupId;

        var titles = $.parseJSON(menuTitles);
        var title = titles.Delete;

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

        var name = $('#GroupName').val();
        var desc = $('#Description').val();

        var editUrl = 'Group/Add/?';
        var params = '';

        params += '&name=' + name;
        params += '&description=' + desc;

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
        if (!Validators.validateInput($('#GroupName').val())) {
            $('#GroupName').focus();
            Validators.showDown($('#GroupName').get(0), 'This field is required', 0, 10, 41, -10, 15);
            return false;
        }

        var name = $('#GroupName').val();
        var desc = $('#Description').val();

        var editUrl = 'Group/Edit/?';
        var params = 'groupId=' + $('#GroupID').val();

        params += '&groupName=' + name;
        params += '&description=' + desc;

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

    Activate: function (groupId, isActive) {
        $.ajax({ url: resolveClientUrl('Group/UpdateStatus/?groupId=' + groupId + '&isActive=' + isActive), type: 'POST' })
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

    Delete: function (groupId) {
        $.ajax({ url: resolveClientUrl('Group/Delete/?groupId=' + groupId), type: 'POST' })
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

    SearchUser: function (groupId) {
        location.href = resolveClientUrl('User/UserList/?username=&groupId=' + groupId + '&/' + $('#selectedGroupName').html());
    }
}