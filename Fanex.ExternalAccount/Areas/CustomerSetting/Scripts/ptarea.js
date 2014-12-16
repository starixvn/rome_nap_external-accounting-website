var module = angular.module("settingApp", []);

angular.element(document.getElementById('ng-app')).ready(function () {
    $('#ng-app').show();
});

toPTSource = function (obj, resetSelected, isComm) {
    var index = -1;
    var itemIndex = 0;
    var min = obj.min;
    var max = obj.max;
    var step = isComm ? 0.0005 : 0.01;
    var selectedValue = obj.selected;
    var breakpt = obj.breakpt;
    min = min < 0 ? 0 : min;

    if (breakpt) {
        return;
    }

    if (resetSelected === true) {
        obj.selected = max;
    }

    var pts = [];

    if (min > max) {
        min = max < 0 ? 0 : max;
    }

    if (selectedValue > max) { // Insert selected value at top
        pts.push(selectedValue);
    }
    var c = Math.round((max * 10000 - min * 10000) / (step * 10000));
    if (c <= 0 && selectedValue != 0) { // Not range
        pts.push(Math.round(min * 10000) / 10000);
    } else {
        for (var i = c; i >= 0; i--) {
            var f = Math.round(min * 10000 + (i * step * 10000)) / 10000;
            if (parseFloat(selectedValue) == parseFloat(f)) {
                index = itemIndex; // Store selected item
            }
            pts.push(f);
            itemIndex++;
        }
    }
    if (selectedValue < min) { // Insert selected value at bottom
        pts.push(selectedValue);
        index = pts.length - 1;
    }

    obj.pts = pts;

    return obj;
};

Number.prototype.ptRound = function () {
    return Math.round(this * 10000) / 10000;
};

String.prototype.ptRound = function () {
    return Math.round(this * 10000) / 10000;
};

