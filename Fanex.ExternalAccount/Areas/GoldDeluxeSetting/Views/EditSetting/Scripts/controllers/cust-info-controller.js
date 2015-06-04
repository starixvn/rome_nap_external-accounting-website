fanex.goldDeluxeSetting.controller('CustomerInfoController',
  [
      '$scope',
      function ($scope) {
          'use strict';

          $scope.enableGoldDeluxe = !settingViewModel.Setting.DisableGoldDeluxe;
          $scope.enableDownline = !settingViewModel.Setting.DisableDownlines;
          $scope.hasDownline = settingViewModel.Setting.HasDownline;
          $scope.disableCheckBoxGoldDeluxe = settingViewModel.Setting.DisableUpline;

          $scope.updateSettingStatus = function (setting) {
              setting.DisableGoldDeluxe = !$scope.enableGoldDeluxe;
              setting.DisableDownlines = !$scope.enableDownline;
          };
      }
  ]);