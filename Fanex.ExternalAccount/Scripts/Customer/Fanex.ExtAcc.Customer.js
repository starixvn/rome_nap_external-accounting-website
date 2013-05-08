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

    CurrentLevel: 4,

    AllowDoubleCommMaster: false,
    AllowDoubleCommAgent: false,
    AllowDoubleComm: false,

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

    URL: '',
    MultipleDblCommError: '',
    Usernames: '',
    CustIds: '',

    Init: function () {
        //Prevent loading group name
        var timeout = setInterval(function () {
            if ($.trim($('#selectedGroupName').html()) != '') {
                Fanex.ExtAcc.Customer.Search(true);
                clearInterval(timeout);
            }
        }, 100);

        $('#exporttoexcel').click(function () {
            var data = Fanex.ExtAcc.Customer.GetParams();

            var params = "groupName=" + data.groupName
                    + "&enabled=" + data.enabled + "&closed=" + data.closed + "&suspended=" + data.suspended
                    + "&sRecommend=" + Fanex.ExtAcc.Customer.superId + "&mRecommend=" + Fanex.ExtAcc.Customer.masterId + "&recommend=" + Fanex.ExtAcc.Customer.agentId + "&custId=" + Fanex.ExtAcc.Customer.custId
                    + "&currentLevel=" + Fanex.ExtAcc.Customer.CurrentLevel + "&username=" + data.username
            document.location.href = resolveClientUrl("Customer/GetExportToExcel?") + params;
        });

        $('#txtUserName').keypress(function (e) {
            if (typeof e == 'undefined') e = window.event;
            var key = (typeof e.keyCode == 'undefined') ? e.which : e.keyCode;
            if (key == 13) {
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

    GetParams: function () {
        var enabled = false;
        var closed = false;
        var suspended = false;

        if ($('#customerStatus').val() == '-1') {
            enabled = null;
            closed = null;
            suspended = null;
        }
        else if ($('#customerStatus').val() == '1') {
            enabled = true;
            closed = false;
            suspended = false;
        }
        else if ($('#customerStatus').val() == '2') {
            enabled = null;
            closed = true;
            suspended = null;
        }
        else if ($('#customerStatus').val() == '3') {
            enabled = null;
            closed = null;
            suspended = true;
        }
        else {
            enabled = false;
            closed = null;
            suspended = null;
        }

        var data = new Object();

        data.srecommend = Fanex.ExtAcc.Customer.superId;
        data.mrecommend = Fanex.ExtAcc.Customer.masterId;
        data.recommend = Fanex.ExtAcc.Customer.agentId;
        data.custId = null;
        data.groupName = $('#selectedGroupName').html() == '' ? 'ACCDDD' : $('#selectedGroupName').html();
        data.enabled = enabled;
        data.closed = closed;
        data.suspended = suspended;
        data.username = $('#txtUserName').val();

        return data;
    },

    Search: function (isFirstLoad) {
        if (isFirstLoad) {
            Fanex.ExtAcc.Customer.GetSuperList();
        }
        else {
            Fanex.ExtAcc.Customer.SearchList();
        }
    },

    ShowEditSuper: function (custId, title) {
        Fanex.ExtAcc.Customer.UserId = custId;
        Fanex.ExtAcc.Customer.TitleEditInfo = title;

        this.ResetFlags();

        $.contextMenu({
            selector: '.editInfo',
            items: {
                editInfo: { name: "Edit Info", height: 25, className: "iconEnableEdit editInfoBg", callback: function (key, opt) { Fanex.ExtAcc.Form.Show(resolveClientUrl('Customer/EditInfo/?custId=' + Fanex.ExtAcc.Customer.UserId), titles[0] + ': <font color="#6dcff6">' + Fanex.ExtAcc.Customer.TitleEditInfo + '</font>', 406, 310, false); } },
                changePassword: { name: "Reset Password", className: "iconEnableEdit editInfoBg", callback: function (key, opt) { Fanex.ExtAcc.Form.Show(resolveClientUrl('Customer/ResetPassword/?custId=' + Fanex.ExtAcc.Customer.UserId), 'Reset password', 420, 230, false); } }
            },
            trigger: "left",
            ignoreRightClick: true,
            autoHide: true
        });
    },

    ShowEditMaster: function (custId, title, transferCondition, syncCasino, disableCasino, syncBingo, disableBingo, disableP2P, syncLiveCasino, disableLiveCasino, disableVirtualSport, superSyncCasino, superSyncBingo, superSyncLiveCasino) {
        Fanex.ExtAcc.Customer.UserId = custId;
        Fanex.ExtAcc.Customer.TitleEditInfo = title;

        this.ResetFlags();

        itemsMaster = {
            sportbook: { name: "Sportsbook", className: "iconEnableEdit", callback: function () { Fanex.ExtAcc.Customer.IsMultiple = false; Fanex.ExtAcc.Customer.IsFullInfo = true; Fanex.ExtAcc.Form.Show(resolveClientUrl('Sportsbook/EditMultiplePT'), 'Edit Sportsbook Setting ' + '<font color="#6dcff6">' + title + '</font>', 894, 700, false, false, 1); } },
            ptSportbook: { name: "PT", className: "iconEnableEdit", callback: function (key, opt) { Fanex.ExtAcc.Form.Show(resolveClientUrl('Sportsbook/EditPT'), 'Edit PT Setting ' + '<font color="#6dcff6">' + title + '</font>', 888, 700, false, false, 1); } },
            minPTSportbook: { name: "Min PT", className: "iconEnableEdit", callback: function (key, opt) { Fanex.ExtAcc.Form.Show(resolveClientUrl('Sportsbook/EditMinPT'), 'Edit Min PT ' + '<font color="#6dcff6">' + title + '</font>', 888, 700, false, false, 1); } },
            betSetting: { name: "Bet Setting", className: "iconEnableEdit", callback: function (key, opt) { Fanex.ExtAcc.Form.Show(resolveClientUrl('Sportsbook/EditBetSetting'), 'Edit Bet Setting ' + '<font color="#6dcff6">' + title + '</font>', 871, 500, false, false, 1); } },
            commission: { name: "Commission", className: "iconEnableEdit", callback: function (key, opt) { Fanex.ExtAcc.Form.Show(resolveClientUrl('Sportsbook/EditCommissionDetail'), 'Edit ' + '<font color="#6dcff6">' + title + '</font>', 871, 225, false, false, 1); } },
            separator: "-----",
            racing: { name: "Racing", className: "iconEnableEdit" },
            minPTRacing: { name: "Min PT", className: "iconEnableEdit" },
            separator1: "-----"
        };

        if (superSyncCasino) {
            if (syncCasino) {
                if (disableCasino) {
                    itemsMaster.casino = { name: "Casino", className: "iconDisableEdit", callback: function () { Fanex.ExtAcc.Form.Show(resolveClientUrl('Casino/EditCasino?custId=' + Fanex.ExtAcc.Customer.UserId + '&uplineId=' + Fanex.ExtAcc.Customer.superId), 'Edit Casino Setting <font color="#6dcff6">' + title + '</font>', 1024, 700, false, null, 1); } };
                }
                else {
                    itemsMaster.casino = { name: "Casino", className: "iconEnableEdit", callback: function () { Fanex.ExtAcc.Form.Show(resolveClientUrl('Casino/EditCasino?custId=' + Fanex.ExtAcc.Customer.UserId + '&uplineId=' + Fanex.ExtAcc.Customer.superId), 'Edit Casino Setting <font color="#6dcff6">' + title + '</font>', 1024, 700, false, null, 1); } };
                }
            }
            else {
                itemsMaster.casino = { name: "Casino", className: "iconUnsyncEdit", callback: function () { Fanex.ExtAcc.Form.Show(resolveClientUrl('Casino/EditCasino?custId=' + Fanex.ExtAcc.Customer.UserId + '&uplineId=' + Fanex.ExtAcc.Customer.superId), 'Edit Casino Setting <font color="#6dcff6">' + title + '</font>', 1024, 700, false, null, 1); } };
            }
        }

        if (superSyncBingo) {
            if (syncBingo) {
                if (disableBingo) {
                    itemsMaster.bingo = { name: "Bingo", className: "iconDisableEdit" };
                }
                else {
                    itemsMaster.bingo = { name: "Bingo", className: "iconEnableEdit" };
                }
            }
            else {
                itemsMaster.bingo = { name: "Bingo", className: "iconUnsyncEdit" };
            }
        }

        if (!disableP2P) {
            itemsMaster.poker = { name: "Poker", className: "iconEnableEdit" };
        }

        if (superSyncLiveCasino) {
            if (syncLiveCasino) {
                if (disableLiveCasino) {
                    itemsMaster.liveCasino = { name: "Live Casino", className: "iconDisableEdit", callback: function () {
                        Fanex.ExtAcc.Form.Show(resolveClientUrl('LiveCasino/EditMaster'), 'Edit Live Casino Setting <font color="#6dcff6">' + title + '</font>', 888, 700, false, false, 1);
                    }
                    };
                }
                else {
                    itemsMaster.liveCasino = { name: "Live Casino", className: "iconEnableEdit", callback: function () {
                        Fanex.ExtAcc.Form.Show(resolveClientUrl('LiveCasino/EditMaster'), 'Edit Live Casino Setting <font color="#6dcff6">' + title + '</font>', 888, 700, false, false, 1);
                    }
                    };
                }
            }
            else {
                itemsMaster.liveCasino = { name: "Live Casino", className: "iconUnsyncEdit", callback: function () {
                    Fanex.ExtAcc.Form.Show(resolveClientUrl('LiveCasino/EditMaster'), 'Edit Live Casino Setting <font color="#6dcff6">' + title + '</font>', 888, 700, false, false, 1);
                }
                };
            }
        }

        var uplineId = 0;
        var uplineRoleId = 0;
        switch (Fanex.ExtAcc.Customer.CurrentLevel) {
            case Fanex.ExtAcc.Customer.Master:
                uplineId = Fanex.ExtAcc.Customer.superId;
                uplineRoleId = Fanex.ExtAcc.Customer.Super;
                break;
            case Fanex.ExtAcc.Customer.Agent:
                uplineId = Fanex.ExtAcc.Customer.masterId;
                uplineRoleId = Fanex.ExtAcc.Customer.Master;
                break;
            case Fanex.ExtAcc.Customer.Member:
                uplineId = Fanex.ExtAcc.Customer.agentId;
                uplineRoleId = Fanex.ExtAcc.Customer.Agent;
                break;
        }

        if (disableVirtualSport) {
            itemsMaster.virtualSport = { name: "Virtual Sports", className: "iconDisableEdit" };
        }
        else {
            itemsMaster.virtualSport = { name: "Virtual Sports", className: "iconEnableEdit", callback: function () { Fanex.ExtAcc.Form.Show(resolveClientUrl('VirtualSports/EditSingle?custId=' + uplineId + '&roleId=' + uplineRoleId + '&targetCustId=' + Fanex.ExtAcc.Customer.UserId + '&targetRoleId=' + Fanex.ExtAcc.Customer.Master), 'Edit Virtual Sports Setting <font color="#6dcff6">' + title + '</font>', 880, 700, false, null, 1); } };
        }

        itemsMaster.separator2 = "-----";
        itemsMaster.editInfo = { name: "Edit Info", className: "iconEnableEdit editInfoBg", callback: function (key, opt) { Fanex.ExtAcc.Form.Show(resolveClientUrl('Customer/EditInfo/?custId=' + Fanex.ExtAcc.Customer.UserId), titles[0] + ': <font color="#6dcff6">' + Fanex.ExtAcc.Customer.TitleEditInfo + '</font>', 406, 310, false); } };
        itemsMaster.changePassword = { name: "Reset Password", className: "iconEnableEdit editInfoBg", callback: function (key, opt) { Fanex.ExtAcc.Form.Show(resolveClientUrl('Customer/ResetPassword/?custId=' + Fanex.ExtAcc.Customer.UserId), 'Reset password', 420, 230, false); } };

        if (transferCondition == false) {
            itemsMaster.transferCondition = { name: "Transfer Condition", className: "iconEnableEdit editInfoBg", callback: function (key, opt) { Fanex.ExtAcc.Form.Show(resolveClientUrl('Customer/EditTransferCondition/?custId=' + Fanex.ExtAcc.Customer.UserId), titles[1] + ': <font color="#6dcff6">' + Fanex.ExtAcc.Customer.TitleEditInfo + '</font>', 600, 250, false, false, 1); } };
        }

        $.contextMenu('destroy');

        $.contextMenu({
            selector: '.editMaster',
            items: itemsMaster,
            trigger: "left",
            ignoreRightClick: true,
            autoHide: true
        });
    },

    ShowEditAgent: function (custId, title, transferCondition, syncCasino, disableCasino, syncBingo, disableBingo, disableP2P, syncLiveCasino, disableLiveCasino, disableVirtualSport, masterSyncCasino, masterSyncBingo, masterSyncLiveCasino) {
        Fanex.ExtAcc.Customer.UserId = custId;
        Fanex.ExtAcc.Customer.TitleEditInfo = title;

        this.ResetFlags();

        var itemsAgent = {
            sportbook: { name: "Sportsbook", className: "iconEnableEdit", callback: function () { Fanex.ExtAcc.Customer.IsMultiple = false; Fanex.ExtAcc.Customer.IsFullInfo = true; Fanex.ExtAcc.Form.Show(resolveClientUrl('Sportsbook/EditMultiplePT'), 'Edit Sportsbook Setting ' + '<font color="#6dcff6">' + title + '</font>', 894, 700, false, false, 1); } },
            ptSportbook: { name: "PT", className: "iconEnableEdit", callback: function (key, opt) { Fanex.ExtAcc.Form.Show(resolveClientUrl('Sportsbook/EditPT'), 'Edit PT Setting ' + '<font color="#6dcff6">' + title + '</font>', 888, 700, false, false, 1); } },
            minPTSportbook: { name: "Min PT", className: "iconEnableEdit", callback: function (key, opt) { Fanex.ExtAcc.Form.Show(resolveClientUrl('Sportsbook/EditMinPT'), 'Edit Min PT ' + '<font color="#6dcff6">' + title + '</font>', 888, 700, false, false, 1); } },
            betSetting: { name: "Bet Setting", className: "iconEnableEdit", callback: function (key, opt) { Fanex.ExtAcc.Form.Show(resolveClientUrl('Sportsbook/EditBetSetting'), 'Edit Bet Setting ' + '<font color="#6dcff6">' + title + '</font>', 871, 500, false, false, 1); } },
            commission: { name: "Commission", className: "iconEnableEdit", callback: function (key, opt) { Fanex.ExtAcc.Form.Show(resolveClientUrl('Sportsbook/EditCommissionDetail'), 'Edit ' + '<font color="#6dcff6">' + title + '</font>', 871, 225, false, false, 1); } },
            separator: "-----",
            racing: { name: "Racing", className: "iconEnableEdit" },
            minPTRacing: { name: "Min PT", className: "iconEnableEdit" },
            separator1: "-----"
        };

        if (masterSyncCasino) {
            if (syncCasino) {
                if (disableCasino) {
                    itemsAgent.casino = { name: "Casino", className: "iconDisableEdit", callback: function () { Fanex.ExtAcc.Form.Show(resolveClientUrl('Casino/EditCasino?custId=' + Fanex.ExtAcc.Customer.UserId + '&uplineId=' + Fanex.ExtAcc.Customer.masterId), 'Edit Casino Setting <font color="#6dcff6">' + title + '</font>', 1024, 700, false, null, 1); } };
                    itemsAgent.scasino = { name: "Super Edit Casino", className: "iconDisableEdit", callback: function () { Fanex.ExtAcc.Customer.isSuperEditAgent = true; Fanex.ExtAcc.Form.Show(resolveClientUrl('Casino/EditCasino?custId=' + Fanex.ExtAcc.Customer.UserId + '&uplineId=' + Fanex.ExtAcc.Customer.superId), 'Super Edit Casino Setting <font color="#6dcff6">' + title + '</font>', 1024, 700, false, null, 1); } };
                }
                else {
                    itemsAgent.casino = { name: "Casino", className: "iconEnableEdit", callback: function () { Fanex.ExtAcc.Form.Show(resolveClientUrl('Casino/EditCasino?custId=' + Fanex.ExtAcc.Customer.UserId + '&uplineId=' + Fanex.ExtAcc.Customer.masterId), 'Edit Casino Setting <font color="#6dcff6">' + title + '</font>', 1024, 700, false, null, 1); } };
                    itemsAgent.scasino = { name: "Super Edit Casino", className: "iconEnableEdit", callback: function () { Fanex.ExtAcc.Customer.isSuperEditAgent = true; Fanex.ExtAcc.Form.Show(resolveClientUrl('Casino/EditCasino?custId=' + Fanex.ExtAcc.Customer.UserId + '&uplineId=' + Fanex.ExtAcc.Customer.superId), 'Super Edit Casino Setting <font color="#6dcff6">' + title + '</font>', 1024, 700, false, null, 1); } };
                }
            }
            else {
                itemsAgent.casino = { name: "Casino", className: "iconUnsyncEdit", callback: function () { Fanex.ExtAcc.Form.Show(resolveClientUrl('Casino/EditCasino?custId=' + Fanex.ExtAcc.Customer.UserId + '&uplineId=' + Fanex.ExtAcc.Customer.masterId), 'Edit Casino Setting <font color="#6dcff6">' + title + '</font>', 1024, 700, false, null, 1); } };
                itemsAgent.scasino = { name: "Super Edit Casino", className: "iconUnsyncEdit", callback: function () { Fanex.ExtAcc.Customer.isSuperEditAgent = true; Fanex.ExtAcc.Form.Show(resolveClientUrl('Casino/EditCasino?custId=' + Fanex.ExtAcc.Customer.UserId + '&uplineId=' + Fanex.ExtAcc.Customer.superId), 'Super Edit Casino Setting <font color="#6dcff6">' + title + '</font>', 1024, 700, false, null, 1); } };
            }
        }

        if (masterSyncBingo) {
            if (syncBingo) {
                if (disableBingo) {
                    itemsAgent.bingo = { name: "Bingo", className: "iconDisableEdit" };
                }
                else {
                    itemsAgent.bingo = { name: "Bingo", className: "iconEnableEdit" };
                }
            }
            else {
                itemsAgent.bingo = { name: "Bingo", className: "iconUnsyncEdit" };
            }
        }

        if (!disableP2P) {
            itemsAgent.poker = { name: "Poker", className: "iconEnableEdit" };
        }

        if (masterSyncLiveCasino) {
            if (syncLiveCasino) {
                if (disableLiveCasino) {
                    itemsAgent.liveCasino = { name: "Live Casino", className: "iconDisableEdit", callback: function () {
                        Fanex.ExtAcc.Form.Show(resolveClientUrl('LiveCasino/EditAgent'), 'Edit Live Casino Setting <font color="#6dcff6">' + title + '</font>', 888, 700, false, false, 1);
                    }
                    };
                }
                else {
                    itemsAgent.liveCasino = { name: "Live Casino", className: "iconEnableEdit", callback: function () {
                        Fanex.ExtAcc.Form.Show(resolveClientUrl('LiveCasino/EditAgent'), 'Edit Live Casino Setting <font color="#6dcff6">' + title + '</font>', 888, 700, false, false, 1);
                    }
                    };
                }
            }
            else {
                itemsAgent.liveCasino = { name: "Live Casino", className: "iconUnsyncEdit", callback: function () {
                    Fanex.ExtAcc.Form.Show(resolveClientUrl('LiveCasino/EditAgent'), 'Edit Live Casino Setting <font color="#6dcff6">' + title + '</font>', 888, 700, false, false, 1);
                }
                };
            }
        }

        if (disableVirtualSport) {
            itemsAgent.virtualSport = { name: "Virtual Sports", className: "iconDisableEdit" };
        }
        else {
            itemsAgent.virtualSport = { name: "Virtual Sports", className: "iconEnableEdit", callback: function () { Fanex.ExtAcc.Form.Show(resolveClientUrl('VirtualSports/EditSingle?custId=' + uplineId + '&roleId=' + uplineRoleId + '&targetCustId=' + Fanex.ExtAcc.Customer.UserId + '&targetRoleId=' + Fanex.ExtAcc.Customer.Master), 'Edit Virtual Sports Setting <font color="#6dcff6">' + title + '</font>', 870, 700, false, null, 1); } };
        }

        itemsAgent.separator2 = "-----";
        itemsAgent.editInfo = { name: "Edit Info", className: "iconEnableEdit editInfoBg", callback: function (key, opt) { Fanex.ExtAcc.Form.Show(resolveClientUrl('Customer/EditInfo/?custId=' + Fanex.ExtAcc.Customer.UserId), titles[0] + ': <font color="#6dcff6">' + Fanex.ExtAcc.Customer.TitleEditInfo + '</font>', 406, 310, false); } };
        itemsAgent.changePassword = { name: "Reset Password", className: "iconEnableEdit editInfoBg", callback: function (key, opt) { Fanex.ExtAcc.Form.Show(resolveClientUrl('Customer/ResetPassword/?custId=' + Fanex.ExtAcc.Customer.UserId), 'Reset password', 420, 230, false); } };

        if (transferCondition == false) {
            itemsAgent.transferCondition = { name: "Transfer Condition", className: "iconEnableEdit editInfoBg", callback: function (key, opt) { Fanex.ExtAcc.Form.Show(resolveClientUrl('Customer/EditTransferCondition/?custId=' + Fanex.ExtAcc.Customer.UserId), titles[1] + ': <font color="#6dcff6">' + Fanex.ExtAcc.Customer.TitleEditInfo + '</font>', 600, 250, false, false, 1); } };
        }

        $.contextMenu('destroy');

        $.contextMenu({
            selector: '.editAgent',
            items: itemsAgent,
            trigger: "left",
            ignoreRightClick: true,
            autoHide: true
        });
    },

    ShowEditMember: function (custId, title, transferCondition, syncCasino, disableCasino, syncBingo, disableBingo, disableP2P, syncLiveCasino, disableLiveCasino, disableVirtualSport, agentSyncCasino, agentSyncBingo, agentSyncLiveCasino) {
        Fanex.ExtAcc.Customer.UserId = custId;
        Fanex.ExtAcc.Customer.TitleEditInfo = title;

        this.ResetFlags();

        var itemsMember = {
            sportbook: { name: "Sportsbook", className: "iconEnableEdit", callback: function () { Fanex.ExtAcc.Customer.IsMultiple = false; Fanex.ExtAcc.Customer.IsFullInfo = true; Fanex.ExtAcc.Form.Show(resolveClientUrl('Sportsbook/EditMultiplePT'), 'Edit Sportsbook Setting ' + '<font color="#6dcff6">' + title + '</font>', 894, 700, false, false, 1); } },
            ptSportbook: { name: "PT", className: "iconEnableEdit", callback: function (key, opt) { Fanex.ExtAcc.Form.Show(resolveClientUrl('Sportsbook/EditPT'), 'Edit PT Setting ' + '<font color="#6dcff6">' + title + '</font>', 888, 700, false, false, 1); } },
            superEditSportbook: { name: "Super Edit Sportsbook", className: "iconEnableEdit", callback: function (key, opt) { Fanex.ExtAcc.Customer.isSuperEditMember = true; Fanex.ExtAcc.Customer.isMasterEditMember = false; Fanex.ExtAcc.Customer.IsMultiple = false; Fanex.ExtAcc.Customer.IsFullInfo = true; Fanex.ExtAcc.Form.Show(resolveClientUrl('Sportsbook/SuperMasterEditSB'), 'Super Edit Sportsbook Setting ' + '<font color="#6dcff6">' + title + '</font>', 894, 700, false, false, 1); } },
            masterEditSportbook: { name: "Master Edit Sportsbook", className: "iconEnableEdit", callback: function (key, opt) { Fanex.ExtAcc.Customer.isMasterEditMember = true; Fanex.ExtAcc.Customer.isSuperEditMember = false; Fanex.ExtAcc.Customer.IsMultiple = false; Fanex.ExtAcc.Customer.IsFullInfo = true; Fanex.ExtAcc.Form.Show(resolveClientUrl('Sportsbook/SuperMasterEditSB'), 'Master Edit Sportsbook Setting ' + '<font color="#6dcff6">' + title + '</font>', 894, 700, false, false, 1); } },
            superEditPT: { name: "Super Edit PT", className: "iconEnableEdit", callback: function (key, opt) { Fanex.ExtAcc.Customer.isSuperEditMember = true; Fanex.ExtAcc.Customer.isMasterEditMember = false; Fanex.ExtAcc.Form.Show(resolveClientUrl('Sportsbook/EditPT'), 'Super Edit PT Setting ' + '<font color="#6dcff6">' + title + '</font>', 888, 700, false, false, 1); } },
            masterEditPT: { name: "Master Edit PT", className: "iconEnableEdit", callback: function (key, opt) { Fanex.ExtAcc.Customer.isMasterEditMember = true; Fanex.ExtAcc.Customer.isSuperEditMember = false; Fanex.ExtAcc.Form.Show(resolveClientUrl('Sportsbook/EditPT'), 'Master Edit PT Setting ' + '<font color="#6dcff6">' + title + '</font>', 888, 700, false, false, 1); } },
            betSetting: { name: "Bet Setting", className: "iconEnableEdit", callback: function (key, opt) { Fanex.ExtAcc.Form.Show(resolveClientUrl('Sportsbook/EditBetSetting'), 'Edit Bet Setting ' + '<font color="#6dcff6">' + title + '</font>', 871, 500, false, false, 1); } },
            commission: { name: "Commission", className: "iconEnableEdit", callback: function (key, opt) { Fanex.ExtAcc.Form.Show(resolveClientUrl('Sportsbook/EditCommissionMemberDetail'), 'Edit ' + '<font color="#6dcff6">' + title + '</font>', 871, 305, false, false, 1); } },
            separator: "-----",
            racing: { name: "Racing", className: "iconEnableEdit" },
            separator1: "-----"
        };

        if (agentSyncCasino) {
            if (syncCasino) {
                if (disableCasino) {
                    itemsMember.casino = { name: "Casino", className: "iconDisableEdit", callback: function () { Fanex.ExtAcc.Form.Show(resolveClientUrl('Casino/EditCasino?custId=' + Fanex.ExtAcc.Customer.UserId + '&uplineId=' + Fanex.ExtAcc.Customer.agentId), 'Edit Casino Setting <font color="#6dcff6">' + title + '</font>', 880, 700, false, null, 1); } };
                }
                else {
                    itemsMember.casino = { name: "Casino", className: "iconEnableEdit", callback: function () { Fanex.ExtAcc.Form.Show(resolveClientUrl('Casino/EditCasino?custId=' + Fanex.ExtAcc.Customer.UserId + '&uplineId=' + Fanex.ExtAcc.Customer.agentId), 'Edit Casino Setting <font color="#6dcff6">' + title + '</font>', 880, 700, false, null, 1); } };
                }
            }
            else {
                itemsMember.casino = { name: "Casino", className: "iconUnsyncEdit", callback: function () { Fanex.ExtAcc.Form.Show(resolveClientUrl('Casino/EditCasino?custId=' + Fanex.ExtAcc.Customer.UserId + '&uplineId=' + Fanex.ExtAcc.Customer.agentId), 'Edit Casino Setting <font color="#6dcff6">' + title + '</font>', 880, 700, false, null, 1); } };
            }
        }

        if (agentSyncBingo) {
            if (syncBingo) {
                if (disableBingo) {
                    itemsMember.bingo = { name: "Bingo", className: "iconDisableEdit" };
                }
                else {
                    itemsMember.bingo = { name: "Bingo", className: "iconEnableEdit" };
                }
            }
            else {
                itemsMember.bingo = { name: "Bingo", className: "iconUnsyncEdit" };
            }
        }

        if (!disableP2P) {
            itemsMember.poker = { name: "Poker", className: "iconEnableEdit" };
        }

        if (agentSyncLiveCasino) {
            if (syncLiveCasino) {
                if (disableLiveCasino) {
                    itemsMember.liveCasino = { name: "Live Casino", className: "iconDisableEdit", callback: function () {
                        Fanex.ExtAcc.Form.Show(resolveClientUrl('LiveCasino/EditMember'), 'Edit Live Casino Setting <font color="#6dcff6">' + title + '</font>', 888, 700, false, false, 1);
                    }
                    };
                    itemsMember.liveCasinoSuper = { name: "Super Edit Live Casino", className: "iconDisableEdit", callback: function () {
                        Fanex.ExtAcc.Customer.isSuperEditMember = true;
                        Fanex.ExtAcc.Customer.isMasterEditMember = false;

                        Fanex.ExtAcc.Form.Show(resolveClientUrl('LiveCasino/SuperEditMember'), 'Super Edit Live Casino Setting <font color="#6dcff6">' + title + '</font>', 888, 700, false, false, 1);
                    }
                    };
                    itemsMember.liveCasinoMaster = { name: "Master Edit Live Casino", className: "iconDisableEdit", callback: function () {
                        Fanex.ExtAcc.Customer.isSuperEditMember = false;
                        Fanex.ExtAcc.Customer.isMasterEditMember = true;

                        Fanex.ExtAcc.Form.Show(resolveClientUrl('LiveCasino/MasterEditMember'), 'Master Edit Live Casino Setting <font color="#6dcff6">' + title + '</font>', 888, 700, false, false, 1);
                    }
                    };
                }
                else {
                    itemsMember.liveCasino = { name: "Live Casino", className: "iconEnableEdit", callback: function () {
                        Fanex.ExtAcc.Form.Show(resolveClientUrl('LiveCasino/EditMember'), 'Edit Live Casino Setting <font color="#6dcff6">' + title + '</font>', 888, 700, false, false, 1);
                    }
                    };
                    itemsMember.liveCasinoSuper = { name: "Super Edit Live Casino", className: "iconEnableEdit", callback: function () {
                        Fanex.ExtAcc.Customer.isSuperEditMember = true;
                        Fanex.ExtAcc.Customer.isMasterEditMember = false;

                        Fanex.ExtAcc.Form.Show(resolveClientUrl('LiveCasino/SuperEditMember'), 'Super Edit Live Casino Setting <font color="#6dcff6">' + title + '</font>', 888, 700, false, false, 1);
                    }
                    };
                    itemsMember.liveCasinoMaster = { name: "Master Edit Live Casino", className: "iconEnableEdit", callback: function () {
                        Fanex.ExtAcc.Customer.isSuperEditMember = false;
                        Fanex.ExtAcc.Customer.isMasterEditMember = true;

                        Fanex.ExtAcc.Form.Show(resolveClientUrl('LiveCasino/MasterEditMember'), 'Master Edit Live Casino Setting <font color="#6dcff6">' + title + '</font>', 888, 700, false, false, 1);
                    }
                    };
                }
            }
            else {
                itemsMember.liveCasino = { name: "Live Casino", className: "iconUnsyncEdit", callback: function () {
                    Fanex.ExtAcc.Form.Show(resolveClientUrl('LiveCasino/EditMember'), 'Edit Live Casino Setting <font color="#6dcff6">' + title + '</font>', 888, 700, false, false, 1);
                }
                };
            }
        }

        if (disableVirtualSport) {
            itemsMember.virtualSport = { name: "Virtual Sports", className: "iconDisableEdit" };
        }
        else {
            itemsMember.virtualSport = { name: "Virtual Sports", className: "iconEnableEdit" };
        }

        itemsMember.separator2 = "-----";
        itemsMember.editInfo = { name: "Edit Info", className: "iconEnableEdit editInfoBg", callback: function (key, opt) { Fanex.ExtAcc.Form.Show(resolveClientUrl('Customer/EditInfo/?custId=' + Fanex.ExtAcc.Customer.UserId), titles[0] + ': <font color="#6dcff6">' + Fanex.ExtAcc.Customer.TitleEditInfo + '</font>', 406, 310, false); } };
        itemsMember.changePassword = { name: "Reset Password", className: "iconEnableEdit editInfoBg", callback: function (key, opt) { Fanex.ExtAcc.Form.Show(resolveClientUrl('Customer/ResetPassword/?custId=' + Fanex.ExtAcc.Customer.UserId), 'Reset password', 420, 230, false); } };

        $.contextMenu('destroy');
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

        var editUrl = 'Customer/EditInfoPost';

        var data = {
            custId: Fanex.ExtAcc.Customer.UserId,
            firstName: Fanex.ExtAcc.EscapeHtml(firstName),
            lastName: Fanex.ExtAcc.EscapeHtml(lastName),
            homePhone: Fanex.ExtAcc.EscapeHtml(homePhone),
            mobilePhone: Fanex.ExtAcc.EscapeHtml(mobilePhone),
            email: Fanex.ExtAcc.EscapeHtml(email),
            fax: Fanex.ExtAcc.EscapeHtml(fax)
        };

        Fanex.ExtAcc.BlockUI();

        $.ajax({ url: resolveClientUrl(editUrl), data: data, type: 'POST' })
        .done(function (data) {
            if (!data.IsSuccess) {
                $('#errMsg').show();
                $('#errMsg').html(data.ErrorMessage.formatErrorMessage());
                Fanex.ExtAcc.UnBlockUI();
            }
            else {
                var timeout = setTimeout(function () {
                    clearTimeout(timeout);
                    Fanex.ExtAcc.Customer.CallBackCustomerList();
                    Fanex.ExtAcc.UnBlockUI();
                    Fanex.ExtAcc.Form.Close();
                }, 2000);
            }
        })
        .error(function (data) {
            if (typeof console != 'undefined') console.log(data.responseText);
            Fanex.ExtAcc.UnBlockUI();
        });
    },

    GetSuperList: function (custId, dataSource) {
        Fanex.ExtAcc.BlockUI();

        Fanex.ExtAcc.Customer.CurrentLevel = Fanex.ExtAcc.Customer.Super;
        Fanex.ExtAcc.Customer.AllowDoubleComm = true;

        var url = resolveClientUrl('Customer/SuperListPost');

        var data = Fanex.ExtAcc.Customer.GetParams();
        data.custId = Fanex.ExtAcc.Customer.custId = custId == 'undefined' ? null : custId;

        if (custId == undefined) {
            Fanex.ExtAcc.Customer.directSuperId = null;
            Fanex.ExtAcc.Customer.directMasterId = null;
            Fanex.ExtAcc.Customer.directAgentId = null;
        }

        Fanex.ExtAcc.Customer.DataFields = [
                    { HeaderText: 'No.', Name: 'RowNumber', Width: 30, CellAlign: 'center' },
                    { HeaderText: '', Name: 'Check', Width: 30, CellAlign: 'center',
                        CustomHeaderRender: function (cell) {
                            return '<div id="editMult" class="editMult"></div>';
                        },
                        CustomCellRender: function (cell) {
                            return '<div onclick="Fanex.ExtAcc.Customer.Select(this)" style="margin-left: 0px !important" id="' + cell.Value.custid + '" class="menuInactive"></div>';
                        }
                    },
                    { HeaderText: 'Username', Name: 'username', Width: 100,
                        CustomCellRender: function (cell) {
                            var custId = cell.Value.custid;
                            var func = "javascript:Fanex.ExtAcc.Customer.GetMasterList(" + custId + ", " + cell.Value.DoubleCommLosing + ", null, null, " + cell.Value.synccasino + ", " + cell.Value.syncBingo + ", " + cell.Value.SyncLiveCasino + ")";
                            Fanex.ExtAcc.Customer.superDisableP2P = cell.Value.DisableP2P;

                            return '<div><a id="usr-' + custId + '" href="' + func + '">' + cell.Value.username + '</a></div>';
                        }
                    },
                    { HeaderText: 'Edit', Name: 'Edit', Width: 30,
                        CustomCellRender: function (cell) {
                            var func = "javascript:Fanex.ExtAcc.Customer.ShowEditSuper(" + cell.Value.custid + ", '" + cell.Value.username.replace('$', '&#36') + "')";

                            return '<div onclick="' + func + '" id="edit-' + cell.Value.custid + '" class="editInfo"></div>';
                        }
                    },
                    { HeaderText: 'Status', Name: 'Status', Width: 75, CellAlign: 'center',
                        CustomCellRender: function (cell) {
                            return !cell.Value.enabled ? 'Disabled' : cell.Value.closed ? 'Closed' : cell.Value.suspended ? 'Suspended' : 'Open';
                        }
                    },
                    { HeaderText: 'Double Comm', Name: 'DoubleCommLosing', Width: 80,
                        CustomCellRender: function (cell) {
                            if (cell.Value.DoubleCommLosing) {
                                return '<div onclick="Fanex.ExtAcc.Customer.ConfigDoubleComm(' + cell.Value.custid + ', true)" id="dbl-' + cell.Value.custid + '" class="allowDblComm"></div>';
                            }

                            return '<div onclick="Fanex.ExtAcc.Customer.ConfigDoubleComm(' + cell.Value.custid + ', true)" id="dbl-' + cell.Value.custid + '" class="disallowDblComm"></div>';
                        }
                    },
                    { HeaderText: 'First Name', Name: 'firstname', Width: 120 },
                    { HeaderText: 'Last Name', Name: 'lastname', Width: 120 },
                    { HeaderText: 'Group A', Name: 'groupa', Width: 60, CellAlign: 'center' },
                    { HeaderText: 'Group B', Name: 'groupb', Width: 60, CellAlign: 'center' },
                    { HeaderText: 'Group C', Name: 'groupc', Width: 60, CellAlign: 'center' },
                    { HeaderText: 'Group D', Name: 'groupd', Width: 60, CellAlign: 'center' },
                    { HeaderText: '1X2', Name: 'discount1x2', Width: 60, CellAlign: 'center' },
                    { HeaderText: 'Others', Name: 'discountcs', Width: 60, CellAlign: 'center' },
                    { HeaderText: 'Game', Name: 'discountnumber', Width: 60, CellAlign: 'center' },
                    { HeaderText: 'Created Date', Name: 'createdate', CellAlign: 'center', Width: 160, FormatDateTime: 'MM/DD/YYYY hh:mm:ss A' },
                    { HeaderText: 'Login IP', Name: 'lastloginip', CellAlign: 'center', Width: 100,
                        CustomCellRender: function (cell) {
                            return '<div><a href="javascript:Fanex.ExtAcc.Customer.ShowIpInfo(\'' + cell.Value + '\')">' + cell.Value + '</a></div>';
                        }
                    }
                ];

        if (dataSource) {
            Fanex.ExtAcc.Customer.RenderSuperList(dataSource);
        }
        else {
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

    GetMasterList: function (srecommend, allow, custId, dataSource, superSyncCasino, superSyncBingo, superSyncLiveCasino) {
        Fanex.ExtAcc.BlockUI();

        Fanex.ExtAcc.Customer.superId = srecommend;
        Fanex.ExtAcc.Customer.CurrentLevel = Fanex.ExtAcc.Customer.Master;
        Fanex.ExtAcc.Customer.AllowDoubleComm = allow;
        Fanex.ExtAcc.Customer.AllowDoubleCommMaster = allow;

        Fanex.ExtAcc.Customer.superSyncCasino = superSyncCasino;
        Fanex.ExtAcc.Customer.superSyncBingo = superSyncBingo;
        Fanex.ExtAcc.Customer.superSyncLiveCasino = superSyncLiveCasino;

        var url = resolveClientUrl('Customer/MasterListPost');
        var data = Fanex.ExtAcc.Customer.GetParams();
        data.custId = Fanex.ExtAcc.Customer.custId = custId == 'undefined' ? null : custId;

        if (custId == undefined) {
            Fanex.ExtAcc.Customer.directSuperId = null;
            Fanex.ExtAcc.Customer.directMasterId = null;
            Fanex.ExtAcc.Customer.directAgentId = null;
        }

        Fanex.ExtAcc.Customer.DataFields = [
                    { HeaderText: 'No.', Name: 'RowNumber', Width: 30, CellAlign: 'center' },
                    { HeaderText: '', Name: 'Check', Width: 30, CellAlign: 'center',
                        CustomHeaderRender: function (cell) {
                            return '<div id="editMult" class="editMult"></div>';
                        },
                        CustomCellRender: function (cell) {
                            return '<div onclick="Fanex.ExtAcc.Customer.Select(this)" style="margin-left: 0px !important" id="' + cell.Value.custid + '" class="menuInactive"></div>';
                        }
                    },
                    { HeaderText: 'Username', Name: 'username', Width: 100,
                        CustomCellRender: function (cell) {
                            var custId = cell.Value.custid;
                            var func = "javascript:Fanex.ExtAcc.Customer.GetAgentList(" + custId + ", " + cell.Value.DoubleCommLosing + ", null, null, " + cell.Value.synccasino + ", " + cell.Value.syncBingo + ", " + cell.Value.SyncLiveCasino + ")";
                            Fanex.ExtAcc.Customer.masterDisableP2P = cell.Value.DisableP2P;

                            return '<div><a href="' + func + '">' + cell.Value.username + '</a></div>';
                        }
                    },
                    { HeaderText: 'Edit', Name: 'Edit', Width: 30,
                        CustomCellRender: function (cell) {
                            var title = cell.Value.username.replace('$', '&#36');
                            var func = "javascript:Fanex.ExtAcc.Customer.ShowEditMaster(" + cell.Value.custid + ", '" + title + "', " + cell.Value.disableTransfer + ", " + cell.Value.synccasino + ", " + cell.Value.disablecasino + ", " + cell.Value.syncBingo + ", " + cell.Value.DisableBingo + ", " + cell.Value.DisableP2P + ", " + cell.Value.SyncLiveCasino + ", " + cell.Value.DisableLiveCasino + ", " + cell.Value.DisableVirtualSports + ", " + superSyncCasino + ", " + superSyncBingo + ", " + superSyncLiveCasino + ")";

                            return '<div onclick="' + func + '" id="edit-' + cell.Value.custid + '" class="editMaster"></div>';
                        }
                    },
                    { HeaderText: 'Status', Name: 'Status', Width: 75, CellAlign: 'center',
                        CustomCellRender: function (cell) {
                            return !cell.Value.enabled ? 'Disabled' : cell.Value.closed ? 'Closed' : cell.Value.suspended ? 'Suspended' : 'Open';
                        }
                    },
                    { HeaderText: 'Double Comm', Name: 'DoubleCommLosing', Width: 80,
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
                    },
                    { HeaderText: 'First Name', Name: 'firstname', Width: 120 },
                    { HeaderText: 'Last Name', Name: 'lastname', Width: 120 },
                    { HeaderText: 'Group A', Name: 'groupa', Width: 60, CellAlign: 'center' },
                    { HeaderText: 'Group B', Name: 'groupb', Width: 60, CellAlign: 'center' },
                    { HeaderText: 'Group C', Name: 'groupc', Width: 60, CellAlign: 'center' },
                    { HeaderText: 'Group D', Name: 'groupd', Width: 60, CellAlign: 'center' },
                    { HeaderText: '1X2', Name: 'discount1x2', Width: 60, CellAlign: 'center' },
                    { HeaderText: 'Others', Name: 'discountcs', Width: 60, CellAlign: 'center' },
                    { HeaderText: 'Game', Name: 'discountnumber', Width: 60, CellAlign: 'center' },
                    { HeaderText: 'Created Date', Name: 'createdate', CellAlign: 'center', Width: 160, FormatDateTime: 'MM/DD/YYYY hh:mm:ss A' },
                    { HeaderText: 'Login IP', Name: 'lastloginip', CellAlign: 'center', Width: 100,
                        CustomCellRender: function (cell) {
                            return '<div><a href="javascript:Fanex.ExtAcc.Customer.ShowIpInfo(\'' + cell.Value + '\')">' + cell.Value + '</a></div>';
                        }
                    }
                ];

        if (dataSource) {
            Fanex.ExtAcc.Customer.RenderMasterList(dataSource);
        }
        else {
            $.ajax({
                url: url,
                data: data,
                dataType: 'json',
                type: 'post'
            })
            .success(function (result) {
                Fanex.ExtAcc.Customer.RenderMasterList(result);
            })
            .error(function (xhr, status, err) {
                Fanex.ExtAcc.UnBlockUI();
            });
        }
    },

    GetAgentList: function (mrecommend, allow, custId, dataSource, masterSyncCasino, masterSyncBingo, masterSyncLiveCasino) {
        Fanex.ExtAcc.BlockUI();

        Fanex.ExtAcc.Customer.masterId = mrecommend;
        Fanex.ExtAcc.Customer.CurrentLevel = Fanex.ExtAcc.Customer.Agent;
        Fanex.ExtAcc.Customer.AllowDoubleComm = allow;
        Fanex.ExtAcc.Customer.AllowDoubleCommAgent = allow;

        Fanex.ExtAcc.Customer.masterSyncCasino = masterSyncCasino;
        Fanex.ExtAcc.Customer.masterSyncBingo = masterSyncBingo;
        Fanex.ExtAcc.Customer.masterSyncLiveCasino = masterSyncLiveCasino;

        var url = resolveClientUrl('Customer/AgentListPost');
        var data = Fanex.ExtAcc.Customer.GetParams();
        data.custId = Fanex.ExtAcc.Customer.custId = custId == 'undefined' ? null : custId;

        if (custId == undefined) {
            Fanex.ExtAcc.Customer.directSuperId = null;
            Fanex.ExtAcc.Customer.directMasterId = null;
            Fanex.ExtAcc.Customer.directAgentId = null;
        }

        Fanex.ExtAcc.Customer.DataFields = [
                    { HeaderText: 'No.', Name: 'RowNumber', Width: 30, CellAlign: 'center' },
                    { HeaderText: '', Name: 'Check', Width: 30, CellAlign: 'center',
                        CustomHeaderRender: function (cell) {
                            return '<div id="editMult" class="editMult"></div>';
                        },
                        CustomCellRender: function (cell) {
                            return '<div onclick="Fanex.ExtAcc.Customer.Select(this)" style="margin-left: 0px !important" id="' + cell.Value.custid + '" class="menuInactive"></div>';
                        }
                    },
                    { HeaderText: 'Username', Name: 'username', Width: 100,
                        CustomCellRender: function (cell) {
                            var custId = cell.Value.custid;
                            var func = "javascript:Fanex.ExtAcc.Customer.GetMemberList(" + custId + ", " + cell.Value.DoubleCommLosing + ", null, null, " + cell.Value.synccasino + ", " + cell.Value.syncBingo + ", " + cell.Value.SyncLiveCasino + ")";
                            Fanex.ExtAcc.Customer.agentDisableP2P = cell.Value.DisableP2P;

                            return '<div><a href="' + func + '">' + cell.Value.username + '</a></div>';
                        }
                    },
                    { HeaderText: 'Edit', Name: 'Edit', Width: 30,
                        CustomCellRender: function (cell) {
                            var title = cell.Value.username.replace('$', '&#36');
                            var func = "javascript:Fanex.ExtAcc.Customer.ShowEditAgent(" + cell.Value.custid + ", '" + title + "', " + cell.Value.disableTransfer + ", " + cell.Value.synccasino + ", " + cell.Value.disablecasino + ", " + cell.Value.syncBingo + ", " + cell.Value.DisableBingo + ", " + cell.Value.DisableP2P + ", " + cell.Value.SyncLiveCasino + ", " + cell.Value.DisableLiveCasino + ", " + cell.Value.DisableVirtualSports + ", " + masterSyncCasino + ", " + masterSyncBingo + ", " + masterSyncLiveCasino + ")";

                            return '<div onclick="' + func + '" id="edit-' + cell.Value.custid + '" class="editAgent"></div>';
                        }
                    },
                    { HeaderText: 'Status', Name: 'Status', Width: 75, CellAlign: 'center',
                        CustomCellRender: function (cell) {
                            return !cell.Value.enabled ? 'Disabled' : cell.Value.closed ? 'Closed' : cell.Value.suspended ? 'Suspended' : 'Open';
                        }
                    },
                    { HeaderText: 'Double Comm', Name: 'DoubleCommLosing', Width: 80,
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
                    },
                    { HeaderText: 'First Name', Name: 'firstname', Width: 120 },
                    { HeaderText: 'Last Name', Name: 'lastname', Width: 120 },
                    { HeaderText: 'Group A', Name: 'groupa', Width: 60, CellAlign: 'center' },
                    { HeaderText: 'Group B', Name: 'groupb', Width: 60, CellAlign: 'center' },
                    { HeaderText: 'Group C', Name: 'groupc', Width: 60, CellAlign: 'center' },
                    { HeaderText: 'Group D', Name: 'groupd', Width: 60, CellAlign: 'center' },
                    { HeaderText: '1X2', Name: 'discount1x2', Width: 60, CellAlign: 'center' },
                    { HeaderText: 'Others', Name: 'discountcs', Width: 60, CellAlign: 'center' },
                    { HeaderText: 'Game', Name: 'discountnumber', Width: 60, CellAlign: 'center' },
                    { HeaderText: 'Created Date', Name: 'createdate', CellAlign: 'center', Width: 160, FormatDateTime: 'MM/DD/YYYY hh:mm:ss A' },
                    { HeaderText: 'Login IP', Name: 'lastloginip', CellAlign: 'center', Width: 100,
                        CustomCellRender: function (cell) {
                            return '<div><a href="javascript:Fanex.ExtAcc.Customer.ShowIpInfo(\'' + cell.Value + '\')">' + cell.Value + '</a></div>';
                        }
                    }
                ];

        if (dataSource) {
            Fanex.ExtAcc.Customer.RenderAgentList(dataSource);
        }
        else {
            $.ajax({
                url: url,
                data: data,
                dataType: 'json',
                type: 'post'
            })
            .success(function (result) {
                Fanex.ExtAcc.Customer.RenderAgentList(result);
            })
            .error(function (xhr, status, err) {
                Fanex.ExtAcc.UnBlockUI();
            });
        }
    },

    GetMemberList: function (recommend, allow, custId, dataSource, agentSyncCasino, agentSyncBingo, agentSyncLiveCasino) {
        Fanex.ExtAcc.BlockUI();

        Fanex.ExtAcc.Customer.agentId = recommend;
        Fanex.ExtAcc.Customer.CurrentLevel = Fanex.ExtAcc.Customer.Member;
        Fanex.ExtAcc.Customer.AllowDoubleComm = allow;

        Fanex.ExtAcc.Customer.agentSyncCasino = agentSyncCasino;
        Fanex.ExtAcc.Customer.agentSyncBingo = agentSyncBingo;
        Fanex.ExtAcc.Customer.agentSyncLiveCasino = agentSyncLiveCasino;

        var url = resolveClientUrl('Customer/MemberListPost');

        var data = Fanex.ExtAcc.Customer.GetParams();
        data.custId = Fanex.ExtAcc.Customer.custId = custId == 'undefined' ? null : custId;

        if (custId == undefined) {
            Fanex.ExtAcc.Customer.directSuperId = null;
            Fanex.ExtAcc.Customer.directMasterId = null;
            Fanex.ExtAcc.Customer.directAgentId = null;
        }

        Fanex.ExtAcc.Customer.DataFields = [
                    { HeaderText: 'No.', Name: 'RowNumber', Width: 30, CellAlign: 'center' },
                    { HeaderText: '', Name: 'Check', Width: 30, CellAlign: 'center',
                        CustomHeaderRender: function (cell) {
                            return '<div id="editMult" class="editMult"></div>';
                        },
                        CustomCellRender: function (cell) {
                            return '<div onclick="Fanex.ExtAcc.Customer.Select(this)" style="margin-left: 0px !important" id="' + cell.Value.custid + '" class="menuInactive"></div>';
                        }
                    },
                    { HeaderText: 'Username', Name: 'username', Width: 100,
                        CustomCellRender: function (cell) {
                            return '<div>' + cell.Value.username + '</div>';
                        }
                    },
                    { HeaderText: 'Edit', Name: 'Edit', Width: 30,
                        CustomCellRender: function (cell) {
                            var title = cell.Value.username.replace('$', '&#36');
                            var func = "javascript:Fanex.ExtAcc.Customer.ShowEditMember(" + cell.Value.custid + ", '" + title + "'," + cell.Value.disableTransfer + ", " + cell.Value.synccasino + ", " + cell.Value.disablecasino + ", " + cell.Value.syncBingo + ", " + cell.Value.DisableBingo + ", " + cell.Value.DisableP2P + ", " + cell.Value.SyncLiveCasino + ", " + cell.Value.DisableLiveCasino + ", " + cell.Value.DisableVirtualSports + ", " + agentSyncCasino + ", " + agentSyncBingo + ", " + agentSyncLiveCasino + ")";

                            return '<div onclick="' + func + '" id="edit-' + cell.Value.custid + '" class="editMember"></div>';
                        }
                    },
                    { HeaderText: 'Status', Name: 'Status', Width: 75, CellAlign: 'center',
                        CustomCellRender: function (cell) {
                            return !cell.Value.enabled ? 'Disabled' : cell.Value.closed ? 'Closed' : cell.Value.suspended ? 'Suspended' : 'Open';
                        }
                    },
                    { HeaderText: 'Double Comm', Name: 'DoubleCommLosing', Width: 57, CellAlign: 'center',
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
                    },
                    { HeaderText: 'Nickname', Name: 'NickName', Width: 120 },
                    { HeaderText: 'First Name', Name: 'firstname', Width: 120 },
                    { HeaderText: 'Last Name', Name: 'lastname', Width: 120 },
                    { HeaderText: 'User Group', Name: 'ugroup', CellAlign: 'center', Width: 50 },
                    { HeaderText: 'Group', Name: 'discount', CellAlign: 'center', Width: 60 },
                    { HeaderText: '1X2', Name: 'discount1x2', CellAlign: 'center', Width: 60 },
                    { HeaderText: 'Others', Name: 'discountcs', CellAlign: 'center', Width: 60 },
                    { HeaderText: 'Game', Name: 'discountnumber', CellAlign: 'center', Width: 60 },
                    { HeaderText: 'Group', Name: 'playerdiscount', CellAlign: 'center', Width: 60 },
                    { HeaderText: '1X2', Name: 'playerdiscount1x2', CellAlign: 'center', Width: 60 },
                    { HeaderText: 'Others', Name: 'playerdiscountcs', CellAlign: 'center', Width: 60 },
                    { HeaderText: 'Game', Name: 'playerdiscountnumber', CellAlign: 'center', Width: 60 },
                    { HeaderText: 'Created Date', Name: 'createdate', CellAlign: 'center', Width: 160, FormatDateTime: 'MM/DD/YYYY hh:mm:ss A' },
                    { HeaderText: 'Login IP', Name: 'lastloginip', CellAlign: 'center', Width: 100,
                        CustomCellRender: function (cell) {
                            return '<div><a href="javascript:Fanex.ExtAcc.Customer.ShowIpInfo(\'' + cell.Value + '\')">' + cell.Value + '</a></div>';
                        }
                    }
                ];

        if (dataSource) {
            Fanex.ExtAcc.Customer.RenderMemberList(dataSource);
        }
        else {
            $.ajax({
                url: url,
                data: data,
                dataType: 'json',
                type: 'post'
            })
            .success(function (result) {
                Fanex.ExtAcc.Customer.RenderMemberList(result);
            })
            .error(function (xhr, status, err) {
                Fanex.ExtAcc.UnBlockUI();
            });
        }
    },

    ResetPwd: function () {
        if (!Validators.validateInput($('#Password').val())) {
            $('#Password').focus();
            Validators.showDown($('#Password').get(0), 'This field is required', 0, 10, 41, -10, 15);
            return false;
        }

        if (!Validators.validatePassword($('#Password').val())) {
            $('#Password').focus();
            Validators.showDown($('#Password').get(0), 'Password must have at least 6 characters, including at least 2 letters and at least 2 digits. (e.g:pass22)', 0, 10, 41, -10, 15, 5000);
            return false;
        }

        if (!Validators.validateInput($('#confirmPwd').val())) {
            $('#confirmPwd').focus();
            Validators.show($('#confirmPwd').get(0), 'This field is required');
            return false;
        }

        if (!Validators.validatePassword($('#confirmPwd').val())) {
            $('#Password').focus();
            Validators.show($('#confirmPwd').get(0), 'Password must have at least 6 characters, including at least 2 letters and at least 2 digits. (e.g:pass22)', null, null, 5000);
            return false;
        }

        if ($('#confirmPwd').val() !== $('#Password').val()) {
            $('#confirmPwd').focus();
            Validators.show($('#confirmPwd').get(0), 'Confirm password is not matched');
            return false;
        }

        Fanex.ExtAcc.BlockUI();

        $.ajax({ url: resolveClientUrl('Customer/UpdatePassword/?custId=' + $('#CustId').val() + '&password=' + $('#Password').val()), type: 'POST' })
        .done(function (data) {
            if (!data.IsSuccess) {
                $('#errMsg').css('display', 'block');
                $('#errMsg').html(data.ErrorMessage.formatErrorMessage());
            }
            else {
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
        if (Fanex.ExtAcc.Customer.CurrentLevel == Fanex.ExtAcc.Customer.Super) {
            Fanex.ExtAcc.Customer.GetSuperList();
        }
        else if (Fanex.ExtAcc.Customer.CurrentLevel == Fanex.ExtAcc.Customer.Master) {
            Fanex.ExtAcc.Customer.GetMasterList(Fanex.ExtAcc.Customer.superId, Fanex.ExtAcc.Customer.AllowDoubleComm, null, null, Fanex.ExtAcc.Customer.superSyncCasino, Fanex.ExtAcc.Customer.superSyncBingo, Fanex.ExtAcc.Customer.superSyncLiveCasino);
        }
        else if (Fanex.ExtAcc.Customer.CurrentLevel == Fanex.ExtAcc.Customer.Agent) {
            Fanex.ExtAcc.Customer.GetAgentList(Fanex.ExtAcc.Customer.masterId, Fanex.ExtAcc.Customer.AllowDoubleComm, null, null, Fanex.ExtAcc.Customer.masterSyncCasino, Fanex.ExtAcc.Customer.masterSyncBingo, Fanex.ExtAcc.Customer.masterSyncLiveCasino);
        }
        else {
            Fanex.ExtAcc.Customer.GetMemberList(Fanex.ExtAcc.Customer.agentId, Fanex.ExtAcc.Customer.AllowDoubleComm, null, null, Fanex.ExtAcc.Customer.agentSyncCasino, Fanex.ExtAcc.Customer.agentSyncBingo, Fanex.ExtAcc.Customer.agentSyncLiveCasino);
        }
    },

    ConfigMultipleDoubleComm: function (custids, allow, index) {
        Fanex.ExtAcc.BlockUI();

        if (index == 0) {
            Fanex.ExtAcc.Customer.MultipleDblCommError = '';
        }

        var len = custids.length - 1;
        var current = index;

        $.ajax({
            url: resolveClientUrl('Customer/ConfigDoubleComm'),
            dataType: 'json',
            data: {
                custid: parseInt(custids[current]),
                allow: allow
            }
        })
        .success(function (result) {
            if (result.ErrorCode && result.ErrorCode == '1') {
                Fanex.ExtAcc.UnBlockUI();
                alert(result.ErrorMessage);
                Fanex.ExtAcc.Form.Close();
            }
            else {
                if (current == len) {
                    var timeout = setTimeout(function () {
                        clearTimeout(timeout);
                        Fanex.ExtAcc.Form.Close();
                        Fanex.ExtAcc.Customer.CallBackCustomerList();
                    }, 2000);
                }
                else {
                    Fanex.ExtAcc.Customer.MultipleDblCommError += result.ErrorMessage + '<br/>';
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
            }
            else if ($('#dbl-' + custid).attr('class') == 'allowDblComm') {
                msg = 'Are you sure you want to disallow Double Commission feature for Sportsbook\'s Losing tickets on this customer? Warning, all members under this agent will be disallowed as well!';
                title = 'Disallow Double Commission';
            }
            else if ($('#dbl-' + custid).attr('class') == 'onDblComm') {
                msg = 'Are you sure you want to turn OFF Double Commission feature for Sportsbook\'s Losing tickets on this customer?';
                title = 'Turn OFF Double Commission';
            }
            else {
                msg = 'Are you sure you want to turn ON Double Commission feature for Sportsbook\'s Losing tickets on this customer?';
                title = 'Turn ON Double Commission';
            }

            Fanex.ExtAcc.Form.ConfirmBox(title, msg, 450, 170, false, function () {
                Fanex.ExtAcc.Form.Close(false);
                Fanex.ExtAcc.BlockUI();

                $.ajax({
                    url: resolveClientUrl('Customer/ConfigDoubleComm'),
                    dataType: 'json',
                    data: {
                        custid: custid,
                        allow: ($('#dbl-' + custid).attr('class') == 'disallowDblComm' || $('#dbl-' + custid).attr('class') == 'offDblComm')
                    }
                })
                .success(function (result) {
                    if (result.IsSuccess) {
                        var timeout = setTimeout(function () {
                            clearTimeout(timeout);
                            Fanex.ExtAcc.Customer.CallBackCustomerList();
                            Fanex.ExtAcc.UnBlockUI();
                        }, 2000);
                    }
                    else {
                        Fanex.ExtAcc.UnBlockUI();
                        alert(result.ErrorCode == "600" ? "This account was disabled" : result.ErrorMessage);
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
        }
        else {
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
        }
        else {
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
            if (Fanex.ExtAcc.Customer.DataSource.length == 0) {
                $('#msgNoinfo').css('display', 'block');
                $('#jqgrid').css('display', 'none');
                $('#exporttoexcel').css('display', 'none');
            }
            else {
                $('#msgNoinfo').css('display', 'none');
                $('#jqgrid').css('display', 'block');
                $('#exporttoexcel').css('display', 'block');
                $('#breadcrumbs').css('display', 'block');
                // Init Grid jqgrid
                $('#jqgrid').fanexGrid(options);
            }

            $('#editMult').attr('style', 'cursor: default !important');

            $('#editMult').attr('onclick', '');
            $('#editMult').contextMenu(false);

            Fanex.ExtAcc.UnBlockUI();
        }
        catch (e) {
            if (typeof console != 'undefined') {
                console.log(e);
            }

            Fanex.ExtAcc.UnBlockUI();
        }
    },

    SearchList: function () {
        if ($('#txtUserName').val() == '') {
            Fanex.ExtAcc.Customer.GetSuperList();
        }
        else {
            Fanex.ExtAcc.Customer.GetSearchList();
        }
    },

    GetSearchList: function () {
        Fanex.ExtAcc.BlockUI();

        var data = Fanex.ExtAcc.Customer.GetParams();

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
                Fanex.ExtAcc.Customer.custId = result.custId;

                if (result.roleId == 4) {
                    Fanex.ExtAcc.Customer.GetSuperList(rows[0].custid, result);
                }
                else if (result.roleId == 3) {
                    Fanex.ExtAcc.Customer.GetMasterList(result.srecommend, Fanex.ExtAcc.Customer.AllowDoubleComm, custId, result, Fanex.ExtAcc.Customer.superSyncCasino, Fanex.ExtAcc.Customer.superSyncBingo, Fanex.ExtAcc.Customer.superSyncLiveCasino);
                }
                else if (result.roleId == 2) {
                    Fanex.ExtAcc.Customer.GetAgentList(result.mrecommend, Fanex.ExtAcc.Customer.AllowDoubleComm, custId, result, Fanex.ExtAcc.Customer.masterSyncCasino, Fanex.ExtAcc.Customer.masterSyncBingo, Fanex.ExtAcc.Customer.masterSyncLiveCasino);
                }
                else {
                    Fanex.ExtAcc.Customer.GetMemberList(result.recommend, Fanex.ExtAcc.Customer.AllowDoubleComm, custId, result, Fanex.ExtAcc.Customer.agentSyncCasino, Fanex.ExtAcc.Customer.agentSyncBingo, Fanex.ExtAcc.Customer.agentSyncLiveCasino);
                }
            } else {
                $('#msgNoinfo').css('display', 'block');
                $('#jqgrid').css('display', 'none');
                $('#exporttoexcel').css('display', 'none');
                $('#breadcrumbs').css('display', 'none');

                Fanex.ExtAcc.UnBlockUI();
            }
        });
    },

    RenderSuperList: function (dataSource) {
        if (dataSource.srecommend) {
            Fanex.ExtAcc.Customer.superId = dataSource.srecommend;
            Fanex.ExtAcc.Customer.masterId = dataSource.mrecommend;
            Fanex.ExtAcc.Customer.agentId = dataSource.recommend;
        }

        var rows = $.parseJSON(dataSource.rows);
        var options = null;
        var bodyRows = [];

        Fanex.ExtAcc.Customer.DataSource = rows;

        for (var i = 0; i < rows.length; i++) {
            var cssRow = !rows[i].enabled ? 'rowDisabled' : rows[i].closed ? 'rowClosed' : rows[i].suspended ? 'rowSuspended' : '';

            bodyRows.push({
                Columns: [
                            { Name: 'RowNumber', Value: rows[i].RowNumber },
                            { Name: 'Check', Value: rows[i] },
                            { Name: 'username', Value: rows[i] },
                            { Name: 'Edit', Value: rows[i] },
                            { Name: 'Status', Value: rows[i] },
                            { Name: 'DoubleCommLosing', Value: rows[i] },
                            { Name: 'firstname', Value: rows[i].firstname },
                            { Name: 'lastname', Value: rows[i].lastname },
                            { Name: 'groupa', Value: rows[i].groupa },
                            { Name: 'groupb', Value: rows[i].groupb },
                            { Name: 'groupc', Value: rows[i].groupc },
                            { Name: 'groupd', Value: rows[i].groupd },
                            { Name: 'discount1x2', Value: rows[i].discount1x2 },
                            { Name: 'discountcs', Value: rows[i].discountcs },
                            { Name: 'discountnumber', Value: rows[i].discountnumber },
                            { Name: 'createdate', Value: rows[i].createdate },
                            { Name: 'lastloginip', Value: rows[i].lastloginip }
                        ],
                CssClass: cssRow
            });
        }

        var groupHeader =
                [{
                    Columns: [
                        { HeaderText: 'No.', Name: 'RowNumber', MergeRowNumber: 2 },
                        { HeaderText: '', Name: 'Check', MergeRowNumber: 2 },
                        { HeaderText: 'Username', Name: 'username', MergeRowNumber: 2 },
                        { HeaderText: 'Edit', Name: 'Edit', MergeRowNumber: 2 },
                        { HeaderText: 'Status', Name: 'Status', MergeRowNumber: 2 },
                        { HeaderText: 'Double Comm', Name: 'DoubleCommLosing', MergeRowNumber: 2 },
                        { HeaderText: 'First Name', Name: 'firstname', MergeRowNumber: 2 },
                        { HeaderText: 'Last Name', Name: 'lastname', MergeRowNumber: 2 },
                        { HeaderText: 'Commission', Name: 'groupa', MergeColNumber: 7 },
                        { HeaderText: 'Created Date', Name: 'createdate', MergeRowNumber: 2 },
                        { HeaderText: 'Login IP', Name: 'lastloginip', MergeRowNumber: 2 }
                    ]
                }]

        options = {
            columns: Fanex.ExtAcc.Customer.DataFields,
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
                currentPage: 0,
                totalItem: rows.length,
                itemsPerPage: 50
            },
            marginRight: 2,
            showDistinctRow: false,
            allowTruncate: true
        };

        Fanex.ExtAcc.Customer.Render(options);

        $('#mainGrid').css('visibility', 'visible');

        $('#breadcrumbs').empty();

        Fanex.ExtAcc.OnResize = function () {
            try {
                $("#jqgrid").fanexGrid("resizeGrid");
            }
            catch (err) {
            }
        }
    },

    RenderMasterList: function (dataSource) {
        if (dataSource.srecommend) {
            Fanex.ExtAcc.Customer.superId = dataSource.srecommend;
            Fanex.ExtAcc.Customer.masterId = dataSource.mrecommend;
            Fanex.ExtAcc.Customer.agentId = dataSource.recommend;
        }

        var rows = $.parseJSON(dataSource.rows);
        var options = null;
        var bodyRows = [];

        Fanex.ExtAcc.Customer.DataSource = rows;

        for (var i = 0; i < rows.length; i++) {
            var cssRow = !rows[i].enabled ? 'rowDisabled' : rows[i].closed ? 'rowClosed' : rows[i].suspended ? 'rowSuspended' : '';

            bodyRows.push({
                Columns: [
                        { Name: 'RowNumber', Value: rows[i].RowNumber },
                        { Name: 'Check', Value: rows[i] },
                        { Name: 'username', Value: rows[i] },
                        { Name: 'Edit', Value: rows[i] },
                        { Name: 'Status', Value: rows[i] },
                        { Name: 'DoubleCommLosing', Value: rows[i] },
                        { Name: 'firstname', Value: rows[i].firstname },
                        { Name: 'lastname', Value: rows[i].lastname },
                        { Name: 'groupa', Value: rows[i].groupa },
                        { Name: 'groupb', Value: rows[i].groupb },
                        { Name: 'groupc', Value: rows[i].groupc },
                        { Name: 'groupd', Value: rows[i].groupd },
                        { Name: 'discount1x2', Value: rows[i].discount1x2 },
                        { Name: 'discountcs', Value: rows[i].discountcs },
                        { Name: 'discountnumber', Value: rows[i].discountnumber },
                        { Name: 'createdate', Value: rows[i].createdate },
                        { Name: 'lastloginip', Value: rows[i].lastloginip }
                    ],
                CssClass: cssRow
            });
        }

        var groupHeader =
            [{
                Columns: [
                    { HeaderText: 'No.', Name: 'RowNumber', MergeRowNumber: 2 },
                    { HeaderText: '', Name: 'Check', MergeRowNumber: 2 },
                    { HeaderText: 'Username', Name: 'username', MergeRowNumber: 2 },
                    { HeaderText: 'Edit', Name: 'Edit', MergeRowNumber: 2 },
                    { HeaderText: 'Status', Name: 'Status', MergeRowNumber: 2 },
                    { HeaderText: 'Double Comm', Name: 'DoubleCommLosing', MergeRowNumber: 2 },
                    { HeaderText: 'First Name', Name: 'firstname', MergeRowNumber: 2 },
                    { HeaderText: 'Last Name', Name: 'lastname', MergeRowNumber: 2 },
                    { HeaderText: 'Commission', Name: 'groupa', MergeColNumber: 7 },
                    { HeaderText: 'Created Date', Name: 'createdate', MergeRowNumber: 2 },
                    { HeaderText: 'Login IP', Name: 'lastloginip', MergeRowNumber: 2 }
                ]
            }
            ]

        options = {
            columns: Fanex.ExtAcc.Customer.DataFields,
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
                currentPage: 0,
                totalItem: rows.length,
                itemsPerPage: 50
            },
            marginRight: 2,
            showDistinctRow: false,
            allowTruncate: true
        };

        Fanex.ExtAcc.Customer.Render(options);

        $('#mainGrid').css('visibility', 'visible');

        $('#breadcrumbs').empty();

        $('#breadcrumbs').append('<a href="' + "javascript:Fanex.ExtAcc.Customer.GetSuperList(" + Fanex.ExtAcc.Customer.directSuperId + ")" + '">' + 'Super' + '</a>');

        Fanex.ExtAcc.OnResize = function () {
            try {
                $("#jqgrid").fanexGrid("resizeGrid");
            }
            catch (err) {
            }
        }
    },

    RenderAgentList: function (dataSource) {
        if (dataSource.srecommend) {
            Fanex.ExtAcc.Customer.superId = dataSource.srecommend;
            Fanex.ExtAcc.Customer.masterId = dataSource.mrecommend;
            Fanex.ExtAcc.Customer.agentId = dataSource.recommend;
        }

        var linkSuper = '<a href="' + "javascript:Fanex.ExtAcc.Customer.GetSuperList(" + Fanex.ExtAcc.Customer.directSuperId + ")" + '">' + 'Super' + '</a>';
        var linkMaster = '<a href="' + "javascript:Fanex.ExtAcc.Customer.GetMasterList(" + Fanex.ExtAcc.Customer.superId + ', ' + Fanex.ExtAcc.Customer.AllowDoubleCommMaster + ',' + Fanex.ExtAcc.Customer.directMasterId + ', null, ' + Fanex.ExtAcc.Customer.superSyncCasino + ',' + Fanex.ExtAcc.Customer.superSyncBingo + ',' + Fanex.ExtAcc.Customer.superSyncLiveCasino + ')"' + '">' + 'Master' + '</a>';

        var rows = $.parseJSON(dataSource.rows);
        var options = null;
        var bodyRows = [];

        Fanex.ExtAcc.Customer.DataSource = rows;

        for (var i = 0; i < rows.length; i++) {
            var cssRow = !rows[i].enabled ? 'rowDisabled' : rows[i].closed ? 'rowClosed' : rows[i].suspended ? 'rowSuspended' : '';

            bodyRows.push({
                Columns: [
                        { Name: 'RowNumber', Value: rows[i].RowNumber },
                        { Name: 'Check', Value: rows[i] },
                        { Name: 'username', Value: rows[i] },
                        { Name: 'Edit', Value: rows[i] },
                        { Name: 'Status', Value: rows[i] },
                        { Name: 'DoubleCommLosing', Value: rows[i] },
                        { Name: 'firstname', Value: rows[i].firstname },
                        { Name: 'lastname', Value: rows[i].lastname },
                        { Name: 'groupa', Value: rows[i].groupa },
                        { Name: 'groupb', Value: rows[i].groupb },
                        { Name: 'groupc', Value: rows[i].groupc },
                        { Name: 'groupd', Value: rows[i].groupd },
                        { Name: 'discount1x2', Value: rows[i].discount1x2 },
                        { Name: 'discountcs', Value: rows[i].discountcs },
                        { Name: 'discountnumber', Value: rows[i].discountnumber },
                        { Name: 'createdate', Value: rows[i].createdate },
                        { Name: 'lastloginip', Value: rows[i].lastloginip }
                    ],
                CssClass: cssRow
            });
        }

        var groupHeader =
            [{
                Columns: [
                    { HeaderText: 'No.', Name: 'RowNumber', MergeRowNumber: 2 },
                    { HeaderText: '', Name: 'Check', MergeRowNumber: 2 },
                    { HeaderText: 'Username', Name: 'username', MergeRowNumber: 2 },
                    { HeaderText: 'Edit', Name: 'Edit', MergeRowNumber: 2 },
                    { HeaderText: 'Status', Name: 'Status', MergeRowNumber: 2 },
                    { HeaderText: 'Double Comm', Name: 'DoubleCommLosing', MergeRowNumber: 2 },
                    { HeaderText: 'First Name', Name: 'firstname', MergeRowNumber: 2 },
                    { HeaderText: 'Last Name', Name: 'lastname', MergeRowNumber: 2 },
                    { HeaderText: 'Commission', Name: 'groupa', MergeColNumber: 7 },
                    { HeaderText: 'Created Date', Name: 'createdate', MergeRowNumber: 2 },
                    { HeaderText: 'Login IP', Name: 'lastloginip', MergeRowNumber: 2 }
                ]
            }
            ]

        options = {
            columns: Fanex.ExtAcc.Customer.DataFields,
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
                currentPage: 0,
                totalItem: rows.length,
                itemsPerPage: 50
            },
            marginRight: 2,
            showDistinctRow: false,
            allowTruncate: true
        };

        Fanex.ExtAcc.Customer.Render(options);

        $('#mainGrid').css('visibility', 'visible');

        $('#breadcrumbs').empty();

        $('#breadcrumbs').append('<div style="float:left">' + linkMaster + '</div><div class="viewNext">&nbsp;</div><div style="float:left">' + linkSuper + '</div>');

        Fanex.ExtAcc.OnResize = function () {
            try {
                $("#jqgrid").fanexGrid("resizeGrid");
            }
            catch (err) {
            }
        }
    },

    RenderMemberList: function (dataSource) {
        if (dataSource.srecommend) {
            Fanex.ExtAcc.Customer.superId = dataSource.srecommend;
            Fanex.ExtAcc.Customer.masterId = dataSource.mrecommend;
            Fanex.ExtAcc.Customer.agentId = dataSource.recommend;
        }

        var rows = $.parseJSON(dataSource.rows);
        var options = null;
        var bodyRows = [];

        Fanex.ExtAcc.Customer.DataSource = rows;

        for (var i = 0; i < rows.length; i++) {
            var cssRow = !rows[i].enabled ? 'rowDisabled' : rows[i].closed ? 'rowClosed' : rows[i].suspended ? 'rowSuspended' : '';

            bodyRows.push({
                Columns: [
                        { Name: 'RowNumber', Value: rows[i].RowNumber },
                        { Name: 'Check', Value: rows[i] },
                        { Name: 'username', Value: rows[i] },
                        { Name: 'Edit', Value: rows[i] },
                        { Name: 'Status', Value: rows[i] },
                        { Name: 'DoubleCommLosing', Value: rows[i] },
                        { Name: 'NickName', Value: rows[i].NickName },
                        { Name: 'firstname', Value: rows[i].firstname },
                        { Name: 'lastname', Value: rows[i].lastname },
                        { Name: 'ugroup', Value: rows[i].ugroup },
                        { Name: 'discount', Value: rows[i].discount },
                        { Name: 'discount1x2', Value: rows[i].discount1x2 },
                        { Name: 'discountcs', Value: rows[i].discountcs },
                        { Name: 'discountnumber', Value: rows[i].discountnumber },
                        { Name: 'playerdiscount', Value: rows[i].playerdiscount },
                        { Name: 'playerdiscount1x2', Value: rows[i].playerdiscount1x2 },
                        { Name: 'playerdiscountcs', Value: rows[i].playerdiscountcs },
                        { Name: 'playerdiscountnumber', Value: rows[i].playerdiscountnumber },
                        { Name: 'createdate', Value: rows[i].createdate },
                        { Name: 'lastloginip', Value: rows[i].lastloginip }
                    ],
                CssClass: cssRow
            });
        }

        var groupHeader =
            [{
                Columns: [
                    { HeaderText: 'No.', Name: 'RowNumber', MergeRowNumber: 2 },
                    { HeaderText: '', Name: 'Check', MergeRowNumber: 2 },
                    { HeaderText: 'Username', Name: 'username', MergeRowNumber: 2 },
                    { HeaderText: 'Edit', Name: 'Edit', MergeRowNumber: 2 },
                    { HeaderText: 'Status', Name: 'Status', MergeRowNumber: 2 },
                    { HeaderText: 'Double Comm', Name: 'DoubleCommLosing', MergeRowNumber: 2 },
                    { HeaderText: 'Nickname', Name: 'NickName', MergeRowNumber: 2 },
                    { HeaderText: 'First Name', Name: 'firstname', MergeRowNumber: 2 },
                    { HeaderText: 'Last Name', Name: 'lastname', MergeRowNumber: 2 },
                    { HeaderText: 'User Group', Name: 'ugroup', MergeRowNumber: 2 },
                    { HeaderText: 'Agent Commission', Name: 'discount', MergeColNumber: 4 },
                    { HeaderText: 'Player Commission', Name: 'playerdiscount', MergeColNumber: 4 },
                    { HeaderText: 'Created Date', Name: 'createdate', MergeRowNumber: 2 },
                    { HeaderText: 'Login IP', Name: 'lastloginip', MergeRowNumber: 2 }
                ]
            }
            ]

        options = {
            columns: Fanex.ExtAcc.Customer.DataFields,
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
                currentPage: 0,
                totalItem: rows.length,
                itemsPerPage: 50
            },
            marginRight: 2,
            showDistinctRow: false,
            allowTruncate: true
        };

        Fanex.ExtAcc.Customer.Render(options);

        $('#mainGrid').css('visibility', 'visible');

        $('#breadcrumbs').empty();

        var linkSuper = '<a href="' + "javascript:Fanex.ExtAcc.Customer.GetSuperList(" + Fanex.ExtAcc.Customer.directSuperId + ")" + '">' + 'Super' + '</a>';
        var linkMaster = '<a href="' + "javascript:Fanex.ExtAcc.Customer.GetMasterList(" + Fanex.ExtAcc.Customer.superId + ', ' + Fanex.ExtAcc.Customer.AllowDoubleCommMaster + ',' + Fanex.ExtAcc.Customer.directMasterId + ', null, ' + Fanex.ExtAcc.Customer.superSyncCasino + ',' + Fanex.ExtAcc.Customer.superSyncBingo + ',' + Fanex.ExtAcc.Customer.superSyncLiveCasino + ')">' + 'Master' + '</a>';
        var linkAgent = '<a href="' + "javascript:Fanex.ExtAcc.Customer.GetAgentList(" + Fanex.ExtAcc.Customer.masterId + ', ' + Fanex.ExtAcc.Customer.AllowDoubleCommAgent + ',' + Fanex.ExtAcc.Customer.directAgentId + ', null, ' + Fanex.ExtAcc.Customer.masterSyncCasino + ',' + Fanex.ExtAcc.Customer.masterSyncBingo + ',' + Fanex.ExtAcc.Customer.masterSyncLiveCasino + ')">' + 'Agent' + '</a>';

        $('#breadcrumbs').append('<div style="float:left">' + linkAgent + '</div><div class="viewNext">&nbsp;</div><div style="float:left">' + linkMaster + '</div><div class="viewNext">&nbsp;</div><div style="float:left">' + linkSuper + '</div>');

        Fanex.ExtAcc.OnResize = function () {
            try {
                $("#jqgrid").fanexGrid("resizeGrid");
            }
            catch (err) {
            }
        }
    },

    EditMultiple: function (div) {
        var menuItems = {};

        Fanex.ExtAcc.Customer.isSuperEditMember = false;
        Fanex.ExtAcc.Customer.isMasterEditMember = false;
        Fanex.ExtAcc.Customer.IsFullInfo = false;

        if (Fanex.ExtAcc.Customer.CurrentLevel == Fanex.ExtAcc.Customer.Super) {
            menuItems = {
                doubleComm: { name: "Edit Multiple Double Comm", className: "iconEnableEdit",
                    callback: function (key, opt) {
                        Fanex.ExtAcc.Form.Show(resolveClientUrl('Customer/ConfigMultipleDoubleComm/' + Fanex.ExtAcc.Customer.CurrentLevel),
                            'Edit Multiple Double Commission For Losing Tickets',
                            430, 140, false, false, 1);
                    }
                }
            };
        }
        else if (Fanex.ExtAcc.Customer.CurrentLevel == Fanex.ExtAcc.Customer.Master) {
            menuItems = {
                sportbook: { name: "Edit Multiple Sportsbook", className: "iconEnableEdit", callback: function () { Fanex.ExtAcc.Customer.ShowEditMultiplePT(); } },
                racing: { name: "Edit Multiple Racing", className: "iconEnableEdit" }
            };

            if (Fanex.ExtAcc.Customer.superSyncCasino) {
                menuItems.casino = { name: "Edit Multiple Casino", className: "iconEnableEdit" };
            }

            if (Fanex.ExtAcc.Customer.superSyncBingo) {
                menuItems.bingo = { name: "Edit Multiple Bingo", className: "iconEnableEdit" };
            }

            if (!Fanex.ExtAcc.Customer.superDisableP2P) {
                menuItems.poker = { name: "Edit Multiple Poker", className: "iconEnableEdit" };
            }

            if (Fanex.ExtAcc.Customer.superSyncLiveCasino) {
                menuItems.liveCasino = { name: "Edit Multiple Live Casino", className: "iconEnableEdit", callback: function () {
                    Fanex.ExtAcc.Customer.ShowEditMultipleLiveCasino();
                }
                };
            }

            menuItems.virtualSport = { name: "Edit Multiple Virtual Sports", className: "iconEnableEdit" };

            if (Fanex.ExtAcc.Customer.AllowDoubleCommMaster) {
                menuItems.doubleComm = { name: "Edit Multiple Double Comm", className: "iconEnableEdit",
                    callback: function (key, opt) {
                        Fanex.ExtAcc.Form.Show(resolveClientUrl('Customer/ConfigMultipleDoubleComm/' + Fanex.ExtAcc.Customer.CurrentLevel),
                                'Edit Multiple Double Commission For Losing Tickets',
                                430, 140, false, false, 1);
                    }
                };
            }
        } else if (Fanex.ExtAcc.Customer.CurrentLevel == Fanex.ExtAcc.Customer.Agent) {
            menuItems = {
                sportbook: { name: "Edit Multiple Sportsbook", className: "iconEnableEdit", callback: function () { Fanex.ExtAcc.Customer.ShowEditMultiplePT(); } },
                racing: { name: "Edit Multiple Racing", className: "iconEnableEdit" }
            };

            if (Fanex.ExtAcc.Customer.masterSyncCasino) {
                menuItems.casino = { name: "Edit Multiple Casino", className: "iconEnableEdit" };
            }

            if (Fanex.ExtAcc.Customer.masterSyncBingo) {
                menuItems.bingo = { name: "Edit Multiple Bingo", className: "iconEnableEdit" };
            }

            if (!Fanex.ExtAcc.Customer.masterDisableP2P) {
                menuItems.poker = { name: "Edit Multiple Poker", className: "iconEnableEdit" };
            }

            if (Fanex.ExtAcc.Customer.masterSyncLiveCasino) {
                menuItems.liveCasino = { name: "Edit Multiple Live Casino", className: "iconEnableEdit", callback: function () {
                    Fanex.ExtAcc.Customer.ShowEditMultipleLiveCasino();
                }
                };
            }

            menuItems.virtualSport = { name: "Edit Multiple Virtual Sports", className: "iconEnableEdit" };

            if (Fanex.ExtAcc.Customer.AllowDoubleCommAgent) {
                menuItems.doubleComm = { name: "Edit Multiple Double Comm", className: "iconEnableEdit",
                    callback: function (key, opt) {
                        Fanex.ExtAcc.Form.Show(resolveClientUrl('Customer/ConfigMultipleDoubleComm/' + Fanex.ExtAcc.Customer.CurrentLevel),
                                'Edit Multiple Double Commission For Losing Tickets',
                                430, 140, false, false, 1);
                    }
                };
            }
        }
        else {
            menuItems = {
                sportbook: { name: "Edit Multiple Sportsbook", className: "iconEnableEdit", callback: function () { Fanex.ExtAcc.Customer.ShowEditMultiplePT(); } },
                superEditSportbook: { name: "Supet Edit Multiple Sportsbook", className: "iconEnableEdit", callback: function () { Fanex.ExtAcc.Customer.isSuperEditMember = true; Fanex.ExtAcc.Customer.isMasterEditMember = false; Fanex.ExtAcc.Customer.ShowSuperMasterEditMultiplePT(); } },
                masterEditSportbook: { name: "Master Edit Multiple Sportsbook", className: "iconEnableEdit", callback: function () { Fanex.ExtAcc.Customer.isSuperEditMember = false; Fanex.ExtAcc.Customer.isMasterEditMember = true; Fanex.ExtAcc.Customer.ShowSuperMasterEditMultiplePT(); } },
                racing: { name: "Edit Multiple Racing", className: "iconEnableEdit" }
            };

            if (Fanex.ExtAcc.Customer.agentSyncCasino) {
                menuItems.casino = { name: "Edit Multiple Casino", className: "iconEnableEdit" };
            }

            if (Fanex.ExtAcc.Customer.agentSyncBingo) {
                menuItems.bingo = { name: "Edit Multiple Bingo", className: "iconEnableEdit" };
            }

            if (!Fanex.ExtAcc.Customer.agentDisableP2P) {
                menuItems.poker = { name: "Edit Multiple Poker", className: "iconEnableEdit" };
            }

            if (Fanex.ExtAcc.Customer.agentSyncLiveCasino) {
                menuItems.liveCasino = { name: "Edit Multiple Live Casino", className: "iconEnableEdit", callback: function () {
                    Fanex.ExtAcc.Customer.ShowEditMultipleLiveCasino();
                }
                };
                menuItems.liveCasinoSuper = { name: "Super Edit Multiple Live Casino", className: "iconEnableEdit", callback: function () {
                    Fanex.ExtAcc.Customer.isSuperEditMember = true;
                    Fanex.ExtAcc.Customer.isMasterEditMember = false;

                    Fanex.ExtAcc.Customer.ShowEditMultipleLiveCasino();
                }
                };
                menuItems.liveCasinoMaster = { name: "Master Edit Multiple Live Casino", className: "iconEnableEdit", callback: function () {
                    Fanex.ExtAcc.Customer.isSuperEditMember = false;
                    Fanex.ExtAcc.Customer.isMasterEditMember = true;

                    Fanex.ExtAcc.Customer.ShowEditMultipleLiveCasino();
                }
                };
            }

            menuItems.virtualSport = { name: "Edit Multiple Virtual Sports", className: "iconEnableEdit" };

            if (Fanex.ExtAcc.Customer.AllowDoubleComm) {
                menuItems.doubleComm = { name: "Edit Multiple Double Comm", className: "iconEnableEdit",
                    callback: function (key, opt) {
                        Fanex.ExtAcc.Form.Show(resolveClientUrl('Customer/ConfigMultipleDoubleComm/' + Fanex.ExtAcc.Customer.CurrentLevel),
                                'Edit Multiple Double Commission For Losing Tickets',
                                430, 140, false, false, 1);
                    }
                };
            }
        }

        $.contextMenu('destroy');

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

        Fanex.ExtAcc.Customer.ConfigMultipleDoubleComm(ret.Ids, allow, 0);
    },

    ShowEditMultiplePT: function () {
        var ret = this.IdsAndNames();

        if (!Fanex.ExtAcc.Customer.IsMultiple)
            Fanex.ExtAcc.Form.Show(resolveClientUrl('Sportsbook/EditPT'), 'Edit PT Setting ' + '<font color="#6dcff6">' + ret.Names[0] + '</font>', 888, 700, false, false, 1);
        else
            Fanex.ExtAcc.Form.Show(resolveClientUrl('Sportsbook/EditMultiplePT'), 'Edit Multiple Sportsbook Setting', 894, 700, false, false, 1);
    },

    ShowEditMultipleLiveCasino: function () {
        var ret = this.IdsAndNames();

        var actionName = '';
        var levelText = '';

        if (Fanex.ExtAcc.Customer.CurrentLevel == Fanex.ExtAcc.Customer.Master) {
            actionName = 'EditMaster';
        }
        else if (Fanex.ExtAcc.Customer.CurrentLevel == Fanex.ExtAcc.Customer.Agent) {
            actionName = 'EditAgent';
        }
        else {
            if (Fanex.ExtAcc.Customer.isSuperEditMember) {
                actionName = 'SuperEditMember';
                levelText = 'Super ';
            }
            else if (Fanex.ExtAcc.Customer.isMasterEditMember) {
                actionName = 'MasterEditMember';
                levelText = 'Master ';
            }
            else {
                actionName = 'EditMember';
            }
        }

        if (!Fanex.ExtAcc.Customer.IsMultiple)
            Fanex.ExtAcc.Form.Show(resolveClientUrl('LiveCasino/' + actionName), levelText + 'Edit Live Casino Setting ' + '<font color="#6dcff6">' + ret.Names[0] + '</font>', 888, 700, false, false, 1);
        else
            Fanex.ExtAcc.Form.Show(resolveClientUrl('LiveCasino/' + actionName), levelText + 'Edit Multiple Live Casino Setting', 888, 700, false, false, 1);
    },

    ShowSuperMasterEditMultiplePT: function () {
        var ret = this.IdsAndNames();
        var levelText = '';

        if (Fanex.ExtAcc.Customer.isSuperEditMember) {
            levelText = 'Super ';
        }
        else if (Fanex.ExtAcc.Customer.isMasterEditMember) {
            levelText = 'Master ';
        }

        if (!Fanex.ExtAcc.Customer.IsMultiple) {
            Fanex.ExtAcc.Form.Show(resolveClientUrl('Sportsbook/EditPT'), levelText + 'Edit PT Setting ' + '<font color="#6dcff6">' + ret.Names[0] + '</font>', 888, 600, false, false, 1);
        }
        else {
            Fanex.ExtAcc.Form.Show(resolveClientUrl('Sportsbook/SuperMasterEditMultiplePTMember'), levelText + 'Edit Multiple Sportsbook Setting', 894, 600, false, false, 1);
        }
    },

    ToogleUsernames: function () {
        if ($('#arrowUpDown').hasClass('collapse')) {
            $('#arrowUpDown').attr('class', 'expand');
            $('#names').html(Fanex.ExtAcc.Customer.Usernames.substring(0, Fanex.ExtAcc.Customer.Usernames.length - 2));
            $('#names').attr('title', '');
        }
        else {
            $('#arrowUpDown').attr('class', 'collapse');
            $('#names').html(Fanex.ExtAcc.Customer.Usernames.substring(0, 98) + '...');
            $('#names').attr('title', Fanex.ExtAcc.Customer.Usernames.substring(0, Fanex.ExtAcc.Customer.Usernames.length - 2));
        }
    },

    IdsAndNames: function () {
        var custids = [];
        var divs = $('div');

        Fanex.ExtAcc.Customer.Usernames = '';
        Fanex.ExtAcc.Customer.CustIds = '';

        for (var i = 0; i < divs.length; i++) {
            if (divs[i].className == 'menuActive' && !custids.contains(divs[i].id)) {
                custids.push(divs[i].id);

                Fanex.ExtAcc.Customer.CustIds += divs[i].id + ',';
            }
        }

        Fanex.ExtAcc.Customer.UserId = parseInt(custids[0]);

        if (custids.length == 1) {
            for (var i = 0; i < Fanex.ExtAcc.Customer.DataSource.length; i++) {
                if (Fanex.ExtAcc.Customer.DataSource[i].custid == parseInt(custids[0])) {
                    Fanex.ExtAcc.Customer.Usernames = Fanex.ExtAcc.Customer.DataSource[i].username + ', ';

                    break;
                }
            }

            Fanex.ExtAcc.Customer.IsMultiple = false;
        }
        else {
            for (var i = 0; i < Fanex.ExtAcc.Customer.DataSource.length; i++) {
                var row = Fanex.ExtAcc.Customer.DataSource[i];

                for (var j = 0; j < custids.length; j++) {
                    if (custids[j] == row.custid) {
                        Fanex.ExtAcc.Customer.Usernames += row.username + ', ';

                        continue;
                    }
                }
            }

            Fanex.ExtAcc.Customer.IsMultiple = true;
        }

        return { Ids: this.Ids(), Names: this.Names() };
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
        Fanex.ExtAcc.Customer.IsMultiple = false;
        Fanex.ExtAcc.Customer.isSuperEditMember = false;
        Fanex.ExtAcc.Customer.isMasterEditMember = false;
        Fanex.ExtAcc.Customer.isSuperEditAgent = false;
        Fanex.ExtAcc.Customer.IsFullInfo = false;
    },

    CustomerInfo: function () {
        var uplineId = 0;
        var uplineRoleId = 0;

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

        return { id: this.UserId, roleId: this.CurrentLevel, uplineId: uplineId, uplineRoleId: uplineRoleId };
    }
}