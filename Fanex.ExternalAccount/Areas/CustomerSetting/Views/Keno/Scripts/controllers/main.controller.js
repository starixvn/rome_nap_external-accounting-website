"use strict";

KenoModule.controller('mainController', function ($scope, kenoRepositories) {
    $scope.kenoSetting = kenoRepositories.kenoSetting;
});


//if (fanex.customerSetting.keno) {
//    fanex.customerSetting.keno.controller('mainController', ['$scope', '$rootScope', '$http', kenoRepositories, function ($scope, $rootScope, $http, kenoRepositories) {
//        $scope.kenoSetting = kenoRepositories.kenoSetting;

//        $scope.updateCustomerSetting = function (isUpdatePartial, sectionName) {
//            var changedBetSettings = null;
//            var changedPositiongTakings = null;
//            var changedCommissionSettings = null;

//            if (isUpdatePartial) {
//                switch (sectionName) {
//                    case "BetSetting":
//                        var betSettingScope = angular.element('[ng-controller="fanex.customerSetting.betSettingController"]').scope();
//                        if (!betSettingScope.isValidData()) {
//                            return;
//                        }
//                        changedBetSettings = betSettingScope.getChangedData();
//                        break;
//                    case "PositionTaking":
//                        var positionTakingScope = angular.element('[ng-controller="fanex.customerSetting.positionTakingController"]').scope();
//                        if (!positionTakingScope.isValidPositionTakings()) {
//                            return;
//                        }
//                        changedPositiongTakings = positionTakingScope.getChangedPositionTakings();

//                        if (!positionTakingScope.isValidCommissionSettings()) {
//                            return;
//                        }
//                        changedCommissionSettings = positionTakingScope.getChangedCommissionSettings();
//                        break;
//                    default:
//                        break;
//                }
//            }
//            else {
//                var betSettingScope = angular.element('[ng-controller="fanex.customerSetting.betSettingController"]').scope();
//                if (!betSettingScope.isValidData()) {
//                    return;
//                }
//                changedBetSettings = betSettingScope.getChangedData();

//                var positionTakingScope = angular.element('[ng-controller="fanex.customerSetting.positionTakingController"]').scope();
//                if (!positionTakingScope.isValidPositionTakings()) {
//                    return;
//                }
//                changedPositiongTakings = positionTakingScope.getChangedPositionTakings();

//                if (!positionTakingScope.isValidCommissionSettings()) {
//                    return;
//                }
//                changedCommissionSettings = positionTakingScope.getChangedCommissionSettings();
//            }

//            if ((!isUpdatePartial && changedBetSettings.length == 0 && changedPositiongTakings.length == 0 && changedCommissionSettings.length == 0)
//                || (isUpdatePartial && sectionName == "BetSetting" && changedBetSettings.length == 0)
//                || (isUpdatePartial && sectionName == "PositionTaking" && changedPositiongTakings.length == 0 && changedCommissionSettings.length == 0)) {
//                alert("Data isn't changed.");
//                return;
//            }

//            $http({
//                method: 'POST',
//                url: domain + "customersetting/keno/kenosetting/UpdateCustomerSetting?custid=1",
//                headers: { 'Content-Type': 'application/json' },
//                data: {
//                    BetSettings: changedBetSettings,
//                    PositiongTakingSections: (changedPositiongTakings == null ?
//                    null : fanex.customerSetting.commonService.composeChangedPositiongTakingSections(changedPositiongTakings, changedCommissionSettings)),
//                    FormType: $scope.model.formType
//                }
//            }).
//            success(function (data, status, headers, config) {
//                if (data.ErrCode != 0) {
//                    alert(data.ErrMsg);
//                }
//            }).
//            error(function (data, status, headers, config) {
//                alert(data.ErrMsg);
//            });
//        };
//    } ]);
//}