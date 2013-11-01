if (typeof Fanex == 'undefined' || typeof Fanex.CustomerSetting == 'undefined' || typeof Fanex.CustomerSetting.Core == 'undefined') {
    alert('Require Fanex.CustomerSetting.Core.js');
};

Fanex.CustomerSetting.Betting = {};

Fanex.CustomerSetting.Betting.Bingo = function ($scope, $element) {
    $scope.model = model;

    if (!model)
        return;

    $scope.gameKinds = $scope.model.gameKindList;

    $scope.Init = function () {
        $scope.model.maxTransferCredit = $scope.model.maxTransferCredit.formatNumber();
    };
};

Fanex.CustomerSetting.Betting.Bingo.Update = function (ondone) {
    var max = model.maxLimit;
    var isUnlimit = max == 0;

    if (model.maxTransferCredit.replaceAll(',', '') < model.minLimit && isUnlimit) {
        Validators.show($('#maxTransferCredit').get(0), 'Member Max Transfer Credit must be greater than or equals to ' + model.minLimit.formatNumber());
        return;
    };

    if (model.maxTransferCredit.replaceAll(',', '') < model.minLimit && !isUnlimit) {
        Validators.show($('#maxTransferCredit').get(0), 'Member Max Transfer Credit must be between ' + model.minLimit.formatNumber() + ' and ' + max.formatNumber());
        return;
    };

    Fanex.CustomerSetting.Core.BlockUI();

    $.ajax({
        dataType: 'json',
        type: 'post',
        url: toUrl('BingoBetting/EditBetSettingPost'),
        data: {
            id: custId,
            uplineId: uplineId,
            userId: userId,
            txtCustomerObject: JSON.stringify(model),
            siteName: siteName
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

        if (JSON.parse(data.responseText).ErrorCode) {
            $('#errMsg').attr('class', 'errMsg');
            $('#errMsg').html(JSON.parse(data.responseText).ErrorMessage.formatErrorMessage());
            $('#errMsg').show();
        }

        Fanex.CustomerSetting.Core.UnBlockUI();

        if (typeof ondone == 'function') {
            ondone({ errorCode: "1", errorMessage: "Error on updating..." });
        }
    });
};