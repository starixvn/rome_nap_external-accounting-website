var superBingoSetting = {
    renderPositionTaking: function (positionTakingSetting) {
        var html = '<table class="tblcollapse" style="width: 100%; margin:6px 0px;" id="tblbingo">\
                        <tr class="title">\
                            <td colspan="5">Position Taking</td>\
                        </tr>';
        $.each(positionTakingSetting.gameKindList, function (index, value) {
            html += superBingoSetting.renderGameKindBO(positionTakingSetting.isGroupSetting, index, value);
        });
        return html + '</table>';
    },
    renderGameKindBO: function (isGroupSetting, index, gameKindBO) {
        var id = gameKindBO.iD;
        var html = '<tr>\
                        <td colspan="2" class="c"></td>\
                        <td class="c"><b>Super Max Position</b></td>\
                        <td class="c"><b>Super Min PT</b></td>\
                        <td class="c"><b>Super Commission</b></td>\
                    </tr>\
                    <tr>\
                        <td rowspan="' + (2 * gameKindBO.gameTypeList.length + 1) + '"><b>' + gameKindBO.name + '</b></td>\
                        <td class="l">';
        if (index === 0) {
            html += '<input type="checkbox" id="chkGroupSetting" onclick="superBingoSetting.showhideSetting(this);" ' + (isGroupSetting ? 'checked' : '') + ' id="chkGroupSetting">';
        }
        html += '<label for="chkGroupSetting"><b>Group Setting</b></label>\
                        </td>\
                        <td>\
                            <select id="listMaxPT_' + id + '" onchange="superBingoSetting.on_ChangeMax(this);" max="' + roundNumber(gameKindBO.positionTakingMax, 2) + '" min="0" class="select-pt">\
                                <option value="' + roundNumber(gameKindBO.positionTaking, 2) + '" selected="true">' + roundNumber(gameKindBO.positionTaking, 2) + '</option>\
                            </select>\
                            <input type="button" onclick="superBingoSetting.ReducePT(\'listMaxPT_' + id + '\',1)" class="btn-dec1" value=" ">\
                            <input type="button" onclick="superBingoSetting.IncreasePT(\'listMaxPT_' + id + '\',1)" class="btn-inc1" value=" ">\
                            <input type="button" onclick="superBingoSetting.ReducePT(\'listMaxPT_' + id + '\',10)" class="btn-dec10" value=" ">\
                            <input type="button" onclick="superBingoSetting.IncreasePT(\'listMaxPT_' + id + '\',10)" class="btn-inc10" value=" ">\
                        </td>\
                        <td>\
                            <select id="listMinPT_' + id + '" onchange="On_ChangeMin(this);" max="' + roundNumber(gameKindBO.positionTaking, 2) + '" min="0" class="select-pt">\
                                <option value="' + roundNumber(gameKindBO.minPositionTaking, 2) + '" selected="true">' + roundNumber(gameKindBO.minPositionTaking, 2) + '</option>\
                            </select>\
                            <input type="button" onclick="superBingoSetting.ReducePT(\'listMinPT_' + id + '\',1)" class="btn-dec1" value=" ">\
                            <input type="button" onclick="superBingoSetting.IncreasePT(\'listMinPT_' + id + '\',1)" class="btn-inc1" value=" ">\
                            <input type="button" onclick="superBingoSetting.ReducePT(\'listMinPT_' + id + '\',10)" class="btn-dec10" value=" ">\
                            <input type="button" onclick="superBingoSetting.IncreasePT(\'listMinPT_' + id + '\',10)" class="btn-inc10" value=" ">\
                        </td>\
                        <td>\
                            <select id="listComm_' + id + '" onchange="superBingoSetting.on_ChangeComm(this);" max="' + roundNumber(gameKindBO.commissionUpline, 4) + '" min="0" class="select-comm">\
                                <option value="' + roundNumber(gameKindBO.commission, 4) + '" selected="true">' + roundNumber(gameKindBO.commission, 4) + '</option>\
                            </select>\
                        </td>\
                    </tr>';
        $.each(gameKindBO.gameTypeList, function (index, value) {
            html += superBingoSetting.renderGameTypeBO(isGroupSetting, id, value);
        });
        return html;
    },
    renderGameTypeBO: function (isGroupSetting, gameKindBOId, gameTypeBO) {
        var id = gameKindBOId + '_' + gameTypeBO.iD;
        var html = '<tr>\
                        <td class="l" rowspan="2">' + gameTypeBO.name + '</td>\
                        <td class="gametypePT c ' + (isGroupSetting ? 'display-none' : '') + '">\
                            <select id="listMaxPT_' + id + '" onchange="superBingoSetting.on_ChangeMaxGameType(this);" max="0.9" min="0" class="select-pt">\
                                <option value="' + roundNumber(gameTypeBO.positionTaking, 2) + '" selected="true">' + roundNumber(gameTypeBO.positionTaking, 2) + '</option>\
                            </select>\
                        </td>\
                        <td class="gametypePT c ' + (isGroupSetting ? 'display-none' : '') + '">\
                            <select id="listMinPT_' + id + '" max="0.9" min="0" class="select-pt">\
                                <option value="' + roundNumber(gameTypeBO.minPositionTaking, 2) + '" selected="true">' + roundNumber(gameTypeBO.minPositionTaking, 2) + '</option>\
                            </select><span class="errMinPT" id="errPT_' + id + '" style="color:#FF0000;visibility:hidden;"> * </span>\
                        </td>\
                        <td class="gametypePT c ' + (isGroupSetting ? 'display-none' : '') + '" style="border:1px solid #CCCCCC !important">\
                            <select id="listComm_' + id + '" max="' + roundNumber(gameTypeBO.commissionUpline, 4) + '" min="0" class="select-comm">\
                                <option value="' + roundNumber(gameTypeBO.commission, 4) + '" selected="true">' + roundNumber(gameTypeBO.commission, 4) + '</option>\
                            </select>\
                        </td>\
                    </tr>\
                    <tr>\
                        <td class="l" colspan="3">Betting Limit:';
        $.each(gameTypeBO.bettingRangeList, function (index, value) {
            var generalId = id + '_' + value.iD;
            html += ptBingoCommon.renderBettingRange(gameKindBOId, generalId, value);
        });
        return html + '</td></tr>';
    },
    updatePT: function (customer) {
        var maxTransferCredit = parent.Fanex.ExtAcc.ReplaceAll($("#txtMaxTransferCredit").val(), ',', '');
        if (!uInteger(maxTransferCredit)) {
            Validators.show($('#txtMaxTransferCredit').get(0), 'Invalid Max Transfer Credit', undefined, undefined, 3000);
            return;
        }
        if (customer.minLimit > maxTransferCredit) {
            Validators.show($('#txtMaxTransferCredit').get(0), 'Invalid Max Transfer Credit', undefined, undefined, 3000);
            return;
        }
        PTBlockUI();
        var totalGameKind = customer.gameKindList.length;
        var totalGameType;
        var totalBettingRange;
        var gamekindid, gametypeid, bettingrangeid;

        //Loop to get PT setting GameKind
        customer.maxTransferCredit = maxTransferCredit;
        customer.isAutoReset = $("#cbxAutoReset").prop("checked");
        customer.isUsedGameKind = $("#chkGroupSetting").prop("checked");
        for (var i = 0; i < totalGameKind; i++) {
            var gamekind = customer.gameKindList[i];
            gamekindid = gamekind.iD;
            totalGameType = gamekind.gameTypeList.length;
            customer.gameKindList[i].positionTaking = roundNumber($("#" + strListMax + "_" + gamekindid).val(), 2);
            customer.gameKindList[i].minPositionTaking = roundNumber($("#" + strListMin + "_" + gamekindid).val(), 2);
            customer.gameKindList[i].commission = roundNumber($("#" + strListComm + "_" + gamekindid).val(), 4);
            //Loop to get PT setting GameType
            for (var j = 0; j < totalGameType; j++) {
                var gametype = customer.gameKindList[i].gameTypeList[j];
                gametypeid = gametype.iD;
                totalBettingRange = gametype.bettingRangeList.length;
                if (!customer.isUsedGameKind) {
                    customer.gameKindList[i].gameTypeList[j].positionTaking = roundNumber($("#" + strListMax + "_" + gamekindid + "_" + gametypeid).val(), 2);
                    customer.gameKindList[i].gameTypeList[j].minPositionTaking = roundNumber($("#" + strListMin + "_" + gamekindid + "_" + gametypeid).val(), 2);
                    customer.gameKindList[i].gameTypeList[j].commission = roundNumber($("#" + strListComm + "_" + gamekindid + "_" + gametypeid).val(), 4);
                }
                //Loop to get BettingRangeList
                for (var z = 0; z < totalBettingRange; z++) {
                    var bettingrange = customer.gameKindList[i].gameTypeList[j].bettingRangeList[z];
                    bettingrangeid = bettingrange.iD;
                    customer.gameKindList[i].gameTypeList[j].bettingRangeList[z].isChecked = $("#" + strChkStake + "_" + gamekindid + "_" + gametypeid + "_" + bettingrangeid).prop("checked");
                }
            }
        }
        customerPTBingoSetting.ajaxUpdatePT(customer);
    },
    showhideSetting: function (obj) {
        if (obj.checked) {
            //$('.gametypePT').css('display', 'none');
            $('.gametypePT').addClass('display-none');
        }
        else {
            //$('.gametypePT').css('display', '');
            $('.gametypePT').removeClass('display-none');
        }
    },
    // Max PT onChange
    on_ChangeMax: function (obj) {
        var Maxlist = $(obj);
        var MaxId = Maxlist.attr('id');
        var Minlist = $('#' + MaxId.replace(strListMax, strListMin));

        //Change  Min PT
        var Minvalue = Minlist.val();
        var Max_Minvalue = Maxlist.val();
        var Min_Minvalue = Minlist.attr('min');

        if (Max_Minvalue < Minvalue) Minvalue = 0;

        Minlist.attr('max', Max_Minvalue);
        CreateOption(Minlist, Minvalue, Max_Minvalue, Min_Minvalue, 0.01, 2);

        //Change Game Type  Max PT 
        var PTlists = $('.select-pt');
        PTlists.each(function () {
            if ($(this).attr('id').indexOf(MaxId + '_') >= 0) {
                $(this).val(Maxlist.val());
                superBingoSetting.on_ChangeMaxGameType(document.getElementById($(this).attr('id')));
            }
        });
    },
    // Min PT onChange
    on_ChangeMin: function (obj) {
        var Minlist = $(obj);
        var MinId = Minlist.attr("id");
        var MinValue = Minlist.val();
        var PTlists = $(".select-pt");
        PTlists.each(function () {
            if ($(this).attr("id").indexOf(MinId + "_") >= 0) {
                if ($(this).attr("max") >= MinValue) {
                    $(this).val(MinValue);
                    $("#" + $(this).attr("id").replace(strListMin, strerrPT)).css("visibility", "hidden");
                }
                else {
                    $("#" + $(this).attr("id").replace(strListMin, strerrPT)).css("visibility", "visible");
                }

            }
        });
    },
    on_ChangeMaxGameType: function (obj) {
        var Maxlist = $(obj);
        var MaxId = Maxlist.attr('id');
        $('#' + MaxId.replace(strListMax, strerrPT)).css('visibility', 'hidden');
        var Minlist = $('#' + MaxId.replace(strListMax, strListMin));

        //Change  Min PT
        var Minvalue = Minlist.val();
        var Max_Minvalue = Maxlist.val();
        var Min_Minvalue = Minlist.attr('min');

        if (Max_Minvalue < Minvalue) Minvalue = 0;

        Minlist.attr('max', Max_Minvalue);
        CreateOption(Minlist, Minvalue, Max_Minvalue, Min_Minvalue, 0.01, 2);
    },
    ReducePT: function (id, step) {
        var obj = $("#" + id);
        var curr = obj.val();
        var min = obj.attr("min");

        if (obj.val() > min) {
            var newvalue = roundNumber((curr * 100 - step) / 100, 2);
            var max = obj.attr("max");
            if (newvalue < min) newvalue = curr;

            if ($("#" + id + " option").size() <= 1) {
                CreateOption(obj, newvalue, max, min, 0.01, 2);
            }
            else {
                $("#" + id + " option[value='" + newvalue + "']").attr("selected", "selected");
            }

            if (id.indexOf(strListMax) >= 0) {
                superBingoSetting.on_ChangeMax(document.getElementById(id));
            }
            else {
                superBingoSetting.on_ChangeMin(document.getElementById(id));
            }
        }
    },
    IncreasePT: function (id, step) {
        var obj = $("#" + id);
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
                $("#" + id + " option[value='" + newvalue + "']").attr("selected", "selected");
            }
            if (id.indexOf(strListMax) >= 0) {
                superBingoSetting.on_ChangeMax(document.getElementById(id));
            }
            else if (id.indexOf(strListMin) >= 0) {
                superBingoSetting.on_ChangeMin(document.getElementById(id));
            }
        }
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
    }
};