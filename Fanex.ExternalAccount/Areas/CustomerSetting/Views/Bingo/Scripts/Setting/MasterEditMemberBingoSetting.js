var masterEditMemberBingoSetting = {
    renderPositionTaking: function (positionTakingSetting) {
        var html = '<table class="tblcollapse" style="width: 100%; margin:6px 0px;" id="tblbingo">\
                        <tr class="title">\
                            <td colspan="4">Position Taking</td>\
                        </tr>';
        $.each(positionTakingSetting.gameKindList, function (index, value) {
            html += masterEditMemberBingoSetting.renderGameKindBO(positionTakingSetting.isGroupSetting, value);
        });
        return html + '</table>';
    },
    renderGameKindBO: function (isGroupSetting, gameKindBO) {
        var id = gameKindBO.iD;
        var html = '<tr>\
                        <td colspan="2" class="c"></td>\
                        <td class="c"><b>Master PT</b></td>\
                        <td class="c"><b>Agent PT</b></td>\
                    </tr>\
                    <tr>\
                        <td rowspan="' + (2 * gameKindBO.gameTypeList.length + 1) + '"><b>' + gameKindBO.name + '</b></td>\
                        <td class="l"><b>Group Setting</b></td>\
                        <td>\
                            <select id="listMasterPT_' + id + '" onchange="masterEditMemberBingoSetting.on_ChangeMasterPT(this);" max="' + roundNumber(gameKindBO.positionTakingMax, 2) + '" min="' + roundNumber(gameKindBO.minPositionTaking, 2) + '" class="select-pt">\
                                <option value="' + roundNumber(gameKindBO.positionTaking, 2) + '" selected="true">' + roundNumber(gameKindBO.positionTaking, 2) + '</option>\
                            </select>\
                            <input type="button" onclick="masterEditMemberBingoSetting.ReducePT(\'listMasterPT_' + id + '\',1)" class="btn-dec1" value=" "' + (gameKindBO.isAutoPositionTaking ? 'Disabled' : '') + '>\
                            <input type="button" onclick="masterEditMemberBingoSetting.IncreasePT(\'listMasterPT_' + id + '\',1)" class="btn-inc1" value=" "' + (gameKindBO.isAutoPositionTaking ? 'Disabled' : '') + '>\
                            <input type="button" onclick="masterEditMemberBingoSetting.ReducePT(\'listMasterPT_' + id + '\',10)" class="btn-dec10" value=" "' + (gameKindBO.isAutoPositionTaking ? 'Disabled' : '') + '>\
                            <input type="button" onclick="masterEditMemberBingoSetting.IncreasePT(\'listMasterPT_' + id + '\',10)" class="btn-inc10" value=" "' + (gameKindBO.isAutoPositionTaking ? 'Disabled' : '') + '>\
                        </td>\
                        <td>' + roundNumber(gameKindBO.positionTakingAgent, 2) + '</td>\
                    </tr>';
        $.each(gameKindBO.gameTypeList, function (index, value) {
            html += masterEditMemberBingoSetting.renderGameTypeBO(isGroupSetting, id, value);
        });
        return html;
    },
    renderGameTypeBO: function (isGroupSetting, gameKindBOId, gameTypeBO) {
        var id = gameKindBOId + '_' + gameTypeBO.iD;
        var html = '<tr>\
                    <td class="l" rowspan="2">' + gameTypeBO.name + '</td>';
        if (!isGroupSetting) {
            html += '<td class="gametypePT c">\
                        <select id="listMasterPT_' + id + '" max="' + roundNumber(gameTypeBO.positionTakingMax, 2) + '" min="' + roundNumber(gameTypeBO.minPositionTaking, 2) + '" class="select-pt MasterPT_' + id + '">\
                            <option value="' + roundNumber(gameTypeBO.positionTaking, 2) + '" selected="true">' + roundNumber(gameTypeBO.positionTaking, 2) + '</option>\
                        </select>\
                    </td>\
                    <td class="gametypePT c">' + roundNumber(gameTypeBO.positionTakingAgent, 2) + '</td>';
        }
        html += '</tr>\
                <tr>\
                    <td class="l" colspan="5">Betting Limit:';
        $.each(gameTypeBO.bettingRangeList, function (index, value) {
            var generalId = id + '_' + value.iD;
            html += ptBingoCommon.renderBettingRange(gameKindBOId, generalId, value);
        });
        return html + '</td></tr>';
    },
    updatePT: function (customer) {
        PTBlockUI();
        var totalGameKind = customer.gameKindList.length;
        var totalGameType;
        var totalBettingRange;
        //Loop to get PT setting GameKind
        for (var i = 0; i < totalGameKind; i++) {
            var gamekind = customer.gameKindList[i];
            var gamekindid = gamekind.iD;
            totalGameType = gamekind.gameTypeList.length;
            customer.gameKindList[i].positionTaking = roundNumber($("#" + strListMaster + "_" + gamekindid).val(), 2);

            //Loop to get PT setting GameType
            for (var j = 0; j < totalGameType; j++) {
                var gametype = customer.gameKindList[i].gameTypeList[j];
                var gametypeid = gametype.iD;
                totalBettingRange = gametype.bettingRangeList.length;
                if (!customer.isUsedGameKind) {
                    customer.gameKindList[i].gameTypeList[j].positionTaking = roundNumber($("#" + strListMaster + "_" + gamekindid + "_" + gametypeid).val(), 2);
                }
                //Loop to get BettingRangeList
                for (var z = 0; z < totalBettingRange; z++) {
                    var bettingrange = customer.gameKindList[i].gameTypeList[j].bettingRangeList[z];
                    var bettingrangeid = bettingrange.iD;
                    customer.gameKindList[i].gameTypeList[j].bettingRangeList[z].isChecked = $("#" + strChkStake + "_" + gamekindid + "_" + gametypeid + "_" + bettingrangeid).attr("checked");
                }
            }
        }
        customerPTBingoSetting.ajaxUpdatePT(customer);
    },
    on_ChangeMasterPT: function (obj) {
        var oMaster = $(obj);
        var strMasterID = oMaster.attr("id");
        var MasterValue = oMaster.val();
        jQuery.each($("." + strMasterID.replace(strListMaster, "MasterPT")), function () {
            if ($(this).prop("id").indexOf(strMasterID) >= 0) {
                $(this).val(MasterValue);
            }
        });
    },
    ReducePT: function (id, step) {
        var obj = $("#" + id);
        var curr = obj.val();
        var min = obj.attr("min");
        if (!obj.attr("disabled")) {
            if (obj.val() > min) {
                var newvalue = roundNumber((curr * 100 - step) / 100, 2);
                var max = obj.attr("max");
                if (newvalue < min) newvalue = curr;

                if ($("#" + id + " option").size() <= 1) {
                    CreateOption(obj, newvalue, max, min, 0.01, 2);
                }
                else {
                    //$("#" + id + " option[value='" + newvalue + "']").attr("selected", "selected");
                    $("#" + id).val(newvalue);
                }

                if (id.indexOf(strListMaster) >= 0) {
                    masterEditMemberBingoSetting.on_ChangeMasterPT(document.getElementById(id));
                }
            }
        }
    },
    IncreasePT: function (id, step) {
        var obj = $("#" + id);
        if (!obj.attr("disabled")) {
            var curr = obj.val();
            var min = obj.attr("min");
            var max = obj.attr("max");
            if (obj.val() < max) {
                var newvalue = roundNumber((curr * 100 + step) / 100, 2);
                if (newvalue > max) newvalue = curr;
                if ($("#" + id + " option").size() <= 1) {
                    CreateOption(obj, newvalue, max, min, 0.01, 2);
                }
                else {
                    //$("#" + id + " option[value='" + newvalue + "']").attr("selected", "selected");
                    $("#" + id).val(newvalue);
                }
                if (id.indexOf(strListMaster) >=0 ) {
                    masterEditMemberBingoSetting.on_ChangeMasterPT(document.getElementById(id));
                }
            }
        }
    }
}