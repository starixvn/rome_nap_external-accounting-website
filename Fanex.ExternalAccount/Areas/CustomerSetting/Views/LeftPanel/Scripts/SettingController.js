function SettingController($scope, $http) {
    if ($scope.$parent.model) {
        $scope.setting = $scope.$parent.model.setting;
        $scope.model = $scope.$parent.model;
    }
    else {
        $scope.model = null;
    }

    $scope.landscapeHeight;
    $scope.portraitHeight;
    $scope.shouldCalculatePopupHeight;

    $scope.GetOnline = function (custId) {
        if (model.onlineLoaded) return;

        parent.Fanex.ExtAcc.BlockUI();
        var antiXSRFToken = document.getElementsByName("__RequestVerificationToken");
        var token = "";
        if (antiXSRFToken.length > 0) {
            token = antiXSRFToken[0].value;
        }

        $http({
            url: parent.resolveClientUrl('CustomerSetting/LeftPanel/SearchCustomer/GetOnlineList?custId=' + custId),
            method: 'post',
            headers: { "X-Requested-With": "XMLHttpRequest", "__RequestVerificationToken": token }
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
                    var item = '<tr><td class="percent10">' + (i + 1) + '</td>' +
                                    '<td class="percent32">' +
                                        '<a href="javascript:;" onclick="parent.Fanex.ExtAcc.Customer.ShowIpInfo(\'' + online.iP.text + '\')">' +
                                            online.iP.text +
                                        '</a>' +
                                    '</td>' +
                                '<td>' + online.loginTime.text + '</td></tr>';
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
            parent.Fanex.ExtAcc.UnBlockUI();
        })
    };

    $scope.Init = function () {
        if ($scope.setting != null) {
            var groupName = parent.$('#selectedGroupName').html();
            var allowAdmins = eval(parent.allowAdminEditSuperStatus);
            for (var i = 0; i < $scope.setting.details.length; i++) {
                if (($scope.setting.details[i].key == 'Transfer' && $scope.model.personalInfo.roleId.value == 1) ||
                    ($scope.setting.details[i].key == 'BetSetting' && parent.showBetSetting == '0')) {
                    $scope.setting.details[i].isHide = true;
                }
            }

            for (var i = 0; i < $scope.setting.editItems.length; i++) {
                if ($scope.setting.editItems[i].key == 'EnableKeno' && parent.enableKeno == '0') {
                    $scope.setting.editItems[i].isHide = true;
                }
            }

            var syncCasino = true;            

            for (var j = 0; j < $scope.setting.syncItems.length; j++) {
                if (!$scope.setting.syncItems[j].value && $scope.setting.syncItems[j].key == 'SyncCasino') {
                    syncCasino = false;
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

            $(window).bind('orientationchange', function (e) {
                var calculatePopupheight = $scope.shouldCalculatePopupHeight && isMobile.phone;
                if (!calculatePopupheight) {
                    return;
                }

                var height;
                var isLandscape = window.matchMedia("(orientation: landscape)").matches;
                if (isMobile.android.phone) {
                    height = isLandscape ? $scope.portraitHeight : $scope.landscapeHeight;
                }
                else {
                    height = isLandscape ? $scope.landscapeHeight : $scope.portraitHeight;
                }

                var dialog = top.$('.ui-dialog');
                dialog.height((height + 34) + 'px');

                top.$("#windowPopup").css("height", height + "px");
            });

        }
    };

    $scope.getPopupHeight = function () {
        var height = isMobile.phone && window.matchMedia("(orientation: landscape)").matches ?
             $scope.landscapeHeight :
             $scope.portraitHeight;

        return height;
    }

    $scope.AddNew = function () {
        var roleId = $scope.model.personalInfo.roleId.value;
        var actionName = '';
        parent.Fanex.ExtAcc.Customer.IsMultiple = false;

        switch (roleId) {
            case 2:
                actionName = 'Customer/AgentAddNewMember/?currencyId=' + $scope.model.currencyId + '&custId=' + $scope.model.personalInfo.custId.value + '&username=' + $scope.model.personalInfo.currentLevel.value + "&groupName=" + parent.$('#selectedGroupName').html();
                break;
            case 3:
                actionName = 'Customer/MasterAddNewAgent/?currencyId=' + $scope.model.currencyId + '&custId=' + $scope.model.personalInfo.custId.value + '&username=' + $scope.model.personalInfo.currentLevel.value + "&groupName=" + parent.$('#selectedGroupName').html();
                break;
            case 4:
                actionName = 'Customer/SuperAddNewMaster/?currencyId=' + $scope.model.currencyId + '&custId=' + $scope.model.personalInfo.custId.value + '&username=' + $scope.model.personalInfo.currentLevel.value + "&groupName=" + parent.$('#selectedGroupName').html();
                break;
            default:
                break;
        }

        if (actionName != '') {
            parent.Fanex.ExtAcc.BlockUI();
            parent.Fanex.ExtAcc.LoadMainFrame(parent.resolveClientUrl(actionName), null, null, null, true);
        }
        $scope.collapseSidebar();
    };

    $scope.DoubleComm = function (allow, id, disabled) {
        if (disabled) {
            return;
        }

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

    $scope.Closed = function (close, id, disabled) {
        if (disabled) {
            return;
        }

        $scope.shouldCalculatePopupHeight = false;
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
                        url: parent.resolveClientUrl('CustomerSetting/LeftPanel/UpdateCustomer/UpdateCloseCutomer'),
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
                            url: parent.resolveClientUrl('CustomerSetting/LeftPanel/UpdateCustomer/UpdateCloseCutomer'),
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

    $scope.Suspended = function (suspend, id, disabled) {
        if (disabled) {
            return;
        }

        $scope.shouldCalculatePopupHeight = false;
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
                        url: parent.resolveClientUrl('CustomerSetting/LeftPanel/UpdateCustomer/UpdateSuspendCutomer'),
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

    $scope.Outright = function (allow, id, disabled) {
        if (disabled) {
            return;
        }

        $scope.shouldCalculatePopupHeight = false;
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
                url: parent.resolveClientUrl('CustomerSetting/LeftPanel/UpdateCustomer/UpdateOutright'),
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

    $scope.AutoPT = function (allow, id, disabled) {
        if (disabled) {
            return;
        }

        $scope.shouldCalculatePopupHeight = false;
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
                    url: parent.resolveClientUrl('CustomerSetting/LeftPanel/UpdateCustomer/UpdateAutoMPT'),
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

    $scope.EnableCasino = function (enable, id, disabled) {
        if (disabled) {
            return;
        }
        $scope.shouldCalculatePopupHeight = false;
        document.getElementById(id).checked = enable;
        $scope.EnableThirdpartyProduct(6, "Casino", 400, 180);
    };

    $scope.EnableNumberGame = function (enable, id, disabled) {
        if (disabled) {
            return;
        }

        $scope.shouldCalculatePopupHeight = false;
        var url_SELECT = 'customersetting/enablestatus/productstatus?';
        var query = 'custid=' + $scope.model.personalInfo.custId.value + '&uplineid=' + ($scope.model.personalInfo.roleId.value != 4 ? $scope.model.personalInfo.uplineId.value : 0) +
        '&userid=' + parent.userId + '&productid=numbergame&level=' + $scope.model.personalInfo.roleId.value +
        '&enable=' + !enable + '&enabledownlines=false&hasdownline=' + ($scope.model.hasDownline ? 'true' : 'false') + '&username=' + $scope.model.personalInfo.currentLevel.value + '&enableuplines=true';
        var url = parent.resolveClientUrl(url_SELECT + query);
        parent.Fanex.ExtAcc.Form.ShowFrame(url, (enable ? 'Disable ' : 'Enable ') + 'Number Game: <font color="#6dcff6">' + $scope.model.personalInfo.currentLevel.value + '</font>', 400, 180, false, function () {
            document.getElementById(id).checked = enable;
        }, 1, null);
    };
    

        $scope.shouldCalculatePopupHeight = false;

    $scope.EnableVirtualSports = function (enable, id, disabled) {
        if (disabled) {
            return;
        }

        $scope.shouldCalculatePopupHeight = false;
        document.getElementById(id).checked = enable;
        $scope.EnableThirdpartyProduct(5, "Virtual Sports", 400, 180);
    };

    $scope.EnableESports = function (enable, id, disabled) {
        if (disabled) {
            return;
        }

        $scope.shouldCalculatePopupHeight = false;
        var url_SELECT = 'customersetting/enablestatus/productstatus?';
        var query = 'custid=' + $scope.model.personalInfo.custId.value + '&uplineid=' + ($scope.model.personalInfo.roleId.value != 4 ? $scope.model.personalInfo.uplineId.value : 0) +
        '&userid=' + parent.userId + '&productid=esports&level=' + $scope.model.personalInfo.roleId.value +
        '&enable=' + !enable + '&enabledownlines=false&hasdownline=' + ($scope.model.hasDownline ? 'true' : 'false') + '&username=' + $scope.model.personalInfo.currentLevel.value + '&enableuplines=true';
        var url = parent.resolveClientUrl(url_SELECT + query);
        parent.Fanex.ExtAcc.Form.ShowFrame(url, (enable ? 'Disable ' : 'Enable ') + 'eSports: <font color="#6dcff6">' + $scope.model.personalInfo.currentLevel.value + '</font>', 400, 180, false, function () {
            document.getElementById(id).checked = enable;
        }, 1, null);
    };   

        $scope.shouldCalculatePopupHeight = false;

    $scope.getPopupHeight = function () {
        var height = isMobile.phone && window.matchMedia("(orientation: landscape)").matches ?
             $scope.landscapeHeight :
             $scope.portraitHeight;

        return height;
    }



    $scope.EnableKeno = function (enable, id, disabled) {
        if (disabled) {
            return;
        }

        $scope.landscapeHeight = 250;
        $scope.portraitHeight = 400;
        $scope.shouldCalculatePopupHeight = true;
        $scope.EnableThirdpartyProduct(8, "Keno", 400, $scope.getPopupHeight());
    };

    $scope.EnableColossusBets = function (enable, id, disabled) {
        if (disabled) {
            return;
        }
        $scope.shouldCalculatePopupHeight = false;
        document.getElementById(id).checked = enable;
        $scope.EnableThirdpartyProduct(15, "Colossus Bets", 400, 180);
    };

    $scope.EnableFishingWorld = function (enable, id, disabled) {
        if (disabled) {
            return;
        }

        $scope.shouldCalculatePopupHeight = false;
        document.getElementById(id).checked = enable;
        $scope.EnableThirdpartyProduct(24, "Fishing World", 400, 180);
    };

    $scope.EnableGoldDeluxe = function (enable, id, disabled) {
        if (disabled) {
            return;
        }

        $scope.shouldCalculatePopupHeight = false;
        document.getElementById(id).checked = enable;
        $scope.EnableThirdpartyProduct(12, "Gold Deluxe", 400, 180);
    };   

        
        $scope.shouldCalculatePopupHeight = false;

    $scope.EnableAllbet = function (enable, id, disabled) {
        if (disabled) {
            return;
        }

        $scope.shouldCalculatePopupHeight = false;
        $scope.EnableThirdpartyProduct(21, "Allbet", 400, 200);
    };

    $scope.EnableVoidBridge = function (enable, id, disabled) {
        if (disabled) {
            return;
        }

        $scope.shouldCalculatePopupHeight = false;
        document.getElementById(id).checked = enable;
        $scope.EnableThirdpartyProduct(22, "Macau Games", 400, 180);
    };

    $scope.EnableNG2 = function (enable, id, disabled) {
        if (disabled) {
            return;
        }

        $scope.shouldCalculatePopupHeight = false;
        document.getElementById(id).checked = enable;
        $scope.EnableThirdpartyProduct(23, parent.ng2Text, 400, 180);
    };

    $scope.EnableLottery = function (enable, id, disabled) {
        if (disabled) {
            return;
        }

        $scope.shouldCalculatePopupHeight = false;
        document.getElementById(id).checked = enable;
        $scope.EnableThirdpartyProduct(28, "Lottery", 400, 180);
    };

    $scope.EnableLucky3 = function (enable, id, disabled) {
        if (disabled) {
            return;
        }

        $scope.shouldCalculatePopupHeight = false;
        document.getElementById(id).checked = enable;
        $scope.EnableThirdpartyProduct(26, "Lucky 3", 400, 180);
    };

    $scope.EnableBullfighting = function (enable, id, disabled) {
        if (disabled) {
            return;
        }

        $scope.shouldCalculatePopupHeight = false;
        document.getElementById(id).checked = enable;
        $scope.EnableThirdpartyProduct(30, "Bullfighting", 400, 180);
    };

    $scope.EnableDCS = function (productId, productName, enable, id, productText, width, height) {
        var url_SELECT = productId == 24 ? 'customersetting/enablestatus/productstatus/GetProductStatusNotAllowDisabledDownline?' : 'customersetting/enablestatus/productstatus?';
        var query = 'custid=' + $scope.model.personalInfo.custId.value + '&uplineid=' + ($scope.model.personalInfo.roleId.value != 4 ? $scope.model.personalInfo.uplineId.value : 0) +
            '&userid=' + parent.userId + '&product=' + productId + '&productid=' + productName + '&level=' + $scope.model.personalInfo.roleId.value +
            '&enable=' + enable + '&enabledownlines=false&hasdownline=' + ($scope.model.hasDownline ? 'true' : 'false') + '&username=' + $scope.model.personalInfo.currentLevel.value + '&enableuplines=true';
        var url = parent.resolveClientUrl(url_SELECT + query);
        var name = productText || productName;
        parent.Fanex.ExtAcc.Form.ShowFrame(url, 'Update ' + name + ' Status: <font color="#6dcff6">' + $scope.model.personalInfo.currentLevel.value + '</font>', (width ? width : 400), (height ? height : 180), false, function () {
            try {
                document.getElementById(id).checked = enable;
            } catch (e) {

            }
        }, 1, null);
    };

    $scope.EnableThirdpartyProduct = function (productId, productName, width, height) {
        var url_SELECT = 'ThirdPartyProductStatus\?';
        var query = 'custid=' + $scope.model.personalInfo.custId.value + '&custname=' + $scope.model.personalInfo.currentLevel.value + '&roleid=' + $scope.model.personalInfo.roleId.value + '&productid=' + productId;
        var url = parent.resolveClientUrl(url_SELECT + query);
        parent.Fanex.ExtAcc.Form.ShowFrame(url, 'Update ' + productName + ' Status: <font color="#6dcff6">' + $scope.model.personalInfo.currentLevel.value + '</font>', width, height, false, null, 1, null);
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
        $scope.collapseSidebar();
    };
    
        $scope.collapseSidebar();
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

    $scope.SuperCasino = function () {
        $scope.ResetGlobalValueAtSuperOnMember();
        var superId = $scope.model.personalInfo.downlines[0].key;
        parent.Fanex.ExtAcc.LoadMainIFrame(parent.resolveClientUrl('CustomerSetting/Casino/Member/EditMemberBySuper?id=' + $scope.model.personalInfo.custId.value + '&uplineid=' + superId + '&groupname=' + parent.$('#selectedGroupName').html()));
    };

    $scope.SuperVirtualSports = function () {
        $scope.RenderSupeOnMemberDCS(5);
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

    $scope.MasterCasino = function () {
        $scope.ResetGlobalValueAtMasterOnMember();
        var masterId = $scope.model.personalInfo.downlines[1].key;
        parent.Fanex.ExtAcc.LoadMainIFrame(parent.resolveClientUrl('CustomerSetting/Casino/Member/EditMemberByMaster?id=' + $scope.model.personalInfo.custId.value + '&uplineid=' + masterId + '&groupname=' + parent.$('#selectedGroupName').html()));
    };    

    $scope.MasterVirtualSports = function () {
        $scope.RenderMasterOnMemberDCS(5);
    };

    $scope.MasterKeno = function () {
        $scope.ResetGlobalValueAtMasterOnMember();
        var masterId = $scope.model.personalInfo.downlines[1].key;
        parent.Fanex.ExtAcc.LoadMainIFrame(parent.resolveClientUrl('customersetting/KenoSetting/kenosetting/MasterEditMember?custId=' + $scope.model.personalInfo.custId.value + "&userId=" + parent.userId));
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

    $scope.SuperCricket = function () {
        $scope.RenderSupeOnMemberDCS(20);
    };

    $scope.SuperAllbet = function () {
        $scope.RenderSupeOnMemberDCS(21);
    };

    $scope.SuperVoidBridge = function () {
        $scope.RenderSupeOnMemberDCS(22);
    };

    $scope.SuperNG2 = function () {
        $scope.RenderSupeOnMemberDCS(23);
    };

    $scope.SuperFishingWorld = function () {
        $scope.RenderSupeOnMemberDCS(24);
    };

    $scope.SuperLottery = function () {
        $scope.RenderSupeOnMemberDCS(28);
    };

    $scope.SuperLucky3 = function () {
        $scope.RenderSupeOnMemberDCS(26);
    };

    $scope.SuperBullfighting = function () {
        $scope.RenderSupeOnMemberDCS(30);
    };

    $scope.RenderSupeOnMemberDCS = function (productId) {
        $scope.ResetGlobalValueAtSuperOnMember();
        parent.Fanex.ExtAcc.LoadMainIFrame(parent.resolveClientUrl('DCS/ViewCustomerSetting?language=en-US&productId=' + productId + '&customerLevel=1&UplineLevel=4&customerId=' + $scope.model.personalInfo.custId.value + "&groupName=" + parent.$('#selectedGroupName').html()));
    };

    $scope.MasterSportsbook2 = function () {
        $scope.RenderMasterOnMemberDCS(16);
    };    

    $scope.MasterCricket = function () {
        $scope.RenderMasterOnMemberDCS(20);
    };

    $scope.MasterAllbet = function () {
        $scope.RenderMasterOnMemberDCS(21);
    };

    $scope.MasterVoidBridge = function () {
        $scope.RenderMasterOnMemberDCS(22);
    };

    $scope.MasterNG2 = function () {
        $scope.RenderMasterOnMemberDCS(23);
    };

    $scope.MasterFishingWorld = function () {
        $scope.RenderMasterOnMemberDCS(24);
    };

    $scope.MasterLottery = function () {
        $scope.RenderMasterOnMemberDCS(28);
    };

    $scope.MasterLucky3 = function () {
        $scope.RenderMasterOnMemberDCS(26);
    };

    $scope.MasterBullfighting = function () {
        $scope.RenderMasterOnMemberDCS(30);
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