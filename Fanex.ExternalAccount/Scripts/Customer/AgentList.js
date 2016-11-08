Fanex.ExtAcc.Customer.ShowEditAgent = function (
    custId,
    title,
    transferCondition,
    syncCasino,
    disableCasino,
    syncBingo,
    disableBingo,
    disableP2P,
    syncLiveCasino,
    disableLiveCasino,
    hideVirtualSport,
    disableVirtualSport,
    masterSyncCasino,
    masterSyncBingo,
    masterSyncLiveCasino,
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
    syncNG2) {
    this.TransferCondition = transferCondition;
    this.UserId = custId;
    this.TitleEditInfo = unescape(title);
    this.HideVirtualSports = hideVirtualSport;
    this.HideKeno = showKenoSetting == 'false' || isHideKeno;
    this.HideGoldDeluxe = showGoldDeluxeSetting == 0 || isHideGoldDeluxe;
    this.HideColossusBet = showColossusBets == 0 || isHideColossusBets;
    this.HideSportsbook2 = showSportsbook2 == 0 || isHideSportsbook2;
    this.HideCockfighting = showCockfighting == 0 || isHideCockfighting;
    this.HideCricket = showCricket == 0 || isHideCricket;
    
    this.ResetFlags();

    var itemsAgent = {
        sportbook: {
            name: "Sportsbook",
            className: "iconEnableEdit",
            callback: function () {
                Fanex.ExtAcc.Customer.UserId = custId;
                Fanex.ExtAcc.Customer.IsMultiple = false;
                Fanex.ExtAcc.Customer.IsFullInfo = true;
                Fanex.ExtAcc.Form.Show(resolveClientUrl('SportsbookSetting/EditMultiplePT?roleId=2&groupName=' + Fanex.ExtAcc.Customer.GroupName().groupName + '&custId=' + Fanex.ExtAcc.Customer.UserId), 'Edit Sportsbook Setting ' + '<font color="#6dcff6">' + title + '</font>', 900, 700, false, false, 1);
            }
        },
        ptSportbook: {
            name: "PT",
            className: "iconEnableEdit",
            callback: function (key, opt) {
                Fanex.ExtAcc.Customer.UserId = custId;
                Fanex.ExtAcc.Customer.IsMultiple = false;
                Fanex.ExtAcc.Form.Show(resolveClientUrl('SportsbookSetting/EditPT?groupName=' + Fanex.ExtAcc.Customer.GroupName().groupName + '&custId=' + Fanex.ExtAcc.Customer.UserId), 'Edit PT Setting ' + '<font color="#6dcff6">' + title + '</font>', 888, 700, false, false, 1);
            }
        },
        minPTSportbook: {
            name: "Min PT",
            className: "iconEnableEdit",
            callback: function (key, opt) {
                Fanex.ExtAcc.Customer.UserId = custId;
                Fanex.ExtAcc.Customer.IsMultiple = false;
                Fanex.ExtAcc.Form.Show(resolveClientUrl('SportsbookSetting/EditMinPT?groupName=' + Fanex.ExtAcc.Customer.GroupName().groupName + '&custId=' + Fanex.ExtAcc.Customer.UserId), 'Edit Min PT ' + '<font color="#6dcff6">' + title + '</font>', 888, 700, false, false, 1);
            }
        },
        betSetting: {
            name: "Bet Setting",
            className: "iconEnableEdit",
            callback: function (key, opt) {
                Fanex.ExtAcc.Customer.UserId = custId;
                Fanex.ExtAcc.Customer.IsMultiple = false;
                Fanex.ExtAcc.Form.Show(resolveClientUrl('SportsbookSetting/EditBetSetting?groupName=' + Fanex.ExtAcc.Customer.GroupName().groupName + '&custId=' + Fanex.ExtAcc.Customer.UserId), 'Edit Bet Setting ' + '<font color="#6dcff6">' + title + '</font>', 871, 490, false, false, 1);
            }
        },
        commission: {
            name: "Commission",
            className: "iconEnableEdit",
            callback: function (key, opt) {
                Fanex.ExtAcc.Customer.UserId = custId;
                Fanex.ExtAcc.Customer.IsMultiple = false;
                Fanex.ExtAcc.Form.Show(resolveClientUrl('SportsbookSetting/EditCommissionDetail?groupName=' + Fanex.ExtAcc.Customer.GroupName().groupName + '&custId=' + Fanex.ExtAcc.Customer.UserId), 'Edit Commission ' + '<font color="#6dcff6">' + title + '</font>', 871, 250, false, false, 1);
            }
        },
        separator: "-----",
        racing: {
            name: "Racing",
            className: "iconEnableEdit",
            callback: function (key, opt) {
                Fanex.ExtAcc.Customer.UserId = custId;
                Fanex.ExtAcc.Customer.IsMultiple = false;
                Fanex.ExtAcc.Form.ShowFrame(resolveClientUrl('customersetting/racebook/racingsetting/agent?id=' + Fanex.ExtAcc.Customer.UserId + '&uplineid=' + Fanex.ExtAcc.Customer.masterId + '&groupName=' + $('#selectedGroupName').html()), 'Edit Racing Setting <font color="#6dcff6">' + title + '</font>', 1050, 775, false, null, 1);
            }
        },
        minPTRacing: {
            name: "Min PT",
            className: "iconEnableEdit",
            callback: function (key, opt) {
                Fanex.ExtAcc.Customer.UserId = custId;
                Fanex.ExtAcc.Customer.IsMultiple = false;
                var url = 'CustomerSetting/Racebook/RacingSetting/AgentMinMaxPT?id=' + Fanex.ExtAcc.Customer.UserId + '&roleId=' + Fanex.ExtAcc.Customer.CurrentLevel;
                Fanex.ExtAcc.Form.ShowFrame(resolveClientUrl(url + '&groupName=' + $('#selectedGroupName').html()), 'Edit Min PT Racing: <font color="#6dcff6">' + title + '</font>', 980, 680);
            }
        },
        separator1: "-----"
    };

    var has3rdParty = false;
    if (masterSyncCasino) {
        has3rdParty = true;
        if (syncCasino) {
            if (disableCasino) {
                itemsAgent.casino = {
                    name: "Casino",
                    className: "iconDisableEdit",
                    callback: function () {
                        Fanex.ExtAcc.Customer.UserId = custId;
                        Fanex.ExtAcc.Customer.IsMultiple = false;
                        Fanex.ExtAcc.Form.ShowFrame(resolveClientUrl('CustomerSetting/Casino/Agent/EditAgent/' + Fanex.ExtAcc.Customer.UserId + '/' + Fanex.ExtAcc.Customer.masterId + '/' + $('#selectedGroupName').html()), 'Edit Casino Setting <font color="#6dcff6">' + title + '</font>', 1064, 700, false, null, 1);
                    }
                };
                /*itemsAgent.scasino = {
                name: "Super Edit Casino",
                className: "iconViewEdit",
                callback: function () {
                Fanex.ExtAcc.Customer.UserId = custId;
                Fanex.ExtAcc.Customer.IsMultiple = false;
                Fanex.ExtAcc.Form.ShowFrame(resolveClientUrl('CustomerSetting/Casino/Agent/ViewAgentBySuper/' + Fanex.ExtAcc.Customer.UserId + '/' + Fanex.ExtAcc.Customer.superId + '/' + $('#selectedGroupName').html()), 'Super View Casino Setting <font color="#6dcff6">' + title + '</font>', 1064, 700, false, null, 1);
                }
                };*/
            } else {
                itemsAgent.casino = {
                    name: "Casino",
                    className: "iconEnableEdit",
                    callback: function () {
                        Fanex.ExtAcc.Customer.UserId = custId;
                        Fanex.ExtAcc.Customer.IsMultiple = false;
                        Fanex.ExtAcc.Form.ShowFrame(resolveClientUrl('CustomerSetting/Casino/Agent/EditAgent/' + Fanex.ExtAcc.Customer.UserId + '/' + Fanex.ExtAcc.Customer.masterId + '/' + $('#selectedGroupName').html()), 'Edit Casino Setting <font color="#6dcff6">' + title + '</font>', 1064, 700, false, null, 1);
                    }
                };
                /*itemsAgent.scasino = {
                name: "Super Edit Casino",
                className: "iconViewEdit",
                callback: function () {
                Fanex.ExtAcc.Customer.UserId = custId;
                Fanex.ExtAcc.Customer.IsMultiple = false;
                Fanex.ExtAcc.Form.ShowFrame(resolveClientUrl('CustomerSetting/Casino/Agent/ViewAgentBySuper/' + Fanex.ExtAcc.Customer.UserId + '/' + Fanex.ExtAcc.Customer.superId + '/' + $('#selectedGroupName').html()), 'Super View Casino Setting <font color="#6dcff6">' + title + '</font>', 1064, 700, false, null, 1);
                }
                };*/
            }
        } else {
            itemsAgent.casino = {
                name: "Casino",
                className: "iconUnsyncEdit",
                callback: function () {
                    Fanex.ExtAcc.Customer.UserId = custId;
                    Fanex.ExtAcc.Customer.IsMultiple = false;
                    Fanex.ExtAcc.Form.ShowFrame(resolveClientUrl('CustomerSetting/Casino/Agent/EditAgent/' + Fanex.ExtAcc.Customer.UserId + '/' + Fanex.ExtAcc.Customer.masterId + '/' + $('#selectedGroupName').html()), 'Edit Casino Setting <font color="#6dcff6">' + title + '</font>', 1064, 700, false, null, 1);
                }
            };
        }
    }

    if (masterSyncLiveCasino) {
        has3rdParty = true;

        itemsAgent.liveCasino = {
            name: "Live Casino",
            className: syncLiveCasino == true ? (disableLiveCasino == true ? "iconDisableEdit" : "iconEnableEdit") : "iconUnsyncEdit",
            callback: function () {
                Fanex.ExtAcc.Customer.UserId = custId;
                url = "LiveCasinoSetting/Setting/EditSetting?uplineId=" + Fanex.ExtAcc.Customer.masterId + "&parentId=" + Fanex.ExtAcc.Customer.masterId + "&uplineLevel=3&customerId=" + Fanex.ExtAcc.Customer.UserId + "&customerLevel=2" + "&actorId=" + parent.userId + "&actorUsername=" + parent.username + "&groupName=" + parent.$('#selectedGroupName').html();
                Fanex.ExtAcc.Form.ShowFrame(resolveClientUrl(url), 'Edit Live Casino Setting <font color="#6dcff6">' + title + '</font>', 900, 700, false, false, 1);
            }
        };
    }

    if (!hideBingo && masterSyncBingo) {
        has3rdParty = true;
        if (syncBingo) {
            if (disableBingo) {
                itemsAgent.bingo = {
                    name: "Bingo",
                    className: "iconDisableEdit",
                    callback: function (key, opt) {
                        Fanex.ExtAcc.Customer.UserId = custId;
                        Fanex.ExtAcc.Customer.IsMultiple = false;
                        Fanex.ExtAcc.Form.ShowFrame(resolveClientUrl('CustomerSetting/Bingo/BingoSetting/Agent?id=' + Fanex.ExtAcc.Customer.UserId + '&uplineid=' + Fanex.ExtAcc.Customer.masterId + '&groupName=' + $('#selectedGroupName').html()), 'Edit Bingo Setting <font color="#6dcff6">' + title + '</font>', 1050, 500, false, null, 1);
                    }
                };
            } else {
                itemsAgent.bingo = {
                    name: "Bingo",
                    className: "iconEnableEdit",
                    callback: function (key, opt) {
                        Fanex.ExtAcc.Customer.UserId = custId;
                        Fanex.ExtAcc.Customer.IsMultiple = false;
                        Fanex.ExtAcc.Form.ShowFrame(resolveClientUrl('CustomerSetting/Bingo/BingoSetting/Agent?id=' + Fanex.ExtAcc.Customer.UserId + '&uplineid=' + Fanex.ExtAcc.Customer.masterId + '&groupName=' + $('#selectedGroupName').html()), 'Edit Bingo Setting <font color="#6dcff6">' + title + '</font>', 1050, 500, false, null, 1);
                    }
                };
            }
        } else {
            itemsAgent.bingo = {
                name: "Bingo",
                className: "iconUnsyncEdit",
                callback: function (key, opt) {
                    Fanex.ExtAcc.Customer.UserId = custId;
                    Fanex.ExtAcc.Customer.IsMultiple = false;
                    Fanex.ExtAcc.Form.ShowFrame(resolveClientUrl('CustomerSetting/Bingo/BingoSetting/Agent?id=' + Fanex.ExtAcc.Customer.UserId + '&uplineid=' + Fanex.ExtAcc.Customer.masterId + '&groupName=' + $('#selectedGroupName').html()), 'Edit Bingo Setting <font color="#6dcff6">' + title + '</font>', 1050, 500, false, null, 1);
                }
            };
        }
    }

    if (!disableP2P) {
        //itemsAgent.poker = { name: "Poker", className: "iconEnableEdit" };
    }

    if (!hideVirtualSport) {
        has3rdParty = true;
        if (disableVirtualSport) {
            itemsAgent.virtualSport = {
                name: "Virtual Sports",
                className: "iconDisableEdit",
                callback: function () {
                    Fanex.ExtAcc.Customer.UserId = custId;
                    Fanex.ExtAcc.Customer.IsMultiple = false;
                    Fanex.ExtAcc.Form.Show(resolveClientUrl('VirtualSports/EditSingle?custId=' + Fanex.ExtAcc.Customer.masterId + '&roleId=' + Fanex.ExtAcc.Customer.Master + '&targetCustId=' + Fanex.ExtAcc.Customer.UserId + '&targetRoleId=' + Fanex.ExtAcc.Customer.Agent + '&groupName=' + Fanex.ExtAcc.Customer.GroupName().groupName), 'Edit Virtual Sports Setting <font color="#6dcff6">' + title + '</font>', 880, 700, false, null, 1);
                }
            };
        } else {
            itemsAgent.virtualSport = {
                name: "Virtual Sports",
                className: "iconEnableEdit",
                callback: function () {
                    Fanex.ExtAcc.Customer.UserId = custId;
                    Fanex.ExtAcc.Customer.IsMultiple = false;
                    Fanex.ExtAcc.Form.Show(resolveClientUrl('VirtualSports/EditSingle?custId=' + Fanex.ExtAcc.Customer.masterId + '&roleId=' + Fanex.ExtAcc.Customer.Master + '&targetCustId=' + Fanex.ExtAcc.Customer.UserId + '&targetRoleId=' + Fanex.ExtAcc.Customer.Agent + '&groupName=' + Fanex.ExtAcc.Customer.GroupName().groupName), 'Edit Virtual Sports Setting <font color="#6dcff6">' + title + '</font>', 880, 700, false, null, 1);
                }
            };
        }
    }

    if (!isHideKeno) {
        has3rdParty = true;
        if (disableKeno) {
            itemsAgent.keno = {
                name: "Keno",
                className: "iconDisableEdit",
                callback: function () {
                    Fanex.ExtAcc.Customer.UserId = custId;
                    Fanex.ExtAcc.Customer.IsMultiple = false;
                    Fanex.ExtAcc.Form.ShowFrame(resolveClientUrl('customersetting/KenoSetting/kenosetting/EditAgent?custId=' + Fanex.ExtAcc.Customer.UserId + "&userId=" + parent.userId), 'Edit Keno Setting <font color="#6dcff6">' + title + '</font>', 900, 700, false, null, 1);
                }
            };
        }
        else {
            itemsAgent.keno = {
                name: "Keno",
                className: "iconEnableEdit",
                callback: function () {
                    Fanex.ExtAcc.Customer.UserId = custId;
                    Fanex.ExtAcc.Customer.IsMultiple = false;
                    Fanex.ExtAcc.Form.ShowFrame(resolveClientUrl('customersetting/KenoSetting/kenosetting/EditAgent?custId=' + Fanex.ExtAcc.Customer.UserId + "&userId=" + parent.userId), 'Edit Keno Setting <font color="#6dcff6">' + title + '</font>', 900, 700, false, null, 1);
                }
            };
        }
    }

    if (!this.HideGoldDeluxe) {
        has3rdParty = true;

        itemsAgent.goldDeluxe = {
            name: "Gold Deluxe",
            className: disableGoldDeluxe == true ? "iconDisableEdit" : "iconEnableEdit",
            callback: function () {
                Fanex.ExtAcc.Customer.UserId = custId;
                url = "GoldDeluxeSetting/GoldDeluxe/EditSetting?upLineLevel=3&formType=1&level=2" + "&uplineId=" + Fanex.ExtAcc.Customer.masterId + "&actorId=" + parent.userId + "&custId=" + Fanex.ExtAcc.Customer.UserId + "&groupName=" + parent.$('#selectedGroupName').html();
                Fanex.ExtAcc.Form.ShowFrame(resolveClientUrl(url), 'Edit Gold Deluxe Setting <font color="#6dcff6">' + title + '</font>', 900, 400, false, false, 1);
            }
        };
    }

    if (!this.HideColossusBet) {
        has3rdParty = true;

        itemsAgent.colossusBets = {
            name: "Colossus Bets",
            className: disableColossusBets == true ? "iconDisableEdit" : "iconEnableEdit",
            callback: function () {
                Fanex.ExtAcc.Customer.UserId = custId;
                Fanex.ExtAcc.Customer.IsMultiple = false;
                url = "customersetting/ColossusBetSetting/ColossusBetSetting/EditSetting?custid=" + Fanex.ExtAcc.Customer.UserId + "&mrecommend=" + Fanex.ExtAcc.Customer.masterId + "&isMultipleEdit=0&userId=" + parent.userId + "&roleId=2&groupName=" + parent.$('#selectedGroupName').html();
                Fanex.ExtAcc.Form.ShowFrame(resolveClientUrl(url), 'Edit ColossusBets Setting <font color="#6dcff6">' + title + '</font>', 900, 400, false, false, 1);
            }
        };
    }


    if (!this.HideSportsbook2) {
        itemsAgent.sportsbook2 = {
            name: "Sportsbook2",
            className: disableSportsbook2 == true ? "iconDisableEdit" : "iconEnableEdit",
            callback: function () {
                Fanex.ExtAcc.Customer.IsMultiple = false;
                Fanex.ExtAcc.Customer.IsMultiple = false;
                Fanex.ExtAcc.Customer.IsFullInfo = true;
                Fanex.ExtAcc.Customer.ShowEditSingleSportsbook2(custId, 3, 2);
            }
        };
    }

    if (!this.HideCockfighting ) {
        itemsAgent.cockFighting = {
            name: "Cockfighting",
            className: disableCockfighting == true ? "iconDisableEdit" : "iconEnableEdit",
            callback: function () {
                Fanex.ExtAcc.Customer.IsMultiple = false;
                Fanex.ExtAcc.Customer.IsMultiple = false;
                Fanex.ExtAcc.Customer.IsFullInfo = true;
                Fanex.ExtAcc.Customer.ShowEditSingleCockfighting(custId, 3, 2);
            }
        };
    }

    if (!this.HideCricket) {
        itemsAgent.cricket = {
            name: "Cricket",
            className: disableCricket == true ? "iconDisableEdit" : "iconEnableEdit",
            callback: function () {
                Fanex.ExtAcc.Customer.IsMultiple = false;
                Fanex.ExtAcc.Customer.IsMultiple = false;
                Fanex.ExtAcc.Customer.IsFullInfo = true;
                Fanex.ExtAcc.Customer.ShowEditSingleCricket(custId, 3, 2);
            }
        };
    }

    if (!isHideAllbet) {
        itemsAgent.allbet = {
            name: "Allbet",
            className: disableAllbet == true ? "iconDisableEdit" : "iconEnableEdit",
            callback: function () {
                Fanex.ExtAcc.Customer.IsMultiple = false;
                Fanex.ExtAcc.Customer.IsMultiple = false;
                Fanex.ExtAcc.Customer.IsFullInfo = true;
                Fanex.ExtAcc.Customer.ShowEditSingleAllbet(custId, 3, 2);
            }
        };
    }

    if (!isHideVoidbridge) {
        itemsAgent.voidbridge = {
            name: "Macau Games",
            className: disableVoidbridge == true ? "iconDisableEdit" : "iconEnableEdit",
            callback: function () {
                Fanex.ExtAcc.Customer.IsMultiple = false;
                Fanex.ExtAcc.Customer.IsMultiple = false;
                Fanex.ExtAcc.Customer.IsFullInfo = true;
                Fanex.ExtAcc.Customer.ShowEditSingleVoidbridge(custId, 3, 2);
            }
        };
    }

    if (!isHideNG2) {
        itemsAgent.ng2 = {
            name: ng2Text,
            className: !syncNG2 ? "iconUnsyncEdit" : (disableNG2 == true ? "iconDisableEdit" : "iconEnableEdit"),
            callback: function () {
                Fanex.ExtAcc.Customer.IsMultiple = false;
                Fanex.ExtAcc.Customer.IsMultiple = false;
                Fanex.ExtAcc.Customer.IsFullInfo = true;
                Fanex.ExtAcc.Customer.ShowEditSingleNG2(custId, 3, 2);
            }
        };
    }


    if (has3rdParty)
        itemsAgent.separator2 = "-----";
    itemsAgent.editInfo = {
        name: "Edit Info",
        className: "iconEnableEdit editInfoBg",
        callback: function (key, opt) {
            Fanex.ExtAcc.Customer.UserId = custId;
            Fanex.ExtAcc.Customer.IsMultiple = false;
            Fanex.ExtAcc.Form.Show(resolveClientUrl('Customer/EditInfo/?custId=' + Fanex.ExtAcc.Customer.UserId + '&groupName=' + Fanex.ExtAcc.Customer.GroupName().groupName), titles[0] + ': <font color="#6dcff6">' + Fanex.ExtAcc.Customer.TitleEditInfo + '</font>', 500, 460, false);
        }
    };
    itemsAgent.changePassword = {
        name: "Reset Password",
        className: "iconEnableEdit editInfoBg",
        callback: function (key, opt) {
            Fanex.ExtAcc.Customer.UserId = custId;
            Fanex.ExtAcc.Customer.IsMultiple = false;
            Fanex.ExtAcc.Form.Show(resolveClientUrl('Customer/ResetPassword/?custId=' + Fanex.ExtAcc.Customer.UserId + '&groupName=' + Fanex.ExtAcc.Customer.GroupName().groupName), 'Reset password: <font color="#6dcff6">' + Fanex.ExtAcc.Customer.TitleEditInfo + '</font>', 415, 400, false);
        }
    };

    if (transferCondition == false) {
        itemsAgent.transferCondition = {
            name: "Transfer Condition",
            className: "iconEnableEdit editInfoBg",
            callback: function (key, opt) {
                Fanex.ExtAcc.Customer.UserId = custId;
                Fanex.ExtAcc.Customer.IsMultiple = false;
                Fanex.ExtAcc.Form.Show(resolveClientUrl('Customer/EditTransferCondition/?custId=' + Fanex.ExtAcc.Customer.UserId + '&groupName=' + Fanex.ExtAcc.Customer.GroupName().groupName), titles[1] + ': <font color="#6dcff6">' + Fanex.ExtAcc.Customer.TitleEditInfo + '</font>', 600, 250, false, false, 1);
            }
        };
    }

    if ($.contextMenu) $.contextMenu('destroy');

    $.contextMenu({
        selector: '.editAgent',
        items: itemsAgent,
        trigger: "left",
        ignoreRightClick: true,
        autoHide: true
    });
};

