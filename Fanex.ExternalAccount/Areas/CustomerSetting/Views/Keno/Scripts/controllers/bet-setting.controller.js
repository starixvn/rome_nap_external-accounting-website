"use strict";

KenoModule.controller('betSettingController', function ($scope, kenoRepositories, betSettingService) {
    $scope.betSettingList = kenoRepositories.kenoSetting.betSettings;
    $scope.isBetSettingPartial = false;
});
