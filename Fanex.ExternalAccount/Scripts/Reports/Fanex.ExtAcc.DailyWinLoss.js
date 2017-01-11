/// <reference path="../_references.js" />
if (typeof Fanex == 'undefined' || typeof Fanex.ExtAcc == 'undefined') {
    alert("Fanex.ExtAcc.js module is required!")
}

Fanex.ExtAcc.DailyWinLoss = {
    gridOptions: null,

    groupName: '',
    username: '',
    srecommend: 0,
    mrecommend: 0,
    recommend: 0,
    custId: 0,
    supername: '',
    mastername: '',
    agentname: '',
    date: new Date(),
    url: resolveClientUrl('DailyWinLoss/GetDailyWinLoss'),
    viewlevel: 5,
    hidedMargin: false,
    dataSource: [],
    $this: null,
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
        this.username = Fanex.encodeHtml($("#txtUserName").val());
        this.Databind();
    },
    Init: function () {
        $this = this;

        Fanex.ExtAcc.InitSuggestion();

        $('#dtpWinLossDate').datepicker({ showOn: 'both', buttonImageOnly: true, buttonImage: resolveClientUrl('Content/Images/calendar.png'), buttonText: '', beforeShow: $this.SetRange, dateFormat: 'M dd, yy' });
        $("#dtpWinLossDate").val($.datepicker.formatDate('M dd, yy', new Date()));

        $('#Submit').click(function (e) {
            $this.CallSubmit();
        });

        $('#exporttoexcel').click(function () {
            var params = "date=" + $this.date + "&viewlevel=" + ($this.viewlevel + 1) + '&custid=' + $this.custId + '&groupname=' + $this.groupName + '&username=' + $this.username;
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
                $this.srecommend = $("#txtUserName").val() == '' ? 0 : $this.srecommend;
                //$("#txtUserName").val('');
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
                //$("#txtUserName").val('');
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
                //$("#txtUserName").val('');
                $this.username = '';
                $this.ToggleTabs('block', 'block', 'none', 2);
                $this.Databind();
            }
        });

        $('#btnHideMG').click(function (e) {
            if (!$this.hidedMargin) {
                $('#btnHideMG').val('Show Margin');
                $this.hidedMargin = true;
                $("#jqgrid").fanexGrid("hideColumns", $this.gridOptions, [12, 14, 16, 18]);
                $('#HeaderWinlossMargin').html('Win/Loss');
            } else {
                $('#btnHideMG').val('Hide Margin');
                $this.hidedMargin = false;
                $("#jqgrid").fanexGrid("showColumns", $this.gridOptions, [12, 14, 16, 18]);
                $('#HeaderWinlossMargin').html('Win/Loss & Margin');
            }
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
            try {
                $("#jqgrid").fanexGrid("resizeGrid");
            }
            catch (err) { }
        };
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
        $('#headlineDaily').css("visibility", "visible");
        $('#headlineDaily').css("display", "block");
    },
    GetDownlineDailyWinLoss: function (custId) {
        this.custId = custId;
        //$("#txtUserName").val('');
        this.username = '';
        this.Databind();
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
    RenderUsernameColumn: function (cell) {
        if (cell == null || cell.Value.Username == 'Grand Total' || $this.viewlevel < 2) {
            return '<div style="line-height: 22px; text-align: left; padding-left: 3px">' + cell.Value.Username + '</div>';
        }
        else {
            var func = "javascript: Fanex.ExtAcc.DailyWinLoss.GetDownlineDailyWinLoss('" + cell.Value.CustId + "')";
            return html = '<div style="line-height: 22px; text-align: left;"><a style="text-decoration: none;margin:0" href="' + func + '">' + cell.Value.Username + '</a></div>';
        }
    },
    defaultColumns: [
        { HeaderText: 'No.', Name: 'No', Width: 40, HeaderAlign: 'center', CellAlign: 'right' },
        {
            HeaderText: 'Username', Name: 'Username', Width: 130, HeaderAlign: 'center',
            CustomCellRender: function (cell) { return $this.RenderUsernameColumn(cell); }
        },
        { HeaderText: 'Total Turnover', Name: 'TotalTO', Width: 130, HeaderAlign: 'center', CellAlign: 'right', FormatNumber: '0,0.00', FormatNegative: '$' },
        { HeaderText: 'Company', Name: 'ComTO', Width: 130, HeaderAlign: 'center', CellAlign: 'right', FormatNumber: '0,0.00', FormatNegative: '$' },
        { HeaderText: '%', Name: 'ComTOPercent', Width: 100, HeaderAlign: 'center', CellAlign: 'right', FormatNumber: '0,0.00', FormatNegative: '$' },
        { HeaderText: 'Super', Name: 'SAgTO', Width: 130, HeaderAlign: 'center', CellAlign: 'right', FormatNumber: '0,0.00', FormatNegative: '$' },
        { HeaderText: '%', Name: 'SAgTOPercent', Width: 100, HeaderAlign: 'center', CellAlign: 'right', FormatNumber: '0,0.00', FormatNegative: '$' },
        { HeaderText: 'Master', Name: 'MAgTO', Width: 130, HeaderAlign: 'center', CellAlign: 'right', FormatNumber: '0,0.00', FormatNegative: '$' },
        { HeaderText: '%', Name: 'MAgTOPercent', Width: 100, HeaderAlign: 'center', CellAlign: 'right', FormatNumber: '0,0.00', FormatNegative: '$' },
        { HeaderText: 'Agent', Name: 'AgTO', Width: 130, HeaderAlign: 'center', CellAlign: 'right', FormatNumber: '0,0.00', FormatNegative: '$' },
        { HeaderText: '%', Name: 'AgTOPercent', Width: 100, HeaderAlign: 'center', CellAlign: 'right', FormatNumber: '0,0.00', FormatNegative: '$' },
        { HeaderText: 'Company', Name: 'ComWinlost', Width: 130, HeaderAlign: 'center', CellAlign: 'right', FormatNumber: '0,0.00', FormatNegative: '$' },
        { HeaderText: '%', Name: 'ComWinlostPercent', Width: 100, HeaderAlign: 'center', CellAlign: 'right', FormatNumber: '0,0.00', FormatNegative: '$' },
        { HeaderText: 'Super', Name: 'SAgWinlost', Width: 130, HeaderAlign: 'center', CellAlign: 'right', FormatNumber: '0,0.00', FormatNegative: '$' },
        { HeaderText: '%', Name: 'SAgWinlostPercent', Width: 100, HeaderAlign: 'center', CellAlign: 'right', FormatNumber: '0,0.00', FormatNegative: '$' },
        { HeaderText: 'Master', Name: 'MAgWinlost', Width: 130, HeaderAlign: 'center', CellAlign: 'right', FormatNumber: '0,0.00', FormatNegative: '$' },
        { HeaderText: '%', Name: 'MAgWinlostPercent', Width: 100, HeaderAlign: 'center', CellAlign: 'right', FormatNumber: '0,0.00', FormatNegative: '$' },
        { HeaderText: 'Agent', Name: 'AgWinlost', Width: 130, HeaderAlign: 'center', CellAlign: 'right', FormatNumber: '0,0.00', FormatNegative: '$' },
        { HeaderText: '%', Name: 'AgWinlostPercent', Width: 100, HeaderAlign: 'center', CellAlign: 'right', FormatNumber: '0,0.00', FormatNegative: '$' }
    ],
    Render: function () {
        var gridOption = {
            customizeHeader: true,
            groupHeaderRows: [
                    {
                        Columns: [
                                { HeaderText: "No.", Name: "No", MergeRowNumber: 2 },
                                { HeaderText: "Username", Name: "Username", MergeRowNumber: 2 },
                                { HeaderText: "Total Turnover", Name: "TotalTO", MergeRowNumber: 2 },
                                { HeaderText: "Turnover & Position Taking", Name: "ComTO", MergeColNumber: 8 },
                                { HeaderText: '<div id="HeaderWinlossMargin">Win/Loss & Margin</div>', Name: "ComWinlost", MergeColNumber: 8 }
                            ]
                    }
            ],
            columns: $this.defaultColumns,
            numberOfFrozenColumn: 3,
            showFooter: true,
            gridExpandHeight: 175,
            autoResizeColumn: true,
            showPager: true,
            pagerOption: {
                itemsPerPageArray: [50, 100, 500],
                currentPage: 0,
                totalItem: $this.totalRows,
                itemsPerPage: 50
            },
            highlightFrozenWidth: 2,
            marginRight: 2,
            bottomBorderWidth: 0,
            showDistinctRow: false,
            allowTruncate: true,
            columnsNeedHide: [12, 14, 16, 18],
            showColumnsShowHide: !$this.hidedMargin
        };

        // For Summary Footer
        this.SumTotalTO = 0;
        this.SumComTO = 0;
        this.SumSAgTO = 0;
        this.SumMAgTO = 0;
        this.SumAgTO = 0;
        this.SumComWinlost = 0;
        this.SumSAgWinlost = 0;
        this.SumMAgWinlost = 0;
        this.SumAgWinlost = 0;

        //// Assing BodyData
        var bodyData = [];
        for (var i = 0; i < this.totalRows; i++) {
            bodyData.push({
                Columns: [
                    { Name: "No", Value: (i + 1) },
                    { Name: "Username", Value: this.dataSource[i] },
                    { Name: "TotalTO", Value: this.dataSource[i].TotalTO },
                    { Name: "ComTO", Value: this.dataSource[i].ComTO },
                    { Name: "ComTOPercent", Value: this.dataSource[i].ComTOPercent },
                    { Name: "SAgTO", Value: this.dataSource[i].SAgTO },
                    { Name: "SAgTOPercent", Value: this.dataSource[i].SAgTOPercent },
                    { Name: "MAgTO", Value: this.dataSource[i].MAgTO },
                    { Name: "MAgTOPercent", Value: this.dataSource[i].MAgTOPercent },
                    { Name: "AgTO", Value: this.dataSource[i].AgTO },
                    { Name: "AgTOPercent", Value: this.dataSource[i].AgTOPercent },
                    { Name: "ComWinlost", Value: this.dataSource[i].ComWinlost },
                    { Name: "ComWinlostPercent", Value: this.dataSource[i].ComWinlostPercent },
                    { Name: "SAgWinlost", Value: this.dataSource[i].SAgWinlost },
                    { Name: "SAgWinlostPercent", Value: this.dataSource[i].SAgWinlostPercent },
                    { Name: "MAgWinlost", Value: this.dataSource[i].MAgWinlost },
                    { Name: "MAgWinlostPercent", Value: this.dataSource[i].MAgWinlostPercent },
                    { Name: "AgWinlost", Value: this.dataSource[i].AgWinlost },
                    { Name: "AgWinlostPercent", Value: this.dataSource[i].AgWinlostPercent }
                ]
            })

            this.SumTotalTO += this.dataSource[i].TotalTO;
            this.SumComTO += this.dataSource[i].ComTO;
            this.SumSAgTO += this.dataSource[i].SAgTO;
            this.SumMAgTO += this.dataSource[i].MAgTO;
            this.SumAgTO += this.dataSource[i].AgTO;
            this.SumComWinlost += this.dataSource[i].ComWinlost;
            this.SumSAgWinlost += this.dataSource[i].SAgWinlost;
            this.SumMAgWinlost += this.dataSource[i].MAgWinlost;
            this.SumAgWinlost += this.dataSource[i].AgWinlost;
        }

        gridOption.bodyRows = bodyData;
        gridOption.pagerOption.totalItem = this.totalRows;
                
        //// Asign Footer Data
        var footerData = [];
        footerData.push(
        {
            Columns: [
            { Name: "No", Value: "Grand Total", MergeColNumber: 2 },
            { Name: "TotalTO", Value: this.SumTotalTO },
            { Name: "ComTO", Value: this.SumComTO },
            { Name: "ComTOPercent", Value: $this.CalculatePercentGrandTotal(this.SumComTO, this.SumTotalTO) },
            { Name: "SAgTO", Value: this.SumSAgTO },
            { Name: "SAgTOPercent", Value: $this.CalculatePercentGrandTotal(this.SumSAgTO, this.SumTotalTO) },
            { Name: "MAgTO", Value: this.SumMAgTO },
            { Name: "MAgTOPercent", Value: $this.CalculatePercentGrandTotal(this.SumMAgTO, this.SumTotalTO) },
            { Name: "AgTO", Value: this.SumAgTO },
            { Name: "AgTOPercent", Value: $this.CalculatePercentGrandTotal(this.SumAgTO, this.SumTotalTO) },

            { Name: "ComWinlost", Value: this.SumComWinlost },
            { Name: "ComWinlostPercent", Value: $this.CalculatePercentGrandTotal(this.SumComWinlost, this.SumComTO) },
            { Name: "SAgWinlost", Value: this.SumSAgWinlost },
            { Name: "SAgWinlostPercent", Value: $this.CalculatePercentGrandTotal(this.SumSAgWinlost, this.SumSAgTO) },
            { Name: "MAgWinlost", Value: this.SumMAgWinlost },
            { Name: "MAgWinlostPercent", Value: $this.CalculatePercentGrandTotal(this.SumMAgWinlost, this.SumMAgTO) },
            { Name: "AgWinlost", Value: this.SumAgWinlost },
            { Name: "AgWinlostPercent", Value: $this.CalculatePercentGrandTotal(this.SumAgWinlost, this.SumAgTO) }
            ]
        });
        gridOption.footerRows = footerData;
        return gridOption;
    },
    Databind: function () {
        if ($("#txtUserName").val() != '') {
            this.username = Fanex.encodeHtml($("#txtUserName").val());
        }

        this.date = $('#dtpWinLossDate').val();
        $("#DateValueInfo").html(this.date);

        var parameterUrl = "?groupname=" + this.groupName
                            + "&srecommend=" + this.srecommend
                            + "&mrecommend=" + this.mrecommend
                            + "&recommend=" + this.recommend
                            + "&custid=" + this.custId
                            + "&username=" + Fanex.ExtAcc.ReplaceAll(this.viewlevel == 5 ? '' : this.username, '&', '')
                            + "&date=" + this.date
                            + "&viewlevel=" + this.viewlevel;

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
                    Fanex.ExtAcc.Form.Alert('Error', data.errorMessage.formatErrorMessage(), 350, 150, 'alertDWL');
                    return;
                }

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

                $('#mainGrid').css('visibility', $this.totalRows < 1 ? 'hidden' : 'visible');
                $('#mainGrid').css('display', $this.totalRows < 1 ? 'none' : 'block');
                $('#headlineDaily').css('display', $this.totalRows < 1 ? 'none' : 'block');
                $('#btnHideMG').css('display', $this.totalRows < 1 ? 'none' : 'block');
                $('#exporttoexcel').css('display', $this.totalRows < 1 ? 'none' : 'block');
                $('#msgNoinfo').css('display', $this.totalRows < 1 ? 'block' : 'none');

                if ($this.totalRows > 0) {
                    $this.gridOptions = $this.Render();
                    $("#jqgrid").fanexGrid($this.gridOptions);
                }

                Fanex.ExtAcc.UnBlockUI();
            },
            error: function () {
                Fanex.ExtAcc.UnBlockUI();
            }
        })
    }
};

$(document).ready(function () {
    Fanex.ExtAcc.DailyWinLoss.Init();
});