if (typeof Fanex.ExtAcc.Customer == 'undefined') {
    Fanex.ExtAcc.Customer = {};
}

Fanex.ExtAcc.Customer.TransferCondition = {
    arrWeekly: new Array("#Sun", "#Mon", "#Tue", "#Wed", "#Thu", "#Fri", "#Sat"),

    Init: function () {
        if ($('input:radio[name=Daily]').is(':checked')) {
            for (var i = 0; i < Fanex.ExtAcc.Customer.TransferCondition.arrWeekly.length; i++) {
                $(Fanex.ExtAcc.Customer.TransferCondition.arrWeekly[i]).attr("disabled", true);
            }
        }

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

        $self = this;
    },

    CheckChooseDaily: function () {
        var chkCount = 0;
        if ($('input:radio[name=Weekly]').is(':checked')) {
            for (var i = 0; i < Fanex.ExtAcc.Customer.TransferCondition.arrWeekly.length; i++) {
                if ($(Fanex.ExtAcc.Customer.TransferCondition.arrWeekly[i]).is(':checked')) {
                    chkCount++;
                    if (chkCount == 7) {
                        Validators.show($('#Daily').get(0), titles[3], null, null, 5000);

                        return false;
                    }
                }
            }
        }

        return true;
    },

    ValidationTransferCondition: function () {
        var isSelect = false;
        if ($('input:radio[name=Weekly]').is(':checked')) {
            for (var i = 0; i < Fanex.ExtAcc.Customer.TransferCondition.arrWeekly.length; i++) {
                if ($(Fanex.ExtAcc.Customer.TransferCondition.arrWeekly[i]).is(':checked')) {
                    isSelect = true;
                    break;
                }
            }

            if (!isSelect) {
                Validators.show($('#Mon').get(0), titles[4], null, null, 5000);

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

        $self.ShowConfirm(titles[1], titles[2], 400, 150, function () {
            if ($('input:radio[name=Weekly]').is(':checked')) {

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
                custId: Fanex.ExtAcc.Customer.UserId
            };

            $this.BlockUI();

            $.ajax({
                url: urlTransferCondition,
                data: data,
                dataType: 'json'
            })
            .success(function (result) {
                if (result.IsSuccess) {
                    Fanex.ExtAcc.Form.Close(false);
                    $.unblockUI(document);
                }
                else {
                    $('#errMsg').css('display', 'block');
                    $('#errMsg').html(result.ErrorMessage);
                }
            })
            .error(function (xhr, status, err) {
                $.unblockUI(document);
                $('#errMsg').css('display', 'block');
                $('#errMsg').html(err);
            });
        });
    },

    ShowConfirm: function (title, message, width, height, okClickEvent) {
        var Form = document.createElement("div");
        var DivContent = document.createElement("div");

        Form.setAttribute('id', 'windowConfirm');
        Form.setAttribute('style', 'display: none; border: none;font-size:12px');

        DivContent.setAttribute('style', 'height: ' + (height - 125) + 'px');

        var divFrame = document.createElement("div");
        divFrame.setAttribute('style', 'height: ' + (height - 125) + 'px; margin: 2px 5px;line-height: 18px');

        var frame = document.createElement("span");
        frame.innerHTML = message;
        frame.setAttribute('style', 'width: 100%; height: 100%; border: none');

        divFrame.appendChild(frame);
        DivContent.appendChild(divFrame);

        Form.appendChild(DivContent);

        document.body.appendChild(Form);

        $('#windowConfirm').css('display', 'block');
        $('#windowConfirm').dialog({ height: height, width: width, minWidth: 300, minHeight: 150,
            title: title,
            resizable: false,
            modal: true,
            buttons: [
            {
                text: "Yes",
                click: function () {
                    if (okClickEvent && typeof okClickEvent === 'function') {
                        okClickEvent();
                    }

                    $('#windowConfirm').remove();
                }
            },
            {
                text: "No",
                click: function () {
                    $('#windowConfirm').remove();
                }
            }],
            close: function (event) {
                $('#windowConfirm').remove();
            }
        });
    }
}

$(document).ready(function () {
    Fanex.ExtAcc.Customer.TransferCondition.Init();
});