var builtUrlTemplate = fanex.goldDeluxeSetting.buildTemplateUrlWithVersion;

fanex.goldDeluxeSetting.directive('editSuperPtAndCom', [
        function () {
        'use strict';
            return {
                restrict: 'E',
                replace: true,
                controller: 'PtAndComSettingController',
                templateUrl: builtUrlTemplate('Areas/GoldDeluxeSetting/Views/EditSetting/Scripts/templates/PtAndCom/edit-super.htm'),
                link: function ($scope) {
                    var currentPt = $scope.positionTaking,
                        currentCom = $scope.commission;
                    $scope.superMaxOptions = fanex.createOptionsFromRange(currentPt.S4, 0, currentPt.L4, fanex.goldDeluxeSetting.PT_INTERVAL, isPT3Decimal ? 3 : 2);
                    $scope.superMinOptions = fanex.createOptionsFromRange(currentPt.N4, 0, currentPt.S4, fanex.goldDeluxeSetting.PT_INTERVAL, isPT3Decimal ? 3 : 2);
                    $scope.superCommissionOptions = fanex.createOptionsFromRange(
                                                            currentCom, 
                                                            0, 
                                                            $scope.parentCommission, 
                                                            fanex.goldDeluxeSetting.COM_INTERVAL, 
                                                            fanex.goldDeluxeSetting.COM_DECIMAL);
                    $scope.updateSuperMax = function () {
                        if (currentPt.N4 > currentPt.S4) {
                            currentPt.N4 = currentPt.S4;
                        }
                        $scope.superMinOptions = fanex.createOptionsFromRange(currentPt.N4, 0, currentPt.S4, fanex.goldDeluxeSetting.PT_INTERVAL, isPT3Decimal ? 3 : 2);
                    };
                }
            };
        }
    ]);

        fanex.goldDeluxeSetting.directive('editMasterPtAndCom', [
        function () {
        'use strict';
            return {
                restrict: 'E',
                replace: true,
                controller: 'PtAndComSettingController',
                templateUrl: builtUrlTemplate('Areas/GoldDeluxeSetting/Views/EditSetting/Scripts/templates/PtAndCom/edit-master.htm'),
                link: function ($scope) {

                    var currentPt = $scope.positionTaking,
                        currentCom = $scope.commission;

                    var actions = {
                        computeMaxM3: function () {
                            if (currentPt.Is3) {
                                return currentPt.S4;
                            }
                            else {
                                return fanex.roundNumber(currentPt.S4 - currentPt.S3);
                            }
                        },
                        updateSuperPreset: function () {
                            var maxOfMasterMax = actions.computeMaxM3();
                            if (currentPt.M3 > maxOfMasterMax) {
                                currentPt.M3 = maxOfMasterMax;
                            }

                            $scope.masterMaxOptions = fanex.createOptionsFromRange(
                                                            currentPt.M3,
                                                            currentPt.N3,
                                                            maxOfMasterMax, 
                                                            fanex.goldDeluxeSetting.PT_INTERVAL, isPT3Decimal ? 3 : 2);
                            $scope.updateMasterMax();
                        },
                        updateMasterMax: function () {
                            if (currentPt.N3 > currentPt.M3) {
                                currentPt.N3 = currentPt.M3;
                            }

                            $scope.masterMinOptions = fanex.createOptionsFromRange(currentPt.N3, 0, currentPt.M3, fanex.goldDeluxeSetting.PT_INTERVAL, isPT3Decimal ? 3 : 2);
                        },
                        updateMasterMin: function () {
                            $scope.masterMaxOptions = fanex.createOptionsFromRange(
                                                            currentPt.M3,
                                                            currentPt.N3,
                                                            actions.computeMaxM3(), 
                                                            fanex.goldDeluxeSetting.PT_INTERVAL, isPT3Decimal ? 3 : 2);
                        }
                    };
                    // PT
                    $scope.superPresetOptions = fanex.createOptionsFromRange(currentPt.S3, 0, currentPt.S4, fanex.goldDeluxeSetting.PT_INTERVAL, isPT3Decimal ? 3 : 2);
                    $scope.superAutoPresetOptions = fanex.createOptionsFromRange(currentPt.Us3, 0, currentPt.S4, fanex.goldDeluxeSetting.PT_INTERVAL, isPT3Decimal ? 3 : 2);
                    $scope.masterMaxOptions = fanex.createOptionsFromRange(
                                                            currentPt.M3,
                                                            currentPt.N3,
                                                            actions.computeMaxM3(), 
                                                            fanex.goldDeluxeSetting.PT_INTERVAL, isPT3Decimal ? 3 : 2);
                    $scope.masterMinOptions = fanex.createOptionsFromRange(currentPt.N3, 0, currentPt.M3, fanex.goldDeluxeSetting.PT_INTERVAL, isPT3Decimal ? 3 : 2);

                    // Com
                    $scope.masterCommissionOptions = fanex.createOptionsFromRange(
                                                            currentCom,
                                                            0,
                                                            $scope.parentCommission,
                                                            fanex.goldDeluxeSetting.COM_INTERVAL, 
                                                            fanex.goldDeluxeSetting.COM_DECIMAL);

                    //// Onchange functions
                    $scope.updateSuperPreset = actions.updateSuperPreset;
                    $scope.updateMasterMax = actions.updateMasterMax;
                    $scope.updateMasterMin = actions.updateMasterMin;
                }
            };
        }
    ]);

        fanex.goldDeluxeSetting.directive('editAgentPtAndCom', [
        function () {
        'use strict';
            return {
                restrict: 'E',
                replace: true,
                controller: 'PtAndComSettingController',
                templateUrl: builtUrlTemplate('Areas/GoldDeluxeSetting/Views/EditSetting/Scripts/templates/PtAndCom/edit-agent.htm'),
                link: function ($scope) {

                    var currentPt = $scope.positionTaking,
                        currentCom = $scope.commission;
                    var actions = {
                        computeMaxA2: function () {
                            if (currentPt.Im2) {
                                return currentPt.M3;
                            }
                            else {
                                return fanex.roundNumber(currentPt.M3 - currentPt.M2);
                            }
                        },
                        updateMasterPreset: function () {
                            var maxOfAgentMax = actions.computeMaxA2();
                            if (currentPt.A2 > maxOfAgentMax) {
                                currentPt.A2 = maxOfAgentMax;
                            }

                            $scope.agentMaxOptions = fanex.createOptionsFromRange(
                                                                currentPt.A2,
                                                                currentPt.N2,
                                                                maxOfAgentMax, 
                                                                fanex.goldDeluxeSetting.PT_INTERVAL, isPT3Decimal ? 3 : 2);
                            $scope.updateAgentMax();
                        },
                        updateAgentMax: function () {
                            if (currentPt.N2 > currentPt.A2) {
                                currentPt.N2 = currentPt.A2;
                            }

                            $scope.agentMinOptions = fanex.createOptionsFromRange(currentPt.N2, 0, currentPt.A2, fanex.goldDeluxeSetting.PT_INTERVAL, isPT3Decimal ? 3 : 2);
                        },
                        updateAgentMin: function () {
                            $scope.agentMaxOptions = fanex.createOptionsFromRange(
                                                                currentPt.A2,
                                                                currentPt.N2,
                                                                actions.computeMaxA2(), 
                                                                fanex.goldDeluxeSetting.PT_INTERVAL, isPT3Decimal ? 3 : 2);
                        }
                    };

                    // PT
                    $scope.masterPresetOptions = fanex.createOptionsFromRange(currentPt.M2, 0, currentPt.M3, fanex.goldDeluxeSetting.PT_INTERVAL, isPT3Decimal ? 3 : 2);
                    $scope.masterAutoPresetOptions = fanex.createOptionsFromRange(currentPt.Um2, 0, currentPt.M3, fanex.goldDeluxeSetting.PT_INTERVAL, isPT3Decimal ? 3 : 2);
                    $scope.agentMaxOptions = fanex.createOptionsFromRange(
                                                        currentPt.A2,
                                                        currentPt.N2,
                                                        actions.computeMaxA2(), 
                                                        fanex.goldDeluxeSetting.PT_INTERVAL, isPT3Decimal ? 3 : 2);
                    $scope.agentMinOptions = fanex.createOptionsFromRange(currentPt.N2, 0, currentPt.A2, fanex.goldDeluxeSetting.PT_INTERVAL, isPT3Decimal ? 3 : 2);

                    // Com
                    $scope.agentCommissionOptions = fanex.createOptionsFromRange(
                                                                    currentCom,
                                                                    0,
                                                                    $scope.parentCommission,
                                                                    fanex.goldDeluxeSetting.COM_INTERVAL, 
                                                                    fanex.goldDeluxeSetting.COM_DECIMAL);

                    //// Onchange functions
                    $scope.updateMasterPreset = actions.updateMasterPreset;
                    $scope.updateAgentMax = actions.updateAgentMax;
                    $scope.updateAgentMin = actions.updateAgentMin;

                }
            };
        }
    ]);

