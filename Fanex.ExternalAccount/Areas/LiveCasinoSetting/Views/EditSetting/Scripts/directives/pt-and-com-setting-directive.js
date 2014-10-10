/* File Created: July 23, 2014 */
var builtUrlTemplate = fanex.liveCasinoSetting.buildTemplateUrlWithVersion;
fanex.liveCasinoSetting.directive('editSuperPtAndCom', [
        function () {
        'use strict';
            return {
                restrict: 'E',
                replace: true,
                controller: 'PtAndComSettingController',
                templateUrl: builtUrlTemplate('Areas/LiveCasinoSetting/Views/EditSetting/Scripts/templates/PtAndCom/edit-super.htm'),
                link: function ($scope) {
                    var currentPt = $scope.positionTaking,
                        currentCom = $scope.commission;
                    $scope.superMaxOptions = fanex.createOptionsFromRange(currentPt.S4, 0, currentPt.L4, fanex.liveCasinoSetting.PT_INTERVAL);
                    $scope.superMinOptions = fanex.createOptionsFromRange(currentPt.N4, 0, currentPt.S4, fanex.liveCasinoSetting.PT_INTERVAL);
                    $scope.superCommissionOptions = fanex.createOptionsFromRange(
                                                            currentCom, 
                                                            0, 
                                                            $scope.parentCommission, 
                                                            fanex.liveCasinoSetting.COM_INTERVAL, 
                                                            fanex.liveCasinoSetting.COM_DECIMAL);
                    $scope.updateSuperMax = function () {
                        if (currentPt.N4 > currentPt.S4) {
                            currentPt.N4 = currentPt.S4;
                        }
                        $scope.superMinOptions = fanex.createOptionsFromRange(currentPt.N4, 0, currentPt.S4, fanex.liveCasinoSetting.PT_INTERVAL);
                    };
                }
            };
        }
    ]);

        fanex.liveCasinoSetting.directive('editMasterPtAndCom', [
        function () {
        'use strict';
            return {
                restrict: 'E',
                replace: true,
                controller: 'PtAndComSettingController',
                templateUrl: builtUrlTemplate('Areas/LiveCasinoSetting/Views/EditSetting/Scripts/templates/PtAndCom/edit-master.htm'),
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
                                                            fanex.liveCasinoSetting.PT_INTERVAL);
                            $scope.updateMasterMax();
                        },
                        updateMasterMax: function () {
                            if (currentPt.N3 > currentPt.M3) {
                                currentPt.N3 = currentPt.M3;
                            }

                            $scope.masterMinOptions = fanex.createOptionsFromRange(currentPt.N3, 0, currentPt.M3, fanex.liveCasinoSetting.PT_INTERVAL);
                        },
                        updateMasterMin: function () {
                            $scope.masterMaxOptions = fanex.createOptionsFromRange(
                                                            currentPt.M3,
                                                            currentPt.N3,
                                                            actions.computeMaxM3(), 
                                                            fanex.liveCasinoSetting.PT_INTERVAL);
                        }
                    };
                    // PT
                    $scope.superPresetOptions = fanex.createOptionsFromRange(currentPt.S3, 0, currentPt.S4, fanex.liveCasinoSetting.PT_INTERVAL);
                    $scope.superAutoPresetOptions = fanex.createOptionsFromRange(currentPt.Us3, 0, currentPt.S4, fanex.liveCasinoSetting.PT_INTERVAL);
                    $scope.masterMaxOptions = fanex.createOptionsFromRange(
                                                            currentPt.M3,
                                                            currentPt.N3,
                                                            actions.computeMaxM3(), 
                                                            fanex.liveCasinoSetting.PT_INTERVAL);
                    $scope.masterMinOptions = fanex.createOptionsFromRange(currentPt.N3, 0, currentPt.M3, fanex.liveCasinoSetting.PT_INTERVAL);

                    // Com
                    $scope.masterCommissionOptions = fanex.createOptionsFromRange(
                                                            currentCom,
                                                            0,
                                                            $scope.parentCommission,
                                                            fanex.liveCasinoSetting.COM_INTERVAL, 
                                                            fanex.liveCasinoSetting.COM_DECIMAL);

                    //// Onchange functions
                    $scope.updateSuperPreset = actions.updateSuperPreset;
                    $scope.updateMasterMax = actions.updateMasterMax;
                    $scope.updateMasterMin = actions.updateMasterMin;
                }
            };
        }
    ]);

        fanex.liveCasinoSetting.directive('editAgentPtAndCom', [
        function () {
        'use strict';
            return {
                restrict: 'E',
                replace: true,
                controller: 'PtAndComSettingController',
                templateUrl: builtUrlTemplate('Areas/LiveCasinoSetting/Views/EditSetting/Scripts/templates/PtAndCom/edit-agent.htm'),
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
                                                                fanex.liveCasinoSetting.PT_INTERVAL);
                            $scope.updateAgentMax();
                        },
                        updateAgentMax: function () {
                            if (currentPt.N2 > currentPt.A2) {
                                currentPt.N2 = currentPt.A2;
                            }

                            $scope.agentMinOptions = fanex.createOptionsFromRange(currentPt.N2, 0, currentPt.A2, fanex.liveCasinoSetting.PT_INTERVAL);
                        },
                        updateAgentMin: function () {
                            $scope.agentMaxOptions = fanex.createOptionsFromRange(
                                                                currentPt.A2,
                                                                currentPt.N2,
                                                                actions.computeMaxA2(), 
                                                                fanex.liveCasinoSetting.PT_INTERVAL);
                        }
                    };

                    // PT
                    $scope.masterPresetOptions = fanex.createOptionsFromRange(currentPt.M2, 0, currentPt.M3, fanex.liveCasinoSetting.PT_INTERVAL);
                    $scope.masterAutoPresetOptions = fanex.createOptionsFromRange(currentPt.Um2, 0, currentPt.M3, fanex.liveCasinoSetting.PT_INTERVAL);
                    $scope.agentMaxOptions = fanex.createOptionsFromRange(
                                                        currentPt.A2,
                                                        currentPt.N2,
                                                        actions.computeMaxA2(), 
                                                        fanex.liveCasinoSetting.PT_INTERVAL);
                    $scope.agentMinOptions = fanex.createOptionsFromRange(currentPt.N2, 0, currentPt.A2, fanex.liveCasinoSetting.PT_INTERVAL);

                    // Com
                    $scope.agentCommissionOptions = fanex.createOptionsFromRange(
                                                                    currentCom,
                                                                    0,
                                                                    $scope.parentCommission,
                                                                    fanex.liveCasinoSetting.COM_INTERVAL, 
                                                                    fanex.liveCasinoSetting.COM_DECIMAL);

                    //// Onchange functions
                    $scope.updateMasterPreset = actions.updateMasterPreset;
                    $scope.updateAgentMax = actions.updateAgentMax;
                    $scope.updateAgentMin = actions.updateAgentMin;

                }
            };
        }
    ]);

