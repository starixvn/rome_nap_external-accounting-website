$(document).ready(function () {
    $.ajaxSetup({
        beforeSend: function (xhr) {
            var antiXSRFToken = document.getElementsByName("__RequestVerificationToken");
            if (antiXSRFToken.length > 0) {
                xhr.setRequestHeader("__RequestVerificationToken", antiXSRFToken[0].value);
            }
        }
    });

    var cachedDOM = {
        btnUpdate: $('#btnUpdate'),
        Messages: $('#result-messages'),
        txtCustId: $('#txtCustId'),
        txtCustIds: $('#txtCustIds'),
        txtCustName: $('#txtCustName'),
        txtCustNames: $('#txtCustNames'),
        chkUnlimitedElements: $('input[id^=chkUnlimited_]'),
        txtInputFields: $('.input-field'),
        txtRoleId: $('#txtRoleId'),
        chkEnableEdits: $('.check-enable-edit')
    };

    cachedDOM.chkEnableEdits.unbind();
    cachedDOM.chkEnableEdits.click(function () {
        var $this = $(this);
        var $tr = $this.closest('tr');
        var $tds = $tr.find('td:not(.row-header)');
        var $childInputsInRow = $tr.find('input:not(.check-enable-edit, .input-field-unlimited)');
        if ($this.is(':checked')) {
            $tds.removeClass('td-disabled');
            $childInputsInRow.attr('disabled', false);
        }
        else {
            $tds.addClass('td-disabled');
            $childInputsInRow.attr('disabled', true);
        }

    });

    cachedDOM.chkUnlimitedElements.unbind();
    cachedDOM.chkUnlimitedElements.change(function () {
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
        var isMultiple = IsMultiple();
        parent.Fanex.ExtAcc.BlockUI();
        ClearMessages();

        if (!ValidateInput(isMultiple)) {
            parent.jQuery.unblockUI();
            return;
        }

        var params = {
            CustId: cachedDOM.txtCustId.val(),
            CustIds: cachedDOM.txtCustIds.val(),
            ParlayMaxPayoutList: []
        };

        var soccerParlayRow = {
            SportType: 1,
            Bettype: 9,
            SingleMatchParlay: GetValue('1_9_SingleMatchParlay'),
            Combo2: GetValue('1_9_Combo2'),
            Combo35: GetValue('1_9_Combo35'),
            ComboOver6: GetValue('1_9_ComboOver6')
        };

        if (!isMultiple || IsEnabledEdit(0))
            params.ParlayMaxPayoutList.push(soccerParlayRow);

        var MixSportsParlayRow = {
            SportType: 99,
            Bettype: 9,
            SingleMatchParlay: GetValue('99_9_SingleMatchParlay'),
            Combo2: GetValue('99_9_Combo2'),
            Combo35: GetValue('99_9_Combo35'),
            ComboOver6: GetValue('99_9_ComboOver6')
        };

        if (!isMultiple || IsEnabledEdit(1))
            params.ParlayMaxPayoutList.push(MixSportsParlayRow);

        //if (params.ParlayMaxPayoutList.length === 0) {
        //    ShowSingleErrorMessage('There is no change to update.');
        //    parent.jQuery.unblockUI();
        //    return;
        //}

        if (isMultiple) {
            PostUpdate(params, isMultiple);
        } else {
            $.ajax({
                type: "POST",
                url: parent.resolveClientUrl("ParlayMaxPayout/Check"),
                contentType: "application/json; charset=utf-8",
                data: JSON.stringify(params),
                success: function (result) {
                    if (parseInt(result.ErrorCode) === 0 || confirm(result.ErrorMessage)) {
                        PostUpdate(params, isMultiple);
                    }
                },
                complete: function () {
                    parent.jQuery.unblockUI();
                }
            });
        }
    });

    function PostUpdate(params, isMultiple) {
        $.ajax({
            type: "POST",
            url: parent.resolveClientUrl("ParlayMaxPayout/Update" + (isMultiple ? "Multiple" : "")),
            contentType: "application/json; charset=utf-8",
            data: JSON.stringify(params),
            success: function (result) {
                if (isMultiple) {
                    var custNamesArray = cachedDOM.txtCustNames.val().split(',');
                    var hasSuccessUpdate = false;
                    var hasError = false;
                    for (var i = 0; i < result.length; i++) {
                        if (parseInt(result[i].ErrorCode) === 0) {
                            hasSuccessUpdate = true;
                        }
                        else {
                            hasError = true;
                            ShowMultipleErrorMessage(custNamesArray[i], result[i].ErrorMessage);
                        }
                    }

                    if (hasSuccessUpdate) {
                        ShowSuccessMessage();
                    }

                    if (!hasError) {
                        setTimeout(function () {
                            window.location.reload();
                        }, 1000);
                    }
                }
                else {
                    if (parseInt(result.ErrorCode) === 0) {
                        ShowSuccessMessage();
                        setTimeout(function () {
                            window.location.reload();
                        }, 1000);
                    } else {
                        ShowSingleErrorMessage(result.ErrorMessage);
                    }
                }
            },
            error: function () {
            },
            complete: function () {
                parent.jQuery.unblockUI();
            }
        });
    }

    function ValidateInput(isMultiple) {
        var isValid = true;
        var errMsg = '';
        var isChanged = false;
        cachedDOM.txtInputFields.each(function () {
            var $this = $(this);
            var $chkUnlimited = $('#' + $this.attr('data-checkbox-unlimited-id'));
            var defaultPayoutValue = parseInt($this.attr('data-default-payout-value').replaceAll(',', ''));
            var rowHeader = $this.attr('data-row-header');
            var columnName = $this.attr('data-column-name');
            var inputValue = GetValue($this.attr('id'));
            var isCheckedUnlimited = $chkUnlimited.prop('checked');
            var origValue = parseInt($this.attr('data-original-value').replaceAll(',', ''));
            if (inputValue !== origValue) {
                isChanged = true;
            }

            if (isNaN(inputValue) || (!isCheckedUnlimited && inputValue <= 0)) {
                isValid = false;
                errMsg = 'The value must be a positive amount.';
                return false;
            }

            if (!isCheckedUnlimited && defaultPayoutValue > 0 && inputValue > defaultPayoutValue) {
                isValid = false;
                errMsg = rowHeader + ": The setting of " + columnName + " is higher than the " + (cachedDOM.txtRoleId.val() === "4" ? "company setting." : "upline's.");
                return false;
            }
        });

        if (!isMultiple && !isChanged) {
            isValid = false;
            errMsg = 'There is no change to update.';
        }

        if (isMultiple && !cachedDOM.chkEnableEdits.eq(0).is(':checked') && !cachedDOM.chkEnableEdits.eq(1).is(':checked')) {
            isValid = false;
            errMsg = 'There is no change to update.';
        }

        if (!isValid) {
            ShowSingleErrorMessage(errMsg);
        }

        return isValid;
    }

    function IsValueChanged(id) {
        var val = GetValue(id);
        var origVal = parseInt($('#' + id).attr('data-original-value').replaceAll(',', ''));
        return val !== origVal;
    }

    function IsEnabledEdit(rowIndex) {
        return cachedDOM.chkEnableEdits.eq(rowIndex).is(':checked');
    }

    function HasValueChanged(prefixId) {
        return IsValueChanged(prefixId + 'SingleMatchParlay')
            || IsValueChanged(prefixId + 'Combo2')
            || IsValueChanged(prefixId + 'Combo35')
            || IsValueChanged(prefixId + 'ComboOver6');
    }

    function GetValue(id) {
        return $('#chkUnlimited_' + id).prop('checked') ? 0 : parseInt($('#' + id).val().replaceAll(',', ''));
    }

    function ShowSingleErrorMessage(errMsg) {
        cachedDOM.Messages.append('<div class="errMsg">' + errMsg + '</div>');
    }

    function ShowMultipleErrorMessage(userName, errMsg) {
        cachedDOM.Messages.append('<div class="errMsg">' + userName + ': ' + errMsg + '</div>');
    };

    function ShowSuccessMessage() {
        cachedDOM.Messages.html('<div class="successMsg">Updated Successfully.</div>' + cachedDOM.Messages.html());
    }

    function ClearMessages() {
        cachedDOM.Messages.html('');
    }

    function IsMultiple() {
        return cachedDOM.txtCustIds.val() !== '';
    }
});