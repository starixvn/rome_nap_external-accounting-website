Fanex.ExtAcc.Customer.ShowEditSuper = function (custId, title, hideGoldDeluxe, hideSportsbook2, hideCockfighting, hideColossusBets, hideCricket, hideAllbet) {
    this.TransferCondition = false;
    this.UserId = custId;
    this.TitleEditInfo = unescape(title);
    this.CurrentLevel = this.Super;
    this.HideGoldDeluxe = hideGoldDeluxe;
    this.HideSportsbook2 = hideSportsbook2;
    this.HideCockfighting = hideCockfighting;
    this.HideCricket = hideCricket;
    this.HideAllbet = hideAllbet;

    this.ResetFlags();

    var items = {
        sportbook: {
            name: "Sportsbook",
            className: "iconViewEdit",
            callback: function () {
                Fanex.ExtAcc.Customer.IsMultiple = false;
                Fanex.ExtAcc.Customer.IsFullInfo = true;
                Fanex.ExtAcc.Form.Show(resolveClientUrl('SportsbookSetting/EditMultiplePT' + '/?roleId=4&groupName=' + $('#selectedGroupName').html() + '&custId=' + Fanex.ExtAcc.Customer.UserId), 'Edit Sportsbook Setting ' + '<font color="#6dcff6">' + Fanex.ExtAcc.Customer.TitleEditInfo + '</font>', 900, 700, false, false, 1);
            }
        },
        separator0: "-----",
        racing: {
            name: "Racing",
            className: "iconViewEdit",
            callback: function (key, opt) {
                Fanex.ExtAcc.Customer.IsMultiple = false;
                Fanex.ExtAcc.Form.ShowFrame(resolveClientUrl('CustomerSetting/Racebook/RacingSetting/Super?id=' + Fanex.ExtAcc.Customer.UserId + '&groupName=' + $('#selectedGroupName').html()), 'Edit Racing Setting <font color="#6dcff6">' + Fanex.ExtAcc.Customer.TitleEditInfo + '</font>', 1050, 620, false, null, 1);
            }
        },
        separator1: "-----",
        casino: {
            name: "Casino",
            className: "iconViewEdit",
            callback: function () {
                Fanex.ExtAcc.Customer.IsMultiple = false;
                Fanex.ExtAcc.Form.ShowFrame(resolveClientUrl('CustomerSetting/Casino/Super/EditSuper/' + Fanex.ExtAcc.Customer.UserId + '/0/' + $('#selectedGroupName').html()), 'Edit Casino Setting <font color="#6dcff6">' + Fanex.ExtAcc.Customer.TitleEditInfo + '</font>', 1064, 700, false, null, 1);
            }
        },
        liveCasino: {
            name: "Live Casino",
            className: "iconViewEdit",
            callback: function () {
                Fanex.ExtAcc.Customer.IsMultiple = false;
                url = "LiveCasinoSetting/Setting/EditSetting?uplineId=0&uplineLevel=0&customerId=" + Fanex.ExtAcc.Customer.UserId + "&customerLevel=4&formType=2" + "&actorId=" + parent.userId + "&actorUsername=" + parent.username + parent.$('#selectedGroupName').html();
                Fanex.ExtAcc.Form.ShowFrame(resolveClientUrl(url), 'Edit Live Casino Setting <font color="#6dcff6">' + title + '</font>', 900, 700, false, false, 1);
            }
        },
        virtualSport: {
            name: "Virtual Sports",
            className: "iconViewEdit",
            callback: function () {
                Fanex.ExtAcc.Customer.IsMultiple = false;
                Fanex.ExtAcc.Customer.IsFullInfo = true;
                Fanex.ExtAcc.Form.Show(resolveClientUrl('VirtualSports/EditSingle?custId=' + Fanex.ExtAcc.Customer.UserId + '&roleId=5&targetCustId=' + Fanex.ExtAcc.Customer.UserId + '&targetRoleId=' + Fanex.ExtAcc.Customer.Super + '&groupName=' + Fanex.ExtAcc.Customer.GroupName().groupName), 'Edit Virtual Sports Setting ' + '<font color="#6dcff6">' + Fanex.ExtAcc.Customer.TitleEditInfo + '</font>', 880, 700, false, false, 1);
            }
        }
    }

    if (!hideBingo) {
        items.bingo = {
            name: "Bingo",
            className: "iconViewEdit",
            callback: function () {
                Fanex.ExtAcc.Customer.IsMultiple = false;
                Fanex.ExtAcc.Form.ShowFrame(resolveClientUrl('CustomerSetting/Bingo/BingoSetting/Super?id=' + Fanex.ExtAcc.Customer.UserId + '&uplineid=' + Fanex.ExtAcc.Customer.UserId + '&groupName=' + $('#selectedGroupName').html()), 'Edit Bingo Setting <font color="#6dcff6">' + title + '</font>', 1050, 380, false, null, 1);
            }
        };
    }

    if (showKenoSetting == 'true') {
        items.keno = {
            name: "Keno",
            className: "iconViewEdit",
            callback: function () {
                Fanex.ExtAcc.Customer.IsMultiple = false;
                Fanex.ExtAcc.Customer.IsFullInfo = true;
                Fanex.ExtAcc.Form.ShowFrame(resolveClientUrl('customersetting/KenoSetting/kenosetting/EditSuper?custId=' + Fanex.ExtAcc.Customer.UserId), 'Edit Keno Setting ' + '<font color="#6dcff6">' + Fanex.ExtAcc.Customer.TitleEditInfo + '</font>', 880, 700, false, false, 1);
            }
        }
    }

    if (!this.HideGoldDeluxe) {
        items.goldDeluxe = {
            name: "Gold Deluxe",
            className: "iconViewEdit",
            callback: function () {
                Fanex.ExtAcc.Customer.IsMultiple = false;
                Fanex.ExtAcc.Customer.IsFullInfo = true;
                url = "GoldDeluxeSetting/GoldDeluxe/EditSetting?upLineLevel=0&uplineId=0&formType=2&level=4" + "&actorId=" + parent.userId + "&custId=" + Fanex.ExtAcc.Customer.UserId + "&groupName=" + parent.$('#selectedGroupName').html();
                Fanex.ExtAcc.Form.ShowFrame(resolveClientUrl(url), 'Edit Gold Deluxe Setting <font color="#6dcff6">' + title + '</font>', 900, 400, false, false, 1);
            }
        }
    }


    if (showColossusBets == 1 && !hideColossusBets) {
        items.colossusBets = {
            name: "Colossus Bets",
            className: "iconViewEdit",
            callback: function () {
                Fanex.ExtAcc.Customer.IsMultiple = false;
                Fanex.ExtAcc.Customer.IsFullInfo = true;
                url = "customersetting/ColossusBetSetting/ColossusBetSetting/EditSetting?custid=" + Fanex.ExtAcc.Customer.UserId + "&isMultipleEdit=0&userId=" + parent.userId + "&roleId=4&groupName=" + parent.$('#selectedGroupName').html();
                Fanex.ExtAcc.Form.ShowFrame(resolveClientUrl(url), 'Edit Colossus Bets Setting <font color="#6dcff6">' + title + '</font>', 900, 400, false, false, 1);
            }
        }
    }


    if (!this.HideSportsbook2) {
        items.sportsbook2 = {
            name: "Sportsbook2",
            className: "iconViewEdit",
            callback: function () {
                Fanex.ExtAcc.Customer.IsMultiple = false;
                Fanex.ExtAcc.Customer.IsFullInfo = true;
                Fanex.ExtAcc.Customer.ShowEditSingleSportsbook2(Fanex.ExtAcc.Customer.UserId, 5, 4);
            }
        }
    }

    if (!this.HideCockfighting) {
        items.cockFighting = {
            name: "Cockfighting",
            className: "iconViewEdit",
            callback: function () {
                Fanex.ExtAcc.Customer.IsMultiple = false;
                Fanex.ExtAcc.Customer.IsFullInfo = true;
                Fanex.ExtAcc.Customer.ShowEditSingleCockfighting(Fanex.ExtAcc.Customer.UserId, 5, 4);
            }
        }
    }

    if (!this.HideCricket) {
        items.cricket = {
            name: "Cricket",
            className: "iconViewEdit",
            callback: function () {
                Fanex.ExtAcc.Customer.IsMultiple = false;
                Fanex.ExtAcc.Customer.IsFullInfo = true;
                Fanex.ExtAcc.Customer.ShowEditSingleCricket(Fanex.ExtAcc.Customer.UserId, 5, 4);
            }
        }
    }

    if (!this.HideAllbet) {
        items.allbet = {
            name: "Allbet",
            className: "iconViewEdit",
            callback: function () {
                Fanex.ExtAcc.Customer.IsMultiple = false;
                Fanex.ExtAcc.Customer.IsFullInfo = true;
                Fanex.ExtAcc.Customer.ShowEditSingleAllbet(Fanex.ExtAcc.Customer.UserId, 5, 4);
            }
        }
    }

    items.separator4 = "-----";

    items.editInfo = {
        name: "Edit Info",
        height: 25,
        className: "iconEnableEdit editInfoBg",
        callback: function (key, opt) {
            Fanex.ExtAcc.Customer.IsMultiple = false;
            Fanex.ExtAcc.Form.Show(resolveClientUrl('Customer/EditInfo/?custId=' + Fanex.ExtAcc.Customer.UserId + '&groupName=' + Fanex.ExtAcc.Customer.GroupName().groupName), titles[0] + ': <font color="#6dcff6">' + Fanex.ExtAcc.Customer.TitleEditInfo + '</font>', 500, 460, false);
        }
    };

    items.changePassword = {
        name: "Reset Password",
        className: "iconEnableEdit editInfoBg",
        callback: function (key, opt) {
            Fanex.ExtAcc.Customer.IsMultiple = false;
            Fanex.ExtAcc.Form.Show(resolveClientUrl('Customer/ResetPassword/?custId=' + Fanex.ExtAcc.Customer.UserId + '&groupName=' + Fanex.ExtAcc.Customer.GroupName().groupName), 'Reset password: <font color="#6dcff6">' + Fanex.ExtAcc.Customer.TitleEditInfo + '</font>', 415, 400, false);
        }
    };

    items.transferCondition = {
        name: "Transfer Condition",
        className: "iconEnableEdit editInfoBg",
        callback: function (key, opt) {
            Fanex.ExtAcc.Customer.IsMultiple = false;
            Fanex.ExtAcc.Form.Show(resolveClientUrl('Customer/EditTransferCondition/?custId=' + Fanex.ExtAcc.Customer.UserId + '&groupName=' + Fanex.ExtAcc.Customer.GroupName().groupName), titles[1] + ': <font color="#6dcff6">' + Fanex.ExtAcc.Customer.TitleEditInfo + '</font>', 600, 250, false, false, 1);
        }
    };

    if (showBetSetting == '1') {
        items.betSetting = {
            name: "Edit Betting Limit",
            className: "iconEnableEdit editInfoBg",
            callback: function (key, opt) {
                Fanex.ExtAcc.Customer.IsMultiple = false;
                Fanex.ExtAcc.Form.ShowFrame(resolveClientUrl('CustomerSetting/BetSetting/Sportsbook/Index/' + Fanex.ExtAcc.Customer.UserId + '/' + Fanex.ExtAcc.Customer.UserId + '/' + userId + '/' + $('#selectedGroupName').html()), 'Edit Bet Setting', 900, 660, false, null, 1, 'bettingLimitPopup');
            }
        };
    }

    if ($.contextMenu) $.contextMenu('destroy');

    $.contextMenu({
        selector: '.editInfo',
        items: items,
        trigger: "left",
        ignoreRightClick: true,
        autoHide: true
    });
};

