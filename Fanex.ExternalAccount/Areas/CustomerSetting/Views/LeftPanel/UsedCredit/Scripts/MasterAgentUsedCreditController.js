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
                    closedOnly: true
                }
            })
            .success(function (result) {
                Fanex.ExtAcc.UnBlockUI();
                Fanex.ExtAcc.Form.Close();

                if (result.isSuccess) {
                    Fanex.ExtAcc.BlockUI();

                    var url = resolveClientUrl('CustomerSetting/LeftPanel/SearchCustomer/UsedCreditPage?id=' + agentId + '&roleId=2');
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
                    Fanex.ExtAcc.Form.Alert('Reset Member Credit Error', result.errorMessage.formatErrorMessage());
                }
            })
            .error(function (data) {
                Fanex.ExtAcc.UnBlockUI();

                if (typeof console != 'undefined') {
                    console.log(data);
                }
            });
        });
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
                    closedOnly: false
                }
            })
            .success(function (result) {
                Fanex.ExtAcc.UnBlockUI();
                Fanex.ExtAcc.Form.Close();

                if (result.isSuccess) {
                    Fanex.ExtAcc.BlockUI();

                    var url = resolveClientUrl('CustomerSetting/LeftPanel/SearchCustomer/UsedCreditPage?id=' + agentId + '&roleId=2');
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
                    Fanex.ExtAcc.Form.Alert('Reset Member Credit Error', result.errorMessage.formatErrorMessage());
                }
            })
            .error(function (data) {
                Fanex.ExtAcc.UnBlockUI();

                if (typeof console != 'undefined') {
                    console.log(data);
                }
            });
        });
};