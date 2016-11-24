fanex.goldDeluxeSetting.controller('CustomerInfoController',
  [
      '$scope',
      function ($scope) {
          'use strict';
           
          $scope.enableGoldDeluxe = !settingViewModel.Setting.DisableGoldDeluxe;
          $scope.enableDownline = !settingViewModel.Setting.DisableDownlines;
          $scope.hasDownline = settingViewModel.Setting.HasDownline;

          $scope.disableCheckBoxGoldDeluxe = settingViewModel.Setting.DisableUpline;
          $scope.disableCheckBoxDownline = true;
          $scope.originalEnableGoldDeluxe = angular.copy($scope.enableGoldDeluxe);

          $scope.updateSettingStatus = function (setting) {
              setting.DisableGoldDeluxe = !$scope.enableGoldDeluxe;
              setting.DisableDownlines = !$scope.enableDownline;
          };

          $scope.updateDownlineStatus = function () {
              if ($scope.enableGoldDeluxe) {
                  if (!settingViewModel.Setting.DisableGoldDeluxe && !settingViewModel.Setting.DisableDownlines) {
                      $scope.enableDownline = true;
                      $scope.disableCheckBoxDownline = true;
                  }
              }
              else {
                  $scope.enableDownline = false;
              }

              $scope.disableCheckBoxDownline = $scope.originalEnableGoldDeluxe ? true : !$scope.enableGoldDeluxe;
          }
      }
  ]);