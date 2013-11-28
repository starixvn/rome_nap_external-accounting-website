var customerPTBingoSetting = {
    context: {},
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
        this.getCustomerSetting();
    },
    getCustomerSetting: function () {
        var validCustID = window.custId && window.custId > 0;
        var validRoleID = window.pageRole && window.pageRole > 0;
        if (!this.isMultiple) {
            $('#arrowUpDown').hide();
        }
        else {
            $('#arrowUpDown').show();
            Fanex.CustomerSetting.Core.ProcessDisplayMultiple(125, this.Usernames);
        }

        if (validCustID && validRoleID) {
            var param = {
                custId: window.custId,
                updatedByRoleID: window.pageRole
            };
            $.ajax({
                type: 'POST',
                url: 'GetCustomerSetting',
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                data: JSON.stringify(param),
                beforeSend: function (jqXHR, settings) {
                    PTBlockUI();
                },
                success: function (rs) {
                    if (rs) {
                        var response = JSON.parse(rs);
                        response.uplineID = window.uplineId;
                        customerPTBingoSetting.initHtml(response);
                    }
                    else {
                        $('#err').html('<div class="alertboxerr"><div>There is no information available.</div></div>');
                        $('.bg_table').addClass('hidden');
                    }
                },
                complete: function () {
                    PTUnBlockUI();
                }
            });
        }
    },
    ajaxUpdatePT: function (customer, index) {
        var param = { customer: customer, updatedByRoleID: window.pageRole, groupName: parent.$('#selectedGroupName').html() };
        if (index != null) {
            param.customer.iD = parseInt(this.CustInfos.Ids[index]);
            param.customer.userName = this.CustInfos.Names[index];
        }

        $.ajax({
            type: 'POST',
            url: 'UpdateCustomerSetting',
            contentType: 'application/json; charset=utf-8',
            dataType: 'json',
            data: JSON.stringify(param),
            beforeSend: function (jqXHR, settings) {
                PTBlockUI();
            },
            success: function (rs) {
                if (index == null) {
                    if (rs.errorCode == 0) {
                        showAlertMessage('err', 'Updated successfully', 0);
                        afterUpdate(2000);
                    }
                    else {
                        showAlertMessage('err', rs.errorMessage, 1);
                    }
                }
                else {
                    customerPTBingoSetting.Error += '<font color="Blue" style="font-weight: bold">' + customerPTBingoSetting.CustInfos.Names[index] + ':</font><br />';
                    ++index;
                    if (rs.errorCode == 0) {
                        customerPTBingoSetting.Error += '<font color="Blue">Updated successfully</font><br /><br />';
                    }
                    else {
                        customerPTBingoSetting.Success = false;
                        customerPTBingoSetting.Error += rs.errorMessage.formatErrorMessage() + '<br /><br />';
                    }
                    if (index < customerPTBingoSetting.CustInfos.Ids.length) {
                        customerPTBingoSetting.ajaxUpdatePT(customer, index);
                    }
                    else {
                        showAlertMessage('err', customerPTBingoSetting.Error, customerPTBingoSetting.Success ? 0 : 1);
                        if (customerPTBingoSetting.Success)
                            afterUpdate(2000);

                        customerPTBingoSetting.Error = '';
                        customerPTBingoSetting.Success = true;
                    }
                }
            },
            error: function (jqXHR, textStatus, errorThrown) {
                if (index == null) {
                    if (jqXHR.responseText) {
                        var errorObject = JSON.parse(jqXHR.responseText);
                        showAlertMessage('err', errorObject.errorMessage, 1);
                    }
                }
                else {
                    customerPTBingoSetting.Error += '<font color="Blue" style="font-weight: bold">' + customerPTBingoSetting.CustInfos.Names[index] + ':</font><br /><br />';
                    ++index;
                    if (index < customerPTBingoSetting.CustInfos.Ids.length) {
                        customerPTBingoSetting.ajaxUpdatePT(customer, index);
                    }
                    else {
                        showAlertMessage('err', customerPTBingoSetting.Error, customerPTBingoSetting.Success ? 0 : 1);
                        if (customerPTBingoSetting.Success)
                            afterUpdate(2000);

                        customerPTBingoSetting.Error = '';
                        customerPTBingoSetting.Success = true;
                    }
                }
            },
            complete: function () {
                if (index == null || index >= customerPTBingoSetting.CustInfos.Ids.length) {
                    PTUnBlockUI();
                }
            }
        });
    },
    initHtml: function (rs) {
        if (rs.result && rs.result.errorCode > 0) {
            showAlertMessage('err', 'Edit Bingo: ' + rs.result.errorMessage, 1);
            $('.bg_table').addClass('hidden');
        }
        else {
            rs.isGroupSetting = rs.isUsedGameKind;

            var settingContent = $('#SettingContent');
            settingContent.empty()
                .append(ptBingoCommon.renderCustomerInfo(rs) + ptBingoCommon.renderBetSetting(rs) + ptBingoCommon.renderIncentiveSetting(rs))
                .append(ptBingoCommon.renderPositionTaking(rs))
                .append(ptBingoCommon.renderSubmitButtion(rs.syncBingo));
            AssignCommission();
            if (rs.syncBingo) {
                $('#buttonPopup').click(function () {
                    updateBingoPTSetting();
                });
            }
            AssignPT();
            customerPTBingoSetting.context = rs;
        }
    }
};

$(document).ready(function () {
    customerPTBingoSetting.init();
});