Fanex.ExtAcc.Customer.GetAgentList = function (
    mrecommend,
    allow,
    custId,
    dataSource,
    masterSyncCasino,
    masterSyncBingo,
    masterSyncLiveCasino,
    isHideKeno,
    isHideVirtualSport,
    isHideGoldDeluxe,
    isHideColossusBets,
    isHideSportsbook2,
    isHideCockfighting,
    isHideCricket,
    isHideAllbet,
    isHideVoidbridge,
    isHideNG2) {
    Fanex.ExtAcc.BlockUI();

    this.masterId = mrecommend;
    this.CurrentLevel = this.Agent;
    this.AllowDoubleComm = allow;
    this.AllowDoubleCommAgent = allow;

    this.masterSyncCasino = masterSyncCasino;
    this.masterSyncBingo = masterSyncBingo;
    this.masterSyncLiveCasino = masterSyncLiveCasino;
    this.HideKeno = showKenoSetting == 'false' || isHideKeno;
    this.HideVirtualSports = isHideVirtualSport;
    this.HideGoldDeluxe = isHideGoldDeluxe;
    this.HideColossusBet = isHideColossusBets;
    this.HideSportsbook2 = isHideSportsbook2;
    this.HideCockfighting = isHideCockfighting;
    this.HideCricket = isHideCricket;

    this.HideAllbet = isHideAllbet;
    this.HideVoidbridge = isHideVoidbridge;
    this.HideNG2 = isHideNG2;

    var url = resolveClientUrl('Customer/AgentListPost');
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
            Fanex.ExtAcc.Customer.agentDisableP2P = cell.Value.DisableP2P;

            var hideAllbetSettingLink = !cell.Value.SyncedAllBetUpline || cell.Value.HideAllBetUpline || !cell.Value.SyncedAllBet;
            var hideVoidbridgeSettingLink = !cell.Value.SyncedVoidBridgeUpline || cell.Value.HideVoidBridgeUpline || !cell.Value.SyncedVoidBridge;
            var hideNG2SettingLink = !cell.Value.SyncedNG2Upline || cell.Value.HideNG2Upline || !cell.Value.SyncedNG2;
            var func = "javascript:Fanex.ExtAcc.Customer.GetMemberList(" + custId + ", " + cell.Value.DoubleCommLosing + ", null, null, " + cell.Value.synccasino + ", " + cell.Value.syncBingo + ", " + cell.Value.SyncLiveCasino + "," + cell.Value.HideKeno + "," + cell.Value.HideVirtualSports + "," + cell.Value.HideGoldDeluxe + "," + cell.Value.HideColossusBet + "," + cell.Value.HideMuayThai + "," + cell.Value.HideCockFighting + "," + cell.Value.HideCricket + "," + hideAllbetSettingLink + "," + hideVoidbridgeSettingLink + "," + hideNG2SettingLink + ")";

            var len = cell.Value.username.length;
            var title = len >= 15 ? ' title="' + cell.Value.username + '"' : '';

            return '<div' + title + '><a href="' + func + '">' + cell.Value.username + '</a></div>';
        }
    }, {
        HeaderText: 'Edit',
        Name: 'Edit',
        Width: 30,
        CustomCellRender: function (cell) {
            var title = escape(cell.Value.username);
            var hideKeno = showKenoSetting == 'false' || cell.Value.HideKeno == true;
            var hideGoldDeluxe = showGoldDeluxeSetting == 0 || cell.Value.HideGoldDeluxe || isHideGoldDeluxe;
            var hideColossusBets = showColossusBets == 0 || cell.Value.HideColossusBet || isHideColossusBets;
            var hideSportsbook2 = showSportsbook2 == 0 || cell.Value.HideMuayThai || isHideSportsbook2;
            var hideCockfighting = showCockfighting == 0 || cell.Value.HideCockFighting || isHideCockfighting;
            var hideCricket = showCricket == 0 || cell.Value.HideCricket || isHideCricket || !cell.Value.IsIMBC;

            var hideAllbetSettingLink = !cell.Value.SyncedAllBetUpline || cell.Value.HideAllBetUpline;
            var hideVoidbridgeSettingLink = !cell.Value.SyncedVoidBridgeUpline || cell.Value.HideVoidBridgeUpline;
            var hideNG2SettingLink = !cell.Value.SyncedNG2Upline || cell.Value.HideNG2Upline;
            
            var func = "javascript:Fanex.ExtAcc.Customer.ShowEditAgent(" + cell.Value.custid + ", '" + title + "', " + cell.Value.disableTransfer + ", " + cell.Value.synccasino + ", " + cell.Value.disablecasino + ", " + cell.Value.syncBingo + ", " + cell.Value.DisableBingo + ", " + cell.Value.DisableP2P + ", " + cell.Value.SyncLiveCasino + ", " + cell.Value.DisableLiveCasino + ", " + cell.Value.HideVirtualSports + ", " + cell.Value.DisableVirtualSports + ", " + masterSyncCasino + ", " + masterSyncBingo + ", " + masterSyncLiveCasino + "," + hideKeno + ", " + cell.Value.DisableKeno + "," + hideGoldDeluxe + ", " + cell.Value.DisableGoldDeluxe + "," + hideColossusBets + ", " + cell.Value.DisableColossusBet + "," + hideSportsbook2 + ", " + cell.Value.DisableMuayThai + "," + hideCockfighting + ", " + cell.Value.DisableCockFighting + "," + hideCricket + ", " + cell.Value.DisableCricket + "," + hideAllbetSettingLink + ", " + cell.Value.DisableAllBet + ", " + hideVoidbridgeSettingLink + ", " + cell.Value.DisableVoidBridge + "," + hideNG2SettingLink + "," + cell.Value.DisableNG2 + "," + cell.Value.SyncedNG2 + ")";

            return '<div onclick="' + func + '" id="edit-' + cell.Value.custid + '" class="editAgent"></div>';
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
            var css = '';

            if (!Fanex.ExtAcc.Customer.AllowDoubleComm) {
                css = 'style="cursor:default !important"';
            }

            if (cell.Value.DoubleCommLosing) {
                return '<div onclick="Fanex.ExtAcc.Customer.ConfigDoubleComm(' + cell.Value.custid + ')" id="dbl-' + cell.Value.custid + '" class="allowDblComm" ' + css + '></div>';
            }

            return '<div onclick="Fanex.ExtAcc.Customer.ConfigDoubleComm(' + cell.Value.custid + ')" id="dbl-' + cell.Value.custid + '" class="disallowDblComm" ' + css + '></div>';
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
        if (!(dataSource.records > 0 && eval(dataSource.rows)[0].IsMyanmarOdds)) {
            this.DataFields.splice(15, 1);
        }
        else {
            Fanex.ExtAcc.Customer.DataFields.splice(11, 1);
            Fanex.ExtAcc.Customer.DataFields.splice(10, 1);
            Fanex.ExtAcc.Customer.DataFields.splice(9, 1);
        }

        this.RenderAgentList(dataSource);
    } else {
        $.ajax({
            url: url,
            data: data,
            dataType: 'json',
            type: 'post'
        })
        .success(function (result) {
            if (!(result.records > 0 && eval(result.rows)[0].IsMyanmarOdds)) {
                Fanex.ExtAcc.Customer.DataFields.splice(15, 1);
            }
            else {
                Fanex.ExtAcc.Customer.DataFields.splice(11, 1);
                Fanex.ExtAcc.Customer.DataFields.splice(10, 1);
                Fanex.ExtAcc.Customer.DataFields.splice(9, 1);
            }

            Fanex.ExtAcc.Customer.RenderAgentList(result);
        })
        .error(function (xhr, status, err) {
            Fanex.ExtAcc.UnBlockUI();
        });
    }
};

