function LeftPanelController($scope, $element) {
    $scope.model = model;

    $scope.GetCustomerList = function () {
        var roleId = $scope.model.personalInfo.roleId.value;
        var custId = $scope.model.personalInfo.custId.value;
        parent.Fanex.ExtAcc.Customer.CustomerList.cachedParams.breadcrumbs = {
            SuperId: $scope.model.personalInfo.downlines[0] ? $scope.model.personalInfo.downlines[0].key : 0,
            MasterId: $scope.model.personalInfo.downlines[1] ? $scope.model.personalInfo.downlines[1].key : 0,
            AgentId: $scope.model.personalInfo.downlines[2] ? $scope.model.personalInfo.downlines[2].key : 0
        };

        parent.Fanex.ExtAcc.Customer.superId = $scope.model.personalInfo.downlines[0] ? $scope.model.personalInfo.downlines[0].key : 0;
        parent.Fanex.ExtAcc.Customer.masterId= $scope.model.personalInfo.downlines[1] ? $scope.model.personalInfo.downlines[1].key : 0;
        parent.Fanex.ExtAcc.Customer.agentId= $scope.model.personalInfo.downlines[2] ? $scope.model.personalInfo.downlines[2].key : 0;
        parent.Fanex.ExtAcc.LoadMainFrame(parent.resolveClientUrl('CustomerList/Index') + '?custid=' + custId + '&roleId=' + roleId, null, false, true);
    };

    $scope.GetStatementList = function (custId) {
        parent.Fanex.ExtAcc.LoadMainFrame(parent.resolveClientUrl('Statement/Index?id=' + custId + '&isinternal=0&username=' + $scope.model.personalInfo.currentLevel.value), null, null, true);
    };

    $scope.PT = function () {
        parent.Fanex.ExtAcc.Customer.CurrentLevel = $scope.model.personalInfo.roleId.value;
        parent.Fanex.ExtAcc.Customer.UserId = $scope.model.personalInfo.custId.value;
        parent.Fanex.ExtAcc.Customer.superId = parent.Fanex.ExtAcc.Customer.masterId = parent.Fanex.ExtAcc.Customer.agentId = $scope.model.personalInfo.roleId.value === 4 ? userId : $scope.model.personalInfo.uplineId.value;

        parent.Fanex.ExtAcc.Customer.IsFullInfo = true;
        parent.Fanex.ExtAcc.Customer.isSuperEditAgent = false;
        $scope.ResetGlobalValue();

        parent.Fanex.ExtAcc.LoadMainFrame(parent.resolveClientUrl('SportsbookSetting/EditMultiplePT' + "?roleId=" + $scope.model.personalInfo.roleId.value + "&custId=" + $scope.model.personalInfo.custId.value + "&groupName=" + parent.$('#selectedGroupName').html()), null, null, true);
        $scope.collapseSidebar();
    };

    $scope.getPopupHeight = function () {
        var height = isMobile.phone && window.matchMedia("(orientation: landscape)").matches ?
            $scope.landscapeHeight :
            $scope.portraitHeight;

        return height;
    };

    $scope.Casino = function () {
        $scope.ResetGlobalValue();

        var roleId = $scope.model.personalInfo.roleId.value;
        var custId = $scope.model.personalInfo.custId.value;
        var url = '';

        switch (roleId) {
            case 4:
                url = parent.resolveClientUrl('CustomerSetting/Casino/Super/EditSuper?id=' + $scope.model.personalInfo.custId.value + '&uplineid=0');
                break;
            case 3:
                url = parent.resolveClientUrl('CustomerSetting/Casino/Master/EditMaster?id=' + $scope.model.personalInfo.custId.value + '&uplineid=' + $scope.model.personalInfo.uplineId.value);
                break;
            case 2:
                url = parent.resolveClientUrl('CustomerSetting/Casino/Agent/EditAgent?id=' + $scope.model.personalInfo.custId.value + '&uplineid=' + $scope.model.personalInfo.uplineId.value);
                break;
            case 1:
                url = parent.resolveClientUrl('CustomerSetting/Casino/Member/EditMember?id=' + $scope.model.personalInfo.custId.value + '&uplineid=' + $scope.model.personalInfo.uplineId.value);
                break;
            default:
                break;
        }

        if (url !== '') {
            parent.Fanex.ExtAcc.LoadMainIFrame(url + '&groupname=' + parent.$('#selectedGroupName').html());
        }

        $scope.collapseSidebar();
    };

    $scope.BetSetting = function () {
        var custId = $scope.model.personalInfo.custId.value;
        var param = 'custId=' + custId + '&uplineId=' + custId + '&userId=' + parent.userId + '&groupName=' + parent.$('#selectedGroupName').html();
        var url = parent.resolveClientUrl('CustomerSetting/BetSetting/Sportsbook/Index?' + param);

        parent.Fanex.ExtAcc.LoadMainIFrame(url);
        $scope.collapseSidebar();
    };

    $scope.VirtualSports = function () {
        $scope.RenderDCS(5);
        $scope.collapseSidebar();
    };

    $scope.Keno = function () {
        $scope.ResetGlobalValue();

        var roleId = $scope.model.personalInfo.roleId.value;
        var custId = $scope.model.personalInfo.custId.value;
        var uplineId = roleId === 4 ? userId : $scope.model.personalInfo.uplineId.value;
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

        $scope.collapseSidebar();
    };

    $scope.GoldDeluxe = function () {
        $scope.ResetGlobalValue();

        var roleId = $scope.model.personalInfo.roleId.value;
        var custId = $scope.model.personalInfo.custId.value;
        var uplineId = roleId === 4 ? userId : $scope.model.personalInfo.uplineId.value;
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

        parent.Fanex.ExtAcc.LoadMainIFrame(parent.resolveClientUrl(url));

        $scope.collapseSidebar();
    };

    $scope.Sportsbook2 = function () {
        $scope.RenderDCS(16);
        $scope.collapseSidebar();
    };

    $scope.LiveCasino = function () {
        $scope.RenderDCS(41);
        $scope.collapseSidebar();
    };

    $scope.VoidBridge = function () {
        $scope.RenderDCS(22);
        $scope.collapseSidebar();
    };

    $scope.NG2 = function () {
        $scope.RenderDCS(23);
        $scope.collapseSidebar();
    };

    $scope.FishingWorld = function () {
        $scope.RenderDCS(24);
        $scope.collapseSidebar();
    };

    $scope.RNGLottery = function () {
        $scope.RenderDCS(44);
        $scope.collapseSidebar();
    };

    $scope.VirtualGames = function () {
        $scope.RenderDCS(38);
        $scope.collapseSidebar();
    };

    $scope.Sabaclub = function () {
        $scope.RenderDCS(42);
        $scope.collapseSidebar();
    };

    $scope.RenderDCS = function (productId) {
        $scope.ResetGlobalValue();

        var roleId = $scope.model.personalInfo.roleId.value;
        var custId = $scope.model.personalInfo.custId.value;
        var uplineId = roleId === 4 ? userId : $scope.model.personalInfo.uplineId.value;
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

        parent.Fanex.ExtAcc.LoadMainIFrame(parent.resolveClientUrl(url));
    };

    $scope.ResetGlobalValue = function (productId) {
        parent.Fanex.ExtAcc.Customer.IsMultiple = false;
        parent.Fanex.ExtAcc.Customer.isSuperEditMember = false;
        parent.Fanex.ExtAcc.Customer.isMasterEditMember = false;
    }

    $scope.collapseSidebar = function () {
        var widthDevice = top.$(top.window).width();
        if (widthDevice <= 768) {

            top.$("#contentSplitter").removeClass("collapse-content").addClass("expand-content");
            top.$("#sidebar").removeClass("expand-sidebar").addClass("collapse-sidebar");
            top.$("#leftSplitter").removeClass("expand-leftpanel").addClass("collapse-leftpanel");
            top.$("#iconNav").removeClass("active");
            top.$("#iconUserInfo").removeClass("active");
        }
        else {
            top.$("#contentSplitter").addClass("collapse-content");
        }
    }
};