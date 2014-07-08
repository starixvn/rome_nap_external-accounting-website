"use strict";

KenoModule.directive('maxPerMatchInput', ['$filter', '$parse', 'commonService',
    function ($filter, $parse, commonService) {
        return {
            require: '^ngModel',
            link: function ($scope, element, attrs, ctrl) {
                if (!ctrl) return;

                $scope.formatInput(element, ctrl);

                $scope.$on('inputDisabledChecking', function (event) {
                    $scope.disabledInput(element);
                })

                $scope.setConstrainMaxPerMatch = function () {
                    var modelValue = isNaN(ctrl.$modelValue) == true ? $scope.betSetting.maxPerMatch : parseFloat(ctrl.$modelValue);
                    var maxValue = $scope.betSetting.upline.maxPerMatch;

                    if (modelValue > maxValue && maxValue < $scope.MAX_VALUE && !$scope.unlimited) {
                        commonService.createTooltip(element, $scope.betSetting.betTypeName + " - Max Per Match " + resources.less_than + " " + maxValue);
                        $scope.valids.maxPerMatch = false;
                        element.addClass('invalidValue');
                        return false;
                    }

                    commonService.createTooltip(element, "");
                    $scope.valids.maxPerMatch = true;
                    element.removeClass('invalidValue');
                }
            }
        };
    }
]);