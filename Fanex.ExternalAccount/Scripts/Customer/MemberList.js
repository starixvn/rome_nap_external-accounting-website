Fanex.ExtAcc.Customer.ShowEditMember = function (
    custId,
    title,
    transferCondition,
    syncCasino,
    disableCasino,
    hideVirtualSport,
    disableVirtualSport,
    agentSyncCasino,
    isHideKeno,
    disableKeno,
    isHideGoldDeluxe,
    disableGoldDeluxe,
    isHideColossusBets,
    disableColossusBets,
    isHideCricket,
    disableCricket,
    productSettingsJson) {
    this.TransferCondition = transferCondition;
    this.UserId = custId;
    this.TitleEditInfo = unescape(title);
    this.IsMultiple = false;
    this.HideVirtualSports = hideVirtualSport;
    this.HideKeno = showKenoSetting == 'false' || isHideKeno;
    this.HideGoldDeluxe = showGoldDeluxeSetting == 0 || isHideGoldDeluxe;
    this.HideColossusBet = showColossusBets == 0 || isHideColossusBets;
    this.HideCricket = showCricket == 0 || isHideCricket;
    var productSettings = JSON.parse(unescape(productSettingsJson));

    this.ResetFlags();

    var itemsMember = {
        sportbook: {
            name: "Sportsbook",
            className: "iconEnableEdit",
            callback: function () {
                Fanex.ExtAcc.Customer.UserId = custId;
                Fanex.ExtAcc.Customer.IsMultiple = false;
                Fanex.ExtAcc.Customer.IsFullInfo = true;
                Fanex.ExtAcc.Form.Show(resolveClientUrl('SportsbookSetting/EditMultiplePT?roleId=1&groupName=' + Fanex.ExtAcc.Customer.GroupName().groupName + '&custId=' + Fanex.ExtAcc.Customer.UserId), 'Edit Sportsbook Setting ' + '<font color="#6dcff6">' + title + '</font>', 900, Fanex.ExtAcc.Customer.GetPopupHeight(270, 480), false, false, 1);
            }
        },
        ptSportbook: {
            name: "PT",
            className: "iconEnableEdit",
            callback: function (key, opt) {
                Fanex.ExtAcc.Customer.UserId = custId;
                Fanex.ExtAcc.Customer.IsMultiple = false;
                Fanex.ExtAcc.Form.Show(resolveClientUrl('SportsbookSetting/EditPT?groupName=' + Fanex.ExtAcc.Customer.GroupName().groupName + '&custId=' + Fanex.ExtAcc.Customer.UserId), 'Edit PT Setting ' + '<font color="#6dcff6">' + title + '</font>', 888, Fanex.ExtAcc.Customer.GetPopupHeight(270, 480), false, false, 1);
            }
        },
        betSetting: {
            name: "Bet Setting",
            className: "iconEnableEdit",            
            callback: function (key, opt) {
                var height = isMobile.phone ? Fanex.ExtAcc.Customer.GetPopupHeight(270, 480) : 480;
                Fanex.ExtAcc.Customer.UserId = custId;
                Fanex.ExtAcc.Customer.IsMultiple = false;
                Fanex.ExtAcc.Form.Show(resolveClientUrl('SportsbookSetting/EditBetSetting?groupName=' + Fanex.ExtAcc.Customer.GroupName().groupName + '&custId=' + Fanex.ExtAcc.Customer.UserId), 'Edit Bet Setting ' + '<font color="#6dcff6">' + title + '</font>', 871, height, false, false, 1);
            }
        },
        commission: {
            name: "Commission",
            className: "iconEnableEdit",
            callback: function (key, opt) {
                var height = isMobile.phone ? Fanex.ExtAcc.Customer.GetPopupHeight(270, 350) : 350;
                Fanex.ExtAcc.Customer.UserId = custId;
                Fanex.ExtAcc.Customer.IsMultiple = false;
                Fanex.ExtAcc.Form.Show(resolveClientUrl('SportsbookSetting/EditCommissionMemberDetail?groupName=' + Fanex.ExtAcc.Customer.GroupName().groupName + '&custId=' + Fanex.ExtAcc.Customer.UserId), 'Edit Commission ' + '<font color="#6dcff6">' + title + '</font>', 876, height, false, false, 1);
            }
        },
        separator1: "-----"
    };

    var has3rdParty = false;
    if (agentSyncCasino) {
        has3rdParty = true;

        itemsMember.casino = {
            name: "Casino",
            className: syncCasino
                ? (disableCasino ? "iconDisableEdit" : "iconEnableEdit")
                : "iconUnsyncEdit",
            callback: function () {
                Fanex.ExtAcc.Customer.UserId = custId;
                Fanex.ExtAcc.Customer.IsMultiple = false;
                Fanex.ExtAcc.Form.ShowFrame(resolveClientUrl('CustomerSetting/Casino/Member/EditMember?id=' + Fanex.ExtAcc.Customer.UserId + '&uplineid=' + Fanex.ExtAcc.Customer.agentId + '&groupname=' + $('#selectedGroupName').html()), 'Edit Casino Setting <font color="#6dcff6">' + title + '</font>', 1150, Fanex.ExtAcc.Customer.GetPopupHeight(270, 480), false, null, 1);
            }
        };
    }

    function addEditProductMemberContextMenuItem(productId, productKey, productName, isDisabled, isSynced) {
        isSynced = typeof (isSynced) == 'undefined' ? true : isSynced;

        itemsMember[productKey] = {
            name: productName,
            className: !isSynced ? "iconUnsyncEdit" : (isDisabled == true ? "iconDisableEdit" : "iconEnableEdit"),
            callback: function () {
                Fanex.ExtAcc.Customer.IsMultiple = false;
                Fanex.ExtAcc.Customer.IsFullInfo = true;
                Fanex.ExtAcc.Customer.ShowEditProductForSingleCust(
                    custId,
                    2,
                    1,
                    productId,
                    productName);
            }
        };
    }

    if (!hideVirtualSport) {
        has3rdParty = true;

        addEditProductMemberContextMenuItem(5, 'virtualSport', 'Virtual Sports', disableVirtualSport);
    }

    if (!isHideKeno) {
        has3rdParty = true;

        itemsMember.keno = {
            name: "Keno",
            className: disableKeno ? "iconDisableEdit" : "iconEnableEdit",
            callback: function () {
                Fanex.ExtAcc.Customer.UserId = custId;
                Fanex.ExtAcc.Customer.IsMultiple = false;
                Fanex.ExtAcc.Form.ShowFrame(resolveClientUrl('customersetting/KenoSetting/kenosetting/AgentEditMember?custId=' + Fanex.ExtAcc.Customer.UserId + "&userId=" + parent.userId), 'Edit Keno Setting <font color="#6dcff6">' + title + '</font>', 900, Fanex.ExtAcc.Customer.GetPopupHeight(270, 480), false, null, 1);
            }
        };
    }

    if (!this.HideGoldDeluxe) {
        has3rdParty = true;

        itemsMember.goldDeluxe = {
            name: "Gold Deluxe",
            className: disableGoldDeluxe == true ? "iconDisableEdit" : "iconEnableEdit",
            callback: function () {
                var height = isMobile.phone ? Fanex.ExtAcc.Customer.GetPopupHeight(270, 400) : 400;
                Fanex.ExtAcc.Customer.UserId = custId;
                url = "GoldDeluxeSetting/GoldDeluxe/EditSetting?upLineLevel=2&formType=1&level=1" + "&uplineId=" + Fanex.ExtAcc.Customer.agentId + "&actorId=" + parent.userId + "&custId=" + Fanex.ExtAcc.Customer.UserId + "&groupName=" + parent.$('#selectedGroupName').html();
                Fanex.ExtAcc.Form.ShowFrame(resolveClientUrl(url), 'Edit Gold Deluxe Setting <font color="#6dcff6">' + title + '</font>', 900, height, false, false, 1);
            }
        };
    }

    if (!this.HideColossusBet) {
        has3rdParty = true;

        itemsMember.colossusBets = {
            name: "Colossus Bets",
            className: disableColossusBets == true ? "iconDisableEdit" : "iconEnableEdit",
            callback: function () {
                var height = isMobile.phone ? Fanex.ExtAcc.Customer.GetPopupHeight(270, 400) : 400;
                Fanex.ExtAcc.Customer.UserId = custId;
                Fanex.ExtAcc.Customer.IsMultiple = false;
                url = "customersetting/ColossusBetSetting/ColossusBetSetting/EditSetting?custid=" + Fanex.ExtAcc.Customer.UserId + "&recommend=" + Fanex.ExtAcc.Customer.agentId + "&isMultipleEdit=0&userId=" + parent.userId + "&roleId=1&groupName=" + parent.$('#selectedGroupName').html();
                Fanex.ExtAcc.Form.ShowFrame(resolveClientUrl(url), 'Edit ColossusBets <font color="#6dcff6">' + title + '</font>', 900, height, false, false, 1);
            }
        };
    }

    if (!this.HideCricket) {
        addEditProductMemberContextMenuItem(20, 'cricket', 'Cricket', disableCricket);
    }

    for (var i = 0, max = productSettings.length; i < max; i++) {
        has3rdParty = true;
        var productSetting = productSettings[i];

        addEditProductMemberContextMenuItem(
            productSetting.productId,
            productSetting.productKey,
            productSetting.productName,
            productSetting.isDisabled,
            productSetting.isSynced);
    }

    if (has3rdParty)
        itemsMember.separator5 = "-----";
    itemsMember.editInfo = {
        name: "Edit Info",
        className: "iconEnableEdit editInfoBg",
        callback: function (key, opt) {
            Fanex.ExtAcc.Customer.UserId = custId;
            Fanex.ExtAcc.Customer.IsMultiple = false;
            var height = isMobile.phone ? Fanex.ExtAcc.Customer.GetPopupHeight(270, 480) : 500;
            Fanex.ExtAcc.Form.Show(
                resolveClientUrl('Customer/EditInfo/?custId=' + Fanex.ExtAcc.Customer.UserId + '&groupName=' + Fanex.ExtAcc.Customer.GroupName().groupName), titles[0] + ': <font color="#6dcff6">' + Fanex.ExtAcc.Customer.TitleEditInfo + '</font>',
                500,
                height,
                false);
        }
    };

    itemsMember.changePassword = {
        name: "Reset Password",
        className: "iconEnableEdit editInfoBg",
        callback: function (key, opt) {
            Fanex.ExtAcc.Customer.UserId = custId;
            Fanex.ExtAcc.Customer.IsMultiple = false;
            var height = isMobile.phone ? Fanex.ExtAcc.Customer.GetPopupHeight(270, 480) : 400;
            Fanex.ExtAcc.Form.Show(
                resolveClientUrl('Customer/ResetPassword/?custId=' + Fanex.ExtAcc.Customer.UserId + '&groupName=' + Fanex.ExtAcc.Customer.GroupName().groupName), 'Reset password: <font color="#6dcff6">' + Fanex.ExtAcc.Customer.TitleEditInfo + '</font>',
                415,
                height,
                false,
                false);
        }
    };

    if ($.contextMenu) $.contextMenu('destroy');
    $.contextMenu({
        selector: '.editMember',
        items: itemsMember,
        trigger: "left",
        ignoreRightClick: true,
        autoHide: true
    });
};

