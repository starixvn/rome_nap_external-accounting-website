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
                var syncCasino = false;
                var syncLiveCasino = false;
                var isHideKeno = true;
                var isHideVirtualSport = true;
                var isHideGoldDeluxe = true;
                var isHideColossusBets = true;
                var isHideSportsbook2 = true;
                var isHideCockfighting = true;
                var isHideCricket = true;
                var isHideAllbet = true;
                var isHideVoidbridge = true;
                var isHideNG2 = true;
                var isHideFishingWorld = true;
                var isHideLottery = true;

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

                    if ($scope.model.setting.syncItems[i].key == 'SyncLiveCasino') {
                        syncLiveCasino = $scope.model.setting.syncItems[i].value;
                    }

                    if ($scope.model.setting.syncItems[i].key == 'HideAllbet') {
                        isHideAllbet = $scope.model.setting.syncItems[i].value;
                    }

                    if ($scope.model.setting.syncItems[i].key == 'HideVoidbridge') {
                        isHideVoidbridge = $scope.model.setting.syncItems[i].value;
                    }

                    if ($scope.model.setting.syncItems[i].key == 'HideNG2') {
                        isHideNG2 = $scope.model.setting.syncItems[i].value;
                    }
                    
                    if ($scope.model.setting.syncItems[i].key == 'HideFishingWorld') {
                        isHideFishingWorld = $scope.model.setting.syncItems[i].value;
                    }

                    if ($scope.model.setting.syncItems[i].key == 'HideLottery') {
                        isHideLottery = $scope.model.setting.syncItems[i].value;
                    }
                }

                for (var i = 0; i < $scope.model.setting.details.length; i++) {
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
                    if ($scope.model.setting.details[i].key == 'Sportsbook2') {
                        isHideSportsbook2 = false;
                    }
                    if ($scope.model.setting.details[i].key == 'Cockfighting') {
                        isHideCockfighting = false;
                    }
                    if ($scope.model.setting.details[i].key == 'Cricket') {
                        isHideCricket = false;
                    }
                }
                
                switch (roleId) {
                    case 4:
                        parent.Fanex.ExtAcc.Customer.GetMasterList(custId, allowDoubleComm, null, null, syncCasino, syncLiveCasino, isHideKeno, isHideVirtualSport, isHideGoldDeluxe, isHideColossusBets, isHideSportsbook2, isHideCockfighting, isHideCricket, isHideAllbet, isHideVoidbridge, isHideNG2, isHideFishingWorld, isHideLottery);
                        break;
                    case 3:
                        parent.Fanex.ExtAcc.Customer.GetAgentList(custId, allowDoubleComm, null, null, syncCasino, syncLiveCasino, isHideKeno, isHideVirtualSport, isHideGoldDeluxe, isHideColossusBets, isHideSportsbook2, isHideCockfighting, isHideCricket, isHideAllbet, isHideVoidbridge, isHideNG2, isHideFishingWorld, isHideLottery);
                        break;
                    case 2:
                        parent.Fanex.ExtAcc.Customer.GetMemberList(custId, allowDoubleComm, null, null, syncCasino, syncLiveCasino, isHideKeno, isHideVirtualSport, isHideGoldDeluxe, isHideColossusBets, isHideSportsbook2, isHideCockfighting, isHideCricket, isHideAllbet, isHideVoidbridge, isHideNG2, isHideFishingWorld, isHideLottery);
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

        parent.Fanex.ExtAcc.Customer.IsFullInfo = true;
        parent.Fanex.ExtAcc.Customer.isSuperEditAgent = false;
        $scope.ResetGlobalValue();

        parent.Fanex.ExtAcc.LoadMainFrame(parent.resolveClientUrl('SportsbookSetting/EditMultiplePT' + "?roleId=" + $scope.model.personalInfo.roleId.value + "&custId=" + $scope.model.personalInfo.custId.value + "&groupName=" + parent.$('#selectedGroupName').html()), null, null, null, true);
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
        $scope.ResetGlobalValue();

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

        $scope.ResetGlobalValue();
        parent.Fanex.ExtAcc.Customer.CurrentLevel = roleId;
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
        $scope.ResetGlobalValue();

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

    $scope.VirtualSports = function () {
        $scope.RenderDCS(5);
    };

    $scope.Keno = function () {
        $scope.ResetGlobalValue();

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
        $scope.ResetGlobalValue();

        var roleId = $scope.model.personalInfo.roleId.value;
        var custId = $scope.model.personalInfo.custId.value;
        var uplineId = roleId == 4 ? userId : $scope.model.personalInfo.uplineId.value;
        var url = "GoldDeluxeSetting/GoldDeluxe/EditSetting?actorId=" + parent.userId + "&custId=" + $scope.model.personalInfo.custId.value + "&groupName=" + parent.$('#selectedGroupName').html();
        switch (roleId) {
            case 4:
                url = url + "&upLineLevel=0&uplineId=0&formType=2&level=4";
                break;
            case 3:
                url = url + "&upLineLevel=4&formType=1&level=3" + "&uplineId=" + $scope.model.personalInfo.uplineId.value;
                break;
            case 2:
                url = url + "&upLineLevel=3&formType=1&level=2" + "&uplineId=" + $scope.model.personalInfo.uplineId.value;
                break;
            case 1:
                url = url + "&upLineLevel=2&formType=1&level=1" + "&uplineId=" + $scope.model.personalInfo.uplineId.value;
                break;
            default:
                break;
        }

        parent.Fanex.ExtAcc.LoadMainIFrame(parent.resolveClientUrl(url), null, null, null, true);
    };

    $scope.ColossusBets = function () {
        $scope.ResetGlobalValue();

        var roleId = $scope.model.personalInfo.roleId.value;
        var custId = $scope.model.personalInfo.custId.value;
        var uplineId = roleId == 4 ? userId : $scope.model.personalInfo.uplineId.value;
        var url = parent.resolveClientUrl('customersetting/ColossusBetSetting/ColossusBetSetting/EditSetting?custid=' + custId + '&isMultipleEdit=0&userId=' + parent.userId + '&roleId=' + roleId + '&groupName=' + parent.$('#selectedGroupName').html());

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

        parent.Fanex.ExtAcc.LoadMainIFrame(url);
    };

    $scope.Sportsbook2 = function () {
        $scope.RenderDCS(16);
    };

    $scope.Cockfighting = function () {
        $scope.RenderDCS(19);
    };

    $scope.Cricket = function () {
        $scope.RenderDCS(20);

    };

    $scope.Allbet = function () {
        $scope.RenderDCS(21);
    };
    
    $scope.MacauGames = function () {
        $scope.RenderDCS(22);
    };

    $scope.NG2 = function () {
        $scope.RenderDCS(23);
    };

    $scope.FishingWorld = function () {
        $scope.RenderDCS(24);
    };

    $scope.Lottery = function () {
        $scope.RenderDCS(28);
    };

    $scope.RenderDCS = function (productId) {
        $scope.ResetGlobalValue();

        var roleId = $scope.model.personalInfo.roleId.value;
        var custId = $scope.model.personalInfo.custId.value;
        var uplineId = roleId == 4 ? userId : $scope.model.personalInfo.uplineId.value;
        var url = "DCS/ViewCustomerSetting?language=en-US&productId=" + productId + "&customerId=" + $scope.model.personalInfo.custId.value + "&groupName=" + parent.$('#selectedGroupName').html();
        switch (roleId) {
            case 4:
                url = url + "&customerLevel=4";
                break;
            case 3:
                url = url + "&customerLevel=3";
                break;
            case 2:
                url = url + "&customerLevel=2";
                break;
            case 1:
                url = url + "&customerLevel=1&UplineLevel=2";
                break;
            default:
                break;
        }

        parent.Fanex.ExtAcc.LoadMainIFrame(parent.resolveClientUrl(url), null, null, null, true);
    };

    $scope.ResetGlobalValue = function (productId) {
        parent.Fanex.ExtAcc.Customer.IsMultiple = false;
        parent.Fanex.ExtAcc.Customer.isSuperEditMember = false;
        parent.Fanex.ExtAcc.Customer.isMasterEditMember = false;
    }
};