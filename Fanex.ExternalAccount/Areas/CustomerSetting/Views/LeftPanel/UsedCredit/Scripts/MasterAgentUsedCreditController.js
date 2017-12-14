function ResetInactiveAccountCredit(agentId) {
    Fanex.ExtAcc.Form.ConfirmBox('Reset Member Credit',
        'Are you sure you want to reset all closed account credit?',
        400,
        150,
        false,
        function () {
            Fanex.ExtAcc.BlockUI();

            $.ajax({
                url: resolveClientUrl('CustomerSetting/LeftPanel/SearchCustomer/ResetMemberCredit'),
                type: 'post',
                data: {
                    agentId: agentId,
                    closedOnly: true,
                    groupName: parent.$('#selectedGroupName').html()
                }
            })
            .success(function (result) {
                Fanex.ExtAcc.UnBlockUI();
                Fanex.ExtAcc.Form.Close(null, null, 'confirmClosedAccCredit');

                if (result.isSuccess) {
                    Fanex.ExtAcc.BlockUI();

                    var url = resolveClientUrl('CustomerSetting/LeftPanel/SearchCustomer/UsedCreditPage?custId=' + agentId + '&roleId=2&groupName=' + parent.$('#selectedGroupName').html());
                    $.ajax({
                        url: url,
                        type: 'post'
                    })
                    .success(function (view) {
                        Fanex.ExtAcc.UnBlockUI();

                        $('#contentSplitter').html(view);
                    })
                    .error(function (data) {
                        Fanex.ExtAcc.UnBlockUI();

                        if (typeof console != 'undefined') {
                            console.log(data);
                        }
                    });
                }
                else {
                    Fanex.ExtAcc.Form.Alert('Reset Member Credit Error', result.errorMessage.formatErrorMessage(), 400, 150, 'alertClosedCredit');
                }
            })
            .error(function (data) {
                Fanex.ExtAcc.UnBlockUI();

                if (typeof console != 'undefined') {
                    console.log(data);
                }
            });
        },
        null,
        'confirmClosedAccCredit');
}

function ResetAllAccountCredit(agentId) {
    Fanex.ExtAcc.Form.ConfirmBox('Reset Member Credit',
        'Are you sure you want to reset all account credit?',
        400,
        150,
        false,
        function () {
            $.ajax({
                url: resolveClientUrl('CustomerSetting/LeftPanel/SearchCustomer/ResetMemberCredit'),
                type: 'post',
                data: {
                    agentId: agentId,
                    closedOnly: false,
                    groupName: parent.$('#selectedGroupName').html()
                }
            })
            .success(function (result) {
                Fanex.ExtAcc.UnBlockUI();
                Fanex.ExtAcc.Form.Close(null, null, 'confirmAllAccCredit');

                if (result.isSuccess) {
                    Fanex.ExtAcc.BlockUI();

                    var url = resolveClientUrl('CustomerSetting/LeftPanel/SearchCustomer/UsedCreditPage?custId=' + agentId + '&roleId=2&groupName=' + parent.$('#selectedGroupName').html());
                    $.ajax({
                        url: url,
                        type: 'post'
                    })
                    .success(function (view) {
                        Fanex.ExtAcc.UnBlockUI();

                        $('#contentSplitter').html(view);
                    })
                    .error(function (data) {
                        Fanex.ExtAcc.UnBlockUI();

                        if (typeof console != 'undefined') {
                            console.log(data);
                        }
                    });
                }
                else {
                    Fanex.ExtAcc.Form.Alert('Reset Member Credit Error', result.errorMessage.formatErrorMessage(), 400, 150, 'alertAllCredit');
                }
            })
            .error(function (data) {
                Fanex.ExtAcc.UnBlockUI();

                if (typeof console != 'undefined') {
                    console.log(data);
                }
            });
        },
        null,
        'confirmAllAccCredit');
};