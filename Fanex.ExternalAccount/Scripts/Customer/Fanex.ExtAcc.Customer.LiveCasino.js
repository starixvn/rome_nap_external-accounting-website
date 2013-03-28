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

        // Set events for checkboxes
        var interval = setInterval(function () {
            if (Fanex.ExtAcc.Customer.LiveCasino.customer.gameSetting != null) {
                clearInterval(interval);
                setEventForCheckbox();
            }
        }, 100);
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

            //Assign data for global variable
            Fanex.ExtAcc.Customer.LiveCasino.customer = data;

            //PT Renderring
            //PT values
            var uplineMin = data.superPTSetting.minPT;
            var uplineMax = data.superPTSetting.maxPT;
            var uplinePreset = data.superPTSetting.presetPT;
            var uplineAutoPreset = data.superPTSetting.autoPresetPT;
            var max = data.masterPTSetting.maxPT;

            //Render super preset pt
            PT_RenderCombo(document.getElementById('pt_1'), uplineMin, uplineMax, 0.01, uplinePreset);

            //Render super auto preset pt
            PT_RenderCombo(document.getElementById('pt_2'), uplineMin, uplineMax, 0.01, uplineAutoPreset);
            if (!data.superPTSetting.isAutoPresetPT) {
                $('#pt_2').attr('disabled', 'disabled');
            }
            else {
                $('#pt_1').attr('disabled', 'disabled');
            }
            $('#chkAutoPT').attr('checked', data.superPTSetting.isAutoPresetPT);

            //Render Master Max PT
            PT_RenderCombo(document.getElementById('pt_3'), 0, (uplineMax - uplinePreset), 0.01, max);

            //Render Master Min PT
            PT_RenderCombo(document.getElementById('pt_4'), 0, data.masterPTSetting.maxPT, 0.01, data.masterPTSetting.minPT);

            //Render Master Commission
            Comm_RenderCombo(document.getElementById('comm_5'), 0, data.superPTSetting.comm, 0.0005, data.masterPTSetting.comm);

            //Render member win limit block
            Fanex.ExtAcc.Customer.LiveCasino.RenderMemberWinLimit(data);

            //Render game setting
            Fanex.ExtAcc.Customer.LiveCasino.RenderGameSetting(data);

            //Add event for changing Min/Max PT
            checkMinMaxPT();

            //Model for binding events on form
            Fanex.ExtAcc.Customer.LiveCasino.LiveCasinoModel = function () {
                this.saveCustomer = function () {
                    alert('Submit');
                },

                this.savePtComm = function () {
                    Fanex.ExtAcc.BlockUI();

                    var isChecked = document.getElementById('chkAutoPT').checked;
                    Fanex.ExtAcc.Customer.LiveCasino.customer.superPTSetting.presetPT = !isChecked ? $('#pt_1').val() : 0;
                    Fanex.ExtAcc.Customer.LiveCasino.customer.superPTSetting.isAutoPresetPT = isChecked;
                    Fanex.ExtAcc.Customer.LiveCasino.customer.superPTSetting.autoPresetPT = $('#pt_2').val();
                    Fanex.ExtAcc.Customer.LiveCasino.customer.masterPTSetting.maxPT = $('#pt_3').val();
                    Fanex.ExtAcc.Customer.LiveCasino.customer.masterPTSetting.minPT = $('#pt_4').val();
                    Fanex.ExtAcc.Customer.LiveCasino.customer.masterPTSetting.comm = $('#comm_5').val();

                    var data = {
                        custId: Fanex.ExtAcc.Customer.UserId,
                        uplineId: Fanex.ExtAcc.Customer.superId,
                        customer: JSON.stringify(Fanex.ExtAcc.Customer.LiveCasino.customer)
                    };

                    $.ajax({
                        url: resolveClientUrl('LiveCasino/UpdateMasterPTPost'),
                        data: data,
                        type: 'post',
                        dataType: 'json'
                    })
                    .success(function (result) {
                        var timeout = setTimeout(function () {
                            clearTimeout(timeout);
                            Fanex.ExtAcc.Customer.LiveCasino.RenderPtCommErrorMessage(result);
                            $.unblockUI(document);
                        }, 2000);
                    })
                    .error(function (data) {
                        if (typeof console != 'undefined') {
                            console.log(data);
                        }

                        $.unblockUI(document);
                    })
                },

                this.saveMemberWinLimit = function () {
                    Fanex.ExtAcc.Customer.LiveCasino.UpdateMemberWinLimit();
                },

                this.saveGameSetting = function () {
                    Fanex.ExtAcc.Customer.LiveCasino.UpdateGameSetting(data);
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
        var limits = data.customerInfo.limit.limits;
        var selectedItem = data.customerInfo.limit.maxWinLimit;

        if (data.customerInfo.limit.maxWinLimitUpline == 0) {
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
        else if (data.customerInfo.limit.maxWinLimitUpline == 10) {
            for (var i = 0; i < limits.length - 1; i++) {
                var value = limits[i];
                var text = limits[i] + 'X';
                var select = '';

                if (value == selectedItem) {
                    select = 'selected = "selected"';
                }

                $("#maximumWinLimit").append('<option value="' + value + '" ' + select + '>' + text + '</option>');
            }
        }
        else if (data.customerInfo.limit.maxWinLimitUpline == 5) {
            for (var i = 0; i < limits.length - 2; i++) {
                var value = limits[i];
                var text = limits[i] + 'X';
                var select = '';

                if (value == selectedItem) {
                    select = 'selected = "selected"';
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

                $("#maximumWinLimit").append('<option value="' + value + '" ' + select + '>' + text + '</option>');
            }
        }

        if (!data.customerInfo.limit.isDailyAutoResetUpline) {
            $('#dailyAutoReset').attr('disabled', 'disabled');
        }

        $('#dailyAutoReset').attr('checked', data.customerInfo.limit.isDailyAutoReset);
    },

    UpdateMemberWinLimit: function () {
        Fanex.ExtAcc.BlockUI();

        Fanex.ExtAcc.Customer.LiveCasino.customer.customerInfo.limit.maxWinLimit = $('#maximumWinLimit').val();
        Fanex.ExtAcc.Customer.LiveCasino.customer.customerInfo.limit.isDailyAutoReset = document.getElementById('dailyAutoReset').checked;

        var data = {
            custId: Fanex.ExtAcc.Customer.UserId,
            isDailyAutoReset: document.getElementById('dailyAutoReset').checked,
            maxWinLimit: $('#maximumWinLimit').val()
        };

        $.ajax({
            url: resolveClientUrl('LiveCasino/UpdateMemberWinLimit'),
            type: 'post',
            dataType: 'json',
            data: data
        })
        .success(function (result) {
            var timeout = setTimeout(function () {
                clearTimeout(timeout);
                var error = '<font color="Blue">Member Win Limit: Updated successfully</font>';

                if (result.ErrorCode != "0") {
                    error = result.ErrorMessage.formatErrorMessage();
                }

                $('#errMsg').css('display', 'block');
                $('#errMsg').html(error);
                $.unblockUI(document);
            }, 2000);
        })
        .error(function (data) {
            if (typeof console != undefined) {
                console.log(data);
            }

            $.unblockUI(document);
        });
    },

    RenderGameSetting: function (data) {
        var gameSetting = data.gameSetting;
        var type = Fanex.ExtAcc.Customer.CurrentLevel == Fanex.ExtAcc.Customer.Member ? 'radio' : 'checkbox';

        for (var i = 0; i < gameSetting.length; i++) {
            var bettingLimitItem = '';

            for (var j = 0; j < gameSetting[i].bettingLimit.length; j++) {
                var selected = gameSetting[i].bettingLimit[j].isSelected ? 'checked="checked"' : '';
                bettingLimitItem += '<input name="gameRow' + i + '" style="vertical-align: middle;margin-bottom: 3px;" id="' + gameSetting[i].bettingLimit[j].id + '" type="' + type + '" ' + selected + '/><span>' + gameSetting[i].bettingLimit[j].text + '</span>';
            }

            var row = '<div class="customerInfoBlock1" style="margin: 0px">' +
                gameSetting[i].name +
                '</div>' +
                '<div class="customerInfoBlock2" style="width: 675px">' +
                bettingLimitItem +
                '</div>';

            $('#gameSetting').append(row);
        }
    },

    UpdateGameSetting: function (data) {
        Fanex.ExtAcc.BlockUI();

        var gameSettings = Fanex.ExtAcc.Customer.LiveCasino.customer.gameSetting;
        Fanex.ExtAcc.Customer.LiveCasino.customer.customerInfo.groupName = $('#selectedGroupName').html();

        for (var i = 0; i < gameSettings.length; i++) {
            var game = gameSettings[i];
            var limits = game.bettingLimit;
            var items = document.getElementsByName('gameRow' + i);

            for (var j = 0; j < limits.length; j++) {
                var limit = limits[j];

                limit.isSelected = items[j].checked;
            }
        }

        var data = {
            uplineRoleId: Fanex.ExtAcc.Customer.CurrentLevel + 1,
            custId: Fanex.ExtAcc.Customer.UserId,
            customer: JSON.stringify(Fanex.ExtAcc.Customer.LiveCasino.customer)
        };

        $.ajax({
            url: resolveClientUrl('LiveCasino/UpdateGameSetting'),
            data: data,
            type: 'post',
            dataType: 'json'
        })
        .success(function (result) {
            var timeout = setTimeout(function () {
                clearTimeout(timeout);

                var error = '';

                for (var i = 0; i < result.length; i++) {
                    if (parseInt(result[i].ErrorCode) != 0) {
                        error += result[i].ErrorMessage.formatErrorMessage() + '<br />';
                    }
                }

                $('#errMsg').show();
                $('#errMsg').html(error != '' ? '<font color="Red">Game Setting: </font>' + error : '<font color="Blue">Game Setting: Updated successfully</font><br />');

                $.unblockUI(document);
            }, 2000);
        })
        .error(function (data) {
            if (typeof console != 'undefined') {
                console.log(data);
            }

            $.unblockUI(document);
        })
    },

    RenderPtCommErrorMessage: function (data) {
        var codes = data.ErrorCode.split('\u0007');
        var msgs = data.ErrorMessage.split('\u0007');
        var error = '';

        for (var i = 0; i < codes.length - 1; i++) {
            var code = parseInt(codes[i]);
            var updatePart = '';

            switch (code) {
                case -101:
                    updatePart = 'Commission: ';
                    break;
                case -102:
                    updatePart = 'Min Position Taking: ';
                    break;
                case -103:
                case 100:
                case 101:
                case 102:
                    updatePart = 'Position Taking: ';
                    break;
                default:
                    break;
            }

            if (code > 0) {
                error += '<font color="Red">' + updatePart + '</font>' + msgs[i].formatErrorMessage() + '<br/>';
            }
            else {
                error += '<font color="Blue">' + updatePart + 'Updated successfully</font><br/>';
            }
        }

        $('#errMsg').css('display', 'block');
        $('#errMsg').html(error == '' ? '<font color="Blue">Position Taking/Commission: Updated successfully</font>' : error);
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
            PT_RenderCombo(pt3, 0, (Fanex.ExtAcc.Customer.LiveCasino.customer.superPTSetting.maxPT - parseFloat(pt1.options[pt1.selectedIndex].value).toFixed(2)), 0.01, pt3.options[pt3.selectedIndex].value);
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
        var uplinePT = Fanex.ExtAcc.Customer.LiveCasino.customer.superPTSetting;

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
    var gameRows = Fanex.ExtAcc.Customer.LiveCasino.customer.gameSetting.length;
    for (var i = 0; i < gameRows; i++) {
        var uncheckCount = 0;
        var items = document.getElementsByName('gameRow' + i);
        if (items[0].type == 'checkbox') {
            for (var j = 0; j < items.length; j++) {
                if (!items[j].checked)
                    uncheckCount++;
            }
        }

        if (uncheckCount == items.length) {
            document.getElementById('warningMsg').innerHTML = '<font color="Red">Unchecking all stake levels will affect and apply to all downlines</font>';
            document.getElementById('warningMsg').style.display = 'block';

            break;
        }
        else {
            document.getElementById('warningMsg').style.display = 'none';
        }
    }
}

function setEventForCheckbox() {
    var gameRows = Fanex.ExtAcc.Customer.LiveCasino.customer.gameSetting.length;
    for (var i = 0; i < gameRows; i++) {
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