fanex.goldDeluxeSetting.controller('CustomerInfoController',
  [
      '$scope',
      function ($scope) {
          'use strict';

          $scope.enableGoldDeluxe = !settingViewModel.Setting.DisableGoldDeluxe;
          $scope.enableDownline = !settingViewModel.Setting.DisableDownlines;
          $scope.hasDownline = settingViewModel.Setting.HasDownline;
          $scope.originalEnableGoldDeluxe = angular.copy($scope.enableGoldDeluxe);
          $scope.originalEnableDownline = angular.copy($scope.enableDownline);

          $scope.updateSettingStatus = function (setting) {
              setting.DisableGoldDeluxe = !$scope.enableGoldDeluxe;
              setting.DisableDownlines = !$scope.enableDownline;
          };

          $scope.updateDownlineStatus = function () {
              if (settingViewModel.Setting.DisableUpline) {
                  $scope.disableCheckBoxGoldDeluxe = $scope.disableCheckBoxDownline = true;
                  return;
              }

              if ($scope.enableGoldDeluxe == $scope.originalEnableGoldDeluxe) {
                  $scope.enableDownline = $scope.originalEnableDownline;
              }

              if ($scope.enableGoldDeluxe) {
                  if ($scope.enableDownline) {
                      $scope.disableCheckBoxGoldDeluxe = false;
                      $scope.disableCheckBoxDownline = true;
                  }
                  else {
                      $scope.disableCheckBoxGoldDeluxe = false;
                      $scope.disableCheckBoxDownline = false;
                  }
              }
              else {
                  $scope.enableDownline = false;
                  $scope.disableCheckBoxGoldDeluxe = false;
                  $scope.disableCheckBoxDownline = true;
              }
          }

          $scope.updateDownlineStatus();
      }
  ]);