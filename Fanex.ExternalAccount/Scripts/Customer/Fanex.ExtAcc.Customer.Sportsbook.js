if (typeof Fanex.ExtAcc.Customer.Sportsbook == 'undefined') {
    Fanex.ExtAcc.Customer.Sportsbook = {};
}

Fanex.ExtAcc.Customer.Sportsbook = {
    Init: function () {
        var data = {
            custId: Fanex.ExtAcc.Customer.UserId
        };

        $.ajax({
            url: resolveClientUrl('Sportsbook/EditBetSettingMasterGet'),
            data: data,
            dataType: 'json'
        })
        .success(function (result) {

            var bettingLimit = null;
            var context = result.context;
            bettingLimit = new Fanex.PTEngine.BettingLimit(result.betting);

            var bettingLimitContainer = document.getElementById('bettingLimitContainer');
            bettingLimit.Render(bettingLimitContainer);

            var BetSettingModel = function () {
                this.saveCustomer = function () {
                    if (!bettingLimit.Validate()) return;
                    if (bettingLimit) {
                        betting = bettingLimit.Save();
                        var text = JSON.stringify(betting);

                        var para = {
                            custId: Fanex.ExtAcc.Customer.UserId,
                            txtCustomerObject: text
                        };
                    }

                    $this.BlockUI();

                    $.ajax({
                        url: resolveClientUrl('Sportsbook/EditBetSettingMasterPost'),
                        data: para,
                        dataType: 'json'
                    })
                    .success(function (result) {
                        if (result.ErrorCode == 0) {
                            Fanex.ExtAcc.Form.Close(false);
                            $.unblockUI(document);
                        }
                        else {
                            $('#errMsg').css('display', 'block');
                            $('#errMsg').html(result.ErrorMessage);
                        }
                    })
                    .error(function (xhr, status, err) {

                    });
                }
            }

            ko.applyBindings(new BetSettingModel());

        })
        .error(function (xhr, status, err) {

        });
    }
}

$(document).ready(function () {
    Fanex.ExtAcc.Customer.Sportsbook.Init();
});