fanex.goldDeluxeSetting.directive('agentEditMemberPtAndCom', [
        function () {
        'use strict';
            return {
                restrict: 'E',
                replace: true,
                controller: 'PtAndComSettingController',
                templateUrl: builtUrlTemplate('Areas/GoldDeluxeSetting/Views/EditSetting/Scripts/templates/PtAndCom/agent-edit-member.htm'),
                link: function ($scope) {
                    var currentPt = $scope.positionTaking,
                        currentCom = $scope.commission;
                    $scope.agentPresetOptions = fanex.createOptionsFromRange(currentPt.A1, currentPt.N2, currentPt.A2, fanex.goldDeluxeSetting.PT_INTERVAL, isPT3Decimal ? 3 : 2);
                    $scope.isAgentPresetOutOfRange = currentPt.A1 < currentPt.A2 || currentPt.A1 > currentPt.N2;
                    $scope.memberCommissionOptions = fanex.createOptionsFromRange(
                                                                    currentCom,
                                                                    0,
                                                                    $scope.parentCommission,
                                                                    fanex.goldDeluxeSetting.COM_INTERVAL,
                                                                    fanex.goldDeluxeSetting.COM_DECIMAL);
                    $scope.agentPresetChange = function () {
                        if ($scope.isAgentPresetOutOfRange) {
                            $scope.agentPresetOptions = fanex.createOptionsFromRange(
                                                                    currentPt.A1,
                                                                    currentPt.N2,
                                                                    currentPt.A2, 
                                                                    fanex.goldDeluxeSetting.PT_INTERVAL, isPT3Decimal ? 3 : 2);
                            $scope.isAgentPresetOutOfRange = false;
                        }
                    };
                }
            };
        }
    ]);

        fanex.goldDeluxeSetting.directive('superEditMemberPt', [
        function () {
            'use strict';
            return {
                restrict: 'E',
                replace: true,
                controller: 'PtAndComSettingController',
                templateUrl: builtUrlTemplate('Areas/GoldDeluxeSetting/Views/EditSetting/Scripts/templates/PtAndCom/super-edit-member.htm'),
                link: function ($scope) {
                    var currentPt = $scope.positionTaking;
                    $scope.superPTOptions = fanex.createOptionsFromRange(currentPt.S1, currentPt.S1Min, currentPt.S1Max, fanex.goldDeluxeSetting.PT_INTERVAL, isPT3Decimal ? 3 : 2);
                    $scope.superAutoPTOptions = fanex.createOptionsFromRange(currentPt.Us1, currentPt.S1Min, currentPt.S4, fanex.goldDeluxeSetting.PT_INTERVAL, isPT3Decimal ? 3 : 2);
                    $scope.isSuperPTOutOfRange = currentPt.S1 < currentPt.S1Min || currentPt.S1 > currentPt.S1Max;
                    $scope.isSuperAutoPTOutOfRange = currentPt.Us1 < currentPt.S1Min || currentPt.Us1 > currentPt.S4;

                    $scope.superPTChange = function () {
                        if ($scope.isSuperPTOutOfRange) {
                            $scope.superPTOptions = fanex.createOptionsFromRange(
                                currentPt.S1,
                                currentPt.S1Min,
                                currentPt.S1Max, 
                                fanex.goldDeluxeSetting.PT_INTERVAL, isPT3Decimal ? 3 : 2);
                            $scope.isSuperPTOutOfRange = false;
                        }
                    };

                    $scope.superAutoPTChange = function () {
                        if ($scope.isSuperAutoPTOutOfRange) {
                            $scope.superAutoPTOptions = fanex.createOptionsFromRange(
                                currentPt.Us1,
                                currentPt.S1Min,
                                currentPt.S4,
                                fanex.goldDeluxeSetting.PT_INTERVAL, isPT3Decimal ? 3 : 2);
                            $scope.isSuperAutoPTOutOfRange = false;
                        }
                    };
                }
            };
        }
    ]);

