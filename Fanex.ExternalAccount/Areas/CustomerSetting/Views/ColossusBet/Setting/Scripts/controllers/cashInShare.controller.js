"use strict";

ColossusBetModule.controller('cashInShareController', function ($scope, colossusBetRepositories, commonService) {
    colossusBetRepositories.getData().then(
        function () {
            $scope.$watch('cashInSharePartial.CashInShareValue', function (newValue, oldValue) {
                var element = $("#cashInShareInputId");
                if (newValue !== oldValue) {
                    if ($scope.cashInSharePartial.CashInShareValue > $scope.cashInSharePartial.MaxCashInShareValue) {
                        commonService.createTooltip(element, "Input must be less than " + $scope.cashInSharePartial.MaxCashInShareValue);
                        element.addClass('invalidValue');
                        $scope.cashInSharePartial.IsCashInShareUpdated = false;
                        return;
                    }

                    element.removeClass('invalidValue');
                    $scope.cashInSharePartial.IsCashInShareUpdated = true;
                }
            }, true);

        }, function (reason) { }, function (update) { });
});