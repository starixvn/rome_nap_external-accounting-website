var module = angular.module("leftPanelApp", []);

function LeftPanelController($scope) {
    $scope.model = model;    

    $scope.Init = function () {        
        if ($scope.model) {
            var roleId = $scope.model.personalInfo.roleId.value;

            switch (roleId) {
                case 4:
                    $scope.model.personalInfo.custList = 'Master List';
                    break;
                case 3:
                    $scope.model.personalInfo.custList = 'Agent List';
                    break;
                case 2:
                    $scope.model.personalInfo.custList = 'Memer List';
                    break;
                default:
                    break;
            }
        }
    };    

    $scope.GetCustomerList = function () {
        var roleId = $scope.model.personalInfo.roleId.value;
        var custId = $scope.model.personalInfo.custId.value;

        if (!parent.document.getElementById('customerListGrid')) {
            parent.Fanex.ExtAcc.LoadMainFrame(parent.resolveClientUrl('Customer/CustomerList'), undefined, true);
        }
        var interval = setInterval(function () {
            if (parent.document.getElementById('customerListGrid')) {
                clearInterval(interval);

                var allowDoubleComm = false;
                var syncBingo = false;
                var syncCasino = false;
                var syncLiveCasino = false;

                for (var i = 0; i < $scope.model.setting.enableItems.length; i++) {
                    if ($scope.model.setting.enableItems[i].key == 'DoubleComm') {
                        allowDoubleComm = $scope.model.setting.enableItems[i].value;

                        break;
                    }
                }

                for (var i = 0; i < $scope.model.setting.syncItems.length; i++) {
                    if ($scope.model.setting.syncItems[i].key == 'SyncCasino') {
                        syncCasino = $scope.model.setting.syncItems[i].value;
                    }

                    if ($scope.model.setting.syncItems[i].key == 'SyncBingo') {
                        syncBingo = $scope.model.setting.syncItems[i].value;
                    }

                    if ($scope.model.setting.syncItems[i].key == 'SyncLiveCasino') {
                        syncLiveCasino = $scope.model.setting.syncItems[i].value;
                    }
                }

                switch (roleId) {
                    case 4:
                        parent.Fanex.ExtAcc.Customer.GetMasterList(custId, allowDoubleComm, null, null, syncCasino, syncBingo, syncLiveCasino);
                        break;
                    case 3:
                        parent.Fanex.ExtAcc.Customer.GetAgentList(custId, allowDoubleComm, null, null, syncCasino, syncBingo, syncLiveCasino);
                        break;
                    case 2:
                        parent.Fanex.ExtAcc.Customer.GetMemberList(custId, allowDoubleComm, null, null, syncCasino, syncBingo, syncLiveCasino);
                        break;
                    default:
                        break;
                }
            }
        }, 100);
    };

    $scope.GetStatementList = function (custId) {
        parent.Fanex.ExtAcc.LoadMainFrame('/Statement/Index/' + custId);
    };

    $scope.PT = function () {
        var title = $scope.model.personalInfo.currentLevel.value;

        parent.Fanex.ExtAcc.Customer.CurrentLevel = 5;
        parent.window.CurrentLevel = $scope.model.personalInfo.roleId.value;
        parent.window.id = $scope.model.personalInfo.custId.value;
        parent.window.uplineId = $scope.model.personalInfo.roleId.value == 4 ? userId : $scope.model.personalInfo.uplineId.value;

        parent.Fanex.ExtAcc.LoadMainFrame(parent.resolveClientUrl('Sportsbook/EditMultiplePT'))//, 'View Super Sportsbook Setting ' + '<font color="#6dcff6">' + title + '</font>', 894, 700, false, false, 1);
    }

    $scope.Info = function () {
        parent.Fanex.ExtAcc.Customer.UserId = $scope.model.personalInfo.custId.value;
        parent.Fanex.ExtAcc.Form.Show(parent.resolveClientUrl('Customer/EditInfo/?custId=' + $scope.model.personalInfo.custId.value), parent.titles[0] + ': <font color="#6dcff6">' + $scope.model.personalInfo.currentLevel.value + '</font>', 406, 310, false);
    };

    $scope.Transfer = function () {
        parent.Fanex.ExtAcc.Form.Show(parent.resolveClientUrl('Customer/EditTransferCondition/?custId=' + $scope.model.personalInfo.custId.value), parent.titles[1] + ': <font color="#6dcff6">' + $scope.model.personalInfo.currentLevel.value + '</font>', 600, 250, false, false, 1);
    };

    $scope.Casino = function () {
        var roleId = $scope.model.personalInfo.roleId.value;
        var custId = $scope.model.personalInfo.custId.value;
        var url = '';

        switch (roleId) {
            case 3:
                url = parent.resolveClientUrl('CustomerSetting/Casino/Master/EditMaster/' + $scope.model.personalInfo.custId.value + '/' + $scope.model.personalInfo.uplineId.value);
                parent.Fanex.ExtAcc.Form.ShowFrame(url, 'Edit Casino Setting <font color="#6dcff6">' + $scope.model.personalInfo.currentLevel.value + '</font>', 1064, 700, false, null, 1);
                break;
            case 2:
                url = parent.resolveClientUrl('CustomerSetting/Casino/Agent/EditAgent/' + $scope.model.personalInfo.custId.value + '/' + $scope.model.personalInfo.uplineId.value);
                parent.Fanex.ExtAcc.Form.ShowFrame(url, 'Edit Casino Setting <font color="#6dcff6">' + $scope.model.personalInfo.currentLevel.value + '</font>', 1064, 700, false, null, 1);
                break;
            case 1:
                url = parent.resolveClientUrl('CustomerSetting/Casino/Member/EditMember/' + $scope.model.personalInfo.custId.value + '/' + $scope.model.personalInfo.uplineId.value);
                parent.Fanex.ExtAcc.Form.ShowFrame(url, 'Edit Casino Setting <font color="#6dcff6">' + $scope.model.personalInfo.currentLevel.value + '</font>', 1064, 700, false, null, 1);
                break;
            default:
                break;
        }
    };
};

function PersonalInfoController($scope) {
    $scope.className = 'fiftyPercent';    

    if ($scope.$parent.model) {
        $scope.personalInfo = $scope.$parent.model.personalInfo;
        $scope.model = $scope.$parent.model;

        $scope.personalInfoHeight = $('#customerInfo').height();

        if ($scope.personalInfo.roleId.value == 2) {
            $scope.className = 'thirdtyThreePercent';
        }
        else if ($scope.personalInfo.roleId.value == 1) {
            $scope.className = 'onehundredPercent';
        }
    }
    else {
        $scope.model = null;
    }    

    $scope.TooglePersonalInfo = function () {
        if ($('#personalInfo').hasClass('userDown')) {
            $('#personalInfo').removeClass('userDown');
            $('#personalInfo').addClass('userUp');
            $('#customerInfo').height(25);
        }
        else {
            $('#personalInfo').addClass('userDown');
            $('#personalInfo').removeClass('userUp');
            $('#customerInfo').height($scope.personalInfoHeight);
        }
    };

    $scope.Login = function () {
    };
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