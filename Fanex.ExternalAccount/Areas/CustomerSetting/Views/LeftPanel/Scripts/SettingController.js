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
        parent.Fanex.ExtAcc.Form.ShowFrame(url, 'Update Colossus Bets Status: <font color="#6dcff6">' + $scope.model.personalInfo.currentLevel.value + '</font>', 400, 150, false, function () {
            document.getElementById(id).checked = enable;
        }, 1, null);
    };

    $scope.EnableCockfighting = function () {
        var url_SELECT = 'customersetting/enablestatus/productstatus?';
        var query = 'custid=' + $scope.model.personalInfo.custId.value + '&uplineid=' + ($scope.model.personalInfo.roleId.value != 4 ? $scope.model.personalInfo.uplineId.value : 0) +
            '&userid=' + parent.userId + '&productid=cockfighting&level=' + $scope.model.personalInfo.roleId.value +
            '&enable=true&enabledownlines=true&hasdownline=' + ($scope.model.hasDownline ? 'true' : 'false') + '&username=' + $scope.model.personalInfo.currentLevel.value + '&enableuplines=true';
        var url = parent.resolveClientUrl(url_SELECT + query);
        parent.Fanex.ExtAcc.Form.ShowFrame(url, 'Enable Cockfighting: <font color="#6dcff6">' + $scope.model.personalInfo.currentLevel.value + '</font>', 400, 200, false, null, 1, 'enableCF');

    };

    $scope.EnableAllbet = function (enable, id) {
        var url_SELECT = 'customersetting/enablestatus/productstatus?';
        var query = 'custid=' + $scope.model.personalInfo.custId.value + '&uplineid=' + ($scope.model.personalInfo.roleId.value != 4 ? $scope.model.personalInfo.uplineId.value : 0) +
            '&userid=' + parent.userId + '&productid=allbet&level=' + $scope.model.personalInfo.roleId.value +
            '&enable=' + !enable + '&enabledownlines=false&hasdownline=' + ($scope.model.hasDownline ? 'true' : 'false') + '&username=' + $scope.model.personalInfo.currentLevel.value + '&enableuplines=true';
        var url = parent.resolveClientUrl(url_SELECT + query);
        parent.Fanex.ExtAcc.Form.ShowFrame(url, 'Update Allbet Status: <font color="#6dcff6">' + $scope.model.personalInfo.currentLevel.value + '</font>', 400, 150, false, function () {
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

        parent.Fanex.ExtAcc.Customer.IsFullInfo = true;
        parent.Fanex.ExtAcc.Customer.UserId = $scope.model.personalInfo.custId.value;
        parent.Fanex.ExtAcc.Customer.agentId = agentId;
        parent.Fanex.ExtAcc.Customer.masterId = masterId;
        parent.Fanex.ExtAcc.Customer.superId = superId;
        $scope.ResetGlobalValueAtSuperOnMember();
        parent.Fanex.ExtAcc.LoadMainFrame(parent.resolveClientUrl('SportsbookSetting/SuperMasterEditSB?custId=' + $scope.model.personalInfo.custId.value + "&groupName=" + parent.$('#selectedGroupName').html()), null, null, null, true);
    };

    $scope.SuperBingo = function () {
        $scope.ResetGlobalValueAtSuperOnMember();
        var superId = $scope.model.personalInfo.downlines[0].key;
        parent.Fanex.ExtAcc.LoadMainIFrame(parent.resolveClientUrl('CustomerSetting/Bingo/BingoSetting/SuperEditMember?id=' + $scope.model.personalInfo.custId.value + '&uplineid=' + superId + '&groupName=' + parent.$('#selectedGroupName').html()));
    };

    $scope.SuperCasino = function () {
        $scope.ResetGlobalValueAtSuperOnMember();
        var superId = $scope.model.personalInfo.downlines[0].key;
        parent.Fanex.ExtAcc.LoadMainIFrame(parent.resolveClientUrl('CustomerSetting/Casino/Member/EditMemberBySuper/' + $scope.model.personalInfo.custId.value + '/' + superId + '/' + parent.$('#selectedGroupName').html()));
    };

    $scope.SuperRacing = function () {
        $scope.ResetGlobalValueAtSuperOnMember();
        var superId = $scope.model.personalInfo.downlines[0].key;
        parent.Fanex.ExtAcc.LoadMainIFrame(parent.resolveClientUrl('customersetting/racebook/racingsetting/supereditmember?id=' + $scope.model.personalInfo.custId.value + '&uplineid=' + superId + '&groupName=' + parent.$('#selectedGroupName').html()));
    };

    $scope.SuperLiveCasino = function () {
        var superId = $scope.model.personalInfo.downlines[0].key;
        $scope.ResetGlobalValueAtSuperOnMember();
        parent.Fanex.ExtAcc.Customer.UserId = $scope.model.personalInfo.custId.value;
        parent.Fanex.ExtAcc.Customer.superId = superId;
        var url = "LiveCasinoSetting/Setting/EditSetting?uplineId=" + superId + "&parentId=" + $scope.model.personalInfo.uplineId.value + "&uplineLevel=4&customerId=" + $scope.model.personalInfo.custId.value + "&customerLevel=1";
        parent.Fanex.ExtAcc.LoadMainIFrame(parent.resolveClientUrl(url + "&actorId=" + parent.userId + "&actorUsername=" + parent.username + "&groupName=" + parent.$('#selectedGroupName').html()), null, null, null, true);
    };

    $scope.SuperVirtualSports = function () {
        $scope.ResetGlobalValueAtSuperOnMember();
        var superId = $scope.model.personalInfo.downlines[0].key;
        parent.Fanex.ExtAcc.LoadMainFrame(parent.resolveClientUrl('VirtualSports/EditSingle?custId=' + superId + '&roleId=4&targetCustId=' + $scope.model.personalInfo.custId.value + '&targetRoleId=1&groupName=' + parent.$('#selectedGroupName').html()), null, null, null, true);
    };

    $scope.SuperKeno = function () {
        var superId = $scope.model.personalInfo.downlines[0].key;
        parent.Fanex.ExtAcc.Customer.UserId = $scope.model.personalInfo.custId.value;
        parent.Fanex.ExtAcc.Customer.superId = superId;
        $scope.ResetGlobalValueAtSuperOnMember();
        parent.Fanex.ExtAcc.LoadMainIFrame(parent.resolveClientUrl('customersetting/KenoSetting/kenosetting/SuperEditMember?custId=' + $scope.model.personalInfo.custId.value + "&userId=" + parent.userId));
    };

    $scope.SuperGoldDeluxe = function () {
        var superId = $scope.model.personalInfo.downlines[0].key;
        parent.Fanex.ExtAcc.Customer.UserId = $scope.model.personalInfo.custId.value;
        parent.Fanex.ExtAcc.Customer.superId = superId;
        $scope.ResetGlobalValueAtSuperOnMember();
        parent.Fanex.ExtAcc.LoadMainIFrame(parent.resolveClientUrl('GoldDeluxeSetting/GoldDeluxe/EditSetting?upLineLevel=4&formType=1&uplineId=' + superId + '&level=1&custId=' + $scope.model.personalInfo.custId.value + "&actorId=" + parent.userId + "&groupName=" + parent.$('#selectedGroupName').html()));
    };

    $scope.MasterPT = function () {
        var superId = $scope.model.personalInfo.downlines[0].key;
        var masterId = $scope.model.personalInfo.downlines[1].key;
        var agentId = $scope.model.personalInfo.downlines[2].key;

        $scope.ResetGlobalValueAtMasterOnMember();
        parent.Fanex.ExtAcc.Customer.IsFullInfo = true;
        parent.Fanex.ExtAcc.Customer.UserId = $scope.model.personalInfo.custId.value;
        parent.Fanex.ExtAcc.Customer.agentId = agentId;
        parent.Fanex.ExtAcc.Customer.masterId = masterId;
        parent.Fanex.ExtAcc.Customer.superId = superId;
        parent.Fanex.ExtAcc.LoadMainFrame(parent.resolveClientUrl('SportsbookSetting/SuperMasterEditSB?custId=' + $scope.model.personalInfo.custId.value + "&groupName=" + parent.$('#selectedGroupName').html()), null, null, null, true);
    };

    $scope.MasterBingo = function () {
        $scope.ResetGlobalValueAtMasterOnMember();
        var masterId = $scope.model.personalInfo.downlines[1].key;
        parent.Fanex.ExtAcc.LoadMainIFrame(parent.resolveClientUrl('CustomerSetting/Bingo/BingoSetting/MasterEditMember?id=' + $scope.model.personalInfo.custId.value + '&uplineid=' + masterId + '&groupName=' + parent.$('#selectedGroupName').html()));
    };

    $scope.MasterCasino = function () {
        $scope.ResetGlobalValueAtMasterOnMember();
        var masterId = $scope.model.personalInfo.downlines[1].key;
        parent.Fanex.ExtAcc.LoadMainIFrame(parent.resolveClientUrl('CustomerSetting/Casino/Member/EditMemberByMaster/' + $scope.model.personalInfo.custId.value + '/' + masterId + '/' + parent.$('#selectedGroupName').html()));
    };

    $scope.MasterRacing = function () {
        $scope.ResetGlobalValueAtMasterOnMember();
        var masterId = $scope.model.personalInfo.downlines[1].key;
        parent.Fanex.ExtAcc.LoadMainIFrame(parent.resolveClientUrl('customersetting/racebook/racingsetting/mastereditmember?id=' + $scope.model.personalInfo.custId.value + '&uplineid=' + masterId + '&groupName=' + parent.$('#selectedGroupName').html()));
    };

    $scope.MasterVirtualSports = function () {
        $scope.ResetGlobalValueAtMasterOnMember();
        parent.Fanex.ExtAcc.Customer.CurrentLevel = 1;
        var masterId = $scope.model.personalInfo.downlines[1].key;
        parent.Fanex.ExtAcc.LoadMainFrame(parent.resolveClientUrl('VirtualSports/EditSingle?custId=' + masterId + '&roleId=3&targetCustId=' + $scope.model.personalInfo.custId.value + '&targetRoleId=1&groupName=' + parent.$('#selectedGroupName').html()), null, null, null, true);
    };

    $scope.MasterKeno = function () {
        $scope.ResetGlobalValueAtMasterOnMember();
        var masterId = $scope.model.personalInfo.downlines[1].key;
        parent.Fanex.ExtAcc.LoadMainIFrame(parent.resolveClientUrl('customersetting/KenoSetting/kenosetting/MasterEditMember?custId=' + $scope.model.personalInfo.custId.value + "&userId=" + parent.userId));
    };

    $scope.MasterLiveCasino = function () {
        var masterId = $scope.model.personalInfo.downlines[1].key;
        $scope.ResetGlobalValueAtMasterOnMember();
        parent.Fanex.ExtAcc.Customer.UserId = $scope.model.personalInfo.custId.value;
        parent.Fanex.ExtAcc.Customer.masterId = masterId;
        var url = "LiveCasinoSetting/Setting/EditSetting?uplineId=" + masterId + "&parentId=" + $scope.model.personalInfo.uplineId.value + "&uplineLevel=3&customerId=" + $scope.model.personalInfo.custId.value + "&customerLevel=1";
        parent.Fanex.ExtAcc.LoadMainIFrame(parent.resolveClientUrl(url + "&actorId=" + parent.userId + "&actorUsername=" + parent.username + "&groupName=" + parent.$('#selectedGroupName').html()), null, null, null, true);
    };

    $scope.MasterGoldDeluxe = function () {
        var masterId = $scope.model.personalInfo.downlines[1].key;
        parent.Fanex.ExtAcc.Customer.UserId = $scope.model.personalInfo.custId.value;
        parent.Fanex.ExtAcc.Customer.masterId = masterId;
        $scope.ResetGlobalValueAtMasterOnMember();
        parent.Fanex.ExtAcc.LoadMainIFrame(parent.resolveClientUrl('GoldDeluxeSetting/GoldDeluxe/EditSetting?upLineLevel=3&formType=1&uplineId=' + masterId + '&level=1&custId=' + $scope.model.personalInfo.custId.value + "&actorId=" + parent.userId + "&groupName=" + parent.$('#selectedGroupName').html()));
    };
    
    $scope.SuperSBPT = function () {
        parent.Fanex.ExtAcc.Customer.IsFullInfo = false;
        var superId = $scope.model.personalInfo.downlines[0].key;
        var masterId = $scope.model.personalInfo.downlines[1].key;
        var agentId = $scope.model.personalInfo.downlines[2].key;

        parent.Fanex.ExtAcc.Customer.UserId = $scope.model.personalInfo.custId.value;
        parent.Fanex.ExtAcc.Customer.agentId = agentId;
        parent.Fanex.ExtAcc.Customer.masterId = masterId;
        parent.Fanex.ExtAcc.Customer.superId = superId;
        $scope.ResetGlobalValueAtSuperOnMember();

        parent.Fanex.ExtAcc.LoadMainFrame(parent.resolveClientUrl('SportsbookSetting/EditPT?custId=' + $scope.model.personalInfo.custId.value + "&groupName=" + parent.$('#selectedGroupName').html()), null, null, null, true);
    };

    $scope.MasterSBPT = function () {
        parent.Fanex.ExtAcc.Customer.IsFullInfo = false;
        var superId = $scope.model.personalInfo.downlines[0].key;
        var masterId = $scope.model.personalInfo.downlines[1].key;
        var agentId = $scope.model.personalInfo.downlines[2].key;

        parent.Fanex.ExtAcc.Customer.UserId = $scope.model.personalInfo.custId.value;
        parent.Fanex.ExtAcc.Customer.agentId = agentId;
        parent.Fanex.ExtAcc.Customer.masterId = masterId;
        parent.Fanex.ExtAcc.Customer.superId = superId;
        $scope.ResetGlobalValueAtMasterOnMember();

        parent.Fanex.ExtAcc.LoadMainFrame(parent.resolveClientUrl('SportsbookSetting/EditPT?custId=' + $scope.model.personalInfo.custId.value + "&groupName=" + parent.$('#selectedGroupName').html()), null, null, null, true);
    };

    $scope.SuperSportsbook2 = function () {
        $scope.RenderSupeOnMemberDCS(16);
    };

    $scope.SuperCockfighting = function () {
        $scope.RenderSupeOnMemberDCS(19);
    };

    $scope.SuperCricket = function () {
        $scope.RenderSupeOnMemberDCS(20);
    };

    $scope.SuperAllbet = function () {
        $scope.RenderSupeOnMemberDCS(21);
    };

    $scope.RenderSupeOnMemberDCS = function (productId) {
        $scope.ResetGlobalValueAtSuperOnMember();
        parent.Fanex.ExtAcc.LoadMainIFrame(parent.resolveClientUrl('DCS/ViewCustomerSetting?language=en-US&productId=' + productId + '&customerLevel=1&UplineLevel=4&customerId=' + $scope.model.personalInfo.custId.value + "&groupName=" + parent.$('#selectedGroupName').html()));
    };

    $scope.MasterSportsbook2 = function () {
        $scope.RenderMasterOnMemberDCS(16);
    };

    $scope.MasterCockfighting = function () {
        $scope.RenderMasterOnMemberDCS(19);
    };

    $scope.MasterCricket = function () {
        $scope.RenderMasterOnMemberDCS(20);
    };

    $scope.MasterAllbet = function () {
        $scope.RenderMasterOnMemberDCS(21);
    };

    $scope.RenderMasterOnMemberDCS = function (productId) {
        $scope.ResetGlobalValueAtMasterOnMember();
        parent.Fanex.ExtAcc.LoadMainIFrame(parent.resolveClientUrl('DCS/ViewCustomerSetting?language=en-US&productId=' + productId + '&customerLevel=1&UplineLevel=3&customerId=' + $scope.model.personalInfo.custId.value + "&groupName=" + parent.$('#selectedGroupName').html()));
    };

    $scope.ResetGlobalValueAtMasterOnMember = function (productId) {
        parent.Fanex.ExtAcc.Customer.CurrentLevel = 1;
        parent.Fanex.ExtAcc.Customer.isSuperEditMember = false;
        parent.Fanex.ExtAcc.Customer.isMasterEditMember = true;
        parent.Fanex.ExtAcc.Customer.IsMultiple = false;
    }

    $scope.ResetGlobalValueAtSuperOnMember = function (productId) {
        parent.Fanex.ExtAcc.Customer.CurrentLevel = 1;
        parent.Fanex.ExtAcc.Customer.isSuperEditMember = true;
        parent.Fanex.ExtAcc.Customer.isMasterEditMember = false;
        parent.Fanex.ExtAcc.Customer.IsMultiple = false;
    }
};