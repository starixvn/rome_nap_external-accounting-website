"use strict";

var UMError = 90014;
KenoModule.controller('mainController', function ($scope, $timeout, $http, kenoRepositories, ngDialog, commonService) {
    kenoRepositories.getData().then(
        function (kenoSetting) {
            $scope.kenoSetting = kenoSetting;

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
            $scope.errors = false;
            $scope.customerInformation = $scope.kenoSetting.customerInformation;
            if ($scope.customerInformation.roleId == 4) {
                kenoRepositories.getSuperInfo($scope.customerInformation.custId).then(function (superInfo) {
                    $scope.customerInformation.firstName = superInfo.firstName;
                    $scope.customerInformation.lastName = superInfo.lastName;
                    $scope.customerInformation.mobilePhone = superInfo.mobilePhone;
                    $scope.customerInformation.phone = superInfo.phone;
                    $scope.customerInformation.fax = superInfo.fax;
                });
            }

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
                if ($scope.positionTakingCopyAll.isCopiedAll) {
                    $scope.$broadcast('resetPositionTaking', { "betTypeId": $scope.positionTakingCopyAll.betTypeId });
                }

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

                $scope.confirmIsAutoArray.length > 0 ? $scope.$broadcast("updateAllMember") : $scope.doUpdated();
            };

            $scope.$on('confirmUpdateAllMember', function () {
                $scope.doUpdated();
            });

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

                var antiXSRFToken = document.getElementsByName("__RequestVerificationToken");
                $http.defaults.headers.post.__RequestVerificationToken = antiXSRFToken[0].value;

                $http.post(url, kenoSetting)
                    .success(function (data, status, headers, config) {

                        if (typeof data === 'string' && data.indexOf('securityCodePage') > -1) {
                            Fanex.CustomerSetting.Core.UnBlockUI();

                            parent.$('#securityCodePageContainer').html(data);
                            return;
                        }

                        if ($scope.multipleEdit.isMultipleEdit && typeof data != "string") {
                            data.custId = $scope.multipleEdit.custids[index];
                            data.username = $scope.multipleEdit.usernameArray[index];
                        }
                        $scope.afterUpdatedSuccessfully(data, kenoSetting, ++index);
                    })
                    .error(function () { $scope.afterUpdatedFailed(kenoSetting, ++index); });
            };

            $scope.afterUpdatedSuccessfully = function (results, kenoSetting, index) {
                var usermessageGroup = '<div class="alertfailgroup">' + kenoSetting.customerInformation.username + '</div>';
                var multipleEditClass = ' multipleedit';

                if (typeof results != 'undefined') {
                    var length = results.length;
                    var isError = false;
                    var html = results && results.length > 0 && results[0].ErrorCode === UMError ? '' : usermessageGroup;

                    if (typeof results == 'string') {
                        html += results;
                        $scope.errors = true;
                        isError = true;
                    }
                    else {
                        for (var i = 0; i < length; i++) {
                            if (results[i].ErrorCode > 0) {
                                html += '<div class="alerterror' + multipleEditClass + '">' + results[i].ErrorMessage + '</div>';
                                $scope.errors = true;
                                isError = true;
                            }
                        }
                    }

                    if (!isError) {
                        var usermessageGroupSuccess = '<div class="alertsuccessgroup">' + kenoSetting.customerInformation.username + '</div>';
                        $scope.multipleEdit.updatedCount++;
                        $scope.showMesseges(usermessageGroupSuccess + '<div class="alertsuccess' + multipleEditClass + '">' + resources.successful + '</div><br>');

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
                    var html = usermessageGroup + '<div class="alerterror' + multipleEditClass + '">' + resources.failed + '</div><br>';
                    $scope.showMesseges(html);
                }

                if (((results && results.length > 0 && results[0].ErrorCode !== UMError) || results.length === 0) && ($scope.multipleEdit.isMultipleEdit && index < $scope.multipleEdit.custids.length)) {
                    $scope.save(kenoSetting, index);
                }
            };

            $scope.afterUpdatedFailed = function (kenoSetting, index) {
                var html = '<div class="alerterror">' + resources.failed + '</div><br>';
                $scope.showMesseges(html);

                if ($scope.multipleEdit.isMultipleEdit && index < $scope.multipleEdit.custids.length) {
                    $scope.save(kenoSetting, index);
                }
            };
        }, function (reason) { }, function (update) { });
});