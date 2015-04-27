"use strict";

KenoModule.directive('memberPositionTakingElement', ['commonService', '$templateCache', function (commonService, $templateCache) {
    return {
        restrict: 'A',
        scope: {
            positionTaking: "=ngModel",
            index: "=index"
        },
        templateUrl: $templateCache.get('memberPositionTakingTemplate'),
        link: function ($scope, elem, attr, ctrl) {
            $scope.originalPositionTaking = angular.copy($scope.positionTaking);
            $scope.fixIECount = 0;
            $scope.isPTFirstChange = false;

            $scope.a1Range = commonService.getRangePTValue($scope.positionTaking.a2, Math.max(0, $scope.positionTaking.n2));
            if ($scope.positionTaking.a2 < $scope.positionTaking.a1) {
                $scope.a1Range.unshift($scope.positionTaking.a1);
            }

            $scope.$on('resetPositionTaking', function (event, data) {
                if (data.betTypeId != $scope.positionTaking.betTypeId) {
                    $scope.positionTaking.a1 = $scope.originalPositionTaking.a1;
                }
            });

            $scope.setRangePTIfFirstChange = function () {
                if (($scope.originalPositionTaking.a1 != $scope.positionTaking.a1) && !$scope.isPTFirstChange) {
                    $scope.a1Range = commonService.getRangePTValue($scope.positionTaking.a2, Math.max(0, $scope.positionTaking.n2));
                    $scope.isPTFirstChange = true;
                }
            };

            $scope.$watch('positionTaking.a1', function () {
                $scope.positionTaking.isUpdateA1 = $scope.originalPositionTaking.a1 != $scope.positionTaking.a1;
                $scope.positionTaking.isUpdatePT = $scope.positionTaking.isUpdateA1;

                $scope.setRangePTIfFirstChange();
                $scope.fixIECount++;
                $scope.$emit('totallyChecking');
            }, true);
        }
    };
} ]);
