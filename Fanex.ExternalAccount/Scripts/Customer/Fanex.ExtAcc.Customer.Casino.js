/// <reference path="../_references.js" />

if (typeof Fanex == 'undefined' || typeof Fanex.ExtAcc == 'undefined') {
    alert("Fanex.ExtAcc.js module is required!");
};

Fanex.ExtAcc.Customer.Casino = {
    customer: null,
    colspan: 1,
    uplineId: Fanex.ExtAcc.Customer.CustomerInfo().uplineId,
    IsMultiple: Fanex.ExtAcc.Customer.IsMultiple,
    ErrorMessage: '',

    Init: function () {
        this.LoadData();
    },

    LoadData: function () {
        Fanex.ExtAcc.BlockUI();

        if (this.IsMultiple) {
            $('#custInfo').remove();
            Fanex.ExtAcc.Customer.ProcessDisplayMultiple();
        }
        else {
            $('#arrowUpDown').remove();
            $('#statusInfo').remove();
        }

        switch (Fanex.ExtAcc.Customer.CurrentLevel) {
            case Fanex.ExtAcc.Customer.Master:
                $('#downline').html('Master');
                $('#member').remove();
                $('#agent').remove();
                $('#sagent').remove();
                $('#smember').remove();
                $('#mmember').remove();
                this.colspan = 5;
                break;
            case Fanex.ExtAcc.Customer.Agent:
                $('#downline').html('Agent');
                $('#member').remove();

                if (!Fanex.ExtAcc.Customer.isSuperEditAgent) {
                    $('#sagent').remove();
                }
                else {
                    $('#agent').remove();
                }

                $('#master').remove();
                $('#smember').remove();
                $('#mmember').remove();
                this.colspan = 4;
                break;
            case Fanex.ExtAcc.Customer.Member:
                if (Fanex.ExtAcc.Customer.isSuperEditMember) {
                    $('#mmember').remove();
                    $('#member').remove();

                    this.colspan = 4;
                }
                else if (Fanex.ExtAcc.Customer.isMasterEditMember) {
                    $('#smember').remove();
                    $('#member').remove();

                    this.colspan = 2;
                }
                else {
                    $('#smember').remove();
                    $('#mmember').remove();

                    this.colspan = 2;
                }

                $('#downline').html('Member');
                $('#master').remove();
                $('#agent').remove();
                $('#sagent').remove();

                break;
        }

        $.ajax({
            url: resolveClientUrl('Casino/EditCasinoPost'),
            type: 'post',
            data: { custId: Fanex.ExtAcc.Customer.UserId, uplineId: Fanex.ExtAcc.Customer.Casino.uplineId },
            dataType: 'json'
        })
        .success(function (result) {
            console.log(result);
            Fanex.ExtAcc.Customer.Casino.customer = result;

            Fanex.ExtAcc.Customer.Casino.RenderSetting(result);

            Fanex.ExtAcc.Customer.Casino.RenderPT(result.pts);

            Fanex.ExtAcc.Customer.Casino.CasinoModel = function () {
                this.saveCustomer = function () {
                    $('#windowPopup').scrollTop(0);

                    if (Fanex.ExtAcc.Customer.Casino.IsMultiple) {
                        Fanex.ExtAcc.Customer.Casino.UpdateSetting(0);
                    }
                    else {
                        Fanex.ExtAcc.Customer.Casino.UpdateSetting(null);
                    }
                }
            }

            ko.applyBindings(new Fanex.ExtAcc.Customer.Casino.CasinoModel());

            Fanex.ExtAcc.UnBlockUI();
        })
        .error(function (data, status, err) {
            if (typeof console != 'undefined') {
                console.log(data.responseText);
            }

            $('#windowPopup').html('<font color="Red">Not casino\'s customer</font>');
            Fanex.ExtAcc.UnBlockUI();
        });
    },

    RenderSetting: function (result) {
        // Renders casino game setting
        if (result && result.casinoGame.length > 0) {
            for (var i = 0; i < result.casinoGame.length; i++) {
                var gameName = result.casinoGame[i].gameTypeName;
                var stakeLevels = '';

                for (var j = 0; j < result.casinoGame[i].stakeLevels.length; j++) {
                    var checked = '';

                    if (result.casinoGame[i].stakeLevels[j].isChecked) {
                        checked = 'checked="checked"';
                    }

                    stakeLevels += '<input ' + checked + ' type="checkbox" id="' + result.casinoGame[i].stakeLevels[j].stakeLevelID + '" />' + result.casinoGame[i].stakeLevels[j].stakeLevelName;
                }

                var row = '<tr>' +
                                    '<td class="R">' + gameName + '</td>' +
                                    '<td colspan="' + Fanex.ExtAcc.Customer.Casino.colspan + '" style="border-right: none">Betting limit:<span id="bettingLimit">' + stakeLevels + '</span></td>' +
                                '</tr>';

                $('#casinoGame').append(row);
            }

            $('#rowspanCasino').attr('rowspan', result.casinoGame.length + 1);
            $('#rowspanCasino').html(result.pts[0].codeName);
        }

        // Renders mini game setting
        if (result && result.miniGame.length > 0) {
            for (var i = 0; i < result.miniGame.length; i++) {
                var gameName = result.miniGame[i].gameTypeName;
                var stakeLevels = '';

                for (var j = 0; j < result.miniGame[i].stakeLevels.length; j++) {
                    var checked = '';

                    if (result.miniGame[i].stakeLevels[j].isChecked) {
                        checked = 'checked="checked"';
                    }

                    stakeLevels += '<input name="gameRow' + i + '" ' + checked + ' type="checkbox" id="' + result.miniGame[i].stakeLevels[j].stakeLevelID + '" />' + result.miniGame[i].stakeLevels[j].stakeLevelName;
                }

                var row = '<tr>' +
                                    '<td class="R">' + gameName + '</td>' +
                                    '<td colspan="' + Fanex.ExtAcc.Customer.Casino.colspan + '" style="border-right: none">Betting limit:<span id="bettingLimit">' + stakeLevels + '</span></td>' +
                                '</tr>';

                $('#miniGame').append(row);
            }

            $('#rowspanMini').attr('rowspan', result.miniGame.length + 1);
            $('#rowspanMini').html(result.pts[1].codeName);
        }

        if (Fanex.ExtAcc.Customer.isSuperEditAgent) {
            var checkboxes = $('input[type=checkbox]');

            for (var i = 0; i < checkboxes.length; i++) {
                checkboxes[i].disabled = true;
            }
        }
    },

    RenderPT: function (pts) {
        var casinoGamePT = pts[0];
        var miniGamePT = pts[1];

        // Renders Master-Agent PT
        if (Fanex.ExtAcc.Customer.CurrentLevel == Fanex.ExtAcc.Customer.Master) {
            //PT Renderring for Casino Game
            //PT values
            var uplineMin = this.customer.customerInfo.casinoSync ? casinoGamePT.minPositionTakingUpline : casinoGamePT.minPositionTaking;
            var uplineMax = this.customer.customerInfo.casinoSync ? casinoGamePT.maxPositionTakingUpline : casinoGamePT.maxPositionTaking;
            var uplinePreset = this.customer.customerInfo.casinoSync ? casinoGamePT.sPositionTaking : casinoGamePT.sPositionTakingUpline;
            var uplineAutoPreset = this.customer.customerInfo.casinoSync ? casinoGamePT.autoPositionTaking : casinoGamePT.autoPositionTakingUpline;
            var downlinePreset = this.customer.customerInfo.casinoSync ? casinoGamePT.maxPositionTaking : casinoGamePT.maxPositionTakingUpline;
            var downlinePreset2 = this.customer.customerInfo.casinoSync ? casinoGamePT.minPositionTaking : casinoGamePT.minPositionTakingUpline;

            //Render super preset pt
            PT_RenderCombo(document.getElementById('pt_1'), uplineMin, uplineMax, 0.01, uplinePreset);

            //Render super auto preset pt
            PT_RenderCombo(document.getElementById('pt_2'), uplineMin, uplineMax, 0.01, uplineAutoPreset);
            if (!casinoGamePT.isAutoPositionTaking) {
                $('#pt_2').attr('disabled', 'disabled');
            }
            else {
                $('#pt_1').attr('disabled', 'disabled');
            }
            $('#chkAutoPT').attr('checked', casinoGamePT.isAutoPositionTaking);

            //Render Master Max PT
            PT_RenderCombo(document.getElementById('pt_3'), 0, (uplineMax - uplinePreset), 0.01, downlinePreset);

            //Render Master Min PT
            PT_RenderCombo(document.getElementById('pt_4'), 0, downlinePreset, 0.01, downlinePreset2);

            //Render Master Commission
            Comm_RenderCombo(document.getElementById('comm_5'), 0, this.customer.customerInfo.casinoSync ? casinoGamePT.sMAgentRollingRateUpline : casinoGamePT.sMAgentRollingRate, 0.0005, this.customer.customerInfo.casinoSync ? casinoGamePT.mAgentRollingRate : casinoGamePT.sMAgentRollingRate);

            //PT Renderring for Mini Game
            //PT values
            uplineMin = this.customer.customerInfo.casinoSync ? miniGamePT.minPositionTakingUpline : miniGamePT.minPositionTaking;
            uplineMax = this.customer.customerInfo.casinoSync ? miniGamePT.maxPositionTakingUpline : miniGamePT.maxPositionTaking;
            uplinePreset = this.customer.customerInfo.casinoSync ? miniGamePT.sPositionTaking : miniGamePT.sPositionTakingUpline;
            uplineAutoPreset = this.customer.customerInfo.casinoSync ? miniGamePT.autoPositionTaking : miniGamePT.autoPositionTakingUpline;
            downlinePreset = this.customer.customerInfo.casinoSync ? miniGamePT.maxPositionTaking : miniGamePT.maxPositionTakingUpline;
            downlinePreset2 = this.customer.customerInfo.casinoSync ? miniGamePT.minPositionTaking : miniGamePT.minPositionTakingUpline;

            //Render super preset pt
            PT_RenderCombo(document.getElementById('pt_1_1'), uplineMin, uplineMax, 0.01, uplinePreset);

            //Render super auto preset pt
            PT_RenderCombo(document.getElementById('pt_2_1'), uplineMin, uplineMax, 0.01, uplineAutoPreset);
            if (!miniGamePT.isAutoPositionTaking) {
                $('#pt_2_1').attr('disabled', 'disabled');
            }
            else {
                $('#pt_1_1').attr('disabled', 'disabled');
            }
            $('#chkAutoPT_1').attr('checked', miniGamePT.isAutoPositionTaking);

            //Render Master Max PT
            PT_RenderCombo(document.getElementById('pt_3_1'), 0, (uplineMax - uplinePreset), 0.01, downlinePreset);

            //Render Master Min PT
            PT_RenderCombo(document.getElementById('pt_4_1'), 0, downlinePreset, 0.01, downlinePreset2);

            //Render Master Commission
            Comm_RenderCombo(document.getElementById('comm_5_1'), 0, this.customer.customerInfo.casinoSync ? miniGamePT.sMAgentRollingRateUpline : miniGamePT.sMAgentRollingRate, 0.0005, this.customer.customerInfo.casinoSync ? miniGamePT.mAgentRollingRate : miniGamePT.sMAgentRollingRate);
        }
        else if (Fanex.ExtAcc.Customer.CurrentLevel == Fanex.ExtAcc.Customer.Agent) {
            //PT Renderring for Casino Game
            //PT values
            var uplineMin = this.customer.customerInfo.casinoSync ? casinoGamePT.minPositionTakingUpline : casinoGamePT.minPositionTaking;
            var uplineMax = this.customer.customerInfo.casinoSync ? casinoGamePT.maxPositionTakingUpline : casinoGamePT.maxPositionTaking;
            var uplinePreset = this.customer.customerInfo.casinoSync ? casinoGamePT.mPositionTaking : casinoGamePT.mPositionTakingUpline;
            var downlinePreset = this.customer.customerInfo.casinoSync ? casinoGamePT.aPositionTaking : casinoGamePT.aPositionTakingUpline;

            if (!Fanex.ExtAcc.Customer.isSuperEditAgent) {
                //Render Master preset pt
                PT_RenderCombo(document.getElementById('pt_1'), uplineMin, uplineMax, 0.01, uplinePreset);

                //Render Agent Max PT
                PT_RenderCombo(document.getElementById('pt_3'), 0, (uplineMax - uplinePreset), 0.01, downlinePreset);

                //Render Agent Min PT
                PT_RenderCombo(document.getElementById('pt_4'), 0, downlinePreset, 0.01, downlinePreset2);

                //Render Agent Commission
                Comm_RenderCombo(document.getElementById('comm_5'), 0, this.customer.customerInfo.casinoSync ? casinoGamePT.mAgentRollingRateUpline : casinoGamePT.mAgentRollingRate, 0.0005, this.customer.customerInfo.casinoSync ? casinoGamePT.agentRollingRate : casinoGamePT.mAgentRollingRate);
            }
            else {
                $('#chkEnable').attr('disabled', true);
                $('#maxTransfer').attr('disabled', true);
                $('#isAutoReset').attr('disabled', true);

                $('#mPT').html(casinoGamePT.mPositionTaking);
                $('#aPT').html(casinoGamePT.aPositionTaking);

                $('.buttonPopup').remove();
            }

            //PT Renderring for Mini Game
            //PT values
            uplineMin = this.customer.customerInfo.casinoSync ? miniGamePT.minPositionTakingUpline : miniGamePT.minPositionTaking;
            uplineMax = this.customer.customerInfo.casinoSync ? miniGamePT.maxPositionTakingUpline : miniGamePT.maxPositionTaking;
            uplinePreset = this.customer.customerInfo.casinoSync ? miniGamePT.mPositionTaking : miniGamePT.mPositionTakingUpline;
            downlinePreset = this.customer.customerInfo.casinoSync ? miniGamePT.aPositionTaking : miniGamePT.aPositionTakingUpline;

            if (!Fanex.ExtAcc.Customer.isSuperEditAgent) {
                //Render Master preset pt
                PT_RenderCombo(document.getElementById('pt_1_1'), uplineMin, uplineMax, 0.01, uplinePreset);

                //Render Agent Max PT
                PT_RenderCombo(document.getElementById('pt_3_1'), 0, (uplineMax - uplinePreset), 0.01, downlinePreset);

                //Render Agent Min PT
                PT_RenderCombo(document.getElementById('pt_4_1'), 0, downlinePreset, 0.01, downlinePreset2);

                //Render Agent Commission
                Comm_RenderCombo(document.getElementById('comm_5_1'), 0, this.customer.customerInfo.casinoSync ? miniGamePT.mAgentRollingRateUpline : miniGamePT.mAgentRollingRate, 0.0005, this.customer.customerInfo.casinoSync ? miniGamePT.agentRollingRate : miniGamePT.mAgentRollingRate);
            }
            else {
                $('#chkEnable').attr('disabled', true);
                $('#maxTransfer').attr('disabled', true);
                $('#isAutoReset').attr('disabled', true);

                $('#mPT_1').html(miniGamePT.mPositionTaking);
                $('#aPT_1').html(miniGamePT.aPositionTaking);
            }
        }
        else {
            // Renders member PT
            if (Fanex.ExtAcc.Customer.isSuperEditMember) {
                $('#chkEnable').attr('disabled', true);
                $('#maxTransfer').attr('disabled', true);
                $('#isAutoReset').attr('disabled', true);

                //PT Renderring for Casino Game
                //PT values
                var uplineMin = 0;
                var uplineMax = casinoGamePT.superSPositionTaking - casinoGamePT.mPositionTaking - casinoGamePT.aPositionTaking;
                var uplinePreset = casinoGamePT.sPositionTaking;
                var uplineAutoPreset = casinoGamePT.autoPositionTaking;
                var max = casinoGamePT.superSPositionTaking;

                //Render Super preset pt
                PT_RenderCombo(document.getElementById('pt_1'), uplineMin, uplineMax, 0.01, uplinePreset);

                //Render super auto preset pt
                PT_RenderCombo(document.getElementById('pt_2'), uplineMin, max, 0.01, uplineAutoPreset);
                if (!casinoGamePT.isAutoPositionTaking) {
                    $('#pt_2').attr('disabled', 'disabled');
                }
                else {
                    $('#pt_1').attr('disabled', 'disabled');
                }
                $('#chkAutoPT').attr('checked', casinoGamePT.isAutoPositionTaking);

                //Render master preset PT
                $('#mPT').html(casinoGamePT.mPositionTaking);

                //Render agent preset PT
                $('#aPT').html(casinoGamePT.aPositionTaking);

                //PT Renderring for Mini Game
                //PT values
                uplineMin = 0;
                uplineMax = miniGamePT.superSPositionTaking - miniGamePT.mPositionTaking - miniGamePT.aPositionTaking;
                uplinePreset = miniGamePT.sPositionTaking;
                uplineAutoPreset = miniGamePT.autoPositionTaking;
                max = miniGamePT.superSPositionTaking;

                //Render Super preset pt
                PT_RenderCombo(document.getElementById('pt_1_1'), uplineMin, uplineMax, 0.01, uplinePreset);

                //Render super auto preset pt
                PT_RenderCombo(document.getElementById('pt_2_1'), uplineMin, max, 0.01, uplineAutoPreset);
                if (!miniGamePT.isAutoPositionTaking) {
                    $('#pt_2_1').attr('disabled', 'disabled');
                }
                else {
                    $('#pt_1_1').attr('disabled', 'disabled');
                }
                $('#chkAutoPT_1').attr('checked', miniGamePT.isAutoPositionTaking);

                //Render master preset PT
                $('#mPT_1').html(miniGamePT.mPositionTaking);

                //Render agent preset PT
                $('#aPT_1').html(miniGamePT.aPositionTaking);
            }
            else if (Fanex.ExtAcc.Customer.isMasterEditMember) {
                $('#chkEnable').attr('disabled', true);
                $('#maxTransfer').attr('disabled', true);
                $('#isAutoReset').attr('disabled', true);

                //PT Renderring for Casino Game
                //PT values
                var uplineMin = 0;
                var uplineMax = casinoGamePT.superSPositionTaking - casinoGamePT.sPositionTaking - casinoGamePT.aPositionTaking;
                var uplinePreset = casinoGamePT.mPositionTaking;

                //Render Super preset pt
                PT_RenderCombo(document.getElementById('pt_1'), uplineMin, uplineMax, 0.01, uplinePreset);

                //Render agent preset PT
                $('#aPT').html(casinoGamePT.aPositionTaking);

                //PT Renderring for Mini Game
                //PT values
                uplineMin = 0;
                uplineMax = miniGamePT.superSPositionTaking - miniGamePT.sPositionTaking - miniGamePT.aPositionTaking;
                uplinePreset = miniGamePT.mPositionTaking;

                //Render Super preset pt
                PT_RenderCombo(document.getElementById('pt_1_1'), uplineMin, uplineMax, 0.01, uplinePreset);

                //Render agent preset PT
                $('#aPT_1').html(miniGamePT.aPositionTaking);
            }
            else {
                //PT Renderring for Casino Game
                //PT values
                var uplineMin = this.customer.customerInfo.casinoSync ? casinoGamePT.minPositionTakingUpline : casinoGamePT.minPositionTaking;
                var uplineMax = this.customer.customerInfo.casinoSync ? casinoGamePT.maxPositionTakingUpline : casinoGamePT.maxPositionTaking;
                var uplinePreset = this.customer.customerInfo.casinoSync ? casinoGamePT.aPositionTaking : casinoGamePT.aPositionTakingUpline;

                //Render Master preset pt
                PT_RenderCombo(document.getElementById('pt_1'), uplineMin, uplineMax, 0.01, uplinePreset);

                //Render Agent Commission
                Comm_RenderCombo(document.getElementById('comm_2'), 0, this.customer.customerInfo.casinoSync ? casinoGamePT.agentRollingRateUpline : casinoGamePT.agentRollingRate, 0.0005, this.customer.customerInfo.casinoSync ? casinoGamePT.memberRollingRate : casinoGamePT.agentRollingRate);

                //PT Renderring for Mini Game
                //PT values
                uplineMin = this.customer.customerInfo.casinoSync ? miniGamePT.minPositionTakingUpline : miniGamePT.minPositionTaking;
                uplineMax = this.customer.customerInfo.casinoSync ? miniGamePT.maxPositionTakingUpline : miniGamePT.maxPositionTaking;
                uplinePreset = this.customer.customerInfo.casinoSync ? miniGamePT.aPositionTaking : miniGamePT.aPositionTakingUpline;

                //Render Master preset pt
                PT_RenderCombo(document.getElementById('pt_1_1'), uplineMin, uplineMax, 0.01, uplinePreset);

                //Render Agent Commission
                Comm_RenderCombo(document.getElementById('comm_2_1'), 0, this.customer.customerInfo.casinoSync ? miniGamePT.agentRollingRateUpline : miniGamePT.agentRollingRate, 0.0005, this.customer.customerInfo.casinoSync ? miniGamePT.memberRollingRate : miniGamePT.agentRollingRate);
            }
        }
    },

    UpdateSetting: function (i) {
        Fanex.ExtAcc.BlockUI();

        var custInfo = Fanex.ExtAcc.Customer.CustomerInfo();

        this.customer.customerInfo.casinoDisabled = !document.getElementById('chkEnable').checked;

        var data = {
            custId: i == null ? Fanex.ExtAcc.Customer.UserId : Fanex.ExtAcc.Customer.Ids()[i],
            roleId: custInfo.roleId,
            uplineId: this.uplineId,
            uplineRoleId: custInfo.uplineRoleId,
            customer: JSON.stringify(this.customer)
        };

        $.ajax({
            url: resolveClientUrl('Casino/UpdateCasinoPost'),
            type: 'post',
            data: data,
            dataType: 'json'
        })
        .success(function (result) {
            if (result.ErrorCode != 0) {
                if (!Fanex.ExtAcc.Customer.Casino.IsMultiple) {
                    Fanex.ExtAcc.Customer.Casino.ErrorMessage += result.ErrorMessage.formatErrorMessage() + '<br />';
                }
                else {
                    Fanex.ExtAcc.Customer.Casino.ErrorMessage += '<font color="Blue" style="font-weight: bold">' + Fanex.ExtAcc.Customer.Names()[i] + ":</font><br />" + result.ErrorMessage.formatErrorMessage() + '<br /><br />';
                }
            }

            if (!Fanex.ExtAcc.Customer.Casino.IsMultiple || (i != null && i == Fanex.ExtAcc.Customer.Ids().length - 1)) {
                if (Fanex.ExtAcc.Customer.Casino.ErrorMessage != '') {
                    $('#errMsg').html(Fanex.ExtAcc.Customer.Casino.ErrorMessage);
                    $('#errMsg').show();

                    Fanex.ExtAcc.Customer.Casino.ErrorMessage = '';
                }
                else {
                    Fanex.ExtAcc.Form.Close();
                    Fanex.ExtAcc.Customer.CallBackCustomerList();
                }

                Fanex.ExtAcc.UnBlockUI();
            }
            else if (Fanex.ExtAcc.Customer.Casino.IsMultiple && i < Fanex.ExtAcc.Customer.Ids().length - 1) {
                Fanex.ExtAcc.Customer.Casino.UpdateSetting(++i);
            }
        })
        .error(function (data) {
            if (typeof console != 'undefined') {
                console.log(data.responseText);
            }

            if (!Fanex.ExtAcc.Customer.Casino.IsMultiple) {
                Fanex.ExtAcc.Customer.Casino.ErrorMessage += "Not Casino\'s customer<br />".formatErrorMessage() + '<br />';
            }
            else {
                Fanex.ExtAcc.Customer.Casino.ErrorMessage += '<font color="Blue" style="font-weight: bold">' + Fanex.ExtAcc.Customer.Names()[i] + ":</font><br />" + " Not Casino\'s customer<br /><br />".formatErrorMessage();
            }

            if (!Fanex.ExtAcc.Customer.Casino.IsMultiple || (i != null && i == Fanex.ExtAcc.Customer.Ids().length - 1)) {
                if (Fanex.ExtAcc.Customer.Casino.ErrorMessage != '') {
                    $('#errMsg').html(Fanex.ExtAcc.Customer.Casino.ErrorMessage);
                    $('#errMsg').show();


                    Fanex.ExtAcc.Customer.Casino.ErrorMessage = '';
                }
                else {
                    Fanex.ExtAcc.Customer.CallBackCustomerList();
                }

                Fanex.ExtAcc.UnBlockUI();
            }
            else if (Fanex.ExtAcc.Customer.Casino.IsMultiple && i < Fanex.ExtAcc.Customer.Ids().length - 1) {
                Fanex.ExtAcc.Customer.Casino.UpdateSetting(++i);
            }
        });
    }
}

