/// <reference path="../_references.js" />

if (typeof Fanex == 'undefined' || typeof Fanex.ExtAcc == 'undefined') {
    alert("Fanex.ExtAcc.js module is required!");
};

Fanex.ExtAcc.Customer.LiveCasino = {
    IsMultiple: Fanex.ExtAcc.Customer.IsMultiple,
    customer: null,

    Init: function () {
        if (Fanex.ExtAcc.Customer.CurrentLevel == Fanex.ExtAcc.Customer.Master) {
            this.GetMasterSetting();
        }
        else if (Fanex.ExtAcc.Customer.CurrentLevel == Fanex.ExtAcc.Customer.Agent) {
            this.GetAgentSetting();
        }
        else {
            if (Fanex.ExtAcc.Customer.isSuperEditMember == true) {
                this.SuperGetMemberSetting();
            }
            else if (Fanex.ExtAcc.Customer.isMasterEditMember == true) {
                this.MasterGetMemberSetting();
            }
            else {
                this.GetMemberSetting();
            }
        }
    },

    GetMasterSetting: function () {
        Fanex.ExtAcc.BlockUI();

        var data = new Object();
        data.custId = Fanex.ExtAcc.Customer.UserId;
        data.uplineId = Fanex.ExtAcc.Customer.superId;

        $.ajax({
            url: resolveClientUrl('LiveCasino/EditMasterPost'),
            dataType: 'json',
            type: 'post',
            data: data
        })
        .success(function (data) {
            if (typeof console != 'undefined') console.log(data);

            Fanex.ExtAcc.Customer.LiveCasino.customer = data;

            //PT Renderring
            //PT values
            var uplineMin = data.uplinePt.minPT;
            var uplineMax = data.uplinePt.maxPT;
            var uplinePreset = data.uplinePt.presetPT;
            var uplineAutoPreset = data.uplinePt.autoPresetPT;

            var max = data.pt.maxPT;

            //Render super preset pt
            PT_RenderCombo(document.getElementById('pt_1'), uplineMin, uplineMax, 0.01, uplinePreset);

            //Render super auto preset pt
            PT_RenderCombo(document.getElementById('pt_2'), uplineMin, uplineMax, 0.01, uplineAutoPreset);
            if (!data.uplinePt.isAutoPresetPT) {
                $('#pt_2').attr('disabled', 'disabled');
            }
            else {
                $('#pt_1').attr('disabled', 'disabled');
            }

            $('#chkAutoPT').attr('checked', data.uplinePt.isAutoPresetPT);

            //Render Master Max PT
            PT_RenderCombo(document.getElementById('pt_3'), 0, (uplineMax - uplinePreset), 0.01, max);

            //Render Master Min PT
            PT_RenderCombo(document.getElementById('pt_4'), 0, data.pt.maxPT, 0.01, data.pt.minPT);

            //Render Master Commission
            Comm_RenderCombo(document.getElementById('comm_5'), 0, data.uplinePt.comm, 0.0005, data.pt.comm);

            //Render member win limit block
            Fanex.ExtAcc.Customer.LiveCasino.RenderMemberWinLimit(data);

            //Render game setting
            Fanex.ExtAcc.Customer.LiveCasino.RenderGameSetting(data);

            checkMinMaxPT();

            Fanex.ExtAcc.Customer.LiveCasino.LiveCasinoModel = function () {
                this.saveCustomer = function () {
                }
            };

            ko.applyBindings(new Fanex.ExtAcc.Customer.LiveCasino.LiveCasinoModel());

            $.unblockUI(document);
        })
        .error(function (data) {
            if (typeof console != 'undefined') console.log(data.responseText);
            $.unblockUI(document);
        });
    },

    GetAgentSetting: function () {
    },

    GetMemberSetting: function () {
    },

    SuperGetMemberSetting: function () {
    },

    MasterGetMemberSetting: function () {
    },

    UpdateMasterSetting: function () {
    },

    UpdateAgentSetting: function () {
    },

    UpdateMemberSetting: function () {
    },

    SuperUpdateMemberSetting: function () {
    },

    MasterUpdateMemberSetting: function () {
    },

    RenderMemberWinLimit: function (data) {
        var limits = data.userInfo.limit.limits;
        var selectedItem = data.userInfo.limit.currentWinLimit;

        if (data.userInfo.limit.maxWinLimitUpline == 0) {
            for (var i = 0; i < limits.length; i++) {
                var value = limits[i];
                var text = limits[i] + 'X';
                var select = '';

                if (value == selectedItem) {
                    select = 'selected = "selected"';
                }

                if (i == limits.length - 1) {
                    text = 'Unlimited';
                }

                $("#maximumWinLimit").append('<option value="' + value + '" ' + select + '>' + text + '</option>');
            }
        }
        else if (data.userInfo.limit.maxWinLimitUpline == 10) {
            for (var i = 0; i < limits.length - 1; i++) {
                var value = limits[i];
                var text = limits[i] + 'X';
                var select = '';

                if (value == selectedItem) {
                    select = 'selected = "selected"';
                }

                if (i == limits.length - 1) {
                    text = 'Unlimited';
                }

                $("#maximumWinLimit").append('<option value="' + value + '" ' + select + '>' + text + '</option>');
            }
        }
        else if (data.userInfo.limit.maxWinLimitUpline == 5) {
            for (var i = 0; i < limits.length - 2; i++) {
                var value = limits[i];
                var text = limits[i] + 'X';
                var select = '';

                if (value == selectedItem) {
                    select = 'selected = "selected"';
                }

                if (i == limits.length - 1) {
                    text = 'Unlimited';
                }

                $("#maximumWinLimit").append('<option value="' + value + '" ' + select + '>' + text + '</option>');
            }
        }
        else {
            for (var i = 0; i < limits.length - 3; i++) {
                var value = limits[i];
                var text = limits[i] + 'X';
                var select = '';

                if (value == selectedItem) {
                    select = 'selected = "selected"';
                }

                if (i == limits.length - 1) {
                    text = 'Unlimited';
                }

                $("#maximumWinLimit").append('<option value="' + value + '" ' + select + '>' + text + '</option>');
            }
        }

        if (!data.userInfo.limit.isDailyAutoResetUpline) {
            $('#dailyAutoReset').attr('disabled', 'disabled');
        }

        $('#dailyAutoReset').attr('checked', 'checked');
    },

    RenderGameSetting: function (data) {
        var gameSetting = data.gameSetting;

        for (var i = 0; i < gameSetting.length; i++) {
            var bettingLimitItem = '';

            for (var j = 0; j < gameSetting[i].bettingLimit.length; j++) {
                var selected = gameSetting[i].bettingLimit[j].isSelected ? 'checked="checked"' : '';
                bettingLimitItem += '<input style="vertical-align: middle;margin-bottom: 3px;" id="' + gameSetting[i].bettingLimit[j].id + '" type="checkbox" ' + selected + '/><span>' + gameSetting[i].bettingLimit[j].text + '</span>';
            }

            var row = '<div class="customerInfoBlock1" style="margin: 0px">' +
                gameSetting[i].name +
                '</div>' +
                '<div class="customerInfoBlock2" style="width: 675px">' +
                bettingLimitItem +
                '</div>';

            $('#gameSetting').append(row);
        }
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

    if (selectedValue) {
        if (document.all)
            combo.options[index].selected = true;
        else
            combo.value = selectedValue;
    }
}

function Comm_RenderCombo(combo, min, max, step, selectedValue) {
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

function checkPT(el) {
    if (el.checked) {
        if (document.getElementById('pt_1'))
            document.getElementById('pt_1').disabled = true;
        if (document.getElementById('pt_2'))
            document.getElementById('pt_2').disabled = false;

        var pt1 = document.getElementById('pt_1');
        var pt3 = document.getElementById("pt_3");
        var pt4 = document.getElementById("pt_4");

        if (pt3)
            PT_RenderCombo(pt3, 0, (Fanex.ExtAcc.Customer.LiveCasino.customer.uplinePt.maxPT - parseFloat(pt1.options[pt1.selectedIndex].value).toFixed(2)), 0.01, pt3.options[pt3.selectedIndex].value);
        if (pt4)
            PT_RenderCombo(pt4, 0, pt3.options[pt3.selectedIndex].value, 0.01, pt4.options[pt4.selectedIndex].value);
    }
    else {
        if (document.getElementById('pt_1')) {
            document.getElementById('pt_1').disabled = false;
            PTChanging(document.getElementById('pt_1'));
        }
        if (document.getElementById('pt_2'))
            document.getElementById('pt_2').disabled = true;
    }
}

function checkMaxPT() {
    if (document.getElementById('pt_3') && document.getElementById('pt_4')) {
        PT_RenderCombo2(document.getElementById("pt_4"), 0, document.getElementById('pt_3').value, 0.01);
        if (parseFloat(document.getElementById('pt_3').value) < parseFloat(document.getElementById('pt_4').value)) {
            document.getElementById('pt_4').value = document.getElementById('pt_3').value;
        }
    }

    return true;
}

function checkMinMaxPT() {
    if (document.getElementById('pt_3') && document.getElementById('pt_4')) {
        document.getElementById('pt_3').onchange = checkMaxPT;
    }
}

function PTChanging(el) {
    var sel = parseFloat(el.value);
    if (document.getElementById("pt_3")) {
        var cur = parseFloat(document.getElementById("pt_3").value);
        var uplinePT = Fanex.ExtAcc.Customer.LiveCasino.customer.uplinePt;

        if (cur == 'undefined' || cur == 'NaN') {
            cur = 0;
        }

        if (cur > uplinePT.maxPT - sel) {
            cur = uplinePT.maxPT - sel;
        }

        PT_RenderCombo(document.getElementById("pt_3"), 0, uplinePT.maxPT - sel, 0.01, cur != 0 ? parseFloat(cur).toFixed(2) : 0);
    }
    if (document.getElementById("pt_4")) {
        PT_RenderCombo2(document.getElementById("pt_4"), 0, uplinePT.maxPT - sel, 0.01);
    }
}

function PT_RenderCombo2(combo, min, max, step) {
    var index = 0;
    var itemIndex = 0;
    var selectedValue = combo.value;

    if (!combo) return;
    combo.options.length = 0;

    if (combo.id == 'pt_4')
        max = document.getElementById('pt_3').value;

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
            document.getElementById('warningMsg').innerHTML = _page.warningMsg;
            document.getElementById('warningMsg').style.display = 'block';
            break;
        }
        else {
            document.getElementById('warningMsg').style.display = 'none';
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

$(document).ready(function () {
    Fanex.ExtAcc.Customer.LiveCasino.Init();
});