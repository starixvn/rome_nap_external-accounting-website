/// <reference path="../_references.js" />
if (typeof Fanex == 'undefined' || typeof Fanex.ExtAcc == 'undefined') {
    alert("Fanex.ExtAcc.js module is required!")
}

if (typeof Fanex.ExtAcc.Customer == 'undefined') {
    Fanex.ExtAcc.Customer = {};
}

Fanex.ExtAcc.Customer.AgentAddNewMember = {
    customer: null,
    context: null,
    commission: null,
    currentStatusClose: 1,
    currentStatusSuspend: 1,
    $this: null,
    Init: function () {
        $this = this;
        extendPTEngine();
        this.AgentAddNewMember();
    },
    AgentAddNewMember: function () {
        Fanex.ExtAcc.BlockUI();

        var data = new Object();
        data.custId = 0;
        data.parentId = $('#parentId').val();
        data.parentUsername = $('#parentUsername').val();
        data.parentCurrencyId = $('#parentCurrencyId').val();

        $.ajax({
            url: resolveClientUrl('Customer/AgentAddNewMemberGet'),
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

                if (context.targetCustId == 0) {
                    $('#group').change(function (e) {
                        Fanex.ExtAcc.Customer.AgentAddNewMember.ChangeGroupCommission(document.getElementById('group'));
                    });
                }
                else {
                    $('#closed').click(function (e) {
                        Fanex.ExtAcc.Customer.AgentAddNewMember.Closed(document.getElementById('closed'), $this.currentStatusClose);
                    });
                }

                Fanex.Customer.CheckUser(2, customer.userName, 'selectedOne', 'selectedOne', 'selectedTwo', 'selectedThree', 'msgIcon');
                Fanex.Customer.CheckUser(2, customer.userName, 'selectedTwo', 'selectedOne', 'selectedTwo', 'selectedThree', 'msgIcon');
                Fanex.Customer.CheckUser(2, customer.userName, 'selectedThree', 'selectedOne', 'selectedTwo', 'selectedThree', 'msgIcon');

                Fanex.PTEngine.CURRENT_CONTEXT = context;
                Fanex.PTEngine.CURRENT_CONTEXT.ia3 = !customer.statusSetting.disableAutoMPT;

                var ptGrid = new Fanex.PTEngine.PTGrid(customer.ptSetting);
                ptGrid.Render(document.getElementById('ptContainer'));

                var bettingLimit = new Fanex.PTEngine.BettingLimit(customer.bettingLimit)
                bettingLimit.Render(document.getElementById('bettingLimitContainer'));

                /* Render Commission to selected*/
                var commission = $this.commission = customer.commissionSetting;
                if (context.targetCustId == 0) {
                    Fanex.Customer.SetNextCustomer(document.getElementById("selectedOne"), document.getElementById("selectedTwo"), document.getElementById("selectedThree"), customer.newUserName);
                    $("#textBoxDiscountAsian").val(commission.groupA);
                    $("#textBoxDiscount1x2").val(commission.discount1x2);
                    $("#textBoxDiscountCS").val(commission.discountCS);
                    Fanex.ExtAcc.Customer.AgentAddNewMember.LoadGroupCommission(document.getElementById("group"), context.currencyId, context.siteName);

                    Fanex.PTEngine.PT_SetSelect($("#discountAsian")[0], 0, commission.groupAMax, Fanex.PTEngine.COMMISSION_STEP, commission.groupA);
                    Fanex.PTEngine.PT_SetSelect($("#discount1x2")[0], 0, commission.discount1x2Max, Fanex.PTEngine.COMMISSION_STEP, commission.discount1x2);
                    Fanex.PTEngine.PT_SetSelect($("#discountCS")[0], 0, commission.discountCSMax, Fanex.PTEngine.COMMISSION_STEP, commission.discountCS);                  
                }
                else {
                    $("#textBoxDiscountAsian").val(commission.discount);
                    Fanex.PTEngine.PT_SetSelect($("#discountAsian")[0], 0, commission.discount, Fanex.PTEngine.COMMISSION_STEP, commission.playerDiscount);
                    Fanex.PTEngine.PT_SetSelect($("#discount1x2")[0], 0, commission.discount1x2, Fanex.PTEngine.COMMISSION_STEP, commission.playerDiscount1x2);
                    Fanex.PTEngine.PT_SetSelect($("#discountCS")[0], 0, commission.discountCS, Fanex.PTEngine.COMMISSION_STEP, commission.playerDiscountCs);               
                    $this.currentStatusClose = customer.statusSetting.closed;
                    $this.currentStatusSuspend = customer.statusSetting.suspended;
                }

                var CustomerModel = function (customer) {
                    var uplineUserName = customer.userName;

                    // Set label MaxCredit
                    customer.credit = String.prototype.FormatNumber(customer.credit);
                    customer.agentMaxCreditTemp = String.prototype.FormatNumber(customer.maxCredit);
                    customer.maxCredit = 0;

                    this.saveCustomer = function () {
                        $('#contentSplitter').scrollTop(0);
                        if (!Fanex.Customer.CheckInfoValue(true)) return;
                        if (!bettingLimit.Validate(result.defaultMinBetValue)) return;

                        // For Check Edit Postion Taking
                        if (context.targetCustId == 0) {
                            // Re-set value for customer object
                            customer.userName = uplineUserName + Fanex.Customer.SetAutoUserName();
                            customer.group = $("#group").val();
                        }

                        var max = parseFloat(Fanex.ExtAcc.ReplaceAll($('#maxCredit').text(), ',', ''));
                        var cur = parseFloat(Fanex.ExtAcc.ReplaceAll($('#credit').val(), ',', ''));
                        if (cur > max) {
                            Validators.show($('#credit').get(0), 'Given Credit must be less than or equals to ' + $('#maxCredit').text());
                            return;
                        }

                        customer.commissionSetting.playerDiscount = $("#discountAsian").val();
                        customer.commissionSetting.playerDiscount1x2 = $("#discount1x2").val();
                        customer.commissionSetting.playerDiscountCs = $("#discountCS").val();                      
                        customer.credit = Fanex.RoundNumber(parseFloat($("#credit").val(), true), 1); // Fix bug for IE7

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
                            url: resolveClientUrl('Customer/AgentAddNewMemberPost'),
                            data: para,
                            type: 'post'
                        })
                            .success(function (result) {

                                if (Fanex.ExtAcc.ShowSecurityCodePage(result, Fanex.ExtAcc.UnBlockUI)) {
                                    return;
                                }

                                if (result.errorCode == 0) {
                                    Fanex.ExtAcc.LoadMainFrame(resolveClientUrl("Customer/Settings/1/" + result.custId + "/" + $('#parentId').val() + "/" + $('#parentUsername').val()));
                                }
                                else {
                                    Fanex.ExtAcc.UnBlockUI();

                                    $('#errMsg').css('display', 'block');
                                    $('#errMsg').html(result.errorMessage.formatErrorMessage());
                                }
                            });
                    }
                }

                ko.applyBindings(new CustomerModel(customer));

                Fanex.ExtAcc.UnBlockUI();
            });
    },

    // Check or UnCheck Close Status
    Closed: function (checkBoxClose, currentStatusClose) {
        if (checkBoxClose.checked == true && checkBoxClose.checked != currentStatusClose) {
            if (!confirm(Language.ALERT_CONFIRM_CLOSE_MEMBER)) {
                $this.customer.statusSetting.closed = false;
                checkBoxClose.checked = false;
            }
        }
        else if (checkBoxClose.checked == false && checkBoxClose.checked != currentStatusClose) {
            if (!confirm(Language.ALERT_CONFIRM_CLOSE_MEMBER)) {
                $this.customer.statusSetting.closed = true;
                checkBoxClose.checked = true;
            }
        }
    },

    // Check or UnCheck Suspend Status
    Suspended: function (checkBoxSuspend, currentStatusSuspend) {
        if (checkBoxSuspend.checked == true && checkBoxSuspend.checked != currentStatusSuspend) {
            if (!confirm(Language.ALERT_CONFIRM_SUSPEND_MEMBER)) {
                $this.customer.statusSetting.suspended = false;
                checkBoxSuspend.checked = false;
            }
        }
        else if (checkBoxSuspend.checked == false && checkBoxSuspend.checked != currentStatusSuspend) {
            if (!confirm(Language.ALERT_CONFIRM_UNSUSPEND_MEMBER)) {
                $this.customer.statusSetting.suspended = true;
                checkBoxSuspend.checked = true;
            }
        }
    },

    // Load Group Commission
    LoadGroupCommission: function (selectId, currencyId, siteName) {
        var listGroupCommission;
        // If Currency name is Won
        if (currencyId == 45) {
            listGroupCommission = new Array("dea", "deb", "dec", "ded");
        }
        else if (siteName == "IBSBET") {
            listGroupCommission = new Array("a", "b", "c", "d", "indoa", "indob", "indoc", "indod");
        }
        else {
            listGroupCommission = new Array("a", "b", "c", "d", "hka", "hkb", "hkc", "hkd");
        }

        for (var i = 0; i < listGroupCommission.length; i++) {
            var selectedOpt = document.createElement("option");
            selectedOpt.text = selectedOpt.value = listGroupCommission[i];
            selectId.options.add(selectedOpt);
        }
    },

    // Set Asian HDP, OU, OE selected value Commission for Member
    SelectedAsianCommissionMember: function (value) {
        Fanex.PTEngine.PT_SetSelect(document.getElementById("discountAsian"), 0, value, Fanex.PTEngine.COMMISSION_STEP, value);
    },

    // Change Group Commission
    ChangeGroupCommission: function (group) {
        if ((group.options[group.selectedIndex].text == "a") || (group.options[group.selectedIndex].text == "hka")
            || (group.options[group.selectedIndex].text == "indoa") || (group.options[group.selectedIndex].text == "dea")) {
            $("#textBoxDiscountAsian").val($this.commission.groupA);
            this.SelectedAsianCommissionMember($this.commission.groupA)
        }
        if ((group.options[group.selectedIndex].text == "b") || (group.options[group.selectedIndex].text == "hkb")
            || (group.options[group.selectedIndex].text == "indob") || (group.options[group.selectedIndex].text == "deb")) {
            $("#textBoxDiscountAsian").val($this.commission.groupB);
            this.SelectedAsianCommissionMember($this.commission.groupB)
        }
        if ((group.options[group.selectedIndex].text == "c") || (group.options[group.selectedIndex].text == "hkc")
            || (group.options[group.selectedIndex].text == "indoc") || (group.options[group.selectedIndex].text == "dec")) {
            $("#textBoxDiscountAsian").val($this.commission.groupC);
            this.SelectedAsianCommissionMember($this.commission.groupC)
        }
        if ((group.options[group.selectedIndex].text == "d") || (group.options[group.selectedIndex].text == "hkd")
            || (group.options[group.selectedIndex].text == "indod") || (group.options[group.selectedIndex].text == "ded")) {
            $("#textBoxDiscountAsian").val($this.commission.groupD);
            this.SelectedAsianCommissionMember($this.commission.groupD)
        }
    }
}

$(document).ready(function () {
    Fanex.ExtAcc.Customer.AgentAddNewMember.Init();
});