function PT_Reduce(id, f) {
    var combo = document.getElementById(id);
    if (combo.disabled) return;

    var selectedIndex = combo.selectedIndex;

    var newSelectedIndex = selectedIndex + f;
    if (newSelectedIndex >= combo.options.length) newSelectedIndex = combo.options.length - 1;

    combo.selectedIndex = newSelectedIndex;

    if (combo.onchange) combo.onchange();
}

function PT_Increase(id, f) {
    var combo = document.getElementById(id);
    if (combo.disabled) return;

    var selectedIndex = combo.selectedIndex;

    var newSelectedIndex = selectedIndex - f;
    if (newSelectedIndex <= 0) newSelectedIndex = 0;

    combo.selectedIndex = newSelectedIndex;
    if (combo.onchange) combo.onchange();
}

function PT_RenderCombo(combo, min, max, step, selectedValue) {
    min = min > max ? max : min;

    if (selectedValue > max) {
        var opt = document.createElement("option");
        opt.text = opt.value = (selectedValue * 100) / 100;
        combo.options.add(opt);
    }

    var index = 0;
    var itemIndex = 0;

    if (!combo) return;
    combo.options.length = 0;

    var c = Math.round((max * 100 - min * 100) / (step * 100));

    if (c <= 0) {
        var opt = document.createElement("option");
        opt.text = opt.value = Math.round(min * 100) / 100;
        combo.options.add(opt);
        return;
    }

    for (var i = c; i >= 0; i--) {
        var f = Math.round(min * 100 + (i * step * 100)) / 100;
        var opt = document.createElement("option");

        if (selectedValue == f) {
            index = itemIndex;
            opt.selected = true;
        }

        opt.text = opt.value = f;
        combo.options.add(opt);
        itemIndex++;
    }


    if (selectedValue && selectedValue <= max) {
        if (document.all)
            combo.options[index].selected = true;
        else
            combo.value = selectedValue;
    }
}

