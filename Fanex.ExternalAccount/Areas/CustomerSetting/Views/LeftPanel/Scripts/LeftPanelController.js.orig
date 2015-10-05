var module = angular.module("leftPanelApp", [])
.directive('ngBlur', function () {
    return function (scope, elem, attrs) {
        elem.bind('blur', function () {
            scope.$apply(attrs.ngBlur);
        });
    };
});

angular.element(document.getElementById('ng-app')).ready(function () {
    parent.Fanex.ExtAcc.UnBlockUI();
    $('#ng-app').show();
    $('#txtLeftPanelUserName').focus();
});

Array.prototype.contains = function (item) {
    for (var i = 0; i < this.length; i++) {
        if (this[i] === item) {
            return true;
        }
    }

    return false;
};

function LeftPanelController($scope, $element) {
    $scope.model = model;

    $scope.GetCustomerList = function () {
        var roleId = $scope.model.personalInfo.roleId.value;
        var custId = $scope.model.personalInfo.custId.value;

        if (!parent.document.getElementById('customerListGrid')) {
            parent.Fanex.ExtAcc.LoadMainFrame(parent.resolveClientUrl('Customer/CustomerList'), undefined, true);
        }
        var interval = setInterval(function () {
            if (parent.document.getElementById('customerListGrid')) {
                clearInterval(interval);

                parent.Fanex.ExtAcc.InitSuggestion();

                parent.$('#exporttoexcel').unbind('click');
                parent.$('#txtUserName').unbind('keypress');
                parent.$('#txtUserName').unbind('keyup');

                parent.$('#exporttoexcel').click(function () {
                    var data = parent.Fanex.ExtAcc.Customer.GetParams();

                    var params = "groupName=" + data.groupName
                    + "&enabled=" + data.enabled + "&closed=" + data.closed + "&suspended=" + data.suspended
                    + "&sRecommend=" + parent.Fanex.ExtAcc.Customer.superId + "&mRecommend=" + parent.Fanex.ExtAcc.Customer.masterId + "&recommend=" + parent.Fanex.ExtAcc.Customer.agentId + "&custId=" + parent.Fanex.ExtAcc.Customer.custId
                    + "&currentLevel=" + parent.Fanex.ExtAcc.Customer.CurrentLevel + "&username=" + data.username;
                    parent.document.location.href = parent.resolveClientUrl("Customer/GetExportToExcel?") + params;
                });

                parent.$('#txtUserName').keypress(function (e) {
                    if (typeof e == 'undefined') e = window.event;
                    var key = (typeof e.keyCode == 'undefined') ? e.which : e.keyCode;
                    if (key == 13) {
                        parent.Fanex.ExtAcc.Customer.Search(false);

                        return true;
                    } else {
                        return true;
                    }
                });

                parent.$('#txtUserName').keyup(function (e) {
                    if (parent.$('#txtUserName').val().length > 1) {
                        parent.Fanex.ExtAcc.InitSuggestion();
                    }
                });

                var allowDoubleComm = false;
                var syncBingo = false;
                var syncCasino = false;
                var syncLiveCasino = false;
                var isHideKeno = true;
                var isHideVirtualSport = true;
                var isHideGoldDeluxe = true;
                var isHideColossusBets = true;

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

                for (var i = 0; i < $scope.model.setting.details.length; i++) {
                    //if ($scope.model.personalInfo.roleId.value != 4) {
                    if ($scope.model.setting.details[i].key == 'Keno') {
                        isHideKeno = false;
                    }
                    if ($scope.model.setting.details[i].key == 'VirtualSports') {
                        isHideVirtualSport = false;
                    }
                    if ($scope.model.setting.details[i].key == 'GoldDeluxe') {
                        isHideGoldDeluxe = false;
                    }
                    if ($scope.model.setting.details[i].key == 'ColossusBets') {
                        isHideColossusBets = false;
                    }
                    //}
                }


                switch (roleId) {
                    case 4:
                        parent.Fanex.ExtAcc.Customer.GetMasterList(custId, allowDoubleComm, null, null, syncCasino, syncBingo, syncLiveCasino, isHideKeno, isHideVirtualSport, isHideGoldDeluxe, isHideColossusBets);
                        break;
                    case 3:
                        parent.Fanex.ExtAcc.Customer.GetAgentList(custId, allowDoubleComm, null, null, syncCasino, syncBingo, syncLiveCasino, isHideKeno, isHideVirtualSport, isHideGoldDeluxe, isHideColossusBets);
                        break;
                    case 2:
                        parent.Fanex.ExtAcc.Customer.GetMemberList(custId, allowDoubleComm, null, null, syncCasino, syncBingo, syncLiveCasino, isHideKeno, isHideVirtualSport, isHideGoldDeluxe, isHideColossusBets);
                        break;
                    default:
                        break;
                }
            }
        }, 100);
    };

    $scope.GetStatementList = function (custId) {
        parent.Fanex.ExtAcc.LoadMainFrame(parent.resolveClientUrl('Statement/Index?id=' + custId + '&isinternal=' + $scope.model.personalInfo.isInternal.value + '&username=' + $scope.model.personalInfo.currentLevel.value), null, null, null, true);
    };

    $scope.PT = function () {
        parent.Fanex.ExtAcc.Customer.CurrentLevel = $scope.model.personalInfo.roleId.value;
        parent.Fanex.ExtAcc.Customer.UserId = $scope.model.personalInfo.custId.value;
        parent.Fanex.ExtAcc.Customer.superId = parent.Fanex.ExtAcc.Customer.masterId = parent.Fanex.ExtAcc.Customer.agentId = $scope.model.personalInfo.roleId.value == 4 ? userId : $scope.model.personalInfo.uplineId.value;
        parent.Fanex.ExtAcc.Customer.IsMultiple = false;
        parent.Fanex.ExtAcc.Customer.IsFullInfo = true;
        parent.Fanex.ExtAcc.Customer.isSuperEditMember = false;
        parent.Fanex.ExtAcc.Customer.isMasterEditMember = false;
        parent.Fanex.ExtAcc.Customer.isSuperEditAgent = false;

        parent.Fanex.ExtAcc.LoadMainFrame(parent.resolveClientUrl('SportsbookSetting/EditMultiplePT' + "?custId=" + $scope.model.personalInfo.custId.value + "&groupName=" + parent.$('#selectedGroupName').html()), null, null, null, true);
    };

    $scope.Info = function () {
        parent.Fanex.ExtAcc.Customer.UserId = $scope.model.personalInfo.custId.value;

        var url = parent.resolveClientUrl('Customer/EditInfo/?' +
            'custId=' + $scope.model.personalInfo.custId.value +
            "&groupName=" + parent.$('#selectedGroupName').html());
        var title = parent.titles[0] + ': <font color="#6dcff6">' + $scope.model.personalInfo.currentLevel.value + '</font>';
        parent.Fanex.ExtAcc.Form.Show(url, title, 500, 460, false);
    };

    $scope.Transfer = function () {
        parent.Fanex.ExtAcc.Form.Show(parent.resolveClientUrl('Customer/EditTransferCondition/?custId=' + $scope.model.personalInfo.custId.value + "&groupName=" + parent.$('#selectedGroupName').html()), parent.titles[1] + ': <font color="#6dcff6">' + $scope.model.personalInfo.currentLevel.value + '</font>', 600, 250, false, false, 1);
    };

    $scope.Casino = function () {
        parent.Fanex.ExtAcc.Customer.IsMultiple = false;
        parent.Fanex.ExtAcc.Customer.isSuperEditMember = false;
        parent.Fanex.ExtAcc.Customer.isMasterEditMember = false;

        var roleId = $scope.model.personalInfo.roleId.value;
        var custId = $scope.model.personalInfo.custId.value;
        var url = '';

        switch (roleId) {
            case 4:
                url = parent.resolveClientUrl('CustomerSetting/Casino/Super/EditSuper/' + $scope.model.personalInfo.custId.value + '/0');
                break;
            case 3:
                url = parent.resolveClientUrl('CustomerSetting/Casino/Master/EditMaster/' + $scope.model.personalInfo.custId.value + '/' + $scope.model.personalInfo.uplineId.value);
                break;
            case 2:
                url = parent.resolveClientUrl('CustomerSetting/Casino/Agent/EditAgent/' + $scope.model.personalInfo.custId.value + '/' + $scope.model.personalInfo.uplineId.value);
                break;
            case 1:
                url = parent.resolveClientUrl('CustomerSetting/Casino/Member/EditMember/' + $scope.model.personalInfo.custId.value + '/' + $scope.model.personalInfo.uplineId.value);
                break;
            default:
                break;
        }

        if (url != '') {
            parent.Fanex.ExtAcc.LoadMainIFrame(url + '/' + parent.$('#selectedGroupName').html());
        }
    };

    $scope.LiveCasino = function () {
        var roleId = $scope.model.personalInfo.roleId.value;
        var custId = $scope.model.personalInfo.custId.value;
        var uplineId = roleId == 4 ? userId : $scope.model.personalInfo.uplineId.value;
        var url = '';
        var title = $scope.model.personalInfo.currentLevel.value;

        parent.Fanex.ExtAcc.Customer.CurrentLevel = roleId;
        parent.Fanex.ExtAcc.Customer.IsMultiple = false;
        parent.Fanex.ExtAcc.Customer.isSuperEditMember = false;
        parent.Fanex.ExtAcc.Customer.isMasterEditMember = false;
        parent.Fanex.ExtAcc.Customer.UserId = custId;
        parent.Fanex.ExtAcc.Customer.superId = parent.Fanex.ExtAcc.Customer.masterId = parent.Fanex.ExtAcc.Customer.agentId = uplineId;

        switch (roleId) {
            case 4:
                url = parent.resolveClientUrl("LiveCasinoSetting/Setting/EditSetting?uplineId=0&uplineLevel=0&customerId=" + $scope.model.personalInfo.custId.value + "&customerLevel=4&formType=2");
                break;
            case 3:
                url = parent.resolveClientUrl("LiveCasinoSetting/Setting/EditSetting?uplineId=" + $scope.model.personalInfo.uplineId.value + "&parentId=" + $scope.model.personalInfo.uplineId.value + "&uplineLevel=4&customerId=" + $scope.model.personalInfo.custId.value + "&customerLevel=3");
                break;
            case 2:
                url = parent.resolveClientUrl("LiveCasinoSetting/Setting/EditSetting?uplineId=" + $scope.model.personalInfo.uplineId.value + "&parentId=" + $scope.model.personalInfo.uplineId.value + "&uplineLevel=3&customerId=" + $scope.model.personalInfo.custId.value + "&customerLevel=2");
                break;
            case 1:
                url = parent.resolveClientUrl("LiveCasinoSetting/Setting/EditSetting?uplineId=" + $scope.model.personalInfo.uplineId.value + "&parentId=" + $scope.model.personalInfo.uplineId.value + "&uplineLevel=2&customerId=" + $scope.model.personalInfo.custId.value + "&customerLevel=1");
                break;
            default:
                break;
        }

        if (url != '') {
            parent.Fanex.ExtAcc.LoadMainIFrame(url + "&actorId=" + parent.userId + "&actorUsername=" + parent.username + "&groupName=" + parent.$('#selectedGroupName').html(), null, null, null, true);
        }
    };

    $scope.BetSetting = function () {
        var custId = $scope.model.personalInfo.custId.value;
        var param = custId + '/' + custId + '/' + parent.userId + '/' + parent.$('#selectedGroupName').html();
        var url = parent.resolveClientUrl('CustomerSetting/BetSetting/Sportsbook/Index/' + param);

        parent.Fanex.ExtAcc.LoadMainIFrame(url);
    };

    $scope.Racing = function () {
        parent.Fanex.ExtAcc.Customer.IsMultiple = false;
        parent.Fanex.ExtAcc.Customer.isSuperEditMember = false;
        parent.Fanex.ExtAcc.Customer.isMasterEditMember = false;

        var roleId = $scope.model.personalInfo.roleId.value;
        var custId = $scope.model.personalInfo.custId.value;
        var uplineId = roleId == 4 ? userId : $scope.model.personalInfo.uplineId.value;
        var url = '';
        switch (roleId) {
            case 4:
                url = parent.resolveClientUrl('CustomerSetting/Racebook/RacingSetting/Super?id=' + custId);
                break;
            case 3:
                url = parent.resolveClientUrl('CustomerSetting/Racebook/RacingSetting/Master?id=' + custId + '&uplineid=' + uplineId);
                break;
            case 2:
                url = parent.resolveClientUrl('CustomerSetting/Racebook/RacingSetting/Agent?id=' + custId + '&uplineid=' + uplineId);
                break;
            case 1:
                url = parent.resolveClientUrl('CustomerSetting/Racebook/RacingSetting/Member?id=' + custId + '&uplineid=' + uplineId);
                break;
            default:
                break;
        }

        if (url !== '') {
            parent.Fanex.ExtAcc.LoadMainIFrame(url + '&groupName=' + parent.$('#selectedGroupName').html());
        }
    };

    $scope.Bingo = function () {
        parent.Fanex.ExtAcc.Customer.IsMultiple = false;
        parent.Fanex.ExtAcc.Customer.isSuperEditMember = false;
        parent.Fanex.ExtAcc.Customer.isMasterEditMember = false;

        var personalObject = $scope.model.personalInfo;
        var roleId = personalObject.roleId.value;
        var custId = personalObject.custId.value;
        var uplineId = personalObject.uplineRoleId.value > 4 ? custId : personalObject.uplineId.value;
        var url = '';
        switch (roleId) {
            case 4:
                url = parent.resolveClientUrl('CustomerSetting/Bingo/BingoSetting/Super?id=' + custId);
                break;
            case 3:
                url = parent.resolveClientUrl('CustomerSetting/Bingo/BingoSetting/Master?id=' + custId);
                break;
            case 2:
                url = parent.resolveClientUrl('CustomerSetting/Bingo/BingoSetting/Agent?id=' + custId);
                break;
            case 1:
                url = parent.resolveClientUrl('CustomerSetting/Bingo/BingoSetting/Member?id=' + custId);
                break;
            default:
                break;
        }

        if (url !== '' && uplineId > 0) {
            url += '&uplineid=' + uplineId + '&groupName=' + parent.$('#selectedGroupName').html();
            parent.Fanex.ExtAcc.LoadMainIFrame(url);
        }
    };

    $scope.VirtualSports = function () {
        parent.Fanex.ExtAcc.Customer.IsMultiple = false;
        parent.Fanex.ExtAcc.Customer.isSuperEditMember = false;
        parent.Fanex.ExtAcc.Customer.isMasterEditMember = false;

        //var roleId;
        var personalObject = $scope.model.personalInfo;
        var uplineRoleId = personalObject.uplineRoleId.value;
        var uplineId = uplineRoleId > 4 ? personalObject.custId.value : personalObject.uplineId.value;
        var targetCustId = personalObject.custId.value;
        var targetRoleId = personalObject.roleId.value;

        if (uplineId > 0 && uplineRoleId > 0 && targetCustId > 0 && targetRoleId > 0) {
            var url = parent.resolveClientUrl('VirtualSports/EditSingle?custId=' + uplineId + '&roleId=' + uplineRoleId + '&targetCustId=' + targetCustId + '&targetRoleId=' + targetRoleId + "&groupName=" + parent.$('#selectedGroupName').html());
            parent.Fanex.ExtAcc.LoadMainFrame(url, undefined, true, false, true);
        }
    };

    $scope.Keno = function () {
        parent.Fanex.ExtAcc.Customer.IsMultiple = false;
        parent.Fanex.ExtAcc.Customer.isSuperEditMember = false;
        parent.Fanex.ExtAcc.Customer.isMasterEditMember = false;

        var roleId = $scope.model.personalInfo.roleId.value;
        var custId = $scope.model.personalInfo.custId.value;
        var uplineId = roleId == 4 ? userId : $scope.model.personalInfo.uplineId.value;
        var url = '';
        switch (roleId) {
            case 4:
                url = parent.resolveClientUrl('customersetting/KenoSetting/kenosetting/EditSuper?custId=' + custId + "&userId=" + parent.userId);
                break;
            case 3:
                url = parent.resolveClientUrl('customersetting/KenoSetting/kenosetting/EditMaster?custId=' + custId + "&userId=" + parent.userId);
                break;
            case 2:
                url = parent.resolveClientUrl('customersetting/KenoSetting/kenosetting/EditAgent?custId=' + custId + "&userId=" + parent.userId);
                break;
            case 1:
                url = parent.resolveClientUrl('customersetting/KenoSetting/kenosetting/AgentEditMember?custId=' + custId + "&userId=" + parent.userId);
                break;
            default:
                break;
        }

        if (url !== '') {
            parent.Fanex.ExtAcc.LoadMainIFrame(url + '&groupName=' + parent.$('#selectedGroupName').html());
        }
    };

    $scope.GoldDeluxe = function () {
        parent.Fanex.ExtAcc.Customer.IsMultiple = false;
        parent.Fanex.ExtAcc.Customer.isSuperEditMember = false;
        parent.Fanex.ExtAcc.Customer.isMasterEditMember = false;

        var roleId = $scope.model.personalInfo.roleId.value;
        var custId = $scope.model.personalInfo.custId.value;
        var uplineId = roleId == 4 ? userId : $scope.model.personalInfo.uplineId.value;
        var url = '';
        switch (roleId) {
            case 4:
                url = parent.resolveClientUrl("GoldDeluxeSetting/GoldDeluxe/EditSetting?upLineLevel=0&uplineId=0&formType=2&level=4");
                break;
            case 3:
                url = parent.resolveClientUrl("GoldDeluxeSetting/GoldDeluxe/EditSetting?upLineLevel=4&formType=1&level=3" + "&uplineId=" + $scope.model.personalInfo.uplineId.value);
                break;
            case 2:
                url = parent.resolveClientUrl("GoldDeluxeSetting/GoldDeluxe/EditSetting?upLineLevel=3&formType=1&level=2" + "&uplineId=" + $scope.model.personalInfo.uplineId.value);
                break;
            case 1:
                url = parent.resolveClientUrl("GoldDeluxeSetting/GoldDeluxe/EditSetting?upLineLevel=2&formType=1&level=1" + "&uplineId=" + $scope.model.personalInfo.uplineId.value);
                break;
            default:
                break;
        }

        if (url != '') {
            parent.Fanex.ExtAcc.LoadMainIFrame(url + "&actorId=" + parent.userId + "&custId=" + $scope.model.personalInfo.custId.value + "&groupName=" + parent.$('#selectedGroupName').html(), null, null, null, true);
        }
    };

    $scope.ColossusBets = function () {
        parent.Fanex.ExtAcc.Customer.IsMultiple = false;
        parent.Fanex.ExtAcc.Customer.isSuperEditMember = false;
        parent.Fanex.ExtAcc.Customer.isMasterEditMember = false;

        var roleId = $scope.model.personalInfo.roleId.value;
        var custId = $scope.model.personalInfo.custId.value;
        var uplineId = roleId == 4 ? userId : $scope.model.personalInfo.uplineId.value;
        var url = parent.resolveClientUrl('customersetting/ColossusBetSetting/ColossusBetSetting/EditSetting?custid=' + custId + '&isMultipleEdit=0&userId=' + parent.userId + '&roleId=' + roleId);

        switch (roleId) {
            case 4:
                break;
            case 3:
                url += "&srecommend=" + uplineId;
                break;
            case 2:
                url += "&mrecommend=" + uplineId;
                break;
            case 1:
                url += "&recommend=" + uplineId;
                break;
            default:
                break;
        }

        if (url !== '') {
            parent.Fanex.ExtAcc.LoadMainIFrame(url + '&groupName=' + parent.$('#selectedGroupName').html());
        }
    };
};

