/* File Created: July 21, 2014 */
/*global settingContext,language, numeral*/
if (fanex.liveCasinoSetting) {
    fanex.liveCasinoSetting.controller('CustomerWinLimitController',
    [
        '$scope',
        '$http',
        'CustomerWinLimitService',
        function ($scope, $http, customerWinLimitService) {
            'use strict';
            var winLimit = settingContext.Customer.LiveCasinoSetting.WinLimit;
            var parentWinLimit = settingContext.Customer.Parent.LiveCasinoSetting.WinLimit;

            $scope.maxWinLimitModel = fanex.cloneObject(settingContext.Customer.LiveCasinoSetting.WinLimit);
            $scope.isShowCurrentWinLimit = false;
            $scope.isPercentShowRedColor = false;
            $scope.maxWinLimits = [{ lable: "3X", value: 3 },
                                   { lable: "5X", value: 5 },
                                   { lable: "10X", value: 10 },
                                   { lable: language.getString("lblUnlimited"), value: 0}];

            switch (parentWinLimit.Max) {
                case 3:
                    $scope.maxWinLimits.splice(1, 3);
                    break;
                case 5:
                    $scope.maxWinLimits.splice(2, 2);
                    break;
                case 10:
                    $scope.maxWinLimits.splice(3, 1);
                    break;
            }

            var index = 0;

            switch (winLimit.Max) {
                case 3:
                    index = 0;
                    break;
                case 5:
                    index = 1;
                    break;
                case 10:
                    index = 2;
                    break;
                default:
                    index = 3;
                    break;

            }

            $scope.maxWinLimitModel = {
                Max: $scope.maxWinLimits[index],
                IsAutoReset: winLimit.IsAutoReset,
                UplineAutoReset: settingContext.Customer.Level === fanex.liveCasinoSetting.customerLevel.Super ? true : parentWinLimit.IsAutoReset
            };

            $scope.maxWinLimits = $scope.maxWinLimits.reverse();

            $scope.showCurrentWinLimit = function (custId) {
                customerWinLimitService.getCurrentMemberWinLimit(custId).success(function (data) {
                    if (data.MaxWinLiveCasino === 0) {
                        $scope.currentWinLimit = -1;
                    }
                    else {
                        $scope.currentWinLimit = numeral(data.MaxWinCredit).format('0,0.00');
                        $scope.percent = numeral(data.WinPercent).format('0,0.00');
                    }

                    $scope.isShowCurrentWinLimit = true;
                    $scope.isPercentShowRedColor = parseFloat(data.WinPercent) >= 100;
                });
            };

            $scope.updateWinLimitSetting = function () {
                if (!settingContext.IsSyncLiveCasino) {
                    $scope.$parent.validateAndUpdateSetting(fanex.liveCasinoSetting.updateType.WinLimit);
                    return;
                }

                customerWinLimitService.updateWinLimitSetting($scope.getWinLimit(), $scope.$parent);
            };

            $scope.getWinLimit = function () {
                return {
                    Max: $scope.maxWinLimitModel.Max.value,
                    IsAutoReset: $scope.maxWinLimitModel.IsAutoReset
                };
            };
        }
        ]);
}
else {
    alert('Missing fanex.liveCasinoSetting!');
}