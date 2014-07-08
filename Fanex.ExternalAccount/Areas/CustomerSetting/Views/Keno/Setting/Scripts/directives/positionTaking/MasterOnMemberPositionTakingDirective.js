"use strict";

KenoModule.directive('masterOnMemberPositionTakingElement', ['commonService', '$templateCache', function (commonService, $templateCache) {
    return {
        restrict: 'A',
        scope: {
            positionTaking: "=ngModel",
            index: "=index"
        },
        templateUrl: $templateCache.get('masterOnMemberPositionTakingTemplate'),
        link: function ($scope, elem, attr, ctrl) {
            $scope.originalPositionTaking = angular.copy($scope.positionTaking);

            $scope.fixIECount = 0;
            $scope.isAutoPTFirstChanged = false;
            $scope.isPTFirstChange = false;

            $scope.m1Range = commonService.getRangePTValue($scope.positionTaking.m1Max, $scope.positionTaking.m1UM1Min);
            $scope.uM1Range = commonService.getRangePTValue($scope.positionTaking.m3, $scope.positionTaking.m1UM1Min);

            $scope.setRangePTWhenInit = function () {
                if ($scope.positionTaking.m1Max < $scope.positionTaking.m1) {
                    $scope.m1Range.unshift($scope.positionTaking.m1);
                }

                if ($scope.positionTaking.m1UM1Min > $scope.positionTaking.m1) {
                    $scope.m1Range.push($scope.positionTaking.s1);
                }

                if ($scope.positionTaking.m3 < $scope.positionTaking.uM1) {
                    $scope.uM1Range.unshift($scope.positionTaking.uM1);
                }

                if ($scope.positionTaking.m1UM1Min > $scope.positionTaking.uM1) {
                    $scope.uM1Range.push($scope.positionTaking.uM1);
                }
            }

            $scope.setRangePTWhenInit();

            $scope.setRangePTIfFirstChange = function () {
                if (($scope.originalPositionTaking.uM1 != $scope.positionTaking.uM1 && !$scope.isAutoPTFirstChanged)) {
                    $scope.uM1Range = commonService.getRangePTValue($scope.positionTaking.m3, $scope.positionTaking.m1UM1Min);
                    $scope.isAutoPTFirstChanged = true;
                }

                if (($scope.originalPositionTaking.m1 != $scope.positionTaking.m1 && !$scope.isPTFirstChange)) {
                    $scope.m1Range = commonService.getRangePTValue($scope.positionTaking.m1Max, $scope.positionTaking.m1UM1Min);
                    $scope.isPTFirstChange = true;
                }
            };

            $scope.monitorSaveButton = function () {
                $scope.positionTaking.isUpdatePT = $scope.positionTaking.isUpdateM1 || $scope.positionTaking.isUpdateUM1;
                $scope.fixIECount++;
                $scope.$emit('totallyChecking');
            };

            $scope.$watch('[positionTaking.m1, positionTaking.uM1,  positionTaking.iM1]', function (newValue, oldValue) {
                if (newValue !== oldValue) {
                    if ($scope.originalPositionTaking.iM1) { // Load in case within isAuto
                        if ($scope.positionTaking.iM1) {
                            $scope.positionTaking.isUpdateM1 = false;
                            $scope.positionTaking.isUpdateUM1 = ($scope.originalPositionTaking.uM1 != $scope.positionTaking.uM1) || ($scope.originalPositionTaking.iM1 != $scope.positionTaking.iM1);
                        }
                        else {
                            $scope.positionTaking.isUpdateM1 = true;
                            $scope.positionTaking.isUpdateUM1 = true;
                        }
                    }
                    else {
                        if ($scope.positionTaking.iM1) {
                            $scope.positionTaking.isUpdateM1 = true;
                            $scope.positionTaking.isUpdateUM1 = true;
                        }
                        else {
                            $scope.positionTaking.isUpdateM1 = $scope.originalPositionTaking.m1 != $scope.positionTaking.m1;
                            $scope.positionTaking.isUpdateUM1 = false;
                        }
                    }

                    $scope.setRangePTIfFirstChange();
                    $scope.monitorSaveButton();
                }
            }, true);
        }
    };
} ]);
