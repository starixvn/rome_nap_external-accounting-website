/// <reference path="../_references.js" />

if (typeof Fanex == 'undefined' || typeof Fanex.ExtAcc == 'undefined') {
    alert("Fanex.ExtAcc.js module is required!");
};

Fanex.ExtAcc.Customer.Casino = {
    customer: null,

    Init: function () {
        if (!Fanex.ExtAcc.Customer.IsMultiple) {
            $('#arrowUpDown').remove();
        }

        this.LoadData();
    },

    LoadData: function () {
        var uplineId = 0;

        switch (Fanex.ExtAcc.Customer.CurrentLevel) {
            case Fanex.ExtAcc.Customer.Master:
                uplineId = Fanex.ExtAcc.Customer.superId;
                break;
            case Fanex.ExtAcc.Customer.Agent:
                uplineId = Fanex.ExtAcc.Customer.masterId;
                break;
            case Fanex.ExtAcc.Customer.Member:
                uplineId = Fanex.ExtAcc.Customer.agentId;
                break;
        }

        $.ajax({
            url: resolveClientUrl('Casino/EditCasinoPost'),
            type: 'post',
            data: { custId: Fanex.ExtAcc.Customer.UserId, uplineId: uplineId },
            dataType: 'json'
        })
        .success(function (result) {
            console.log(result);
            Fanex.ExtAcc.Customer.Casino.customer = result;

            Fanex.ExtAcc.Customer.Casino.CasinoModel = function () {
                this.saveCustomer = function () {
                    alert('Submit');
                }
            }

            ko.applyBindings(new Fanex.ExtAcc.Customer.Casino.CasinoModel());
        })
        .error(function (data) {
            if (typeof console != undefined) {
                console.log(data.responseText);
            }
        });
    }
}

$(document).ready(function () {
    Fanex.ExtAcc.Customer.Casino.Init();
});