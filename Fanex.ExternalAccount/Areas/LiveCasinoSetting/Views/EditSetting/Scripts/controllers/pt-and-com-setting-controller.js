/*global custSetting, language*/
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

        $scope.isEditPresetOnMember = function () {
            var currentPt = $scope.ptAndComSetting.PositionTaking;
            switch (custSetting.SettingContext.TargetRoleId) {
                case fanex.liveCasinoSetting.custRole.Master:
                    if (custSetting.PtAndComSetting.PositionTaking.Is3
                        && !currentPt.Is3) {
                        currentPt.NewPresetOnMember = fanex.roundNumber(currentPt.S4 - currentPt.M3);
                        $scope.presetOnMemberOptions = fanex.createOptionsFromRange(
                                                            currentPt.NewPresetOnMember,
                                                            0,
                                                            currentPt.NewPresetOnMember, 
                                                            fanex.liveCasinoSetting.PT_INTERVAL);
                        me.$presetOnMemberModal.modal({
                            backdrop: 'static',
                            show: true,
                            keyboard: false
                        });
                        return true;
                    }
                    break;
                case fanex.liveCasinoSetting.custRole.Agent:
                    if (custSetting.PtAndComSetting.PositionTaking.Im2
                        && !currentPt.Im2) {
                        currentPt.NewPresetOnMember = fanex.roundNumber(currentPt.M3 - currentPt.A2);
                        $scope.presetOnMemberOptions = fanex.createOptionsFromRange(
                                                            currentPt.NewPresetOnMember,
                                                            0,
                                                            currentPt.NewPresetOnMember, 
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
        $scope.ptAndComSetting = fanex.cloneObject(custSetting.PtAndComSetting);
        $scope.presetOnMemberOptions = [];
        $scope.isUpdateAllSetting = false;
        $scope.language = language;
        me.updatePtAndCommUrl = $('#tblPtComSetting').data('update-href');

        //// Action
        $scope.updatePtAndComSetting = function () {
            if (!custSetting.CustInfo.IsSyncLiveCasino) {
                $scope.$parent.validateAndUpdateSetting(fanex.liveCasinoSetting.updateType.PositionTakingAndCommission);
                return;
            }

            if ($scope.isEditPresetOnMember()) {
                return;
            }

            ptAndComSettingService.updatePtAndComSetting(me.updatePtAndCommUrl, $scope.ptAndComSetting, $scope.$parent);
        };

        $scope.updatePresetOnMember = function (isUpdate) {
            $scope.ptAndComSetting.PositionTaking.UpdateAllMembers = isUpdate;
            if ($scope.$parent.isUpdateAllSetting) {
                $scope.$parent.updateAllSetting();
            }
            else {
                ptAndComSettingService.updatePtAndComSetting(me.updatePtAndCommUrl, $scope.ptAndComSetting, $scope.$parent);
            }
        };
    }
]);