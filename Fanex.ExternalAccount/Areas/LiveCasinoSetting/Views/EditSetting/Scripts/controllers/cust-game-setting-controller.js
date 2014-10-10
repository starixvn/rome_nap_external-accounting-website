/* File Created: July 21, 2014 */
/*global settingContext, language*/
if (fanex.liveCasinoSetting) {
    fanex.liveCasinoSetting.controller('CustGameSettingController',
    [
        '$scope',
        '$http',
        'CustomerGameSettingService',
        function ($scope, $http, customerGameSettingService) {
            'use strict';
            $scope.gameSetting = fanex.cloneObject(settingContext.Customer.LiveCasinoSetting.GameSetting);
            var uplineUncheckedAllMsg = '';

            if (settingContext.Customer.Level === fanex.liveCasinoSetting.customerLevel.Member && settingContext.Customer.IsSyncLiveCasino) {
                angular.forEach($scope.gameSetting, function (gameType) {
                    if (gameType.CheckedStakeLevelId === '') {
                        gameType.BettingLimit[0].IsChecked = true;
                        gameType.CheckedStakeLevelId = String(gameType.BettingLimit[0].Id);
                        uplineUncheckedAllMsg += gameType.Name + ', ';
                    }
                });

                if (uplineUncheckedAllMsg !== '') {
                    uplineUncheckedAllMsg = '{' + uplineUncheckedAllMsg.slice(0, -2) + '} ' + language.getString('WarningUplineUncheckAllStakeLevelMsg');
                    $scope.$parent.mainResult.Warning = {
                        IsShow: true,
                        Message: uplineUncheckedAllMsg
                    };
                }
            }

            $scope.getGameSetting = function () {
                if (settingContext.Customer.Level === fanex.liveCasinoSetting.customerLevel.Member) {
                    for (var i = 0; i < $scope.gameSetting.length; i++) {
                        var gameType = $scope.gameSetting[i];
                        for (var j = 0; j < gameType.BettingLimit.length; j++) {
                            var bettingLimit = gameType.BettingLimit[j];
                            bettingLimit.IsChecked = bettingLimit.Id === parseInt(gameType.CheckedStakeLevelId);
                        }
                    }
                }

                return $scope.gameSetting;
            };

            $scope.updateGameSetting = function () {
                if (!settingContext.IsSyncLiveCasino) {
                    $scope.$parent.validateAndUpdateSetting(fanex.liveCasinoSetting.updateType.GameSetting);
                    return;
                }

                var custInfoScope = angular.element('[data-ng-controller="CustomerInfoController"]').scope();
                var customer = custInfoScope.getCustInfo();

                customerGameSettingService.updateGameSetting($scope.getGameSetting(), customer.DisableLiveCasino, $scope.$parent);
            };

            $scope.checkShowWarningUncheckedAll = function () {
                var isShowWarning = false;
                angular.forEach($scope.gameSetting, function (gameType) {
                    var isUncheckedAll = true;
                    angular.forEach(gameType.BettingLimit, function (stakeLevel) {
                        if (stakeLevel.IsChecked) {
                            isUncheckedAll = false;
                            return;
                        }
                    });

                    if (isUncheckedAll) {
                        isShowWarning = true;
                        return;
                    }
                });

                var message = language.getString('WarningUncheckAllStakeLevelMsg');
                $scope.$parent.mainResult.Warning = {
                    IsShow: isShowWarning,
                    Message: message
                };
            };
        }
        ]);
}
else {
    alert('Missing fanex.liveCasinoSetting!');
}