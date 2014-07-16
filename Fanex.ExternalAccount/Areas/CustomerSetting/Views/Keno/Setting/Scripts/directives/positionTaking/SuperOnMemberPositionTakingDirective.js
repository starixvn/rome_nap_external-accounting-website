"use strict";

KenoModule.directive('superOnMemberPositionTakingElement', ['commonService', '$templateCache', function (commonService, $templateCache) {
    return {
        restrict: 'A',
        scope: {
            positionTaking: "=ngModel",
            index: "=index"
        },
        templateUrl: $templateCache.get('superOnMemberPositionTakingTemplate'),
        link: function ($scope, elem, attr, ctrl) {
            $scope.originalPositionTaking = angular.copy($scope.positionTaking);

            $scope.fixIECount = 0;
            $scope.isAutoPTFirstChanged = false;
            $scope.isPTFirstChange = false;

            $scope.s1Range = commonService.getRangePTValue($scope.positionTaking.s1Max, $scope.positionTaking.s1US1Min);
            $scope.uS1Range = commonService.getRangePTValue($scope.positionTaking.s4, $scope.positionTaking.s1US1Min);

            $scope.setRangePTWhenInit = function () {
                if ($scope.positionTaking.s1Max < $scope.positionTaking.s1) {
                    $scope.s1Range.unshift($scope.positionTaking.s1);
                }

                if ($scope.positionTaking.s1US1Min > $scope.positionTaking.s1) {
                    $scope.s1Range.push($scope.positionTaking.s1);
                }

                if ($scope.positionTaking.s4 < $scope.positionTaking.uS1) {
                    $scope.uS1Range.unshift($scope.positionTaking.uS1);
                }

                if ($scope.positionTaking.s1US1Min > $scope.positionTaking.uS1) {
                    $scope.uS1Range.push($scope.positionTaking.uS1);
                }
            }

            $scope.setRangePTWhenInit();

            $scope.setRangePTIfFirstChange = function () {
                if (($scope.originalPositionTaking.uS1 != $scope.positionTaking.uS1) && !$scope.isAutoPTFirstChanged) {
                    $scope.uS1Range = commonService.getRangePTValue($scope.positionTaking.s4, $scope.positionTaking.s1US1Min);
                    $scope.isAutoPTFirstChanged = true;
                }

                if (($scope.originalPositionTaking.s1 != $scope.positionTaking.s1) && !$scope.isPTFirstChange) {
                    $scope.s1Range = commonService.getRangePTValue($scope.positionTaking.s1Max, $scope.positionTaking.s1US1Min);
                    $scope.isPTFirstChange = true;
                }
            };

            $scope.monitorSaveButton = function () {
                $scope.positionTaking.isUpdatePT = $scope.positionTaking.isUpdateS1 || $scope.positionTaking.isUpdateUS1;
                $scope.fixIECount++;
                $scope.$emit('totallyChecking');
            };


            $scope.$watch('[positionTaking.s1, positionTaking.uS1, positionTaking.iS1]', function (newValue, oldValue) {
                if (newValue !== oldValue) {
                    if ($scope.originalPositionTaking.iS1) { // Load in case within isAuto
                        if ($scope.positionTaking.iS1) {
                            $scope.positionTaking.isUpdateS1 = false;
                            $scope.positionTaking.isUpdateUS1 = ($scope.originalPositionTaking.uS1 != $scope.positionTaking.uS1) || ($scope.originalPositionTaking.iS1 != $scope.positionTaking.iS1);
                        }
                        else {
                            $scope.positionTaking.isUpdateS1 = $scope.originalPositionTaking.iS3 == true ? false : true;
                            $scope.positionTaking.isUpdateUS1 = true;
                        }
                    }
                    else {
                        if ($scope.positionTaking.iS1) {
                            $scope.positionTaking.isUpdateS1 = true;
                            $scope.positionTaking.isUpdateUS1 = true;
                        }
                        else {
                            $scope.positionTaking.isUpdateS1 = $scope.originalPositionTaking.s1 != $scope.positionTaking.s1;
                            $scope.positionTaking.isUpdateUS1 = false;
                        }
                    }

                    $scope.setRangePTIfFirstChange();
                    $scope.monitorSaveButton();
                }
            }, true);
        }
    };
} ]);
