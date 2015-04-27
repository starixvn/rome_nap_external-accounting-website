"use strict";

KenoModule.directive('superPositionTakingElement', ['commonService', '$templateCache', function (commonService, $templateCache) {
    return {
        restrict: 'A',
        scope: {
            positionTaking: "=ngModel",
            index: "=index"
        },
        transclude: true,
        templateUrl: $templateCache.get('superPositionTakingTemplate'),
        link: function ($scope, elem, attr, ctrl) {
            $scope.originalPositionTaking = angular.copy($scope.positionTaking);

            $scope.maxPTRange = commonService.getRangePTValue($scope.positionTaking.l4, 0);
            $scope.fixIECount = 0;

            $scope.monitorSaveButton = function () {
                $scope.positionTaking.isUpdatePT = $scope.positionTaking.isUpdateS4 || $scope.positionTaking.isUpdateN4;
                $scope.fixIECount++;
                $scope.$emit('totallyChecking');
            };

            $scope.$on('resetPositionTaking', function (event, data) {
                if (data.betTypeId != $scope.positionTaking.betTypeId) {
                    $scope.positionTaking.s4 = $scope.originalPositionTaking.s4;
                    $scope.positionTaking.n4 = $scope.originalPositionTaking.n4;
                }
            });

            $scope.$watch('positionTaking.s4', function () {
                $scope.positionTaking.isUpdateS4 = $scope.originalPositionTaking.s4 != $scope.positionTaking.s4;

                if ($scope.positionTaking.s4 < $scope.positionTaking.n4) {
                    $scope.positionTaking.n4 = $scope.positionTaking.s4;
                }

                $scope.minPTRange = commonService.getRangePTValue($scope.positionTaking.s4, 0);
                $scope.monitorSaveButton();
            }, true);

            $scope.$watch('positionTaking.n4', function () {
                $scope.positionTaking.isUpdateN4 = $scope.originalPositionTaking.n4 != $scope.positionTaking.n4;
                $scope.monitorSaveButton();
            }, true);

        }
    };
} ]);
