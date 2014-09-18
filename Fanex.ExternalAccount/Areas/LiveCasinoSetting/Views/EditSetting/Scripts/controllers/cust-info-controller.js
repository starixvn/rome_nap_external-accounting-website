/* File Created: July 21, 2014 */
/*global settingContext*/
fanex.liveCasinoSetting.controller('CustomerInfoController',
    [
        '$scope',
        function ($scope) {
            'use strict';
            $scope.enableLiveCasino = !settingContext.Customer.DisableLiveCasino;
            $scope.disableCheckBoxLiveCasino = settingContext.Customer.UplineDisableLiveCasino;

            //// Functions
            $scope.getCustInfo = function () {
                settingContext.Customer.DisableLiveCasino = !$scope.enableLiveCasino;
                return settingContext.Customer;
            };
        }
        ]);
