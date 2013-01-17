/// <reference path="../_references.js" />
if (typeof Fanex == 'undefined' || typeof Fanex.ExtAcc == 'undefined') {
    alert("Fanex.ExtAcc.js module is required!")
}

Fanex.ExtAcc.WinLossDetail = {
    theme: Fanex.ExtAcc.theme,
    leftMenuWidth: Fanex.ExtAcc.leftMenuWidth,
    gHeight: 200,
    separatedLineHeight: 15,
    gSize: { width: 500, height: 300 },
    totalRows: 0,
    totalRowsInPage: 50,
    groupName: '',
    username: '',
    srecommend: 0,
    mrecommend: 0,
    recommend: 0,
    custId: 0,
    supername: '',
    mastername: '',
    agentname: '',
    pageSize: 50,
    fromdate: new Date(),
    todate: new Date(),
    parameterProducts: '',
    isCheckAll: true,
    dataResult: new Array(),
    fixedsortdata: new Array(),
    removedsortdata: new Array(),
    fixedsortlist: new Array(),
    url: resolveClientUrl('WinLossDetail/GetData'),
    viewlevel: 5,
    bindingCompleted: true,
    dataSource: [],
    $this: null,
    fixedFrozen: 0,
    fixedFrozenFullHeight: 17,
    sumTurnover: 0,
    sumGrossComm: 0,
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
            $('#GroupFromToDate').css('display', 'none');
            $('#GroupMonthDay').css('display', 'block');
        }
        else {
            $('#GroupViewHistory').css('display', 'block');
            $('#GroupBackToCurrent').css('display', 'none');
            $('#GroupFromToDate').css('display', 'block');
            $('#GroupMonthDay').css('display', 'none');
        }
    },
    CallSubmit: function () {
        this.groupName = $('#selectedGroupName').html();
        this.ResetParameters();
        this.username = $("#txtUserName").val();
        this.Databind()
    },
    initMonthlyCalendar: function () {
        $('#monPickr1').monPickr({
            today: $('#MaxDate').val(),
            minDate: '2010/1/1',
            showDateRange: true,
            fromDayId: 'fromDay1',
            toDayId: 'toDay1',
            log: function (msg) {
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
                    fdate = new Date(tdate.getFullYear(), tdate.getMonth() - 2, 1);
                    tdate = new Date(tdate.getFullYear(), tdate.getMonth() - 1, 0);
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
            }, showOn: 'both', buttonImageOnly: true, buttonImage: '/Images/calendar.png', buttonText: '', beforeShow: $this.SetRange, dateFormat: 'M dd, yy'
        });
        $("#dtpFromDate").val($.datepicker.formatDate('M dd, yy', new Date($('#MaxDate').val())));

        $("#dtpToDate").datepicker({
            onClose: function (date) {
                $("#userGroup input").val($("#DateList")[0].options[7].value);
            }, showOn: 'both', buttonImageOnly: true, buttonImage: '/Images/calendar.png', buttonText: '', beforeShow: $this.SetRange, dateFormat: 'M dd, yy'
        });
        $("#dtpToDate").val($.datepicker.formatDate('M dd, yy', new Date($('#MaxDate').val())));

        $this.initMonthlyCalendar();

        this.gHeight = Math.round($("#footer").position().top - $("#mainGrid").position().top);

        $('#ddlGroupName').bind('change', function (event) {
            var args = event.args;
            if (args) {
                var item = args.item;
                $this.groupName = item.innerHTML;
                $("#txtUserName").val('');
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
            var params = "fromdate=" + $this.fromdate + "&todate=" + $this.todate + "&viewlevel=" + $this.viewlevel + '&custid=' + $this.custId + '&groupname=' + $this.groupName + '&username=' + $this.username;
            document.location.href = "WinLossDetail/GetExportToExcelWinLossDetail?" + params;
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
                $this.srecommend = 0;
                $("#txtUserName").val('');
                $this.username = '';
                $this.ToggleTabs('none', 'none', 'none', 4);
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
                $("#txtUserName").val('');
                $this.username = '';
                $this.ToggleTabs('block', 'none', 'none', 3);
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
                $("#txtUserName").val('');
                $this.username = '';
                $this.ToggleTabs('block', 'block', 'none', 2);
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

        Fanex.ExtAcc.OnResize = function () {
            $this.leftMenuWidth = Fanex.ExtAcc.leftMenuWidth;
            $this.Resize(true);
        }
    },
    ToggleTabs: function (statusSAg, statusMAg, statusAg, status) {
        $('#tabSuper').css('display', statusSAg);
        $('#tabMaster').css('display', statusMAg);
        $('#tabAgent').css('display', statusAg);
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
    VAlignColumns: function (gridId, colsIndex) {
    },
    BorderRightColumn: function (gridId, colIdx) {
    },
    GroupColums: function (gridId, colText, colsIndex) {
    },

    RenderUsernameColumn: function (value, model, row) {
        if (row == null || value == 'Grand Total' || $this.viewlevel < 2) {
            return '<div style="line-height: 22px; text-align: left; padding-left: 3px">' + value + '</div>';
        }
        else {
            var func = "javascript: Fanex.ExtAcc.WinLossDetail.GetDownlineWinLossDetail('" + row.CustId + "')";
            return html = '<div style="line-height: 22px; text-align: left;"><a style="text-decoration: none;margin:0" href="' + func + '">' + value + '</a></div>';
        }
    },
    RenderValueNum: function (value) {
        var classNum = "posNum";
        if (value < 0) {
            classNum = "negNum";
        }
        return '<div class="' + classNum + '">' + numeral(value).format('0,0.00') + '</div>';
    },
    defaultColumns: [
        { name: '', width: 0, hidden: true, frozen: true, fixed: true },
        { name: 'CustId', width: 0, hidden: true, frozen: true, fixed: true },
        { name: 'Username', index: 'Username', width: 100, resizable: false, sorttype: "text", frozen: true, formatter: function (value, model, row) { return $this.RenderUsernameColumn(value, model, row); }, fixed: true },
        { name: 'Turnover', index: 'Turnover', width: 110, resizable: false, sorttype: "float", frozen: true, formatter: function (value) { return $this.RenderValueNum(value); }, fixed: true },
        { name: 'GrossComm', index: 'GrossComm', width: 110, resizable: false, sorttype: "float", frozen: true, formatter: function (value) { return $this.RenderValueNum(value); }, fixed: true },
        { name: 'MemberWinlost', index: 'MemberWinlost', width: 110, resizable: false, sorttype: "float", formatter: function (value) { return $this.RenderValueNum(value); }, fixed: true },
        { name: 'PlayerComm', index: 'PlayerComm', width: 110, resizable: false, sorttype: "float", formatter: function (value) { return $this.RenderValueNum(value); }, fixed: true },
        { name: 'MemberTotal', index: 'MemberTotal', width: 110, resizable: false, sorttype: "float", formatter: function (value) { return $this.RenderValueNum(value); }, fixed: true },
        { name: 'AgWinlost', index: 'AgWinlost', width: 110, resizable: false, sorttype: "float", formatter: function (value) { return $this.RenderValueNum(value); }, fixed: true },
        { name: 'AgComm', index: 'AgComm', width: 110, resizable: false, sorttype: "float", formatter: function (value) { return $this.RenderValueNum(value); }, fixed: true },
        { name: 'AgTotal', index: 'AgTotal', width: 110, resizable: false, sorttype: "float", formatter: function (value) { return $this.RenderValueNum(value); }, fixed: true },
        { name: 'MAgWinlost', index: 'MAgWinlost', width: 110, resizable: false, sorttype: "float", formatter: function (value) { return $this.RenderValueNum(value); }, fixed: true },
        { name: 'MAgComm', index: 'MAgComm', width: 110, resizable: false, sorttype: "float", formatter: function (value) { return $this.RenderValueNum(value); }, fixed: true },
        { name: 'MAgTotal', index: 'MAgTotal', width: 110, resizable: false, sorttype: "float", formatter: function (value) { return $this.RenderValueNum(value); }, fixed: true },
        { name: 'SAgWinlost', index: 'SAgWinlost', width: 110, resizable: false, sorttype: "float", formatter: function (value) { return $this.RenderValueNum(value); }, fixed: true },
        { name: 'SAgComm', index: 'SAgComm', width: 110, resizable: false, sorttype: "float", formatter: function (value) { return $this.RenderValueNum(value); }, fixed: true },
        { name: 'SAgTotal', index: 'SAgTotal', width: 110, resizable: false, sorttype: "float", formatter: function (value) { return $this.RenderValueNum(value); }, fixed: true },
        { name: 'Company', index: 'Company', width: 110, resizable: false, sorttype: "float", formatter: function (value) { return $this.RenderValueNum(value); }, fixed: true }
        ],
    MAgColumns: [
        { name: '', width: 0, hidden: true, frozen: true, fixed: true },
        { name: 'CustId', width: 0, hidden: true, frozen: true, fixed: true },
        { name: 'Username', index: 'Username', width: 100, resizable: false, sorttype: "text", frozen: true, formatter: function (value, model, row) { return $this.RenderUsernameColumn(value, model, row); }, fixed: true },
        { name: 'Turnover', index: 'Turnover', width: 110, resizable: false, sorttype: "float", frozen: true, formatter: function (value) { return $this.RenderValueNum(value); }, fixed: true },
        { name: 'GrossComm', index: 'GrossComm', width: 110, resizable: false, sorttype: "float", frozen: true, formatter: function (value) { return $this.RenderValueNum(value); }, fixed: true },
        { name: 'MemberWinlost', index: 'MemberWinlost', width: 110, resizable: false, sorttype: "float", formatter: function (value) { return $this.RenderValueNum(value); }, fixed: true },
        { name: 'PlayerComm', index: 'PlayerComm', width: 110, resizable: false, sorttype: "float", formatter: function (value) { return $this.RenderValueNum(value); }, fixed: true },
        { name: 'MemberTotal', index: 'MemberTotal', width: 110, resizable: false, sorttype: "float", formatter: function (value) { return $this.RenderValueNum(value); }, fixed: true },
        { name: 'AgWinlost', index: 'AgWinlost', width: 110, resizable: false, sorttype: "float", formatter: function (value) { return $this.RenderValueNum(value); }, fixed: true },
        { name: 'AgComm', index: 'AgComm', width: 110, resizable: false, sorttype: "float", formatter: function (value) { return $this.RenderValueNum(value); }, fixed: true },
        { name: 'AgTotal', index: 'AgTotal', width: 110, resizable: false, sorttype: "float", formatter: function (value) { return $this.RenderValueNum(value); }, fixed: true },
        { name: 'MAgWinlost', index: 'MAgWinlost', width: 110, resizable: false, sorttype: "float", formatter: function (value) { return $this.RenderValueNum(value); }, fixed: true },
        { name: 'MAgComm', index: 'MAgComm', width: 110, resizable: false, sorttype: "float", formatter: function (value) { return $this.RenderValueNum(value); }, fixed: true },
        { name: 'MAgTotal', index: 'MAgTotal', width: 110, resizable: false, sorttype: "float", formatter: function (value) { return $this.RenderValueNum(value); }, fixed: true },
        { name: 'Company', index: 'Company', width: 110, resizable: false, sorttype: "float", formatter: function (value) { return $this.RenderValueNum(value); }, fixed: true }
        ],
    AgColumns: [
        { name: '', width: 0, hidden: true, frozen: true, fixed: true },
        { name: 'CustId', width: 0, hidden: true, frozen: true, fixed: true },
        { name: 'Username', index: 'Username', width: 100, resizable: false, sorttype: "string", frozen: true, formatter: function (value, model, row) { return $this.RenderUsernameColumn(value, model, row); }, fixed: true },
        { name: 'Turnover', index: 'Turnover', width: 110, resizable: false, sorttype: "float", frozen: true, formatter: function (value) { return $this.RenderValueNum(value); }, fixed: true },
        { name: 'GrossComm', index: 'GrossComm', width: 110, resizable: false, sorttype: "float", frozen: true, formatter: function (value) { return $this.RenderValueNum(value); }, fixed: true },
        { name: 'MemberWinlost', index: 'MemberWinlost', width: 110, resizable: false, sorttype: "float", formatter: function (value) { return $this.RenderValueNum(value); }, fixed: true },
        { name: 'PlayerComm', index: 'PlayerComm', width: 110, resizable: false, sorttype: "float", formatter: function (value) { return $this.RenderValueNum(value); }, fixed: true },
        { name: 'MemberTotal', index: 'MemberTotal', width: 110, resizable: false, sorttype: "float", formatter: function (value) { return $this.RenderValueNum(value); }, fixed: true },
        { name: 'AgWinlost', index: 'AgWinlost', width: 110, resizable: false, sorttype: "float", formatter: function (value) { return $this.RenderValueNum(value); }, fixed: true },
        { name: 'AgComm', index: 'AgComm', width: 110, resizable: false, sorttype: "float", formatter: function (value) { return $this.RenderValueNum(value); }, fixed: true },
        { name: 'AgTotal', index: 'AgTotal', width: 110, resizable: false, sorttype: "float", formatter: function (value) { return $this.RenderValueNum(value); }, fixed: true },
        { name: 'Company', index: 'Company', width: 110, resizable: false, sorttype: "float", formatter: function (value) { return $this.RenderValueNum(value); }, fixed: true }
        ],
    Render: function () {
        // Init Grid jqgrid
        $('#gbox_jqgrid').remove();
        $('#jqgrid').remove();
        $('#jqpager').remove();
        $('#mainGrid').append($('<table id="jqgrid" class="removeRadius"></table><div id="jqpager"></div>'));

        var arrColData = new Array();
        var arrColNames = new Array();
        var arrGroupHeaders = new Array();

        if (this.viewlevel == 1) {
            arrColData = this.AgColumns;
            arrColNames = ['', '', 'Username', 'Turnover', 'GrossComm', 'Win/Loss', 'Comm', 'Total', 'Win/Loss', 'Comm', 'Total', 'Company'];
            arrGroupHeaders = [
	                { startColumnName: 'MemberWinlost', numberOfColumns: 3, titleText: 'Member' },
                    { startColumnName: 'AgWinlost', numberOfColumns: 3, titleText: 'Agent' }
                ];
        } else if (this.viewlevel == 2) {
            arrColData = this.MAgColumns;
            arrColNames = ['', '', 'Username', 'Turnover', 'GrossComm', 'Win/Loss', 'Comm', 'Total', 'Win/Loss', 'Comm', 'Total', 'Win/Loss', 'Comm', 'Total', 'Company'];
            arrGroupHeaders = [
	                { startColumnName: 'MemberWinlost', numberOfColumns: 3, titleText: 'Member' },
                    { startColumnName: 'AgWinlost', numberOfColumns: 3, titleText: 'Agent' },
                    { startColumnName: 'MAgWinlost', numberOfColumns: 3, titleText: 'Master' }
                ];
        } else {
            arrColData = this.defaultColumns;
            arrColNames = ['', '', 'Username', 'Turnover', 'GrossComm', 'Win/Loss', 'Comm', 'Total', 'Win/Loss', 'Comm', 'Total', 'Win/Loss', 'Comm', 'Total', 'Win/Loss', 'Comm', 'Total', 'Company'];
            arrGroupHeaders = [
                                { startColumnName: 'MemberWinlost', numberOfColumns: 3, titleText: 'Member' },
                                { startColumnName: 'AgWinlost', numberOfColumns: 3, titleText: 'Agent' },
                                { startColumnName: 'MAgWinlost', numberOfColumns: 3, titleText: 'Master' },
            	                { startColumnName: 'SAgWinlost', numberOfColumns: 3, titleText: 'Super' }
                            ];
        }

        // Render with jqGrid
        $('#jqgrid').jqGrid({
            datatype: "local",
            width: this.gSize.width,
            height: '100%',
            pager: '#jqpager',
            rowNum: this.pageSize,
            rowList: [50, 100, 500],
            colNames: arrColNames,
            colModel: arrColData,
            hoverrows: true,
            footerrow: true,
            userDataOnFooter: true,
            viewsortcols: 'vertical',
            beforeSelectRow: function (rowid, e) {
                return false;
            },
            onPaging: function () {
                if ($('#jqgrid').jqGrid('getGridParam', 'page') == $('#jqgrid').jqGrid('getGridParam', 'total')) {
                    $this.totalRowsInPage = $this.totalRows % $this.pageSize;
                }
                else {
                    $this.totalRowsInPage = $this.pageSize;
                }
                $this.CalculateHeight();

                // For Fixed Column Footer
                var topFooterPosition = $this.gHeight - $this.fixedFrozen + 21;
                $('#frozenFooter').remove();
                $('#jqpager').before('<div id="frozenFooter" style="top:' + topFooterPosition + 'px;" class="ui-jqgrid-bdiv frozen-div fixedColumnFooter"><table cellspacing="0" cellpadding="0" border="0" role="grid" class="ui-jqgrid-ftable"><tbody><tr role="row" class="ui-widget-content footrow footrow-ltr"><td role="gridcell" style="display:none;" aria-describedby="jqgrid_"></td><td title="Grand Total" role="gridcell" style="width: 100px;" aria-describedby="jqgrid_Username"><div class="grandTotal">Grand Total</div></td><td title="' + numeral($this.sumTurnover).format('0,0.00') + '" role="gridcell" style="width: 110px;" aria-describedby="jqgrid_Turnover"><div class="posNum">' + numeral($this.sumTurnover).format('0,0.00') + '</div></td><td title="' + numeral($this.sumGrossComm).format('0,0.00') + '" role="gridcell" style="width: 110px;" aria-describedby="jqgrid_GrossComm"><div class="posNum">' + numeral($this.sumGrossComm).format('0,0.00') + '</div></td></tr></tbody></table></div>');
            },
            previousSelectedTh: null,
            onSortCol: function (iCol, index, sortorder) {
                this.previousSelectedTh = (this.previousSelectedTh != null) ? this.previousSelectedTh : $("tr.ui-jqgrid-labels:last th:eq(" + this.p.lastsort + ")", this.grid.fhDiv);
                var newSelectedTh = $("tr.jqg-second-row-header th:eq(" + index + ")", this.grid.fhDiv);

                // sort on another column
                if (index != this.p.lastsort) {
                    $("span.ui-grid-ico-sort", this.previousSelectedTh).addClass('ui-state-disabled');
                    $(this.previousSelectedTh).attr("aria-selected", "false");
                }
                else { //sort on same column
                    var prevSortOrder = (sortorder == "asc") ? "desc" : "asc";
                    $("span.ui-icon-" + prevSortOrder, newSelectedTh).addClass('ui-state-disabled');
                }

                // enable sort order icon
                $("span.ui-icon-" + sortorder, newSelectedTh).removeClass('ui-state-disabled');
                $(newSelectedTh).attr("aria-selected", "true");

                this.previousSelectedTh = newSelectedTh;
            },
            loadComplete: function () {
                $("tr.jqgrow").css("background", "#ffffff");
            }
        });

        $('#jqgrid').jqGrid('setGroupHeaders', {
            useColSpanStyle: true,
            groupHeaders: arrGroupHeaders
        });

        $('#jqgrid').jqGrid('setFrozenColumns');

        $('#jqgrid').clearGridData(true);

        $this.sumTurnover = 0;
        $this.sumGrossComm = 0;
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

        for (var i = 0; i < this.totalRows; i++) {
            $('#jqgrid').jqGrid('addRowData', i + 1, this.dataSource[i]);
            $this.sumTurnover += this.dataSource[i].Turnover;
            $this.sumGrossComm += this.dataSource[i].GrossComm;
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

        if (this.viewlevel == 1) {
            $('#jqgrid').jqGrid('footerData', 'set', { Username: 'Grand Total', Turnover: $this.sumTurnover, GrossComm: $this.sumGrossComm
                        , MemberWinlost: sumMemberWinlost, PlayerComm: sumPlayerComm, MemberTotal: sumMemberTotal
                        , AgWinlost: sumAgWinlost, AgComm: sumAgComm, AgTotal: sumAgTotal
                        , Company: sumCompany
            });
        } else if (this.viewlevel == 2) {
            $('#jqgrid').jqGrid('footerData', 'set', { Username: 'Grand Total', Turnover: $this.sumTurnover, GrossComm: $this.sumGrossComm
                        , MemberWinlost: sumMemberWinlost, PlayerComm: sumPlayerComm, MemberTotal: sumMemberTotal
                        , AgWinlost: sumAgWinlost, AgComm: sumAgComm, AgTotal: sumAgTotal
                        , MAgWinlost: sumMAgWinlost, MAgComm: sumMAgComm, MAgTotal: sumMAgTotal
                        , Company: sumCompany
            });
        }
        else {
            $('#jqgrid').jqGrid('footerData', 'set', { Username: 'Grand Total', Turnover: $this.sumTurnover, GrossComm: $this.sumGrossComm
                        , MemberWinlost: sumMemberWinlost, PlayerComm: sumPlayerComm, MemberTotal: sumMemberTotal
                        , AgWinlost: sumAgWinlost, AgComm: sumAgComm, AgTotal: sumAgTotal
                        , MAgWinlost: sumMAgWinlost, MAgComm: sumMAgComm, MAgTotal: sumMAgTotal
                        , SAgWinlost: sumSAgWinlost, SAgComm: sumSAgComm, SAgTotal: sumSAgTotal
                        , Company: sumCompany
            });
        }

        $("#jqgrid").jqGrid('setGridParam', { rowNum: $this.pageSize, page: 1, total: parseInt($this.totalRows / $this.pageSize + 1) }).trigger("reloadGrid");

        if (this.totalRows < this.pageSize) {
            this.totalRowsInPage = this.totalRows;
            $('#jqpager').hide();
        }
        else {
            this.totalRowsInPage = this.pageSize;
            $('#jqpager').show();
        }

        $('#mainGrid').css('visibility', 'visible');
        this.Resize(true);
    },
    CalculateHeight: function () {
        this.gHeight = Math.round($("#footer").position().top - $("#mainGrid").position().top) - 75;
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
            for (var i = 0; i < this.defaultColumns.length; i++) {
                this.totalWidth += this.defaultColumns[i].width;
            }

            var width = this.width = $("#footer").position().left - this.leftMenuWidth - 8;
            var height = this.gHeight - (width > this.totalWidth ? 16 : 0);

            if (this.gSize && this.gSize.width == width && this.gSize.height == height) {
            }
            else {
                if (autoResizeGrid && this.gSize.width != width) {
                    $("#jqgrid").setGridWidth(width, false);
                }
                if (autoResizeGrid && this.gSize.height != height) {
                    $("#jqgrid").setGridHeight(height);
                }
                this.gSize = { width: width, height: height };
            }

            // For Frozen
            $(".ui-jqgrid-bdiv").css('height', ($this.gHeight - $this.fixedFrozen - 25) + 'px');

            // For Fixed Column Footer
            var topFooterPosition = $this.gHeight - $this.fixedFrozen + 21;
            $('#frozenFooter').remove();
            $('#jqpager').before('<div id="frozenFooter" style="top:' + topFooterPosition + 'px;" class="ui-jqgrid-bdiv frozen-div fixedColumnFooter" style="overflow:hidden"><table cellspacing="0" cellpadding="0" border="0" role="grid" class="ui-jqgrid-ftable"><tbody><tr role="row" class="ui-widget-content footrow footrow-ltr"><td role="gridcell" style="display:none;" aria-describedby="jqgrid_"></td><td title="Grand Total" role="gridcell" style="width: 100px;" aria-describedby="jqgrid_Username"><div class="grandTotal">Grand Total</div></td><td title="' + numeral($this.sumTurnover).format('0,0.00') + '" role="gridcell" style="width: 110px;" aria-describedby="jqgrid_Turnover"><div class="posNum">' + numeral($this.sumTurnover).format('0,0.00') + '</div></td><td title="' + numeral($this.sumGrossComm).format('0,0.00') + '" role="gridcell" style="width: 110px;" aria-describedby="jqgrid_GrossComm"><div class="posNum">' + numeral($this.sumGrossComm).format('0,0.00') + '</div></td></tr></tbody></table></div>');

            var moreFooterWidth = this.fixedFrozen != this.fixedFrozenFullHeight ? 0 : 17;
            $(".ui-jqgrid-sdiv").css('width', $("#gview_jqgrid").width() - moreFooterWidth + 'px');
            $($("#gview_jqgrid").children()[1]).css('width', $("#gview_jqgrid").width() - moreFooterWidth + 'px');
        } catch (e) { }
    },
    ChangeHeadlineTab: function () {
        if (this.viewlevel >= 4) {
            this.srecommend = this.custId;
            this.ToggleTabs('none', 'none', 'none', 4);
        } else if (this.viewlevel == 3) {
            this.mrecommend = this.custId;
            this.ToggleTabs('block', 'none', 'none', 3);
            $("#tabSuper").attr("class", "tabActiveSuper");
        } else if (this.viewlevel == 2) {
            this.recommend = this.custId;
            this.ToggleTabs('block', 'block', 'none', 2);
            $("#tabSuper").attr("class", "tabDisableSuper");
            $("#tabMaster").attr("class", "tabActive");
        } else {
            this.ToggleTabs('block', 'block', 'block', 1);
            $("#tabSuper").attr("class", "tabDisableSuper");
            $("#tabMaster").attr("class", "tabDisable");
            $("#tabAgent").attr("class", "tabActive");
        };
        $('#headlineDaily').css("visibility", "visible")
    },
    GetDownlineWinLossDetail: function (custId) {
        this.custId = custId;
        $("#txtUserName").val('');
        this.username = '';
        this.Databind();
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
        this.username == '';
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
    customsortfunc: function (column, direction) {
        $this = Fanex.ExtAcc.WinLossDetail;
        if (direction == 'ascending') direction = true;
        if (direction == 'descending') direction = false;
        var tmpToString = Object.prototype.toString;
        Object.prototype.toString = (typeof column == "function") ? column : function () { return this[column] };
        if (direction != null) {
            var countFixedSort = $this.fixedsortlist.length;
            if (countFixedSort > 0) {
                $this.dataSource.sort($this.compare);
                if (!direction) {
                    $this.dataSource.reverse();
                }

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
            else {
                $this.dataSource.sort($this.compare);
                if (!direction) {
                    $this.dataSource.reverse();
                }
            }
        }

        $("#statusbarjqxgrid").css("visibility", "hidden");
        $this.dataSource.localdata = $this.dataSource;
        $this.GroupAllColumns();

        setTimeout(function () {
            $("#statusbarjqxgrid").css("visibility", "visible");
            $("#statusbarjqxgrid").css("z-index", "1000");
        }, 100);
    },
    GroupAllColumns: function () {
    },
    compare: function (value1, value2) {
        value1 = String(value1).toLowerCase();
        value2 = String(value2).toLowerCase();
        try {
            var tmpvalue1 = parseFloat(value1);
            if (isNaN(tmpvalue1)) {
                if (value1 < value2) { return -1; }
                if (value1 > value2) { return 1; }
            }
            else {
                var tmpvalue2 = parseFloat(value2);
                if (tmpvalue1 < tmpvalue2) { return -1; }
                if (tmpvalue1 > tmpvalue2) { return 1; }
            }
        }
        catch (error) {
            var er = error;
        }
        return 0;
    },
    Databind: function () {
        if (!this.bindingCompleted) return;
        this.bindingCompleted = false;
        if ($("#txtUserName").val() != '') {
            this.username = $("#txtUserName").val();
        }
        this.GetFromDateToDate();
        this.groupName = $('#selectedGroupName').html();

        if (this.fromdate == this.todate) {
            $("#DateValueInfo").html('on ' + this.fromdate);
        }
        else {
            $("#DateValueInfo").html('from ' + (new Date(Date.parse(this.fromdate))).toStringFormat('MMM dd, yyyy') + ' to ' + (new Date(Date.parse(this.todate))).toStringFormat('MMM dd, yyyy'));
        }

        $this.getParameterProducts();
        var parameterUrl = "?groupname=" + this.groupName
                            + "&srecommend=" + this.srecommend
                            + "&mrecommend=" + this.mrecommend
                            + "&recommend=" + this.recommend
                            + "&custid=" + this.custId
                            + "&username=" + this.username
                            + "&fromdate=" + this.fromdate
                            + "&todate=" + this.todate
                            + "&viewlevel=" + this.viewlevel
                            + this.parameterProducts;

        $.blockUI({ message: '<img src="../../Images/Loading.gif" />' });

        $.ajax({
            context: $this,
            url: this.url + parameterUrl,
            type: 'GET',
            dataType: 'json',
            cache: false,
            beforesend: function (xhr) { },
            success: function (data) {
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

                $('#headlineDaily').css('display', $this.totalRows < 1 ? 'none' : 'block');
                $('#btnHideMG').css('display', $this.totalRows < 1 ? 'none' : 'block');
                $('#mainGrid').css('display', $this.totalRows < 1 ? 'none' : 'block');
                $('#exporttoexcel').css('display', $this.totalRows < 1 ? 'none' : 'block');
                $('#msgNoinfo').css('display', $this.totalRows < 1 ? 'block' : 'none');

                if ($this.totalRows > 0) {
                    $this.Render();
                }

                $this.bindingCompleted = true;
                $(document).ajaxStop($.unblockUI);
            },
            error: function () {
                $this.bindingCompleted = true;
                $(document).ajaxStop($.unblockUI);
            }
        })
    }
};