fanex.liveCasinoSetting.directive('agentEditMemberPtAndCom', [
        function () {
        'use strict';
            return {
                restrict: 'E',
                replace: true,
                controller: 'PtAndComSettingController',
                templateUrl: builtUrlTemplate('Areas/LiveCasinoSetting/Views/EditSetting/Scripts/templates/PtAndCom/agent-edit-member.htm'),
                link: function ($scope) {
                    var currentPt = $scope.positionTaking,
                        currentCom = $scope.commission;
                    $scope.agentPresetOptions = fanex.createOptionsFromRange(currentPt.A1, currentPt.N2, currentPt.A2, fanex.liveCasinoSetting.PT_INTERVAL);
                    $scope.isAgentPresetOutOfRange = currentPt.A1 < currentPt.A2 || currentPt.A1 > currentPt.N2;
                    $scope.memberCommissionOptions = fanex.createOptionsFromRange(
                                                                    currentCom,
                                                                    0,
                                                                    $scope.parentCommission,
                                                                    fanex.liveCasinoSetting.COM_INTERVAL,
                                                                    fanex.liveCasinoSetting.COM_DECIMAL);
                    $scope.agentPresetChange = function () {
                        if ($scope.isAgentPresetOutOfRange) {
                            $scope.agentPresetOptions = fanex.createOptionsFromRange(
                                                                    currentPt.A1,
                                                                    currentPt.N2,
                                                                    currentPt.A2, 
                                                                    fanex.liveCasinoSetting.PT_INTERVAL);
                            $scope.isAgentPresetOutOfRange = false;
                        }
                    };
                }
            };
        }
    ]);

        fanex.liveCasinoSetting.directive('superEditMemberPt', [
        function () {
            'use strict';
            return {
                restrict: 'E',
                replace: true,
                controller: 'PtAndComSettingController',
                templateUrl: builtUrlTemplate('Areas/LiveCasinoSetting/Views/EditSetting/Scripts/templates/PtAndCom/super-edit-member.htm'),
                link: function ($scope) {
                    var currentPt = $scope.positionTaking;
                    $scope.superPTOptions = fanex.createOptionsFromRange(currentPt.S1, currentPt.S1Min, currentPt.S1Max, fanex.liveCasinoSetting.PT_INTERVAL);
                    $scope.superAutoPTOptions = fanex.createOptionsFromRange(currentPt.Us1, currentPt.S1Min, currentPt.S4, fanex.liveCasinoSetting.PT_INTERVAL);
                    $scope.isSuperPTOutOfRange = currentPt.S1 < currentPt.S1Min || currentPt.S1 > currentPt.S1Max;
                    $scope.isSuperAutoPTOutOfRange = currentPt.Us1 < currentPt.S1Min || currentPt.Us1 > currentPt.S4;

                    $scope.superPTChange = function () {
                        if ($scope.isSuperPTOutOfRange) {
                            $scope.superPTOptions = fanex.createOptionsFromRange(
                                currentPt.S1,
                                currentPt.S1Min,
                                currentPt.S1Max, 
                                fanex.liveCasinoSetting.PT_INTERVAL);
                            $scope.isSuperPTOutOfRange = false;
                        }
                    };

                    $scope.superAutoPTChange = function () {
                        if ($scope.isSuperAutoPTOutOfRange) {
                            $scope.superAutoPTOptions = fanex.createOptionsFromRange(
                                currentPt.Us1,
                                currentPt.S1Min,
                                currentPt.S4,
                                fanex.liveCasinoSetting.PT_INTERVAL);
                            $scope.isSuperAutoPTOutOfRange = false;
                        }
                    };
                }
            };
        }
    ]);

