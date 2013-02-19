if (typeof Fanex.ExtAcc.Customer.Commission == 'undefined') {
    Fanex.ExtAcc.Customer.Commission = {};
}

Fanex.ExtAcc.Customer.Sportsbook = {
    Init: function () {
        if (Fanex.ExtAcc.Customer.CurrentLevel == Fanex.ExtAcc.Customer.Master) {
            this.GetMasterCommission();
        }
        else if (Fanex.ExtAcc.Customer.CurrentLevel == Fanex.ExtAcc.Customer.Agent) {
            this.GetAgentCommission();
        }
        else {
            this.GetMemberCommission();
        }
    },

    GetMasterCommission: function () {
        var data = {
            custId: Fanex.ExtAcc.Customer.UserId,
            parentId: Fanex.ExtAcc.Customer.superId
        };

        $.ajax({
            url: resolveClientUrl('Sportsbook/EditCommissionMasterGet'),
            data: data,
            dataType: 'json'
        })
        .success(function (result) {
            var commission = null;
            var context = result.context;
            commission = new Fanex.PTEngine.BettingLimit(result.betting);

            var commissionContainer = document.getElementById('commissionContainer');
            commission.Render(commissionContainer);

            var CommissionModel = function () {
                this.saveCustomer = function () {
                    if (!commission.Validate()) return;
                    if (commission) {
                        betting = commission.Save();
                        var text = JSON.stringify(betting);

                        var para = {
                            custId: Fanex.ExtAcc.Customer.UserId,
                            parentId: Fanex.ExtAcc.Customer.superId,
                            txtCustomerObject: text
                        };
                    }

                    Fanex.ExtAcc.BlockUI();

                    $.ajax({
                        url: resolveClientUrl('Sportsbook/EditCommissionPost'),
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

            ko.applyBindings(new CommissionModel());
        })
        .error(function (xhr, status, err) {
            $.unblockUI(document);
        });
    },

    GetAgentCommission: function () {
        var data = {
            custId: Fanex.ExtAcc.Customer.UserId,
            parentId: Fanex.ExtAcc.Customer.masterId
        };

        $.ajax({
            url: resolveClientUrl('Sportsbook/EditCommissionAgentGet'),
            data: data,
            dataType: 'json'
        })
        .success(function (result) {
            var commission = null;
            var context = result.context;
            commission = new Fanex.PTEngine.BettingLimit(result.betting);

            var commissionContainer = document.getElementById('commissionContainer');
            commission.Render(commissionContainer);

            var CommissionModel = function () {
                this.saveCustomer = function () {
                    if (!commission.Validate()) return;
                    if (commission) {
                        betting = commission.Save();
                        var text = JSON.stringify(betting);

                        var para = {
                            custId: Fanex.ExtAcc.Customer.UserId,
                            parentId: Fanex.ExtAcc.Customer.masterId,
                            txtCustomerObject: text
                        };
                    }

                    Fanex.ExtAcc.BlockUI();

                    $.ajax({
                        url: resolveClientUrl('Sportsbook/EditCommissionPost'),
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

            ko.applyBindings(new CommissionModel());
        })
        .error(function (xhr, status, err) {
            $.unblockUI(document);
        });
    },
    GetMemberCommission: function () {
        var data = {
            custId: Fanex.ExtAcc.Customer.UserId,
            parentId: Fanex.ExtAcc.Customer.agentId
        };

        $.ajax({
            url: resolveClientUrl('Sportsbook/EditCommissionMemberGet'),
            data: data,
            dataType: 'json'
        })
        .success(function (result) {
            var commission = null;
            var context = result.context;
            commission = new Fanex.PTEngine.BettingLimit(result.betting);

            var commissionContainer = document.getElementById('commissionContainer');
            commission.Render(commissionContainer);

            var CommissionModel = function () {
                this.saveCustomer = function () {
                    if (!commission.Validate()) return;
                    if (commission) {
                        betting = commission.Save();
                        var text = JSON.stringify(betting);

                        var para = {
                            custId: Fanex.ExtAcc.Customer.UserId,
                            parentId: Fanex.ExtAcc.Customer.agentId,
                            txtCustomerObject: text
                        };
                    }

                    Fanex.ExtAcc.BlockUI();

                    $.ajax({
                        url: resolveClientUrl('Sportsbook/EditCommissionPost'),
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

            ko.applyBindings(new CommissionModel());
        })
        .error(function (xhr, status, err) {
            $.unblockUI(document);
        });
    }
}

$(document).ready(function () {
    Fanex.ExtAcc.Customer.Sportsbook.Init();
});