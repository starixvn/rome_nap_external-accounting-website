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

function SettingController($scope, $http) {
    if ($scope.$parent.model) {
        $scope.setting = $scope.$parent.model.setting;
        $scope.model = $scope.$parent.model;
    }
    else {
        $scope.model = null;
    }

    $scope.AddNew = function () {
        var roleId = $scope.model.personalInfo.roleId.value;
        var actionName = '';

        switch (roleId) {
            case 2:
                actionName = 'Customer/AgentAddNewMember/' + $scope.model.personalInfo.custId.value + '/' + $scope.model.personalInfo.currentLevel.value + '/' + parent.$('#selectedGroupName').html(); ;
                break;
            case 3:
                actionName = 'Customer/MasterAddNewAgent/' + $scope.model.personalInfo.custId.value + '/' + $scope.model.personalInfo.currentLevel.value + '/' + parent.$('#selectedGroupName').html(); ;
                break;
            case 4:
                actionName = 'Customer/SuperAddNewMaster/' + $scope.model.personalInfo.custId.value + '/' + $scope.model.personalInfo.currentLevel.value + '/' + parent.$('#selectedGroupName').html(); ;
                break;
            default:
                break;
        }

        if (actionName != '') {
            parent.Fanex.ExtAcc.BlockUI();

            $http({ url: parent.resolveClientUrl(actionName), method: 'post', headers: { "X-Requested-With": "XMLHttpRequest"} })
            .success(function (result) {
                parent.$('#contentSplitter').html(result);
            });
        }
    }
};

function LoginController($scope) {
    if ($scope.$parent.model) {
        $scope.loginInfo = $scope.$parent.model.login;
        $scope.model = $scope.$parent.model;
    }
    else {
        $scope.model = null;
    }
};