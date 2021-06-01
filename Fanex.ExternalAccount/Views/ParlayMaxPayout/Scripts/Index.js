$(document).ready(function () {
    var cachedDOM = {
        btnUpdate: $('#btnUpdate'),
        Messages: $('#result-messages'),
        txtCustId: $('#txtCustId'),
        txtCustIds: $('#txtCustIds'),
        txtCustName: $('#txtCustName'),
        txtCustNames: $('#txtCustNames'),
        chkUnlimitedElements: $('input[id^=chkUnlimited_]')
    };

    cachedDOM.chkUnlimitedElements.unbind();
    cachedDOM.chkUnlimitedElements.change(function () { 
        debugger;
        var $this = $(this);
        var alternativeFieldId = $this.attr('id').replace('chkUnlimited_', '');
        var $field = $('#' + alternativeFieldId);
        var $fieldUnlimited = $('#' + alternativeFieldId + '_Unlimited');
        if ($this.prop('checked')) {
            $field.hide();
            $fieldUnlimited.show();
        }
        else {
            $field.show();
            $fieldUnlimited.hide();
        }
    });


    cachedDOM.btnUpdate.click(function () {
        ClearMessages();
        var params = {
            CustId: cachedDOM.txtCustId.val(),
            CustIds: cachedDOM.txtCustIds.val(),
            ParlayMaxPayoutList: []
        };

        params.ParlayMaxPayoutList.push({
            SportType: 1,
            Bettype: 9,
            SingleMatchParlay: $('#chkUnlimited_1_9_SingleMatchParlay').prop('checked') ? 0: parseInt($('#1_9_SingleMatchParlay').val().replaceAll(',', '')),
            Combo2: $('#chkUnlimited_1_9_Combo2').prop('checked') ? 0 : parseInt($('#1_9_Combo2').val().replaceAll(',', '')),
            Combo35: $('#chkUnlimited_1_9_Combo35').prop('checked') ? 0 : parseInt($('#1_9_Combo35').val().replaceAll(',', '')),
            ComboOver6: $('#chkUnlimited_1_9_ComboOver6').prop('checked') ? 0 : parseInt($('#1_9_ComboOver6').val().replaceAll(',', ''))
        });

        params.ParlayMaxPayoutList.push({
            SportType: 99,
            Bettype: 9,
            SingleMatchParlay: $('#chkUnlimited_99_9_SingleMatchParlay').prop('checked') ? 0 : parseInt($('#99_9_SingleMatchParlay').val().replaceAll(',', '')),
            Combo2: $('#chkUnlimited_99_9_Combo2').prop('checked') ? 0 : parseInt($('#99_9_Combo2').val().replaceAll(',', '')),
            Combo35: $('#chkUnlimited_99_9_Combo35').prop('checked') ? 0 : parseInt($('#99_9_Combo35').val().replaceAll(',', '')),
            ComboOver6: $('#chkUnlimited_99_9_ComboOver6').prop('checked') ? 0 : parseInt($('#99_9_ComboOver6').val().replaceAll(',', ''))
        });

        var isMultiple = cachedDOM.txtCustIds.val() !== '';
        $.ajax({
            type: "POST",
            url: resolveClientUrl("ParlayMaxPayout/Update" + (isMultiple ? "Multiple" : "")),
            contentType: "application/json; charset=utf-8",
            data: JSON.stringify(params),
            success: function (result) {
                if (isMultiple) {
                    var custNamesArray = cachedDOM.txtCustNames.val().split(',');
                    var hasSuccessUpdate = false;
                    for (var i = 0; i < result.length; i++) {
                        if (parseInt(result[i].ErrorCode) === 0) {
                            hasSuccessUpdate = true;
                        }
                        else {
                            ShowErrorMessage(custNamesArray[i], result[i].ErrorMessage);
                        }
                    }

                    if (hasSuccessUpdate) {
                        ShowSuccessMessage();
                    }
                }
                else {
                    if (parseInt(result.ErrorCode) === 0) {
                        ShowSuccessMessage();
                        parent.Fanex.ExtAcc.Form.Close(false, 3000);
                    } else {
                        ShowErrorMessage(result.ErrorMessage);
                    }
                }
            },
            error: function () {
            },
            complete: function () {
                parent.jQuery.unblockUI();
                Fanex.ExtAcc.Form.Close(false, 3000);
            }
        });
    });

    function ShowErrorMessage(userName, errMsg) {
        cachedDOM.Messages.append('<div class="errMsg">' + userName + ': ' + errMsg + '</div>');
    };

    function ShowSuccessMessage() {
        cachedDOM.Messages.html('<div class="successMsg">Updated Successfully.</div>' + cachedDOM.Messages.html());
    }

    function ClearMessages() {
        cachedDOM.Messages.html('');
    }
});