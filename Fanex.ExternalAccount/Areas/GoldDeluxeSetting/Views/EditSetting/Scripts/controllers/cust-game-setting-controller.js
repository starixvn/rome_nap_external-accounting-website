if (fanex.goldDeluxeSetting) {
    fanex.goldDeluxeSetting.controller('CustGameSettingController',
    [
        '$scope',
        '$http',
        'CustomerGameSettingService',
        function ($scope, $http, customerGameSettingService) {
            'use strict';
            $scope.gameSetting = fanex.cloneObject(settingViewModel.Setting.GameSetting);
            $scope.language = language;
            $scope.customerLevel = fanex.goldDeluxeSetting.customerLevel;
            $scope.level = settingViewModel.Level;

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
        }
    ]);
}
else {
    alert('Missing fanex.goldDeluxeSetting!');
}