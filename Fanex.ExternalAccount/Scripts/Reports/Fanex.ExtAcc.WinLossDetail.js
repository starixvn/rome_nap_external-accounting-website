/// <reference path="../_references.js" />
if (typeof Fanex == 'undefined' || typeof Fanex.ExtAcc == 'undefined') {
    alert("Fanex.ExtAcc.js module is required!")
}

Fanex.ExtAcc.WinLossDetail = {
    groupName: '',
    username: '',
    srecommend: 0,
    mrecommend: 0,
    recommend: 0,
    custId: 0,
    supername: '',
    mastername: '',
    agentname: '',
    fromdate: new Date(),
    todate: new Date(),
    parameterProducts: '',
    fixedsortdata: new Array(),
    removedsortdata: new Array(),
    fixedsortlist: new Array(),
    url: resolveClientUrl('WinLossDetail/GetData'),
    viewlevel: 5,
    dataSource: [],
    $this: null,
    SetRange: function () {
        return {
            minDate: new Date($('#MinDate').val()),
            maxDate: new Date($('#MaxDate').val())
        }
    },
    SwitchCalendarType: function (showType) {
        if (showType == 1) {
            $('#GroupViewHistory').css('display', 'none');
            $('#GroupBackToCurrent').css('display', 'block');
            $('.from-to-date').css('display', 'none');
            $('.group-month-day').css('display', 'inline-block');
            //$('.from-to-date').removeClass("showcontent").addClass("hide");
            //$('.group-month-day').removeClass("hide").addClass('showcontent');
        }
        else {
            $('#GroupViewHistory').css('display', 'block');
            $('#GroupBackToCurrent').css('display', 'none');
            $('.from-to-date').css('display', 'inline-block');
            $('.group-month-day').css('display', 'none');
            //$('.from-to-date').removeClass("hide").addClass("showcontent");
            //$('.group-month-day').removeClass("showcontent").addClass("hide");
        }
    },
    CallSubmit: function () {
        this.groupName = $('#selectedGroupName').html();
        this.ResetParameters();
        this.username = Fanex.encodeHtml($("#txtUserName").val());
        this.Databind(this.username);
    },
    initMonthlyCalendar: function () {
        $('#monPickr1').monPickr({
            today: $('#MaxDate').val(),
            minDate: '2010/1/1',
            showDateRange: true,
            fromDayId: 'fromDay1',
            toDayId: 'toDay1',
            log: function (msg) {
                if (typeof console != 'undefined')
                    console.warn(msg);
            }
        });

        var today = new Date($('#MaxDate').val());
        $('#monPickr1').monPickr("setMonth", today);
        $('#monPickr1').monPickr("setToDay", today.getDate());
        $('#monPickr1').monPickr("setFromDay", 1);
    },
    getDayOfYear: function (date) {
        var day = date.getDate();
        var month = date.getMonth() + 1;
        var year = date.getFullYear();
        var totalDays = day + (2 * month) + parseInt(3 * (month + 1) / 5) + year + parseInt(year / 4) - parseInt(year / 100) + parseInt(year / 400) + 2;
        var dayNo = (totalDays % 7);
        if (dayNo == 0) {
            dayNo = 7;
        }
        return dayNo;
    },
    getParameterProducts: function () {
        this.parameterProducts = '';
        var ids = $(".ui-multiselect-checkboxes :input");
        for (var i = 0; i < ids.length; i++) {
            this.parameterProducts += "&" + ids[i].value + "=" + ids[i].checked;
        }
    },
    InitProductsList: function () {
        $('#GroupProducts').css('display', 'block');
        $("#GroupProducts").multiSelect({
            minWidth: 170,
            showHeader: false,
            onCheck: function () {
                if (this.value == 'showAll') {
                    var ids = $(".ui-multiselect-checkboxes :input");
                    for (var i = 0; i < ids.length; i++) {
                        ids[i].checked = this.checked;
                    }
                }
                else {
                    if (!this.checked) {
                        $('#ui-multiselect-GroupProducts-option-0')[0].checked = false;
                    }
                }
            },
            noneSelectedText: function () {
                $("#GroupProducts").attr("title", "");
                return '';
            },
            selectedText: function (numChecked, numTotal, checkedItems) {
                if (checkedItems[0].value != 'showAll') {
                    numChecked++;
                }

                if (numChecked != numTotal) {
                    var mainTitle = '';
                    $.each(checkedItems, function (i) {
                        mainTitle += checkedItems[i].title + ', ';
                    });

                    mainTitle = mainTitle.substring(0, mainTitle.length - 2);
                    $("#GroupProducts").attr("title", mainTitle);

                    return mainTitle;
                }
                else {
                    $('#ui-multiselect-GroupProducts-option-0')[0].checked = true;
                    $("#GroupProducts").attr("title", "All");
                    return 'All';
                }
            }
        });
    },
    ProductsList: function () {
        $.ajax({
            context: $this,
            url: resolveClientUrl("Filter/ProductsList"),
            type: 'GET',
            dataType: 'json',
            cache: false,
            success: function (result) {
                var productList = '';
                $.each(result, function (i) {
                    productList += '<option value="' + result[i].Key + '" selected="selected">' + result[i].Value + '</option>';
                });
                $('#GroupProducts').html(productList);
                $this.InitProductsList();
                $("#GroupProducts :input").width($("#GroupProducts").width() - 21);
            }
        })
    },

    Init: function () {
        $this = this;
        Fanex.ExtAcc.InitSuggestion();

        $("#DateList").combobox({
            selected: function (event, ui) {
                var itemIndex = event.target.selectedIndex;
                $("#DateList").prop("selectedIndex", itemIndex);

                var fdate = new Date($('#MaxDate').val());
                var tdate = new Date($('#MaxDate').val());
                if (itemIndex == 1) {
                    fdate = new Date(tdate.getFullYear(), tdate.getMonth(), tdate.getDate() - 1);
                    tdate = fdate;
                } else if (itemIndex == 2) {
                    fdate = new Date(tdate.getFullYear(), tdate.getMonth(), tdate.getDate() - $this.getDayOfYear(tdate) + 1);
                } else if (itemIndex == 3) {
                    fdate = new Date(tdate.getFullYear(), tdate.getMonth(), tdate.getDate() - $this.getDayOfYear(tdate) - 6);
                    tdate = new Date(tdate.getFullYear(), tdate.getMonth(), tdate.getDate() - $this.getDayOfYear(tdate));
                } else if (itemIndex == 4) {
                    fdate = new Date(tdate.getFullYear(), tdate.getMonth(), 1);
                    tdate = new Date(tdate.getFullYear(), tdate.getMonth(), tdate.getDate());
                } else if (itemIndex == 5) {
                    fdate = new Date(tdate.getFullYear(), tdate.getMonth() - 1, 1);
                    tdate = new Date(tdate.getFullYear(), tdate.getMonth(), 0);
                } else if (itemIndex == 6) {
                    fdate = new Date(tdate.getFullYear(), tdate.getMonth() - 1, 1);
                } else if (itemIndex == 7) {
                    return;
                }

                $("#dtpFromDate").val($.datepicker.formatDate('M dd, yy', fdate));
                $("#dtpToDate").val($.datepicker.formatDate('M dd, yy', tdate));
            }
        });

        $("#userGroup input").attr("readonly", "true");
        $("#userGroup input").bind("click", function () {
            $(this).autocomplete("search", '');
        });

        $("#userGroup input").val($("#DateList")[0].options[0].value);

        $("#dtpFromDate").datepicker({
            onClose: function (date) {
                $("#userGroup input").val($("#DateList")[0].options[7].value);
            }, showOn: 'both', buttonImageOnly: true, buttonImage: resolveClientUrl('Content/Images/calendar.png'), buttonText: '', beforeShow: $this.SetRange, dateFormat: 'M dd, yy'
        });
        $("#dtpFromDate").val($.datepicker.formatDate('M dd, yy', new Date($('#MaxDate').val())));

        $("#dtpToDate").datepicker({
            onClose: function (date) {
                $("#userGroup input").val($("#DateList")[0].options[7].value);
            }, showOn: 'both', buttonImageOnly: true, buttonImage: resolveClientUrl('Content/Images/calendar.png'), buttonText: '', beforeShow: $this.SetRange, dateFormat: 'M dd, yy'
        });
        $("#dtpToDate").val($.datepicker.formatDate('M dd, yy', new Date($('#MaxDate').val())));

        $this.initMonthlyCalendar();

        $('#ddlGroupName').bind('change', function (event) {
            var args = event.args;
            if (args) {
                var item = args.item;
                $this.groupName = item.innerHTML;
                //$("#txtUserName").val('');
                $this.ResetParameters();
                $this.InitShowHideControls('none');
            }
        });

        this.ProductsList();

        $('#Submit').click(function (e) {
            $this.CallSubmit();
        });

        $('#txtUserName').keyup(function (e) {
            if ($('#txtUserName').val().length > 1) {
                Fanex.ExtAcc.InitSuggestion();
            }
        });

        $('#exporttoexcel').click(function () {
            Fanex.ExtAcc.WinLossDetail.getParameterProducts();
            var params = "fromdate=" + $this.fromdate + "&todate=" + $this.todate + "&viewlevel=" + $this.viewlevel +
            '&custid=' + $this.custId + '&groupname=' + $this.groupName + '&username=' + Fanex.ExtAcc.WinLossDetail.username;
            document.location.href = resolveClientUrl("WinLossDetail/GetExportToExcelWinLossDetail?") + params + Fanex.ExtAcc.WinLossDetail.parameterProducts;
        });

        $('#ViewHistoryReport').click(function () {
            $this.SwitchCalendarType(1);
        });
        $('#BackToCurrentReport').click(function () {
            $this.SwitchCalendarType(0);
        });

        $('#tabSuper').click(function (e) {
            if ($this.supername != '') {
                $this.ResetParameters();
                $this.username = $this.supername;
                $this.Databind();
            }
            else {
                $this.viewlevel = 5;
                $this.custId = 0;
                $this.srecommend = $("#txtUserName").val() == '' ? 0 : $this.srecommend;
                //$("#txtUserName").val('');
                $this.username = '';
                $this.ToggleTabs('none', 'none', 'none', 'none', 4);
                $this.Databind();
            }
        });
        $('#tabMaster').click(function (e) {
            if ($this.mastername != '') {
                $this.ResetParameters();
                $this.username = $this.mastername;
                $this.Databind();
            }
            else {
                $this.viewlevel = 4;
                $this.custId = $this.srecommend;
                //$("#txtUserName").val('');
                $this.username = '';
                $this.ToggleTabs('block', 'block', 'none', 'none', 3);
                $this.Databind();
            }
        });
        $('#tabAgent').click(function (e) {
            if ($this.agentname != '') {
                $this.ResetParameters();
                $this.username = $this.agentname;
                $this.Databind();
            }
            else {
                $this.viewlevel = 3;
                $this.custId = $this.mrecommend;
                //$("#txtUserName").val('');
                $this.username = '';
                $this.ToggleTabs('block', 'block', 'block', 'none', 2);
                $this.Databind();
            }
        });

        $this.groupName = $('#selectedGroupName').html();

        $('#txtUserName').keypress(function (e) {
            if (typeof e == 'undefined') e = window.event;
            var key = (typeof e.keyCode == 'undefined') ? e.which : e.keyCode;
            if (key == 13) {
                $this.CallSubmit();
                return true;
            } else {
                return true;
            }
        });

        $this.sorters = $(".col-sorter");
        $this.sorters.click(function () {
            var elem = $(this);
            $this.sortTable(elem);
        });

        Fanex.ExtAcc.OnResize = $this.resizeReportGrid;
        $(".icon-filter").click($this.showHideFilter);
    },

    resizeReportGrid: function () {
        try {
            $("#jqgrid").fanexGrid("resizeGrid");
        }
        catch (err) { }
    },
    hideFilter: false,
    showHideFilter: function () {
        if ($this.hideFilter) {
            $(".icon-filter").addClass("active");
            $(".filterbox").removeClass("hide");
        } else {
            $(".icon-filter").removeClass("active");
            $(".filterbox").addClass("hide");
        }

        $this.resizeReportGrid();
        $this.hideFilter = !$this.hideFilter;
    },
    ToggleTabs: function (levelLine, statusSAg, statusMAg, statusAg, status) {
        $('#tabSuper').css('display', statusSAg);
        $('#tabMaster').css('display', statusMAg);
        $('#tabAgent').css('display', statusAg);
        $('#levelLine').css('display', levelLine);
        if (status == 1) {
            $('#SelectedLevel').html('Member');
        }
        else if (status == 2) {
            $('#SelectedLevel').html('Agent');
        }
        else if (status == 3) {
            $('#SelectedLevel').html('Master');
        }
        else if (status == 4) {
            $('#SelectedLevel').html('Super');
        }
    },

    RenderUsernameColumn: function (cell) {
        if (cell == null || cell.Value.Username == 'Grand Total' || $this.viewlevel < 2) {
            return '<div class="text-truncate" style="line-height: 22px; text-align: left; padding-left: 3px" title="' + cell.Value.Username + '">' + cell.Value.Username + '</div>';
        }

        var func = "javascript: Fanex.ExtAcc.WinLossDetail.GetDownlineWinLossDetail('" + cell.Value.CustId + "')";
        return html = '<div style="line-height: 22px; text-align: left;">' +
                            '<a class="text-truncate" style="text-decoration: none; margin: 0" href="' + func + '" title="' + cell.Value.Username + '">' + cell.Value.Username + '</a>' +
                        '</div>';
    },
    CustomUsernameSortFunction: function (cell1Value, cell2Value, sortDirection) {
        if (sortDirection == 'ASC') {
            return cell1Value.Username > cell2Value.Username ? 1 : -1;
        }

        return cell1Value.Username < cell2Value.Username ? 1 : -1;
    },
    defaultColumns: [
        {
            HeaderText: 'Username', Name: 'Username', Width: 130, Sort: true, HeaderAlign: 'center',
            CustomCellRender: function (cell) { return $this.RenderUsernameColumn(cell); }, CustomSortFunction: function (cell1Value, cell2Value, sortDirection) { return $this.CustomUsernameSortFunction(cell1Value, cell2Value, sortDirection) }
        },
        { HeaderText: 'Turnover', Name: 'Turnover', Width: 130, Sort: true, HeaderAlign: "center", CellAlign: 'right', FormatNumber: '0,0.00', FormatNegative: '$' },
        { HeaderText: 'Net Turnover', Name: 'NetTurnover', Width: 130, Sort: true, HeaderAlign: "center", CellAlign: 'right', FormatNumber: '0,0.00', FormatNegative: '$' },
        { HeaderText: 'GrossComm', Name: 'GrossComm', Width: 130, Sort: true, HeaderAlign: "center", CellAlign: 'right', FormatNumber: '0,0.00', FormatNegative: '$' },
        { HeaderText: 'Win/Loss', Name: 'MemberWinlost', Width: 130, Sort: true, HeaderAlign: "center", CellAlign: 'right', FormatNumber: '0,0.00', FormatNegative: '$' },
        { HeaderText: 'Comm', Name: 'PlayerComm', Width: 130, Sort: true, HeaderAlign: "center", CellAlign: 'right', FormatNumber: '0,0.00', FormatNegative: '$' },
        { HeaderText: 'Total', Name: 'MemberTotal', Width: 130, Sort: true, HeaderAlign: "center", CellAlign: 'right', FormatNumber: '0,0.00', FormatNegative: '$' },
        { HeaderText: 'Win/Loss', Name: 'AgWinlost', Width: 130, Sort: true, HeaderAlign: "center", CellAlign: 'right', FormatNumber: '0,0.00', FormatNegative: '$' },
        { HeaderText: 'Comm', Name: 'AgComm', Width: 130, Sort: true, HeaderAlign: "center", CellAlign: 'right', FormatNumber: '0,0.00', FormatNegative: '$' },
        { HeaderText: 'Total', Name: 'AgTotal', Width: 130, Sort: true, HeaderAlign: "center", CellAlign: 'right', FormatNumber: '0,0.00', FormatNegative: '$' },
        { HeaderText: 'Win/Loss', Name: 'MAgWinlost', Width: 130, Sort: true, HeaderAlign: "center", CellAlign: 'right', FormatNumber: '0,0.00', FormatNegative: '$' },
        { HeaderText: 'Comm', Name: 'MAgComm', Width: 130, Sort: true, HeaderAlign: "center", CellAlign: 'right', FormatNumber: '0,0.00', FormatNegative: '$' },
        { HeaderText: 'Total', Name: 'MAgTotal', Width: 130, Sort: true, HeaderAlign: "center", CellAlign: 'right', FormatNumber: '0,0.00', FormatNegative: '$' },
        { HeaderText: 'Win/Loss', Name: 'SAgWinlost', Width: 130, Sort: true, HeaderAlign: "center", CellAlign: 'right', FormatNumber: '0,0.00', FormatNegative: '$' },
        { HeaderText: 'Comm', Name: 'SAgComm', Width: 130, Sort: true, HeaderAlign: "center", CellAlign: 'right', FormatNumber: '0,0.00', FormatNegative: '$' },
        { HeaderText: 'Total', Name: 'SAgTotal', Width: 130, Sort: true, HeaderAlign: "center", CellAlign: 'right', FormatNumber: '0,0.00', FormatNegative: '$' },
        { HeaderText: 'Company', Name: 'Company', Width: 130, Sort: true, HeaderAlign: "center", CellAlign: 'right', FormatNumber: '0,0.00', FormatNegative: '$' }
    ],
    MAgColumns: [
        {
            HeaderText: 'Username', Name: 'Username', Width: 130,
            Sort: true, HeaderAlign: 'center', CustomCellRender: function (cell) { return $this.RenderUsernameColumn(cell); }, CustomSortFunction: function (cell1Value, cell2Value, sortDirection) { return $this.CustomUsernameSortFunction(cell1Value, cell2Value, sortDirection) }
        },
        { HeaderText: 'Turnover', Name: 'Turnover', Width: 130, Sort: true, HeaderAlign: "center", CellAlign: 'right', FormatNumber: '0,0.00', FormatNegative: '$' },
        { HeaderText: 'Net Turnover', Name: 'NetTurnover', Width: 130, Sort: true, HeaderAlign: "center", CellAlign: 'right', FormatNumber: '0,0.00', FormatNegative: '$' },
        { HeaderText: 'GrossComm', Name: 'GrossComm', Width: 130, Sort: true, HeaderAlign: "center", CellAlign: 'right', FormatNumber: '0,0.00', FormatNegative: '$' },
        { HeaderText: 'Win/Loss', Name: 'MemberWinlost', Width: 130, Sort: true, HeaderAlign: "center", CellAlign: 'right', FormatNumber: '0,0.00', FormatNegative: '$' },
        { HeaderText: 'Comm', Name: 'PlayerComm', Width: 130, Sort: true, HeaderAlign: "center", CellAlign: 'right', FormatNumber: '0,0.00', FormatNegative: '$' },
        { HeaderText: 'Total', Name: 'MemberTotal', Width: 130, Sort: true, HeaderAlign: "center", CellAlign: 'right', FormatNumber: '0,0.00', FormatNegative: '$' },
        { HeaderText: 'Win/Loss', Name: 'AgWinlost', Width: 130, Sort: true, HeaderAlign: "center", CellAlign: 'right', FormatNumber: '0,0.00', FormatNegative: '$' },
        { HeaderText: 'Comm', Name: 'AgComm', Width: 130, Sort: true, HeaderAlign: "center", CellAlign: 'right', FormatNumber: '0,0.00', FormatNegative: '$' },
        { HeaderText: 'Total', Name: 'AgTotal', Width: 130, Sort: true, HeaderAlign: "center", CellAlign: 'right', FormatNumber: '0,0.00', FormatNegative: '$' },
        { HeaderText: 'Win/Loss', Name: 'MAgWinlost', Width: 130, Sort: true, HeaderAlign: "center", CellAlign: 'right', FormatNumber: '0,0.00', FormatNegative: '$' },
        { HeaderText: 'Comm', Name: 'MAgComm', Width: 130, Sort: true, HeaderAlign: "center", CellAlign: 'right', FormatNumber: '0,0.00', FormatNegative: '$' },
        { HeaderText: 'Total', Name: 'MAgTotal', Width: 130, Sort: true, HeaderAlign: "center", CellAlign: 'right', FormatNumber: '0,0.00', FormatNegative: '$' },
        { HeaderText: 'Company', Name: 'Company', Width: 130, Sort: true, HeaderAlign: "center", CellAlign: 'right', FormatNumber: '0,0.00', FormatNegative: '$' }
    ],
    AgColumns: [
        {
            HeaderText: 'Username', Name: 'Username', Width: 130,
            Sort: true, HeaderAlign: 'center', CustomCellRender: function (cell) { return $this.RenderUsernameColumn(cell); }, CustomSortFunction: function (cell1Value, cell2Value, sortDirection) { return $this.CustomUsernameSortFunction(cell1Value, cell2Value, sortDirection) }
        },
        { HeaderText: 'Turnover', Name: 'Turnover', Width: 130, Sort: true, HeaderAlign: "center", CellAlign: 'right', FormatNumber: '0,0.00', FormatNegative: '$' },
        { HeaderText: 'Net Turnover', Name: 'NetTurnover', Width: 130, Sort: true, HeaderAlign: "center", CellAlign: 'right', FormatNumber: '0,0.00', FormatNegative: '$' },
        { HeaderText: 'GrossComm', Name: 'GrossComm', Width: 130, Sort: true, HeaderAlign: "center", CellAlign: 'right', FormatNumber: '0,0.00', FormatNegative: '$' },
        { HeaderText: 'Win/Loss', Name: 'MemberWinlost', Width: 130, Sort: true, HeaderAlign: "center", CellAlign: 'right', FormatNumber: '0,0.00', FormatNegative: '$' },
        { HeaderText: 'Comm', Name: 'PlayerComm', Width: 130, Sort: true, HeaderAlign: "center", CellAlign: 'right', FormatNumber: '0,0.00', FormatNegative: '$' },
        { HeaderText: 'Total', Name: 'MemberTotal', Width: 130, Sort: true, HeaderAlign: "center", CellAlign: 'right', FormatNumber: '0,0.00', FormatNegative: '$' },
        { HeaderText: 'Win/Loss', Name: 'AgWinlost', Width: 130, Sort: true, HeaderAlign: "center", CellAlign: 'right', FormatNumber: '0,0.00', FormatNegative: '$' },
        { HeaderText: 'Comm', Name: 'AgComm', Width: 130, Sort: true, HeaderAlign: "center", CellAlign: 'right', FormatNumber: '0,0.00', FormatNegative: '$' },
        { HeaderText: 'Total', Name: 'AgTotal', Width: 130, Sort: true, HeaderAlign: "center", CellAlign: 'right', FormatNumber: '0,0.00', FormatNegative: '$' },
        { HeaderText: 'Company', Name: 'Company', Width: 130, Sort: true, HeaderAlign: "center", CellAlign: 'right', FormatNumber: '0,0.00', FormatNegative: '$' }
    ],
    SAgHeaderColumns: [
        { HeaderText: "Username", Name: "Username", MergeRowNumber: 2 },
        { HeaderText: "Turnover", Name: "Turnover", MergeRowNumber: 2 },
        { HeaderText: "Net Turnover&nbsp;<span id=\"netTurnover\" class=\"bet-type-detail-icon\"></span>", Name: "NetTurnover", MergeRowNumber: 2 },
        { HeaderText: "GrossComm", Name: "GrossComm", MergeRowNumber: 2 },
        { HeaderText: "Member", Name: "MemberWinlost", MergeColNumber: 3 },
        { HeaderText: "Agent", Name: "AgWinlost", MergeColNumber: 3 },
        { HeaderText: "Master", Name: "MAgWinlost", MergeColNumber: 3 },
        { HeaderText: "Super", Name: "SAgWinlost", MergeColNumber: 3 },
        { HeaderText: "Company", Name: "Company", MergeRowNumber: 2 }
    ],
    MAgHeaderColumns: [
        { HeaderText: "Username", Name: "Username", MergeRowNumber: 2 },
        { HeaderText: "Turnover", Name: "Turnover", MergeRowNumber: 2 },
        { HeaderText: "Net Turnover&nbsp;<span id=\"netTurnover\" class=\"bet-type-detail-icon\"></span>", Name: "NetTurnover", MergeRowNumber: 2 },
        { HeaderText: "GrossComm", Name: "GrossComm", MergeRowNumber: 2 },
        { HeaderText: "Member", Name: "MemberWinlost", MergeColNumber: 3 },
        { HeaderText: "Agent", Name: "AgWinlost", MergeColNumber: 3 },
        { HeaderText: "Master", Name: "MAgWinlost", MergeColNumber: 3 },
        { HeaderText: "Company", Name: "Company", MergeRowNumber: 2 }
    ],
    AgHeaderColumns: [
        { HeaderText: "Username", Name: "Username", MergeRowNumber: 2 },
        { HeaderText: "Turnover", Name: "Turnover", MergeRowNumber: 2 },
        { HeaderText: "Net Turnover&nbsp;<span id=\"netTurnover\" class=\"bet-type-detail-icon\"></span>", Name: "NetTurnover", MergeRowNumber: 2 },
        { HeaderText: "GrossComm", Name: "GrossComm", MergeRowNumber: 2 },
        { HeaderText: "Member", Name: "MemberWinlost", MergeColNumber: 3 },
        { HeaderText: "Agent", Name: "AgWinlost", MergeColNumber: 3 },
        { HeaderText: "Company", Name: "Company", MergeRowNumber: 2 }
    ],
    Render: function (dateInfo) {
        var arrColData = new Array();
        var arrGroupHeaders = new Array();
        var arrGroupFooters = new Array();

        if (this.viewlevel == 1) {
            arrColData = this.AgColumns;
            arrGroupHeaders = this.AgHeaderColumns;
            arrGroupFooters = this.AgFooterColumns;
        } else if (this.viewlevel == 2) {
            arrColData = this.MAgColumns;
            arrGroupHeaders = this.MAgHeaderColumns;
            arrGroupFooters = this.MAgFooterColumns;
        } else {
            arrColData = this.defaultColumns;
            arrGroupHeaders = this.SAgHeaderColumns;
            arrGroupFooters = this.SAgFooterColumns;
        }

        var level = $this.viewlevel == 4 ? "Super" :
             $this.viewlevel == 3 ? "Master" :
              $this.viewlevel == 2 ? "Agent" :
              "Member";

        var gridOption = {
            customizeHeader: true,
            groupHeaderRows: [{ Columns: arrGroupHeaders }],
            columns: arrColData,
            numberOfFrozenColumn: isMobile.phone ? 1 : 3,
            autoCalculateGridExpandHeight: true,
            fullHeight: !isMobile.phone,
            gridHeight: isMobile.phone ? 400 : 0,
            showTitle: true,
            titleInfo: {
                text: level + " Win Loss Detail " + dateInfo
            },
            showFooter: true,
            gridExpandHeight: 256,
            showPager: true,
            pagerOption: {
                itemsPerPageArray: [50, 100, 500],
                currentPage: 0,
                totalItem: $this.totalRows,
                itemsPerPage: 50
            },
            marginRight: 2,
            bottomBorderWidth: 0,
            showDistinctRow: false,
            allowTruncate: true,
            sortOption:
            {
                afterGridSorted: function (rows) {
                    return $this.customsortfunc(rows);
                }
            }
        };

        // For Summary Footer
        var sumTurnover = 0;
        var sumNetTurnover = 0;
        var sumGrossComm = 0;
        var sumMemberWinlost = 0;
        var sumPlayerComm = 0;
        var sumMemberTotal = 0;
        var sumAgWinlost = 0;
        var sumAgComm = 0;
        var sumAgTotal = 0;
        var sumMAgWinlost = 0;
        var sumMAgComm = 0;
        var sumMAgTotal = 0;
        var sumSAgWinlost = 0;
        var sumSAgComm = 0;
        var sumSAgTotal = 0;
        var sumCompany = 0;

        //// Assing BodyData
        $this.InitSort();
        var bodyData = [];
        for (var i = 0; i < this.totalRows; i++) {
            if (this.viewlevel == 1) {
                bodyData.push({
                    Columns: [
                    { Name: "Username", Value: this.dataSource[i] },
                    { Name: "Turnover", Value: this.dataSource[i].Turnover },
                    { Name: "NetTurnover", Value: this.dataSource[i].NetTurnover },
                    { Name: "GrossComm", Value: this.dataSource[i].GrossComm },
                    { Name: "MemberWinlost", Value: this.dataSource[i].MemberWinlost },
                    { Name: "PlayerComm", Value: this.dataSource[i].PlayerComm },
                    { Name: "MemberTotal", Value: this.dataSource[i].MemberTotal },
                    { Name: "AgWinlost", Value: this.dataSource[i].AgWinlost },
                    { Name: "AgComm", Value: this.dataSource[i].AgComm },
                    { Name: "AgTotal", Value: this.dataSource[i].AgTotal },
                    { Name: "Company", Value: this.dataSource[i].Company }
                    ]
                })
            } else if (this.viewlevel == 2) {
                bodyData.push({
                    Columns: [
                    { Name: "Username", Value: this.dataSource[i] },
                    { Name: "Turnover", Value: this.dataSource[i].Turnover },
                    { Name: "NetTurnover", Value: this.dataSource[i].NetTurnover },
                    { Name: "GrossComm", Value: this.dataSource[i].GrossComm },
                    { Name: "MemberWinlost", Value: this.dataSource[i].MemberWinlost },
                    { Name: "PlayerComm", Value: this.dataSource[i].PlayerComm },
                    { Name: "MemberTotal", Value: this.dataSource[i].MemberTotal },
                    { Name: "AgWinlost", Value: this.dataSource[i].AgWinlost },
                    { Name: "AgComm", Value: this.dataSource[i].AgComm },
                    { Name: "AgTotal", Value: this.dataSource[i].AgTotal },
                    { Name: "MAgWinlost", Value: this.dataSource[i].MAgWinlost },
                    { Name: "MAgComm", Value: this.dataSource[i].MAgComm },
                    { Name: "MAgTotal", Value: this.dataSource[i].MAgTotal },
                    { Name: "Company", Value: this.dataSource[i].Company }
                    ]
                })
            } else {
                bodyData.push({
                    Columns: [
                    { Name: "Username", Value: this.dataSource[i] },
                    { Name: "Turnover", Value: this.dataSource[i].Turnover },
                    { Name: "NetTurnover", Value: this.dataSource[i].NetTurnover },
                    { Name: "GrossComm", Value: this.dataSource[i].GrossComm },
                    { Name: "MemberWinlost", Value: this.dataSource[i].MemberWinlost },
                    { Name: "PlayerComm", Value: this.dataSource[i].PlayerComm },
                    { Name: "MemberTotal", Value: this.dataSource[i].MemberTotal },
                    { Name: "AgWinlost", Value: this.dataSource[i].AgWinlost },
                    { Name: "AgComm", Value: this.dataSource[i].AgComm },
                    { Name: "AgTotal", Value: this.dataSource[i].AgTotal },
                    { Name: "MAgWinlost", Value: this.dataSource[i].MAgWinlost },
                    { Name: "MAgComm", Value: this.dataSource[i].MAgComm },
                    { Name: "MAgTotal", Value: this.dataSource[i].MAgTotal },
                    { Name: "SAgWinlost", Value: this.dataSource[i].SAgWinlost },
                    { Name: "SAgComm", Value: this.dataSource[i].SAgComm },
                    { Name: "SAgTotal", Value: this.dataSource[i].SAgTotal },
                    { Name: "Company", Value: this.dataSource[i].Company }
                    ]
                })
            }

            sumTurnover += this.dataSource[i].Turnover;
            sumNetTurnover += this.dataSource[i].NetTurnover;
            sumGrossComm += this.dataSource[i].GrossComm;
            sumMemberWinlost += this.dataSource[i].MemberWinlost;
            sumPlayerComm += this.dataSource[i].PlayerComm;
            sumMemberTotal += this.dataSource[i].MemberTotal;
            sumAgWinlost += this.dataSource[i].AgWinlost;
            sumAgComm += this.dataSource[i].AgComm;
            sumAgTotal += this.dataSource[i].AgTotal;
            if (this.viewlevel > 1) {
                sumMAgWinlost += this.dataSource[i].MAgWinlost;
                sumMAgComm += this.dataSource[i].MAgComm;
                sumMAgTotal += this.dataSource[i].MAgTotal;
            }
            if (this.viewlevel > 2) {
                sumSAgWinlost += this.dataSource[i].SAgWinlost;
                sumSAgComm += this.dataSource[i].SAgComm;
                sumSAgTotal += this.dataSource[i].SAgTotal;
            }
            sumCompany += this.dataSource[i].Company;
        }

        gridOption.bodyRows = bodyData;
        gridOption.pagerOption.totalItem = this.totalRows;

        //// Asign Footer Data
        var footerData = [];
        if (this.viewlevel == 1) {
            footerData.push({
                Columns: [
                    { Name: "Username", Value: "Grand Total" },
                    { Name: "Turnover", Value: sumTurnover },
                    { Name: "NetTurnover", Value: sumNetTurnover },
                    { Name: "GrossComm", Value: sumGrossComm },
                    { Name: "MemberWinlost", Value: sumMemberWinlost },
                    { Name: "PlayerComm", Value: sumPlayerComm },
                    { Name: "MemberTotal", Value: sumMemberTotal },
                    { Name: "AgWinlost", Value: sumAgWinlost },
                    { Name: "AgComm", Value: sumAgComm },
                    { Name: "AgTotal", Value: sumAgTotal },
                    { Name: "Company", Value: sumCompany }
                ]
            });
        } else if (this.viewlevel == 2) {
            footerData.push({
                Columns: [
                    { Name: "Username", Value: "Grand Total" },
                    { Name: "Turnover", Value: sumTurnover },
                    { Name: "NetTurnover", Value: sumNetTurnover },
                    { Name: "GrossComm", Value: sumGrossComm },
                    { Name: "MemberWinlost", Value: sumMemberWinlost },
                    { Name: "PlayerComm", Value: sumPlayerComm },
                    { Name: "MemberTotal", Value: sumMemberTotal },
                    { Name: "AgWinlost", Value: sumAgWinlost },
                    { Name: "AgComm", Value: sumAgComm },
                    { Name: "AgTotal", Value: sumAgTotal },
                    { Name: "MAgWinlost", Value: sumMAgWinlost },
                    { Name: "MAgComm", Value: sumMAgComm },
                    { Name: "MAgTotal", Value: sumMAgTotal },
                    { Name: "Company", Value: sumCompany }
                ]
            });
        } else {
            footerData.push({
                Columns: [
                    { Name: "Username", Value: "Grand Total" },
                    { Name: "Turnover", Value: sumTurnover },
                    { Name: "NetTurnover", Value: sumNetTurnover },
                    { Name: "GrossComm", Value: sumGrossComm },
                    { Name: "MemberWinlost", Value: sumMemberWinlost },
                    { Name: "PlayerComm", Value: sumPlayerComm },
                    { Name: "MemberTotal", Value: sumMemberTotal },
                    { Name: "AgWinlost", Value: sumAgWinlost },
                    { Name: "AgComm", Value: sumAgComm },
                    { Name: "AgTotal", Value: sumAgTotal },
                    { Name: "MAgWinlost", Value: sumMAgWinlost },
                    { Name: "MAgComm", Value: sumMAgComm },
                    { Name: "MAgTotal", Value: sumMAgTotal },
                    { Name: "SAgWinlost", Value: sumSAgWinlost },
                    { Name: "SAgComm", Value: sumSAgComm },
                    { Name: "SAgTotal", Value: sumSAgTotal },
                    { Name: "Company", Value: sumCompany }
                ]
            });
        }

        gridOption.footerRows = footerData;

        $("#jqgrid").fanexGrid(gridOption);
    },
    customsortfunc: function (rows) {
        var countFixedSort = $this.fixedsortlist.length;
        if (countFixedSort > 0) {
            for (var i = 0; i < countFixedSort; i++) {
                for (var j = 0; j < rows.length; j++) {
                    if ($this.fixedsortlist[i] == rows[j].Columns[0].Value.CustId) {
                        $this.fixedsortdata.push(rows.slice(j, j + 1)[0]);
                        rows.unshift(rows.splice(j, 1)[0]);
                        break;
                    }
                }
            }
        }
        return rows;
    },
    InitSort: function () {
        var countFixedSort = $this.fixedsortlist.length;
        if (countFixedSort > 0) {
            for (var i = 0; i < countFixedSort; i++) {
                for (var j = 0; j < $this.dataSource.length; j++) {
                    if ($this.fixedsortlist[i] == $this.dataSource[j].CustId) {
                        $this.fixedsortdata.push($this.dataSource.slice(j, j + 1)[0]);
                        $this.dataSource.unshift($this.dataSource.splice(j, 1)[0]);
                        break;
                    }
                }
            }
        }
    },
    ChangeHeadlineTab: function () {
        if (this.viewlevel >= 4) {
            this.srecommend = this.custId;
            this.ToggleTabs('none', 'none', 'none', 'none', 4);
        } else if (this.viewlevel == 3) {
            this.mrecommend = this.custId;
            this.ToggleTabs('block', 'block', 'none', 'none', 3);
            $("#tabSuper").attr("class", "tabActiveSuper");
        } else if (this.viewlevel == 2) {
            this.recommend = this.custId;
            this.ToggleTabs('block', 'block', 'block', 'none', 2);
            $("#tabSuper").attr("class", "tabDisableSuper");
            $("#tabMaster").attr("class", "tabActive");
        } else {
            this.ToggleTabs('block', 'block', 'block', 'block', 1);
            $("#tabSuper").attr("class", "tabDisableSuper");
            $("#tabMaster").attr("class", "tabDisable");
            $("#tabAgent").attr("class", "tabActive");
        };
        $('#headlineDaily').css("visibility", "visible");
    },
    GetDownlineWinLossDetail: function (custId) {
        this.custId = custId;
        //$("#txtUserName").val('');
        this.username = '';
        this.Databind(this.username);
    },
    InitShowHideControls: function (status) {
        $("#headlineDaily").css('display', status);
        $("#mainGrid").css('display', status);
    },
    ResetParameters: function () {
        this.srecommend = 0;
        this.mrecommend = 0;
        this.recommend = 0;
        this.custId = 0;
        this.username = '';
        this.viewlevel = 4;
    },
    GetFromDateToDate: function () {
        if ($("#GroupFromToDate").css('display') != 'none') {
            this.fromdate = $('#dtpFromDate').val();
            this.todate = $('#dtpToDate').val();
        }
        else {
            this.fromdate = $('#monPickr1').monPickr("getFromDate", "MM/dd/yyyy");
            this.todate = $('#monPickr1').monPickr("getToDate", "MM/dd/yyyy");
        }
    },
    Databind: function (username) {
        this.username = !username ? '' : username;
        this.GetFromDateToDate();
        this.groupName = $('#selectedGroupName').html();

        var dateInfo;
        if (this.fromdate == this.todate) {
            dateInfo = 'on ' + this.fromdate;
        }
        else {
            dateInfo = 'from ' + (new Date(Date.parse(this.fromdate))).toStringFormat('MMM dd, yyyy') + ' to ' + (new Date(Date.parse(this.todate))).toStringFormat('MMM dd, yyyy');
        }

        $this.getParameterProducts();
        var parameterUrl = "?groupname=" + this.groupName
                            + "&srecommend=" + this.srecommend
                            + "&mrecommend=" + this.mrecommend
                            + "&recommend=" + this.recommend
                            + "&custid=" + this.custId
                            + "&username=" + Fanex.ExtAcc.ReplaceAll(this.viewlevel == 5 ? '' : this.username, '&', '')
                            + "&fromdate=" + this.fromdate
                            + "&todate=" + this.todate
                            + "&viewlevel=" + this.viewlevel
                            + this.parameterProducts;

        Fanex.ExtAcc.BlockUI();

        $.ajax({
            context: $this,
            url: this.url + parameterUrl,
            type: 'post',
            dataType: 'json',
            cache: false,
            beforesend: function (xhr) { },
            success: function (data) {
                if (data.errorCode) {
                    Fanex.ExtAcc.UnBlockUI();
                    Fanex.ExtAcc.Form.Alert('Error', data.errorMessage.formatErrorMessage(), 350, 150, 'alertWLD');
                    return;
                }

                $this.fixedsortdata = new Array();
                $this.removedsortdata = new Array();

                $this.dataSource = data.rows;
                $this.totalRows = data.rows.length;
                $this.fixedsortlist = data.sortingorderlist;

                $this.viewlevel = data.viewlevel;
                if ($this.viewlevel == 3) {
                    $this.srecommend = data.srecommend;
                } else if ($this.viewlevel == 2) {
                    $this.srecommend = data.srecommend;
                    $this.mrecommend = data.mrecommend;
                } else if ($this.viewlevel == 1) {
                    $this.srecommend = data.srecommend;
                    $this.mrecommend = data.mrecommend;
                    $this.recommend = data.recommend;
                }

                if ($this.custId == 0) {
                    $this.supername = data.supername;
                    $this.mastername = data.mastername;
                    $this.agentname = data.agentname;
                }

                $this.ChangeHeadlineTab();

                $('#mainGrid').css('display', $this.totalRows < 1 ? 'none' : 'block');
                $('#mainGrid').css('visibility', $this.totalRows < 1 ? 'hidden' : 'visible');
                $('#headlineDaily').css('display', $this.totalRows < 1 ? 'none' : 'block');
                $('#headlineDaily').css('visibility', $this.totalRows < 1 ? 'hidden' : 'visible');
                $('#exporttoexcel').css('display', $this.totalRows < 1 ? 'none' : 'block');
                $('#msgNoinfo').css('display', $this.totalRows < 1 ? 'block' : 'none');

                if ($this.totalRows > 0) {
                    $this.Render(dateInfo);
                    $("#exporttoexcel").removeClass("hide");
                    $(".groupBack").removeClass("hide");
                } else {
                    $("#exporttoexcel").addClass("hide");
                    $(".groupBack").addClass("hide");
                }

                $('#netTurnover').bind('click', function () {
                    var message = "- For Sportbook, Net Turnover is Turnover which excludes the draw tickets and halves of the turnover of the half won, half lost tickets.";
                    if (data.tooltips.showAllbet) {
                        var messageGroup = message + "<br/>" + "<br/>" + "- For AllBet, Net Turnover is total valid bets that we use to compute commission.";
                        Fanex.ExtAcc.Form.Alert("Net Turnover Information", messageGroup, 430, 220);
                        return false;
                    }

                    Fanex.ExtAcc.Form.Alert("Net Turnover Information", message, 430, 170);
                    return false;
                });

                Fanex.ExtAcc.UnBlockUI();
            },
            error: function () {
                Fanex.ExtAcc.UnBlockUI();
            }
        });
    }
};

$(document).ready(function () {
    Fanex.ExtAcc.WinLossDetail.Init();
});