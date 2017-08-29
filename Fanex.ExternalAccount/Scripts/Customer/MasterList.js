Fanex.ExtAcc.Customer.ShowEditMaster = function (
    custId,
    title,
    transferCondition,
    syncCasino,
    disableCasino,
    syncLiveCasino,
    disableLiveCasino,
    hideVirtualSport,
    disableVirtualSport,
    superSyncCasino,
    superSyncLiveCasino,
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
    disableLucky3) {
    this.TransferCondition = transferCondition;
    Fanex.ExtAcc.Customer.UserId = custId;
    this.TitleEditInfo = unescape(title);
    this.HideVirtualSports = hideVirtualSport;
    this.HideKeno = showKenoSetting == 'false' || isHideKeno;
    this.HideGoldDeluxe = showGoldDeluxeSetting == 0 || isHideGoldDeluxe;
    this.HideColossusBet = showColossusBets == 0 || isHideColossusBets;
    this.HideSportsbook2 = showSportsbook2 == 0 || isHideSportsbook2;
    this.HideCockfighting = showCockfighting == 0 || isHideCockfighting;
    this.HideCricket = showCricket == 0 || isHideCricket;

    this.ResetFlags();

    itemsMaster = {
        sportbook: {
            name: "Sportsbook",
            className: "iconEnableEdit",
            callback: function () {
                Fanex.ExtAcc.Customer.UserId = custId;
                Fanex.ExtAcc.Customer.IsMultiple = false;
                Fanex.ExtAcc.Customer.IsFullInfo = true;
                Fanex.ExtAcc.Form.Show(resolveClientUrl('SportsbookSetting/EditMultiplePT?roleId=3&groupName=' + Fanex.ExtAcc.Customer.GroupName().groupName + '&custId=' + Fanex.ExtAcc.Customer.UserId), 'Edit Sportsbook Setting ' + '<font color="#6dcff6">' + title + '</font>', 900, 700, false, false, 1);
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
                Fanex.ExtAcc.Form.ShowFrame(resolveClientUrl('customersetting/racebook/racingsetting/master?id=' + Fanex.ExtAcc.Customer.UserId + '&uplineid=' + Fanex.ExtAcc.Customer.superId + '&groupName=' + $('#selectedGroupName').html()), 'Edit Racing Setting <font color="#6dcff6">' + title + '</font>', 1050, 775, false, null, 1);
            }
        },
        minPTRacing: {
            name: "Min PT",
            className: "iconEnableEdit",
            callback: function (key, opt) {
                Fanex.ExtAcc.Customer.UserId = custId;
                Fanex.ExtAcc.Customer.IsMultiple = false;
                var url = 'CustomerSetting/Racebook/RacingSetting/MasterMinMaxPT?id=' + Fanex.ExtAcc.Customer.UserId + '&roleId=' + Fanex.ExtAcc.Customer.CurrentLevel;
                Fanex.ExtAcc.Form.ShowFrame(resolveClientUrl(url + '&groupName=' + $('#selectedGroupName').html()), 'Edit Min PT Racing: <font color="#6dcff6">' + title + '</font>', 980, 680);
            }
        },
        separator1: "-----"
    };

    var has3rdParty = false;
    if (superSyncCasino) {
        has3rdParty = true;
        if (syncCasino) {
            if (disableCasino) {
                itemsMaster.casino = {
                    name: "Casino",
                    className: "iconDisableEdit",
                    callback: function () {
                        Fanex.ExtAcc.Customer.UserId = custId;
                        Fanex.ExtAcc.Customer.IsMultiple = false;
                        Fanex.ExtAcc.Form.ShowFrame(resolveClientUrl('CustomerSetting/Casino/Master/EditMaster/' + Fanex.ExtAcc.Customer.UserId + '/' + Fanex.ExtAcc.Customer.superId + '/' + $('#selectedGroupName').html()), 'Edit Casino Setting <font color="#6dcff6">' + title + '</font>', 1064, 700, false, null, 1);
                    }
                };
            } else {
                itemsMaster.casino = {
                    name: "Casino",
                    className: "iconEnableEdit",
                    callback: function () {
                        Fanex.ExtAcc.Customer.UserId = custId;
                        Fanex.ExtAcc.Customer.IsMultiple = false;
                        Fanex.ExtAcc.Form.ShowFrame(resolveClientUrl('CustomerSetting/Casino/Master/EditMaster/' + Fanex.ExtAcc.Customer.UserId + '/' + Fanex.ExtAcc.Customer.superId + '/' + $('#selectedGroupName').html()), 'Edit Casino Setting <font color="#6dcff6">' + title + '</font>', 1064, 700, false, null, 1);
                    }
                };
            }
        } else {
            itemsMaster.casino = {
                name: "Casino",
                className: "iconUnsyncEdit",
                callback: function () {
                    Fanex.ExtAcc.Customer.UserId = custId;
                    Fanex.ExtAcc.Customer.IsMultiple = false;
                    Fanex.ExtAcc.Form.ShowFrame(resolveClientUrl('CustomerSetting/Casino/Master/EditMaster/' + Fanex.ExtAcc.Customer.UserId + '/' + Fanex.ExtAcc.Customer.superId + '/' + $('#selectedGroupName').html()), 'Edit Casino Setting <font color="#6dcff6">' + title + '</font>', 1064, 700, false, null, 1);
                }
            };
        }
    }

    if (superSyncLiveCasino) {
        has3rdParty = true;

        itemsMaster.liveCasino = {
            name: "Live Casino",
            className: syncLiveCasino == true ? (disableLiveCasino == true ? "iconDisableEdit" : "iconEnableEdit") : "iconUnsyncEdit",
            callback: function () {
                Fanex.ExtAcc.Customer.UserId = custId;
                url = "LiveCasinoSetting/Setting/EditSetting?uplineId=" + Fanex.ExtAcc.Customer.superId + "&parentId=" + Fanex.ExtAcc.Customer.superId + "&uplineLevel=4&customerId=" + Fanex.ExtAcc.Customer.UserId + "&customerLevel=3" + "&actorId=" + parent.userId + "&actorUsername=" + parent.username + "&groupName=" + parent.$('#selectedGroupName').html();
                Fanex.ExtAcc.Form.ShowFrame(resolveClientUrl(url), 'Edit Live Casino Setting <font color="#6dcff6">' + title + '</font>', 900, 700, false, false, 1);
            }
        };
    }

    var ret = this.CustomerInfo();

    if (!hideVirtualSport) {
        has3rdParty = true;
        if (disableVirtualSport) {
            itemsMaster.virtualSport = {
                name: "Virtual Sports",
                className: "iconDisableEdit",
                callback: function () {
                    Fanex.ExtAcc.Customer.UserId = custId;
                    Fanex.ExtAcc.Customer.IsMultiple = false;
                    Fanex.ExtAcc.Customer.ShowEditSingleVirtualSports(custId, 4, 3);
                }
            };
        } else {
            itemsMaster.virtualSport = {
                name: "Virtual Sports",
                className: "iconEnableEdit",
                callback: function () {
                    Fanex.ExtAcc.Customer.UserId = custId;
                    Fanex.ExtAcc.Customer.IsMultiple = false;
                    Fanex.ExtAcc.Customer.ShowEditSingleVirtualSports(custId, 4, 3);
                }
            };
        }
    }

    if (!isHideKeno) {
        has3rdParty = true;
        if (disableKeno) {
            itemsMaster.keno = {
                name: "Keno",
                className: "iconDisableEdit",
                callback: function () {
                    Fanex.ExtAcc.Customer.UserId = custId;
                    Fanex.ExtAcc.Customer.IsMultiple = false;
                    Fanex.ExtAcc.Form.ShowFrame(resolveClientUrl('customersetting/KenoSetting/kenosetting/EditMaster?custId=' + Fanex.ExtAcc.Customer.UserId + "&userId=" + parent.userId), 'Edit Keno Setting <font color="#6dcff6">' + title + '</font>', 900, 700, false, null, 1);
                }
            };
        } else {
            itemsMaster.keno = {
                name: "Keno",
                className: "iconEnableEdit",
                callback: function () {
                    Fanex.ExtAcc.Customer.UserId = custId;
                    Fanex.ExtAcc.Customer.IsMultiple = false;
                    Fanex.ExtAcc.Form.ShowFrame(resolveClientUrl('customersetting/KenoSetting/kenosetting/EditMaster?custId=' + Fanex.ExtAcc.Customer.UserId + "&userId=" + parent.userId), 'Edit Keno Setting <font color="#6dcff6">' + title + '</font>', 900, 700, false, null, 1);
                }
            };
        }
    }

    if (!this.HideGoldDeluxe) {
        has3rdParty = true;

        itemsMaster.goldDeluxe = {
            name: "Gold Deluxe",
            className: disableGoldDeluxe == true ? "iconDisableEdit" : "iconEnableEdit",
            callback: function () {
                Fanex.ExtAcc.Customer.UserId = custId;
                url = "GoldDeluxeSetting/GoldDeluxe/EditSetting?upLineLevel=4&formType=1&level=3" + "&uplineId=" + Fanex.ExtAcc.Customer.superId + "&actorId=" + parent.userId + "&custId=" + Fanex.ExtAcc.Customer.UserId + "&groupName=" + parent.$('#selectedGroupName').html();
                Fanex.ExtAcc.Form.ShowFrame(resolveClientUrl(url), 'Edit Gold Deluxe Setting <font color="#6dcff6">' + title + '</font>', 900, 400, false, false, 1);
            }
        }
    }

    if (!this.HideColossusBet) {
        has3rdParty = true;

        itemsMaster.colossusBets = {
            name: "Colossus Bets",
            className: disableColossusBets == true ? "iconDisableEdit" : "iconEnableEdit",
            callback: function () {
                Fanex.ExtAcc.Customer.UserId = custId;
                Fanex.ExtAcc.Customer.IsMultiple = false;
                url = "customersetting/ColossusBetSetting/ColossusBetSetting/EditSetting?custid=" + Fanex.ExtAcc.Customer.UserId + "&srecommend=" + Fanex.ExtAcc.Customer.superId + "&isMultipleEdit=0&userId=" + parent.userId + "&roleId=3&groupName=" + parent.$('#selectedGroupName').html();
                Fanex.ExtAcc.Form.ShowFrame(resolveClientUrl(url), 'Edit ColossusBets Setting <font color="#6dcff6">' + title + '</font>', 900, 400, false, false, 1);
            }
        }
    }

    if (!this.HideSportsbook2) {
        has3rdParty = true;
        itemsMaster.sportsbook2 = {
            name: "Sportsbook 2",
            className: disableSportsbook2 == true ? "iconDisableEdit" : "iconEnableEdit",
            callback: function () {
                Fanex.ExtAcc.Customer.IsMultiple = false;
                Fanex.ExtAcc.Customer.IsMultiple = false;
                Fanex.ExtAcc.Customer.IsFullInfo = true;
                Fanex.ExtAcc.Customer.ShowEditSingleSportsbook2(custId, 4, 3);
            }
        };
    }

    if (!this.HideCockfighting) {
        has3rdParty = true;
        itemsMaster.cockFighting = {
            name: "Cockfighting",
            className: disableCockfighting == true ? "iconDisableEdit" : "iconEnableEdit",
            callback: function () {
                Fanex.ExtAcc.Customer.IsMultiple = false;
                Fanex.ExtAcc.Customer.IsMultiple = false;
                Fanex.ExtAcc.Customer.IsFullInfo = true;
                Fanex.ExtAcc.Customer.ShowEditSingleCockfighting(custId, 4, 3);
            }
        };
    }

    if (!this.HideCricket) {
        has3rdParty = true;
        itemsMaster.cricket = {
            name: "Cricket",
            className: disableCricket == true ? "iconDisableEdit" : "iconEnableEdit",
            callback: function () {
                Fanex.ExtAcc.Customer.IsMultiple = false;
                Fanex.ExtAcc.Customer.IsMultiple = false;
                Fanex.ExtAcc.Customer.IsFullInfo = true;
                Fanex.ExtAcc.Customer.ShowEditSingleCricket(custId, 4, 3);
            }
        };
    }

    if (!isHideAllbet) {
        has3rdParty = true;
        itemsMaster.allbet = {
            name: "Allbet",
            className: disableAllbet == true ? "iconDisableEdit" : "iconEnableEdit",
            callback: function () {
                Fanex.ExtAcc.Customer.IsMultiple = false;
                Fanex.ExtAcc.Customer.IsMultiple = false;
                Fanex.ExtAcc.Customer.IsFullInfo = true;
                Fanex.ExtAcc.Customer.ShowEditSingleAllbet(custId, 4, 3);
            }
        };
    }

    if (!isHideVoidbridge) {
        has3rdParty = true;
        itemsMaster.voidbridge = {
            name: "Macau Games",
            className: disableVoidbridge == true ? "iconDisableEdit" : "iconEnableEdit",
            callback: function () {
                Fanex.ExtAcc.Customer.IsMultiple = false;
                Fanex.ExtAcc.Customer.IsMultiple = false;
                Fanex.ExtAcc.Customer.IsFullInfo = true;
                Fanex.ExtAcc.Customer.ShowEditSingleVoidbridge(custId, 4, 3);
            }
        };
    }

    if (!isHideNG2) {
        has3rdParty = true;
        itemsMaster.ng2 = {
            name: ng2Text,
            className: !syncNG2 ? "iconUnsyncEdit" : (disableNG2 == true ? "iconDisableEdit" : "iconEnableEdit"),
            callback: function () {
                Fanex.ExtAcc.Customer.IsMultiple = false;
                Fanex.ExtAcc.Customer.IsMultiple = false;
                Fanex.ExtAcc.Customer.IsFullInfo = true;
                Fanex.ExtAcc.Customer.ShowEditSingleNG2(custId, 4, 3);
            }
        };
    }

    if (!isHideFishingWorld) {
        has3rdParty = true;
        itemsMaster.fishingWorld = {
            name: "Fishing World",
            className: disableFishingWorld === true ? "iconDisableEdit" : "iconEnableEdit",
            callback: function () {
                Fanex.ExtAcc.Customer.IsMultiple = false;
                Fanex.ExtAcc.Customer.IsFullInfo = true;
                Fanex.ExtAcc.Customer.ShowEditSingleFishingWorld(custId, 4, 3);
            }
        };
    }

    if (!isHideLottery) {
        has3rdParty = true;
        itemsMaster.lottery = {
            name: "Lottery",
            className: disableLottery === true ? "iconDisableEdit" : "iconEnableEdit",
            callback: function () {
                Fanex.ExtAcc.Customer.IsMultiple = false;
                Fanex.ExtAcc.Customer.IsFullInfo = true;
                Fanex.ExtAcc.Customer.ShowEditSingleLottery(custId, 4, 3);
            }
        };
    }

    if (!isHideLucky3) {
        has3rdParty = true;
        itemsMaster.lucky3 = {
            name: "Lucky 3",
            className: disableLucky3 === true ? "iconDisableEdit" : "iconEnableEdit",
            callback: function () {
                Fanex.ExtAcc.Customer.IsMultiple = false;
                Fanex.ExtAcc.Customer.IsFullInfo = true;
                Fanex.ExtAcc.Customer.ShowEditSingleLucky3(custId, 4, 3);
            }
        };
    }
    
    if (has3rdParty)
        itemsMaster.separator2 = "-----";
    itemsMaster.editInfo = {
        name: "Edit Info",
        className: "iconEnableEdit editInfoBg",
        callback: function (key, opt) {
            Fanex.ExtAcc.Customer.UserId = custId;
            Fanex.ExtAcc.Customer.IsMultiple = false;
            Fanex.ExtAcc.Form.Show(resolveClientUrl('Customer/EditInfo/?custId=' + Fanex.ExtAcc.Customer.UserId + '&groupName=' + Fanex.ExtAcc.Customer.GroupName().groupName), titles[0] + ': <font color="#6dcff6">' + Fanex.ExtAcc.Customer.TitleEditInfo + '</font>', 500, 460, false);
        }
    };
    itemsMaster.changePassword = {
        name: "Reset Password",
        className: "iconEnableEdit editInfoBg",
        callback: function (key, opt) {
            Fanex.ExtAcc.Customer.UserId = custId;
            Fanex.ExtAcc.Customer.IsMultiple = false;
            Fanex.ExtAcc.Form.Show(resolveClientUrl('Customer/ResetPassword/?custId=' + Fanex.ExtAcc.Customer.UserId + '&groupName=' + Fanex.ExtAcc.Customer.GroupName().groupName), 'Reset password: <font color="#6dcff6">' + Fanex.ExtAcc.Customer.TitleEditInfo + '</font>', 415, 400, false);
        }
    };

    if (this.hasSecurityCode) {
        var self = this;
        itemsMaster.securityCode = {
            name: "Security Code",
            className: "iconEnableEdit editInfoBg",
            callback: function (key, opt) {
                Fanex.ExtAcc.Customer.IsMultiple = false;
                var params = {
                    id: custId,
                    roleId: 3,
                    username: Fanex.ExtAcc.Customer.TitleEditInfo,
                    groupName: Fanex.ExtAcc.Customer.GroupName().groupName
                };

                Fanex.ExtAcc.Form.ShowFrame(
                    resolveClientUrl('CustomerSetting/LeftPanel/SearchCustomer/GetSecurityCode?' + $.param(params)),
                    'Reset security code: <font color="#6dcff6">' + Fanex.ExtAcc.Customer.TitleEditInfo + '</font>',
                    650,
                    500,
                    false);
            }
        };
    }

    if (transferCondition == false) {
        itemsMaster.transferCondition = {
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

    try {
        $.contextMenu({
            selector: '.editMaster',
            items: itemsMaster,
            trigger: "left",
            ignoreRightClick: true,
            autoHide: true
        });
    } catch (e) { }
};

Fanex.ExtAcc.Customer.GetMasterList = function (
    srecommend,
    allow,
    custId,
    dataSource,
    superSyncCasino,
    superSyncLiveCasino,
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
    isHideLucky3) {
    Fanex.ExtAcc.BlockUI();

    this.superId = srecommend;
    this.CurrentLevel = Fanex.ExtAcc.Customer.Master;
    this.AllowDoubleComm = allow;
    this.AllowDoubleCommMaster = allow;

    this.superSyncCasino = superSyncCasino;
    this.superSyncLiveCasino = superSyncLiveCasino;
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
    this.HideFishingWorld = isHideFishingWorld;
    this.hasSecurityCode = false;
    this.HideLottery = isHideLottery;
    this.HideLucky3 = isHideLucky3;

    var url = resolveClientUrl('Customer/MasterListPost');
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
            return '<div onclick="Fanex.ExtAcc.Customer.Select(this)" style="margin-left: 0px !important" id="' +
                cell.Value.custid + '" class="menuInactive" data-unsyncedproducts="' + Fanex.ExtAcc.Customer.GetUnsyncedProductIds(cell) + '"></div>';
        }
    }, {
        HeaderText: 'Username',
        Name: 'username',
        Width: 100,
        CustomCellRender: function (cell) {
            var custId = cell.Value.custid;
            var hideAllbetSettingLink = !cell.Value.SyncedAllBetUpline || cell.Value.HideAllBetUpline || !cell.Value.SyncedAllBet;
            var hideVoidbridgeSettingLink = !cell.Value.SyncedVoidBridgeUpline || cell.Value.HideVoidBridgeUpline || !cell.Value.SyncedVoidBridge;
            var hideNG2SettingLink = !cell.Value.SyncedNG2Upline || cell.Value.HideNG2Upline || !cell.Value.SyncedNG2;
            var hideFishingWorldSettingLink = !cell.Value.SyncedFishingWorldUpline || cell.Value.HideFishingWorldUpline;
            var hideLotterySettingLink = cell.Value.HideLotteryUpline;
            var hideLucky3SettingLink = cell.Value.HideLucky3Upline;

            var func = "javascript:Fanex.ExtAcc.Customer.GetAgentList(" + custId + ", " + cell.Value.DoubleCommLosing + ", null, null, " + cell.Value.synccasino + ", " + cell.Value.SyncLiveCasino + "," + cell.Value.HideKeno + "," + cell.Value.HideVirtualSports + "," + cell.Value.HideGoldDeluxe + "," + cell.Value.HideColossusBet + "," + cell.Value.HideMuayThai + "," + cell.Value.HideCockFighting + "," + cell.Value.HideCricket + "," + hideAllbetSettingLink + "," + hideVoidbridgeSettingLink + "," + hideNG2SettingLink + "," + hideFishingWorldSettingLink + "," + hideLotterySettingLink + "," + hideLucky3SettingLink + ")";

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
            var hideFishingWorldSettingLink = cell.Value.HideFishingWorldUpline || !cell.Value.SyncedFishingWorldUpline;
            var hideLotterySettingLink = cell.Value.HideLotteryUpline;
            var hideLucky3SettingLink = cell.Value.HideLucky3Upline;

            var func = "javascript:Fanex.ExtAcc.Customer.ShowEditMaster(" + cell.Value.custid + ", '" + title + "', " + cell.Value.disableTransfer + ", " + cell.Value.synccasino + ", " + cell.Value.disablecasino + ", " + cell.Value.SyncLiveCasino + ", " + cell.Value.DisableLiveCasino + ", " + cell.Value.HideVirtualSports + ", " + cell.Value.DisableVirtualSports + ", " + superSyncCasino + ", " + superSyncLiveCasino + "," + hideKeno + ", " + cell.Value.DisableKeno + "," + hideGoldDeluxe + ", " + cell.Value.DisableGoldDeluxe + "," + hideColossusBets + ", " + cell.Value.DisableColossusBet + "," + hideSportsbook2 + ", " + cell.Value.DisableMuayThai + "," + hideCockfighting + ", " + cell.Value.DisableCockFighting + "," + hideCricket + ", " + cell.Value.DisableCricket + "," + hideAllbetSettingLink + ", " + cell.Value.DisableAllBet + "," + hideVoidbridgeSettingLink + ", " + cell.Value.DisableVoidBridge + "," + hideNG2SettingLink + "," + cell.Value.DisableNG2 + "," + cell.Value.SyncedNG2 + "," + hideFishingWorldSettingLink + "," + cell.Value.DisableFishingWorld + "," + hideLotterySettingLink + "," + cell.Value.DisableLottery + "," + hideLucky3SettingLink + "," + cell.Value.DisableLucky3 + ")";

            return '<div onclick="' + func + '" id="edit-' + cell.Value.custid + '" class="editMaster"></div>';
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
        Name: 'groupd',
        HeaderText: 'Group D',
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

        this.RenderMasterList(dataSource);
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

            Fanex.ExtAcc.Customer.RenderMasterList(result);
        })
        .error(function (xhr, status, err) {
            Fanex.ExtAcc.UnBlockUI();
        });
    }
};

