"use strict";

ColossusBetModule.controller('statusController', function ($scope, colossusBetRepositories) {
    colossusBetRepositories.getData().then(function () {
        $scope.originalstatusPartial = angular.copy($scope.statusPartial);
        $scope.$watch('[statusPartial.IsEnabled, statusPartial.IsDownlinesEnabled]', function (newValue, oldValue) {
            if (newValue !== oldValue) {
                if ($scope.originalstatusPartial.IsEnabled) {
                    $scope.IsDownlinechkDisabled = true;
                    if ($scope.statusPartial.IsEnabled != $scope.originalstatusPartial.IsEnabled) {
                        if ($scope.originalstatusPartial.IsDownlinesEnabled) {
                            $scope.statusPartial.IsDownlinesEnabled = !$scope.originalstatusPartial.IsDownlinesEnabled;
                        }
                        $scope.statusPartial.IsStatusUpdated = true;
                    } else {
                        $scope.statusPartial.IsDownlinesEnabled = $scope.originalstatusPartial.IsDownlinesEnabled;
                        $scope.statusPartial.IsStatusUpdated = false;
                    }
                } else {
                    $scope.statusPartial.IsStatusUpdated = $scope.statusPartial.IsEnabled != $scope.originalstatusPartial.IsEnabled;
                    $scope.IsDownlinechkDisabled = !$scope.statusPartial.IsEnabled;
                }
            }
        }, true);

        $scope.$watch('isDownlinechkDisabled', function (newValue, oldValue) {
            if (newValue !== oldValue) {
                if ($scope.IsDownlinechkDisabled) {
                    $scope.statusPartial.IsDownlinesEnabled = false;
                }
            }
        }, true);

    }, function (reason) { }, function (update) { });
});