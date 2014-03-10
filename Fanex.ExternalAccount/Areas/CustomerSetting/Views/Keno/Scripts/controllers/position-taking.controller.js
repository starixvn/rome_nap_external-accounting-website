if (fanex.customerSetting) {
    fanex.customerSetting.controller('fanex.customerSetting.positionTakingController',
    [
        '$scope',
        '$rootScope',
        '$filter',
        function ($scope, $rootScope, $filter) {
            $scope.model = $rootScope.model.positiongTakingSections;
            $scope.positionTakingService = fanex.customerSetting.positionTakingService;
            $scope.commissionSettingService = fanex.customerSetting.commissionSettingService;

            $scope.getChangedPositionTakings = function () {
                return $scope.positionTakingService.getChangedData($scope.model);
            }

            $scope.isValidPositionTakings = function () {
                return $scope.positionTakingService.isValidData($scope.model);
            };

            $scope.getChangedCommissionSettings = function () {
                return $scope.commissionSettingService.getChangedData($scope.model);
            }

            $scope.isValidCommissionSettings = function () {
                return $scope.commissionSettingService.isValidData($scope.model);
            };

            return {
                getScope: function () {
                    return $scope;
                }
            }
        } ]);
}