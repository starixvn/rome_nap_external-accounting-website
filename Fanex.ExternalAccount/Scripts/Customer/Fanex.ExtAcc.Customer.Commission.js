/// <reference path="../_references.js" />
if (typeof Fanex == 'undefined' || typeof Fanex.ExtAcc == 'undefined') {
    alert("Fanex.ExtAcc.js module is required!")
}

Fanex.ExtAcc.Customer.Commission = {
    commission: null,
    currentLevel: 4,
    $this: null,
    Init: function () {
        $this = this;

        if (Fanex.ExtAcc.Customer.CurrentLevel == Fanex.ExtAcc.Customer.Master) {
            $this.GetCommission(Fanex.ExtAcc.Customer.superId);
            $this.currentLevel = 3;
        }
        else if (Fanex.ExtAcc.Customer.CurrentLevel == Fanex.ExtAcc.Customer.Agent) {
            $this.GetCommission(Fanex.ExtAcc.Customer.masterId);
            $this.currentLevel = 2;
        }
        else {
            $this.GetCommission(Fanex.ExtAcc.Customer.agentId);
            $this.currentLevel = 1;
        }

        $('#btnUpdate').click(function (e) {
            Fanex.ExtAcc.Customer.Commission.UpdateCommission();
        });
    },

    GetCommission: function (parentId) {
        Fanex.ExtAcc.BlockUI();
        var data = {
            custId: Fanex.ExtAcc.Customer.UserId,
            parentId: parentId,
            roleId: Fanex.ExtAcc.Customer.CurrentLevel,
            groupName: Fanex.ExtAcc.Customer.GroupName().groupName
        };

        $.ajax({
            url: resolveClientUrl('SportsbookSetting/EditCommissionGet'),
            data: data,
            dataType: 'json',
            type: 'post'
        })
            .success(function (result) {
                if (result.errorCode && result.errorCode == "1") {
                    Fanex.ExtAcc.UnBlockUI();
                    Fanex.ExtAcc.Form.Close();
                    var timeout = setTimeout(function () {
                        clearTimeout(timeout);
                        Fanex.ExtAcc.Form.Alert('Error', result.errorMessage.formatErrorMessage(), 450, 150);
                    }, 1000);

                    return;
                }

                if (!result.context.targetCustId) {
                    Fanex.ExtAcc.UnBlockUI();
                    Fanex.ExtAcc.Form.Close();
                    var timeout = setTimeout(function () {
                        clearTimeout(timeout);
                        Fanex.ExtAcc.Form.Alert('Error', "There is no information available".formatErrorMessage(), 450, 150);
                    }, 1000);

                    return;
                }

                commission = result.commission;
                var context = result.context;
                if (context.roleId == 1) {
                    $("#textBoxDiscountAsian").val(commission.discount);
                    $("#textBoxDiscount1x2").val(commission.discount1x2);
                    $("#textBoxDiscountCS").val(commission.discountCS);
                    if (commission.isMyanmarOdds) {
                        $("#textBoxDiscountMyanmarOdds").val(commission.discountMyanmarOdds);
                    }
                    else {
                        $('#discountMyanmar').hide();
                    }
                    Fanex.PTEngine.PT_SetSelect($("#discountAsian")[0], 0, commission.discount, Fanex.PTEngine.COMMISSION_STEP, commission.playerDiscount);
                    Fanex.PTEngine.PT_SetSelect($("#discount1x2")[0], 0, commission.discount1x2, Fanex.PTEngine.COMMISSION_STEP, commission.playerDiscount1x2);
                    Fanex.PTEngine.PT_SetSelect($("#discountCS")[0], 0, commission.discountCS, Fanex.PTEngine.COMMISSION_STEP, commission.playerDiscountCs);
                    if (commission.isMyanmarOdds) {
                        Fanex.PTEngine.PT_SetSelect($("#discountMyanmarOdds")[0], 0, commission.discountMyanmarOdds, Fanex.PTEngine.COMMISSION_STEP, commission.playerDiscountMyanmarOdds);
                    }
                    else {
                        $('#discountMyanmar').hide();
                    }
                }
                else {
                    Fanex.PTEngine.PT_SetSelect($("#groupA")[0], 0, commission.groupAMax, Fanex.PTEngine.COMMISSION_STEP, commission.groupA);
                    Fanex.PTEngine.PT_SetSelect($("#groupB")[0], 0, commission.groupBMax, Fanex.PTEngine.COMMISSION_STEP, commission.groupB);
                    Fanex.PTEngine.PT_SetSelect($("#groupC")[0], 0, commission.groupCMax, Fanex.PTEngine.COMMISSION_STEP, commission.groupC);
                    Fanex.PTEngine.PT_SetSelect($("#groupD")[0], 0, commission.groupDMax, Fanex.PTEngine.COMMISSION_STEP, commission.groupD);
                    Fanex.PTEngine.PT_SetSelect($("#discount1x2")[0], 0, commission.discount1x2Max, Fanex.PTEngine.COMMISSION_STEP, commission.discount1x2);
                    Fanex.PTEngine.PT_SetSelect($("#discountCS")[0], 0, commission.discountCSMax, Fanex.PTEngine.COMMISSION_STEP, commission.discountCS);
                    if (commission.isMyanmarOdds) {
                        Fanex.PTEngine.PT_SetSelect($("#discountMyanmarOdds")[0], 0, commission.discountMyanmarOddsMax, Fanex.PTEngine.COMMISSION_STEP, commission.discountMyanmarOdds);

                        var groups = $('.group');
                        for (var i = 0; i < groups.length; i++) {
                            groups[i].remove();
                        }                        
                    } else {
                        $('#discountMyanmar').hide();
                    }
                }

                Fanex.ExtAcc.UnBlockUI();
            });
    },

    UpdateCommission: function () {
        // if (!commission.Validate()) return;
        Fanex.ExtAcc.BlockUI();

        if (commission) {
            if ($this.currentLevel == 1) {
                commission.playerDiscount = $("#discountAsian").val();
                commission.playerDiscount1x2 = $("#discount1x2").val();
                commission.playerDiscountCs = $("#discountCS").val();
                if (commission.isMyanmarOdds)
                    commission.playerDiscountMyanmarOdds = $("#discountMyanmarOdds").val();
            }
            else {
                commission.groupA = $("#groupA").val();
                commission.discount1x2 = $("#discount1x2").val();
                commission.discountCS = $("#discountCS").val();
                if (commission.isMyanmarOdds)
                    commission.discountMyanmarOdds = $("#discountMyanmarOdds").val();
                else {
                    commission.groupB = $("#groupB").val();
                    commission.groupC = $("#groupC").val();
                    commission.groupD = $("#groupD").val();
                }
            }

            var text = JSON.stringify(commission);
            var para = {
                custId: Fanex.ExtAcc.Customer.UserId,
                parentId: Fanex.ExtAcc.Customer.superId,
                currentLevel: $this.currentLevel,
                txtCustomerObject: text,
                groupName: Fanex.ExtAcc.Customer.GroupName().groupName
            };
        }

        $.ajax({
            url: resolveClientUrl('SportsbookSetting/EditCommissionPost'),
            data: para,
            type: 'post'
        })
            .success(function (result) {

                if (Fanex.ExtAcc.ShowSecurityCodePage(result, Fanex.ExtAcc.UnBlockUI)) {
                    return;
                }

                if (result.errorCode == 0) {
                    var timeout = setTimeout(function () {
                        clearTimeout(timeout);
                        Fanex.ExtAcc.Customer.CallBackCustomerList();
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

$(document).ready(function () {
    Fanex.ExtAcc.Customer.Commission.Init();
});