function PersonalInfoController($scope, $http) {
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
            $('#customerInfo').height(140);
        }
    };

    $scope.Login = function (username) {
        $http({
            url: parent.resolveClientUrl('User/InternalLogin/?username=' + username),
            method: 'post',
            headers: { "X-Requested-With": "XMLHttpRequest" }
        })
        .success(function (result) {
            window.open(result.url, '_blank');
        })
        .error(function (result) {
            if (typeof console != 'undefined') {
                console.log(result);
            }
        });
    };

    $scope.GetBalance = function (custId) {
        parent.Fanex.ExtAcc.BlockUI();
        $http({
            url: parent.resolveClientUrl('CustomerSetting/LeftPanel/SearchCustomer/SelectClearBalance?custId=' + custId + '&groupName=' + parent.$('#selectedGroupName').html()),
            method: 'post',
            headers: { "X-Requested-With": "XMLHttpRequest" }
        })
        .success(function (result) {
            parent.Fanex.ExtAcc.UnBlockUI();
            parent.$('#contentSplitter').html(result);
        })
        .error(function (result) {
            parent.Fanex.ExtAcc.UnBlockUI();
            if (typeof console != 'undefined') {
                console.log(result);
            }
        });
    };

    $scope.ViewUser = function (username) {
        parent.Fanex.ExtAcc.ViewUser(username);
    };
};

function NoInfoController($scope, $http) {
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
};

function LineInfoController($scope, $http) {
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

    $scope.Login = function (username) {
        $http({
            url: parent.resolveClientUrl('User/InternalLogin/?username=' + username),
            method: 'post',
            headers: { "X-Requested-With": "XMLHttpRequest" }
        })
        .success(function (result) {
            window.open(result.url, '_blank');
        })
        .error(function (result) {
            if (typeof console != 'undefined') {
                console.log(result);
            }
        });
    };

    $scope.ViewUser = function (username) {
        parent.Fanex.ExtAcc.ViewUser(username);
    };
};

function ViewInfoController($scope, $http) {
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

    $scope.Login = function (username) {
        $http({
            url: parent.resolveClientUrl('User/InternalLogin/?username=' + username),
            method: 'post',
            headers: { "X-Requested-With": "XMLHttpRequest" }
        })
        .success(function (result) {
            window.open(result.url, '_blank');
        })
        .error(function (result) {
            if (typeof console != 'undefined') {
                console.log(result);
            }
        });
    };

    $scope.GetBalance = function (custId) {
        parent.Fanex.ExtAcc.BlockUI();
        $http({
            url: parent.resolveClientUrl('CustomerSetting/LeftPanel/SearchCustomer/SelectClearBalance?custId=' + custId + '&groupName=' + parent.$('#selectedGroupName').html()),
            method: 'post',
            headers: { "X-Requested-With": "XMLHttpRequest" }
        })
        .success(function (result) {
            parent.Fanex.ExtAcc.UnBlockUI();
            parent.$('#contentSplitter').html(result);
        })
        .error(function (result) {
            parent.Fanex.ExtAcc.UnBlockUI();
            if (typeof console != 'undefined') {
                console.log(result);
            }
        });
    };
};

