var ptBingoCommon = {
    adminRoleId: 5,
    superRoleId: 4,
    masterRoleId: 3,
    agentRoleId: 2,
    memberRoleId: 1,
    FormatNumber: function (number) { // Format a number to 1,234,567.89
        number += '';

        x = number.split('.');
        x1 = x[0];
        x2 = x.length > 1 ? '.' + x[1] : '';
        var rgx = /(\d+)(\d{3})/;
        while (rgx.test(x1)) {
            x1 = x1.replace(rgx, '$1' + ',' + '$2');
        }
        return x1 + x2;
    },

    renderCustomerInfo: function (custInfo) {
        var html = '<table class="tblcollapse" style="width: 100%;">\
                        <tr class="title">\
                            <td colspan="4">Customer Information</td>\
                        </tr>\
                        <tr>\
                            <td class="r" style="width:20%">Username</td>\
                            <td class="l" style="width:30%">' + (custInfo.userName ? custInfo.userName : '') + '</td>\
                            <td class="r" style="width:20%">Nick Name</td>\
                            <td class="l" style="width:30%">' + (custInfo.nickName ? custInfo.nickName : '') + '</td>\
                        </tr>\
                        <tr>\
                            <td class="r">First Name</td>\
                            <td class="l">' + custInfo.firstName + '</td>\
                            <td class="r">Last Name</td>\
                            <td class="l">' + custInfo.lastName + '</td>\
                        </tr>\
                        <tr>\
                            <td class="r">Mobile Phone</td>\
                            <td class="l">' + custInfo.mobilePhone + '</td>\
                            <td class="r">Fax</td>\
                            <td class="l">' + custInfo.fax + '</td>\
                        </tr>\
                        <tr>\
                            <td class="r">Phone</td>\
                            <td class="l">' + custInfo.phone + '</td>\
                            <td class="l">&nbsp</td>\
                            <td class="l">&nbsp</td>\
                        </tr>';
        return html;
    },
    renderBetSetting: function (betSetting) {
        var html = '';
        if (betSetting.showBetSetting) {
            var directDownline = ptBingoCommon.isDirectDownline();
            var superEditMaster = window.pageRole === ptBingoCommon.adminRoleId && window.targetRoleId === ptBingoCommon.superRoleId;
            var dontShowResetLink = superEditMaster || !betSetting.syncBingo;
            html = '<tr class="title">\
                        <td colspan="4">Bet Setting</td>\
                    </tr>\
                    <tr>\
                        <td colspan="4" class="c">\
                            <table cellpadding="4" cellspacing="0" border="0" width="100%" class="noneborder">\
                                <tr>\
                                    <td class="r" style="width:300px">\
                                        <b>Accumulated transferable amount</b>\
                                    </td>\
                                    <td class="r" style="width:100px">' + this.FormatNumber(betSetting.actualMaxTransfer) + '</td>\
                                    <td class="l" colspan="2">' + (!dontShowResetLink ? '<span onclick="ResetMaxTransfer(' + betSetting.iD + ')" class="canchange">Reset' : '') + '</td>\
                                </tr>\
                                <tr>\
                                    <td class="r" style="width:300px">\
                                        <b>Member Max Transfer Credit</b>\
                                    </td>\
                                    <td class="r" style="width:100px">';
            if (directDownline) {
                html += '<input type="text" value="' + this.FormatNumber(betSetting.maxTransferCredit) + '" id="txtMaxTransferCredit" class="r" maxLength="14" onpaste="return false;" onkeypress="return parent.Fanex.ExtAcc.CheckNumber(event, this, this)" onkeyup="return parent.Fanex.ExtAcc.CheckNumber(event, this, this)">';
            }
            else {
                html += this.FormatNumber(betSetting.maxTransferCredit);
            }
            html += '</td>\
                                    <td class="l" colspan="2">\
                                        <input type="checkbox"' + (betSetting.isAutoReset ? 'checked' : '') + ' id="cbxAutoReset" ' + (!directDownline ? 'disabled' : '') + '><label for="cbxAutoReset">Daily auto reset limit (at 12 noon GMT+8)</label>\
                                    </td>\
                                </tr>\
                                <tr>\
                                     <td class="r" style="width:300px">&nbsp</td>\
                                     <td class="r" style="width:100px">(' + this.FormatNumber(betSetting.minLimit) + ' - ' + (betSetting.maxLimit == 0 ? 'No limited' : this.FormatNumber(betSetting.maxLimit)) + ')</td>\
                                     <td class="l" colspan="2">&nbsp</td>\
                                </tr>\
                            </table>\
                        </td>\
                    </tr>';
        }
        return html;
    },
    renderIncentiveSetting: function (incentiveSetting) {
        var role = incentiveSetting.roleId;
        if (!ptBingoCommon.isDirectDownline() || role === this.superRoleId) {
            return '</table>';
        }
        var html = '<tr class="title">\
                            <td colspan="4">Incentive Setting</td>\
                        </tr>\
                        <tr>\
                            <td colspan="4" class="c">\
                                <table cellpadding="4" cellspacing="0" border="0" width="100%" class="noneborder" style="height:30px">\
                                    <tr>\
                                        <td style="width:300px" class="r b">' + this.getIncentiveByRole(role) + ' Incentive</td>\
                                        <td class="l">\
                                            <select ' + (role === this.memberRoleId ? 'disabled="disabled"' : '') + ' id="listIncentiveRate" max="' + roundNumber(incentiveSetting.maxPayout, 2) + '" min="0" class="select-pt">\
                                                <option value="' + roundNumber(incentiveSetting.incentiveRate, 2) + '" selected="true">' + roundNumber(incentiveSetting.incentiveRate, 2) + '</option>\
                                            </select>\
                                        </td>\
                                    </tr>\
                                </table>\
                            </td>\
                        </tr>\
                    </table>';
        return html;
    },
    getIncentiveByRole: function (role) {
        var title;
        switch (role) {
            case this.masterRoleId:
                title = 'Super';
                break;
            case this.agentRoleId:
                title = 'Master';
                break;
            case this.memberRoleId:
                title = 'Agent';
                break;
            default:
                title = '';
                break;
        }
        return title;
    },
    renderPositionTaking: function (positionTakingSetting) {
        var role = positionTakingSetting.roleId;
        var html;
        if (ptBingoCommon.isDirectDownline()) {
            switch (role) {
                case ptBingoCommon.superRoleId:
                    html = superBingoSetting.renderPositionTaking(positionTakingSetting);
                    break;
                case ptBingoCommon.masterRoleId:
                    html = masterBingoSetting.renderPositionTaking(positionTakingSetting);
                    break;
                case ptBingoCommon.agentRoleId:
                    html = agentBingoSetting.renderPositionTaking(positionTakingSetting);
                    break;
                case ptBingoCommon.memberRoleId:
                    html = memberBingoSetting.renderPositionTaking(positionTakingSetting);
                    break;
                default:
                    html = '';
                    break;
            }
        }
        else {
            if (window.pageRole === ptBingoCommon.superRoleId) {
                html = superEditMemberBingoSetting.renderPositionTaking(positionTakingSetting);
            }
            else if (window.pageRole === ptBingoCommon.masterRoleId) {
                html = masterEditMemberBingoSetting.renderPositionTaking(positionTakingSetting);
            }
        }
        return html;
    },
    renderBettingRange: function (gameKindBOId, generalId, bettingRange) {
        return '<span style="margin-right:6px;"><input type="checkbox"'
            + (bettingRange.isChecked ? 'checked' : '') + ' id="chkStake_' + generalId + '">' + bettingRange.name + '</span>';
    },
    renderSubmitButtion: function (syncBingo) {
        if (ptBingoCommon.isDirectDownline() && window.targetRoleId === ptBingoCommon.superRoleId) {
            return '';
        }

        var html = '<table id="tblHideUpdate" style=" line-height: 25px;"><tr>';
        if (!syncBingo) {
            html += '<td id="tdIsSyncData"><label for="chkIsSyncData">Enable Bingo</label> <input type="checkbox" id="chkIsSyncData" onclick="IsSyncData(this)" /></td>';
        }
        html += '<td><input type="button" id="buttonPopup" class="buttonPopup btnTop ' + (!syncBingo ? 'disabled' : '') + '" value="Submit"/></td>\
            </tr>\
        </table>';
        return html;
    },
    isDirectDownline: function () {
        return (window.pageRole - window.targetRoleId) === 1;
    }
};