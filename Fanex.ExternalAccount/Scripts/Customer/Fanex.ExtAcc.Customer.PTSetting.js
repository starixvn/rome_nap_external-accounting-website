if (typeof Fanex.ExtAcc.Customer.PTSetting == 'undefined') {
    Fanex.ExtAcc.Customer.PTSetting = {};
};

Fanex.ExtAcc.Customer.PTSetting = {
    IsMultiple: Fanex.ExtAcc.Customer.IsMultiple,
    customer: null,
    Saved: false,
    IsUpdateAllMember: false,
    NewPtTaking: 0.0,

    Init: function () {
        if (Fanex.ExtAcc.Customer.CurrentLevel == Fanex.ExtAcc.Customer.Master) {
            this.GetMasterPTSetting();
        }
        else if (Fanex.ExtAcc.Customer.CurrentLevel == Fanex.ExtAcc.Customer.Agent) {
            this.GetAgentPTSetting();
        }
        else {
            if (Fanex.ExtAcc.Customer.isSuperEditMember == true) {
                this.SuperGetMemberPTSetting();
            }
            else if (Fanex.ExtAcc.Customer.isMasterEditMember == true) {
                this.MasterGetMemberPTSetting();
            }
            else {
                this.GetMemberPTSetting();
            }
        }
    },

    GetMasterPTSetting: function () {
        Fanex.ExtAcc.BlockUI();

        var data = new Object();

        data.custId = Fanex.ExtAcc.Customer.UserId;
        data.parentId = Fanex.ExtAcc.Customer.superId;

        $.ajax({
            url: resolveClientUrl('Sportsbook/EditPTMasterGet'),
            data: data,
            dataType: 'json'
        })
        .success(function (result) {
            var ptGrid = null;
            var context = result.context;
            customer = result.customer;
            var isChangeM3 = false;
            var isShowPopUpSetPTMember = false;
            var oldPTSetting = Fanex.copy(customer.ptSetting);

            Fanex.PTEngine.CURRENT_CONTEXT = context;
            Fanex.PTEngine.CURRENT_CONTEXT.ia3 = false;

            ptGrid = new Fanex.PTEngine.PTGrid(customer.ptSetting);

            var ptContainer = document.getElementById('ptContainer');
            ptGrid.Render(ptContainer);

            Fanex.ExtAcc.Customer.PTSetting.PTSettingModel = function () {
                this.saveCustomer = function () {
                    if (ptGrid) {
                        customer.ptSetting = ptGrid.Save();

                        // For Check Edit Postion Taking
                        if (context.targetCustId > 0 && !Fanex.ExtAcc.Customer.PTSetting.Saved) {
                            if (!isShowPopUpSetPTMember) {
                                isChangeM3 = Fanex.EditPT.CheckChangeM3(oldPTSetting, customer.ptSetting);
                            }
                            if (isChangeM3 && !isShowPopUpSetPTMember) {
                                Popups.Confirm(Fanex.EditPT.MemberMaximumPT(), Language.CONFIRM_MASTER_NOT_TO_TAKE_AUTO_POSITION_TAKING);
                                return;
                            }
                            else if (isChangeM3 && isShowPopUpSetPTMember) {
                                Popups.Close();
                            }
                        }
                    }

                    Fanex.ExtAcc.BlockUI();

                    var text = JSON.stringify(customer);
                    var para = {
                        custId: Fanex.ExtAcc.Customer.UserId,
                        parentId: Fanex.ExtAcc.Customer.superId,
                        txtCustomerObject: text,
                        isUpdateAllMember: Fanex.ExtAcc.Customer.PTSetting.IsUpdateAllMember,
                        newPositionTaking: Fanex.ExtAcc.Customer.PTSetting.NewPtTaking
                    };

                    Fanex.ExtAcc.Customer.PTSetting.Saved = false;

                    $.ajax({
                        url: resolveClientUrl('Sportsbook/SuperUpdateMasterPTPost'),
                        data: para,
                        dataType: 'json',
                        type: 'post'
                    })
                    .success(function (result) {
                        var codes = result.ErrorCode.split('\u0007');
                        var msgs = result.ErrorMessage.split('\u0007');
                        var success = true;
                        var error = '';

                        for (var i = 0; i < codes.length; i++) {
                            if (parseInt(codes[i]) > 0) {
                                success = false;

                                error += msgs[i] + '<br/>';
                            }
                        }

                        if (success) {
                            var timeout = setTimeout(function () {
                                clearTimeout(timeout);
                                $.unblockUI(document);

                                Fanex.ExtAcc.Form.Close(false);
                            }, 2000);
                        }
                        else {
                            $.unblockUI(document);

                            $('#errMsg').css('display', 'block');
                            $('#errMsg').html(error);
                        }
                    })
                    .error(function (xhr, status, err) {
                        $.unblockUI(document);
                    });
                }
            }

            ko.applyBindings(new Fanex.ExtAcc.Customer.PTSetting.PTSettingModel());

            $.unblockUI(document);
        })
        .error(function (xhr, status, err) {
            $.unblockUI(document);
        });
    },

    GetAgentPTSetting: function () {
        Fanex.ExtAcc.BlockUI();

        var data = new Object();

        data.custId = Fanex.ExtAcc.Customer.UserId;
        data.parentId = Fanex.ExtAcc.Customer.masterId;

        $.ajax({
            url: resolveClientUrl('Sportsbook/EditPTAgentGet'),
            data: data,
            dataType: 'json'
        })
        .success(function (result) {
            var ptGrid = null;
            var context = result.context;
            customer = result.customer;
            var isChangeM2 = false;
            var isShowPopUpSetPTMember = false;
            var oldPTSetting = Fanex.copy(customer.ptSetting);

            Fanex.PTEngine.CURRENT_CONTEXT = context;
            Fanex.PTEngine.CURRENT_CONTEXT.ia3 = !customer.statusSetting.disableAutoMPT;

            ptGrid = new Fanex.PTEngine.PTGrid(customer.ptSetting);

            var ptContainer = document.getElementById('ptContainer');
            ptGrid.Render(ptContainer);

            Fanex.ExtAcc.Customer.PTSetting.PTSettingModel = function () {
                this.saveCustomer = function () {
                    if (ptGrid) {
                        customer.ptSetting = ptGrid.Save();

                        // For Check Edit Postion Taking
                        if (context.targetCustId > 0 && !Fanex.ExtAcc.Customer.PTSetting.Saved) {
                            if (!isShowPopUpSetPTMember) {
                                isChangeM2 = Fanex.EditPT.CheckChangeM2(oldPTSetting, customer.ptSetting);
                            }
                            if (isChangeM2 && !isShowPopUpSetPTMember) {
                                Popups.Confirm(Fanex.EditPT.AgentMemberMaximumPT(), Language.CONFIRM_AGENT_NOT_TO_TAKE_AUTO_POSITION_TAKING);
                                return;
                            }
                            else if (isChangeM2 && isShowPopUpSetPTMember) {
                                Popups.Close();
                            }
                        }
                    }

                    Fanex.ExtAcc.BlockUI();

                    var text = JSON.stringify(customer);
                    var para = {
                        custId: Fanex.ExtAcc.Customer.UserId,
                        parentId: Fanex.ExtAcc.Customer.superId,
                        txtCustomerObject: text,
                        isUpdateAllMember: Fanex.ExtAcc.Customer.PTSetting.IsUpdateAllMember,
                        newPositionTaking: Fanex.ExtAcc.Customer.PTSetting.NewPtTaking
                    };

                    Fanex.ExtAcc.Customer.PTSetting.Saved = false;

                    $.ajax({
                        url: resolveClientUrl('Sportsbook/MasterUpdateAgentPTPost'),
                        data: para,
                        dataType: 'json',
                        type: 'post'
                    })
                    .success(function (result) {
                        var codes = result.ErrorCode.split('\u0007');
                        var msgs = result.ErrorMessage.split('\u0007');
                        var success = true;
                        var error = '';

                        for (var i = 0; i < codes.length; i++) {
                            if (parseInt(codes[i]) > 0) {
                                success = false;

                                error += msgs[i] + '<br/>';
                            }
                        }

                        if (success) {
                            var timeout = setTimeout(function () {
                                clearTimeout(timeout);
                                $.unblockUI(document);

                                Fanex.ExtAcc.Form.Close(false);
                            }, 2000);
                        }
                        else {
                            $.unblockUI(document);

                            $('#errMsg').css('display', 'block');
                            $('#errMsg').html(error);
                        }
                    })
                    .error(function (xhr, status, err) {
                        $.unblockUI(document);
                    });
                }
            }

            ko.applyBindings(new Fanex.ExtAcc.Customer.PTSetting.PTSettingModel());

            $.unblockUI(document);
        })
        .error(function (xhr, status, err) {
            $.unblockUI(document);
        });
    },
    GetMemberPTSetting: function () {
        Fanex.ExtAcc.BlockUI();

        var data = new Object();

        data.custId = Fanex.ExtAcc.Customer.UserId;
        data.parentId = Fanex.ExtAcc.Customer.agentId;

        $.ajax({
            url: resolveClientUrl('Sportsbook/EditPTMemberGet'),
            data: data,
            dataType: 'json'
        })
        .success(function (result) {
            var ptGrid = null;
            var context = result.context;
            var customer = result.customer;

            Fanex.PTEngine.CURRENT_CONTEXT = context;
            Fanex.PTEngine.CURRENT_CONTEXT.ia3 = false;

            ptGrid = new Fanex.PTEngine.PTGrid(customer.ptSetting);

            var ptContainer = document.getElementById('ptContainer');
            ptGrid.Render(ptContainer);

            Fanex.ExtAcc.Customer.PTSetting.PTSettingModel = function () {
                this.saveCustomer = function () {
                    Fanex.ExtAcc.BlockUI();

                    if (ptGrid) {
                        customer.ptSetting = ptGrid.Save();
                        var text = JSON.stringify(customer);

                        var para = {
                            custId: Fanex.ExtAcc.Customer.UserId,
                            parentId: Fanex.ExtAcc.Customer.superId,
                            txtCustomerObject: text
                        };
                    }

                    $.ajax({
                        url: resolveClientUrl('Sportsbook/AgentUpdateMemberPTPost'),
                        data: para,
                        dataType: 'json',
                        type: 'post'
                    })
                    .success(function (result) {
                        var codes = result.ErrorCode.split('\u0007');
                        var msgs = result.ErrorMessage.split('\u0007');
                        var success = true;
                        var error = '';

                        for (var i = 0; i < codes.length; i++) {
                            if (parseInt(codes[i]) > 0) {
                                success = false;

                                error += msgs[i] + '<br/>';
                            }
                        }

                        if (success) {
                            var timeout = setTimeout(function () {
                                clearTimeout(timeout);
                                $.unblockUI(document);

                                Fanex.ExtAcc.Form.Close(false);
                            }, 2000);
                        }
                        else {
                            $.unblockUI(document);

                            $('#errMsg').css('display', 'block');
                            $('#errMsg').html(error);
                        }
                    })
                    .error(function (xhr, status, err) {
                        $.unblockUI(document);
                    });
                }
            }

            ko.applyBindings(new Fanex.ExtAcc.Customer.PTSetting.PTSettingModel());

            $.unblockUI(document);
        })
        .error(function (xhr, status, err) {
            $.unblockUI(document);
        });
    },
    SuperGetMemberPTSetting: function () {
        Fanex.ExtAcc.BlockUI();

        var data = new Object();

        data.custId = Fanex.ExtAcc.Customer.UserId;
        data.parentId = Fanex.ExtAcc.Customer.superId;
        data.agentId = Fanex.ExtAcc.Customer.agentId;

        $.ajax({
            url: resolveClientUrl('Sportsbook/SuperEditPTMemberGet'),
            data: data,
            dataType: 'json'
        })
        .success(function (result) {
            var ptGrid = null;
            var context = result.context;
            var customer = result.customer;

            Fanex.PTEngine.CURRENT_CONTEXT = context;

            ptGrid = new Fanex.PTEngine.PTGrid(customer.ptSetting);

            var ptContainer = document.getElementById('ptContainer');
            ptGrid.Render(ptContainer);

            Fanex.ExtAcc.Customer.PTSetting.PTSettingModel = function () {
                this.saveCustomer = function () {
                    Fanex.ExtAcc.BlockUI();

                    if (ptGrid) {
                        customer.ptSetting = ptGrid.Save();
                        var text = JSON.stringify(customer);

                        var para = {
                            custId: Fanex.ExtAcc.Customer.UserId,
                            parentId: Fanex.ExtAcc.Customer.superId,
                            agentId: Fanex.ExtAcc.Customer.agentId,
                            txtCustomerObject: text
                        };
                    }

                    $.ajax({
                        url: resolveClientUrl('Sportsbook/SuperEditPTMemberPost'),
                        data: para,
                        dataType: 'json',
                        type: 'post'
                    })
                    .success(function (result) {
                        var codes = result.ErrorCode.split('\u0007');
                        var msgs = result.ErrorMessage.split('\u0007');
                        var success = true;
                        var error = '';

                        for (var i = 0; i < codes.length; i++) {
                            if (parseInt(codes[i]) > 0) {
                                success = false;

                                error += msgs[i] + '<br/>';
                            }
                        }

                        if (success) {
                            var timeout = setTimeout(function () {
                                clearTimeout(timeout);
                                $.unblockUI(document);

                                Fanex.ExtAcc.Form.Close(false);
                            }, 2000);
                        }
                        else {
                            $.unblockUI(document);

                            $('#errMsg').css('display', 'block');
                            $('#errMsg').html(error);
                        }
                    })
                    .error(function (xhr, status, err) {
                        $.unblockUI(document);
                    });
                }
            }

            ko.applyBindings(new Fanex.ExtAcc.Customer.PTSetting.PTSettingModel());

            $.unblockUI(document);
        })
        .error(function (xhr, status, err) {
            $.unblockUI(document);
        });
    },
    MasterGetMemberPTSetting: function () {
        Fanex.ExtAcc.BlockUI();

        var data = new Object();

        data.custId = Fanex.ExtAcc.Customer.UserId;
        data.parentId = Fanex.ExtAcc.Customer.masterId;
        data.agentId = Fanex.ExtAcc.Customer.agentId;

        $.ajax({
            url: resolveClientUrl('Sportsbook/MasterEditPTMemberGet'),
            data: data,
            dataType: 'json'
        })
        .success(function (result) {
            var ptGrid = null;
            var context = result.context;
            var customer = result.customer;

            Fanex.PTEngine.CURRENT_CONTEXT = context;

            ptGrid = new Fanex.PTEngine.PTGrid(customer.ptSetting);

            var ptContainer = document.getElementById('ptContainer');
            ptGrid.Render(ptContainer);

            Fanex.ExtAcc.Customer.PTSetting.PTSettingModel = function () {
                this.saveCustomer = function () {
                    Fanex.ExtAcc.BlockUI();

                    if (ptGrid) {
                        customer.ptSetting = ptGrid.Save();
                        var text = JSON.stringify(customer);

                        var para = {
                            custId: Fanex.ExtAcc.Customer.UserId,
                            parentId: Fanex.ExtAcc.Customer.masterId,
                            agentId: Fanex.ExtAcc.Customer.agentId,
                            txtCustomerObject: text
                        };
                    }

                    $.ajax({
                        url: resolveClientUrl('Sportsbook/MasterEditPTMemberPost'),
                        data: para,
                        dataType: 'json',
                        type: 'post'
                    })
                    .success(function (result) {
                        var codes = result.ErrorCode.split('\u0007');
                        var msgs = result.ErrorMessage.split('\u0007');
                        var success = true;
                        var error = '';

                        for (var i = 0; i < codes.length; i++) {
                            if (parseInt(codes[i]) > 0) {
                                success = false;

                                error += msgs[i] + '<br/>';
                            }
                        }

                        if (success) {
                            var timeout = setTimeout(function () {
                                clearTimeout(timeout);
                                $.unblockUI(document);

                                Fanex.ExtAcc.Form.Close(false);
                            }, 2000);
                        }
                        else {
                            $.unblockUI(document);

                            $('#errMsg').css('display', 'block');
                            $('#errMsg').html(error);
                        }
                    })
                    .error(function (xhr, status, err) {
                        $.unblockUI(document);
                    });
                }
            }

            ko.applyBindings(new Fanex.ExtAcc.Customer.PTSetting.PTSettingModel());

            $.unblockUI(document);
        })
        .error(function (xhr, status, err) {
            $.unblockUI(document);
        });
    }
};

