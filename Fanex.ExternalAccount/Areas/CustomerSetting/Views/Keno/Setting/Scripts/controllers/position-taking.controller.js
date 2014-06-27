"use strict";

KenoModule.controller('positionTakingController', function ($scope, kenoRepositories, ngDialog, $templateCache) {
    kenoRepositories.getData().then(
        function () {
            $scope.isCopyAll = false;
            $scope.roleId = $scope.$parent.customerInformation.roleId;
            $scope.isDisableAutoMPT = $scope.$parent.customerInformation.isDisableAutoMPT;

            $scope.copyAll = function (isCopy, betTypeId) {
                $scope.isCopyAll = isCopy;
                $scope.positionTakingCopyAll.betTypeId = isCopy == true ? betTypeId : -1; ;
                $scope.positionTakingCopyAll.isCopiedAll = isCopy;
                $scope.$broadcast('totallyChecking');
            };

            $scope.setValueAllMember = function (isUpdateAllMember) {
                var length = $scope.positionTakingList.length;
                if (isUpdateAllMember) {
                    for (var i = 0; i < length; i++) {
                        $scope.positionTakingList[i].isUpdateAllMember = true;
                    }
                } else {
                    for (var i = 0; i < length; i++) {
                        $scope.positionTakingList[i].isUpdateAllMember = false;
                    }
                }
                ngDialog.close();
                $scope.$emit('confirmUpdateAllMember');
            };

            $scope.$on('updateAllMember', function (event) {
                if ($scope.confirmIsAutoArray.length > 0) {
                    ngDialog.open({
                        template: $templateCache.get('confirmUpdateAllMember'),
                        scope: $scope,
                        closeByCloseButton: true
                    });
                }
            });


            $scope.$on('confirmIsAuto', function (event, data) {
                var index = $scope.$parent.confirmIsAutoArray.indexOf(data.betTypeId);

                if (data.isAdd && index == -1) {
                    $scope.$parent.confirmIsAutoArray.push(data.betTypeId);
                }

                if (!data.isAdd && index != -1) {
                    $scope.$parent.confirmIsAutoArray.splice(index, 1);
                }

                return false;
            });

            $scope.$on('totallyChecking', function (event) {
                var length = $scope.positionTakingList.length;

                $scope.$parent.isPositionTakingPartial = false;

                if ($scope.isCopyAll) {
                    $scope.$parent.isPositionTakingPartial = true;
                } else {
                    for (var i = 0; i < length; i++) {
                        var positionTakingTmp = $scope.positionTakingList[i];
                        if (positionTakingTmp.isUpdatePT) {
                            $scope.$parent.isPositionTakingPartial = true;
                            break;
                        }
                    }
                }

            });
        }, function (reason) { }, function (update) { });
});