/// <reference path="../_references.js" />
if (typeof Fanex == 'undefined' || typeof Fanex.ExtAcc == 'undefined') {
    alert("Fanex.ExtAcc.js module is required!")
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

        $.ajax({
            url: resolveClientUrl('SportsbookSetting/' + editMinPTStatus + 'Get'),
            data: data,
            dataType: 'json',
            type: 'post'
        })
        .success(function (result) {
            customer = result.customer;
            var context = result.context;

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