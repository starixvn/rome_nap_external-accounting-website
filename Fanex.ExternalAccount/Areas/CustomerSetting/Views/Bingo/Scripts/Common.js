function PTBlockUI() {
    try {
        parent.Fanex.ExtAcc.BlockUI();
    }
    catch (e) {
    }
}
function PTUnBlockUI() {
    try {
        parent.Fanex.ExtAcc.UnBlockUI();
    } catch (e) {
    }
}
function uInteger(str) {
    str = str.replace(/^\s+|\s+$/g, '');
    return /^[0-9]+$/.test(str);
}
function IsSyncData(obj) {
    var tblUpdate = $("#buttonPopup");
    if (tblUpdate && obj.checked) {
        tblUpdate.removeClass('disabled');
        tblUpdate.click(function () {
            updateBingoPTSetting();
        });
    }
    else {
        tblUpdate.addClass('disabled');
        tblUpdate.unbind('click');
    }
}
function updateBingoPTSetting() {
    var customer = customerPTBingoSetting.context;
    if (customer) {
        if (ptBingoCommon.isDirectDownline()) {
            var role = customer.roleId;
            switch (role) {
                case ptBingoCommon.superRoleId:
                    superBingoSetting.updatePT(customer);
                    break;

                case ptBingoCommon.masterRoleId:
                    masterBingoSetting.updatePT(customer);
                    break;

                case ptBingoCommon.agentRoleId:
                    agentBingoSetting.updatePT(customer);
                    break;

                case ptBingoCommon.memberRoleId:
                    memberBingoSetting.updatePT(customer);
                    break;

                default:
                    break;
            }
        }
        else {
            if (window.pageRole === ptBingoCommon.superRoleId) {
                superEditMemberBingoSetting.updatePT(customer);
            }
            else if (window.pageRole === ptBingoCommon.masterRoleId) {
                masterEditMemberBingoSetting.updatePT(customer);
            }
        }
    }
}