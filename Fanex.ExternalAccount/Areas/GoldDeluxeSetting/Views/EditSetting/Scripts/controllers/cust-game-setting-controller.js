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
            $scope.init = function () {
                $scope.checkShowWarningUncheckedAll();
            };

            $scope.getGameSetting = function () {
                return $scope.gameSetting;
            };

            $scope.updateGameSetting = function () {

                var custInfoScope = angular.element('[data-ng-controller="CustomerInfoController"]').scope();

                if (isBelongToSpecialGroups && $scope.mainResult.Warning !== undefined && $scope.mainResult.Warning.IsShow) {
                    return;
                }

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

                var message = language.getString(isBelongToSpecialGroups ? 'WarningUncheckAllStakeLevelMsg_DirectionV3' :'WarningUncheckAllStakeLevelMsg');
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