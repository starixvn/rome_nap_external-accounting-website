function CasinoController(userId, username, isMultiple, custIds, usernames, syncStatuses) {

    var isAutoReset = customerModel.isAutoReset;
    var CustomerModel = customerModel;
    var SettingModel = customerSetting;
    var CustomerInfo = { id: custId, uplineId: uplineId, userId: userId, username: username };
    var IsMultiple = isMultiple;
    var CustIds = custIds;
    var Usernames = usernames;
    var SyncStatuses = syncStatuses;
    var ErrorMessage = '';
    var SuccessMessage = '';
    var actionName = '';
    CustomerModel.casinoEnabled = CustomerModel.casinoEnabled = !CustomerModel.casinoDisabled && (CustomerModel.casinoSync || !CustomerModel.uplineDisabledCasino);

    if (isMultiple) {
        if (!custIds) {
            alert('Please input custIds parameter is array');
        }
        else {
            CustIds = custIds;
        }

        if (!usernames) {
            alert('Please input usernames parameter is array');
        }
        else {
            Usernames = usernames;
        }
    }

    if (IsMultiple) {
        jQuery('#headerUserInfo').remove();
        jQuery('#userInfo').remove();
        $('#statusInfo').attr('style', 'display: block');

        var names = "";
        jQuery(Usernames).each(function (index, username) {
            names += username + ", ";
        });

        Fanex.CustomerSetting.Core.ProcessDisplayMultiple(125, names);
    }
    else {
        $('#arrowUpDown').hide();
        $('#statusInfo').hide();
    }

    this.Update = function (ondone) {
        $(document).scrollTop(0);
        actionName = ActionName();

        var min1 = parseFloat(Fanex.CustomerSetting.Core.ReplaceAll($('#minCredit1').html(), ',', ''));
        var max1 = parseFloat(Fanex.CustomerSetting.Core.ReplaceAll($('#maxCredit1').html(), ',', ''));
        var cur1 = parseFloat(Fanex.CustomerSetting.Core.ReplaceAll($('#maxTransfer1').val(), ',', ''));
        max1 = isNaN(max1) ? 0 : max1;

        //var min = parseFloat(Fanex.CustomerSetting.Core.ReplaceAll($('#minCredit').html(), ',', ''));
        //var max = parseFloat(Fanex.CustomerSetting.Core.ReplaceAll($('#maxCredit').html(), ',', ''));
        //var cur = parseFloat(Fanex.CustomerSetting.Core.ReplaceAll($('#maxTransfer').val(), ',', ''));
        //max = isNaN(max) ? 0 : max;

        var isSuperMasterOnMember = (CustomerModel.uplineRoleId == 4 || CustomerModel.uplineRoleId == 3) && CustomerModel.roleId == 1;
        if (!isSuperMasterOnMember && (isNaN(cur1) || cur1 < min1 || (max1 != 0 && cur1 > max1))) {
            Validators.show($('#maxTransfer1').get(0), 'Maximum Loss must be between ' + $('#minCredit1').html() + ' and ' + $('#maxCredit1').html(), undefined, undefined, 1500);
            return;
        }

        var stakeLevelCheckboxes = $('.chk-stake-level');
        if (isBelongToSpecialGroups && $('input:checked', stakeLevelCheckboxes).length === 0) {
            Validators.show(stakeLevelCheckboxes.get(0), 'Each bet type must have at least one Bet Limit value to be selected.', undefined, undefined, 3000);
            return;
        }

        //if (isNaN(cur) || cur < min || (max != 0 && cur > max)) {
        //    Validators.show($('#maxTransfer').get(0), 'Member Max Transfer Credit must be between ' + $('#minCredit').html() + ' and ' + $('#maxCredit').html(), undefined, undefined, 1500);
        //    return;
        //}

        Fanex.CustomerSetting.Core.BlockUI();

        if (IsMultiple) {
            ProcessUpdate(0, ondone);
        }
        else {
            ProcessUpdate(null, ondone);
        }
    };

    var ActionName = function () {
        var actionName = '';

        switch (UplineRoleId) {
            case 5:
                actionName = 'Super/UpdateSuperPost';
                break;
            case 4:
                if (IsMaster) {
                    actionName = 'Master/UpdateMasterPost';
                }
                else if (IsMember) {
                    actionName = 'Member/UpdateMemberBySuperPost';
                }
                break;
            case 3:
                if (IsAgent) {
                    actionName = 'Agent/UpdateAgentPost';
                }
                else if (IsMember) {
                    actionName = 'Member/UpdateMemberByMasterPost';
                }
                break;
            case 2:
                actionName = 'Member/UpdateMemberPost';
                break;
            default:
                break;
        }

        return actionName;
    };

    var ProcessUpdate = function (custIdIndex, ondone) {
        CustomerModel.isAutoReset = isAutoReset;
        var isSync = SyncStatuses[custIdIndex] == 1 ? true : false;
        CustomerModel.casinoSync = custIdIndex == null ? CustomerModel.casinoSync : isSync;
        var data = {
            id: CustomerInfo.id,
            uplineId: CustomerInfo.uplineId,
            userId: CustomerInfo.userId,
            actorName: CustomerInfo.username,
            settingModel: JSON.stringify(SettingModel),
            customerModel: JSON.stringify(CustomerModel),
            groupName: parent.$('#selectedGroupName').html()
        };

        data.id = custIdIndex == null ? CustomerInfo.id : CustIds[custIdIndex];
        data.username = custIdIndex == null ? CustomerModel.userName : Usernames[custIdIndex];

        $.ajax({
            url: toUrl(actionName),
            data: JSON.stringify(data),
            contentType: "application/json; charset=utf-8",
            type: 'post',
            dataType: 'json'
        })
            .success(function (result) {
                if (result.errorCode != 0) {
                    ErrorMessage += '<font color="Blue" style="font-weight: bold">' + data.username + ":</font><br />" + result.errorMessage.formatErrorMessage() + '<br /><br />';
                }
                else {
                    SuccessMessage += '<font color="Blue" style="font-weight: bold">' + data.username + ':</font><br /><font color="Blue">Updated successfully</font><br /><br />';
                }

                if (!IsMultiple) {
                    if (ErrorMessage != '') {
                        $('#errMsg').html(ErrorMessage + SuccessMessage);
                        $('#errMsg').show();

                        if (typeof ondone == 'function') {
                            ondone(1, ErrorMessage + SuccessMessage);
                        }

                        ErrorMessage = '';
                        SuccessMessage = '';
                        Fanex.CustomerSetting.Core.UnBlockUI();

                        return false;
                    }
                    else {
                        $('#errMsg').attr('class', 'successMsg');
                        $('#errMsg').html(SuccessMessage);
                        $('#errMsg').show();

                        if (typeof ondone == 'function') {
                            ondone(0, SuccessMessage);
                        }

                        ErrorMessage = '';
                        SuccessMessage = '';
                        Fanex.CustomerSetting.Core.UnBlockUI();

                        return true;
                    }
                }
                else {
                    if (custIdIndex == CustIds.length - 1) {
                        if (ErrorMessage != '') {
                            $('#errMsg').html(ErrorMessage + SuccessMessage);
                            $('#errMsg').show();

                            if (typeof ondone == 'function') {
                                ondone(1, ErrorMessage + SuccessMessage);
                            }

                            ErrorMessage = '';
                            SuccessMessage = '';
                            Fanex.CustomerSetting.Core.UnBlockUI();

                            return false;
                        }
                        else {
                            $('#errMsg').attr('class', 'successMsg');
                            $('#errMsg').html(SuccessMessage);
                            $('#errMsg').show();

                            if (typeof ondone == 'function') {
                                ondone(0, SuccessMessage);
                            }

                            ErrorMessage = '';
                            SuccessMessage = '';
                            Fanex.CustomerSetting.Core.UnBlockUI();

                            return true;
                        }
                    }
                    else {
                        ProcessUpdate(++custIdIndex, ondone);
                    }
                }
            })
            .error(function (response) {
                ErrorMessage += '<div style="margin-bottom: 5px;"><font color="Blue" style="font-weight: bold">' + data.username + ":</font><br /><font color='red'> Not casino's Customer. </font></div>";
                if (!IsMultiple) {
                    $('#errMsg').html(ErrorMessage + SuccessMessage);
                    $('#errMsg').show();

                    if (typeof ondone == 'function') {
                        ondone(1, ErrorMessage + SuccessMessage);
                    }

                    ErrorMessage = '';
                    SuccessMessage = '';
                    Fanex.CustomerSetting.Core.UnBlockUI();

                    return false;
                }
                else {
                    if (custIdIndex == CustIds.length - 1) {
                        $('#errMsg').attr('class', 'errMsg').html(ErrorMessage + SuccessMessage);
                        $('#errMsg').show();

                        if (typeof ondone == 'function') {
                            ondone(1, ErrorMessage);
                        }

                        ErrorMessage = '';
                        SuccessMessage = '';
                        Fanex.CustomerSetting.Core.UnBlockUI();

                        return false;
                    }
                    else {
                        ProcessUpdate(++custIdIndex, ondone);
                    }
                }
            });
    }
}