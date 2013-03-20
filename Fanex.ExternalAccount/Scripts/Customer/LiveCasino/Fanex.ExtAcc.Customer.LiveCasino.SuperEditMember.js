/*
Super edits member
*/
function editMemberPTCommBySuper() {
    var upPreset = $('pt_1');
    var upAutoPreset = $('pt_2');
    var isAuto = $('chkAutoPT') && $('chkAutoPT').checked;

    var url = '../Handlers/Super/MemberPTComm.ashx';

    url = SetParameterValue('custids', _page.isMult ? $('custids').innerHTML : _page.custId, url);
    url = SetParameterValue('roleid', _page.custRoleId, url);
    url = SetParameterValue('submit', submit, url);

    if (upPreset != null)
        url = SetParameterValue('presetPT', isAuto ? upPreset.options[upPreset.options.length - 1].value : upPreset.options[upPreset.selectedIndex].value, url);
    else
        url = SetParameterValue('presetPT', $('presetPT').innerHTML, url);

    if (upAutoPreset)
        url = SetParameterValue('autoPresetPT', isAuto ? upAutoPreset.options[upAutoPreset.selectedIndex].value : 0.0, url);
    if ($('chkAutoPT'))
        url = SetParameterValue('isAutoPresetPT', isAuto, url);

    var pars = getGameSetting();

    url = SetParameterValue('pars', pars, url);

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

function editMemberBySuper() {
    editMemberPTCommBySuper();
    editGameSetting();
}


function editMemberPTComm2() {
    var upPreset = $('pt_1');
    var comm = $('comm_2');

    var url = '../Handlers/Agent/MemberPTComm.ashx';

    url = SetParameterValue('custids', _page.isMult ? $('custids').innerHTML : _page.custId, url);
    url = SetParameterValue('roleid', _page.custRoleId, url);
    url = SetParameterValue('submit', submit, url);

    url = SetParameterValue('presetPT', upPreset.options[upPreset.selectedIndex].value, url);
    url = SetParameterValue('comm', comm.options[comm.selectedIndex].value, url);
    url = SetParameterValue('enable', $('bettype_chkEnableLiveCS').checked, url);
    url = SetParameterValue('disable', $('bettype_chkEnableLiveCS').disabled, url);

    var pars = getGameSetting();

    url = SetParameterValue('pars', pars, url);

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

            setTimeout('editGameSetting()', 2000);
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
