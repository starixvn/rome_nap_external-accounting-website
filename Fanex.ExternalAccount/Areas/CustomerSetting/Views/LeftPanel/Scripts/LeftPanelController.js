var module = angular.module("leftPanelApp", []);

function LeftPanelController($scope) {
    $scope.model = model;
};

function PersonalInfoController($scope) {
    if ($scope.$parent.model) {
        $scope.personalInfo = $scope.$parent.model.personalInfo;
        $scope.model = $scope.$parent.model;
    }
    else {
        $scope.model = null;
    }
};

function BalanceController($scope) {
    if ($scope.$parent && $scope.$parent.$parent && $scope.$parent.$parent.model) {
        $scope.balanceInfo = $scope.$parent.$parent.model.balanceInfo;
        $scope.model = $scope.balanceInfo;
    }
    else {
        $scope.model = null;
    }
};

function DownlineListController($scope) {
    if ($scope.$parent && $scope.$parent.$parent && $scope.$parent.$parent.model) {
        $scope.downlines = $scope.$parent.$parent.model.downlineList;
        $scope.model = $scope.downlines.downlines;
    }
    else {
        $scope.model = null;
    }
};

function SettingController($scope) {
    if ($scope.$parent.model) {
        $scope.setting = $scope.$parent.model.setting;
        $scope.model = $scope.$parent.model;
    }
    else {
        $scope.model = null;
    }
};