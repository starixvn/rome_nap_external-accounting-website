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
    custInfo: Fanex.ExtAcc.Customer.CustomerInfo(),

    Init: function () {
        this.LoadData();
    },

    LoadData: function () {
        Fanex.ExtAcc.BlockUI();

        if (this.IsMultiple) {
            $('#custInfo').remove();
            Fanex.ExtAcc.Customer.ProcessDisplayMultiple(125);
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
            data: Fanex.ExtAcc.Customer.Casino.custInfo, //{ custId: Fanex.ExtAcc.Customer.UserId, uplineId: Fanex.ExtAcc.Customer.Casino.uplineId },
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

        var level = $('#downline').html();
        $('#chkEnable').bind('click', function () {
            if (this.checked) {
                $('#downline').html(level);
            }
            else {
                $('#downline').html(level + '&nbsp;' + "<font color='Red'>(Casino downlines will be disabled as well)</font>");
            }
        });
    },

    RenderSetting: function (result) {
        var isPtByGameType = this.customer.casinoInfo.pTByGameType;
        var rowspan = isPtByGameType ? ' rowspan="2" ' : ' rowspan="1" ';

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

                    stakeLevels += '<input ' + checked + ' type="checkbox" id="rng-' + result.casinoGame[i].stakeLevels[j].stakeLevelID + '" />' + result.casinoGame[i].stakeLevels[j].stakeLevelName;
                }

                var rowPT = isPtByGameType ? this.RenderPtByGameType(result.casinoGame[i].pt, i, '_rng') : '';
                var row = rowPT == '' ?
                                '<tr>' +
                                    '<td class="R">' + gameName + '</td>' +
                                    '<td colspan="' + this.colspan + '" style="border-right: none">Betting limit:<span id="rng-bettingLimit-' + i + '">' + stakeLevels + '</span></td>' +
                                '</tr>' :
                                '<tr>' +
                                    '<td class="R" ' + rowspan + '>' + gameName + '</td>' +
                                    rowPT +
                                '</tr>' +
                                '<tr>' +
                                    '<td colspan="' + this.colspan + '" style="border-right: none">Betting limit:<span id="rng-bettingLimit-' + i + '">' + stakeLevels + '</span></td>' +
                                '</tr>';

                $('#casinoGame').append(row);
            }

            $('#rowspanCasino').attr('rowspan', !isPtByGameType ? result.casinoGame.length + 1 : (result.casinoGame.length) * 2 + 1);
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

                    stakeLevels += '<input name="gameRow' + i + '" ' + checked + ' type="checkbox" id="mini-' + result.miniGame[i].stakeLevels[j].stakeLevelID + '" />' + result.miniGame[i].stakeLevels[j].stakeLevelName;
                }

                var rowPT = isPtByGameType ? this.RenderPtByGameType(result.miniGame[i].pt, i, '_mini') : '';
                var row = rowPT == '' ?
                                '<tr>' +
                                    '<td class="R">' + gameName + '</td>' +
                                    '<td colspan="' + this.colspan + '" style="border-right: none">Betting limit:<span id="mini-bettingLimit-' + i + '">' + stakeLevels + '</span></td>' +
                                '</tr>' :
                                '<tr>' +
                                    '<td class="R" ' + rowspan + '>' + gameName + '</td>' +
                                    rowPT +
                                '</tr>' +
                                '<tr>' +
                                    '<td colspan="' + this.colspan + '" style="border-right: none">Betting limit:<span id="mini-bettingLimit-' + i + '">' + stakeLevels + '</span></td>' +
                                '</tr>';

                $('#miniGame').append(row);
            }

            $('#rowspanMini').attr('rowspan', !isPtByGameType ? result.miniGame.length + 1 : (result.miniGame.length) * 2 + 1);
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
        var uplineMin = 0;
        var uplineMax = 0;
        var uplinePreset = 0;
        var uplineAutoPreset = 0;
        var downlinePreset = 0;
        var downlinePreset2 = 0;
        var commMax = 0;
        var commPreset = 0;
        var max = 0;
        var synced = this.customer.customerInfo.casinoSync;

        // Renders Master-Agent PT
        if (Fanex.ExtAcc.Customer.CurrentLevel == Fanex.ExtAcc.Customer.Master) {
            //PT Renderring for Casino Game
            //PT values
            uplineMax = synced ? casinoGamePT.maxPositionTakingUpline : casinoGamePT.maxPositionTaking;
            uplinePreset = synced ? casinoGamePT.sPositionTaking : casinoGamePT.sPositionTakingUpline;
            uplineAutoPreset = synced ? casinoGamePT.autoPositionTaking : casinoGamePT.autoPositionTakingUpline;
            downlinePreset = synced ? casinoGamePT.maxPositionTaking : casinoGamePT.maxPositionTakingUpline;
            downlinePreset2 = synced ? casinoGamePT.minPositionTaking : casinoGamePT.minPositionTakingUpline;
            commMax = synced ? casinoGamePT.sMAgentRollingRateUpline : casinoGamePT.sMAgentRollingRate;
            commPreset = synced ? casinoGamePT.mAgentRollingRate : casinoGamePT.sMAgentRollingRate;

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
            Comm_RenderCombo(document.getElementById('comm_5'), 0, commMax, 0.0005, commPreset);

            //PT Renderring for Mini Game
            //PT values
            uplineMax = synced ? miniGamePT.maxPositionTakingUpline : miniGamePT.maxPositionTaking;
            uplinePreset = synced ? miniGamePT.sPositionTaking : miniGamePT.sPositionTakingUpline;
            uplineAutoPreset = synced ? miniGamePT.autoPositionTaking : miniGamePT.autoPositionTakingUpline;
            downlinePreset = synced ? miniGamePT.maxPositionTaking : miniGamePT.maxPositionTakingUpline;
            downlinePreset2 = synced ? miniGamePT.minPositionTaking : miniGamePT.minPositionTakingUpline;
            commMax = synced ? miniGamePT.sMAgentRollingRateUpline : miniGamePT.sMAgentRollingRate;
            commPreset = synced ? miniGamePT.mAgentRollingRate : miniGamePT.sMAgentRollingRate;

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
            Comm_RenderCombo(document.getElementById('comm_5_1'), 0, commMax, 0.0005, commPreset);
        }
        else if (Fanex.ExtAcc.Customer.CurrentLevel == Fanex.ExtAcc.Customer.Agent) {
            //PT Renderring for Casino Game
            //PT values
            uplineMax = synced ? casinoGamePT.maxPositionTakingUpline : casinoGamePT.maxPositionTaking;
            uplinePreset = synced ? casinoGamePT.mPositionTaking : casinoGamePT.mPositionTakingUpline;
            downlinePreset = synced ? casinoGamePT.aPositionTaking : casinoGamePT.aPositionTakingUpline;
            downlinePreset2 = synced ? casinoGamePT.minPositionTaking : casinoGamePT.minPositionTakingUpline;
            commMax = synced ? casinoGamePT.mAgentRollingRateUpline : casinoGamePT.mAgentRollingRate;
            commPreset = synced ? casinoGamePT.agentRollingRate : casinoGamePT.mAgentRollingRate;

            if (!Fanex.ExtAcc.Customer.isSuperEditAgent) {
                //Render Master preset pt
                PT_RenderCombo(document.getElementById('pt_1'), uplineMin, uplineMax, 0.01, uplinePreset);

                //Render Agent Max PT
                PT_RenderCombo(document.getElementById('pt_3'), 0, (uplineMax - uplinePreset), 0.01, downlinePreset);

                //Render Agent Min PT
                PT_RenderCombo(document.getElementById('pt_4'), 0, downlinePreset, 0.01, downlinePreset2);

                //Render Agent Commission
                Comm_RenderCombo(document.getElementById('comm_5'), 0, commMax, 0.0005, commPreset);
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
            uplineMax = synced ? miniGamePT.maxPositionTakingUpline : miniGamePT.maxPositionTaking;
            uplinePreset = synced ? miniGamePT.mPositionTaking : miniGamePT.mPositionTakingUpline;
            downlinePreset = synced ? miniGamePT.aPositionTaking : miniGamePT.aPositionTakingUpline;
            downlinePreset2 = synced ? miniGamePT.minPositionTaking : miniGamePT.minPositionTakingUpline;
            commMax = synced ? miniGamePT.mAgentRollingRateUpline : miniGamePT.mAgentRollingRate;
            commPreset = synced ? miniGamePT.agentRollingRate : miniGamePT.mAgentRollingRate;

            if (!Fanex.ExtAcc.Customer.isSuperEditAgent) {
                //Render Master preset pt
                PT_RenderCombo(document.getElementById('pt_1_1'), uplineMin, uplineMax, 0.01, uplinePreset);

                //Render Agent Max PT
                PT_RenderCombo(document.getElementById('pt_3_1'), 0, (uplineMax - uplinePreset), 0.01, downlinePreset);

                //Render Agent Min PT
                PT_RenderCombo(document.getElementById('pt_4_1'), 0, downlinePreset, 0.01, downlinePreset2);

                //Render Agent Commission
                Comm_RenderCombo(document.getElementById('comm_5_1'), 0, commMax, 0.0005, commPreset);
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
                uplineMin = casinoGamePT.isAutoPositionTaking ? casinoGamePT.superMinPositionTaking - casinoGamePT.mPositionTaking - casinoGamePT.aPositionTaking : 0;
                uplineMax = casinoGamePT.superMaxPositionTaking - casinoGamePT.mPositionTaking - casinoGamePT.aPositionTaking;
                uplinePreset = casinoGamePT.sPositionTaking;
                uplineAutoPreset = casinoGamePT.autoPositionTaking;
                max = casinoGamePT.superMaxPositionTaking;

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
                uplineMin = miniGamePT.isAutoPositionTaking ? miniGamePT.superMinPositionTaking - miniGamePT.mPositionTaking - miniGamePT.aPositionTaking : 0;
                uplineMax = miniGamePT.superMaxPositionTaking - miniGamePT.mPositionTaking - miniGamePT.aPositionTaking;
                uplinePreset = miniGamePT.sPositionTaking;
                uplineAutoPreset = miniGamePT.autoPositionTaking;
                max = miniGamePT.superMaxPositionTaking;

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
                uplineMax = casinoGamePT.masterMaxPositionTaking - casinoGamePT.aPositionTaking;
                uplinePreset = casinoGamePT.mPositionTaking;

                //Render Super preset pt
                PT_RenderCombo(document.getElementById('pt_1'), uplineMin, uplineMax, 0.01, uplinePreset);

                //Render agent preset PT
                $('#aPT').html(casinoGamePT.aPositionTaking);

                //PT Renderring for Mini Game
                //PT values
                uplineMax = miniGamePT.masterMaxPositionTaking - miniGamePT.aPositionTaking;
                uplinePreset = miniGamePT.mPositionTaking;

                //Render Super preset pt
                PT_RenderCombo(document.getElementById('pt_1_1'), uplineMin, uplineMax, 0.01, uplinePreset);

                //Render agent preset PT
                $('#aPT_1').html(miniGamePT.aPositionTaking);
            }
            else {
                //PT Renderring for Casino Game
                //PT values
                uplineMax = synced ? casinoGamePT.maxPositionTakingUpline : casinoGamePT.maxPositionTaking;
                uplinePreset = synced ? casinoGamePT.aPositionTaking : casinoGamePT.aPositionTakingUpline;
                commMax = synced ? casinoGamePT.agentRollingRateUpline : casinoGamePT.agentRollingRate;
                commPreset = synced ? casinoGamePT.memberRollingRate : casinoGamePT.agentRollingRate;

                //Render Master preset pt
                PT_RenderCombo(document.getElementById('pt_1'), uplineMin, uplineMax, 0.01, uplinePreset);

                //Render Agent Commission
                Comm_RenderCombo(document.getElementById('comm_2'), 0, commMax, 0.0005, commPreset);

                //PT Renderring for Mini Game
                //PT values
                uplineMax = synced ? miniGamePT.maxPositionTakingUpline : miniGamePT.maxPositionTaking;
                uplinePreset = synced ? miniGamePT.aPositionTaking : miniGamePT.aPositionTakingUpline;
                commMax = synced ? miniGamePT.agentRollingRateUpline : miniGamePT.agentRollingRate;
                commPreset = synced ? miniGamePT.memberRollingRate : miniGamePT.agentRollingRate;

                //Render Master preset pt
                PT_RenderCombo(document.getElementById('pt_1_1'), uplineMin, uplineMax, 0.01, uplinePreset);

                //Render Agent Commission
                Comm_RenderCombo(document.getElementById('comm_2_1'), 0, commMax, 0.0005, commPreset);
            }
        }
    },

    UpdateSetting: function (idx) {
        var min = parseFloat(Fanex.ExtAcc.ReplaceAll($('#minCredit').text(), ',', ''));
        var max = parseFloat(Fanex.ExtAcc.ReplaceAll($('#maxCredit').text(), ',', ''));
        var cur = parseFloat(Fanex.ExtAcc.ReplaceAll($('#maxTransfer').val(), ',', ''));

        if (cur < min || cur > max) {
            Validators.show($('#maxTransfer').get(0), 'Member Max Transfer Credit must be between ' + $('#minCredit').text() + ' and ' + $('#maxCredit').text());

            return;
        }

        Fanex.ExtAcc.BlockUI();

        var custInfo = this.custInfo;

        // Sets status, max transfer limit and daily auto reset RnG Casino
        this.customer.customerInfo.casinoDisabled = !document.getElementById('chkEnable').checked;
        this.customer.casinoInfo.maxTransferCredit = parseFloat(Fanex.ExtAcc.ReplaceAll($('#maxTransfer').val(), ',', ''));
        this.customer.casinoInfo.isAutoReset = document.getElementById('isAutoReset').checked;

        // PT and Betting limit variables
        var ptRngGame = this.customer.pts[0];
        var ptMiniGame = this.customer.pts[1];
        var rngGames = this.customer.casinoGame;
        var miniGames = this.customer.miniGame;
        var isPtByGameType = this.customer.casinoInfo.pTByGameType;

        // Sets PT for RnG and Mini Game
        switch (custInfo.uplineRoleId) {
            case Fanex.ExtAcc.Customer.Super:
                if (custInfo.roleId == Fanex.ExtAcc.Customer.Master) {
                    this.SetPtForMaster(ptRngGame);
                    this.SetPtForMaster(ptMiniGame, '_1');
                }
                else if (custInfo.roleId == Fanex.ExtAcc.Customer.Member) {
                    this.SetPtForMemberBySuper(ptRngGame);
                    this.SetPtForMemberBySuper(ptMiniGame, '_1');
                }
                break;
            case Fanex.ExtAcc.Customer.Master:
                if (custInfo.roleId == Fanex.ExtAcc.Customer.Agent) {
                    this.SetPtForAgent(ptRngGame);
                    this.SetPtForAgent(ptMiniGame, '_1');
                }
                else if (custInfo.roleId == Fanex.ExtAcc.Customer.Member) {
                    this.SetPtForMemberByMaster(ptRngGame);
                    this.SetPtForMemberByMaster(ptMiniGame, '_1');
                }
                break;
            case Fanex.ExtAcc.Customer.Agent:
                if (custInfo.roleId == Fanex.ExtAcc.Customer.Member) {
                    this.SetPtForMember(ptRngGame);
                    this.SetPtForMember(ptMiniGame, '_1');
                }
                break;
            default:
                break;
        }

        // Sets Betting Limits and PT By Game Type for RnG Game
        for (var i = 0; i < rngGames.length; i++) {
            var game = rngGames[i];

            if (game.stakeLevels.length > 0) {
                for (var j = 0; j < game.stakeLevels.length; j++) {
                    game.stakeLevels[j].isChecked = document.getElementById('rng-' + game.stakeLevels[j].stakeLevelID).checked;
                }

            }

            if (isPtByGameType) {
                this.SetPtForGameType(game.pt, '_1_' + i + '_rng');
            }
        }

        // Sets Betting Limits for Mini Game
        for (var i = 0; i < miniGames.length; i++) {
            var game = miniGames[i];

            if (game.stakeLevels.length > 0) {
                for (var j = 0; j < game.stakeLevels.length; j++) {
                    game.stakeLevels[j].isChecked = document.getElementById('mini-' + +game.stakeLevels[j].stakeLevelID).checked;
                }
            }

            if (isPtByGameType) {
                this.SetPtForGameType(game.pt, '_1_' + i + '_mini');
            }
        }

        var data = {
            custId: idx == null ? Fanex.ExtAcc.Customer.UserId : Fanex.ExtAcc.Customer.Ids()[idx],
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
                    Fanex.ExtAcc.Customer.Casino.ErrorMessage += '<font color="Blue" style="font-weight: bold">' + Fanex.ExtAcc.Customer.Names()[idx] + ":</font><br />" + result.ErrorMessage.formatErrorMessage() + '<br /><br />';
                }
            }

            if (!Fanex.ExtAcc.Customer.Casino.IsMultiple || (idx != null && idx == Fanex.ExtAcc.Customer.Ids().length - 1)) {
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
            else if (Fanex.ExtAcc.Customer.Casino.IsMultiple && idx < Fanex.ExtAcc.Customer.Ids().length - 1) {
                Fanex.ExtAcc.Customer.Casino.UpdateSetting(++idx);
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
                Fanex.ExtAcc.Customer.Casino.ErrorMessage += '<font color="Blue" style="font-weight: bold">' + Fanex.ExtAcc.Customer.Names()[idx] + ":</font><br />" + " Not Casino\'s customer<br /><br />".formatErrorMessage();
            }

            if (!Fanex.ExtAcc.Customer.Casino.IsMultiple || (idx != null && idx == Fanex.ExtAcc.Customer.Ids().length - 1)) {
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
            else if (Fanex.ExtAcc.Customer.Casino.IsMultiple && idx < Fanex.ExtAcc.Customer.Ids().length - 1) {
                Fanex.ExtAcc.Customer.Casino.UpdateSetting(++idx);
            }
        });
    },

    SetPtForGameType: function (pt, subfix) {
        switch (this.custInfo.uplineRoleId) {
            case Fanex.ExtAcc.Customer.Super:
                if (this.custInfo.roleId == Fanex.ExtAcc.Customer.Master) {
                    this.SetPtForMaster(pt, subfix);
                }
                else if (this.custInfo.roleId == Fanex.ExtAcc.Customer.Member) {
                    this.SetPtForMemberBySuper(pt, subfix);
                }
                break;
            case Fanex.ExtAcc.Customer.Master:
                if (this.custInfo.roleId == Fanex.ExtAcc.Customer.Agent) {
                    this.SetPtForAgent(pt, subfix);
                }
                else if (this.custInfo.roleId == Fanex.ExtAcc.Customer.Member) {
                    this.SetPtForMemberByMaster(pt, subfix);
                }
                break;
            case Fanex.ExtAcc.Customer.Agent:
                this.SetPtForMember(pt, subfix);
                break;
            default:
                break;
        }
    },

    SetPtForMaster: function (pt, _subfix) {
        var subfix = _subfix != null ? _subfix : '';
        var chkId = (subfix.indexOf('_rng') != -1 || subfix.indexOf('_mini') != -1) ? ('chkAutoPT_1' + subfix) : ('chkAutoPT' + subfix);
        // Super preset PT
        pt.sPositionTaking = $('#pt_1' + subfix).val();
        // Super Is Auto PT
        pt.isAutoPositionTaking = document.getElementById(chkId).checked;

        // Super Auto Preset PT
        pt.autoPositionTaking = !pt.isAutoPositionTaking ? 0 : $('#pt_2' + subfix).val();

        // Master Preset PT
        pt.mPositionTaking = $('#pt_3' + subfix).val();

        // Master Min PT
        pt.minPositionTaking = $('#pt_4' + subfix).val();

        // Master Commission
        pt.mAgentRollingRate = $('#comm_5' + subfix).val();

        return pt;
    },

    SetPtForAgent: function (pt, _subfix) {
        var subfix = _subfix != null ? _subfix : '';
        // Master Preset PT
        pt.mPositionTaking = $('#pt_1' + subfix).val();

        // Agent Preset PT
        pt.aPositionTaking = $('#pt_3' + subfix).val();

        // Agent Min PT
        pt.minPositionTaking = $('#pt_4' + subfix).val();

        // Agent Commission
        pt.agentRollingRate = $('#comm_5' + subfix).val();

        return pt;
    },

    SetPtForMember: function (pt, _subfix) {
        var subfix = _subfix != null ? _subfix : '';
        // Agent Preset PT
        pt.aPositionTaking = $('#pt_1' + subfix).val();

        // Member Commission
        pt.memberRollingRate = $('#comm_2' + subfix).val();

        return pt;
    },

    SetPtForMemberBySuper: function (pt, _subfix) {
        var subfix = _subfix != null ? _subfix : '';
        // Super preset PT
        pt.sPositionTaking = $('#pt_1' + subfix).val();

        // Super Is Auto PT
        pt.isAutoPositionTaking = document.getElementById('chkAutoPT' + subfix).checked;

        // Super Auto Preset PT
        pt.autoPositionTaking = $('#pt_2' + subfix).val();

        return pt;
    },

    SetPtForMemberByMaster: function (pt, _subfix) {
        var subfix = _subfix != null ? _subfix : '';
        // Master Preset PT
        ptRngGame.mPositionTaking = $('#pt_1' + subfix).val();

        return pt;
    },

    RenderPtByGameType: function (pt, i, subfix) {
        var row = '';

        switch (this.custInfo.uplineRoleId) {
            case Fanex.ExtAcc.Customer.Super:
                if (this.custInfo.roleId == Fanex.ExtAcc.Customer.Master) {
                    row = this.RenderPtByGameTypeMaster(pt, i, subfix);
                }
                else if (this.custInfo.roleId == Fanex.ExtAcc.Customer.Agent) {
                    row = this.RenderPtByGameTypeAgentBySuper(pt);
                }
                else if (this.custInfo.roleId == Fanex.ExtAcc.Customer.Member) {
                    row = this.RenderPtByGameTypeMemberBySuper(pt, i, subfix);
                }
                break;
            case Fanex.ExtAcc.Customer.Master:
                if (this.custInfo.roleId == Fanex.ExtAcc.Customer.Agent) {
                    row = this.RenderPtByGameTypeAgent(pt, i, subfix);
                }
                else if (this.custInfo.roleId == Fanex.ExtAcc.Customer.Member) {
                    row = this.RenderPtByGameTypeMemberByMaster(pt, i, subfix);
                }
                break;
            case Fanex.ExtAcc.Customer.Agent:
                row = this.RenderPtByGameTypeMember(pt, i, subfix);
                break;
            default:
                break;
        }

        return row;
    },

    RenderPtByGameTypeMaster: function (pt, idx, subfix) {
        var row = '';
        var synced = this.customer.customerInfo.casinoSync;

        var uplineMax = synced ? pt.maxPositionTakingUpline : pt.maxPositionTaking;
        var uplinePreset = synced ? pt.sPositionTaking : pt.sPositionTakingUpline;
        var uplineAutoPreset = synced ? pt.autoPositionTaking : pt.autoPositionTakingUpline;
        var downlinePreset = synced ? pt.maxPositionTaking : pt.maxPositionTakingUpline;
        var downlinePreset2 = synced ? pt.minPositionTaking : pt.minPositionTakingUpline;
        var commMax = synced ? pt.sMAgentRollingRateUpline : pt.sMAgentRollingRate;
        var commPreset = synced ? pt.mAgentRollingRate : pt.sMAgentRollingRate;

        var isAuto = synced ? pt.isAutoPositionTaking : pt.isAutoPositionTakingUpline;
        var checked = isAuto ? ' checked="checked"' : '';
        var input = '<input name="' + subfix + '" onclick="checkPT(this, \'_1_' + idx + subfix + '\', ' + uplineMax + ')" type="checkbox" id="chkAutoPT_1_1_' + idx + subfix + '" ' + checked + ' />';
        var disabledPT1 = isAuto ? ' disabled="disabled"' : '';
        var disabledPT2 = !isAuto ? ' disabled="disabled"' : '';

        row += '<td class="C"><select name="pt_1' + subfix + '" ' + disabledPT1 + ' id="pt_1_1_' + idx + subfix + '" onchange="PTChanging(this, \'_1_' + idx + subfix + '\', ' + uplineMax + ')">' + PT_RenderOption(0, uplineMax, 0.01, uplinePreset) + '</select></td>';
        row += '<td class="C">' + input + '<select name="pt_2' + subfix + '" ' + disabledPT2 + ' id="pt_2_1_' + idx + subfix + '">' + PT_RenderOption(0, uplineMax, 0.01, uplineAutoPreset) + '</select></td>';
        row += '<td class="C"><select name="pt_3' + subfix + '" id="pt_3_1_' + idx + subfix + '" onchange="PTChanging(this, \'_1_' + idx + subfix + '\', ' + uplineMax + ')">' + PT_RenderOption(0, uplineMax - uplinePreset, 0.01, downlinePreset) + '</select></td>';
        row += '<td class="C"><select name="pt_4' + subfix + '" id="pt_4_1_' + idx + subfix + '">' + PT_RenderOption(0, downlinePreset, 0.01, downlinePreset2) + '</select></td>';
        row += '<td class="C" style="border-right: none"><select name="comm_5' + subfix + '" id="comm_5_1_' + idx + subfix + '">' + Comm_RenderOption(0, commMax, 0.0005, commPreset) + '</select></td>';

        return row;
    },

    RenderPtByGameTypeAgent: function (pt, idx, subfix) {
        var row = '';
        var synced = this.customer.customerInfo.casinoSync;

        var uplineMin = 0;
        var uplineMax = synced ? pt.maxPositionTakingUpline : pt.maxPositionTaking;
        var uplinePreset = synced ? pt.mPositionTaking : pt.mPositionTakingUpline;
        var downlinePreset = synced ? pt.aPositionTaking : pt.aPositionTakingUpline;
        var downlinePreset2 = synced ? pt.minPositionTaking : pt.minPositionTakingUpline;
        var commMax = synced ? pt.mAgentRollingRateUpline : pt.mAgentRollingRate;
        var commPreset = synced ? pt.agentRollingRate : pt.mAgentRollingRate;

        row += '<td class="C"><select name="pt_1' + subfix + '" id="pt_1_1_' + idx + subfix + '" onchange="PTChanging(this, \'_1_' + idx + subfix + '\', ' + uplineMax + ')">' + PT_RenderOption(0, uplineMax, 0.01, uplinePreset) + '</select></td>';
        row += '<td class="C"><select name="pt_3' + subfix + '" id="pt_3_1_' + idx + subfix + '" onchange="PTChanging(this, \'_1_' + idx + subfix + '\', ' + uplineMax + ')">' + PT_RenderOption(0, uplineMax - uplinePreset, 0.01, downlinePreset) + '</select></td>';
        row += '<td class="C"><select name="pt_4' + subfix + '" id="pt_4_1_' + idx + subfix + '">' + PT_RenderOption(0, downlinePreset, 0.01, downlinePreset2) + '</select></td>';
        row += '<td class="C" style="border-right: none"><select name="comm_5' + subfix + '" id="comm_5_1_' + idx + subfix + '">' + Comm_RenderOption(0, commMax, 0.0005, commPreset) + '</select></td>';

        return row;
    },

    RenderPtByGameTypeAgentBySuper: function (pt) {
        var row = '';

        row += '<td class="C">' + pt.mPositionTaking + '</td>';
        row += '<td class="C" style="border-right: none">' + pt.aPositionTaking + '</td>';

        return row;
    },

    RenderPtByGameTypeMember: function (pt, idx, subfix) {
        var row = '';
        var synced = this.customer.customerInfo.casinoSync;

        var uplineMin = 0;
        var uplineMax = synced ? pt.maxPositionTakingUpline : pt.maxPositionTaking;
        var uplinePreset = synced ? pt.aPositionTaking : pt.aPositionTakingUpline;
        var commMax = synced ? pt.agentRollingRateUpline : pt.agentRollingRate;
        var commPreset = synced ? pt.memberRollingRate : pt.agentRollingRate;

        row += '<td class="C"><select name="pt_1' + subfix + '" id="pt_1_1_' + idx + subfix + '">' + PT_RenderOption(uplineMin, uplineMax, 0.01, uplinePreset) + '</select></td>';
        row += '<td class="C" style="border-right: none"><select name="comm_2' + subfix + '" id="comm_2_1_' + idx + subfix + '">' + Comm_RenderOption(0, commMax, 0.0005, commPreset) + '</select></td>';

        return row;
    },

    RenderPtByGameTypeMemberBySuper: function (pt, idx, subfix) {
        var row = '';
        var uplineMin = pt.isAutoPositionTaking ? pt.superMinPositionTaking - pt.mPositionTaking - pt.aPositionTaking : 0;
        var uplineMax = pt.superMaxPositionTaking - pt.mPositionTaking - pt.aPositionTaking;
        var uplinePreset = pt.sPositionTaking;
        var uplineAutoPreset = pt.autoPositionTaking;
        var max = pt.superMaxPositionTaking;

        var isAuto = pt.isAutoPositionTaking;
        var checked = isAuto ? ' checked="checked"' : '';
        var input = '<input name="' + subfix + '" onclick="checkPT(this, \'_1_' + idx + subfix + '\', ' + uplineMax + ')" type="checkbox" id="chkAutoPT_1_1_' + idx + subfix + '" ' + checked + ' />';
        var disabledPT1 = isAuto ? ' disabled="disabled"' : '';
        var disabledPT2 = !isAuto ? ' disabled="disabled"' : '';

        row += '<td class="C"><select name="pt_1' + subfix + '" ' + disabledPT1 + ' id="pt_1_1_' + idx + subfix + '" onchange="PTChanging(this, \'_1_' + idx + subfix + '\', ' + uplineMax + ')">' + PT_RenderOption(uplineMin, uplineMax, 0.01, uplinePreset) + '</select></td>';
        row += '<td class="C">' + input + '<select name="pt_2' + subfix + '" ' + disabledPT2 + ' id="pt_2_1_' + idx + subfix + '">' + PT_RenderOption(uplineMin, max, 0.01, uplineAutoPreset) + '</select></td>';
        row += '<td class="C">' + pt.mPositionTaking + '</td>';
        row += '<td class="C" style="border-right: none">' + pt.aPositionTaking + '</td>';

        return row;
    },

    RenderPtByGameTypeMemberByMaster: function (pt, idx, subfix) {
        var row = '';

        var uplineMin = 0;
        var uplineMax = pt.masterMaxPositionTaking;
        var uplinePreset = pt.mPositionTaking;


        row += '<td class="C"><select name="pt_1' + subfix + '" id="pt_1_1_' + idx + subfix + '" onchange="PTChanging(this, \'_1_' + idx + subfix + '\', ' + uplineMax + ')">' + PT_RenderOption(uplineMin, uplineMax, 0.01, uplinePreset) + '</select></td>';
        row += '<td class="C" style="border-right: none">' + pt.aPositionTaking + '</td>';

        return row;
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

function PT_RenderOption(min, max, step, selectedValue) {
    var option = '';
    min = min > max ? max : min;

    if (selectedValue > max) {
        option = '<option value="' + ((selectedValue * 100) / 100) + '">' + ((selectedValue * 100) / 100) + '</option>';
    }

    var c = Math.round((max * 100 - min * 100) / (step * 100));

    if (c <= 0) {
        return '<option value="' + (Math.round(min * 100) / 100) + '">' + (Math.round(min * 100) / 100) + '</option>';
    }

    for (var i = c; i >= 0; i--) {
        var f = Math.round(min * 100 + (i * step * 100)) / 100;
        var selected = '';

        if (selectedValue == f) {
            selected = ' selected="selected"';
        }

        option += '<option value="' + f + '"' + selected + '>' + f + '</option>';
    }

    return option;
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

function Comm_RenderOption(min, max, step, selectedValue) {
    var option = '';

    min = min > max ? max : min;
    selectedValue = selectedValue > max ? max : selectedValue;

    var c = Math.round((max * 10000 - min * 10000) / (step * 10000));

    if (c <= 0) {
        return '<option value="' + Math.round(min * 10000) / 10000 + '">' + Math.round(min * 10000) / 10000 + '</option>';
    }

    for (var i = c; i >= 0; i--) {
        var f = Math.round(min * 10000 + (i * step * 10000)) / 10000;
        var selected = '';
           
        if (selectedValue == f) {
            selected = ' selected="selected"';
        }

        option += '<option value="' + f + '"' + selected + '>' + f + '</option>';
    }

    return option;
}

function checkPT(el, sub_id, max) {
    var ptByGameType = Fanex.ExtAcc.Customer.Casino.customer.casinoInfo.pTByGameType;
    var isChangedGroupRnG = sub_id == '' && ptByGameType;
    var isChangedGroupMini = sub_id == '_1' && ptByGameType;

    if (el.checked) {
        if (isChangedGroupRnG) {
            var chkRnGs = $('input[name=_rng]');
            for (var i = 0; i < chkRnGs.length; i++) {
                chkRnGs[i].checked = true;
                chkRnGs[i].onclick();
            }
        }
        else if (isChangedGroupMini) {
            var chkMinis = $('input[name=_mini]');
            for (var i = 0; i < chkMinis.length; i++) {
                chkMinis[i].checked = true;
                chkMinis[i].onclick();
            }
        }

        var synced = Fanex.ExtAcc.Customer.Casino.customer.customerInfo.casinoSync;
        if (document.getElementById('pt_1' + sub_id))
            document.getElementById('pt_1' + sub_id).disabled = true;
        if (document.getElementById('pt_2' + sub_id))
            document.getElementById('pt_2' + sub_id).disabled = false;

        var pt1 = document.getElementById('pt_1' + sub_id);
        var pt3 = document.getElementById("pt_3" + sub_id);
        var pt4 = document.getElementById("pt_4" + sub_id);
        var maxPT = !max ? Fanex.ExtAcc.Customer.Casino.customer.pts[sub_id != '' ? 1 : 0].maxPositionTakingUpline : max;

        if (!synced) {
            maxPT = !max ? Fanex.ExtAcc.Customer.Casino.customer.pts[sub_id != '' ? 1 : 0].maxPositionTaking : max;
        }

        if (pt3)
            PT_RenderCombo(pt3, 0, (maxPT - parseFloat(pt1.options[pt1.selectedIndex].value).toFixed(2)), 0.01, pt3.options[pt3.selectedIndex].value);
        if (pt4)
            PT_RenderCombo(pt4, 0, pt3.options[pt3.selectedIndex].value, 0.01, pt4.options[pt4.selectedIndex].value);
    }
    else {
        if (isChangedGroupRnG) {
            var chkRnGs = $('input[name=_rng]');
            for (var i = 0; i < chkRnGs.length; i++) {
                chkRnGs[i].checked = false;
                chkRnGs[i].onclick();
            }
        }
        else if (isChangedGroupMini) {
            var chkMinis = $('input[name=_mini]');
            for (var i = 0; i < chkMinis.length; i++) {
                chkMinis[i].checked = false;
                chkMinis[i].onclick();
            }
        }

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

function PTChanging(el, sub_id, max) {
    var ptByGameType = Fanex.ExtAcc.Customer.Casino.customer.casinoInfo.pTByGameType;
    var isChangedGroupRnG = sub_id == '' && ptByGameType;
    var isChangedGroupMini = sub_id == '_1' && ptByGameType;
    var sel = parseFloat(el.value);
    var id = el.id;
    var cur = 0;
    var synced = Fanex.ExtAcc.Customer.Casino.customer.customerInfo.casinoSync;
    var maxPT = !max ? (synced ? Fanex.ExtAcc.Customer.Casino.customer.pts[sub_id != '' ? 1 : 0].maxPositionTakingUpline : Fanex.ExtAcc.Customer.Casino.customer.pts[sub_id != '' ? 1 : 0].maxPositionTaking) : max;

    if (isChangedGroupRnG) {
        // Render comboboxes for game type setting PT of RnG game
        var chkRnGs = $('select[name=' + id + '_rng]');
        for (var i = 0; i < chkRnGs.length; i++) {
            if (document.getElementById("pt_3" + sub_id)) {
                cur = document.getElementById("pt_3" + sub_id) ? parseFloat(document.getElementById("pt_3" + sub_id).value) : 0;

                if (document.getElementById("pt_1" + sub_id)) {
                    sel = document.getElementById("pt_1" + sub_id).value;
                }

                if (cur == 'undefined' || cur == 'NaN') {
                    cur = 0;
                }

                if (cur > maxPT - sel) {
                    cur = maxPT - sel;
                }

                PT_RenderCombo(document.getElementById("pt_3_1_" + i + "_rng"), 0, maxPT - sel, 0.01, cur != 0 ? (cur.toFixed(2) * 100) / 100 : 0);
            }
            if (document.getElementById("pt_4" + sub_id)) {
                PT_RenderCombo2(document.getElementById("pt_4_1_" + i + "_rng"), 0, cur, 0.01, sub_id);
            }

            chkRnGs[i].value = el.value;
        }
    }
    else if (isChangedGroupMini) {
        // Render comboboxes for game type setting PT of mini game
        var chkMinis = $('select[name=' + id.substring(0, id.length - 2) + '_mini]');
        for (var i = 0; i < chkMinis.length; i++) {
            if (document.getElementById("pt_3" + sub_id)) {
                cur = document.getElementById("pt_3" + sub_id) ? parseFloat(document.getElementById("pt_3" + sub_id).value) : 0;

                if (document.getElementById("pt_1" + sub_id)) {
                    sel = document.getElementById("pt_1" + sub_id).value;
                }

                if (cur == 'undefined' || cur == 'NaN') {
                    cur = 0;
                }

                if (cur > maxPT - sel) {
                    cur = maxPT - sel;
                }

                PT_RenderCombo(document.getElementById("pt_3_1_" + i + "_mini"), 0, maxPT - sel, 0.01, cur != 0 ? (cur.toFixed(2) * 100) / 100 : 0);
            }
            if (document.getElementById("pt_4" + sub_id)) {
                PT_RenderCombo2(document.getElementById("pt_4_1_" + i + "_mini"), 0, cur, 0.01, sub_id);
            }

            chkMinis[i].value = el.value;
        }
    }

    // Render comboboxes for game group setting PT
    if (document.getElementById("pt_3" + sub_id)) {
        cur = document.getElementById("pt_3" + sub_id) ? parseFloat(document.getElementById("pt_3" + sub_id).value) : 0;

        if (document.getElementById("pt_1" + sub_id)) {
            sel = document.getElementById("pt_1" + sub_id).value;
        }

        if (cur == 'undefined' || cur == 'NaN') {
            cur = 0;
        }

        if (cur > maxPT - sel) {
            cur = maxPT - sel;
        }

        PT_RenderCombo(document.getElementById("pt_3" + sub_id), 0, maxPT - sel, 0.01, cur != 0 ? (cur.toFixed(2) * 100) / 100 : 0);
    }
    if (document.getElementById("pt_4" + sub_id)) {
        PT_RenderCombo2(document.getElementById("pt_4" + sub_id), 0, cur, 0.01, sub_id);
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