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
            var commission = result.commission;
            var context = result.context;

            if (commission.roleId == 1) {
                $("#textBoxDiscountAsian").val(commission.discount);
                $("#textBoxDiscount1x2").val(commission.discount1x2);
                $("#textBoxDiscountCS").val(commission.discountCS);
                $("#textBoxDiscountNumber").val(commission.discountNumber);
                Fanex.PTEngine.PT_SetSelect($("#discountAsian")[0], 0, commission.discount, Fanex.PTEngine.COMMISSION_STEP, commission.playerDiscount);
                Fanex.PTEngine.PT_SetSelect($("#discount1x2")[0], 0, commission.discount1x2, Fanex.PTEngine.COMMISSION_STEP, commission.playerDiscount1x2);
                Fanex.PTEngine.PT_SetSelect($("#discountCS")[0], 0, commission.discountCS, Fanex.PTEngine.COMMISSION_STEP, commission.playerDiscountCs);
                Fanex.PTEngine.PT_SetSelect($("#discountNumber")[0], 0, commission.discountNumber, Fanex.PTEngine.COMMISSION_STEP, commission.playerDiscountNumber);
            }
            else {
                Fanex.PTEngine.PT_SetSelect($("#groupA")[0], 0, commission.groupAMax, Fanex.PTEngine.COMMISSION_STEP, commission.groupA);
                Fanex.PTEngine.PT_SetSelect($("#groupB")[0], 0, commission.groupBMax, Fanex.PTEngine.COMMISSION_STEP, commission.groupB);
                Fanex.PTEngine.PT_SetSelect($("#groupC")[0], 0, commission.groupCMax, Fanex.PTEngine.COMMISSION_STEP, commission.groupC);
                Fanex.PTEngine.PT_SetSelect($("#groupD")[0], 0, commission.groupDMax, Fanex.PTEngine.COMMISSION_STEP, commission.groupD);
                Fanex.PTEngine.PT_SetSelect($("#discount1x2")[0], 0, commission.discount1x2Max, Fanex.PTEngine.COMMISSION_STEP, commission.discount1x2);
                Fanex.PTEngine.PT_SetSelect($("#discountCS")[0], 0, commission.discountCSMax, Fanex.PTEngine.COMMISSION_STEP, commission.discountCS);
                Fanex.PTEngine.PT_SetSelect($("#discountNumber")[0], 0, commission.discountNumberMax, Fanex.PTEngine.COMMISSION_STEP, commission.discountNumber);
            };

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

                    $this.BlockUI();

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

                    $this.BlockUI();

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

                    $this.BlockUI();

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