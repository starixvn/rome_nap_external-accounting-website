/*global settingContext, language*/
fanex.liveCasinoSetting.controller('PtAndComSettingController',
    [
    '$scope',
    '$window',
    'PtAndComSettingService',
    function ($scope, $window, ptAndComSettingService) {
        'use strict';
        //// Privates
        var me = this;
        me.$presetOnMemberModal = $('#presetOnMemberModal');
        $scope.uplinePTOnMember =
        {
            newUplinePositionTaking: 0,
            isApplyNewUplinePositionTaking: false
        };

        $scope.isEditPresetOnMember = function () {
            var currentPt = $scope.positionTaking;
            switch (settingContext.Customer.Level) {
                case fanex.liveCasinoSetting.customerLevel.Master:
                    if (settingContext.Customer.LiveCasinoSetting.PositionTaking.Is3
                        && !currentPt.Is3) {
                        $scope.uplinePTOnMember.newUplinePositionTaking = fanex.roundNumber(currentPt.S4 - currentPt.M3);
                        $scope.uplinePTOnMemberOptions = fanex.createOptionsFromRange(
                                                            $scope.uplinePTOnMember.newUplinePositionTaking,
                                                            0,
                                                            $scope.uplinePTOnMember.newUplinePositionTaking,
                                                            fanex.liveCasinoSetting.PT_INTERVAL);
                        me.$presetOnMemberModal.modal({
                            backdrop: 'static',
                            show: true,
                            keyboard: false
                        });
                        return true;
                    }
                    break;
                case fanex.liveCasinoSetting.customerLevel.Agent:
                    if (settingContext.Customer.LiveCasinoSetting.PositionTaking.Im2
                        && !currentPt.Im2) {
                        $scope.uplinePTOnMember.newUplinePositionTaking = fanex.roundNumber(currentPt.M3 - currentPt.A2);
                        $scope.uplinePTOnMemberOptions = fanex.createOptionsFromRange(
                                                            $scope.uplinePTOnMember.newUplinePositionTaking,
                                                            0,
                                                            $scope.uplinePTOnMember.newUplinePositionTaking,
                                                            fanex.liveCasinoSetting.PT_INTERVAL);
                        me.$presetOnMemberModal.modal({
                            backdrop: 'static',
                            show: true,
                            keyboard: false
                        });
                        return true;
                    }
                    break;
            }
            return false;
        };

        //// Properties
        $scope.positionTaking = fanex.cloneObject(settingContext.Customer.LiveCasinoSetting.PositionTaking);
        $scope.commission = settingContext.Customer.LiveCasinoSetting.Commission;
        $scope.parentCommission = settingContext.Customer.Parent.LiveCasinoSetting.Commission;
        $scope.uplinePTOnMemberOptions = [];
        $scope.isUpdateAllSetting = false;
        $scope.language = language;
        me.updatePTComUrl = $('#UpdatePTComSettingUrl').val();

        //// Action
        $scope.updatePtAndComSetting = function () {
            if (!settingContext.IsSyncLiveCasino) {
                $scope.$parent.validateAndUpdateSetting(fanex.liveCasinoSetting.updateType.PositionTakingAndCommission);
                return;
            }

            if ($scope.isEditPresetOnMember()) {
                return;
            }

            ptAndComSettingService.updatePtAndComSetting(me.updatePTComUrl, $scope.commission, $scope.positionTaking, $scope.uplinePTOnMember, $scope.$parent);
        };

        $scope.updatePTOnMember = function (isUpdate) {
            $scope.uplinePTOnMember.isApplyNewUplinePositionTaking = isUpdate;
            if ($scope.$parent.isUpdateAllSetting) {
                $scope.$parent.updateAllSetting();
            }
            else {
                ptAndComSettingService.updatePtAndComSetting(
                            me.updatePTComUrl,
                            $scope.commission,
                            $scope.positionTaking,
                            $scope.uplinePTOnMember,
                            $scope.$parent);
            }
        };
    }
]);