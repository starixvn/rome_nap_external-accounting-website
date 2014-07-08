"use strict";

KenoModule.directive('checkDisableElement', function () {
    return {
        require: 'ngModel',
        restrict: 'A',
        link: function (scope, element, attrs, ctrl) {
            scope.$watch(attrs.ngModel, function () {
                var className = attrs.ngClassName;
                scope.$eval(attrs.ngModel) == false ? element.attr("disabled", "disabled").addClass(className + "Disabled").removeClass(className) : element.removeAttr("disabled", "disabled").addClass(className).removeClass(className + "Disabled");
            }, true);
        }
    }
});

KenoModule.directive('ieSelectFix', function ($timeout) {
    return {
        require: 'ngModel',
        restrict: 'A',
        link: function (scope, element, attrs, ctrl) {
            attrs.$observe(
                "ieSelectFix",
                function (value) {
                    var isIE = document.attachEvent;
                    if (isIE) {
                        var control = element[0];
                        var option = document.createElement("option");
                        control.add(option, null);
                        control.remove(control.options.length - 1);

                        var tmp = angular.element('<option></option>');
                        element.append(tmp);
                        tmp.remove();
                    }
                });
        }
    }
});

KenoModule.directive('betSettingConstrain', ['$filter',
    function ($filter) {
        return {
            require: '?ngModel',
            link: function ($scope, element, attrs, ctrl) {
                if (!ctrl) return;

                var valueModel = eval("$scope." + attrs.ngModel);
                var MAX_VALUE = Math.pow(2, 53);
                if (valueModel > 0 && valueModel < MAX_VALUE) {
                    var symbol = attrs.betSettingConstrain == 1 ? " >= " : " <= ";

                    element.text(symbol + $scope.formatNumber(valueModel));
                }
            }
        };
    }
]);