module.directive("ptarea", function ($compile, $timeout, $rootScope) {
    return {
        require: "ngModel",
        link: function (scope, elm, attrs, ctrl) {

            var isComm = attrs.iscomm == 'true';
            var source = "$" + attrs.selected;
            scope[attrs.ngModel][source] = {};

            function getValue(name) {
                return isNaN(parseFloat(name)) ? scope[attrs.ngModel][name] : parseFloat(name);
            }

            function basicRender() {
                var min = getValue(attrs.selected);
                var max = min;
                var selected = min;
                // $ + 'selected' attribute
                var obj = scope[attrs.ngModel][source];
                obj.min = min;
                obj.max = max;
                obj.selected = selected;

                toPTSource(obj, false, isComm);
            }

            function render() {
                var obj = scope[attrs.ngModel][source];

                if (obj.pts.length > 1) return;

                var min = getValue(attrs.min).ptRound();
                var max = 0;

                if (source == "$m3") {
                    max = source == "$m3" && scope[attrs.ngModel].is3 ?
                        scope[attrs.ngModel].s4 : (source == "$m3" && typeof scope[attrs.ngModel]["n3"] != 'undefined' && typeof scope[attrs.ngModel].is3 != 'undefined' ? scope[attrs.ngModel].s4 - scope[attrs.ngModel].s3 : getValue(attrs.max));
                }
                else {
                    max = source == "$a2" && scope[attrs.ngModel].im2 ?
                        scope[attrs.ngModel].m3 : (source == "$a2" && typeof scope[attrs.ngModel]["n2"] != 'undefined' && typeof scope[attrs.ngModel].im2 != 'undefined' ? scope[attrs.ngModel].m3 - scope[attrs.ngModel].m2 : getValue(attrs.max));
                }
                var selected = getValue(attrs.selected).ptRound();
                max = max.ptRound();

                // $ + 'selected' attribute
                var warned = obj.warning;
                var curWarn = selected > max || selected < min;

                toPTSource(scope[attrs.ngModel][source] = {
                    name: attrs.selected,
                    min: min,
                    max: max,
                    selected: selected,
                    warning: warned || curWarn
                }, false, isComm);
            }

            var template = "<select ng-blur=\"" + source + "blur($event)\" ng-touchstart=\"" + source + "focus($event)\" ng-mouseenter=\"" + source + "focus($event)\" ng-change=\"" + source + "change($event, setting." + source + ")\" class='input-small' ng-disabled='" + attrs.enable + "' ng-model='setting." + source + ".selected' ng-options='pt for pt in setting." + source + ".pts'>" +
                            "</select>";

            if (attrs.islabel == "true") {
                var model = "setting." + source + ".selected";
                template = "<span style='font-weight: normal'>{{" + model + "}}</span>";
            }
            else {
                if (attrs.showbutton != "false") {
                    template +=
                            "<input class='btnPTDown' ng-touchstart=\"" + source + "down($event, setting." + source + ", 1, " + isComm + ")\" ng-click=\"" + source + "down($event, setting." + source + ", 1, " + isComm + ")\" class='btn' ng-disabled='" + attrs.enable + "' />" +
                            "<input class='btnPTUp' ng-touchstart=\"" + source + "up($event, setting." + source + ", 1, " + isComm + ")\" ng-click=\"" + source + "up($event, setting." + source + ", 1, " + isComm + ")\" class='btn' ng-disabled='" + attrs.enable + "' />" +
                            "<input class='btnPTSub' ng-touchstart=\"" + source + "down($event, setting." + source + ", 10, " + isComm + ")\" ng-click=\"" + source + "down($event, setting." + source + ", 10, " + isComm + ")\"  class='btn' ng-disabled='" + attrs.enable + "' />" +
                            "<input class='btnPTAdd' ng-touchstart=\"" + source + "up($event, setting." + source + ", 10, " + isComm + ")\" ng-click=\"" + source + "up($event, setting." + source + ", 10, " + isComm + ")\"  class='btn' ng-disabled='" + attrs.enable + "' />";
                }
                else {
                    template +=
                            "<span ng-init=\"setting." + source + ".warning=false\" ng-model=\"setting." + source + ".warning\" ng-show=\"setting." + source + ".warning\" style=\"color: Red\">*</span>";
                }
            }

            scope[source + "change"] = function (event, source) {
                scope[attrs.ngModel][attrs.selected] = source.selected.ptRound();
                scope[attrs.ngModel][attrs.warning] = source.selected < source.min || source.selected > source.max;
            }

            scope[source + "up"] = function (event, source, step, isComm) {
                if ((event.target || event.srcElement).disabled) return;

                if (attrs.max == "l4" && scope[attrs.ngModel]["is3"]) {
                    attrs.l4 = scope[attrs.ngModel]["l4"] = scope[attrs.ngModel]["s4"].ptRound();
                }

                var newValue = (source.selected + (isComm ? 5 * step : step) / (isComm ? 10000 : 100)).ptRound();
                var max = getValue(attrs.max).ptRound();
                var min = getValue(attrs.min).ptRound();

                if (newValue < 0 || newValue > max || newValue < min || attrs.breakpt) {
                    return;
                }

                newValue = newValue < min ? min : newValue > max ? max : newValue;
                scope[attrs.ngModel][attrs.selected] = source.selected = newValue;
            }

            scope[source + "down"] = function (event, source, step, isComm) {
                if ((event.target || event.srcElement).disabled) return;

                if (attrs.max == "l4" && scope[attrs.ngModel]["is3"]) {
                    attrs.l4 = scope[attrs.ngModel]["l4"] = scope[attrs.ngModel]["s4"].ptRound();
                }

                var newValue = (source.selected - (isComm ? 5 * step : step) / (isComm ? 10000 : 100)).ptRound();

                var max = getValue(attrs.max).ptRound();
                var min = getValue(attrs.min).ptRound();

                if (newValue < 0 || newValue > max || newValue < min || attrs.breakpt) {
                    return;
                }

                newValue = newValue < min ? min : newValue > max ? max : newValue;
                scope[attrs.ngModel][attrs.selected] = source.selected = newValue;
            }

            scope[source + "focus"] = function (event) {
                render();
            }

            scope[source + "blur"] = function (event) {
                basicRender();
            }

            elm.html(template);
            $compile(elm.contents())(scope);

            basicRender();

            if (typeof scope.setupWatch === "function" && $rootScope[scope.$id] == undefined) {
                scope.setupWatch();
                $rootScope[scope.$id] = true;
            }
        }
    }
});

module.directive('ngFocus', ['$parse', function ($parse) {
    return function (scope, element, attr) {
        var fn = $parse(attr['ngFocus']);
        element[0].addEventListener = element[0].addEventListener || element[0].attachEvent;
        element[0].addEventListener('focus', function (event) {
            scope.$apply(function () {
                fn(scope, { $event: event });
            });
        });
    };
} ]);

module.directive('ngTouchStart', ['$parse', function ($parse) {
    return function (scope, element, attr) {
        var fn = $parse(attr['ngTouchStart']);
        element[0].addEventListener = element[0].addEventListener || element[0].attachEvent;
        element[0].addEventListener('touchstart', function (event) {
            scope.$apply(function () {
                fn(scope, { $event: event });
            });
        });
    };
} ]);

module.directive('ngBlur', ['$parse', function ($parse) {
    return function (scope, element, attr) {
        var fn = $parse(attr['ngBlur']);
        element[0].addEventListener = element[0].addEventListener || element[0].attachEvent;
        element[0].addEventListener('blur', function (event) {
            scope.$apply(function () {
                fn(scope, { $event: event });
            });
        });
    };
} ]);

module.directive('ngFinishRender', function ($timeout) {
    return {
        restrict: 'A',
        link: function (scope, element, attr) {
            if (scope.$last === true) {
                $timeout(function () {
                    scope.$emit(attr.ngFinishRender);
                });
            }
        }
    }
});