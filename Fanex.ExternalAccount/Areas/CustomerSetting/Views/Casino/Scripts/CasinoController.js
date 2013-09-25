function CasinoController(userId, username, isMultiple, custIds, usernames) {
    var CustomerModel = customerModel;
    var SettingModel = customerSetting;
    var CustomerInfo = { id: custId, uplineId: uplineId, userId: userId, username: username };
    var IsMultiple = isMultiple;
    var CustIds = custIds;
    var Usernames = usernames;
    var ErrorMessage = '';
    var actionName = '';

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
        jQuery('#userInfo').remove();
        $('#statusInfo').attr('style', 'display: block');

        var names = "";
        jQuery(Usernames).each(function (index, username) {
            names += username + ", ";
        });

        Fanex.CustomerSetting.Core.ProcessDisplayMultiple(125, names);
        names = names.substring(0, names.length - 2);

        jQuery('#names').html(names);
        jQuery('#names').show();
    }
    else {
        $('#arrowUpDown').hide();
        $('#statusInfo').hide();
    }

    this.Update = function (ondone) {
        actionName = ActionName();

        var min = parseFloat(Fanex.CustomerSetting.Core.ReplaceAll($('#minCredit').html(), ',', ''));
        var max = parseFloat(Fanex.CustomerSetting.Core.ReplaceAll($('#maxCredit').html(), ',', ''));
        var cur = parseFloat(Fanex.CustomerSetting.Core.ReplaceAll($('#maxTransfer').val(), ',', ''));

        if (isNaN(cur) || cur < min || cur > max) {
            Validators.show($('#maxTransfer').get(0), 'Member Max Transfer Credit must be between ' + $('#minCredit').html() + ' and ' + $('#maxCredit').html(), undefined, undefined, 1500);
            return;
        }

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
        var data = {
            id: CustomerInfo.id,
            uplineId: CustomerInfo.uplineId,
            userId: CustomerInfo.userId,
            username: CustomerInfo.username,
            settingModel: JSON.stringify(SettingModel),
            customerModel: JSON.stringify(CustomerModel)
        };

        data.id = custIdIndex == null ? CustomerInfo.id : CustIds[custIdIndex];
        data.custName = custIdIndex == null ? CustomerModel.userName : Usernames[custIdIndex];

        $.ajax({
            url: toUrl(actionName),
            data: data,
            type: 'post',
            dataType: 'json'
        })
        .success(function (result) {
            if (result.errorCode != 0) {
                ErrorMessage += result.errorMessage.formatErrorMessage() + '<br />';
            }

            if (!IsMultiple) {
                if (ErrorMessage != '') {
                    $('#errMsg').html(ErrorMessage);
                    $('#errMsg').show();

                    if (typeof ondone == 'function') {
                        ondone(1, ErrorMessage);
                    }

                    ErrorMessage = '';
                    Fanex.CustomerSetting.Core.UnBlockUI();

                    return false;
                }
                else {
                    $('#errMsg').attr('class', 'successMsg');
                    $('#errMsg').html('<font color="Blue">Updated successfully</font>');
                    $('#errMsg').show();

                    if (typeof ondone == 'function') {
                        ondone(0, ErrorMessage);
                    }

                    ErrorMessage = '';
                    Fanex.CustomerSetting.Core.UnBlockUI();

                    return true;
                }
            }
            else {
                if (custIdIndex == CustIds.length - 1) {
                    if (ErrorMessage != '') {
                        $('#errMsg').html(ErrorMessage);
                        $('#errMsg').show();

                        if (typeof ondone == 'function') {
                            ondone(1, ErrorMessage);
                        }

                        ErrorMessage = '';
                        Fanex.CustomerSetting.Core.UnBlockUI();

                        return false;
                    }
                    else {
                        $('#errMsg').attr('class', 'successMsg');
                        $('#errMsg').html('<font color="Blue">Updated successfully</font>');
                        $('#errMsg').show();

                        if (typeof ondone == 'function') {
                            ondone(0, ErrorMessage);
                        }

                        ErrorMessage = '';
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
            if (typeof console != 'undefined') {
                console.log(response);
            }

            ErrorMessage += $(response.responseText)[1].text.formatErrorMessage() + '<br />';

            if (!IsMultiple) {
                if (ErrorMessage != '') {
                    $('#errMsg').html(ErrorMessage);
                    $('#errMsg').show();

                    if (typeof ondone == 'function') {
                        ondone(1, ErrorMessage);
                    }

                    ErrorMessage = '';
                    Fanex.CustomerSetting.Core.UnBlockUI();

                    return false;
                }
                else {
                    $('#errMsg').attr('class', 'successMsg');
                    $('#errMsg').html('<font color="Blue">Updated successfully</font>');
                    $('#errMsg').show();

                    if (typeof ondone == 'function') {
                        ondone(0, ErrorMessage);
                    }

                    ErrorMessage = '';
                    Fanex.CustomerSetting.Core.UnBlockUI();

                    return true;
                }
            }
            else {
                if (custIdIndex == CustIds.length - 1) {
                    if (ErrorMessage != '') {
                        $('#errMsg').html(ErrorMessage);
                        $('#errMsg').show();

                        if (typeof ondone == 'function') {
                            ondone(1, ErrorMessage);
                        }

                        ErrorMessage = '';
                        Fanex.CustomerSetting.Core.UnBlockUI();

                        return false;
                    }
                    else {
                        $('#errMsg').attr('class', 'successMsg');
                        $('#errMsg').html('<font color="Blue">Updated successfully</font>');
                        $('#errMsg').show();

                        if (typeof ondone == 'function') {
                            ondone(0, ErrorMessage);
                        }

                        ErrorMessage = '';
                        Fanex.CustomerSetting.Core.UnBlockUI();

                        return true;
                    }
                }
                else {
                    ProcessUpdate(++custIdIndex, ondone);
                }
            }
        });
    }
}