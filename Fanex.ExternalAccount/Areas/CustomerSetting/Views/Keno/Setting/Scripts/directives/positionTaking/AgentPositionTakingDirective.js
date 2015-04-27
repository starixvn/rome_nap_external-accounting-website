"use strict";

KenoModule.directive('agentPositionTakingElement', ['commonService', '$templateCache', function (commonService, $templateCache) {
    return {
        restrict: 'A',
        scope: {
            positionTaking: "=ngModel",
            index: "=index"
        },
        templateUrl: $templateCache.get('agentPositionTakingTemplate'),
        link: function ($scope, elem, attr, ctrl) {
            $scope.originalPositionTaking = angular.copy($scope.positionTaking);

            $scope.m2Range = $scope.uM2Range = commonService.getRangePTValue($scope.positionTaking.m3, 0);

            $scope.positionTaking.l2 = $scope.positionTaking.iM2 ? $scope.positionTaking.m3 : commonService.roundPTValue($scope.positionTaking.m3 - $scope.positionTaking.m2);
            $scope.minOfMaxPTRange = commonService.roundPTValue($scope.positionTaking.n3 - ($scope.positionTaking.iM2 ? $scope.positionTaking.uM2 : $scope.positionTaking.m2));
            $scope.maxPTRange = commonService.getRangePTValue($scope.positionTaking.l2, Math.max($scope.minOfMaxPTRange, 0));
            if ($scope.positionTaking.a2 < $scope.minOfMaxPTRange) {
                $scope.maxPTRange.push($scope.positionTaking.a2);
            }

            $scope.minPTRange = commonService.getRangePTValue($scope.positionTaking.a2, 0);
            $scope.positionTaking.memberNewValueRange = {};

            $scope.fixIECount = 0;

            $scope.monitorSaveButton = function () {
                $scope.positionTaking.isUpdatePT = $scope.positionTaking.isUpdateUM2 || $scope.positionTaking.isUpdateM2 || $scope.positionTaking.isUpdateN2 || $scope.positionTaking.isUpdateA2;
                $scope.fixIECount++;
                $scope.$emit('totallyChecking');
            };

            $scope.$on('resetPositionTaking', function (event, data) {
                if (data.betTypeId != $scope.positionTaking.betTypeId) {
                    $scope.positionTaking.m2 = $scope.originalPositionTaking.m2;
                    $scope.positionTaking.uM2 = $scope.originalPositionTaking.uM2;
                    $scope.positionTaking.iM2 = $scope.originalPositionTaking.iM2;
                    $scope.positionTaking.a2 = $scope.originalPositionTaking.a2;
                    $scope.positionTaking.n2 = $scope.originalPositionTaking.n2;
                }
            });

            $scope.arangeMemberRange = function () {
                var minOfMemberNewValueRange = Math.max(0, commonService.roundPTValue($scope.positionTaking.n3 - $scope.positionTaking.a2));
                var maxOfMemberNewValueRange = commonService.roundPTValue($scope.positionTaking.m3 - $scope.positionTaking.a2);
                $scope.positionTaking.newMemberPT = $scope.positionTaking.m2;
                $scope.positionTaking.memberNewValueRange = maxOfMemberNewValueRange <= 0 ? [0] : commonService.getRangePTValue(maxOfMemberNewValueRange, minOfMemberNewValueRange);
            };

            $scope.$watch('[positionTaking.m2, positionTaking.uM2, positionTaking.iM2]', function (newValue, oldValue) {
                if (newValue !== oldValue) {
                    $scope.positionTaking.l2 = $scope.positionTaking.iM2 ? $scope.positionTaking.m3 : commonService.roundPTValue($scope.positionTaking.m3 - $scope.positionTaking.m2);

                    var minRange = commonService.roundPTValue($scope.positionTaking.n3 - ($scope.positionTaking.iM2 ? $scope.positionTaking.uM2 : $scope.positionTaking.m2));
                    $scope.maxPTRange = commonService.getRangePTValue($scope.positionTaking.l2, Math.max(minRange, 0));

                    if ($scope.positionTaking.iM2) {
                        if ($scope.positionTaking.a2 < minRange || $scope.positionTaking.a2 > $scope.positionTaking.l2) {
                            $scope.positionTaking.a2 = $scope.positionTaking.l2;
                        }

                        if ($scope.originalPositionTaking.iM2) {
                            $scope.positionTaking.isUpdateM2 = false;
                            $scope.positionTaking.isUpdateUM2 = ($scope.originalPositionTaking.uM2 != $scope.positionTaking.uM2) || ($scope.originalPositionTaking.iM2 != $scope.positionTaking.iM2);
                        }
                        else {
                            $scope.positionTaking.isUpdateM2 = true;
                            $scope.positionTaking.isUpdateUM2 = true;
                        }

                        $scope.positionTaking.isConfirmIsAuto = false;
                        $scope.$emit('confirmIsAuto', { "betTypeId": $scope.positionTaking.betTypeId, "isAdd": false });
                    }
                    else {
                        $scope.positionTaking.a2 = commonService.roundPTValue($scope.positionTaking.m3 - $scope.positionTaking.m2);

                        if ($scope.originalPositionTaking.iM2) {
                            $scope.positionTaking.isUpdateM2 = true;
                            $scope.positionTaking.isUpdateUM2 = true;
                        }
                        else {
                            $scope.positionTaking.isUpdateM2 = $scope.originalPositionTaking.m2 != $scope.positionTaking.m2;
                            $scope.positionTaking.isUpdateUM2 = false;
                        }

                        $scope.arangeMemberRange();
                        $scope.positionTaking.isConfirmIsAuto = true;
                        $scope.$emit('confirmIsAuto', { "betTypeId": $scope.positionTaking.betTypeId, "isAdd": true });
                    }

                    $scope.monitorSaveButton();
                }
            }, true);

            $scope.$watch('positionTaking.a2', function (newValue, oldValue) {
                if (newValue !== oldValue) {
                    $scope.positionTaking.isUpdateA2 = $scope.originalPositionTaking.a2 != $scope.positionTaking.a2;

                    if ($scope.positionTaking.a2 <= $scope.positionTaking.n2) {
                        $scope.positionTaking.n2 = $scope.positionTaking.a2;
                    }

                    $scope.arangeMemberRange();
                    $scope.minPTRange = commonService.getRangePTValue($scope.positionTaking.a2, 0);
                    $scope.monitorSaveButton();
                }
            }, true);

            $scope.$watch('positionTaking.n2', function (newValue, oldValue) {
                if (newValue !== oldValue) {
                    $scope.positionTaking.isUpdateN2 = $scope.originalPositionTaking.n2 != $scope.positionTaking.n2;
                    $scope.monitorSaveButton();
                }
            }, true)
        }
    };
} ]);
