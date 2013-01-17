/// <reference path="../_references.js" />
if (typeof Fanex == 'undefined' || typeof Fanex.ExtAcc == 'undefined') {
    alert("Fanex.ExtAcc.js module is required!")
}

Fanex.ExtAcc.Group = {
    Adapter: null,
    DataFields: null,
    gridHeight: 200,
    DataSource: null,
    GroupID: 0,
    leftMenuWidth: Fanex.ExtAcc.leftMenuWidth,
    URL: '',
    gHeight: 200,
    gSize: { width: 500, height: 300 },
    totalRows: 0,

    Init: function () {
        var titles = $.parseJSON(menuTitles);
        this.URL = resolveClientUrl("Group/GroupListPost");
        this.DataFields = [
                    { name: 'GroupName', index: 'GroupName', width: 120, sortable: false, resizable: false,
                        formatter: function (value, model, row) {
                            var func = "javascript:Fanex.ExtAcc.Group.ShowEdit(" + row.GroupID + ", '" + titles.EditGroup + "')";

                            return '<div class="editInfo"><a href="' + func + '">' + value + '</a></div>';
                        }
                    },
                    { name: 'NoUsers', index: 'NoUsers', width: 80, align: 'center', sortable: false, resizable: false,
                        formatter: function (value, model, row) {
                            var groupId = row.GroupID;
                            var onclick = "javascript:Fanex.ExtAcc.Group.SearchUser(" + groupId + ")";

                            return '<div style="margin-top: 3px; text-align: center"><a href="' + onclick + '">' + value + '</a></div>';
                        }
                    },
                    { name: 'Description', index: 'Description', sortable: false, resizable: false, width: 150 },
                    { name: 'Creator', index: 'Creator', width: 80, sortable: false, resizable: false, align: 'center' },
                    { name: 'CreatedDate', index: 'CreatedDate', sortable: false, resizable: false, width: 150, align: 'center' },
                    { name: '', width: 75, align: 'center', sortable: false, resizable: false, fixed: true,
                        formatter: function (value, model, row) {
                            var groupId = row.GroupID;
                            var groupName = row.GroupName;
                            var onclick = "javascript:Fanex.ExtAcc.Group.ShowPermission(" + groupId + ", '" + groupName + "')";

                            return '<div id="' + groupId + '" class="edit" onclick="' + onclick + '"></div>';
                        }
                    },
                    { name: '', width: 50, align: 'center', sortable: false, resizable: false, fixed: true,
                        formatter: function (value, model, row) {
                            var style = "menuDelete";
                            var groupId = row.GroupID;
                            var groupName = row.GroupName;
                            var onclick = "javascript:Fanex.ExtAcc.Group.ShowDelete(" + groupId + ", '" + groupName + "')";

                            return '<div id="' + groupId + '" class="' + style + '" onclick="' + onclick + '"></div>';
                        }
                    }
                ];

        $('#jqbutton').bind('click', function () {
            Fanex.ExtAcc.Form.Show(resolveClientUrl('Group/AddGroup'), 'Add New Group', 400, 330, false);
        });

        $this = this;

        Fanex.ExtAcc.OnResize = function () {
            $this.leftMenuWidth = Fanex.ExtAcc.leftMenuWidth;
            $this.Resize(true)
        }

        this.RenderGrid(this.dataTexts);
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
                url: Fanex.ExtAcc.Group.URL,
                datatype: "json",
                colNames: columns,
                colModel: Fanex.ExtAcc.Group.DataFields,
                height: '100%',
                jsonReader: { repeatitems: false, id: 'GroupName' },
                hoverrows: true,
                beforeSelectRow: function (rowid, e) {
                    return false;
                },
                loadComplete: function () {
                    $("tr.jqgrow").css("background", "#ffffff");
                }
            });
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
        var index = '?index=0';

        if (location.href.indexOf('?index=') != -1) {
            index = '?index=' + location.href.split('?index=')[1];
        }

        location.href = resolveClientUrl('User/UserList/?username=&groupId=' + groupId + index);
    }
}