Fanex.EditPT = {
    // Check or UnCheck Close Status
    Closed: function (checkBoxClose, currentStatusClose) {
        if (checkBoxClose.checked == true && checkBoxClose.checked != currentStatusClose) {
            if (!confirm(Language.ALERT_CONFIRM_CLOSE)) {
                customer.statusSetting.closed = false;
                checkBoxClose.checked = false;
            }
        }
        else if (checkBoxClose.checked == false && checkBoxClose.checked != currentStatusClose) {
            if (!confirm(Language.ALERT_CONFIRM_OPEN)) {
                customer.statusSetting.closed = true;
                checkBoxClose.checked = true;
            }
        }
    },

    // Check or UnCheck Suspend Status
    Suspended: function (checkBoxSuspend, currentStatusSuspend) {
        if (checkBoxSuspend.checked == true && checkBoxSuspend.checked != currentStatusSuspend) {
            if (!confirm(Language.ALERT_CONFIRM_SUSPEND)) {
                customer.statusSetting.suspended = false;
                checkBoxSuspend.checked = false;
            }
        }
        else if (checkBoxSuspend.checked == false && checkBoxSuspend.checked != currentStatusSuspend) {
            if (!confirm(Language.ALERT_CONFIRM_UNSUSPEND)) {
                customer.statusSetting.suspended = true;
                checkBoxSuspend.checked = true;
            }
        }
    },

    // We need check have any PT value change from auto to none auto. And Set new value to Member
    CheckChangeM3: function (ptSettingOld, ptSettingNew) {
        var sportTypes = ptSettingOld.sportTypes;
        for (var sportTypeIndex = 0; sportTypeIndex < sportTypes.length; sportTypeIndex++) {
            var sportTypesItem = sportTypes[sportTypeIndex]
            var betTypeGroupSets = sportTypesItem.betTypeGroupSets;

            for (var groupSetIndex = 0; groupSetIndex < betTypeGroupSets.length; groupSetIndex++) {
                var betTypeGroupSetsItem = betTypeGroupSets[groupSetIndex];
                var betTypeGroups = betTypeGroupSetsItem.betTypeGroups;

                for (var groupIndex = 0; groupIndex < betTypeGroups.length; groupIndex++) {
                    var betTypeGroupItem = betTypeGroups[groupIndex];
                    var betTypes = betTypeGroupItem.betTypes;

                    for (var betTypeIndex = 0; betTypeIndex < betTypes.length; betTypeIndex++) {
                        betTypeItem = betTypes[betTypeIndex];
                        var isAutoOld = betTypeItem.pt.is3;
                        var isAutoNew = ptSettingNew.sportTypes[sportTypeIndex].betTypeGroupSets[groupSetIndex].betTypeGroups[groupIndex].betTypes[betTypeIndex].pt.is3;

                        if (isAutoOld && !isAutoNew) {
                            return true;
                        }
                    }
                }
            }
        }
        return false;
    },

    // We need check have any PT value change from auto to none auto. And Set new value to Member
    CheckChangeM2: function (ptSettingOld, ptSettingNew) {
        var sportTypes = ptSettingOld.sportTypes;
        for (var sportTypeIndex = 0; sportTypeIndex < sportTypes.length; sportTypeIndex++) {
            var sportTypesItem = sportTypes[sportTypeIndex]
            var betTypeGroupSets = sportTypesItem.betTypeGroupSets;

            for (var groupSetIndex = 0; groupSetIndex < betTypeGroupSets.length; groupSetIndex++) {
                var betTypeGroupSetsItem = betTypeGroupSets[groupSetIndex];
                var betTypeGroups = betTypeGroupSetsItem.betTypeGroups;

                for (var groupIndex = 0; groupIndex < betTypeGroups.length; groupIndex++) {
                    var betTypeGroupItem = betTypeGroups[groupIndex];
                    var betTypes = betTypeGroupItem.betTypes;

                    for (var betTypeIndex = 0; betTypeIndex < betTypes.length; betTypeIndex++) {
                        betTypeItem = betTypes[betTypeIndex];
                        var isAutoOld = betTypeItem.pt.im2;
                        var isAutoNew = ptSettingNew.sportTypes[sportTypeIndex].betTypeGroupSets[groupSetIndex].betTypeGroups[groupIndex].betTypes[betTypeIndex].pt.im2;

                        if (isAutoOld && !isAutoNew) {
                            return true;
                        }
                    }
                }
            }
        }
        return false;
    },

    // Get selected and m4 value of Soccer HdpD Master Preset
    MemberMaximumPT: function () {
        var ptSetting = customer.ptSetting;
        var s4 = ptSetting.sportTypes[0].betTypeGroupSets[0].betTypeGroups[0].betTypes[0].pt.s4;
        var m3 = ptSetting.sportTypes[0].betTypeGroupSets[0].betTypeGroups[0].betTypes[0].pt.m3;
        return Fanex.Round(s4 - m3);
    },

    // Get selected and m4 value of Soccer HdpD Master Preset
    AgentMemberMaximumPT: function () {
        var ptSetting = customer.ptSetting;
        var l3 = ptSetting.sportTypes[0].betTypeGroupSets[0].betTypeGroups[0].betTypes[0].pt.l3;
        var a2 = ptSetting.sportTypes[0].betTypeGroupSets[0].betTypeGroups[0].betTypes[0].pt.a2;
        return Math.max(0, Fanex.Round(l3 - a2));
    },

    // Confirm new value for all member before transfer to server
    SetNewValueToMember: function (isUpdateAllMember) {
        if (isUpdateAllMember) {
            var valueMemberPositionTaking = $("#selectedMemberPositionTaking").val();

            Fanex.ExtAcc.Customer.PTSetting.IsUpdateAllMember = parseInt(isUpdateAllMember) == 1 ? true : false;
            Fanex.ExtAcc.Customer.PTSetting.NewPtTaking = parseFloat(valueMemberPositionTaking);
        }

        Fanex.ExtAcc.Customer.PTSetting.Saved = true;
        var model = new Fanex.ExtAcc.Customer.PTSetting.PTSettingModel();
        model.saveCustomer();
        Popups.Close();
    }
};

function ConfirmYes() {
    Fanex.EditPT.SetNewValueToMember(1);
};

function ConfirmNo() {
    Fanex.EditPT.SetNewValueToMember(0);
};

$(document).ready(function () {
    Fanex.ExtAcc.Customer.PTSetting.Init();
});