Fanex.ExtAcc.Customer.RenderAgentList = function (dataSource) {
    if (dataSource.srecommend) {
        this.superId = dataSource.srecommend;
        this.masterId = dataSource.mrecommend;
        this.agentId = dataSource.recommend;
    }

    var rows = $.parseJSON(dataSource.rows);
    var options = null;
    var bodyRows = [];

    var len = rows.length;
    var parentItem = rows[0];
    var hideAB = Fanex.ExtAcc.Customer.HideAllbet = len > 0 ? (parentItem.HideAllBetUpline || !parentItem.SyncedAllBetUpline) : Fanex.ExtAcc.Customer.HideAllbet;
    var hideVB = Fanex.ExtAcc.Customer.HideVoidbridge = len > 0 ? (parentItem.HideVoidBridgeUpline || !parentItem.SyncedVoidBridgeUpline) : Fanex.ExtAcc.Customer.HideVoidbridge;
    var hideNG2 = Fanex.ExtAcc.Customer.HideNG2 = len > 0 ? (parentItem.HideNG2Upline || !parentItem.SyncedNG2Upline) : Fanex.ExtAcc.Customer.HideNG2;

    var linkSuper = '<a href="' + "javascript:Fanex.ExtAcc.Customer.GetSuperList(" + Fanex.ExtAcc.Customer.directSuperId + ")" + '">' + 'Super' + '</a>';
    var linkMaster = '<a href="' + "javascript:Fanex.ExtAcc.Customer.GetMasterList(" + Fanex.ExtAcc.Customer.superId + ', ' + Fanex.ExtAcc.Customer.AllowDoubleCommMaster + ',' + Fanex.ExtAcc.Customer.directMasterId + ', null, ' + Fanex.ExtAcc.Customer.superSyncCasino + ',' + Fanex.ExtAcc.Customer.superSyncBingo + ',' + Fanex.ExtAcc.Customer.superSyncLiveCasino + ',' + Fanex.ExtAcc.Customer.HideKeno + ',' + Fanex.ExtAcc.Customer.HideVirtualSports + "," + Fanex.ExtAcc.Customer.HideGoldDeluxe + "," + Fanex.ExtAcc.Customer.HideColossusBet + "," + Fanex.ExtAcc.Customer.HideSportsbook2 + "," + Fanex.ExtAcc.Customer.HideCockfighting + "," + Fanex.ExtAcc.Customer.HideCricket + "," + hideAB + ',' + hideVB + ',' + hideNG2 + ')">' + 'Master' + '</a>';

    this.DataSource = rows;
    var isMyanmarOdds = rows.length > 0 ? rows[0].IsMyanmarOdds : false;
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
            MergeColNumber: (isMyanmarOdds ? 5 : 7)
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

    for (var i = 0; i < bodyRows.length; i++) {
        if (!isMyanmarOdds) {
            bodyRows[i].Columns.splice(15, 1);
        }
        else {
            bodyRows[i].Columns.splice(11, 1);
            bodyRows[i].Columns.splice(10, 1);
            bodyRows[i].Columns.splice(9, 1);
        }
    }

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

    $('#breadcrumbs').append('<div style="float:left">' + linkMaster + '</div><div class="viewNext">&nbsp;</div><div style="float:left">' + linkSuper + '</div>');

    $('#breadcrumbs').show();

    Fanex.ExtAcc.OnResize = function () {
        try {
            $("#jqgrid").fanexGrid("resizeGrid");
        } catch (err) { }
    }
};