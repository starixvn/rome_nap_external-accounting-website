/// <reference path="../_references.js" />
if (typeof Fanex == 'undefined' || typeof Fanex.ExtAcc == 'undefined') {
    alert("Fanex.ExtAcc.js module is required!")
}

Fanex.ExtAcc.SuperAddNewMaster = {
    customer: null,
    context: null,
    Init: function () {
        this.SuperAddNewMaster();
    },
    SuperAddNewMaster: function () {
        Fanex.ExtAcc.BlockUI();

        var data = new Object();
        data.custId = 0;
        data.parentId = $('#parentId').val();
        data.parentUsername = $('#parentUsername').val();

        $.ajax({
            url: resolveClientUrl('Customer/SuperAddNewMasterGet'),
            data: data,
            dataType: 'json',
            type: 'post'
        })
        .success(function (result) {
            customer = result.customer;
            context = result.context;

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
            Fanex.PTEngine.PT_SetSelect($("#groupB")[0], 0, commission.groupBMax, Fanex.PTEngine.COMMISSION_STEP, commission.groupB);
            Fanex.PTEngine.PT_SetSelect($("#groupC")[0], 0, commission.groupCMax, Fanex.PTEngine.COMMISSION_STEP, commission.groupC);
            Fanex.PTEngine.PT_SetSelect($("#groupD")[0], 0, commission.groupDMax, Fanex.PTEngine.COMMISSION_STEP, commission.groupD);
            Fanex.PTEngine.PT_SetSelect($("#discount1x2")[0], 0, commission.discount1x2Max, Fanex.PTEngine.COMMISSION_STEP, commission.discount1x2);
            Fanex.PTEngine.PT_SetSelect($("#discountCS")[0], 0, commission.discountCSMax, Fanex.PTEngine.COMMISSION_STEP, commission.discountCS);
            Fanex.PTEngine.PT_SetSelect($("#discountNumber")[0], 0, commission.discountNumberMax, Fanex.PTEngine.COMMISSION_STEP, commission.discountNumber);

            Fanex.Customer.SetNextCustomer(document.getElementById("selectedOne"), document.getElementById("selectedTwo"), null, customer.newUserName);

            // For Check Edit Postion Taking
            if (context.targetCustId > 0) {
                var currentStatusClose = customer.statusSetting.closed;
                var currentStatusSuspend = customer.statusSetting.suspended;
                $("#minCreditId").attr("class", "");
                customer.minCreditTemp = String.prototype.FormatNumber(customer.minCredit);
                $('#allowautoptId').css('display', 'none');
            }
            else {
                $("#minCreditId").attr("class", "hide");
                $('#closedId').css('display', 'none');
                customer.minCreditTemp = 0;
            }
            
            var CustomerModel = function () {

                var uplineUserName = customer.userName;

                // Check Transfer condition
                displayTransferCondition = Fanex.Customer.DisplayTransferCondition(customer.transferOption);

                // Super allow master using auto master PT
                customer.statusSetting.disableAutoMPT = ko.observable(true);

                // Set label MaxCredit
                customer.maxCreditTemp = String.prototype.FormatNumber(customer.maxCredit);
                customer.credit = String.prototype.FormatNumber(customer.credit);

                this.saveCustomer = function () {          
                    if (!Fanex.Customer.CheckInfoValue()) return;
                    if (!bettingLimit.Validate()) return;
                    
                    if (typeof (customer.statusSetting.disableAutoMPT) == 'function')
                        customer.statusSetting.disableAutoMPT = customer.statusSetting.disableAutoMPT();
                    else
                        customer.statusSetting.disableAutoMPT = customer.statusSetting.disableAutoMPT;

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
                        url: resolveClientUrl('Customer/SuperAddNewMasterPost'),
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
}

$(document).ready(function () {
    Fanex.ExtAcc.SuperAddNewMaster.Init();
});