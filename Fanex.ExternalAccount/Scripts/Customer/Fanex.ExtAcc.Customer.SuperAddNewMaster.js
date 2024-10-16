/// <reference path="../_references.js" />
if (typeof Fanex == 'undefined' || typeof Fanex.ExtAcc == 'undefined') {
    alert("Fanex.ExtAcc.js module is required!")
}

if (typeof Fanex.ExtAcc.Customer == 'undefined') {
    Fanex.ExtAcc.Customer = {};
}

Fanex.ExtAcc.Customer.SuperAddNewMaster = {
    customer: null,
    context: null,
    currentStatusClose: 1,
    currentStatusSuspend: 1,
    $this: null,
    Init: function () {
        $this = this;
        extendPTEngine();
        this.SuperAddNewMaster();
    },
    SuperAddNewMaster: function () {
        Fanex.ExtAcc.BlockUI();

        var data = new Object();
        data.custId = 0;
        data.parentId = $('#parentId').val();
        data.parentUsername = $('#parentUsername').val();
        data.parentCurrencyId = $('#parentCurrencyId').val();

        $.ajax({
            url: resolveClientUrl('Customer/SuperAddNewMasterGet'),
            data: data,
            dataType: 'json',
            type: 'post'
        })
            .success(function (result) {
                Fanex.PTEngine.PT_STEP = $('#PT3DecimalSites').val().splitAndContains(result.customer.siteLabel) ? 0.005 : 0.01;
                if (result.customer.bettingLimit.bettingLimitItems.length == 0) {
                    Fanex.ExtAcc.UnBlockUI();
                    Fanex.ExtAcc.Form.Close();
                    var timeout = setTimeout(function () {
                        clearTimeout(timeout);
                        Fanex.ExtAcc.Form.Alert('Error', "There is no information available".formatErrorMessage(), 450, 150);
                    }, 1000);

                    return;
                }

                customer = $this.customer = result.customer;
                context = result.context;

                if (context.targetCustId > 0) {
                    $('#closed').click(function (e) {
                        $this.Closed(document.getElementById('closed'), $this.currentStatusClose);
                    });
                }

                Fanex.Customer.CheckUser(4, customer.userName, 'selectedOne', 'selectedOne', 'selectedTwo', null, 'msgIcon');
                Fanex.Customer.CheckUser(4, customer.userName, 'selectedTwo', 'selectedOne', 'selectedTwo', null, 'msgIcon');

                Fanex.PTEngine.CURRENT_CONTEXT = context;
                Fanex.PTEngine.CURRENT_CONTEXT.ia3 = false;

                var isChangeM3 = false;
                var isShowPopUpSetPTMember = false;
                var newValueMemberPT = "";
                var oldPTSetting = Fanex.copy(customer.ptSetting);

                var ptGrid = new Fanex.PTEngine.PTGrid(customer.ptSetting);
                ptGrid.Render(document.getElementById('ptContainer'));

                var bettingLimit = new Fanex.PTEngine.BettingLimit(customer.bettingLimit)
                bettingLimit.Render(document.getElementById('bettingLimitContainer'));

                /* Render Commission to selected*/
                var commission = customer.commissionSetting;
                Fanex.PTEngine.PT_SetSelect($("#groupA")[0], 0, commission.groupAMax, Fanex.PTEngine.COMMISSION_STEP, commission.groupA);
                Fanex.PTEngine.PT_SetSelect($("#discount1x2")[0], 0, commission.discount1x2Max, Fanex.PTEngine.COMMISSION_STEP, commission.discount1x2);
                Fanex.PTEngine.PT_SetSelect($("#discountCS")[0], 0, commission.discountCSMax, Fanex.PTEngine.COMMISSION_STEP, commission.discountCS);
                Fanex.PTEngine.PT_SetSelect($("#groupB")[0], 0, commission.groupBMax, Fanex.PTEngine.COMMISSION_STEP, commission.groupB);
                Fanex.PTEngine.PT_SetSelect($("#groupC")[0], 0, commission.groupCMax, Fanex.PTEngine.COMMISSION_STEP, commission.groupC);
                Fanex.PTEngine.PT_SetSelect($("#groupD")[0], 0, commission.groupDMax, Fanex.PTEngine.COMMISSION_STEP, commission.groupD);   

                Fanex.Customer.SetNextCustomer(document.getElementById("selectedOne"), document.getElementById("selectedTwo"), null, customer.newUserName);

                // For Check Edit Postion Taking
                if (context.targetCustId > 0) {
                    $this.currentStatusClose = customer.statusSetting.closed;
                    $this.currentStatusSuspend = customer.statusSetting.suspended;
                    $("#minCreditId").attr("class", "");
                    customer.minCreditTemp = String.prototype.FormatNumber(customer.minCredit);
                    $('#allowautoptId').css('display', 'none');
                }
                else {
                    $("#minCreditId").attr("class", "hide");
                    $('#closedId').css('display', 'none');
                    customer.minCreditTemp = 0;
                }

                var CustomerModel = function (customer) {
                    var uplineUserName = customer.userName;

                    // Check Transfer condition
                    displayTransferCondition = Fanex.Customer.DisplayTransferCondition(customer.transferOption);

                    // Super allow master using auto master PT
                    customer.statusSetting.disableAutoMPT = ko.observable(true);

                    // Set label MaxCredit
                    customer.maxCreditTemp = String.prototype.FormatNumber(customer.maxCredit);
                    customer.credit = String.prototype.FormatNumber(customer.credit);

                    this.saveCustomer = function () {
                        $('#contentSplitter').scrollTop(0);
                        if (!Fanex.Customer.CheckInfoValue()) return;
                        if (!bettingLimit.Validate(result.defaultMinBetValue)) return;

                        if (typeof (customer.statusSetting.disableAutoMPT) == 'function')
                            customer.statusSetting.disableAutoMPT = customer.statusSetting.disableAutoMPT();
                        else
                            customer.statusSetting.disableAutoMPT = customer.statusSetting.disableAutoMPT;

                        var max = parseFloat(Fanex.ExtAcc.ReplaceAll($('#maxCredit').text(), ',', ''));
                        var cur = parseFloat(Fanex.ExtAcc.ReplaceAll($('#credit').val(), ',', ''));

                        if (cur > max) {
                            Validators.show($('#credit').get(0), 'Master Max Credit must be less than or equals to ' + $('#maxCredit').text());
                            return;
                        }

                        // Set value to transfer option
                        customer.transferOption = Fanex.Customer.TransferOption(customer.transferOption);
                        customer.commissionSetting = Fanex.Customer.SetValueCommissionSettings(customer.commissionSetting);
                        customer.credit = Fanex.RoundNumber(parseFloat($("#credit").val(), true), 1); // Fix bug for IE7

                        // For Check Edit Postion Taking
                        if (context.targetCustId == 0) {
                            // Set next to userName
                            customer.userName = uplineUserName + Fanex.Customer.SetAutoUserName();
                        }

                        // EscapeHTML String
                        customer.firstName = customer.firstName.escapeHtml().encodeHtml();
                        customer.lastName = customer.lastName.escapeHtml().encodeHtml();
                        customer.phone = customer.phone.escapeHtml().encodeHtml();
                        customer.mobilePhone = customer.mobilePhone.escapeHtml().encodeHtml();
                        customer.fax = customer.fax.escapeHtml().encodeHtml();

                        if (ptGrid) {
                            customer.ptSetting = ptGrid.Save();
                            customer.bettingLimit = bettingLimit.Save();

                            var text = JSON.stringify(customer);
                            var para = {
                                custId: 0,
                                parentId: $('#parentId').val(),
                                txtCustomerObject: text,
                                groupName: $('#selectedGroupName').html()
                            };
                        }

                        Fanex.ExtAcc.BlockUI();
                        $.ajax({
                            url: resolveClientUrl('Customer/SuperAddNewMasterPost'),
                            data: para,
                            type: 'post'
                        }).success(function (result) {

                            if (Fanex.ExtAcc.ShowSecurityCodePage(result, Fanex.ExtAcc.UnBlockUI)) {
                                return;
                            }

                            if (result.errorCode == 0) {
                                Fanex.ExtAcc.LoadMainFrame(resolveClientUrl("Customer/Settings/3/" + result.custId + "/" + $('#parentId').val() + "/" + $('#parentUsername').val()));
                            }
                            else {
                                Fanex.ExtAcc.UnBlockUI();

                                $('#errMsg').css('display', 'block');
                                $('#errMsg').html(result.errorMessage.formatErrorMessage());
                            }
                        });
                    };
                };

                ko.applyBindings(new CustomerModel(customer));

                Fanex.ExtAcc.UnBlockUI();
            });
    },

    // Check or UnCheck Close Status
    Closed: function (checkBoxClose, currentStatusClose) {
        if (checkBoxClose.checked == true && checkBoxClose.checked != currentStatusClose) {
            if (!confirm(Language.ALERT_CONFIRM_CLOSE)) {
                $this.customer.statusSetting.closed = false;
                checkBoxClose.checked = false;
            }
        }
        else if (checkBoxClose.checked == false && checkBoxClose.checked != currentStatusClose) {
            if (!confirm(Language.ALERT_CONFIRM_OPEN)) {
                $this.customer.statusSetting.closed = true;
                checkBoxClose.checked = true;
            }
        }
    },

    // Check or UnCheck Suspend Status
    Suspended: function (checkBoxSuspend, currentStatusSuspend) {
        if (checkBoxSuspend.checked == true && checkBoxSuspend.checked != currentStatusSuspend) {
            if (!confirm(Language.ALERT_CONFIRM_SUSPEND)) {
                $this.customer.statusSetting.suspended = false;
                checkBoxSuspend.checked = false;
            }
        }
        else if (checkBoxSuspend.checked == false && checkBoxSuspend.checked != currentStatusSuspend) {
            if (!confirm(Language.ALERT_CONFIRM_UNSUSPEND)) {
                $this.customer.statusSetting.suspended = true;
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

    // Get selected and m4 value of Soccer HdpD Master Preset
    MemberMaximumPT: function () {
        var ptSetting = $this.customer.ptSetting;
        var s4 = ptSetting.sportTypes[0].betTypeGroupSets[0].betTypeGroups[0].betTypes[0].pt.s4;
        var m3 = ptSetting.sportTypes[0].betTypeGroupSets[0].betTypeGroups[0].betTypes[0].pt.m3;
        return Fanex.Round(s4 - m3);
    },

    // Confirm new value for all member before transfer to server
    SetNewValueToMember: function (isUpdateAllMember) {
        isShowPopUpSetPTMember = true;
        if (isUpdateAllMember) {
            var valueMemberPositionTaking = $("#selectedMemberPositionTaking").val();
            newValueMemberPT = valueMemberPositionTaking + ";" + isUpdateAllMember;
        }
        var customerModel = new CustomerModel($this.customer);
        customerModel.saveCustomer();
    },

    ConfirmYes: function () {
        this.SetNewValueToMember(1);
    },

    ConfirmNo: function () {
        this.SetNewValueToMember(0);
    }
}

$(document).ready(function () {
    Fanex.ExtAcc.Customer.SuperAddNewMaster.Init();
});