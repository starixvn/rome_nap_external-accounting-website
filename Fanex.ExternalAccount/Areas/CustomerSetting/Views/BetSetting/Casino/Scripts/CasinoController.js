if (typeof Fanex == 'undefined' || typeof Fanex.CustomerSetting == 'undefined' || typeof Fanex.CustomerSetting.Core == 'undefined') {
    alert('Require Fanex.CustomerSetting.Core.js');
};

Fanex.CustomerSetting.Betting = {};

Fanex.CustomerSetting.Betting.Casino = function ($scope, $element) {
    $scope.model = model;
    
    if (!model)
        return;

    $scope.customerModel = model != null ? model.customerModel : null;
    $scope.gameKinds = model != null ? $scope.model.settingModel.gameGroups : null;
  
};

Fanex.CustomerSetting.Betting.Casino.Update = function (ondone) {
    $(document).scrollTop(0);

    if (isBelongToSpecialGroups) {
        var isValidBetLimit = true;
        $('.stake-level').each(function () {
            var $checkBoxes = $('input[type=checkbox]', $(this));
            if ($('input[type=checkbox]:checked', $(this)).length === 0) {
                Validators.show($checkBoxes.get(0), 'Each bet type must have at least one Bet Limit value to be selected.', undefined, undefined, 1500);
                isValidBetLimit = false;
                return false; //break each loop
            }
        });

        if (!isValidBetLimit)
            return;
    }

    Fanex.CustomerSetting.Core.BlockUI();

    $.ajax({
        dataType: 'json',
        type: 'post',
        url: toUrl('CasinoBetting/EditBetSettingPost'),
        data: {
            custId: custId,
            uplineId: uplineId,
            userId: userId,
            txtCustomerObject: JSON.stringify(model)
        }
    })
    .success(function (data) {
        if (data.errorCode == "0") {
            $('#errMsg').attr('class', 'successMsg');
            $('#errMsg').html('<font color="Blue">Updated successfully</font>');
            $('#errMsg').show();
        }
        else {
            $('#errMsg').attr('class', 'errMsg');
            $('#errMsg').html(data.errorMessage.formatErrorMessage());
            $('#errMsg').show();
        }

        Fanex.CustomerSetting.Core.UnBlockUI();

        if (typeof ondone == 'function') {
            ondone(data);
        }
    })
    .error(function (data) {
        if (typeof console != 'undefined')
            console.log(data);

        if (JSON.parse(data.responseText).errorCode) {
            $('#errMsg').attr('class', 'errMsg');
            $('#errMsg').html(JSON.parse(data.responseText).errorMessage.formatErrorMessage());
            $('#errMsg').show();
        }

        Fanex.CustomerSetting.Core.UnBlockUI();

        if (typeof ondone == 'function') {
            ondone({ errorCode: "1", errorMessage: "Error on updating..." });
        }
    });
};