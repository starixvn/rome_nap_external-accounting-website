function Check(checkbox) {
    if (!checkbox.checked) {
        $('#all').get(0).checked = false;
    }
};

function CheckAll() {
    var checkboxes = $('input[type=checkbox]');
    var checked = false;

    checkboxes.each(function (index, item) {
        if (item.id == 'all' && item.checked) {
            checked = true;
        }

        if (item.id != 'all' && !item.disabled) {            
            item.checked = checked;
        }
    });    
};

function Update() {
    var checkboxes = $('input[type=checkbox]');
    var custIds = '';
    var isCheckedAll = false;

    checkboxes.each(function (index, item) {
        if (item.id != 'all' && item.checked) {
            custIds += item.id + ',';
        }
        if(item.id == 'all' && item.checked){
            isCheckedAll = true;
        }
    });

    if (custIds != '') {
        custIds = custIds.substring(0, custIds.length - 1);

        Fanex.ExtAcc.Form.ConfirmBox('Transfer', 'Full multiple transfer will reset max transfer limit<br/>Are you sure you want to transfer checked members?',
            410,
            150,
            false,
            function () {
                Fanex.ExtAcc.BlockUI();
                $.ajax({
                    url: resolveClientUrl('CustomerSetting/LeftPanel/UpdateCustomer/UpdateClearBalance'),
                    data: {
                        custId: custId,
                        custIds: custIds,
                        isCheckedAll: isCheckedAll,
                        groupName: parent.$('#selectedGroupName').html()
                    },
                    type: 'post'
                }).success(function (result) {
                    Fanex.ExtAcc.Form.Close(null, null, 'confirmTransfer');
                    if (result.isSuccess) {
                        var timeout = setTimeout(function () {
                            clearTimeout(timeout);
                            $.ajax({
                                url: parent.resolveClientUrl('CustomerSetting/LeftPanel/SearchCustomer/SelectClearBalance?custId=' + custId + '&groupName=' + parent.$('#selectedGroupName').html()),
                                type: 'post'
                            })
                            .success(function (result) {
                                Fanex.ExtAcc.UnBlockUI();
                                $('#contentSplitter').html(result);
                            });
                        }, 2000);
                    }
                    else {
                        Fanex.ExtAcc.UnBlockUI();
                        var msg = '<div id="err-message" class="errMsg">'+result.errorMessage.formatErrorMessage()+'</div>';
                        Fanex.ExtAcc.Form.Alert('Error', msg, 410, 200, 'errorTransfer');
                    }
                });
            },
            null,
            'confirmTransfer');
    }
};