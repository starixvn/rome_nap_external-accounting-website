"use strict";

KenoModule.controller('customerInfoController', function ($scope, kenoRepositories) {
    $scope.customerInfo = kenoRepositories.kenoSetting.customerInformation;
    return {
        getScope: function () {
            return $scope;
        }
    }
});
