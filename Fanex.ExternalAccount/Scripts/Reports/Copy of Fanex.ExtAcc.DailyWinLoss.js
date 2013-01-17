if (typeof Fanex == 'undefined' || typeof Fanex.ExtAcc == 'undefined') {
    alert("Fanex.ExtAcc.js module is required!")
}

Fanex.ExtAcc.DailyWinLoss = {
    theme: Fanex.ExtAcc.theme,
    leftMenuWidth: Fanex.ExtAcc.leftMenuWidth,
    gHeight: 200,
    gSize: { width: 500, height: 300 },
    separatedLineHeight: 15,
    totalRows: 0,
    totalRowsInPage: 50,
    pageIndex: 0,
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
    url: resolveClientUrl('Report/GetDailyWinLoss'),
    viewlevel: 5,
    bindingCompleted: true,
    hidedMargin: false,
    dataSource: [],
    $this: null,

    SetRange: function () {
        return {
            minDate: new Date($('#BalanceUpDay').val()),
            maxDate: new Date()
        }
    },
    CallSubmit: function () {
        this.ResetParameters();
        this.username = $("#txtUserName input").val();
        this.Databind()
    },
    Init: function () {
        $this = this;

        $('#dtpWinLossDate').datepicker({ showOn: 'both', buttonImageOnly: true, buttonImage: '/Images/calendar.png', buttonText: '', beforeShow: $this.SetRange, dateFormat: 'M dd, yy' });
        $("#dtpWinLossDate").val($.datepicker.formatDate('M dd, yy', new Date()));
        $("#Submit").jqxButton({
            theme: 'darkblue',
            width: '80px'
        });

        $("#exporttoexcel").jqxButton({
            theme: $this.theme,
            width: '17px',
            height: '17px'
        });

        $("#btnHideMG").jqxButton({
            theme: $this.theme,
            width: '120px',
            height: '26px'
        });

        this.gHeight = Math.round($("#footer").position().top - $("#jqgrid").position().top);

        $('#ddlGroupName').bind('change', function (event) {
            var item = event.target;
            $this.groupName = item.innerHTML;
            $("#txtUserName input").val('');
            $("#txtUserName").jqxComboBox('clear');
            $this.ResetParameters();
            $this.InitShowHideControls('none');
        });

        $('#Submit').click(function (e) {
            $this.CallSubmit();
        });
        $('#exporttoexcel').click(function () {
            var params = "date=" + $this.date + "&viewlevel=" + $this.viewlevel + '&custid=' + $this.custId + '&groupname=' + $this.groupName + '&username=' + $this.username;
            document.location.href = resolveClientUrl("Report/GetExportToExcel?") + params;
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
                $("#txtUserName input").val('');
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
                $("#txtUserName input").val('');
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
                $("#txtUserName input").val('');
                $this.username = '';
                $this.ToggleTabs('block', 'block', 'none', 2);
                $this.Databind();
            }
        });
        $('#btnHideMG').click(function (e) {
            if (!$this.hidedMargin) {
                $('#btnHideMG').val('Show Margin');
                $this.hidedMargin = true;
                //$this.Render();
                $("#jqgrid").jqGrid('hideCol', ["ComWinlostPercent", "SAgWinlostPercent", "MAgWinlostPercent", "AgWinlostPercent"]);
            } else {
                $('#btnHideMG').val('Hide Margin');
                $this.hidedMargin = false;
                //$this.Render();
                $("#jqgrid").jqGrid('showCol', ["ComWinlostPercent", "SAgWinlostPercent", "MAgWinlostPercent", "AgWinlostPercent"]);
            }
            $this.Resize(true);
        });

        $('#txtUserName').bind('open', function (e) {
            if (typeof e == 'undefined') e = window.event;
            var key = (typeof e.keyCode == 'undefined') ? e.which : e.keyCode;
            if (key == 13) {
                $("#txtUserName").jqxComboBox("close");
                $this.ResetParameters();
                $this.username = $("#txtUserName input").val();
                $this.Databind();
                return true
            } else {
                return true
            }
        });

        $('#txtUserName').bind('close', function (e) {
            if (typeof e == 'undefined') e = window.event;
            var key = (typeof e.keyCode == 'undefined') ? e.which : e.keyCode;
            if (key == 13) {
                $("#txtUserName").jqxComboBox("close");
                $this.ResetParameters();
                $this.username = $("#txtUserName input").val();
                $this.Databind();
                return true
            } else {
                return true
            }
        });

        $('#txtUserName').keypress(function (e) {
            if (typeof e == 'undefined') e = window.event;
            var key = (typeof e.keyCode == 'undefined') ? e.which : e.keyCode;
            if (key == 13) {
                $("#txtUserName").jqxComboBox("close");
                $this.ResetParameters();
                $this.username = $("#txtUserName input").val();
                $this.Databind();
                return true
            } else {
                return true
            }
        });

        var usernames = {
            datatype: "json",
            datafields: [{
                name: 'username'
            }],
            url: null
        };
        var dataAdapter = new $.jqx.dataAdapter(usernames, {
            formatData: function (data) {
                var searchString = $("#txtUserName input").val();
                $this.groupName = $('#selectedGroupName').html();
                data.name_startsWith = searchString;
                data.groupname = $this.groupName;
                return data;
            }
        });
        $("#txtUserName").jqxComboBox({
            width: 140,
            height: 25,
            source: dataAdapter,
            remoteAutoComplete: true,
            theme: this.theme,
            animationType: 'none',
            displayMember: "username",
            valueMember: "username",
            renderer: function (index, label, value) {
                var item = dataAdapter.records[index];
                if (item != null) {
                    var label = item.username;
                    return label
                }
                return ""
            },
            renderSelectedItem: function (index, item) {
                var item = dataAdapter.records[index];
                if (item != null) {
                    var label = item.username;
                    return label
                }
                return ""
            },
            search: function (searchString) {
                usernames.url = resolveClientUrl("Report/GetSuggestedUserName");
                dataAdapter.dataBind();
            }
        });
        // Hide arrow button
        $('#dropdownlistArrowtxtUserName').hide();
        $('#dropdownlistContenttxtUserName').height('100%').width('100%');
        $('#dropdownlistContenttxtUserName input').css("text-transform", "uppercase");

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
        $("#txtUserName input").val('');
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
    SumTotalTO: 0,
    SumComTO: 0,
    SumSAgTO: 0,
    SumMAgTO: 0,
    SumAgTO: 0,
    SumComWinlost: 0,
    SumSAgWinlost: 0,
    SumMAgWinlost: 0,
    SumAgWinlost: 0,
    GetSumValue: function (columnName, value) {
        if (columnName == 'TotalTO') {
            this.SumTotalTO = value;
        }
        else if (columnName == 'ComTO') {
            this.SumComTO = value;
        }
        else if (columnName == 'SAgTO') {
            this.SumSAgTO = value;
        }
        else if (columnName == 'MAgTO') {
            this.SumMAgTO = value;
        }
        else if (columnName == 'AgTO') {
            this.SumAgTO = value;
        }
        else if (columnName == 'ComWinlost') {
            this.SumComWinlost = value;
        }
        else if (columnName == 'SAgWinlost') {
            this.SumSAgWinlost = value;
        }
        else if (columnName == 'MAgWinlost') {
            this.SumMAgWinlost = value;
        }
        else if (columnName == 'AgWinlost') {
            this.SumAgWinlost = value;
        }
    },
    SummaryPercent: function (aggregates, column, element, summaryData) {
        var value = 0;
        var columnName = column.datafield;
        if (columnName == 'ComTOPercent') {
            value = $this.CalculatePercentGrandTotal(this.SumComTO, this.SumTotalTO);
        }
        else if (columnName == 'SAgTOPercent') {
            value = $this.CalculatePercentGrandTotal(this.SumSAgTO, this.SumTotalTO);
        }
        else if (columnName == 'MAgTOPercent') {
            value = $this.CalculatePercentGrandTotal(this.SumMAgTO, this.SumTotalTO);
        }
        else if (columnName == 'AgTOPercent') {
            value = $this.CalculatePercentGrandTotal(this.SumAgTO, this.SumTotalTO);
        }
        else if (columnName == 'ComWinlostPercent') {
            value = $this.CalculatePercentGrandTotal(this.SumComWinlost, this.SumComTO);
        }
        else if (columnName == 'SAgWinlostPercent') {
            value = $this.CalculatePercentGrandTotal(this.SumSAgWinlost, this.SumSAgTO);
        }
        else if (columnName == 'MAgWinlostPercent') {
            value = $this.CalculatePercentGrandTotal(this.SumMAgWinlost, this.SumMAgTO);
        }
        else if (columnName == 'AgWinlostPercent') {
            value = $this.CalculatePercentGrandTotal(this.SumAgWinlost, this.SumAgTO);
        }
        return "<div class='bgFooter' style='float: left; width: 100%; height: 100%;'><div id='Sum" + column.datafield + "' class=" + (value < 0 ? 'RedSum' : 'GreenSum') + ">" + value + "</div></div>";
    },
    CalculatePercentGrandTotal: function (value1, value2) {
        value1 = value1.replace(/,/g, '');
        value2 = value2.replace(/,/g, '');
        if (value2 != 0 && value2 != '0.00') {
            return numeral(parseFloat(value1) / parseFloat(value2) * 100).format('0,0.00');
        }
        else {
            return '0.00';
        }
    },
    Summary: function (aggregates, column, element, summaryData) {
        if (summaryData != null && summaryData != 'undefined') {
            var renderstring = "<div class='bgFooter' style='float: left; width: 100%; height: 100%;'>";
            $.each(aggregates, function (key, value) {
                var classSum = 'GreenSum';
                if (summaryData['sum'] < 0) {
                    classSum = 'RedSum'
                }
                renderstring += "<div id='Sum" + column.datafield + "' class='" + classSum + "'>" + value + "</div>";
                $this.GetSumValue(column.datafield, value);
            });
            renderstring += "</div>";
            return renderstring
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
        { name: 'No', index: 'No', width: 60, sorttype: "float", frozen: true, fixed: true, sortable: false },
        { name: 'Username', index: 'Username', width: 90, sorttype: "string", frozen: true,
            formatter: function (value, model, row, viewlevel) {
                if (row == null || $this.viewlevel < 3) {
                    return '<div style="line-height: 22px; text-align: left; padding-left: 3px">' + value + '</div>';
                }
                else {
                    var func = "javascript: Fanex.ExtAcc.DailyWinLoss.GetDownlineDailyWinLoss('" + row.CustId + "')";
                    return html = '<div style="line-height: 22px; text-align: left;"><a style="text-decoration: none;margin:0" href="' + func + '">' + value + '</a></div>';
                }
            }, fixed: true
        },
        { name: 'TotalTO', index: 'TotalTO', width: 110, sorttype: "float", frozen: true,
            formatter: 'number', fixed: true
        },
        { name: 'ComTO', index: 'ComTO', width: 110, sorttype: "float",
            formatter: function (value) {
                return $this.RenderValueNum(value);
            }, fixed: true
        },
        { name: 'ComTOPercent', index: 'ComTOPercent', width: 60, sorttype: "float",
            formatter: function (value) {
                return $this.RenderValueNum(value);
            }, fixed: true
        },
        { name: 'SAgTO', index: 'SAgTO', width: 110, sorttype: "float",
            formatter: function (value) {
                return $this.RenderValueNum(value);
            }, fixed: true
        },
        { name: 'SAgTOPercent', index: 'SAgTOPercent', width: 60, sorttype: "float",
            formatter: function (value) {
                return $this.RenderValueNum(value);
            }, fixed: true
        },
        { name: 'MAgTO', index: 'MAgTO', width: 110, sorttype: "float",
            formatter: function (value) {
                return $this.RenderValueNum(value);
            }, fixed: true
        },
        { name: 'MAgTOPercent', index: 'MAgTOPercent', width: 60, sorttype: "float",
            formatter: function (value) {
                return $this.RenderValueNum(value);
            }, fixed: true
        },
        { name: 'AgTO', index: 'AgTO', width: 110, sorttype: "float",
            formatter: function (value) {
                return $this.RenderValueNum(value);
            }, fixed: true
        },
        { name: 'AgTOPercent', index: 'AgTOPercent', width: 60, sorttype: "float",
            formatter: function (value) {
                return $this.RenderValueNum(value);
            }, fixed: true
        },
        { name: 'ComWinlost', index: 'ComWinlost', width: 110, sorttype: "float",
            formatter: function (value) {
                return $this.RenderValueNum(value);
            }, fixed: true
        },
        { name: 'ComWinlostPercent', index: 'ComWinlostPercent', width: 60, sorttype: "float",
            formatter: function (value) {
                return $this.RenderValueNum(value);
            }, fixed: true
        },
        { name: 'SAgWinlost', index: 'SAgWinlost', width: 110, sorttype: "float",
            formatter: function (value) {
                return $this.RenderValueNum(value);
            }, fixed: true
        },
        { name: 'SAgWinlostPercent', index: 'SAgWinlostPercent', width: 60, sorttype: "float",
            formatter: function (value) {
                return $this.RenderValueNum(value);
            }, fixed: true
        },
        { name: 'MAgWinlost', index: 'MAgWinlost', width: 110, sorttype: "float",
            formatter: function (value) {
                return $this.RenderValueNum(value);
            }, fixed: true
        },
        { name: 'MAgWinlostPercent', index: 'MAgWinlostPercent', width: 60, sorttype: "float",
            formatter: function (value) {
                return $this.RenderValueNum(value);
            }, fixed: true
        },
        { name: 'AgWinlost', index: 'AgWinlost', width: 110, sorttype: "float",
            formatter: function (value) {
                return $this.RenderValueNum(value);
            }, fixed: true
        },
        { name: 'AgWinlostPercent', index: 'AgWinlostPercent', width: 60, sorttype: "float",
            formatter: function (value) {
                return $this.RenderValueNum(value);
            }, fixed: true
        },
    ],
    GetXScrollPosition: function () {
    },
    CalculateHeight: function () {
        this.gHeight = Math.round($("#footer").position().top - $("#jqgrid").position().top) - 75;
        var oHeight = 103;
        var mHeight = this.totalRowsInPage * 23 + oHeight - 1;
        if (mHeight < this.gHeight) {
            this.gHeight = mHeight;
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
                return;
            }
            else {
                if (autoResizeGrid && this.gSize.width != width) {
                    $("#jqgrid").setGridWidth(width, false);
                }
                if (autoResizeGrid && this.gSize.height != height) {
                    $("#jqgrid").setGridHeight(height - 168);
                }
                this.gSize = { width: width, height: height };
            }
        } catch (e) { }
    },
    VAlignColumns: function (gridId, colsIndex) {
    },
    BorderRightColumn: function (gridId, colIdx) {
    },
    GroupColums: function (gridId, colText, colsIndex) {
    },
    Databind: function () {
        if (!$this.bindingCompleted) return;
        $this.bindingCompleted = false;

        if ($("#txtUserName input").val() != '') {
            this.username = $("#txtUserName input").val();
        }
        this.date = $('#dtpWinLossDate').val();
        this.groupName = $('#selectedGroupName').html();
        $("#DateValueInfo").html(this.date);

        $.blockUI({ message: '<img src="../../Images/Loading.gif" />' });

        var parameterUrl = "?groupname=" + this.groupName
                            + "&srecommend=" + this.srecommend
                            + "&mrecommend=" + this.mrecommend
                            + "&recommend=" + this.recommend
                            + "&custid=" + this.custId
                            + "&username=" + this.username
                            + "&date=" + this.date
                            + "&viewlevel=" + this.viewlevel;

        // Init Grid jqxgrid
        $('#gbox_jqgrid').remove();
        $('#mainGrid').append($('<table id="jqgrid" class="removeRadius"></table><div id="jqpager"></div>'));

        $('#jqgrid').jqGrid({
            url: this.url + parameterUrl,
            datatype: "json",
            jsonReader: { repeatitems: false },
            width: this.gSize.width,
            height: this.gSize.height,
            pager: '#jqpager',
            rowNum: this.pageSize,
            rowList: [50, 100, 500],            
            //                hoverrows: true,
            //                viewrecords: true,
            //                footerrow: true,
            //                userDataOnFooter: true,
            beforeSelectRow: function (rowid, e) {
                return false;
            },
            loadComplete: function (data) {
                $this.totalRows = data.records;
                $this.dataSource = data.rows;

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

                $("#txtUserName").jqxComboBox("close");

                //($this.totalRows > 0) && this.Render();


                $("#jqgrid").jqGrid('navGrid', '#jqpager', { edit: false, add: false, del: false });
                //            $("#jqgrid").jqGrid('footerData', 'set', { Rate: 'Summary', ExpenseCharges: ExpenseChargesSum, FoodEditor: FoodEditorSum, Technician: TechnicianSum, CopyEditor: CopyEditorSum, Nutrition: NutritionSum, Groceries: GroceriesSum, GroceryShopper: GroceryShopperSum });


                $('#jqgrid').jqGrid('setGroupHeaders', {
                    useColSpanStyle: true,
                    groupHeaders: [
	                    { startColumnName: 'ComTO', numberOfColumns: 8, titleText: 'Turnover & Position Taking' },
	                    { startColumnName: 'ComWinlost', numberOfColumns: 8, titleText: 'Win/Loss & Margin' }
                    ]
                });

                $('#jqgrid').jqGrid('setFrozenColumns');

                $this.Resize(true);
                $('#mainGrid').css('visibility', 'visible');

                $this.bindingCompleted = true;
                $(document).ajaxStop($.unblockUI);
            },
            loadError: function (e) {
                $this.bindingCompleted = true;
                $(document).ajaxStop($.unblockUI);
            },
            colNames: ['No.', 'Username', 'Total Turnover', 'Company', '%', 'Super', '%', 'Master', '%', 'Agent', '%', 'Company', '%', 'Super', '%', 'Master', '%', 'Agent', '%'],
            colModel: this.defaultColumns
        });
    }
};