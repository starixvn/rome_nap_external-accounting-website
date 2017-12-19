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
        parent.Fanex.ExtAcc.Form.Show(parent.resolveClientUrl('Customer/ResetPassword/?custId=' + custId + '&groupName=' + parent.$('#selectedGroupName').html()), 'Reset Password: <font color=\'#6dcff6\'>' + title + '</font>', 415, (roleId != 1 ? 400 : 400));
    };

    $scope.ShowIp = function (ip) {
        parent.Fanex.ExtAcc.Customer.ShowIpInfo(ip);
    };

    $scope.ResetSecurityCode = function (id, roleId, username) {
        var url = parent.resolveClientUrl('CustomerSetting/LeftPanel/SearchCustomer/GetSecurityCode?id=' + id + '&roleId=' + roleId + '&username=' + username + '&groupName=' + parent.$('#selectedGroupName').html());
        parent.Fanex.ExtAcc.Form.ShowFrame(
            url,
            'Reset security code: <font color="#6dcff6">' + username + '</font>',
            650,
            500,
            false);
    };

    $scope.RemoveOTP = function (custId, username) {
        var url = parent.resolveClientUrl('CustomerSetting/LeftPanel/SearchCustomer/RemoveOTP?custId=' + custId + '&username=' + username + '&groupName=' + parent.$('#selectedGroupName').html());
        parent.Fanex.ExtAcc.LoadMainIFrame(url);
    };
};