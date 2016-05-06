if (fanex.goldDeluxeSetting) {
    fanex.goldDeluxeSetting.controller('CustomerSettingController',
    [
        '$scope',
        '$http',
        '$window',
        '$timeout',
        'CustomerSettingService',
        function ($scope, $http, $window, $timeout, customerSettingService) {
            'use strict';
            var updateAllSettingUrl = $('#UpdateGoldDeluxeSettingUrl').val();
            $scope.mainResult = {
                CommandResultsList: []
            };

            $scope.isUpdateAllSetting = false;
            $scope.isReadOnly = settingViewModel.FormType === fanex.goldDeluxeSetting.formType.ReadOnly;
            $scope.isEditMultiple = settingViewModel.CustIds !== undefined && settingViewModel.Usernames !== undefined;
            $scope.isEditDirect = (settingViewModel.UpLineLevel - settingViewModel.Level === 1);
            $scope.isEditDirect = (settingViewModel.UpLineLevel - settingViewModel.Level === 1);
            $scope.isUpdateWinLimitPartial = !((settingViewModel.UpLineLevel === fanex.goldDeluxeSetting.customerLevel.Super || settingViewModel.UpLineLevel === fanex.goldDeluxeSetting.customerLevel.Master) && settingViewModel.Level === fanex.goldDeluxeSetting.customerLevel.Member);
            $scope.isUpdateGameSettingPartial = $scope.isUpdateWinLimitPartial;

            $scope.validateAndUpdateSetting = function (updateType) {
                $scope.isUpdateAllSetting = true;
                var ptAndComScope = angular.element('[data-ng-controller="PtAndComSettingController"]').scope();

                if (ptAndComScope.isEditPresetOnMember()) {
                    return;
                }

                $scope.updateAllSetting(updateType);
            };

            $scope.updateAllSetting = function (updateType) {
                updateType = updateType || fanex.goldDeluxeSetting.updateType.AllSetting;
                var custInfoScope = angular.element('[data-ng-controller="CustomerInfoController"]').scope(),
                    ptAndComScope = angular.element('[data-ng-controller="PtAndComSettingController"]').scope(),
                    winLimitScope = angular.element('[data-ng-controller="CustomerWinLimitController"]').scope(),
                    gameSettingScope = angular.element('[data-ng-controller="CustGameSettingController"]').scope();

                custInfoScope.updateSettingStatus(settingViewModel.Setting);

                settingViewModel.Setting.PositionTaking = ptAndComScope.positionTaking;
                settingViewModel.Setting.Commission = { Commission: ptAndComScope.commission };
                settingViewModel.Setting.WinLimit = winLimitScope.getWinLimit();
                settingViewModel.Setting.GameSetting = gameSettingScope.getGameSetting();
                settingViewModel.IsUpdatePTAllMembers = ptAndComScope.uplinePTOnMember.isApplyNewUplinePositionTaking;
                settingViewModel.PTOnMember = ptAndComScope.uplinePTOnMember.newUplinePositionTaking;

                customerSettingService.updateAllSetting(updateAllSettingUrl, settingViewModel, $scope);
                $scope.isUpdateAllSetting = false;
            };

            $scope.afterServerReturnSuccess = function (mainResult, isUpdatePartial) {
                $scope.mainResult = mainResult;

                if (mainResult.IsSuccessful) {
                    $scope.isReadOnly = true;
                    $timeout(function () {
                        if ($scope.isEditMultiple
                            && $window.parent.actionAfterUpdatedLiveCasinoSettingMultiple) {
                            $window.parent.actionAfterUpdatedLiveCasinoSettingMultiple(isUpdatePartial);
                        }
                        else {
                            if ($window.parent.actionAfterUpdatedLiveCasinoSettingSingle) {
                                $window.parent.actionAfterUpdatedLiveCasinoSettingSingle(isUpdatePartial);
                            }
                            else {
                                $window.location.href = $window.location.href;
                            }
                        }
                    }, fanex.goldDeluxeSetting.TIME_OUT_SHOW_MESSAGE);
                }
            };

            $('#tblSetting').show();
        }
    ]);
}
else {
    alert('Missing fanex.goldDeluxeSetting!');
}