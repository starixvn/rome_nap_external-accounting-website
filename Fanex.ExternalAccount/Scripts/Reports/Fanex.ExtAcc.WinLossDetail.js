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
                $('#btnHideMG').css('display', $this.totalRows < 1 ? 'none' : 'block');
                $('#exporttoexcel').css('display', $this.totalRows < 1 ? 'none' : 'block');
                $('#msgNoinfo').css('display', $this.totalRows < 1 ? 'block' : 'none');

                if ($this.totalRows > 0) {
                    $this.renderReport(data);
                    $("#exporttoexcel").removeClass("hide");
                } else {
                    $("#exporttoexcel").addClass("hide");
                }

                Fanex.ExtAcc.UnBlockUI();
            },
            error: function () {
                Fanex.ExtAcc.UnBlockUI();
            }
        });
    },

    renderFooter: function (grandTotal) {
        var html = $this.renderRow(grandTotal, true);
        html += '<tr><td colspan="19"><div id="report-pager"></div></td></tr>';

        return html;
    },
    sumValues: function (row, grandTotal) {
        grandTotal.Turnover += row.Turnover;
        grandTotal.NetTurnover += row.NetTurnover;
        grandTotal.GrossComm += row.GrossComm;

        grandTotal.MemberWinlost += row.MemberWinlost;
        grandTotal.PlayerComm += row.PlayerComm;
        grandTotal.MemberTotal += row.MemberTotal;

        grandTotal.AgWinlost += row.AgWinlost;
        grandTotal.AgComm += row.AgComm;
        grandTotal.AgTotal += row.AgTotal;

        if (this.viewlevel > 1) {
            grandTotal.MAgWinlost += row.MAgWinlost;
            grandTotal.MAgComm += row.MAgComm;
            grandTotal.MAgTotal += row.MAgTotal;
        }

        if (this.viewlevel > 2) {
            grandTotal.SAgWinlost += row.SAgWinlost;
            grandTotal.SAgComm += row.SAgComm;
            grandTotal.SAgTotal += row.SAgTotal;
        }

        grandTotal.Company += row.Company;
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
            var func = "javascript: Fanex.ExtAcc.WinLossDetail.GetDownlineWinLossDetail('" + custId + "')";
            html = '<div class="col-username">' +
                        '<a class="text-truncate" style="text-decoration: none; margin: 0" href="' + func + '" title="' + username + '">' + username + '</a>' +
                    '</div>';
        }

        return html;
    },
    renderRow: function (row, isTotal) {
        var html;
        if (isTotal) {
            html = '<tr class="row-grand-total">\
                        <td class="align-right">Grand Total</td>';
        } else {
            html = '<tr class="hide">\
                        <td>' + $this.renderUsernameColumn(row.Username, row.CustId) + '</td>';
        }

        html += '<td class="align-right">' + $this.renderNegativeNumber(row.Turnover) + '</td>';
        html += '<td class="align-right">' + $this.renderNegativeNumber(row.NetTurnover) + '</td>';
        html += '<td class="align-right">' + $this.renderNegativeNumber(row.GrossComm) + '</td>';

        html += '<td class="align-right">' + $this.renderNegativeNumber(row.MemberWinlost) + '</td>';
        html += '<td class="align-right">' + $this.renderNegativeNumber(row.PlayerComm) + '</td>';
        html += '<td class="align-right">' + $this.renderNegativeNumber(row.MemberTotal) + '</td>';

        html += '<td class="align-right">' + $this.renderNegativeNumber(row.AgWinlost) + '</td>';
        html += '<td class="align-right">' + $this.renderNegativeNumber(row.AgComm) + '</td>';
        html += '<td class="align-right">' + $this.renderNegativeNumber(row.AgTotal) + '</td>';

        if ($this.viewlevel >= 2) {
            html += '<td class="align-right">' + $this.renderNegativeNumber(row.MAgWinlost) + '</td>';
            html += '<td class="align-right">' + $this.renderNegativeNumber(row.MAgComm) + '</td>';
            html += '<td class="align-right">' + $this.renderNegativeNumber(row.MAgTotal) + '</td>';
        }

        if ($this.viewlevel >= 3) {
            html += '<td class="align-right">' + $this.renderNegativeNumber(row.SAgWinlost) + '</td>';
            html += '<td class="align-right">' + $this.renderNegativeNumber(row.SAgComm) + '</td>';
            html += '<td class="align-right">' + $this.renderNegativeNumber(row.SAgTotal) + '</td>';
        }

        html += '<td class="align-right">' + $this.renderNegativeNumber(row.Company) + '</td>';
        html += '</tr>';

        return html;
    },
    renderBody: function (grandTotal) {
        var html = '';

        for (var i = 0; i < $this.totalRows; i++) {
            var row = $this.dataSource[i];
            html += $this.renderRow(row, false);
            $this.sumValues(row, grandTotal);
        }

        return html;
    },
    hideColumnByLevel: function () {
        if (this.viewlevel == 1) {
            $(".col-master").addClass("hide");
            $(".col-super").addClass("hide");
            return;
        }

        if (this.viewlevel == 2) {
            $(".col-super").addClass("hide");
            $(".col-master").removeClass("hide");
            return;
        }

        if (this.viewlevel == 3) {
            $(".col-master").removeClass("hide");
            $(".col-super").removeClass("hide");
        }
    },
    renderReport: function (data) {
        $this.hideColumnByLevel();

        var grandTotal = {
            Turnover: 0,
            NetTurnover: 0,
            GrossComm: 0,

            MemberWinlost: 0,
            PlayerComm: 0,
            MemberTotal: 0,

            AgWinlost: 0,
            AgComm: 0,
            AgTotal: 0,

            MAgWinlost: 0,
            MAgComm: 0,
            MAgTotal: 0,

            SAgWinlost: 0,
            SAgComm: 0,
            SAgTotal: 0,

            Company: 0
        };
        $this.movePriorityInfoToFirst();

        var bodyContent = $this.renderBody(grandTotal);
        var footerContent = $this.renderFooter(grandTotal);

        $this.sorters.removeClass("desc").removeClass("asc");
        $("#tbl-report tbody").empty().append(bodyContent);
        $("#tbl-report tfoot").empty().append(footerContent);

        $this.renderPager();
        $('#netTurnover').bind('click',
            function () {
                var message =
                    "- For Sportbook, Net Turnover is Turnover which excludes the draw tickets and halves of the turnover of the half won, half lost tickets.";

                if (data.tooltips.showAllbet) {
                    message += "<br/>" +
                        "<br/>" +
                        "- For AllBet, Net Turnover is total valid bets that we use to compute commission.";
                }

                var height = data.tooltips.showAllbet ? 220 : 170;
                Fanex.ExtAcc.Form.Alert("Net Turnover Information", message, 430, height);
                return false;
            });
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
    },

    sorters: null,
    sortTable: function (elem) {
        var currentDirection = elem.hasClass("desc") ? -1 : 1;
        $this.updateSortIcon(elem, currentDirection);

        var sortedColumnName = elem.attr("data-sortedname");
        $this.dataSource.sort(function (a, b) {
            return $this.sortValues(a[sortedColumnName], b[sortedColumnName], currentDirection);
        });

        $this.reRenderBody();
    },
    updateSortIcon: function (elem, currentDirection) {
        $this.sorters.removeClass("desc").removeClass("asc");
        var nextDirection = currentDirection === 1 ? "desc" : "asc";
        elem.addClass(nextDirection);
    },
    movePriorityInfoToFirst: function () {
        var countFixedSort = $this.fixedsortlist.length;
        if (countFixedSort === 0) {
            return;
        }

        var fixedRows = [];
        var usedRows = {};
        var j;
        var data;

        for (var i = 0; i < countFixedSort; i++) {
            var fixedSort = $this.fixedsortlist[i];

            for (j = 0; j < $this.dataSource.length; j++) {
                data = $this.dataSource[j];

                if (fixedSort === data.CustId) {
                    usedRows[fixedSort] = true;
                    fixedRows.push(data);
                }
            }
        }

        var canSortRows = [];
        for (j = 0; j < $this.dataSource.length; j++) {
            data = $this.dataSource[j];

            if (usedRows[data.CustId]) {
                continue;
            }

            canSortRows.push(data);
        }

        $this.dataSource = fixedRows.concat(canSortRows);
    },
    reRenderBody: function () {
        var html = '';

        for (var i = 0; i < $this.totalRows; i++) {
            var row = $this.dataSource[i];
            html += $this.renderRow(row, false);
        }

        $("#tbl-report > tbody").empty().append(html);

        $this.bodyRows = $("#tbl-report > tbody > tr");
        $this.showBodyRows();
    },
    sortValues: function (aValue, bValue, currentDirection) {
        if (aValue === bValue) {
            return 0;
        }

        var direction = aValue < bValue ? 1 : -1;
        return direction * currentDirection;
    }
};

$(document).ready(function () {
    Fanex.ExtAcc.WinLossDetail.Init();
});