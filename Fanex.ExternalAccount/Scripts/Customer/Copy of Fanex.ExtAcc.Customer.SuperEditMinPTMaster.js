if (typeof Fanex.ExtAcc.Customer.MinPT == 'undefined') {
    Fanex.ExtAcc.Customer.MinPT = {};
}

Fanex.ExtAcc.Customer.MinPT = {
    commission: null,
    targetRoleId: 1,
    $this: null,
    Init: function () {
        $this = this;

        if (Fanex.ExtAcc.Customer.targetRoleId == Fanex.ExtAcc.Customer.Master) {
            this.GetMinPT(Fanex.ExtAcc.Customer.superId);
            this.targetRoleId = 3;
        }
        else if (Fanex.ExtAcc.Customer.targetRoleId == Fanex.ExtAcc.Customer.Agent) {
            this.GetMinPT(Fanex.ExtAcc.Customer.masterId);
            this.targetRoleId = 2;
        }
        else {
            this.GetMinPT(Fanex.ExtAcc.Customer.agentId);
            this.targetRoleId = 1;
        }

        $('#btnUpdate').click(function (e) {
            Fanex.ExtAcc.Customer.MinPT.UpdateMinPT();
        });
    },

    GetMinPT: function (parentId) {
        Fanex.ExtAcc.BlockUI();
        var data = {
            custId: Fanex.ExtAcc.Customer.UserId,
            parentId: parentId
        };

        $.ajax({
            url: resolveClientUrl('MinPT/SuperEditMinPTMasterGet'),
            data: data,
            dataType: 'json',
            type: 'post'
        })
        .success(function (result) {
            commission = result.commission;
            var context = result.context;
            if ($this.targetRoleId == 1) {
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
            }

            $.unblockUI(document);
        })
        .error(function (xhr, status, err) {
            $.unblockUI(document);
        });
    },

    UpdateMinPT: function () {
        // if (!commission.Validate()) return;
        if (commission) {
            if ($this.targetRoleId == 1) {
                commission.playerDiscount = $("#discountAsian").val();
                commission.playerDiscount1x2 = $("#discount1x2").val();
                commission.playerDiscountCs = $("#discountCS").val();
                commission.playerDiscountNumber = $("#discountNumber").val();
            }
            else {
                commission.groupA = $("#groupA").val();
                commission.groupB = $("#groupB").val();
                commission.groupC = $("#groupC").val();
                commission.groupD = $("#groupD").val();
                commission.discount1x2 = $("#discount1x2").val();
                commission.discountCS = $("#discountCS").val();
                commission.discountNumber = $("#discountNumber").val();
            }

            var text = JSON.stringify(commission);
            var para = {
                custId: Fanex.ExtAcc.Customer.UserId,
                parentId: Fanex.ExtAcc.Customer.superId,
                targetRoleId: $this.targetRoleId,
                txtCustomerObject: text
            };
        }

        Fanex.ExtAcc.BlockUI();

        $.ajax({
            url: resolveClientUrl('MinPT/SuperEditMinPTMasterPost'),
            data: para,
            dataType: 'json',
            type: 'post'
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

$(document).ready(function () {
    Fanex.ExtAcc.Customer.MinPT.Init();
});