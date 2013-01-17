/// <reference path="../_references.js" />

if (typeof Fanex == 'undefined' || typeof Fanex.ExtAcc == 'undefined') {
    alert("Fanex.ExtAcc.js module is required!")
}

Fanex.ExtAcc.DailyWinLoss = {
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
    date: new Date(),
    url: resolveClientUrl('DailyWinLoss/GetDailyWinLoss'),
    viewlevel: 5,
    bindingCompleted: true,
    hidedMargin: false,
    dataSource: [],
    $this: null,
    fixedFrozen: 0,
    fixedFrozenFullHeight: 17,
    SumTotalTO: 0,
    SumComTO: 0,
    SumSAgTO: 0,
    SumMAgTO: 0,
    SumAgTO: 0,
    SumComWinlost: 0,
    SumSAgWinlost: 0,
    SumMAgWinlost: 0,
    SumAgWinlost: 0,
    SetRange: function () {
        return {
            minDate: new Date($('#BalanceUpDay').val()),
            maxDate: new Date()
        }
    },
    CallSubmit: function () {
        this.groupName = $('#selectedGroupName').html();
        this.ResetParameters();
        this.username = $("#txtUserName").val();
        this.Databind();
    },

    Init: function () {
        $this = this;

        Fanex.ExtAcc.InitSuggestion();

        $('#dtpWinLossDate').datepicker({ showOn: 'both', buttonImageOnly: true, buttonImage: '/Images/calendar.png', buttonText: '', beforeShow: $this.SetRange, dateFormat: 'M dd, yy' });
        $("#dtpWinLossDate").val($.datepicker.formatDate('M dd, yy', new Date()));

        this.gHeight = Math.round($("#footer").position().top - $("#mainGrid").position().top);

        $('#Submit').click(function (e) {
            $this.CallSubmit();
        });
        $('#exporttoexcel').click(function () {
            var params = "date=" + $this.date + "&viewlevel=" + $this.viewlevel + '&custid=' + $this.custId + '&groupname=' + $this.groupName + '&username=' + $this.username;
            document.location.href = resolveClientUrl("DailyWinLoss/GetExportToExcel?") + params;
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
        $('#btnHideMG').click(function (e) {
            if (!$this.hidedMargin) {
                $('#btnHideMG').val('Show Margin');
                $this.hidedMargin = true;
            } else {
                $('#btnHideMG').val('Hide Margin');
                $this.hidedMargin = false;
            }
            $this.Render();
            //$("#jqgrid").setGridWidth($this.gSize.width, false);
        });

        $this.groupName = $('#selectedGroupName').html();
        var source = [];

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

        $('#txtUserName').keyup(function (e) {
            if ($('#txtUserName').val().length > 1) {
                Fanex.ExtAcc.InitSuggestion();
            }
        });

        Fanex.ExtAcc.OnResize = function () {
            $this.leftMenuWidth = Fanex.ExtAcc.leftMenuWidth;
            $this.Resize(true)
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
    GetDownlineDailyWinLoss: function (custId) {
        this.custId = custId;
        $("#txtUserName").val('');
        this.username = '';
        this.Databind();
    },
    InitShowHideControls: function (status) {
        $("#headlineDaily").css('display', status);
        $("#mainGrid").css('display', status);
        $("#btnHideMG").css('display', status);
    },
    ResetParameters: function () {
        this.srecommend = 0;
        this.mrecommend = 0;
        this.recommend = 0;
        this.custId = 0;
        this.username == '';
        this.viewlevel = 4;
    },
    CalculatePercentGrandTotal: function (value1, value2) {
        value1 = String(value1).replace(/,/g, '');
        value2 = String(value2).replace(/,/g, '');
        if (value2 != 0 && value2 != '0.00') {
            return numeral(parseFloat(value1) / parseFloat(value2) * 100).format('0,0.00');
        }
        else {
            return '0.00';
        }
    },
    RenderUsernameColumn: function (value, model, row) {
        if (row == null || value == 'Grand Total' || $this.viewlevel < 2) {
            return '<div style="line-height: 22px; text-align: left; padding-left: 3px">' + value + '</div>';
        }
        else {
            var func = "javascript: Fanex.ExtAcc.DailyWinLoss.GetDownlineDailyWinLoss('" + row.CustId + "')";
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
        { name: 'No', index: 'No', width: 40, resizable: false, sorttype: "float", align: "right", frozen: true, fixed: true, sortable: false },
        { name: 'CustId', width: 0, hidden: true, frozen: true, fixed: true },
        { name: 'Username', index: 'Username', width: 100, resizable: false, sorttype: "string", frozen: true, formatter: function (value, model, row) { return $this.RenderUsernameColumn(value, model, row); }, fixed: true, sortable: false },
        { name: 'TotalTO', index: 'TotalTO', width: 110, resizable: false, sorttype: "float", frozen: true, formatter: function (value) { return $this.RenderValueNum(value); }, fixed: true, sortable: false },
        { name: 'ComTO', index: 'ComTO', width: 110, resizable: false, sorttype: "float", formatter: function (value) { return $this.RenderValueNum(value); }, fixed: true, sortable: false },
        { name: 'ComTOPercent', index: 'ComTOPercent', width: 45, resizable: false, sorttype: "float", formatter: function (value) { return $this.RenderValueNum(value); }, fixed: true, sortable: false },
        { name: 'SAgTO', index: 'SAgTO', width: 110, resizable: false, sorttype: "float", formatter: function (value) { return $this.RenderValueNum(value); }, fixed: true, sortable: false },
        { name: 'SAgTOPercent', index: 'SAgTOPercent', width: 45, resizable: false, sorttype: "float", formatter: function (value) { return $this.RenderValueNum(value); }, fixed: true, sortable: false },
        { name: 'MAgTO', index: 'MAgTO', width: 110, resizable: false, sorttype: "float", formatter: function (value) { return $this.RenderValueNum(value); }, fixed: true, sortable: false },
        { name: 'MAgTOPercent', index: 'MAgTOPercent', width: 45, resizable: false, sorttype: "float", formatter: function (value) { return $this.RenderValueNum(value); }, fixed: true, sortable: false },
        { name: 'AgTO', index: 'AgTO', width: 110, resizable: false, sorttype: "float", formatter: function (value) { return $this.RenderValueNum(value); }, fixed: true, sortable: false },
        { name: 'AgTOPercent', index: 'AgTOPercent', width: 45, resizable: false, sorttype: "float", formatter: function (value) { return $this.RenderValueNum(value); }, fixed: true, sortable: false },
        { name: 'ComWinlost', index: 'ComWinlost', width: 110, resizable: false, sorttype: "float", formatter: function (value) { return $this.RenderValueNum(value); }, fixed: true, sortable: false },
        { name: 'ComWinlostPercent', index: 'ComWinlostPercent', width: 45, resizable: false, sorttype: "float", formatter: function (value) { return $this.RenderValueNum(value); }, fixed: true, sortable: false },
        { name: 'SAgWinlost', index: 'SAgWinlost', width: 110, resizable: false, sorttype: "float", formatter: function (value) { return $this.RenderValueNum(value); }, fixed: true, sortable: false },
        { name: 'SAgWinlostPercent', index: 'SAgWinlostPercent', width: 45, resizable: false, sorttype: "float", formatter: function (value) { return $this.RenderValueNum(value); }, fixed: true, sortable: false },
        { name: 'MAgWinlost', index: 'MAgWinlost', width: 110, resizable: false, sorttype: "float", formatter: function (value) { return $this.RenderValueNum(value); }, fixed: true, sortable: false },
        { name: 'MAgWinlostPercent', index: 'MAgWinlostPercent', width: 45, resizable: false, sorttype: "float", formatter: function (value) { return $this.RenderValueNum(value); }, fixed: true, sortable: false },
        { name: 'AgWinlost', index: 'AgWinlost', width: 110, resizable: false, sorttype: "float", formatter: function (value) { return $this.RenderValueNum(value); }, fixed: true, sortable: false },
        { name: 'AgWinlostPercent', index: 'AgWinlostPercent', width: 45, resizable: false, sorttype: "float", formatter: function (value) { return $this.RenderValueNum(value); }, fixed: true, sortable: false }
    ],
    hideMarginColumns: [
        { name: 'No', index: 'No', width: 40, resizable: false, sorttype: "float", align: "right", frozen: true, fixed: true, sortable: false },
        { name: 'CustId', width: 0, hidden: true, frozen: true, fixed: true },
        { name: 'Username', index: 'Username', width: 100, resizable: false, sorttype: "string", frozen: true, formatter: function (value, model, row) { return $this.RenderUsernameColumn(value, model, row); }, fixed: true, sortable: false },
        { name: 'TotalTO', index: 'TotalTO', width: 110, resizable: false, sorttype: "float", frozen: true, formatter: function (value) { return $this.RenderValueNum(value); }, fixed: true, sortable: false },
        { name: 'ComTO', index: 'ComTO', width: 110, resizable: false, sorttype: "float", formatter: function (value) { return $this.RenderValueNum(value); }, fixed: true, sortable: false },
        { name: 'ComTOPercent', index: 'ComTOPercent', width: 45, resizable: false, sorttype: "float", formatter: function (value) { return $this.RenderValueNum(value); }, fixed: true, sortable: false },
        { name: 'SAgTO', index: 'SAgTO', width: 110, resizable: false, sorttype: "float", formatter: function (value) { return $this.RenderValueNum(value); }, fixed: true, sortable: false },
        { name: 'SAgTOPercent', index: 'SAgTOPercent', width: 45, resizable: false, sorttype: "float", formatter: function (value) { return $this.RenderValueNum(value); }, fixed: true, sortable: false },
        { name: 'MAgTO', index: 'MAgTO', width: 110, resizable: false, sorttype: "float", formatter: function (value) { return $this.RenderValueNum(value); }, fixed: true, sortable: false },
        { name: 'MAgTOPercent', index: 'MAgTOPercent', width: 45, resizable: false, sorttype: "float", formatter: function (value) { return $this.RenderValueNum(value); }, fixed: true, sortable: false },
        { name: 'AgTO', index: 'AgTO', width: 110, resizable: false, sorttype: "float", formatter: function (value) { return $this.RenderValueNum(value); }, fixed: true, sortable: false },
        { name: 'AgTOPercent', index: 'AgTOPercent', width: 45, resizable: false, sorttype: "float", formatter: function (value) { return $this.RenderValueNum(value); }, fixed: true, sortable: false },
        { name: 'ComWinlost', index: 'ComWinlost', width: 110, resizable: false, sorttype: "float", formatter: function (value) { return $this.RenderValueNum(value); }, fixed: true, sortable: false },
        { name: 'SAgWinlost', index: 'SAgWinlost', width: 110, resizable: false, sorttype: "float", formatter: function (value) { return $this.RenderValueNum(value); }, fixed: true, sortable: false },
        { name: 'MAgWinlost', index: 'MAgWinlost', width: 110, resizable: false, sorttype: "float", formatter: function (value) { return $this.RenderValueNum(value); }, fixed: true, sortable: false },
        { name: 'AgWinlost', index: 'AgWinlost', width: 110, resizable: false, sorttype: "float", formatter: function (value) { return $this.RenderValueNum(value); }, fixed: true, sortable: false }
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

        if ($this.hidedMargin) {
            arrColData = this.hideMarginColumns;
            arrColNames = ['No.', '', 'Username', 'Total Turnover', 'Company', '%', 'Super', '%', 'Master', '%', 'Agent', '%', 'Company', 'Super', 'Master', 'Agent'];
            arrGroupHeaders = [
	                            { startColumnName: 'ComTO', numberOfColumns: 8, titleText: 'Turnover & Position Taking' },
	                            { startColumnName: 'ComWinlost', numberOfColumns: 4, titleText: '<div id="HeaderWinlossMargin">Win/Loss</div>' }
                            ];
        }
        else {
            arrColData = this.defaultColumns;
            arrColNames = ['No.', '', 'Username', 'Total Turnover', 'Company', '%', 'Super', '%', 'Master', '%', 'Agent', '%', 'Company', '%', 'Super', '%', 'Master', '%', 'Agent', '%'];
            arrGroupHeaders = [
	                            { startColumnName: 'ComTO', numberOfColumns: 8, titleText: 'Turnover & Position Taking' },
	                            { startColumnName: 'ComWinlost', numberOfColumns: 8, titleText: '<div id="HeaderWinlossMargin">Win/Loss & Margin</div>' }
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
                $('#jqpager').before('<div id="frozenFooter" style="top:' + topFooterPosition + 'px;" class="ui-jqgrid-bdiv frozen-div fixedColumnFooter"><table cellspacing="0" cellpadding="0" border="0" role="grid" class="ui-jqgrid-ftable"><tbody><tr role="row" class="ui-widget-content footrow footrow-ltr"><td role="gridcell" style="display:none;" aria-describedby="jqgrid_"></td><td title="Grand Total" role="gridcell" style="width: 40px;" aria-describedby="jqgrid_Username"></td><td title="Grand Total" role="gridcell" style="width: 100px;" aria-describedby="jqgrid_Username"><div class="grandTotal">Grand Total</div></td><td title="' + numeral($this.SumTotalTO).format('0,0.00') + '" role="gridcell" style="width: 110px;" aria-describedby="jqgrid_GrossComm"><div class="posNum">' + numeral($this.SumTotalTO).format('0,0.00') + '</div></td></tr></tbody></table></div>');
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
        this.SumTotalTO = 0;
        this.SumComTO = 0;
        this.SumSAgTO = 0;
        this.SumMAgTO = 0;
        this.SumAgTO = 0;
        this.SumComWinlost = 0;
        this.SumSAgWinlost = 0;
        this.SumMAgWinlost = 0;
        this.SumAgWinlost = 0;

        for (var i = 0; i < this.totalRows; i++) {
            $('#jqgrid').jqGrid('addRowData', i + 1, this.dataSource[i]);
            this.SumTotalTO += this.dataSource[i].TotalTO;
            this.SumComTO += this.dataSource[i].ComTO;
            this.SumSAgTO += this.dataSource[i].MAgTO;
            this.SumMAgTO += this.dataSource[i].SAgTO;
            this.SumAgTO += this.dataSource[i].AgTO;
            this.SumComWinlost += this.dataSource[i].ComWinlost;
            this.SumSAgWinlost += this.dataSource[i].SAgWinlost;
            this.SumMAgWinlost += this.dataSource[i].MAgWinlost;
            this.SumAgWinlost += this.dataSource[i].AgWinlost;
        }

        if ($this.hidedMargin) {
            $('#jqgrid').jqGrid('footerData', 'set', { Username: 'Grand Total', TotalTO: this.SumTotalTO
                        , ComTO: this.SumComTO, ComTOPercent: $this.CalculatePercentGrandTotal(this.SumComTO, this.SumTotalTO)
                        , SAgTO: this.SumSAgTO, SAgTOPercent: $this.CalculatePercentGrandTotal(this.SumSAgTO, this.SumTotalTO)
                        , MAgTO: this.SumMAgTO, MAgTOPercent: $this.CalculatePercentGrandTotal(this.SumMAgTO, this.SumTotalTO)
                        , AgTO: this.SumAgTO, AgTOPercent: $this.CalculatePercentGrandTotal(this.SumAgTO, this.SumTotalTO)

                        , ComWinlost: this.SumComTO
                        , SAgWinlost: this.SumSAgTO
                        , MAgWinlost: this.SumMAgTO
                        , AgWinlost: this.SumAgTO
            });
        }
        else {
            $('#jqgrid').jqGrid('footerData', 'set', { Username: 'Grand Total', TotalTO: this.SumTotalTO
                        , ComTO: this.SumComTO, ComTOPercent: $this.CalculatePercentGrandTotal(this.SumComTO, this.SumTotalTO)
                        , SAgTO: this.SumSAgTO, SAgTOPercent: $this.CalculatePercentGrandTotal(this.SumSAgTO, this.SumTotalTO)
                        , MAgTO: this.SumMAgTO, MAgTOPercent: $this.CalculatePercentGrandTotal(this.SumMAgTO, this.SumTotalTO)
                        , AgTO: this.SumAgTO, AgTOPercent: $this.CalculatePercentGrandTotal(this.SumAgTO, this.SumTotalTO)

                        , ComWinlost: this.SumComTO, ComWinlostPercent: $this.CalculatePercentGrandTotal(this.SumComWinlost, this.SumComTO)
                        , SAgWinlost: this.SumSAgTO, SAgWinlostPercent: $this.CalculatePercentGrandTotal(this.SumSAgWinlost, this.SumSAgTO)
                        , MAgWinlost: this.SumMAgTO, MAgWinlostPercent: $this.CalculatePercentGrandTotal(this.SumMAgWinlost, this.SumMAgTO)
                        , AgWinlost: this.SumAgTO, AgWinlostPercent: $this.CalculatePercentGrandTotal(this.SumAgWinlost, this.SumAgTO)
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
            $('#jqpager').before('<div id="frozenFooter" style="top:' + topFooterPosition + 'px;" class="ui-jqgrid-bdiv frozen-div fixedColumnFooter"><table cellspacing="0" cellpadding="0" border="0" role="grid" class="ui-jqgrid-ftable"><tbody><tr role="row" class="ui-widget-content footrow footrow-ltr"><td role="gridcell" style="display:none;" aria-describedby="jqgrid_"></td><td title="Grand Total" role="gridcell" style="width: 40px;" aria-describedby="jqgrid_Username"></td><td title="Grand Total" role="gridcell" style="width: 100px;" aria-describedby="jqgrid_Username"><div class="grandTotal">Grand Total</div></td><td title="' + numeral($this.SumTotalTO).format('0,0.00') + '" role="gridcell" style="width: 110px;" aria-describedby="jqgrid_GrossComm"><div class="posNum">' + numeral($this.SumTotalTO).format('0,0.00') + '</div></td></tr></tbody></table></div>');

            var moreFooterWidth = this.fixedFrozen != this.fixedFrozenFullHeight ? 0 : 17;
            $(".ui-jqgrid-sdiv").css('width', $("#gview_jqgrid").width() - moreFooterWidth + 'px');
            $($("#gview_jqgrid").children()[1]).css('width', $("#gview_jqgrid").width() - moreFooterWidth + 'px');
        } catch (e) { }
    },
    VAlignColumns: function (gridId, colsIndex) {
    },
    BorderRightColumn: function (gridId, colIdx) {
    },
    GroupColums: function (gridId, colText, colsIndex) {
    },
    Databind: function () {
        if (!this.bindingCompleted) return;
        this.bindingCompleted = false;

        if ($("#txtUserName").val() != '') {
            this.username = $("#txtUserName").val();
        }

        this.date = $('#dtpWinLossDate').val();
        $("#DateValueInfo").html(this.date);

        var parameterUrl = "?groupname=" + this.groupName
                            + "&srecommend=" + this.srecommend
                            + "&mrecommend=" + this.mrecommend
                            + "&recommend=" + this.recommend
                            + "&custid=" + this.custId
                            + "&username=" + this.username
                            + "&date=" + this.date
                            + "&viewlevel=" + this.viewlevel;

        $.blockUI({ message: '<img src="../../Images/Loading.gif" />' });

        $.ajax({
            context: $this,
            url: this.url + parameterUrl,
            type: 'GET',
            dataType: 'json',
            cache: false,
            beforesend: function (xhr) { },
            success: function (data) {
                $this.dataSource = data.rows;
                $this.totalRows = data.rows.length;

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

                ($this.totalRows > 0) && $this.Render();

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