Fanex.ExtAcc.Customer.RenderMasterList = function (dataSource) {
    if (dataSource.srecommend) {
        this.superId = dataSource.srecommend;
        this.masterId = dataSource.mrecommend;
        this.agentId = dataSource.recommend;
        this.hasSecurityCode = dataSource.hasSecurityCode;
    }

    var rows = $.parseJSON(dataSource.rows);
    var options = null;
    var bodyRows = [];

    var len = rows.length;
    var parentItem = rows[0];

    if (rows.length > 0) {
        this.hasSecurityCode = rows[0].UplineHasSecurityCode || rows[0].HasSecurityCode;
    }
    
    Fanex.ExtAcc.Customer.HideAllbet = len > 0 ? (parentItem.HideAllBetUpline || !parentItem.SyncedAllBetUpline) : Fanex.ExtAcc.Customer.HideAllbet;
    Fanex.ExtAcc.Customer.HideVoidbridge = len > 0 ? (parentItem.HideVoidBridgeUpline || !parentItem.SyncedVoidBridgeUpline) : Fanex.ExtAcc.Customer.HideVoidbridge;
    Fanex.ExtAcc.Customer.HideNG2 = len > 0 ? (parentItem.HideNG2Upline || !parentItem.SyncedNG2Upline) : Fanex.ExtAcc.Customer.HideNG2;
    Fanex.ExtAcc.Customer.HideFishingWorld = len > 0 ? (parentItem.HideFishingWorldUpline || !parentItem.SyncedFishingWorldUpline) : Fanex.ExtAcc.Customer.HideFishingWorld;
    Fanex.ExtAcc.Customer.HideLottery = len > 0 ? (parentItem.HideLotteryUpline) : Fanex.ExtAcc.Customer.HideLottery;

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

    $('#breadcrumbs').append('<a href="' + "javascript:Fanex.ExtAcc.Customer.GetSuperList(" + Fanex.ExtAcc.Customer.directSuperId + ")" + '">' + 'Super' + '</a>');

    $('#breadcrumbs').show();

    Fanex.ExtAcc.OnResize = function () {
        try {
            $("#jqgrid").fanexGrid("resizeGrid");
        } catch (err) { }
    }
};