Fanex.ExtAcc.Customer.GetMemberList = function (
    recommend,
    allow,
    custId,
    dataSource,
    agentSyncCasino,
    isHideKeno,
    isHideVirtualSport,
    isHideGoldDeluxe,
    isHideColossusBets,
    isHideCricket) {
    Fanex.ExtAcc.BlockUI();

    this.agentId = recommend;
    this.CurrentLevel = this.Member;
    this.AllowDoubleComm = allow;

    this.agentSyncCasino = agentSyncCasino;
    this.HideKeno = showKenoSetting == 'false' || isHideKeno;
    this.HideVirtualSports = isHideVirtualSport;
    this.HideGoldDeluxe = isHideGoldDeluxe;
    this.HideColossusBet = isHideColossusBets
    this.HideCricket = isHideCricket;

    this.AllbetSyncIdList = [];
    this.AllbetSyncList = [];

    this.VoidbridgeSyncIdList = [];
    this.VoidbridgeSyncList = [];

    this.NG2SyncIdList = [];
    this.NG2SyncList = [];

    var url = resolveClientUrl('Customer/MemberListPost');

    var data = this.GetParams();
    this.custId = custId == 'undefined' ? null : custId;
    data.custId = data.username == '' ? null : custId;

    Fanex.ExtAcc.Customer.ShowCustomerLimitDateFilter();
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
            var cellValue = cell.Value;
            return '<div sync-ng2="' + cellValue.SyncedNG2 +
                '" sync-voidbridge="' + cellValue.SyncedVoidBridge +
                '" sync-allbet="' + cellValue.SyncedAllBet +
                '" sync-fishingworld="' + cellValue.SyncedFishingWorld +
                '" onclick="Fanex.ExtAcc.Customer.Select(this)" style="margin-left: 0px !important" id="' + cellValue.custid +
                '" class="menuInactive" name="' + cellValue.username +
                '" data-unsyncedproducts="' + Fanex.ExtAcc.Customer.GetUnsyncedProductIds(cell) + '"></div>';
        }
    }, {
        HeaderText: 'Username',
        Name: 'username',
        Width: 100,
        CustomCellRender: function (cell) {
            var userName = cell.Value.username;
            var len = userName.length;
            var title = len >= 15 ? ' title="' + userName + '"' : '';

            return '<div' + title + '>' + userName + '</div>';
        }
    }, {
        HeaderText: 'Edit',
        Name: 'Edit',
        Width: 30,
        CustomCellRender: function (cell) {
            var cellValue = cell.Value;
            var title = escape(cellValue.username);
            var hideKeno = showKenoSetting == 'false' || cellValue.HideKeno == true;
            var hideGoldDeluxe = showGoldDeluxeSetting == 0 || !cellValue.SyncGoldDeluxeUpline;
            var hideColossusBets = showColossusBets == 0 || cellValue.HideColossusBet || isHideColossusBets;
            var hideCricket = showCricket == 0 || cellValue.HideCricket || isHideCricket || !cellValue.IsIMBC;

            var func = "javascript:Fanex.ExtAcc.Customer.ShowEditMember("
                + cellValue.custid + ",'"
                + title + "',"
                + cellValue.disableTransfer + ","
                + cellValue.synccasino + ","
                + cellValue.disablecasino + ","
                + cellValue.HideVirtualSports + ","
                + cellValue.DisableVirtualSports + ","
                + agentSyncCasino + ","
                + hideKeno + ","
                + cellValue.DisableKeno + ","
                + hideGoldDeluxe + ","
                + cellValue.DisableGoldDeluxe + ","
                + hideColossusBets + ","
                + cellValue.DisableColossusBet + ","
                + hideCricket + ","
                + cellValue.DisableCricket + ",'"
                + escape(JSON.stringify(cellValue.productSettings)) + "')";

            return '<div onclick="' + func + '" id="edit-' + cellValue.custid + '" class="editMember"></div>';
        }
    }, {
        HeaderText: 'Status',
        Name: 'Status',
        Width: 75,
        CellAlign: 'center',
        CustomCellRender: function (cell) {
            var cellValue = cell.Value;
            return !cellValue.enabled ? 'Disabled' : cellValue.closed ? 'Closed' : cellValue.suspended ? 'Suspended' : 'Open';
        }
    }, {
        HeaderText: 'Double Comm',
        Name: 'DoubleCommLosing',
        Width: 57,
        CellAlign: 'center',
        CustomCellRender: function (cell) {
            var css = '';
            var custId = cell.Value.custid;

            if (!Fanex.ExtAcc.Customer.AllowDoubleComm) {
                css = 'style="cursor:default !important"';
            }

            if (cell.Value.DoubleCommLosing) {
                return '<div onclick="Fanex.ExtAcc.Customer.ConfigDoubleComm(' + custId + ')" id="dbl-' + custId + '" class="onDblComm" ' + css + '></div>';
            }

            return '<div onclick="Fanex.ExtAcc.Customer.ConfigDoubleComm(' + custId + ')" id="dbl-' + custId + '" class="offDblComm" ' + css + '></div>';
        }
    }, {
        HeaderText: 'Nickname',
        Name: 'NickName',
        Width: 120
    }, {
        HeaderText: 'First Name',
        Name: 'firstname',
        Width: 120
    }, {
        HeaderText: 'Last Name',
        Name: 'lastname',
        Width: 120
    }, {
        HeaderText: 'User Group',
        Name: 'ugroup',
        CellAlign: 'center',
        Width: 50
    }, {
        HeaderText: 'Group',
        Name: 'discount',
        CellAlign: 'center',
        Width: 60,
        CustomAttr: ' title="Applies to all types of Asian Handicap, Over/Under, and Odd/Even bettypes" '
    }, {
        HeaderText: '1X2',
        Name: 'discount1x2',
        CellAlign: 'center',
        Width: 60,
        CustomAttr: ' title="Applies to all types of 1x2 bettype" '
    }, {
        HeaderText: 'Others',
        Name: 'discountcs',
        CellAlign: 'center',
        Width: 60,
        CustomAttr: ' title="Applies to all other bettypes" '
    }, {
        HeaderText: 'Game',
        Name: 'discountnumber',
        CellAlign: 'center',
        Width: 60,
        CustomAttr: ' title="Number Game" '
    }, {
        HeaderText: '% Odds',
        Name: 'discountmo',
        Width: 60,
        CellAlign: 'center',
        CustomAttr: ' title="Myanmar Odds" '
    }, {
        HeaderText: 'Group',
        Name: 'playerdiscount',
        CellAlign: 'center',
        Width: 60,
        CustomAttr: ' title="Applies to all types of Asian Handicap, Over/Under, and Odd/Even bettypes" '
    }, {
        HeaderText: '1X2',
        Name: 'playerdiscount1x2',
        CellAlign: 'center',
        Width: 60,
        CustomAttr: ' title="Applies to all types of 1x2 bettype" '
    }, {
        HeaderText: 'Others',
        Name: 'playerdiscountcs',
        CellAlign: 'center',
        Width: 60,
        CustomAttr: ' title="Applies to all other bettypes" '
    }, {
        HeaderText: 'Game',
        Name: 'playerdiscountnumber',
        CellAlign: 'center',
        Width: 60,
        CustomAttr: ' title="Number Game" '
    }, {
        HeaderText: '% Odds',
        Name: 'playerdiscountmo',
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
        if (!(dataSource.records > 0 && eval(dataSource.rows)[0].IsMyanmarOdds)) {
            this.DataFields.splice(14, 1);
            this.DataFields.splice(18, 1);
        }
        this.RenderMemberList(dataSource);
    } else {
        $.ajax({
            url: url,
            data: data,
            dataType: 'json',
            type: 'post'
        })
        .success(function (result) {
            if (!(result.records > 0 && eval(result.rows)[0].IsMyanmarOdds)) {
                Fanex.ExtAcc.Customer.DataFields.splice(14, 1);
                Fanex.ExtAcc.Customer.DataFields.splice(18, 1);
            }
            Fanex.ExtAcc.Customer.RenderMemberList(result);
        });
    }
};

