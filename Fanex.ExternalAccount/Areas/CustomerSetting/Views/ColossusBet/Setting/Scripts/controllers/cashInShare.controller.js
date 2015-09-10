"use strict";

ColossusBetModule.controller('cashInShareController', function ($scope, colossusBetRepositories, commonService) {
    colossusBetRepositories.getData().then(
        function () {
            $scope.originalCashInSharePartial = angular.copy($scope.cashInSharePartial);
            $scope.$watch('cashInSharePartial.CashInShareValue', function (newValue, oldValue) {
                var element = $("#cashInShareInputId");
                if (newValue !== oldValue) {
                    if ($scope.cashInSharePartial.CashInShareValue > $scope.cashInSharePartial.MaxCashInShareValue) {
                        commonService.createTooltip(element, resources.less_than + $scope.cashInSharePartial.MaxCashInShareValue);
                        element.addClass('invalidValue');
                        $scope.cashInSharePartial.IsCashInShareUpdated = false;
                        return;
                    }
                    else {
                        element.removeClass('invalidValue');
                    }


                    $scope.cashInSharePartial.IsCashInShareUpdated = $scope.originalCashInSharePartial.CashInShareValue != $scope.cashInSharePartial.CashInShareValue ? true : false;
                }
            }, true);

        }, function (reason) { }, function (update) { });
});