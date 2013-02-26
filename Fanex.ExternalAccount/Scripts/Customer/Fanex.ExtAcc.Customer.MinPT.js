if (typeof Fanex.ExtAcc.Customer.MinPT == 'undefined') {
    Fanex.ExtAcc.Customer.MinPT = {};
}

Fanex.ExtAcc.Customer.MinPT = {
    customer: null,
    Init: function () {
        if (Fanex.ExtAcc.Customer.CurrentLevel == Fanex.ExtAcc.Customer.Master) {
            this.GetEditMinPT('SuperEditMinPTMaster');
        }
        else if (Fanex.ExtAcc.Customer.CurrentLevel == Fanex.ExtAcc.Customer.Agent) {
            this.GetEditMinPT('MasterEditMinPTAgent');
        }
    },

    GetEditMinPT: function (editMinPTStatus) {
        Fanex.ExtAcc.BlockUI();

        var data = new Object();
        data.custId = Fanex.ExtAcc.Customer.UserId;
        data.parentId = editMinPTStatus == 'SuperEditMinPTMaster' ? Fanex.ExtAcc.Customer.superId : Fanex.ExtAcc.Customer.masterId;

        $.ajax({
            url: resolveClientUrl('Sportsbook/' + editMinPTStatus + 'Get'),
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
                            txtCustomerObject: text
                        };
                    }

                    $.ajax({
                        url: resolveClientUrl('Sportsbook/' + editMinPTStatus + 'Post'),
                        data: para,
                        dataType: 'json',
                        type: 'post'
                    })
                    .success(function (result) {
                        if (result.ErrorCode == 0) {
                            var timeout = setTimeout(function () {
                                clearTimeout(timeout);
                                $.unblockUI(document);

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
    Fanex.ExtAcc.Customer.MinPT.Init();
});