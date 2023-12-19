/// <reference path="../_references.js" />
if (typeof Fanex == 'undefined' || typeof Fanex.ExtAcc == 'undefined') {
    alert("Fanex.ExtAcc.js module is required!")
}

Fanex.ExtAcc.Customer.TransferCondition = {
    arrWeekly: new Array("#Transfer_Sun", "#Transfer_Mon", "#Transfer_Tue", "#Transfer_Wed", "#Transfer_Thu", "#Transfer_Fri", "#Transfer_Sat"),

    Init: function () {
        if ($('input:radio[name="Transfer.Daily"]').is(':checked')) {
            for (var i = 0; i < Fanex.ExtAcc.Customer.TransferCondition.arrWeekly.length; i++) {
                $(Fanex.ExtAcc.Customer.TransferCondition.arrWeekly[i]).attr("disabled", true);
            }
        }

        for (var i = 0; i < this.arrWeekly.length; i++) {
            $(this.arrWeekly[i]).bind('click', this.CheckChooseDaily);
        }

        $('#Transfer_Daily').bind('click', function () {
            for (var i = 0; i < Fanex.ExtAcc.Customer.TransferCondition.arrWeekly.length; i++) {
                $(Fanex.ExtAcc.Customer.TransferCondition.arrWeekly[i]).attr("disabled", true);
            }

            $('input:radio[name="Transfer.Weekly"]').attr("checked", false);
            $('.rowTransferCondition.note').addClass('hidden');
        });

        $('#Transfer_Weekly').bind('click', function () {
            for (var i = 0; i < Fanex.ExtAcc.Customer.TransferCondition.arrWeekly.length; i++) {
                $(Fanex.ExtAcc.Customer.TransferCondition.arrWeekly[i]).attr("disabled", false);
            }

            $('input:radio[name="Transfer.Daily"]').attr("checked", false);
            $('.rowTransferCondition.note').removeClass('hidden');
        });

        $self = this;
    },

    CheckChooseDaily: function () {
        var chkCount = 0;
        if ($('input:radio[name="Transfer.Weekly"]').is(':checked')) {
            for (var i = 0; i < Fanex.ExtAcc.Customer.TransferCondition.arrWeekly.length; i++) {
                if ($(Fanex.ExtAcc.Customer.TransferCondition.arrWeekly[i]).is(':checked')) {
                    chkCount++;
                    if (chkCount == 7) {
                        Validators.show($('#Transfer_Daily').get(0), titles[3], -10, 7, -40, 35, 15, 5000);

                        return false;
                    }
                }
            }
        }

        return true;
    },

    ValidationTransferCondition: function () {
        var isSelect = false;
        if ($('input:radio[name="Transfer.Weekly"]').is(':checked')) {
            for (var i = 0; i < Fanex.ExtAcc.Customer.TransferCondition.arrWeekly.length; i++) {
                if ($(Fanex.ExtAcc.Customer.TransferCondition.arrWeekly[i]).is(':checked')) {
                    isSelect = true;
                    break;
                }
            }

            if (!isSelect) {
                Validators.show($('#Transfer_Mon').get(0), titles[4], -10, 7, -40, 35, 15, 5000);

                return false;
            }
        }

        return true;
    },

    EditTransferCondition: function (transferOption) {
        var str = '';
        var urlTransferCondition = resolveClientUrl('Customer/EditTransferConditionPost');

        if (!Fanex.ExtAcc.Customer.TransferCondition.ValidationTransferCondition()) {
            return;
        }

        var autoTransferCondition = $('.rad-change-affect:checked').val();

        var message = titles[2] +
            ($('.rowTransferCondition.note').hasClass('hidden') ? "" : '<br />' + messages[autoTransferCondition == "0" ? 0 : 1]);

        $self.ShowConfirm(titles[1], message, 400, 180, function () {
            if ($('input:radio[name="Transfer.Weekly"]').is(':checked')) {
                for (var i = 0; i < Fanex.ExtAcc.Customer.TransferCondition.arrWeekly.length; i++) {
                    if ($(Fanex.ExtAcc.Customer.TransferCondition.arrWeekly[i]).is(':checked')) {
                        str += "1";
                    }
                    else {
                        str += "0";
                    }
                }

                str += "1";
            }
            else {
                str = '00000000';
            }

            var data = {
                transferOption: str,
                custId: custId,
                groupName: Fanex.ExtAcc.Customer.GroupName().groupName,
                autoTransferCondition: autoTransferCondition
            };

            Fanex.ExtAcc.BlockUI();

            $.ajax({
                url: urlTransferCondition,
                data: data
            })
                .success(function (result) {

                    if (Fanex.ExtAcc.ShowSecurityCodePage(result, Fanex.ExtAcc.UnBlockUI)) {
                        return;
                    }

                    if (result.isSuccess) {
                        Fanex.ExtAcc.Form.Close(false, null, 'windowConfirm');
                        Fanex.ExtAcc.Form.Close(false);
                        Fanex.ExtAcc.UnBlockUI();
                    }
                    else {
                        $('#errMsg').css('display', 'block');
                        $('#errMsg').html(result.errorMessage.formatErrorMessage());
                        Fanex.ExtAcc.UnBlockUI();
                        Fanex.ExtAcc.Form.Close(false, null, 'windowConfirm');
                    }
                })
                .error(function (xhr, status, err) {
                    Fanex.ExtAcc.UnBlockUI();
                    Fanex.ExtAcc.Form.Close(false, null, 'windowConfirm');
                    $('#errMsg').css('display', 'block');
                    $('#errMsg').html(err);
                });
        });
    },

    ShowConfirm: function (title, message, width, height, okClickEvent) {
        Fanex.ExtAcc.Form.ConfirmBox(title, message, width, height, false, okClickEvent, null, 'windowConfirm', true);
    }
}

$(document).ready(function () {
    Fanex.ExtAcc.Customer.TransferCondition.Init();
});