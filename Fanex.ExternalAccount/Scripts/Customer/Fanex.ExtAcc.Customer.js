Fanex.ExtAcc.Customer = {
    RoleNameConst: ['', 'Member', 'Agent', 'Master', 'Super'],
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
    UplineProductSettings: [],
    ProductGroups: [],

    isSuperEditMember: false,
    isMasterEditMember: false,
    isSuperEditAgent: false,

    IsMultiple: false,
    IsFullInfo: false,

    Closed: false,
    Suspended: false,
    TransferCondition: false,

    URL: '',
    MultipleDblCommError: '',
    Usernames: '',
    CustIds: '',

    LiveCasinoSyncList: [],
    LiveCasinoSyncIdList: [],

    VoidbridgeSyncList: [],
    VoidbridgeSyncIdList: [],

    RnGCasinoStatuses: [],
    TitleEditInfo: '',
    MessageClosed: 'Are you sure you want to change Closed status for this customer? Warning, all downlines will be closed as well!',
    MessageUnClosed: 'Are you sure you want to open this account?',
    MessageSuspended: 'Are you sure you want to change Suspend status for this customer? Warning, all members under this agent can\'t place bet!',
    MessageUnSuspended: 'Are you sure you want to change Suspend status for this customer? All downlines will be un-suspended as well.',

    MessageClosedMember: 'Are you sure you want to change Closed status for this customer?',
    MessageSuspendedMember: 'Are you sure you want to change Suspend status for this customer? Warning, this member can\'t place bet!',
    MessageUnSuspendedMember: 'Are you sure you want to change Suspend status for this customer?',

    RecheckinXHR: null,
    $this: null,
    IsLoading: false,

    ReCheckin: function () {
        if (this.RecheckinXHR != null)
            this.RecheckinXHR.abort();

        this.RecheckinXHR = $.ajax({
            url: resolveClientUrl('Shared/ReCheckIn'),
            type: 'GET',
            dataType: 'html',
            cache: false,
            success: function (data) {
                if (!Fanex.ExtAcc.IsSwitchLine) {
                    if (data == -1 || isNaN(data)) {
                        try {
                            if (typeof Fanex.ExtAcc.Form != 'undefined') {
                                if (document.getElementById('windowPopup')) {
                                    var tag = document.getElementById('windowPopup');

                                    if (tag.tagName == "IFRAME") {
                                        Fanex.ExtAcc.Form.Close();
                                    } else {
                                        $('#windowPopup').html('<div>You have logged in from another session or your login session has expired!<div>'); // Prevent opening popup if any
                                    }
                                }

                                var timeout = setTimeout(function () {
                                    clearTimeout(timeout);
                                    Fanex.ExtAcc.Form.Alert('Session Expired', 'You have logged in from another session or your login session has expired!', 400, 150, null, function () {
                                        location.href = resolveClientUrl('Account/LogOff');
                                    }, 1000);
                                }, 1000);
                            }
                        } catch (e) {
                            top.location.href = resolveClientUrl('Account/LogOff');
                        }
                    }
                    else if (data == 2) {
                        top.location.href = resolveClientUrl('Shared/Offline');
                    }
                    else if (data == 3) {
                        top.location.href = resolveClientUrl('Shared/Error');
                    }

                    Fanex.ExtAcc.Account.RecheckinXHR = null;
                }
            },
            error: function (xhr, status, e) {
                if (JSON && xhr.responseText) {
                    var errorResult = JSON.parse(xhr.responseText);
                    if (errorResult.code === 999999) {
                        top.location.href = parent.resolveClientUrl('Shared/Offline');
                    }
                }
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

        $this = this;
        $('#exporttoexcel').click(function () {
            var data = Fanex.ExtAcc.Customer.GetParams();

            var params = "groupName=" +
                data.groupName +
                "&enabled=" +

                data.enabled +
                "&closed=" +
                data.closed +
                "&suspended=" +
                data.suspended +
                "&sRecommend=" +
                Fanex.ExtAcc.Customer.superId +
                "&mRecommend=" +
                Fanex.ExtAcc.Customer.masterId +
                "&recommend=" +
                Fanex.ExtAcc.Customer.agentId +
                "&roleId=" +
                Fanex.ExtAcc.Customer.CurrentLevel +
                "&username=" +
                data.username +
                "&custId=" +
                Fanex.ExtAcc.Customer.custId;

            if (Fanex.ExtAcc.Customer.CurrentLevel == Fanex.ExtAcc.Customer.Super) {
                params += "&withDataLimited=" + $("#chk-show-limit-date").is(":checked");
            }

            top.exportExcel(resolveClientUrl("Customer/GetExportToExcel?") + params, document);
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

        $(".icon-filter").click($this.showHideFilter);
        Fanex.ExtAcc.Customer.bindOrientationChange();
        Fanex.ExtAcc.Customer.ShowCustomerLimitDateFilter();
    },
    bindOrientationChange: function () {
        top.landscapeHeight = 260;
        top.portraitHeight = 500;
        top.shouldCalculatePopupHeight = true;
        $(window).bind('orientationchange', function (e) {
            var calculatePopupheight = top.shouldCalculatePopupHeight && isMobile.phone;
            if (!calculatePopupheight) {
                return;
            }

            var height;
            var isLandscape = window.matchMedia("(orientation: landscape)").matches;
            if (isMobile.android.phone) {
                height = isLandscape ? top.portraitHeight : top.landscapeHeight;
            }
            else {
                height = isLandscape ? top.landscapeHeight : top.portraitHeight;
            }
            var dialog = top.$('.ui-dialog');
            dialog.height((height + 34) + 'px');

            top.$("#windowPopup").css("height", height + "px");
        });
    },
    hideFilter: false,
    showHideFilter: function () {
        if ($this.hideFilter) {
            $(".icon-filter").addClass("active");
            $(".filterbox").removeClass("hide");
        } else {
            $(".icon-filter").removeClass("active");
            $(".filterbox").addClass("hide");
        }

        try {
            if ($("#jqgrid").is(":visible"))
                $("#jqgrid").fanexGrid("resizeGrid");
        }
        catch (err) { }

        $this.hideFilter = !$this.hideFilter;
    },
    resizeReportGrid: function () {
        try {
            $("#jqgrid").fanexGrid("resizeGrid");
        }
        catch (err) { }
    },

    GroupName: function () {
        var data = new Object();
        data.groupName = $('#selectedGroupName').html();
        return data;
    },

    GetGroupId: function () {
        return $('#ddlGroupName').val();
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
        data.withDataLimited = $("#chk-show-limit-date").length ? $("#chk-show-limit-date").is(":checked") : false;

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
        $('#Phone').val(Fanex.ExtAcc.Customer.Phone);
        $('#MobilePhone').val(Fanex.ExtAcc.Customer.MobilePhone);
        $('#Email').val(Fanex.ExtAcc.Customer.Email);
        $('#Fax').val(Fanex.ExtAcc.Customer.Fax);
        $('#FirstName').focus();
    },

    EditInfo: function (isEditSuper) {
        var firstName = $('#FirstName').val();
        var lastName = $('#LastName').val();
        var phone = $('#Phone').val();
        var mobilePhone = $('#MobilePhone').val();
        var email = $('#Email').val();
        var fax = $('#Fax').val();
        var remark = $('#Remark').val();
        var editUrl = 'Customer/EditInfoPost';
        if (isEditSuper) {
            editUrl = 'MainSuper/EditInfoPost';
        }

        var data = {
            custId: this.UserId,
            firstName: Fanex.ExtAcc.EscapeHtml(firstName),
            lastName: Fanex.ExtAcc.EscapeHtml(lastName),
            phone: Fanex.ExtAcc.EscapeHtml(phone),
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
                if (Fanex.ExtAcc.ShowSecurityCodePage(data, Fanex.ExtAcc.UnBlockUI)) {
                    return;
                }

                if (!data.isSuccess) {
                    Fanex.ExtAcc.UnBlockUI();

                    $('#errMsg').show();
                    $('#errMsg').html(data.errorMessage.formatErrorMessage());
                    return;
                }
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
            })
            .error(function (data) {
                if (data.responseText) { console.log(data.responseText); }
                Fanex.ExtAcc.UnBlockUI();
            });
    },

    ResetPwd: function (roleId) {
        var errorMessage = "";
        var errorField;

        if (!Validators.validateInput($('#Password').val())) {
            errorField = $('#Password');
            errorMessage = 'Password is required';
        } else if (!this.validatePassword($('#Password').val(), roleId === Fanex.ExtAcc.Customer.Member)) {
            errorField = $('#Password');
            errorMessage = 'Your password does not meet the minimum security requirement';
        } else if (!Validators.validateInput($('#confirmPwd').val())) {
            errorField = $('#confirmPwd');
            errorMessage = 'Confirm password is required';
        } else if ($('#confirmPwd').val() !== $('#Password').val()) {
            errorField = $('#confirmPwd');
            errorMessage = 'Confirm password is not matched';
        }

        if (errorMessage !== "") {
            $("#err-message").html(errorMessage).removeClass("hide");
            errorField.addClass("invalid-field").focus();
            return false;
        }

        $("#err-message").addClass("hide");
        Fanex.ExtAcc.BlockUI();
        var encryptCode = $("#leftFrame").contents().find("#SCode").val();
        if (!encryptCode) {
            encryptCode = $("#SCode").val();
        }
        $.ajax({
            url: resolveClientUrl('Customer/UpdatePassword/?custId=' + $('#CustId').val() + '&password=' + encodeURIComponent($('#Password').val().encrypt(encryptCode)) + '&roleId=' + roleId),
            type: 'POST',
            data: Fanex.ExtAcc.Customer.GroupName()
        })
            .done(function (data) {
                if (Fanex.ExtAcc.ShowSecurityCodePage(data, Fanex.ExtAcc.UnBlockUI)) {
                    return;
                }

                if (!data.isSuccess) {
                    $('#err-message').html(data.errorMessage.formatErrorMessage()).removeClass('hide');
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
        var _this = Fanex.ExtAcc.Customer.CustomerList;
        _this.ReloadAfterEdit();
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
                    type: 'post',
                    data: {
                        custid: custid,
                        allow: ($('#dbl-' + custid).attr('class') == 'disallowDblComm' || $('#dbl-' + custid).attr('class') == 'offDblComm'),
                        groupName: Fanex.ExtAcc.Customer.GroupName().groupName,
                        isFilteredAction: false
                    }
                })
                    .success(function (result) {
                        if (Fanex.ExtAcc.ShowSecurityCodePage(result, Fanex.ExtAcc.UnBlockUI)) {
                            return;
                        }

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

            if ($(div).attr('sync-livecasino') == 'true' && !Fanex.ExtAcc.Customer.LiveCasinoSyncIdList.contains($(div).attr('id'))) {
                Fanex.ExtAcc.Customer.LiveCasinoSyncIdList.push($(div).attr('id'));
                Fanex.ExtAcc.Customer.LiveCasinoSyncList.push($(div).attr('name'));
            }

            if ($(div).attr('sync-voidbridge') == 'true' && !Fanex.ExtAcc.Customer.VoidbridgeSyncIdList.contains($(div).attr('id'))) {
                Fanex.ExtAcc.Customer.VoidbridgeSyncIdList.push($(div).attr('id'));
                Fanex.ExtAcc.Customer.VoidbridgeSyncList.push($(div).attr('name'));
            }
        } else {
            div.className = 'menuInactive';
            $('#editMult').attr('style', 'cursor: default !important');
            $('#editMult').attr('onclick', '');
            $('#editMult').contextMenu(false);

            if ($(div).attr('sync-livecasino') == 'true') {
                Fanex.ExtAcc.Customer.LiveCasinoSyncIdList.pop($(div).attr('id'));
                Fanex.ExtAcc.Customer.LiveCasinoSyncList.pop($(div).attr('name'));
            }

            if ($(div).attr('sync-voidbridge') == 'true') {
                Fanex.ExtAcc.Customer.VoidbridgeSyncIdList.pop($(div).attr('id'));
                Fanex.ExtAcc.Customer.VoidbridgeSyncList.pop($(div).attr('name'));
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

                if ($(divs[i]).attr('sync-livecasino') == 'true' && divs[i].className == 'menuActive' && !Fanex.ExtAcc.Customer.LiveCasinoSyncIdList.contains($(divs[i]).attr('id'))) {
                    Fanex.ExtAcc.Customer.LiveCasinoSyncIdList.push($(divs[i]).attr('id'));
                    Fanex.ExtAcc.Customer.LiveCasinoSyncList.push($(divs[i]).attr('name'));
                }
                else if ($(divs[i]).attr('sync-livecasino') == 'true' && divs[i].className == 'menuInactive') {
                    Fanex.ExtAcc.Customer.LiveCasinoSyncIdList.pop($(divs[i]).attr('id'));
                    Fanex.ExtAcc.Customer.LiveCasinoSyncList.pop($(divs[i]).attr('name'));
                }

                if ($(divs[i]).attr('sync-voidbridge') == 'true' && divs[i].className == 'menuActive' && !Fanex.ExtAcc.Customer.VoidbridgeSyncIdList.contains($(divs[i]).attr('id'))) {
                    Fanex.ExtAcc.Customer.VoidbridgeSyncIdList.push($(divs[i]).attr('id'));
                    Fanex.ExtAcc.Customer.VoidbridgeSyncList.push($(divs[i]).attr('name'));
                }
                else if ($(divs[i]).attr('sync-voidbridge') == 'true' && divs[i].className == 'menuInactive') {
                    Fanex.ExtAcc.Customer.VoidbridgeSyncIdList.pop($(divs[i]).attr('id'));
                    Fanex.ExtAcc.Customer.VoidbridgeSyncList.pop($(divs[i]).attr('name'));
                }
            }
        }
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
        var divs = $('div.menuActive');
        this.Usernames = '';
        this.CustIds = '';
        this.RnGCasinoStatuses = [];
        for (var i = 0; i < divs.length; i++) {
            if (!custids.contains(divs[i].id)) {
                custids.push(divs[i].id);
                this.CustIds += divs[i].id + ',';
                this.Usernames += divs.eq(i).attr('data-username') + ', ';
                this.RnGCasinoStatuses.push(divs.eq(i).attr('data-sync-casino') == 'true' ? '1' : '0');
            }
        }

        this.UserId = parseInt(custids[0]);
        this.IsMultiple = custids.length > 1;
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
            { id: 41, name: "LiveCasino" },
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
            case 4:
                return "closed-byArchive";
            default:
                return "";
        }
    },

    GetPopupHeight: function (landscapeHeight, portraitHeight) {
        if (!(isMobile.phone || isMobile.tablet)) {
            height = 700;
        }
        else {
            height = isMobile.phone && window.matchMedia("(orientation: landscape)").matches ?
                landscapeHeight :
                portraitHeight;
        }

        return height;
    },

    DisableMultipleContextMenu: function () {
        var $editMultDiv = $('#editMult');
        if ($editMultDiv.length) {
            $editMultDiv
                .attr('style', 'cursor: default !important')
                .attr('onclick', '')
                .contextMenu(false);
        }
    },

    GetProductSettings: function (customerProductStatus, isCheckUpline) {
        isCheckUpline = typeof (isCheckUpline) == 'undefined' ? true : isCheckUpline;
        var productSettings = [];

        for (var i = 0, max = siteLevelProducts.length; i < max; i++) {
            var currentSiteProduct = siteLevelProducts[i];
            var currentProduct = null;          

            if (currentProduct && !currentProduct.IsHidden) {
                productSettings.push({
                    productId: currentSiteProduct.Id,
                    productKey: currentSiteProduct.Key,
                    productName: currentSiteProduct.Name,
                    isDisabled: currentProduct.isDisabled,
                    isSynced: currentProduct.isSynced
                });
            }
        }

        return productSettings;
    },

    UnlockAccess: function (custId, locked) {
        var confirmMessage = "";
        if (locked === 1) {
            confirmMessage = "This account has been locked due to multiple failed OTP/Security Code authentication attempts. Are you sure you want to unlock it?";
        } else if (locked === 2) {
            confirmMessage = "This account has been locked due to multiple failed password attempts. Are you sure you want to unlock it?";
        }

        if (confirmMessage.length) {
            var popupId = "popupUnlockAccess";
            Fanex.ExtAcc.Form.ConfirmBox(
                "Unlock Access",
                confirmMessage,
                480,
                "auto",
                false,
                function () {
                    Fanex.ExtAcc.BlockUI();
                    $.ajax({
                        type: "POST",
                        url: resolveClientUrl("CustomerSetting/LeftPanel/UpdateCustomer/UnlockAccess"),
                        data: {
                            custid: custId,
                            groupName: Fanex.ExtAcc.Customer.GroupName().groupName,
                        },
                        success: function (result) {
                            if (result.isSuccess) {
                                var timeout = setTimeout(function () {
                                    clearTimeout(timeout);
                                    Fanex.ExtAcc.Customer.CallBackCustomerList();
                                    Fanex.ExtAcc.UnBlockUI();
                                }, 2000);
                            } else {
                                Fanex.ExtAcc.UnBlockUI();
                                Fanex.ExtAcc.Form.Alert("Error", result.errorMessage, 480, "auto");
                            }
                        },
                        complete: function () {
                            Fanex.ExtAcc.Form.Close(null, null, popupId);
                        }
                    })
                },
                null,
                popupId);
        }
    }
};