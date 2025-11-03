function ResetInactiveAccountCredit(uplineId, viewId) {
    Fanex.ExtAcc.Form.ConfirmBox('Reset Member Credit',
        'Are you sure you want to reset all closed account credit?',
        400,
        150,
        false,
        function () {
            Fanex.ExtAcc.BlockUI();

            $.ajax({
                url: resolveClientUrl('CustomerSetting/LeftPanel/UpdateCustomer/ResetMemberCredit'),
                type: 'post',
                data: {
                    uplineId: uplineId,
                    closedOnly: true,
                    groupName: parent.$('#selectedGroupName').html()
                }
            })
            .success(function (result) {
                Fanex.ExtAcc.UnBlockUI();
                Fanex.ExtAcc.Form.Close(null, null, 'confirmClosedAccCredit');

                if (result.isSuccess) {
                    Fanex.ExtAcc.BlockUI();

                    var url = resolveClientUrl(`CustomerSetting/LeftPanel/SearchCustomer/UsedCreditPage?custId=${uplineId}&roleId=${viewId}&groupName=${parent.$('#selectedGroupName').html()}`);
                    $.ajax({
                        url: url,
                        type: 'post'
                    })
                    .success(function (view) {
                        Fanex.ExtAcc.UnBlockUI();

                        $('#contentSplitter').html(view);
                    });
                }
                else {
                    Fanex.ExtAcc.Form.Alert('Reset Member Credit Error', result.errorMessage.formatErrorMessage(), 400, 150, 'alertClosedCredit');
                }
            });
        },
        null,
        'confirmClosedAccCredit');
}

function ResetAllAccountCredit(uplineId, viewId) {
    Fanex.ExtAcc.Form.ConfirmBox('Reset Member Credit',
        'Are you sure you want to reset all account credit?',
        400,
        150,
        false,
        function () {
            $.ajax({
                url: resolveClientUrl('CustomerSetting/LeftPanel/UpdateCustomer/ResetMemberCredit'),
                type: 'post',
                data: {
                    uplineId: uplineId,
                    closedOnly: false,
                    groupName: parent.$('#selectedGroupName').html()
                }
            })
            .success(function (result) {
                Fanex.ExtAcc.UnBlockUI();
                Fanex.ExtAcc.Form.Close(null, null, 'confirmAllAccCredit');

                if (result.isSuccess) {
                    Fanex.ExtAcc.BlockUI();

                    var url = resolveClientUrl(`CustomerSetting/LeftPanel/SearchCustomer/UsedCreditPage?custId=${uplineId}&roleId=${viewId}&groupName=${parent.$('#selectedGroupName').html()}`);
                    $.ajax({
                        url: url,
                        type: 'post'
                    })
                    .success(function (view) {
                        Fanex.ExtAcc.UnBlockUI();

                        $('#contentSplitter').html(view);
                    });
                }
                else {
                    Fanex.ExtAcc.Form.Alert('Reset Member Credit Error', result.errorMessage.formatErrorMessage(), 400, 150, 'alertAllCredit');
                }
            });
        },
        null,
        'confirmAllAccCredit');
};