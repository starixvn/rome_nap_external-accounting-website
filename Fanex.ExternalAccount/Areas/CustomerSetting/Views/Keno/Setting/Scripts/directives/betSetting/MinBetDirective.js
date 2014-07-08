"use strict";

KenoModule.directive('minBetInput', ['$filter', '$parse', 'commonService',
    function ($filter, $parse, commonService) {
        return {
            require: '^ngModel',
            link: function ($scope, element, attrs, ctrl) {
                if (!ctrl) return;

                $scope.formatInput(element, ctrl);

                $scope.$on('inputDisabledChecking', function (event) {
                    $scope.disabledInput(element);
                });

                $scope.setConstrainMinBet = function () {
                    var modelValue = isNaN(ctrl.$modelValue) == true ? $scope.betSetting.minBet : parseFloat(ctrl.$modelValue);
                    var minValue = $scope.betSetting.upline.minBet;
                    var maxValue = $scope.betSetting.maxBet;

                    if (modelValue < minValue && minValue < $scope.MAX_VALUE && !$scope.unlimited) {
                        commonService.createTooltip(element, $scope.betSetting.betTypeName + " - Min Bet " + resources.more_than + " " + minValue);
                        $scope.valids.minBet = false;
                        element.addClass('invalidValue');
                        return false;
                    }

                    if (modelValue > maxValue && maxValue < $scope.MAX_VALUE && !$scope.unlimited) {
                        commonService.createTooltip(element, $scope.betSetting.betTypeName + " - Min Bet " + resources.less_than + " Max Bet.");
                        $scope.valids.minBet = false;
                        element.addClass('invalidValue');
                        return false;
                    }

                    commonService.createTooltip(element, "");
                    $scope.valids.minBet = true;
                    element.removeClass('invalidValue');
                }
            }
        };
    }
]);