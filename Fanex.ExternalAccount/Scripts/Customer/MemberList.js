Fanex.ExtAcc.Customer.ShowEditMember = function (
    custId,
    title,
    transferCondition,
    syncCasino,
    disableCasino,
    syncLiveCasino,
    disableLiveCasino,
    hideVirtualSport,
    disableVirtualSport,
    agentSyncCasino,
    agentSyncLiveCasino,
    isHideKeno,
    disableKeno,
    isHideGoldDeluxe,
    disableGoldDeluxe,
    isHideColossusBets,
    disableColossusBets,
    isHideSportsbook2,
    disableSportsbook2,
    isHideCockfighting,
    disableCockfighting,
    isHideCricket,
    disableCricket,
    isHideAllbet,
    disableAllbet,
    isHideVoidbridge,
    disableVoidbridge,
    isHideNG2,
    disableNG2,
    syncNG2,
    isHideFishingWorld,
    disableFishingWorld,
    isHideLottery,
    disableLottery,
    isHideLucky3,
    disableLucky3,
    isHideBullfighting,
    disableBullfighting) {
    this.TransferCondition = transferCondition;
    this.UserId = custId;
    this.TitleEditInfo = unescape(title);
    this.IsMultiple = false;
    this.HideVirtualSports = hideVirtualSport;
    this.HideKeno = showKenoSetting == 'false' || isHideKeno;
    this.HideGoldDeluxe = showGoldDeluxeSetting == 0 || isHideGoldDeluxe;
    this.HideColossusBet = showColossusBets == 0 || isHideColossusBets;
    this.HideSportsbook2 = showSportsbook2 == 0 || isHideSportsbook2;
    this.HideCockfighting = showCockfighting == 0 || isHideCockfighting;
    this.HideCricket = showCricket == 0 || isHideCricket;

    this.ResetFlags();

    var itemsMember = {
        sportbook: {
            name: "Sportsbook",
            className: "iconEnableEdit",
            callback: function () {
                Fanex.ExtAcc.Customer.UserId = custId;
                Fanex.ExtAcc.Customer.IsMultiple = false;
                Fanex.ExtAcc.Customer.IsFullInfo = true;
                Fanex.ExtAcc.Form.Show(
                    resolveClientUrl('SportsbookSetting/EditMultiplePT?roleId=1&groupName=' + Fanex.ExtAcc.Customer.GroupName().groupName + '&custId=' + Fanex.ExtAcc.Customer.UserId), 'Edit Sportsbook Setting ' + '<font color="#6dcff6">' + title + '</font>',
                    900,
                    Fanex.ExtAcc.Customer.GetPopupHeight(270, 480), false, false, 1);
            }
        },
        ptSportbook: {
            name: "PT",
            className: "iconEnableEdit",
            callback: function (key, opt) {
                Fanex.ExtAcc.Customer.UserId = custId;
                Fanex.ExtAcc.Customer.IsMultiple = false;
                Fanex.ExtAcc.Form.Show(
                    resolveClientUrl('SportsbookSetting/EditPT?groupName=' + Fanex.ExtAcc.Customer.GroupName().groupName + '&custId=' + Fanex.ExtAcc.Customer.UserId), 'Edit PT Setting ' + '<font color="#6dcff6">' + title + '</font>',
                    888,
                    Fanex.ExtAcc.Customer.GetPopupHeight(270, 480), false, false, 1);
            }
        },
        betSetting: {
            name: "Bet Setting",
            className: "iconEnableEdit",
            callback: function (key, opt) {
                Fanex.ExtAcc.Customer.UserId = custId;
                Fanex.ExtAcc.Customer.IsMultiple = false;
                Fanex.ExtAcc.Form.Show(
                    resolveClientUrl('SportsbookSetting/EditBetSetting?groupName=' + Fanex.ExtAcc.Customer.GroupName().groupName + '&custId=' + Fanex.ExtAcc.Customer.UserId), 'Edit Bet Setting ' + '<font color="#6dcff6">' + title + '</font>',
                    871,
                    Fanex.ExtAcc.Customer.GetPopupHeight(270, 480), false, false, 1);
            }
        },
        commission: {
            name: "Commission",
            className: "iconEnableEdit",
            callback: function (key, opt) {
                Fanex.ExtAcc.Customer.UserId = custId;
                Fanex.ExtAcc.Customer.IsMultiple = false;
                Fanex.ExtAcc.Form.Show(
                    resolveClientUrl('SportsbookSetting/EditCommissionMemberDetail?groupName=' + Fanex.ExtAcc.Customer.GroupName().groupName + '&custId=' + Fanex.ExtAcc.Customer.UserId), 'Edit Commission ' + '<font color="#6dcff6">' + title + '</font>',
                    876,
                    Fanex.ExtAcc.Customer.GetPopupHeight(270, 350), false, false, 1);
            }
        },
        separator: "-----",
        racing: {
            name: "Racing",
            className: "iconEnableEdit",
            callback: function (key, opt) {
                Fanex.ExtAcc.Customer.UserId = custId;
                Fanex.ExtAcc.Customer.IsMultiple = false;
                Fanex.ExtAcc.Form.ShowFrame(
                    resolveClientUrl('customersetting/racebook/racingsetting/member?id=' + Fanex.ExtAcc.Customer.UserId + '&uplineid=' + Fanex.ExtAcc.Customer.agentId + '&groupName=' + $('#selectedGroupName').html()), 'Edit Racing Setting <font color="#6dcff6">' + title + '</font>',
                    1050,
                    Fanex.ExtAcc.Customer.GetPopupHeight(270, 480), false, null, 1);
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
                    Fanex.ExtAcc.Form.ShowFrame(
                        resolveClientUrl('CustomerSetting/Casino/Member/EditMember?id=' + Fanex.ExtAcc.Customer.UserId + '&uplineid=' + Fanex.ExtAcc.Customer.agentId + '&groupname=' + $('#selectedGroupName').html()), 'Edit Casino Setting <font color="#6dcff6">' + title + '</font>',
                        1064,
                        Fanex.ExtAcc.Customer.GetPopupHeight(270, 480), false, null, 1);
            }
        };
    }

    if (agentSyncLiveCasino) {
        has3rdParty = true;

        itemsMember.liveCasino = {
            name: "Live Casino",
            className: syncLiveCasino == true ? (disableLiveCasino == true ? "iconDisableEdit" : "iconEnableEdit") : "iconUnsyncEdit",
            callback: function () {
                Fanex.ExtAcc.Customer.UserId = custId;
                url = "LiveCasinoSetting/Setting/EditSetting?uplineId=" + Fanex.ExtAcc.Customer.agentId + "&parentId=" + Fanex.ExtAcc.Customer.agentId + "&uplineLevel=2&customerId=" + Fanex.ExtAcc.Customer.UserId + "&customerLevel=1" + "&actorId=" + parent.userId + "&actorUsername=" + parent.username + "&groupName=" + parent.$('#selectedGroupName').html();
                Fanex.ExtAcc.Form.ShowFrame(
                    resolveClientUrl(url), 'Edit Live Casino Setting <font color="#6dcff6">' + title + '</font>',
                    900, Fanex.ExtAcc.Customer.GetPopupHeight(270, 480), false, false, 1);
            }
        };
    }

    function addEditProductMemberContextMenuItem(menuId, menuName, isDisabled, showEditPopUpFunc, isSynced) {
        isSynced = typeof (isSynced) == 'undefined' ? true : isSynced;

        itemsMember[menuId] = {
            name: menuName,
            className: !isSynced ? "iconUnsyncEdit" : (isDisabled ? "iconDisableEdit" : "iconEnableEdit"),
            callback: function () {
                    Fanex.ExtAcc.Customer.shouldCalculatePopupHeight = true;
                Fanex.ExtAcc.Customer.IsMultiple = false;
                Fanex.ExtAcc.Customer.IsFullInfo = true;
                showEditPopUpFunc(custId, 2, 1);
            }
        };
    }

    if (!hideVirtualSport) {
        has3rdParty = true;

        addEditProductMemberContextMenuItem('virtualSport', 'Virtual Sports', disableVirtualSport, Fanex.ExtAcc.Customer.ShowEditSingleVirtualSports);
    }

    if (!isHideKeno) {
        has3rdParty = true;

        itemsMember.keno = {
            name: "Keno",
            className: disableKeno ? "iconDisableEdit" : "iconEnableEdit",
            callback: function () {
                Fanex.ExtAcc.Customer.UserId = custId;
                Fanex.ExtAcc.Customer.IsMultiple = false;
                    Fanex.ExtAcc.Form.ShowFrame(
                        resolveClientUrl('customersetting/KenoSetting/kenosetting/AgentEditMember?custId=' + Fanex.ExtAcc.Customer.UserId + "&userId=" + parent.userId), 'Edit Keno Setting <font color="#6dcff6">' + title + '</font>',
                        900,
                        Fanex.ExtAcc.Customer.GetPopupHeight(270, 480), false, null, 1);
                    Fanex.ExtAcc.Form.ShowFrame(
                        resolveClientUrl('customersetting/KenoSetting/kenosetting/AgentEditMember?custId=' + Fanex.ExtAcc.Customer.UserId + "&userId=" + parent.userId), 'Edit Keno Setting <font color="#6dcff6">' + title + '</font>',
                        900, Fanex.ExtAcc.Customer.GetPopupHeight(270, 480), false, null, 1);
            }
        };
    }


    if (!this.HideGoldDeluxe) {
        has3rdParty = true;

        itemsMember.goldDeluxe = {
            name: "Gold Deluxe",
            className: disableGoldDeluxe == true ? "iconDisableEdit" : "iconEnableEdit",
            callback: function () {
                Fanex.ExtAcc.Customer.UserId = custId;
                url = "GoldDeluxeSetting/GoldDeluxe/EditSetting?upLineLevel=2&formType=1&level=1" + "&uplineId=" + Fanex.ExtAcc.Customer.agentId + "&actorId=" + parent.userId + "&custId=" + Fanex.ExtAcc.Customer.UserId + "&groupName=" + parent.$('#selectedGroupName').html();
                Fanex.ExtAcc.Form.ShowFrame(
                    resolveClientUrl(url), 'Edit Gold Deluxe Setting <font color="#6dcff6">' + title + '</font>',
                    900,
                    Fanex.ExtAcc.Customer.GetPopupHeight(270, 400), false, false, 1);
            }
        };
    }

    if (!this.HideColossusBet) {
        has3rdParty = true;

        itemsMember.colossusBets = {
            name: "Colossus Bets",
            className: disableColossusBets == true ? "iconDisableEdit" : "iconEnableEdit",
            callback: function () {
                Fanex.ExtAcc.Customer.UserId = custId;
                Fanex.ExtAcc.Customer.IsMultiple = false;
                url = "customersetting/ColossusBetSetting/ColossusBetSetting/EditSetting?custid=" + Fanex.ExtAcc.Customer.UserId + "&recommend=" + Fanex.ExtAcc.Customer.agentId + "&isMultipleEdit=0&userId=" + parent.userId + "&roleId=1&groupName=" + parent.$('#selectedGroupName').html();
                Fanex.ExtAcc.Form.ShowFrame(
                    resolveClientUrl(url), 'Edit ColossusBets <font color="#6dcff6">' + title + '</font>',
                    900,
                    Fanex.ExtAcc.Customer.GetPopupHeight(270, 400), false, false, 1);
            }
        };
    }

    if (!this.HideSportsbook2) {
        addEditProductMemberContextMenuItem('sportsbook2', 'Sportsbook 2', disableSportsbook2, Fanex.ExtAcc.Customer.ShowEditSingleSportsbook2);
    }

    if (!this.HideCockfighting) {
        addEditProductMemberContextMenuItem('cockFighting', 'Cockfighting', disableCockfighting, Fanex.ExtAcc.Customer.ShowEditSingleCockfighting);
    }

    if (!this.HideCricket) {
        addEditProductMemberContextMenuItem('cricket', 'Cricket', disableCricket, Fanex.ExtAcc.Customer.ShowEditSingleCricket);
    }

    if (!isHideAllbet) {
        addEditProductMemberContextMenuItem('allbet', 'Allbet', disableAllbet, Fanex.ExtAcc.Customer.ShowEditSingleAllbet);
    }

    if (!isHideVoidbridge) {
        addEditProductMemberContextMenuItem('voidbridge', 'Macau Games', disableVoidbridge, Fanex.ExtAcc.Customer.ShowEditSingleVoidbridge);
    }

    if (!isHideNG2) {
        addEditProductMemberContextMenuItem('ng2', ng2Text, disableNG2, Fanex.ExtAcc.Customer.ShowEditSingleNG2, syncNG2);
    }

    if (!isHideFishingWorld) {
        addEditProductMemberContextMenuItem('fishingWorld', 'Fishing World', disableFishingWorld, Fanex.ExtAcc.Customer.ShowEditSingleFishingWorld);
    }

    if (!isHideLottery) {
        addEditProductMemberContextMenuItem('lottery', 'Lottery', disableLottery, Fanex.ExtAcc.Customer.ShowEditSingleLottery);
    }

    if (!isHideLucky3) {
        addEditProductMemberContextMenuItem('lucky3', 'Lucky 3', disableLucky3, Fanex.ExtAcc.Customer.ShowEditSingleLucky3);
    }

    if (!isHideBullfighting) {
        addEditProductMemberContextMenuItem('bullfighting', 'Bullfighting', disableBullfighting, Fanex.ExtAcc.Customer.ShowEditSingleBullfighting);
    }

    if (has3rdParty)
        itemsMember.separator5 = "-----";
        itemsMember.editInfo = {
        name: "Edit Info",
        className: "iconEnableEdit editInfoBg",
        callback: function (key, opt) {
            Fanex.ExtAcc.Customer.UserId = custId;
            Fanex.ExtAcc.Customer.IsMultiple = false;
            Fanex.ExtAcc.Form.Show(
                resolveClientUrl('Customer/EditInfo/?custId=' + Fanex.ExtAcc.Customer.UserId + '&groupName=' + Fanex.ExtAcc.Customer.GroupName().groupName), titles[0] + ': <font color="#6dcff6">' + Fanex.ExtAcc.Customer.TitleEditInfo + '</font>',
                500,
                Fanex.ExtAcc.Customer.GetPopupHeight(270, 480),
                false);
        }
        };

    itemsMember.changePassword = {
        name: "Reset Password",
        className: "iconEnableEdit editInfoBg",
        callback: function (key, opt) {
            Fanex.ExtAcc.Customer.UserId = custId;
            Fanex.ExtAcc.Customer.IsMultiple = false;
            Fanex.ExtAcc.Form.Show(
                resolveClientUrl('Customer/ResetPassword/?custId=' + Fanex.ExtAcc.Customer.UserId + '&groupName=' + Fanex.ExtAcc.Customer.GroupName().groupName), 'Reset password: <font color="#6dcff6">' + Fanex.ExtAcc.Customer.TitleEditInfo + '</font>',
                415,
                Fanex.ExtAcc.Customer.GetPopupHeight(270, 400),
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
    agentSyncLiveCasino,
    isHideKeno,
    isHideVirtualSport,
    isHideGoldDeluxe,
    isHideColossusBets,
    isHideSportsbook2,
    isHideCockfighting,
    isHideCricket,
    isHideAllbet,
    isHideVoidbridge,
    isHideNG2,
    isHideFishingWorld,
    isHideLottery,
    isHideLucky3,
    isHideBullfighting) {
    Fanex.ExtAcc.BlockUI();

    this.agentId = recommend;
    this.CurrentLevel = this.Member;
    this.AllowDoubleComm = allow;

    this.agentSyncCasino = agentSyncCasino;
    this.agentSyncLiveCasino = agentSyncLiveCasino;
    this.HideKeno = showKenoSetting == 'false' || isHideKeno;
    this.HideVirtualSports = isHideVirtualSport;
    this.HideGoldDeluxe = isHideGoldDeluxe;
    this.HideColossusBet = isHideColossusBets
    this.HideSportsbook2 = isHideSportsbook2;
    this.HideCockfighting = isHideCockfighting;
    this.HideCricket = isHideCricket;

    this.HideAllbet = isHideAllbet;
    this.HideVoidbridge = isHideVoidbridge;
    this.HideNG2 = isHideNG2;
    this.HideFishingWorld = isHideFishingWorld;
    this.HideLottery = isHideLottery;
    this.HideLucky3 = isHideLucky3;
    this.HideBullfighting = isHideBullfighting;

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
            return '<div sync-ng2="' + cell.Value.SyncedNG2 +
                '" sync-voidbridge="' + cell.Value.SyncedVoidBridge +
                '" sync-allbet="' + cell.Value.SyncedAllBet +
                '" sync-fishingworld="' + cell.Value.SyncedFishingWorld +
                '" onclick="Fanex.ExtAcc.Customer.Select(this)" style="margin-left: 0px !important" id="' + cell.Value.custid +
                '" class="menuInactive" name="' + cell.Value.username +
                '" data-unsyncedproducts="' + Fanex.ExtAcc.Customer.GetUnsyncedProductIds(cell) + '"></div>';
        }
    }, {
        HeaderText: 'Username',
        Name: 'username',
        Width: 100,
        CustomCellRender: function (cell) {
            var len = cell.Value.username.length;
            var title = len >= 15 ? ' title="' + cell.Value.username + '"' : '';

            return '<div' + title + '>' + cell.Value.username + '</div>';
        }
    }, {
        HeaderText: 'Edit',
        Name: 'Edit',
        Width: 30,
        CustomCellRender: function (cell) {
            var title = escape(cell.Value.username);
            var hideKeno = showKenoSetting == 'false' || cell.Value.HideKeno == true;
            var hideGoldDeluxe = showGoldDeluxeSetting == 0 || !cell.Value.SyncGoldDeluxeUpline;
            var hideColossusBets = showColossusBets == 0 || cell.Value.HideColossusBet || isHideColossusBets;
            var hideSportsbook2 = showSportsbook2 == 0 || cell.Value.HideMuayThai || isHideSportsbook2;
            var hideCockfighting = showCockfighting == 0 || cell.Value.HideCockFighting || isHideCockfighting;
            var hideCricket = showCricket == 0 || cell.Value.HideCricket || isHideCricket || !cell.Value.IsIMBC;

            var hideAllbetSettingLink = !cell.Value.SyncedAllBetUpline || cell.Value.HideAllBetUpline;
            var hideVoidbridgeSettingLink = !cell.Value.SyncedVoidBridgeUpline || cell.Value.HideVoidBridgeUpline;
            var hideNG2SettingLink = !cell.Value.SyncedNG2Upline || cell.Value.HideNG2Upline;
            var hideFishingWorldSettingLink = cell.Value.HideFishingWorldUpline || !cell.Value.SyncedFishingWorldUpline;
            var hideLotterySettingLink = cell.Value.HideLotteryUpline;
            var hideLucky3SettingLink = cell.Value.HideLucky3Upline;
            var hideBullfightingSettingLink = cell.Value.HideBullFightingUpline;

            Fanex.ExtAcc.Customer.HideAllbetMember = hideAllbetSettingLink;
            Fanex.ExtAcc.Customer.HideVoidbridgeMember = hideVoidbridgeSettingLink;
            Fanex.ExtAcc.Customer.HideNG2Member = hideNG2SettingLink;
            Fanex.ExtAcc.Customer.HideFishingWorldMember = hideFishingWorldSettingLink;
            Fanex.ExtAcc.Customer.HideLotteryMember = hideLotterySettingLink;
            Fanex.ExtAcc.Customer.HideBullfightingMember = hideBullfightingSettingLink;

            var func = "javascript:Fanex.ExtAcc.Customer.ShowEditMember(" + cell.Value.custid + ", '" + title + "'," + cell.Value.disableTransfer + ", " + cell.Value.synccasino + ", " + cell.Value.disablecasino + ", " + cell.Value.SyncLiveCasino + ", " + cell.Value.DisableLiveCasino + ", " + cell.Value.HideVirtualSports + ", " + cell.Value.DisableVirtualSports + ", " + agentSyncCasino + ", " + agentSyncLiveCasino + "," + hideKeno + ", " + cell.Value.DisableKeno + "," + hideGoldDeluxe + ", " + cell.Value.DisableGoldDeluxe + "," + hideColossusBets + ", " + cell.Value.DisableColossusBet + "," + hideSportsbook2 + ", " + cell.Value.DisableMuayThai + "," + hideCockfighting + ", " + cell.Value.DisableCockFighting + "," + hideCricket + ", " + cell.Value.DisableCricket + "," + hideAllbetSettingLink + ", " + cell.Value.DisableAllBet + "," + hideVoidbridgeSettingLink + ", " + cell.Value.DisableVoidBridge + "," + hideNG2SettingLink + "," + cell.Value.DisableNG2 + "," + cell.Value.SyncedNG2 + "," + hideFishingWorldSettingLink + "," + cell.Value.DisableFishingWorld + "," + hideLotterySettingLink + "," + cell.Value.DisableLottery + "," + hideLucky3SettingLink + "," + cell.Value.DisableLucky3 + "," + hideBullfightingSettingLink + "," + cell.Value.DisableBullFighting + ")";

            return '<div onclick="' + func + '" id="edit-' + cell.Value.custid + '" class="editMember"></div>';
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
        Width: 57,
        CellAlign: 'center',
        CustomCellRender: function (cell) {
            var css = '';

            if (!Fanex.ExtAcc.Customer.AllowDoubleComm) {
                css = 'style="cursor:default !important"';
            }

            if (cell.Value.DoubleCommLosing) {
                return '<div onclick="Fanex.ExtAcc.Customer.ConfigDoubleComm(' + cell.Value.custid + ')" id="dbl-' + cell.Value.custid + '" class="onDblComm" ' + css + '></div>';
            }

            return '<div onclick="Fanex.ExtAcc.Customer.ConfigDoubleComm(' + cell.Value.custid + ')" id="dbl-' + cell.Value.custid + '" class="offDblComm" ' + css + '></div>';
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

    this.DataSource = rows;
    var isMyanmarOdds = rows.length > 0 ? rows[0].IsMyanmarOdds : false;
    for (var i = 0; i < rows.length; i++) {
        var cssRow = !rows[i].enabled ? 'rowDisabled' : rows[i].closed ? Fanex.ExtAcc.Customer.GetClosedClassName(rows[i]) : rows[i].suspended ? 'rowSuspended' : '';

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
                Name: 'NickName',
                Value: rows[i].NickName
            }, {
                Name: 'firstname',
                Value: rows[i].firstname
            }, {
                Name: 'lastname',
                Value: rows[i].lastname
            }, {
                Name: 'ugroup',
                Value: rows[i].ugroup
            }, {
                Name: 'discount',
                Value: rows[i].discount
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
                Name: 'playerdiscount',
                Value: rows[i].playerdiscount
            }, {
                Name: 'playerdiscount1x2',
                Value: rows[i].playerdiscount1x2
            }, {
                Name: 'playerdiscountcs',
                Value: rows[i].playerdiscountcs
            }, {
                Name: 'playerdiscountnumber',
                Value: rows[i].playerdiscountnumber
            }, {
                Name: 'playerdiscountmo',
                Value: rows[i].IsMyanmarOdds ? (rows[i].playerdiscountMyanmarOdds ? rows[i].playerdiscountMyanmarOdds : 0) : 'N/A'
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
    currentPage = (rows.length <= Fanex.ExtAcc.Customer.ItemsPerPage || (parseInt(rows.length / Fanex.ExtAcc.Customer.ItemsPerPage) < Fanex.ExtAcc.Customer.CurrentPage)) ? 0 : Fanex.ExtAcc.Customer.CurrentPage;
    Fanex.ExtAcc.Customer.CurrentPage = currentPage;

    options = {
        columns: this.DataFields,
        bodyRows: bodyRows,
        autoCalculateGridExpandHeight: true,
        numberOfFrozenColumn: isMobile.phone ? 3 : 6,
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
            totalItem: rows.length,
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

    var len = rows.length;
    var parentItem = rows[0];
    var hideAB = Fanex.ExtAcc.Customer.HideAllbet = len > 0 ? (parentItem.HideAllBetUpline || !parentItem.SyncedAllBetUpline) : Fanex.ExtAcc.Customer.HideAllbet;
    var hideVB = Fanex.ExtAcc.Customer.HideVoidbridge = len > 0 ? (parentItem.HideVoidBridgeUpline || !parentItem.SyncedVoidBridgeUpline) : Fanex.ExtAcc.Customer.HideVoidbridge;
    var hideNG2 = Fanex.ExtAcc.Customer.HideNG2 = len > 0 ? (parentItem.HideNG2Upline || !parentItem.SyncedNG2Upline) : Fanex.ExtAcc.Customer.HideNG2;
    var hideFishingWorld = Fanex.ExtAcc.Customer.HideFishingWorld = len > 0 ? (parentItem.HideFishingWorldUpline || !parentItem.SyncedFishingWorldUpline) : Fanex.ExtAcc.Customer.HideFishingWorld;
    var hideLottery = Fanex.ExtAcc.Customer.HideLottery = len > 0 ? (parentItem.HideLotteryUpline) : Fanex.ExtAcc.Customer.HideLottery;
    var hideLucky3 = Fanex.ExtAcc.Customer.HideLucky3 = len > 0 ? (parentItem.HideLucky3Upline) : Fanex.ExtAcc.Customer.HideLucky3;
    var hideBullfighting = Fanex.ExtAcc.Customer.HideBullfighting = len > 0 ? (parentItem.HideBullFightingUpline) : Fanex.ExtAcc.Customer.HideBullfighting;

    var linkSuper = '<a href="' + "javascript:Fanex.ExtAcc.Customer.GetSuperList(" + Fanex.ExtAcc.Customer.directSuperId + ")" + '">' + 'Super' + '</a>';
    var linkMaster = '<a href="' + "javascript:Fanex.ExtAcc.Customer.GetMasterList(" + Fanex.ExtAcc.Customer.superId + ', ' + Fanex.ExtAcc.Customer.AllowDoubleCommMaster + ',' + Fanex.ExtAcc.Customer.directMasterId + ', null, ' + Fanex.ExtAcc.Customer.superSyncCasino + ',' + Fanex.ExtAcc.Customer.superSyncLiveCasino + ',' + Fanex.ExtAcc.Customer.HideKeno + ',' + Fanex.ExtAcc.Customer.HideVirtualSports + ',' + Fanex.ExtAcc.Customer.HideGoldDeluxe + ',' + Fanex.ExtAcc.Customer.HideColossusBet + ',' + Fanex.ExtAcc.Customer.HideSportsbook2 + ',' + Fanex.ExtAcc.Customer.HideCockfighting + ',' + Fanex.ExtAcc.Customer.HideCricket + ',' + hideAB + ',' + hideVB + ',' + hideNG2 + ',' + hideFishingWorld + ',' + hideLottery + ',' + hideLucky3 + ',' + hideBullfighting + ')">' + 'Master' + '</a>';
    var linkAgent = '<a href="' + "javascript:Fanex.ExtAcc.Customer.GetAgentList(" + Fanex.ExtAcc.Customer.masterId + ', ' + Fanex.ExtAcc.Customer.AllowDoubleCommAgent + ',' + Fanex.ExtAcc.Customer.directAgentId + ', null, ' + Fanex.ExtAcc.Customer.masterSyncCasino + ',' + Fanex.ExtAcc.Customer.masterSyncLiveCasino + ',' + Fanex.ExtAcc.Customer.HideKeno + ',' + Fanex.ExtAcc.Customer.HideVirtualSports + ',' + Fanex.ExtAcc.Customer.HideGoldDeluxe + ',' + Fanex.ExtAcc.Customer.HideColossusBet + ',' + Fanex.ExtAcc.Customer.HideSportsbook2 + ',' + Fanex.ExtAcc.Customer.HideCockfighting + ',' + Fanex.ExtAcc.Customer.HideCricket + ',' + hideAB + ',' + hideVB + ',' + hideNG2 + ',' + hideFishingWorld + ',' + hideLottery + ',' + hideLucky3 + ',' + hideBullfighting + ')">' + 'Agent' + '</a>';

    $('#breadcrumbs').append('<div class="level-line"></div><div style="float:left">' + linkSuper + '</div><div class="viewNext">&nbsp;</div><div style="float:left">' + linkMaster + '</div><div class="viewNext">&nbsp;</div><div class="level-active">' + linkAgent + '</div>');

    $('#breadcrumbs').show();

    Fanex.ExtAcc.OnResize = function () {
        try {
            $("#jqgrid").fanexGrid("resizeGrid");
        } catch (err) { }
    }
};