Fanex.ExtAcc.Customer.GetSuperList = function (custId, dataSource) {
    Fanex.ExtAcc.BlockUI();

    this.CurrentLevel = this.Super;
    this.AllowDoubleComm = true;

    var url = resolveClientUrl('Customer/SuperListPost');

    var data = this.GetParams();
    this.custId = custId == 'undefined' ? null : custId;
    data.custId = data.username == '' ? null : custId;

    if (custId == undefined) {
        this.directSuperId = null;
        this.directMasterId = null;
        this.directAgentId = null;
    }

    this.DataFields = [{
        HeaderText: 'No.',
        Name: 'RowNumber',
        Width: 30,
        CellAlign: 'center'
    }, {
        HeaderText: '',
        Name: 'Check',
        Width: 30,
        CellAlign: 'center',
        CustomHeaderRender: function (cell) {
            return '<div id="editMult" class="editMult"></div>';
        },
        CustomCellRender: function (cell) {
            return '<div onclick="Fanex.ExtAcc.Customer.Select(this)" style="margin-left: 0px !important" id="' + cell.Value.custid + '" class="menuInactive"></div>';
        }
    }, {
        HeaderText: 'Username',
        Name: 'username',
        Width: 100,
        CustomCellRender: function (cell) {
            var custId = cell.Value.custid;
            var hideKeno = showKenoSetting == 'false' || cell.Value.HideKeno == true;
            var hideColossusBets = showColossusBets == 0 || cell.Value.HideColossusBet == true;

            var func = "javascript:Fanex.ExtAcc.Customer.GetMasterList(" + custId + ", " + cell.Value.DoubleCommLosing + ", null, null, " + cell.Value.synccasino + ", " + cell.Value.syncBingo + ", " + cell.Value.SyncLiveCasino + "," + hideKeno + "," + cell.Value.HideVirtualSports + "," + cell.Value.HideGoldDeluxe + "," + cell.Value.HideColossusBet + "," + cell.Value.HideMuayThai + "," + cell.Value.HideCockFighting + "," + cell.Value.HideCricket + ")";
            Fanex.ExtAcc.Customer.superDisableP2P = cell.Value.DisableP2P;
            var len = cell.Value.username.length;
            var title = len >= 15 ? ' title="' + cell.Value.username + '"' : '';

            return '<div' + title + '><a id="usr-' + custId + '" href="' + func + '">' + cell.Value.username + '</a></div>';
        }
    }, {
        HeaderText: 'Edit',
        Name: 'Edit',
        Width: 30,
        CustomCellRender: function (cell) {
            var hideAllbetSettingLink = showAllbet && (cell.Value.HideAllbet || !cell.Value.SyncedAllBetUpline);
            var func = "javascript:Fanex.ExtAcc.Customer.ShowEditSuper(" + cell.Value.custid + ", '" + escape(cell.Value.username) + "', " + (cell.Value.HideGoldDeluxe) + ", " + (cell.Value.HideMuayThai) + ", " + (cell.Value.HideCockFighting) + ", " + cell.Value.HideColossusBet + ", " + (cell.Value.HideCricket) + ", " + (hideAllbetSettingLink) + ")";

            return '<div onclick="' + func + '" id="edit-' + cell.Value.custid + '" class="editInfo"></div>';
        }
    }, {
        HeaderText: 'Status',
        Name: 'Status',
        Width: 75,
        CellAlign: 'center',
        CustomCellRender: function (cell) {
            return !cell.Value.enabled ? 'Disabled' : cell.Value.closed ? 'Closed' : cell.Value.suspended ? 'Suspended' : 'Open';
        }
    }, {
        HeaderText: 'Double Comm',
        Name: 'DoubleCommLosing',
        Width: 80,
        CustomCellRender: function (cell) {
            if (cell.Value.DoubleCommLosing) {
                return '<div onclick="Fanex.ExtAcc.Customer.ConfigDoubleComm(' + cell.Value.custid + ', true)" id="dbl-' + cell.Value.custid + '" class="allowDblComm"></div>';
            }

            return '<div onclick="Fanex.ExtAcc.Customer.ConfigDoubleComm(' + cell.Value.custid + ', true)" id="dbl-' + cell.Value.custid + '" class="disallowDblComm"></div>';
        }
    }, {
        HeaderText: 'First Name',
        Name: 'firstname',
        Width: 120
    }, {
        HeaderText: 'Last Name',
        Name: 'lastname',
        Width: 120
    }, {
        HeaderText: 'Group A',
        Name: 'groupa',
        Width: 60,
        CellAlign: 'center',
        CustomAttr: ' title="Applies to all types of Asian Handicap, Over/Under, and Odd/Even bettypes" '
    }, {
        HeaderText: 'Group B',
        Name: 'groupb',
        Width: 60,
        CellAlign: 'center',
        CustomAttr: ' title="Applies to all types of Asian Handicap, Over/Under, and Odd/Even bettypes" '
    }, {
        HeaderText: 'Group C',
        Name: 'groupc',
        Width: 60,
        CellAlign: 'center',
        CustomAttr: ' title="Applies to all types of Asian Handicap, Over/Under, and Odd/Even bettypes" '
    }, {
        HeaderText: 'Group D',
        Name: 'groupd',
        Width: 60,
        CellAlign: 'center',
        CustomAttr: ' title="Applies to all types of Asian Handicap, Over/Under, and Odd/Even bettypes" '
    }, {
        HeaderText: '1X2',
        Name: 'discount1x2',
        Width: 60,
        CellAlign: 'center',
        CustomAttr: ' title="Applies to all types of 1x2 bettype" '
    }, {
        HeaderText: 'Others',
        Name: 'discountcs',
        Width: 60,
        CellAlign: 'center',
        CustomAttr: ' title="Applies to all other bettypes" '
    }, {
        HeaderText: 'Game',
        Name: 'discountnumber',
        Width: 60,
        CellAlign: 'center',
        CustomAttr: ' title="Number Game" '
    }, {
        HeaderText: '% Odds',
        Name: 'discountmo',
        Width: 60,
        CellAlign: 'center',
        CustomAttr: ' title="Myanmar Odds" '
    }, {
        HeaderText: 'Created Date',
        Name: 'createdate',
        CellAlign: 'center',
        Width: 160,
        FormatDateTime: 'MM/DD/YYYY hh:mm:ss A'
    }, {
        HeaderText: 'Login IP',
        Name: 'lastloginip',
        CellAlign: 'center',
        Width: 100,
        CustomCellRender: function (cell) {
            return '<div><a href="javascript:Fanex.ExtAcc.Customer.ShowIpInfo(\'' + cell.Value + '\')">' + cell.Value + '</a></div>';
        }
    }];

    if (dataSource) {
        this.RenderSuperList(dataSource);
    } else {
        $.ajax({
            url: url,
            data: data,
            dataType: 'json',
            type: 'post'
        })
        .success(function (result) {
            Fanex.ExtAcc.Customer.RenderSuperList(result);
        })
        .error(function (xhr, status, err) {
            Fanex.ExtAcc.UnBlockUI();
        });
    }
};

