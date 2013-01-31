if (typeof Fanex.ExtAcc.Customer == 'undefined') {
    Fanex.ExtAcc.Customer = {};
}

Fanex.ExtAcc.Customer.TransferCondition = {
    arrWeekly: new Array("#Sun", "#Mon", "#Tue", "#Wed", "#Thu", "#Fri", "#Sat"),

    Init: function () {
        for (var i = 0; i < this.arrWeekly.length; i++) {
            $(this.arrWeekly[i]).bind('click', this.CheckChooseDaily);
        }

        $('#Daily').bind('click', function () {
            for (var i = 0; i < Fanex.ExtAcc.Customer.TransferCondition.arrWeekly.length; i++) {
                $(Fanex.ExtAcc.Customer.TransferCondition.arrWeekly[i]).attr("disabled", true);
            }

            $('input:radio[name=Weekly]').attr("checked", false);
        });

        $('#Weekly').bind('click', function () {
            for (var i = 0; i < Fanex.ExtAcc.Customer.TransferCondition.arrWeekly.length; i++) {
                $(Fanex.ExtAcc.Customer.TransferCondition.arrWeekly[i]).attr("disabled", false);
            }

            $('input:radio[name=Daily]').attr("checked", false);
        });
    },

    CheckChooseDaily: function () {
        var chkCount = 0;
        if ($('input:radio[name=Weekly]').is(':checked')) {
            for (var i = 0; i < Fanex.ExtAcc.Customer.TransferCondition.arrWeekly.length; i++) {
                if ($(Fanex.ExtAcc.Customer.TransferCondition.arrWeekly[i]).is(':checked')) {
                    chkCount++;
                    if (chkCount == 7) {
                        Validators.show($('#Daily').get(0), titles[3]);

                        return false;
                    }
                }
            }
        }

        return true;
    },

    ValidationTransferCondition: function() {
        var isSelect = false;
        if ($('input:radio[name=Weekly]').is(':checked')) {
            for (var i = 0; i < Fanex.ExtAcc.Customer.TransferCondition.arrWeekly.length; i++) {
                if ($(Fanex.ExtAcc.Customer.TransferCondition.arrWeekly[i]).is(':checked')) {
                    isSelect = true;
                    break;
                }
            }

            if (!isSelect) {
                Validators.show($('#Daily').get(0), titles[4]);

                return false;
            }
        }

        return true;
    },

    TransferOption: function (transferOption) {
        var str = transferOption;
        var transferConditionId = document.getElementsByName("transferCondition");

        if (transferConditionId != 'undefined' && transferConditionId.length > 0) {
            if (transferConditionId[1].checked) {
                str = "";
                var arrWeekly = new Array("chkSun", "chkMon", "chkTue", "chkWed", "chkThurs", "chkFri", "chkSat");
                for (var i = 0; i < arrWeekly.length; i++) {
                    if (document.getElementById(arrWeekly[i]).checked) {
                        str += "1";
                    }
                    else {
                        str += "0";
                    }
                }

                str += "1";
            }
        }

        return str;
    }
}

$(document).ready(function () {
    Fanex.ExtAcc.Customer.TransferCondition.Init();
});