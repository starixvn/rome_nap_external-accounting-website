$.ajaxSetup({
    beforeSend: function (xhr) {
        var antiXSRFToken = document.getElementsByName("__RequestVerificationToken");
        if (antiXSRFToken.length > 0) {
            xhr.setRequestHeader("__RequestVerificationToken", antiXSRFToken[0].value);
        }
    }
});

var customerPTRacingSetting =
{
    context: {},
    ptSetting: null,
    bettingCommSetting: null,
    oldSetting: {},
    currentSetting: {},
    otherSettings: null,
    isMultiple: false,
    CustInfos: [],
    Usernames: [],
    Error: '',
    Success: true,
    init: function () {
        try {
            this.isMultiple = parent.Fanex.ExtAcc.Customer.IsMultiple;
            this.CustInfos = !this.isMultiple ? [] : parent.Fanex.ExtAcc.Customer.IdsAndNames();
            this.Usernames = parent.Fanex.ExtAcc.Customer.Usernames;
        } catch (e) {
        }
        this.getContextObject();
    },
    getContextObject: function () {
        PTBlockUI();

        var param = {
            uplineId: window.uplineId,
            targetCustId: window.custId,
            targetRoleId: window.targetRoleId,
            roleId: window.pageRole
        };

        $.ajax({
            type: "POST",
            url: "GetSetting",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            data: JSON.stringify(param),
            success: function (stringContext) {
                customerPTRacingSetting.context = JSON.parse(stringContext);
                customerPTRacingSetting.getCustomerInfo();
            }
            , error: function (e) {
                showAlertMessage("err", "Init Context Error", 1);
                PTUnBlockUI();
            }
        });
    },
    getCustomerInfo: function () {
        customerPTRacingSetting.context.adminId = parent.userId;
        $.ajax({
            type: "POST",
            url: "GetCustomerInfo",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            data: '{"requestContext":' + JSON.stringify(customerPTRacingSetting.context) + '}',
            beforeSend: function (jqXHR, settings) { PTBlockUI(); },
            success: function (stringCustomer) {
                customerPTRacingSetting.currentSetting = JSON.parse(stringCustomer);
                customerPTRacingSetting.oldSetting = JSON.parse(stringCustomer);
                customerPTRacingSetting.initHtml();
                PTUnBlockUI();
            }
            , error: function (e) {
                showAlertMessage("err", "Init Customer Error", 1);
                PTUnBlockUI();
            }, complete: function (jqXHR, textStatus) { PTUnBlockUI(); }
        });
    },
    initHtml: function () {
        if (window.targetRoleId !== ptRacingCommon.superRoleId) {
            $('.floatL').removeClass('display-none');
        }

        if (!this.isMultiple) {
            $('#arrowUpDown').hide();
            ptRacingCommon.renderCustomerInfo(customerPTRacingSetting.getPageName(window.targetRoleId), customerPTRacingSetting.currentSetting);
        }
        else {
            Fanex.CustomerSetting.Core.ProcessDisplayMultiple(125, this.Usernames);
        }

        $('#bettingCommContainer').html('');
        if (this.currentSetting && this.currentSetting.settings &&
            this.currentSetting.settings.sportTypes && this.currentSetting.settings.sportTypes.length > 0) {
            Fanex.PTRacing.CURRENT_CONTEXT = this.context;
            if (window.targetRoleId == ptRacingCommon.agentRoleId || (window.targetRoleId == ptRacingCommon.memberRoleId && window.pageRole == ptRacingCommon.masterRoleId))
                Fanex.PTRacing.CURRENT_CONTEXT.ia3 = !this.currentSetting.disableAutoMasterPt;

            /*region render betting & comm*/
            this.bettingCommSetting = new Fanex.PTRacing.BettingCommSetting(this.currentSetting.settings.sportTypes);
            var bettingCommContainer = document.getElementById('bettingCommContainer');
            bettingCommContainer.innerHTML = "";
            this.bettingCommSetting.Render(bettingCommContainer);
            /*endregion render betting & comm*/

            /*region render pt*/
            this.ptSetting = new Fanex.PTRacing.PTGrid(this.currentSetting.settings);
            var ptContainer = document.getElementById('ptContainer');
            ptContainer.innerHTML = "";
            this.ptSetting.Render(ptContainer);
            /*endregion render pt*/

            $('#tbCustomerInfo').css('width', ($('#tbBettingCommSetting').width() + 'px'));

        } else {
            $('#bettingCommContainer').append('There is no information available.');
            $('#ptContainer').append('There is no information available.');
        }
    },
    updateCustomerSettings: function () {
        var needToValidate = true;
        if (window.targetRoleId == ptRacingCommon.memberRoleId &&
            (window.pageRole == ptRacingCommon.superRoleId || window.pageRole == ptRacingCommon.masterRoleId)) {
            needToValidate = false;
        }
        if (needToValidate && !this.bettingCommSetting.Validate()) {
            return;
        }

        var isUpdateAllMember = this.ptSetting.Validate(this.currentSetting.settings, false);
        this.currentSetting.settings = this.ptSetting.Save();
        if (!isUpdateAllMember) {
            PTBlockUI();
            customerPTRacingSetting.updateAllSettings(this.isMultiple ? 0 : null);
        }
    },
    updateAllSettings: function (index) {
        var currentContext = customerPTRacingSetting.context;
        var newContext = customerPTRacingSetting.context;
        if (index != null)
            newContext.targetCustId = this.CustInfos.Ids[index];

        var ctx = index == null ? currentContext : newContext;
        PTBlockUI();
        $.ajax({
            type: "POST",
            url: "UpdateCustomerSetting",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            data: '{"oldSettings":' + JSON.stringify(customerPTRacingSetting.oldSetting) +
                ', "newSettings":' + JSON.stringify(customerPTRacingSetting.currentSetting) +
                ', "currentContext":' + JSON.stringify(ctx) +
                ', "otherSettings":' + JSON.stringify(customerPTRacingSetting.otherSettings) +
                ', "groupName": "' + parent.$('#selectedGroupName').html() + '"' +
                '}',            
            success: function (result) {
                if (!customerPTRacingSetting.isMultiple) {
                    $('#divShowInfo').show();

                    if (typeof (result.errorCode) != 'undefined') {
                        showAlertMessage("err", result.errorMessage, 1);
                    }
                    else {
                        customerPTRacingSetting.Success = ptRacingCommon.renderErrorMessage('err', result);

                        if (customerPTRacingSetting.Success) {
                            var timeout = setTimeout(function () {
                                clearTimeout(timeout);
                                document.location.reload(true);
                            }, 2000);
                        }
                    }
                    PTUnBlockUI();
                }
                else {
                    if (typeof (result.errorCode) != 'undefined') {
                        customerPTRacingSetting.Error += '<font color="Blue" style="font-weight: bold">' + customerPTRacingSetting.CustInfos.Names[index] + ':</font><br />' + result.errorMessage + '<br />';
                        customerPTRacingSetting.Success = false;
                    }
                    else {
                        var msg = customerPTRacingSetting.checkError(result);
                        customerPTRacingSetting.Error += '<font color="Blue" style="font-weight: bold">' + customerPTRacingSetting.CustInfos.Names[index] + ':</font><br />' + msg.message + '<br />';
                        customerPTRacingSetting.Success = customerPTRacingSetting.Success && msg.success;
                    }

                    if (index < customerPTRacingSetting.CustInfos.Ids.length - 1) {
                        ++index;
                        customerPTRacingSetting.updateAllSettings(index);
                    }
                    else {
                        $('#divShowInfo').show();
                        showAlertMessage("err", customerPTRacingSetting.Error, (customerPTRacingSetting.Success ? 0 : 1));
                        PTUnBlockUI();

                        if (customerPTRacingSetting.Success) {
                            var timeout = setTimeout(function () {
                                clearTimeout(timeout);
                                document.location.reload(true);
                            }, 2000);
                        }

                        customerPTRacingSetting.Error = '';
                        customerPTRacingSetting.Success = true;
                    }
                }
            }
            , error: function (response) {
                customerPTRacingSetting.Success = false;

                if (!customerPTRacingSetting.isMultiple) {
                    var ErrorMessage = '';

                    if ($.parseJSON($(response)[0].responseText).errorCode) {
                        ErrorMessage += '<font color="red">' + $.parseJSON($(response)[0].responseText).errorMessage + '</font><br />';
                    }
                    else {
                        ErrorMessage += '<font color="red">' + $(response.responseText)[1].text + '</font><br />';
                    }

                    $('#divShowInfo').show();
                    showAlertMessage("err", ErrorMessage, 1);
                    PTUnBlockUI();
                }
                else {
                    if ($.parseJSON($(response)[0].responseText).errorCode) {
                        customerPTRacingSetting.Error += '<font color="Blue" style="font-weight: bold">' + customerPTRacingSetting.CustInfos.Names[index] + ':</font><br />' + '<font color="red">' + $.parseJSON($(response)[0].responseText).errorMessage + '</font><br />';
                    }
                    else {
                        customerPTRacingSetting.Error += '<font color="Blue" style="font-weight: bold">' + customerPTRacingSetting.CustInfos.Names[index] + ':</font><br />' + '<font color="red">' + $(response.responseText)[1].text + '</font><br />';
                    }

                    if (index < customerPTRacingSetting.CustInfos.Ids.length - 1) {
                        ++index;
                        customerPTRacingSetting.updateAllSettings(index);
                    }
                    else {
                        $('#divShowInfo').show();
                        showAlertMessage("err", customerPTRacingSetting.Error, (customerPTRacingSetting.Error == '' ? 0 : 1));
                        PTUnBlockUI();
                        customerPTRacingSetting.Error = '';
                        customerPTRacingSetting.Success = true;
                    }
                }
            }
        });
    },
    setNewValueToMember: function (isUpdateAllMember) {
        this.otherSettings = { "isupdateallmembers": isUpdateAllMember, "newmpositiontaking": Popups.GetNewPt() };

        this.updateAllSettings();
        Popups.Close();
    },

    getPageName: function (targetRoleId) {
        switch (targetRoleId) {
            case ptRacingCommon.superRoleId:
                return "Super";
            case ptRacingCommon.masterRoleId:
                return "Master";
            case ptRacingCommon.agentRoleId:
                return "Agent";

            default:
                return "Member";
        }
    },

    checkError: function (ErrorHandler) {
        var isError500 = true;
        var isSuccess = true;
        var result = '';
        if (ErrorHandler != null && ErrorHandler.length > 0) {
            $.each(ErrorHandler, function (index, Result) {
                if (Result.errorCode == 100 || Result.errorCode == 99 || Result.errorCode == 98) {
                    if (isError500) {
                        result += ptRacingCommon.formatHtmlErrorMessage(Result.errorMessage, 2);
                    }
                    isSuccess = false;
                }
                else if (Result.errorCode > 0) {
                    if (isError500) {
                        result += ptRacingCommon.formatHtmlErrorMessage(Result.errorMessage, 1);
                    }
                    isSuccess = false;
                }
                else {
                    result += ptRacingCommon.formatHtmlErrorMessage(Result.errorMessage, 0);
                }
                if (Result.errorCode == 500) isError500 = false;
            });
        }

        return { message: result, success: isSuccess };
    }

};

$(document).ready(function () {
    customerPTRacingSetting.init();
});