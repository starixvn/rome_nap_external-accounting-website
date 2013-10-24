function Update() {
    if (!Validators.validateInput($('#securityCode').val())) {
        Validators.show($('#securityCode').get(0), "Sorry, security code can't be empty", undefined, undefined, 3000);
        return;
    }

    if (!IsSecurityCode($('#securityCode').val())) {
        Validators.show($('#securityCode').get(0), "Sorry, security code is incorrect format", undefined, undefined, 3000);
        return;
    }

    if (!Validators.validateInput($('#securityCodeConfirm').val())) {
        Validators.show($('#securityCodeConfirm').get(0), "Sorry, confirm security code can't be empty", undefined, undefined, 3000);
        return;
    }

    if (!IsSecurityCode($('#securityCodeConfirm').val())) {
        Validators.show($('#securityCodeConfirm').get(0), "Sorry, confirm security code is incorrect format", undefined, undefined, 3000);
        return;
    }

    if($('#securityCode').val() != $('#securityCodeConfirm').val()){
        Validators.show($('#securityCodeConfirm').get(0), "Sorry, confirm security code doesn't match with security code", undefined, undefined, 3000);
        return;
    }

    Fanex.CustomerSetting.Core.BlockUI();

    $.ajax({
        url: parent.resolveClientUrl('CustomerSetting/LeftPanel/SearchCustomer/UpdateSecurityCode'),
        type: 'post',
        data: {
            id: custId,
            code: $('#securityCode').val(),
            force: document.getElementById('forceChange').checked,
            roleId: roleId
        }
    }).success(function (result) {
        if (result.IsSuccess) {
            $('#errMsg').attr('class', 'successMsg');
            $('#errMsg').html('<font color="Blue">Updated successfully</font>');
            $('#errMsg').show();
        }
        else {
            $('#errMsg').attr('class', 'errMsg');
            $('#errMsg').html(result.ErrorMessage.formatErrorMessage());
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