function Comm_RenderCombo(combo, min, max, step, selectedValue) {
    min = min > max ? max : min;
    selectedValue = selectedValue > max ? max : selectedValue;

    var index = 0;
    var itemIndex = 0;

    if (!combo) return;
    combo.options.length = 0;

    var c = Math.round((max * 10000 - min * 10000) / (step * 10000));

    if (c <= 0) {
        var opt = document.createElement("option");
        opt.text = opt.value = Math.round(min * 10000) / 10000;
        combo.options.add(opt);
        return;
    }

    for (var i = c; i >= 0; i--) {
        var f = Math.round(min * 10000 + (i * step * 10000)) / 10000;
        var opt = document.createElement("option");

        if (selectedValue == f) {
            index = itemIndex;
            opt.selected = true;
        }

        opt.text = opt.value = f;
        combo.options.add(opt);
        itemIndex++;
    }

    if (selectedValue) {
        if (document.all)
            combo.options[index].selected = true;
        else
            combo.value = selectedValue;
    }
}

function checkPT(el, sub_id) {
    if (el.checked) {
        var synced = Fanex.ExtAcc.Customer.Casino.customer.customerInfo.syncCasino;
        if (document.getElementById('pt_1' + sub_id))
            document.getElementById('pt_1' + sub_id).disabled = true;
        if (document.getElementById('pt_2' + sub_id))
            document.getElementById('pt_2' + sub_id).disabled = false;

        var pt1 = document.getElementById('pt_1' + sub_id);
        var pt3 = document.getElementById("pt_3" + sub_id);
        var pt4 = document.getElementById("pt_4" + sub_id);
        var maxPT = Fanex.ExtAcc.Customer.Casino.customer.pts[sub_id != '' ? 1 : 0].maxPositionTakingUpline;

        if (!synced) {
            maxPT = Fanex.ExtAcc.Customer.Casino.customer.pts[sub_id != '' ? 1 : 0].maxPositionTaking;
        }

        if (pt3)
            PT_RenderCombo(pt3, 0, (maxPT - parseFloat(pt1.options[pt1.selectedIndex].value).toFixed(2)), 0.01, pt3.options[pt3.selectedIndex].value);
        if (pt4)
            PT_RenderCombo(pt4, 0, pt3.options[pt3.selectedIndex].value, 0.01, pt4.options[pt4.selectedIndex].value);
    }
    else {
        if (document.getElementById('pt_1' + sub_id)) {
            document.getElementById('pt_1' + sub_id).disabled = false;
            PTChanging(document.getElementById('pt_1' + sub_id));
        }
        if (document.getElementById('pt_2' + sub_id))
            document.getElementById('pt_2' + sub_id).disabled = true;
    }
}

