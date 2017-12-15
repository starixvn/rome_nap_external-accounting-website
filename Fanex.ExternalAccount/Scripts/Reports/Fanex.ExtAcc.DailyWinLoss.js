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

                $(".col-win-loss-margin").addClass("hide");
                $(".col-percent").addClass("hide");
                $(".col-win-loss").removeClass("hide");
            } else {
                $('#btnHideMG').val('Hide Margin');
                $this.hidedMargin = false;

                $(".col-win-loss").addClass("hide");
                $(".col-win-loss-margin").removeClass("hide");
                $(".col-percent").removeClass("hide");
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

        $('#txtUserName').keyup(function (e) {
            if ($('#txtUserName').val().length > 1) {
                Fanex.ExtAcc.InitSuggestion();
            }
        });
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
        this.username = '';
        this.Databind();
    },
    ResetParameters: function () {
        this.srecommend = 0;
        this.mrecommend = 0;
        this.recommend = 0;
        this.custId = 0;
        this.username = '';
        this.viewlevel = 4;
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
                    $this.renderReport();
                }

                Fanex.ExtAcc.UnBlockUI();
            },
            error: function () {
                Fanex.ExtAcc.UnBlockUI();
            }
        });
    },

    percent: function (partialValue, total) {
        total = String(total).replace(/,/g, '');
        if (total == 0 || total == '0.00') {
            return '0.00';
        }

        partialValue = String(partialValue).replace(/,/g, '');

        return numeral(parseFloat(partialValue) / parseFloat(total) * 100).format('0,0.00');
    },
    calculateGrandTotalPercent: function (grandTotal) {
        grandTotal.ComTOPercent = $this.percent(grandTotal.ComTO, grandTotal.TotalTO);
        grandTotal.SAgTOPercent = $this.percent(grandTotal.SAgTO, grandTotal.TotalTO);
        grandTotal.MAgTOPercent = $this.percent(grandTotal.MAgTO, grandTotal.TotalTO);
        grandTotal.AgTOPercent = $this.percent(grandTotal.AgTO, grandTotal.TotalTO);

        grandTotal.ComWinlostPercent = $this.percent(grandTotal.ComWinlost, grandTotal.ComTO);
        grandTotal.SAgWinlostPercent = $this.percent(grandTotal.SAgWinlost, grandTotal.SAgTO);
        grandTotal.MAgWinlostPercent = $this.percent(grandTotal.MAgWinlost, grandTotal.MAgTO);
        grandTotal.AgWinlostPercent = $this.percent(grandTotal.AgWinlost, grandTotal.AgTO);
    },
    sumValues: function (row, grandTotal) {
        grandTotal.TotalTO += row.TotalTO;
        grandTotal.ComTO += row.ComTO;
        grandTotal.SAgTO += row.SAgTO;
        grandTotal.MAgTO += row.MAgTO;
        grandTotal.AgTO += row.AgTO;
        grandTotal.ComWinlost += row.ComWinlost;
        grandTotal.SAgWinlost += row.SAgWinlost;
        grandTotal.MAgWinlost += row.MAgWinlost;
        grandTotal.AgWinlost += row.AgWinlost;
    },
    renderNegativeNumber: function (value) {
        var html = numeral(value).format('0,0.00');

        return value < 0 ? '<span class="format-negative">' + html + '</span>' : html;
    },
    renderUsernameColumn: function (username, custId) {
        var html;

        if ($this.viewlevel < 2) {
            html = '<div class="text-truncate" class="col-username" title="' + username + '">' + username + '</div>';
        }
        else {
            var func = "javascript: Fanex.ExtAcc.DailyWinLoss.GetDownlineDailyWinLoss('" + custId + "')";
            html = '<div class="col-username">' +
                '<a class="text-truncate" style="text-decoration: none; margin: 0" href="' + func + '" title="' + username + '">' + username + '</a>' +
                '</div>';
        }

        return html;
    },
    renderRow: function (row, isTotal, rowNo) {
        var html;
        if (isTotal) {
            html = '<tr class="row-grand-total">\
                        <td colspan="2" class="align-right">Grand Total</td>';
        } else {
            html = '<tr class="hide">\
                        <td class="align-right">' + rowNo + '</td>\
                        <td>' + $this.renderUsernameColumn(row.Username, row.CustId) + '</td>';
        }

        html += '<td class="align-right">' + $this.renderNegativeNumber(row.TotalTO) + '</td>\
                <td class="align-right">' + $this.renderNegativeNumber(row.ComTO) + '</td>\
                <td class="align-right">' + $this.renderNegativeNumber(row.ComTOPercent) + '</td>\
                <td class="align-right">' + $this.renderNegativeNumber(row.SAgTO) + '</td>\
                <td class="align-right">' + $this.renderNegativeNumber(row.SAgTOPercent) + '</td>\
                <td class="align-right">' + $this.renderNegativeNumber(row.MAgTO) + '</td>\
                <td class="align-right">' + $this.renderNegativeNumber(row.MAgTOPercent) + '</td>\
                <td class="align-right">' + $this.renderNegativeNumber(row.AgTO) + '</td>\
                <td class="align-right">' + $this.renderNegativeNumber(row.AgTOPercent) + '</td>\
                <td class="align-right">' + $this.renderNegativeNumber(row.ComWinlost) + '</td>\
                <td class="align-right col-percent">' + $this.renderNegativeNumber(row.ComWinlostPercent) + '</td>\
                <td class="align-right">' + $this.renderNegativeNumber(row.SAgWinlost) + '</td>\
                <td class="align-right col-percent">' + $this.renderNegativeNumber(row.SAgWinlostPercent) + '</td>\
                <td class="align-right">' + $this.renderNegativeNumber(row.MAgWinlost) + '</td>\
                <td class="align-right col-percent">' + $this.renderNegativeNumber(row.MAgWinlostPercent) + '</td>\
                <td class="align-right">' + $this.renderNegativeNumber(row.AgWinlost) + '</td>\
                <td class="align-right col-percent">' + $this.renderNegativeNumber(row.AgWinlostPercent) + '</td>\
            </tr>';

        return html;
    },
    renderBody: function (grandTotal) {
        var html = '';

        for (var i = 0; i < $this.totalRows; i++) {
            var row = $this.dataSource[i];

            html += $this.renderRow(row, false, i + 1);
            $this.sumValues(row, grandTotal);
        }

        $this.calculateGrandTotalPercent(grandTotal);

        return html;
    },
    renderFooter: function (grandTotal) {
        var html = $this.renderRow(grandTotal, true);
        html += '<tr><td><div id="report-pager"></div></td></tr>';

        return html;
    },
    renderReport: function () {
        $this.totalRows = 1000;
        for (var i = 0; i < 999; i++) {
            $this.dataSource.push($this.dataSource[0]);
        }

        var grandTotal = {
            TotalTO: 0,
            ComTO: 0,
            SAgTO: 0,
            MAgTO: 0,
            AgTO: 0,
            ComWinlost: 0,
            SAgWinlost: 0,
            MAgWinlost: 0,
            AgWinlost: 0
        };

        var bodyContent = $this.renderBody(grandTotal);
        var footerContent = $this.renderFooter(grandTotal);

        $("#tbl-report tbody").empty().append(bodyContent);
        $("#tbl-report tfoot").empty().append(footerContent);
        $this.renderPager();
    },
    pageSize: 50,
    pageIndex: 1,
    bodyRows: null,
    renderPager: function () {
        $this.bodyRows = $("#tbl-report > tbody > tr");
        $("#report-pager").pagination({
            pageList: [50, 100, 500],
            total: $this.totalRows,
            pageSize: $this.pageSize,
            pageNumber: $this.pageIndex,

            showRefresh: false,
            onSelectPage: $this.onSelectPage,
            beforePageText: "Page",
            afterPageTex: "of {pages}",
            displayMsg: "Displaying {from} to {to} of {total} items"
        });

        $this.showBodyRows();
    },
    onSelectPage: function (pageNumber, pageSize) {
        $this.pageSize = pageSize;
        $this.pageIndex = pageNumber;
        $this.showBodyRows();
    },
    showBodyRows: function () {
        if (!$this.totalRows) {
            return;
        }

        var startIndex = $this.pageSize * ($this.pageIndex - 1);
        var endIndex = startIndex + $this.pageSize;

        $this.bodyRows.addClass("hide");
        for (var i = startIndex; i < endIndex && i < $this.totalRows; i++) {
            $($this.bodyRows[i]).removeClass("hide");
        }
    }
};

$(document).ready(function () {
    Fanex.ExtAcc.DailyWinLoss.Init();
});