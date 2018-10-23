if (fanex.goldDeluxeSetting) {
    fanex.goldDeluxeSetting.controller('CustomerWinLimitController',
    [
        '$scope',
        '$http',
        'CustomerWinLimitService',
        function ($scope, $http, customerWinLimitService) {
            'use strict';
            var winLimit = settingViewModel.Setting.WinLimit;

            $scope.maxWinLimitModel = fanex.cloneObject(settingViewModel.Setting.WinLimit);
            $scope.isShowCurrentWinLimit = false;
            $scope.isPercentShowRedColor = false;
            $scope.maxWinLimits = [{ lable: "3X", value: 3 },
                                   { lable: "5X", value: 5 },
                                   { lable: "10X", value: 10 },
                                   { lable: language.getString("lblUnlimited"), value: 0 }];

            switch (winLimit.UplineMax) {
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
                UplineAutoReset: settingViewModel.Level === fanex.goldDeluxeSetting.customerLevel.Super ? true : winLimit.UplineIsAutoReset
            };

            $scope.maxWinLimits = $scope.maxWinLimits.reverse();

            $scope.showCurrentWinLimit = function (custId) {
                customerWinLimitService.getCurrentMemberWinLimit(custId).success(function (data) {
                    $scope.currentWinLimit = numeral(data.MaxWinCredit).format('0,0.00');

                    if (data.MaxWinLimit === 0) {
                        $scope.currentWinLimit = -1;
                    }
                    else if (data.WinPercent == 0) {
                        $scope.percent = 0;
                    }
                    else {
                        $scope.percent = numeral(data.WinPercent).format('0,0.00');
                    }

                    $scope.isShowCurrentWinLimit = true;
                    $scope.isPercentShowRedColor = parseFloat(data.WinPercent) >= 100 || parseInt(data.WinPercent) == -2;

                    $scope.CurrentAmount = numeral(data.CurrentAmount).format('0,0.00');
                    $scope.MaxWinCredit = numeral(data.MaxWinCredit).format('0,0.00');
                    $scope.Current = data.CurrentAmount;
                    $scope.Max = data.MaxWinCredit;

                    if (parseInt($scope.percent) == -2) {
                        $scope.percent = "N/A";
                    }
                });
            };

            $scope.updateWinLimitSetting = function () {
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
    alert('Missing fanex.goldDeluxeSetting!');
}