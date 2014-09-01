/* File Created: July 21, 2014 */
/*global custSetting*/
/// <reference path="../../../../Scripts/core/fanex-live-casino-setting.js" />
if (fanex.liveCasinoSetting) {
    fanex.liveCasinoSetting.controller('CustomerSettingController',
    [
        '$scope',
        '$http',
        '$window',
        '$timeout',
        'CustomerSettingService',
        function ($scope, $http, $window, $timeout, customerSettingService) {
            'use strict';
            var updateAllSettingUrl = $('#tblSetting').data('update-href');
            $scope.mainResult = {
                CommandResultsList: []
            };
            $scope.isUpdateAllSetting = false;
            $scope.isReadOnly = custSetting.SettingContext.FormType === fanex.liveCasinoSetting.formType.ReadOnly;
            if (custSetting.SettingContext.IsEditMultiple
                && custSetting.SettingContext.TargetSyncStatus
                && custSetting.SettingContext.IsEditDirectDownline) {
                custSetting.CustInfo.IsSyncLiveCasino = custSetting.SettingContext.TargetSyncStatus.indexOf('0') === -1;
            }

            $scope.validateAndUpdateSetting = function (updateType) {
                $scope.isUpdateAllSetting = true;
                var ptAndComScope = angular.element('[data-ng-controller="PtAndComSettingController"]').scope();

                if (ptAndComScope.isEditPresetOnMember()) {
                    return;
                }

                $scope.updateAllSetting(updateType);
            };

            $scope.updateAllSetting = function (updateType) {
                updateType = updateType || fanex.liveCasinoSetting.updateType.AllSetting;
                var custInfoScope = angular.element('[data-ng-controller="CustomerInfoController"]').scope(),
                    ptAndComScope = angular.element('[data-ng-controller="PtAndComSettingController"]').scope(),
                    winLimitScope = angular.element('[data-ng-controller="CustomerWinLimitController"]').scope(),
                    gameSettingScope = angular.element('[data-ng-controller="CustGameSettingController"]').scope();

                var custInfo = custInfoScope.getCustInfo();
                custInfo.MemberWinLimit = winLimitScope.getWinLimit();


                var custSettingForUpdate = {
                    settingContext: custSetting.SettingContext,
                    custInfo: custInfo,
                    ptAndComSetting: ptAndComScope.ptAndComSetting,
                    gameSetting: gameSettingScope.getGameSetting(),
                    updateType: updateType
                };

                customerSettingService.updateAllSetting(updateAllSettingUrl, custSettingForUpdate, $scope);
                $scope.isUpdateAllSetting = false;
            };

            $scope.afterServerReturnSuccess = function (mainResult, isUpdatePartial) {
                $scope.mainResult = mainResult;

                if (mainResult.IsSuccessful) {
                    $scope.isReadOnly = true;
                    $timeout(function () {
                        if (custSetting.SettingContext.IsEditMultiple
                            && $window.parent.actionAfterUpdatedLiveCasinoSettingMultiple) {
                            $window.parent.actionAfterUpdatedLiveCasinoSettingMultiple(isUpdatePartial);
                        }
                        else {
                            if (!isUpdatePartial
                                && $window.parent.actionAfterUpdatedLiveCasinoSettingSingle) {
                                $window.parent.actionAfterUpdatedLiveCasinoSettingSingle(isUpdatePartial);
                            }

                            $window.location.href = $window.location.href;
                        }
                    }, fanex.liveCasinoSetting.TIME_OUT_SHOW_MESSAGE);
                }
            };

            $('#tblSetting').show();
        }
        ]);
}
else {
    alert('Missing fanex.liveCasinoSetting!');
}