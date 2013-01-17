/// <reference path="../_references.js" />
if (typeof Fanex == 'undefined' || typeof Fanex.ExtAcc == 'undefined') {
    alert("Fanex.ExtAcc.js module is required!")
}

Fanex.ExtAcc.Customer = {
    DataFields: null,
    DataSource: null,
    gridHeight: 200,
    UserId: null,
    superId: null,
    masterId: null,
    leftMenuWidth: Fanex.ExtAcc.leftMenuWidth,
    URL: '',
    gHeight: 200,
    gSize: { width: 500, height: 300 },
    totalRows: 0,
    $this: null,
    fixedFrozen: 0,
    fixedFrozenFullHeight: 17,
    totalRowsInPage: 50,

    Init: function () {
        $this = this;

        //Prevent loading group name
        var timeout = setInterval(function () {
            if ($.trim($('#selectedGroupName').html()) != '') {
                $this.Search(true);
                clearInterval(timeout);
            }
        }, 100);

        $('#txtUserName').keypress(function (e) {
            if (typeof e == 'undefined') e = window.event;
            var key = (typeof e.keyCode == 'undefined') ? e.which : e.keyCode;
            if (key == 13) {
                $this.Search(false);

                return true;
            } else {
                return true;
            }
        });

        $('#txtUserName').keyup(function (e) {
            if ($('#txtUserName').val().length > 1) {
                Fanex.ExtAcc.InitSuggestion();
            }
        });

        Fanex.ExtAcc.OnResize = function () {
            $this.leftMenuWidth = Fanex.ExtAcc.leftMenuWidth;
            try {
                $this.Resize(true);
                $("#jqgrid").nexcelGrid("resizeGrid");
            }
            catch (err) {
            }
        }
    },

    CalculateHeight: function () {
        this.gHeight = Math.round($("#footer").position().top - $("#mainGrid").position().top) - 50;
        var mHeight = (this.totalRowsInPage) * 25 + 25;
        if (mHeight < this.gHeight) {
            this.gHeight = mHeight;
            this.fixedFrozen = this.totalRowsInPage * 2;
        }
        else {
            this.fixedFrozen = this.fixedFrozenFullHeight;
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
            var gHeight = 0;

            for (var i = 0; i < this.totalRows; i++) {
                gHeight += 22;
            }

            $("#jqgrid").setGridWidth(width, false);
            if (gHeight >= height) {
                $("#jqgrid").setGridHeight(height);
                var moreFooterWidth = this.fixedFrozen != this.fixedFrozenFullHeight ? 0 : 17;
                $(".ui-jqgrid-sdiv").css('width', $("#gview_jqgrid").width() - moreFooterWidth + 'px');
                $($("#gview_jqgrid").children()[1]).css('width', $("#gview_jqgrid").width() - moreFooterWidth + 'px');
            }
            else {
                $("#jqgrid").setGridHeight('100%');
                $(".ui-jqgrid-sdiv").css('width', $("#gview_jqgrid").width() + 'px');
                $($("#gview_jqgrid").children()[1]).css('width', $("#gview_jqgrid").width() + 'px');
            }

            this.gSize = { width: width, height: height };

            $('#jqgrid_frozen').css('margin-bottom', '2px');
        }
        catch (e) {
            if (typeof console != 'undefined') {
                console.log(e);
            }
        }
    },

    Search: function (isFirstLoad) {
        if (isFirstLoad) {
            $.blockUI({ message: '<img src="../../Images/Loading.gif" />' });

            this.GetSuperList();
        }
        else {
            this.SearchList();
        }
    },

    GetSuperList: function () {
        $.blockUI({ message: '<img src="../../Images/Loading.gif" />' });

        var url = resolveClientUrl('Customer/SuperListPost');
        var data = {
            groupName: $('#selectedGroupName').html().trim() == '' ? 'ACCDDD' : $('#selectedGroupName').html(),
            enabled: $('#customerStatus').val() == '1',
            closed: $('#customerStatus').val() == '2',
            suspended: $('#customerStatus').val() == '3'
        };

        $this.DataFields = [
                    { HeaderText: 'No', Name: 'RowNumber', Width: 30, CellAlign: 'center' },
                    { HeaderText: 'kjki', Name: 'Check', Width: 100, CellAlign: 'center'
                        //                                            ,
                        //                        CustomHeaderRender: function (col) {
                        //                            return '<div style="text-align: right;">' + "Demo" + "</div>";
                        //                        }
                        ,
                        CustomCellRenderer: function (value, model, row) {
                            return '<div onclick="Fanex.ExtAcc.Customer.Select(this)" style="margin-left: 8px !important" id="' + row.custid + '" class="menuInactive"></div>';
                        }
                    }
                            ,
                            { HeaderText: 'Username', Name: 'username', Width: 120,
                                CustomCellRenderer: function (value, model, row) {
                                    var custId = row.custid;

                                    var func = "javascript:Fanex.ExtAcc.Customer.GetMasterList(" + custId + ")";

                                    return '<div><a href="' + func + '">' + value + '</a></div>';
                                }
                            },
                            { HeaderText: 'Edit', Name: 'Edit', Width: 40,
                                CustomCellRenderer: function (value, model, row) {
                                    return '<div onclick="" id="edit-' + row.custid + '" class="editInfo"></div>';
                                }
                            },
                            { HeaderText: 'Status', Name: 'Status', Width: 60, CellAlign: 'center',
                                CustomCellRenderer: function (value, model, row) {
                                    return row.closed ? 'Closed' : (row.suspended ? 'Suspended' : (!row.enabled ? 'Disabled' : 'Open'));
                                }
                            },
                            { HeaderText: 'Double Comm', Name: 'DoubleCommLosing', Width: 150,
                                CustomCellRender12: function (cell) {
                                    if (cell.Value) {
                                        return '<div id="dbl-' + row.custid + '" class="allowDblComm"></div>';
                                    }

                                    return '<div id="dbl-' + row.custid + '" class="disallowDblComm"></div>';
                                }
                            },
                            { HeaderText: 'First Name', Name: 'firstname', Width: 120 },
                            { HeaderText: 'Last Name', Name: 'lastname', Width: 120 },
                            { HeaderText: 'Group A', Name: 'groupa', Width: 100, CellAlign: 'center' },
                            { HeaderText: 'Group B', Name: 'groupb', Width: 100, CellAlign: 'center' },
                            { HeaderText: 'Group C', Name: 'groupc', Width: 100, CellAlign: 'center' },
                            { HeaderText: 'Group D', Name: 'groupd', Width: 100, CellAlign: 'center' },
                            { HeaderText: '1X2', Name: 'discount1x2', Width: 100, CellAlign: 'center' },
                            { HeaderText: 'Others', Name: 'discountcs', Width: 200, CellAlign: 'center' },
                            { HeaderText: 'Game', Name: 'discountnumber', Width: 200, CellAlign: 'center' },
                            { HeaderText: 'Created Date', Name: 'createdate', CellAlign: 'center', Width: 200,
                                CustomCellRender: function (cell) {
                                    if (cell.Value) {
                                        return cell.Value.replace('T', ' ');
                                    }

                                    return new Date().toDateString();
                                }
                            },
                            { HeaderText: 'Login IP', Name: 'lastloginip', CellAlign: 'center', Width: 200,
                                CustomCellRender: function (cell) {
                                    return '<div><a href="#">' + cell.Value + '</a></div>';
                                }
                            }
                ];

        $.ajax({
            url: url,
            data: data,
            dataType: 'json'
        })
        .success(function (result) {
            var rows = $.parseJSON(result.rows);
            var options = null;
            var bodyRows = [];

            for (var i = 0; i < rows.length; i++) {
                bodyRows.push({
                    Columns: [
                        { Name: 'RowNumber', Value: rows[i].RowNumber },
                        { Name: 'Check', Value: 'aaa' },
                        { Name: 'username', Value: rows[i].username },
                        { Name: 'Edit', Value: '' },
                        { Name: 'Status', Value: '' },
                        { Name: 'DoubleCommLosing', Value: '' },
                        { Name: 'firstname', Value: rows[i].firstname },
                        { Name: 'lastname', Value: rows[i].lastname },
                        { Name: 'groupa', Value: rows[i].groupa },
                        { Name: 'groupb', Value: rows[i].groupb },
                        { Name: 'groupc', Value: rows[i].groupc },
                        { Name: 'groupd', Value: rows[i].groupd },
                        { Name: 'discount1x2', Value: rows[i].discount1x2 },
                        { Name: 'discountcs', Value: rows[i].discountcs },
                        { Name: 'discountnumber', Value: rows[i].discountnumber },
                        { Name: 'createdate', Value: rows[i].createdate },
                        { Name: 'lastloginip', Value: rows[i].lastloginip }
                    ]
                });
            }

            var groupHeader =
            [{
                Columns: [
                    { HeaderText: 'No', Name: 'RowNumber', MergeRowNumber: 2 },
                    { HeaderText: 'aaa', Name: 'Check', MergeRowNumber: 2 },
                    { HeaderText: 'Username', Name: 'username', MergeRowNumber: 2 },
                    { HeaderText: 'Edit', Name: 'Edit', MergeRowNumber: 2 },
                    { HeaderText: 'Status', Name: 'Status', MergeRowNumber: 2 },
                    { HeaderText: 'Double Comm', Name: 'DoubleCommLosing', MergeRowNumber: 2 },
                    { HeaderText: 'First Name', Name: 'firstname', MergeRowNumber: 2 },
                    { HeaderText: 'Last Name', Name: 'lastname', MergeRowNumber: 2 },
                    { HeaderText: 'Commission', Name: 'groupa', MergeColNumber: 7 },
                    { HeaderText: 'CreateDate', Name: 'createdate', MergeRowNumber: 2 },
                    { HeaderText: 'Login IP', Name: 'lastloginip', MergeRowNumber: 2 }
                ]
            }
            ]

            options = {
                columns: Fanex.ExtAcc.Customer.DataFields,
                bodyRows: bodyRows,
                gridExpandHeight: 180,
                numberOfFrozenColumn: 6,
                customizeHeader: true,
                groupHeaderRows: groupHeader
            };

            Fanex.ExtAcc.Customer.Render(options);
            Fanex.ExtAcc.Customer.totalRows = rows.length;

            //$("#jqgrid").closest(".ui-jqgrid-bdiv").attr("style", $("#jqgrid").closest(".ui-jqgrid-bdiv").attr("style") + " overflow-x: scroll; ");

            $('#mainGrid').css('visibility', 'visible');

            $('#breadcrumbs').empty();
            //Fanex.ExtAcc.Customer.Resize(true);
        })
        .error(function (xhr, status, err) {
            $(document).ajaxStop($.unblockUI);
        });
    },

    GetMasterList: function (srecommend) {
        $.blockUI({ message: '<img src="../../Images/Loading.gif" />' });

        var url = resolveClientUrl('Customer/MasterListPost');
        var data = {
            srecommend: srecommend,
            groupName: $('#selectedGroupName').html().trim() == '' ? 'ACCDDD' : $('#selectedGroupName').html(),
            enabled: $('#customerStatus').val() == '1',
            closed: $('#customerStatus').val() == '2',
            suspended: $('#customerStatus').val() == '3'
        };

        $this.superId = srecommend;
        $this.DataFields = [
                { name: 'RowNumber', fixed: true, sortable: false, resizable: false, width: 30, align: 'center', frozen: true },
                { name: '', fixed: true, sortable: false, resizable: false, width: 30, align: 'center', frozen: true,
                    formatter: function (value, model, row) {
                        return '<div onclick="Fanex.ExtAcc.Customer.Select(this)" style="margin-left: 8px !important" id="' + row.custid + '" class="menuInactive"></div>';
                    }
                },
                { name: 'username', sortable: false, resizable: false, fixed: true, width: 120, frozen: true,
                    formatter: function (value, model, row) {
                        var custId = row.custid;

                        var func = "javascript:Fanex.ExtAcc.Customer.GetAgentList(" + custId + ")";

                        return '<div><a href="' + func + '">' + value + '</a></div>';
                    }
                },
                { name: '', fixed: true, sortable: false, resizable: false, width: 30, fixed: true, frozen: true,
                    formatter: function (value, model, row) {
                        return '<div onclick="" id="edit-' + row.custid + '" class="editInfo"></div>';
                    }
                },
                { name: '', sortable: false, resizable: false, fixed: true, width: 60, frozen: true, align: 'center',
                    formatter: function (value, model, row) {
                        return row.closed ? 'Closed' : (row.suspended ? 'Suspended' : (!row.enabled ? 'Disabled' : 'Open'));
                    }
                },
                { name: 'DoubleCommLosing', sortable: false, resizable: false, fixed: true, width: 100, frozen: true,
                    formatter: function (value, model, row) {
                        if (value) {
                            return '<div id="dbl-' + row.custid + '" class="allowDblComm"></div>';
                        }

                        return '<div id="dbl-' + row.custid + '" class="disallowDblComm"></div>';
                    }
                },
                { name: 'firstname', sortable: false, resizable: false, fixed: true, width: 120 },
                { name: 'lastname', sortable: false, resizable: false, fixed: true, width: 120 },
                { name: 'groupa', fixed: true, sortable: false, resizable: false, width: 60, align: 'center' },
                { name: 'groupb', fixed: true, sortable: false, resizable: false, width: 60, align: 'center' },
                { name: 'groupc', fixed: true, sortable: false, resizable: false, width: 60, align: 'center' },
                { name: 'groupd', fixed: true, sortable: false, resizable: false, width: 60, align: 'center' },
                { name: 'discount1x2', fixed: true, sortable: false, resizable: false, width: 60, align: 'center' },
                { name: 'discountcs', fixed: true, sortable: false, resizable: false, width: 60, align: 'center' },
                { name: 'discountnumber', fixed: true, sortable: false, resizable: false, width: 60, align: 'center' },
                { name: 'createdate', sortable: false, resizable: false, align: 'center', width: 140, fixed: true,
                    formatter: function (value) {
                        if (value) {
                            return value.replace('T', ' ');
                        }

                        return new Date().toDateString();
                    }
                },
                { name: 'lastloginip', sortable: false, resizable: false, align: 'center', width: 100, fixed: true,
                    formatter: function (value) {
                        return '<div><a href="#">' + value + '</a></div>';
                    }
                },
            ];

        $.ajax({
            url: url,
            data: data,
            dataType: 'json'
        })
        .success(function (result) {
            var rows = $.parseJSON(result.rows);

            Fanex.ExtAcc.Customer.Render();
            Fanex.ExtAcc.Customer.totalRows = rows.length;

            for (var i = 0; i < rows.length; i++) {
                $('#jqgrid').jqGrid('addRowData', i + 1, rows[i]);
            }

            $("#jqgrid").closest(".ui-jqgrid-bdiv").attr("style", $("#jqgrid").closest(".ui-jqgrid-bdiv").attr("style") + " overflow-x: scroll; ");

            $('#mainGrid').css('visibility', 'visible');
            $('#breadcrumbs').empty();
            $('#breadcrumbs').append('<a href="' + "javascript:Fanex.ExtAcc.Customer.GetSuperList()" + '">' + 'Super' + '</a>');

            Fanex.ExtAcc.Customer.Resize(true);
        })
        .error(function (xhr, status, err) {
            $(document).ajaxStop($.unblockUI);
        });
    },

    GetAgentList: function (mrecommend) {
        $.blockUI({ message: '<img src="../../Images/Loading.gif" />' });

        var url = resolveClientUrl('Customer/AgentListPost');
        var linkSuper = '<a href="' + "javascript:Fanex.ExtAcc.Customer.GetSuperList()" + '">' + 'Super' + '</a>';
        var linkMaster = '<a href="' + "javascript:Fanex.ExtAcc.Customer.GetMasterList(" + $this.superId + ')"' + '">' + 'Master' + '</a>';
        var data = {
            mrecommend: mrecommend,
            groupName: $('#selectedGroupName').html().trim() == '' ? 'ACCDDD' : $('#selectedGroupName').html(),
            enabled: $('#customerStatus').val() == '1',
            closed: $('#customerStatus').val() == '2',
            suspended: $('#customerStatus').val() == '3'
        };

        $this.masterId = mrecommend;
        $this.DataFields = [
                    { name: 'RowNumber', fixed: true, sortable: false, resizable: false, width: 30, align: 'center', frozen: true },
                    { name: '', fixed: true, sortable: false, resizable: false, width: 30, align: 'center', frozen: true,
                        formatter: function (value, model, row) {
                            return '<div onclick="Fanex.ExtAcc.Customer.Select(this)" style="margin-left: 8px !important" id="' + row.custid + '" class="menuInactive"></div>';
                        }
                    },
                    { name: 'username', sortable: false, resizable: false, fixed: true, width: 120, frozen: true,
                        formatter: function (value, model, row) {
                            var custId = row.custid;

                            var func = "javascript:Fanex.ExtAcc.Customer.GetMemberList(" + custId + ")";

                            return '<div><a href="' + func + '">' + value + '</a></div>';
                        }
                    },
                    { name: '', fixed: true, sortable: false, resizable: false, width: 30, fixed: true, frozen: true,
                        formatter: function (value, model, row) {
                            return '<div onclick="" id="edit-' + row.custid + '" class="editInfo"></div>';
                        }
                    },
                    { name: '', sortable: false, resizable: false, fixed: true, width: 60, frozen: true, align: 'center',
                        formatter: function (value, model, row) {
                            return row.closed ? 'Closed' : (row.suspended ? 'Suspended' : (!row.enabled ? 'Disabled' : 'Open'));
                        }
                    },
                    { name: 'DoubleCommLosing', sortable: false, resizable: false, fixed: true, width: 100, frozen: true,
                        formatter: function (value, model, row) {
                            if (value) {
                                return '<div id="dbl-' + row.custid + '" class="allowDblComm"></div>';
                            }

                            return '<div id="dbl-' + row.custid + '" class="disallowDblComm"></div>';
                        }
                    },
                    { name: 'firstname', sortable: false, resizable: false, fixed: true, width: 120 },
                    { name: 'lastname', sortable: false, resizable: false, fixed: true, width: 120 },
                    { name: 'groupa', fixed: true, sortable: false, resizable: false, width: 60, align: 'center' },
                    { name: 'groupb', fixed: true, sortable: false, resizable: false, width: 60, align: 'center' },
                    { name: 'groupc', fixed: true, sortable: false, resizable: false, width: 60, align: 'center' },
                    { name: 'groupd', fixed: true, sortable: false, resizable: false, width: 60, align: 'center' },
                    { name: 'discount1x2', fixed: true, sortable: false, resizable: false, width: 60, align: 'center' },
                    { name: 'discountcs', fixed: true, sortable: false, resizable: false, width: 60, align: 'center' },
                    { name: 'discountnumber', fixed: true, sortable: false, resizable: false, width: 60, align: 'center' },
                    { name: 'createdate', sortable: false, resizable: false, align: 'center', width: 140, fixed: true,
                        formatter: function (value) {
                            if (value) {
                                return value.replace('T', ' ');
                            }

                            return new Date().toDateString();
                        }
                    },
                    { name: 'lastloginip', sortable: false, resizable: false, align: 'center', width: 100, fixed: true,
                        formatter: function (value) {
                            return '<div><a href="#">' + value + '</a></div>';
                        }
                    },
                ];

        $.ajax({
            url: url,
            data: data,
            dataType: 'json'
        })
        .success(function (result) {
            var rows = $.parseJSON(result.rows);

            Fanex.ExtAcc.Customer.Render();
            Fanex.ExtAcc.Customer.totalRows = rows.length;

            for (var i = 0; i < rows.length; i++) {
                $('#jqgrid').jqGrid('addRowData', i + 1, rows[i]);
            }

            $("#jqgrid").closest(".ui-jqgrid-bdiv").attr("style", $("#jqgrid").closest(".ui-jqgrid-bdiv").attr("style") + " overflow-x: scroll; ");

            $('#mainGrid').css('visibility', 'visible');
            $('#breadcrumbs').empty();
            $('#breadcrumbs').append('<div style="float:left">' + linkMaster + '</div><div class="viewNext">&nbsp;</div><div style="float:left">' + linkSuper + '</div>');
            Fanex.ExtAcc.Customer.Resize(true);
        })
        .error(function (xhr, status, err) {
            $(document).ajaxStop($.unblockUI);
        });
    },

    GetMemberList: function (recommend) {
        $.blockUI({ message: '<img src="../../Images/Loading.gif" />' });

        var url = resolveClientUrl('Customer/MemberListPost');
        var data = {
            recommend: recommend,
            groupName: $('#selectedGroupName').html().trim() == '' ? 'ACCDDD' : $('#selectedGroupName').html(),
            enabled: $('#customerStatus').val() == '1',
            closed: $('#customerStatus').val() == '2',
            suspended: $('#customerStatus').val() == '3'
        };

        $this.DataFields = [
                    { name: 'RowNumber', fixed: true, sortable: false, resizable: false, width: 30, align: 'center', frozen: true },
                    { name: '', fixed: true, sortable: false, resizable: false, width: 30, align: 'center', frozen: true,
                        formatter: function (value, model, row) {
                            return '<div onclick="Fanex.ExtAcc.Customer.Select(this)" style="margin-left: 8px !important" id="' + row.custid + '" class="menuInactive"></div>';
                        }
                    },
                    { name: 'username', sortable: false, resizable: false, fixed: true, width: 120, frozen: true },
                    { name: '', fixed: true, sortable: false, resizable: false, width: 30, fixed: true, frozen: true,
                        formatter: function (value, model, row) {
                            return '<div onclick="" id="edit-' + row.custid + '" class="editInfo"></div>';
                        }
                    },
                    { name: '', sortable: false, resizable: false, fixed: true, width: 60, frozen: true, align: 'center',
                        formatter: function (value, model, row) {
                            return row.closed ? 'Closed' : (row.suspended ? 'Suspended' : (!row.enabled ? 'Disabled' : 'Open'));
                        }
                    },
                    { name: 'DoubleCommLosing', sortable: false, resizable: false, fixed: true, width: 100, frozen: true,
                        formatter: function (value, model, row) {
                            if (value) {
                                return '<div id="dbl-' + row.custid + '" class="onDblComm"></div>';
                            }

                            return '<div id="dbl-' + row.custid + '" class="offDblComm"></div>';
                        }
                    },
                    { name: 'nickname', sortable: false, resizable: false, fixed: true, width: 120 },
                    { name: 'firstname', sortable: false, resizable: false, fixed: true, width: 120 },
                    { name: 'lastname', sortable: false, resizable: false, fixed: true, width: 120 },
                    { name: 'ugroup', fixed: true, sortable: false, resizable: false, width: 120, align: 'center' },
                    { name: 'discount', fixed: true, sortable: false, resizable: false, width: 60, align: 'center' },
                    { name: 'discount1x2', fixed: true, sortable: false, resizable: false, width: 60, align: 'center' },
                    { name: 'discountcs', fixed: true, sortable: false, resizable: false, width: 60, align: 'center' },
                    { name: 'discountnumber', fixed: true, sortable: false, resizable: false, width: 60, align: 'center' },
                    { name: 'playerdiscount', fixed: true, sortable: false, resizable: false, width: 60, align: 'center' },
                    { name: 'playerdiscount1x2', fixed: true, sortable: false, resizable: false, width: 60, align: 'center' },
                    { name: 'playerdiscountcs', fixed: true, sortable: false, resizable: false, width: 60, align: 'center' },
                    { name: 'playerdiscountnumber', fixed: true, sortable: false, resizable: false, width: 60, align: 'center' },
                    { name: 'createdate', sortable: false, resizable: false, align: 'center', width: 140, fixed: true,
                        formatter: function (value) {
                            if (value) {
                                return value.replace('T', ' ');
                            }

                            return new Date().toDateString();
                        }
                    },
                    { name: 'lastloginip', sortable: false, resizable: false, align: 'center', width: 100, fixed: true,
                        formatter: function (value) {
                            return '<div><a href="#">' + value + '</a></div>';
                        }
                    },
                ];

        $.ajax({
            url: url,
            data: data,
            dataType: 'json'
        })
        .success(function (result) {
            var rows = $.parseJSON(result.rows);
            var linkSuper = '<a href="' + "javascript:Fanex.ExtAcc.Customer.GetSuperList()" + '">' + 'Super' + '</a>';
            var linkMaster = '<a href="' + "javascript:Fanex.ExtAcc.Customer.GetMasterList(" + $this.superId + ')"' + '">' + 'Master' + '</a>';
            var linkAgent = '<a href="' + "javascript:Fanex.ExtAcc.Customer.GetAgentList(" + $this.masterId + ')"' + '">' + 'Agent' + '</a>';

            Fanex.ExtAcc.Customer.RenderMember();
            Fanex.ExtAcc.Customer.totalRows = rows.length;

            for (var i = 0; i < rows.length; i++) {
                $('#jqgrid').jqGrid('addRowData', i + 1, rows[i]);
            }

            $("#jqgrid").closest(".ui-jqgrid-bdiv").attr("style", $("#jqgrid").closest(".ui-jqgrid-bdiv").attr("style") + " overflow-x: scroll; ");

            $('#mainGrid').css('visibility', 'visible');
            $('#breadcrumbs').empty();
            $('#breadcrumbs').append('<div style="float:left">' + linkAgent + '</div><div class="viewNext">&nbsp;</div><div style="float:left">' + linkMaster + '</div><div class="viewNext">&nbsp;</div><div style="float:left">' + linkSuper + '</div>');

            Fanex.ExtAcc.Customer.Resize(true);
        })
        .error(function (xhr, status, err) {
            $(document).ajaxStop($.unblockUI);
        });
    },

    ShowEditUserInfo: function () {
    },

    ShowResetPwd: function () {
    },

    ShowDoubleComm: function () {
    },

    ShowTransferCondition: function () {
    },

    ShowIpInfo: function () {
    },

    EditUserInfo: function () {
    },

    ResetPwd: function () {
    },

    ConfigDoubleComm: function () {
    },

    EditTransferCondition: function () {
    },

    Select: function (div) {
        if (div.className == 'menuInactive') {
            div.className = 'menuActive';
        }
        else {
            div.className = 'menuInactive'
        }
    },

    SelectAll: function (className) {
        var divs = document.getElementsByTagName('div');

        for (var i = 0; i < divs.length; i++) {
            if (divs[i].id && (divs[i].className == 'menuActive' || divs[i].className == 'menuInactive')) {
                divs[i].className = className;
            }
        }
    },

    Render: function (options) {
        try {
            // Init Grid jqgrid
            $('#gbox_jqgrid').remove();
            //$('#jqgrid').remove();
            $('#mainGrid').append($('<table id="jqgrid" class="removeRadius"></table>'));
        }
        catch (e) {
            if (typeof console != 'undefined') {
                console.log(e);
            }
        }

        $('#jqgrid').nexcelGrid(options);

        $(document).ajaxStop($.unblockUI);
    },

    RenderMember: function () {
        try {
            // Init Grid jqgrid
            $('#gbox_jqgrid').remove();
            $('#jqgrid').remove();
            $('#mainGrid').append($('<table id="jqgrid" class="removeRadius"></table>'));
        }
        catch (e) {
            if (typeof console != 'undefined') {
                console.log(e);
            }
        }

        $('#jqgrid').jqGrid({
            sortable: false,
            autowidth: true,
            height: '600px',
            datatype: 'local',
            colNames: ['No.', '<div class="editMult" style="cursor: pointer !important"></div>', 'Username', 'Edit', 'Status', 'Double Comm', 'Nickname', 'First Name', 'Last Name', 'User Group', 'Group', '1X2', 'Others', 'Game', 'Group', '1X2', 'Others', 'Game', 'Created Date', 'Login IP'],
            colModel: Fanex.ExtAcc.Customer.DataFields,
            rowNum: 500,
            beforeSelectRow: function (rowid, e) {
                return false;
            },
            gridComplete: function () {
                $("tr.jqgrow").css("background", "#ffffff");
                $(document).ajaxStop($.unblockUI);
            }
        });

        //$('#jqxgrid').clearGridData(true);

        $('#jqgrid').jqGrid('setGroupHeaders', {
            useColSpanStyle: true,
            groupHeaders: [
                    { startColumnName: 'discount', numberOfColumns: 4, titleText: 'Agent Commission' },
                    { startColumnName: 'playerdiscount', numberOfColumns: 4, titleText: 'Player Commission' }
                ]
        });

        $('#jqgrid').jqGrid('setFrozenColumns');
    },

    SearchList: function () {
    }
}