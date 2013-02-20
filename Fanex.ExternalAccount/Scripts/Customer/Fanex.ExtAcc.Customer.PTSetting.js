if (typeof Fanex.ExtAcc.Customer.PTSetting == 'undefined') {
    Fanex.ExtAcc.Customer.PTSetting = {};
}

Fanex.ExtAcc.Customer.PTSetting = {
    Init: function () {
        if (Fanex.ExtAcc.Customer.CurrentLevel == Fanex.ExtAcc.Customer.Master) {
            this.GetMasterPTSetting();
        }
        else if (Fanex.ExtAcc.Customer.CurrentLevel == Fanex.ExtAcc.Customer.Agent) {
            this.GetAgentPTSetting();
        }
        else {
            this.GetMemberPTSetting();
        }
    },

    GetMasterPTSetting: function () {
        Fanex.ExtAcc.BlockUI();

        var data = new Object();

        data.custId = Fanex.ExtAcc.Customer.UserId;
        data.parentId = Fanex.ExtAcc.Customer.superId;

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
                            parentId: Fanex.ExtAcc.Customer.superId,
                            txtCustomerObject: text
                        };
                    }

                    Fanex.ExtAcc.BlockUI();

                    $.ajax({
                        url: resolveClientUrl('Sportsbook/EditBetSettingPost'),
                        data: para,
                        dataType: 'json'
                    })
                    .success(function (result) {
                        if (result.ErrorCode == 0) {
                            Fanex.ExtAcc.Form.Close(false);
                        }
                        else {
                            $('#errMsg').css('display', 'block');
                            $('#errMsg').html(result.ErrorMessage);
                        }

                        $.unblockUI(document);
                    })
                    .error(function (xhr, status, err) {
                        $.unblockUI(document);
                    });
                }
            }

            ko.applyBindings(new BetSettingModel());

            $.unblockUI(document);
        })
        .error(function (xhr, status, err) {
            $.unblockUI(document);
        });
    },

    GetAgentPTSetting: function () {
        Fanex.ExtAcc.BlockUI();

        var data = new Object();

        data.custId = Fanex.ExtAcc.Customer.UserId;
        data.parentId = Fanex.ExtAcc.Customer.masterId;

        $.ajax({
            url: resolveClientUrl('Sportsbook/EditBetSettingAgentGet'),
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
                            parentId: Fanex.ExtAcc.Customer.masterId,
                            txtCustomerObject: text
                        };
                    }

                    Fanex.ExtAcc.BlockUI();

                    $.ajax({
                        url: resolveClientUrl('Sportsbook/EditBetSettingPost'),
                        data: para,
                        dataType: 'json'
                    })
                    .success(function (result) {
                        if (result.ErrorCode == 0) {
                            Fanex.ExtAcc.Form.Close(false);
                        }
                        else {
                            $('#errMsg').css('display', 'block');
                            $('#errMsg').html(result.ErrorMessage);
                        }

                        $.unblockUI(document);
                    })
                    .error(function (xhr, status, err) {
                        $.unblockUI(document);
                    });
                }
            }

            ko.applyBindings(new BetSettingModel());

            $.unblockUI(document);
        })
        .error(function (xhr, status, err) {
            $.unblockUI(document);
        });
    },
    GetMemberPTSetting: function () {
        Fanex.ExtAcc.BlockUI();

        var data = new Object();
        data.custId = Fanex.ExtAcc.Customer.UserId;
        data.parentId = Fanex.ExtAcc.Customer.agentId;

        $.ajax({
            url: resolveClientUrl('Sportsbook/EditBetSettingMemberGet'),
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
                            parentId: Fanex.ExtAcc.Customer.agentId,
                            txtCustomerObject: text
                        };
                    }

                    Fanex.ExtAcc.BlockUI();

                    $.ajax({
                        url: resolveClientUrl('Sportsbook/EditBetSettingPost'),
                        data: para,
                        dataType: 'json'
                    })
                    .success(function (result) {
                        if (result.ErrorCode == 0) {
                            Fanex.ExtAcc.Form.Close(false);
                        }
                        else {
                            $('#errMsg').css('display', 'block');
                            $('#errMsg').html(result.ErrorMessage);
                        }

                        $.unblockUI(document);
                    })
                    .error(function (xhr, status, err) {
                        $.unblockUI(document);
                    });
                }
            }

            ko.applyBindings(new BetSettingModel());

            $.unblockUI(document);
        })
        .error(function (xhr, status, err) {
            $.unblockUI(document);
        });
    }
}

$(document).ready(function () {
    Fanex.ExtAcc.Customer.PTSetting.Init();
});