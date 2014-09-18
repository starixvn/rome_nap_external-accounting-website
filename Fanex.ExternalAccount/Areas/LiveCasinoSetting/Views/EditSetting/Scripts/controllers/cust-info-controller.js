/* File Created: July 21, 2014 */
/*global settingContext*/
fanex.liveCasinoSetting.controller('CustomerInfoController',
    [
        '$scope',
        function ($scope) {
            'use strict';
            $scope.enableLiveCasino = !settingContext.Customer.DisableLiveCasino;
            $scope.disableCheckBoxLiveCasino = settingContext.Customer.Parent.DisableLiveCasino;

            //// Functions
            $scope.getCustInfo = function () {
                settingContext.Customer.DisableLiveCasino = !$scope.enableLiveCasino;
                return settingContext.Customer;
            };
        }
        ]);
