function PTChanging(el) {
    var sel = el.value;
    if ($("pt_3")) {
        var cur = $("pt_3").value;
        PT_RenderCombo($("pt_3"), _page.MaxPT_Min, _page.MaxPT_Max - sel, 0.01, cur);
    }
    if ($("pt_4")) {
        PT_RenderCombo2($("pt_4"), _page.MinPT_Min, _page.MinPT_Max - sel, 0.01);
    }
}
//=========================================================================================================================
window.onload = function () {
    flag = _page.oldAuto;
    setEventForCheckbox();

    if (_page.uncheck && _page.custRoleId == 1) {
        $('warningMsg').innerHTML = _page.uncheckMsg;
        $('warningMsg').style.display = 'block';
    }

    if (_page.showWinLimit) {
        taskCount += 1;
    }
}

function PT_RenderCombo(combo, min, max, step, selectedValue) {
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

        if (selectedValue && parseFloat(selectedValue) == parseFloat(f))
            index = itemIndex;

        opt.text = opt.value = f;
        combo.options.add(opt);
        itemIndex++;
    }

    if (selectedValue) {
        combo.options[index].selected = true;
    }
}

function PT_RenderCombo2(combo, min, max, step) {
    var index = 0;
    var itemIndex = 0;
    var selectedValue = combo.value;

    if (!combo) return;
    combo.options.length = 0;

    if (combo.id == 'pt_4')
        max = $('pt_3').value;

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

        if (parseFloat(selectedValue) == parseFloat(f))
            index = itemIndex;

        opt.text = opt.value = f;
        combo.options.add(opt);
        itemIndex++;
    }

    if (document.all)
        combo.options[index].selected = true;
    else
        combo.value = selectedValue;
}

function onchangeCheckbox() {
    for (var i = 0; i < _page.gameRows; i++) {
        var uncheckCount = 0;
        var items = document.getElementsByName('gameRow' + i);
        if (items[0].type == 'checkbox') {
            for (var j = 0; j < items.length; j++) {
                if (!items[j].checked)
                    uncheckCount++;
            }
        }

        if (uncheckCount == items.length) {
            $('warningMsg').innerHTML = _page.warningMsg;
            $('warningMsg').style.display = 'block';
            break;
        }
        else {
            $('warningMsg').style.display = 'none';
        }
    }
}

function setEventForCheckbox() {
    for (var i = 0; i < _page.gameRows; i++) {
        var items = document.getElementsByName('gameRow' + i);
        if (items[0].type == 'checkbox') {
            for (var j = 0; j < items.length; j++) {
                items[j].onclick = onchangeCheckbox;
            }
        }
    }
}

function updatePT() {
    if (_page.roleId == 4 && _page.custRoleId == 3)         //Super edits master
        editMasterPTComm2();
    else if (_page.roleId == 4 && _page.custRoleId == 1)    //Super edits member
        editMemberPTCommBySuper();
    else if (_page.roleId == 3 && _page.custRoleId == 2)    //Master edits agent
        editAgentPTComm2();
    else if (_page.roleId == 3 && _page.custRoleId == 1)    //Master edits member
        editMemberPTCommByMaster();
    else if (_page.roleId == 2 && _page.custRoleId == 1)    //Agents edits member
        editMemberPTComm2();
    else                                                    //Invalid request
        alert(_page.noAccess);
}


function getGameSetting() {
    var pars = '';

    //pars = [{1^1101, 'Baccarat', (1^ '10-100'^ true$ 2^ '20-200'^ false)}; {2^1102, 'Sicbo', (3^ '20-200'^ false$ 4^ '20-200'^ false)}]
    for (var i = 0; i < _page.gameRows; i++) {
        var betting = '';
        var betID = $('betTypeID' + i).innerHTML;
        var gameTypeID = $('gameTypeID' + i).innerHTML;
        var gameTypeName = $('gameType' + i).innerHTML;
        var bets = document.getElementsByName('gameRow' + i);

        for (var j = 0; j < bets.length; j++) {
            var id = bets[j].id.split('_')[1];
            var name = document.getElementById('innerText' + id).innerHTML;
            var checked = bets[j].checked;

            betting += id + "^" + name + "^" + checked + '$';
        }

        betting = betting.substring(0, betting.length - 1);

        pars += '{' + gameTypeID + '|' + betID + "|" + gameTypeName + "|(" + betting + ')};';
    }

    pars = '[' + pars.substring(0, pars.length - 1) + ']';

    return pars;
}

function saveMemberWinLimit() {
    initData();

    var url = '../Handlers/MemberWinLimitHandler.ashx';

    url = SetParameterValue('custids', _page.isMult ? $('custids').innerHTML : _page.custId, url);
    url = SetParameterValue('roleid', _page.custRoleId, url);
    url = SetParameterValue('submit', submit, url);
    url = SetParameterValue('isDailyReset', $('chkAutoReset').checked, url);
    url = SetParameterValue('maxWinLimit', $('comm_0').value, url);

    jx.request(
    url,
    function (res) {
        try {
            var ret = eval('(' + res.responseText + ')');
            ret = ret.errMsg;
            var users = userNames;
            var codes = ret.split('|')[0].split('$');
            var msgs = ret.split('|')[1].split('$');

            for (var i = 0; i < codes.length; i++) {
                top.userNames.push(users[i]);
                top.errCodes.push(codes[i] == 0 ? -1 : 1);
                top.errMsgs.push(msgs[i]);
            }
        } catch (ex) { }

        if (submit == true) {
            finish1Task++;
        }
        else {
            showErrMsg();
        }
    },
    'post',
    '',
    true
    );
}

function viewCurrentWinLimit() {
    initData();

    var url = '../Handlers/CurrentMemberWinLimitHandler.ashx';

    url = SetParameterValue('custids', _page.isMult ? $('custids').innerHTML : _page.custId, url);
    url = SetParameterValue('roleid', _page.custRoleId, url);
    url = SetParameterValue('submit', submit, url);

    jx.request(
    url,
    function (res) {
        try {
            var ret = eval('(' + res.responseText + ')');
            ret = ret.errMsg;

            $('winLimitTitle').innerHTML = _page.currentWinLimit;
            $('currentWinLimit').innerHTML = '';
            $('winLimitValue').innerHTML = ret;
        } catch (ex) { }

        if (submit == true) {
            finish1Task++;
        }
        else {
            showErrMsg();
        }
    },
    'post',
    '',
    true
    );
}
