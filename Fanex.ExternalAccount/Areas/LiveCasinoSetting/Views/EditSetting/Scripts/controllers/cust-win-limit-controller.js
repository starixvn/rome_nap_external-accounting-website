/* File Created: July 21, 2014 */
/*global custSetting,language, numeral*/
if (fanex.liveCasinoSetting) {
    fanex.liveCasinoSetting.controller('CustomerWinLimitController',
    [
        '$scope',
        '$http',
        'CustomerWinLimitService',
        function ($scope, $http, customerWinLimitService) {
            'use strict';
            $scope.maxWinLimitModel = fanex.cloneObject(custSetting.CustInfo.MemberWinLimit);
            var memberWinLimit = custSetting.CustInfo.MemberWinLimit;
            $scope.isShowCurrentWinLimit = false;
            $scope.maxWinLimits = [{ lable: "3X", value: 3 },
                                   { lable: "5X", value: 5 },
                                   { lable: "10X", value: 10 },
                                   { lable: language.getString("lblUnlimited"), value: 0}];

            switch (memberWinLimit.Upline.Max) {
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

            switch (memberWinLimit.Max) {
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
                IsAutoReset: memberWinLimit.IsAutoReset,
                UplineAutoReset: custSetting.SettingContext.TargetRoleId === fanex.liveCasinoSetting.custRole.Super ? true : memberWinLimit.Upline.IsAutoReset
            };

            $scope.maxWinLimits = $scope.maxWinLimits.reverse();

            $scope.showCurrentWinLimit = function (custId) {
                customerWinLimitService.getCurrentMemberWinLimit(custId).success(function (data) {
                    if (data.MaxWinLiveCasino === 0) {
                        $scope.winLimit = -1;
                    }
                    else {
                        $scope.winLimit = numeral(data.MaxWinCredit).format('0,0');
                        $scope.percent = data.WinPercent;
                    }

                    $scope.isShowCurrentWinLimit = true;
                });
            };

            $scope.updateWinLimitSetting = function () {
                if (!custSetting.CustInfo.IsSyncLiveCasino) {
                    $scope.$parent.validateAndUpdateSetting(fanex.liveCasinoSetting.updateType.WinLimit);
                    return;
                }

                customerWinLimitService.updateWinLimitSetting($scope.maxWinLimitModel, $scope.$parent);
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