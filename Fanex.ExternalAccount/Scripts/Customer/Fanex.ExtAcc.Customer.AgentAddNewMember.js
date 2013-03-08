/// <reference path="../_references.js" />
if (typeof Fanex == 'undefined' || typeof Fanex.ExtAcc == 'undefined') {
    alert("Fanex.ExtAcc.js module is required!")
}

Fanex.ExtAcc.AgentAddNewMember = {
    customer: null,
    context: null,
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

        $.ajax({
            url: resolveClientUrl('Customer/AgentAddNewMemberGet'),
            data: data,
            dataType: 'json',
            type: 'post'
        })
        .success(function (result) {
            customer = result.customer;
            context = result.context;

            if (context.targetCustId == 0) {
                $('#group').change(function (e) {
                    $this.ChangeGroupCommission(document.getElementById('group'));
                });
            }
            else {
                $('#closed').click(function (e) {
                    $this.Closed(document.getElementById('closed'), $this.currentStatusClose);
                });

                $('#suspended').click(function (e) {
                    $this.Suspended(document.getElementById('suspended'), $this.currentStatusSuspend);
                });
            }

            Fanex.Customer.CheckUser(customer.userName, 'selectedOne', 'selectedOne', 'selectedTwo', 'selectedThree', 'msgIcon');
            Fanex.Customer.CheckUser(customer.userName, 'selectedTwo', 'selectedOne', 'selectedTwo', 'selectedThree', 'msgIcon');
            Fanex.Customer.CheckUser(customer.userName, 'selectedThree', 'selectedOne', 'selectedTwo', 'selectedThree', 'msgIcon');

            Fanex.PTEngine.CURRENT_CONTEXT = context;
            Fanex.PTEngine.CURRENT_CONTEXT.ia3 = !customer.statusSetting.disableAutoMPT;

            var ptGrid = new Fanex.PTEngine.PTGrid(customer.ptSetting);
            ptGrid.Render(document.getElementById('ptContainer'));

            var bettingLimit = new Fanex.PTEngine.BettingLimit(customer.bettingLimit)
            bettingLimit.Render(document.getElementById('bettingLimitContainer'));

            /* Render Commission to selected*/
            var commission = customer.commissionSetting;
            if (context.targetCustId == 0) {
                Fanex.Customer.SetNextCustomer(document.getElementById("selectedOne"), document.getElementById("selectedTwo"), document.getElementById("selectedThree"), customer.newUserName);
                $("#textBoxDiscountAsian").val(commission.groupA);
                Fanex.ExtAcc.AgentAddNewMember.LoadGroupCommission(document.getElementById("group"), context.currencyId, context.siteName);

                Fanex.PTEngine.PT_SetSelect($("#discountAsian")[0], 0, commission.groupAMax, Fanex.PTEngine.COMMISSION_STEP, commission.groupA);
                Fanex.PTEngine.PT_SetSelect($("#discount1x2")[0], 0, commission.discount1x2Max, Fanex.PTEngine.COMMISSION_STEP, commission.discount1x2);
                Fanex.PTEngine.PT_SetSelect($("#discountCS")[0], 0, commission.discountCSMax, Fanex.PTEngine.COMMISSION_STEP, commission.discountCS);
                Fanex.PTEngine.PT_SetSelect($("#discountNumber")[0], 0, commission.discountNumberMax, Fanex.PTEngine.COMMISSION_STEP, commission.discountNumber);
            }
            else {
                $("#textBoxDiscountAsian").val(commission.discount);
                Fanex.PTEngine.PT_SetSelect($("#discountAsian")[0], 0, commission.discount, Fanex.PTEngine.COMMISSION_STEP, commission.playerDiscount);
                Fanex.PTEngine.PT_SetSelect($("#discount1x2")[0], 0, commission.discount1x2, Fanex.PTEngine.COMMISSION_STEP, commission.playerDiscount1x2);
                Fanex.PTEngine.PT_SetSelect($("#discountCS")[0], 0, commission.discountCS, Fanex.PTEngine.COMMISSION_STEP, commission.playerDiscountCs);
                Fanex.PTEngine.PT_SetSelect($("#discountNumber")[0], 0, commission.discountNumber, Fanex.PTEngine.COMMISSION_STEP, commission.playerDiscountNumber);

                $this.currentStatusClose = customer.statusSetting.closed;
                $this.currentStatusSuspend = customer.statusSetting.suspended;
            }
            
            var CustomerModel = function () {

                var uplineUserName = customer.userName;

                // Set label MaxCredit
                customer.maxCredit = 0;
                customer.credit = String.prototype.FormatNumber(customer.credit);
                customer.agentMaxCreditTemp = String.prototype.FormatNumber(customer.maxCredit);

                this.saveCustomer = function () {          
                    if (!Fanex.Customer.CheckInfoValue()) return;
                    if (!bettingLimit.Validate()) return;
                    
                    // For Check Edit Postion Taking
                    if (context.targetCustId == 0) {
                        // Re-set value for customer object
                        customer.userName = uplineUserName + Fanex.Customer.SetAutoUserName();
                        customer.group = $("#group").val();
                    }

                    customer.commissionSetting.playerDiscount = $("#discountAsian").val();
                    customer.commissionSetting.playerDiscount1x2 = $("#discount1x2").val();
                    customer.commissionSetting.playerDiscountCs = $("#discountCS").val();
                    customer.commissionSetting.playerDiscountNumber = $("#discountNumber").val();
                    customer.credit = Fanex.RoundNumber(parseFloat($("#credit").val(), true), 1); // Fix bug for IE7

                    // EscapeHTML String                     
                    customer.firstName = customer.firstName.escapeHtml();
                    customer.lastName = customer.lastName.escapeHtml();
                    customer.phone = customer.phone.escapeHtml();
                    customer.mobilePhone = customer.mobilePhone.escapeHtml();
                    customer.fax = customer.fax.escapeHtml();

                    if (ptGrid) {
                        customer.ptSetting = ptGrid.Save();
                        customer.bettingLimit = bettingLimit.Save();
            
                        var text = JSON.stringify(customer);
                        var para = {
                            custId: 0,
                            parentId: $('#parentId').val(),
                            txtCustomerObject: text
                        };
                    }

                    Fanex.ExtAcc.BlockUI();
                    $.ajax({
                        url: resolveClientUrl('Customer/AgentAddNewMemberPost'),
                        data: para,
                        dataType: 'json',
                        type: 'post'
                    })
                    .success(function (result) {
                        if (result.ErrorCode == 0) {
                            var timeout = setTimeout(function () {
                                clearTimeout(timeout);
                                $.unblockUI(document);
                                document.location.href = resolveClientUrl("Customer/CustomerList/ACCTHAI");
                                Fanex.ExtAcc.Form.Close(false);
                            }, 2000);
                        }
                        else {
                            $.unblockUI(document);

                            $('#errMsg').css('display', 'block');
                            $('#errMsg').html(result.ErrorMessage);
                        }
                    })
                    .error(function (xhr, status, err) {
                        $.unblockUI(document);
                    });
                }
            }

            ko.applyBindings(new CustomerModel());

            $.unblockUI(document);
        })
        .error(function (xhr, status, err) {
            $.unblockUI(document);
        });
    },

    // Check or UnCheck Close Status
    Closed: function (checkBoxClose, currentStatusClose) {
        if (checkBoxClose.checked == true && checkBoxClose.checked != currentStatusClose) {
            if (!confirm(Language.ALERT_CONFIRM_CLOSE_MEMBER)) {
                customer.statusSetting.closed = false;
                checkBoxClose.checked = false;
            }
        }
        else if (checkBoxClose.checked == false && checkBoxClose.checked != currentStatusClose) {
            if (!confirm(Language.ALERT_CONFIRM_CLOSE_MEMBER)) {
                customer.statusSetting.closed = true;
                checkBoxClose.checked = true;
            }
        }
    },

    // Check or UnCheck Suspend Status
    Suspended: function (checkBoxSuspend, currentStatusSuspend) {
        if (checkBoxSuspend.checked == true && checkBoxSuspend.checked != currentStatusSuspend) {
            if (!confirm(Language.ALERT_CONFIRM_SUSPEND_MEMBER)) {
                customer.statusSetting.suspended = false;
                checkBoxSuspend.checked = false;
            }
        }
        else if (checkBoxSuspend.checked == false && checkBoxSuspend.checked != currentStatusSuspend) {
            if (!confirm(Language.ALERT_CONFIRM_UNSUSPEND_MEMBER)) {
                customer.statusSetting.suspended = true;
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
            $("#textBoxDiscountAsian").val(commission.groupA);
            this.SelectedAsianCommissionMember(commission.groupA)
        }
        if ((group.options[group.selectedIndex].text == "b") || (group.options[group.selectedIndex].text == "hkb")
		|| (group.options[group.selectedIndex].text == "indob") || (group.options[group.selectedIndex].text == "deb")) {
            $("#textBoxDiscountAsian").val(commission.groupB);
            this.SelectedAsianCommissionMember(commission.groupB)
        }
        if ((group.options[group.selectedIndex].text == "c") || (group.options[group.selectedIndex].text == "hkc")
		|| (group.options[group.selectedIndex].text == "indoc") || (group.options[group.selectedIndex].text == "dec")) {
            $("#textBoxDiscountAsian").val(commission.groupC);
            this.SelectedAsianCommissionMember(commission.groupC)
        }
        if ((group.options[group.selectedIndex].text == "d") || (group.options[group.selectedIndex].text == "hkd")
		|| (group.options[group.selectedIndex].text == "indod") || (group.options[group.selectedIndex].text == "ded")) {
            $("#textBoxDiscountAsian").val(commission.groupD);
            this.SelectedAsianCommissionMember(commission.groupD)
        }
    }
}

$(document).ready(function () {
    Fanex.ExtAcc.AgentAddNewMember.Init();    
});