"use strict";

KenoModule.directive('betSettingElement', ['$templateCache', function ($templateCache) {
    return {
        restrict: 'EA',
        priority: 1,
        $scope: {},
        templateUrl: $templateCache.get('betSettingTemplate'),
        controller: ['$scope', '$filter', '$parse',
            function ($scope, $filter, $parse) {
                $scope.originalBetSetting = angular.copy($scope.betSetting);
                $scope.MAX_VALUE = Math.pow(2, 53);
                $scope.valids = {
                    "minBet": true,
                    "maxBet": true,
                    "maxPerMatch": true
                };

                $scope.unlimited = false;
                $scope.isDownlinechkDisabled = true;

                if ($scope.betSetting.upline.minBet == 0) {
                    $scope.betSetting.upline.minBet = $scope.MAX_VALUE;
                }

                if ($scope.betSetting.upline.maxBet == 0) {
                    $scope.betSetting.upline.maxBet = $scope.MAX_VALUE;
                }

                if ($scope.betSetting.upline.maxPerMatch == 0) {
                    $scope.betSetting.upline.maxPerMatch = $scope.MAX_VALUE;
                }

                $scope.disabledInput = function (element) {
                    if ($scope.betSettingCopyAll.isCopiedAll && $scope.$index != 0) {
                        element.addClass('inputDisabled');
                        $(element).attr('disabled', true);
                    } else {
                        element.removeClass('inputDisabled');
                        $(element).removeAttr('disabled');
                    }
                }

                $scope.formatNumber = function (value) {
                    return $filter("number")(value);
                };

                $scope.formatInput = function (element, ctrl) {
                    ctrl.$formatters.unshift(function () {
                        return $scope.formatNumber(ctrl.$modelValue);
                    });

                    ctrl.$parsers.unshift(function (viewValue) {
                        if (viewValue == "") {
                            viewValue = "0";
                        }

                        var plainNumber = viewValue.replace(/[^\d|\-+|\.+]/g, '');
                        ctrl.$modelValue = eval(plainNumber);
                        element.val($scope.formatNumber(plainNumber));
                        return plainNumber;
                    });
                }

                $scope.$watch('[betSetting.minBet,betSetting.maxBet,betSetting.maxPerMatch,valids]', function () {
                    $scope.unlimited = $scope.betSetting.minBet == 0 && $scope.betSetting.maxBet == 0 && $scope.betSetting.maxPerMatch == 0;

                    $scope.betSetting.isValid = ($scope.valids.minBet && $scope.valids.maxBet && $scope.valids.maxPerMatch) || $scope.unlimited;

                    $scope.betSetting.isBetSettingUpdated = ($scope.betSetting.minBet != $scope.originalBetSetting.minBet) || ($scope.betSetting.maxBet != $scope.originalBetSetting.maxBet) || ($scope.betSetting.maxPerMatch != $scope.originalBetSetting.maxPerMatch);

                    $scope.$emit('totallyChecking');
                    $scope.setConstrainMinBet();
                    $scope.setConstrainMaxbet();
                    $scope.setConstrainMaxPerMatch();

                }, true);


                $scope.$watch('betSetting.isEnabled', function (newValue, oldValue) {
                    if (newValue !== oldValue) {
                        if ($scope.originalBetSetting.isEnabled) {
                            $scope.isDownlinechkDisabled = true;
                            if ($scope.betSetting.isEnabled != $scope.originalBetSetting.isEnabled) {
                                if ($scope.originalBetSetting.isDownlinesEnabled) {
                                    $scope.betSetting.isDownlinesEnabled = !$scope.originalBetSetting.isDownlinesEnabled;
                                }
                                $scope.betSetting.isStatusUpdated = true;
                            } else {
                                $scope.betSetting.isDownlinesEnabled = $scope.originalBetSetting.isDownlinesEnabled;
                                $scope.betSetting.isStatusUpdated = false;
                            }
                        } else {
                            $scope.betSetting.isStatusUpdated = $scope.betSetting.isEnabled != $scope.originalBetSetting.isEnabled;
                            $scope.isDownlinechkDisabled = $scope.betSetting.isEnabled == false;
                        }

                        $scope.$emit('totallyChecking');
                    }
                }, true);

                $scope.$watch('isDownlinechkDisabled', function (newValue, oldValue) {
                    if (newValue !== oldValue) {
                        if ($scope.isDownlinechkDisabled) {
                            $scope.betSetting.isDownlinesEnabled = false;
                        }
                    }
                }, true);
            }
        ]
    };
} ]);
