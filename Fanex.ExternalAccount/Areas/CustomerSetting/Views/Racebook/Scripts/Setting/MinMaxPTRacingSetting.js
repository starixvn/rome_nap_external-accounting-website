var minMaxPTRacingSetting = {
    roleId: {
        master: 3,
        agent: 2,
        masterMinMax: -4,
        agentMinMax: -5
    },
    currentSetting: {},
    pageCrypt: undefined,
    initialize: function () {
        this.getMinMaxPTSetting();
    },
    getMinMaxPTSetting: function () {
        var param = {
            custId: window.custId,
            roleId: window.targetRoleId
        };

        $.ajax({
            type: 'POST',
            url: 'GetMinMaxPTSetting',
            contentType: 'application/json; charset=utf-8',
            dataType: 'json',
            data: JSON.stringify(param),
            beforeSend: function (jqXHR, settings) {
                PTBlockUI();
            },
            success: function (data, textStatus, jqXHR) {
                if (data) {
                    minMaxPTRacingSetting.currentSetting = JSON.parse(data);
                    minMaxPTRacingSetting.initializeHtml(minMaxPTRacingSetting.currentSetting);
                }
            },
            error: function (jqXHR, textStatus, errorThrown) {
                showAlertMessage('err', 'Init context error.', 1);
            },
            complete: function (jqXHR, textStatus) {
                PTUnBlockUI();
            }
        });
    },
    updatePT: function () {
        if (window.targetRoleId === this.roleId.masterMinMax || window.targetRoleId === this.roleId.agentMinMax) {
            PTBlockUI();
            minMaxPTRacingSetting.setParamUpdatePT();

            var data = JSON.stringify({ setting: this.currentSetting, targetRoleId: window.targetRoleId, groupName: parent.$('#selectedGroupName').html() });
            $.ajax({
                type: 'POST',
                url: 'UpdateMinMaxPT',
                contentType: 'application/json; charset=utf-8',
                dataType: 'json',
                data: data,
                beforeSend: function (jqXHR, settings) { },
                success: function (data, textStatus, jqXHR) {
                    if (data) {
                        var result = JSON.parse(data);
                        minMaxPTRacingSetting.afterUpdatePT(result);
                    }
                },
                error: function (jqXHR, textStatus, errorThrown) { },
                complete: function (jqXHR, textStatus) {
                    PTUnBlockUI();
                }
            });
        }
    },
    afterUpdatePT: function (result) {
        var message = result.ErrorCode === 0 ? 'Updated successfully.' : result.ErrorMessage;
        showAlertMessage('err', message, result.ErrorCode === 0 ? 0 : 1);
        window.setTimeout(function () {
            minMaxPTRacingSetting.getMinMaxPTSetting();
        }, 3000)
    },
    initializeHtml: function (data) {
        if (data.RacingSportType == null || data.RacingSportType.length === 0) {
            showAlertMessage('err', 'There is no Racing PositionTaking data.', 1);
            return;
        }
        clearAlertMessage('err');
        var $container = $('#SettingContent');
        $container.html('')
            .append(this.initCutomerInformationSection(data.CustInfo))
            .append(this.initCopyAllPT(data))
            .append(this.initRacingSportType(data.RacingSportType));
        $('.floatR').removeClass('display-none'); // show update buttons

        AssignMinMaxPT();
        $container.undelegate('.expanded', 'click');
        $container.undelegate('.btnmiddle', 'click');
        $container.delegate('.expanded', 'click', function () {
            var obj = $(this);
            if (obj.hasClass('expand')) {
                obj.removeClass('expand');
                $('#' + obj.attr('target')).show();
            }
            else {
                obj.addClass("expand");
                $('#' + obj.attr('target')).hide();
            }
        });
        $container.delegate('.btnmiddle', 'click', function () {
            var obj = $(this);
            var target = obj.attr('target');
            var targetobj = $('.' + target);
            if (targetobj.hasClass('md3')) {
                targetobj.removeClass('md3');
                targetobj.addClass('md2');
                $('#' + target).show();
            }
            else {
                targetobj.addClass('md3');
                targetobj.removeClass('md2');
                $('#' + target).hide();
            }
        });
    },
    initCutomerInformationSection: function (customerInfo) {
        return '<table class="tblcollapse w1000 marginT6"><tbody>\
            <tr class="title">\
                <td colspan="4">' + this.getRoleNameById(customerInfo.CustLevel) + ': ' + customerInfo.UserName + '</td>\
            </tr>\
            <tr>\
                <td width="20%" class="r">First Name</td>\
                <td width="30%" class="l">' + customerInfo.FirstName + '</td>\
                <td width="20%" class="r">Last Name</td>\
                <td width="30%" class="l">' + customerInfo.LastName + '</td>\
            </tr>\
            <tr>\
                <td class="r">Phone</td>\
                <td class="l">' + customerInfo.Phone + '</td>\
                <td class="r">Mobile Phone</td>\
                <td class="l">' + customerInfo.MobilePhone + '</td>\
            </tr>\
            <tr>\
                <td class="r">Fax</td>\
                <td class="l">' + customerInfo.Fax + '</td>\
                <td></td>\
                <td></td>\
            </tr>\
            </tbody></table>';
    },
    initCopyAllPT: function (data) {
        var html = '<br style="clear:both;" /><div class="l"><input type="checkbox" onclick="minMaxPTRacingSetting.chkCopyAllSetting(this)" id="chkCopyAllSetting">&nbsp;<label id="lblCopyAllSetting" for="chkCopyAllSetting">Copy all \
            Position Taking from ' + data.DefaultRacingSportType + ' - Win</label></div>';
        if (!data.IsHideCopyBetSetting) {
            html += '<div class="l"><input type="checkbox" onclick="minMaxPTRacingSetting.chkCopyAllBetSetting(this)" id="chkCopyAllBetting">&nbsp;<b>Copy all Bet Setting from ' + data.DefaultRacingSportType + ' - Win</b></div>';
        }
        html += '<div class="l"></div>';
        return html;
    },
    initRacingSportType: function (racingBox) {
        if (!racingBox || racingBox.length === 0) {
            return '';
        }
        var html = '';
        $(racingBox).each(function (index, sportType) {
            html += '<div class="racingSection ' + (!sportType.Visible ? 'hidden' : '') + '"><table class="tblcollapse w1000 marginT6"><tbody>\
            <tr class="title">\
                <td class="l"><span class="expanded" target="setting_' + sportType.Id + '">' + sportType.Name + '</span></td>\
            </tr>';
            if (sportType.Totes && sportType.Totes.length > 0) {
                html += '<tr id="setting_' + sportType.Id + '"><td class="l"><div>';
                $(sportType.Totes).each(function (ind, betType) {
                    html += minMaxPTRacingSetting.initRacingBetType(sportType.Id, betType);
                });
                html += '</div></td></tr>';
            }
            html += '</tbody></table></div>';
        });
        return html;
    },
    initRacingBetType: function (sportId, racingBetType) {
        var html = '<div class="' + (!racingBetType.Visible ? 'hidden' : '') + '"><table cellspacing="0" cellpadding="0" border="0" class="tblPTheader"><tbody>\
            <tr id="btntop" style="cursor: pointer;">\
                <td width="120" class="l btnmiddle" target="setting_' + racingBetType.Id + '" >\
                    <div class="mp">' + racingBetType.Name + '</div>\
                </td>\
                <td width="20" class="hidden setting_' + racingBetType.Id + ' r ' + (racingBetType.IsTop ? 'md2' : 'md3') + '"></td>\
            </tr></tbody></table>\
            <div id="' + racingBetType.Id + '_errmsg" class="w800 errmsg"></div>';
        if (racingBetType.Bettypes && racingBetType.Bettypes.length > 0) {
            html += '<div id="setting_' + racingBetType.Id + '" class="l" style="' + (!racingBetType.IsTop ? 'display:none;' : '') + '">';
            if (window.targetRoleId === minMaxPTRacingSetting.roleId.masterMinMax) {
                html += masterMinMaxPT.initMinMaxPTSetting(sportId, racingBetType);
            }
            else if (window.targetRoleId === minMaxPTRacingSetting.roleId.agentMinMax) {
                html += agentMinMaxPT.initMinMaxPTSetting(sportId, racingBetType);
            }
            html += '</div>';
        }
        html += '</div>'
        return html;
    },
    getRoleNameById: function (roleId) {
        var roleName = '';
        if (roleId === minMaxPTRacingSetting.roleId.master) {
            roleName = 'Master';
        }
        else if (roleId === minMaxPTRacingSetting.roleId.agent) {
            roleName = 'Agent';
        }
        return roleName;
    },
    // events handling
    chkCopyAllSetting: function (checkbox) {
        var isCopied = $(checkbox).prop('checked');
        var sportId;
        var winId;
        var placeId;
        var chkCopyFromWin = 'chkCopyFromWin';
        for (var i = 0; i < this.currentSetting.RacingSportType.length; i++) {
            sportId = this.currentSetting.RacingSportType[i].Id;
            for (var j = 0; j < this.currentSetting.RacingSportType[i].Totes.length; j++) {
                var obj = this.currentSetting.RacingSportType[i].Totes[j];
                winId = sportId + '_' + obj.Bettypes[0].Id;
                placeId = sportId + '_' + obj.Bettypes[1].Id;
                if (obj.Bettypes.length > 0 && (winId == this.currentSetting.DefaultRacingSportTypeId)) {
                    if (isCopied) {
                        $('#' + placeId + '_minpt').attr('disabled', 'disabled');
                        $('#' + placeId + '_maxpt').attr('disabled', 'disabled');
                        $('#' + winId + '_' + chkCopyFromWin).attr('disabled', 'disabled');
                    }
                    else {
                        $('#' + winId + '_' + chkCopyFromWin).removeAttr('disabled');

                        if ($('#' + winId + '_' + chkCopyFromWin).prop('checked') === false) {
                            $('#' + placeId + '_minpt').removeAttr('disabled');
                            $('#' + placeId + '_maxpt').removeAttr('disabled');
                        }
                    }
                }
                else {
                    if (isCopied) {
                        $('#' + winId + '_' + chkCopyFromWin).attr('disabled', 'disabled');
                        $('#' + winId + '_minpt').attr('disabled', 'disabled');
                        $('#' + placeId + '_minpt').attr('disabled', 'disabled');
                        $('#' + winId + '_maxpt').attr('disabled', 'disabled');
                        $('#' + placeId + '_maxpt').attr('disabled', 'disabled');
                    }
                    else {
                        $('#' + winId + '_' + chkCopyFromWin).removeAttr('disabled');
                        $('#' + winId + '_minpt').removeAttr('disabled');
                        $('#' + winId + '_maxpt').removeAttr('disabled');

                        if ($('#' + winId + '_' + chkCopyFromWin).prop('checked') === false) {
                            $('#' + placeId + '_minpt').removeAttr('disabled');
                            $('#' + placeId + '_maxpt').removeAttr('disabled');
                        }
                    }
                }
            }
        }
    },
    chkCopyAllBetSetting: function (container) {
        var isCopied = $(container).prop('checked');
        var sportId;
        var winId;
        var placeId;

        for (var i = 0; i < this.currentSetting.RacingSportType.length; i++) {
            sportId = this.currentSetting.RacingSportType[i].Id;
            for (var j = 0; j < this.currentSetting.RacingSportType[i].Totes.length; j++) {
                var obj = this.currentSetting.RacingSportType[i].Totes[j];
                winId = sportId + '_' + obj.Bettypes[0].Id;
                placeId = sportId + '_' + obj.Bettypes[1].Id;
                if (obj.Bettypes.length > 0 && (winId == this.currentSetting.DefaultRacingSportTypeId)) {
                    if (isCopied) {
                        $('#' + winId + '_' + txtMinbet).attr('disabled', 'disabled');
                        $('#' + winId + '_' + txtMaxbet).attr('disabled', 'disabled');
                        $('#' + winId + '_' + txtMaxPerMatch).attr('disabled', 'disabled');
                    }
                    else {
                        $('#' + placeId + '_' + txtMinbet).removeAttr('disabled');
                        $('#' + placeId + '_' + txtMaxbet).removeAttr('disabled');
                        $('#' + placeId + '_' + txtMaxPerMatch).removeAttr('disabled');
                    }
                }
            }
        }
    },
    chkCopyFromWin: function (obj) {
        var IdWin = $(obj).prop('id').split('_')[1];
        var IdPlace = $(obj).prop('id').split('_')[0] + '_' + minMaxPTRacingSetting.getIdPlace(IdWin);

        var isCopied = $(obj).prop('checked');
        if (isCopied) {
            $('#' + IdPlace + '_minpt').attr('disabled', 'disabled');
            $('#' + IdPlace + '_maxpt').attr('disabled', 'disabled');
        }
        else {
            $('#' + IdPlace + '_minpt').removeAttr('disabled');
            $('#' + IdPlace + '_maxpt').removeAttr('disabled');
        }
    },
    changeMaxPT: function (obj) {
        var IdWin = $(obj).prop('id').split('_')[0] + '_' + $(obj).prop('id').split('_')[1];
        var max = $(obj).val();
        var master_obj = $('#' + IdWin + '_' + 'minpt');

        max = Math.min(master_obj.attr('max'), max)
        var curr = Math.min(master_obj.val(), max);
        CreateOption(master_obj, curr, max, 0, 0.1, 2);
    },
    changeMinPT: function (obj) {
        var IdWin = $(obj).attr('id').split('_')[0] + '_' + $(obj).attr('id').split('_')[1];
        var min_of_max = $(obj).val();
        var max_obj = $('#' + IdWin + '_' + 'maxpt');
        var max_of_max = max_obj.attr('max');
        var cur_of_max = max_obj.val();
        CreateOption(max_obj, cur_of_max, max_of_max, min_of_max, 0.1, 2);
    },
    // helper methods
    getIdPlace: function (IdWin) {
        return parseInt(IdWin) + 1;
    },
    getId: function (index) {
        return this.parent.parent.data.Id + '_' + this.data.Bettypes[index].Id;
    },
    setParamUpdatePT: function () {
        // const
        var chkCopyAllSetting = 'chkCopyAllSetting';
        var chkCopyFromWin = 'chkCopyFromWin';
        // parameters
        var bettype, tote, sport;
        var id, status151_41 = { Enable: false, EnableDownline: false, UpdateDisable: false };
        var isCopyAll = $('#' + chkCopyAllSetting).prop('checked');
        var isCopyfromWin;
        var pt, uplinept, ptUK, uplineptUK;

        for (var i = 0; i < this.currentSetting.RacingSportType.length; i++) {
            sport = this.currentSetting.RacingSportType[i];
            for (var j = 0; j < sport.Totes.length; j++) {
                tote = sport.Totes[j];
                //set deafault update PT
                tote.UpdatePT = false;
                isCopyfromWin = $('#' + tote.Id + '_' + chkCopyFromWin).prop('checked');
                for (var k = 0; k < tote.Bettypes.length; k++) {
                    bettype = tote.Bettypes[k];
                    id = sport.Id + '_' + bettype.Id;

                    if (i == 0 && j == 0 && k == 0) {
                        pt = $('#' + id + '_minpt').val();
                        uplinept = $('#' + id + '_maxpt').val();
                    }
                    if (isCopyAll) {
                        if (!this.isSATote(tote)) {
                            bettype.PositionTaking.Pt = pt;
                            bettype.PositionTaking.UplinePt = uplinept;
                        }
                        tote.UpdatePT = true;
                    }
                    else {
                        if (k == 0) {
                            ptUK = $('#' + id + '_minpt').val();
                            uplineptUK = $('#' + id + '_maxpt').val();
                        }

                        if (isCopyfromWin) {
                            bettype.PositionTaking.Pt = ptUK;
                            bettype.PositionTaking.UplinePt = uplineptUK;
                            tote.UpdatePT = true;
                        }
                        else {
                            bettype.PositionTaking.Pt = $('#' + id + '_minpt').val();
                            bettype.PositionTaking.UplinePt = $('#' + id + '_maxpt').val();
                            // check is update master min/max
                            var oldmin = $('#' + id + '_minpt').attr('old');
                            var oldmax = $('#' + id + '_maxpt').attr('old');
                            if (bettype.PositionTaking.Pt != oldmin || bettype.PositionTaking.UplinePt != oldmax) {
                                tote.UpdatePT = true;
                            }
                        }
                    }

                    // set racing 151
                    if (i == 0 && j == 0) {
                        sport151.setBettype(bettype);
                    }
                    // check racing is hidden 
                    if (!sport.Visible || !tote.Visible) {
                        if (!sport.Visible) { sportHidden.addSport(sport); }
                        bettype = sport151.getBettype(bettype);
                    }
                }
                // set comm racing 151
                if (i == 0 && j == 0) {
                    tote151.setComm(tote);
                }
                // check copy comm to hidden tote
                if (!sport.Visible || !tote.Visible) {
                    tote.Comm = tote151.getComm(tote);
                    tote.HousePt = tote151.getHousePt(tote);
                }
            }
        }
    },
    isSATote: function (tote) {
        if (typeof tote != 'undefined') {
            if (!tote.Visible && (tote.Id.toString().indexOf('31') != -1 || tote.Id.toString().indexOf('32') != -1)) {
                return true;
            }
        }
        return false;
    }
};
// sport 151
var sport151 = {
    bettype: [{ 41: null }, { 42: null}],
    setBettype: function (bettype) {
        var _bettype_id = bettype.Id;
        var _bettype_name = (_bettype_id % 2 != 0) ? 'win' : 'place';
        switch (_bettype_name) {
            case 'win': //41
                this.bettype[41] = bettype;
                break;
            case 'place': //42
                this.bettype[42] = bettype;
                break;
            default:
                break;
        }
        return false;
    },
    getBettype: function (bettype) {
        var _bettype_id = bettype.Id;
        // not check hidden by curency : SA Tote
        if (_bettype_id == 31 || _bettype_id == 32) { return bettype; }
        var _bettype_name = (_bettype_id % 2 != 0) ? 'win' : 'place';
        switch (_bettype_name) {
            case 'win': //41
                bettype = $.extend(bettype, this.bettype[41]);
                break;
            case 'place': //42
                bettype = $.extend(bettype, this.bettype[42]);
                break;
            default:
                break;
        }
        bettype.Id = _bettype_id;
        return bettype;
    }
};
// tote 151
var tote151 = {
    tote: { Comm: 0, HousePt: 0 },
    setComm: function (tote) {
        if (typeof tote != 'undefined') {
            this.tote.Comm = tote.Comm;
            this.tote.HousePt = tote.HousePt;
        }
        return false;
    },
    getComm: function (tote) {
        // not check hidden by curency : SA Tote
        if (tote.Id.indexOf('31') != -1 || tote.Id.indexOf('32') != -1) { return 0; }
        return this.tote.Comm;
    },
    getHousePt: function (tote) {
        // not check hidden by curency : SA Tote
        if (tote.Id.indexOf('31') != -1 || tote.Id.indexOf('32') != -1) { return 0; }
        return this.tote.HousePt;
    }
};
// sport hidden
var sportHidden = {
    sports: [],
    addSport: function (sport) {
        if ($.inArray(sport.Name, this.sports) == -1)
            this.sports.push(sport.Name);
        return false;
    }
};
$(function () {
    minMaxPTRacingSetting.initialize();
});