function BalanceController($scope, $http) {
    if ($scope.$parent && $scope.$parent.$parent && $scope.$parent.$parent.model) {
        $scope.balanceInfo = $scope.$parent.$parent.model.balanceInfo;
        $scope.rewardInfo = $scope.$parent.$parent.model.rewardInfo;
        $scope.model = $scope.balanceInfo;
    }
    else {
        $scope.model = null;
    }

    $scope.EditCredit = function () {
        var value = $('#linkGivenCredit').html();
        $('#linkGivenCredit').hide();
        $('#givenCredit').val(value.replaceAll(',', '').toFixed(2).formatNumber());
        $('#givenCredit').show();
        $('#givenCredit').focus();
    };

    $scope.CreditDetail = function () {
        var id = $scope.personalInfo.custId.value;
        var roleId = $scope.personalInfo.roleId.value;

        parent.Fanex.ExtAcc.BlockUI();

        $.ajax({
            url: parent.resolveClientUrl('CustomerSetting/LeftPanel/SearchCustomer/UsedCreditPage?custId=' + id + '&roleId=' + roleId + '&groupName=' + parent.$('#selectedGroupName').html()),
            method: 'post'
        })
            .success(function (result) {
                parent.Fanex.ExtAcc.UnBlockUI();

                parent.$('#contentSplitter').html(result);
            })
            .error(function (data) {
                parent.Fanex.ExtAcc.UnBlockUI();
            });
    };

    $scope.UpdateCredit = function (event) {
        var keyCode = event.keyCode;
        var value = event.target.value;
        var id = $scope.personalInfo.custId.value;
        var uplineId = userId;

        if (keyCode == 13 && value != '') {
            parent.Fanex.ExtAcc.BlockUI();
            value = value.replace(/,/g, "");


            $.ajax({
                url: parent.resolveClientUrl('CustomerSetting/LeftPanel/UpdateCustomer/UpdateGivenCreditPost?custId=' + id + '&uplineId=' + uplineId + '&newGivenCredit=' + value + '&groupName=' + parent.$('#selectedGroupName').html()),
                method: 'post'
            })
                .success(function (result) {
                    parent.Fanex.ExtAcc.UnBlockUI();

                    if (parent.Fanex.ExtAcc.ShowSecurityCodePage(result)) {
                        return;
                    }

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
                });
        }
    };

    $scope.RevertCredit = function (preVal) {
        $('#linkGivenCredit').show();
        $('#givenCredit').val(preVal.replaceAll(',', '').toFixed(2).formatNumber());
        $('#givenCredit').hide();
    };
}