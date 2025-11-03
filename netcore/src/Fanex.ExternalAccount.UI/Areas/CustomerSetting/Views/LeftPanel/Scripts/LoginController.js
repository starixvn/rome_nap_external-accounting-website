function LoginController($scope) {
    if ($scope.$parent.model) {
        $scope.loginInfo = $scope.$parent.model.login;
        $scope.model = $scope.$parent.model;
    }
    else {
        $scope.model = null;
    }
    
    $(window).on('orientationchange', function (e) {
        var calculatePopupheight = top.shouldCalculatePopupHeight && isMobile.phone;
        if (!calculatePopupheight) {
            return;
        }
        var height;
        var isLandscape = window.matchMedia("(orientation: landscape)").matches;
        if (isMobile.android.phone) {
            height = isLandscape ? top.portraitHeight : top.landscapeHeight;
        }
        else {
            height = isLandscape ? top.landscapeHeight : top.portraitHeight;
        }

        var dialog = top.$('.ui-dialog');
        dialog.height((height + 34) + 'px');
        top.$("#windowPopup").css("height", height + "px");
    });
    $scope.getPopupHeight = function () {
        var height = isMobile.phone && window.matchMedia("(orientation: landscape)").matches ?
             top.landscapeHeight :
             top.portraitHeight;

        return height;
    };

    $scope.ResetPassword = function (custId, title) {
        top.landscapeHeight = 240;
        top.portraitHeight = 400;
        top.shouldCalculatePopupHeight = true;
        var roleId = model.personalInfo.roleId.value;
        parent.Fanex.ExtAcc.Form.Show(
            parent.resolveClientUrl('Customer/ResetPassword/?custId=' + custId + '&groupName=' + parent.$('#selectedGroupName').html()), 'Reset Password: <font color=\'#6dcff6\'>' + title + '</font>',
            415,
            $scope.getPopupHeight(),
            false,
            false);
    };

    $scope.ShowIp = function (ip) {
        top.shouldCalculatePopupHeight = false;
        parent.Fanex.ExtAcc.Customer.ShowIpInfo(ip);
    };

    $scope.ResetSecurityCode = function (id, roleId, username) {
        var url = parent.resolveClientUrl('CustomerSetting/LeftPanel/SearchCustomer/GetSecurityCode?custId=' + id + '&roleId=' + roleId + '&username=' + username + '&groupName=' + parent.$('#selectedGroupName').html());
        top.landscapeHeight = 240;
        top.portraitHeight = 450;
        top.shouldCalculatePopupHeight = true;
        parent.Fanex.ExtAcc.Form.ShowFrame(
            url,
            'Reset security code: <font color="#6dcff6">' + username + '</font>',
            650,
            $scope.getPopupHeight(),
            false,
            false);
    };

    $scope.RemoveOTP = function (custId, username) {
        var url = parent.resolveClientUrl('CustomerSetting/LeftPanel/SearchCustomer/RemoveOTP?custId=' + custId + '&username=' + username + '&groupName=' + parent.$('#selectedGroupName').html());
        parent.Fanex.ExtAcc.LoadMainIFrame(url);
    };

    $scope.collapseSidebar = function () {
        var widthDevice = top.$(top.window).width();
        if (widthDevice < 1024) {
            top.$("#contentSplitter").removeClass("collapse-content").addClass("expand-content");
            top.$("#sidebar").removeClass("expand-sidebar").addClass("collapse-sidebar");
            top.$("#leftSplitter").removeClass("expand-leftpanel").addClass("collapse-leftpanel");
            top.$("#iconNav").removeClass("active");
            top.$("#iconUserInfo").removeClass("active");
        }
        else {
            top.$("#contentSplitter").addClass("collapse-content");
        }
    };
};