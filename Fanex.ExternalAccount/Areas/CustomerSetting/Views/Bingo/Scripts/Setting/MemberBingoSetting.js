var memberBingoSetting = {
    renderPositionTaking: function (positionTakingSetting) {
        var html = '<table class="tblcollapse" style="width: 100%; margin:6px 0px;" id="tblbingo">\
                        <tr class="title">\
                            <td colspan="4">Position Taking</td>\
                        </tr>';
        $.each(positionTakingSetting.gameKindList, function (index, value) {
            html += memberBingoSetting.renderGameKindBO(positionTakingSetting.isGroupSetting, value);
        });
        return html + '</table>';
    },
    renderGameKindBO: function (isGroupSetting, gameKindBO) {
        var id = gameKindBO.iD;
        var html = '<tr>\
                        <td colspan="2" class="c"></td>\
                        <td class="c"><b>Agent Preset PT</b></td>\
                        <td class="c"><b>Member Commission</b></td>\
                    </tr>\
                    <tr>\
                        <td rowspan="' + (2 * gameKindBO.gameTypeList.length + 1) + '"><b>' + gameKindBO.name + '</b></td>\
                        <td class="l"><b>Group Setting</b></td>\
                        <td>\
                            <select id="listPrePT_' + id + '" onchange="on_ChangePreset(this);" max="' + roundNumber(gameKindBO.positionTakingMax, 2) + '" min="' + roundNumber(gameKindBO.minPositionTaking, 2) + '" class="select-pt" ' + (gameKindBO.isAutoPositionTaking ? 'Disabled' : '') + '>\
                                <option value="' + roundNumber(gameKindBO.positionTaking, 2) + '" selected="true">' + roundNumber(gameKindBO.positionTaking, 2) + '</option>\
                            </select>\
                            <input type="button" onclick="memberBingoSetting.ReducePT(\'listPrePT_' + id + '\',1)" class="btn-dec1 Pre_' + id + '" value=" "' + (gameKindBO.isAutoPositionTaking ? 'Disabled' : '') + '>\
                            <input type="button" onclick="memberBingoSetting.IncreasePT(\'listPrePT_' + id + '\',1)" class="btn-inc1 Pre_' + id + '" value=" "' + (gameKindBO.isAutoPositionTaking ? 'Disabled' : '') + '>\
                            <input type="button" onclick="memberBingoSetting.ReducePT(\'listPrePT_' + id + '\',10)" class="btn-dec10 Pre_' + id + '" value=" "' + (gameKindBO.isAutoPositionTaking ? 'Disabled' : '') + '>\
                            <input type="button" onclick="memberBingoSetting.IncreasePT(\'listPrePT_' + id + '\',10)" class="btn-inc10 Pre_' + id + '" value=" "' + (gameKindBO.isAutoPositionTaking ? 'Disabled' : '') + '>\
                        </td>\
                        <td>\
                            <select id="listComm_' + id + '" onchange="On_ChangeComm(this);" max="' + roundNumber(gameKindBO.commissionUpline, 4) + '" min="0" class="select-comm">\
                                <option value="' + roundNumber(gameKindBO.commission, 4) + '" selected="true">' + roundNumber(gameKindBO.commission, 4) + '</option>\
                            </select>\
                        </td>\
                    </tr>';
        $.each(gameKindBO.gameTypeList, function (index, value) {
            html += memberBingoSetting.renderGameTypeBO(isGroupSetting, id, value);
        });
        return html;
    },
    renderGameTypeBO: function (isGroupSetting, gameKindBOId, gameTypeBO) {
        var id = gameKindBOId + '_' + gameTypeBO.iD;
        var html = '<tr>\
                    <td class="l" rowspan="2">' + gameTypeBO.name + '</td>';
        if (!isGroupSetting) {
            html += '<td class="gametypePT c">\
                        <select id="listPrePT_' + id + '" max="' + roundNumber(gameTypeBO.positionTakingMax, 2) + '" min="' + roundNumber(gameTypeBO.minPositionTaking, 2) + '" class="select-pt Pre_' + id + '"' + (gameTypeBO.isAutoPositionTaking ? 'Disabled' : '') + '>\
                            <option value="' + roundNumber(gameTypeBO.positionTaking, 2) + '" selected="true">' + roundNumber(gameTypeBO.positionTaking, 2) + '</option>\
                        </select>\
                    </td>\
                    <td class="gametypePT c">\
                        <select id="listComm_' + id + '" max="' + roundNumber(gameTypeBO.commissionUpline, 4) + '" min="0" class="select-comm">\
                            <option value="' + roundNumber(gameTypeBO.commission, 4) + '" selected="true">' + roundNumber(gameTypeBO.commission, 4) + '</option>\
                        </select>\
                    </td>';
        }
        html += '</tr>\
                <tr>\
                    <td class="l" colspan="6">Betting Limit:';
        $.each(gameTypeBO.bettingRangeList, function (index, value) {
            var generalId = id + '_' + value.iD;
            html += ptBingoCommon.renderBettingRange(gameKindBOId, generalId, value);
        });
        return html + '</td></tr>';
    },
    updatePT: function (customer) {
        var maxTransferCredit = parent.Fanex.ExtAcc.ReplaceAll($("#txtMaxTransferCredit").val(), ',', '');
        if (!uInteger(maxTransferCredit)) {
            showAlertMessage("err", "Invalid Max Transder Credit", 1);
            return;
        }
        if (customer.maxLimit > 0) {
            if (customer.minLimit > maxTransferCredit || maxTransferCredit > customer.maxLimit) {
                showAlertMessage("err", "Invalid Max Transder Credit", 1);
                return;
            }
        }
        else {
            if (customer.minLimit > maxTransferCredit) {
                showAlertMessage("err", "Invalid Max Transder Credit", 1);
                return;
            }
        }
        PTBlockUI();
        var totalGameKind = customer.gameKindList.length;
        var totalGameType;
        var totalBettingRange;
        //Loop to get PT setting GameKind
        customer.maxTransferCredit = maxTransferCredit;
        customer.isAutoReset = $("#cbxAutoReset").prop("checked");
        customer.incentiveRate = customer.maxPayout;

        for (var i = 0; i < totalGameKind; i++) {
            var gamekind = customer.gameKindList[i];
            var gamekindid = gamekind.iD;
            totalGameType = gamekind.gameTypeList.length;
            customer.gameKindList[i].positionTaking = roundNumber($("#" + strListPreset + "_" + gamekindid).val(), 2);
            customer.gameKindList[i].commission = roundNumber($("#" + strListComm + "_" + gamekindid).val(), 4);
            //Loop to get PT setting GameType
            for (var j = 0; j < totalGameType; j++) {
                var gametype = customer.gameKindList[i].gameTypeList[j];
                var gametypeid = gametype.iD;
                totalBettingRange = gametype.bettingRangeList.length;
                if (!customer.isUsedGameKind) {
                    customer.gameKindList[i].gameTypeList[j].positionTaking = roundNumber($("#" + strListPreset + "_" + gamekindid + "_" + gametypeid).val(), 2);
                    customer.gameKindList[i].gameTypeList[j].commission = roundNumber($("#" + strListComm + "_" + gamekindid + "_" + gametypeid).val(), 4);
                }
                //Loop to get BettingRangeList
                for (var z = 0; z < totalBettingRange; z++) {
                    var bettingrange = customer.gameKindList[i].gameTypeList[j].bettingRangeList[z];
                    var bettingrangeid = bettingrange.ID;
                    customer.gameKindList[i].gameTypeList[j].bettingRangeList[z].isChecked = $("#" + strChkStake + "_" + gamekindid + "_" + gametypeid + "_" + bettingrangeid).attr("checked");
                }
            }
        }
        customerPTBingoSetting.ajaxUpdatePT(customer);
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
                if (id.indexOf(strListPreset) >= 0) {
                    memberBingoSetting.on_ChangePreset(document.getElementById(id));
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

                if (id.indexOf(strListPreset) >= 0) {
                    memberBingoSetting.on_ChangePreset(document.getElementById(id));
                }
            }
        }
    },
    on_ChangePreset: function (obj) {
        var oPreset = $(obj);
        var strPresetID = oPreset.attr("id");
        var PresetValue = oPreset.val();
        jQuery.each($("." + strPresetID.replace(strListPreset, "Pre")), function () {
            if ($(this).prop("id").indexOf(strPresetID) >= 0) {
                $(this).val(PresetValue);
            }
        });
    },
    // Comm onChange
    on_ChangeComm: function (obj) {
        var Commlist = $(obj);
        var CommId = Commlist.attr("id");
        var CommValue = Commlist.val();
        var Commlists = $(".select-comm");
        Commlists.each(function () {
            if ($(this).prop("id").indexOf(CommId + "_") >= 0) {
                $(this).val(CommValue);
            }
        });
    }
};