Fanex.ExtAcc.Customer.RenderMemberList = function (dataSource) {
    if (dataSource.srecommend) {
        this.superId = dataSource.srecommend;
        this.masterId = dataSource.mrecommend;
        this.agentId = dataSource.recommend;
    }

    var rows = $.parseJSON(dataSource.rows);
    var options = null;
    var bodyRows = [];
    var rowsLength = rows.length;

    if (rowsLength > 0) {
        var firstMemberId = rows[0].custid;
        this.UplineProductSettings = this.GetProductSettings(dataSource.customerProductStatus[firstMemberId], true);
    } else {
        this.UplineProductSettings = [];
    }

    this.DataSource = rows;
    var isMyanmarOdds = rowsLength > 0 ? rows[0].IsMyanmarOdds : false;
    for (var i = 0; i < rowsLength; i++) {
        var currentRow = rows[i];
        var cssRow = !currentRow.enabled ? 'rowDisabled' : currentRow.closed ? Fanex.ExtAcc.Customer.GetClosedClassName(currentRow) : currentRow.suspended ? 'rowSuspended' : '';

        currentRow.productSettings = this.GetProductSettings(dataSource.customerProductStatus[currentRow.custid], true);

        bodyRows.push({
            Columns: [{
                Name: 'RowNumber',
                Value: currentRow.RowNumber
            }, {
                Name: 'Check',
                Value: currentRow
            }, {
                Name: 'username',
                Value: currentRow
            }, {
                Name: 'Edit',
                Value: currentRow
            }, {
                Name: 'Status',
                Value: currentRow
            }, {
                Name: 'DoubleCommLosing',
                Value: currentRow
            }, {
                Name: 'NickName',
                Value: currentRow.NickName
            }, {
                Name: 'firstname',
                Value: currentRow.firstname
            }, {
                Name: 'lastname',
                Value: currentRow.lastname
            }, {
                Name: 'ugroup',
                Value: currentRow.ugroup
            }, {
                Name: 'discount',
                Value: currentRow.discount
            }, {
                Name: 'discount1x2',
                Value: currentRow.discount1x2
            }, {
                Name: 'discountcs',
                Value: currentRow.discountcs
            }, {
                Name: 'discountnumber',
                Value: currentRow.discountnumber
            }, {
                Name: 'discountmo',
                Value: currentRow.IsMyanmarOdds ? (currentRow.discountMyanmarOdds ? currentRow.discountMyanmarOdds : 0) : 'N/A'
            }, {
                Name: 'playerdiscount',
                Value: currentRow.playerdiscount
            }, {
                Name: 'playerdiscount1x2',
                Value: currentRow.playerdiscount1x2
            }, {
                Name: 'playerdiscountcs',
                Value: currentRow.playerdiscountcs
            }, {
                Name: 'playerdiscountnumber',
                Value: currentRow.playerdiscountnumber
            }, {
                Name: 'playerdiscountmo',
                Value: currentRow.IsMyanmarOdds ? (currentRow.playerdiscountMyanmarOdds ? currentRow.playerdiscountMyanmarOdds : 0) : 'N/A'
            }, {
                Name: 'createdate',
                Value: currentRow.createdate
            }, {
                Name: 'lastloginip',
                Value: currentRow.lastloginip
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
            HeaderText: 'Nickname',
            Name: 'NickName',
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
            HeaderText: 'User Group',
            Name: 'ugroup',
            MergeRowNumber: 2
        }, {
            HeaderText: 'Agent Commission',
            Name: 'discount',
            MergeColNumber: (isMyanmarOdds ? 5 : 4)
        }, {
            HeaderText: 'Player Commission',
            Name: 'playerdiscount',
            MergeColNumber: (isMyanmarOdds ? 5 : 4)
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

    if (!isMyanmarOdds) {
        for (var i = 0; i < bodyRows.length; i++) {
            bodyRows[i].Columns.splice(14, 1);
            bodyRows[i].Columns.splice(18, 1);
        }
    }

    var currentPage = 0;
    currentPage = (rowsLength <= Fanex.ExtAcc.Customer.ItemsPerPage || (parseInt(rowsLength / Fanex.ExtAcc.Customer.ItemsPerPage) < Fanex.ExtAcc.Customer.CurrentPage)) ? 0 : Fanex.ExtAcc.Customer.CurrentPage;
    Fanex.ExtAcc.Customer.CurrentPage = currentPage;
    var numberOfFrozenColumn = 6;
    var widthDevice = top.$(top.window).width();
    if (widthDevice <= 768) {
        numberOfFrozenColumn = 3;
    }

    options = {
        columns: this.DataFields,
        bodyRows: bodyRows,
        autoCalculateGridExpandHeight: true,
        numberOfFrozenColumn: numberOfFrozenColumn,
        bodyRowHeight: isMobile.phone ? 28 : 25,
        fullHeight: !isMobile.phone,
        gridHeight: isMobile.phone ? $(window).height() - 70 : 0,
        customizeHeader: true,
        groupHeaderRows: groupHeader,
        highlightFrozenWidth: 2,
        showPager: true,
        customPager: false,
        pagerOption: {
            itemsPerPageArray: [50, 100, 500],
            currentPage: Fanex.ExtAcc.Customer.CurrentPage,
            totalItem: rowsLength,
            itemsPerPage: Fanex.ExtAcc.Customer.ItemsPerPage,
            afterPagedFunction: function (options) {
                Fanex.ExtAcc.Customer.CurrentPage = options.currentPage;
                Fanex.ExtAcc.Customer.ItemsPerPage = options.itemsPerPage;
                Fanex.ExtAcc.Customer.DisableMultipleContextMenu();
            }
        },
        marginRight: 12,
        showDistinctRow: false,
        allowTruncate: true
    };

    this.Render(options);

    $('#customerListGrid').css('visibility', 'visible');

    $('#breadcrumbs').empty();

    var linkSuper = '<a href="' + "javascript:Fanex.ExtAcc.Customer.GetSuperList(" + Fanex.ExtAcc.Customer.directSuperId + ")" + '">' + 'Super' + '</a>';
    var linkMaster = '<a href="' + "javascript:Fanex.ExtAcc.Customer.GetMasterList("
        + Fanex.ExtAcc.Customer.superId + ','
        + Fanex.ExtAcc.Customer.AllowDoubleCommMaster + ','
        + Fanex.ExtAcc.Customer.directMasterId + ',null,'
        + Fanex.ExtAcc.Customer.superSyncCasino + ','
        + Fanex.ExtAcc.Customer.HideKeno + ','
        + Fanex.ExtAcc.Customer.HideVirtualSports + ','
        + Fanex.ExtAcc.Customer.HideGoldDeluxe + ','
        + Fanex.ExtAcc.Customer.HideColossusBet + ','
        + Fanex.ExtAcc.Customer.HideCricket + ')">' + 'Master' + '</a>';
    var linkAgent = '<a href="' + "javascript:Fanex.ExtAcc.Customer.GetAgentList("
        + Fanex.ExtAcc.Customer.masterId + ','
        + Fanex.ExtAcc.Customer.AllowDoubleCommAgent + ','
        + Fanex.ExtAcc.Customer.directAgentId + ',null,'
        + Fanex.ExtAcc.Customer.masterSyncCasino + ','
        + Fanex.ExtAcc.Customer.HideKeno + ','
        + Fanex.ExtAcc.Customer.HideVirtualSports + ','
        + Fanex.ExtAcc.Customer.HideGoldDeluxe + ','
        + Fanex.ExtAcc.Customer.HideColossusBet + ','
        + Fanex.ExtAcc.Customer.HideCricket + ')">' + 'Agent' + '</a>';

    $('#breadcrumbs').append('<div class="level-line"></div><div style="float:left">' + linkSuper + '</div><div class="viewNext">&nbsp;</div><div style="float:left">' + linkMaster + '</div><div class="viewNext">&nbsp;</div><div class="level-active">' + linkAgent + '</div>');

    $('#breadcrumbs').show();

    Fanex.ExtAcc.OnResize = function () {
        try {
            $("#jqgrid").fanexGrid("resizeGrid");
        } catch (err) { }
    }
};