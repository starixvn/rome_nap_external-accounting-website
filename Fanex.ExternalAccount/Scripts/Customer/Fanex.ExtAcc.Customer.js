/// <reference path="../_references.js" />
if (typeof Fanex == 'undefined' || typeof Fanex.ExtAcc == 'undefined') {
    alert("Fanex.ExtAcc.js module is required!")
}

Fanex.ExtAcc.Customer = {
    DataFields: null,
    DataSource: null,

    superId: 0,
    masterId: 0,
    agentId: 0,
    UserId: 0,

    directSuperId: null,
    directMasterId: null,
    directAgentId: null,

    custId: null,

    Super: 4,
    Master: 3,
    Agent: 2,
    Member: 1,

    CurrentLevel: 5,
    CurrentPage: 0,
    ItemsPerPage: 50,

    AllowDoubleCommMaster: false,
    AllowDoubleCommAgent: false,
    AllowDoubleComm: false,
    HideVirtualSports: false,
    HideKeno: false,
    HideGoldDeluxe: false,
    HideColossusBet: false,

    superSyncCasino: false,
    superSyncBingo: false,
    superSyncLiveCasino: false,
    superDisableP2P: false,
    masterSyncCasino: false,
    masterSyncBingo: false,
    masterSyncLiveCasino: false,
    masterDisableP2P: false,
    agentSyncCasino: false,
    agentSyncBingo: false,
    agentSyncLiveCasino: false,
    agentDisableP2P: false,

    isSuperEditMember: false,
    isMasterEditMember: false,
    isSuperEditAgent: false,

    IsMultiple: false,
    IsFullInfo: false,

    Closed: false,
    Suspended: false,
    TransferCondition: false,
    IsLoading: false,

    URL: '',
    MultipleDblCommError: '',
    Usernames: '',
    CustIds: '',
    LiveCasinoStatuses: '',
    RnGCasinoStatuses: [],
    TitleEditInfo: '',
    MessageClosed: 'Are you sure you want to change Close status for this customer? Warning, all downlines will be closed as well!',
    MessageUnClosed: 'Are you sure you want to change Close status for this customer?',
    MessageSuspended: 'Are you sure you want to change Suspend status for this customer? Warning, all members under this agent can\'t place bet!',
    MessageUnSuspended: 'Are you sure you want to change Suspend status for this customer? All downlines will be un-suspended as well.',

    MessageClosedMember: 'Are you sure you want to change Close status for this customer?',
    MessageSuspendedMember: 'Are you sure you want to change Suspend status for this customer? Warning, this member can\'t place bet!',
    MessageUnSuspendedMember: 'Are you sure you want to change Suspend status for this customer?',

    RecheckinXHR: null,

    ReCheckin: function () {
        if (this.RecheckinXHR != null)
            this.RecheckinXHR.abort();

        this.RecheckinXHR = $.ajax({
            url: resolveClientUrl('Account/ReCheckIn'),
            type: 'GET',
            dataType: 'html',
            cache: false,
            success: function (data) {
                if (data == -1) {
                    try {
                        if (typeof Fanex.ExtAcc.Form != 'undefined') {
                            if (document.getElementById('windowPopup')) {
                                var tag = document.getElementById('windowPopup');

                                if (tag.tagName == "IFRAME") {
                                    Fanex.ExtAcc.Form.Close();
                                } else {
                                    $('#windowPopup').html('You have logged in from another session or your login session has expired!'); // Prevent opening popup if any
                                }
                            }

                            var timeout = setTimeout(function () {
                                clearTimeout(timeout);
                                Fanex.ExtAcc.Form.Alert('Session Expired', 'You have logged in from another session or your login session has expired!', 400, 150, null, function () {
                                    location.href = resolveClientUrl('Account/Login');
                                }, 1000);
                            }, 1000);
                        }
                    } catch (e) {
                        location.href = resolveClientUrl('Account/Login');
                    }
                }
                else if (data == 2) {
                    location.href = resolveClientUrl('Account/Offline');
                }
                else if (data == 3) {
                    location.href = resolveClientUrl('Shared/Error');
                }

                Fanex.ExtAcc.Account.RecheckinXHR = null;
            },
            error: function (xhr, status, e) {
                if (typeof console != 'undefined') console.log(e);

                Fanex.ExtAcc.Account.RecheckinXHR = null;
            }
        });
    },

    Init: function () {
        if (this.RecheckinXHR == null && document.URL.toLowerCase().indexOf("account/login") != -1) {
            setInterval(this.ReCheckin, 10000);
        }

        if (this.IsLoading)
            return;

        this.IsLoading = true;
        //Prevent loading group name
        var timeout = setInterval(function () {
            if ($.trim($('#selectedGroupName').html()) != '') {
                Fanex.ExtAcc.Customer.Search(true);
                clearInterval(timeout);
            }
        }, 100);

        $('#exporttoexcel').click(function () {
            var data = Fanex.ExtAcc.Customer.GetParams();

            var params = "groupName=" + data.groupName + "&enabled=" + data.enabled + "&closed=" + data.closed + "&suspended=" + data.suspended + "&sRecommend=" + Fanex.ExtAcc.Customer.superId + "&mRecommend=" + Fanex.ExtAcc.Customer.masterId + "&recommend=" + Fanex.ExtAcc.Customer.agentId + "&custId=" + Fanex.ExtAcc.Customer.custId + "&currentLevel=" + Fanex.ExtAcc.Customer.CurrentLevel + "&username=" + data.username
            document.location.href = resolveClientUrl("Customer/GetExportToExcel?") + params;
        });

        $('#txtUserName').keypress(function (e) {
            if (typeof e == 'undefined') e = window.event;
            var key = (typeof e.keyCode == 'undefined') ? e.which : e.keyCode;
            if (key == 13) {
                if (Fanex.ExtAcc.SuggestionXHR != null)
                    Fanex.ExtAcc.SuggestionXHR.abort();

                $('.ui-autocomplete').hide();
                Fanex.ExtAcc.Customer.Search(false);

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

        Fanex.ExtAcc.InitSuggestion();
    },

    GroupName: function () {
        var data = new Object();
        data.groupName = $('#selectedGroupName').html();
        return data;
    },

    GetParams: function () {
        var enabled = false;
        var closed = false;
        var suspended = false;

        if ($('#customerStatus').val() == '-1') {
            enabled = null;
            closed = null;
            suspended = null;
        } else if ($('#customerStatus').val() == '1') {
            enabled = true;
            closed = false;
            suspended = false;
        } else if ($('#customerStatus').val() == '2') {
            enabled = null;
            closed = true;
            suspended = null;
        } else if ($('#customerStatus').val() == '3') {
            enabled = null;
            closed = null;
            suspended = true;
        } else {
            enabled = false;
            closed = null;
            suspended = null;
        }

        var data = new Object();

        data.srecommend = this.superId;
        data.mrecommend = this.masterId;
        data.recommend = this.agentId;
        data.groupName = $('#selectedGroupName').html() == '' ? 'ACCDDD' : $('#selectedGroupName').html();
        data.enabled = enabled;
        data.closed = closed;
        data.suspended = suspended;
        data.username = Fanex.encodeHtml($('#txtUserName').val());
        data.roleId = this.CurrentLevel;
        data.custId = data.username == '' ? null : this.UserId;

        return data;
    },

    Search: function (isFirstLoad) {
        if (isFirstLoad) {
            this.GetSuperList();
        } else {
            this.SearchList();
        }
    },

    ShowEditSuper: function (custId, title, hideGoldDeluxe) {
        this.TransferCondition = false;
        this.UserId = custId;
        this.TitleEditInfo = unescape(title);
        this.CurrentLevel = this.Super;
        this.HideGoldDeluxe = hideGoldDeluxe;

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
            bingo: {
                name: "Bingo",
                className: "iconViewEdit",
                callback: function () {
                    Fanex.ExtAcc.Customer.IsMultiple = false;
                    Fanex.ExtAcc.Form.ShowFrame(resolveClientUrl('CustomerSetting/Bingo/BingoSetting/Super?id=' + Fanex.ExtAcc.Customer.UserId + '&uplineid=' + Fanex.ExtAcc.Customer.UserId + '&groupName=' + $('#selectedGroupName').html()), 'Edit Bingo Setting <font color="#6dcff6">' + title + '</font>', 1050, 380, false, null, 1);
                }
            },
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
                    Fanex.ExtAcc.Customer.IsMultiple = false;
                    Fanex.ExtAcc.Customer.IsFullInfo = true;
                    Fanex.ExtAcc.Form.Show(resolveClientUrl('VirtualSports/EditSingle?custId=' + Fanex.ExtAcc.Customer.UserId + '&roleId=5&targetCustId=' + Fanex.ExtAcc.Customer.UserId + '&targetRoleId=' + Fanex.ExtAcc.Customer.Super + '&groupName=' + Fanex.ExtAcc.Customer.GroupName().groupName), 'Edit Virtual Sports Setting ' + '<font color="#6dcff6">' + Fanex.ExtAcc.Customer.TitleEditInfo + '</font>', 880, 700, false, false, 1);
                }
            }
        }

        if (showKenoSetting == 'true') {
            items.keno = {
                name: "Keno",
                className: "iconViewEdit",
                callback: function () {
                    Fanex.ExtAcc.Customer.IsMultiple = false;
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
                    Fanex.ExtAcc.Customer.IsMultiple = false;
                    Fanex.ExtAcc.Customer.IsFullInfo = true;
                    url = "GoldDeluxeSetting/GoldDeluxe/EditSetting?upLineLevel=0&uplineId=0&formType=2&level=4" + "&actorId=" + parent.userId + "&custId=" + Fanex.ExtAcc.Customer.UserId + "&groupName=" + parent.$('#selectedGroupName').html();
                    Fanex.ExtAcc.Form.ShowFrame(resolveClientUrl(url), 'Edit Gold Deluxe Setting <font color="#6dcff6">' + title + '</font>', 900, 400, false, false, 1);
                }
            }
        }

        if (showColossusBets == 1) {
            items.colossusBets = {
                name: "Colossus Bets",
                className: "iconViewEdit",
                callback: function () {
                    Fanex.ExtAcc.Customer.IsMultiple = false;
                    Fanex.ExtAcc.Customer.IsMultiple = false;
                    Fanex.ExtAcc.Customer.IsFullInfo = true;
                    url = "customersetting/ColossusBetSetting/ColossusBetSetting/EditSetting?custid=" + Fanex.ExtAcc.Customer.UserId + "&isMultipleEdit=0&userId=" + parent.userId + "&roleId=4&groupName=" + parent.$('#selectedGroupName').html();
                    Fanex.ExtAcc.Form.ShowFrame(resolveClientUrl(url), 'Edit Gold Deluxe Setting <font color="#6dcff6">' + title + '</font>', 900, 400, false, false, 1);
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
    },

    ShowEditMaster: function (custId, title, transferCondition, syncCasino, disableCasino, syncBingo, disableBingo, disableP2P, syncLiveCasino, disableLiveCasino, hideVirtualSport, disableVirtualSport, superSyncCasino, superSyncBingo, superSyncLiveCasino, isHideKeno, disableKeno, isHideGoldDeluxe, disableGoldDeluxe, isHideColossusBets, disableColossusBets) {
        this.TransferCondition = transferCondition;
        Fanex.ExtAcc.Customer.UserId = custId;
        this.TitleEditInfo = unescape(title);
        this.HideVirtualSports = hideVirtualSport;
        this.HideKeno = showKenoSetting == 'false' || isHideKeno;
        this.HideGoldDeluxe = showGoldDeluxeSetting == 0 || isHideGoldDeluxe;
        this.HideColossusBet = showColossusBets == 0 || isHideColossusBets;

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

        if (superSyncBingo) {
            has3rdParty = true;
            if (syncBingo) {
                if (disableBingo) {
                    itemsMaster.bingo = {
                        name: "Bingo",
                        className: "iconDisableEdit",
                        callback: function (key, opt) {
                            Fanex.ExtAcc.Customer.UserId = custId;
                            Fanex.ExtAcc.Customer.IsMultiple = false;
                            Fanex.ExtAcc.Form.ShowFrame(resolveClientUrl('CustomerSetting/Bingo/BingoSetting/Master?id=' + Fanex.ExtAcc.Customer.UserId + '&uplineid=' + Fanex.ExtAcc.Customer.superId + '&groupName=' + $('#selectedGroupName').html()), 'Edit Bingo Setting <font color="#6dcff6">' + title + '</font>', 1050, 500, false, null, 1);
                        }
                    };
                } else {
                    itemsMaster.bingo = {
                        name: "Bingo",
                        className: "iconEnableEdit",
                        callback: function (key, opt) {
                            Fanex.ExtAcc.Customer.UserId = custId;
                            Fanex.ExtAcc.Customer.IsMultiple = false;
                            Fanex.ExtAcc.Form.ShowFrame(resolveClientUrl('CustomerSetting/Bingo/BingoSetting/Master?id=' + Fanex.ExtAcc.Customer.UserId + '&uplineid=' + Fanex.ExtAcc.Customer.superId + '&groupName=' + $('#selectedGroupName').html()), 'Edit Bingo Setting <font color="#6dcff6">' + title + '</font>', 1050, 500, false, null, 1);
                        }
                    };
                }
            } else {
                itemsMaster.bingo = {
                    name: "Bingo",
                    className: "iconUnsyncEdit",
                    callback: function (key, opt) {
                        Fanex.ExtAcc.Customer.UserId = custId;
                        Fanex.ExtAcc.Customer.IsMultiple = false;
                        Fanex.ExtAcc.Form.ShowFrame(resolveClientUrl('CustomerSetting/Bingo/BingoSetting/Master?id=' + Fanex.ExtAcc.Customer.UserId + '&uplineid=' + Fanex.ExtAcc.Customer.superId + '&groupName=' + $('#selectedGroupName').html()), 'Edit Bingo Setting <font color="#6dcff6">' + title + '</font>', 1050, 500, false, null, 1);
                    }
                };
            }
        }

        if (!disableP2P) {
            //itemsMaster.poker = { name: "Poker", className: "iconEnableEdit" };
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
                        Fanex.ExtAcc.Form.Show(resolveClientUrl('VirtualSports/EditSingle?custId=' + Fanex.ExtAcc.Customer.superId + '&roleId=' + Fanex.ExtAcc.Customer.Super + '&targetCustId=' + Fanex.ExtAcc.Customer.UserId + '&targetRoleId=' + Fanex.ExtAcc.Customer.Master + '&groupName=' + Fanex.ExtAcc.Customer.GroupName().groupName), 'Edit Virtual Sports Setting <font color="#6dcff6">' + title + '</font>', 880, 700, false, null, 1);
                    }
                };
            } else {
                itemsMaster.virtualSport = {
                    name: "Virtual Sports",
                    className: "iconEnableEdit",
                    callback: function () {
                        Fanex.ExtAcc.Customer.UserId = custId;
                        Fanex.ExtAcc.Customer.IsMultiple = false;
                        Fanex.ExtAcc.Form.Show(resolveClientUrl('VirtualSports/EditSingle?custId=' + Fanex.ExtAcc.Customer.superId + '&roleId=' + Fanex.ExtAcc.Customer.Super + '&targetCustId=' + Fanex.ExtAcc.Customer.UserId + '&targetRoleId=' + Fanex.ExtAcc.Customer.Master + '&groupName=' + Fanex.ExtAcc.Customer.GroupName().groupName), 'Edit Virtual Sports Setting <font color="#6dcff6">' + title + '</font>', 880, 700, false, null, 1);
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
                    url  = "customersetting/ColossusBetSetting/ColossusBetSetting/EditSetting?custid=" + Fanex.ExtAcc.Customer.UserId + "&srecommend=" + Fanex.ExtAcc.Customer.superId + "&isMultipleEdit=0&userId=" + parent.userId + "&roleId=3&groupName=" + parent.$('#selectedGroupName').html();
                    Fanex.ExtAcc.Form.ShowFrame(resolveClientUrl(url), 'Edit ColossusBets Setting <font color="#6dcff6">' + title + '</font>', 900, 400, false, false, 1);
                }
            }
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
    },

    ShowEditAgent: function (custId, title, transferCondition, syncCasino, disableCasino, syncBingo, disableBingo, disableP2P, syncLiveCasino, disableLiveCasino, hideVirtualSport, disableVirtualSport, masterSyncCasino, masterSyncBingo, masterSyncLiveCasino, isHideKeno, disableKeno, isHideGoldDeluxe, disableGoldDeluxe, isHideColossusBets, disableColossusBets) {
        this.TransferCondition = transferCondition;
        this.UserId = custId;
        this.TitleEditInfo = unescape(title);
        this.HideVirtualSports = hideVirtualSport;
        this.HideKeno = showKenoSetting == 'false' || isHideKeno;
        this.HideGoldDeluxe = showGoldDeluxeSetting == 0 || isHideGoldDeluxe;
        this.HideColossusBet = showColossusBets == 0 || isHideColossusBets;

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

        if (masterSyncBingo) {
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
                    url  = "customersetting/ColossusBetSetting/ColossusBetSetting/EditSetting?custid=" + Fanex.ExtAcc.Customer.UserId + "&mrecommend=" + Fanex.ExtAcc.Customer.masterId + "&isMultipleEdit=0&userId=" + parent.userId + "&roleId=2&groupName=" + parent.$('#selectedGroupName').html();
                    Fanex.ExtAcc.Form.ShowFrame(resolveClientUrl(url), 'Edit ColossusBets Setting <font color="#6dcff6">' + title + '</font>', 900, 400, false, false, 1);
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
    },

    ShowEditMember: function (custId, title, transferCondition, syncCasino, disableCasino, syncBingo, disableBingo, disableP2P, syncLiveCasino, disableLiveCasino, hideVirtualSport, disableVirtualSport, agentSyncCasino, agentSyncBingo, agentSyncLiveCasino, isHideKeno, disableKeno, isHideGoldDeluxe, disableGoldDeluxe, isHideColossusBets, disableColossusBets) {
        this.TransferCondition = transferCondition;
        this.UserId = custId;
        this.TitleEditInfo = unescape(title);
        this.IsMultiple = false;
        this.HideVirtualSports = hideVirtualSport;
        this.HideKeno = showKenoSetting == 'false' || isHideKeno;
        this.HideGoldDeluxe = showGoldDeluxeSetting == 0 || isHideGoldDeluxe;
        this.HideColossusBet = showColossusBets == 0 || isHideColossusBets;

        this.ResetFlags();

        var itemsMember = {
            sportbook: {
                name: "Sportsbook",
                className: "iconEnableEdit",
                callback: function () {
                    Fanex.ExtAcc.Customer.UserId = custId;
                    Fanex.ExtAcc.Customer.IsMultiple = false;
                    Fanex.ExtAcc.Customer.IsFullInfo = true;
                    Fanex.ExtAcc.Form.Show(resolveClientUrl('SportsbookSetting/EditMultiplePT?roleId=1&groupName=' + Fanex.ExtAcc.Customer.GroupName().groupName + '&custId=' + Fanex.ExtAcc.Customer.UserId), 'Edit Sportsbook Setting ' + '<font color="#6dcff6">' + title + '</font>', 900, 700, false, false, 1);
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
                    Fanex.ExtAcc.Form.Show(resolveClientUrl('SportsbookSetting/EditCommissionMemberDetail?groupName=' + Fanex.ExtAcc.Customer.GroupName().groupName + '&custId=' + Fanex.ExtAcc.Customer.UserId), 'Edit Commission ' + '<font color="#6dcff6">' + title + '</font>', 871, 350, false, false, 1);
                }
            },
            separator: "-----",
            racing: {
                name: "Racing",
                className: "iconEnableEdit",
                callback: function (key, opt) {
                    Fanex.ExtAcc.Customer.UserId = custId;
                    Fanex.ExtAcc.Customer.IsMultiple = false;
                    Fanex.ExtAcc.Form.ShowFrame(resolveClientUrl('customersetting/racebook/racingsetting/member?id=' + Fanex.ExtAcc.Customer.UserId + '&uplineid=' + Fanex.ExtAcc.Customer.agentId + '&groupName=' + $('#selectedGroupName').html()), 'Edit Racing Setting <font color="#6dcff6">' + title + '</font>', 1050, 720, false, null, 1);
                }
            },
            separator1: "-----"
        };

        var has3rdParty = false;
        if (agentSyncCasino) {
            has3rdParty = true;
            if (syncCasino) {
                if (disableCasino) {
                    itemsMember.casino = {
                        name: "Casino",
                        className: "iconDisableEdit",
                        callback: function () {
                            Fanex.ExtAcc.Customer.UserId = custId;
                            Fanex.ExtAcc.Customer.IsMultiple = false;
                            Fanex.ExtAcc.Form.ShowFrame(resolveClientUrl('CustomerSetting/Casino/Member/EditMember/' + Fanex.ExtAcc.Customer.UserId + '/' + Fanex.ExtAcc.Customer.agentId + '/' + $('#selectedGroupName').html()), 'Edit Casino Setting <font color="#6dcff6">' + title + '</font>', 1064, 700, false, null, 1);
                        }
                    };
                } else {
                    itemsMember.casino = {
                        name: "Casino",
                        className: "iconEnableEdit",
                        callback: function () {
                            Fanex.ExtAcc.Customer.UserId = custId;
                            Fanex.ExtAcc.Customer.IsMultiple = false;
                            Fanex.ExtAcc.Form.ShowFrame(resolveClientUrl('CustomerSetting/Casino/Member/EditMember/' + Fanex.ExtAcc.Customer.UserId + '/' + Fanex.ExtAcc.Customer.agentId + '/' + $('#selectedGroupName').html()), 'Edit Casino Setting <font color="#6dcff6">' + title + '</font>', 1064, 700, false, null, 1);
                        }
                    };
                }
            } else {
                itemsMember.casino = {
                    name: "Casino",
                    className: "iconUnsyncEdit",
                    callback: function () {
                        Fanex.ExtAcc.Customer.UserId = custId;
                        Fanex.ExtAcc.Customer.IsMultiple = false;
                        Fanex.ExtAcc.Form.ShowFrame(resolveClientUrl('CustomerSetting/Casino/Member/EditMember/' + Fanex.ExtAcc.Customer.UserId + '/' + Fanex.ExtAcc.Customer.agentId + '/' + $('#selectedGroupName').html()), 'Edit Casino Setting <font color="#6dcff6">' + title + '</font>', 1064, 700, false, null, 1);
                    }
                };
            }
        }

        if (agentSyncBingo) {
            has3rdParty = true;
            if (syncBingo) {
                if (disableBingo) {
                    itemsMember.bingo = {
                        name: "Bingo",
                        className: "iconDisableEdit",
                        callback: function (key, opt) {
                            Fanex.ExtAcc.Customer.UserId = custId;
                            Fanex.ExtAcc.Customer.IsMultiple = false;
                            Fanex.ExtAcc.Form.ShowFrame(resolveClientUrl('CustomerSetting/Bingo/BingoSetting/Member?id=' + Fanex.ExtAcc.Customer.UserId + '&uplineid=' + Fanex.ExtAcc.Customer.agentId + '&groupName=' + $('#selectedGroupName').html()), 'Edit Bingo Setting <font color="#6dcff6">' + title + '</font>', 1050, 500, false, null, 1);
                        }
                    };
                } else {
                    itemsMember.bingo = {
                        name: "Bingo",
                        className: "iconEnableEdit",
                        callback: function (key, opt) {
                            Fanex.ExtAcc.Customer.UserId = custId;
                            Fanex.ExtAcc.Customer.IsMultiple = false;
                            Fanex.ExtAcc.Form.ShowFrame(resolveClientUrl('CustomerSetting/Bingo/BingoSetting/Member?id=' + Fanex.ExtAcc.Customer.UserId + '&uplineid=' + Fanex.ExtAcc.Customer.agentId + '&groupName=' + $('#selectedGroupName').html()), 'Edit Bingo Setting <font color="#6dcff6">' + title + '</font>', 1050, 500, false, null, 1);
                        }
                    };
                }
            } else {
                itemsMember.bingo = {
                    name: "Bingo",
                    className: "iconUnsyncEdit",
                    callback: function (key, opt) {
                        Fanex.ExtAcc.Customer.UserId = custId;
                        Fanex.ExtAcc.Customer.IsMultiple = false;
                        Fanex.ExtAcc.Form.ShowFrame(resolveClientUrl('CustomerSetting/Bingo/BingoSetting/Member?id=' + Fanex.ExtAcc.Customer.UserId + '&uplineid=' + Fanex.ExtAcc.Customer.agentId + '&groupName=' + $('#selectedGroupName').html()), 'Edit Bingo Setting <font color="#6dcff6">' + title + '</font>', 1050, 500, false, null, 1);
                    }
                };
            }
        }

        if (!disableP2P) {
            //itemsMember.poker = { name: "Poker", className: "iconEnableEdit" };
        }

        if (agentSyncLiveCasino) {
            has3rdParty = true;

            itemsMember.liveCasino = {
                name: "Live Casino",
                className: syncLiveCasino == true ? (disableLiveCasino == true ? "iconDisableEdit" : "iconEnableEdit") : "iconUnsyncEdit",
                callback: function () {
                    Fanex.ExtAcc.Customer.UserId = custId;
                    url = "LiveCasinoSetting/Setting/EditSetting?uplineId=" + Fanex.ExtAcc.Customer.agentId + "&parentId=" + Fanex.ExtAcc.Customer.agentId + "&uplineLevel=2&customerId=" + Fanex.ExtAcc.Customer.UserId + "&customerLevel=1" + "&actorId=" + parent.userId + "&actorUsername=" + parent.username + "&groupName=" + parent.$('#selectedGroupName').html();
                    Fanex.ExtAcc.Form.ShowFrame(resolveClientUrl(url), 'Edit Live Casino Setting <font color="#6dcff6">' + title + '</font>', 900, 700, false, false, 1);
                }
            };
        }

        if (!hideVirtualSport) {
            has3rdParty = true;
            if (disableVirtualSport) {
                itemsMember.virtualSport = {
                    name: "Virtual Sports",
                    className: "iconDisableEdit",
                    callback: function () {
                        Fanex.ExtAcc.Customer.UserId = custId;
                        Fanex.ExtAcc.Customer.IsMultiple = false;
                        Fanex.ExtAcc.Form.Show(resolveClientUrl('VirtualSports/EditSingle?custId=' + Fanex.ExtAcc.Customer.agentId + '&roleId=' + Fanex.ExtAcc.Customer.Agent + '&targetCustId=' + Fanex.ExtAcc.Customer.UserId + '&targetRoleId=' + Fanex.ExtAcc.Customer.Member + '&groupName=' + Fanex.ExtAcc.Customer.GroupName().groupName), 'Edit Virtual Sports Setting <font color="#6dcff6">' + title + '</font>', 880, 700, false, null, 1);
                    }
                };
            } else {
                itemsMember.virtualSport = {
                    name: "Virtual Sports",
                    className: "iconEnableEdit",
                    callback: function () {
                        Fanex.ExtAcc.Customer.UserId = custId;
                        Fanex.ExtAcc.Customer.IsMultiple = false;
                        Fanex.ExtAcc.Form.Show(resolveClientUrl('VirtualSports/EditSingle?custId=' + Fanex.ExtAcc.Customer.agentId + '&roleId=' + Fanex.ExtAcc.Customer.Agent + '&targetCustId=' + Fanex.ExtAcc.Customer.UserId + '&targetRoleId=' + Fanex.ExtAcc.Customer.Member + '&groupName=' + Fanex.ExtAcc.Customer.GroupName().groupName), 'Edit Virtual Sports Setting <font color="#6dcff6">' + title + '</font>', 880, 700, false, null, 1);
                    }
                };
            }
        }

        if (!isHideKeno) {
            has3rdParty = true;
            if (disableKeno) {
                itemsMember.keno = {
                    name: "Keno",
                    className: "iconDisableEdit",
                    callback: function () {
                        Fanex.ExtAcc.Customer.UserId = custId;
                        Fanex.ExtAcc.Customer.IsMultiple = false;
                        Fanex.ExtAcc.Form.ShowFrame(resolveClientUrl('customersetting/KenoSetting/kenosetting/AgentEditMember?custId=' + Fanex.ExtAcc.Customer.UserId + "&userId=" + parent.userId), 'Edit Keno Setting <font color="#6dcff6">' + title + '</font>', 900, 700, false, null, 1);
                    }
                };
            }
            else {
                itemsMember.keno = {
                    name: "Keno",
                    className: "iconEnableEdit",
                    callback: function () {
                        Fanex.ExtAcc.Customer.UserId = custId;
                        Fanex.ExtAcc.Customer.IsMultiple = false;
                        Fanex.ExtAcc.Form.ShowFrame(resolveClientUrl('customersetting/KenoSetting/kenosetting/AgentEditMember?custId=' + Fanex.ExtAcc.Customer.UserId + "&userId=" + parent.userId), 'Edit Keno Setting <font color="#6dcff6">' + title + '</font>', 900, 700, false, null, 1);
                    }
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
                    Fanex.ExtAcc.Form.ShowFrame(resolveClientUrl(url), 'Edit Gold Deluxe Setting <font color="#6dcff6">' + title + '</font>', 900, 400, false, false, 1);
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
                    Fanex.ExtAcc.Form.ShowFrame(resolveClientUrl(url), 'Edit ColossusBets <font color="#6dcff6">' + title + '</font>', 900, 400, false, false, 1);
                }
            };
        }

        if (has3rdParty)
            itemsMember.separator5 = "-----";
        itemsMember.editInfo = {
            name: "Edit Info",
            className: "iconEnableEdit editInfoBg",
            callback: function (key, opt) {
                Fanex.ExtAcc.Customer.UserId = custId;
                Fanex.ExtAcc.Customer.IsMultiple = false;
                Fanex.ExtAcc.Form.Show(resolveClientUrl('Customer/EditInfo/?custId=' + Fanex.ExtAcc.Customer.UserId + '&groupName=' + Fanex.ExtAcc.Customer.GroupName().groupName), titles[0] + ': <font color="#6dcff6">' + Fanex.ExtAcc.Customer.TitleEditInfo + '</font>', 500, 460, false);
            }
        };
        itemsMember.changePassword = {
            name: "Reset Password",
            className: "iconEnableEdit editInfoBg",
            callback: function (key, opt) {
                Fanex.ExtAcc.Customer.UserId = custId;
                Fanex.ExtAcc.Customer.IsMultiple = false;
                Fanex.ExtAcc.Form.Show(resolveClientUrl('Customer/ResetPassword/?custId=' + Fanex.ExtAcc.Customer.UserId + '&groupName=' + Fanex.ExtAcc.Customer.GroupName().groupName), 'Reset password: <font color="#6dcff6">' + Fanex.ExtAcc.Customer.TitleEditInfo + '</font>', 415, 350, false);
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
    },

    ResetInfo: function () {
        $('#FirstName').val(Fanex.ExtAcc.Customer.FirstName);
        $('#LastName').val(Fanex.ExtAcc.Customer.LastName);
        $('#HomePhone').val(Fanex.ExtAcc.Customer.HomePhone);
        $('#MobilePhone').val(Fanex.ExtAcc.Customer.MobilePhone);
        $('#Email').val(Fanex.ExtAcc.Customer.Email);
        $('#Fax').val(Fanex.ExtAcc.Customer.Fax);
        $('#FirstName').focus();
    },

    EditInfo: function () {
        var firstName = $('#FirstName').val();
        var lastName = $('#LastName').val();
        var homePhone = $('#HomePhone').val();
        var mobilePhone = $('#MobilePhone').val();
        var email = $('#Email').val();
        var fax = $('#Fax').val();
        var remark = $('#Remark').val();

        var editUrl = 'Customer/EditInfoPost';

        var data = {
            custId: this.UserId,
            firstName: Fanex.ExtAcc.EscapeHtml(firstName),
            lastName: Fanex.ExtAcc.EscapeHtml(lastName),
            homePhone: Fanex.ExtAcc.EscapeHtml(homePhone),
            mobilePhone: Fanex.ExtAcc.EscapeHtml(mobilePhone),
            email: Fanex.ExtAcc.EscapeHtml(email),
            fax: Fanex.ExtAcc.EscapeHtml(fax),
            remark: Fanex.ExtAcc.EscapeHtml(remark),
            groupName: Fanex.ExtAcc.Customer.GroupName().groupName
        };

        Fanex.ExtAcc.BlockUI();

        $.ajax({
            url: resolveClientUrl(editUrl),
            data: data,
            type: 'POST'
        })
            .done(function (data) {
                if (!data.isSuccess) {
                    $('#errMsg').show();
                    $('#errMsg').html(data.errorMessage.formatErrorMessage());
                    Fanex.ExtAcc.UnBlockUI();
                } else {
                    if (document.getElementById('customerListGrid')) {
                        var timeout = setTimeout(function () {
                            clearTimeout(timeout);
                            Fanex.ExtAcc.Customer.CallBackCustomerList();
                            Fanex.ExtAcc.UnBlockUI();
                            Fanex.ExtAcc.Form.Close();
                        }, 2000);
                    } else {
                        Fanex.ExtAcc.UnBlockUI();
                        Fanex.ExtAcc.Form.Close();
                    }
                }
            })
            .error(function (data) {
                if (typeof console != 'undefined') console.log(data.responseText);
                Fanex.ExtAcc.UnBlockUI();
            });
    },

    GetSuperList: function (custId, dataSource) {
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
                var func = "javascript:Fanex.ExtAcc.Customer.GetMasterList(" + custId + ", " + cell.Value.DoubleCommLosing + ", null, null, " + cell.Value.synccasino + ", " + cell.Value.syncBingo + ", " + cell.Value.SyncLiveCasino + "," + hideKeno + "," + cell.Value.HideVirtualSports + "," + cell.Value.HideGoldDeluxe + "," + cell.Value.HideColossusBet + ")";
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
                var func = "javascript:Fanex.ExtAcc.Customer.ShowEditSuper(" + cell.Value.custid + ", '" + escape(cell.Value.username) + "', " + (cell.Value.HideGoldDeluxe) + ")";

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
    },

    GetMasterList: function (srecommend, allow, custId, dataSource, superSyncCasino, superSyncBingo, superSyncLiveCasino, isHideKeno, isHideVirtualSport, isHideGoldDeluxe, isHideColossusBets) {
        Fanex.ExtAcc.BlockUI();

        this.superId = srecommend;
        this.CurrentLevel = Fanex.ExtAcc.Customer.Master;
        this.AllowDoubleComm = allow;
        this.AllowDoubleCommMaster = allow;

        this.superSyncCasino = superSyncCasino;
        this.superSyncBingo = superSyncBingo;
        this.superSyncLiveCasino = superSyncLiveCasino;
        this.HideKeno = showKenoSetting == 'false' || isHideKeno;
        this.HideVirtualSports = isHideVirtualSport;
        this.HideGoldDeluxe = isHideGoldDeluxe;
        this.HideColossusBet = isHideColossusBets;

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
                return '<div onclick="Fanex.ExtAcc.Customer.Select(this)" style="margin-left: 0px !important" id="' + cell.Value.custid + '" class="menuInactive"></div>';
            }
        }, {
            HeaderText: 'Username',
            Name: 'username',
            Width: 100,
            CustomCellRender: function (cell) {
                var custId = cell.Value.custid;
                var func = "javascript:Fanex.ExtAcc.Customer.GetAgentList(" + custId + ", " + cell.Value.DoubleCommLosing + ", null, null, " + cell.Value.synccasino + ", " + cell.Value.syncBingo + ", " + cell.Value.SyncLiveCasino + "," + cell.Value.HideKeno + "," + cell.Value.HideVirtualSports + "," + cell.Value.HideGoldDeluxe + "," + cell.Value.HideColossusBet + ")";
                Fanex.ExtAcc.Customer.masterDisableP2P = cell.Value.DisableP2P;
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
                var func = "javascript:Fanex.ExtAcc.Customer.ShowEditMaster(" + cell.Value.custid + ", '" + title + "', " + cell.Value.disableTransfer + ", " + cell.Value.synccasino + ", " + cell.Value.disablecasino + ", " + cell.Value.syncBingo + ", " + cell.Value.DisableBingo + ", " + cell.Value.DisableP2P + ", " + cell.Value.SyncLiveCasino + ", " + cell.Value.DisableLiveCasino + ", " + cell.Value.HideVirtualSports + ", " + cell.Value.DisableVirtualSports + ", " + superSyncCasino + ", " + superSyncBingo + ", " + superSyncLiveCasino + "," + hideKeno + ", " + cell.Value.DisableKeno + "," + hideGoldDeluxe + ", " + cell.Value.DisableGoldDeluxe + "," + hideColossusBets + ", " + cell.Value.DisableColossusBet + ")";

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
    },

        GetAgentList: function (mrecommend, allow, custId, dataSource, masterSyncCasino, masterSyncBingo, masterSyncLiveCasino, isHideKeno, isHideVirtualSport, isHideGoldDeluxe, isHideColossusBets, disableColossusBets) {
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
                var func = "javascript:Fanex.ExtAcc.Customer.GetMemberList(" + custId + ", " + cell.Value.DoubleCommLosing + ", null, null, " + cell.Value.synccasino + ", " + cell.Value.syncBingo + ", " + cell.Value.SyncLiveCasino + "," + cell.Value.HideKeno + "," + cell.Value.HideVirtualSports + "," + cell.Value.HideGoldDeluxe + "," + cell.Value.HideColossusBet + ")";
                Fanex.ExtAcc.Customer.agentDisableP2P = cell.Value.DisableP2P;
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
                var func = "javascript:Fanex.ExtAcc.Customer.ShowEditAgent(" + cell.Value.custid + ", '" + title + "', " + cell.Value.disableTransfer + ", " + cell.Value.synccasino + ", " + cell.Value.disablecasino + ", " + cell.Value.syncBingo + ", " + cell.Value.DisableBingo + ", " + cell.Value.DisableP2P + ", " + cell.Value.SyncLiveCasino + ", " + cell.Value.DisableLiveCasino + ", " + cell.Value.HideVirtualSports + ", " + cell.Value.DisableVirtualSports + ", " + masterSyncCasino + ", " + masterSyncBingo + ", " + masterSyncLiveCasino + "," + hideKeno + ", " + cell.Value.DisableKeno + "," + hideGoldDeluxe + ", " + cell.Value.DisableGoldDeluxe + "," + hideColossusBets + ", " + cell.Value.DisableColossusBet + ")";

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
    },

        GetMemberList: function (recommend, allow, custId, dataSource, agentSyncCasino, agentSyncBingo, agentSyncLiveCasino, isHideKeno, isHideVirtualSport, isHideGoldDeluxe, isHideColossusBets) {
        Fanex.ExtAcc.BlockUI();

        this.agentId = recommend;
        this.CurrentLevel = this.Member;
        this.AllowDoubleComm = allow;

        this.agentSyncCasino = agentSyncCasino;
        this.agentSyncBingo = agentSyncBingo;
        this.agentSyncLiveCasino = agentSyncLiveCasino;
        this.HideKeno = showKenoSetting == 'false' || isHideKeno;
        this.HideVirtualSports = isHideVirtualSport;
        this.HideGoldDeluxe = isHideGoldDeluxe;
        this.HideColossusBet = isHideColossusBets

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
                return '<div onclick="Fanex.ExtAcc.Customer.Select(this)" style="margin-left: 0px !important" id="' + cell.Value.custid + '" class="menuInactive"></div>';
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
                var hideGoldDeluxe = showGoldDeluxeSetting == 0 || cell.Value.HideGoldDeluxe || isHideGoldDeluxe;
                var hideColossusBets = showColossusBets == 0 || cell.Value.HideColossusBet || isHideColossusBets;
                var func = "javascript:Fanex.ExtAcc.Customer.ShowEditMember(" + cell.Value.custid + ", '" + title + "'," + cell.Value.disableTransfer + ", " + cell.Value.synccasino + ", " + cell.Value.disablecasino + ", " + cell.Value.syncBingo + ", " + cell.Value.DisableBingo + ", " + cell.Value.DisableP2P + ", " + cell.Value.SyncLiveCasino + ", " + cell.Value.DisableLiveCasino + ", " + cell.Value.HideVirtualSports + ", " + cell.Value.DisableVirtualSports + ", " + agentSyncCasino + ", " + agentSyncBingo + ", " + agentSyncLiveCasino + "," + hideKeno + ", " + cell.Value.DisableKeno + "," + hideGoldDeluxe + ", " + cell.Value.DisableGoldDeluxe + "," + hideColossusBets + ", " + cell.Value.DisableColossusBet + ")";

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
                })
                .error(function (xhr, status, err) {
                    Fanex.ExtAcc.UnBlockUI();
                });
        }
    },

    ResetPwd: function (isMember) {
        if (!Validators.validateInput($('#Password').val())) {
            $('#Password').focus();
            Validators.show($('#Password').get(0), 'This field is required');
            return false;
        }

        if (!this.validatePassword($('#Password').val(), isMember)) {
            $('#Password').focus();
            Validators.show($('#Password').get(0), 'Your password does not meet the minimum security requirement');
            return false;
        }

        if (!Validators.validateInput($('#confirmPwd').val())) {
            $('#confirmPwd').focus();
            Validators.show($('#confirmPwd').get(0), 'This field is required');
            return false;
        }

        if (!this.validatePassword($('#confirmPwd').val(), isMember)) {
            $('#Password').focus();
            Validators.show($('#confirmPwd').get(0), 'Your password does not meet the minimum security requirement');
            return false;
        }

        if ($('#confirmPwd').val() !== $('#Password').val()) {
            $('#confirmPwd').focus();
            Validators.show($('#confirmPwd').get(0), 'Confirm password is not matched');
            return false;
        }

        Fanex.ExtAcc.BlockUI();

        $.ajax({
            url: resolveClientUrl('Customer/UpdatePassword/?custId=' + $('#CustId').val() + '&password=' + $('#Password').val()),
            type: 'POST',
            data: Fanex.ExtAcc.Customer.GroupName()
        })
            .done(function (data) {
                if (!data.isSuccess) {
                    $('#errMsg').css('display', 'block');
                    $('#errMsg').html(data.errorMessage.formatErrorMessage());
                } else {
                    Fanex.ExtAcc.Form.Close(false);
                }

                Fanex.ExtAcc.UnBlockUI();
            })
            .error(function (data) {
                if (typeof console != 'undefined') console.log(data.responseText);
                Fanex.ExtAcc.UnBlockUI();
            });
    },

    ShowIpInfo: function (ip) {
        Fanex.ExtAcc.BlockUI();

        Fanex.ExtAcc.Form.Show(resolveClientUrl('Customer/ViewIP?ip=' + ip), 'IP Information', 450, 230, false,
            function () {
                Fanex.ExtAcc.UnBlockUI();
            },
            2);
    },

    CallBackCustomerList: function () {
        if (this.CurrentLevel == this.Super) {
            this.GetSuperList(this.custId);
        } else if (this.CurrentLevel == this.Master) {
            this.GetMasterList(this.superId, this.AllowDoubleComm, this.custId, null, this.superSyncCasino, this.superSyncBingo, this.superSyncLiveCasino, this.HideKeno, this.HideVirtualSports, this.HideGoldDeluxe, this.HideColossusBet);
        } else if (this.CurrentLevel == this.Agent) {
            this.GetAgentList(this.masterId, this.AllowDoubleComm, this.custId, null, this.masterSyncCasino, this.masterSyncBingo, this.masterSyncLiveCasino, this.HideKeno, this.HideVirtualSports, this.HideGoldDeluxe, this.HideColossusBet);
        } else {
            this.GetMemberList(this.agentId, this.AllowDoubleComm, this.custId, null, this.agentSyncCasino, this.agentSyncBingo, this.agentSyncLiveCasino, this.HideKeno, this.HideVirtualSports, this.HideGoldDeluxe, this.HideColossusBet);
        }
    },

    ConfigMultipleDoubleComm: function (custids, allow, index) {
        Fanex.ExtAcc.BlockUI();

        if (index == 0) {
            this.MultipleDblCommError = '';
        }

        var len = custids.length - 1;
        var current = index;

        $.ajax({
            url: resolveClientUrl('Customer/ConfigDoubleComm'),
            dataType: 'json',
            type: 'post',
            data: {
                custid: parseInt(custids[current]),
                allow: allow,
                groupName: Fanex.ExtAcc.Customer.GroupName().groupName
            }
        })
            .success(function (result) {
                if (result.errorCode && result.errorCode == '1') {
                    Fanex.ExtAcc.UnBlockUI();
                    Fanex.ExtAcc.Form.Alert('Error', result.errorMessage, 400, 150, 'alert');
                    Fanex.ExtAcc.Form.Close();
                } else {
                    if (!result.isSuccess) {
                        Fanex.ExtAcc.Customer.MultipleDblCommError += (result.errorCode == '600' ? ('Account ' + Fanex.ExtAcc.Customer.Names()[current] + ' was disableed<br />') : (result.errorMessage + '<br/>'));
                    }

                    if (current == len) {
                        var timeout = setTimeout(function () {
                            clearTimeout(timeout);
                            Fanex.ExtAcc.Form.Close();
                            Fanex.ExtAcc.UnBlockUI();

                            if (Fanex.ExtAcc.Customer.MultipleDblCommError != '') {
                                Fanex.ExtAcc.Form.Alert('Error', Fanex.ExtAcc.Customer.MultipleDblCommError.formatErrorMessage(), 300, 150, 'alert');
                            }

                            Fanex.ExtAcc.Customer.CallBackCustomerList();

                        }, 2000);
                    }

                    current++;

                    if (current <= len) {
                        Fanex.ExtAcc.Customer.ConfigMultipleDoubleComm(custids, allow, current);
                    }
                }
            })
            .error(function (xhr, status, err) {
                if (typeof console != 'undefined') {
                    console.log(err);
                }

                Fanex.ExtAcc.Customer.MultipleDblCommError += err + '<br/>';

                current++;

                if (current <= len) {
                    Fanex.ExtAcc.Customer.ConfigMultipleDoubleComm(custids, allow, current);
                }

                Fanex.ExtAcc.UnBlockUI();
            });
    },

    ConfigDoubleComm: function (custid, isSuper) {
        var allow = $('#dbl-' + custid).css('cursor') == 'pointer';

        if (allow || isSuper) {
            var msg = '';
            var title = '';

            if ($('#dbl-' + custid).attr('class') == 'disallowDblComm') {
                msg = 'Are you sure you want to allow Double Commission feature for Sportsbook\'s Losing tickets on this customer?';
                title = 'Allow Double Commission';
            } else if ($('#dbl-' + custid).attr('class') == 'allowDblComm') {
                msg = 'Are you sure you want to disallow Double Commission feature for Sportsbook\'s Losing tickets on this customer? Warning, all members under this agent will be disallowed as well!';
                title = 'Disallow Double Commission';
            } else if ($('#dbl-' + custid).attr('class') == 'onDblComm') {
                msg = 'Are you sure you want to turn OFF Double Commission feature for Sportsbook\'s Losing tickets on this customer?';
                title = 'Turn OFF Double Commission';
            } else {
                msg = 'Are you sure you want to turn ON Double Commission feature for Sportsbook\'s Losing tickets on this customer?';
                title = 'Turn ON Double Commission';
            }

            Fanex.ExtAcc.Form.ConfirmBox(title, msg, 450, 170, false, function () {
                Fanex.ExtAcc.Form.Close(false);
                Fanex.ExtAcc.BlockUI();

                $.ajax({
                    url: resolveClientUrl('Customer/ConfigDoubleComm'),
                    dataType: 'json',
                    type: 'post',
                    data: {
                        custid: custid,
                        allow: ($('#dbl-' + custid).attr('class') == 'disallowDblComm' || $('#dbl-' + custid).attr('class') == 'offDblComm'),
                        groupName: Fanex.ExtAcc.Customer.GroupName().groupName
                    }
                })
                    .success(function (result) {
                        if (result.isSuccess) {
                            var timeout = setTimeout(function () {
                                clearTimeout(timeout);
                                Fanex.ExtAcc.Customer.CallBackCustomerList();
                                Fanex.ExtAcc.UnBlockUI();
                            }, 2000);
                        } else {
                            Fanex.ExtAcc.UnBlockUI();
                            Fanex.ExtAcc.Form.Alert('Error', result.errorCode == "600" ? "This account was disabled".formatErrorMessage() : result.errorMessage.formatErrorMessage(), 300, 150, 'alert');
                        }
                    })
                    .error(function (xhr) {
                        if (typeof console != 'undefined') {
                            console.log(xhr);
                        }
                    });
            });
        }
    },

    Select: function (div) {
        $('#editMult').attr('style', 'cursor: default !important');

        if (div.className == 'menuInactive') {
            div.className = 'menuActive';
            $('#editMult').attr('style', 'cursor: pointer !important');
            $('#editMult').attr('onclick', 'javascript:Fanex.ExtAcc.Customer.EditMultiple(this)');
            $('#editMult').contextMenu(true);
        } else {
            div.className = 'menuInactive';
            $('#editMult').attr('style', 'cursor: default !important');
            $('#editMult').attr('onclick', '');
            $('#editMult').contextMenu(false);
        }

        var divs = $('div');

        for (var i = 0; i < divs.length; i++) {
            if (divs[i].id == div.id && div.className == 'menuInactive') {
                divs[i].className = 'menuInactive';
            }
        }

        for (var i = 0; i < divs.length; i++) {
            if ((divs[i].className == 'menuActive' || divs[i].className == 'menuInactive') && divs[i].className == 'menuActive') {
                $('#editMult').attr('style', 'cursor: pointer !important');
                $('#editMult').attr('onclick', 'javascript:Fanex.ExtAcc.Customer.EditMultiple(this)');
                $('#editMult').contextMenu(true);

                break;
            }
        }
    },

    SelectAll: function (className) {
        if (className == 'menuActive') {
            $('#editMult').attr('style', 'cursor: pointer !important');
            $('#editMult').attr('onclick', 'javascript:Fanex.ExtAcc.Customer.EditMultiple(this)');
            $('#editMult').contextMenu(true);
        } else {
            $('#editMult').attr('style', 'cursor: default !important');
            $('#editMult').attr('onclick', '');
            $('#editMult').contextMenu(false);
        }

        var divs = document.getElementsByTagName('div');

        for (var i = 0; i < divs.length; i++) {
            if (divs[i].id && (divs[i].className == 'menuActive' || divs[i].className == 'menuInactive')) {
                divs[i].className = className;
            }
        }
    },

    Render: function (options) {
        try {
            if (this.DataSource.length == 0) {
                $('#msgNoinfo').css('display', 'block');
                $('#jqgrid').css('display', 'none');
                $('#exporttoexcel').css('display', 'none');
            } else {
                $('#msgNoinfo').css('display', 'none');
                $('#jqgrid').css('display', 'block');

                $('#exporttoexcel').show();

                // Init Grid jqgrid
                $('#jqgrid').fanexGrid(options);
            }

            $('#editMult').attr('style', 'cursor: default !important');

            $('#editMult').attr('onclick', '');
            $('#editMult').contextMenu(false);

            Fanex.ExtAcc.UnBlockUI();
        } catch (e) {
            if (typeof console != 'undefined') {
                console.log(e);
            }

            Fanex.ExtAcc.UnBlockUI();
        }
    },

    SearchList: function () {
        if ($('#txtUserName').val() == '' && this.CurrentLevel == this.Super) {
            this.GetSuperList();
        } else {
            this.GetSearchList();
        }
    },

    GetSearchList: function () {
        Fanex.ExtAcc.BlockUI();

        var data = this.GetParams();

        var url = resolveClientUrl('Customer/SearchListPost');

        $.ajax({
            url: url,
            data: data,
            dataType: 'json',
            type: 'post'
        })
        .success(function (result) {
            var rows = $.parseJSON(result.rows);
            var options = null;
            var bodyRows = [];

            Fanex.ExtAcc.Customer.DataSource = rows;

            if (result.roleId != 0 && rows.length > 0) {
                var custId = rows[0].custid;

                Fanex.ExtAcc.Customer.directSuperId = result.srecommend;
                Fanex.ExtAcc.Customer.directMasterId = result.mrecommend;
                Fanex.ExtAcc.Customer.directAgentId = result.recommend;
                Fanex.ExtAcc.Customer.custId = Fanex.ExtAcc.Customer.UserId = result.custId;
                Fanex.ExtAcc.Customer.AllowDoubleComm = result.allowDoubleComm;

                if (result.roleId == 4) {
                    Fanex.ExtAcc.Customer.GetSuperList(rows[0].custid, result);
                } else if (result.roleId == 3) {
                    Fanex.ExtAcc.Customer.superSyncCasino = result.syncCasino;
                    Fanex.ExtAcc.Customer.superSyncBingo = result.syncBingo;
                    Fanex.ExtAcc.Customer.superSyncLiveCasino = result.syncLiveCasino;
                    Fanex.ExtAcc.Customer.GetMasterList(result.srecommend, Fanex.ExtAcc.Customer.AllowDoubleComm, custId, result, Fanex.ExtAcc.Customer.superSyncCasino, Fanex.ExtAcc.Customer.superSyncBingo, Fanex.ExtAcc.Customer.superSyncLiveCasino, Fanex.ExtAcc.Customer.HideKeno, Fanex.ExtAcc.Customer.HideVirtualSports, Fanex.ExtAcc.Customer.HideGoldDeluxe, Fanex.ExtAcc.Customer.HideColossusBet);
                } else if (result.roleId == 2) {
                    Fanex.ExtAcc.Customer.masterSyncCasino = result.syncCasino;
                    Fanex.ExtAcc.Customer.masterSyncBingo = result.syncBingo;
                    Fanex.ExtAcc.Customer.masterSyncLiveCasino = result.syncLiveCasino;
                    Fanex.ExtAcc.Customer.GetAgentList(result.mrecommend, Fanex.ExtAcc.Customer.AllowDoubleComm, custId, result, Fanex.ExtAcc.Customer.masterSyncCasino, Fanex.ExtAcc.Customer.masterSyncBingo, Fanex.ExtAcc.Customer.masterSyncLiveCasino, Fanex.ExtAcc.Customer.HideKeno, Fanex.ExtAcc.Customer.HideVirtualSports, Fanex.ExtAcc.Customer.HideGoldDeluxe, Fanex.ExtAcc.Customer.HideColossusBet);
                    Fanex.ExtAcc.Customer.GetAgentList(result.mrecommend, Fanex.ExtAcc.Customer.AllowDoubleComm, custId, result, Fanex.ExtAcc.Customer.masterSyncCasino, Fanex.ExtAcc.Customer.masterSyncBingo, Fanex.ExtAcc.Customer.masterSyncLiveCasino, Fanex.ExtAcc.Customer.HideKeno, Fanex.ExtAcc.Customer.HideVirtualSports, Fanex.ExtAcc.Customer.HideGoldDeluxe, Fanex.ExtAcc.Customer.HideColossusBet);
                } else {
                    Fanex.ExtAcc.Customer.agentSyncCasino = result.syncCasino;
                    Fanex.ExtAcc.Customer.agentSyncBingo = result.syncBingo;
                    Fanex.ExtAcc.Customer.agentSyncLiveCasino = result.syncLiveCasino;
                    Fanex.ExtAcc.Customer.GetMemberList(result.recommend, Fanex.ExtAcc.Customer.AllowDoubleComm, custId, result, Fanex.ExtAcc.Customer.agentSyncCasino, Fanex.ExtAcc.Customer.agentSyncBingo, Fanex.ExtAcc.Customer.agentSyncLiveCasino, Fanex.ExtAcc.Customer.HideKeno, Fanex.ExtAcc.Customer.HideVirtualSports, Fanex.ExtAcc.Customer.HideGoldDeluxe, Fanex.ExtAcc.Customer.HideColossusBet);
                }
            } else {
                $('#msgNoinfo').css('display', 'block');
                $('#jqgrid').css('display', 'none');
                $('#exporttoexcel').css('display', 'none');

                Fanex.ExtAcc.UnBlockUI();
            }
        });
    },

    RenderSuperList: function (dataSource) {
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
    },

    RenderMasterList: function (dataSource) {
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

        $('#breadcrumbs').append('<a href="' + "javascript:Fanex.ExtAcc.Customer.GetSuperList(" + Fanex.ExtAcc.Customer.directSuperId + ")" + '">' + 'Super' + '</a>');

        $('#breadcrumbs').show();

        Fanex.ExtAcc.OnResize = function () {
            try {
                $("#jqgrid").fanexGrid("resizeGrid");
            } catch (err) { }
        }
    },

    RenderAgentList: function (dataSource) {
        if (dataSource.srecommend) {
            this.superId = dataSource.srecommend;
            this.masterId = dataSource.mrecommend;
            this.agentId = dataSource.recommend;
        }

        var linkSuper = '<a href="' + "javascript:Fanex.ExtAcc.Customer.GetSuperList(" + Fanex.ExtAcc.Customer.directSuperId + ")" + '">' + 'Super' + '</a>';
        var linkMaster = '<a href="' + "javascript:Fanex.ExtAcc.Customer.GetMasterList(" + Fanex.ExtAcc.Customer.superId + ', ' + Fanex.ExtAcc.Customer.AllowDoubleCommMaster + ',' + Fanex.ExtAcc.Customer.directMasterId + ', null, ' + Fanex.ExtAcc.Customer.superSyncCasino + ',' + Fanex.ExtAcc.Customer.superSyncBingo + ',' + Fanex.ExtAcc.Customer.superSyncLiveCasino + ',' + Fanex.ExtAcc.Customer.HideKeno + ',' + Fanex.ExtAcc.Customer.HideVirtualSports + "," + Fanex.ExtAcc.Customer.HideGoldDeluxe + "," + Fanex.ExtAcc.Customer.HideColossusBet + ')">' + 'Master' + '</a>';

        var rows = $.parseJSON(dataSource.rows);
        var options = null;
        var bodyRows = [];

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
    },

    RenderMemberList: function (dataSource) {
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
                    Value: rows[i].IsMyanmarOdds ? (rows[i].playerDiscountMyanmarOdds ? rows[i].playerDiscountMyanmarOdds : 0) : 'N/A'
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

        var linkSuper = '<a href="' + "javascript:Fanex.ExtAcc.Customer.GetSuperList(" + Fanex.ExtAcc.Customer.directSuperId + ")" + '">' + 'Super' + '</a>';
        var linkMaster = '<a href="' + "javascript:Fanex.ExtAcc.Customer.GetMasterList(" + Fanex.ExtAcc.Customer.superId + ', ' + Fanex.ExtAcc.Customer.AllowDoubleCommMaster + ',' + Fanex.ExtAcc.Customer.directMasterId + ', null, ' + Fanex.ExtAcc.Customer.superSyncCasino + ',' + Fanex.ExtAcc.Customer.superSyncBingo + ',' + Fanex.ExtAcc.Customer.superSyncLiveCasino + ',' + Fanex.ExtAcc.Customer.HideKeno + ',' + Fanex.ExtAcc.Customer.HideVirtualSports + ',' + Fanex.ExtAcc.Customer.HideGoldDeluxe + ',' + Fanex.ExtAcc.Customer.HideColossusBet + ')">' + 'Master' + '</a>';
        var linkAgent = '<a href="' + "javascript:Fanex.ExtAcc.Customer.GetAgentList(" + Fanex.ExtAcc.Customer.masterId + ', ' + Fanex.ExtAcc.Customer.AllowDoubleCommAgent + ',' + Fanex.ExtAcc.Customer.directAgentId + ', null, ' + Fanex.ExtAcc.Customer.masterSyncCasino + ',' + Fanex.ExtAcc.Customer.masterSyncBingo + ',' + Fanex.ExtAcc.Customer.masterSyncLiveCasino + ',' + Fanex.ExtAcc.Customer.HideKeno + ',' + Fanex.ExtAcc.Customer.HideVirtualSports + ',' + Fanex.ExtAcc.Customer.HideGoldDeluxe + ',' + Fanex.ExtAcc.Customer.HideColossusBet + ')">' + 'Agent' + '</a>';

        $('#breadcrumbs').append('<div style="float:left">' + linkAgent + '</div><div class="viewNext">&nbsp;</div><div style="float:left">' + linkMaster + '</div><div class="viewNext">&nbsp;</div><div style="float:left">' + linkSuper + '</div>');

        $('#breadcrumbs').show();

        Fanex.ExtAcc.OnResize = function () {
            try {
                $("#jqgrid").fanexGrid("resizeGrid");
            } catch (err) { }
        }
    },

    EditMultiple: function (div) {
        this.ResetFlags();
        var menuItems = {};

        this.isSuperEditMember = false;
        this.isMasterEditMember = false;
        this.IsFullInfo = false;
        this.IsMultiple = true;
        var ret = this.IdsAndNames();
        var info = this.CustomerInfo();
        //        this.HideGoldDeluxe = showGoldDeluxeSetting == 0;

        if (this.CurrentLevel == this.Super) {
            menuItems = {
                doubleComm: {
                    name: "Edit Multiple Double Comm",
                    className: "iconEnableEdit",
                    callback: function (key, opt) {
                        Fanex.ExtAcc.Form.Show(resolveClientUrl('Customer/ConfigMultipleDoubleComm?id=' + Fanex.ExtAcc.Customer.CurrentLevel + '&custId=' + Fanex.ExtAcc.Customer.UserId + '&groupName=' + Fanex.ExtAcc.Customer.GroupName().groupName),
                            'Edit Multiple Double Commission For Losing Tickets',
                            430, 150, false, false, 1);
                    }
                }
            };
        } else if (this.CurrentLevel == this.Master) {

            menuItems = {
                sportbook: {
                    name: "Edit Multiple Sportsbook",
                    className: "iconEnableEdit",
                    callback: function () {
                        Fanex.ExtAcc.Customer.ShowEditMultiplePT();
                    }
                },
                racing: {
                    name: "Edit Multiple Racing",
                    className: "iconEnableEdit",
                    callback: function (key, opt) {
                        Fanex.ExtAcc.Form.ShowFrame(resolveClientUrl('customersetting/racebook/racingsetting/master?id=' + info.custId + '&uplineid=' + info.uplineId + '&groupName=' + Fanex.ExtAcc.Customer.GroupName().groupName), Fanex.ExtAcc.Customer.IsMultiple ? 'Edit Multiple Racing Setting' : 'Edit Racing Setting ' + '<font color="#6dcff6">' + ret.Names[0] + '</font>', 1050, 775, false, null, 1);
                    }
                }
            };

            if (this.superSyncCasino) {
                menuItems.casino = {
                    name: "Edit Multiple Casino",
                    className: "iconEnableEdit",
                    callback: function () {
                        Fanex.ExtAcc.Customer.ShowEditMultipleCasino();
                    }
                };
            }

            if (this.superSyncBingo) {
                menuItems.bingo = {
                    name: "Edit Multiple Bingo",
                    className: "iconEnableEdit",
                    callback: function (key, opt) {
                        Fanex.ExtAcc.Form.ShowFrame(resolveClientUrl('CustomerSetting/Bingo/BingoSetting/Master?id=' + Fanex.ExtAcc.Customer.UserId + '&uplineid=' + Fanex.ExtAcc.Customer.superId + '&groupName=' + $('#selectedGroupName').html()), Fanex.ExtAcc.Customer.IsMultiple ? 'Edit Multiple Bingo Setting' : 'Edit Bingo Setting ' + '<font color="#6dcff6">' + ret.Names[0] + '</font>', 1050, 500, false, null, 1);
                    }
                };
            }

            if (!this.superDisableP2P) {
                //menuItems.poker = { name: "Edit Multiple Poker", className: "iconEnableEdit" };
            }

            if (this.superSyncLiveCasino) {
                menuItems.liveCasino = {
                    name: "Edit Multiple Live Casino",
                    className: "iconEnableEdit",
                    callback: function () {
                        Fanex.ExtAcc.Customer.ShowEditMultipleLiveCasino(Fanex.ExtAcc.Customer.superId, Fanex.ExtAcc.Customer.superId, 4, 3);
                    }
                };
            }

            if (!this.HideVirtualSports) {
                menuItems.virtualSport = {
                    name: "Edit Multiple Virtual Sports",
                    className: "iconEnableEdit",
                    callback: function () {
                        Fanex.ExtAcc.Form.Show(resolveClientUrl('VirtualSports/EditSingle?custId=' + Fanex.ExtAcc.Customer.superId + '&roleId=' + Fanex.ExtAcc.Customer.Super + '&targetCustId=' + Fanex.ExtAcc.Customer.UserId + '&targetRoleId=' + Fanex.ExtAcc.Customer.Master + '&groupName=' + Fanex.ExtAcc.Customer.GroupName().groupName), Fanex.ExtAcc.Customer.IsMultiple ? 'Edit Multiple Virtual Sports Setting' : 'Edit Virtual Sports Setting <font color="#6dcff6">' + ret.Names[0] + '</font>', 880, 700, false, null, 1);
                    }
                };
            }

            if (!this.HideKeno) {
                menuItems.keno = {
                    name: "Edit Multiple Keno",
                    className: "iconEnableEdit",
                    callback: function () {
                        Fanex.ExtAcc.Form.ShowFrame(resolveClientUrl('customersetting/KenoSetting/kenosetting/EditMaster?custid=' + Fanex.ExtAcc.Customer.UserId + "&userId=" + parent.userId + '&isMultipleEdit=true'), Fanex.ExtAcc.Customer.IsMultiple ? 'Edit Multiple Keno Setting' : 'Edit Keno Setting <font color="#6dcff6">' + ret.Names[0] + '</font>', 900, 700, false, null, 1);
                    }
                };

            }

            if (!this.HideGoldDeluxe) {
                menuItems.goldDeluxe = {
                    name: "Edit Multiple Gold Deluxe",
                    className: "iconEnableEdit",
                    callback: function () {
                        Fanex.ExtAcc.Customer.ShowEditMultipleGoldDeluxe(Fanex.ExtAcc.Customer.superId, 4, 3);
                    }
                };

            }

            if (!this.HideColossusBet) {
                menuItems.colossusBets = {
                    name: "Edit Multiple ColossusBets",
                    className: "iconEnableEdit",
                    callback: function () {
                        url = "customersetting/ColossusBetSetting/ColossusBetSetting/EditSetting?custid=" + Fanex.ExtAcc.Customer.UserId + "&srecommend=" + Fanex.ExtAcc.Customer.superId + "&isMultipleEdit=1&userId=" + parent.userId + "&roleId=3&groupName=" + parent.$('#selectedGroupName').html();
                        Fanex.ExtAcc.Form.ShowFrame(resolveClientUrl(url), Fanex.ExtAcc.Customer.IsMultiple ? 'Edit Multiple ColossusBets Setting' : 'Edit ColossusBets Setting <font color="#6dcff6">' + ret.Names[0] + '</font>', 900, 400, false, null, 1);
                    }
                };

            }

            if (this.AllowDoubleCommMaster) {
                menuItems.doubleComm = {
                    name: "Edit Multiple Double Comm",
                    className: "iconEnableEdit",
                    callback: function (key, opt) {
                        Fanex.ExtAcc.Form.Show(resolveClientUrl('Customer/ConfigMultipleDoubleComm?id=' + Fanex.ExtAcc.Customer.CurrentLevel + '&custId=' + Fanex.ExtAcc.Customer.UserId + '&groupName=' + Fanex.ExtAcc.Customer.GroupName().groupName),
                            'Edit Multiple Double Commission For Losing Tickets',
                            430, 150, false, false, 1);
                    }
                };
            }
        } else if (this.CurrentLevel == this.Agent) {
            menuItems = {
                sportbook: {
                    name: "Edit Multiple Sportsbook",
                    className: "iconEnableEdit",
                    callback: function () {
                        Fanex.ExtAcc.Customer.ShowEditMultiplePT();
                    }
                },
                racing: {
                    name: "Edit Multiple Racing",
                    className: "iconEnableEdit",
                    callback: function (key, opt) {
                        Fanex.ExtAcc.Form.ShowFrame(resolveClientUrl('customersetting/racebook/racingsetting/agent?id=' + info.custId + '&uplineid=' + info.uplineId + '&groupName=' + Fanex.ExtAcc.Customer.GroupName().groupName), Fanex.ExtAcc.Customer.IsMultiple ? 'Edit Multiple Racing Setting' : 'Edit Racing Setting ' + '<font color="#6dcff6">' + ret.Names[0] + '</font>', 1050, 775, false, null, 1);
                    }
                }
            };

            if (this.masterSyncCasino) {
                menuItems.casino = {
                    name: "Edit Multiple Casino",
                    className: "iconEnableEdit",
                    callback: function () {
                        Fanex.ExtAcc.Customer.ShowEditMultipleCasino();
                    }
                };
            }

            if (this.masterSyncBingo) {
                menuItems.bingo = {
                    name: "Edit Multiple Bingo",
                    className: "iconEnableEdit",
                    callback: function (key, opt) {
                        Fanex.ExtAcc.Form.ShowFrame(resolveClientUrl('CustomerSetting/Bingo/BingoSetting/Agent?id=' + Fanex.ExtAcc.Customer.UserId + '&uplineid=' + Fanex.ExtAcc.Customer.masterId + '&groupName=' + $('#selectedGroupName').html()), Fanex.ExtAcc.Customer.IsMultiple ? 'Edit Multiple Bingo Setting' : 'Edit Bingo Setting ' + '<font color="#6dcff6">' + ret.Names[0] + '</font>', 1050, 500, false, null, 1);
                    }
                };
            }

            if (!this.masterDisableP2P) {
                //menuItems.poker = { name: "Edit Multiple Poker", className: "iconEnableEdit" };
            }

            if (this.masterSyncLiveCasino) {
                menuItems.liveCasino = {
                    name: "Edit Multiple Live Casino",
                    className: "iconEnableEdit",
                    callback: function () {
                        Fanex.ExtAcc.Customer.ShowEditMultipleLiveCasino(Fanex.ExtAcc.Customer.masterId, Fanex.ExtAcc.Customer.masterId, 3, 2);
                    }
                };
            }

            if (!this.HideVirtualSports) {
                menuItems.virtualSport = {
                    name: "Edit Multiple Virtual Sports",
                    className: "iconEnableEdit",
                    callback: function () {
                        Fanex.ExtAcc.Form.Show(resolveClientUrl('VirtualSports/EditSingle?custId=' + Fanex.ExtAcc.Customer.masterId + '&roleId=' + Fanex.ExtAcc.Customer.Master + '&targetCustId=' + Fanex.ExtAcc.Customer.UserId + '&targetRoleId=' + Fanex.ExtAcc.Customer.Agent + '&groupName=' + Fanex.ExtAcc.Customer.GroupName().groupName), Fanex.ExtAcc.Customer.IsMultiple ? 'Edit Multiple Virtual Sports Setting' : 'Edit Virtual Sports Setting <font color="#6dcff6">' + ret.Names[0] + '</font>', 880, 700, false, null, 1);
                    }
                };
            }

            if (!this.HideKeno) {
                menuItems.keno = {
                    name: "Edit Multiple Keno",
                    className: "iconEnableEdit",
                    callback: function () {
                        Fanex.ExtAcc.Form.ShowFrame(resolveClientUrl('customersetting/KenoSetting/kenosetting/EditAgent?custid=' + Fanex.ExtAcc.Customer.UserId + "&userId=" + parent.userId + '&isMultipleEdit=true'), Fanex.ExtAcc.Customer.IsMultiple ? 'Edit Multiple Keno Setting' : 'Edit Keno Setting <font color="#6dcff6">' + ret.Names[0] + '</font>', 900, 700, false, null, 1);
                    }
                };

            }

            if (!this.HideGoldDeluxe) {
                menuItems.goldDeluxe = {
                    name: "Edit Multiple Gold Deluxe",
                    className: "iconEnableEdit",
                    callback: function () {
                        Fanex.ExtAcc.Customer.ShowEditMultipleGoldDeluxe(Fanex.ExtAcc.Customer.masterId, 3, 2);
                    }
                };

            }


            if (!this.HideColossusBet) {
                menuItems.colossusBets = {
                    name: "Edit Multiple ColossusBets",
                    className: "iconEnableEdit",
                    callback: function () {
                        url = "customersetting/ColossusBetSetting/ColossusBetSetting/EditSetting?custid=" + Fanex.ExtAcc.Customer.UserId + "&mrecommend=" + Fanex.ExtAcc.Customer.masterId + "&isMultipleEdit=1&userId=" + parent.userId + "&roleId=2&groupName=" + parent.$('#selectedGroupName').html();
                        Fanex.ExtAcc.Form.ShowFrame(resolveClientUrl(url), Fanex.ExtAcc.Customer.IsMultiple ? 'Edit Multiple ColossusBets Setting' : 'Edit ColossusBets Setting <font color="#6dcff6">' + ret.Names[0] + '</font>', 900, 400, false, null, 1);
                    }
                };

            }

            if (this.AllowDoubleCommAgent) {
                menuItems.doubleComm = {
                    name: "Edit Multiple Double Comm",
                    className: "iconEnableEdit",
                    callback: function (key, opt) {
                        Fanex.ExtAcc.Form.Show(resolveClientUrl('Customer/ConfigMultipleDoubleComm?id=' + Fanex.ExtAcc.Customer.CurrentLevel + '&custId=' + Fanex.ExtAcc.Customer.UserId + '&groupName=' + Fanex.ExtAcc.Customer.GroupName().groupName),
                            'Edit Multiple Double Commission For Losing Tickets',
                            430, 150, false, false, 1);
                    }
                };
            }
        } else {
            menuItems = {
                sportbook: {
                    name: "Edit Multiple Sportsbook",
                    className: "iconEnableEdit",
                    callback: function () {
                        Fanex.ExtAcc.Customer.ShowEditMultiplePT();
                    }
                },
                superEditSportbook: {
                    name: "Super Edit Multiple Sportsbook",
                    className: "iconEnableEdit",
                    callback: function () {
                        Fanex.ExtAcc.Customer.isSuperEditMember = true;
                        Fanex.ExtAcc.Customer.isMasterEditMember = false;
                        Fanex.ExtAcc.Customer.ShowSuperMasterEditMultiplePT();
                    }
                },
                masterEditSportbook: {
                    name: "Master Edit Multiple Sportsbook",
                    className: "iconEnableEdit",
                    callback: function () {
                        Fanex.ExtAcc.Customer.isSuperEditMember = false;
                        Fanex.ExtAcc.Customer.isMasterEditMember = true;
                        Fanex.ExtAcc.Customer.ShowSuperMasterEditMultiplePT();
                    }
                },
                separate: "------",
                racing: {
                    name: "Edit Multiple Racing",
                    className: "iconEnableEdit",
                    callback: function (key, opt) {
                        Fanex.ExtAcc.Form.ShowFrame(resolveClientUrl('customersetting/racebook/racingsetting/member?id=' + info.custId + '&uplineid=' + info.uplineId + '&groupName=' + Fanex.ExtAcc.Customer.GroupName().groupName), Fanex.ExtAcc.Customer.IsMultiple ? 'Edit Multiple Racing Setting' : 'Edit Racing Setting ' + '<font color="#6dcff6">' + ret.Names[0] + '</font>', 1050, 775, false, null, 1);
                    }
                },
                racingSuper: {
                    name: "Super Edit Multiple Racing",
                    className: "iconEnableEdit",
                    callback: function (key, opt) {
                        Fanex.ExtAcc.Form.ShowFrame(resolveClientUrl('customersetting/racebook/racingsetting/supereditmember?id=' + info.custId + '&uplineid=' + info.uplineId + '&groupName=' + Fanex.ExtAcc.Customer.GroupName().groupName), Fanex.ExtAcc.Customer.IsMultiple ? 'Super Edit Multiple Racing Setting' : 'Super Edit Racing Setting ' + '<font color="#6dcff6">' + ret.Names[0] + '</font>', 1050, 775, false, null, 1);
                    }
                },
                racingMaster: {
                    name: "Master Edit Multiple Racing",
                    className: "iconEnableEdit",
                    callback: function (key, opt) {
                        Fanex.ExtAcc.Form.ShowFrame(resolveClientUrl('customersetting/racebook/racingsetting/mastereditmember?id=' + info.custId + '&uplineid=' + info.uplineId + '&groupName=' + Fanex.ExtAcc.Customer.GroupName().groupName), Fanex.ExtAcc.Customer.IsMultiple ? 'Master Edit Multiple Racing Setting' : 'Master Edit Racing Setting ' + '<font color="#6dcff6">' + ret.Names[0] + '</font>', 1050, 775, false, null, 1);
                    }
                },
                separate1: "------"
            };

            if (this.agentSyncCasino) {
                menuItems.casino = {
                    name: "Edit Multiple Casino",
                    className: "iconEnableEdit",
                    callback: function () {
                        Fanex.ExtAcc.Customer.ShowEditMultipleCasino();
                    }
                };
                menuItems.scasino = {
                    name: "Super Edit Multiple Casino",
                    className: "iconEnableEdit",
                    callback: function () {
                        Fanex.ExtAcc.Customer.isSuperEditMember = true;
                        Fanex.ExtAcc.Customer.isMasterEditMember = false;
                        Fanex.ExtAcc.Customer.ShowEditMultipleCasino();
                    }
                };
                menuItems.mcasino = {
                    name: "Master Edit Multiple Casino",
                    className: "iconEnableEdit",
                    callback: function () {
                        Fanex.ExtAcc.Customer.isSuperEditMember = false;
                        Fanex.ExtAcc.Customer.isMasterEditMember = true;
                        Fanex.ExtAcc.Customer.ShowEditMultipleCasino();
                    }
                };
                menuItems.separate2 = "------";
            }

            if (this.agentSyncBingo) {
                menuItems.bingo = {
                    name: "Edit Multiple Bingo",
                    className: "iconEnableEdit",
                    callback: function (key, opt) {
                        Fanex.ExtAcc.Form.ShowFrame(resolveClientUrl('CustomerSetting/Bingo/BingoSetting/Member?id=' + Fanex.ExtAcc.Customer.UserId + '&uplineid=' + Fanex.ExtAcc.Customer.agentId + '&groupName=' + $('#selectedGroupName').html()), Fanex.ExtAcc.Customer.IsMultiple ? 'Edit Multiple Bingo Setting' : 'Edit Bingo Setting ' + '<font color="#6dcff6">' + ret.Names[0] + '</font>', 1050, 500, false, null, 1);
                    }
                };
                menuItems.sbingo = {
                    name: "Super Edit Multiple Bingo",
                    className: "iconEnableEdit",
                    callback: function (key, opt) {
                        Fanex.ExtAcc.Form.ShowFrame(resolveClientUrl('CustomerSetting/Bingo/BingoSetting/SuperEditMember?id=' + Fanex.ExtAcc.Customer.UserId + '&uplineid=' + Fanex.ExtAcc.Customer.superId + '&groupName=' + $('#selectedGroupName').html()), Fanex.ExtAcc.Customer.IsMultiple ? 'Super Edit Multiple Bingo Setting' : 'Super Edit Bingo Setting ' + '<font color="#6dcff6">' + ret.Names[0] + '</font>', 1050, 500, false, null, 1);
                    }
                };
                menuItems.mbingo = {
                    name: "Master Edit Multiple Bingo",
                    className: "iconEnableEdit",
                    callback: function (key, opt) {
                        Fanex.ExtAcc.Form.ShowFrame(resolveClientUrl('CustomerSetting/Bingo/BingoSetting/MasterEditMember?id=' + Fanex.ExtAcc.Customer.UserId + '&uplineid=' + Fanex.ExtAcc.Customer.masterId + '&groupName=' + $('#selectedGroupName').html()), Fanex.ExtAcc.Customer.IsMultiple ? 'Master Edit Multiple Bingo Setting' : 'Master Edit Bingo Setting ' + '<font color="#6dcff6">' + ret.Names[0] + '</font>', 1050, 500, false, null, 1);
                    }
                };
                menuItems.separate3 = "------";
            }

            if (!this.agentDisableP2P) {
                //menuItems.poker = { name: "Edit Multiple Poker", className: "iconEnableEdit" };
                //menuItems.separate4 = "------";
            }

            if (this.agentSyncLiveCasino) {
                menuItems.liveCasino = {
                    name: "Edit Multiple Live Casino",
                    className: "iconEnableEdit",
                    callback: function () {
                        Fanex.ExtAcc.Customer.ShowEditMultipleLiveCasino(Fanex.ExtAcc.Customer.agentId, Fanex.ExtAcc.Customer.agentId, 2, 1);
                    }
                };
                menuItems.liveCasinoSuper = {
                    name: "Super Edit Multiple Live Casino",
                    className: "iconEnableEdit",
                    callback: function () {
                        Fanex.ExtAcc.Customer.isSuperEditMember = true;
                        Fanex.ExtAcc.Customer.isMasterEditMember = false;

                        Fanex.ExtAcc.Customer.ShowEditMultipleLiveCasino(Fanex.ExtAcc.Customer.superId, Fanex.ExtAcc.Customer.agentId, 4, 1);
                    }
                };
                menuItems.liveCasinoMaster = {
                    name: "Master Edit Multiple Live Casino",
                    className: "iconEnableEdit",
                    callback: function () {
                        Fanex.ExtAcc.Customer.isSuperEditMember = false;
                        Fanex.ExtAcc.Customer.isMasterEditMember = true;

                        Fanex.ExtAcc.Customer.ShowEditMultipleLiveCasino(Fanex.ExtAcc.Customer.masterId, Fanex.ExtAcc.Customer.agentId, 3, 1);
                    }
                };
                menuItems.separate5 = "------";
            }

            if (!this.HideVirtualSports) {
                menuItems.virtualSport = {
                    name: "Edit Multiple Virtual Sports",
                    className: "iconEnableEdit",
                    callback: function () {
                        Fanex.ExtAcc.Form.Show(resolveClientUrl('VirtualSports/EditSingle?custId=' + Fanex.ExtAcc.Customer.agentId + '&roleId=' + Fanex.ExtAcc.Customer.Agent + '&targetCustId=' + Fanex.ExtAcc.Customer.UserId + '&targetRoleId=' + Fanex.ExtAcc.Customer.Member + '&groupName=' + Fanex.ExtAcc.Customer.GroupName().groupName), Fanex.ExtAcc.Customer.IsMultiple ? 'Edit Multiple Virtual Sports Setting' : 'Edit Virtual Sports Setting <font color="#6dcff6">' + ret.Names[0] + '</font>', 880, 700, false, null, 1);
                    }
                };
                menuItems.svirtualSport = {
                    name: "Super Edit Multiple Virtual Sports",
                    className: "iconEnableEdit",
                    callback: function () {
                        Fanex.ExtAcc.Customer.isSuperEditMember = true;
                        Fanex.ExtAcc.Customer.isMasterEditMember = false;
                        Fanex.ExtAcc.Form.Show(resolveClientUrl('VirtualSports/EditSingle?custId=' + Fanex.ExtAcc.Customer.superId + '&roleId=' + Fanex.ExtAcc.Customer.Super + '&targetCustId=' + Fanex.ExtAcc.Customer.UserId + '&targetRoleId=' + Fanex.ExtAcc.Customer.Member + '&groupName=' + Fanex.ExtAcc.Customer.GroupName().groupName), Fanex.ExtAcc.Customer.IsMultiple ? 'Super Edit Multiple Virtual Sports Setting' : 'Super Edit Virtual Sports Setting <font color="#6dcff6">' + ret.Names[0] + '</font>', 880, 700, false, null, 1);
                    }
                };
                menuItems.mvirtualSport = {
                    name: "Master Edit Multiple Virtual Sports",
                    className: "iconEnableEdit",
                    callback: function () {
                        Fanex.ExtAcc.Customer.isSuperEditMember = false;
                        Fanex.ExtAcc.Customer.isMasterEditMember = true;
                        Fanex.ExtAcc.Form.Show(resolveClientUrl('VirtualSports/EditSingle?custId=' + Fanex.ExtAcc.Customer.masterId + '&roleId=' + Fanex.ExtAcc.Customer.Master + '&targetCustId=' + Fanex.ExtAcc.Customer.UserId + '&targetRoleId=' + Fanex.ExtAcc.Customer.Member + '&groupName=' + Fanex.ExtAcc.Customer.GroupName().groupName), Fanex.ExtAcc.Customer.IsMultiple ? 'Master Edit Multiple Virtual Sports Setting' : 'Master Edit Virtual Sports Setting <font color="#6dcff6">' + ret.Names[0] + '</font>', 880, 700, false, null, 1);
                    }
                };
                menuItems.separate6 = "------";
            }

            if (!this.HideKeno) {
                menuItems.keno = {
                    name: "Edit Multiple Keno",
                    className: "iconEnableEdit",
                    callback: function () {
                        Fanex.ExtAcc.Form.ShowFrame(resolveClientUrl('customersetting/KenoSetting/kenosetting/AgentEditMember?custid=' + Fanex.ExtAcc.Customer.UserId + "&userId=" + parent.userId + '&isMultipleEdit=true'), Fanex.ExtAcc.Customer.IsMultiple ? 'Edit Multiple Keno Setting' : 'Edit Keno Setting <font color="#6dcff6">' + ret.Names[0] + '</font>', 900, 700, false, null, 1);
                    }
                };
                menuItems.skeno = {
                    name: "Super Edit Multiple Keno",
                    className: "iconEnableEdit",
                    callback: function () {
                        Fanex.ExtAcc.Form.ShowFrame(resolveClientUrl('customersetting/KenoSetting/kenosetting/SuperEditMember?custid=' + Fanex.ExtAcc.Customer.UserId + "&userId=" + parent.userId + '&isMultipleEdit=true'), Fanex.ExtAcc.Customer.IsMultiple ? 'Edit Multiple Keno Setting' : 'Edit Keno Setting <font color="#6dcff6">' + ret.Names[0] + '</font>', 900, 700, false, null, 1);
                    }
                };
                menuItems.mkeno = {
                    name: "Master Edit Multiple Keno",
                    className: "iconEnableEdit",
                    callback: function () {
                        Fanex.ExtAcc.Form.ShowFrame(resolveClientUrl('customersetting/KenoSetting/kenosetting/MasterEditMember?custid=' + Fanex.ExtAcc.Customer.UserId + "&userId=" + parent.userId + '&isMultipleEdit=true'), Fanex.ExtAcc.Customer.IsMultiple ? 'Edit Multiple Keno Setting' : 'Edit Keno Setting <font color="#6dcff6">' + ret.Names[0] + '</font>', 900, 700, false, null, 1);
                    }
                };
                menuItems.separate7 = "------";
            }

            if (!this.HideGoldDeluxe) {
                menuItems.goldDeluxe = {
                    name: "Edit Multiple Gold Deluxe",
                    className: "iconEnableEdit",
                    callback: function () {
                        Fanex.ExtAcc.Customer.ShowEditMultipleGoldDeluxe(Fanex.ExtAcc.Customer.superId, 2, 1);
                    }
                };
                menuItems.goldDeluxeSuper = {
                    name: "Super Edit Multiple Gold Deluxe",
                    className: "iconEnableEdit",
                    callback: function () {
                        Fanex.ExtAcc.Customer.isSuperEditMember = true;
                        Fanex.ExtAcc.Customer.isMasterEditMember = false;

                        Fanex.ExtAcc.Customer.ShowEditMultipleGoldDeluxe(Fanex.ExtAcc.Customer.superId, 4, 1);
                    }
                };
                menuItems.goldDeluxeMaster = {
                    name: "Master Edit Multiple Gold Deluxe",
                    className: "iconEnableEdit",
                    callback: function () {
                        Fanex.ExtAcc.Customer.isSuperEditMember = false;
                        Fanex.ExtAcc.Customer.isMasterEditMember = true;

                        Fanex.ExtAcc.Customer.ShowEditMultipleGoldDeluxe(Fanex.ExtAcc.Customer.superId, 3, 1);
                    }
                };
                menuItems.separate8 = "------";
            }

            if (!this.HideColossusBet) {
                menuItems.colossusBets = {
                    name: "Edit Multiple ColossusBets",
                    className: "iconEnableEdit",
                    callback: function () {
                        url = "customersetting/ColossusBetSetting/ColossusBetSetting/EditSetting?custid=" + Fanex.ExtAcc.Customer.UserId + "&recommend=" + Fanex.ExtAcc.Customer.agentId + "&isMultipleEdit=1&userId=" + parent.userId + "&roleId=1&groupName=" + parent.$('#selectedGroupName').html();
                        Fanex.ExtAcc.Form.ShowFrame(resolveClientUrl(url), Fanex.ExtAcc.Customer.IsMultiple ? 'Edit Multiple ColossusBets Setting' : 'Edit ColossusBets Setting <font color="#6dcff6">' + ret.Names[0] + '</font>', 900, 400, false, null, 1);
                    }
                };

            }

            if (this.AllowDoubleComm) {
                menuItems.doubleComm = {
                    name: "Edit Multiple Double Comm",
                    className: "iconEnableEdit",
                    callback: function (key, opt) {
                        Fanex.ExtAcc.Form.Show(resolveClientUrl('Customer/ConfigMultipleDoubleComm?id=' + Fanex.ExtAcc.Customer.CurrentLevel + '&custId=' + Fanex.ExtAcc.Customer.UserId + '&groupName=' + Fanex.ExtAcc.Customer.GroupName().groupName),
                            'Edit Multiple Double Commission For Losing Tickets',
                            430, 150, false, false, 1);
                    }
                };
            }
        }

        if ($.contextMenu) $.contextMenu('destroy');

        $.contextMenu({
            selector: '.editMult',
            items: menuItems,
            trigger: "left",
            ignoreRightClick: true,
            autoHide: true
        });
    },

    UpdateMultipleDoubleComm: function (allow) {
        var ret = this.IdsAndNames();

        this.ConfigMultipleDoubleComm(ret.Ids, allow, 0);
    },

    ShowEditMultiplePT: function () {
        var ret = this.IdsAndNames();

        if (!this.IsMultiple)
            Fanex.ExtAcc.Form.Show(resolveClientUrl('SportsbookSetting/EditPT?custId=' + Fanex.ExtAcc.Customer.UserId + '&groupName=' + Fanex.ExtAcc.Customer.GroupName().groupName), 'Edit PT Setting ' + '<font color="#6dcff6">' + ret.Names[0] + '</font>', 888, 700, false, false, 1);
        else
            Fanex.ExtAcc.Form.Show(resolveClientUrl('SportsbookSetting/EditMultiplePT?custId=' + Fanex.ExtAcc.Customer.UserId + '&groupName=' + Fanex.ExtAcc.Customer.GroupName().groupName), 'Edit Multiple Sportsbook Setting', 894, 700, false, false, 1);
    },

    AppendInputToForm: function (id, name, value, targetForm) {
        $('<input>').attr({
            type: 'hidden',
            id: id,
            name: name,
            value: value
        }).appendTo(targetForm);
    },

    ShowEditMultipleLiveCasino: function (uplineId, directUplineId, uplineRoleId, targetRoleId) {
        var ret = this.IdsAndNames();

        var actionName = '';
        var levelText = '';

        if (this.CurrentLevel == this.Master) {
            actionName = 'EditMaster';
        } else if (this.CurrentLevel == this.Agent) {
            actionName = 'EditAgent';
        } else {
            if (this.isSuperEditMember) {
                actionName = 'SuperEditMember';
                levelText = 'Super ';
            } else if (this.isMasterEditMember) {
                actionName = 'MasterEditMember';
                levelText = 'Master ';
            } else {
                actionName = 'EditMember';
            }
        }

        if (this.IsMultiple) {
            document.getElementById('parentId').value = directUplineId;
            document.getElementById('uplineId').value = uplineId;
            document.getElementById('customerIds').value = Fanex.ExtAcc.Customer.CustIds;
            document.getElementById('syncStatuses').value = Fanex.ExtAcc.Customer.LiveCasinoStatuses;
            document.getElementById('usernames').value = Fanex.ExtAcc.Customer.Usernames.replaceAll(' ', '');
            document.getElementById('uplineLevel').value = uplineRoleId;
            document.getElementById('customerLevel').value = targetRoleId;
            document.getElementById('actorId').value = parent.userId;
            document.getElementById('actorIdUsername').value = parent.username;
            document.getElementById('subAccId').value = 0;
            document.getElementById('language').value = 'E';
            document.getElementById('groupName').value = Fanex.ExtAcc.Customer.GroupName().groupName;

            Fanex.ExtAcc.Form.ShowFrame(resolveClientUrl('LiveCasinoSetting/Setting/EditMultipleSetting'), levelText + 'Edit Multiple Live Casino Setting', 900, 700, false, false, 1);
            $("#targetForm").attr("action", resolveClientUrl('LiveCasinoSetting/Setting/EditMultipleSetting')).submit();
        }
        else {
            var url = '';
            switch (uplineRoleId) {
                case 4: url = "LiveCasinoSetting/Setting/EditSetting?uplineId=" + Fanex.ExtAcc.Customer.superId + "&parentId=" + Fanex.ExtAcc.Customer.superId + "&uplineLevel=4&customerId=" + Fanex.ExtAcc.Customer.UserId + "&customerLevel=" + targetRoleId;
                    break;
                case 3: url = "LiveCasinoSetting/Setting/EditSetting?uplineId=" + Fanex.ExtAcc.Customer.masterId + "&parentId=" + Fanex.ExtAcc.Customer.masterId + "&uplineLevel=3&customerId=" + Fanex.ExtAcc.Customer.UserId + "&customerLevel=" + targetRoleId;
                    break;
                default: url = "LiveCasinoSetting/Setting/EditSetting?uplineId=" + Fanex.ExtAcc.Customer.agentId + "&parentId=" + Fanex.ExtAcc.Customer.agentId + "&uplineLevel=2&customerId=" + Fanex.ExtAcc.Customer.UserId + "&customerLevel=" + targetRoleId;
                    break;
            }

            Fanex.ExtAcc.Form.ShowFrame(resolveClientUrl(url + "&actorId=" + parent.userId + "&actorUsername=" + parent.username + "&groupName=" + Fanex.ExtAcc.Customer.GroupName().groupName), levelText + 'Edit Live Casino Setting ' + '<font color="#6dcff6">' + ret.Names[0] + '</font>', 900, 700, false, false, 1);
        }
    },

    ShowEditMultipleGoldDeluxe: function (uplineId, uplineRoleId, targetRoleId) {
        var ret = this.IdsAndNames();

        var actionName = '';
        var levelText = '';

        if (this.CurrentLevel == this.Master) {
            actionName = 'EditMaster';
        } else if (this.CurrentLevel == this.Agent) {
            actionName = 'EditAgent';
        } else {
            if (this.isSuperEditMember) {
                actionName = 'SuperEditMember';
                levelText = 'Super ';
            } else if (this.isMasterEditMember) {
                actionName = 'MasterEditMember';
                levelText = 'Master ';
            } else {
                actionName = 'EditMember';
            }
        }

        if (this.IsMultiple) {
            var goldDeluxeUsernames = '';
            var goldDeluxeCustIds = '';
            var userNamesArr = this.Names();
            var custIdsArr = this.Ids()

            var customerlength = userNamesArr.length;
            for (var i = 0; i < customerlength; i++) {
                var linkChar = i == customerlength - 1 ? "" : ", ";
                goldDeluxeUsernames += userNamesArr[i] + linkChar;
                goldDeluxeCustIds += custIdsArr[i] + linkChar;
            }

            document.getElementById('uplineId').value = uplineId;
            document.getElementById('customerIds').value = goldDeluxeCustIds;
            document.getElementById('usernames').value = goldDeluxeUsernames;
            document.getElementById('uplineLevel').value = uplineRoleId;
            document.getElementById('customerLevel').value = targetRoleId;
            document.getElementById('actorId').value = parent.userId;
            document.getElementById('subAccId').value = 0;
            document.getElementById('language').value = 'E';
            document.getElementById('groupName').value = Fanex.ExtAcc.Customer.GroupName().groupName;

            Fanex.ExtAcc.Form.ShowFrame(resolveClientUrl('GoldDeluxeSetting/GoldDeluxe/EditSettingMultiple'), levelText + 'Edit Multiple Gold Deluxe Setting', 900, 400, false, false, 1);
            $("#targetForm").attr("action", resolveClientUrl('GoldDeluxeSetting/GoldDeluxe/EditSettingMultiple')).submit();
        }
        else {
            var url = '';
            switch (uplineRoleId) {
                case 4: url = "GoldDeluxeSetting/GoldDeluxe/EditSetting?upLineLevel=" + uplineRoleId + "&formType=1&level=" + targetRoleId + "&uplineId=" + Fanex.ExtAcc.Customer.superId;
                    break;
                case 3: url = "GoldDeluxeSetting/GoldDeluxe/EditSetting?upLineLevel=" + uplineRoleId + "&formType=1&level=" + targetRoleId + "&uplineId=" + Fanex.ExtAcc.Customer.masterId;
                    break;
                default: url = "GoldDeluxeSetting/GoldDeluxe/EditSetting?upLineLevel=" + uplineRoleId + "&formType=1&level=" + targetRoleId + "&uplineId=" + Fanex.ExtAcc.Customer.agentId;
                    break;
            }

            Fanex.ExtAcc.Form.ShowFrame(resolveClientUrl(url + "&actorId=" + parent.userId + "&custId=" + Fanex.ExtAcc.Customer.UserId + "&groupName=" + Fanex.ExtAcc.Customer.GroupName().groupName), levelText + 'Edit Gold Deluxe Setting ' + '<font color="#6dcff6">' + ret.Names[0] + '</font>', 900, 400, false, false, 1);
        }
    },

    ShowSuperMasterEditMultiplePT: function () {
        var ret = this.IdsAndNames();
        var levelText = '';

        if (this.isSuperEditMember) {
            levelText = 'Super ';
        } else if (this.isMasterEditMember) {
            levelText = 'Master ';
        }

        if (!this.IsMultiple) {
            Fanex.ExtAcc.Form.Show(resolveClientUrl('SportsbookSetting/EditPT?custId=' + Fanex.ExtAcc.Customer.UserId + '&groupName=' + Fanex.ExtAcc.Customer.GroupName().groupName), levelText + 'Edit PT Setting ' + '<font color="#6dcff6">' + ret.Names[0] + '</font>', 888, 700, false, false, 1);
        } else {
            Fanex.ExtAcc.Form.Show(resolveClientUrl('SportsbookSetting/SuperMasterEditMultiplePTMember?custId=' + Fanex.ExtAcc.Customer.UserId + '&groupName=' + Fanex.ExtAcc.Customer.GroupName().groupName), levelText + 'Edit Multiple Sportsbook Setting', 894, 700, false, false, 1);
        }
    },

    ShowEditMultipleCasino: function () {
        var ret = this.IdsAndNames();
        var data = this.CustomerInfo();
        var levelText = '';
        var actionName = 'CustomerSetting/Casino/';

        switch (this.CurrentLevel) {
            case this.Master:
                actionName += 'Master/EditMaster/' + this.Ids()[0] + '/' + this.superId + '/' + Fanex.ExtAcc.Customer.GroupName().groupName;
                break;
            case this.Agent:
                actionName += 'Agent/EditAgent/' + this.Ids()[0] + '/' + this.masterId + '/' + Fanex.ExtAcc.Customer.GroupName().groupName;
                break;
            case this.Member:
                if (this.isSuperEditMember)
                    actionName += 'Member/EditMemberBySuper/' + this.Ids()[0] + '/' + this.superId + '/' + Fanex.ExtAcc.Customer.GroupName().groupName;
                else if (this.isMasterEditMember)
                    actionName += 'Member/EditMemberByMaster/' + this.Ids()[0] + '/' + this.masterId + '/' + Fanex.ExtAcc.Customer.GroupName().groupName;
                else
                    actionName += 'Member/EditMember/' + this.Ids()[0] + '/' + this.agentId + '/' + Fanex.ExtAcc.Customer.GroupName().groupName;
                break;
            default:
                break;
        }

        if (this.isSuperEditMember) {
            levelText = 'Super ';
        } else if (this.isMasterEditMember) {
            levelText = 'Master ';
        }

        if (!this.IsMultiple) {
            Fanex.ExtAcc.Form.ShowFrame(resolveClientUrl(actionName), levelText + 'Edit Casino Setting ' + '<font color="#6dcff6">' + ret.Names[0] + '</font>', 1064, 700, false, false, 1, null);
        } else {
            window.IsMultiple = true;
            window.Ids = this.Ids();
            window.Names = this.Names();
            Fanex.ExtAcc.Form.ShowFrame(resolveClientUrl(actionName), levelText + 'Edit Multiple Casino Setting', 1064, 700, false, false, 1, null);
        }
    },

    ToogleUsernames: function (length) {
        if ($('#arrowUpDown').hasClass('collapse')) {
            $('#arrowUpDown').attr('class', 'expand');
            $('#names').html(this.Usernames.substring(0, this.Usernames.length - 2));
            $('#names').attr('title', '');
        } else {
            $('#arrowUpDown').attr('class', 'collapse');
            $('#names').html(this.Usernames.substring(0, (length ? length : 98)) + '...');
            $('#names').attr('title', this.Usernames.substring(0, this.Usernames.length - 2));
        }
    },

    IdsAndNames: function () {
        var custids = [];
        var divs = $('div');

        this.Usernames = '';
        this.CustIds = '';
        this.LiveCasinoStatuses = '';
        this.RnGCasinoStatuses = [];

        for (var i = 0; i < divs.length; i++) {
            if (divs[i].className == 'menuActive' && !custids.contains(divs[i].id)) {
                custids.push(divs[i].id);

                this.CustIds += divs[i].id + ',';
            }
        }

        this.UserId = parseInt(custids[0]);

        if (custids.length == 1) {
            for (var i = 0; i < this.DataSource.length; i++) {
                if (this.DataSource[i].custid == parseInt(custids[0])) {
                    this.Usernames = this.DataSource[i].username + ', ';

                    break;
                }
            }

            this.IsMultiple = false;
        } else {
            for (var i = 0; i < this.DataSource.length; i++) {
                var row = this.DataSource[i];

                for (var j = 0; j < custids.length; j++) {
                    if (custids[j] == row.custid) {
                        this.Usernames += row.username + ', ';
                        this.LiveCasinoStatuses += (row.SyncLiveCasino ? '1' : '0') + ',';
                        this.RnGCasinoStatuses.push(row.synccasino ? '1' : '0');
                        continue;
                    }
                }
            }

            this.IsMultiple = true;
        }

        return {
            Ids: this.Ids(),
            Names: this.Names()
        };
    },

    Ids: function () {
        if (this.CustIds.length > 0 && this.CustIds.indexOf(',') != -1)
            return this.CustIds.substring(0, this.CustIds.length - 1).split(',');

        return [];
    },

    Names: function () {
        if (this.Usernames.length > 0 && this.Usernames.indexOf(',') != -1)
            return this.Usernames.substring(0, this.Usernames.length - 2).split(', ');

        return [];
    },


    ResetFlags: function () {
        this.IsMultiple = false;
        this.isSuperEditMember = false;
        this.isMasterEditMember = false;
        this.isSuperEditAgent = false;
        this.IsFullInfo = false;
    },

    CustomerInfo: function () {
        var uplineId = 0;
        var uplineRoleId = 0;
        var info = this.IdsAndNames();

        switch (this.CurrentLevel) {
            case this.Master:
                uplineId = this.superId;
                uplineRoleId = this.Super;
                break;
            case this.Agent:
                uplineId = this.isSuperEditAgent ? this.superId : this.masterId;
                uplineRoleId = this.isSuperEditAgent ? this.Super : this.Master;
                break;
            case this.Member:
                uplineId = this.isSuperEditMember ? this.superId : this.isMasterEditMember ? this.masterId : this.agentId;
                uplineRoleId = this.isSuperEditMember ? this.Super : this.isMasterEditMember ? this.Master : this.Agent;
                break;
            default:
                uplineId = 0;
                uplineRoleId = 5;
                break;
        }

        return {
            custId: this.IsMultiple ? info.Ids[0] : this.UserId,
            roleId: this.CurrentLevel,
            uplineId: uplineId,
            uplineRoleId: uplineRoleId
        };
    },

    ProcessDisplayMultiple: function (length) {
        $('#arrowUpDown').bind('click', function () {
            Fanex.ExtAcc.Customer.ToogleUsernames(length);
        });

        $('#names').css('display', 'block');

        if (this.Usernames.length < (length ? length : 98)) {
            $('#names').css('cursor', 'default');
            $('#names').html(this.Usernames.substring(0, this.Usernames.length - 2));
            $('#arrowUpDown').css('display', 'none');
        } else {
            $('#names').css('cursor', 'pointer');
            $('#names').bind('click', function () {
                Fanex.ExtAcc.Customer.ToogleUsernames(length);
            });
            $('#names').html(this.Usernames.substring(0, (length ? length : 98)) + '...');
            $('#names').attr('title', this.Usernames.substring(0, this.Usernames.length - 2));
            $('#arrowUpDown').css('display', 'block');
        }
    },

    ProcessConfirmClosedStatus: function (id) {
        var currentLevel = this.CurrentLevel;
        if (currentLevel != 4) {
            var timeout = setTimeout(function () {
                clearTimeout(timeout);

                Fanex.ExtAcc.Customer.Closed = document.getElementById(id ? id : 'closed').checked;

                $(id ? '#' + id : '#closed').bind('click', function () {
                    var checkbox = this;

                    if (this.checked && !Fanex.ExtAcc.Customer.Closed) {
                        Fanex.ExtAcc.Form.ConfirmBox('Edit Status',
                            currentLevel == 1 ? Fanex.ExtAcc.Customer.MessageClosedMember : Fanex.ExtAcc.Customer.MessageClosed,
                            400,
                            150,
                            false,
                            function () {
                                Fanex.ExtAcc.Form.Close(false, false, 'confirmStatus');
                            },
                            function () {
                                checkbox.checked = false;
                                Fanex.ExtAcc.Form.Close(false, false, 'confirmStatus');
                            },
                            'confirmStatus');
                    } else if (!this.checked && Fanex.ExtAcc.Customer.Closed) {
                        Fanex.ExtAcc.Form.ConfirmBox('Edit Status',
                            Fanex.ExtAcc.Customer.MessageUnClosed,
                            400,
                            150,
                            false,
                            function () {
                                Fanex.ExtAcc.Form.Close(false, false, 'confirmStatus');
                            },
                            function () {
                                checkbox.checked = true;
                                Fanex.ExtAcc.Form.Close(false, false, 'confirmStatus');
                            },
                            'confirmStatus');
                    }
                });
            }, 1000);
        }
    },

    ProcessConfirmSuspendedStatus: function (id) {
        var currentLevel = this.CurrentLevel;
        if (currentLevel != 4) {
            var timeout = setTimeout(function () {
                clearTimeout(timeout);
                Fanex.ExtAcc.Customer.Suspended = document.getElementById(id ? id : 'suspended').checked;

                $(id ? '#' + id : '#suspended').bind('click', function () {
                    var checkbox = this;

                    if (this.checked && !Fanex.ExtAcc.Customer.Suspended) {
                        Fanex.ExtAcc.Form.ConfirmBox('Edit Status',
                            currentLevel == 1 ? Fanex.ExtAcc.Customer.MessageSuspendedMember : Fanex.ExtAcc.Customer.MessageSuspended,
                            400,
                            180,
                            false,
                            function () {
                                Fanex.ExtAcc.Form.Close(false, false, 'confirmStatus');
                            },
                            function () {
                                checkbox.checked = false;
                                Fanex.ExtAcc.Form.Close(false, false, 'confirmStatus');
                            },
                            'confirmStatus');
                    } else if (!this.checked && Fanex.ExtAcc.Customer.Suspended) {
                        Fanex.ExtAcc.Form.ConfirmBox('Edit Status',
                            currentLevel == 1 ? Fanex.ExtAcc.Customer.MessageUnSuspendedMember : Fanex.ExtAcc.Customer.MessageUnSuspended,
                            400,
                            180,
                            false,
                            function () {
                                Fanex.ExtAcc.Form.Close(false, false, 'confirmStatus');
                            },
                            function () {
                                checkbox.checked = true;
                                Fanex.ExtAcc.Form.Close(false, false, 'confirmStatus');
                            },
                            'confirmStatus');
                    }
                });
            }, 1000);
        }
    },

    validatePassword: function (str, isMember) {
        return isMember ? this.validatePwdMember(str) : this.validatePwdAgent(str);
    },

    validatePwdAgent: function (str) {
        var regex1 = /^(?=.*\d).{8,100}$/;
        var regex2 = /^(?=.*[a-z]).{8,100}$/;
        var regex3 = /^(?=.*[A-Z]).{8,100}$/;
        var regex4 = /^(?=.*[\[\]\^\$\.\|\?\*\+\(\)\\~`\!@#%&\-_+={}'""<>:;,]).{8,100}$/;
        var whiteSpace = /^\S*$/;
        var case1 = str.search(regex1) != -1 ? 1 : 0;
        var case2 = str.search(regex2) != -1 ? 1 : 0;
        var case3 = str.search(regex3) != -1 ? 1 : 0;
        var case4 = str.search(regex4) != -1 ? 1 : 0;
        var case5 = str.search(whiteSpace) != -1;
        return (case1 + case2 + case3 + case4 >= 3) && case5;
    },

    validatePwdMember: function (str) {
        var regex = /^(?=.*\d)(?=.*[a-z])(?=.*[A-Z]).{6,10}$/;
        var whiteSpace = /^\S*$/;
        var case5 = str.search(whiteSpace) != -1;
        return (str.search(regex) != -1) && case5;
    }
}

/*
$(document).ready(function () {
Fanex.ExtAcc.Customer.Init();
})*/