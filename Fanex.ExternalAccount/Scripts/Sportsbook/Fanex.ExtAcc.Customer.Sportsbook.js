if (typeof Fanex.ExtAcc.Sportsbook == 'undefined') {
    Fanex.ExtAcc.Sportsbook = {};
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
                bettingLimit = new Fanex.PTEngine.BettingLimit(result);
                var bettingLimitContainer = document.getElementById('bettingLimitContainer');
                bettingLimit.Render(bettingLimitContainer);

                var BetSettingModel = function () {
                    this.saveCustomer = function () {
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