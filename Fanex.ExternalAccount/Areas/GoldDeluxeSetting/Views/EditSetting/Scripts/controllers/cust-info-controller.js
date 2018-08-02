fanex.goldDeluxeSetting.controller('CustomerInfoController',
  [
      '$scope',
      'CustomerSettingService',
      function ($scope, customerSettingService) {
          'use strict';

          $scope.SuperInfo =
              {
                  FirstName: settingViewModel.Setting.FirstName,
                  LastName: settingViewModel.Setting.LastName,
                  Phone: settingViewModel.Setting.Phone,
                  MobilePhone: settingViewModel.Setting.MobilePhone,
                  Fax: settingViewModel.Setting.Fax
              };
          if (settingViewModel.Level == 4) {
              var getSuperInfoUrl = $('#GetSuperInfoUrl').val();
              customerSettingService.getSuperInfo(settingViewModel.CustId, getSuperInfoUrl).success(function (data) {
                  $scope.SuperInfo =
                      {
                          FirstName: data.firstName,
                          LastName: data.lastName,
                          Phone: data.phone,
                          MobilePhone: data.mobilePhone,
                          Fax: data.fax
                      };
              });
          }

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