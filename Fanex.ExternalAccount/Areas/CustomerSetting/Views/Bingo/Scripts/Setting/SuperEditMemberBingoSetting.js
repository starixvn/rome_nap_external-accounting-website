var superEditMemberBingoSetting = {
    renderPositionTaking: function (positionTakingSetting) {
        var html = '<table class="tblcollapse" style="width: 100%; margin:6px 0px;" id="tblbingo">\
                        <tr class="title">\
                            <td colspan="7">Position Taking</td>\
                        </tr>';
        $.each(positionTakingSetting.gameKindList, function (index, value) {
            html += superEditMemberBingoSetting.renderGameKindBO(positionTakingSetting.isGroupSetting, value);
        });
        return html + '</table>';
    },
    renderGameKindBO: function (isGroupSetting, gameKindBO) {
        var id = gameKindBO.iD;
        var html = '<tr>\
                        <td colspan="2" class="c"></td>\
                        <td class="c"><b>Super PT</b></td>\
                        <td colspan="2" class="c"><b>Super Auto PT</b></td>\
                        <td class="c"><b>Master PT</b></td>\
                        <td class="c"><b>Agent PT</b></td>\
                    </tr>\
                    <tr>\
                        <td rowspan="' + (2 * gameKindBO.gameTypeList.length + 1) + '"><b>' + gameKindBO.name + '</b></td>\
                        <td class="l"><b>Group Setting</b></td>\
                        <td>\
                            <select id="listSuperPT_' + id + '" onchange="superEditMemberBingoSetting.on_ChangeSuperPT(this);" max="' + roundNumber(gameKindBO.positionTakingMax, 2) + '" min="' + roundNumber(gameKindBO.minPositionTaking, 2) + '" class="select-pt" ' + (gameKindBO.isAutoPositionTaking ? 'Disabled' : '') + '>\
                                <option value="' + roundNumber(gameKindBO.positionTaking, 2) + '" selected="true">' + roundNumber(gameKindBO.positionTaking, 2) + '</option>\
                            </select>\
                            <input type="button" onclick="superEditMemberBingoSetting.ReducePT(\'listSuperPT_' + id + '\',1)" class="btn-dec1 SuperPT_' + id + '" value=" "' + (gameKindBO.isAutoPositionTaking ? 'Disabled' : '') + '>\
                            <input type="button" onclick="superEditMemberBingoSetting.IncreasePT(\'listSuperPT_' + id + '\',1)" class="btn-inc1 SuperPT_' + id + '" value=" "' + (gameKindBO.isAutoPositionTaking ? 'Disabled' : '') + '>\
                            <input type="button" onclick="superEditMemberBingoSetting.ReducePT(\'listSuperPT_' + id + '\',10)" class="btn-dec10 SuperPT_' + id + '" value=" "' + (gameKindBO.isAutoPositionTaking ? 'Disabled' : '') + '>\
                            <input type="button" onclick="superEditMemberBingoSetting.IncreasePT(\'listSuperPT_' + id + '\',10)" class="btn-inc10 SuperPT_' + id + '" value=" "' + (gameKindBO.isAutoPositionTaking ? 'Disabled' : '') + '>\
                        </td>\
                        <td>\
                            <input type="checkbox"' + (gameKindBO.isAutoPositionTaking ? 'checked' : '') + ' onclick="superEditMemberBingoSetting.checkAutoPT(this)" id="chkAuto_' + id + '">\
                        </td>\
                        <td>\
                            <select id="listSuperAutoPT_' + id + '" onchange="superEditMemberBingoSetting.on_ChangeSuperAutoPT(this);" max="0.9" min="0" class="select-pt" ' + (!gameKindBO.isAutoPositionTaking ? 'Disabled' : '') + '>\
                                <option value="' + roundNumber(gameKindBO.autoPositionTaking, 2) + '" selected="true">' + roundNumber(gameKindBO.autoPositionTaking, 2) + '</option>\
                            </select>\
                            <input type="button" onclick="superEditMemberBingoSetting.ReducePT(\'listSuperAutoPT_' + id + '\',1)" class="btn-dec1 SuperAuto_' + id + '" value=" "' + (!gameKindBO.isAutoPositionTaking ? 'Disabled' : '') + '>\
                            <input type="button" onclick="superEditMemberBingoSetting.IncreasePT(\'listSuperAutoPT_' + id + '\',1)" class="btn-inc1 SuperAuto_' + id + '" value=" "' + (!gameKindBO.isAutoPositionTaking ? 'Disabled' : '') + '>\
                            <input type="button" onclick="superEditMemberBingoSetting.ReducePT(\'listSuperAutoPT_' + id + '\',10)" class="btn-dec10 SuperAuto_' + id + '" value=" "' + (!gameKindBO.isAutoPositionTaking ? 'Disabled' : '') + '>\
                            <input type="button" onclick="superEditMemberBingoSetting.IncreasePT(\'listSuperAutoPT_' + id + '\',10)" class="btn-inc10 SuperAuto_' + id + '" value=" "' + (!gameKindBO.isAutoPositionTaking ? 'Disabled' : '') + '>\
                        </td>\
                        <td>' + roundNumber(gameKindBO.positionTakingMaster,2) + '</td>\
                        <td>' + roundNumber(gameKindBO.positionTakingAgent,2) + '</td>\
                    </tr>';
        $.each(gameKindBO.gameTypeList, function (index, value) {
            html += superEditMemberBingoSetting.renderGameTypeBO(isGroupSetting, id, value);
        });
        return html;
    },
    renderGameTypeBO: function (isGroupSetting, gameKindBOId, gameTypeBO) {
        var id = gameKindBOId + '_' + gameTypeBO.iD;
        var html = '<tr>\
                    <td class="l" rowspan="2">' + gameTypeBO.name + '</td>';
        if (!isGroupSetting) {
            html += '<td class="gametypePT c">\
                        <select id="listSuperPT_' + id + '" max="' + roundNumber(gameTypeBO.positionTakingMax, 2) + '" min="' + roundNumber(gameTypeBO.minPositionTaking, 2) + '" class="select-pt SuperPT_' + id + '"' + (gameTypeBO.isAutoPositionTaking ? 'Disabled' : '') + '>\
                            <option value="' + roundNumber(gameTypeBO.positionTaking, 2) + '" selected="true">' + roundNumber(gameTypeBO.positionTaking, 2) + '</option>\
                        </select>\
                    </td>\
                    <td>\
                        <input class="chkauto" type="checkbox"' + (gameTypeBO.isAutoPositionTaking ? 'checked' : '') + ' onclick="masterBingoSetting.checkAutoPTGameType(this)" id="chkAuto_' + id + '">\
                    </td>\
                    <td class="gametypePT c">\
                        <select id="listSuperAutoPT_' + id + '" max="0.9" min="0" class="select-pt SuperAuto_' + id + '"' + (!gameTypeBO.isAutoPositionTaking ? 'Disabled' : '') + '>\
                            <option value="' + roundNumber(gameTypeBO.autoPositionTaking, 2) + '" selected="true">' + roundNumber(gameTypeBO.autoPositionTaking, 2) + '</option>\
                        </select>\
                    </td>\
                    <td class="gametypePT c">' + roundNumber(gameTypeBO.positionTakingMaster,2) + '</td>\
                    <td class="gametypePT c">' + roundNumber(gameTypeBO.positionTakingAgent,2) + '</td>';
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
            var isAutoGameKind = $("#" + strChkAuto + "_" + gamekindid).prop("checked");

            if (isAutoGameKind) {
                customer.gameKindList[i].autoPositionTaking = roundNumber($("#" + strListSuperAuto + "_" + gamekindid).val(), 2);
                customer.gameKindList[i].positionTaking = 0;
            }
            else {
                customer.gameKindList[i].autoPositionTaking = 0;
                customer.gameKindList[i].positionTaking = roundNumber($("#" + strListSuper + "_" + gamekindid).val(), 2);
            }
            customer.gameKindList[i].isAutoPositionTaking = isAutoGameKind;

            //Loop to get PT setting GameType
            for (var j = 0; j < totalGameType; j++) {
                var gametype = customer.gameKindList[i].gameTypeList[j];
                var gametypeid = gametype.iD;
                totalBettingRange = gametype.bettingRangeList.length;
                if (!customer.isUsedGameKind) {
                    var isAutoGameType = $("#" + strChkAuto + "_" + gamekindid + "_" + gametypeid).prop("checked");
                    if (isAutoGameType) {
                        customer.gameKindList[i].gameTypeList[j].positionTaking = 0;
                        customer.gameKindList[i].gameTypeList[j].autoPositionTaking = roundNumber($("#" + strListSuperAuto + "_" + gamekindid + "_" + gametypeid).val(), 2);
                    }
                    else {
                        customer.gameKindList[i].gameTypeList[j].autoPositionTaking = 0;
                        customer.gameKindList[i].gameTypeList[j].positionTaking = roundNumber($("#" + strListSuper + "_" + gamekindid + "_" + gametypeid).val(), 2);
                    }
                    customer.gameKindList[i].gameTypeList[j].isAutoPositionTaking = isAutoGameType;
                }
                //Loop to get BettingRangeList
                for (var z = 0; z < totalBettingRange; z++) {
                    var bettingrange = customer.gameKindList[i].gameTypeList[j].bettingRangeList[z];
                    var bettingrangeid = bettingrange.ID;
                    customer.gameKindList[i].gameTypeList[j].bettingRangeList[z].isChecked = $("#" + strChkStake + "_" + gamekindid + "_" + gametypeid + "_" + bettingrangeid).prop("checked");
                }
            }
        }
        customerPTBingoSetting.ajaxUpdatePT(customer);
    },
    on_ChangeSuperPT: function (obj) {
        var oSuper = $(obj);
        var strSuperID = oSuper.attr("id");
        var SuperValue = oSuper.val();
        jQuery.each($("." + strSuperID.replace(strListSuper, "SuperPT")), function () {
            if ($(this).prop("id").indexOf(strSuperID) >= 0) {
                $(this).val(SuperValue);
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
                if (id.indexOf(strListSuper) >= 0) {
                    superEditMemberBingoSetting.on_ChangeSuperPT(document.getElementById(id));
                }
                else {
                    superEditMemberBingoSetting.on_ChangeSuperAutoPT(document.getElementById(id));
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
                if (id.indexOf(strListSuper) >= 0) {
                    superEditMemberBingoSetting.on_ChangeSuperPT(document.getElementById(id));
                }
                else  {
                    superEditMemberBingoSetting.on_ChangeSuperAutoPT(document.getElementById(id));
                }
            }
        }
    },
    on_ChangeSuperAutoPT: function (obj) {
        var Auto = $(obj);
        var AutoID = Auto.attr("id");
        var AutoValue = Auto.val();
        var Autolists = $(".SuperAuto" + AutoID.replace(strListSuperAuto,""));
        Autolists.each(function () {
            if ($(this).prop("id").indexOf(AutoID + "_") >= 0) {
                $(this).val(AutoValue);
            }
        });
    },
    checkAutoPT: function (obj) {   
        var chkAuto = $(obj);
        var chkAutoID = chkAuto.attr("id");
        if (chkAuto.prop("checked")) {
            $("#" + chkAutoID.replace(strChkAuto,strListSuper)).attr("disabled","disabled");
            $("#" + chkAutoID.replace(strChkAuto,strListSuperAuto)).removeAttr("disabled");
        
            $(".SuperPT" + chkAutoID.replace(strChkAuto,"")).attr("disabled","disabled");
            $(".SuperAuto" + chkAutoID.replace(strChkAuto,"")).removeAttr("disabled");
              
            jQuery.each($(".chkauto"), function() {
                if ($(this).prop("id").indexOf(chkAutoID) >= 0) {
                    $(this).attr("checked","checked");
                    superEditMemberBingoSetting.CheckAutoPTGameType(this);
                 }
            });
        }
        else {
            $("#" + chkAutoID.replace(strChkAuto,strListSuperAuto)).attr("disabled","disabled");
            $("#" + chkAutoID.replace(strChkAuto,strListSuper)).removeAttr("disabled");
        
            $(".SuperAuto" + chkAutoID.replace(strChkAuto,"")).attr("disabled","disabled");
            $(".SuperPT" + chkAutoID.replace(strChkAuto,"")).removeAttr("disabled");
        
            jQuery.each($(".chkauto"), function() {
                if ($(this).prop("id").indexOf(chkAutoID)>=0) {
                    $(this).removeAttr("checked");
                    superEditMemberBingoSetting.CheckAutoPTGameType(this);
                }
            });
        }
    },
    checkAutoPTGameType: function (obj) {
        var chkAuto = $(obj);
        var chkAutoID = chkAuto.attr("id");
        if (chkAuto.prop("checked")) {
            $("#" + chkAutoID.replace(strChkAuto,strListSuper)).attr("disabled","disabled");
            $("#" + chkAutoID.replace(strChkAuto,strListSuperAuto)).removeAttr("disabled");
        }
        else {
            $("#" + chkAutoID.replace(strChkAuto,strListSuperAuto)).attr("disabled","disabled");
            $("#" + chkAutoID.replace(strChkAuto,strListSuper)).removeAttr("disabled");
        }
    }
};