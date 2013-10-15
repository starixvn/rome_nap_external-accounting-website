var masterBingoSetting = {
    renderPositionTaking: function (positionTakingSetting) {
        var html = '<table class="tblcollapse" style="width: 100%; margin:6px 0px;" id="tblbingo">\
                        <tr class="title">\
                            <td colspan="8">Position Taking</td>\
                        </tr>';
        $.each(positionTakingSetting.gameKindList, function (index, value) {
            html += masterBingoSetting.renderGameKindBO(positionTakingSetting.isGroupSetting, value);
        });
        return html + '</table>';
    },
    renderGameKindBO: function (isGroupSetting, gameKindBO) {
        var id = gameKindBO.iD;
        var html = '<tr>\
                        <td colspan="2" class="c"></td>\
                        <td class="c"><b>Super Preset PT</b></td>\
                        <td colspan="2" class="c"><b>Super Auto Preset PT</b></td>\
                        <td class="c"><b>Master Max PT</b></td>\
                        <td class="c"><b>Master Min PT</b></td>\
                        <td class="c"><b>Master Commission</b></td>\
                    </tr>\
                    <tr>\
                        <td rowspan="' + (2 * gameKindBO.gameTypeList.length + 1) + '"><b>' + gameKindBO.name + '</b></td>\
                        <td class="l"><b>Group Setting</b></td>\
                        <td>\
                            <select id="listPrePT_' + id + '" onchange="masterBingoSetting.on_ChangePreset(this);" max="' + roundNumber(gameKindBO.positionTakingUplineMax, 2) + '" min="0" class="select-pt" ' + (gameKindBO.isAutoPositionTaking ? 'Disabled' : '') + '>\
                                <option value="' + roundNumber(gameKindBO.positionTakingUpline, 2) + '" selected="true">' + roundNumber(gameKindBO.positionTakingUpline, 2) + '</option>\
                            </select>\
                            <input type="button" onclick="masterBingoSetting.ReducePT(\'listPrePT_' + id + '\',1)" class="btn-dec1 Pre_' + id + '" value=" "' + (gameKindBO.isAutoPositionTaking ? 'Disabled' : '') + '>\
                            <input type="button" onclick="masterBingoSetting.IncreasePT(\'listPrePT_' + id + '\',1)" class="btn-inc1 Pre_' + id + '" value=" "' + (gameKindBO.isAutoPositionTaking ? 'Disabled' : '') + '>\
                            <input type="button" onclick="masterBingoSetting.ReducePT(\'listPrePT_' + id + '\',10)" class="btn-dec10 Pre_' + id + '" value=" "' + (gameKindBO.isAutoPositionTaking ? 'Disabled' : '') + '>\
                            <input type="button" onclick="masterBingoSetting.IncreasePT(\'listPrePT_' + id + '\',10)" class="btn-inc10 Pre_' + id + '" value=" "' + (gameKindBO.isAutoPositionTaking ? 'Disabled' : '') + '>\
                        </td>\
                        <td>\
                            <input type="checkbox"' + (gameKindBO.isAutoPositionTaking ? 'checked' : '') + ' onclick="masterBingoSetting.checkAutoPresetPT(this)" id="chkAutoPreset_' + id + '">\
                        </td>\
                        <td>\
                            <select id="listAutoPrePT_' + id + '" onchange="masterBingoSetting.on_ChangeAutoPreset(this);" max="' + roundNumber(gameKindBO.positionTakingUplineMax, 2) + '" min="0" class="select-pt" ' + (!gameKindBO.isAutoPositionTaking ? 'Disabled' : '') + '>\
                                <option value="' + roundNumber(gameKindBO.autoPositionTaking, 2) + '" selected="true">' + roundNumber(gameKindBO.autoPositionTaking, 2) + '</option>\
                            </select>\
                            <input type="button" onclick="masterBingoSetting.ReducePT(\'listAutoPrePT_' + id + '\',1)" class="btn-dec1 AutoPre_' + id + '" value=" "' + (!gameKindBO.isAutoPositionTaking ? 'Disabled' : '') + '>\
                            <input type="button" onclick="masterBingoSetting.IncreasePT(\'listAutoPrePT_' + id + '\',1)" class="btn-inc1 AutoPre_' + id + '" value=" "' + (!gameKindBO.isAutoPositionTaking ? 'Disabled' : '') + '>\
                            <input type="button" onclick="masterBingoSetting.ReducePT(\'listAutoPrePT_' + id + '\',10)" class="btn-dec10 AutoPre_' + id + '" value=" "' + (!gameKindBO.isAutoPositionTaking ? 'Disabled' : '') + '>\
                            <input type="button" onclick="masterBingoSetting.IncreasePT(\'listAutoPrePT_' + id + '\',10)" class="btn-inc10 AutoPre_' + id + '" value=" "' + (!gameKindBO.isAutoPositionTaking ? 'Disabled' : '') + '>\
                        </td>\
                        <td>\
                            <select id="listMaxPT_' + id + '" onchange="masterBingoSetting.on_ChangeMax(this);" max="' + roundNumber(gameKindBO.positionTakingMax, 2) + '" min="0" class="select-pt">\
                                <option value="' + roundNumber(gameKindBO.positionTaking, 2) + '" selected="true">' + roundNumber(gameKindBO.positionTaking, 2) + '</option>\
                            </select>\
                            <input type="button" onclick="masterBingoSetting.ReducePT(\'listMaxPT_' + id + '\',1)" class="btn-dec1" value=" ">\
                            <input type="button" onclick="masterBingoSetting.IncreasePT(\'listMaxPT_' + id + '\',1)" class="btn-inc1" value=" ">\
                            <input type="button" onclick="masterBingoSetting.ReducePT(\'listMaxPT_' + id + '\',10)" class="btn-dec10" value=" ">\
                            <input type="button" onclick="masterBingoSetting.IncreasePT(\'listMaxPT_' + id + '\',10)" class="btn-inc10" value=" ">\
                        </td>\
                        <td>\
                            <select id="listMinPT_' + id + '" onchange="masterBingoSetting.on_ChangeMin(this);" max="' + roundNumber(gameKindBO.positionTaking, 2) + '" min="0" class="select-pt">\
                                <option value="' + roundNumber(gameKindBO.minPositionTaking, 2) + '" selected="true">' + roundNumber(gameKindBO.minPositionTaking, 2) + '</option>\
                            </select>\
                            <input type="button" onclick="masterBingoSetting.ReducePT(\'listMinPT_' + id + '\',1)" class="btn-dec1" value=" ">\
                            <input type="button" onclick="masterBingoSetting.IncreasePT(\'listMinPT_' + id + '\',1)" class="btn-inc1" value=" ">\
                            <input type="button" onclick="masterBingoSetting.ReducePT(\'listMinPT_' + id + '\',10)" class="btn-dec10" value=" ">\
                            <input type="button" onclick="masterBingoSetting.IncreasePT(\'listMinPT_' + id + '\',10)" class="btn-inc10" value=" ">\
                        </td>\
                        <td>\
                            <select id="listComm_' + id + '" onchange="masterBingoSetting.on_ChangeComm(this);" max="' + roundNumber(gameKindBO.commissionUpline, 4) + '" min="0" class="select-comm">\
                                <option value="' + roundNumber(gameKindBO.commission, 4) + '" selected="true">' + roundNumber(gameKindBO.commission, 4) + '</option>\
                            </select>\
                        </td>\
                    </tr>';
        $.each(gameKindBO.gameTypeList, function (index, value) {
            html += masterBingoSetting.renderGameTypeBO(isGroupSetting, id, value);
        });
        return html;
    },
    //    renderGameTypeBO: function (isGroupSetting, gameKindBOId, gameTypeBO) {
    //        var id = gameKindBOId + '_' + gameTypeBO.iD;
    //        var html = '<tr><td class="l" rowspan="2">' + gameTypeBO.name + '</td>';
    //        if (!isGroupSetting) {
    //            html += '<td class="gametypePT c">\
    //                        <select id="listPrePT_' + id + '" onchange="masterBingoSetting.on_ChangePresetGameType(this)" max="' + roundNumber(gameTypeBO.positionTakingUplineMax, 2) + '" min="0" class="select-pt Pre_' + gameKindBOId + '"' + (gameTypeBO.isAutoPositionTaking ? 'Disabled' : '') + '>\
    //                            <option value="' + roundNumber(gameTypeBO.positionTakingUpline, 2) + '" selected="true">' + roundNumber(gameTypeBO.positionTakingUpline, 2) + '</option>\
    //                        </select>\
    //                    </td>\
    //                    <td>\
    //                        <input class="chkauto" type="checkbox"' + (gameTypeBO.isAutoPositionTaking ? 'checked' : '') + ' onclick="masterBingoSetting.checkAutoPresetPTGameType(this)" id="chkAutoPreset_' + id + '">\
    //                    </td>\
    //                    <td class="gametypePT c">\
    //                        <select id="listAutoPrePT_' + id + '" max="' + roundNumber(gameTypeBO.positionTakingUplineMax, 2) + '" min="0" class="select-pt AutoPre_' + id + '"' + (!gameTypeBO.isAutoPositionTaking ? 'Disabled' : '') + '>\
    //                            <option value="' + roundNumber(gameTypeBO.positionTakingUpline, 2) + '" selected="true">' + roundNumber(gameTypeBO.positionTakingUpline, 2) + '</option>\
    //                        </select>\
    //                    </td>\
    //                    <td class="gametypePT c">\
    //                        <select id="listMaxPT_' + id + '" onchange="masterBingoSetting.on_ChangeMaxGameType(this);" max="' + roundNumber(gameTypeBO.positionTakingMax, 2) + '" min="0" class="select-pt Max_' + id + '">\
    //                            <option value="' + roundNumber(gameTypeBO.positionTaking, 2) + '" selected="true">' + roundNumber(gameTypeBO.positionTaking, 2) + '</option>\
    //                        </select><span class="errPT" id="errPT_' + id + '" style="color:#FF0000;visibility:hidden;"> * </span>\
    //                    </td>\
    //                    <td class="gametypePT c">\
    //                        <select id="listMinPT_' + id + '" max="' + roundNumber(gameTypeBO.positionTakingMax, 2) + '" min="0" class="select-pt Min">\
    //                            <option value="' + roundNumber(gameTypeBO.minPositionTaking, 2) + '" selected="true">' + roundNumber(gameTypeBO.minPositionTaking, 2) + '</option>\
    //                        </select><span class="errPT" id="errPTMin_' + id + '" style="color:#FF0000;visibility:hidden;"> * </span>\
    //                    </td>\
    //                    <td class="gametypePT c">\
    //                        <select id="listComm_' + id + '" max="' + roundNumber(gameTypeBO.commissionUpline, 4) + '" min="0" class="select-comm">\
    //                            <option value="' + roundNumber(gameTypeBO.commission, 4) + '" selected="true">' + roundNumber(gameTypeBO.commission, 4) + '</option>\
    //                        </select>\
    //                    </td>';
    //        }
    //        html += '</tr>\
    //                    <tr>\
    //                    <td class="l" colspan="6">Betting Limit:';
    //        $.each(gameTypeBO.bettingRangeList, function (index, value) {
    //            var generalId = id + '_' + value.iD;
    //            html += ptBingoCommon.renderBettingRange(gameKindBOId, generalId, value);
    //        });
    //        return html + '</td></tr>';
    //    },
    renderGameTypeBO: function (isGroupSetting, gameKindBOId, gameTypeBO) {
        var id = gameKindBOId + '_' + gameTypeBO.iD;
        var html = '<tr><td class="l" rowspan="2">' + gameTypeBO.name + '</td>\
                    <td class="gametypePT c" style="' + (isGroupSetting ? 'display:none' : '') + '">\
                        <select id="listPrePT_' + id + '" onchange="masterBingoSetting.on_ChangePresetGameType(this)" max="' + roundNumber(gameTypeBO.positionTakingUplineMax, 2) + '" min="0" class="select-pt Pre_' + gameKindBOId + '"' + (gameTypeBO.isAutoPositionTaking ? 'Disabled' : '') + '>\
                            <option value="' + roundNumber(gameTypeBO.positionTakingUpline, 2) + '" selected="true">' + roundNumber(gameTypeBO.positionTakingUpline, 2) + '</option>\
                        </select>\
                    </td>\
                    <td style="' + (isGroupSetting ? 'display:none' : '') + '">\
                        <input class="chkauto" type="checkbox"' + (gameTypeBO.isAutoPositionTaking ? 'checked' : '') + ' onclick="masterBingoSetting.checkAutoPresetPTGameType(this)" id="chkAutoPreset_' + id + '">\
                    </td>\
                    <td class="gametypePT c" style="' + (isGroupSetting ? 'display:none' : '') + '">\
                        <select id="listAutoPrePT_' + id + '" max="' + roundNumber(gameTypeBO.positionTakingUplineMax, 2) + '" min="0" class="select-pt AutoPre_' + id + '"' + (!gameTypeBO.isAutoPositionTaking ? 'Disabled' : '') + '>\
                            <option value="' + roundNumber(gameTypeBO.positionTakingUpline, 2) + '" selected="true">' + roundNumber(gameTypeBO.positionTakingUpline, 2) + '</option>\
                        </select>\
                    </td>\
                    <td class="gametypePT c" style="' + (isGroupSetting ? 'display:none' : '') + '">\
                        <select id="listMaxPT_' + id + '" onchange="masterBingoSetting.on_ChangeMaxGameType(this);" max="' + roundNumber(gameTypeBO.positionTakingMax, 2) + '" min="0" class="select-pt Max_' + id + '">\
                            <option value="' + roundNumber(gameTypeBO.positionTaking, 2) + '" selected="true">' + roundNumber(gameTypeBO.positionTaking, 2) + '</option>\
                        </select><span class="errPT" id="errPT_' + id + '" style="color:#FF0000;visibility:hidden;"> * </span>\
                    </td>\
                    <td class="gametypePT c" style="' + (isGroupSetting ? 'display:none' : '') + '">\
                        <select id="listMinPT_' + id + '" max="' + roundNumber(gameTypeBO.positionTakingMax, 2) + '" min="0" class="select-pt Min">\
                            <option value="' + roundNumber(gameTypeBO.minPositionTaking, 2) + '" selected="true">' + roundNumber(gameTypeBO.minPositionTaking, 2) + '</option>\
                        </select><span class="errPT" id="errPTMin_' + id + '" style="color:#FF0000;visibility:hidden;"> * </span>\
                    </td>\
                    <td class="gametypePT c" style="' + (isGroupSetting ? 'display:none' : '') + '">\
                        <select id="listComm_' + id + '" max="' + roundNumber(gameTypeBO.commissionUpline, 4) + '" min="0" class="select-comm">\
                            <option value="' + roundNumber(gameTypeBO.commission, 4) + '" selected="true">' + roundNumber(gameTypeBO.commission, 4) + '</option>\
                        </select>\
                    </td>\
                </tr>\
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
        customer.incentiveRate = $('#listIncentiveRate').val();
        for (var i = 0; i < totalGameKind; i++) {
            var gamekind = customer.gameKindList[i];
            var gamekindid = gamekind.iD;
            totalGameType = gamekind.gameTypeList.length;
            var isAutoGameKind = $("#" + strChkAutoPreset + "_" + gamekindid).attr("checked");

            if (isAutoGameKind) {
                customer.gameKindList[i].autoPositionTaking = roundNumber($("#" + strListAutoPreset + "_" + gamekindid).val(), 2);
                customer.gameKindList[i].positionTakingUpline = 0;
            }
            else {
                customer.gameKindList[i].autoPositionTaking = 0;
                customer.gameKindList[i].positionTakingUpline = roundNumber($("#" + strListPreset + "_" + gamekindid).val(), 2);
            }
            customer.gameKindList[i].isAutoPositionTaking = isAutoGameKind;
            customer.gameKindList[i].positionTaking = roundNumber($("#" + strListMax + "_" + gamekindid).val(), 2);
            customer.gameKindList[i].minPositionTaking = roundNumber($("#" + strListMin + "_" + gamekindid).val(), 2);
            customer.gameKindList[i].commission = roundNumber($("#" + strListComm + "_" + gamekindid).val(), 4);
            //Loop to get PT setting GameType
            for (var j = 0; j < totalGameType; j++) {
                var gametype = customer.gameKindList[i].gameTypeList[j];
                var gametypeid = gametype.ID;
                totalBettingRange = gametype.bettingRangeList.length;
                if (!customer.isUsedGameKind) {
                    var isAutoGameType = $("#" + strChkAutoPreset + "_" + gamekindid + "_" + gametypeid).attr("checked");
                    if (isAutoGameType) {
                        customer.gameKindList[i].gameTypeList[j].autoPositionTaking = roundNumber($("#" + strListAutoPreset + "_" + gamekindid + "_" + gametypeid).val(), 2);
                        customer.gameKindList[i].gameTypeList[j].positionTakingUpline = 0;
                    }
                    else {
                        customer.gameKindList[i].gameTypeList[j].autoPositionTaking = 0;
                        customer.gameKindList[i].gameTypeList[j].positionTakingUpline = roundNumber($("#" + strListPreset + "_" + gamekindid + "_" + gametypeid).val(), 2);
                    }
                    customer.gameKindList[i].gameTypeList[j].isAutoPositionTaking = isAutoGameType;
                    customer.gameKindList[i].gameTypeList[j].positionTaking = roundNumber($("#" + strListMax + "_" + gamekindid + "_" + gametypeid).val(), 2);
                    customer.gameKindList[i].gameTypeList[j].minPositionTaking = roundNumber($("#" + strListMin + "_" + gamekindid + "_" + gametypeid).val(), 2);
                    customer.gameKindList[i].gameTypeList[j].commission = roundNumber($("#" + strListComm + "_" + gamekindid + "_" + gametypeid).val(), 4);
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
    // Max PT onChange
    on_ChangeMax: function (obj) {
        var oMax = $(obj);
        var strMaxID = oMax.attr("id");
        var MaxValue = oMax.val();

        jQuery.each($("." + strMaxID.replace(strListMax, "Max")), function () {
            if ($(this).prop("id").indexOf(strMaxID) >= 0) {
                if ($(this).attr("max") >= MaxValue) {
                    $(this).val(MaxValue);
                    masterBingoSetting.on_ChangeMaxGameType(this);
                    $("#" + $(this).attr("id").replace(strListMax, strerrPT)).css("visibility", "hidden");
                }
                else {
                    $("#" + $(this).attr("id").replace(strListMax, strerrPT)).css("visibility", "visible");
                }
            }
        });

        var oMin = $("#" + strMaxID.replace(strListMax, strListMin));
        var strMinID = oMin.attr("id");
        var MinValue = oMin.val();
        var MinValueMax = MaxValue;

        if (MinValue > MinValueMax) {
            MinValue = MinValueMax;
        }
        oMin.attr("max", MinValueMax);
        CreateOption(oMin, MinValue, MinValueMax, 0, 0.01, 2);
    },
    // Min PT onChange
    on_ChangeMin: function (obj) {
        var Min = $(obj);
        var MinID = Min.attr("id");
        var MinValue = Min.val();
        var Minlists = $(".Min");
        Minlists.each(function () {
            if ($(this).attr("id").indexOf(MinID + "_") >= 0) {
                if (MinValue <= $(this).attr("max")) {
                    $(this).val(MinValue);
                    $("#" + $(this).attr("id").replace(strListMin, strerrPTMin)).css("visibility", "hidden");
                }
                else {
                    $("#" + $(this).attr("id").replace(strListMin, strerrPTMin)).css("visibility", "visible");
                }
            }
        });
    },
    on_ChangeMaxGameType: function (obj) {
        var oMax = $(obj);
        var strMaxID = oMax.attr("id");
        var MaxValue = oMax.val();
        $("#" + strMaxID.replace(strListMax, strerrPT)).css("visibility", "hidden");
        $("#" + strMaxID.replace(strListMax, strerrPTMin)).css("visibility", "hidden");
        var oMin = $("#" + strMaxID.replace(strListMax, strListMin));

        var MinValue = oMin.val();
        var MinValueMax = MaxValue;

        if (MinValue > MinValueMax) {
            MinValue = MinValueMax;
        }
        oMin.attr("max", MinValueMax);
        CreateOption(oMin, MinValue, MinValueMax, 0, 0.01, 2);
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
                    masterBingoSetting.on_ChangePreset(document.getElementById(id));
                }
                else if (id.indexOf(strListAutoPreset) >= 0) {
                    masterBingoSetting.on_ChangeAutoPreset(document.getElementById(id));
                }
                else if (id.indexOf(strListMax) >= 0) {
                    masterBingoSetting.on_ChangeMax(document.getElementById(id));
                }
                else if (id.indexOf(strListMin) >= 0) {
                    masterBingoSetting.on_ChangeMin(document.getElementById(id));
                }
            }
        }
    },
    IncreasePT: function (id, step) {
        var obj = $("#" + id);
        if (!obj.prop("disabled")) {
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
                    masterBingoSetting.on_ChangePreset(document.getElementById(id));
                }
                else if (id.indexOf(strListAutoPreset) >= 0) {
                    masterBingoSetting.on_ChangeAutoPreset(document.getElementById(id));
                }
                else if (id.indexOf(strListMax) >= 0) {
                    masterBingoSetting.on_ChangeMax(document.getElementById(id));
                }
                else if (id.indexOf(strListMin) >= 0) {
                    masterBingoSetting.on_ChangeMin(document.getElementById(id));
                }
            }
        }
    },
    on_ChangePreset: function (obj) {
        var oPreset = $(obj);
        var strPresetID = oPreset.attr("id");
        var PresetValue = oPreset.val();
        var PresetValueMax = oPreset.attr("max");

        jQuery.each($("." + strPresetID.replace(strListPreset, "Pre")), function () {
            if ($(this).prop("id").indexOf(strPresetID) >= 0) {
                $(this).val(PresetValue);
                masterBingoSetting.on_ChangePresetGameType(this);
            }
        });

        var oMax = $("#" + strPresetID.replace(strListPreset, strListMax));
        var strMaxID = oMax.attr("id");
        var MaxValue = oMax.val();

        var MaxValueMax = roundNumber(PresetValueMax - PresetValue, 2);

        //if (MaxValue>MaxValueMax)
        //{
        MaxValue = MaxValueMax;
        //}
        oMax.attr("max", MaxValueMax);
        CreateOption(oMax, MaxValue, MaxValueMax, 0, 0.01, 2);
        masterBingoSetting.on_ChangeMax(document.getElementById(strMaxID));
    },
    on_ChangePresetGameType: function (obj) {
        var oPreset = $(obj);
        var strPresetID = oPreset.attr("id");
        var PresetValue = oPreset.val();
        var PresetValueMax = oPreset.attr("max");
        $("#" + strPresetID.replace(strListPreset, strerrPT)).css("visibility", "hidden");
        $("#" + strPresetID.replace(strListPreset, strerrPTMin)).css("visibility", "hidden");
        var oMax = $("#" + strPresetID.replace(strListPreset, strListMax));
        var MaxValue = oMax.val();

        var MaxValueMax = roundNumber(PresetValueMax - PresetValue, 2);

        if (MaxValue > MaxValueMax) {
            MaxValue = MaxValueMax;
        }
        oMax.attr("max", MaxValueMax);
        CreateOption(oMax, MaxValue, MaxValueMax, 0, 0.01, 2);
    },
    checkAutoPresetPT: function (obj) {
        var chkAuto = $(obj);
        var chkAutoID = chkAuto.prop("id");
        if (chkAuto.prop("checked")) {
            $("#" + chkAutoID.replace(strChkAutoPreset, strListPreset)).attr("disabled", "disabled");
            $("#" + chkAutoID.replace(strChkAutoPreset, strListAutoPreset)).removeAttr("disabled");

            $(".Pre" + chkAutoID.replace(strChkAutoPreset, "")).attr("disabled", "disabled");
            $(".AutoPre" + chkAutoID.replace(strChkAutoPreset, "")).removeAttr("disabled");

            //Change Max value of Max Min 
            var oMax = $("#" + chkAutoID.replace(strChkAutoPreset, strListMax));
            var oMin = $("#" + chkAutoID.replace(strChkAutoPreset, strListMin));
            var oAutoPreset = $("#" + chkAutoID.replace(strChkAutoPreset, strListAutoPreset));
            oMax.attr("max", oAutoPreset.attr("max"));
            oMin.attr("max", oMax.val());
            CreateOption(oMax, oMax.val(), oAutoPreset.attr("max"), 0, 0.01, 2);
            CreateOption(oMin, oMin.val(), oMax.val(), 0, 0.01, 2);

            jQuery.each($(".chkauto"), function () {
                if ($(this).prop("id").indexOf(chkAutoID) >= 0) {
                    $(this).attr("checked", "checked");
                    masterBingoSetting.checkAutoPresetPTGameType(this);
                }
            });
        }
        else {
            $("#" + chkAutoID.replace(strChkAutoPreset, strListAutoPreset)).attr("disabled", "disabled");
            $("#" + chkAutoID.replace(strChkAutoPreset, strListPreset)).removeAttr("disabled");

            $(".AutoPre" + chkAutoID.replace(strChkAutoPreset, "")).attr("disabled", "disabled");
            $(".Pre" + chkAutoID.replace(strChkAutoPreset, "")).removeAttr("disabled");

            jQuery.each($(".chkauto"), function () {
                if ($(this).prop("id").indexOf(chkAutoID) >= 0) {
                    $(this).removeAttr("checked");
                    masterBingoSetting.checkAutoPresetPTGameType(this);
                }
            });

            masterBingoSetting.on_ChangePreset(document.getElementById(chkAutoID.replace(strChkAutoPreset, strListPreset)));
        }
    },
    on_ChangeAutoPreset: function (obj) {
        var Auto = $(obj);
        var AutoID = Auto.attr("id");
        var AutoValue = Auto.val();
        var Autolists = $(".AutoPre" + AutoID.replace(strListAutoPreset, ""));
        Autolists.each(function () {
            if ($(this).prop("id").indexOf(AutoID + "_") >= 0) {
                $(this).val(AutoValue);
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
            if ($(this).attr("id").indexOf(CommId + "_") >= 0) {
                $(this).val(CommValue);
            }
        });
    },
    checkAutoPresetPTGameType: function (obj) {
        var chkAuto = $(obj);
        var chkAutoID = chkAuto.attr("id");
        if (chkAuto.attr("checked")) {
            $("#" + chkAutoID.replace(strChkAutoPreset, strListPreset)).attr("disabled", "disabled");
            $("#" + chkAutoID.replace(strChkAutoPreset, strListAutoPreset)).removeAttr("disabled");

            //Change Max value of Max Min
            var oMax = $("#" + chkAutoID.replace(strChkAutoPreset, strListMax));
            var oMin = $("#" + chkAutoID.replace(strChkAutoPreset, strListMin));
            var oAutoPreset = $("#" + chkAutoID.replace(strChkAutoPreset, strListAutoPreset));
            oMax.attr("max", oAutoPreset.attr("max"));
            oMin.attr("max", oMax.val());
            CreateOption(oMax, oMax.val(), oAutoPreset.attr("max"), 0, 0.01, 2);
            CreateOption(oMin, oMin.val(), oMax.val(), 0, 0.01, 2);
        }
        else {
            $("#" + chkAutoID.replace(strChkAutoPreset, strListAutoPreset)).attr("disabled", "disabled");
            $("#" + chkAutoID.replace(strChkAutoPreset, strListPreset)).removeAttr("disabled");
            //on_ChangePresetGameType(document.getElementById(chkAutoID.replace(strChkAutoPreset,strListPreset)));
        }
    }
};