function checkMaxPT(sub_id) {
    if (document.getElementById('pt_3' + sub_id) && document.getElementById('pt_4' + sub_id)) {
        PT_RenderCombo2(document.getElementById("pt_4" + sub_id), 0, document.getElementById('pt_3' + sub_id).value, 0.01, sub_id);
        if (parseFloat(document.getElementById('pt_3' + sub_id).value) < parseFloat(document.getElementById('pt_4' + sub_id).value)) {
            document.getElementById('pt_4' + sub_id).value = document.getElementById('pt_3' + sub_id).value;
        }
    }

    return true;
}

function checkMinMaxPT(sub_id) {
    if (document.getElementById('pt_3' + sub_id) && document.getElementById('pt_4' + sub_id)) {
        document.getElementById('pt_3' + sub_id).onchange = function () {
            checkMaxPT(sub_id);
        }
    }
}

function PTChanging(el, sub_id) {
    var sel = parseFloat(el.value);
    if (document.getElementById("pt_3" + sub_id)) {
        var cur = parseFloat(document.getElementById("pt_3" + sub_id).value);
        var uplinePT = Fanex.ExtAcc.Customer.Casino.customer.pts[sub_id != '' ? 1 : 0];
        var synced = Fanex.ExtAcc.Customer.Casino.customer.customerInfo.syncCasino;

        if (document.getElementById("pt_1" + sub_id)) {
            sel = document.getElementById("pt_1" + sub_id).value;
        }

        if (cur == 'undefined' || cur == 'NaN') {
            cur = 0;
        }

        if (cur > (synced ? uplinePT.maxPositionTakingUpline : uplinePT.maxPositionTaking) - sel) {
            cur = (synced ? uplinePT.maxPositionTakingUpline : uplinePT.maxPositionTaking) - sel;
        }

        PT_RenderCombo(document.getElementById("pt_3" + sub_id), 0, (synced ? uplinePT.maxPositionTakingUpline : uplinePT.maxPositionTaking) - sel, 0.01, cur != 0 ? (cur.toFixed(2) * 100) / 100 : 0);
    }
    if (document.getElementById("pt_4" + sub_id)) {
        PT_RenderCombo2(document.getElementById("pt_4" + sub_id), 0, (synced ? uplinePT.maxPositionTakingUpline : uplinePT.maxPositionTaking) - sel, 0.01, sub_id);
    }
}

function PT_RenderCombo2(combo, min, max, step, sub_id) {
    min = min > max ? max : min;

    var index = 0;
    var itemIndex = 0;
    var selectedValue = combo.value;

    if (!combo) return;
    combo.options.length = 0;

    if (combo.id == 'pt_4' + sub_id)
        max = document.getElementById('pt_3' + sub_id).value;

    if (selectedValue > max) {
        selectedValue = max;
    }

    var c = Math.round((max * 100 - min * 100) / (step * 100));

    if (c <= 0) {
        var opt = document.createElement("option");
        opt.text = opt.value = Math.round(min * 100) / 100;
        combo.options.add(opt);
        return;
    }

    for (var i = c; i >= 0; i--) {
        var f = Math.round(min * 100 + (i * step * 100)) / 100;
        var opt = document.createElement("option");

        if (parseFloat(selectedValue) == parseFloat(f)) {
            index = itemIndex;
            opt.selected = true;
        }

        opt.text = opt.value = f;
        combo.options.add(opt);
        itemIndex++;
    }

    if (document.all)
        combo.options[index].selected = true;
    else
        combo.value = selectedValue;
}

$(document).ready(function () {
    Fanex.ExtAcc.Customer.Casino.Init();
});