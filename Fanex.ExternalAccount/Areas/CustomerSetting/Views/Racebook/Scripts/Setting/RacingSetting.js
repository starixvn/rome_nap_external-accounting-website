var customerPTRacingSetting =
{
    context: {},
    ptSetting: null,
    bettingCommSetting: null,
    oldSetting: {},
    currentSetting: {},
    otherSettings: null,
    init: function () {
        this.getContextObject();
    },
    getContextObject: function () {
        var param = {
            uplineId: window.uplineId,
            targetCustId: window.custId,
            targetRoleId: window.targetRoleId,
            roleId: window.pageRole
        };

        $.ajax({
            type: "POST",
            url: parent.resolveClientUrl("CustomerSetting/Racebook/RacingSetting/GetSetting"),
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            data: JSON.stringify(param),
            success: function (stringContext) {
                customerPTRacingSetting.context = JSON.parse(stringContext);
                customerPTRacingSetting.getCustomerInfo();
            }
            , error: function (e) {
                showAlertMessage("err", "Init Context Error", 1);
            }
        });
    },
    getCustomerInfo: function () {
        $.ajax({
            type: "POST",
            url: parent.resolveClientUrl("CustomerSetting/Racebook/RacingSetting/GetCustomerInfo"),
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            data: '{"requestContext":' + JSON.stringify(customerPTRacingSetting.context) + '}',
            beforeSend: function (jqXHR, settings) { parent.Fanex.ExtAcc.BlockUI(); },
            success: function (stringCustomer) {
                customerPTRacingSetting.currentSetting = JSON.parse(stringCustomer);
                customerPTRacingSetting.oldSetting = JSON.parse(stringCustomer);
                customerPTRacingSetting.initHtml();
            }
            , error: function (e) {
                showAlertMessage("err", "Init Customer Error", 1);
            }, complete: function (jqXHR, textStatus) { parent.Fanex.ExtAcc.UnBlockUI(); }
        });
    },
    initHtml: function () {
        if (window.targetRoleId !== ptRacingCommon.superRoleId) {
            $('.floatL').removeClass('display-none');
        }
        ptRacingCommon.renderCustomerInfo(customerPTRacingSetting.getPageName(window.targetRoleId), customerPTRacingSetting.currentSetting);
        $('#bettingCommContainer').html('');
        if (this.currentSetting && this.currentSetting.settings &&
            this.currentSetting.settings.sportTypes && this.currentSetting.settings.sportTypes.length > 0) {
            Fanex.PTRacing.CURRENT_CONTEXT = this.context;
            if (window.targetRoleId == ptRacingCommon.agentRoleId || (window.targetRoleId == ptRacingCommon.memberRoleId && window.pageRole == ptRacingCommon.masterRoleId))
                Fanex.PTRacing.CURRENT_CONTEXT.ia3 = !this.currentSetting.DisableAutoMasterPt;

            /*region render betting & comm*/
            this.bettingCommSetting = new Fanex.PTRacing.BettingCommSetting(this.currentSetting.settings.sportTypes);
            var bettingCommContainer = document.getElementById('bettingCommContainer');
            bettingCommContainer.innerHTML = "";
            this.bettingCommSetting.Render(bettingCommContainer, window.targetRoleId);
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
            customerPTRacingSetting.updateAllSettings();
        }
    },
    updateAllSettings: function () {
        $.ajax({
            type: "POST",
            url: parent.resolveClientUrl("CustomerSetting/Racebook/RacingSetting/UpdateCustomerSetting"),
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            data: '{"oldSettings":' + JSON.stringify(customerPTRacingSetting.oldSetting) +
                ', "newSettings":' + JSON.stringify(customerPTRacingSetting.currentSetting) +
                ', "currentContext":' + JSON.stringify(customerPTRacingSetting.context) +
                ', "otherSettings":' + JSON.stringify(customerPTRacingSetting.otherSettings) +
                '}',
            beforeSend: function (jqXHR, settings) { parent.Fanex.ExtAcc.BlockUI(); },
            success: function (result) {
                if (typeof (result.ErrorCode) != 'undefined') {
                    showAlertMessage("err", result.ErrorMessage, 1);
                }
                else {
                    $('#divShowInfo').show();
                    ptRacingCommon.renderErrorMessage('err', result);
                }
                parent.Fanex.ExtAcc.UnBlockUI();
            }
            , error: function (e) {
                showAlertMessage("err", "Update Agent Unsuccessfully!", 1);
                parent.Fanex.ExtAcc.UnBlockUI();
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
    }

};

$(function () {
    customerPTRacingSetting.init();
});