var ptRacingCommon = {
    adminRoleId: 5,
    superRoleId: 4,
    masterRoleId: 3,
    agentRoleId: 2,
    memberRoleId: 1,
    renderCustomerInfo: function (level, custInfo) {
        var html = '<table class="tblcollapse" style="width: 844px; margin-bottom: 10px;" id="tbCustomerInfo">\
                        <tr class="title">\
                            <td colspan="4" style="text-align: center;font-weight:bold">\
                               ' + level + ': ' + custInfo.userName + '\
                            </td>\
                        </tr>\
                        <tr>\
                            <td class="r" style="width:15%">First Name</td>\
                            <td class="l" style="width:35%">' + custInfo.firstName + '</td>\
                            <td class="r" style="width:15%">Last Name</td>\
                            <td class="l" style="width:35%">' + custInfo.lastName + '</td>\
                        </tr>\
                        <tr>\
                            <td class="r" style="width:15%">Phone</td>\
                            <td class="l" style="width:35%">' + custInfo.phone + '</td>\
                            <td class="r" style="width:15%">Mobile Phone</td>\
                            <td class="l" style="width:35%">' + custInfo.mobilePhone + '</td>\
                        </tr>\
                        <tr>\
                            <td class="r" style="width:15%">Fax</td>\
                            <td class="l" style="width:35%">' + custInfo.fax + '</td>\
                            <td class="r" style="width:15%">&nbsp;</td>\
                            <td class="l" style="width:35%">&nbsp;</td>\
                        </tr>\
                    </table>';
        $('#divCustInfo').html(html);
    },
    renderErrorMessage: function (containerid, ErrorHandler) {
        var isError500 = true;
        var isSuccess = true;
        var result = '';
        if (ErrorHandler != null && ErrorHandler.length > 0) {
            $.each(ErrorHandler, function (index, Result) {
                if (Result.ErrorCode == 100 || Result.ErrorCode == 99 || Result.ErrorCode == 98) {
                    if (isError500) {
                        result += ptRacingCommon.formatHtmlErrorMessage(Result.ErrorMessage, 2);
                    }
                    isSuccess = false;
                }
                else if (Result.ErrorCode > 0) {
                    if (isError500) {
                        result += ptRacingCommon.formatHtmlErrorMessage(Result.ErrorMessage, 1);
                    }
                    isSuccess = false;
                }
                else {
                    result += ptRacingCommon.formatHtmlErrorMessage(Result.ErrorMessage, 0);
                }
                if (Result.ErrorCode == 500) isError500 = false;
            });
        }

        if (isSuccess) {
            t1 = setTimeout(function () {
                parent.Fanex.ExtAcc.Form.Close(false);
            }, 3000);
        }
        else {
            t2 = setTimeout("ptRacingCommon.clearSuccessMessages()", 3000);
        }

        var containerElement = $('#'.concat(containerid));
        containerElement.removeClass('display-none');
        containerElement.html(result);

        window.scroll(0, 0);
        parent.scroll(0, 0);
        return isSuccess;
    },
    formatHtmlErrorMessage: function (msg, type) {
        switch (type) {
            case 0:
                return "<div class='msgcorrect'>" + msg + "</div>";
                break;
            case 1:
                return "<div class='msgerror'>" + msg + "</div>";
                break;
            case 2:
                return "<div class='msgwarning'>" + msg + "</div>";
                break;
            default:
                return "";
                break;
        }
    },
    clearSuccessMessages: function () {
        $("#err .msgcorrect").remove();
        clearTimeout(t2);
    },
    clearMessage: function (containerid) {
        var containerElement = $('#'.concat(containerid));
        containerElement.addClass('display-none');
        containerElement.html('');
    },
    afterUpdate: function () {
        var t = setTimeout(function () {
            customerPTRacingSetting.init();
        }, 3000);
    }

};

function ConfirmYes() {
    customerPTRacingSetting.setNewValueToMember(true);
}

function ConfirmNo() {
    customerPTRacingSetting.setNewValueToMember(false);
}