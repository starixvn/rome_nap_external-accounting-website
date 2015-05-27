fanex.goldDeluxeSetting.controller('PtAndComSettingController',
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
            switch (settingViewModel.Level) {
                case fanex.goldDeluxeSetting.customerLevel.Master:
                    if (settingViewModel.Setting.PositionTaking.Is3
                        && !currentPt.Is3) {
                        $scope.uplinePTOnMember.newUplinePositionTaking = fanex.roundNumber(currentPt.S4 - currentPt.M3);
                        $scope.uplinePTOnMemberOptions = fanex.createOptionsFromRange(
                                                            $scope.uplinePTOnMember.newUplinePositionTaking,
                                                            0,
                                                            $scope.uplinePTOnMember.newUplinePositionTaking,
                                                            fanex.goldDeluxeSetting.PT_INTERVAL);
                        me.$presetOnMemberModal.modal({
                            backdrop: 'static',
                            show: true,
                            keyboard: false
                        });
                        return true;
                    }
                    break;
                case fanex.goldDeluxeSetting.customerLevel.Agent:
                    if (settingViewModel.Setting.PositionTaking.Im2
                        && !currentPt.Im2) {
                        $scope.uplinePTOnMember.newUplinePositionTaking = fanex.roundNumber(currentPt.M3 - currentPt.A2);
                        $scope.uplinePTOnMemberOptions = fanex.createOptionsFromRange(
                                                            $scope.uplinePTOnMember.newUplinePositionTaking,
                                                            0,
                                                            $scope.uplinePTOnMember.newUplinePositionTaking,
                                                            fanex.goldDeluxeSetting.PT_INTERVAL);
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
        $scope.positionTaking = fanex.cloneObject(settingViewModel.Setting.PositionTaking);
        $scope.commission = settingViewModel.Setting.Commission.Commission;
        $scope.parentCommission = settingViewModel.Setting.Commission.ParentCommission;
        $scope.uplinePTOnMemberOptions = [];
        $scope.isUpdateAllSetting = false;
        $scope.language = language;
        me.updatePTComUrl = $('#UpdatePTComSettingUrl').val();

        //// Action
        $scope.updatePtAndComSetting = function () {
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