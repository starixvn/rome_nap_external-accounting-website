/* File Created: July 21, 2014 */
/*global custSetting*/
fanex.liveCasinoSetting.controller('CustomerInfoController',
    [
        '$scope',
        function ($scope) {
            'use strict';
            $scope.custInfo = fanex.cloneObject(custSetting.CustInfo);
            $scope.enableLiveCasino = !custSetting.CustInfo.DisableLiveCasino;
            $scope.disableCheckBoxLiveCasino = custSetting.CustInfo.UplineDisableLiveCasino;

            //// Functions
            $scope.getCustInfo = function () {
                $scope.custInfo.DisableLiveCasino = !$scope.enableLiveCasino;
                return $scope.custInfo;
            };
        }
        ]);
