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

    BlockedUI: false,

    URL: '',
    MultipleDblCommError: '',

    $this: null,

    Init: function () {
        $this = this;

        //Prevent loading group name
        var timeout = setInterval(function () {
            if ($.trim($('#selectedGroupName').html()) != '') {
                $this.Search(true);
                clearInterval(timeout);
            }
        }, 100);

        $('#exporttoexcel').click(function () {
            var data = Fanex.ExtAcc.Customer.GetParams();

            var params = "groupName=" + data.groupName
                    + "&enabled=" + data.enabled + "&closed=" + data.closed + "&suspended=" + data.suspended
                    + "&sRecommend=" + $this.superId + "&mRecommend=" + $this.masterId + "&recommend=" + $this.agentId + "&custId="
                    + "&currentLevel=" + $this.CurrentLevel + "&username=" + data.username
            document.location.href = resolveClientUrl("Customer/GetExportToExcel?") + params;
        });

        $('#txtUserName').keypress(function (e) {
            if (typeof e == 'undefined') e = window.event;
            var key = (typeof e.keyCode == 'undefined') ? e.which : e.keyCode;
            if (key == 13) {
                $this.Search(false);

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
            enabled = false;
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

        var data = {
            srecommend: $this.superId,
            mrecommend: $this.masterId,
            recommend: $this.agentId,
            custId: null,
            groupName: $('#selectedGroupName').html().trim() == '' ? 'ACCDDD' : $('#selectedGroupName').html(),
            enabled: enabled,
            closed: closed,
            suspended: suspended,
            username: $('#txtUserName').val()
        };

        return data;
    },

    Search: function (isFirstLoad) {
        if (isFirstLoad) {
            this.GetSuperList();
        }
        else {
            this.SearchList();
        }
    },

    EscapeHtml: function (string) {
        var result = '';
        if (string.length < 1) {
            return result;
        }

        for (var i = 0; i < string.length; i++) {
            if (string.charAt(i) == "&" && string.length - i - 1 >= 4 && string.substr(i, 4) != "&amp;") {
                result = result + "&amp;";
            } else if (string.charAt(i) == "<") {
                result = result + "&lt;";
            } else if (string.charAt(i) == ">") {
                result = result + "&gt;";
            } else {
                result = result + string.charAt(i);
            }
        }

        return result;
    },

    ShowEditSuper: function (custId, title) {
        Fanex.ExtAcc.Customer.UserId = custId;
        Fanex.ExtAcc.Customer.TitleEditInfo = title;

        $.contextMenu({
            selector: '.editInfo',
            items: {
                editInfo: { name: "Edit Info", height: 25, className: "iconEnableEdit editInfoBg", callback: function (key, opt) { Fanex.ExtAcc.Form.Show(resolveClientUrl('Customer/EditInfo/?custId=' + Fanex.ExtAcc.Customer.UserId), titles[0] + ': <font color="#6dcff6">' + Fanex.ExtAcc.Customer.TitleEditInfo + '</font>', 410, 300, false); } },
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

        itemsMaster = {
            sportbook: { name: "Sportsbook", className: "iconEnableEdit" },
            minPTSportbook: { name: "Min PT", className: "iconEnableEdit" },
            betSetting: { name: "Bet Setting", className: "iconEnableEdit" },
            commission: { name: "Commission", className: "iconEnableEdit" },
            separator: "-----",
            racing: { name: "Racing", className: "iconEnableEdit" },
            minPTRacing: { name: "Min PT", className: "iconEnableEdit" },
            separator1: "-----"
        };

        if (superSyncCasino) {
            if (syncCasino) {
                if (disableCasino) {
                    itemsMaster.casino = { name: "Casino", className: "iconDisableEdit" };
                }
                else {
                    itemsMaster.casino = { name: "Casino", className: "iconEnableEdit" };
                }
            }
            else {
                itemsMaster.casino = { name: "Casino", className: "iconUnsyncEdit" };
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
                    itemsMaster.liveCasino = { name: "Live Casino", className: "iconDisableEdit" };
                }
                else {
                    itemsMaster.liveCasino = { name: "Live Casino", className: "iconEnableEdit" };
                }
            }
            else {
                itemsMaster.liveCasino = { name: "Live Casino", className: "iconUnsyncEdit" };
            }
        }

        if (disableVirtualSport) {
            itemsMaster.virtualSport = { name: "Virtual Sport", className: "iconDisableEdit" };
        }
        else {
            itemsMaster.virtualSport = { name: "Virtual Sport", className: "iconEnableEdit" };
        }

        itemsMaster.separator2 = "-----";
        itemsMaster.editInfo = { name: "Edit Info", className: "iconEnableEdit editInfoBg", callback: function (key, opt) { Fanex.ExtAcc.Form.Show(resolveClientUrl('Customer/EditInfo/?custId=' + Fanex.ExtAcc.Customer.UserId), titles[0] + ': <font color="#6dcff6">' + Fanex.ExtAcc.Customer.TitleEditInfo + '</font>', 410, 300, false); } };
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

        var itemsAgent = {
            sportbook: { name: "Sportsbook", className: "iconEnableEdit" },
            minPT: { name: "Min PT", className: "iconEnableEdit" },
            betSetting: { name: "Bet Setting", className: "iconEnableEdit" },
            commission: { name: "Commission", className: "iconEnableEdit" },
            separator: "-----",
            racing: { name: "Racing", className: "iconEnableEdit" },
            minPTRacing: { name: "Min PT", className: "iconEnableEdit" },
            separator1: "-----"
        };

        if (masterSyncCasino) {
            if (syncCasino) {
                if (disableCasino) {
                    itemsAgent.casino = { name: "Casino", className: "iconDisableEdit" };
                }
                else {
                    itemsAgent.casino = { name: "Casino", className: "iconEnableEdit" };
                }
            }
            else {
                itemsAgent.casino = { name: "Casino", className: "iconUnsyncEdit" };
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
                    itemsAgent.liveCasino = { name: "Live Casino", className: "iconDisableEdit" };
                }
                else {
                    itemsAgent.liveCasino = { name: "Live Casino", className: "iconEnableEdit" };
                }
            }
            else {
                itemsAgent.liveCasino = { name: "Live Casino", className: "iconUnsyncEdit" };
            }
        }

        if (disableVirtualSport) {
            itemsAgent.virtualSport = { name: "Virtual Sport", className: "iconDisableEdit" };
        }
        else {
            itemsAgent.virtualSport = { name: "Virtual Sport", className: "iconEnableEdit" };
        }

        itemsAgent.separator2 = "-----";
        itemsAgent.editInfo = { name: "Edit Info", className: "iconEnableEdit editInfoBg", callback: function (key, opt) { Fanex.ExtAcc.Form.Show(resolveClientUrl('Customer/EditInfo/?custId=' + Fanex.ExtAcc.Customer.UserId), titles[0] + ': <font color="#6dcff6">' + Fanex.ExtAcc.Customer.TitleEditInfo + '</font>', 410, 300, false); } };
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

        var itemsMember = {
            sportbook: { name: "Sportsbook", className: "iconEnableEdit" },
            minPT: { name: "Min PT", className: "iconEnableEdit" },
            betSetting: { name: "Bet Setting", className: "iconEnableEdit" },
            commission: { name: "Commission", className: "iconEnableEdit" },
            separator: "-----",
            racing: { name: "Racing", className: "iconEnableEdit" },
            separator1: "-----"
        };

        if (agentSyncCasino) {
            if (syncCasino) {
                if (disableCasino) {
                    itemsMember.casino = { name: "Casino", className: "iconDisableEdit" };
                }
                else {
                    itemsMember.casino = { name: "Casino", className: "iconEnableEdit" };
                }
            }
            else {
                itemsMember.casino = { name: "Casino", className: "iconUnsyncEdit" };
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
                    itemsMember.liveCasino = { name: "Live Casino", className: "iconDisableEdit" };
                }
                else {
                    itemsMember.liveCasino = { name: "Live Casino", className: "iconEnableEdit" };
                }
            }
            else {
                itemsMember.liveCasino = { name: "Live Casino", className: "iconUnsyncEdit" };
            }
        }

        if (disableVirtualSport) {
            itemsMember.virtualSport = { name: "Virtual Sport", className: "iconDisableEdit" };
        }
        else {
            itemsMember.virtualSport = { name: "Virtual Sport", className: "iconEnableEdit" };
        }

        itemsMember.separator2 = "-----";
        itemsMember.editInfo = { name: "Edit Info", className: "iconEnableEdit editInfoBg", callback: function (key, opt) { Fanex.ExtAcc.Form.Show(resolveClientUrl('Customer/EditInfo/?custId=' + Fanex.ExtAcc.Customer.UserId), titles[0] + ': <font color="#6dcff6">' + Fanex.ExtAcc.Customer.TitleEditInfo + '</font>', 410, 300, false); } };
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
        $('#FirstName').val('');
        $('#LastName').val('');
        $('#HomePhone').val('');
        $('#MobilePhone').val('');
        $('#Email').val('');
        $('#Fax').val('');
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
            custId: $('#CustId').val(),
            firstName: Fanex.ExtAcc.Customer.EscapeHtml(firstName),
            lastName: Fanex.ExtAcc.Customer.EscapeHtml(lastName),
            homePhone: Fanex.ExtAcc.Customer.EscapeHtml(homePhone),
            mobilePhone: Fanex.ExtAcc.Customer.EscapeHtml(mobilePhone),
            email: Fanex.ExtAcc.Customer.EscapeHtml(email),
            fax: Fanex.ExtAcc.Customer.EscapeHtml(fax)
        };

        Fanex.ExtAcc.Form.Close(false);
        $this.BlockUI();

        $.ajax({ url: resolveClientUrl(editUrl), data: data, type: 'POST' })
        .done(function (data) {
            if (!data.IsSuccess) {
                alert(data.ErrorMessage);
                $.unblockUI(document);
            }
            else {
                var timeout = setTimeout(function () {
                    clearTimeout(timeout);

                    if ($this.CurrentLevel == $this.Super) {
                        $this.GetSuperList();
                    }
                    else if ($this.CurrentLevel == $this.Master) {
                        Fanex.ExtAcc.Customer.GetMasterList($this.superId, $this.AllowDoubleComm, null, null, $this.superSyncCasino, $this.superSyncBingo, $this.superSyncLiveCasino);
                    }
                    else if ($this.CurrentLevel == $this.Agent) {
                        Fanex.ExtAcc.Customer.GetAgentList($this.masterId, $this.AllowDoubleComm, null, null, $this.masterSyncCasino, $this.masterSyncBingo, $this.masterSyncLiveCasino);
                    }
                    else {
                        Fanex.ExtAcc.Customer.GetMemberList($this.agentId, $this.AllowDoubleComm, null, null, $this.agentSyncCasino, $this.agentSyncBingo, $this.agentSyncLiveCasino);
                    }

                    $.unblockUI(document);
                }, 2000);
            }
        })
        .error(function (data) {
            if (typeof console != 'undefined') console.log(data.responseText);
            $.unblockUI(document);
        });
    },

    GetSuperList: function (custId, dataSource) {
        $this.BlockUI();

        $this.CurrentLevel = $this.Super;
        $this.AllowDoubleComm = true;

        var url = resolveClientUrl('Customer/SuperListPost');

        var data = Fanex.ExtAcc.Customer.GetParams();
        data.custId = custId == 'undefined' ? null : custId;

        if (custId == undefined) {
            $this.directSuperId = null;
            $this.directMasterId = null;
            $this.directAgentId = null;
        }

        $this.DataFields = [
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
                            $this.superDisableP2P = cell.Value.DisableP2P;

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
                            return cell.Value.closed ? 'Closed' : (cell.Value.suspended ? 'Suspended' : (!cell.Value.enabled ? 'Disabled' : 'Open'));
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
                    { HeaderText: 'Created Date', Name: 'createdate', CellAlign: 'center', Width: 110,
                        CustomCellRender: function (cell) {
                            if (cell.Value) {
                                return '<div class="formatDate">' + new Date(cell.Value.replace('T', ' ')).toLocaleString('en-US') + '</div>';
                            }

                            return '<div class="formatDate">' + new Date().toLocaleString('en-US') + '</div>';
                        }
                    },
                    { HeaderText: 'Login IP', Name: 'lastloginip', CellAlign: 'center', Width: 100,
                        CustomCellRender: function (cell) {
                            return '<div><a href="javascript:Fanex.ExtAcc.Customer.ShowIpInfo(\'' + cell.Value + '\')">' + cell.Value + '</a></div>';
                        }
                    }
                ];

        if (dataSource) {
            $this.RenderSuperList(dataSource);
        }
        else {
            $.ajax({
                url: url,
                data: data,
                dataType: 'json'
            })
            .success(function (result) {
                $this.RenderSuperList(result);
            })
            .error(function (xhr, status, err) {
                $.unblockUI(document);
            });
        }
    },

    GetMasterList: function (srecommend, allow, custId, dataSource, superSyncCasino, superSyncBingo, superSyncLiveCasino) {
        $this.BlockUI();

        $this.superId = srecommend;
        $this.CurrentLevel = $this.Master;
        $this.AllowDoubleComm = allow;
        $this.AllowDoubleCommMaster = allow;

        $this.superSyncCasino = superSyncCasino;
        $this.superSyncBingo = superSyncBingo;
        $this.superSyncLiveCasino = superSyncLiveCasino;

        var url = resolveClientUrl('Customer/MasterListPost');
        var data = Fanex.ExtAcc.Customer.GetParams();
        data.custId = custId == 'undefined' ? null : custId;

        if (custId == undefined) {
            $this.directSuperId = null;
            $this.directMasterId = null;
            $this.directAgentId = null;
        }

        $this.DataFields = [
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
                            $this.masterDisableP2P = cell.Value.DisableP2P;

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
                            return cell.Value.closed ? 'Closed' : (cell.Value.suspended ? 'Suspended' : (!cell.Value.enabled ? 'Disabled' : 'Open'));
                        }
                    },
                    { HeaderText: 'Double Comm', Name: 'DoubleCommLosing', Width: 80,
                        CustomCellRender: function (cell) {
                            var css = '';

                            if (!$this.AllowDoubleComm) {
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
                    { HeaderText: 'Created Date', Name: 'createdate', CellAlign: 'center', Width: 110,
                        CustomCellRender: function (cell) {
                            if (cell.Value) {
                                return '<div class="formatDate">' + new Date(cell.Value.replace('T', ' ')).toLocaleString('en-US') + '</div>';
                            }

                            return '<div class="formatDate">' + new Date().toLocaleString('en-US') + '</div>';
                        }
                    },
                    { HeaderText: 'Login IP', Name: 'lastloginip', CellAlign: 'center', Width: 100,
                        CustomCellRender: function (cell) {
                            return '<div><a href="javascript:Fanex.ExtAcc.Customer.ShowIpInfo(\'' + cell.Value + '\')">' + cell.Value + '</a></div>';
                        }
                    }
                ];

        if (dataSource) {
            $this.RenderMasterList(dataSource);
        }
        else {
            $.ajax({
                url: url,
                data: data,
                dataType: 'json'
            })
            .success(function (result) {
                $this.RenderMasterList(result);
            })
            .error(function (xhr, status, err) {
                $.unblockUI(document);
            });
        }
    },

    GetAgentList: function (mrecommend, allow, custId, dataSource, masterSyncCasino, masterSyncBingo, masterSyncLiveCasino) {
        $this.BlockUI();

        $this.masterId = mrecommend;
        $this.CurrentLevel = $this.Agent;
        $this.AllowDoubleComm = allow;
        $this.AllowDoubleCommAgent = allow;

        $this.masterSyncCasino = masterSyncCasino;
        $this.masterSyncBingo = masterSyncBingo;
        $this.masterSyncLiveCasino = masterSyncLiveCasino;

        var url = resolveClientUrl('Customer/AgentListPost');
        var data = Fanex.ExtAcc.Customer.GetParams();
        data.custId = custId == 'undefined' ? null : custId;

        if (custId == undefined) {
            $this.directSuperId = null;
            $this.directMasterId = null;
            $this.directAgentId = null;
        }

        $this.DataFields = [
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
                            $this.agentDisableP2P = cell.Value.DisableP2P;

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
                            return cell.Value.closed ? 'Closed' : (cell.Value.suspended ? 'Suspended' : (!cell.Value.enabled ? 'Disabled' : 'Open'));
                        }
                    },
                    { HeaderText: 'Double Comm', Name: 'DoubleCommLosing', Width: 80,
                        CustomCellRender: function (cell) {
                            var css = '';

                            if (!$this.AllowDoubleComm) {
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
                    { HeaderText: 'Created Date', Name: 'createdate', CellAlign: 'center', Width: 110,
                        CustomCellRender: function (cell) {
                            if (cell.Value) {
                                return '<div class="formatDate">' + new Date(cell.Value.replace('T', ' ')).toLocaleString('en-US') + '</div>';
                            }

                            return '<div class="formatDate">' + new Date().toLocaleString('en-US') + '</div>';
                        }
                    },
                    { HeaderText: 'Login IP', Name: 'lastloginip', CellAlign: 'center', Width: 100,
                        CustomCellRender: function (cell) {
                            return '<div><a href="javascript:Fanex.ExtAcc.Customer.ShowIpInfo(\'' + cell.Value + '\')">' + cell.Value + '</a></div>';
                        }
                    }
                ];

        if (dataSource) {
            $this.RenderAgentList(dataSource);
        }
        else {
            $.ajax({
                url: url,
                data: data,
                dataType: 'json'
            })
            .success(function (result) {
                $this.RenderAgentList(result);
            })
            .error(function (xhr, status, err) {
                $.unblockUI(document);
            });
        }
    },

    GetMemberList: function (recommend, allow, custId, dataSource, agentSyncCasino, agentSyncBingo, agentSyncLiveCasino) {
        $this.BlockUI();

        $this.agentId = recommend;
        $this.CurrentLevel = $this.Member;
        $this.AllowDoubleComm = allow;

        $this.agentSyncCasino = agentSyncCasino;
        $this.agentSyncBingo = agentSyncBingo;
        $this.agentSyncLiveCasino = agentSyncLiveCasino;

        var url = resolveClientUrl('Customer/MemberListPost');

        var data = Fanex.ExtAcc.Customer.GetParams();
        data.custId = custId == 'undefined' ? null : custId;

        if (custId == undefined) {
            $this.directSuperId = null;
            $this.directMasterId = null;
            $this.directAgentId = null;
        }

        $this.DataFields = [
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
                            return cell.Value.closed ? 'Closed' : (cell.Value.suspended ? 'Suspended' : (!cell.Value.enabled ? 'Disabled' : 'Open'));
                        }
                    },
                    { HeaderText: 'Double Comm', Name: 'DoubleCommLosing', Width: 57, CellAlign: 'center',
                        CustomCellRender: function (cell) {
                            var css = '';

                            if (!$this.AllowDoubleComm) {
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
                    { HeaderText: 'Created Date', Name: 'createdate', CellAlign: 'center', Width: 110,
                        CustomCellRender: function (cell) {
                            if (cell.Value) {
                                return '<div class="formatDate">' + new Date(cell.Value.replace('T', ' ')).toLocaleString('en-US') + '</div>';
                            }

                            return '<div class="formatDate">' + new Date().toLocaleString('en-US') + '</div>';
                        }
                    },
                    { HeaderText: 'Login IP', Name: 'lastloginip', CellAlign: 'center', Width: 100,
                        CustomCellRender: function (cell) {
                            return '<div><a href="javascript:Fanex.ExtAcc.Customer.ShowIpInfo(\'' + cell.Value + '\')">' + cell.Value + '</a></div>';
                        }
                    }
                ];

        if (dataSource) {
            $this.RenderMemberList(dataSource);
        }
        else {
            $.ajax({
                url: url,
                data: data,
                dataType: 'json'
            })
            .success(function (result) {
                $this.RenderMemberList(result);
            })
            .error(function (xhr, status, err) {
                $.unblockUI(document);
            });
        }
    },

    ResetPwd: function () {
        if (!Validators.validateInput($('#Password').val())) {
            $('#Password').focus();
            Validators.showDown($('#Password').get(0), 'This field is required', 0, 10, 41, -10, 15);
            return false;
        }

        if (!Validators.validateInput($('#confirmPwd').val())) {
            $('#confirmPwd').focus();
            Validators.show($('#confirmPwd').get(0), 'This field is required');
            return false;
        }

        if ($('#confirmPwd').val() !== $('#Password').val()) {
            $('#confirmPwd').focus();
            Validators.show($('#confirmPwd').get(0), 'Confirm password is not matched');
            return false;
        }

        $this.BlockUI();

        $.ajax({ url: resolveClientUrl('Customer/UpdatePassword/?custId=' + $('#CustId').val() + '&password=' + $('#Password').val()), type: 'POST' })
        .done(function (data) {
            if (!data.IsSuccess) {
                $('#errMsg').css('display', 'block');
                $('#errMsg').html(data.ErrorMessage);
            }
            else {
                Fanex.ExtAcc.Form.Close(false);
            }

            $.unblockUI(document);
        })
        .error(function (data) {
            if (typeof console != 'undefined') console.log(data.responseText);
            $.unblockUI(document);
        });
    },

    ShowIpInfo: function (ip) {
        $this.BlockUI();

        Fanex.ExtAcc.Form.Show(resolveClientUrl('Customer/ViewIP?ip=' + ip), 'IP Information', 450, 230, false,
            function () {
                $.unblockUI(document);
            },
            2);
    },

    ConfigMultipleDoubleComm: function (custids, allow, index) {
        $this.BlockUI();

        if (index == 0) {
            $this.MultipleDblCommError = '';
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
            if (current == len) {
                var timeout = setTimeout(function () {
                    clearTimeout(timeout);

                    if ($this.CurrentLevel == $this.Super) {
                        $this.GetSuperList();
                    }
                    else if ($this.CurrentLevel == $this.Master) {
                        $this.GetMasterList($this.superId, $this.AllowDoubleComm, null, null, $this.superSyncCasino, $this.superSyncBingo, $this.superSyncLiveCasino);
                    }
                    else if ($this.CurrentLevel == $this.Agent) {
                        $this.GetAgentList($this.masterId, $this.AllowDoubleComm, null, null, $this.masterSyncCasino, $this.masterSyncBingo, $this.masterSyncLiveCasino);
                    }
                    else {
                        $this.GetMemberList($this.agentId, $this.AllowDoubleComm, null, null, $this.agentSyncCasino, $this.agentSyncBingo, $this.agentSyncLiveCasino);
                    }
                }, 2000);
            }
            else {
                $this.MultipleDblCommError += result.ErrorMessage + '<br/>';
            }

            current++;

            if (current <= len) {
                $this.ConfigMultipleDoubleComm(custids, allow, current);
            }
        })
        .error(function (xhr, status, err) {
            if (typeof console != 'undefined') {
                console.log(err);
            }

            $this.MultipleDblCommError += err + '<br/>';

            current++;

            if (current <= len) {
                $this.ConfigMultipleDoubleComm(custids, allow, current);
            }
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
                $this.BlockUI();

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

                            if ($this.CurrentLevel == $this.Super) {
                                $this.GetSuperList();
                            }
                            else if ($this.CurrentLevel == $this.Master) {
                                Fanex.ExtAcc.Customer.GetMasterList($this.superId, $this.AllowDoubleComm, null, null, $this.superSyncCasino, $this.superSyncBingo, $this.superSyncLiveCasino);
                            }
                            else if ($this.CurrentLevel == $this.Agent) {
                                Fanex.ExtAcc.Customer.GetAgentList($this.masterId, $this.AllowDoubleComm, null, null, $this.masterSyncCasino, $this.masterSyncBingo, $this.masterSyncLiveCasino);
                            }
                            else {
                                Fanex.ExtAcc.Customer.GetMemberList($this.agentId, $this.AllowDoubleComm, null, null, $this.agentSyncCasino, $this.agentSyncBingo, $this.agentSyncLiveCasino);
                            }
                        }, 2000);
                    }
                    else {
                        $.unblockUI(document);
                        alert(result.ErrorMessage);
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
                // Init Grid jqgrid
                $('#jqgrid').fanexGrid(options);
            }

            $('#editMult').attr('style', 'cursor: default !important');

            $('#editMult').attr('onclick', '');
            $('#editMult').contextMenu(false);

            $.unblockUI(document);
        }
        catch (e) {
            if (typeof console != 'undefined') {
                console.log(e);
            }

            $.unblockUI(document);
        }
    },

    SearchList: function () {
        if ($('#txtUserName').val() == '') {
            $this.GetSuperList();
        }
        else {
            $this.GetSearchList();
        }
    },

    BlockUI: function () {
        if (!$this.BlockedUI) {
            $.blockUI({
                message: '<img src="../../Images/Loading.gif" />',
                onBlock: function () {
                    $this.BlockedUI = true;
                },
                onUnblock: function () {
                    $this.BlockedUI = false;
                }
            });
        }
    },

    GetSearchList: function () {
        $this.BlockUI();

        var data = Fanex.ExtAcc.Customer.GetParams();

        var url = resolveClientUrl('Customer/SearchListPost');

        $.ajax({
            url: url,
            data: data,
            dataType: 'json'
        })
        .success(function (result) {
            var rows = $.parseJSON(result.rows);
            var options = null;
            var bodyRows = [];

            Fanex.ExtAcc.Customer.DataSource = rows;

            if (result.roleId != 0 && rows.length > 0) {
                var custId = rows[0].custid;

                $this.directSuperId = result.srecommend;
                $this.directMasterId = result.mrecommend;
                $this.directAgentId = result.recommend;

                if (result.roleId == 4) {
                    $this.GetSuperList(rows[0].custid, result);
                }
                else if (result.roleId == 3) {
                    $this.GetMasterList(result.srecommend, $this.AllowDoubleComm, custId, result, $this.superSyncCasino, $this.superSyncBingo, $this.superSyncLiveCasino);
                }
                else if (result.roleId == 2) {
                    $this.GetAgentList(result.mrecommend, $this.AllowDoubleComm, custId, result, $this.masterSyncCasino, $this.masterSyncBingo, $this.masterSyncLiveCasino);
                }
                else {
                    $this.GetMemberList(result.recommend, $this.AllowDoubleComm, custId, result, $this.agentSyncCasino, $this.agentSyncBingo, $this.agentSyncLiveCasino);
                }
            } else {
                $('#msgNoinfo').css('display', 'block');
                $('#jqgrid').css('display', 'none');
                $('#exporttoexcel').css('display', 'none');

                $.unblockUI(document);
            }
        });
    },

    RenderSuperList: function (dataSource) {
        if (dataSource.srecommend) {
            $this.superId = dataSource.srecommend;
            $this.masterId = dataSource.mrecommend;
            $this.agentId = dataSource.recommend;
        }

        var rows = $.parseJSON(dataSource.rows);
        var options = null;
        var bodyRows = [];

        Fanex.ExtAcc.Customer.DataSource = rows;

        for (var i = 0; i < rows.length; i++) {
            var cssRow = rows[i].closed ? 'rowClosed' : (rows[i].suspended ? 'rowSuspended' : (!rows[i].enabled ? 'rowDisabled' : ''));

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
            $this.superId = dataSource.srecommend;
            $this.masterId = dataSource.mrecommend;
            $this.agentId = dataSource.recommend;
        }

        var rows = $.parseJSON(dataSource.rows);
        var options = null;
        var bodyRows = [];

        Fanex.ExtAcc.Customer.DataSource = rows;

        for (var i = 0; i < rows.length; i++) {
            var cssRow = rows[i].closed ? 'rowClosed' : (rows[i].suspended ? 'rowSuspended' : (!rows[i].enabled ? 'rowDisabled' : ''));

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

        $('#breadcrumbs').append('<a href="' + "javascript:Fanex.ExtAcc.Customer.GetSuperList(" + $this.directSuperId + ")" + '">' + 'Super' + '</a>');

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
            $this.superId = dataSource.srecommend;
            $this.masterId = dataSource.mrecommend;
            $this.agentId = dataSource.recommend;
        }

        var linkSuper = '<a href="' + "javascript:Fanex.ExtAcc.Customer.GetSuperList(" + $this.directSuperId + ")" + '">' + 'Super' + '</a>';
        var linkMaster = '<a href="' + "javascript:Fanex.ExtAcc.Customer.GetMasterList(" + $this.superId + ', ' + $this.AllowDoubleCommMaster + ',' + $this.directMasterId + ', null, ' + $this.superSyncCasino + ',' + $this.superSyncBingo + ',' + $this.superSyncLiveCasino + ')"' + '">' + 'Master' + '</a>';

        var rows = $.parseJSON(dataSource.rows);
        var options = null;
        var bodyRows = [];

        Fanex.ExtAcc.Customer.DataSource = rows;

        for (var i = 0; i < rows.length; i++) {
            var cssRow = rows[i].closed ? 'rowClosed' : (rows[i].suspended ? 'rowSuspended' : (!rows[i].enabled ? 'rowDisabled' : ''));

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
            $this.superId = dataSource.srecommend;
            $this.masterId = dataSource.mrecommend;
            $this.agentId = dataSource.recommend;
        }

        var rows = $.parseJSON(dataSource.rows);
        var options = null;
        var bodyRows = [];

        Fanex.ExtAcc.Customer.DataSource = rows;

        for (var i = 0; i < rows.length; i++) {
            var cssRow = rows[i].closed ? 'rowClosed' : (rows[i].suspended ? 'rowSuspended' : (!rows[i].enabled ? 'rowDisabled' : ''));

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

        var linkSuper = '<a href="' + "javascript:Fanex.ExtAcc.Customer.GetSuperList(" + $this.directSuperId + ")" + '">' + 'Super' + '</a>';
        var linkMaster = '<a href="' + "javascript:Fanex.ExtAcc.Customer.GetMasterList(" + $this.superId + ', ' + $this.AllowDoubleCommMaster + ',' + $this.directMasterId + ', null, ' + $this.superSyncCasino + ',' + $this.superSyncBingo + ',' + $this.superSyncLiveCasino + ')">' + 'Master' + '</a>';
        var linkAgent = '<a href="' + "javascript:Fanex.ExtAcc.Customer.GetAgentList(" + $this.masterId + ', ' + $this.AllowDoubleCommAgent + ',' + $this.directAgentId + ', null, ' + $this.masterSyncCasino + ',' + $this.masterSyncBingo + ',' + $this.masterSyncLiveCasino + ')">' + 'Agent' + '</a>';

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

        if ($this.CurrentLevel == $this.Super) {
            menuItems = {
                doubleComm: { name: "Edit Multiple Double Comm", className: "iconEnableEdit",
                    callback: function (key, opt) {
                        Fanex.ExtAcc.Form.Show(resolveClientUrl('Customer/ConfigMultipleDoubleComm/' + $this.CurrentLevel),
                            'Edit Multiple Double Commission For Losing Tickets',
                            430, 140, false, false, 1);
                    }
                }
            };
        }
        else if ($this.CurrentLevel == $this.Master) {
            menuItems = {
                sportbook: { name: "Edit Multiple Sportsbook", className: "iconEnableEdit" },
                racing: { name: "Edit Multiple Racing", className: "iconEnableEdit" }
            };

            if ($this.superSyncCasino) {
                menuItems.casino = { name: "Edit Multiple Casino", className: "iconEnableEdit" };
            }

            if ($this.superSyncBingo) {
                menuItems.bingo = { name: "Edit Multiple Bingo", className: "iconEnableEdit" };
            }

            if (!$this.superDisableP2P) {
                menuItems.poker = { name: "Edit Multiple Poker", className: "iconEnableEdit" };
            }

            if ($this.superSyncLiveCasino) {
                menuItems.liveCasino = { name: "Edit Multiple Live Casino", className: "iconEnableEdit" };
            }

            menuItems.virtualSport = { name: "Edit Multiple Virtual Sports", className: "iconEnableEdit" };

            if ($this.AllowDoubleCommMaster) {
                menuItems.doubleComm = { name: "Edit Multiple Double Comm", className: "iconEnableEdit",
                    callback: function (key, opt) {
                        Fanex.ExtAcc.Form.Show(resolveClientUrl('Customer/ConfigMultipleDoubleComm/' + $this.CurrentLevel),
                                'Edit Multiple Double Commission For Losing Tickets',
                                430, 140, false, false, 1);
                    }
                };
            }
        } else if ($this.CurrentLevel == $this.Agent) {
            menuItems = {
                sportbook: { name: "Edit Multiple Sportsbook", className: "iconEnableEdit" },
                racing: { name: "Edit Multiple Racing", className: "iconEnableEdit" }
            };

            if ($this.masterSyncCasino) {
                menuItems.casino = { name: "Edit Multiple Casino", className: "iconEnableEdit" };
            }

            if ($this.masterSyncBingo) {
                menuItems.bingo = { name: "Edit Multiple Bingo", className: "iconEnableEdit" };
            }

            if (!$this.masterDisableP2P) {
                menuItems.poker = { name: "Edit Multiple Poker", className: "iconEnableEdit" };
            }

            if ($this.masterSyncLiveCasino) {
                menuItems.liveCasino = { name: "Edit Multiple Live Casino", className: "iconEnableEdit" };
            }

            menuItems.virtualSport = { name: "Edit Multiple Virtual Sports", className: "iconEnableEdit" };

            if ($this.AllowDoubleCommAgent) {
                menuItems.doubleComm = { name: "Edit Multiple Double Comm", className: "iconEnableEdit",
                    callback: function (key, opt) {
                        Fanex.ExtAcc.Form.Show(resolveClientUrl('Customer/ConfigMultipleDoubleComm/' + $this.CurrentLevel),
                                'Edit Multiple Double Commission For Losing Tickets',
                                430, 140, false, false, 1);
                    }
                };
            }
        }
        else {
            menuItems = {
                sportbook: { name: "Edit Multiple Sportsbook", className: "iconEnableEdit" },
                racing: { name: "Edit Multiple Racing", className: "iconEnableEdit" }
            };

            if ($this.agentSyncCasino) {
                menuItems.casino = { name: "Edit Multiple Casino", className: "iconEnableEdit" };
            }

            if ($this.agentSyncBingo) {
                menuItems.bingo = { name: "Edit Multiple Bingo", className: "iconEnableEdit" };
            }

            if (!$this.agentDisableP2P) {
                menuItems.poker = { name: "Edit Multiple Poker", className: "iconEnableEdit" };
            }

            if ($this.agentSyncLiveCasino) {
                menuItems.liveCasino = { name: "Edit Multiple Live Casino", className: "iconEnableEdit" };
            }

            menuItems.virtualSport = { name: "Edit Multiple Virtual Sports", className: "iconEnableEdit" };

            if ($this.AllowDoubleComm) {
                menuItems.doubleComm = { name: "Edit Multiple Double Comm", className: "iconEnableEdit",
                    callback: function (key, opt) {
                        Fanex.ExtAcc.Form.Show(resolveClientUrl('Customer/ConfigMultipleDoubleComm/' + $this.CurrentLevel),
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
        Fanex.ExtAcc.Form.Close(false);
        var custids = [];
        var divs = $('div');

        for (var i = 0; i < divs.length; i++) {
            if (divs[i].className == 'menuActive' && !custids.contains(divs[i].id)) {
                custids.push(divs[i].id);
            }
        }

        $this.ConfigMultipleDoubleComm(custids, allow, 0);
    }
}