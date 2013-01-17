/// <reference path="../_references.js" />
if (typeof Fanex == 'undefined' || typeof Fanex.ExtAcc == 'undefined') {
    alert("Fanex.ExtAcc.js module is required!")
}

Fanex.ExtAcc.Function = {
    Adapter: null,
    DataFields: null,
    DataSource: null,
    gridHeight: 200,
    FunctionId: null,
    leftMenuWidth: Fanex.ExtAcc.leftMenuWidth,
    URL: '',
    gHeight: 200,
    gSize: { width: 500, height: 300 },
    totalRows: 0,

    Init: function () {
        this.URL = resolveClientUrl("Function/FunctionListPost");

        var titles = $.parseJSON(menuTitles);
        var index = 0;

        Fanex.ExtAcc.Function.DataFields = [
            {
                name: 'FuncName', align: 'left', width: 250, sortable: false, resizable: false, title: false,
                formatter: function (value, model, row) {
                    var funcId = row.FuncID;
                    var isParent = row.ParentID == 0;
                    var hasChild = false;

                    if (Fanex.ExtAcc.Function.DataSource.rows[index + 1]) {
                        hasChild = Fanex.ExtAcc.Function.DataSource.rows[index + 1].ParentID != 0;
                    }

                    var func = "javascript:Fanex.ExtAcc.Function.ShowEdit(" + row.FuncID + ", '" + titles.EditFunction + "')";
                    var style = isParent && !hasChild ? "menuItemParentOnly" : (!isParent ? "menuItemChild" : "menuItemParent");
                    var div = '';

                    if (isParent && hasChild) {
                        var ids = [];
                        var length = Fanex.ExtAcc.Function.DataSource.rows.length;

                        for (var i = 0; i < length; i++) {
                            if (Fanex.ExtAcc.Function.DataSource.rows[i].ParentID == funcId) {
                                ids.push(i);
                            }
                        }

                        var onclickParent = 'Fanex.ExtAcc.TogglejqRows(' + funcId + ',[' + ids + '])';
                        div = '<div id="fn-' + funcId + '" onclick="' + onclickParent + '" style="float: left; cursor: pointer" class="itemParent">&nbsp;</div>';
                    }

                    index++;

                    return div + '<div class="' + style + '"><a href="' + func + '">' + value + '</a></div>';
                }
            },
            { name: 'FuncKey', align: 'left', width: 200, sortable: false, resizable: false },
            { name: 'FuncDesc', align: 'left', width: 250, sortable: false, resizable: false },
        //                  { name: 'ParentName', align: 'left', width: 150, sortable: false, resizable: false },
            {name: 'DisplayOrder', align: 'center', width: 50, sortable: false, resizable: false },
            { name: 'ModifiedDate', align: 'center', width: 150, sortable: false, resizable: false },
            { name: 'IsTest', align: 'center', width: 50, fixed: true, sortable: false, resizable: false,
                formatter: function (value, model, row) {
                    var style = row.IsTest == true ? "menuActive" : "menuInactive";
                    var funcId = row.FuncID;
                    var funcName = row.FuncName;
                    var onclick = "javascript:Fanex.ExtAcc.Function.ShowActivate(" + funcId + ", '" + funcName + "')";

                    return '<div id="test-' + funcId + '" class="' + style + '" onclick="' + onclick + '"></div>';
                }
            },
        ];

        Fanex.ExtAcc.Function.RenderGrid(Fanex.ExtAcc.Function.dataTexts);

        $this = this;

        $('#jqbutton').bind('click', function () {
            $this.AddNew();
        });

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
        $("#jqgrid").jqGrid({
            autowidth: true,
            url: Fanex.ExtAcc.Function.URL,
            datatype: "json",
            colNames: columns,
            colModel: Fanex.ExtAcc.Function.DataFields,
            height: '100%',
            jsonReader: { repeatitems: false },
            hoverrows: true,
            beforeSelectRow: function (rowid, e) {
                return false;
            },
            beforeProcessing: function (data, status, xhr) {
                Fanex.ExtAcc.Function.DataSource = data;
            },
            loadComplete: function () {
                $("tr.jqgrow").css("background", "#ffffff");
            }
        });
    },

    ShowEdit: function (funcId, title) {
        Fanex.ExtAcc.Function.FunctionId = funcId;
        Fanex.ExtAcc.Form.Show(
            resolveClientUrl('Function/EditFunction/' + funcId),
            title,
            410,
            460,
            false,
            function () {
                var url = resolveClientUrl("Function/PolicyList/?funcId=" + $('#FuncID').val());

                $.ajax(url).success(function (result) {
                    var groups = '';

                    result.forEach(function (data) {
                        groups += '<option value="' + data.GroupID + '"' + (data.HasPermission ? ' selected="selected">' : '>') + data.GroupName + '</option>';
                    });

                    $('#groups').html(groups);

                    $('#groups').css('display', 'block');
                    $("#groups").multiSelect({
                        minWidth: 260,
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
                        noneSelectedText: function () {
                            $("#groups").attr("title", "");
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

                    $('#groups input').css('width', '240px');
                });
            });
    },

    ShowActivate: function (funcId, funcName) {
        var titles = $.parseJSON(menuTitles);
        var title = '';

        title = $('#test-' + funcId).attr('class') == 'menuActive' ? titles.Deactivate : titles.Activate;

        Fanex.ExtAcc.Function.FunctionId = funcId;
        Fanex.ExtAcc.Form.ConfirmBox(
            title,
            'Are you sure to ' + title.toLowerCase() + ' [Is Test] of function <font color="#4b91be">' + funcName + '</font> ?',
            400,
            150,
            false,
            function () {
                Fanex.ExtAcc.Function.Activate(funcId, !($('#test-' + funcId).attr('class') == 'menuActive'));
            });
    },

    ShowDelete: function (funcId, funcName) {
        Fanex.ExtAcc.Function.FunctionId = funcId;

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
        Fanex.ExtAcc.Form.Show(resolveClientUrl("Function/AddFunction"), $.parseJSON(menuTitles).CreateFunction, 400, 460, false, function () {
            var url = resolveClientUrl("Function/PolicyList/?funcId=1&addNew=true");
            var itemsChecked = '';

            $.ajax(url).success(function (result) {
                var groups = '';

                result.forEach(function (data) {
                    groups += '<option value="' + data.GroupID + '"' + (data.HasPermission ? ' selected="selected">' : '>') + data.GroupName + '</option>';
                });

                $('#groupsAdd').html(groups);

                $(".groupCboMenu input").val($("#ParentID")[0].options[0].text);

                $('#groupsAdd').css('display', 'block');
                $("#groupsAdd").multiSelect({
                    minWidth: 260,
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

                $('#groupsAdd input').css('width', '240px');
            });
        });
    },

    Add: function () {
        if (!Validators.validateInput($('#FuncName').val())) {
            $('#FuncName').focus();
            Validators.showDown($('#FuncName').get(0), 'This field is required', 0, 10, 41, -10, 15);
            return false;
        }

        if (!Validators.validateInput($('#FuncKey').val())) {
            $('#FuncKey').focus();
            Validators.show($('#FuncKey').get(0), 'This field is required');
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

        var name = $('#FuncName').val();
        var desc = $('#FuncDesc').val();
        var parentId = $('#ParentID').val() != '-1' ? $('#ParentID').val() : '0';
        var parent = $('#ParentID option:selected').val() != '-1' ? $('#ParentID option:selected').text() : '';
        var order = $('#DisplayOrder').val();
        var newGroups = $('#newGroups').val();
        var key = $('#FuncKey').val();

        var editUrl = 'Function/Add/?';
        var params = '';

        params += '&name=' + name;
        params += '&key=' + key;
        params += '&desc=' + desc;
        params += '&parentId=' + parentId;
        params += '&parent=' + parent;
        params += '&order=' + order;
        params += '&groups=' + newGroups;

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
        if (!Validators.validateInput($('#FuncName').val())) {
            $('#FuncName').focus();
            Validators.showDown($('#FuncName').get(0), 'This field is required', 0, 10, 41, -10, 15);
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

        var editUrl = 'Function/Edit/?';
        var params = 'funcId=' + funcId;

        params += '&name=' + name;
        params += '&desc=' + desc;
        params += '&parentId=' + parentId;
        params += '&parent=' + parent;
        params += '&order=' + order;
        params += '&oldGroups=' + oldGroups;
        params += '&groups=' + newGroups;

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

    Activate: function (funcId, isActive) {
        $.ajax({ url: resolveClientUrl('Function/UpdateStatus/?funcId=' + funcId + '&isTest=' + isActive), type: 'POST' })
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

    Delete: function (funcId) {
        $.ajax({ url: resolveClientUrl('Function/Delete/?funcId=' + funcId), type: 'POST' })
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
    }
}