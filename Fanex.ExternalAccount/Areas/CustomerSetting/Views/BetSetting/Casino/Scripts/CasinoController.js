var module = angular.module("casinoApp", []);

if (typeof Fanex == 'undefined' || typeof Fanex.CustomerSetting == 'undefined' || typeof Fanex.CustomerSetting.Core == 'undefined') {
    alert('Require Fanex.CustomerSetting.Core.js');
};

Fanex.CustomerSetting.Betting = {};

Fanex.CustomerSetting.Betting.Casino = function ($scope, $http) {
    $scope.model = model;
    $scope.customerModel = model != null ? model.customerModel : null;
    $scope.gameKinds = model != null ? $scope.model.settingModel.gameGroups : null;

    $scope.Init = function () {
        if ($scope.customerModel)
            $scope.customerModel.maxTransferCredit = $scope.customerModel.maxTransferCredit.formatNumber();
    };
};

Fanex.CustomerSetting.Betting.Casino.Update = function (ondone) {
    $(document).scrollTop(0);

    var max = model.customerModel.maxWin;
    var isUnlimit = max == 0;

    if (model.customerModel.maxTransferCredit.replaceAll(',', '') < model.customerModel.minTransferCreditLimit && isUnlimit) {
        Validators.show($('#maxTransferCredit').get(0), 'Member Max Transfer Credit must be greater than or equals to ' + model.customerModel.minTransferCreditLimit.formatNumber(), undefined, undefined, 3000);
        return;
    };

    if (model.customerModel.maxTransferCredit.replaceAll(',', '') < model.customerModel.minTransferCreditLimit && !isUnlimit) {
        Validators.show($('#maxTransferCredit').get(0), 'Member Max Transfer Credit must be between ' + model.customerModel.minTransferCreditLimit.formatNumber() + ' and ' + max.formatNumber(), undefined, undefined, 3000);
        return;
    };

    Fanex.CustomerSetting.Core.BlockUI();

    $.ajax({
        dataType: 'json',
        type: 'post',
        url: toUrl('CasinoBetting/EditBetSettingPost'),
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