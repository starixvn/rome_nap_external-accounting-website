"use strict";

ColossusBetModule.controller('commissionController', function ($scope, colossusBetRepositories, commonService) {
    colossusBetRepositories.getData().then(
        function () {
            $scope.originalComissionPartial = angular.copy($scope.comissionPartial);
            $scope.$watch('comissionPartial.CommisionValue', function (newValue, oldValue) {
                var element = $("#commissionInputId");
                if ((newValue !== oldValue) && !$scope.multipleEdit.isMultipleEdit) {
                    if ($scope.comissionPartial.CommisionValue > $scope.comissionPartial.MaxCommisionValue) {
                        commonService.createTooltip(element, resources.less_than + $scope.comissionPartial.MaxCommisionValue);
                        element.addClass('invalidValue');
                        $scope.comissionPartial.IsCommisionUpdated = false;
                        return;
                    }
                    else {
                        element.removeClass('invalidValue');
                    }
                    if ($scope.multipleEdit.isMultipleEdit) {
                        $scope.comissionPartial.IsCommisionUpdated = true;
                    }
                    else {
                        $scope.comissionPartial.IsCommisionUpdated = $scope.originalComissionPartial.CommisionValue != $scope.comissionPartial.CommisionValue ? true : false;
                    }
                }
            }, true);

        }, function (reason) { }, function (update) { });
});

