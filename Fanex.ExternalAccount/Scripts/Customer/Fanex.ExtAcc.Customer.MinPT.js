/// <reference path="../_references.js" />
if (typeof Fanex == 'undefined' || typeof Fanex.ExtAcc == 'undefined') {
    alert("Fanex.ExtAcc.js module is required!")
}

Validators.show = function (elementInput, message, xPos, pointerXPos, yPos, pointerYPos, height, timeout) {
    // <span class="hint"><span>This is the message.<span><span class="hint-pointer">&nbsp;</span></span>
    var hideTimeout = timeout ? timeout : 1000;
    var hint = document.getElementById('fHint');
    var hintPointer = null;
    if (hint == null) {
        var hint = document.createElement("SPAN");
        hint.className = 'hint';
        var hintMsg = document.createElement("SPAN");
        hint.appendChild(hintMsg);
        hintPointer = document.createElement("SPAN");
        hintPointer.className = 'hint-pointer';
        hint.appendChild(hintPointer);
        hint.id = 'fHint';
        hint.style.top = '10px';
        hint.style.left = '10px';
        hint.style.zIndex = 999999;
        hint.onclick = function () { // Hide onclick
            hint.style.display = 'none';
        };
        document.body.appendChild(hint);
    };
    hintPointer = hint.lastChild;
    hint.firstChild.innerHTML = message;
    if (this.timeOutHideTooltip) {
        clearTimeout(this.timeOutHideTooltip);
        this.timeOutHideTooltip = null;
    };
    this.timeOutHideTooltip = setTimeout(function () {
        hint.style.display = 'none';
    }, hideTimeout); // Auto hide in 1s
    if (elementInput != null) {
        if (typeof xPos == 'undefined') {
            xPos = 0;
        }
        if (typeof pointerXPos == 'undefined') {
            pointerXPos = 10;
        }
        if (typeof yPos == 'undefined') {
            yPos = -45;
        }
        if (typeof (pointerYPos) == 'undefined') {
            pointerYPos = 38;
        }
        if (typeof (height) == 'undefined') {
            height = 18;
        }
        hint.style.height = height + 'px';
        hintPointer.style.left = (pointerXPos + 'px');
        hintPointer.style.top = (pointerYPos + 'px');
        var pos = Validators.utils.findPosRelativeToViewport(elementInput);
        hint.style.left = (pos[0] + xPos) + 'px';
        hint.style.top = (pos[1] + yPos < 0 ? 0 : pos[1] + yPos) + 'px';
        elementInput.onchange = function () {
            hint.style.display = 'none';
        };
        elementInput.focus();
    };
    //hint.style.minHeight = '18px';
    hint.style.display = 'block';
}

Fanex.ExtAcc.Customer.MinPT = {
    customer: null,

    Init: function () {
        if (Fanex.ExtAcc.Customer.CurrentLevel == Fanex.ExtAcc.Customer.Master) {
            Fanex.ExtAcc.Customer.MinPT.GetEditMinPT('SuperEditMinPTMaster');
        }
        else if (Fanex.ExtAcc.Customer.CurrentLevel == Fanex.ExtAcc.Customer.Agent) {
            Fanex.ExtAcc.Customer.MinPT.GetEditMinPT('MasterEditMinPTAgent');
        }        
    },

    GetEditMinPT: function (editMinPTStatus) {
        Fanex.ExtAcc.BlockUI();

        var data = new Object();
        data.custId = Fanex.ExtAcc.Customer.UserId;
        data.parentId = editMinPTStatus == 'SuperEditMinPTMaster' ? Fanex.ExtAcc.Customer.superId : Fanex.ExtAcc.Customer.masterId;
        data.groupName = Fanex.ExtAcc.Customer.GroupName().groupName;

        $.ajax({
            url: resolveClientUrl('SportsbookSetting/' + editMinPTStatus + 'Get'),
            data: data,
            dataType: 'json',
            type: 'post'
        })
        .success(function (result) {
            if (result.errorCode && result.errorCode == "1") {
                Fanex.ExtAcc.UnBlockUI();
                Fanex.ExtAcc.Form.Close();
                Fanex.ExtAcc.Form.Alert('Error', result.errorMessage.formatErrorMessage(), 450, 150);

                return;
            }

            customer = result.customer;
            var context = result.context;

            $('.headerPositionTaking').html($('.headerPositionTaking').html() + ': ' + customer.userName);

            Fanex.PTEngine.CURRENT_CONTEXT = context;

            ptGrid = new Fanex.PTEngine.PTGrid(customer.ptSetting);

            ptGrid.Render(document.getElementById('ptContainer'));

            var CustomerModel = function () {
                this.saveCustomer = function () {
                    $('#windowPopup').scrollTop(0);
                    // EscapeHTML String
                    Fanex.ExtAcc.BlockUI();

                    customer.firstName = customer.firstName.escapeHtml();
                    customer.lastName = customer.lastName.escapeHtml();
                    customer.phone = customer.phone.escapeHtml();
                    customer.mobilePhone = customer.mobilePhone.escapeHtml();
                    customer.fax = customer.fax.escapeHtml();

                    if (ptGrid) {
                        customer.ptSetting = ptGrid.Save();
                        customer.bettingLimit = null;
                        customer.statusSetting = null;
                        customer.commissionSetting = null;
                        customer.commissionSetting2 = null;

                        var text = JSON.stringify(customer);
                        var para = {
                            custId: Fanex.ExtAcc.Customer.UserId,
                            parentId: editMinPTStatus == 'SuperEditMinPTMaster' ? Fanex.ExtAcc.Customer.superId : Fanex.ExtAcc.Customer.masterId,
                            txtCustomerObject: text,
                            groupName: Fanex.ExtAcc.Customer.GroupName().groupName
                        };
                    }

                    $.ajax({
                        url: resolveClientUrl('SportsbookSetting/' + editMinPTStatus + 'Post'),
                        data: para,
                        dataType: 'json',
                        type: 'post'
                    })
                    .success(function (result) {
                        if (result.errorCode == 0) {

                            var timeout = setTimeout(function () {
                                clearTimeout(timeout);
                                Fanex.ExtAcc.UnBlockUI();

                                $('#errMsg').css('display', 'block');
                                $('#errMsg').html('<font color="Blue">Updated successfully</font>');

                                Fanex.ExtAcc.Form.Close(false);
                            }, 2000);
                        }
                        else {
                            Fanex.ExtAcc.UnBlockUI();

                            $('#errMsg').css('display', 'block');
                            $('#errMsg').html(result.errorMessage.formatErrorMessage());
                        }
                    })
                    .error(function (xhr, status, err) {
                        Fanex.ExtAcc.UnBlockUI();
                    });
                }
            }

            ko.applyBindings(new CustomerModel());

            Fanex.ExtAcc.UnBlockUI();
        })
        .error(function (xhr, status, err) {
            Fanex.ExtAcc.UnBlockUI();
        });
    }
}

$(document).ready(function () {
    Fanex.ExtAcc.Customer.MinPT.Init();
});