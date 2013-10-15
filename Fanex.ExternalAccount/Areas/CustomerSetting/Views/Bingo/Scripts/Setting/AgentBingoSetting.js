var agentBingoSetting = {
    renderPositionTaking: function (positionTakingSetting) {
        var html = '<table class="tblcollapse" style="width: 100%; margin:6px 0px;" id="tblbingo">\
                        <tr class="title">\
                            <td colspan="6">Position Taking</td>\
                        </tr>';
        $.each(positionTakingSetting.gameKindList, function (index, value) {
            html += agentBingoSetting.renderGameKindBO(positionTakingSetting.isGroupSetting, value);
        });
        return html + '</table>';
    },
    renderGameKindBO: function (isGroupSetting, gameKindBO) {
        var id = gameKindBO.iD;
        var html = '<tr>\
                        <td colspan="2" class="c"></td>\
                        <td class="c"><b>Master Preset PT</b></td>\
                        <td class="c"><b>Agent Max PT</b></td>\
                        <td class="c"><b>Agent Min PT</b></td>\
                        <td class="c"><b>Agent Commission</b></td>\
                    </tr>\
                    <tr>\
                        <td rowspan="' + (2 * gameKindBO.gameTypeList.length + 1) + '"><b>' + gameKindBO.name + '</b></td>\
                        <td class="l"><b>Group Setting</b></td>\
                        <td>\
                            <select id="listPrePT_' + id + '" onchange="agentBingoSetting.on_ChangePreset(this);" max="' + roundNumber(gameKindBO.positionTakingUplineMax, 2) + '" min="0" class="select-pt" ' + (gameKindBO.isAutoPositionTaking ? 'Disabled' : '') + '>\
                                <option value="' + roundNumber(gameKindBO.positionTakingUpline, 2) + '" selected="true">' + roundNumber(gameKindBO.positionTakingUpline, 2) + '</option>\
                            </select>\
                            <input type="button" onclick="agentBingoSetting.ReducePT(\'listPrePT_' + id + '\',1)" class="btn-dec1 Pre_' + id + '" value=" "' + (gameKindBO.isAutoPositionTaking ? 'Disabled' : '') + '>\
                            <input type="button" onclick="agentBingoSetting.IncreasePT(\'listPrePT_' + id + '\',1)" class="btn-inc1 Pre_' + id + '" value=" "' + (gameKindBO.isAutoPositionTaking ? 'Disabled' : '') + '>\
                            <input type="button" onclick="agentBingoSetting.ReducePT(\'listPrePT_' + id + '\',10)" class="btn-dec10 Pre_' + id + '" value=" "' + (gameKindBO.isAutoPositionTaking ? 'Disabled' : '') + '>\
                            <input type="button" onclick="agentBingoSetting.IncreasePT(\'listPrePT_' + id + '\',10)" class="btn-inc10 Pre_' + id + '" value=" "' + (gameKindBO.isAutoPositionTaking ? 'Disabled' : '') + '>\
                        </td>\
                        <td>\
                            <select id="listMaxPT_' + id + '" onchange="agentBingoSetting.on_ChangeMax(this);" max="' + roundNumber(gameKindBO.positionTakingMax, 2) + '" min="0" class="select-pt">\
                                <option value="' + roundNumber(gameKindBO.positionTaking, 2) + '" selected="true">' + roundNumber(gameKindBO.positionTaking, 2) + '</option>\
                            </select>\
                            <input type="button" onclick="agentBingoSetting.ReducePT(\'listMaxPT_' + id + '\',1)" class="btn-dec1" value=" ">\
                            <input type="button" onclick="agentBingoSetting.IncreasePT(\'listMaxPT_' + id + '\',1)" class="btn-inc1" value=" ">\
                            <input type="button" onclick="agentBingoSetting.ReducePT(\'listMaxPT_' + id + '\',10)" class="btn-dec10" value=" ">\
                            <input type="button" onclick="agentBingoSetting.IncreasePT(\'listMaxPT_' + id + '\',10)" class="btn-inc10" value=" ">\
                        </td>\
                        <td>\
                            <select id="listMinPT_' + id + '" onchange="agentBingoSetting.on_ChangeMin(this);" max="' + roundNumber(gameKindBO.positionTaking, 2) + '" min="0" class="select-pt">\
                                <option value="' + roundNumber(gameKindBO.minPositionTaking, 2) + '" selected="true">' + roundNumber(gameKindBO.minPositionTaking, 2) + '</option>\
                            </select>\
                            <input type="button" onclick="agentBingoSetting.ReducePT(\'listMinPT_' + id + '\',1)" class="btn-dec1" value=" ">\
                            <input type="button" onclick="agentBingoSetting.IncreasePT(\'listMinPT_' + id + '\',1)" class="btn-inc1" value=" ">\
                            <input type="button" onclick="agentBingoSetting.ReducePT(\'listMinPT_' + id + '\',10)" class="btn-dec10" value=" ">\
                            <input type="button" onclick="agentBingoSetting.IncreasePT(\'listMinPT_' + id + '\',10)" class="btn-inc10" value=" ">\
                        </td>\
                        <td>\
                            <select id="listComm_' + id + '" onchange="agentBingoSetting.on_ChangeComm(this);" max="' + roundNumber(gameKindBO.commissionUpline, 4) + '" min="0" class="select-comm">\
                                <option value="' + roundNumber(gameKindBO.commission, 4) + '" selected="true">' + roundNumber(gameKindBO.commission, 4) + '</option>\
                            </select>\
                        </td>\
                    </tr>';
        $.each(gameKindBO.gameTypeList, function (index, value) {
            html += agentBingoSetting.renderGameTypeBO(isGroupSetting, id, value);
        });
        return html;
    },
    renderGameTypeBO: function (isGroupSetting, gameKindBOId, gameTypeBO) {
        var id = gameKindBOId + '_' + gameTypeBO.iD;
        var html = '<tr>\
                    <td class="l" rowspan="2">' + gameTypeBO.name + '</td>';
        if (!isGroupSetting) {
            html += '<td class="gametypePT c">\
                        <select id="listPrePT_' + id + '" onchange="agentBingoSetting.on_ChangePresetGameType(this)" max="' + roundNumber(gameTypeBO.positionTakingUplineMax, 2) + '" min="0" class="select-pt Pre_' + id + '"' + (gameTypeBO.isAutoPositionTaking ? 'Disabled' : '') + '>\
                            <option value="' + roundNumber(gameTypeBO.positionTakingUpline, 2) + '" selected="true">' + roundNumber(gameTypeBO.positionTakingUpline, 2) + '</option>\
                        </select>\
                    </td>\
                    <td class="gametypePT c">\
                        <select id="listMaxPT_' + id + '" onchange="agentBingoSetting.on_ChangeMaxGameType(this);" max="' + roundNumber(gameTypeBO.positionTakingMax, 2) + '" min="0" class="select-pt Max_' + id + '">\
                            <option value="' + roundNumber(gameTypeBO.positionTaking, 2) + '" selected="true">' + roundNumber(gameTypeBO.positionTaking, 2) + '</option>\
                        </select><span class="errPT" id="errPT_' + id + '" style="color:#FF0000;visibility:hidden;"> * </span>\
                    </td>\
                    <td class="gametypePT c">\
                        <select id="listMinPT_' + id + '" max="' + roundNumber(gameTypeBO.positionTakingMax, 2) + '" min="0" class="select-pt Min">\
                            <option value="' + roundNumber(gameTypeBO.minPositionTaking, 2) + '" selected="true">' + roundNumber(gameTypeBO.minPositionTaking, 2) + '</option>\
                        </select><span class="errPT" id="errPTMin_' + id + '" style="color:#FF0000;visibility:hidden;"> * </span>\
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
        var maxTransferCredit = $("#txtMaxTransferCredit").val()
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
            customer.gameKindList[i].positionTakingUpline = roundNumber($("#" + strListPreset + "_" + gamekindid).val(), 2);
            customer.gameKindList[i].positionTaking = roundNumber($("#" + strListMax + "_" + gamekindid).val(), 2);
            customer.gameKindList[i].minPositionTaking = roundNumber($("#" + strListMin + "_" + gamekindid).val(), 2);
            customer.gameKindList[i].commission = roundNumber($("#" + strListComm + "_" + gamekindid).val(), 4);
            //Loop to get PT setting GameType
            for (var j = 0; j < totalGameType; j++) {
                var gametype = customer.gameKindList[i].gameTypeList[j];
                var gametypeid = gametype.iD;
                totalBettingRange = gametype.bettingRangeList.length;
                if (!customer.isUsedGameKind) {
                    customer.gameKindList[i].gameTypeList[j].positionTakingUpline = roundNumber($("#" + strListPreset + "_" + gamekindid + "_" + gametypeid).val(), 2);
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
                    agentBingoSetting.on_ChangeMaxGameType(this);
                    $("#" + $(this).attr("id").replace(strListMax, strerrPT)).css("visibility", "hidden");
                }
                else {
                    $("#" + $(this).attr("id").replace(strListMax, strerrPT)).css("visibility", "visible");
                }
            }
        });
    
        var oMin = $("#" + strMaxID.replace(strListMax,strListMin));
        var strMinID = oMin.attr("id");
        var MinValue = oMin.val();
        var MinValueMax = MaxValue;

        if (MinValue > MinValueMax) {
            MinValue = MinValueMax;
        }
        oMin.attr("max",MinValueMax);
        CreateOption(oMin, MinValue, MinValueMax, 0, 0.01, 2);
    },
    // Min PT onChange
    on_ChangeMin: function (obj) {
        var Min = $(obj);
        var MinID = Min.attr("id");
        var MinValue = Min.val();
        var Minlists = $(".Min");
        Minlists.each(function() {
            if ($(this).prop("id").indexOf(MinID + "_") >= 0) {
                if (MinValue <= $(this).attr("max")) {
                    $(this).val(MinValue);
                    $("#" + $(this).attr("id").replace(strListMin,strerrPTMin)).css("visibility","hidden");
                }
                else {
                    $("#" + $(this).attr("id").replace(strListMin,strerrPTMin)).css("visibility","visible");
                }
            }
        });
    },
    on_ChangeMaxGameType: function (obj) {
        var oMax = $(obj);
        var strMaxID = oMax.attr("id");
        var MaxValue = oMax.val();
        var MaxValueMax = oMax.attr("max");
        $("#" + strMaxID.replace(strListMax, strerrPT)).css("visibility","hidden");
        $("#" + strMaxID.replace(strListMax, strerrPTMin)).css("visibility","hidden");
        var oMin=$("#" + strMaxID.replace(strListMax, strListMin));
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
                    agentBingoSetting.on_ChangePreset(document.getElementById(id));
                }
                else if (id.indexOf(strListMax) >= 0) {
                    agentBingoSetting.on_ChangeMax(document.getElementById(id));
                }
                else if (id.indexOf(strListMin) >= 0) {
                    agentBingoSetting.on_ChangeMin(document.getElementById(id));
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
                    agentBingoSetting.on_ChangePreset(document.getElementById(id));
                }
                else if (id.indexOf(strListMax) >= 0) {
                    agentBingoSetting.on_ChangeMax(document.getElementById(id));
                }
                else if (id.indexOf(strListMin) >= 0) {
                    agentBingoSetting.on_ChangeMin(document.getElementById(id));
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
                agentBingoSetting.on_ChangePresetGameType(this);
            }
        });
    
        var oMax = $("#" + strPresetID.replace(strListPreset,strListMax));
        var strMaxID = oMax.attr("id");
        var MaxValue = oMax.val();
    
        var MaxValueMax = roundNumber(PresetValueMax-PresetValue,2);
        MaxValue = MaxValueMax;
    
        oMax.attr("max",MaxValueMax);
        CreateOption(oMax, MaxValue, MaxValueMax, 0, 0.01, 2);
        agentBingoSetting.on_ChangeMax(document.getElementById(strMaxID));
    },
    on_ChangePresetGameType: function (obj) {
        var oPreset = $(obj);
        var strPresetID = oPreset.attr("id");
        var PresetValue = oPreset.val();
        var PresetValueMax = oPreset.attr("max");
        $("#" + strPresetID.replace(strListPreset,strerrPT)).css("visibility","hidden");
        $("#" + strPresetID.replace(strListPreset,strerrPTMin)).css("visibility","hidden");
        var oMax = $("#" + strPresetID.replace(strListPreset,strListMax));
        var MaxValue = oMax.val();
    
        var MaxValueMax = roundNumber(PresetValueMax-PresetValue,2);
    
        //if (MaxValue>MaxValueMax)
        //{
        MaxValue=MaxValueMax;
        //}
        oMax.attr("max",MaxValueMax);
        CreateOption(oMax, MaxValue, MaxValueMax, 0, 0.01, 2);
    },
    // Comm onChange
    on_ChangeComm: function (obj) {
        var Commlist = $(obj);
        var CommId = Commlist.attr("id");
        var CommValue = Commlist.val();
        var Commlists = $(".select-comm");
        Commlists.each(function() {
           if ($(this).prop("id").indexOf(CommId + "_") >= 0) {
               $(this).val(CommValue);
           }
        });
    }
};