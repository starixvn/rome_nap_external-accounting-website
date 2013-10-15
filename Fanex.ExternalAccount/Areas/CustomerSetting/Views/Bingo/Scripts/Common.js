function PTBlockUI() {
    /*if (parent.BlockUI) {
        try { parent.BlockUI(); } catch (e) { }
    }
    //Called inside a frame set (#centerFr)
    else {
        if (parent.parent.BlockUI) {
            parent.parent.BlockUI();
        }
    }
    //Called from popup
    if (opener) {
        if (opener.BlockUI) {
            opener.BlockUI();
        }
        opener.parent.BlockUI();
    }*/
    parent.Fanex.ExtAcc.BlockUI();
}
function PTUnBlockUI() {
    /*if (parent.jQuery && parent.jQuery.unblockUI) {
        parent.jQuery.unblockUI();
    }
    //Called inside a frame set (#centerFr)
    if (parent.parent.jQuery && parent.parent.jQuery.unblockUI) {
        parent.parent.jQuery.unblockUI();
    }
    //Called from popup
    if (opener) {
        if (opener.parent.jQuery.unblockUI) {
            opener.parent.jQuery.unblockUI();
        }
    }*/
    parent.Fanex.ExtAcc.UnBlockUI();
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