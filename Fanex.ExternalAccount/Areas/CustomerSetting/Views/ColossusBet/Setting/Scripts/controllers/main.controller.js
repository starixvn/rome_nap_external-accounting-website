"use strict";
var ColossusBetModule = angular.module('colossusBetSettingModule', []);
ColossusBetModule.controller('mainController', function ($scope, $timeout, colossusBetRepositories, commonService) {
    colossusBetRepositories.getData().then(
        function (colossusBetSetting) {
            $scope.colossusBetSetting = colossusBetSetting;

            $scope.customerInformationPartial = $scope.colossusBetSetting.CustomerInformationPartial;
            $scope.statusPartial = $scope.colossusBetSetting.StatusPartial;
            $scope.comissionPartial = $scope.colossusBetSetting.ComissionPartial;
            $scope.cashInSharePartial = $scope.colossusBetSetting.CashInSharePartial;

            $scope.isUpdatedAll = false;
            $scope.errors = {};

            $scope.$watch('[statusPartial.IsStatusUpdated, comissionPartial.IsCommisionUpdated, cashInSharePartial.IsCashInShareUpdated]', function (newValue, oldValue) {
                if (newValue !== oldValue) {
                    $scope.isUpdatedAll = $scope.statusPartial.IsStatusUpdated || $scope.comissionPartial.IsCommisionUpdated || $scope.cashInSharePartial.IsCashInShareUpdated;
                }
            }, true);

            $scope.updated = function (isUpdatePartial, partialName) {
                if (isUpdatePartial) {
                    switch (partialName) {
                        case "StatusPartial":
                            $scope.comissionPartial = null;
                            $scope.cashInSharePartial = null;
                            break;
                        case "ComissionPartial":
                            $scope.statusPartial = null;
                            $scope.cashInSharePartial = null;
                            break;
                        case "CashInSharePartial":
                            $scope.statusPartial = null;
                            $scope.comissionPartial = null;
                            break;
                        default:
                            break;
                    }
                }


                $scope.doUpdated($("#UpdateSettingtUrl").val());

            };

            $scope.showMesseges = function (html, isSuccess) {
                if (isSuccess) {
                    $timeout(function () {
                        $("#divShowInfo").append(html);
                        commonService.unblockUI();
                    }, 4000);
                }
                else {
                    $("#divShowInfo").append(html);
                    commonService.unblockUI();
                }
            }

            $scope.doUpdated = function (url) {
                commonService.blockUI();

                colossusBetRepositories.save(url, $scope.colossusBetSetting).then(
                    function (results) {
                        $("#divShowInfo").empty().show();

                        var html = "";
                        if (typeof results != 'undefined') {
                            var length = results.length;
                            var isError = false;
                            for (var i = 0; i < length; i++) {
                                if (results[i].ErrorCode > 0) {
                                    isError = true;
                                    html += '<div class="alerterror">' + results[i].ErrorMessage + '</div>';
                                }
                            }

                            if (!isError) {
                                $scope.showMesseges('<div class="alertsuccess">' + resources.successful + '</div>', true);
                                $timeout(function () {
                                    location.reload();
                                    parent.frames["centerFr"].location.reload();
                                }, 5000);
                            } else {
                                $scope.showMesseges(html, false);
                            }
                        } else {
                            $scope.showMesseges('<div class="alerterror">' + resources.failed + '</div>', false);
                        }
                    },
                    function () {
                        $scope.showMesseges('<div class="alerterror">' + resources.failed + '</div>', false);
                    });
            };
        }, function (reason) { }, function (update) { });
});