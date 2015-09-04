"use strict";

ColossusBetModule.controller('commissionController', function ($scope, colossusBetRepositories, commonService) {
    colossusBetRepositories.getData().then(
        function () {
            $scope.$watch('comissionPartial.CommisionValue', function (newValue, oldValue) {
                var element = $("#commissionInputId");
                if (newValue !== oldValue) {
                    if ($scope.comissionPartial.CommisionValue > $scope.comissionPartial.MaxCommisionValue) {
                        commonService.createTooltip(element, "Input must be less than " + $scope.comissionPartial.MaxCommisionValue);
                        element.addClass('invalidValue');
                        $scope.comissionPartial.IsCommisionUpdated = false;
                        return;
                    }

                    element.removeClass('invalidValue');
                    $scope.comissionPartial.IsCommisionUpdated = true;
                }
            }, true);

        }, function (reason) { }, function (update) { });
});

