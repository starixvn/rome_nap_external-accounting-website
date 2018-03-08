/// <reference path="../_references.js" />
if (typeof Fanex == 'undefined' || typeof Fanex.ExtAcc == 'undefined') {
    alert("Fanex.ExtAcc.js module is required!")
}

Fanex.ExtAcc.Customer.BetSetting = {
    Init: function () {
        if (Fanex.ExtAcc.Customer.CurrentLevel == Fanex.ExtAcc.Customer.Master) {
            this.GetMasterBetSetting();
        }
        else if (Fanex.ExtAcc.Customer.CurrentLevel == Fanex.ExtAcc.Customer.Agent) {
            this.GetAgentBetSetting();
        }
        else {
            this.GetMemberBetSetting();
        }
    },

    GetMasterBetSetting: function () {
        Fanex.ExtAcc.BlockUI();

        var data = new Object();

        data.custId = Fanex.ExtAcc.Customer.UserId;
        data.parentId = Fanex.ExtAcc.Customer.superId;
        data.groupName = Fanex.ExtAcc.Customer.GroupName().groupName;

        $.ajax({
            url: resolveClientUrl('SportsbookSetting/EditBetSettingMasterGet'),
            data: data,
            dataType: 'json',
            type: 'post'
        })
        .success(function (result) {
            if (!result.betting) {
                Fanex.ExtAcc.UnBlockUI();
                Fanex.ExtAcc.Form.Close();
                var timeout = setTimeout(function () {
                    clearTimeout(timeout);
                    Fanex.ExtAcc.Form.Alert('Error', "There is no information available".formatErrorMessage(), 450, 150);
                }, 1000);

                return;
            }

            var bettingLimit = null;
            var context = result.context;
            bettingLimit = new Fanex.PTEngine.BettingLimit(result.betting);

            var bettingLimitContainer = document.getElementById('bettingLimitContainer');
            bettingLimit.Render(bettingLimitContainer);

            var BetSettingModel = function () {
                this.saveCustomer = function () {
                    $('#windowPopup').scrollTop(0);
                    if (!bettingLimit.Validate()) return;

                    Fanex.ExtAcc.BlockUI();

                    if (bettingLimit) {
                        betting = bettingLimit.Save();
                        var text = JSON.stringify(betting);

                        var para = {
                            custId: Fanex.ExtAcc.Customer.UserId,
                            parentId: Fanex.ExtAcc.Customer.superId,
                            txtCustomerObject: text,
                            groupName: Fanex.ExtAcc.Customer.GroupName().groupName
                        };
                    }

                    $.ajax({
                        url: resolveClientUrl('SportsbookSetting/EditBetSettingPost'),
                        data: para,
                        dataType: 'json'
                    })
                    .success(function (result) {
                        if (result.errorCode == 0) {
                            var timeout = setTimeout(function () {
                                clearTimeout(timeout);
                                Fanex.ExtAcc.UnBlockUI();

                                Fanex.ExtAcc.Form.Close(false);
                            }, 2000);
                        }
                        else {
                            Fanex.ExtAcc.UnBlockUI();

                            $('#errMsg').css('display', 'block');
                            $('#errMsg').html(result.errorMessage.formatErrorMessage());
                        }
                    });
                }
            }

            ko.applyBindings(new BetSettingModel());

            Fanex.ExtAcc.UnBlockUI();
        });
    },

    GetAgentBetSetting: function () {
        Fanex.ExtAcc.BlockUI();

        var data = new Object();

        data.custId = Fanex.ExtAcc.Customer.UserId;
        data.parentId = Fanex.ExtAcc.Customer.masterId;
        data.groupName = Fanex.ExtAcc.Customer.GroupName().groupName;

        $.ajax({
            url: resolveClientUrl('SportsbookSetting/EditBetSettingAgentGet'),
            data: data,
            dataType: 'json',
            type: 'post'
        })
        .success(function (result) {
            if (!result.betting) {
                Fanex.ExtAcc.UnBlockUI();
                Fanex.ExtAcc.Form.Close();
                var timeout = setTimeout(function () {
                    clearTimeout(timeout);
                    Fanex.ExtAcc.Form.Alert('Error', "There is no information available".formatErrorMessage(), 450, 150);
                }, 1000);

                return;
            }

            var bettingLimit = null;
            var context = result.context;
            bettingLimit = new Fanex.PTEngine.BettingLimit(result.betting);

            var bettingLimitContainer = document.getElementById('bettingLimitContainer');
            bettingLimit.Render(bettingLimitContainer);

            var BetSettingModel = function () {
                this.saveCustomer = function () {
                    $('#windowPopup').scrollTop(0);
                    if (!bettingLimit.Validate()) return;

                    Fanex.ExtAcc.BlockUI();

                    if (bettingLimit) {
                        betting = bettingLimit.Save();
                        var text = JSON.stringify(betting);

                        var para = {
                            custId: Fanex.ExtAcc.Customer.UserId,
                            parentId: Fanex.ExtAcc.Customer.masterId,
                            txtCustomerObject: text,
                            groupName: Fanex.ExtAcc.Customer.GroupName().groupName
                        };
                    }

                    $.ajax({
                        url: resolveClientUrl('SportsbookSetting/EditBetSettingPost'),
                        data: para,
                        dataType: 'json'
                    })
                    .success(function (result) {
                        if (result.errorCode == 0) {
                            var timeout = setTimeout(function () {
                                clearTimeout(timeout);
                                Fanex.ExtAcc.UnBlockUI();

                                Fanex.ExtAcc.Form.Close(false);
                            }, 2000);
                        }
                        else {
                            Fanex.ExtAcc.UnBlockUI();

                            $('#errMsg').css('display', 'block');
                            $('#errMsg').html(result.errorMessage.formatErrorMessage());
                        }
                    });
                }
            }

            ko.applyBindings(new BetSettingModel());

            Fanex.ExtAcc.UnBlockUI();
        });
    },
    GetMemberBetSetting: function () {
        Fanex.ExtAcc.BlockUI();

        var data = new Object();
        data.custId = Fanex.ExtAcc.Customer.UserId;
        data.parentId = Fanex.ExtAcc.Customer.agentId;
        data.groupName = Fanex.ExtAcc.Customer.GroupName().groupName;

        $.ajax({
            url: resolveClientUrl('SportsbookSetting/EditBetSettingMemberGet'),
            data: data,
            dataType: 'json',
            type: 'post'
        })
        .success(function (result) {
            if (!result.betting) {
                Fanex.ExtAcc.UnBlockUI();
                Fanex.ExtAcc.Form.Close();
                var timeout = setTimeout(function () {
                    clearTimeout(timeout);
                    Fanex.ExtAcc.Form.Alert('Error', "There is no information available".formatErrorMessage(), 450, 150);
                }, 1000);

                return;
            }

            var bettingLimit = null;
            var context = result.context;
            bettingLimit = new Fanex.PTEngine.BettingLimit(result.betting);

            var bettingLimitContainer = document.getElementById('bettingLimitContainer');
            bettingLimit.Render(bettingLimitContainer);

            var BetSettingModel = function () {
                this.saveCustomer = function () {
                    $('#windowPopup').scrollTop(0);
                    if (!bettingLimit.Validate()) return;

                    Fanex.ExtAcc.BlockUI();

                    if (bettingLimit) {
                        betting = bettingLimit.Save();
                        var text = JSON.stringify(betting);

                        var para = {
                            custId: Fanex.ExtAcc.Customer.UserId,
                            parentId: Fanex.ExtAcc.Customer.agentId,
                            txtCustomerObject: text,
                            groupName: Fanex.ExtAcc.Customer.GroupName().groupName
                        };
                    }

                    $.ajax({
                        url: resolveClientUrl('SportsbookSetting/EditBetSettingPost'),
                        data: para,
                        dataType: 'json'
                    })
                    .success(function (result) {
                        if (result.errorCode == 0) {
                            var timeout = setTimeout(function () {
                                clearTimeout(timeout);
                                Fanex.ExtAcc.UnBlockUI();

                                Fanex.ExtAcc.Form.Close(false);
                            }, 2000);
                        }
                        else {
                            Fanex.ExtAcc.UnBlockUI();

                            $('#errMsg').css('display', 'block');
                            $('#errMsg').html(result.errorMessage.formatErrorMessage());
                        }
                    });
                }
            }

            ko.applyBindings(new BetSettingModel());

            Fanex.ExtAcc.UnBlockUI();
        });
    }
}

$(document).ready(function () {
    Fanex.ExtAcc.Customer.BetSetting.Init();
});