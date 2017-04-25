"use strict";

ColossusBetModule.controller('statusController', function ($scope, colossusBetRepositories) {
    colossusBetRepositories.getData().then(function () {
        $scope.originalstatusPartial = angular.copy($scope.statusPartial);
        $scope.IsDownlinechkDisabled = true;

        $scope.$watch('statusPartial.IsEnabled', function (newValue, oldValue) {
            if (newValue) {
                if ($scope.originalstatusPartial.IsDownlinesEnabled) {
                    $scope.statusPartial.IsDownlinesEnabled = true;
                    $scope.IsDownlinechkDisabled = true;
                }
                else {
                    $scope.statusPartial.IsDownlinesEnabled = false;
                    $scope.IsDownlinechkDisabled = false;
                }
            }
            else {
                $scope.statusPartial.IsDownlinesEnabled = false;
                $scope.IsDownlinechkDisabled = true;
            }

            if (newValue == $scope.originalstatusPartial.IsEnabled) {
                $scope.statusPartial.IsStatusUpdated = false;
            }
            else {
                $scope.statusPartial.IsStatusUpdated = true;
            }
        }, true);

        $scope.$watch('statusPartial.IsDownlinesEnabled', function (newValue, oldValue) {
            if (newValue == $scope.originalstatusPartial.IsDownlinesEnabled) {
                $scope.statusPartial.IsStatusUpdated = false;
            } else {
                $scope.statusPartial.IsStatusUpdated = true;
            }
        }, true);
    }, function (reason) { }, function (update) { });
});