function GetDownline(custId, pageIndex) {
    parent.Fanex.ExtAcc.BlockUI();

    $.ajax({
        url: parent.resolveClientUrl('CustomerSetting/LeftPanel/SearchCustomer/GetDownlineList?custId=' + custId + '&pageIndex=' + pageIndex),
        type: 'post'
    })
    .success(function (result) {
        model.downlineList = result;
        model.downlineList.downlines.currentPage = pageIndex;

        var users = result.downlines;
        var length = users.length;

        if (length > 0) {
            $('#downlines').empty();
            $('#downlines').show();
            $('#nodownline').hide();
        }
        else {
            $('#downlines').hide();
            $('#nodownline').show();
        }

        for (var i = 0; i < length; i++) {
            var item = '<div class="downlineTitle percent50">' +
                            '<a class="detail2" href="javascript:;" onclick="parent.Fanex.ExtAcc.ViewUser(\'' + users[i].text + '\', 1)">' + users[i].text + '</a>' +
                        '</div>';
            $('#downlines').append(item);
        }

        var pages = model.downlineList.pages;
        if (model.downlineList.pages.length > 1) {
            $('#pages').empty();
            $('#page').show();
            $('#pages').show();

            for (var i = 0; i < pages.length; i++) {
                var page = '<a id="link-' + pages[i].value + '" class="page" href="javascript:;" onclick="GetDownline(\'' + model.personalInfo.custId.value + '\', ' + pages[i].value + ')">' + pages[i].value + '</a>';
                $('#pages').append(page);
            }

            $('#downlines').scrollTop(0);
        }

        $('#downlines').css('height', (pages.length > 1 ? '178px' : '208px'));

        if (pages && pages.length > 1) {
            var timeout = setTimeout(function () {
                clearTimeout(timeout);

                for (var i = 1; i <= pages.length; i++) {
                    $('#link-' + i).removeClass('pageSelected');
                }

                $('#link-' + model.downlineList.downlines.currentPage).addClass('pageSelected');
            }, 100);
        }

        parent.Fanex.ExtAcc.UnBlockUI();
    })
    .error(function (result) {
        parent.Fanex.ExtAcc.UnBlockUI();
        if (typeof console != 'undefined') {
            console.log(result);
        }
    });
};

function BalanceDownlineInfoController($scope, $http) {
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

    $scope.GetDownline = function (custId) {
        if (model.downlineLoaded) return;

        parent.Fanex.ExtAcc.BlockUI();

        $http({ url: parent.resolveClientUrl('CustomerSetting/LeftPanel/SearchCustomer/GetDownlineList?custId=' + custId + '&pageIndex=1'),
            method: 'post',
            headers: { "X-Requested-With": "XMLHttpRequest" }
        })
        .success(function (result) {
            model.downlineLoaded = true;
            model.downlineList = result;
            model.downlineList.downlines.currentPage = 1;

            var users = result.downlines;
            var length = users.length;

            if (length > 0) {
                $('#downlines').empty();
                $('#downlines').show();
                $('#nodownline').hide();
            }
            else {
                $('#downlines').hide();
                $('#nodownline').show();
            }

            for (var i = 0; i < length; i++) {
                var item = '<div class="downlineTitle percent50">' +
                                '<a class="detail2" href="javascript:;" onclick="parent.Fanex.ExtAcc.ViewUser(\'' + users[i].text + '\', 1)">' + users[i].text + '</a>' +
                            '</div>';
                $('#downlines').append(item);
            }

            var pages = model.downlineList.pages;
            if (model.downlineList.pages.length > 1) {
                $('#pages').empty();
                $('#page').show();
                $('#pages').show();

                for (var i = 0; i < pages.length; i++) {
                    var page = '<a id="link-' + pages[i].value + '" class="page" href="javascript:;" onclick="GetDownline(\'' + model.personalInfo.custId.value + '\', ' + pages[i].value + ')">' + pages[i].value + '</a>';
                    $('#pages').append(page);
                }

                $('#downlines').scrollTop(0);
            }

            $('#downlines').css('height', (pages.length > 1 ? '178px' : '208px'));

            if (pages && pages.length > 1) {
                var timeout = setTimeout(function () {
                    clearTimeout(timeout);

                    for (var i = 1; i <= pages.length; i++) {
                        $('#link-' + i).removeClass('pageSelected');
                    }

                    $('#link-1').addClass('pageSelected');
                }, 100);
            }

            parent.Fanex.ExtAcc.UnBlockUI();
        })
        .error(function (result) {
            parent.Fanex.ExtAcc.UnBlockUI();
            if (typeof console != 'undefined') {
                console.log(result);
            }
        });
    };

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

    $scope.Login = function (username) {
        $http({
            url: parent.resolveClientUrl('User/InternalLogin/?username=' + username),
            method: 'post',
            headers: { "X-Requested-With": "XMLHttpRequest" }
        })
        .success(function (result) {
            window.open(result.url, '_blank');
        })
        .error(function (result) {
            if (typeof console != 'undefined') {
                console.log(result);
            }
        });
    };

    $scope.GetBalance = function (custId) {
        parent.Fanex.ExtAcc.BlockUI();
        $http({
            url: parent.resolveClientUrl('CustomerSetting/LeftPanel/SearchCustomer/SelectClearBalance?custId=' + custId + '&groupName=' + parent.$('#selectedGroupName').html()),
            method: 'post',
            headers: { "X-Requested-With": "XMLHttpRequest" }
        })
        .success(function (result) {
            parent.Fanex.ExtAcc.UnBlockUI();
            parent.$('#contentSplitter').html(result);
        })
        .error(function (result) {
            parent.Fanex.ExtAcc.UnBlockUI();
            if (typeof console != 'undefined') {
                console.log(result);
            }
        });
    };
};

function BalanceController($scope, $http) {
    if ($scope.$parent && $scope.$parent.$parent && $scope.$parent.$parent.model) {
        $scope.balanceInfo = $scope.$parent.$parent.model.balanceInfo;
        $scope.model = $scope.balanceInfo;
    }
    else {
        $scope.model = null;
    }

    $scope.EditCredit = function () {
        var value = $('#linkGivenCredit').html();
        $('#linkGivenCredit').hide();
        $('#givenCredit').val(value.replaceAll(',', '').toFixed(2).formatNumber())
        $('#givenCredit').show();
        $('#givenCredit').focus();
    };

    $scope.CreditDetail = function () {
        var id = $scope.personalInfo.custId.value;
        var roleId = $scope.personalInfo.roleId.value;

        parent.Fanex.ExtAcc.BlockUI();

        $http({
            url: parent.resolveClientUrl('CustomerSetting/LeftPanel/SearchCustomer/UsedCreditPage?id=' + id + '&roleId=' + roleId + '&groupName=' + parent.$('#selectedGroupName').html()),
            method: 'post',
            headers: { "X-Requested-With": "XMLHttpRequest" }
        })
        .success(function (result) {
            parent.Fanex.ExtAcc.UnBlockUI();

            parent.$('#contentSplitter').html(result);
        })
        .error(function (data) {
            parent.Fanex.ExtAcc.UnBlockUI();

            if (typeof console != 'undefined') {
                console.log(data);
            }
        })
    };

    $scope.UpdateCredit = function (event) {
        var keyCode = event.keyCode;
        var value = event.target.value;
        var id = $scope.personalInfo.custId.value;
        var uplineId = userId;

        if (keyCode == 13 && value != '') {
            parent.Fanex.ExtAcc.BlockUI();
            value = value.replace(/,/g, "");
            $http({
                url: parent.resolveClientUrl('CustomerSetting/LeftPanel/SearchCustomer/UpdateGivenCreditPost?id=' + id + '&uplineId=' + uplineId + '&newGivenCredit=' + value + '&groupName=' + parent.$('#selectedGroupName').html()),
                method: 'post',
                headers: { "X-Requested-With": "XMLHttpRequest" }
            })
            .success(function (result) {
                parent.Fanex.ExtAcc.UnBlockUI();

                $('#linkGivenCredit').show();
                $('#givenCredit').hide();

                if (result.isSuccess) {
                    $('#linkGivenCredit').html(value.replaceAll(',', '').toFixed(2).formatNumber());
                    $('#givenCredit').val(value);
                    $('#givenCredit').attr('ng-blur', 'RevertCredit("' + value.replaceAll(',', '').toFixed(2).formatNumber() + '")');
                }
                else {
                    $('#givenCredit').val($scope.preVal.replaceAll(',', '').toFixed(2).formatNumber());
                    parent.Fanex.ExtAcc.Form.Alert('Update Credit Error', result.errorMessage.formatErrorMessage(), 400, 200, 'alert');
                }
            })
            .error(function (data) {
                parent.Fanex.ExtAcc.UnBlockUI();

                if (typeof console != 'undefined') {
                    console.log(data);
                }
            });
        }
    };

    $scope.RevertCredit = function (preVal) {
        $('#linkGivenCredit').show();
        $('#givenCredit').val(preVal.replaceAll(',', '').toFixed(2).formatNumber());
        $('#givenCredit').hide();
    };
};

function RevertCredit(preVal) {
    $('#linkGivenCredit').show();
    $('#givenCredit').val(preVal.replaceAll(',', '').toFixed(2).formatNumber());
    $('#givenCredit').hide();
};

function DownlineListController($scope, $http) {
    if ($scope.$parent && $scope.$parent.$parent && $scope.$parent.$parent.model) {
        $scope.downlines = $scope.$parent.$parent.model.downlineList;
        $scope.model = $scope.downlines.downlines;
        $scope.pages = $scope.downlines.pages;
        $scope.maxHeight = '178px';
        $('#downlines').css('height', $scope.maxHeight);
        $('#downlines').css('overflow', 'auto');
    }
    else {
        $scope.model = null;
        $scope.maxHeight = 'auto';
    }

    $scope.Init = function () {
        $('#downlines').css('height', ($scope.pages.length > 1 ? '178px' : '208px'));

        if ($scope.pages && $scope.pages.length > 1) {
            for (var i = 1; i <= $scope.pages.length; i++) {
                $('#link-' + i).removeClass('pageSelected');
            }

            var interval = setInterval(function () {
                if (document.getElementById('link-1')) {
                    clearInterval(interval);

                    $('#link-1').addClass('pageSelected');
                }
            }, 100);
        }
    };

    $scope.ViewUser = function (username, pageIndex) {
        parent.Fanex.ExtAcc.BlockUI();

        $http({ url: parent.resolveClientUrl('CustomerSetting/LeftPanel/SearchCustomer/ViewDownline/' + username + '/' + parent.$('#selectedGroupName').html() + '/' + pageIndex), method: 'post', headers: { "X-Requested-With": "XMLHttpRequest"} })
            .success(function (result) {
                model = result;
                $scope.$parent.$parent.model = model;

                $scope.downlines = $scope.$parent.$parent.model.downlineList;
                $scope.model = $scope.downlines.downlines;
                $scope.pages = $scope.downlines.pages;
                $scope.maxHeight = '178px';

                $('#downlines').scrollTop(0);
                $('#downlines').css('height', ($scope.pages.length > 1 ? $scope.maxHeight : '208px'));

                var timeout = setTimeout(function () {
                    clearTimeout(timeout);

                    for (var i = 1; i <= $scope.pages.length; i++) {
                        $('#link-' + i).removeClass('pageSelected');
                    }

                    $('#link-' + $scope.downlines.currentPage).addClass('pageSelected');
                }, 500);

                parent.Fanex.ExtAcc.UnBlockUI();
            });
    };

    $scope.SearchUser = function (username) {
        parent.Fanex.ExtAcc.ViewUser(username);
    };
};