Fanex.ExtAcc.Customer.RenderSuperList = function (dataSource) {
    if (dataSource.srecommend) {
        this.superId = dataSource.srecommend;
        this.masterId = dataSource.mrecommend;
        this.agentId = dataSource.recommend;
    }

    var rows = $.parseJSON(dataSource.rows);
    var options = null;
    var bodyRows = [];

    this.DataSource = rows;

    for (var i = 0; i < rows.length; i++) {
        var cssRow = !rows[i].enabled ? 'rowDisabled' : rows[i].closed ? 'rowClosed' : rows[i].suspended ? 'rowSuspended' : '';

        bodyRows.push({
            Columns: [{
                Name: 'RowNumber',
                Value: rows[i].RowNumber
            }, {
                Name: 'Check',
                Value: rows[i]
            }, {
                Name: 'username',
                Value: rows[i]
            }, {
                Name: 'Edit',
                Value: rows[i]
            }, {
                Name: 'Status',
                Value: rows[i]
            }, {
                Name: 'DoubleCommLosing',
                Value: rows[i]
            }, {
                Name: 'firstname',
                Value: rows[i].firstname
            }, {
                Name: 'lastname',
                Value: rows[i].lastname
            }, {
                Name: 'groupa',
                Value: rows[i].groupa
            }, {
                Name: 'groupb',
                Value: rows[i].groupb
            }, {
                Name: 'groupc',
                Value: rows[i].groupc
            }, {
                Name: 'groupd',
                Value: rows[i].groupd
            }, {
                Name: 'discount1x2',
                Value: rows[i].discount1x2
            }, {
                Name: 'discountcs',
                Value: rows[i].discountcs
            }, {
                Name: 'discountnumber',
                Value: rows[i].discountnumber
            }, {
                Name: 'discountmo',
                Value: rows[i].IsMyanmarOdds ? (rows[i].discountMyanmarOdds ? rows[i].discountMyanmarOdds : 0) : 'N/A'
            }, {
                Name: 'createdate',
                Value: rows[i].createdate
            }, {
                Name: 'lastloginip',
                Value: rows[i].lastloginip
            }],
            CssClass: cssRow
        });
    }

    var groupHeader = [{
        Columns: [{
            HeaderText: 'No.',
            Name: 'RowNumber',
            MergeRowNumber: 2
        }, {
            HeaderText: '',
            Name: 'Check',
            MergeRowNumber: 2
        }, {
            HeaderText: 'Username',
            Name: 'username',
            MergeRowNumber: 2
        }, {
            HeaderText: 'Edit',
            Name: 'Edit',
            MergeRowNumber: 2
        }, {
            HeaderText: 'Status',
            Name: 'Status',
            MergeRowNumber: 2
        }, {
            HeaderText: 'Double Comm',
            Name: 'DoubleCommLosing',
            MergeRowNumber: 2
        }, {
            HeaderText: 'First Name',
            Name: 'firstname',
            MergeRowNumber: 2
        }, {
            HeaderText: 'Last Name',
            Name: 'lastname',
            MergeRowNumber: 2
        }, {
            HeaderText: 'Commission',
            Name: 'groupa',
            MergeColNumber: 8
        }, {
            HeaderText: 'Created Date',
            Name: 'createdate',
            MergeRowNumber: 2
        }, {
            HeaderText: 'Login IP',
            Name: 'lastloginip',
            MergeRowNumber: 2
        }]
    }]

    var currentPage = 0;
    currentPage = (rows.length <= Fanex.ExtAcc.Customer.ItemsPerPage || (parseInt(rows.length / Fanex.ExtAcc.Customer.ItemsPerPage) < Fanex.ExtAcc.Customer.CurrentPage)) ? 0 : Fanex.ExtAcc.Customer.CurrentPage;
    Fanex.ExtAcc.Customer.CurrentPage = currentPage;

    options = {
        columns: this.DataFields,
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
            currentPage: Fanex.ExtAcc.Customer.CurrentPage,
            totalItem: rows.length,
            itemsPerPage: Fanex.ExtAcc.Customer.ItemsPerPage,
            afterPagedFunction: function (options) {
                Fanex.ExtAcc.Customer.CurrentPage = options.currentPage;
                Fanex.ExtAcc.Customer.ItemsPerPage = options.itemsPerPage;
            }
        },
        marginRight: 2,
        showDistinctRow: false,
        allowTruncate: true
    };

    this.Render(options);

    $('#customerListGrid').css('visibility', 'visible');

    $('#breadcrumbs').empty();

    Fanex.ExtAcc.OnResize = function () {
        try {
            $("#jqgrid").fanexGrid("resizeGrid");
        } catch (err) { }
    }

    this.IsLoading = false;
};