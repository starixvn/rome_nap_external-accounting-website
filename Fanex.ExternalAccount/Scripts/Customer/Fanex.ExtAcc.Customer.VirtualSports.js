/// <reference path="../_references.js" />
if (typeof Fanex == 'undefined' || typeof Fanex.ExtAcc == 'undefined') {
    alert("Fanex.ExtAcc.js module is required!")
}

Fanex.ExtAcc.Customer.VirtualSports = {
    oldCustomer: null,
    newCustomer: null,
    uplineId: null,
    uplineRoleId: null,
    custids: null,
    usernames: null,
    updateType: null,
    ptGrid: null,
    bettingLimit1: null,
    otherSetting: {
        'isupdateallmembers': false
    },
    UpdateType: {
        "All": 0,
        "Status": 1,
        "BetSetting": 2,
        "PositionTaking": 3
    },
    isSuccess: true,

    InitArrays: function () {
        if (this.custids == null || this.custids == undefined) {
            this.custids = Fanex.ExtAcc.Customer.CustIds.split(',');
            this.custids.pop();
            this.usernames = Fanex.ExtAcc.Customer.Usernames.split(', ');
            this.usernames.pop();

            if (this.custids.length == 0) {
                this.custids = [context.targetCustId];
                this.usernames = [customer.userName];
            }
        }
    },

    IsMultiple: function () {
        return Fanex.ExtAcc.Customer.IsMultiple;
    },

    CustomerModel: function (customer) {
        customer.userNames = Fanex.ExtAcc.Customer.VirtualSports.usernames.join(", ");
        var updateType;

        this.saveCustomer = function () {
            $('#windowPopup').scrollTop(0);
            updateType = Fanex.ExtAcc.Customer.VirtualSports.UpdateType.All;
            this.save.call(Fanex.ExtAcc.Customer.VirtualSports);
        }

        this.saveStatus = function () {
            $('#windowPopup').scrollTop(0);
            updateType = Fanex.ExtAcc.Customer.VirtualSports.UpdateType.Status;
            this.save.call(Fanex.ExtAcc.Customer.VirtualSports);
        }

        this.saveBettingLimit = function () {
            $('#windowPopup').scrollTop(0);
            updateType = Fanex.ExtAcc.Customer.VirtualSports.UpdateType.BetSetting;
            this.save.call(Fanex.ExtAcc.Customer.VirtualSports);
        }

        this.savePositionTaking = function () {
            $('#windowPopup').scrollTop(0);
            updateType = Fanex.ExtAcc.Customer.VirtualSports.UpdateType.PositionTaking;
            this.save.call(Fanex.ExtAcc.Customer.VirtualSports);
        }

        this.save = function () {
            $('#windowPopup').scrollTop(0);
            this.updateType = updateType;
            this.newCustomer = Fanex.copy(this.oldCustomer);
            if (this.ptGrid != null) {
                // get status settings
                if (updateType == Fanex.ExtAcc.Customer.VirtualSports.UpdateType.All || updateType == Fanex.ExtAcc.Customer.VirtualSports.UpdateType.Status) {
                    var disableDownlinesVirtualSports = true;
                    this.newCustomer.statusSetting.disableVirtualSports = !document.getElementById("chkEnabled").checked;
                    if (context.targetRoleId != 1) {
                        this.newCustomer.statusSetting.disableDownlinesVirtualSports = !document.getElementById("chkDownline").checked;
                    }
                }

                // get bettingLimit
                if (updateType == Fanex.ExtAcc.Customer.VirtualSports.UpdateType.All || updateType == Fanex.ExtAcc.Customer.VirtualSports.UpdateType.BetSetting) {
                    if (context.targetRoleId == 1 && (context.roleId == 4 || context.roleId == 3)) {
                        this.newCustomer.bettingLimit = null;
                    }
                    else {
                        if (!this.bettingLimit1.Validate()) {
                            return;
                        }
                        this.newCustomer.bettingLimit = this.bettingLimit1.Save();
                    }
                }

                // get pt setting
                if (updateType == Fanex.ExtAcc.Customer.VirtualSports.UpdateType.All || updateType == Fanex.ExtAcc.Customer.VirtualSports.UpdateType.PositionTaking) {
                    var isupdateallmembers = this.ptGrid.Validate(this.newCustomer.ptSetting, this.IsMultiple());
                    this.newCustomer.ptSetting = this.ptGrid.Save();

                    if (!isupdateallmembers) {
                        this.UpdateSettings();
                    }
                }
                else {
                    this.UpdateSettings();
                }
            }
        }
    },

    IsHideTextBox: function (context) {
        var result = true;
        if (context.roleId - context.targetRoleId == 1 || context.targetRoleId == 4) {
            result = false;
        }

        return result;
    },

    Init: function () {        
        Fanex.ExtAcc.BlockUI();
        this.InitArrays();

        if (!Fanex.ExtAcc.Customer.IsMultiple) {
            $('#multiple').remove();
        } else {
            $('.general').remove();
            Fanex.ExtAcc.Customer.ProcessDisplayMultiple(80);
        }

        this.uplineId = context.custId;
        this.uplineRoleId = context.roleId;
        Fanex.PTVirtualSports.CURRENT_CONTEXT = context;
        Fanex.PTVirtualSports.CURRENT_CONTEXT.ia3 = !customer.statusSetting.disableAutoMPT;

        // decode html
        customer.firstName = this.decodeHtml(customer.firstName);
        customer.lastName = this.decodeHtml(customer.lastName);
        customer.phone = this.decodeHtml(customer.phone);
        customer.mobilePhone = this.decodeHtml(customer.mobilePhone);
        customer.fax = this.decodeHtml(customer.fax);

        this.oldCustomer = Fanex.copy(customer);
        this.oldCustomer.firstName = null;
        this.oldCustomer.lastName = null;
        this.oldCustomer.phone = null;
        this.oldCustomer.mobilePhone = null;
        this.oldCustomer.fax = null;
        this.newCustomer = Fanex.copy(this.oldCustomer);

        if (customer.bettingLimit.bettingLimitItems.length > 0) {
            this.bettingLimit1 = new Fanex.PTVirtualSports.BettingLimit(customer.bettingLimit);
            var bettingLimitContainer = document.getElementById('bettingLimitContainer');
            this.bettingLimit1.Render(bettingLimitContainer, this.IsHideTextBox(context));
        }
        else {
            document.getElementById("bettingLimitContainer").className = "noInfoBlock";
            document.getElementById("bettingLimitContainer").innerHTML = "There is no information available";
            document.getElementById("btnSubmit").disabled = true;
        }

        if (customer.ptSetting.sportTypes.length > 0) {
            this.ptGrid = new Fanex.PTVirtualSports.PTGrid(/*Fanex.copy*/(customer.ptSetting));
            var ptContainer = document.getElementById('positionTaking');
            this.ptGrid.Render(ptContainer);
        }
        else {
            document.getElementById("positionTaking").className = "noInfoBlock";
            document.getElementById("positionTaking").innerHTML = "There is no information available";
            document.getElementById("btnSubmit").disabled = true;
        }

        // using for both of multi & single
        this.InitUI();
        Fanex.ExtAcc.UnBlockUI();
    },

    InitUI: function () {
        ko.applyBindings(new this.CustomerModel(customer));

        if (this.IsHideTextBox(context)) {
            document.getElementById("saveStatus").style.display = "none";
            document.getElementById("saveBetSetting").style.display = "none";
        }

        this.ReloadCheckboxes();

        if (this.IsMultiple()) {
            if (context.targetRoleId == 1) {
                if (context.roleId != 2) {
                    $('#generalInfo').hide();
                }
                else {
                    document.getElementById("divDownline").style.display = "none";
                }
            }
        }
        else {
            if (context.targetRoleId == 1) {
                if (context.roleId == 4) {
                    document.getElementById("divEnabled").style.display = "none";
                    document.getElementById("lblEnabled").innerHTML = "";
                    document.getElementById("divDownline").style.display = "none";
                }
                else if (context.roleId == 3) {
                    document.getElementById("divEnabled").style.display = "none";
                    document.getElementById("lblEnabled").innerHTML = "";
                    document.getElementById("divDownline").style.display = "none";
                }
                else if (context.roleId == 2) {
                    document.getElementById("divDownline").style.display = "none";
                }
            }
        }
    },
    ReloadCheckboxes: function () {
        var chkEnabled = document.getElementById("chkEnabled");
        var chkDownline = document.getElementById("chkDownline");
        var originEnabled = $(chkEnabled).prop("checked");
        var originEnabledDownline = $(chkDownline).prop("checked");
        if (!chkEnabled.checked || (chkEnabled.checked && chkDownline.checked))
            chkDownline.disabled = true;

        if (this.newCustomer.statusSetting.disableUplineVirtualSports === true) {
            chkEnabled.disabled = true;
            chkDownline.disabled = true;
        }

        if (context.targetRoleId != 1) {
            if (!chkEnabled.checked) {
                chkDownline.disabled = true;
            }

            var newCustomer = this.newCustomer;
            chkEnabled.onclick = function () {
                var checked = $('#chkEnabled').prop("checked");
                var checkedDownline = $('#chkDownline').prop("checked");
                var oldvalue = $('#chkEnabled').attr("value") == "true" ? true : false;
                var oldvaluedownline = $("#chkDownline").attr("value") == "true" ? true : false;

                if (checked != oldvalue) {
                    if (checked) {
                        $("#chkDownline").prop("disabled", checkedDownline);
                    }
                    else {
                        $("#chkDownline").prop("disabled", true);
                        $("#chkDownline").prop("checked", false);
                    }
                }
                else {
                    $("#chkDownline").prop("disabled", !oldvalue || oldvaluedownline);
                    $("#chkDownline").prop("checked", oldvaluedownline);
                }
            }
        }
    },
    SetNewValueToMember: function (isupdateallmembers) {
        this.otherSetting.isupdateallmembers = isupdateallmembers;

        if (isupdateallmembers) {
            for (i = 0; i < this.newCustomer.ptSetting.sportTypes.length; i++) {
                var sportType = this.newCustomer.ptSetting.sportTypes[i];
                for (j = 0; j < sportType.betTypeGroupSets.length; j++) {
                    var betTypeGroupSet = sportType.betTypeGroupSets[j];
                    for (k = 0; k < betTypeGroupSet.betTypeGroups.length; k++) {
                        var betTypeGroup = betTypeGroupSet.betTypeGroups[k];
                        for (l = 0; l < betTypeGroup.betTypes.length; l++) {
                            var betType = betTypeGroup.betTypes[l];
                            switch (context.targetRoleId) {
                                case 3: // super edit master
                                    betType.pt.s1 = Popups.GetSportPTValue(sportType.id);
                                    break;
                                case 2: // master edit agent
                                    betType.pt.m1 = Popups.GetSportPTValue(sportType.id);
                                    break;
                            }
                        }
                    }
                }
            }
        }

        this.UpdateSettings();
        Popups.Close();
    },
    UpdateSettings: function () {
        this.isSuccess = true;
        Fanex.ExtAcc.BlockUI();
        $('#errMsg').html('');
        this.UpdateCustSettings(this.IsMultiple() ? 0 : null);
    },
    UpdateCustSettings: function (id) {
        var object = this;
        var custid = id != null ? this.custids[id] : context.targetCustId;
        var username = id != null ? this.usernames[id] : customer.userName;
        var isMulti = false;

        var url = resolveClientUrl("VirtualSports/UpdateCustomerSetting?custId=") + this.uplineId + '&roleId=' + this.uplineRoleId + "&targetCustId=" + custid + "&targetRoleId=" + context.targetRoleId + "&updateType=" + this.updateType + "&isMulti=" + isMulti.toString();
        var parameters = ['oldCustomer=', JSON.stringify(this.oldCustomer), '&newCustomer=', JSON.stringify(this.newCustomer), '&otherSetting=', JSON.stringify(this.otherSetting), '&groupName=' + Fanex.ExtAcc.Customer.GroupName().groupName].join('');

        $.ajax({
            url: url,
            type: 'POST',
            data: parameters,
            dataType: 'json',
            cache: false,
            success: function (result) {
                var ArrUserName = new Array();
                ArrUserName.push(username);

                if (result instanceof Array) {
                    var len = result.length;
                    for (var i = 0; i < len; i++) {
                        var item = result[i];
                        if (item.errorCode >= 0) {
                            Fanex.ExtAcc.Customer.VirtualSports.isSuccess = false;
                        }
                        else if (item.errorCode == -2) { // update status successfully
                            Fanex.ExtAcc.Customer.VirtualSports.ReloadCheckboxes();
                        }
                    }
                }
                else if (result.errorCode >= 0) {
                    Fanex.ExtAcc.Customer.VirtualSports.isSuccess = false;
                }

                $('#errMsg').css('display', 'block');
                $('#errMsg').append(Fanex.ExtAcc.FormatErrorMessage(ArrUserName, result));

                if (id != null) {
                    id++;
                    if (id < object.custids.length) {
                        Fanex.ExtAcc.Customer.VirtualSports.UpdateCustSettings(id);
                    }
                    else {
                        if (Fanex.ExtAcc.Customer.VirtualSports.isSuccess) {
                            object.oldCustomer = object.newCustomer;
                            ''.formatSuccessMessage();
                        }
                        else {
                            ''.formatErrorMessage();
                        }
                        Fanex.ExtAcc.UnBlockUI();
                        $('#windowPopup').scrollTop(0);
                    }
                }
                else {
                    Fanex.ExtAcc.UnBlockUI();
                }
            }});
    },
    afterUpdateSuccess: function (timeout) {
        t1 = setTimeout(function () {
            // $('#errMsg').css('display', 'none');
            // $('#errMsg').html('');
            Fanex.ExtAcc.Form.Close(false);
        }, timeout);
    },
    decodeHtml: function (value) {
        if (!value || value.length === 0) {
            return '';
        }
        return Fanex.decodeHtml(value);
    }
}

function ConfirmYes() {
    Fanex.ExtAcc.Customer.VirtualSports.SetNewValueToMember(true);
}

function ConfirmNo() {
    Fanex.ExtAcc.Customer.VirtualSports.SetNewValueToMember(false);
}

$(document).ready(function () {
    Fanex.ExtAcc.Customer.VirtualSports.Init();
});

//@ sourceURL=Fanex.ExtAcc.Customer.VirtualSports.js