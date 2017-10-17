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
    HideSportsbook2: false,
    HideCockfighting: false,
    HideCricket: false,
    HideAllbet: false,
    HideVoidbridge: false,
    HideNG2: false,
    HideFishingWorld: false,
    HideLucky3: false,
    HideBullfighting: false,

    HideAllbetMember: false,
    HideVoidbridgeMember: false,
    HideNG2Member: false,
    HideFishingWorldMember: false,

    superSyncCasino: false,
    superSyncLiveCasino: false,
    masterSyncCasino: false,
    masterSyncLiveCasino: false,
    agentSyncCasino: false,
    agentSyncLiveCasino: false,

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

    AllbetSyncList: [],
    AllbetSyncIdList: [],

    VoidbridgeSyncList: [],
    VoidbridgeSyncIdList: [],

    NG2SyncList: [],
    NG2SyncIdList: [],
    FishingWorldSyncList: [],
    FishingWorldSyncIdList: [],

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
                        top.location.href = resolveClientUrl('Account/Login');
                    }
                }
                else if (data == 2) {
                    top.location.href = resolveClientUrl('Account/Offline');
                }
                else if (data == 3) {
                    top.location.href = resolveClientUrl('Shared/Error');
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
        var enabled = null;
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
        } else if ($('#customerStatus').val() == '4') {
            enabled = false;
            closed = null;
            suspended = null;
        }

        var data = new Object();

        data.srecommend = this.superId == 0 ? null : this.superId;
        data.mrecommend = this.masterId == 0 ? null : this.masterId;
        data.recommend = this.agentId == 0 ? null : this.agentId;
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
            Fanex.ExtAcc.Customer.GetSuperList();
        } else {
            this.SearchList();
        }
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
            Fanex.ExtAcc.Customer.GetSuperList(this.custId);
        } else if (this.CurrentLevel == this.Master) {
            Fanex.ExtAcc.Customer.GetMasterList(this.superId, this.AllowDoubleComm, this.custId, null, this.superSyncCasino, this.superSyncLiveCasino, this.HideKeno, this.HideVirtualSports, this.HideGoldDeluxe, this.HideColossusBet, this.HideSportsbook2, this.HideCockfighting, this.HideCricket, this.HideAllbet, this.HideVoidbridge, this.HideNG2, this.HideFishingWorld, this.HideLottery, this.HideBullfighting);
        } else if (this.CurrentLevel == this.Agent) {
            Fanex.ExtAcc.Customer.GetAgentList(this.masterId, this.AllowDoubleComm, this.custId, null, this.masterSyncCasino, this.masterSyncLiveCasino, this.HideKeno, this.HideVirtualSports, this.HideGoldDeluxe, this.HideColossusBet, this.HideSportsbook2, this.HideCockfighting, this.HideCricket, this.HideAllbet, this.HideVoidbridge, this.HideNG2, this.HideFishingWorld, this.HideLottery, this.HideBullfighting);
        } else {
            Fanex.ExtAcc.Customer.GetMemberList(this.agentId, this.AllowDoubleComm, this.custId, null, this.agentSyncCasino, this.agentSyncLiveCasino, this.HideKeno, this.HideVirtualSports, this.HideGoldDeluxe, this.HideColossusBet, this.HideSportsbook2, this.HideCockfighting, this.HideCricket, this.HideAllbet, this.HideVoidbridge, this.HideNG2, this.HideFishingWorld, this.HideLottery, this.HideBullfighting);
        }
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

            if ($(div).attr('sync-allbet') == 'true' && !Fanex.ExtAcc.Customer.AllbetSyncIdList.contains($(div).attr('id'))) {
                Fanex.ExtAcc.Customer.AllbetSyncIdList.push($(div).attr('id'));
                Fanex.ExtAcc.Customer.AllbetSyncList.push($(div).attr('name'));
            }

            if ($(div).attr('sync-voidbridge') == 'true' && !Fanex.ExtAcc.Customer.VoidbridgeSyncIdList.contains($(div).attr('id'))) {
                Fanex.ExtAcc.Customer.VoidbridgeSyncIdList.push($(div).attr('id'));
                Fanex.ExtAcc.Customer.VoidbridgeSyncList.push($(div).attr('name'));
            }

            if ($(div).attr('sync-ng2') == 'true' && !Fanex.ExtAcc.Customer.NG2SyncIdList.contains($(div).attr('id'))) {
                Fanex.ExtAcc.Customer.NG2SyncIdList.push($(div).attr('id'));
                Fanex.ExtAcc.Customer.NG2SyncList.push($(div).attr('name'));
            }

            if ($(div).attr('sync-fishingworld') == 'true' && !Fanex.ExtAcc.Customer.FishingWorldSyncIdList.contains($(div).attr('id'))) {
                Fanex.ExtAcc.Customer.FishingWorldSyncIdList.push($(div).attr('id'));
                Fanex.ExtAcc.Customer.FishingWorldSyncList.push($(div).attr('name'));
            }
        } else {
            div.className = 'menuInactive';
            $('#editMult').attr('style', 'cursor: default !important');
            $('#editMult').attr('onclick', '');
            $('#editMult').contextMenu(false);

            if ($(div).attr('sync-allbet') == 'true') {
                Fanex.ExtAcc.Customer.AllbetSyncIdList.pop($(div).attr('id'));
                Fanex.ExtAcc.Customer.AllbetSyncList.pop($(div).attr('name'));
            }

            if ($(div).attr('sync-voidbridge') == 'true') {
                Fanex.ExtAcc.Customer.VoidbridgeSyncIdList.pop($(div).attr('id'));
                Fanex.ExtAcc.Customer.VoidbridgeSyncList.pop($(div).attr('name'));
            }

            if ($(div).attr('sync-ng2') == 'true') {
                Fanex.ExtAcc.Customer.NG2SyncIdList.pop($(div).attr('id'));
                Fanex.ExtAcc.Customer.NG2SyncList.pop($(div).attr('name'));
            }

            if ($(div).attr('sync-fishingworld') == 'true') {
                Fanex.ExtAcc.Customer.FishingWorldSyncIdList.pop($(div).attr('id'));
                Fanex.ExtAcc.Customer.FishingWorldSyncList.pop($(div).attr('name'));
            }
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

                if ($(divs[i]).attr('sync-allbet') == 'true' && divs[i].className == 'menuActive' && !Fanex.ExtAcc.Customer.AllbetSyncIdList.contains($(divs[i]).attr('id'))) {
                    Fanex.ExtAcc.Customer.AllbetSyncIdList.push($(divs[i]).attr('id'));
                    Fanex.ExtAcc.Customer.AllbetSyncList.push($(divs[i]).attr('name'));
                }
                else if ($(divs[i]).attr('sync-allbet') == 'true' && divs[i].className == 'menuInactive') {
                    Fanex.ExtAcc.Customer.AllbetSyncIdList.pop($(divs[i]).attr('id'));
                    Fanex.ExtAcc.Customer.AllbetSyncList.pop($(divs[i]).attr('name'));
                }

                if ($(divs[i]).attr('sync-voidbridge') == 'true' && divs[i].className == 'menuActive' && !Fanex.ExtAcc.Customer.VoidbridgeSyncIdList.contains($(divs[i]).attr('id'))) {
                    Fanex.ExtAcc.Customer.VoidbridgeSyncIdList.push($(divs[i]).attr('id'));
                    Fanex.ExtAcc.Customer.VoidbridgeSyncList.push($(divs[i]).attr('name'));
                }
                else if ($(divs[i]).attr('sync-voidbridge') == 'true' && divs[i].className == 'menuInactive') {
                    Fanex.ExtAcc.Customer.VoidbridgeSyncIdList.pop($(divs[i]).attr('id'));
                    Fanex.ExtAcc.Customer.VoidbridgeSyncList.pop($(divs[i]).attr('name'));
                }

                if ($(divs[i]).attr('sync-fishingworld') == 'true' && divs[i].className == 'menuActive' && !Fanex.ExtAcc.Customer.FishingWorldSyncIdList.contains($(divs[i]).attr('id'))) {
                    Fanex.ExtAcc.Customer.FishingWorldSyncIdList.push($(divs[i]).attr('id'));
                    Fanex.ExtAcc.Customer.FishingWorldSyncList.push($(divs[i]).attr('name'));
                }
                else if ($(divs[i]).attr('sync-fishingworld') == 'true' && divs[i].className == 'menuInactive') {
                    Fanex.ExtAcc.Customer.FishingWorldSyncIdList.pop($(divs[i]).attr('id'));
                    Fanex.ExtAcc.Customer.FishingWorldSyncList.pop($(divs[i]).attr('name'));
                }

                if ($(divs[i]).attr('sync-ng2') == 'true' && divs[i].className == 'menuActive' && !Fanex.ExtAcc.Customer.NG2SyncIdList.contains($(divs[i]).attr('id'))) {
                    Fanex.ExtAcc.Customer.NG2SyncIdList.push($(divs[i]).attr('id'));
                    Fanex.ExtAcc.Customer.NG2SyncList.push($(divs[i]).attr('name'));
                }
                else if ($(divs[i]).attr('sync-ng2') == 'true' && divs[i].className == 'menuInactive') {
                    Fanex.ExtAcc.Customer.NG2SyncIdList.pop($(divs[i]).attr('id'));
                    Fanex.ExtAcc.Customer.NG2SyncList.pop($(divs[i]).attr('name'));
                }
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
            Fanex.ExtAcc.Customer.GetSuperList();
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

            Fanex.ExtAcc.Customer.DataSource = rows;

            if (result.roleId != 0 && rows.length > 0) {
                var custId = rows[0].custid;

                Fanex.ExtAcc.Customer.directSuperId = result.srecommend;
                Fanex.ExtAcc.Customer.directMasterId = result.mrecommend;
                Fanex.ExtAcc.Customer.directAgentId = result.recommend;
                Fanex.ExtAcc.Customer.custId = Fanex.ExtAcc.Customer.UserId = result.custId;
                Fanex.ExtAcc.Customer.AllowDoubleComm = result.allowDoubleComm;

                Fanex.ExtAcc.Customer.HideSportsbook2 = rows[0].HideMuayThai;
                Fanex.ExtAcc.Customer.HideCricket = rows[0].HideCricket;
                Fanex.ExtAcc.Customer.HideCockfighting = rows[0].HideCockFighting;
                Fanex.ExtAcc.Customer.HideColossusBet = rows[0].HideColossusBet;
                Fanex.ExtAcc.Customer.HideGoldDeluxe = rows[0].HideGoldDeluxe;
                Fanex.ExtAcc.Customer.HideKeno = rows[0].HideKeno;
                Fanex.ExtAcc.Customer.HideVirtualSports = rows[0].HideVirtualSports;
                Fanex.ExtAcc.Customer.HideAllbet = !rows[0].SyncedAllBet;
                Fanex.ExtAcc.Customer.HideVoidbridge = !rows[0].SyncedVoidBridge;
                Fanex.ExtAcc.Customer.HideNG2 = !rows[0].SyncedNG2;
                Fanex.ExtAcc.Customer.HideFishingWorld = !rows[0].SyncedFishingWorld;
                Fanex.ExtAcc.Customer.HideLottery = rows[0].HideLottery;
                Fanex.ExtAcc.Customer.HideBullfighting = rows[0].HideBullFighting;

                if (result.roleId == 4) {
                    Fanex.ExtAcc.Customer.GetSuperList(rows[0].custid, result);
                } else if (result.roleId == 3) {
                    Fanex.ExtAcc.Customer.superSyncCasino = result.syncCasino;
                    Fanex.ExtAcc.Customer.superSyncLiveCasino = result.syncLiveCasino;
                    Fanex.ExtAcc.Customer.GetMasterList(result.srecommend, Fanex.ExtAcc.Customer.AllowDoubleComm, custId, result, Fanex.ExtAcc.Customer.superSyncCasino, Fanex.ExtAcc.Customer.superSyncLiveCasino, Fanex.ExtAcc.Customer.HideKeno, Fanex.ExtAcc.Customer.HideVirtualSports, Fanex.ExtAcc.Customer.HideGoldDeluxe, Fanex.ExtAcc.Customer.HideColossusBet, Fanex.ExtAcc.Customer.HideSportsbook2, Fanex.ExtAcc.Customer.HideCockfighting, Fanex.ExtAcc.Customer.HideCricket, Fanex.ExtAcc.Customer.HideAllbet, Fanex.ExtAcc.Customer.HideVoidbridge, Fanex.ExtAcc.Customer.HideNG2, Fanex.ExtAcc.Customer.HideFishingWorld, Fanex.ExtAcc.Customer.HideLottery, Fanex.ExtAcc.Customer.HideBullfighting);
                } else if (result.roleId == 2) {
                    Fanex.ExtAcc.Customer.masterSyncCasino = result.syncCasino;
                    Fanex.ExtAcc.Customer.masterSyncLiveCasino = result.syncLiveCasino;
                    Fanex.ExtAcc.Customer.GetAgentList(result.mrecommend, Fanex.ExtAcc.Customer.AllowDoubleComm, custId, result, Fanex.ExtAcc.Customer.masterSyncCasino, Fanex.ExtAcc.Customer.masterSyncLiveCasino, Fanex.ExtAcc.Customer.HideKeno, Fanex.ExtAcc.Customer.HideVirtualSports, Fanex.ExtAcc.Customer.HideGoldDeluxe, Fanex.ExtAcc.Customer.HideColossusBet, Fanex.ExtAcc.Customer.HideSportsbook2, Fanex.ExtAcc.Customer.HideCockfighting, Fanex.ExtAcc.Customer.HideCricket, Fanex.ExtAcc.Customer.HideAllbet, Fanex.ExtAcc.Customer.HideVoidbridge, Fanex.ExtAcc.Customer.HideNG2, Fanex.ExtAcc.Customer.HideFishingWorld, Fanex.ExtAcc.Customer.HideLottery, Fanex.ExtAcc.Customer.HideBullfighting);
                } else {
                    Fanex.ExtAcc.Customer.agentSyncCasino = result.syncCasino;
                    Fanex.ExtAcc.Customer.agentSyncLiveCasino = result.syncLiveCasino;
                    Fanex.ExtAcc.Customer.GetMemberList(result.recommend, Fanex.ExtAcc.Customer.AllowDoubleComm, custId, result, Fanex.ExtAcc.Customer.agentSyncCasino, Fanex.ExtAcc.Customer.agentSyncLiveCasino, Fanex.ExtAcc.Customer.HideKeno, Fanex.ExtAcc.Customer.HideVirtualSports, Fanex.ExtAcc.Customer.HideGoldDeluxe, Fanex.ExtAcc.Customer.HideColossusBet, Fanex.ExtAcc.Customer.HideSportsbook2, Fanex.ExtAcc.Customer.HideCockfighting, Fanex.ExtAcc.Customer.HideCricket, Fanex.ExtAcc.Customer.HideAllbet, Fanex.ExtAcc.Customer.HideVoidbridge, Fanex.ExtAcc.Customer.HideNG2, Fanex.ExtAcc.Customer.HideFishingWorld, Fanex.ExtAcc.Customer.HideLottery, Fanex.ExtAcc.Customer.HideBullfighting);
                }
            } else {
                $('#msgNoinfo').css('display', 'block');
                $('#jqgrid').css('display', 'none');
                $('#exporttoexcel').css('display', 'none');

                Fanex.ExtAcc.UnBlockUI();
            }
        });
    },

    AppendInputToForm: function (id, name, value, targetForm) {
        $('<input>').attr({
            type: 'hidden',
            id: id,
            name: name,
            value: value
        }).appendTo(targetForm);
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
    },

    GetUnsyncedProductIds: function (cell) {
        var row = cell.Value;
        var unsyncedProductIds = [];
        var dcsProducts = [
            { id: 21, name: "AllBet" },
            { id: 22, name: "VoidBridge" }
        ];

        for (var i = 0, max = dcsProducts.length; i < max; i++) {
            var syncedColumnName = "Synced" + dcsProducts[i].name;
            var synced = row[syncedColumnName];
            if (synced === false) {
                unsyncedProductIds.push(dcsProducts[i].id);
            }
        }

        return unsyncedProductIds.join(",");
    },

    GetClosedClassName: function (cell) {
        switch (cell.ClosedReason) {
            case 1:
                return "closed-byAdmin";
            case 2:
                return "closed-byUpline";
            case 3:
                return "closed-wrongOTPSC";
            case 0:
            default:
                return "closed-wrongPwdOthers";
        }        
    }
};