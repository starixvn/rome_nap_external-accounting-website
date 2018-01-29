"use strict";

ColossusBetModule.controller('statusController', function ($scope, colossusBetRepositories) {
    colossusBetRepositories.getData().then(function () {
        $scope.originalstatusPartial = angular.copy($scope.statusPartial);
        $scope.$watch('statusPartial.IsEnabled', function (newValue, oldValue) {
            $scope.statusPartial.IsStatusUpdated = newValue != $scope.originalstatusPartial.IsEnabled;
        }, true);
    }, function (reason) { }, function (update) { });
});