fanex.liveCasinoSetting.directive('masterEditMemberPt', [
        function () {
            'use strict';
            return {
                restrict: 'E',
                replace: true,
                controller: 'PtAndComSettingController',
                templateUrl: builtUrlTemplate('Areas/LiveCasinoSetting/Views/EditSetting/Scripts/templates/PtAndCom/master-edit-member.htm'),
                link: function ($scope) {
                    var currentPt = $scope.positionTaking;
                    $scope.masterPTOptions = fanex.createOptionsFromRange(currentPt.M1, currentPt.M1Min, currentPt.M1Max, fanex.liveCasinoSetting.PT_INTERVAL);
                    $scope.masterAutoPTOptions = fanex.createOptionsFromRange(
                                                    currentPt.Um1,
                                                    currentPt.M1Min,
                                                    currentPt.M3, 
                                                    fanex.liveCasinoSetting.PT_INTERVAL);
                    $scope.isMasterPTOutOfRange = currentPt.M1 < currentPt.M1Min || currentPt.M1 > currentPt.M1Max;
                    $scope.isMasterAutoPTOutOfRange = currentPt.Um1 < currentPt.M1Min || currentPt.Um1 > currentPt.M3;
                    
                    $scope.masterPTChange = function () {
                        if ($scope.isMasterPTOutOfRange) {
                            $scope.masterPTOptions = fanex.createOptionsFromRange(
                                                            currentPt.M1,
                                                            currentPt.M1Min,
                                                            currentPt.M1Max, 
                                                            fanex.liveCasinoSetting.PT_INTERVAL);
                            $scope.isMasterPTOutOfRange = false;
                        }
                    };

                    $scope.masterAutoPTChange = function () {
                        if ($scope.isMasterAutoPTOutOfRange) {
                            $scope.masterAutoPTOptions = fanex.createOptionsFromRange(
                                                            currentPt.Um1,
                                                            currentPt.M1Min,
                                                            currentPt.M3,
                                                            fanex.liveCasinoSetting.PT_INTERVAL);
                            $scope.isMasterAutoPTOutOfRange = false;
                        }
                    };
                }
            };
        }
    ]);