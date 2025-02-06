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
        parent.Fanex.ExtAcc.Customer.masterId = $scope.model.personalInfo.downlines[1] ? $scope.model.personalInfo.downlines[1].key : 0;
        parent.Fanex.ExtAcc.Customer.agentId = $scope.model.personalInfo.downlines[2] ? $scope.model.personalInfo.downlines[2].key : 0;
        parent.Fanex.ExtAcc.LoadMainFrame(parent.resolveClientUrl('CustomerList/Index') + '?custid=' + custId + '&roleId=' + roleId, null, false, true);
    };

    $scope.GetStatementList = function (custId) {
        parent.Fanex.ExtAcc.LoadMainFrame(parent.resolveClientUrl('Statement/Index?id=' + custId + '&isinternal=0&username=' + $scope.model.personalInfo.currentLevel.value), null, null, true);
    };

    $scope.GetUplineRoleId = function (agentId, roleId) {
        if (agentId == -4) {
            return 4;
        }

        if(agentId == -3) {
            return 3;
        }

        return (roleId + 1);
    };

    $scope.PT = function () {
        parent.Fanex.ExtAcc.Customer.CurrentLevel = $scope.model.personalInfo.roleId.value;
        parent.Fanex.ExtAcc.Customer.UserId = $scope.model.personalInfo.custId.value;
        parent.Fanex.ExtAcc.Customer.superId = parent.Fanex.ExtAcc.Customer.masterId = parent.Fanex.ExtAcc.Customer.agentId = $scope.model.personalInfo.roleId.value === 4 ? userId : $scope.model.personalInfo.uplineId.value;

        parent.Fanex.ExtAcc.Customer.IsFullInfo = true;
        parent.Fanex.ExtAcc.Customer.isSuperEditAgent = false;
        $scope.ResetGlobalValue();
        var agentId = parent.Fanex.ExtAcc.Customer.agentId;
        var roleId = $scope.model.personalInfo.roleId.value;
        var uplineRoleId = $scope.GetUplineRoleId(agentId, roleId);
        var url = parent.resolveClientUrl('Customer/EditCustomer' + "?roleId=" + roleId + "&uplineRoleId=" + uplineRoleId + "&custId=" + $scope.model.personalInfo.custId.value + "&productGroupId=1" + "&groupName=" + parent.$('#selectedGroupName').html() + "&agentId=" + agentId);

        parent.Fanex.ExtAcc.LoadMainIFrame(url, null, null, true);
        $scope.collapseSidebar();
    };

    $scope.LiveAndRNGCasino = function () {
        parent.Fanex.ExtAcc.Customer.CurrentLevel = $scope.model.personalInfo.roleId.value;
        parent.Fanex.ExtAcc.Customer.UserId = $scope.model.personalInfo.custId.value;
        parent.Fanex.ExtAcc.Customer.superId = parent.Fanex.ExtAcc.Customer.masterId = parent.Fanex.ExtAcc.Customer.agentId = $scope.model.personalInfo.roleId.value === 4 ? userId : $scope.model.personalInfo.uplineId.value;

        parent.Fanex.ExtAcc.Customer.IsFullInfo = true;
        parent.Fanex.ExtAcc.Customer.isSuperEditAgent = false;
        $scope.ResetGlobalValue();
        var agentId = parent.Fanex.ExtAcc.Customer.agentId;
        var roleId = $scope.model.personalInfo.roleId.value;
        var uplineRoleId = $scope.GetUplineRoleId(agentId, roleId);
        var url = parent.resolveClientUrl('Customer/EditCustomer' + "?roleId=" + roleId + "&uplineRoleId=" + uplineRoleId + "&custId=" + $scope.model.personalInfo.custId.value + "&productGroupId=3" + "&groupName=" + parent.$('#selectedGroupName').html() + "&agentId=" + agentId);
        parent.Fanex.ExtAcc.LoadMainIFrame(url, null, null, true);
        $scope.collapseSidebar();
    }

    $scope.RNGGames = function () {
        parent.Fanex.ExtAcc.Customer.CurrentLevel = $scope.model.personalInfo.roleId.value;
        parent.Fanex.ExtAcc.Customer.UserId = $scope.model.personalInfo.custId.value;
        parent.Fanex.ExtAcc.Customer.superId = parent.Fanex.ExtAcc.Customer.masterId = parent.Fanex.ExtAcc.Customer.agentId = $scope.model.personalInfo.roleId.value === 4 ? userId : $scope.model.personalInfo.uplineId.value;

        parent.Fanex.ExtAcc.Customer.IsFullInfo = true;
        parent.Fanex.ExtAcc.Customer.isSuperEditAgent = false;
        $scope.ResetGlobalValue();
        var agentId = parent.Fanex.ExtAcc.Customer.agentId;
        var roleId = $scope.model.personalInfo.roleId.value;
        var uplineRoleId = $scope.GetUplineRoleId(agentId, roleId);
        var url = parent.resolveClientUrl('Customer/EditCustomer' + "?roleId=" + roleId + "&uplineRoleId=" + uplineRoleId + "&custId=" + $scope.model.personalInfo.custId.value + "&productGroupId=2" + "&groupName=" + parent.$('#selectedGroupName').html() + "&agentId=" + agentId);
        parent.Fanex.ExtAcc.LoadMainIFrame(url, null, null, true);
        $scope.collapseSidebar();
    }

    $scope.getPopupHeight = function () {
        var height = isMobile.phone && window.matchMedia("(orientation: landscape)").matches ?
            $scope.landscapeHeight :
            $scope.portraitHeight;

        return height;
    };

    $scope.BetSetting = function () {
        var custId = $scope.model.personalInfo.custId.value;
        var roleId = $scope.model.personalInfo.roleId.value;
        var uplineId = roleId === 4 ? userId : $scope.model.personalInfo.uplineId.value;
        var param = 'custId=' + custId + '&roleId=' + roleId + '&userId=' + parent.userId + '&uplineId=' + uplineId + '&groupName=' + parent.$('#selectedGroupName').html();
        var url = parent.resolveClientUrl('CustomerSetting/BetSetting/Sportsbook/Index?' + param);

        parent.Fanex.ExtAcc.LoadMainIFrame(url);
        $scope.collapseSidebar();
    };

    $scope.VirtualSports = function () {
        $scope.RenderDCS(5);
        $scope.collapseSidebar();
    };

    $scope.RNGLottery = function () {
        $scope.RenderDCS(44);
        $scope.collapseSidebar();
    };

    $scope.RenderDCS = function (productId) {
        $scope.ResetGlobalValue();

        var roleId = $scope.model.personalInfo.roleId.value;
        var custId = $scope.model.personalInfo.custId.value;
        var uplineId = roleId === 4 ? userId : $scope.model.personalInfo.uplineId.value;
        var uplineRoleId = roleId + 1;

        if (productId === 41) {
            parent.Fanex.ExtAcc.LoadMainIFrame(parent.resolveClientUrl('Customer/EditCustomer?custId=' + custId + "&roleId=" + roleId + "&uplineRoleId=" + uplineRoleId + "&productGroupId=3" + "&groupName=" + parent.$('#selectedGroupName').html()));
            return;
        }

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