function ViewUser(username, pageIndex) {
    parent.Fanex.ExtAcc.BlockUI();

    $.ajax({ url: parent.resolveClientUrl('CustomerSetting/LeftPanel/SearchCustomer/ViewDownline/' + username + '/' + parent.$('#selectedGroupName').html() + '/' + pageIndex),
        type: 'post'
    })
            .success(function (result) {
                model = result;

                var pages = model.downlineList.downlines.pages;
                var maxHeight = '178px';

                $('#downlines').scrollTop(0);
                $('#downlines').css('height', (pages.length > 1 ? maxHeight : '208px'));

                var timeout = setTimeout(function () {
                    clearTimeout(timeout);

                    for (var i = 1; i <= pages.length; i++) {
                        $('#link-' + i).removeClass('pageSelected');
                    }

                    $('#link-' + model.downlineList.downlines.currentPage).addClass('pageSelected');
                }, 500);

                parent.Fanex.ExtAcc.UnBlockUI();
            });
};

function SettingController($scope, $http) {
    if ($scope.$parent.model) {
        $scope.setting = $scope.$parent.model.setting;
        $scope.model = $scope.$parent.model;
    }
    else {
        $scope.model = null;
    }

    $scope.GetOnline = function (custId) {
        if (model.onlineLoaded) return;

        parent.Fanex.ExtAcc.BlockUI();

        $http({ url: parent.resolveClientUrl('CustomerSetting/LeftPanel/SearchCustomer/GetOnlineList?custId=' + custId),
            method: 'post',
            headers: { "X-Requested-With": "XMLHttpRequest" }
        })
        .success(function (result) {
            model.onlineLoaded = true;
            model.onlineList.onlines = result;
            var onlines = result.onlines;

            if (onlines.length > 0) {
                $('#onlines').empty();
                $('#onlines').show();
                $('#online').hide();

                for (var i = 0; i < onlines.length; i++) {
                    var online = onlines[i];
                    var item = '<div style="clear: both"><div class="floatLeft percent10">' + (i + 1) + '</div>' +
                                    '<div class="floatLeft percent32">' +
                                        '<a href="javascript:;" onclick="parent.Fanex.ExtAcc.Customer.ShowIpInfo(\'' + online.iP.text + '\')">' +
                                            online.iP.text +
                                        '</a>' +
                                    '</div>' +
                                '<div class="floatRight percent58">' + online.loginTime.text + '</div></div>';
                    $('#onlines').append(item);

                }

                $('#onlines').scrollTop(0);
            }
            else {
                $('#onlines').hide();
                $('#online').show();
            }

            parent.Fanex.ExtAcc.UnBlockUI();
        })
        .error(function (result) {
            if (typeof console != 'undefined') {
                console.log(result);
            }
        })
    };

    $scope.Init = function () {
        if ($scope.setting != null) {
            var groupName = parent.$('#selectedGroupName').html();
            var allowAdmins = eval(parent.allowAdminEditSuperStatus);
            for (var i = 0; i < $scope.setting.details.length; i++) {
                if ($scope.setting.details[i].key == 'Poker' ||
                    ($scope.setting.details[i].key == 'Transfer' && $scope.model.personalInfo.roleId.value == 1) ||
                    ($scope.setting.details[i].key == 'BetSetting' && parent.showBetSetting == '0')) {
                    $scope.setting.details[i].isHide = true;
                }
            }

            for (var i = 0; i < $scope.setting.editItems.length; i++) {
                if ($scope.setting.editItems[i].key == 'EnablePoker' || ($scope.setting.editItems[i].key == 'EnableKeno' && parent.enableKeno == '0')) {
                    $scope.setting.editItems[i].isHide = true;
                }
            }

            var syncCasino = true;
            var syncBingo = true;
            var syncLiveCasino = true;

            for (var j = 0; j < $scope.setting.syncItems.length; j++) {
                if (!$scope.setting.syncItems[j].value && $scope.setting.syncItems[j].key == 'SyncCasino') {
                    syncCasino = false;
                }
                if (!$scope.setting.syncItems[j].value && $scope.setting.syncItems[j].key == 'SyncBingo') {
                    syncBingo = false;
                }
                if (!$scope.setting.syncItems[j].value && $scope.setting.syncItems[j].key == 'SyncLiveCasino') {
                    syncLiveCasino = false;
                }
            }

            for (var i = 0; i < $scope.setting.enableItems.length; i++) {
                if ($scope.model.personalInfo.roleId.value == 4 && ($scope.setting.enableItems[i].key == 'Closed') &&
                    !allowAdmins.contains(parseInt(parent.userId)) && $scope.setting.enableItems[i].value == true) {
                    $scope.setting.enableItems[i].disabled = true;
                    $scope.setting.enableItems[i].tooltip = 'You are not authorized on this function';
                }

                if ($scope.setting.enableItems[i].key == 'EnableCasino')
                    $scope.setting.enableItems[i].isHide = !syncCasino;

                if ($scope.setting.enableItems[i].key == 'EnableBingo')
                    $scope.setting.enableItems[i].isHide = !syncBingo;

                if ($scope.setting.enableItems[i].key == 'EnableLiveCasino')
                    $scope.setting.enableItems[i].isHide = !syncLiveCasino;
            }

            for (var i = 0; i < $scope.setting.masterPts.length; i++) {
                for (var j = 0; j < $scope.setting.syncItems.length; j++) {
                    if ($scope.setting.syncItems[j].key == "Sync" + $scope.setting.masterPts[i].key) {
                        if (!$scope.setting.syncItems[j].value) {
                            $scope.setting.masterPts[i].isHide = true;
                        }
                    }

                    if ($scope.setting.syncItems[j].key == "Sync" + $scope.setting.superPts[i].key) {
                        if (!$scope.setting.syncItems[j].value) {
                            $scope.setting.superPts[i].isHide = true;
                        }
                    }
                }
            }
        }
    };

    $scope.AddNew = function () {
        var roleId = $scope.model.personalInfo.roleId.value;
        var actionName = '';
        parent.Fanex.ExtAcc.Customer.IsMultiple = false;

        switch (roleId) {
            case 2:
                actionName = 'Customer/AgentAddNewMember/?custId=' + $scope.model.personalInfo.custId.value + '&username=' + $scope.model.personalInfo.currentLevel.value + "&groupName=" + parent.$('#selectedGroupName').html();
                break;
            case 3:
                actionName = 'Customer/MasterAddNewAgent/?custId=' + $scope.model.personalInfo.custId.value + '&username=' + $scope.model.personalInfo.currentLevel.value + "&groupName=" + parent.$('#selectedGroupName').html();
                break;
            case 4:
                actionName = 'Customer/SuperAddNewMaster/?custId=' + $scope.model.personalInfo.custId.value + '&username=' + $scope.model.personalInfo.currentLevel.value + "&groupName=" + parent.$('#selectedGroupName').html();
                break;
            default:
                break;
        }

        if (actionName != '') {
            parent.Fanex.ExtAcc.BlockUI();
            parent.Fanex.ExtAcc.LoadMainFrame(parent.resolveClientUrl(actionName), null, null, null, true);
        }
    };

    $scope.DoubleComm = function (allow, id) {
        var roleId = $scope.model.personalInfo.roleId.value;
        var msgMember = allow ? "Are you sure you want to turn OFF Double Commission feature for Sportsbook's Losing tickets on this customer?" : "Are you sure you want to turn ON Double Commission feature for Sportsbook's Losing tickets on this customer?";
        var msg = allow ?
            "Are you sure you want to disallow Double Commission feature for Sportsbook's Losing tickets on this customer? Warning, all members under this agent will be disallowed as well!" :
            "Are you sure you want to allow Double Commission feature for Sportsbook's Losing tickets on this customer?";

        var message = roleId == 1 ? msgMember : msg;
        var titleMember = allow ? "Turn OFF Double Commission" : "Turn ON Double Commission";
        var title = roleId == 1 ? titleMember : allow ? 'Disable Allow Double Commission' : 'Allow Double Commission';
        parent.Fanex.ExtAcc.Form.ConfirmBox(title,
        message,
        450,
        170,
        false,
        function () {
            parent.Fanex.ExtAcc.BlockUI();

            $.ajax({
                url: parent.resolveClientUrl('Customer/ConfigDoubleComm'),
                dataType: 'json',
                type: 'post',
                data: {
                    custid: $scope.model.personalInfo.custId.value,
                    allow: !allow,
                    groupName: parent.$('#selectedGroupName').html()
                }
            })
            .success(function (result) {
                parent.Fanex.ExtAcc.Form.Close();
                if (result.isSuccess) {
                    var timeout = setTimeout(function () {
                        clearTimeout(timeout);
                        parent.Fanex.ExtAcc.UnBlockUI();
                        document.location.reload(true);
                    }, 2000);
                }
                else {
                    parent.Fanex.ExtAcc.UnBlockUI();
                    parent.Fanex.ExtAcc.Form.Alert('Error', result.errorCode == "600" ? "This account was disabled".formatErrorMessage() : result.errorMessage.formatErrorMessage(), 350, 150, 'alert');
                    document.getElementById('DoubleComm').checked = allow;
                }
            })
            .error(function (xhr) {
                parent.Fanex.ExtAcc.UnBlockUI();
                parent.Fanex.ExtAcc.Form.Close();
                document.getElementById(id).checked = allow;
                if (typeof console != 'undefined') {
                    console.log(xhr);
                }
            });
        },
        function () {
            parent.Fanex.ExtAcc.Form.Close();
            document.getElementById(id).checked = allow;
        },
        null,
        null,
        function () {
            document.getElementById(id).checked = allow;
        });
    };

    $scope.Closed = function (close, id) {
        var allowAdmins = eval(parent.allowAdminEditSuperStatus);
        if ($scope.model.personalInfo.roleId.value == 4 && !allowAdmins.contains(parseInt(parent.userId)) && close) {
            return;
        }

        var disableClearCredit = parent.hasClearCredit == 1 ? '' : 'disabled="disabled"';
        var title = disableClearCredit ? 'You are not authorized on this function' : '';
        var checked = !close ? 'checked="checked"' : "";
        var content = '<input name="chkA" type="checkbox" disabled="disabled" ' + checked + ' />Closed' +
            '&nbsp;<input name="chkB" type="checkbox" id="clearCredit" title="' + title + '" ' + disableClearCredit + ' />Clear All Credit';

        if (close) {
            parent.Fanex.ExtAcc.Form.ConfirmBox('Open Customer: <font color="#6dcff6">' + $scope.model.personalInfo.currentLevel.value + '</font>',
                'Are you sure you want to change Closed status for this customer?',
                460,
                150,
                false,
                function () {
                    parent.Fanex.ExtAcc.BlockUI();

                    $.ajax({
                        url: parent.resolveClientUrl('CustomerSetting/LeftPanel/SearchCustomer/UpdateCloseCutomer'),
                        dataType: 'json',
                        type: 'post',
                        data: {
                            custId: $scope.model.personalInfo.custId.value,
                            uplineId: $scope.model.personalInfo.roleId.value == 4 ? 0 : $scope.model.personalInfo.uplineId.value,
                            close: !close,
                            clearCredit: false,
                            custName: $scope.model.personalInfo.currentLevel.value,
                            groupName: parent.$('#selectedGroupName').html()
                        }
                    })
                    .success(function (result) {
                        parent.Fanex.ExtAcc.Form.Close(null, null, 'popupClose');
                        if (result.isSuccess) {
                            var timeout = setTimeout(function () {
                                clearTimeout(timeout);
                                parent.Fanex.ExtAcc.UnBlockUI();
                                document.location.reload(true);
                            }, 2000);
                        }
                        else {
                            parent.Fanex.ExtAcc.UnBlockUI();
                            parent.Fanex.ExtAcc.Form.Close(null, null, 'popupClose');
                            parent.Fanex.ExtAcc.Form.Alert('Error', result.errorCode == "600" ? "This account was disabled".formatErrorMessage() : result.errorMessage.formatErrorMessage(), 350, 150, 'alert');
                            document.getElementById(id).checked = close;
                        }
                    })
                    .error(function (xhr) {
                        parent.Fanex.ExtAcc.UnBlockUI();
                        parent.Fanex.ExtAcc.Form.Close(null, null, 'popupClose');
                        document.getElementById(id).checked = close;
                        if (typeof console != 'undefined') {
                            console.log(xhr);
                        }
                    });
                },
                function () {
                    parent.Fanex.ExtAcc.Form.Close(null, null, 'popupClose');
                    document.getElementById(id).checked = close;
                },
                'popupClose',
                null,
                function () {
                    document.getElementById(id).checked = close;
                });
        }
        else {
            parent.Fanex.ExtAcc.Form.ConfirmBox('Close Customer: <font color="#6dcff6">' + $scope.model.personalInfo.currentLevel.value + '</font>',
            content,
            350,
            150,
            false,
            function () {
                parent.Fanex.ExtAcc.Form.ConfirmBox('Warning!!!',
                    'Are you sure you want to change Close status for this customer? Warning, all downlines will be closed as well!',
                    460,
                    150,
                    false,
                    function () {
                        parent.Fanex.ExtAcc.BlockUI();

                        $.ajax({
                            url: parent.resolveClientUrl('CustomerSetting/LeftPanel/SearchCustomer/UpdateCloseCutomer'),
                            dataType: 'json',
                            type: 'post',
                            data: {
                                custId: $scope.model.personalInfo.custId.value,
                                uplineId: $scope.model.personalInfo.roleId.value == 4 ? 0 : $scope.model.personalInfo.uplineId.value,
                                close: !close,
                                clearCredit: parent.$('#clearCredit').get(0).checked,
                                custName: $scope.model.personalInfo.currentLevel.value,
                                groupName: parent.$('#selectedGroupName').html()
                            }
                        })
                        .success(function (result) {
                            parent.Fanex.ExtAcc.Form.Close(null, null, 'confirmClose');
                            parent.Fanex.ExtAcc.Form.Close(null, null, 'popupClose');
                            if (result.isSuccess) {
                                var timeout = setTimeout(function () {
                                    clearTimeout(timeout);
                                    parent.Fanex.ExtAcc.UnBlockUI();
                                    document.location.reload(true);
                                }, 2000);
                            }
                            else {
                                parent.Fanex.ExtAcc.UnBlockUI();
                                parent.Fanex.ExtAcc.Form.Close(null, null, 'confirmClose');
                                parent.Fanex.ExtAcc.Form.Close(null, null, 'popupClose');
                                parent.Fanex.ExtAcc.Form.Alert('Error', result.errorCode == "600" ? "This account was disabled".formatErrorMessage() : result.errorMessage.formatErrorMessage(), 350, 150, 'alert');
                                document.getElementById(id).checked = close;
                            }
                        })
                        .error(function (xhr) {
                            parent.Fanex.ExtAcc.UnBlockUI();
                            parent.Fanex.ExtAcc.Form.Close(null, null, 'confirmClose');
                            parent.Fanex.ExtAcc.Form.Close(null, null, 'popupClose');
                            document.getElementById(id).checked = close;
                            if (typeof console != 'undefined') {
                                console.log(xhr);
                            }
                        });
                    },
                    null,
                    'confirmClose',
                    null,
                    function () {
                        parent.Fanex.ExtAcc.Form.Close(null, null, 'confirmClose');
                    });
            },
            function () {
                parent.Fanex.ExtAcc.Form.Close(null, null, 'popupClose');
                document.getElementById(id).checked = close;
            },
            'popupClose',
            true,
            function () {
                document.getElementById(id).checked = close;
            },
            true);
        }
    };

    $scope.Suspended = function (suspend, id) {
        var message = '';
        var title = '';
        var roleId = $scope.model.personalInfo.roleId.value;
        var subMessage = roleId == 1 ? 'Warning, this member can\'t place bet!' : 'Warning, all members under this agent can\'t place bet!';

        if (suspend) {
            message = 'Are you sure you want to change Suspended status for this customer? All downlines will be un-suspended as well';
            title = 'Un-suspend Customer: <font color="#6dcff6">' + $scope.model.personalInfo.currentLevel.value + '</font>';
        }
        else {
            message = "Are you sure you want to change Suspended status for this customer? " + subMessage;
            title = 'Suspend Customer: <font color="#6dcff6">' + $scope.model.personalInfo.currentLevel.value + '</font>';
        }

        parent.Fanex.ExtAcc.Form.ConfirmBox(title,
                message,
                480,
                150,
                false,
                function () {
                    parent.Fanex.ExtAcc.BlockUI();

                    $.ajax({
                        url: parent.resolveClientUrl('CustomerSetting/LeftPanel/SearchCustomer/UpdateSuspendCutomer'),
                        dataType: 'json',
                        type: 'post',
                        data: {
                            custId: $scope.model.personalInfo.custId.value,
                            uplineId: $scope.model.personalInfo.roleId.value == 4 ? 0 : $scope.model.personalInfo.uplineId.value,
                            suspend: !suspend,
                            groupName: parent.$('#selectedGroupName').html()
                        }
                    })
                    .success(function (result) {
                        parent.Fanex.ExtAcc.Form.Close(null, null, 'popupSuspend');
                        if (result.isSuccess) {
                            var timeout = setTimeout(function () {
                                clearTimeout(timeout);
                                parent.Fanex.ExtAcc.UnBlockUI();
                                document.location.reload(true);
                            }, 2000);
                        }
                        else {
                            parent.Fanex.ExtAcc.UnBlockUI();
                            parent.Fanex.ExtAcc.Form.Close(null, null, 'popupSuspend');
                            parent.Fanex.ExtAcc.Form.Alert('Error', result.errorCode == "600" ? "This account was disabled".formatErrorMessage() : result.errorMessage.formatErrorMessage(), 350, 150, 'alert');
                            document.getElementById(id).checked = suspend;
                        }
                    })
                    .error(function (xhr) {
                        parent.Fanex.ExtAcc.UnBlockUI();
                        parent.Fanex.ExtAcc.Form.Close(null, null, 'popupSuspend');
                        document.getElementById(id).checked = suspend;
                        if (typeof console != 'undefined') {
                            console.log(xhr);
                        }
                    });
                },
                function () {
                    parent.Fanex.ExtAcc.Form.Close(null, null, 'popupSuspend');
                    document.getElementById(id).checked = suspend;
                },
                'popupSuspend',
                null,
                function () {
                    document.getElementById(id).checked = suspend;
                });
    };

    $scope.Outright = function (allow, id) {
        var roleId = $scope.model.personalInfo.roleId.value;
        var title = '';
        var subtitle = roleId == 1 ? 'Warning, this member can\'t place Outright bet!' : 'Warning, all members under this agent can\'t place Outright bet!';
        var message = allow ? 'Are you sure you want to change Outright status for this customer? ' + subtitle : 'Are you sure you want to change Outright status for this customer?';
        title = allow ? ('Disallow Outright: <font color="#6dcff6">' + $scope.model.personalInfo.currentLevel.value + '</font>') : ('Allow Outright: <font color="#6dcff6">' + $scope.model.personalInfo.currentLevel.value + '</font>');
        parent.Fanex.ExtAcc.Form.ConfirmBox(title,
        message,
        480,
        150,
        false,
        function () {
            parent.Fanex.ExtAcc.BlockUI();

            $.ajax({
                url: parent.resolveClientUrl('CustomerSetting/LeftPanel/SearchCustomer/UpdateOutright'),
                dataType: 'json',
                type: 'post',
                data: {
                    custId: $scope.model.personalInfo.custId.value,
                    allow: !allow,
                    groupName: parent.$('#selectedGroupName').html()
                }
            })
            .success(function (result) {
                parent.Fanex.ExtAcc.Form.Close(null, null, 'popupOutright');
                if (result.isSuccess) {
                    var timeout = setTimeout(function () {
                        clearTimeout(timeout);
                        parent.Fanex.ExtAcc.UnBlockUI();
                        document.location.reload(true);
                    }, 2000);
                }
                else {
                    parent.Fanex.ExtAcc.UnBlockUI();
                    parent.Fanex.ExtAcc.Form.Close(null, null, 'popupOutright');
                    parent.Fanex.ExtAcc.Form.Alert('Error', result.errorCode == "600" ? "This account was disabled".formatErrorMessage() : result.errorMessage.formatErrorMessage(), 350, 150, 'alert');
                    document.getElementById(id).checked = allow;
                }
            })
            .error(function (xhr) {
                parent.Fanex.ExtAcc.UnBlockUI();
                parent.Fanex.ExtAcc.Form.Close(null, null, 'popupOutright');
                document.getElementById(id).checked = allow;
                if (typeof console != 'undefined') {
                    console.log(xhr);
                }
            });
        },
        function () {
            parent.Fanex.ExtAcc.Form.Close(null, null, 'popupOutright');
            document.getElementById(id).checked = allow;
        },
        'popupOutright',
        null,
        function () {
            document.getElementById(id).checked = allow;
        });
    };

    $scope.AutoPT = function (allow, id) {
        var message = 'Are you sure you want to change Auto Master PT status for this customer?';
        var title = allow ? ('Disallow Auto Master PT: <font color="#6dcff6">' + $scope.model.personalInfo.currentLevel.value + '</font>') : ('Allow Auto Master PT: <font color="#6dcff6">' + $scope.model.personalInfo.currentLevel.value + '</font>');

        parent.Fanex.ExtAcc.Form.ConfirmBox(title,
            message,
            510,
            150,
            false,
            function () {
                parent.Fanex.ExtAcc.BlockUI();

                $.ajax({
                    url: parent.resolveClientUrl('CustomerSetting/LeftPanel/SearchCustomer/UpdateAutoMPT'),
                    dataType: 'json',
                    type: 'post',
                    data: {
                        custId: $scope.model.personalInfo.custId.value,
                        allow: !allow,
                        uplineId: $scope.model.personalInfo.roleId.value == 4 ? 0 : $scope.model.personalInfo.uplineId.value,
                        groupName: parent.$('#selectedGroupName').html()
                    }
                })
                .success(function (result) {
                    parent.Fanex.ExtAcc.Form.Close(null, null, 'popupAutoPT');
                    if (result.isSuccess) {
                        var timeout = setTimeout(function () {
                            clearTimeout(timeout);
                            parent.Fanex.ExtAcc.UnBlockUI();
                            document.location.reload(true);
                        }, 2000);
                    }
                    else {
                        parent.Fanex.ExtAcc.UnBlockUI();
                        parent.Fanex.ExtAcc.Form.Close(null, null, 'popupAutoPT');
                        parent.Fanex.ExtAcc.Form.Alert('Error', result.errorCode == "600" ? "This account was disabled".formatErrorMessage() : result.errorMessage.formatErrorMessage(), 350, 150, 'alert');
                        document.getElementById(id).checked = allow;
                    }
                })
                .error(function (xhr) {
                    parent.Fanex.ExtAcc.UnBlockUI();
                    parent.Fanex.ExtAcc.Form.Close(null, null, 'popupAutoPT');
                    document.getElementById(id).checked = allow;
                    if (typeof console != 'undefined') {
                        console.log(xhr);
                    }
                });
            },
            function () {
                parent.Fanex.ExtAcc.Form.Close(null, null, 'popupAutoPT');
                document.getElementById(id).checked = allow;
            },
            'popupAutoPT',
            null,
            function () {
                document.getElementById(id).checked = allow;
            });
    };

    $scope.EnableCasino = function (enable, id) {
        var url_SELECT = 'customersetting/enablestatus/productstatus?';
        var query = 'custid=' + $scope.model.personalInfo.custId.value + '&uplineid=' + ($scope.model.personalInfo.roleId.value != 4 ? $scope.model.personalInfo.uplineId.value : 0) +
        '&userid=' + parent.userId + '&productid=casino&level=' + $scope.model.personalInfo.roleId.value +
        '&enable=' + !enable + '&enabledownlines=false&hasdownline=' + ($scope.model.hasDownline ? 'true' : 'false') + '&username=' + $scope.model.personalInfo.currentLevel.value + '&enableuplines=true&synced=' + $scope.model.setting.syncItems[0].value + '&isCasino=true';
        var url = parent.resolveClientUrl(url_SELECT + query);
        parent.Fanex.ExtAcc.Form.ShowFrame(url, (enable ? 'Disable ' : 'Enable ') + 'Casino: <font color="#6dcff6">' + $scope.model.personalInfo.currentLevel.value + '</font>', 400, 150, false, function () {
            document.getElementById(id).checked = enable;
        }, 1, null);
    };

    $scope.EnableBingo = function (enable, id) {
        var url_SELECT = 'customersetting/enablestatus/productstatus?';
        var query = 'custid=' + $scope.model.personalInfo.custId.value + '&uplineid=' + ($scope.model.personalInfo.roleId.value != 4 ? $scope.model.personalInfo.uplineId.value : 0) +
        '&userid=' + parent.userId + '&productid=bingo&level=' + $scope.model.personalInfo.roleId.value +
        '&enable=' + !enable + '&enabledownlines=false&hasdownline=' + ($scope.model.hasDownline ? 'true' : 'false') + '&username=' + $scope.model.personalInfo.currentLevel.value + '&enableuplines=true&synced=' + $scope.model.setting.syncItems[1].value;
        var url = parent.resolveClientUrl(url_SELECT + query);
        parent.Fanex.ExtAcc.Form.ShowFrame(url, (enable ? 'Disable ' : 'Enable ') + 'Bingo: <font color="#6dcff6">' + $scope.model.personalInfo.currentLevel.value + '</font>', 400, 150, false, function () {
            document.getElementById(id).checked = enable;
        }, 1, null);
    };

    $scope.EnableNumberGame = function (enable, id) {
        var url_SELECT = 'customersetting/enablestatus/productstatus?';
        var query = 'custid=' + $scope.model.personalInfo.custId.value + '&uplineid=' + ($scope.model.personalInfo.roleId.value != 4 ? $scope.model.personalInfo.uplineId.value : 0) +
        '&userid=' + parent.userId + '&productid=numbergame&level=' + $scope.model.personalInfo.roleId.value +
        '&enable=' + !enable + '&enabledownlines=false&hasdownline=' + ($scope.model.hasDownline ? 'true' : 'false') + '&username=' + $scope.model.personalInfo.currentLevel.value + '&enableuplines=true';
        var url = parent.resolveClientUrl(url_SELECT + query);
        parent.Fanex.ExtAcc.Form.ShowFrame(url, (enable ? 'Disable ' : 'Enable ') + 'Number Game: <font color="#6dcff6">' + $scope.model.personalInfo.currentLevel.value + '</font>', 400, 150, false, function () {
            document.getElementById(id).checked = enable;
        }, 1, null);
    };

    $scope.EnableLiveCasino = function (enable, id) {
        var url_SELECT = 'customersetting/enablestatus/productstatus?';
        var query = 'custid=' + $scope.model.personalInfo.custId.value + '&uplineid=' + ($scope.model.personalInfo.roleId.value != 4 ? $scope.model.personalInfo.uplineId.value : 0) +
        '&userid=' + parent.userId + '&productid=livecasino&level=' + $scope.model.personalInfo.roleId.value +
        '&enable=' + !enable + '&enabledownlines=false&hasdownline=' + ($scope.model.hasDownline ? 'true' : 'false') + '&username=' + $scope.model.personalInfo.currentLevel.value + '&enableuplines=true';
        var url = parent.resolveClientUrl(url_SELECT + query);
        parent.Fanex.ExtAcc.Form.ShowFrame(url, (enable ? 'Disable ' : 'Enable ') + 'Live Casino: <font color="#6dcff6">' + $scope.model.personalInfo.currentLevel.value + '</font>', 400, 150, false, function () {
            document.getElementById(id).checked = enable;
        }, 1, null);
    };

    $scope.EnableVirtualSports = function (enable, id) {
        var url_SELECT = 'customersetting/enablestatus/productstatus?';
        var query = 'custid=' + $scope.model.personalInfo.custId.value + '&uplineid=' + ($scope.model.personalInfo.roleId.value != 4 ? $scope.model.personalInfo.uplineId.value : 0) +
        '&userid=' + parent.userId + '&productid=virtualsports&level=' + $scope.model.personalInfo.roleId.value +
        '&enable=' + !enable + '&enabledownlines=false&hasdownline=' + ($scope.model.hasDownline ? 'true' : 'false') + '&username=' + $scope.model.personalInfo.currentLevel.value + '&enableuplines=true';
        var url = parent.resolveClientUrl(url_SELECT + query);
        parent.Fanex.ExtAcc.Form.ShowFrame(url, (enable ? 'Disable ' : 'Enable ') + 'Virtual Sports: <font color="#6dcff6">' + $scope.model.personalInfo.currentLevel.value + '</font>', 400, 150, false, function () {
            document.getElementById(id).checked = enable;
        }, 1, null);
    };

    $scope.EnableESports = function (enable, id) {
        var url_SELECT = 'customersetting/enablestatus/productstatus?';
        var query = 'custid=' + $scope.model.personalInfo.custId.value + '&uplineid=' + ($scope.model.personalInfo.roleId.value != 4 ? $scope.model.personalInfo.uplineId.value : 0) +
        '&userid=' + parent.userId + '&productid=esports&level=' + $scope.model.personalInfo.roleId.value +
        '&enable=' + !enable + '&enabledownlines=false&hasdownline=' + ($scope.model.hasDownline ? 'true' : 'false') + '&username=' + $scope.model.personalInfo.currentLevel.value + '&enableuplines=true';
        var url = parent.resolveClientUrl(url_SELECT + query);
        parent.Fanex.ExtAcc.Form.ShowFrame(url, (enable ? 'Disable ' : 'Enable ') + 'eSports: <font color="#6dcff6">' + $scope.model.personalInfo.currentLevel.value + '</font>', 400, 150, false, function () {
            document.getElementById(id).checked = enable;
        }, 1, null);
    };

    $scope.EnableHRFixedOdds = function (enable, id) {
        var url_SELECT = 'customersetting/enablestatus/productstatus?';
        var query = 'custid=' + $scope.model.personalInfo.custId.value + '&uplineid=' + ($scope.model.personalInfo.roleId.value != 4 ? $scope.model.personalInfo.uplineId.value : 0) +
        '&userid=' + parent.userId + '&productid=hrfixedodds&level=' + $scope.model.personalInfo.roleId.value +
        '&enable=' + !enable + '&enabledownlines=false&hasdownline=' + ($scope.model.hasDownline ? 'true' : 'false') + '&username=' + $scope.model.personalInfo.currentLevel.value + '&enableuplines=true';
        var url = parent.resolveClientUrl(url_SELECT + query);
        parent.Fanex.ExtAcc.Form.ShowFrame(url, (enable ? 'Disable ' : 'Enable ') + 'HR Fixed Odds: <font color="#6dcff6">' + $scope.model.personalInfo.currentLevel.value + '</font>', 400, 150, false, function () {
            document.getElementById(id).checked = enable;
        }, 1, null);
    };

    $scope.EnableRacing = function () {
        var url = parent.resolveClientUrl('CustomerSetting/LeftPanel/SearchCustomer/GetEnableRacing?id=' + $scope.model.personalInfo.custId.value +
            '&uplineId=' + ($scope.model.personalInfo.roleId.value == 4 ? 0 : $scope.model.personalInfo.uplineId.value) +
            '&roleId=' + $scope.model.personalInfo.roleId.value + '&groupName=' + parent.$('#selectedGroupName').html());
        var title = 'Update Racing Status: <font color="#6dcff6">' + $scope.model.personalInfo.currentLevel.value + '</font>';
        parent.Fanex.ExtAcc.Form.ShowFrame(url, title, 400, 350, false, null, 1, 'enableRacing');
    };

    $scope.EnableKeno = function () {
        var url_SELECT = 'customersetting/enablestatus/productstatus?';
        var query = 'custid=' + $scope.model.personalInfo.custId.value + '&uplineid=' + ($scope.model.personalInfo.roleId.value != 4 ? $scope.model.personalInfo.uplineId.value : 0) +
            '&userid=' + parent.userId + '&productid=keno&level=' + $scope.model.personalInfo.roleId.value +
            '&enable=true&enabledownlines=true&hasdownline=' + ($scope.model.hasDownline ? 'true' : 'false') + '&username=' + $scope.model.personalInfo.currentLevel.value + '&enableuplines=true';
        var url = parent.resolveClientUrl(url_SELECT + query);
        parent.Fanex.ExtAcc.Form.ShowFrame(url, 'Enable Keno: <font color="#6dcff6">' + $scope.model.personalInfo.currentLevel.value + '</font>', 400, 420, false, null, 1, 'enableGD');

    };

    $scope.EnableGoldDeluxe = function (enable, id) {
        var url_SELECT = 'customersetting/enablestatus/productstatus?';
        var query = 'custid=' + $scope.model.personalInfo.custId.value + '&uplineid=' + ($scope.model.personalInfo.roleId.value != 4 ? $scope.model.personalInfo.uplineId.value : 0) +
            '&userid=' + parent.userId + '&productid=golddeluxe&level=' + $scope.model.personalInfo.roleId.value +
            '&enable=' + !enable + '&enabledownlines=false&hasdownline=' + ($scope.model.hasDownline ? 'true' : 'false') + '&username=' + $scope.model.personalInfo.currentLevel.value + '&enableuplines=true';
        var url = parent.resolveClientUrl(url_SELECT + query);
        parent.Fanex.ExtAcc.Form.ShowFrame(url, (enable ? 'Disable ' : 'Enable ') + 'Gold Deluxe: <font color="#6dcff6">' + $scope.model.personalInfo.currentLevel.value + '</font>', 400, 150, false, function () {
            document.getElementById(id).checked = enable;
        }, 1, null);
    };

    $scope.EnableColossusBets = function (enable, id) {
        var url_SELECT = 'customersetting/enablestatus/productstatus?';
        var query = 'custid=' + $scope.model.personalInfo.custId.value + '&uplineid=' + ($scope.model.personalInfo.roleId.value != 4 ? $scope.model.personalInfo.uplineId.value : 0) +
            '&userid=' + parent.userId + '&productid=colossusbet&level=' + $scope.model.personalInfo.roleId.value +
            '&enable=' + !enable + '&enabledownlines=false&hasdownline=' + ($scope.model.hasDownline ? 'true' : 'false') + '&username=' + $scope.model.personalInfo.currentLevel.value + '&enableuplines=true';
        var url = parent.resolveClientUrl(url_SELECT + query);
        parent.Fanex.ExtAcc.Form.ShowFrame(url, (enable ? 'Disable ' : 'Enable ') + 'Colossus Bets: <font color="#6dcff6">' + $scope.model.personalInfo.currentLevel.value + '</font>', 400, 150, false, function () {
            document.getElementById(id).checked = enable;
        }, 1, null);
    };

    $scope.ViewSubTab = function (id) {
        switch (id) {
            case 'sub-edit':
                if (!$('#sub-edit').hasClass('subTab-active'))
                    $('#sub-edit').addClass('subTab-active');

                $('#sub-min-max').removeClass('subTab-active');
                $('#sub-super-pt').removeClass('subTab-active');
                $('#sub-master-pt').removeClass('subTab-active');

                $('#edit').show();
                $('#minMax').hide();
                $('#superPT').hide();
                $('#masterPT').hide();

                break;
            case 'sub-min-max':
                if (!$('#sub-min-max').hasClass('subTab-active'))
                    $('#sub-min-max').addClass('subTab-active');

                $('#sub-edit').removeClass('subTab-active');
                $('#sub-super-pt').removeClass('subTab-active');
                $('#sub-master-pt').removeClass('subTab-active');

                $('#edit').hide();
                $('#minMax').show();
                $('#superPT').hide();
                $('#masterPT').hide();

                break;
            case 'sub-super-pt':
                if (!$('#sub-super-pt').hasClass('subTab-active'))
                    $('#sub-super-pt').addClass('subTab-active');

                $('#sub-min-max').removeClass('subTab-active');
                $('#sub-edit').removeClass('subTab-active');
                $('#sub-master-pt').removeClass('subTab-active');

                $('#edit').hide();
                $('#minMax').hide();
                $('#superPT').show();
                $('#masterPT').hide();

                break;
            case 'sub-master-pt':
                if (!$('#sub-master-pt').hasClass('subTab-active'))
                    $('#sub-master-pt').addClass('subTab-active');

                $('#sub-min-max').removeClass('subTab-active');
                $('#sub-super-pt').removeClass('subTab-active');
                $('#sub-edit').removeClass('subTab-active');

                $('#edit').hide();
                $('#minMax').hide();
                $('#superPT').hide();
                $('#masterPT').show();

                break;
            default:
                break;
        }
    };

    $scope.MinSB = function () {
        parent.Fanex.ExtAcc.Customer.CurrentLevel = $scope.model.personalInfo.roleId.value;
        parent.Fanex.ExtAcc.Customer.UserId = $scope.model.personalInfo.custId.value;
        parent.Fanex.ExtAcc.Customer.superId = parent.Fanex.ExtAcc.Customer.masterId = $scope.model.personalInfo.uplineId.value;
        parent.Fanex.ExtAcc.LoadMainFrame(parent.resolveClientUrl('SportsbookSetting/EditMinPT' + "?custId=" + $scope.model.personalInfo.custId.value + "&groupName=" + parent.$('#selectedGroupName').html()), null, null, null, true);
    };

    $scope.MinRB = function () {
        var typeUrl = '';
        var roleId = $scope.model.personalInfo.roleId.value;
        if (roleId === 3) { // master
            typeUrl = 'MasterMinMaxPT';
        }
        else if (roleId === 2) { // agent
            typeUrl = 'AgentMinMaxPT';
        }

        if (typeUrl !== '') {
            var url = 'CustomerSetting/Racebook/RacingSetting/' + typeUrl + '?id=' + $scope.model.personalInfo.custId.value + '&roleId=' + roleId;
            parent.Fanex.ExtAcc.LoadMainIFrame(parent.resolveClientUrl(url + '&groupName=' + parent.$('#selectedGroupName').html()));
        }
    };

    $scope.SuperPT = function () {
        var superId = $scope.model.personalInfo.downlines[0].key;
        var masterId = $scope.model.personalInfo.downlines[1].key;
        var agentId = $scope.model.personalInfo.downlines[2].key;

        parent.Fanex.ExtAcc.Customer.CurrentLevel = 1;
        parent.Fanex.ExtAcc.Customer.isSuperEditMember = true;
        parent.Fanex.ExtAcc.Customer.isMasterEditMember = false;
        parent.Fanex.ExtAcc.Customer.IsMultiple = false;
        parent.Fanex.ExtAcc.Customer.IsFullInfo = true;
        parent.Fanex.ExtAcc.Customer.UserId = $scope.model.personalInfo.custId.value;
        parent.Fanex.ExtAcc.Customer.agentId = agentId;
        parent.Fanex.ExtAcc.Customer.masterId = masterId;
        parent.Fanex.ExtAcc.Customer.superId = superId;
        parent.Fanex.ExtAcc.LoadMainFrame(parent.resolveClientUrl('SportsbookSetting/SuperMasterEditSB?custId=' + $scope.model.personalInfo.custId.value + "&groupName=" + parent.$('#selectedGroupName').html()), null, null, null, true);
    };

    $scope.SuperBingo = function () {
        parent.Fanex.ExtAcc.Customer.IsMultiple = false;
        parent.Fanex.ExtAcc.Customer.isSuperEditMember = true;
        parent.Fanex.ExtAcc.Customer.isMasterEditMember = false;
        var superId = $scope.model.personalInfo.downlines[0].key;
        parent.Fanex.ExtAcc.LoadMainIFrame(parent.resolveClientUrl('CustomerSetting/Bingo/BingoSetting/SuperEditMember?id=' + $scope.model.personalInfo.custId.value + '&uplineid=' + superId + '&groupName=' + parent.$('#selectedGroupName').html()));
    };

    $scope.SuperCasino = function () {
        parent.Fanex.ExtAcc.Customer.IsMultiple = false;
        parent.Fanex.ExtAcc.Customer.isSuperEditMember = true;
        parent.Fanex.ExtAcc.Customer.isMasterEditMember = false;
        var superId = $scope.model.personalInfo.downlines[0].key;
        parent.Fanex.ExtAcc.LoadMainIFrame(parent.resolveClientUrl('CustomerSetting/Casino/Member/EditMemberBySuper/' + $scope.model.personalInfo.custId.value + '/' + superId + '/' + parent.$('#selectedGroupName').html()));
    };

    $scope.SuperRacing = function () {
        parent.Fanex.ExtAcc.Customer.IsMultiple = false;
        parent.Fanex.ExtAcc.Customer.isSuperEditMember = true;
        parent.Fanex.ExtAcc.Customer.isMasterEditMember = false;
        var superId = $scope.model.personalInfo.downlines[0].key;
        parent.Fanex.ExtAcc.LoadMainIFrame(parent.resolveClientUrl('customersetting/racebook/racingsetting/supereditmember?id=' + $scope.model.personalInfo.custId.value + '&uplineid=' + superId + '&groupName=' + parent.$('#selectedGroupName').html()));
    };

    $scope.SuperLiveCasino = function () {
        var superId = $scope.model.personalInfo.downlines[0].key;
        parent.Fanex.ExtAcc.Customer.CurrentLevel = 1;
        parent.Fanex.ExtAcc.Customer.isSuperEditMember = true;
        parent.Fanex.ExtAcc.Customer.isMasterEditMember = false;
        parent.Fanex.ExtAcc.Customer.IsMultiple = false;
        parent.Fanex.ExtAcc.Customer.UserId = $scope.model.personalInfo.custId.value;
        parent.Fanex.ExtAcc.Customer.superId = superId;
        var url = "LiveCasinoSetting/Setting/EditSetting?uplineId=" + superId + "&parentId=" + $scope.model.personalInfo.uplineId.value + "&uplineLevel=4&customerId=" + $scope.model.personalInfo.custId.value + "&customerLevel=1";
        parent.Fanex.ExtAcc.LoadMainIFrame(parent.resolveClientUrl(url + "&actorId=" + parent.userId + "&actorUsername=" + parent.username + "&groupName=" + parent.$('#selectedGroupName').html()), null, null, null, true);
    };

    $scope.SuperVirtualSports = function () {
        parent.Fanex.ExtAcc.Customer.IsMultiple = false;
        parent.Fanex.ExtAcc.Customer.isSuperEditMember = true;
        parent.Fanex.ExtAcc.Customer.isMasterEditMember = false;
        var superId = $scope.model.personalInfo.downlines[0].key;
        parent.Fanex.ExtAcc.LoadMainFrame(parent.resolveClientUrl('VirtualSports/EditSingle?custId=' + superId + '&roleId=4&targetCustId=' + $scope.model.personalInfo.custId.value + '&targetRoleId=1&groupName=' + parent.$('#selectedGroupName').html()), null, null, null, true);
    };

    $scope.SuperKeno = function () {
        var superId = $scope.model.personalInfo.downlines[0].key;
        parent.Fanex.ExtAcc.Customer.CurrentLevel = 1;
        parent.Fanex.ExtAcc.Customer.isSuperEditMember = true;
        parent.Fanex.ExtAcc.Customer.isMasterEditMember = false;
        parent.Fanex.ExtAcc.Customer.IsMultiple = false;
        parent.Fanex.ExtAcc.Customer.UserId = $scope.model.personalInfo.custId.value;
        parent.Fanex.ExtAcc.Customer.superId = superId;
        parent.Fanex.ExtAcc.LoadMainIFrame(parent.resolveClientUrl('customersetting/KenoSetting/kenosetting/SuperEditMember?custId=' + $scope.model.personalInfo.custId.value + "&userId=" + parent.userId));
    };

    $scope.SuperGoldDeluxe = function () {
        var superId = $scope.model.personalInfo.downlines[0].key;
        parent.Fanex.ExtAcc.Customer.CurrentLevel = 1;
        parent.Fanex.ExtAcc.Customer.isSuperEditMember = true;
        parent.Fanex.ExtAcc.Customer.isMasterEditMember = false;
        parent.Fanex.ExtAcc.Customer.IsMultiple = false;
        parent.Fanex.ExtAcc.Customer.UserId = $scope.model.personalInfo.custId.value;
        parent.Fanex.ExtAcc.Customer.superId = superId;

        parent.Fanex.ExtAcc.LoadMainIFrame(parent.resolveClientUrl('GoldDeluxeSetting/GoldDeluxe/EditSetting?upLineLevel=4&formType=1&uplineId=' + superId + '&level=1&custId=' + $scope.model.personalInfo.custId.value + "&actorId=" + parent.userId + "&groupName=" + parent.$('#selectedGroupName').html()));
    };

    $scope.MasterPT = function () {
        var superId = $scope.model.personalInfo.downlines[0].key;
        var masterId = $scope.model.personalInfo.downlines[1].key;
        var agentId = $scope.model.personalInfo.downlines[2].key;

        parent.Fanex.ExtAcc.Customer.CurrentLevel = 1;
        parent.Fanex.ExtAcc.Customer.isSuperEditMember = false;
        parent.Fanex.ExtAcc.Customer.isMasterEditMember = true;
        parent.Fanex.ExtAcc.Customer.IsMultiple = false;
        parent.Fanex.ExtAcc.Customer.IsFullInfo = true;
        parent.Fanex.ExtAcc.Customer.UserId = $scope.model.personalInfo.custId.value;
        parent.Fanex.ExtAcc.Customer.agentId = agentId;
        parent.Fanex.ExtAcc.Customer.masterId = masterId;
        parent.Fanex.ExtAcc.Customer.superId = superId;
        parent.Fanex.ExtAcc.LoadMainFrame(parent.resolveClientUrl('SportsbookSetting/SuperMasterEditSB?custId=' + $scope.model.personalInfo.custId.value + "&groupName=" + parent.$('#selectedGroupName').html()), null, null, null, true);
    };

    $scope.MasterBingo = function () {
        parent.Fanex.ExtAcc.Customer.IsMultiple = false;
        parent.Fanex.ExtAcc.Customer.isSuperEditMember = false;
        parent.Fanex.ExtAcc.Customer.isMasterEditMember = true;
        var masterId = $scope.model.personalInfo.downlines[1].key;
        parent.Fanex.ExtAcc.LoadMainIFrame(parent.resolveClientUrl('CustomerSetting/Bingo/BingoSetting/MasterEditMember?id=' + $scope.model.personalInfo.custId.value + '&uplineid=' + masterId + '&groupName=' + parent.$('#selectedGroupName').html()));
    };

    $scope.MasterCasino = function () {
        parent.Fanex.ExtAcc.Customer.IsMultiple = false;
        parent.Fanex.ExtAcc.Customer.isSuperEditMember = false;
        parent.Fanex.ExtAcc.Customer.isMasterEditMember = true;
        var masterId = $scope.model.personalInfo.downlines[1].key;
        parent.Fanex.ExtAcc.LoadMainIFrame(parent.resolveClientUrl('CustomerSetting/Casino/Member/EditMemberByMaster/' + $scope.model.personalInfo.custId.value + '/' + masterId + '/' + parent.$('#selectedGroupName').html()));
    };

    $scope.MasterRacing = function () {
        parent.Fanex.ExtAcc.Customer.IsMultiple = false;
        parent.Fanex.ExtAcc.Customer.isSuperEditMember = false;
        parent.Fanex.ExtAcc.Customer.isMasterEditMember = true;
        var masterId = $scope.model.personalInfo.downlines[1].key;
        parent.Fanex.ExtAcc.LoadMainIFrame(parent.resolveClientUrl('customersetting/racebook/racingsetting/mastereditmember?id=' + $scope.model.personalInfo.custId.value + '&uplineid=' + masterId + '&groupName=' + parent.$('#selectedGroupName').html()));
    };

    $scope.MasterLiveCasino = function () {
        var masterId = $scope.model.personalInfo.downlines[1].key;
        parent.Fanex.ExtAcc.Customer.CurrentLevel = 1;
        parent.Fanex.ExtAcc.Customer.isSuperEditMember = false;
        parent.Fanex.ExtAcc.Customer.isMasterEditMember = true;
        parent.Fanex.ExtAcc.Customer.IsMultiple = false;
        parent.Fanex.ExtAcc.Customer.UserId = $scope.model.personalInfo.custId.value;
        parent.Fanex.ExtAcc.Customer.masterId = masterId;
        var url = "LiveCasinoSetting/Setting/EditSetting?uplineId=" + masterId + "&parentId=" + $scope.model.personalInfo.uplineId.value + "&uplineLevel=3&customerId=" + $scope.model.personalInfo.custId.value + "&customerLevel=1";
        parent.Fanex.ExtAcc.LoadMainIFrame(parent.resolveClientUrl(url + "&actorId=" + parent.userId + "&actorUsername=" + parent.username + "&groupName=" + parent.$('#selectedGroupName').html()), null, null, null, true);
    };

    $scope.MasterVirtualSports = function () {
        parent.Fanex.ExtAcc.Customer.IsMultiple = false;
        parent.Fanex.ExtAcc.Customer.isSuperEditMember = false;
        parent.Fanex.ExtAcc.Customer.isMasterEditMember = true;
        var masterId = $scope.model.personalInfo.downlines[1].key;
        parent.Fanex.ExtAcc.LoadMainFrame(parent.resolveClientUrl('VirtualSports/EditSingle?custId=' + masterId + '&roleId=3&targetCustId=' + $scope.model.personalInfo.custId.value + '&targetRoleId=1&groupName=' + parent.$('#selectedGroupName').html()), null, null, null, true);
    };

    $scope.MasterKeno = function () {
        parent.Fanex.ExtAcc.Customer.IsMultiple = false;
        parent.Fanex.ExtAcc.Customer.isSuperEditMember = false;
        parent.Fanex.ExtAcc.Customer.isMasterEditMember = true;
        var masterId = $scope.model.personalInfo.downlines[1].key;
        parent.Fanex.ExtAcc.LoadMainIFrame(parent.resolveClientUrl('customersetting/KenoSetting/kenosetting/MasterEditMember?custId=' + $scope.model.personalInfo.custId.value + "&userId=" + parent.userId));
    };

    $scope.MasterGoldDeluxe = function () {
        var masterId = $scope.model.personalInfo.downlines[1].key;
        parent.Fanex.ExtAcc.Customer.CurrentLevel = 1;
        parent.Fanex.ExtAcc.Customer.isSuperEditMember = false;
        parent.Fanex.ExtAcc.Customer.isMasterEditMember = true;
        parent.Fanex.ExtAcc.Customer.IsMultiple = false;
        parent.Fanex.ExtAcc.Customer.UserId = $scope.model.personalInfo.custId.value;
        parent.Fanex.ExtAcc.Customer.masterId = masterId;

        parent.Fanex.ExtAcc.LoadMainIFrame(parent.resolveClientUrl('GoldDeluxeSetting/GoldDeluxe/EditSetting?upLineLevel=3&formType=1&uplineId=' + masterId + '&level=1&custId=' + $scope.model.personalInfo.custId.value + "&actorId=" + parent.userId + "&groupName=" + parent.$('#selectedGroupName').html()));
    };

    $scope.MasterLiveCasino = function () {
        var masterId = $scope.model.personalInfo.downlines[1].key;
        parent.Fanex.ExtAcc.Customer.CurrentLevel = 1;
        parent.Fanex.ExtAcc.Customer.isSuperEditMember = false;
        parent.Fanex.ExtAcc.Customer.isMasterEditMember = true;
        parent.Fanex.ExtAcc.Customer.IsMultiple = false;
        parent.Fanex.ExtAcc.Customer.UserId = $scope.model.personalInfo.custId.value;
        parent.Fanex.ExtAcc.Customer.masterId = masterId;

        parent.Fanex.ExtAcc.LoadMainIFrame(parent.resolveClientUrl('GoldDeluxeSetting/GoldDeluxe/EditSetting?upLineLevel=3&formType=1&uplineId=' + masterId + '&level=1&custId=' + $scope.model.personalInfo.custId.value + "&actorId=" + parent.userId + "&groupName=" + parent.$('#selectedGroupName').html()));
    };

    $scope.SuperSBPT = function () {
        parent.Fanex.ExtAcc.Customer.IsFullInfo = false;
        parent.Fanex.ExtAcc.Customer.IsMultiple = false;
        var superId = $scope.model.personalInfo.downlines[0].key;
        var masterId = $scope.model.personalInfo.downlines[1].key;
        var agentId = $scope.model.personalInfo.downlines[2].key;

        parent.Fanex.ExtAcc.Customer.CurrentLevel = 1;
        parent.Fanex.ExtAcc.Customer.UserId = $scope.model.personalInfo.custId.value;
        parent.Fanex.ExtAcc.Customer.agentId = agentId;
        parent.Fanex.ExtAcc.Customer.masterId = masterId;
        parent.Fanex.ExtAcc.Customer.superId = superId;
        parent.Fanex.ExtAcc.Customer.isSuperEditMember = true;
        parent.Fanex.ExtAcc.Customer.isMasterEditMember = false;
        parent.Fanex.ExtAcc.LoadMainFrame(parent.resolveClientUrl('SportsbookSetting/EditPT?custId=' + $scope.model.personalInfo.custId.value + "&groupName=" + parent.$('#selectedGroupName').html()), null, null, null, true);
    };

    $scope.MasterSBPT = function () {
        parent.Fanex.ExtAcc.Customer.IsFullInfo = false;
        parent.Fanex.ExtAcc.Customer.IsMultiple = false;
        var superId = $scope.model.personalInfo.downlines[0].key;
        var masterId = $scope.model.personalInfo.downlines[1].key;
        var agentId = $scope.model.personalInfo.downlines[2].key;

        parent.Fanex.ExtAcc.Customer.CurrentLevel = 1;
        parent.Fanex.ExtAcc.Customer.UserId = $scope.model.personalInfo.custId.value;
        parent.Fanex.ExtAcc.Customer.agentId = agentId;
        parent.Fanex.ExtAcc.Customer.masterId = masterId;
        parent.Fanex.ExtAcc.Customer.superId = superId;
        parent.Fanex.ExtAcc.Customer.isSuperEditMember = false;
        parent.Fanex.ExtAcc.Customer.isMasterEditMember = true;
        parent.Fanex.ExtAcc.LoadMainFrame(parent.resolveClientUrl('SportsbookSetting/EditPT?custId=' + $scope.model.personalInfo.custId.value + "&groupName=" + parent.$('#selectedGroupName').html()), null, null, null, true);
    };
};

