/// <reference path="../_references.js" />
if (typeof Fanex == 'undefined' || typeof Fanex.ExtAcc == 'undefined') {
    alert("Fanex.ExtAcc.js module is required!")
}

Fanex.ExtAcc.Customer = {
    DataFields: null,
    DataSource: null,

    UserId: null,
    superId: null,
    masterId: null,

    URL: '',

    $this: null,

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
                    { HeaderText: 'No.', Name: 'RowNumber', Width: 30, CellAlign: 'center' },
                    { HeaderText: '', Name: 'Check', Width: 30, CellAlign: 'center',
                        CustomHeaderRender: function (cell) {
                            return '<div class="editMult"></div>';
                        },
                        CustomCellRender: function (cell) {
                            return '<div onclick="Fanex.ExtAcc.Customer.Select(this)" style="margin-left: 0px !important" id="' + cell.Value.custid + '" class="menuInactive"></div>';
                        }
                    },
                    { HeaderText: 'Username', Name: 'username', Width: 120,
                        CustomCellRender: function (cell) {
                            var custId = cell.Value.custid;

                            var func = "javascript:Fanex.ExtAcc.Customer.GetMasterList(" + custId + ")";

                            return '<div><a href="' + func + '">' + cell.Value.username + '</a></div>';
                        }
                    },
                    { HeaderText: 'Edit', Name: 'Edit', Width: 30,
                        CustomCellRender: function (cell) {
                            return '<div onclick="" id="edit-' + cell.Value.custid + '" class="editInfo"></div>';
                        }
                    },
                    { HeaderText: 'Status', Name: 'Status', Width: 60, CellAlign: 'center',
                        CustomCellRender: function (cell) {
                            return cell.Value.closed ? 'Closed' : (cell.Value.suspended ? 'Suspended' : (!cell.Value.enabled ? 'Disabled' : 'Open'));
                        }
                    },
                    { HeaderText: 'Double Comm', Name: 'DoubleCommLosing', Width: 100,
                        CustomCellRender: function (cell) {
                            if (cell.Value.DoubleCommLosing) {
                                return '<div id="dbl-' + cell.Value.custid + '" class="allowDblComm"></div>';
                            }

                            return '<div id="dbl-' + cell.Value.custid + '" class="disallowDblComm"></div>';
                        }
                    },
                    { HeaderText: 'First Name', Name: 'firstname', Width: 120 },
                    { HeaderText: 'Last Name', Name: 'lastname', Width: 120 },
                    { HeaderText: 'Group A', Name: 'groupa', Width: 60, CellAlign: 'center' },
                    { HeaderText: 'Group B', Name: 'groupb', Width: 60, CellAlign: 'center' },
                    { HeaderText: 'Group C', Name: 'groupc', Width: 60, CellAlign: 'center' },
                    { HeaderText: 'Group D', Name: 'groupd', Width: 60, CellAlign: 'center' },
                    { HeaderText: '1X2', Name: 'discount1x2', Width: 60, CellAlign: 'center' },
                    { HeaderText: 'Others', Name: 'discountcs', Width: 60, CellAlign: 'center' },
                    { HeaderText: 'Game', Name: 'discountnumber', Width: 60, CellAlign: 'center' },
                    { HeaderText: 'Created Date', Name: 'createdate', CellAlign: 'center', Width: 140,
                        CustomCellRender: function (cell) {
                            if (cell.Value) {
                                return cell.Value.replace('T', ' ');
                            }

                            return new Date().toDateString();
                        }
                    },
                    { HeaderText: 'Login IP', Name: 'lastloginip', CellAlign: 'center', Width: 140,
                        CustomCellRender: function (cell) {
                            return '<div><a href="javascript:Fanex.ExtAcc.Customer.ShowIpInfo(\'' + cell.Value + '\')">' + cell.Value + '</a></div>';
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

            Fanex.ExtAcc.Customer.DataSource = rows;

            for (var i = 0; i < rows.length; i++) {
                bodyRows.push({
                    Columns: [
                        { Name: 'RowNumber', Value: rows[i].RowNumber },
                        { Name: 'Check', Value: rows[i] },
                        { Name: 'username', Value: rows[i] },
                        { Name: 'Edit', Value: rows[i] },
                        { Name: 'Status', Value: rows[i] },
                        { Name: 'DoubleCommLosing', Value: rows[i] },
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
                    { HeaderText: 'No.', Name: 'RowNumber', MergeRowNumber: 2 },
                    { HeaderText: '', Name: 'Check', MergeRowNumber: 2 },
                    { HeaderText: 'Username', Name: 'username', MergeRowNumber: 2 },
                    { HeaderText: 'Edit', Name: 'Edit', MergeRowNumber: 2 },
                    { HeaderText: 'Status', Name: 'Status', MergeRowNumber: 2 },
                    { HeaderText: 'Double Comm', Name: 'DoubleCommLosing', MergeRowNumber: 2 },
                    { HeaderText: 'First Name', Name: 'firstname', MergeRowNumber: 2 },
                    { HeaderText: 'Last Name', Name: 'lastname', MergeRowNumber: 2 },
                    { HeaderText: 'Commission', Name: 'groupa', MergeColNumber: 7 },
                    { HeaderText: 'Created Date', Name: 'createdate', MergeRowNumber: 2 },
                    { HeaderText: 'Login IP', Name: 'lastloginip', MergeRowNumber: 2 }
                ]
            }]

            options = {
                columns: Fanex.ExtAcc.Customer.DataFields,
                bodyRows: bodyRows,
                gridExpandHeight: 165,
                numberOfFrozenColumn: 6,
                customizeHeader: true,
                groupHeaderRows: groupHeader,
                highlightFrozenWidth: 2,
                showPager: true,
                customPager: false,
                pagerOption: {
                    itemsPerPageArray: [50, 100, 500],
                    currentPage: 0,
                    totalItem: rows.length,
                    itemsPerPage: 50
                },
                marginRight: 2,
                showDistinctRow: false
            };

            Fanex.ExtAcc.Customer.Render(options);
            Fanex.ExtAcc.Customer.totalRows = rows.length;

            $('#mainGrid').css('visibility', 'visible');

            $('#breadcrumbs').empty();

            Fanex.ExtAcc.OnResize = function () {
                $this.leftMenuWidth = Fanex.ExtAcc.leftMenuWidth;
                try {
                    $("#jqgrid").fanexGrid("resizeGrid");
                }
                catch (err) {
                }
            }
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
                    { HeaderText: 'No.', Name: 'RowNumber', Width: 30, CellAlign: 'center' },
                    { HeaderText: '', Name: 'Check', Width: 30, CellAlign: 'center',
                        CustomHeaderRender: function (cell) {
                            return '<div class="editMult"></div>';
                        },
                        CustomCellRender: function (cell) {
                            return '<div onclick="Fanex.ExtAcc.Customer.Select(this)" style="margin-left: 0px !important" id="' + cell.Value.custid + '" class="menuInactive"></div>';
                        }
                    },
                    { HeaderText: 'Username', Name: 'username', Width: 120,
                        CustomCellRender: function (cell) {
                            var custId = cell.Value.custid;

                            var func = "javascript:Fanex.ExtAcc.Customer.GetAgentList(" + custId + ")";

                            return '<div><a href="' + func + '">' + cell.Value.username + '</a></div>';
                        }
                    },
                    { HeaderText: 'Edit', Name: 'Edit', Width: 30,
                        CustomCellRender: function (cell) {
                            return '<div onclick="" id="edit-' + cell.Value.custid + '" class="editInfo"></div>';
                        }
                    },
                    { HeaderText: 'Status', Name: 'Status', Width: 60, CellAlign: 'center',
                        CustomCellRender: function (cell) {
                            return cell.Value.closed ? 'Closed' : (cell.Value.suspended ? 'Suspended' : (!cell.Value.enabled ? 'Disabled' : 'Open'));
                        }
                    },
                    { HeaderText: 'Double Comm', Name: 'DoubleCommLosing', Width: 100,
                        CustomCellRender: function (cell) {
                            if (cell.Value.DoubleCommLosing) {
                                return '<div id="dbl-' + cell.Value.custid + '" class="allowDblComm"></div>';
                            }

                            return '<div id="dbl-' + cell.Value.custid + '" class="disallowDblComm"></div>';
                        }
                    },
                    { HeaderText: 'First Name', Name: 'firstname', Width: 120 },
                    { HeaderText: 'Last Name', Name: 'lastname', Width: 120 },
                    { HeaderText: 'Group A', Name: 'groupa', Width: 60, CellAlign: 'center' },
                    { HeaderText: 'Group B', Name: 'groupb', Width: 60, CellAlign: 'center' },
                    { HeaderText: 'Group C', Name: 'groupc', Width: 60, CellAlign: 'center' },
                    { HeaderText: 'Group D', Name: 'groupd', Width: 60, CellAlign: 'center' },
                    { HeaderText: '1X2', Name: 'discount1x2', Width: 60, CellAlign: 'center' },
                    { HeaderText: 'Others', Name: 'discountcs', Width: 60, CellAlign: 'center' },
                    { HeaderText: 'Game', Name: 'discountnumber', Width: 60, CellAlign: 'center' },
                    { HeaderText: 'Created Date', Name: 'createdate', CellAlign: 'center', Width: 140,
                        CustomCellRender: function (cell) {
                            if (cell.Value) {
                                return cell.Value.replace('T', ' ');
                            }

                            return new Date().toDateString();
                        }
                    },
                    { HeaderText: 'Login IP', Name: 'lastloginip', CellAlign: 'center', Width: 140,
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

            Fanex.ExtAcc.Customer.DataSource = rows;

            for (var i = 0; i < rows.length; i++) {
                bodyRows.push({
                    Columns: [
                        { Name: 'RowNumber', Value: rows[i].RowNumber },
                        { Name: 'Check', Value: rows[i] },
                        { Name: 'username', Value: rows[i] },
                        { Name: 'Edit', Value: rows[i] },
                        { Name: 'Status', Value: rows[i] },
                        { Name: 'DoubleCommLosing', Value: rows[i] },
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
                    { HeaderText: 'No.', Name: 'RowNumber', MergeRowNumber: 2 },
                    { HeaderText: '', Name: 'Check', MergeRowNumber: 2 },
                    { HeaderText: 'Username', Name: 'username', MergeRowNumber: 2 },
                    { HeaderText: 'Edit', Name: 'Edit', MergeRowNumber: 2 },
                    { HeaderText: 'Status', Name: 'Status', MergeRowNumber: 2 },
                    { HeaderText: 'Double Comm', Name: 'DoubleCommLosing', MergeRowNumber: 2 },
                    { HeaderText: 'First Name', Name: 'firstname', MergeRowNumber: 2 },
                    { HeaderText: 'Last Name', Name: 'lastname', MergeRowNumber: 2 },
                    { HeaderText: 'Commission', Name: 'groupa', MergeColNumber: 7 },
                    { HeaderText: 'Created Date', Name: 'createdate', MergeRowNumber: 2 },
                    { HeaderText: 'Login IP', Name: 'lastloginip', MergeRowNumber: 2 }
                ]
            }
            ]

            options = {
                columns: Fanex.ExtAcc.Customer.DataFields,
                bodyRows: bodyRows,
                gridExpandHeight: 165,
                numberOfFrozenColumn: 6,
                customizeHeader: true,
                groupHeaderRows: groupHeader,
                highlightFrozenWidth: 2,
                showPager: true,
                customPager: false,
                pagerOption: {
                    itemsPerPageArray: [50, 100, 500],
                    currentPage: 0,
                    totalItem: rows.length,
                    itemsPerPage: 50
                },
                marginRight: 2,
                showDistinctRow: false
            };

            Fanex.ExtAcc.Customer.Render(options);
            Fanex.ExtAcc.Customer.totalRows = rows.length;

            $('#mainGrid').css('visibility', 'visible');

            $('#breadcrumbs').empty();

            $('#breadcrumbs').append('<a href="' + "javascript:Fanex.ExtAcc.Customer.GetSuperList()" + '">' + 'Super' + '</a>');

            Fanex.ExtAcc.OnResize = function () {
                $this.leftMenuWidth = Fanex.ExtAcc.leftMenuWidth;
                try {
                    $("#jqgrid").fanexGrid("resizeGrid");
                }
                catch (err) {
                }
            }
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
                    { HeaderText: 'No.', Name: 'RowNumber', Width: 30, CellAlign: 'center' },
                    { HeaderText: '', Name: 'Check', Width: 30, CellAlign: 'center',
                        CustomHeaderRender: function (cell) {
                            return '<div class="editMult"></div>';
                        },
                        CustomCellRender: function (cell) {
                            return '<div onclick="Fanex.ExtAcc.Customer.Select(this)" style="margin-left: 0px !important" id="' + cell.Value.custid + '" class="menuInactive"></div>';
                        }
                    },
                    { HeaderText: 'Username', Name: 'username', Width: 120,
                        CustomCellRender: function (cell) {
                            var custId = cell.Value.custid;

                            var func = "javascript:Fanex.ExtAcc.Customer.GetMemberList(" + custId + ")";

                            return '<div><a href="' + func + '">' + cell.Value.username + '</a></div>';
                        }
                    },
                    { HeaderText: 'Edit', Name: 'Edit', Width: 30,
                        CustomCellRender: function (cell) {
                            return '<div onclick="" id="edit-' + cell.Value.custid + '" class="editInfo"></div>';
                        }
                    },
                    { HeaderText: 'Status', Name: 'Status', Width: 60, CellAlign: 'center',
                        CustomCellRender: function (cell) {
                            return cell.Value.closed ? 'Closed' : (cell.Value.suspended ? 'Suspended' : (!cell.Value.enabled ? 'Disabled' : 'Open'));
                        }
                    },
                    { HeaderText: 'Double Comm', Name: 'DoubleCommLosing', Width: 100,
                        CustomCellRender: function (cell) {
                            if (cell.Value.DoubleCommLosing) {
                                return '<div id="dbl-' + cell.Value.custid + '" class="allowDblComm"></div>';
                            }

                            return '<div id="dbl-' + cell.Value.custid + '" class="disallowDblComm"></div>';
                        }
                    },
                    { HeaderText: 'First Name', Name: 'firstname', Width: 120 },
                    { HeaderText: 'Last Name', Name: 'lastname', Width: 120 },
                    { HeaderText: 'Group A', Name: 'groupa', Width: 60, CellAlign: 'center' },
                    { HeaderText: 'Group B', Name: 'groupb', Width: 60, CellAlign: 'center' },
                    { HeaderText: 'Group C', Name: 'groupc', Width: 60, CellAlign: 'center' },
                    { HeaderText: 'Group D', Name: 'groupd', Width: 60, CellAlign: 'center' },
                    { HeaderText: '1X2', Name: 'discount1x2', Width: 60, CellAlign: 'center' },
                    { HeaderText: 'Others', Name: 'discountcs', Width: 60, CellAlign: 'center' },
                    { HeaderText: 'Game', Name: 'discountnumber', Width: 60, CellAlign: 'center' },
                    { HeaderText: 'Created Date', Name: 'createdate', CellAlign: 'center', Width: 140,
                        CustomCellRender: function (cell) {
                            if (cell.Value) {
                                return cell.Value.replace('T', ' ');
                            }

                            return new Date().toDateString();
                        }
                    },
                    { HeaderText: 'Login IP', Name: 'lastloginip', CellAlign: 'center', Width: 140,
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

            Fanex.ExtAcc.Customer.DataSource = rows;

            for (var i = 0; i < rows.length; i++) {
                bodyRows.push({
                    Columns: [
                        { Name: 'RowNumber', Value: rows[i].RowNumber },
                        { Name: 'Check', Value: rows[i] },
                        { Name: 'username', Value: rows[i] },
                        { Name: 'Edit', Value: rows[i] },
                        { Name: 'Status', Value: rows[i] },
                        { Name: 'DoubleCommLosing', Value: rows[i] },
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
                    { HeaderText: 'No.', Name: 'RowNumber', MergeRowNumber: 2 },
                    { HeaderText: '', Name: 'Check', MergeRowNumber: 2 },
                    { HeaderText: 'Username', Name: 'username', MergeRowNumber: 2 },
                    { HeaderText: 'Edit', Name: 'Edit', MergeRowNumber: 2 },
                    { HeaderText: 'Status', Name: 'Status', MergeRowNumber: 2 },
                    { HeaderText: 'Double Comm', Name: 'DoubleCommLosing', MergeRowNumber: 2 },
                    { HeaderText: 'First Name', Name: 'firstname', MergeRowNumber: 2 },
                    { HeaderText: 'Last Name', Name: 'lastname', MergeRowNumber: 2 },
                    { HeaderText: 'Commission', Name: 'groupa', MergeColNumber: 7 },
                    { HeaderText: 'Created Date', Name: 'createdate', MergeRowNumber: 2 },
                    { HeaderText: 'Login IP', Name: 'lastloginip', MergeRowNumber: 2 }
                ]
            }
            ]

            options = {
                columns: Fanex.ExtAcc.Customer.DataFields,
                bodyRows: bodyRows,
                gridExpandHeight: 165,
                numberOfFrozenColumn: 6,
                customizeHeader: true,
                groupHeaderRows: groupHeader,
                highlightFrozenWidth: 2,
                showPager: true,
                customPager: false,
                pagerOption: {
                    itemsPerPageArray: [50, 100, 500],
                    currentPage: 0,
                    totalItem: rows.length,
                    itemsPerPage: 50
                },
                marginRight: 2,
                showDistinctRow: false
            };

            Fanex.ExtAcc.Customer.Render(options);
            Fanex.ExtAcc.Customer.totalRows = rows.length;

            $('#mainGrid').css('visibility', 'visible');

            $('#breadcrumbs').empty();

            $('#breadcrumbs').append('<div style="float:left">' + linkMaster + '</div><div class="viewNext">&nbsp;</div><div style="float:left">' + linkSuper + '</div>');

            Fanex.ExtAcc.OnResize = function () {
                $this.leftMenuWidth = Fanex.ExtAcc.leftMenuWidth;
                try {
                    $("#jqgrid").fanexGrid("resizeGrid");
                }
                catch (err) {
                }
            }
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
                    { HeaderText: 'No.', Name: 'RowNumber', Width: 30, CellAlign: 'center' },
                    { HeaderText: '', Name: 'Check', Width: 30, CellAlign: 'center',
                        CustomHeaderRender: function (cell) {
                            return '<div class="editMult"></div>';
                        },
                        CustomCellRender: function (cell) {
                            return '<div onclick="Fanex.ExtAcc.Customer.Select(this)" style="margin-left: 0px !important" id="' + cell.Value.custid + '" class="menuInactive"></div>';
                        }
                    },
                    { HeaderText: 'Username', Name: 'username', Width: 120,
                        CustomCellRender: function (cell) {
                            return '<div style="margin-left:5px">' + cell.Value.username + '</div>';
                        }
                    },
                    { HeaderText: 'Edit', Name: 'Edit', Width: 30,
                        CustomCellRender: function (cell) {
                            return '<div onclick="" id="edit-' + cell.Value.custid + '" class="editInfo"></div>';
                        }
                    },
                    { HeaderText: 'Status', Name: 'Status', Width: 60, CellAlign: 'center',
                        CustomCellRender: function (cell) {
                            return cell.Value.closed ? 'Closed' : (cell.Value.suspended ? 'Suspended' : (!cell.Value.enabled ? 'Disabled' : 'Open'));
                        }
                    },
                    { HeaderText: 'Double Comm', Name: 'DoubleCommLosing', Width: 100,
                        CustomCellRender: function (cell) {
                            if (cell.Value.DoubleCommLosing) {
                                return '<div id="dbl-' + cell.Value.custid + '" class="onDblComm"></div>';
                            }

                            return '<div id="dbl-' + cell.Value.custid + '" class="offDblComm"></div>';
                        }
                    },
                    { HeaderText: 'Nickname', Name: 'NickName', Width: 120 },
                    { HeaderText: 'First Name', Name: 'firstname', Width: 120 },
                    { HeaderText: 'Last Name', Name: 'lastname', Width: 120 },
                    { HeaderText: 'User Group', Name: 'ugroup', CellAlign: 'center', Width: 120 },
                    { HeaderText: 'Group', Name: 'discount', CellAlign: 'center', Width: 60 },
                    { HeaderText: '1X2', Name: 'discount1x2', CellAlign: 'center', Width: 60 },
                    { HeaderText: 'Others', Name: 'discountcs', CellAlign: 'center', Width: 60 },
                    { HeaderText: 'Game', Name: 'discountnumber', CellAlign: 'center', Width: 60 },
                    { HeaderText: 'Group', Name: 'playerdiscount', CellAlign: 'center', Width: 60 },
                    { HeaderText: '1X2', Name: 'playerdiscount1x2', CellAlign: 'center', Width: 60 },
                    { HeaderText: 'Others', Name: 'playerdiscountcs', CellAlign: 'center', Width: 60 },
                    { HeaderText: 'Game', Name: 'playerdiscountnumber', CellAlign: 'center', Width: 60 },
                    { HeaderText: 'Created Date', Name: 'createdate', CellAlign: 'center', Width: 140,
                        CustomCellRender: function (cell) {
                            if (cell.Value) {
                                return cell.Value.replace('T', ' ');
                            }

                            return new Date().toDateString();
                        }
                    },
                    { HeaderText: 'Login IP', Name: 'lastloginip', CellAlign: 'center', Width: 140,
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

            Fanex.ExtAcc.Customer.DataSource = rows;

            for (var i = 0; i < rows.length; i++) {
                bodyRows.push({
                    Columns: [
                        { Name: 'RowNumber', Value: rows[i].RowNumber },
                        { Name: 'Check', Value: rows[i] },
                        { Name: 'username', Value: rows[i] },
                        { Name: 'Edit', Value: rows[i] },
                        { Name: 'Status', Value: rows[i] },
                        { Name: 'DoubleCommLosing', Value: rows[i] },
                        { Name: 'NickName', Value: rows[i].NickName },
                        { Name: 'firstname', Value: rows[i].firstname },
                        { Name: 'lastname', Value: rows[i].lastname },
                        { Name: 'ugroup', Value: rows[i].ugroup },
                        { Name: 'discount', Value: rows[i].discount },
                        { Name: 'discount1x2', Value: rows[i].discount1x2 },
                        { Name: 'discountcs', Value: rows[i].discountcs },
                        { Name: 'discountnumber', Value: rows[i].discountnumber },
                        { Name: 'playerdiscount', Value: rows[i].playerdiscount },
                        { Name: 'playerdiscount1x2', Value: rows[i].playerdiscount1x2 },
                        { Name: 'playerdiscountcs', Value: rows[i].playerdiscountcs },
                        { Name: 'playerdiscountnumber', Value: rows[i].playerdiscountnumber },
                        { Name: 'createdate', Value: rows[i].createdate },
                        { Name: 'lastloginip', Value: rows[i].lastloginip }
                    ]
                });
            }

            var groupHeader =
            [{
                Columns: [
                    { HeaderText: 'No.', Name: 'RowNumber', MergeRowNumber: 2 },
                    { HeaderText: '', Name: 'Check', MergeRowNumber: 2 },
                    { HeaderText: 'Username', Name: 'username', MergeRowNumber: 2 },
                    { HeaderText: 'Edit', Name: 'Edit', MergeRowNumber: 2 },
                    { HeaderText: 'Status', Name: 'Status', MergeRowNumber: 2 },
                    { HeaderText: 'Double Comm', Name: 'DoubleCommLosing', MergeRowNumber: 2 },
                    { HeaderText: 'Nickname', Name: 'NickName', MergeRowNumber: 2 },
                    { HeaderText: 'First Name', Name: 'firstname', MergeRowNumber: 2 },
                    { HeaderText: 'Last Name', Name: 'lastname', MergeRowNumber: 2 },
                    { HeaderText: 'User Group', Name: 'ugroup', MergeRowNumber: 2 },
                    { HeaderText: 'Agent Commission', Name: 'discount', MergeColNumber: 4 },
                    { HeaderText: 'Player Commission', Name: 'playerdiscount', MergeColNumber: 4 },
                    { HeaderText: 'Created Date', Name: 'createdate', MergeRowNumber: 2 },
                    { HeaderText: 'Login IP', Name: 'lastloginip', MergeRowNumber: 2 }
                ]
            }
            ]

            options = {
                columns: Fanex.ExtAcc.Customer.DataFields,
                bodyRows: bodyRows,
                gridExpandHeight: 165,
                numberOfFrozenColumn: 6,
                customizeHeader: true,
                groupHeaderRows: groupHeader,
                highlightFrozenWidth: 2,
                showPager: true,
                customPager: false,
                pagerOption: {
                    itemsPerPageArray: [50, 100, 500],
                    currentPage: 0,
                    totalItem: rows.length,
                    itemsPerPage: 50
                },
                marginRight: 2,
                showDistinctRow: false
            };

            Fanex.ExtAcc.Customer.Render(options);
            Fanex.ExtAcc.Customer.totalRows = rows.length;

            $('#mainGrid').css('visibility', 'visible');

            $('#breadcrumbs').empty();

            var linkSuper = '<a href="' + "javascript:Fanex.ExtAcc.Customer.GetSuperList()" + '">' + 'Super' + '</a>';
            var linkMaster = '<a href="' + "javascript:Fanex.ExtAcc.Customer.GetMasterList(" + $this.superId + ')"' + '">' + 'Master' + '</a>';
            var linkAgent = '<a href="' + "javascript:Fanex.ExtAcc.Customer.GetAgentList(" + $this.masterId + ')"' + '">' + 'Agent' + '</a>';

            $('#breadcrumbs').append('<div style="float:left">' + linkAgent + '</div><div class="viewNext">&nbsp;</div><div style="float:left">' + linkMaster + '</div><div class="viewNext">&nbsp;</div><div style="float:left">' + linkSuper + '</div>');

            Fanex.ExtAcc.OnResize = function () {
                $this.leftMenuWidth = Fanex.ExtAcc.leftMenuWidth;
                try {
                    $("#jqgrid").fanexGrid("resizeGrid");
                }
                catch (err) {
                }
            }
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

    ShowIpInfo: function (ip) {
        Fanex.ExtAcc.Form.Show(resolveClientUrl('Customer/ViewIP?ip=' + ip), 'IP Information', 450, 230, false, null, 2);
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
            if (Fanex.ExtAcc.Customer.DataSource.length == 0) {
                $('#msgNoinfo').css('display', 'block');
                $('#jqgrid').css('display', 'none');
            }
            else {
                $('#msgNoinfo').css('display', 'none');
                $('#jqgrid').css('display', 'block');
                // Init Grid jqgrid
                $('#jqgrid').fanexGrid(options);
            }

            $(document).ajaxStop($.unblockUI);
        }
        catch (e) {
            if (typeof console != 'undefined') {
                console.log(e);
            }

            $(document).ajaxStop($.unblockUI);
        }
    },

    SearchList: function () {
    }
}