fanex.goldDeluxeSetting.directive('masterEditMemberPt', [
        function () {
            'use strict';
            return {
                restrict: 'E',
                replace: true,
                controller: 'PtAndComSettingController',
                templateUrl: builtUrlTemplate('Areas/GoldDeluxeSetting/Views/EditSetting/Scripts/templates/PtAndCom/master-edit-member.htm'),
                link: function ($scope) {
                    var currentPt = $scope.positionTaking;
                    $scope.masterPTOptions = fanex.createOptionsFromRange(currentPt.M1, currentPt.M1Min, currentPt.M1Max, fanex.goldDeluxeSetting.PT_INTERVAL, isPT3Decimal ? 3 : 2);
                    $scope.masterAutoPTOptions = fanex.createOptionsFromRange(
                                                    currentPt.Um1,
                                                    currentPt.M1Min,
                                                    currentPt.M3, 
                                                    fanex.goldDeluxeSetting.PT_INTERVAL, isPT3Decimal ? 3 : 2);
                    $scope.isMasterPTOutOfRange = currentPt.M1 < currentPt.M1Min || currentPt.M1 > currentPt.M1Max;
                    $scope.isMasterAutoPTOutOfRange = currentPt.Um1 < currentPt.M1Min || currentPt.Um1 > currentPt.M3;
                    
                    $scope.masterPTChange = function () {
                        if ($scope.isMasterPTOutOfRange) {
                            $scope.masterPTOptions = fanex.createOptionsFromRange(
                                                            currentPt.M1,
                                                            currentPt.M1Min,
                                                            currentPt.M1Max, 
                                                            fanex.goldDeluxeSetting.PT_INTERVAL, isPT3Decimal ? 3 : 2);
                            $scope.isMasterPTOutOfRange = false;
                        }
                    };

                    $scope.masterAutoPTChange = function () {
                        if ($scope.isMasterAutoPTOutOfRange) {
                            $scope.masterAutoPTOptions = fanex.createOptionsFromRange(
                                                            currentPt.Um1,
                                                            currentPt.M1Min,
                                                            currentPt.M3,
                                                            fanex.goldDeluxeSetting.PT_INTERVAL, isPT3Decimal ? 3 : 2);
                            $scope.isMasterAutoPTOutOfRange = false;
                        }
                    };
                }
            };
        }
    ]);