function LoginController($scope) {
    if ($scope.$parent.model) {
        $scope.loginInfo = $scope.$parent.model.login;
        $scope.model = $scope.$parent.model;
    }
    else {
        $scope.model = null;
    }

    $scope.ResetPassword = function (custId, title) {
        var roleId = model.personalInfo.roleId.value;
        parent.Fanex.ExtAcc.Form.Show(parent.resolveClientUrl('Customer/ResetPassword/?custId=' + custId + '&groupName=' + parent.$('#selectedGroupName').html()), 'Reset Password: <font color=\'#6dcff6\'>' + title + '</font>', 415, (roleId != 1 ? 400 : 350));
    };

    $scope.ShowIp = function (ip) {
        parent.Fanex.ExtAcc.Customer.ShowIpInfo(ip);
    };

    $scope.ResetSecurityCode = function (id, roleId, username) {
        var url = parent.resolveClientUrl('CustomerSetting/LeftPanel/SearchCustomer/GetSecurityCode?id=' + id + '&roleId=' + roleId + '&username=' + username + '&groupName=' + parent.$('#selectedGroupName').html());
        parent.Fanex.ExtAcc.LoadMainIFrame(url);
    };
};

function OnlineListController($scope) {
    if ($scope.$parent && $scope.$parent.model) {
        $scope.model = $scope.$parent.model;
        $scope.onlineList = $scope.$parent.model.onlineList.onlines;
    }
    else {
        $scope.model = null;
        $scope.onlineList = null;
    }

    $scope.Init = function () {
        if ($scope.onlineList && $scope.onlineList.length > 0) {
            for (var i = 0; i < $scope.onlineList.length; i++) {
                $scope.onlineList[i].index = i + 1;
            }
        }
    };

    $scope.ShowIp = function (ip) {
        parent.Fanex.ExtAcc.Customer.ShowIpInfo(ip);
    };
};