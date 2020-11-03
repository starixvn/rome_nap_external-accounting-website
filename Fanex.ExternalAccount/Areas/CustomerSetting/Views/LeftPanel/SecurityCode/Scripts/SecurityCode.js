function Update() {
    $('#errMsg').hide();

    var errorMessage = "";
    var errorField;

    if (!Validators.validateInput($('#securityCode').val())) {
        errorMessage = "Sorry, security code can't be empty";
        errorField = $('#securityCode');
    } else if (!IsSecurityCode($('#securityCode').val())) {
        errorMessage = "Sorry, security code is incorrect format";
        errorField = $('#securityCode');
    } else if (!Validators.validateInput($('#securityCodeConfirm').val())) {
        errorMessage = "Sorry, confirm security code can't be empty";
        errorField = $('#securityCodeConfirm');
    } else if ($('#securityCode').val() != $('#securityCodeConfirm').val()) {
        errorMessage = "Sorry, confirm security code doesn't match with security code";
        errorField = $('#securityCodeConfirm');
    }

    if (errorMessage !== "") {
        $("#err-message").html(errorMessage).removeClass("hide");
        errorField.addClass("invalid-field").focus();
        return;
    }

    $("#err-message").addClass("hide");
    Fanex.CustomerSetting.Core.BlockUI();

    $.ajax({
        url: parent.resolveClientUrl('CustomerSetting/LeftPanel/UpdateCustomer/UpdateSecurityCode'),
        type: 'post',
        data: {
            custId: custId,
            code: $('#securityCode').val(),
            force: document.getElementById('forceChange').checked,
            roleId: roleId,
            groupName: parent.$('#selectedGroupName').html()
        }
    }).success(function (result) {

        if (parent.Fanex.ExtAcc.ShowSecurityCodePage(result, Fanex.CustomerSetting.Core.UnBlockUI)) {
            return;
        }

        if (result.isSuccess) {
            $('#errMsg').attr('class', 'successMsg');
            $('#errMsg').html('<font color="Blue">Updated successfully</font>');
            $('#errMsg').show();
        }
        else {
            $('#errMsg').attr('class', 'errMsg');
            $('#errMsg').html(result.errorMessage.formatErrorMessage());
            $('#errMsg').show();
        }

        Fanex.CustomerSetting.Core.UnBlockUI();
    }).error(function (data) {
        Fanex.CustomerSetting.Core.UnBlockUI();

        if (typeof console != 'undefined')
            console.log(data);
    });
};

function IsSecurityCode(secCode) {
    var secCodeLength = secCode.length;
    if (secCodeLength != 6) { // check length
        return false;
    }
    var regex = /^([0-9])(?!\1+$)[0-9]+$/; // check digits and all of them are not the same
    var result = secCode.search(regex) != -1 ? true : false;
    if (result) { // check 'Forward Run' and 'Backward Run'
        var isForwardRun = true;
        var isBackwardRun = true;
        var forward = 0;
        var backward = 0;
        var forward = parseInt(secCode.charAt(0));
        var backward = parseInt(secCode.charAt(0));
        for (var i = 1; i < secCodeLength; i++) {
            var current = parseInt(secCode.charAt(i));
            if (++forward != current) {
                isForwardRun = false;
            }
            if (--backward != current) {
                isBackwardRun = false;
            }
        }
        result = !(isForwardRun || isBackwardRun);
    }
    return result;
};

function Reset() {
    $('#securityCode').val('');
    $('#securityCodeConfirm').val('');
    $('#securityCode').focus();
    document.getElementById('forceChange').checked = true;
};

$(document).ready(function () {
    parent.Fanex.ExtAcc.UnBlockUI();
});