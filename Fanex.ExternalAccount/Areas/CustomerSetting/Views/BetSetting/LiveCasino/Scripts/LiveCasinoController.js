var module = angular.module("liveCasinoApp", []);

if (typeof Fanex == 'undefined' || typeof Fanex.CustomerSetting == 'undefined' || typeof Fanex.CustomerSetting.Core == 'undefined') {
    alert('Require Fanex.CustomerSetting.Core.js');
};

Fanex.CustomerSetting.Betting = {};

Fanex.CustomerSetting.Betting.LiveCasino = function ($scope, $http) {
    $scope.model = model;
    $scope.gameTypes = $scope.model.bettingLimits;

    $scope.ChangeLimit = function (scope) {
        var gameTypes = scope.gameTypes;
        var allUnchecked = true;

        for (var i = 0; i < gameTypes.length; i++) {
            var bettings = gameTypes[i].bettingLimit;

            if (scope.gameType.id == gameTypes[i].id) {
                for (var j = 0; j < bettings.length; j++) {
                    if (bettings[j].isSelected) {
                        allUnchecked = false;
                        break;
                    }
                }

                if (!allUnchecked) {
                    break;
                }
            }
        }

        if (allUnchecked) {
            $('#warningMsg').show();
        }
        else {
            $('#warningMsg').hide();
        }
    };
};

Fanex.CustomerSetting.Betting.LiveCasino.Update = function (ondone) {
    Fanex.CustomerSetting.Core.BlockUI();

    $.ajax({
        dataType: 'json',
        type: 'post',
        url: toUrl('LiveCasinoBetting/EditBetSettingPost'),
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