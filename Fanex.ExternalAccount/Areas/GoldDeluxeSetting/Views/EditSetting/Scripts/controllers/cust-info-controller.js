fanex.goldDeluxeSetting.controller('CustomerInfoController', function ($scope) {

    $scope.enableGoldDeluxe = !settingViewModel.Setting.DisableGoldDeluxe;
    $scope.enableDownline = !settingViewModel.Setting.DisableDownlines;
    $scope.hasDownline = settingViewModel.Setting.HasDownline;
    $scope.disableCheckBoxGoldDeluxe = settingViewModel.Setting.DisableUpline;

    $scope.updateSettingStatus = function (setting) {
        setting.DisableGoldDeluxe = !$scope.enableGoldDeluxe;
        setting.DisableDownlines = !$scope.enableDownline;
    };
});
