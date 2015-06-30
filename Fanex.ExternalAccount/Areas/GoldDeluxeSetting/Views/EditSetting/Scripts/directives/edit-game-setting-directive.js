var builtUrlTemplate = fanex.goldDeluxeSetting.buildTemplateUrlWithVersion;

fanex.goldDeluxeSetting.directive('editGameSetting', [
        function () {
            'use strict';
            return {
                restrict: 'E',
                replace: true,
                controller: 'CustGameSettingController',
                templateUrl: builtUrlTemplate('Areas/GoldDeluxeSetting/Views/EditSetting/Scripts/templates/GameSetting/edit-game-setting.htm'),
                link: function ($scope) {
                    var uplineUncheckedAllMsg = '';

                    if (settingViewModel.Level === fanex.goldDeluxeSetting.customerLevel.Member) {
                        angular.forEach($scope.gameSetting, function (gameType) {
                            if (gameType.CheckedStakeLevelId === '') {
                                gameType.BettingLimit[0].IsChecked = true;
                                gameType.CheckedStakeLevelId = String(gameType.BettingLimit[0].Id);
                                uplineUncheckedAllMsg += gameType.Name + ', ';
                            }
                        });

                        if (uplineUncheckedAllMsg !== '') {
                            uplineUncheckedAllMsg = '{' + uplineUncheckedAllMsg.slice(0, -2) + '} ' + language.getString('WarningUplineUncheckAllStakeLevelMsg');
                            $scope.$parent.mainResult.Warning = {
                                IsShow: true,
                                Message: uplineUncheckedAllMsg
                            };
                        }
                    }

                    $scope.checkShowWarningUncheckedAll = function () {
                        var isShowWarning = false;
                        angular.forEach($scope.gameSetting, function (gameType) {
                            var isUncheckedAll = true;
                            angular.forEach(gameType.BettingLimit, function (stakeLevel) {
                                if (stakeLevel.IsChecked) {
                                    isUncheckedAll = false;
                                    return;
                                }
                            });

                            if (isUncheckedAll) {
                                isShowWarning = true;
                                return;
                            }
                        });

                        var message = language.getString('WarningUncheckAllStakeLevelMsg');
                        $scope.$parent.mainResult.Warning = {
                            IsShow: isShowWarning,
                            Message: message
                        };
                    };
                }
            };
        }
]);
