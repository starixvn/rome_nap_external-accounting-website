"use strict";

KenoModule.controller('mainController', function ($scope, $timeout, $http, kenoRepositories, ngDialog, commonService) {
    kenoRepositories.getData().then(
        function (kenoSetting) {
            $scope.kenoSetting = kenoSetting;
            $scope.customerInformation = $scope.kenoSetting.customerInformation;
            $scope.betSettingList = $scope.kenoSetting.betSettings;
            $scope.betSettingCopyAll = $scope.kenoSetting.betSettingCopyAll;
            $scope.positionTakingList = $scope.kenoSetting.positionTakings;
            $scope.positionTakingCopyAll = $scope.kenoSetting.positionTakingCopyAll;
            $scope.statusCopyAll = $scope.kenoSetting.statusCopyAll;

            $scope.isUpdatedKeno = false;
            $scope.isPositionTakingPartial = false;
            $scope.isBetSettingPartial = false;
            $scope.isBetSettingValid = true;
            $scope.confirmIsAutoArray = [];
            $scope.errors = {};

            $scope.multipleEdit = {
                isMultipleEdit: isMultipleEdit,
                custids: isMultipleEdit ? custids : [],
                usernameArray: usernames,
                usernames: isMultipleEdit ? usernames.join(", ") : [],
                updatedCount: 0
            };

            $scope.$watch('[isBetSettingPartial, isPositionTakingPartial, isBetSettingValid]', function (newValue, oldValue) {
                if ($scope.multipleEdit.isMultipleEdit) {
                    $scope.isPositionTakingPartial = true;
                }

                if (newValue !== oldValue) {
                    $scope.isUpdatedKeno = ($scope.isPositionTakingPartial || $scope.isBetSettingPartial) && $scope.isBetSettingValid;
                }
            }, true);

            $scope.updated = function (isUpdatePartial, partialName) {
                if (isUpdatePartial) {
                    switch (partialName) {
                        case "PositionTaking":
                            $scope.kenoSetting.betSettings = null;
                            break;
                        case "BetSetting":
                            $scope.kenoSetting.positionTakings = null;
                            break;
                        default:
                            break;
                    }
                }

                if ($scope.confirmIsAutoArray.length > 0) {
                    $scope.$broadcast("updateAllMember");
                    $scope.$on('confirmUpdateAllMember', function (event) {
                        $scope.doUpdated();
                    });
                }
                else {
                    $scope.doUpdated();
                }
            };

            $scope.getUrlUpdated = function (username) {
                var url = "";
                switch ($scope.kenoSetting.customerInformation.actionType) {
                    case 4: url = 'customersetting/KenoSetting/kenosetting/UpdateSuper?&username=' + username;
                        break;
                    case 3: url = 'customersetting/KenoSetting/kenosetting/UpdateMaster?&username=' + username;
                        break;
                    case 2: url = 'customersetting/KenoSetting/kenosetting/UpdateAgent?&username=' + username;
                        break;
                    case 21: url = 'customersetting/KenoSetting/kenosetting/UpdateAgentOnMember?&username=' + username;
                        break;
                    case 41: url = 'customersetting/KenoSetting/kenosetting/UpdateSuperOnMember?&username=' + username;
                        break;
                    case 31: url = 'customersetting/KenoSetting/kenosetting/UpdateMasterOnMember?&username=' + username;
                        break;
                    default:
                        break;
                }

                return domain + url;
            }

            $scope.showMesseges = function (html) {
                $("#errMsg").append(html).show();
                Fanex.CustomerSetting.Core.UnBlockUI();
            }

            $scope.doUpdated = function () {
                Fanex.CustomerSetting.Core.BlockUI();
                $("#errMsg").empty();

                if (!$scope.multipleEdit.isMultipleEdit) {
                    $scope.save($scope.kenoSetting, 0);
                }
                else {
                    $scope.multipleEdit.updatedCount = 0;
                    $scope.save($scope.kenoSetting, 0);
                }
            };

            $scope.save = function (kenoSetting, index) {
                if ($scope.multipleEdit.isMultipleEdit) {
                    if (index > 0) {
                        kenoSetting.customerInformation.isMultiple = true;
                    }

                    kenoSetting.customerInformation.custId = $scope.multipleEdit.custids[index];
                    kenoSetting.customerInformation.username = $scope.multipleEdit.usernameArray[index];
                }
                var url = $scope.getUrlUpdated(kenoSetting.customerInformation.username);

                $http.post(url, kenoSetting)
                .success(function (data, status, headers, config) {
                    if ($scope.multipleEdit.isMultipleEdit) {
                        data.custId = $scope.multipleEdit.custids[index];
                        data.username = $scope.multipleEdit.usernameArray[index];
                    }
                    $scope.afterUpdatedSuccessfully(data, kenoSetting, ++index);
                })
                .error(function () { $scope.afterUpdatedFailed(kenoSetting, ++index); });
            };

            $scope.afterUpdatedSuccessfully = function (results, kenoSetting, index) {
                var usermessageGroup = $scope.multipleEdit.isMultipleEdit ? '<div >' + results.username + '</div>' : '';
                var multipleEditClass = $scope.multipleEdit.isMultipleEdit ? ' multipleedit' : '';

                if (typeof results != 'undefined') {
                    var length = results.length;
                    var isError = false;
                    var html = usermessageGroup;

                    for (var i = 0; i < length; i++) {
                        if (results[i].ErrorCode > 0) {
                            html += '<div class="alerterror' + multipleEditClass + '">' + results[i].ErrorMessage + '</div>';
                            isError = true;
                        }
                    }

                    if (!isError) {
                        $scope.multipleEdit.updatedCount++;
                        $scope.showMesseges(usermessageGroup + '<div class="alertsuccess' + multipleEditClass + '">' + resources.successful + '</div>');

                        if (($scope.multipleEdit.isMultipleEdit && $scope.multipleEdit.updatedCount == $scope.multipleEdit.custids.length)
                        || !$scope.multipleEdit.isMultipleEdit) {
                            $timeout(function () {
                                location.reload();
                            }, 2000);
                        }
                    } else {
                        $scope.showMesseges(html);
                    }
                }
                else {
                    var html = 
                     + '<div class="alerterror' + multipleEditClass + '">' + resources.failed + '</div>';
                    $scope.showMesseges(html);
                }

                if ($scope.multipleEdit.isMultipleEdit && index < $scope.multipleEdit.custids.length) {
                    $scope.save(kenoSetting, index);
                }
            };

            $scope.afterUpdatedFailed = function (kenoSetting, index) {
                var html = '<div class="alerterror">' + resources.failed + '</div>';
                $scope.showMesseges(html);

                if ($scope.multipleEdit.isMultipleEdit && index < $scope.multipleEdit.custids.length) {
                    $scope.save(kenoSetting, index);
                }
            };
        }, function (reason) { }, function (update) { });
});