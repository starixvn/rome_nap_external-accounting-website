/* File Created: July 21, 2014 */
/*global settingContext*/
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
            var updateAllSettingUrl = $('#UpdateLiveCasinoSettingUrl').val();
            $scope.mainResult = {
                CommandResultsList: []
            };

            settingContext.IsSyncLiveCasino = settingContext.Customer.IsSyncLiveCasino;
            $scope.isUpdateAllSetting = false;
            $scope.isReadOnly = settingContext.FormType === fanex.liveCasinoSetting.formType.ReadOnly;
            $scope.isEditMultiple = settingContext.Customers && settingContext.Customers.length > 0;
            $scope.isEditDirect = (settingContext.Upline.Level - settingContext.Customer.Level === 1);
            $scope.isUpdateWinLimitPartial = !((settingContext.Upline.Level === fanex.liveCasinoSetting.customerLevel.Super || settingContext.Upline.Level === fanex.liveCasinoSetting.customerLevel.Master) && settingContext.Customer.Level === fanex.liveCasinoSetting.customerLevel.Member);
            $scope.isUpdateGameSettingPartial = $scope.isUpdateWinLimitPartial;

            if ($scope.isEditMultiple
                && settingContext.Customer.IsSyncLiveCasino
                && $scope.isEditDirect) {
                for (var i = 0; i < settingContext.Customers.length; i++) {
                    if (!settingContext.Customers[i].IsSyncLiveCasino) {
                        settingContext.IsSyncLiveCasino = false;
                        break;
                    }
                }
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

                var customer = custInfoScope.getCustInfo();
                customer.LiveCasinoSetting.WinLimit = winLimitScope.getWinLimit();
                customer.LiveCasinoSetting.PositionTaking = ptAndComScope.positionTaking;
                customer.LiveCasinoSetting.Commission = ptAndComScope.commission;
                customer.LiveCasinoSetting.GameSetting = gameSettingScope.getGameSetting();

                var settingContextForUpdate = {
                    settingContext: settingContext,
                    customer: customer,
                    isApplyPositionTakingAllMembers: ptAndComScope.uplinePTOnMember.isApplyNewUplinePositionTaking,
                    positionTakingOnMembers: ptAndComScope.uplinePTOnMember.newUplinePositionTaking,
                    updateType: updateType
                };

                customerSettingService.updateAllSetting(updateAllSettingUrl, settingContextForUpdate, $scope);
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