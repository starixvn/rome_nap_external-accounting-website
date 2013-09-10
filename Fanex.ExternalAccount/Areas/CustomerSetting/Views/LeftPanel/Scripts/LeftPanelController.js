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
                    $scope.model.personalInfo.custList = 'Member List';
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
        parent.Fanex.ExtAcc.LoadMainFrame(parent.resolveClientUrl('Statement/Index/' + custId));
    };

    $scope.PT = function () {
        parent.Fanex.ExtAcc.Customer.CurrentLevel = $scope.model.personalInfo.roleId.value;
        parent.Fanex.ExtAcc.Customer.UserId = $scope.model.personalInfo.custId.value;
        parent.Fanex.ExtAcc.Customer.superId = parent.Fanex.ExtAcc.Customer.masterId = parent.Fanex.ExtAcc.Customer.agentId = $scope.model.personalInfo.roleId.value == 4 ? userId : $scope.model.personalInfo.uplineId.value;
        parent.Fanex.ExtAcc.Customer.IsMultiple = false;
        parent.Fanex.ExtAcc.Customer.IsFullInfo = true;
        parent.Fanex.ExtAcc.Customer.MessageClosed = 'Are you sure you want to change Close status for this customer? Warning, all downlines will be closed as well!';
        parent.Fanex.ExtAcc.Customer.MessageUnClosed = 'Are you sure you want to change Close status for this customer?';
        parent.Fanex.ExtAcc.Customer.MessageSuspended = 'Are you sure you want to change Suspend status for this customer? Warning, all members under this agent can\'t place bet!';
        parent.Fanex.ExtAcc.Customer.MessageUnSuspended = 'Are you sure you want to change Suspend status for this customer? All downlines will be un-suspended as well.';
        
        parent.Fanex.ExtAcc.Customer.ProcessConfirmClosedStatus = function (id) {
            if (parent.Fanex.ExtAcc.Customer.CurrentLevel != 4) {
                var timeout = setTimeout(function () {
                    clearTimeout(timeout);

                    parent.Fanex.ExtAcc.Customer.Closed = parent.document.getElementById(id ? id : 'closed').checked;

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
                    parent.Fanex.ExtAcc.Customer.Suspended = parent.document.getElementById(id ? id : 'suspended').checked;

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

        parent.Fanex.ExtAcc.LoadMainFrame(parent.resolveClientUrl('Sportsbook/EditMultiplePT'));

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

function DownlineListController($scope, $http) {
    if ($scope.$parent && $scope.$parent.$parent && $scope.$parent.$parent.model) {
        $scope.downlines = $scope.$parent.$parent.model.downlineList;
        $scope.model = $scope.downlines.downlines;
        $scope.pages = $scope.downlines.pages;
        $scope.maxHeight = ($scope.pages.length > 1 || $scope.model.length > 8) ? '187px' : 'auto';
        $('#downlines').css('max-height', $scope.maxHeight);
    }
    else {
        $scope.model = null;
        $scope.maxHeight = 'auto';
    }

    $scope.Init = function () {
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
                $scope.maxHeight = ($scope.pages.length > 1 || $scope.model.length > 8) ? '187px' : 'auto';

                $('#downlines').scrollTop(0);
                $('#downlines').css('max-height', $scope.maxHeight);

                var timeout = setTimeout(function () {
                    clearTimeout(timeout);

                    for (var i = 1; i <= $scope.pages.length; i++) {
                        $('#link-' + i).removeClass('pageSelected');
                    }

                    $('#link-' + $scope.downlines.currentPage).addClass('pageSelected');
                }, 250);

                parent.Fanex.ExtAcc.UnBlockUI();
            });
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

    $scope.ResetPassword = function (custId, title) {
        var roleId = model.personalInfo.roleId.value;
        parent.Fanex.ExtAcc.Form.Show(parent.resolveClientUrl('Customer/ResetPassword/?custId=' + custId), 'Reset Password: <font color=\'#6dcff6\'>' + title + '</font>', 415, (roleId != 1 ? 400 : 350));
    }
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
};