if (fanex.goldDeluxeSetting) {
    fanex.goldDeluxeSetting.controller('CustGameSettingController',
    [
        '$scope',
        '$http',
        'CustomerGameSettingService',
        function ($scope, $http, customerGameSettingService) {
            'use strict';
            $scope.gameSetting = fanex.cloneObject(settingViewModel.Setting.GameSetting);
            var uplineUncheckedAllMsg = '';

            if (settingViewModel.Level === fanex.goldDeluxeSetting.customerLevel.Member) {
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
                if (settingViewModel.Level === fanex.goldDeluxeSetting.customerLevel.Member) {
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

                var custInfoScope = angular.element('[data-ng-controller="CustomerInfoController"]').scope();

                customerGameSettingService.updateGameSetting($scope.getGameSetting(), $scope.$parent);
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
    alert('Missing fanex.goldDeluxeSetting!');
}