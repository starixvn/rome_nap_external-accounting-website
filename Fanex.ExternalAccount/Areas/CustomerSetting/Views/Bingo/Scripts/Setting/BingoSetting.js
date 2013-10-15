var customerPTBingoSetting = {
    context: {},
    init: function () {
        this.getCustomerSetting();
    },
    getCustomerSetting: function () {
        var validCustID = window.custId && window.custId > 0;
        var validRoleID = window.targetRoleId && window.targetRoleId > 0
        if (validCustID && validRoleID) {
            var param = {
                custId: window.custId,
                targetRoleId: window.targetRoleId
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
    ajaxUpdatePT: function (customer) {
        var param = { customer: customer, updatedByRoleID: window.pageRole };
        $.ajax({
            type: 'POST',
            url: 'UpdateCustomerSetting',
            contentType: 'application/json; charset=utf-8',
            dataType: 'json',
            data: JSON.stringify(param),
            success: function (rs) {
                if (rs.ErrorCode == 0) {
                    showAlertMessage('err', 'Updated Successfully', 0);
                    afterUpdate(2000);
                }
                else if (rs.ErrorCode == 500) {
                    showAlertMessage('err', rs.ErrorMessage, 1);
                }
                else {
                    showAlertMessage('err', rs.ErrorMessage, 1);
                }
            },
            error: function (jqXHR, textStatus, errorThrown) {
                if (jqXHR.responseText) {
                    var errorObject = JSON.parse(jqXHR.responseText);
                    showAlertMessage('err', errorObject.errorMessage, 1);
                }
            },
            complete: function () {
                PTUnBlockUI();
            }
        });
    },
    initHtml: function (rs) {
        if (rs.Result && rs.Result.ErrorCode > 0) {
            $('#err').html('<div class="alertboxerr"><div>' + rs.Result.ErrorMessage + '</div></div>');
            $('.bg_table').addClass('hidden');
        }
        else {
            $('.bg_table').removeClass('hidden');
            rs.isGroupSetting = rs.isUsedGameKind;

            var settingContent = $('#SettingContent');
            settingContent.empty()
                .append(ptBingoCommon.renderCustomerInfo(rs) + ptBingoCommon.renderBetSetting(rs))
                .append(ptBingoCommon.renderIncentiveSetting(rs))
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

$(function () {
    customerPTBingoSetting.init();
});