/// <reference path="../_references.js" />

if (typeof Fanex == 'undefined' || typeof Fanex.ExtAcc == 'undefined') {
    alert("Fanex.ExtAcc.js module is required!");
};

Fanex.ExtAcc.Customer.LiveCasino = {
    IsMultiple: Fanex.ExtAcc.Customer.IsMultiple,
    IsUncheckWinLimitMember: true,
    customer: null,
    CustIdIndex: 0,
    Error: '',

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
        var timeout = setTimeout(function () {
            clearTimeout(timeout);
            if (Fanex.ExtAcc.Customer.LiveCasino.customer != null && Fanex.ExtAcc.Customer.LiveCasino.customer.gameSetting != null) {
                setEventForCheckbox();
            }
        }, 2000);
    },

    GetMasterSetting: function () {
        Fanex.ExtAcc.BlockUI();

        if (!Fanex.ExtAcc.Customer.LiveCasino.IsMultiple) {
            $('#arrowUpDown').remove();
        }

        if (Fanex.ExtAcc.Customer.LiveCasino.IsMultiple) {
            $('#custInfo').remove();

            $('#arrowUpDown').bind('click', function () {
                Fanex.ExtAcc.Customer.ToogleUsernames();
            });

            $('#names').css('display', 'block');

            if (Fanex.ExtAcc.Customer.Usernames.length < 98) {
                $('#names').css('cursor', 'default');
                $('#names').html(Fanex.ExtAcc.Customer.Usernames.substring(0, Fanex.ExtAcc.Customer.Usernames.length - 2));
                $('#arrowUpDown').css('display', 'none');
            }
            else {
                $('#names').css('cursor', 'pointer');
                $('#names').bind('click', function () {
                    Fanex.ExtAcc.Customer.ToogleUsernames();
                });
                $('#names').html(Fanex.ExtAcc.Customer.Usernames.substring(0, 98) + '...');
                $('#names').attr('title', Fanex.ExtAcc.Customer.Usernames.substring(0, Fanex.ExtAcc.Customer.Usernames.length - 2));
                $('#arrowUpDown').css('display', 'block');
            }
        }

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
            //Assign data for global variable
            Fanex.ExtAcc.Customer.LiveCasino.customer = data;

            //PT Renderring
            //PT values
            var uplineMin = data.uplinePtSetting.minPT;
            var uplineMax = data.uplinePtSetting.maxPT;
            var uplinePreset = data.uplinePtSetting.presetPT;
            var uplineAutoPreset = data.uplinePtSetting.autoPresetPT;
            var max = data.ptSetting.maxPT;

            //Render super preset pt
            PT_RenderCombo(document.getElementById('pt_1'), uplineMin, uplineMax, 0.01, uplinePreset);

            //Render super auto preset pt
            PT_RenderCombo(document.getElementById('pt_2'), uplineMin, uplineMax, 0.01, uplineAutoPreset);
            if (!data.uplinePtSetting.isAutoPresetPT) {
                $('#pt_2').attr('disabled', 'disabled');
            }
            else {
                $('#pt_1').attr('disabled', 'disabled');
            }
            $('#chkAutoPT').attr('checked', data.uplinePtSetting.isAutoPresetPT);

            //Render Master Max PT
            PT_RenderCombo(document.getElementById('pt_3'), 0, (uplineMax - uplinePreset), 0.01, max);

            //Render Master Min PT
            PT_RenderCombo(document.getElementById('pt_4'), 0, data.ptSetting.maxPT, 0.01, data.ptSetting.minPT);

            //Render Master Commission
            Comm_RenderCombo(document.getElementById('comm_5'), 0, data.uplinePtSetting.comm, 0.0005, data.ptSetting.comm);

            //Render member win limit block
            Fanex.ExtAcc.Customer.LiveCasino.RenderMemberWinLimit(data);

            //Render game setting
            Fanex.ExtAcc.Customer.LiveCasino.RenderGameSetting(data);

            //Add event for changing Min/Max PT
            var interval = setInterval(function () {
                if (Fanex.ExtAcc.Customer.LiveCasino.customer != null) {
                    clearInterval(interval);

                    checkMinMaxPT();
                }
            }, 100);

            //Model for binding events on form
            Fanex.ExtAcc.Customer.LiveCasino.LiveCasinoModel = function () {
                this.saveCustomer = function () {
                    // Set PT values
                    var isChecked = document.getElementById('chkAutoPT').checked;
                    Fanex.ExtAcc.Customer.LiveCasino.customer.uplinePtSetting.presetPT = !isChecked ? $('#pt_1').val() : 0;
                    Fanex.ExtAcc.Customer.LiveCasino.customer.uplinePtSetting.autoPresetPT = $('#pt_2').val();
                    Fanex.ExtAcc.Customer.LiveCasino.customer.ptSetting.maxPT = $('#pt_3').val();
                    Fanex.ExtAcc.Customer.LiveCasino.customer.ptSetting.minPT = $('#pt_4').val();
                    Fanex.ExtAcc.Customer.LiveCasino.customer.ptSetting.comm = $('#comm_5').val();

                    // Set member win limit values
                    Fanex.ExtAcc.Customer.LiveCasino.customer.customerInfo.limit.maxWinLimit = $('#maximumWinLimit').val();
                    Fanex.ExtAcc.Customer.LiveCasino.customer.customerInfo.limit.isDailyAutoReset = document.getElementById('dailyAutoReset').checked;

                    // Set game setting values
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

                    if ((Fanex.ExtAcc.Customer.LiveCasino.customer.uplinePtSetting.isAutoPresetPT || Fanex.ExtAcc.Customer.LiveCasino.IsMultiple) && !isChecked) {
                        var items = '';

                        for (var i = (uplineMax - parent.document.getElementById('pt_1').value); i.toFixed(2) >= 0.00; i -= 0.01) {
                            items += '<option>' + (((i != 0 ? i.toFixed(2) : i) * 100) / 100) + '</option>';
                        }

                        Fanex.ExtAcc.Form.ConfirmBox
                        (
                            'You have changed this master not to take auto position taking',
                            'Do you want to update all member\'s position taking to: <select style="width: 60px" id="memberPT">' + items + '</select>',
                            500,
                            140,
                            false,
                            function () {
                                Fanex.ExtAcc.Customer.LiveCasino.customer.uplinePtSetting.isAutoPresetPT = isChecked;
                                Fanex.ExtAcc.Customer.LiveCasino.SubmitMasterPT($('#memberPT').val(), Fanex.ExtAcc.Customer.LiveCasino.IsMultiple ? 0 : null);
                                Fanex.ExtAcc.Form.Close(false, null, 'confirmAutoMember');
                            },
                            function () {
                                Fanex.ExtAcc.Customer.LiveCasino.customer.uplinePtSetting.isAutoPresetPT = isChecked;
                                Fanex.ExtAcc.Customer.LiveCasino.SubmitMasterPT(null, Fanex.ExtAcc.Customer.LiveCasino.IsMultiple ? 0 : null);
                                Fanex.ExtAcc.Form.Close(false, null, 'confirmAutoMember');
                            },
                            'confirmAutoMember'
                        );
                    }
                    else {
                        Fanex.ExtAcc.Customer.LiveCasino.customer.uplinePtSetting.isAutoPresetPT = isChecked;

                        Fanex.ExtAcc.Customer.LiveCasino.SubmitMasterPT(null, Fanex.ExtAcc.Customer.LiveCasino.IsMultiple ? 0 : null);
                    }
                },

                this.savePtComm = function () {
                    var isChecked = document.getElementById('chkAutoPT').checked;

                    if ((Fanex.ExtAcc.Customer.LiveCasino.customer.uplinePtSetting.isAutoPresetPT || Fanex.ExtAcc.Customer.LiveCasino.IsMultiple) && !isChecked) {
                        var items = '';

                        for (var i = (uplineMax - parent.document.getElementById('pt_1').value); i.toFixed(2) >= 0.00; i -= 0.01) {
                            items += '<option>' + (((i != 0 ? i.toFixed(2) : i) * 100) / 100) + '</option>';
                        }

                        Fanex.ExtAcc.Form.ConfirmBox
                        (
                            'You have changed this master not to take auto position taking',
                            'Do you want to update all member\'s position taking to: <select style="width: 60px" id="memberPT">' + items + '</select>',
                            500,
                            140,
                            false,
                            function () {
                                Fanex.ExtAcc.Customer.LiveCasino.customer.uplinePtSetting.isAutoPresetPT = isChecked;
                                Fanex.ExtAcc.Customer.LiveCasino.UpdateMasterPT($('#memberPT').val(), Fanex.ExtAcc.Customer.LiveCasino.IsMultiple ? 0 : null);
                                Fanex.ExtAcc.Form.Close(false, null, 'confirmAutoMember');
                            },
                            function () {
                                Fanex.ExtAcc.Customer.LiveCasino.customer.uplinePtSetting.isAutoPresetPT = isChecked;
                                Fanex.ExtAcc.Customer.LiveCasino.UpdateMasterPT(null, Fanex.ExtAcc.Customer.LiveCasino.IsMultiple ? 0 : null);
                                Fanex.ExtAcc.Form.Close(false, null, 'confirmAutoMember');
                            },
                            'confirmAutoMember'
                        );
                    }
                    else {
                        Fanex.ExtAcc.Customer.LiveCasino.UpdateMasterPT(null, Fanex.ExtAcc.Customer.LiveCasino.IsMultiple ? 0 : null);
                    }
                },

                this.saveMemberWinLimit = function () {
                    Fanex.ExtAcc.Customer.LiveCasino.UpdateMemberWinLimit(Fanex.ExtAcc.Customer.LiveCasino.IsMultiple ? 0 : null);
                },

                this.saveGameSetting = function () {
                    if (Fanex.ExtAcc.Customer.LiveCasino.customer.gameSetting.length > 0) {
                        Fanex.ExtAcc.Customer.LiveCasino.UpdateGameSetting(data, Fanex.ExtAcc.Customer.LiveCasino.IsMultiple ? 0 : null);
                    }
                }
            };

            ko.applyBindings(new Fanex.ExtAcc.Customer.LiveCasino.LiveCasinoModel());

            Fanex.ExtAcc.UnBlockUI();
        })
        .error(function (data) {
            if (typeof console != 'undefined') console.log(data.responseText);
            Fanex.ExtAcc.UnBlockUI();
        });
    },

    GetAgentSetting: function () {
        Fanex.ExtAcc.BlockUI();

        if (!Fanex.ExtAcc.Customer.LiveCasino.IsMultiple) {
            $('#arrowUpDown').remove();
        }

        if (Fanex.ExtAcc.Customer.LiveCasino.IsMultiple) {
            $('#custInfo').remove();

            $('#arrowUpDown').bind('click', function () {
                Fanex.ExtAcc.Customer.ToogleUsernames();
            });

            $('#names').css('display', 'block');

            if (Fanex.ExtAcc.Customer.Usernames.length < 98) {
                $('#names').css('cursor', 'default');
                $('#names').html(Fanex.ExtAcc.Customer.Usernames.substring(0, Fanex.ExtAcc.Customer.Usernames.length - 2));
                $('#arrowUpDown').css('display', 'none');
            }
            else {
                $('#names').css('cursor', 'pointer');
                $('#names').bind('click', function () {
                    Fanex.ExtAcc.Customer.ToogleUsernames();
                });
                $('#names').html(Fanex.ExtAcc.Customer.Usernames.substring(0, 98) + '...');
                $('#names').attr('title', Fanex.ExtAcc.Customer.Usernames.substring(0, Fanex.ExtAcc.Customer.Usernames.length - 2));
                $('#arrowUpDown').css('display', 'block');
            }
        }

        var data = new Object();
        data.custId = Fanex.ExtAcc.Customer.UserId;
        data.uplineId = Fanex.ExtAcc.Customer.masterId;

        $.ajax({
            url: resolveClientUrl('LiveCasino/EditAgentPost'),
            dataType: 'json',
            type: 'post',
            data: data
        })
        .success(function (data) {
            //Assign data for global variable
            Fanex.ExtAcc.Customer.LiveCasino.customer = data;

            //PT Renderring
            //PT values
            var uplineMin = data.uplinePtSetting.minPT;
            var uplineMax = data.uplinePtSetting.maxPT;
            var uplinePreset = data.uplinePtSetting.presetPT;
            var uplineAutoPreset = data.uplinePtSetting.autoPresetPT;
            var max = data.ptSetting.maxPT;

            //Render super preset pt
            PT_RenderCombo(document.getElementById('pt_1'), uplineMin, uplineMax, 0.01, uplinePreset);

            //Render super auto preset pt
            PT_RenderCombo(document.getElementById('pt_2'), uplineMin, uplineMax, 0.01, uplineAutoPreset);
            if (!data.uplinePtSetting.isAutoPresetPT) {
                $('#pt_2').attr('disabled', 'disabled');
            }
            else {
                $('#pt_1').attr('disabled', 'disabled');
            }
            $('#chkAutoPT').attr('checked', data.uplinePtSetting.isAutoPresetPT);

            //Render Master Max PT
            PT_RenderCombo(document.getElementById('pt_3'), 0, (uplineMax - uplinePreset), 0.01, max);

            //Render Master Min PT
            PT_RenderCombo(document.getElementById('pt_4'), 0, data.ptSetting.maxPT, 0.01, data.ptSetting.minPT);

            //Render Master Commission
            Comm_RenderCombo(document.getElementById('comm_5'), 0, data.uplinePtSetting.comm, 0.0005, data.ptSetting.comm);

            //Render member win limit block
            Fanex.ExtAcc.Customer.LiveCasino.RenderMemberWinLimit(data);

            //Render game setting
            Fanex.ExtAcc.Customer.LiveCasino.RenderGameSetting(data);

            //Add event for changing Min/Max PT
            var interval = setInterval(function () {
                if (Fanex.ExtAcc.Customer.LiveCasino.customer != null) {
                    clearInterval(interval);

                    checkMinMaxPT();
                }
            }, 100);

            //Model for binding events on form
            Fanex.ExtAcc.Customer.LiveCasino.LiveCasinoModel = function () {
                this.saveCustomer = function () {
                    // Set PT values
                    var isChecked = document.getElementById('chkAutoPT').checked;
                    Fanex.ExtAcc.Customer.LiveCasino.customer.uplinePtSetting.presetPT = !isChecked ? $('#pt_1').val() : 0;
                    Fanex.ExtAcc.Customer.LiveCasino.customer.uplinePtSetting.autoPresetPT = $('#pt_2').val();
                    Fanex.ExtAcc.Customer.LiveCasino.customer.ptSetting.maxPT = $('#pt_3').val();
                    Fanex.ExtAcc.Customer.LiveCasino.customer.ptSetting.minPT = $('#pt_4').val();
                    Fanex.ExtAcc.Customer.LiveCasino.customer.ptSetting.comm = $('#comm_5').val();

                    // Set member win limit values
                    Fanex.ExtAcc.Customer.LiveCasino.customer.customerInfo.limit.maxWinLimit = $('#maximumWinLimit').val();
                    Fanex.ExtAcc.Customer.LiveCasino.customer.customerInfo.limit.isDailyAutoReset = document.getElementById('dailyAutoReset').checked;

                    // Set game setting values
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

                    if ((Fanex.ExtAcc.Customer.LiveCasino.customer.uplinePtSetting.isAutoPresetPT || Fanex.ExtAcc.Customer.LiveCasino.IsMultiple) && !isChecked) {
                        var items = '';

                        for (var i = (uplineMax - parent.document.getElementById('pt_1').value); i.toFixed(2) >= 0.00; i -= 0.01) {
                            items += '<option>' + (((i != 0 ? i.toFixed(2) : i) * 100) / 100) + '</option>';
                        }

                        Fanex.ExtAcc.Form.ConfirmBox
                        (
                            'You have changed this agent not to take auto position taking',
                            'Do you want to update all member\'s position taking to: <select style="width: 60px" id="memberPT">' + items + '</select>',
                            500,
                            140,
                            false,
                            function () {
                                Fanex.ExtAcc.Customer.LiveCasino.customer.uplinePtSetting.isAutoPresetPT = isChecked;
                                Fanex.ExtAcc.Customer.LiveCasino.SubmitAgentPT($('#memberPT').val(), Fanex.ExtAcc.Customer.LiveCasino.IsMultiple ? 0 : null);
                                Fanex.ExtAcc.Form.Close(false, null, 'confirmAutoMember');
                            },
                            function () {
                                Fanex.ExtAcc.Customer.LiveCasino.customer.uplinePtSetting.isAutoPresetPT = isChecked;
                                Fanex.ExtAcc.Customer.LiveCasino.SubmitAgentPT(null, Fanex.ExtAcc.Customer.LiveCasino.IsMultiple ? 0 : null);
                                Fanex.ExtAcc.Form.Close(false, null, 'confirmAutoMember');
                            },
                            'confirmAutoMember'
                        );
                    }
                    else {
                        Fanex.ExtAcc.Customer.LiveCasino.customer.uplinePtSetting.isAutoPresetPT = isChecked;

                        Fanex.ExtAcc.Customer.LiveCasino.SubmitAgentPT(null, Fanex.ExtAcc.Customer.LiveCasino.IsMultiple ? 0 : null);
                    }
                },

                this.savePtComm = function () {
                    var isChecked = document.getElementById('chkAutoPT').checked;

                    if ((Fanex.ExtAcc.Customer.LiveCasino.customer.uplinePtSetting.isAutoPresetPT || Fanex.ExtAcc.Customer.LiveCasino.IsMultiple) && !isChecked) {
                        var items = '';

                        for (var i = (uplineMax - parent.document.getElementById('pt_1').value); i.toFixed(2) >= 0.00; i -= 0.01) {
                            items += '<option>' + (((i != 0 ? i.toFixed(2) : i) * 100) / 100) + '</option>';
                        }

                        Fanex.ExtAcc.Form.ConfirmBox
                        (
                            'You have changed this agent not to take auto position taking',
                            'Do you want to update all member\'s position taking to: <select style="width: 60px" id="memberPT">' + items + '</select>',
                            500,
                            140,
                            false,
                            function () {
                                Fanex.ExtAcc.Customer.LiveCasino.UpdateAgentPT($('#memberPT').val(), Fanex.ExtAcc.Customer.LiveCasino.IsMultiple ? 0 : null);
                                Fanex.ExtAcc.Form.Close(false, null, 'confirmAutoMember');
                            },
                            function () {
                                Fanex.ExtAcc.Customer.LiveCasino.UpdateAgentPT(null, Fanex.ExtAcc.Customer.LiveCasino.IsMultiple ? 0 : null);
                                Fanex.ExtAcc.Form.Close(false, null, 'confirmAutoMember');
                            },
                            'confirmAutoMember'
                        );
                    }
                    else {
                        Fanex.ExtAcc.Customer.LiveCasino.UpdateAgentPT(null, Fanex.ExtAcc.Customer.LiveCasino.IsMultiple ? 0 : null);
                    }
                },

                this.saveMemberWinLimit = function () {
                    Fanex.ExtAcc.Customer.LiveCasino.UpdateMemberWinLimit(Fanex.ExtAcc.Customer.LiveCasino.IsMultiple ? 0 : null);
                },

                this.saveGameSetting = function () {
                    if (Fanex.ExtAcc.Customer.LiveCasino.customer.gameSetting.length > 0) {
                        Fanex.ExtAcc.Customer.LiveCasino.UpdateGameSetting(data, Fanex.ExtAcc.Customer.LiveCasino.IsMultiple ? 0 : null);
                    }
                }
            };

            ko.applyBindings(new Fanex.ExtAcc.Customer.LiveCasino.LiveCasinoModel());

            Fanex.ExtAcc.UnBlockUI();
        })
        .error(function (data) {
            if (typeof console != 'undefined') console.log(data.responseText);
            Fanex.ExtAcc.UnBlockUI();
        });
    },

    GetMemberSetting: function () {
        Fanex.ExtAcc.BlockUI();

        if (!Fanex.ExtAcc.Customer.LiveCasino.IsMultiple) {
            $('#arrowUpDown').remove();
            $('#enableSection').remove();
        }

        if (Fanex.ExtAcc.Customer.LiveCasino.IsMultiple) {
            $('#custInfo').remove();

            $('#arrowUpDown').bind('click', function () {
                Fanex.ExtAcc.Customer.ToogleUsernames();
            });

            $('#names').css('display', 'block');

            if (Fanex.ExtAcc.Customer.Usernames.length < 98) {
                $('#names').css('cursor', 'default');
                $('#names').html(Fanex.ExtAcc.Customer.Usernames.substring(0, Fanex.ExtAcc.Customer.Usernames.length - 2));
                $('#arrowUpDown').css('display', 'none');
            }
            else {
                $('#names').css('cursor', 'pointer');
                $('#names').bind('click', function () {
                    Fanex.ExtAcc.Customer.ToogleUsernames();
                });
                $('#names').html(Fanex.ExtAcc.Customer.Usernames.substring(0, 98) + '...');
                $('#names').attr('title', Fanex.ExtAcc.Customer.Usernames.substring(0, Fanex.ExtAcc.Customer.Usernames.length - 2));
                $('#arrowUpDown').css('display', 'block');
            }
        }

        var data = new Object();
        data.custId = Fanex.ExtAcc.Customer.UserId;
        data.uplineId = Fanex.ExtAcc.Customer.agentId;

        $.ajax({
            url: resolveClientUrl('LiveCasino/EditMemberPost'),
            dataType: 'json',
            type: 'post',
            data: data
        })
        .success(function (data) {
            //Assign data for global variable
            Fanex.ExtAcc.Customer.LiveCasino.customer = data;

            //PT Renderring
            //PT values
            var uplineMin = data.uplinePtSetting.minPT;
            var uplineMax = data.uplinePtSetting.maxPT;
            var uplinePreset = data.uplinePtSetting.presetPT;
            var uplineAutoPreset = data.uplinePtSetting.autoPresetPT;
            var max = data.ptSetting.maxPT;

            //Render agent preset pt
            PT_RenderCombo(document.getElementById('pt_1'), uplineMin, uplineMax, 0.01, uplinePreset);

            //Render Master Commission
            Comm_RenderCombo(document.getElementById('comm_2'), 0, data.uplinePtSetting.comm, 0.0005, data.ptSetting.comm);

            //Render member win limit block
            Fanex.ExtAcc.Customer.LiveCasino.RenderMemberWinLimit(data);

            //Render game setting
            Fanex.ExtAcc.Customer.LiveCasino.RenderGameSetting(data);

            //Check if all game setting is unchecked
            if (Fanex.ExtAcc.Customer.LiveCasino.customer.gameSetting.length > 0 && Fanex.ExtAcc.Customer.LiveCasino.IsUncheckWinLimitMember) {
                $('#warningMsg').show();
                $('#warningMsg').html('Your downline has uncheck all stake levels, by default, the system checked the lowest level. Please check and update.'.formatErrorMessage());

                document.getElementsByName('gameRow0')[0].checked = true;
            }

            //Model for binding events on form
            Fanex.ExtAcc.Customer.LiveCasino.LiveCasinoModel = function () {
                this.saveCustomer = function () {
                    // Set PT values
                    Fanex.ExtAcc.Customer.LiveCasino.customer.uplinePtSetting.presetPT = $('#pt_1').val();
                    Fanex.ExtAcc.Customer.LiveCasino.customer.ptSetting.comm = $('#comm_2').val();

                    // Set member win limit values
                    Fanex.ExtAcc.Customer.LiveCasino.customer.customerInfo.limit.maxWinLimit = $('#maximumWinLimit').val();
                    Fanex.ExtAcc.Customer.LiveCasino.customer.customerInfo.limit.isDailyAutoReset = document.getElementById('dailyAutoReset').checked;

                    // Set game setting values
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

                    Fanex.ExtAcc.Customer.LiveCasino.SubmitMemberPT(Fanex.ExtAcc.Customer.LiveCasino.IsMultiple ? 0 : null);
                },

                this.savePtComm = function () {
                    Fanex.ExtAcc.Customer.LiveCasino.UpdateMemberPT(Fanex.ExtAcc.Customer.LiveCasino.IsMultiple ? 0 : null);
                },

                this.saveMemberWinLimit = function () {
                    Fanex.ExtAcc.Customer.LiveCasino.UpdateMemberWinLimit(Fanex.ExtAcc.Customer.LiveCasino.IsMultiple ? 0 : null);
                },

                this.saveGameSetting = function () {
                    if (Fanex.ExtAcc.Customer.LiveCasino.customer.gameSetting.length > 0) {
                        Fanex.ExtAcc.Customer.LiveCasino.UpdateGameSetting(data, Fanex.ExtAcc.Customer.LiveCasino.IsMultiple ? 0 : null);
                    }
                }
            };

            ko.applyBindings(new Fanex.ExtAcc.Customer.LiveCasino.LiveCasinoModel());

            Fanex.ExtAcc.UnBlockUI();
        })
        .error(function (data) {
            if (typeof console != 'undefined') console.log(data.responseText);
            Fanex.ExtAcc.UnBlockUI();
        });
    },

    SuperGetMemberSetting: function () {
        Fanex.ExtAcc.BlockUI();

        if (!Fanex.ExtAcc.Customer.LiveCasino.IsMultiple) {
            $('#arrowUpDown').remove();
        }

        if (Fanex.ExtAcc.Customer.LiveCasino.IsMultiple) {
            $('#custInfo').remove();

            $('#arrowUpDown').bind('click', function () {
                Fanex.ExtAcc.Customer.ToogleUsernames();
            });

            $('#names').css('display', 'block');

            if (Fanex.ExtAcc.Customer.Usernames.length < 98) {
                $('#names').css('cursor', 'default');
                $('#names').html(Fanex.ExtAcc.Customer.Usernames.substring(0, Fanex.ExtAcc.Customer.Usernames.length - 2));
                $('#arrowUpDown').css('display', 'none');
            }
            else {
                $('#names').css('cursor', 'pointer');
                $('#names').bind('click', function () {
                    Fanex.ExtAcc.Customer.ToogleUsernames();
                });
                $('#names').html(Fanex.ExtAcc.Customer.Usernames.substring(0, 98) + '...');
                $('#names').attr('title', Fanex.ExtAcc.Customer.Usernames.substring(0, Fanex.ExtAcc.Customer.Usernames.length - 2));
                $('#arrowUpDown').css('display', 'block');
            }
        }

        var data = new Object();
        data.custId = Fanex.ExtAcc.Customer.UserId;
        data.uplineId = Fanex.ExtAcc.Customer.superId;

        $.ajax({
            url: resolveClientUrl('LiveCasino/SuperEditMemberPost'),
            dataType: 'json',
            type: 'post',
            data: data
        })
        .success(function (data) {
            //Assign data for global variable
            Fanex.ExtAcc.Customer.LiveCasino.customer = data;

            //PT Renderring
            //PT values
            var uplineMin = data.superPtSetting.minPT;
            var uplineMax = data.superPtSetting.maxPT;
            var uplinePreset = data.superPtSetting.presetPT;
            var uplineAutoPreset = data.superPtSetting.autoPresetPT;
            var min = data.superPtSetting.minPT;
            var max = data.superPtSetting.maxPT;

            if (min > max) {
                min = max = uplinePreset;
            }

            //Render Super preset pt
            if (!data.masterPtSetting.isAutoPresetPT) {
                PT_RenderCombo(document.getElementById('pt_1'), min, max, 0.01, uplinePreset);
            }
            else {
                $('#pt1').remove();
                $('#pt1Section').html(data.superPtSetting.presetPT);
            }

            //Render Super auto preset pt
            PT_RenderCombo(document.getElementById('pt_2'), uplineMin, uplineMax, 0.01, uplineAutoPreset);
            if (!data.superPtSetting.isAutoPresetPT) {
                $('#pt_2').attr('disabled', 'disabled');
            }
            else {
                $('#pt_1').attr('disabled', 'disabled');
            }
            $('#chkAutoPT').attr('checked', data.superPtSetting.isAutoPresetPT);

            //Render master preset PT
            $('#masterPT').html(data.masterPtSetting.presetPT);

            //Render agent preset PT
            $('#agentPT').html(data.uplinePtSetting.presetPT);

            //Render member win limit block
            Fanex.ExtAcc.Customer.LiveCasino.RenderMemberWinLimit(data);

            //Render game setting
            Fanex.ExtAcc.Customer.LiveCasino.RenderGameSetting(data);

            //Check if all game setting is unchecked
            if (Fanex.ExtAcc.Customer.LiveCasino.customer.gameSetting.length > 0 && Fanex.ExtAcc.Customer.LiveCasino.IsUncheckWinLimitMember) {
                $('#warningMsg').show();
                $('#warningMsg').html('Your downline has uncheck all stake levels, by default, the system checked the lowest level. Please check and update.'.formatErrorMessage());

                document.getElementsByName('gameRow0')[0].checked = true;
            }

            //Model for binding events on form
            Fanex.ExtAcc.Customer.LiveCasino.LiveCasinoModel = function () {
                this.saveCustomer = function () {
                    // Set PT values
                    var isChecked = document.getElementById('chkAutoPT').checked;
                    if (document.getElementById('pt_1'))
                        Fanex.ExtAcc.Customer.LiveCasino.customer.superPtSetting.presetPT = isChecked ? 0 : $('#pt_1').val();
                    Fanex.ExtAcc.Customer.LiveCasino.customer.superPtSetting.autoPresetPT = $('#pt_2').val();
                    Fanex.ExtAcc.Customer.LiveCasino.customer.superPtSetting.isAutoPresetPT = isChecked;

                    // Set member win limit values
                    Fanex.ExtAcc.Customer.LiveCasino.customer.customerInfo.limit.maxWinLimit = $('#maximumWinLimit').val();
                    Fanex.ExtAcc.Customer.LiveCasino.customer.customerInfo.limit.isDailyAutoReset = document.getElementById('dailyAutoReset').checked;

                    // Set game setting values
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

                    Fanex.ExtAcc.Customer.LiveCasino.SuperSubmitMemberPT(Fanex.ExtAcc.Customer.LiveCasino.IsMultiple ? 0 : null);
                },

                this.savePtComm = function () {
                    Fanex.ExtAcc.Customer.LiveCasino.SuperUpdateMemberPT(Fanex.ExtAcc.Customer.LiveCasino.IsMultiple ? 0 : null);
                },

                this.saveMemberWinLimit = function () {
                    Fanex.ExtAcc.Customer.LiveCasino.UpdateMemberWinLimit(Fanex.ExtAcc.Customer.LiveCasino.IsMultiple ? 0 : null);
                },

                this.saveGameSetting = function () {
                    if (Fanex.ExtAcc.Customer.LiveCasino.customer.gameSetting.length > 0) {
                        Fanex.ExtAcc.Customer.LiveCasino.UpdateGameSetting(data, Fanex.ExtAcc.Customer.LiveCasino.IsMultiple ? 0 : null);
                    }
                }
            };

            ko.applyBindings(new Fanex.ExtAcc.Customer.LiveCasino.LiveCasinoModel());

            Fanex.ExtAcc.UnBlockUI();
        })
        .error(function (data) {
            if (typeof console != 'undefined') console.log(data.responseText);
            Fanex.ExtAcc.UnBlockUI();
        });
    },

    MasterGetMemberSetting: function () {
        Fanex.ExtAcc.BlockUI();

        if (!Fanex.ExtAcc.Customer.LiveCasino.IsMultiple) {
            $('#arrowUpDown').remove();
        }

        if (Fanex.ExtAcc.Customer.LiveCasino.IsMultiple) {
            $('#custInfo').remove();

            $('#arrowUpDown').bind('click', function () {
                Fanex.ExtAcc.Customer.ToogleUsernames();
            });

            $('#names').css('display', 'block');

            if (Fanex.ExtAcc.Customer.Usernames.length < 98) {
                $('#names').css('cursor', 'default');
                $('#names').html(Fanex.ExtAcc.Customer.Usernames.substring(0, Fanex.ExtAcc.Customer.Usernames.length - 2));
                $('#arrowUpDown').css('display', 'none');
            }
            else {
                $('#names').css('cursor', 'pointer');
                $('#names').bind('click', function () {
                    Fanex.ExtAcc.Customer.ToogleUsernames();
                });
                $('#names').html(Fanex.ExtAcc.Customer.Usernames.substring(0, 98) + '...');
                $('#names').attr('title', Fanex.ExtAcc.Customer.Usernames.substring(0, Fanex.ExtAcc.Customer.Usernames.length - 2));
                $('#arrowUpDown').css('display', 'block');
            }
        }

        var data = new Object();
        data.custId = Fanex.ExtAcc.Customer.UserId;
        data.uplineId = Fanex.ExtAcc.Customer.masterId;

        $.ajax({
            url: resolveClientUrl('LiveCasino/MasterEditMemberPost'),
            dataType: 'json',
            type: 'post',
            data: data
        })
        .success(function (data) {
            //Assign data for global variable
            Fanex.ExtAcc.Customer.LiveCasino.customer = data;

            //PT Renderring
            //PT values
            var uplineMin = data.masterPtSetting.minPT;
            var uplineMax = data.masterPtSetting.maxPT;
            var uplinePreset = data.masterPtSetting.presetPT;
            var uplineAutoPreset = data.masterPtSetting.autoPresetPT;
            var min = data.masterPtSetting.minPT;
            var max = data.masterPtSetting.maxPT;

            if (min > max) {
                min = max = uplinePreset;
            }

            //Render master preset pt
            if (!data.uplinePtSetting.isAutoPresetPT) {
                PT_RenderCombo(document.getElementById('pt_1'), min, max, 0.01, uplinePreset);
            }
            else {
                $('#pt1').remove();
                $('#pt1Section').html(data.masterPtSetting.presetPT);
            }

            //Render master auto preset pt
            PT_RenderCombo(document.getElementById('pt_2'), uplineMin, uplineMax, 0.01, uplineAutoPreset);
            if (!data.masterPtSetting.isAutoPresetPT) {
                $('#pt_2').attr('disabled', 'disabled');
            }
            else {
                $('#pt_1').attr('disabled', 'disabled');
            }
            $('#chkAutoPT').attr('checked', data.masterPtSetting.isAutoPresetPT);

            //Render agent preset PT
            $('#agentPT').html(data.uplinePtSetting.presetPT);

            //Render member win limit block
            Fanex.ExtAcc.Customer.LiveCasino.RenderMemberWinLimit(data);

            //Render game setting
            Fanex.ExtAcc.Customer.LiveCasino.RenderGameSetting(data);

            //Check if all game setting is unchecked
            if (Fanex.ExtAcc.Customer.LiveCasino.customer.gameSetting.length > 0 && Fanex.ExtAcc.Customer.LiveCasino.IsUncheckWinLimitMember) {
                $('#warningMsg').show();
                $('#warningMsg').html('Your downline has uncheck all stake levels, by default, the system checked the lowest level. Please check and update.'.formatErrorMessage());

                document.getElementsByName('gameRow0')[0].checked = true;
            }

            //Model for binding events on form
            Fanex.ExtAcc.Customer.LiveCasino.LiveCasinoModel = function () {
                this.saveCustomer = function () {
                    // Set PT values
                    var isChecked = document.getElementById('chkAutoPT').checked;
                    if (document.getElementById('pt_1'))
                        Fanex.ExtAcc.Customer.LiveCasino.customer.masterPtSetting.presetPT = isChecked ? 0 : $('#pt_1').val();
                    Fanex.ExtAcc.Customer.LiveCasino.customer.masterPtSetting.autoPresetPT = $('#pt_2').val();
                    Fanex.ExtAcc.Customer.LiveCasino.customer.masterPtSetting.isAutoPresetPT = isChecked;

                    // Set member win limit values
                    Fanex.ExtAcc.Customer.LiveCasino.customer.customerInfo.limit.maxWinLimit = $('#maximumWinLimit').val();
                    Fanex.ExtAcc.Customer.LiveCasino.customer.customerInfo.limit.isDailyAutoReset = document.getElementById('dailyAutoReset').checked;

                    // Set game setting values
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

                    Fanex.ExtAcc.Customer.LiveCasino.MasterSubmitMemberPT(Fanex.ExtAcc.Customer.LiveCasino.IsMultiple ? 0 : null);
                },

                this.savePtComm = function () {
                    Fanex.ExtAcc.Customer.LiveCasino.MasterUpdateMemberPT(Fanex.ExtAcc.Customer.LiveCasino.IsMultiple ? 0 : null);
                },

                this.saveMemberWinLimit = function () {
                    Fanex.ExtAcc.Customer.LiveCasino.UpdateMemberWinLimit(Fanex.ExtAcc.Customer.LiveCasino.IsMultiple ? 0 : null);
                },

                this.saveGameSetting = function () {
                    if (Fanex.ExtAcc.Customer.LiveCasino.customer.gameSetting.length > 0) {
                        Fanex.ExtAcc.Customer.LiveCasino.UpdateGameSetting(data, Fanex.ExtAcc.Customer.LiveCasino.IsMultiple ? 0 : null);
                    }
                }
            };

            ko.applyBindings(new Fanex.ExtAcc.Customer.LiveCasino.LiveCasinoModel());

            Fanex.ExtAcc.UnBlockUI();
        })
        .error(function (data) {
            if (typeof console != 'undefined') console.log(data.responseText);
            Fanex.ExtAcc.UnBlockUI();
        });
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

    UpdateMemberWinLimit: function (i) {
        Fanex.ExtAcc.BlockUI();

        Fanex.ExtAcc.Customer.LiveCasino.customer.customerInfo.groupName = $('#selectedGroupName').html();
        Fanex.ExtAcc.Customer.LiveCasino.customer.customerInfo.limit.maxWinLimit = $('#maximumWinLimit').val();
        Fanex.ExtAcc.Customer.LiveCasino.customer.customerInfo.limit.isDailyAutoReset = document.getElementById('dailyAutoReset').checked;

        var uplineId = 0;

        switch (Fanex.ExtAcc.Customer.CurrentLevel) {
            case 3:
                uplineId = Fanex.ExtAcc.Customer.superId;
                break;
            case 2:
                uplineId = Fanex.ExtAcc.Customer.masterId;
                break;
            case 1:
                uplineId = Fanex.ExtAcc.Customer.agentId;
                break;
            default:
                break;
        }

        if (i != null) {
            Fanex.ExtAcc.Customer.LiveCasino.customer.customerInfo.userName = Fanex.ExtAcc.Customer.Usernames.split(', ')[i];
        }

        var data = {
            uplineId: uplineId,
            custId: i == null ? Fanex.ExtAcc.Customer.UserId : parseInt(Fanex.ExtAcc.Customer.CustIds.split(',')[i]),
            customer: JSON.stringify(Fanex.ExtAcc.Customer.LiveCasino.customer)
        };

        $.ajax({
            url: resolveClientUrl('LiveCasino/UpdateMemberWinLimitPost'),
            type: 'post',
            dataType: 'json',
            data: data
        })
        .success(function (result) {
            if (result.ErrorCode && result.ErrorCode == '1') {
                $('#errMsg').show();
                $('#errMsg').html(result.ErrorMessage.formatErrorMessage() + '<br />');

                Fanex.ExtAcc.UnBlockUI();
            }
            else {
                var error = Fanex.ExtAcc.Customer.LiveCasino.RenderMemberWinLimitMessage(result);
                Fanex.ExtAcc.Customer.LiveCasino.Error += '<font color="Blue" style="font-weight: bold">' + Fanex.ExtAcc.Customer.Usernames.split(', ')[i] + '</font><br />' + error + '<br />';

                if (!Fanex.ExtAcc.Customer.LiveCasino.IsMultiple) {
                    var timeout = setTimeout(function () {
                        clearTimeout(timeout);

                        $('#errMsg').css('display', 'block');
                        $('#errMsg').html(error);
                        Fanex.ExtAcc.UnBlockUI();
                    }, 2000);
                }
                else {
                    if (Fanex.ExtAcc.Customer.LiveCasino.CustIdIndex < Fanex.ExtAcc.Customer.CustIds.split(',').length - 2) {
                        Fanex.ExtAcc.Customer.LiveCasino.UpdateMemberWinLimit(++Fanex.ExtAcc.Customer.LiveCasino.CustIdIndex);
                    }
                    else {
                        Fanex.ExtAcc.Customer.LiveCasino.CustIdIndex = 0;

                        $('#errMsg').show();
                        $('#errMsg').html(Fanex.ExtAcc.Customer.LiveCasino.Error);

                        Fanex.ExtAcc.Customer.LiveCasino.Error = '';

                        Fanex.ExtAcc.UnBlockUI();
                    }
                }
            }
        })
        .error(function (data) {
            if (typeof console != undefined) {
                console.log(data);
            }

            Fanex.ExtAcc.UnBlockUI();
        });
    },

    RenderGameSetting: function (data) {
        var gameSetting = data.gameSetting;
        var type = Fanex.ExtAcc.Customer.CurrentLevel == Fanex.ExtAcc.Customer.Member ? 'radio' : 'checkbox';

        for (var i = 0; i < gameSetting.length; i++) {
            var bettingLimitItem = '';

            for (var j = 0; j < gameSetting[i].bettingLimit.length; j++) {
                if (Fanex.ExtAcc.Customer.CurrentLevel == Fanex.ExtAcc.Customer.Member && gameSetting[i].bettingLimit[j].isSelected) {
                    Fanex.ExtAcc.Customer.LiveCasino.IsUncheckWinLimitMember = false;
                }

                var selected = gameSetting[i].bettingLimit[j].isSelected ? 'checked="checked"' : '';
                bettingLimitItem += '&nbsp;<input name="gameRow' + i + '" style="vertical-align: middle;margin-bottom: 3px;" id="' + gameSetting[i].bettingLimit[j].id + '" type="' + type + '" ' + selected + '/><span>' + gameSetting[i].bettingLimit[j].text + '</span>';
            }

            var row = '<div class="customerInfoBlock1" style="margin: 0px">' +
                gameSetting[i].name +
                '</div>' +
                '<div class="customerInfoBlock2" style="width: 675px; height: auto">' +
                bettingLimitItem +
                '</div>';

            $('#gameSetting').append(row);
        }
    },

    UpdateGameSetting: function (data, idx) {
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

        var uplineId = 0;

        switch (Fanex.ExtAcc.Customer.CurrentLevel) {
            case 3:
                uplineId = Fanex.ExtAcc.Customer.superId;
                break;
            case 2:
                uplineId = Fanex.ExtAcc.Customer.masterId;
                break;
            case 1:
                uplineId = Fanex.ExtAcc.Customer.agentId;
                break;
            default:
                break;
        }

        if (idx != null) {
            Fanex.ExtAcc.Customer.LiveCasino.customer.customerInfo.userName = Fanex.ExtAcc.Customer.Usernames.split(', ')[idx];
        }

        var data = {
            uplineRoleId: Fanex.ExtAcc.Customer.CurrentLevel + 1,
            custId: idx == null ? Fanex.ExtAcc.Customer.UserId : parseInt(Fanex.ExtAcc.Customer.CustIds.split(',')[idx]),
            uplineId: uplineId,
            customer: JSON.stringify(Fanex.ExtAcc.Customer.LiveCasino.customer)
        };

        $.ajax({
            url: resolveClientUrl('LiveCasino/UpdateGameSettingPost'),
            data: data,
            type: 'post',
            dataType: 'json'
        })
        .success(function (result) {
            if (result.ErrorCode && result.ErrorCode == '1') {
                $('#errMsg').show();
                $('#errMsg').html(result.ErrorMessage.formatErrorMessage() + '<br />');

                Fanex.ExtAcc.UnBlockUI();
            }
            else {
                var error = Fanex.ExtAcc.Customer.LiveCasino.RenderGameSettingMessage(result);
                Fanex.ExtAcc.Customer.LiveCasino.Error += '<font color="Blue" style="font-weight: bold">' + Fanex.ExtAcc.Customer.Usernames.split(', ')[idx] + '</font><br />' + error + '<br />';

                if (!Fanex.ExtAcc.Customer.LiveCasino.IsMultiple) {
                    var timeout = setTimeout(function () {
                        clearTimeout(timeout);

                        $('#errMsg').show();
                        $('#errMsg').html(error);

                        Fanex.ExtAcc.UnBlockUI();
                    }, 2000);
                }
                else {
                    if (Fanex.ExtAcc.Customer.LiveCasino.CustIdIndex < Fanex.ExtAcc.Customer.CustIds.split(',').length - 2) {
                        Fanex.ExtAcc.Customer.LiveCasino.UpdateGameSetting(data, ++Fanex.ExtAcc.Customer.LiveCasino.CustIdIndex);
                    }
                    else {
                        Fanex.ExtAcc.Customer.LiveCasino.CustIdIndex = 0;

                        $('#errMsg').show();
                        $('#errMsg').html(Fanex.ExtAcc.Customer.LiveCasino.Error);

                        Fanex.ExtAcc.Customer.LiveCasino.Error = '';

                        Fanex.ExtAcc.UnBlockUI();
                    }
                }
            }
        })
        .error(function (data) {
            if (typeof console != 'undefined') {
                console.log(data);
            }

            Fanex.ExtAcc.UnBlockUI();
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
                case 103:
                    updatePart = 'Position Taking: ';
                    break;
                default:
                    updatePart = 'Position Taking/Commission: ';
                    break;
            }

            if (code > 0) {
                error += '<font color="Red">' + updatePart + '</font>' + msgs[i].formatErrorMessage() + '<br/>';
            }
            else {
                error += '<font color="Blue">' + updatePart + 'Updated successfully</font><br/>';
            }
        }

        if (error == '' && data.ErrorMessage != '') {
            error = data.ErrorMessage.formatErrorMessage() + '<br />';
        }

        return error == '' ? '<font color="Blue">Position Taking/Commission: Updated successfully</font><br />' : error;
    },

    RenderMemberWinLimitMessage: function (data) {
        var error = '<font color="Blue">Member Win Limit: Updated successfully</font><br />';

        if (data.ErrorCode != "0") {
            error = '<font color="Red">Member Win Limit: ' + data.ErrorMessage.formatErrorMessage() + '</font><br />';
        }

        return error;
    },

    RenderGameSettingMessage: function (data) {
        var error = '';

        for (var i = 0; i < data.length; i++) {
            if (data[i].ErrorCode != '' && data[i].ErrorCode != 'GS0000' && parseInt(data[i].ErrorCode) != 0) {
                error += data[i].ErrorMessage.formatErrorMessage() + '<br />';
            }
        }

        return error != '' ? '<font color="Red">Game Setting: </font>' + error : '<font color="Blue">Game Setting: Updated successfully</font><br />';
    },

    UpdateMasterPT: function (newMemberPT, i) {
        Fanex.ExtAcc.BlockUI();
        var isChecked = document.getElementById('chkAutoPT').checked;
        Fanex.ExtAcc.Customer.LiveCasino.customer.uplinePtSetting.presetPT = !isChecked ? $('#pt_1').val() : 0;
        Fanex.ExtAcc.Customer.LiveCasino.customer.uplinePtSetting.isAutoPresetPT = isChecked;
        Fanex.ExtAcc.Customer.LiveCasino.customer.uplinePtSetting.autoPresetPT = $('#pt_2').val();
        Fanex.ExtAcc.Customer.LiveCasino.customer.ptSetting.maxPT = $('#pt_3').val();
        Fanex.ExtAcc.Customer.LiveCasino.customer.ptSetting.minPT = $('#pt_4').val();
        Fanex.ExtAcc.Customer.LiveCasino.customer.ptSetting.comm = $('#comm_5').val();

        if (newMemberPT) {
            Fanex.ExtAcc.Customer.LiveCasino.customer.uplinePtSetting.newMemberPT = $('#memberPT').val();
        }

        Fanex.ExtAcc.Customer.LiveCasino.customer.customerInfo.groupName = $('#selectedGroupName').html();

        if (i != null) {
            Fanex.ExtAcc.Customer.LiveCasino.customer.customerInfo.userName = Fanex.ExtAcc.Customer.Usernames.split(', ')[i];
        }

        var data = {
            custId: i == null ? Fanex.ExtAcc.Customer.UserId : Fanex.ExtAcc.Customer.CustIds.split(',')[i],
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
            if (result.ErrorCode && result.ErrorCode == '1') {
                $('#errMsg').show();
                $('#errMsg').html(result.ErrorMessage.formatErrorMessage() + '<br />');

                Fanex.ExtAcc.UnBlockUI();
            }
            else {
                var msg = Fanex.ExtAcc.Customer.LiveCasino.RenderPtCommErrorMessage(result);
                Fanex.ExtAcc.Customer.LiveCasino.Error += '<font color="Blue" style="font-weight: bold;">' + Fanex.ExtAcc.Customer.Usernames.split(', ')[i] + ':</font><br />' + msg + '<br />';

                if (!Fanex.ExtAcc.Customer.LiveCasino.IsMultiple) {
                    var timeout = setTimeout(function () {
                        clearTimeout(timeout);

                        $('#errMsg').show();
                        $('#errMsg').html(msg);

                        Fanex.ExtAcc.UnBlockUI();
                    }, 2000);
                }
                else {
                    if (Fanex.ExtAcc.Customer.LiveCasino.CustIdIndex < Fanex.ExtAcc.Customer.CustIds.split(',').length - 2) {
                        Fanex.ExtAcc.Customer.LiveCasino.UpdateMasterPT(null, ++Fanex.ExtAcc.Customer.LiveCasino.CustIdIndex);
                    }
                    else {
                        Fanex.ExtAcc.Customer.LiveCasino.CustIdIndex = 0;

                        $('#errMsg').show();
                        $('#errMsg').html(Fanex.ExtAcc.Customer.LiveCasino.Error);

                        Fanex.ExtAcc.Customer.LiveCasino.Error = '';

                        Fanex.ExtAcc.UnBlockUI();
                    }
                }
            }
        })
        .error(function (data) {
            if (typeof console != 'undefined') {
                console.log(data);
            }

            Fanex.ExtAcc.UnBlockUI();
        })
    },

    SubmitMasterPT: function (newMemberPT, i) {
        Fanex.ExtAcc.BlockUI();

        if (newMemberPT) {
            Fanex.ExtAcc.Customer.LiveCasino.customer.uplinePtSetting.newMemberPT = $('#memberPT').val();
        }

        // Set params
        if (i != null) {
            Fanex.ExtAcc.Customer.LiveCasino.customer.customerInfo.userName = Fanex.ExtAcc.Customer.Usernames.split(', ')[i];
        }

        var data = {
            custId: i == null ? Fanex.ExtAcc.Customer.UserId : parseInt(Fanex.ExtAcc.Customer.CustIds.split(',')[i]),
            uplineId: Fanex.ExtAcc.Customer.superId,
            customer: JSON.stringify(Fanex.ExtAcc.Customer.LiveCasino.customer)
        };

        $.ajax({
            url: resolveClientUrl('LiveCasino/UpdateMasterLiveCasinoPost'),
            type: 'post',
            data: data,
            dataType: 'json'
        })
        .success(function (result) {
            if (result.ErrorCode && result.ErrorCode == '1') {
                $('#errMsg').show();
                $('#errMsg').html(result.ErrorMessage.formatErrorMessage());

                Fanex.ExtAcc.UnBlockUI();
            }
            else {
                var pt = result[0];
                var mw = result[1];
                var gs = result[2];
                var ptMsg = '', mwMsg = '', gsMsg = '';

                // PT messages
                ptMsg = Fanex.ExtAcc.Customer.LiveCasino.RenderPtCommErrorMessage(pt);

                // Member win limit messages
                mwMsg = Fanex.ExtAcc.Customer.LiveCasino.RenderMemberWinLimitMessage(mw);

                // Game setting messages
                if (gs[0].BettypeId != 0) {
                    gsMsg = Fanex.ExtAcc.Customer.LiveCasino.RenderGameSettingMessage(gs);
                }

                Fanex.ExtAcc.Customer.LiveCasino.Error += '<font color="Blue" style="font-weight: bold;">' + Fanex.ExtAcc.Customer.Usernames.split(', ')[i] + ':</font><br />' + ptMsg + mwMsg + gsMsg + '<br />';

                if (!Fanex.ExtAcc.Customer.LiveCasino.IsMultiple) {
                    var timeout = setTimeout(function () {
                        clearTimeout(timeout);

                        $('#errMsg').show();
                        $('#errMsg').html(ptMsg + mwMsg + gsMsg);

                        Fanex.ExtAcc.UnBlockUI();
                    }, 2000);
                }
                else {
                    if (Fanex.ExtAcc.Customer.LiveCasino.CustIdIndex < Fanex.ExtAcc.Customer.CustIds.split(',').length - 2) {
                        Fanex.ExtAcc.Customer.LiveCasino.SubmitMasterPT(null, ++Fanex.ExtAcc.Customer.LiveCasino.CustIdIndex);
                    }
                    else {
                        Fanex.ExtAcc.Customer.LiveCasino.CustIdIndex = 0;

                        $('#errMsg').show();
                        $('#errMsg').html(Fanex.ExtAcc.Customer.LiveCasino.Error);

                        Fanex.ExtAcc.Customer.LiveCasino.Error = '';

                        Fanex.ExtAcc.UnBlockUI();
                    }
                }
            }
        })
        .error(function (data) {
            if (typeof console != 'undefined') {
                console.log(data);
            }

            Fanex.ExtAcc.UnBlockUI();
        });
    },

    UpdateAgentPT: function (newMemberPT, i) {
        Fanex.ExtAcc.BlockUI();
        var isChecked = document.getElementById('chkAutoPT').checked;
        Fanex.ExtAcc.Customer.LiveCasino.customer.uplinePtSetting.presetPT = !isChecked ? $('#pt_1').val() : 0;
        Fanex.ExtAcc.Customer.LiveCasino.customer.uplinePtSetting.isAutoPresetPT = isChecked;
        Fanex.ExtAcc.Customer.LiveCasino.customer.uplinePtSetting.autoPresetPT = $('#pt_2').val();
        Fanex.ExtAcc.Customer.LiveCasino.customer.ptSetting.maxPT = $('#pt_3').val();
        Fanex.ExtAcc.Customer.LiveCasino.customer.ptSetting.minPT = $('#pt_4').val();
        Fanex.ExtAcc.Customer.LiveCasino.customer.ptSetting.comm = $('#comm_5').val();

        if (newMemberPT) {
            Fanex.ExtAcc.Customer.LiveCasino.customer.uplinePtSetting.newMemberPT = $('#memberPT').val();
        }

        Fanex.ExtAcc.Customer.LiveCasino.customer.customerInfo.groupName = $('#selectedGroupName').html();

        if (i != null) {
            Fanex.ExtAcc.Customer.LiveCasino.customer.customerInfo.userName = Fanex.ExtAcc.Customer.Usernames.split(', ')[i];
        }

        var data = {
            custId: i == null ? Fanex.ExtAcc.Customer.UserId : parseInt(Fanex.ExtAcc.Customer.CustIds.split(',')[i]),
            uplineId: Fanex.ExtAcc.Customer.masterId,
            customer: JSON.stringify(Fanex.ExtAcc.Customer.LiveCasino.customer)
        };

        $.ajax({
            url: resolveClientUrl('LiveCasino/UpdateAgentPTPost'),
            data: data,
            type: 'post',
            dataType: 'json'
        })
        .success(function (result) {
            if (result.ErrorCode && result.ErrorCode == '1') {
                $('#errMsg').show();
                $('#errMsg').html(result.ErrorMessage.formatErrorMessage() + '<br />');

                Fanex.ExtAcc.UnBlockUI();
            }
            else {
                var msg = Fanex.ExtAcc.Customer.LiveCasino.RenderPtCommErrorMessage(result);
                Fanex.ExtAcc.Customer.LiveCasino.Error += '<font color="Blue" style="font-weight: bold;">' + Fanex.ExtAcc.Customer.Usernames.split(', ')[i] + ':</font><br />' + msg + '<br />';

                if (!Fanex.ExtAcc.Customer.LiveCasino.IsMultiple) {
                    var timeout = setTimeout(function () {
                        clearTimeout(timeout);


                        $('#errMsg').show();
                        $('#errMsg').html(msg);

                        Fanex.ExtAcc.UnBlockUI();
                    }, 2000);
                }
                else {
                    if (Fanex.ExtAcc.Customer.LiveCasino.CustIdIndex < Fanex.ExtAcc.Customer.CustIds.split(',').length - 2) {
                        Fanex.ExtAcc.Customer.LiveCasino.UpdateAgentPT(null, ++Fanex.ExtAcc.Customer.LiveCasino.CustIdIndex);
                    }
                    else {
                        Fanex.ExtAcc.Customer.LiveCasino.CustIdIndex = 0;

                        $('#errMsg').show();
                        $('#errMsg').html(Fanex.ExtAcc.Customer.LiveCasino.Error);

                        Fanex.ExtAcc.Customer.LiveCasino.Error = '';

                        Fanex.ExtAcc.UnBlockUI();
                    }
                }
            }
        })
        .error(function (data) {
            if (typeof console != 'undefined') {
                console.log(data);
            }

            Fanex.ExtAcc.UnBlockUI();
        })
    },

    SubmitAgentPT: function (newMemberPT, i) {
        Fanex.ExtAcc.BlockUI();

        if (newMemberPT) {
            Fanex.ExtAcc.Customer.LiveCasino.customer.uplinePtSetting.newMemberPT = $('#memberPT').val();
        }

        // Set params
        if (i != null) {
            Fanex.ExtAcc.Customer.LiveCasino.customer.customerInfo.userName = Fanex.ExtAcc.Customer.Usernames.split(', ')[i];
        }

        var data = {
            custId: i == null ? Fanex.ExtAcc.Customer.UserId : parseInt(Fanex.ExtAcc.Customer.CustIds.split(',')[i]),
            uplineId: Fanex.ExtAcc.Customer.masterId,
            customer: JSON.stringify(Fanex.ExtAcc.Customer.LiveCasino.customer)
        };

        $.ajax({
            url: resolveClientUrl('LiveCasino/UpdateAgentLiveCasinoPost'),
            type: 'post',
            data: data,
            dataType: 'json'
        })
        .success(function (result) {
            if (result.ErrorCode && result.ErrorCode == '1') {
                $('#errMsg').show();
                $('#errMsg').html(result.ErrorMessage.formatErrorMessage());

                Fanex.ExtAcc.UnBlockUI();
            }
            else {
                var pt = result[0];
                var mw = result[1];
                var gs = result[2];
                var ptMsg = '', mwMsg = '', gsMsg = '';

                // PT messages
                ptMsg = Fanex.ExtAcc.Customer.LiveCasino.RenderPtCommErrorMessage(pt);

                // Member win limit messages
                mwMsg = Fanex.ExtAcc.Customer.LiveCasino.RenderMemberWinLimitMessage(mw);

                // Game setting messages
                if (gs[0].BettypeId != 0) {
                    gsMsg = Fanex.ExtAcc.Customer.LiveCasino.RenderGameSettingMessage(gs);
                }

                Fanex.ExtAcc.Customer.LiveCasino.Error += '<font color="Blue" style="font-weight: bold;">' + Fanex.ExtAcc.Customer.Usernames.split(', ')[i] + ':</font><br />' + ptMsg + mwMsg + gsMsg + '<br />';

                if (!Fanex.ExtAcc.Customer.LiveCasino.IsMultiple) {
                    var timeout = setTimeout(function () {
                        clearTimeout(timeout);

                        $('#errMsg').show();
                        $('#errMsg').html(ptMsg + mwMsg + gsMsg);

                        Fanex.ExtAcc.UnBlockUI();
                    }, 2000);
                }
                else {
                    if (Fanex.ExtAcc.Customer.LiveCasino.CustIdIndex < Fanex.ExtAcc.Customer.CustIds.split(',').length - 2) {
                        Fanex.ExtAcc.Customer.LiveCasino.SubmitAgentPT(null, ++Fanex.ExtAcc.Customer.LiveCasino.CustIdIndex);
                    }
                    else {
                        Fanex.ExtAcc.Customer.LiveCasino.CustIdIndex = 0;

                        $('#errMsg').show();
                        $('#errMsg').html(Fanex.ExtAcc.Customer.LiveCasino.Error);

                        Fanex.ExtAcc.Customer.LiveCasino.Error = '';

                        Fanex.ExtAcc.UnBlockUI();
                    }
                }
            }
        })
        .error(function (data) {
            if (typeof console != 'undefined') {
                console.log(data);
            }

            Fanex.ExtAcc.UnBlockUI();
        });
    },

    UpdateMemberPT: function (i) {
        Fanex.ExtAcc.BlockUI();
        Fanex.ExtAcc.Customer.LiveCasino.customer.uplinePtSetting.presetPT = $('#pt_1').val();
        Fanex.ExtAcc.Customer.LiveCasino.customer.ptSetting.comm = $('#comm_2').val();
        Fanex.ExtAcc.Customer.LiveCasino.customer.customerInfo.groupName = $('#selectedGroupName').html();

        if (i != null) {
            Fanex.ExtAcc.Customer.LiveCasino.customer.customerInfo.userName = Fanex.ExtAcc.Customer.Usernames.split(', ')[i];
        }

        var data = {
            custId: i == null ? Fanex.ExtAcc.Customer.UserId : parseInt(Fanex.ExtAcc.Customer.CustIds.split(',')[i]),
            uplineId: Fanex.ExtAcc.Customer.agentId,
            customer: JSON.stringify(Fanex.ExtAcc.Customer.LiveCasino.customer)
        };

        $.ajax({
            url: resolveClientUrl('LiveCasino/UpdateMemberPTPost'),
            data: data,
            type: 'post',
            dataType: 'json'
        })
        .success(function (result) {
            if (result.ErrorCode && result.ErrorCode == '1') {
                $('#errMsg').show();
                $('#errMsg').html(result.ErrorMessage.formatErrorMessage() + '<br />');

                Fanex.ExtAcc.UnBlockUI();
            }
            else {
                var msg = Fanex.ExtAcc.Customer.LiveCasino.RenderPtCommErrorMessage(result);
                Fanex.ExtAcc.Customer.LiveCasino.Error += '<font color="Blue" style="font-weight: bold;">' + Fanex.ExtAcc.Customer.Usernames.split(', ')[i] + ':</font><br />' + msg + '<br />';

                if (!Fanex.ExtAcc.Customer.LiveCasino.IsMultiple) {
                    var timeout = setTimeout(function () {
                        clearTimeout(timeout);


                        $('#errMsg').show();
                        $('#errMsg').html(msg);

                        Fanex.ExtAcc.UnBlockUI();
                    }, 2000);
                }
                else {
                    if (Fanex.ExtAcc.Customer.LiveCasino.CustIdIndex < Fanex.ExtAcc.Customer.CustIds.split(',').length - 2) {
                        Fanex.ExtAcc.Customer.LiveCasino.UpdateMemberPT(++Fanex.ExtAcc.Customer.LiveCasino.CustIdIndex);
                    }
                    else {
                        Fanex.ExtAcc.Customer.LiveCasino.CustIdIndex = 0;

                        $('#errMsg').show();
                        $('#errMsg').html(Fanex.ExtAcc.Customer.LiveCasino.Error);

                        Fanex.ExtAcc.Customer.LiveCasino.Error = '';

                        Fanex.ExtAcc.UnBlockUI();
                    }
                }
            }
        })
        .error(function (data) {
            if (typeof console != 'undefined') {
                console.log(data);
            }

            Fanex.ExtAcc.UnBlockUI();
        })
    },

    SubmitMemberPT: function (i) {
        Fanex.ExtAcc.BlockUI();

        // Set params
        Fanex.ExtAcc.Customer.LiveCasino.customer.customerInfo.disableLiveCasino = !document.getElementById('chkEnable').checked;

        if (i != null) {
            Fanex.ExtAcc.Customer.LiveCasino.customer.customerInfo.userName = Fanex.ExtAcc.Customer.Usernames.split(', ')[i];
        }

        var data = {
            custId: i == null ? Fanex.ExtAcc.Customer.UserId : parseInt(Fanex.ExtAcc.Customer.CustIds.split(',')[i]),
            uplineId: Fanex.ExtAcc.Customer.agentId,
            customer: JSON.stringify(Fanex.ExtAcc.Customer.LiveCasino.customer)
        };

        $.ajax({
            url: resolveClientUrl('LiveCasino/UpdateMemberLiveCasinoPost'),
            type: 'post',
            data: data,
            dataType: 'json'
        })
        .success(function (result) {
            if (result.ErrorCode && result.ErrorCode == '1') {
                $('#errMsg').show();
                $('#errMsg').html(result.ErrorMessage.formatErrorMessage());

                Fanex.ExtAcc.UnBlockUI();
            }
            else {
                var pt = result[0];
                var mw = result[1];
                var gs = result[2];
                var st = result[3];
                var ptMsg = '', mwMsg = '', gsMsg = '', stMsg = '';

                // PT messages
                ptMsg = Fanex.ExtAcc.Customer.LiveCasino.RenderPtCommErrorMessage(pt);

                // Member win limit messages
                mwMsg = Fanex.ExtAcc.Customer.LiveCasino.RenderMemberWinLimitMessage(mw);

                // Game setting messages
                if (gs[0].BettypeId != 0) {
                    gsMsg = Fanex.ExtAcc.Customer.LiveCasino.RenderGameSettingMessage(gs);
                }

                if (st.ErrorCode == '' || st.ErrorCode == '0' || st.ErrorCode == "0") {
                    stMsg = '<font color="Blue">Enable: Updated successfully</font><br />';
                }
                else {
                    stMsg = '<font color="Red">Enable: ' + st.ErrorMessage.formatErrorMessage() + '</font><br />'
                }

                Fanex.ExtAcc.Customer.LiveCasino.Error += '<font color="Blue" style="font-weight: bold;">' + Fanex.ExtAcc.Customer.Usernames.split(', ')[i] + ':</font><br />' + ptMsg + mwMsg + gsMsg + stMsg + '<br />';

                if (!Fanex.ExtAcc.Customer.LiveCasino.IsMultiple) {
                    var timeout = setTimeout(function () {
                        clearTimeout(timeout);

                        $('#errMsg').show();
                        $('#errMsg').html(ptMsg + mwMsg + gsMsg + stMsg);

                        Fanex.ExtAcc.UnBlockUI();
                    }, 2000);
                }
                else {
                    if (Fanex.ExtAcc.Customer.LiveCasino.CustIdIndex < Fanex.ExtAcc.Customer.CustIds.split(',').length - 2) {
                        Fanex.ExtAcc.Customer.LiveCasino.SubmitMemberPT(++Fanex.ExtAcc.Customer.LiveCasino.CustIdIndex);
                    }
                    else {
                        Fanex.ExtAcc.Customer.LiveCasino.CustIdIndex = 0;

                        $('#errMsg').show();
                        $('#errMsg').html(Fanex.ExtAcc.Customer.LiveCasino.Error);

                        Fanex.ExtAcc.Customer.LiveCasino.Error = '';

                        Fanex.ExtAcc.UnBlockUI();
                    }
                }
            }
        })
        .error(function (data) {
            if (typeof console != 'undefined') {
                console.log(data);
            }

            Fanex.ExtAcc.UnBlockUI();
        });
    },

    SuperUpdateMemberPT: function (i) {
        Fanex.ExtAcc.BlockUI();

        var isChecked = document.getElementById('chkAutoPT').checked;
        if (document.getElementById('pt_1'))
            Fanex.ExtAcc.Customer.LiveCasino.customer.superPtSetting.presetPT = isChecked ? 0 : $('#pt_1').val();
        Fanex.ExtAcc.Customer.LiveCasino.customer.superPtSetting.autoPresetPT = $('#pt_2').val();
        Fanex.ExtAcc.Customer.LiveCasino.customer.superPtSetting.isAutoPresetPT = isChecked;

        Fanex.ExtAcc.Customer.LiveCasino.customer.customerInfo.groupName = $('#selectedGroupName').html();

        if (i != null) {
            Fanex.ExtAcc.Customer.LiveCasino.customer.customerInfo.userName = Fanex.ExtAcc.Customer.Usernames.split(', ')[i];
        }

        var data = {
            custId: i == null ? Fanex.ExtAcc.Customer.UserId : parseInt(Fanex.ExtAcc.Customer.CustIds.split(',')[i]),
            uplineId: Fanex.ExtAcc.Customer.superId,
            customer: JSON.stringify(Fanex.ExtAcc.Customer.LiveCasino.customer)
        };

        $.ajax({
            url: resolveClientUrl('LiveCasino/SuperUpdateMemberPTPost'),
            data: data,
            type: 'post',
            dataType: 'json'
        })
        .success(function (result) {
            if (result.ErrorCode && result.ErrorCode == '1') {
                $('#errMsg').show();
                $('#errMsg').html(result.ErrorMessage.formatErrorMessage() + '<br />');

                Fanex.ExtAcc.UnBlockUI();
            }
            else {
                var msg = Fanex.ExtAcc.Customer.LiveCasino.RenderPtCommErrorMessage(result);
                Fanex.ExtAcc.Customer.LiveCasino.Error += '<font color="Blue" style="font-weight: bold;">' + Fanex.ExtAcc.Customer.Usernames.split(', ')[i] + ':</font><br />' + msg + '<br />';

                if (!Fanex.ExtAcc.Customer.LiveCasino.IsMultiple) {
                    var timeout = setTimeout(function () {
                        clearTimeout(timeout);


                        $('#errMsg').show();
                        $('#errMsg').html(msg);

                        Fanex.ExtAcc.UnBlockUI();
                    }, 2000);
                }
                else {
                    if (Fanex.ExtAcc.Customer.LiveCasino.CustIdIndex < Fanex.ExtAcc.Customer.CustIds.split(',').length - 2) {
                        Fanex.ExtAcc.Customer.LiveCasino.SuperUpdateMemberPT(++Fanex.ExtAcc.Customer.LiveCasino.CustIdIndex);
                    }
                    else {
                        Fanex.ExtAcc.Customer.LiveCasino.CustIdIndex = 0;

                        $('#errMsg').show();
                        $('#errMsg').html(Fanex.ExtAcc.Customer.LiveCasino.Error);

                        Fanex.ExtAcc.Customer.LiveCasino.Error = '';

                        Fanex.ExtAcc.UnBlockUI();
                    }
                }
            }
        })
        .error(function (data) {
            if (typeof console != 'undefined') {
                console.log(data);
            }

            Fanex.ExtAcc.UnBlockUI();
        })
    },

    SuperSubmitMemberPT: function (i) {
        Fanex.ExtAcc.BlockUI();

        // Set params
        if (i != null) {
            Fanex.ExtAcc.Customer.LiveCasino.customer.customerInfo.userName = Fanex.ExtAcc.Customer.Usernames.split(', ')[i];
        }

        var data = {
            custId: i == null ? Fanex.ExtAcc.Customer.UserId : parseInt(Fanex.ExtAcc.Customer.CustIds.split(',')[i]),
            uplineId: Fanex.ExtAcc.Customer.superId,
            customer: JSON.stringify(Fanex.ExtAcc.Customer.LiveCasino.customer)
        };

        $.ajax({
            url: resolveClientUrl('LiveCasino/SuperUpdateMemberLiveCasinoPost'),
            type: 'post',
            data: data,
            dataType: 'json'
        })
        .success(function (result) {
            if (result.ErrorCode && result.ErrorCode == '1') {
                $('#errMsg').show();
                $('#errMsg').html(result.ErrorMessage.formatErrorMessage());

                Fanex.ExtAcc.UnBlockUI();
            }
            else {
                var pt = result[0];
                var mw = result[1];
                var gs = result[2];
                var ptMsg = '', mwMsg = '', gsMsg = '';

                // PT messages
                ptMsg = Fanex.ExtAcc.Customer.LiveCasino.RenderPtCommErrorMessage(pt);

                // Member win limit messages
                mwMsg = Fanex.ExtAcc.Customer.LiveCasino.RenderMemberWinLimitMessage(mw);

                // Game setting messages
                if (gs[0].BettypeId != 0) {
                    gsMsg = Fanex.ExtAcc.Customer.LiveCasino.RenderGameSettingMessage(gs);
                }

                Fanex.ExtAcc.Customer.LiveCasino.Error += '<font color="Blue" style="font-weight: bold;">' + Fanex.ExtAcc.Customer.Usernames.split(', ')[i] + ':</font><br />' + ptMsg + mwMsg + gsMsg + '<br />';

                if (!Fanex.ExtAcc.Customer.LiveCasino.IsMultiple) {
                    var timeout = setTimeout(function () {
                        clearTimeout(timeout);

                        $('#errMsg').show();
                        $('#errMsg').html(ptMsg + mwMsg + gsMsg);

                        Fanex.ExtAcc.UnBlockUI();
                    }, 2000);
                }
                else {
                    if (Fanex.ExtAcc.Customer.LiveCasino.CustIdIndex < Fanex.ExtAcc.Customer.CustIds.split(',').length - 2) {
                        Fanex.ExtAcc.Customer.LiveCasino.SuperSubmitMemberPT(++Fanex.ExtAcc.Customer.LiveCasino.CustIdIndex);
                    }
                    else {
                        Fanex.ExtAcc.Customer.LiveCasino.CustIdIndex = 0;

                        $('#errMsg').show();
                        $('#errMsg').html(Fanex.ExtAcc.Customer.LiveCasino.Error);

                        Fanex.ExtAcc.Customer.LiveCasino.Error = '';

                        Fanex.ExtAcc.UnBlockUI();
                    }
                }
            }
        })
        .error(function (data) {
            if (typeof console != 'undefined') {
                console.log(data);
            }

            Fanex.ExtAcc.UnBlockUI();
        });
    },

    MasterUpdateMemberPT: function (i) {
        Fanex.ExtAcc.BlockUI();

        var isChecked = document.getElementById('chkAutoPT').checked;
        if (document.getElementById('pt_1'))
            Fanex.ExtAcc.Customer.LiveCasino.customer.masterPtSetting.presetPT = isChecked ? 0 : $('#pt_1').val();
        Fanex.ExtAcc.Customer.LiveCasino.customer.masterPtSetting.autoPresetPT = $('#pt_2').val();
        Fanex.ExtAcc.Customer.LiveCasino.customer.masterPtSetting.isAutoPresetPT = isChecked;
        Fanex.ExtAcc.Customer.LiveCasino.customer.customerInfo.groupName = $('#selectedGroupName').html();

        if (i != null) {
            Fanex.ExtAcc.Customer.LiveCasino.customer.customerInfo.userName = Fanex.ExtAcc.Customer.Usernames.split(', ')[i];
        }

        var data = {
            custId: i == null ? Fanex.ExtAcc.Customer.UserId : parseInt(Fanex.ExtAcc.Customer.CustIds.split(',')[i]),
            uplineId: Fanex.ExtAcc.Customer.masterId,
            customer: JSON.stringify(Fanex.ExtAcc.Customer.LiveCasino.customer)
        };

        $.ajax({
            url: resolveClientUrl('LiveCasino/MasterUpdateMemberPTPost'),
            data: data,
            type: 'post',
            dataType: 'json'
        })
        .success(function (result) {
            if (result.ErrorCode && result.ErrorCode == '1') {
                $('#errMsg').show();
                $('#errMsg').html(result.ErrorMessage.formatErrorMessage() + '<br />');

                Fanex.ExtAcc.UnBlockUI();
            }
            else {
                var msg = Fanex.ExtAcc.Customer.LiveCasino.RenderPtCommErrorMessage(result);
                Fanex.ExtAcc.Customer.LiveCasino.Error += '<font color="Blue" style="font-weight: bold;">' + Fanex.ExtAcc.Customer.Usernames.split(', ')[i] + ':</font><br />' + msg + '<br />';

                if (!Fanex.ExtAcc.Customer.LiveCasino.IsMultiple) {
                    var timeout = setTimeout(function () {
                        clearTimeout(timeout);


                        $('#errMsg').show();
                        $('#errMsg').html(msg);

                        Fanex.ExtAcc.UnBlockUI();
                    }, 2000);
                }
                else {
                    if (Fanex.ExtAcc.Customer.LiveCasino.CustIdIndex < Fanex.ExtAcc.Customer.CustIds.split(',').length - 2) {
                        Fanex.ExtAcc.Customer.LiveCasino.MasterUpdateMemberPT(++Fanex.ExtAcc.Customer.LiveCasino.CustIdIndex);
                    }
                    else {
                        Fanex.ExtAcc.Customer.LiveCasino.CustIdIndex = 0;

                        $('#errMsg').show();
                        $('#errMsg').html(Fanex.ExtAcc.Customer.LiveCasino.Error);

                        Fanex.ExtAcc.Customer.LiveCasino.Error = '';

                        Fanex.ExtAcc.UnBlockUI();
                    }
                }
            }
        })
        .error(function (data) {
            if (typeof console != 'undefined') {
                console.log(data);
            }

            Fanex.ExtAcc.UnBlockUI();
        })
    },

    MasterSubmitMemberPT: function (i) {
        Fanex.ExtAcc.BlockUI();

        // Set params
        if (i != null) {
            Fanex.ExtAcc.Customer.LiveCasino.customer.customerInfo.userName = Fanex.ExtAcc.Customer.Usernames.split(', ')[i];
        }

        var data = {
            custId: i == null ? Fanex.ExtAcc.Customer.UserId : parseInt(Fanex.ExtAcc.Customer.CustIds.split(',')[i]),
            uplineId: Fanex.ExtAcc.Customer.masterId,
            customer: JSON.stringify(Fanex.ExtAcc.Customer.LiveCasino.customer)
        };

        $.ajax({
            url: resolveClientUrl('LiveCasino/MasterUpdateMemberLiveCasinoPost'),
            type: 'post',
            data: data,
            dataType: 'json'
        })
        .success(function (result) {
            if (result.ErrorCode && result.ErrorCode == '1') {
                $('#errMsg').show();
                $('#errMsg').html(result.ErrorMessage.formatErrorMessage());

                Fanex.ExtAcc.UnBlockUI();
            }
            else {
                var pt = result[0];
                var mw = result[1];
                var gs = result[2];
                var ptMsg = '', mwMsg = '', gsMsg = '';

                // PT messages
                ptMsg = Fanex.ExtAcc.Customer.LiveCasino.RenderPtCommErrorMessage(pt);

                // Member win limit messages
                mwMsg = Fanex.ExtAcc.Customer.LiveCasino.RenderMemberWinLimitMessage(mw);

                // Game setting messages
                if (gs[0].BettypeId != 0) {
                    gsMsg = Fanex.ExtAcc.Customer.LiveCasino.RenderGameSettingMessage(gs);
                }

                Fanex.ExtAcc.Customer.LiveCasino.Error += '<font color="Blue" style="font-weight: bold;">' + Fanex.ExtAcc.Customer.Usernames.split(', ')[i] + ':</font><br />' + ptMsg + mwMsg + gsMsg + '<br />';

                if (!Fanex.ExtAcc.Customer.LiveCasino.IsMultiple) {
                    var timeout = setTimeout(function () {
                        clearTimeout(timeout);

                        $('#errMsg').show();
                        $('#errMsg').html(ptMsg + mwMsg + gsMsg);

                        Fanex.ExtAcc.UnBlockUI();
                    }, 2000);
                }
                else {
                    if (Fanex.ExtAcc.Customer.LiveCasino.CustIdIndex < Fanex.ExtAcc.Customer.CustIds.split(',').length - 2) {
                        Fanex.ExtAcc.Customer.LiveCasino.MasterSubmitMemberPT(++Fanex.ExtAcc.Customer.LiveCasino.CustIdIndex);
                    }
                    else {
                        Fanex.ExtAcc.Customer.LiveCasino.CustIdIndex = 0;

                        $('#errMsg').show();
                        $('#errMsg').html(Fanex.ExtAcc.Customer.LiveCasino.Error);

                        Fanex.ExtAcc.Customer.LiveCasino.Error = '';

                        Fanex.ExtAcc.UnBlockUI();
                    }
                }
            }
        })
        .error(function (data) {
            if (typeof console != 'undefined') {
                console.log(data);
            }

            Fanex.ExtAcc.UnBlockUI();
        });
    },

    ShowCurrentWinLimit: function () {
        $('#showWinLimit').hide();

        Fanex.ExtAcc.BlockUI();

        var data = {
            custId: Fanex.ExtAcc.Customer.UserId
        };

        $.ajax({
            url: resolveClientUrl('LiveCasino/GetMemberWinLimit'),
            type: 'post',
            data: data,
            dataType: 'json'
        })
        .success(function (result) {
            if (result.currentWinLimit == 0 && result.maxWinLimit == 0 && result.totalWinLimit == 0) {
                $('#currentWinLimit').html('Unlimited');
                $('#ofMaxCredit').html('');
            }
            else {
                var value = result.currentWinLimit >= 100 ? '<font color="Red">' + String.prototype.FormatNumber(result.currentWinLimit) + '%</font>' : String.prototype.FormatNumber(result.currentWinLimit) + '%';
                var ofTotal = '&nbsp;of ' + String.prototype.FormatNumber(result.totalWinLimit);

                $('#currentWinLimit').html(value);
                $('#ofMaxCredit').html(ofTotal);
            }

            $('#winLimit').show();

            Fanex.ExtAcc.UnBlockUI();
        })
        .error(function (data) {
            if (typeof console != 'undefined') {
                console.log(data);
            }

            $.unblockUI(document)
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
        opt.text = opt.value = (selectedValue.toFixed(2) * 100)/100;
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
            PT_RenderCombo(pt3, 0, (Fanex.ExtAcc.Customer.LiveCasino.customer.uplinePtSetting.maxPT - parseFloat(pt1.options[pt1.selectedIndex].value).toFixed(2)), 0.01, pt3.options[pt3.selectedIndex].value);
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
        var uplinePT = Fanex.ExtAcc.Customer.LiveCasino.customer.uplinePtSetting;

        if (cur == 'undefined' || cur == 'NaN') {
            cur = 0;
        }

        if (cur > uplinePT.maxPT - sel) {
            cur = uplinePT.maxPT - sel;
        }

        PT_RenderCombo(document.getElementById("pt_3"), 0, uplinePT.maxPT - sel, 0.01, cur != 0 ? (cur.toFixed(2) * 100) / 100 : 0);
    }
    if (document.getElementById("pt_4")) {
        PT_RenderCombo2(document.getElementById("pt_4"), 0, uplinePT.maxPT - sel, 0.01);
    }
}

function PT_RenderCombo2(combo, min, max, step) {
    min = min > max ? max : min;

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