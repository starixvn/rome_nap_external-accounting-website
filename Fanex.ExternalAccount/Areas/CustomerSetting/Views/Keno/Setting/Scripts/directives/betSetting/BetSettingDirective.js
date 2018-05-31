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
                $scope.notAllowPlaceBets = false;

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
                    if (value.toString().endsWith(".")) {
                        return value;
                    }

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
                    $scope.notAllowPlaceBets = $scope.betSetting.minBet == 0 && $scope.betSetting.maxBet == 0.1 && $scope.betSetting.maxPerMatch == 0.1;

                    $scope.betSetting.isValid = ($scope.valids.minBet && $scope.valids.maxBet && $scope.valids.maxPerMatch)
                        || $scope.unlimited
                        || $scope.notAllowPlaceBets;

                    $scope.betSetting.isBetSettingUpdated = ($scope.betSetting.minBet != $scope.originalBetSetting.minBet) || ($scope.betSetting.maxBet != $scope.originalBetSetting.maxBet) || ($scope.betSetting.maxPerMatch != $scope.originalBetSetting.maxPerMatch);

                    $scope.$emit('totallyChecking');
                    $scope.setConstrainMinBet();
                    $scope.setConstrainMaxbet();
                    $scope.setConstrainMaxPerMatch();

                }, true);


                $scope.$watch('betSetting.isEnabled', function (newValue, oldValue) {
                    if (newValue) {
                        if ($scope.originalBetSetting.isDownlinesEnabled) {
                            $scope.betSetting.isDownlinesEnabled = true;
                            $scope.isDownlinechkDisabled = true;
                        }
                        else {
                            $scope.betSetting.isDownlinesEnabled = false;
                            $scope.isDownlinechkDisabled = false;
                        }
                    }
                    else {
                        $scope.betSetting.isDownlinesEnabled = false;
                        $scope.isDownlinechkDisabled = true;
                    }

                    if (newValue == $scope.originalBetSetting.isEnabled) {
                        $scope.betSetting.isStatusUpdated = false;
                    }
                    else {
                        $scope.betSetting.isStatusUpdated = true;
                    }

                    $scope.$emit('totallyChecking');
                }, true);



                $scope.$watch('betSetting.isDownlinesEnabled', function (newValue, oldValue) {
                    if (newValue == $scope.originalBetSetting.isDownlinesEnabled) {
                        $scope.betSetting.isStatusUpdated = false;
                    } else {
                        $scope.betSetting.isStatusUpdated = true;
                    }

                    $scope.$emit('totallyChecking');
                }, true);
            }
        ]
    };
}]);
