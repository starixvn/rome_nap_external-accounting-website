"use strict";

KenoModule.directive('checkDisableElement', function () {
    return {
        require: 'ngModel',
        link: function (scope, element, attrs, ctrl) {
            scope.$watch(attrs.ngModel, function () {
                var className = attrs.ngClassName;
                scope.$eval(attrs.ngModel) == false ? element.attr("disabled", "disabled").addClass(className + "Disabled").removeClass(className) : element.removeAttr("disabled", "disabled").addClass(className).removeClass(className + "Disabled");
            }, true);
        }
    }
});

KenoModule.directive('betSettingElement', function () {
    return {
        restrict: 'A',
        scope: {
            betSetting: "=ngModel",
            index: "=index"
        },
        templateUrl: '/Areas/CustomerSetting/Views/Keno/Setting/Partial/BetSetting/_BetSetting.htm',
        link: function ($scope, elem, attr, ctrl) {
//            console.log($scope);

        }
    };
});



KenoModule.controller('betSettingRowController', function ($scope, kenoRepositories, betSettingService, commonService) {
    console.log($scope.betSetting);
    $scope.originalBetSetting = angular.copy($scope.betSetting);

//     $scope.$watch('positionTaking.s4', function () {
//        
//     }, true);
});



KenoModule.directive('format', ['$filter', function ($filter) {
    return {
        require: '?ngModel',

        link: function (scope, elem, attrs, ctrl) {
            if (!ctrl) return;

            var maxValue = eval(attrs.maxValue);
            var minValue = eval(attrs.minValue);


            ctrl.$formatters.unshift(function (a) {
                return $filter(attrs.format)(ctrl.$modelValue)
            });

//            scope.$watch('ctrl.$modelValue', function () {
//                console.log(ctrl.$modelValue);
//            }, true);

            ctrl.$parsers.unshift(function (viewValue) {
//                if (ctrl.$modelValue > maxValue) {
//                    alert("max invalid");
//                    return ctrl.$modelValue;
//                }

//                if (ctrl.$modelValue < minValue) {
//                    alert("min invalid");
//                    return ctrl.$modelValue;
//                }

                var plainNumber = viewValue.replace(/[^\d|\-+|\.+]/g, '');
                elem.val($filter('number')(plainNumber));
                return plainNumber;
            });
        }
    };
} ]);