"use strict";

KenoModule.directive('maxBetInput', ['$filter', '$parse', 'commonService',
    function ($filter, $parse, commonService) {
        return {
            require: '^ngModel',
            link: function ($scope, element, attrs, ctrl) {
                if (!ctrl) return;

                $scope.formatInput(element, ctrl);

                $scope.$on('inputDisabledChecking', function (event) {
                    $scope.disabledInput(element);
                });

                $scope.setConstrainMaxbet = function () {
                    var modelValue = isNaN(ctrl.$modelValue) == true ? $scope.betSetting.maxBet : parseFloat(ctrl.$modelValue);
                    var maxValue = Math.min($scope.betSetting.upline.maxBet, $scope.betSetting.maxPerMatch);

                    if (modelValue > maxValue && maxValue < $scope.MAX_VALUE && !$scope.unlimited) {
                        var lastOfMessege = $scope.betSetting.upline.maxBet > $scope.betSetting.maxPerMatch ? "Max Per Match." : $scope.betSetting.upline.maxBet;
                        commonService.createTooltip(element, $scope.betSetting.betTypeName + " - Max Bet " + resources.less_than + " " + lastOfMessege);
                        $scope.valids.maxBet = false;
                        element.addClass('invalidValue');
                        return false;
                    }

                    commonService.createTooltip(element, "");
                    $scope.valids.maxBet = true;
                    element.removeClass('invalidValue');
                }
            }
        };
    }
]);