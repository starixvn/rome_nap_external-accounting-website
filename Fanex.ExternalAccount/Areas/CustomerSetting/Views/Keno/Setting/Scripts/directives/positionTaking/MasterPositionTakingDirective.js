"use strict";

KenoModule.directive('masterPositionTakingElement', ['commonService', '$templateCache', function (commonService, $templateCache) {
    return {
        restrict: 'A',
        scope: {
            positionTaking: "=ngModel",
            index: "=index"
        },
        transclude: true,
        templateUrl: $templateCache.get('masterPositionTakingTemplate'),
        link: function ($scope, elem, attr, ctrl) {
            $scope.originalPositionTaking = angular.copy($scope.positionTaking);

            $scope.s3Range = $scope.uS3Range = commonService.getRangePTValue($scope.positionTaking.s4, 0);

            $scope.positionTaking.l3 = $scope.positionTaking.iS3 ? $scope.positionTaking.s4 : commonService.roundPTValue($scope.positionTaking.s4 - $scope.positionTaking.s3);
            $scope.minOfMaxPTRange = commonService.roundPTValue($scope.positionTaking.n4 - ($scope.positionTaking.iS3 ? $scope.positionTaking.uS3 : $scope.positionTaking.s3));
            $scope.maxPTRange = commonService.getRangePTValue($scope.positionTaking.l3, Math.max($scope.minOfMaxPTRange, 0));
            if ($scope.positionTaking.m3 < $scope.minOfMaxPTRange) {
                $scope.maxPTRange.push($scope.positionTaking.m3);
            }

            $scope.minPTRange = commonService.getRangePTValue($scope.positionTaking.m3, 0);
            $scope.positionTaking.memberNewValueRange = {};

            $scope.fixIECount = 0;

            $scope.monitorSaveButton = function () {
                $scope.positionTaking.isUpdatePT = $scope.positionTaking.isUpdateUS3 || $scope.positionTaking.isUpdateS3 || $scope.positionTaking.isUpdateN3 || $scope.positionTaking.isUpdateM3;
                $scope.fixIECount++;
                $scope.$emit('totallyChecking');
            };

            $scope.$on('resetPositionTaking', function (event, data) {
                if (data.betTypeId != $scope.positionTaking.betTypeId) {
                    $scope.positionTaking.s3 = $scope.originalPositionTaking.s3;
                    $scope.positionTaking.uS3 = $scope.originalPositionTaking.uS3;
                    $scope.positionTaking.iS3 = $scope.originalPositionTaking.iS3;
                    $scope.positionTaking.m3 = $scope.originalPositionTaking.m3;
                    $scope.positionTaking.n3 = $scope.originalPositionTaking.n3;
                }
            });

            $scope.arangeMemberRange = function () {
                var minOfMemberNewValueRange = Math.max(0, commonService.roundPTValue($scope.positionTaking.n4 - $scope.positionTaking.m3));
                var maxOfMemberNewValueRange = commonService.roundPTValue($scope.positionTaking.s4 - $scope.positionTaking.m3);
                $scope.positionTaking.newMemberPT = $scope.positionTaking.s3;
                $scope.positionTaking.memberNewValueRange = maxOfMemberNewValueRange <= 0 ? [0] : commonService.getRangePTValue(maxOfMemberNewValueRange, minOfMemberNewValueRange);
            };

            $scope.$watch('[positionTaking.s3, positionTaking.uS3, positionTaking.iS3]', function (newValue, oldValue) {
                if (newValue !== oldValue) {
                    $scope.positionTaking.l3 = $scope.positionTaking.iS3 ? $scope.positionTaking.s4 : commonService.roundPTValue($scope.positionTaking.s4 - $scope.positionTaking.s3);

                    var minRange = commonService.roundPTValue($scope.positionTaking.n4 - ($scope.positionTaking.iS3 ? $scope.positionTaking.uS3 : $scope.positionTaking.s3));
                    $scope.maxPTRange = commonService.getRangePTValue($scope.positionTaking.l3, Math.max(minRange, 0));

                    if ($scope.positionTaking.iS3) {
                        if ($scope.positionTaking.m3 < minRange || $scope.positionTaking.m3 > $scope.positionTaking.l3) {
                            $scope.positionTaking.m3 = $scope.positionTaking.l3;
                        }

                        if ($scope.originalPositionTaking.iS3) {
                            $scope.positionTaking.isUpdateS3 = false;
                            $scope.positionTaking.isUpdateUS3 = ($scope.originalPositionTaking.uS3 != $scope.positionTaking.uS3) || ($scope.originalPositionTaking.iS3 != $scope.positionTaking.iS3);
                        }
                        else {
                            $scope.positionTaking.isUpdateS3 = true;
                            $scope.positionTaking.isUpdateUS3 = true;
                        }

                        $scope.positionTaking.isConfirmIsAuto = false;
                        $scope.$emit('confirmIsAuto', { "betTypeId": $scope.positionTaking.betTypeId, "isAdd": false });
                    }
                    else {
                        $scope.positionTaking.m3 = commonService.roundPTValue($scope.positionTaking.s4 - $scope.positionTaking.s3);

                        if ($scope.originalPositionTaking.iS3) {
                            $scope.positionTaking.isUpdateS3 = true;
                            $scope.positionTaking.isUpdateUS3 = true;
                        }
                        else {
                            $scope.positionTaking.isUpdateS3 = $scope.originalPositionTaking.s3 != $scope.positionTaking.s3;
                            $scope.positionTaking.isUpdateUS3 = false;
                        }

                        $scope.arangeMemberRange();
                        $scope.positionTaking.isConfirmIsAuto = true;
                        $scope.$emit('confirmIsAuto', { "betTypeId": $scope.positionTaking.betTypeId, "isAdd": true });

                    }

                    $scope.monitorSaveButton();
                }
            }, true);

            $scope.$watch('positionTaking.m3', function (newValue, oldValue) {
                if (newValue !== oldValue) {
                    $scope.positionTaking.isUpdateM3 = $scope.originalPositionTaking.m3 != $scope.positionTaking.m3;

                    if ($scope.positionTaking.m3 <= $scope.positionTaking.n3) {
                        $scope.positionTaking.n3 = $scope.positionTaking.m3;
                    }

                    $scope.minPTRange = commonService.getRangePTValue($scope.positionTaking.m3, 0);
                    $scope.arangeMemberRange();
                    $scope.monitorSaveButton();
                }
            }, true);

            $scope.$watch('positionTaking.n3', function (newValue, oldValue) {
                if (newValue !== oldValue) {
                    $scope.positionTaking.isUpdateN3 = $scope.originalPositionTaking.n3 != $scope.positionTaking.n3;
                    $scope.monitorSaveButton();
                }
            }, true)
        }
    };
} ]);

