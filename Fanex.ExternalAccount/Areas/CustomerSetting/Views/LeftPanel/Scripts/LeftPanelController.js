var module = angular.module("leftPanelApp", [])
.directive('ngBlur', function () {
    return function (scope, elem, attrs) {
        elem.bind('blur', function () {
            scope.$apply(attrs.ngBlur);
        });
    };
});

angular.element(document.getElementById('ng-app')).ready(function () {
    $('#ng-app').show();
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
                    + "&currentLevel=" + parent.Fanex.ExtAcc.Customer.CurrentLevel + "&username=" + data.username
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
        parent.Fanex.ExtAcc.LoadMainFrame(parent.resolveClientUrl('Statement/Index/' + custId + '/' + $scope.model.personalInfo.currentLevel.value));
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
        parent.Fanex.ExtAcc.Customer.MessageClosed = 'Are you sure you want to change Close status for this customer? Warning, all downlines will be closed as well!';
        parent.Fanex.ExtAcc.Customer.MessageUnClosed = 'Are you sure you want to change Close status for this customer?';
        parent.Fanex.ExtAcc.Customer.MessageSuspended = 'Are you sure you want to change Suspend status for this customer? Warning, all members under this agent can\'t place bet!';
        parent.Fanex.ExtAcc.Customer.MessageUnSuspended = 'Are you sure you want to change Suspend status for this customer? All downlines will be un-suspended as well.';

        parent.Fanex.ExtAcc.Customer.ProcessConfirmClosedStatus = function (id) {
            if (parent.Fanex.ExtAcc.Customer.CurrentLevel != 4) {
                var timeout = setTimeout(function () {
                    clearTimeout(timeout);
                    var _closed = parent.document.getElementById(id ? id : 'closed');
                    parent.Fanex.ExtAcc.Customer.Closed = _closed ? _closed.checked : false;

                    parent.$(id ? '#' + id : '#closed').bind('click', function () {
                        var checkbox = this;

                        if (this.checked && !parent.Fanex.ExtAcc.Customer.Closed) {
                            parent.Fanex.ExtAcc.Form.ConfirmBox('Edit Status',
                            parent.Fanex.ExtAcc.Customer.MessageClosed,
                            400,
                            150,
                            false,
                            function () {
                                parent.Fanex.ExtAcc.Form.Close(false, false, 'confirmStatus');
                            },
                            function () {
                                checkbox.checked = false;
                                parent.Fanex.ExtAcc.Form.Close(false, false, 'confirmStatus');
                            },
                            'confirmStatus');
                        }
                        else if (!this.checked && parent.Fanex.ExtAcc.Customer.Closed) {
                            parent.Fanex.ExtAcc.Form.ConfirmBox('Edit Status',
                            parent.Fanex.ExtAcc.Customer.MessageUnClosed,
                            400,
                            150,
                            false,
                            function () {
                                parent.Fanex.ExtAcc.Form.Close(false, false, 'confirmStatus');
                            },
                            function () {
                                checkbox.checked = true;
                                parent.Fanex.ExtAcc.Form.Close(false, false, 'confirmStatus');
                            },
                            'confirmStatus');
                        }
                    });
                }, 1000);
            }
        };

        parent.Fanex.ExtAcc.Customer.ProcessConfirmSuspendedStatus = function (id) {
            if (parent.Fanex.ExtAcc.Customer.CurrentLevel != 4) {
                var timeout = setTimeout(function () {
                    clearTimeout(timeout);
                    var suspended = parent.document.getElementById(id ? id : 'suspended')
                    parent.Fanex.ExtAcc.Customer.Suspended = suspended ? suspended.checked : false;

                    parent.$(id ? '#' + id : '#suspended').bind('click', function () {
                        var checkbox = this;

                        if (this.checked && !parent.Fanex.ExtAcc.Customer.Suspended) {
                            parent.Fanex.ExtAcc.Form.ConfirmBox('Edit Status',
                            parent.Fanex.ExtAcc.Customer.MessageSuspended,
                            400,
                            180,
                            false,
                            function () {
                                parent.Fanex.ExtAcc.Form.Close(false, false, 'confirmStatus');
                            },
                            function () {
                                checkbox.checked = false;
                                parent.Fanex.ExtAcc.Form.Close(false, false, 'confirmStatus');
                            },
                            'confirmStatus');
                        }
                        else if (!this.checked && parent.Fanex.ExtAcc.Customer.Suspended) {
                            parent.Fanex.ExtAcc.Form.ConfirmBox('Edit Status',
                            parent.Fanex.ExtAcc.Customer.MessageUnSuspended,
                            400,
                            180,
                            false,
                            function () {
                                parent.Fanex.ExtAcc.Form.Close(false, false, 'confirmStatus');
                            },
                            function () {
                                checkbox.checked = true;
                                parent.Fanex.ExtAcc.Form.Close(false, false, 'confirmStatus');
                            },
                            'confirmStatus');
                        }
                    });
                }, 1000);
            }
        };

        parent.Fanex.ExtAcc.LoadMainFrame(parent.resolveClientUrl('SportsbookSetting/EditMultiplePT'));

        parent.Fanex.ExtAcc.Customer.Init = function () {
            //Prevent loading group name
            var timeout = parent.window.setInterval(function () {
                if (parent.$.trim(parent.$('#selectedGroupName').html()) != '') {
                    parent.Fanex.ExtAcc.Customer.Search(true);
                    parent.clearInterval(timeout);
                }
            }, 100);

            parent.$('#exporttoexcel').click(function () {
                var data = parent.Fanex.ExtAcc.Customer.GetParams();

                var params = "groupName=" + data.groupName
                    + "&enabled=" + data.enabled + "&closed=" + data.closed + "&suspended=" + data.suspended
                    + "&sRecommend=" + parent.Fanex.ExtAcc.Customer.superId + "&mRecommend=" + parent.Fanex.ExtAcc.Customer.masterId + "&recommend=" + parent.Fanex.ExtAcc.Customer.agentId + "&custId=" + parent.Fanex.ExtAcc.Customer.custId
                    + "&currentLevel=" + parent.Fanex.ExtAcc.Customer.CurrentLevel + "&username=" + data.username
                parent.document.location.href = parent.resolveClientUrl("Customer/GetExportToExcel?") + params;
            });

            parent.$('#txtUserName').keypress(function (e) {
                if (typeof e == 'undefined') e = window.event;
                var key = (typeof e.keyCode == 'undefined') ? e.which : e.keyCode;
                if (key == 13) {
                    parent.$('.ui-autocomplete').hide();
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

            parent.Fanex.ExtAcc.InitSuggestion();
        };
    };

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
            parent.Fanex.ExtAcc.LoadMainIFrame(url);
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
        parent.Fanex.ExtAcc.Customer.UserId = parent.window.id = custId;
        parent.window.uplineId = parent.Fanex.ExtAcc.Customer.superId = parent.Fanex.ExtAcc.Customer.masterId = parent.Fanex.ExtAcc.Customer.agentId = uplineId;
        parent.window.CurrentLevel = roleId;

        switch (roleId) {
            case 4:
                url = parent.resolveClientUrl('LiveCasino/EditSuper/' + $scope.model.personalInfo.custId.value + '/' + uplineId);
                break;
            case 3:
                url = parent.resolveClientUrl('LiveCasino/EditMaster/' + $scope.model.personalInfo.custId.value + '/' + $scope.model.personalInfo.uplineId.value);
                break;
            case 2:
                url = parent.resolveClientUrl('LiveCasino/EditAgent/' + $scope.model.personalInfo.custId.value + '/' + $scope.model.personalInfo.uplineId.value);
                break;
            case 1:
                url = parent.resolveClientUrl('LiveCasino/EditMember/' + $scope.model.personalInfo.custId.value + '/' + $scope.model.personalInfo.uplineId.value);
                break;
            default:
                break;
        }

        if (url != '') {
            parent.Fanex.ExtAcc.LoadMainFrame(url);
        }
    };

    $scope.BetSetting = function () {
        var custId = $scope.model.personalInfo.custId.value;
        var param = custId + '/' + custId + '/' + parent.userId + '/' + parent.$('#selectedGroupName').html();
        var url = parent.resolveClientUrl('CustomerSetting/BetSetting/Sportsbook/Index/' + param);
        
        parent.Fanex.ExtAcc.LoadMainIFrame(url);
    };

    $scope.Racing = function () {
        var roleId = $scope.model.personalInfo.roleId.value;
        var custId = $scope.model.personalInfo.custId.value;
        var uplineId = roleId == 4 ? userId : $scope.model.personalInfo.uplineId.value;
        var url = '';
        switch (roleId) {
            case 4:
                url = parent.resolveClientUrl('CustomerSetting/Racebook/RacingSetting/Super?custId=' + custId);
                break;
            case 3:
                url = parent.resolveClientUrl('CustomerSetting/Racebook/RacingSetting/Master?custid=' + custId + '&uplineid=' + uplineId);
                break;
            case 2:
                url = parent.resolveClientUrl('CustomerSetting/Racebook/RacingSetting/Agent?custid=' + custId + '&uplineid=' + uplineId);
                break;
            case 1:
                url = parent.resolveClientUrl('CustomerSetting/Racebook/RacingSetting/Member?custid=' + custId + '&uplineid=' + uplineId);
                break;
            default:
                break;
        }

        if (url !== '') {
            parent.Fanex.ExtAcc.LoadMainIFrame(url);
        }
    };

    $scope.Bingo = function () {
        var personalObject = $scope.model.personalInfo;
        var roleId = personalObject.roleId.value;
        var custId = personalObject.custId.value;
        var uplineId = personalObject.uplineRoleId.value > 4 ? custId : personalObject.uplineId.value;
        var url = '';
        switch (roleId) {
            case 4:
                url = parent.resolveClientUrl('CustomerSetting/Bingo/BingoSetting/Super?custId=' + custId);
                break;
            case 3:
                url = parent.resolveClientUrl('CustomerSetting/Bingo/BingoSetting/Master?custid=' + custId);
                break;
            case 2:
                url = parent.resolveClientUrl('CustomerSetting/Bingo/BingoSetting/Agent?custid=' + custId);
                break;
            case 1:
                url = parent.resolveClientUrl('CustomerSetting/Bingo/BingoSetting/Member?custid=' + custId);
                break;
            default:
                break;
        }

        if (url !== '' && uplineId > 0) {
            url += '&uplineid=' + uplineId;
            parent.Fanex.ExtAcc.LoadMainIFrame(url);
        }
    };

    $scope.VirtualSports = function () {
        //var roleId;
        var personalObject = $scope.model.personalInfo;
        var uplineRoleId = personalObject.uplineRoleId.value;
        var uplineId = uplineRoleId > 4 ? personalObject.custId.value : personalObject.uplineId.value;
        var targetCustId = personalObject.custId.value;
        var targetRoleId = personalObject.roleId.value;

        if (uplineId > 0 && uplineRoleId > 0 && targetCustId > 0 && targetRoleId > 0) {
            var url = parent.resolveClientUrl('VirtualSports/EditSingle?custId=' + uplineId + '&roleId=' + uplineRoleId + '&targetCustId=' + targetCustId + '&targetRoleId=' + targetRoleId);
            parent.Fanex.ExtAcc.LoadMainFrame(url, undefined, true, false, true);
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
            url: parent.resolveClientUrl('CustomerSetting/LeftPanel/SearchCustomer/SelectClearBalance?custId=' + custId),
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
            url: parent.resolveClientUrl('CustomerSetting/LeftPanel/SearchCustomer/SelectClearBalance?custId=' + custId),
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
            url: parent.resolveClientUrl('CustomerSetting/LeftPanel/SearchCustomer/SelectClearBalance?custId=' + custId),
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

    $scope.EditCredit = function (value) {
        $('#linkGivenCredit').hide();
        $('#givenCredit').val(value.toFixed(2).formatNumber())
        $('#givenCredit').show();
        $('#givenCredit').focus();
    };

    $scope.CreditDetail = function () {
        var id = $scope.personalInfo.custId.value;
        var roleId = $scope.personalInfo.roleId.value;

        parent.Fanex.ExtAcc.BlockUI();

        $http({
            url: parent.resolveClientUrl('CustomerSetting/LeftPanel/SearchCustomer/UsedCreditPage?id=' + id + '&roleId=' + roleId),
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
            $http({
                url: parent.resolveClientUrl('CustomerSetting/LeftPanel/SearchCustomer/UpdateGivenCreditPost?id=' + id + '&uplineId=' + uplineId + '&newGivenCredit=' + value),
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
                    $('#givenCredit').attr('onblur', 'RevertCredit("' + value.replaceAll(',', '').toFixed(2).formatNumber() + '")');
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

function SettingController($scope, $http) {
    if ($scope.$parent.model) {
        $scope.setting = $scope.$parent.model.setting;
        $scope.model = $scope.$parent.model;
    }
    else {
        $scope.model = null;
    }

    $scope.Init = function () {
        if ($scope.setting != null) {
            var groupName = parent.$('#selectedGroupName').html();
            var allowGroups = eval(parent.allowBetSettingGroups);
            for (var i = 0; i < $scope.setting.details.length; i++) {
                if ($scope.setting.details[i].key == 'Poker' ||
                    ($scope.setting.details[i].key == 'Transfer' && $scope.model.personalInfo.roleId.value == 1) ||
                    ($scope.setting.details[i].key == 'BetSetting' && (parent.showBetSetting == '0' || !allowGroups.contains(parseInt(parent.$('#selectedGroupIndex').html()))))) {
                    $scope.setting.details[i].isHide = true;
                }
            }

            for (var i = 0; i < $scope.setting.enableItems.length; i++) {
                if ($scope.model.personalInfo.roleId.value == 4 && ($scope.setting.enableItems[i].key == 'Closed' || $scope.setting.enableItems[i].key == 'Suspended') &&
                    (parent.showBetSetting == '0' || !allowGroups.contains(parseInt(parent.$('#selectedGroupIndex').html())))) {
                    $scope.setting.enableItems[i].disabled = true;
                    $scope.setting.enableItems[i].tooltip = 'You are not authorized on this function';
                } 
            }
        }
    };

    $scope.AddNew = function () {
        var roleId = $scope.model.personalInfo.roleId.value;
        var actionName = '';

        switch (roleId) {
            case 2:
                actionName = 'Customer/AgentAddNewMember/' + $scope.model.personalInfo.custId.value + '/' + $scope.model.personalInfo.currentLevel.value;
                break;
            case 3:
                actionName = 'Customer/MasterAddNewAgent/' + $scope.model.personalInfo.custId.value + '/' + $scope.model.personalInfo.currentLevel.value;
                break;
            case 4:
                actionName = 'Customer/SuperAddNewMaster/' + $scope.model.personalInfo.custId.value + '/' + $scope.model.personalInfo.currentLevel.value;
                break;
            default:
                break;
        }

        if (actionName != '') {
            parent.Fanex.ExtAcc.BlockUI();
            /*
            $http({ url: parent.resolveClientUrl(actionName), method: 'post', headers: { "X-Requested-With": "XMLHttpRequest"} })
            .success(function (result) {
            parent.$('#contentSplitter').html(result);
            });*/
            parent.Fanex.ExtAcc.LoadMainFrame(parent.resolveClientUrl(actionName));
        }
    };

    $scope.DoubleComm = function (allow, id) {
        var msg = allow ?
            "Are you sure you want to disallow Double Commission feature for Sportsbook's Losing tickets on this customer? Warning, all members under this agent will be disallowed as well!" :
            "Are you sure you want to allow Double Commission feature for Sportsbook's Losing tickets on this customer?";
        parent.Fanex.ExtAcc.Form.ConfirmBox(allow ? 'Disable Allow Double Commission' : 'Allow Double Commission',
        msg,
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
                    allow: !allow
                }
            })
            .success(function (result) {
                parent.Fanex.ExtAcc.Form.Close();
                if (result.IsSuccess) {
                    var timeout = setTimeout(function () {
                        clearTimeout(timeout);
                        parent.Fanex.ExtAcc.UnBlockUI();
                        parent.$('#leftFrame').attr('src', document.URL.split('?')[0] + '?v=' + new Date().toGMTString());                                                
                    }, 2000);
                }
                else {
                    parent.Fanex.ExtAcc.UnBlockUI();
                    parent.Fanex.ExtAcc.Form.Alert('Error', result.ErrorCode == "600" ? "This account was disabled".formatErrorMessage() : result.ErrorMessage.formatErrorMessage(), 350, 150, 'alert');
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
        var allowGroups = eval(parent.allowBetSettingGroups);
        if ($scope.model.personalInfo.roleId.value == 4 && (parent.showBetSetting == '0' || !allowGroups.contains(parseInt(parent.$('#selectedGroupIndex').html())))) {
            return;
        }

        var checked = !close ? 'checked="checked"' : "";
        var content = '<input name="chkA" type="checkbox" disabled="disabled" ' + checked + ' />Closed' +
            '&nbsp;<input name="chkB" type="checkbox" id="clearCredit" />Clear All Credit';

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
                            custName: $scope.model.personalInfo.currentLevel.value
                        }
                    })
                    .success(function (result) {
                        parent.Fanex.ExtAcc.Form.Close(null, null, 'popupClose');
                        if (result.IsSuccess) {
                            var timeout = setTimeout(function () {
                                clearTimeout(timeout);
                                parent.Fanex.ExtAcc.UnBlockUI();
                                parent.$('#leftFrame').attr('src', document.URL.split('?')[0] + '?v=' + new Date().toGMTString());
                            }, 2000);
                        }
                        else {
                            parent.Fanex.ExtAcc.UnBlockUI();
                            parent.Fanex.ExtAcc.Form.Close(null, null, 'popupClose');
                            parent.Fanex.ExtAcc.Form.Alert('Error', result.ErrorCode == "600" ? "This account was disabled".formatErrorMessage() : result.ErrorMessage.formatErrorMessage(), 350, 150, 'alert');
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
                                custName: $scope.model.personalInfo.currentLevel.value
                            }
                        })
                        .success(function (result) {
                            parent.Fanex.ExtAcc.Form.Close(null, null, 'confirmClose');
                            parent.Fanex.ExtAcc.Form.Close(null, null, 'popupClose');
                            if (result.IsSuccess) {
                                var timeout = setTimeout(function () {
                                    clearTimeout(timeout);
                                    parent.Fanex.ExtAcc.UnBlockUI();
                                    parent.$('#leftFrame').attr('src', document.URL.split('?')[0] + '?v=' + new Date().toGMTString());
                                }, 2000);
                            }
                            else {
                                parent.Fanex.ExtAcc.UnBlockUI();
                                parent.Fanex.ExtAcc.Form.Close(null, null, 'confirmClose');
                                parent.Fanex.ExtAcc.Form.Close(null, null, 'popupClose');
                                parent.Fanex.ExtAcc.Form.Alert('Error', result.ErrorCode == "600" ? "This account was disabled".formatErrorMessage() : result.ErrorMessage.formatErrorMessage(), 350, 150, 'alert');
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
        var allowGroups = eval(parent.allowBetSettingGroups);
        if ($scope.model.personalInfo.roleId.value == 4 && (parent.showBetSetting == '0' || !allowGroups.contains(parseInt(parent.$('#selectedGroupIndex').html())))) {
            return;
        }

        var message = '';
        var title = '';

        if (suspend) {
            message = 'Are you sure you want to change Suspended status for this customer? All downlines will be un-suspended as well';
            title = 'Un-suspend Customer: <font color="#6dcff6">' + $scope.model.personalInfo.currentLevel.value + '</font>';
        }
        else {
            message = "Are you sure you want to change Suspended status for this customer? Warning, all members under this agent can't place bet!";
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
                            suspend: !suspend
                        }
                    })
                    .success(function (result) {
                        parent.Fanex.ExtAcc.Form.Close(null, null, 'popupSuspend');
                        if (result.IsSuccess) {
                            var timeout = setTimeout(function () {
                                clearTimeout(timeout);
                                parent.Fanex.ExtAcc.UnBlockUI();
                                parent.$('#leftFrame').attr('src', document.URL.split('?')[0] + '?v=' + new Date().toGMTString());
                            }, 2000);
                        }
                        else {
                            parent.Fanex.ExtAcc.UnBlockUI();
                            parent.Fanex.ExtAcc.Form.Close(null, null, 'popupSuspend');
                            parent.Fanex.ExtAcc.Form.Alert('Error', result.ErrorCode == "600" ? "This account was disabled".formatErrorMessage() : result.ErrorMessage.formatErrorMessage(), 350, 150, 'alert');
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
            var title = '';
            var message = allow ? 'Are you sure you want to change Outright status for this customer? Warning, all members under this agent can\'t place Outright bet!' : 'Are you sure you want to change Outright status for this customer?';
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
                        allow: !allow
                    }
                })
                .success(function (result) {
                    parent.Fanex.ExtAcc.Form.Close(null, null, 'popupOutright');
                    if (result.IsSuccess) {
                        var timeout = setTimeout(function () {
                            clearTimeout(timeout);
                            parent.Fanex.ExtAcc.UnBlockUI();
                            parent.$('#leftFrame').attr('src', document.URL.split('?')[0] + '?v=' + new Date().toGMTString());
                        }, 2000);
                    }
                    else {
                        parent.Fanex.ExtAcc.UnBlockUI();
                        parent.Fanex.ExtAcc.Form.Close(null, null, 'popupOutright');
                        parent.Fanex.ExtAcc.Form.Alert('Error', result.ErrorCode == "600" ? "This account was disabled".formatErrorMessage() : result.ErrorMessage.formatErrorMessage(), 350, 150, 'alert');
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
                        allow: !allow
                    }
                })
                .success(function (result) {
                    parent.Fanex.ExtAcc.Form.Close(null, null, 'popupAutoPT');
                    if (result.IsSuccess) {
                        var timeout = setTimeout(function () {
                            clearTimeout(timeout);
                            parent.Fanex.ExtAcc.UnBlockUI();
                            parent.$('#leftFrame').attr('src', document.URL.split('?')[0] + '?v=' + new Date().toGMTString());
                        }, 2000);
                    }
                    else {
                        parent.Fanex.ExtAcc.UnBlockUI();
                        parent.Fanex.ExtAcc.Form.Close(null, null, 'popupAutoPT');
                        parent.Fanex.ExtAcc.Form.Alert('Error', result.ErrorCode == "600" ? "This account was disabled".formatErrorMessage() : result.ErrorMessage.formatErrorMessage(), 350, 150, 'alert');
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
        var message = 'Are you sure you want to change casino status for this customer? Warning, casino downlines will be disabled as well';

        var checked = !enable ? 'checked="checked"' : "";
        var content = '<input name="chkA" type="checkbox" disabled="disabled" checked="checked" />' + $scope.model.personalInfo.currentLevel.text +
            '&nbsp;<input name="chkB" type="checkbox" id="enableDownline" />Casino\'s downlines';
        content = enable ? '<input name="chkA" type="checkbox" disabled="disabled" />' + $scope.model.personalInfo.currentLevel.text : content;

        if (enable) {
            parent.Fanex.ExtAcc.Form.ConfirmBox('Disable Casino: <font color="#6dcff6">' + $scope.model.personalInfo.currentLevel.value + '</font>',
                    content,
                    350,
                    150,
                    false,
                    function () {
                        parent.Fanex.ExtAcc.Form.ConfirmBox(
                            'Warning!!!',
                            message,
                            460,
                            150,
                            false,
                            function () {
                                parent.Fanex.ExtAcc.BlockUI();

                                $.ajax({
                                    url: parent.resolveClientUrl('CustomerSetting/LeftPanel/SearchCustomer/UpdateCasino'),
                                    dataType: 'json',
                                    type: 'post',
                                    data: {
                                        custId: $scope.model.personalInfo.custId.value,
                                        uplineId: $scope.model.personalInfo.roleId.value == 4 ? 0 : $scope.model.personalInfo.uplineId.value,
                                        enable: !enable,
                                        enableDownline: false,
                                        custName: $scope.model.personalInfo.currentLevel.value
                                    }
                                })
                                .success(function (result) {
                                    parent.Fanex.ExtAcc.Form.Close(null, null, 'confirmCasino');
                                    parent.Fanex.ExtAcc.Form.Close(null, null, 'popupCasino');
                                    if (result.IsSuccess) {
                                        var timeout = setTimeout(function () {
                                            clearTimeout(timeout);
                                            parent.Fanex.ExtAcc.UnBlockUI();
                                            parent.$('#leftFrame').attr('src', document.URL.split('?')[0] + '?v=' + new Date().toGMTString());
                                        }, 2000);
                                    }
                                    else {
                                        parent.Fanex.ExtAcc.UnBlockUI();
                                        parent.Fanex.ExtAcc.Form.Close(null, null, 'confirmCasino');
                                        parent.Fanex.ExtAcc.Form.Alert('Error', result.ErrorCode == "600" ? "This account was disabled".formatErrorMessage() : result.ErrorMessage.formatErrorMessage(), 350, 150, 'alert');
                                        document.getElementById(id).checked = enable;
                                    }
                                })
                                .error(function (xhr) {
                                    parent.Fanex.ExtAcc.UnBlockUI();
                                    parent.Fanex.ExtAcc.Form.Close(null, null, 'confirmCasino');
                                    parent.Fanex.ExtAcc.Form.Close(null, null, 'popupCasino');
                                    document.getElementById(id).checked = enable;
                                    if (typeof console != 'undefined') {
                                        console.log(xhr);
                                    }
                                });
                            },
                            function () {
                                parent.Fanex.ExtAcc.Form.Close(null, null, 'confirmCasino');
                            },
                            'confirmCasino');
                    },
                    function () {
                        parent.Fanex.ExtAcc.Form.Close(null, null, 'popupCasino');
                        document.getElementById(id).checked = enable;
                    },
                    'popupCasino',
                    true,
                    function () {
                        parent.Fanex.ExtAcc.Form.Close(null, null, 'popupCasino');
                        document.getElementById(id).checked = enable;
                    },
                    true);
        }
        else {
            parent.Fanex.ExtAcc.Form.ConfirmBox('Enable Casino: <font color="#6dcff6">' + $scope.model.personalInfo.currentLevel.value + '</font>',
                content,
                350,
                150,
                false,
                function () {
                    parent.Fanex.ExtAcc.BlockUI();

                    $.ajax({
                        url: parent.resolveClientUrl('CustomerSetting/LeftPanel/SearchCustomer/UpdateCasino'),
                        dataType: 'json',
                        type: 'post',
                        data: {
                            custId: $scope.model.personalInfo.custId.value,
                            uplineId: $scope.model.personalInfo.roleId.value == 4 ? 0 : $scope.model.personalInfo.uplineId.value,
                            enable: !enable,
                            enableDownline: parent.$('#enableDownline').get(0).checked,
                            custName: $scope.model.personalInfo.currentLevel.value
                        }
                    })
                    .success(function (result) {
                        parent.Fanex.ExtAcc.Form.Close(null, null, 'popupCasino');
                        if (result.IsSuccess) {
                            var timeout = setTimeout(function () {
                                clearTimeout(timeout);
                                parent.Fanex.ExtAcc.UnBlockUI();
                                parent.$('#leftFrame').attr('src', document.URL.split('?')[0] + '?v=' + new Date().toGMTString());
                            }, 2000);
                        }
                        else {
                            parent.Fanex.ExtAcc.UnBlockUI();
                            parent.Fanex.ExtAcc.Form.Close(null, null, 'popupCasino');
                            parent.Fanex.ExtAcc.Form.Alert('Error', result.ErrorCode == "600" ? "This account was disabled".formatErrorMessage() : result.ErrorMessage.formatErrorMessage(), 350, 150, 'alert');
                            document.getElementById(id).checked = enable;
                        }
                    })
                    .error(function (xhr) {
                        parent.Fanex.ExtAcc.UnBlockUI();
                        parent.Fanex.ExtAcc.Form.Close(null, null, 'popupCasino');
                        document.getElementById(id).checked = enable;
                        if (typeof console != 'undefined') {
                            console.log(xhr);
                        }
                    });
                },
                function () {
                    parent.Fanex.ExtAcc.Form.Close(null, null, 'popupCasino');
                    document.getElementById(id).checked = enable;
                },
                'popupCasino',
                true,
                function () {
                    parent.Fanex.ExtAcc.Form.Close(null, null, 'popupCasino');
                    document.getElementById(id).checked = enable;
                },
                true);
        }
    };

    $scope.EnableBingo = function (enable, id) {
        var message = 'Are you sure you want to change bingo status for this customer? Warning, bingo downlines will be disabled as well';

        var checked = !enable ? 'checked="checked"' : "";
        var content = '<input name="chkA" type="checkbox" disabled="disabled" checked="checked" />' + $scope.model.personalInfo.currentLevel.text +
        '&nbsp;<input name="chkB" type="checkbox" id="enableDownline" />Bingo\'s downlines';
        content = enable ? '<input name="chkA" type="checkbox" disabled="disabled" />' + $scope.model.personalInfo.currentLevel.text : content;

        if (enable) {
            parent.Fanex.ExtAcc.Form.ConfirmBox('Disable Bingo: <font color="#6dcff6">' + $scope.model.personalInfo.currentLevel.value + '</font>',
                content,
                350,
                150,
                false,
                function () {
                    parent.Fanex.ExtAcc.Form.ConfirmBox(
                        'Warning!!!',
                        message,
                        460,
                        150,
                        false,
                        function () {
                            parent.Fanex.ExtAcc.BlockUI();

                            $.ajax({
                                url: parent.resolveClientUrl('CustomerSetting/LeftPanel/SearchCustomer/UpdateBingo'),
                                dataType: 'json',
                                type: 'post',
                                data: {
                                    custId: $scope.model.personalInfo.custId.value,
                                    uplineId: $scope.model.personalInfo.roleId.value == 4 ? 0 : $scope.model.personalInfo.uplineId.value,
                                    enable: !enable,
                                    enableDownline: false,
                                    custName: $scope.model.personalInfo.currentLevel.value
                                }
                            })
                            .success(function (result) {
                                parent.Fanex.ExtAcc.Form.Close(null, null, 'confirmBingo');
                                parent.Fanex.ExtAcc.Form.Close(null, null, 'popupBingo');
                                if (result.IsSuccess) {
                                    var timeout = setTimeout(function () {
                                        clearTimeout(timeout);
                                        parent.Fanex.ExtAcc.UnBlockUI();
                                        parent.$('#leftFrame').attr('src', document.URL.split('?')[0] + '?v=' + new Date().toGMTString());
                                    }, 2000);
                                }
                                else {
                                    parent.Fanex.ExtAcc.UnBlockUI();
                                    parent.Fanex.ExtAcc.Form.Close(null, null, 'confirmBingo');
                                    parent.Fanex.ExtAcc.Form.Alert('Error', result.ErrorCode == "600" ? "This account was disabled".formatErrorMessage() : result.ErrorMessage.formatErrorMessage(), 350, 150, 'alert');
                                    document.getElementById(id).checked = enable;
                                }
                            })
                            .error(function (xhr) {
                                parent.Fanex.ExtAcc.UnBlockUI();
                                parent.Fanex.ExtAcc.Form.Close(null, null, 'confirmBingo');
                                parent.Fanex.ExtAcc.Form.Close(null, null, 'popupBingo');
                                document.getElementById(id).checked = enable;
                                if (typeof console != 'undefined') {
                                    console.log(xhr);
                                }
                            });
                        },
                        function () {
                            parent.Fanex.ExtAcc.Form.Close(null, null, 'confirmBingo');
                        },
                        'confirmBingo');
                },
                function () {
                    parent.Fanex.ExtAcc.Form.Close(null, null, 'popupBingo');
                    document.getElementById(id).checked = enable;
                },
                'popupBingo',
                true,
                function () {
                    parent.Fanex.ExtAcc.Form.Close(null, null, 'popupBingo');
                    document.getElementById(id).checked = enable;
                },
                true);
        }
        else {
            parent.Fanex.ExtAcc.Form.ConfirmBox('Enable Bingo: <font color="#6dcff6">' + $scope.model.personalInfo.currentLevel.value + '</font>',
            content,
            350,
            150,
            false,
            function () {
                parent.Fanex.ExtAcc.BlockUI();

                $.ajax({
                    url: parent.resolveClientUrl('CustomerSetting/LeftPanel/SearchCustomer/UpdateBingo'),
                    dataType: 'json',
                    type: 'post',
                    data: {
                        custId: $scope.model.personalInfo.custId.value,
                        uplineId: $scope.model.personalInfo.roleId.value == 4 ? 0 : $scope.model.personalInfo.uplineId.value,
                        enable: !enable,
                        enableDownline: parent.$('#enableDownline').get(0).checked,
                        custName: $scope.model.personalInfo.currentLevel.value
                    }
                })
                .success(function (result) {
                    parent.Fanex.ExtAcc.Form.Close(null, null, 'popupBingo');
                    if (result.IsSuccess) {
                        var timeout = setTimeout(function () {
                            clearTimeout(timeout);
                            parent.Fanex.ExtAcc.UnBlockUI();
                            parent.$('#leftFrame').attr('src', document.URL.split('?')[0] + '?v=' + new Date().toGMTString());
                        }, 2000);
                    }
                    else {
                        parent.Fanex.ExtAcc.UnBlockUI();
                        parent.Fanex.ExtAcc.Form.Close(null, null, 'popupBingo');
                        parent.Fanex.ExtAcc.Form.Alert('Error', result.ErrorCode == "600" ? "This account was disabled".formatErrorMessage() : result.ErrorMessage.formatErrorMessage(), 350, 150, 'alert');
                        document.getElementById(id).checked = enable;
                    }
                })
                .error(function (xhr) {
                    parent.Fanex.ExtAcc.UnBlockUI();
                    parent.Fanex.ExtAcc.Form.Close(null, null, 'popupBingo');
                    document.getElementById(id).checked = enable;
                    if (typeof console != 'undefined') {
                        console.log(xhr);
                    }
                });
            },
            function () {
                parent.Fanex.ExtAcc.Form.Close(null, null, 'popupBingo');
                document.getElementById(id).checked = enable;
            },
            'popupBingo',
            true,
            function () {
                document.getElementById(id).checked = enable;
            },
            true);
        }
    };

    $scope.EnableNumberGame = function (enable, id) {
        var checked = !enable ? 'checked="checked"' : "";
        var disabledDownline = enable ? 'disabled="disabled"' : ''
        var content = '<input name="chkA" type="checkbox" disabled="disabled" ' + checked + ' />' + $scope.model.personalInfo.currentLevel.text +
        '&nbsp;<input name="chkB" type="checkbox" id="enableDownline" ' + disabledDownline + '/>' + $scope.model.personalInfo.currentLevel.text + '\'s downlines';
        var title = enable ? 'Disable Number Game: <font color="#6dcff6">' + $scope.model.personalInfo.currentLevel.value + '</font>' :
                             'Enable Number Game: <font color="#6dcff6">' + $scope.model.personalInfo.currentLevel.value + '</font>';

        parent.Fanex.ExtAcc.Form.ConfirmBox(
            title,
            content,
            380,
            150,
            false,
            function () {
                parent.Fanex.ExtAcc.BlockUI();

                $.ajax({
                    url: parent.resolveClientUrl('CustomerSetting/LeftPanel/SearchCustomer/UpdateNumberGame'),
                    dataType: 'json',
                    type: 'post',
                    data: {
                        custId: $scope.model.personalInfo.custId.value,
                        enable: !enable,
                        enableDownline: parent.$('#enableDownline').get(0).checked
                    }
                })
                .success(function (result) {
                    parent.Fanex.ExtAcc.Form.Close(null, null, 'confirmNG');
                    parent.Fanex.ExtAcc.Form.Close(null, null, 'popupNG');
                    if (result.IsSuccess) {
                        var timeout = setTimeout(function () {
                            clearTimeout(timeout);
                            parent.Fanex.ExtAcc.UnBlockUI();
                            parent.$('#leftFrame').attr('src', document.URL.split('?')[0] + '?v=' + new Date().toGMTString());
                        }, 2000);
                    }
                    else {
                        parent.Fanex.ExtAcc.UnBlockUI();
                        parent.Fanex.ExtAcc.Form.Close(null, null, 'confirmNG');
                        parent.Fanex.ExtAcc.Form.Alert('Error', result.ErrorCode == "600" ? "This account was disabled".formatErrorMessage() : result.ErrorMessage.formatErrorMessage(), 350, 150, 'alert');
                        document.getElementById(id).checked = enable;
                    }
                })
                .error(function (xhr) {
                    parent.Fanex.ExtAcc.UnBlockUI();
                    parent.Fanex.ExtAcc.Form.Close(null, null, 'confirmNG');
                    parent.Fanex.ExtAcc.Form.Close(null, null, 'popupNG');
                    document.getElementById(id).checked = enable;
                    if (typeof console != 'undefined') {
                        console.log(xhr);
                    }
                });
            },
            function () {
                parent.Fanex.ExtAcc.Form.Close(null, null, 'popupNG');
                document.getElementById(id).checked = enable;
            },
            'popupNG',
            true,
            function () {
                parent.Fanex.ExtAcc.Form.Close(null, null, 'popupNG');
                document.getElementById(id).checked = enable;
            },
            true);        
    };

    $scope.EnableLiveCasino = function (enable, id) {
        var checked = !enable ? 'checked="checked"' : "";
        var disabledDownline = enable ? 'disabled="disabled"' : ''
        var content = '<input name="chkA" type="checkbox" disabled="disabled" ' + checked + ' />' + $scope.model.personalInfo.currentLevel.text +
                        '&nbsp;<input name="chkB" type="checkbox" id="enableDownline" ' + disabledDownline + '/>' + $scope.model.personalInfo.currentLevel.text + '\'s downlines';
        var title = enable ? 'Disable Live Casino: <font color="#6dcff6">' + $scope.model.personalInfo.currentLevel.value + '</font>' :
                                'Enable Live Casino: <font color="#6dcff6">' + $scope.model.personalInfo.currentLevel.value + '</font>';

        parent.Fanex.ExtAcc.Form.ConfirmBox(
        title,
        content,
        380,
        150,
        false,
        function () {
            parent.Fanex.ExtAcc.BlockUI();

            $.ajax({
                url: parent.resolveClientUrl('CustomerSetting/LeftPanel/SearchCustomer/UpdateLiveCasino'),
                dataType: 'json',
                type: 'post',
                data: {
                    custId: $scope.model.personalInfo.custId.value,
                    enable: !enable,
                    enableDownline: parent.$('#enableDownline').get(0).checked
                }
            })
            .success(function (result) {
                parent.Fanex.ExtAcc.Form.Close(null, null, 'confirmLC');
                parent.Fanex.ExtAcc.Form.Close(null, null, 'popupLC');
                if (result.IsSuccess) {
                    var timeout = setTimeout(function () {
                        clearTimeout(timeout);
                        parent.Fanex.ExtAcc.UnBlockUI();
                        parent.$('#leftFrame').attr('src', document.URL.split('?')[0] + '?v=' + new Date().toGMTString());
                    }, 2000);
                }
                else {
                    parent.Fanex.ExtAcc.UnBlockUI();
                    parent.Fanex.ExtAcc.Form.Close(null, null, 'confirmLC');
                    parent.Fanex.ExtAcc.Form.Alert('Error', result.ErrorCode == "600" ? "This account was disabled".formatErrorMessage() : result.ErrorMessage.formatErrorMessage(), 350, 150, 'alert');
                    document.getElementById(id).checked = enable;
                }
            })
            .error(function (xhr) {
                parent.Fanex.ExtAcc.UnBlockUI();
                parent.Fanex.ExtAcc.Form.Close(null, null, 'confirmLC');
                parent.Fanex.ExtAcc.Form.Close(null, null, 'popupLC');
                document.getElementById(id).checked = enable;
                if (typeof console != 'undefined') {
                    console.log(xhr);
                }
            });
        },
        function () {
            parent.Fanex.ExtAcc.Form.Close(null, null, 'popupLC');
            document.getElementById(id).checked = enable;
        },
        'popupLC',
        true,
        function () {
            parent.Fanex.ExtAcc.Form.Close(null, null, 'popupLC');
            document.getElementById(id).checked = enable;
        },
        true);
    };

    $scope.EnableVirtualSports = function (enable, id) {
        var checked = !enable ? 'checked="checked"' : "";
        var disabledDownline = enable ? 'disabled="disabled"' : ''
        var content = '<input name="chkA" type="checkbox" disabled="disabled" ' + checked + ' />' + $scope.model.personalInfo.currentLevel.text +
                        '&nbsp;<input name="chkB" type="checkbox" id="enableDownline" ' + disabledDownline + '/>' + $scope.model.personalInfo.currentLevel.text + '\'s downlines';
        var title = enable ? 'Disable Virtual Sports: <font color="#6dcff6">' + $scope.model.personalInfo.currentLevel.value + '</font>' :
                                'Enable Virtual Sports: <font color="#6dcff6">' + $scope.model.personalInfo.currentLevel.value + '</font>';

        parent.Fanex.ExtAcc.Form.ConfirmBox(
        title,
        content,
        380,
        150,
        false,
        function () {
            parent.Fanex.ExtAcc.BlockUI();

            $.ajax({
                url: parent.resolveClientUrl('CustomerSetting/LeftPanel/SearchCustomer/UpdateVirtualSports'),
                dataType: 'json',
                type: 'post',
                data: {
                    custId: $scope.model.personalInfo.custId.value,
                    enable: !enable,
                    enableDownline: parent.$('#enableDownline').get(0).checked
                }
            })
            .success(function (result) {
                parent.Fanex.ExtAcc.Form.Close(null, null, 'confirmVS');
                parent.Fanex.ExtAcc.Form.Close(null, null, 'popupVS');
                if (result.IsSuccess) {
                    var timeout = setTimeout(function () {
                        clearTimeout(timeout);
                        parent.Fanex.ExtAcc.UnBlockUI();
                        parent.$('#leftFrame').attr('src', document.URL.split('?')[0] + '?v=' + new Date().toGMTString());
                    }, 2000);
                }
                else {
                    parent.Fanex.ExtAcc.UnBlockUI();
                    parent.Fanex.ExtAcc.Form.Close(null, null, 'confirmVS');
                    parent.Fanex.ExtAcc.Form.Alert('Error', result.ErrorCode == "600" ? "This account was disabled".formatErrorMessage() : result.ErrorMessage.formatErrorMessage(), 350, 150, 'alert');
                    document.getElementById(id).checked = enable;
                }
            })
            .error(function (xhr) {
                parent.Fanex.ExtAcc.UnBlockUI();
                parent.Fanex.ExtAcc.Form.Close(null, null, 'confirmVS');
                parent.Fanex.ExtAcc.Form.Close(null, null, 'popupVS');
                document.getElementById(id).checked = enable;
                if (typeof console != 'undefined') {
                    console.log(xhr);
                }
            });
        },
        function () {
            parent.Fanex.ExtAcc.Form.Close(null, null, 'popupVS');
            document.getElementById(id).checked = enable;
        },
        'popupVS',
        true,
        function () {
            parent.Fanex.ExtAcc.Form.Close(null, null, 'popupVS');
            document.getElementById(id).checked = enable;
        },
        true);
    };

    $scope.EnableRacing = function () {
        var url = parent.resolveClientUrl('CustomerSetting/LeftPanel/SearchCustomer/GetEnableRacing?id=' + $scope.model.personalInfo.custId.value +
            '&uplineId=' + ($scope.model.personalInfo.roleId.value == 4 ? 0 : $scope.model.personalInfo.uplineId.value) +
            '&roleId=' + $scope.model.personalInfo.roleId.value);
        var title = 'Update Racing Status: <font color="#6dcff6">' + $scope.model.personalInfo.currentLevel.value + '</font>';
        parent.Fanex.ExtAcc.Form.ShowFrame(url, title, 400, 350, false, null, 1, 'enableRacing');
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
        parent.Fanex.ExtAcc.LoadMainFrame(parent.resolveClientUrl('SportsbookSetting/EditMinPT'));
    };

    $scope.MinRB = function () {
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
        parent.Fanex.ExtAcc.LoadMainFrame(parent.resolveClientUrl('SportsbookSetting/SuperMasterEditSB'));
    };

    $scope.SuperBingo = function () {
        var superId = $scope.model.personalInfo.downlines[0].key;
        parent.Fanex.ExtAcc.Customer.isSuperEditMember = true;
        parent.Fanex.ExtAcc.Customer.isMasterEditMember = false;
        parent.Fanex.ExtAcc.Customer.IsMultiple = false;
        parent.Fanex.ExtAcc.Customer.CurrentLevel = 1;
        parent.Fanex.ExtAcc.LoadMainIFrame(parent.resolveClientUrl('CustomerSetting/Bingo/BingoSetting/SuperEditMember?custId=' + $scope.model.personalInfo.custId.value + '&uplineid=' + superId));
    };

    $scope.SuperCasino = function () {
        var superId = $scope.model.personalInfo.downlines[0].key;
        parent.Fanex.ExtAcc.Customer.isSuperEditMember = true;
        parent.Fanex.ExtAcc.Customer.isMasterEditMember = false;
        parent.Fanex.ExtAcc.Customer.IsMultiple = false;
        parent.Fanex.ExtAcc.Customer.CurrentLevel = 1;
        parent.Fanex.ExtAcc.LoadMainIFrame(parent.resolveClientUrl('CustomerSetting/Casino/Member/EditMemberBySuper/' + $scope.model.personalInfo.custId.value + '/' + superId));
    };

    $scope.SuperRacing = function () {
        var superId = $scope.model.personalInfo.downlines[0].key;
        parent.Fanex.ExtAcc.Customer.isSuperEditMember = true;
        parent.Fanex.ExtAcc.Customer.isMasterEditMember = false;
        parent.Fanex.ExtAcc.Customer.IsMultiple = false;
        parent.Fanex.ExtAcc.Customer.CurrentLevel = 1;
        parent.Fanex.ExtAcc.LoadMainIFrame(parent.resolveClientUrl('customersetting/racebook/racingsetting/supereditmember?custid=' + $scope.model.personalInfo.custId.value + '&uplineid=' + superId));
    };

    $scope.SuperLiveCasino = function () {
        var superId = $scope.model.personalInfo.downlines[0].key;
        parent.Fanex.ExtAcc.Customer.isSuperEditMember = true;
        parent.Fanex.ExtAcc.Customer.isMasterEditMember = false;
        parent.Fanex.ExtAcc.Customer.CurrentLevel = 1;
        parent.Fanex.ExtAcc.Customer.UserId = $scope.model.personalInfo.custId.value;
        parent.Fanex.ExtAcc.Customer.superId = superId;
        parent.Fanex.ExtAcc.LoadMainFrame(parent.resolveClientUrl('LiveCasino/SuperEditMember'));
    };

    $scope.SuperVirtualSports = function () {
        var superId = $scope.model.personalInfo.downlines[0].key;
        parent.Fanex.ExtAcc.Customer.isSuperEditMember = true;
        parent.Fanex.ExtAcc.Customer.isMasterEditMember = false;
        parent.Fanex.ExtAcc.Customer.IsMultiple = false;
        parent.Fanex.ExtAcc.Customer.CurrentLevel = 1;
        parent.Fanex.ExtAcc.LoadMainFrame(parent.resolveClientUrl('VirtualSports/EditSingle?custId=' + superId + '&roleId=4&targetCustId=' + $scope.model.personalInfo.custId.value + '&targetRoleId=1&groupName='));
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
        parent.Fanex.ExtAcc.LoadMainFrame(parent.resolveClientUrl('SportsbookSetting/SuperMasterEditSB'));
    };

    $scope.MasterBingo = function () {
        var masterId = $scope.model.personalInfo.downlines[1].key;
        parent.Fanex.ExtAcc.Customer.CurrentLevel = 1;
        parent.Fanex.ExtAcc.Customer.isSuperEditMember = false;
        parent.Fanex.ExtAcc.Customer.isMasterEditMember = true;
        parent.Fanex.ExtAcc.Customer.IsMultiple = false;
        parent.Fanex.ExtAcc.LoadMainIFrame(parent.resolveClientUrl('CustomerSetting/Bingo/BingoSetting/MasterEditMember?custId=' + $scope.model.personalInfo.custId.value + '&uplineid=' + masterId));
    };

    $scope.MasterCasino = function () {
        var masterId = $scope.model.personalInfo.downlines[1].key;
        parent.Fanex.ExtAcc.Customer.CurrentLevel = 1;
        parent.Fanex.ExtAcc.Customer.isSuperEditMember = false;
        parent.Fanex.ExtAcc.Customer.isMasterEditMember = true;
        parent.Fanex.ExtAcc.Customer.IsMultiple = false;
        parent.Fanex.ExtAcc.LoadMainIFrame(parent.resolveClientUrl('CustomerSetting/Casino/Member/EditMemberByMaster/' + $scope.model.personalInfo.custId.value + '/' + masterId));
    };

    $scope.MasterRacing = function () {
        var masterId = $scope.model.personalInfo.downlines[1].key;
        parent.Fanex.ExtAcc.Customer.CurrentLevel = 1;
        parent.Fanex.ExtAcc.Customer.isSuperEditMember = false;
        parent.Fanex.ExtAcc.Customer.isMasterEditMember = true;
        parent.Fanex.ExtAcc.Customer.IsMultiple = false;
        parent.Fanex.ExtAcc.LoadMainIFrame(parent.resolveClientUrl('customersetting/racebook/racingsetting/mastereditmember?custid=' + $scope.model.personalInfo.custId.value + '&uplineid=' + masterId));
    };

    $scope.MasterLiveCasino = function () {
        var masterId = $scope.model.personalInfo.downlines[1].key;
        parent.Fanex.ExtAcc.Customer.CurrentLevel = 1;
        parent.Fanex.ExtAcc.Customer.isSuperEditMember = false;
        parent.Fanex.ExtAcc.Customer.isMasterEditMember = true;
        parent.Fanex.ExtAcc.Customer.IsMultiple = false;
        parent.Fanex.ExtAcc.Customer.UserId = $scope.model.personalInfo.custId.value;
        parent.Fanex.ExtAcc.Customer.masterId = masterId;
        parent.Fanex.ExtAcc.LoadMainFrame(parent.resolveClientUrl('LiveCasino/MasterEditMember'));
    };

    $scope.MasterVirtualSports = function () {
        var masterId = $scope.model.personalInfo.downlines[1].key;
        parent.Fanex.ExtAcc.Customer.CurrentLevel = 1;
        parent.Fanex.ExtAcc.Customer.isSuperEditMember = false;
        parent.Fanex.ExtAcc.Customer.isMasterEditMember = true;
        parent.Fanex.ExtAcc.Customer.IsMultiple = false;
        parent.Fanex.ExtAcc.LoadMainFrame(parent.resolveClientUrl('VirtualSports/EditSingle?custId=' + masterId + '&roleId=3&targetCustId=' + $scope.model.personalInfo.custId.value + '&targetRoleId=1&groupName='));
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
        parent.Fanex.ExtAcc.LoadMainFrame(parent.resolveClientUrl('SportsbookSetting/EditPT'));
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
        parent.Fanex.ExtAcc.LoadMainFrame(parent.resolveClientUrl('SportsbookSetting/EditPT'));
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
        parent.Fanex.ExtAcc.Form.Show(parent.resolveClientUrl('Customer/ResetPassword/?custId=' + custId), 'Reset Password: <font color=\'#6dcff6\'>' + title + '</font>', 415, (roleId != 1 ? 400 : 350));
    };

    $scope.ShowIp = function (ip) {
        parent.Fanex.ExtAcc.Customer.ShowIpInfo(ip);
    };

    $scope.ResetSecurityCode = function (id, roleId, username) {
        var url = parent.resolveClientUrl('CustomerSetting/LeftPanel/SearchCustomer/GetSecurityCode?id=' + id + '&roleId=' + roleId + '&username=' + username);
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