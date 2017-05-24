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

        var antiXSRFToken = document.getElementsByName("__RequestVerificationToken");
        $http.defaults.headers.post.__RequestVerificationToken = antiXSRFToken[0].value;

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
            var antiXSRFToken = document.getElementsByName("__RequestVerificationToken");
            $http.defaults.headers.post.__RequestVerificationToken = antiXSRFToken[0].value;

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