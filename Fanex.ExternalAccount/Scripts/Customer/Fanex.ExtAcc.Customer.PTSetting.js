if (typeof Fanex == 'undefined' || typeof Fanex.ExtAcc == 'undefined') {
    alert("Fanex.ExtAcc.js module is required!")
}

var UMError = 90014;

Validators.show = function (elementInput, message, xPos, pointerXPos, yPos, pointerYPos, height, timeout) {
    // <span class="hint"><span>This is the message.<span><span class="hint-pointer">&nbsp;</span></span>
    var hideTimeout = timeout ? timeout : 1000;
    var hint = document.getElementById('fHint');
    var hintPointer = null;
    if (hint == null) {
        var hint = document.createElement("SPAN");
        hint.className = 'hint';
        var hintMsg = document.createElement("SPAN");
        hint.appendChild(hintMsg);
        hintPointer = document.createElement("SPAN");
        hintPointer.className = 'hint-pointer';
        hint.appendChild(hintPointer);
        hint.id = 'fHint';
        hint.style.top = '10px';
        hint.style.left = '10px';
        hint.style.zIndex = 999999;
        hint.onclick = function () { // Hide onclick
            hint.style.display = 'none';
        };
        document.body.appendChild(hint);
    };
    hintPointer = hint.lastChild;
    hint.firstChild.innerHTML = message;
    if (this.timeOutHideTooltip) {
        clearTimeout(this.timeOutHideTooltip);
        this.timeOutHideTooltip = null;
    };
    this.timeOutHideTooltip = setTimeout(function () {
        hint.style.display = 'none';
    }, hideTimeout); // Auto hide in 1s

    hint.style.display = 'block';
    if (elementInput != null) {
        if (typeof xPos == 'undefined') {
            xPos = 0;
        }
        if (typeof pointerXPos == 'undefined') {
            pointerXPos = 10;
        }
        if (typeof yPos == 'undefined') {
            yPos = -45;
        }
        if (typeof (pointerYPos) == 'undefined') {
            pointerYPos = 38;
        }
        if (typeof (height) == 'undefined') {
            height = 18;
        }

        var pos = Validators.utils.findPosRelativeToViewport(elementInput);
        if (pos[0] + hint.offsetWidth > document.body.offsetWidth) {
            hintPointer.style.left = 'auto';
            hintPointer.style.right = '10px';
            hint.style.left = 'auto';
            hint.style.right = document.body.offsetLeft + document.body.offsetWidth - pos[0] - elementInput.offsetWidth + 'px';
        } else {
            hintPointer.style.left = '10px';
            hintPointer.style.right = 'auto';
            hint.style.left = pos[0] + 'px';
            hint.style.right = 'auto';
        }

        hint.style.height = height + 'px';
        hintPointer.style.top = pointerYPos + 'px';
        hint.style.top = (pos[1] + yPos < 0 ? 0 : pos[1] + yPos) + 'px';

        elementInput.onchange = function () {
            hint.style.display = 'none';
        };
        elementInput.focus();
    }
};

Fanex.ExtAcc.Customer.PTSetting = {
    IsMultiple: Fanex.ExtAcc.Customer.IsMultiple,
    IsFullInfo: Fanex.ExtAcc.Customer.IsFullInfo,
    ptGrid: null,
    context: null,
    customer: null,
    bettingLimit: null,
    Saved: false,
    IsUpdateAllMember: false,
    NewPtTaking: 0.0,
    errorMessage: '',
    CustIdIndex: 0,
    BetSettings: ['min bet', 'max bet', 'max per match', 'max per ball'],

    Init: function () {
        // Extend some methods of PTEngine Core.js
        extendPTEngine();
        if (this.IsFullInfo) {
            var interval = setInterval(function () {
                if (Fanex.ExtAcc.Customer.PTSetting.customer) {
                    clearInterval(interval);
                    Fanex.ExtAcc.Customer.PTSetting.bettingLimit = new Fanex.PTEngine.BettingLimit(Fanex.ExtAcc.Customer.PTSetting.customer.bettingLimit)
                    var bettingLimitContainer = document.getElementById('betSetting');

                    /* Render Commission && bet setting*/
                    if (!Fanex.ExtAcc.Customer.isSuperEditMember && !Fanex.ExtAcc.Customer.isMasterEditMember) {
                        Fanex.ExtAcc.Customer.PTSetting.bettingLimit.Render(bettingLimitContainer);
                    }
                    else {
                        Fanex.ExtAcc.Customer.PTSetting.bettingLimit.Render(bettingLimitContainer, true);
                    }
                }
            }, 100);
        }

        if (Fanex.ExtAcc.Customer.CurrentLevel == 4) {
            if (!Fanex.ExtAcc.Customer.PTSetting.IsMultiple) {
                $('#masterSection').remove();
                $('#agentSection').remove();
                $('#agentSection2').remove();
                $('#agentSection3').remove();

                $('#memberSection').remove();
                $('#memberSection2').remove();
                $('#memberSection3').remove();
            }

            //this.GetSuperPTSetting();
        }
        else if (Fanex.ExtAcc.Customer.CurrentLevel == 3) {
            if (!Fanex.ExtAcc.Customer.PTSetting.IsMultiple) {
                $('#agentSection').remove();
                $('#agentSection2').remove();
                $('#agentSection3').remove();

                $('#memberSection').remove();
                $('#memberSection2').remove();
                $('#memberSection3').remove();
            }

            this.GetMasterPTSetting();
        }
        else if (Fanex.ExtAcc.Customer.CurrentLevel == 2) {
            if (!Fanex.ExtAcc.Customer.PTSetting.IsMultiple) {
                $('#masterSection').remove();
                $('#masterStatus').remove();

                $('#memberSection').remove();
                $('#memberSection2').remove();
                $('#memberSection3').remove();
            }

            this.GetAgentPTSetting();
        }
        else {
            if (!Fanex.ExtAcc.Customer.PTSetting.IsMultiple) {
                $('#masterSection').remove();
                $('#masterStatus').remove();

                $('#agentSection').remove();
                $('#agentSection2').remove();
                $('#agentSection3').remove();
            }
            if (Fanex.ExtAcc.Customer.isSuperEditMember == true) {
                this.SuperGetMemberPTSetting();
            }
            else if (Fanex.ExtAcc.Customer.isMasterEditMember == true) {
                this.MasterGetMemberPTSetting();
            }
            else {
                this.GetMemberPTSetting();
            }
        }

        if (Fanex.ExtAcc.Customer.CurrentLevel != 4) {
            $('#superSection').remove();
        }

        if (Fanex.ExtAcc.Customer.PTSetting.IsMultiple) {
            $('#custInfo').remove();

            Fanex.ExtAcc.Customer.ProcessDisplayMultiple();

            // Render status, comm, bet setting
            if (Fanex.ExtAcc.Customer.isSuperEditMember == true || Fanex.ExtAcc.Customer.isMasterEditMember == true) {
                var interval = setInterval(function () {
                    if (Fanex.ExtAcc.Customer.PTSetting.customer) {
                        clearInterval(interval);

                        // Generate checkboxes
                        Fanex.ExtAcc.Customer.PTSetting.GenerateCheckBoxStatus();
                        Fanex.ExtAcc.Customer.PTSetting.GenerateCheckBoxPositionTaking();
                        initPositionTaking();
                    }
                }, 100);
            }
            else {
                var interval = setInterval(function () {
                    if (Fanex.ExtAcc.Customer.PTSetting.customer) {
                        clearInterval(interval);

                        Fanex.ExtAcc.Customer.PTSetting.bettingLimit = new Fanex.PTEngine.BettingLimit(Fanex.ExtAcc.Customer.PTSetting.customer.bettingLimit)
                        var bettingLimitContainer = document.getElementById('betSetting');
                        Fanex.ExtAcc.Customer.PTSetting.bettingLimit.Render(bettingLimitContainer);

                        // Generate checkboxes
                        Fanex.ExtAcc.Customer.PTSetting.GenerateCheckBoxStatus();
                        Fanex.ExtAcc.Customer.PTSetting.GenerateCheckBoxCommission();
                        Fanex.ExtAcc.Customer.PTSetting.GenerateCheckBoxBettingLimit();
                        initBettingLimit();
                        Fanex.ExtAcc.Customer.PTSetting.GenerateCheckBoxPositionTaking();
                        initPositionTaking();
                    }
                }, 100);
            }
        }
        else {
            $('#statusHeader').remove();
            $('#statusSection').remove();
        }

        if (!this.IsMultiple && this.IsFullInfo) {
            $('#arrowUpDown').remove();

            Fanex.ExtAcc.Customer.ProcessConfirmClosedStatus();
        }

        if (this.IsMultiple) {
            Fanex.ExtAcc.Customer.ProcessConfirmClosedStatus();
        }
    },

    InsertUsernameIntoTitle: function (customer) {
        Fanex.ExtAcc.AddPtInfoPage(customer.custId);

        if (this.IsMultiple)
            return;

        if ($('.headerTab').length > 0)
            $('.headerTab').html($('.headerTab').html() + ': ' + customer.userName);
        else
            $('.headerPositionTaking').html($('.headerPositionTaking').html() + ': ' + customer.userName);
    },

    GetSuperPTSetting: function () {
        Fanex.ExtAcc.BlockUI();

        Fanex.ExtAcc.Customer.PTSetting.errorMessage = '';

        var data = new Object();

        data.custId = Fanex.ExtAcc.Customer.UserId;
        data.parentId = Fanex.ExtAcc.Customer.superId;
        data.groupName = Fanex.ExtAcc.Customer.GroupName().groupName;
        data.roleId = Fanex.ExtAcc.Customer.CurrentLevel;

        $.ajax({
            url: resolveClientUrl('Customer/EditCustomer'),
            data: data,
            dataType: 'json',
            type: 'post'
        })
            .success(function (result) {
                Fanex.PTEngine.PT_STEP = $('#PT3DecimalSites').val().splitAndContains(result.customer.siteLabel) ? 0.005 : 0.01;
                if (result.customer.custId == 0) {
                    console.log(result.customer.custId);
                    Fanex.ExtAcc.UnBlockUI();
                    Fanex.ExtAcc.Form.Close();
                    var timeout = setTimeout(function () {
                        clearTimeout(timeout);
                        Fanex.ExtAcc.Form.Alert('Error', "There is no information available".formatErrorMessage(), 450, 150);
                    }, 1000);

                    return;
                }

                if (result.errorCode && result.errorCode == "1") {
                    Fanex.ExtAcc.UnBlockUI();
                    Fanex.ExtAcc.Form.Close();
                    var timeout = setTimeout(function () {
                        clearTimeout(timeout);
                        Fanex.ExtAcc.Form.Alert('Error', result.errorMessage.formatErrorMessage(), 450, 150);
                    }, 1000);

                    return;
                }

                $('#buttonPopup').remove();
                $('#buttonPopup2').remove();
                $('#arrowUpDown').remove();

                var context = Fanex.ExtAcc.Customer.PTSetting.context = result.context;
                var customer = Fanex.ExtAcc.Customer.PTSetting.customer = Fanex.copy(result.customer);

                Fanex.ExtAcc.Customer.PTSetting.InsertUsernameIntoTitle(customer);

                Fanex.PTEngine.CURRENT_CONTEXT = context;
                Fanex.PTEngine.CURRENT_CONTEXT.ia3 = false;

                Fanex.ExtAcc.Customer.PTSetting.ptGrid = new Fanex.PTEngine.PTGrid(customer.ptSetting);

                var ptContainer = document.getElementById('ptContainer');
                Fanex.ExtAcc.Customer.PTSetting.ptGrid.Render(ptContainer);

                Fanex.ExtAcc.Customer.PTSetting.PTSettingModel = function () {
                }

                ko.applyBindings(new Fanex.ExtAcc.Customer.PTSetting.PTSettingModel());

                var timeout = setTimeout(function () {
                    clearTimeout(timeout);
                    Fanex.ExtAcc.UnBlockUI();
                }, Fanex.ExtAcc.Customer.PTSetting.IsMultiple ? 2000 : 100);
            });
    },

    GenerateCheckBoxStatus: function () {
        document.getElementById('closed').disabled = true;

        var header = document.getElementById('statusHeader');
        var checkBox = document.createElement('input');
        checkBox.setAttribute('type', 'checkbox');
        checkBox.setAttribute('value', 'checkbox');
        checkBox.setAttribute('id', 'status');
        checkBox.onclick = function () {
            if (this.checked) {
                document.getElementById('closed').disabled = false;
            }
            else {
                document.getElementById('closed').disabled = true;
            }
        }

        header.insertBefore(checkBox, header.firstChild);
    },

    GenerateCheckBoxCommission: function () {
        Fanex.ExtAcc.Customer.PTSetting.DisableCommission(true);

        var header = document.getElementById('commissionHeader');
        var checkBoxHeader = document.createElement('input');

        checkBoxHeader.setAttribute('type', 'checkbox');
        checkBoxHeader.setAttribute('value', 'checkbox');
        checkBoxHeader.setAttribute('id', 'commission');
        checkBoxHeader.onclick = function () {
            if (this.checked) {
                Fanex.ExtAcc.Customer.PTSetting.DisableCommission(false);
            }
            else {
                Fanex.ExtAcc.Customer.PTSetting.DisableCommission(true);
            }
        }

        header.insertBefore(checkBoxHeader, header.firstChild);
    },

    DisableCommission: function (isDisable) {
        if (document.getElementById("groupA")) {
            document.getElementById("groupA").disabled = isDisable;
        }
        if (document.getElementById("groupB")) {
            document.getElementById("groupB").disabled = isDisable;
        }
        if (document.getElementById("groupC")) {
            document.getElementById("groupC").disabled = isDisable;
        }
        if (document.getElementById("groupD")) {
            document.getElementById("groupD").disabled = isDisable;
        }
        if (document.getElementById("discountAsian")) {
            document.getElementById("discountAsian").disabled = isDisable;
        }

        if (document.getElementById("discount1x2")) {
            document.getElementById("discount1x2").disabled = isDisable;
        }

        if (document.getElementById("discountCS")) {
            document.getElementById("discountCS").disabled = isDisable;
        }
        if (document.getElementById("discountMyanmarOdds")) {
            document.getElementById("discountMyanmarOdds").disabled = isDisable;
        }
    },

    GenerateCheckBoxBettingLimit: function () {
        var items = Fanex.ExtAcc.Customer.PTSetting.bettingLimit.items;
        // Render for soccer 
        Fanex.ExtAcc.Customer.PTSetting.GenerateCheckBoxBettingLimitForSportContainCopyallIcon(items[0], 0);

        // Render for Basket ball
        Fanex.ExtAcc.Customer.PTSetting.GenerateCheckBoxBettingLimitForSportContainCopyallIcon(items[3], 3);

        // Render for soccer parlay
        Fanex.ExtAcc.Customer.PTSetting.GenerateCheckBoxBettingLimitForSportWithoutCopyallIcon(items[2], 2);

        // Render for Saba Soccer
        Fanex.ExtAcc.Customer.PTSetting.GenerateCheckBoxBettingLimitForSportWithoutCopyallIcon(items[1], 1);

        // Render for the rest
        for (var i = 4; i < items.length; i++) {
            Fanex.ExtAcc.Customer.PTSetting.GenerateCheckBoxBettingLimitForSportWithoutCopyallIcon(items[i], i);
        }
    },

    GenerateCheckBoxBettingLimitForSportContainCopyallIcon: function (sportItem, sportItemIndex) {
        var checkBox = document.createElement('input');
        checkBox.setAttribute('type', 'checkbox');
        checkBox.setAttribute('value', 'checkbox');
        checkBox.setAttribute('id', 'bettingLimit' + sportItemIndex);
        checkBox.setAttribute('title', Language.CHECKBOX_TOOLTIP);
        checkBox.onclick = function (event) {
            var items = Fanex.ExtAcc.Customer.PTSetting.bettingLimit.items;
            var item = this.id.toString();
            var id = item.substring(12, item.length);
            if (this.checked) {
                reEnableChildren(items[id].element, 0);
            }
            else {
                disableChildren(items[id].element, "BetTypeInactive");
                var headerSoccer = items[id].header;
                var count = parseInt(id);
                if (items[id].active === true) { // Uncheck --> Checked
                    items[id].active = false;
                    headerSoccer.className = "BettingLimitHeaderCopied";
                    for (var i = count + 1; i < items.length; i++) {
                        items[i].copiedItem = null; // Remove copiedItem
                        if (items[i].inactive === false) break;
                        items[i].inactive = false;
                        var checkBox = 'bettingLimit' + i;
                        $(checkBox).checked = false;
                        if ($(checkBox).checked) {
                            Fanex.PTEngine.ReEnableChildren(items[i].element, 0);
                        }
                        else {
                            reEnableChildren(items[i].element, 1);
                        }
                    }
                }
            }

            //Disable event click of parent
            if (typeof (event) == 'undefined') {
                event = window.event;
            }

            event.cancelBubble = true;
            if (event.stopPropagation) {
                event.stopPropagation();
            }
        }

        sportItem.element.childNodes[0].insertBefore(checkBox, sportItem.element.childNodes[0].firstChild);
    },

    GenerateCheckBoxBettingLimitForSportWithoutCopyallIcon: function (sportItem, sportItemIndex) {
        var checkBox = document.createElement('input');
        checkBox.setAttribute('type', 'checkbox');
        checkBox.setAttribute('value', 'checkbox');
        checkBox.setAttribute('id', 'bettingLimit' + sportItemIndex);
        checkBox.setAttribute('title', Language.CHECKBOX_TOOLTIP);
        checkBox.onclick = function () {
            var items = Fanex.ExtAcc.Customer.PTSetting.bettingLimit.items;
            var item = this.id.toString();
            var id = item.substring(12, 13);
            if (this.checked) {
                reEnableChildren(sportItem.element, 0);
            }
            else {
                disableChildren(sportItem.element, "BetTypeInactive");
            }
        }

        sportItem.element.childNodes[0].insertBefore(checkBox, sportItem.element.childNodes[0].firstChild);
    },

    GenerateCheckBoxPositionTaking: function () {
        var ignoreCopySports = [161];
        var j = 0;
        var timeout = setTimeout(function () {
            clearTimeout(timeout);
            var doc = document.getElementById('ptContainer');
            var sportTypes = Fanex.ExtAcc.Customer.PTSetting.ptGrid.items;
            for (var i = 0; i < sportTypes.length; i++) {
                var checkBox = document.createElement('input');
                checkBox.setAttribute('type', 'checkbox');
                checkBox.setAttribute('value', 'checkbox');
                checkBox.setAttribute('id', 'positionTaking' + i);
                checkBox.onclick = function (event) {
                    var items = Fanex.ExtAcc.Customer.PTSetting.ptGrid.items;
                    var item = this.id.toString();
                    var id = item.substring(14);

                    if (this.checked) {
                        disableSportType(id, false);
                    }
                    else {
                        disableSportType(id, true);
                        if (!ignoreCopySports.includes(sportTypes[id].id)) {
                            var cloneFromSoccer = items[0].cloneButton;
                            var cloneFromBasketball = items[1].cloneButton;
                            cloneFromSoccer.parent.copiedItem = null;
                            cloneFromBasketball.parent.copiedItem = null;
                            if (cloneFromSoccer.firstChild.checked) {
                                cloneFromSoccer.firstChild.checked = false;
                                cloneFromSoccer.className = "PTSportCopied";
                                for (var i = 1; i < items.length; i++) {
                                    var idPT = 'positionTaking' + i;
                                    if (i > 1) {
                                        items[i].cloneButton.className = "PTSportCopiedInactive";
                                        items[i].cloneButton.title = String.Empty;
                                    }

                                    document.getElementById(idPT).disabled = false;
                                    disableSportType(i, true);
                                };

                                items[1].editButton.className = "PTSportCopiedInactive";
                                items[1].editButton.title = String.Empty;
                            }

                            if (cloneFromBasketball.firstChild.checked) {
                                cloneFromBasketball.firstChild.checked = false;
                                cloneFromBasketball.className = "PTSportCopied";
                                for (var i = 2; i < items.length; i++) {
                                    items[i].cloneButton.className = "PTSportCopiedInactive";
                                    items[i].cloneButton.title = String.Empty;

                                    var idPT = 'positionTaking' + i;
                                    document.getElementById(idPT).disabled = false;
                                    disableSportType(i, true);
                                };
                            }
                        }
                    }

                    //Disable event click of parent
                    if (typeof (event) == 'undefined') {
                        event = window.event;
                    }

                    event.cancelBubble = true;
                    if (event.stopPropagation) {
                        event.stopPropagation();
                    }
                }

                if (doc.childNodes[0].childNodes[j])
                    doc.childNodes[0].childNodes[j].childNodes[0].insertBefore(checkBox, doc.childNodes[0].childNodes[j].childNodes[0].firstChild);
                else
                    doc.childNodes[1].childNodes[j].childNodes[0].insertBefore(checkBox, doc.childNodes[1].childNodes[j].childNodes[0].firstChild);

                j += 2;
            }
        }, 2000);
    },

    SuperUpdateMasterPT: function (custId, data, i, customer) {
        if (!i) {
            data.custId = custId;
        }
        else {
            customer.custId = parseInt(Fanex.ExtAcc.Customer.CustIds.split(',')[i]);
            data.txtCustomerObject = JSON.stringify(customer).escapeHtml();
            data.custId = parseInt(Fanex.ExtAcc.Customer.CustIds.split(',')[i]);
        }

        data.groupName = Fanex.ExtAcc.Customer.GroupName().groupName;

        $.ajax({
            url: resolveClientUrl('SportsbookSetting/SuperUpdateMasterPTPost'),
            data: data,
            type: 'post'
        })
            .success(function (result) {

                if (Fanex.ExtAcc.ShowSecurityCodePage(result, Fanex.ExtAcc.UnBlockUI)) {
                    return;
                }

                if (result.errorCode && (result.errorCode == '1' || result.errorCode == UMError)) {
                    $('#errMsg').show();
                    $('#errMsg').html(result.errorMessage.formatErrorMessage() + '<br />');

                    Fanex.ExtAcc.UnBlockUI();
                }
                else {
                    Fanex.ExtAcc.Customer.PTSetting.CustIdIndex++;

                    var codes = result.errorCode.split('\u0007');
                    var msgs = result.errorMessage.split('\u0007');
                    var success = true;
                    var error = '';

                    for (var i = 0; i < codes.length; i++) {
                        if (parseInt(codes[i]) > 0 && parseInt(codes[i]) != 1000) {
                            success = false;

                            error += msgs[i] + '<br/>';
                            Fanex.ExtAcc.Customer.PTSetting.errorMessage += msgs[i].formatErrorMessage() + '<br/>';
                        }
                    }

                    if (success) {

                        if (!Fanex.ExtAcc.Customer.PTSetting.IsMultiple) {
                            var timeout = setTimeout(function () {
                                Fanex.ExtAcc.Customer.PTSetting.CustIdIndex = 0;
                                clearTimeout(timeout);
                                Fanex.ExtAcc.UnBlockUI();

                                if (Fanex.ExtAcc.Customer.PTSetting.IsFullInfo && document.getElementById('customerListGrid')) {
                                    Fanex.ExtAcc.Form.Close(false);
                                    Fanex.ExtAcc.Customer.CallBackCustomerList();
                                }
                                else {
                                    $('#errMsg').css('display', 'block');
                                    $('#errMsg').addClass('successMsg').removeClass('errMsg');
                                    $('#errMsg').html('<font color="Blue">Updated successfully</font>');
                                }

                            }, 2000);
                        }
                        else {
                            if (Fanex.ExtAcc.Customer.PTSetting.CustIdIndex < Fanex.ExtAcc.Customer.CustIds.split(',').length - 1) {
                                Fanex.ExtAcc.Customer.PTSetting.SuperUpdateMasterPT(null, data, Fanex.ExtAcc.Customer.PTSetting.CustIdIndex, customer);
                            }
                            else {
                                if (Fanex.ExtAcc.Customer.PTSetting.errorMessage.length == 0) {
                                    var timeout = setTimeout(function () {
                                        Fanex.ExtAcc.Customer.PTSetting.CustIdIndex = 0;
                                        clearTimeout(timeout);
                                        Fanex.ExtAcc.UnBlockUI();

                                        if (document.getElementById('customerListGrid')) {
                                            Fanex.ExtAcc.Form.Close(false);
                                            Fanex.ExtAcc.Customer.CallBackCustomerList();
                                        }
                                        else {
                                            $('#errMsg').css('display', 'block');
                                            $('#errMsg').addClass('successMsg').removeClass('errMsg');
                                            $('#errMsg').html('<font color="Blue">Updated successfully</font>');
                                        }
                                    }, 2000);
                                }
                                else {
                                    Fanex.ExtAcc.UnBlockUI();

                                    $('#errMsg').css('display', 'block');
                                    $('#errMsg').html(Fanex.ExtAcc.Customer.PTSetting.errorMessage);
                                }

                                Fanex.ExtAcc.Customer.PTSetting.errorMessage = '';
                                Fanex.ExtAcc.Customer.PTSetting.CustIdIndex = 0;
                            }
                        }
                    }
                    else {
                        if (!Fanex.ExtAcc.Customer.PTSetting.IsMultiple) {
                            Fanex.ExtAcc.Customer.PTSetting.CustIdIndex = 0;
                            Fanex.ExtAcc.UnBlockUI();

                            /*
                            var hasBetSettingError = false;
    
                            for (var i = 0; i < Fanex.ExtAcc.Customer.PTSetting.BetSettings.length; i++) {
                            if (Fanex.ExtAcc.Customer.PTSetting.errorMessage.indexOf(Fanex.ExtAcc.Customer.PTSetting.BetSettings[i]) != -1) {
                            hasBetSettingError = true;
                            break;
                            }
                            }
                            */
                            if (Fanex.ExtAcc.Customer.PTSetting.errorMessage == '') {
                                Fanex.ExtAcc.Form.Close(false);
                            }
                            else {
                                $('#errMsg').css('display', 'block');
                                $('#errMsg').html(Fanex.ExtAcc.Customer.PTSetting.errorMessage);
                            }

                            Fanex.ExtAcc.Customer.PTSetting.errorMessage = '';
                            Fanex.ExtAcc.Customer.PTSetting.CustIdIndex = 0;
                        }
                        else {
                            if (Fanex.ExtAcc.Customer.PTSetting.CustIdIndex < Fanex.ExtAcc.Customer.CustIds.split(',').length - 1) {
                                Fanex.ExtAcc.Customer.PTSetting.SuperUpdateMasterPT(null, data, Fanex.ExtAcc.Customer.PTSetting.CustIdIndex, customer);
                            }
                            else {
                                if (Fanex.ExtAcc.Customer.PTSetting.errorMessage.length == 0) {
                                    var timeout = setTimeout(function () {
                                        Fanex.ExtAcc.Customer.PTSetting.CustIdIndex = 0;
                                        clearTimeout(timeout);
                                        Fanex.ExtAcc.UnBlockUI();

                                        Fanex.ExtAcc.Form.Close(false);
                                    }, 2000);
                                }
                                else {
                                    Fanex.ExtAcc.UnBlockUI();

                                    $('#errMsg').css('display', 'block');
                                    $('#errMsg').html(Fanex.ExtAcc.Customer.PTSetting.errorMessage);
                                }

                                Fanex.ExtAcc.Customer.PTSetting.errorMessage = '';
                                Fanex.ExtAcc.Customer.PTSetting.CustIdIndex = 0;
                            }
                        }
                    }
                }
            });
    },

    MasterUpdateAgentPT: function (custId, data, i, customer) {
        if (!i) {
            data.custId = custId;
        }
        else {
            customer.custId = parseInt(Fanex.ExtAcc.Customer.CustIds.split(',')[i]);
            data.txtCustomerObject = JSON.stringify(customer).escapeHtml();
            data.custId = parseInt(Fanex.ExtAcc.Customer.CustIds.split(',')[i]);
        }

        data.groupName = Fanex.ExtAcc.Customer.GroupName().groupName;

        $.ajax({
            url: resolveClientUrl('SportsbookSetting/MasterUpdateAgentPTPost'),
            data: data,
            type: 'post'
        })
            .success(function (result) {

                if (Fanex.ExtAcc.ShowSecurityCodePage(result, Fanex.ExtAcc.UnBlockUI)) {
                    return;
                }

                if (result.errorCode && (result.errorCode == '1' || result.errorCode == UMError)) {
                    $('#errMsg').show();
                    $('#errMsg').html(result.errorMessage.formatErrorMessage() + '<br />');

                    Fanex.ExtAcc.UnBlockUI();
                }
                else {
                    Fanex.ExtAcc.Customer.PTSetting.CustIdIndex++;

                    var codes = result.errorCode.split('\u0007');
                    var msgs = result.errorMessage.split('\u0007');
                    var success = true;
                    var error = '';

                    for (var i = 0; i < codes.length; i++) {
                        if (parseInt(codes[i]) > 0 && parseInt(codes[i]) != 1000) {
                            success = false;

                            error += msgs[i] + '<br/>';
                            Fanex.ExtAcc.Customer.PTSetting.errorMessage += msgs[i].formatErrorMessage() + '<br/>';
                        }
                    }

                    if (success) {
                        if (!Fanex.ExtAcc.Customer.PTSetting.IsMultiple) {
                            var timeout = setTimeout(function () {
                                Fanex.ExtAcc.Customer.PTSetting.CustIdIndex = 0;
                                clearTimeout(timeout);
                                Fanex.ExtAcc.UnBlockUI();

                                if (Fanex.ExtAcc.Customer.PTSetting.IsFullInfo && document.getElementById('customerListGrid')) {
                                    Fanex.ExtAcc.Form.Close(false);
                                    Fanex.ExtAcc.Customer.CallBackCustomerList();
                                }
                                else {
                                    $('#errMsg').css('display', 'block');
                                    $('#errMsg').html('<font color="Blue">Updated successfully</font>');
                                }

                            }, 2000);
                        }
                        else {
                            if (Fanex.ExtAcc.Customer.PTSetting.CustIdIndex < Fanex.ExtAcc.Customer.CustIds.split(',').length - 1) {
                                Fanex.ExtAcc.Customer.PTSetting.MasterUpdateAgentPT(null, data, Fanex.ExtAcc.Customer.PTSetting.CustIdIndex, customer);
                            }
                            else {
                                if (Fanex.ExtAcc.Customer.PTSetting.errorMessage.length == 0) {
                                    var timeout = setTimeout(function () {
                                        Fanex.ExtAcc.Customer.PTSetting.CustIdIndex = 0;
                                        clearTimeout(timeout);
                                        Fanex.ExtAcc.UnBlockUI();

                                        if (document.getElementById('customerListGrid')) {
                                            Fanex.ExtAcc.Form.Close(false);
                                            Fanex.ExtAcc.Customer.CallBackCustomerList();
                                        }
                                        else {
                                            $('#errMsg').css('display', 'block');
                                            $('#errMsg').html('<font color="Blue">Updated successfully</font>');
                                        }
                                    }, 2000);
                                }
                                else {
                                    Fanex.ExtAcc.UnBlockUI();

                                    $('#errMsg').css('display', 'block');
                                    $('#errMsg').html(Fanex.ExtAcc.Customer.PTSetting.errorMessage);
                                }

                                Fanex.ExtAcc.Customer.PTSetting.errorMessage = '';
                                Fanex.ExtAcc.Customer.PTSetting.CustIdIndex = 0;
                            }
                        }
                    }
                    else {
                        if (!Fanex.ExtAcc.Customer.PTSetting.IsMultiple) {
                            Fanex.ExtAcc.Customer.PTSetting.CustIdIndex = 0;
                            Fanex.ExtAcc.UnBlockUI();
                            /*
                            var hasBetSettingError = false;
    
                            for (var i = 0; i < Fanex.ExtAcc.Customer.PTSetting.BetSettings.length; i++) {
                            if (Fanex.ExtAcc.Customer.PTSetting.errorMessage.indexOf(Fanex.ExtAcc.Customer.PTSetting.BetSettings[i]) != -1) {
                            hasBetSettingError = true;
                            break;
                            }
                            }
                            */
                            if (Fanex.ExtAcc.Customer.PTSetting.errorMessage == '') {
                                Fanex.ExtAcc.Form.Close(false);
                            }
                            else {
                                $('#errMsg').css('display', 'block');
                                $('#errMsg').html(Fanex.ExtAcc.Customer.PTSetting.errorMessage);
                            }

                            Fanex.ExtAcc.Customer.PTSetting.errorMessage = '';
                            Fanex.ExtAcc.Customer.PTSetting.CustIdIndex = 0;
                        }
                        else {
                            if (Fanex.ExtAcc.Customer.PTSetting.CustIdIndex < Fanex.ExtAcc.Customer.CustIds.split(',').length - 1) {
                                Fanex.ExtAcc.Customer.PTSetting.MasterUpdateAgentPT(null, data, Fanex.ExtAcc.Customer.PTSetting.CustIdIndex, customer);
                            }
                            else {
                                if (Fanex.ExtAcc.Customer.PTSetting.errorMessage.length == 0) {
                                    var timeout = setTimeout(function () {
                                        Fanex.ExtAcc.Customer.PTSetting.CustIdIndex = 0;
                                        clearTimeout(timeout);
                                        Fanex.ExtAcc.UnBlockUI();

                                        Fanex.ExtAcc.Form.Close(false);
                                    }, 2000);
                                }
                                else {
                                    Fanex.ExtAcc.UnBlockUI();

                                    $('#errMsg').css('display', 'block');
                                    $('#errMsg').html(Fanex.ExtAcc.Customer.PTSetting.errorMessage);
                                }

                                Fanex.ExtAcc.Customer.PTSetting.errorMessage = '';
                                Fanex.ExtAcc.Customer.PTSetting.CustIdIndex = 0;
                            }
                        }
                    }
                }
            });
    },

    AgentUpdateMemberPT: function (custId, data, i, customer) {
        if (!i) {
            data.custId = custId;
        }
        else {
            customer.custId = parseInt(Fanex.ExtAcc.Customer.CustIds.split(',')[i]);
            data.txtCustomerObject = JSON.stringify(customer).escapeHtml();
            data.custId = parseInt(Fanex.ExtAcc.Customer.CustIds.split(',')[i]);
        }

        data.groupName = Fanex.ExtAcc.Customer.GroupName().groupName;

        $.ajax({
            url: resolveClientUrl('SportsbookSetting/AgentUpdateMemberPTPost'),
            data: data,
            type: 'post'
        })
            .success(function (result) {

                if (Fanex.ExtAcc.ShowSecurityCodePage(result, Fanex.ExtAcc.UnBlockUI)) {
                    return;
                }

                if (result.errorCode && (result.errorCode == '1' || result.errorCode == UMError)) {
                    $('#errMsg').show();
                    $('#errMsg').html(result.errorMessage.formatErrorMessage() + '<br />');

                    Fanex.ExtAcc.UnBlockUI();
                }
                else {
                    Fanex.ExtAcc.Customer.PTSetting.CustIdIndex++;

                    var codes = result.errorCode.split('\u0007');
                    var msgs = result.errorMessage.split('\u0007');
                    var success = true;
                    var error = '';

                    for (var i = 0; i < codes.length; i++) {
                        if (parseInt(codes[i]) > 0 && parseInt(codes[i]) != 1000) {
                            success = false;

                            error += msgs[i] + '<br/>';
                            Fanex.ExtAcc.Customer.PTSetting.errorMessage += msgs[i].formatErrorMessage() + '<br/>';
                        }
                    }

                    if (success) {
                        if (!Fanex.ExtAcc.Customer.PTSetting.IsMultiple) {
                            var timeout = setTimeout(function () {
                                Fanex.ExtAcc.Customer.PTSetting.CustIdIndex = 0;
                                clearTimeout(timeout);
                                Fanex.ExtAcc.UnBlockUI();

                                if (Fanex.ExtAcc.Customer.PTSetting.IsFullInfo && document.getElementById('customerListGrid')) {
                                    Fanex.ExtAcc.Form.Close(false);
                                    Fanex.ExtAcc.Customer.CallBackCustomerList();
                                }
                                else {
                                    $('#errMsg').css('display', 'block');
                                    $('#errMsg').addClass('successMsg').removeClass('errMsg');
                                    $('#errMsg').html('<font color="Blue">Updated successfully</font>');
                                }
                            }, 2000);
                        }
                        else {
                            if (Fanex.ExtAcc.Customer.PTSetting.CustIdIndex < Fanex.ExtAcc.Customer.CustIds.split(',').length - 1) {
                                Fanex.ExtAcc.Customer.PTSetting.AgentUpdateMemberPT(null, data, Fanex.ExtAcc.Customer.PTSetting.CustIdIndex, customer);
                            }
                            else {
                                if (Fanex.ExtAcc.Customer.PTSetting.errorMessage.length == 0) {
                                    var timeout = setTimeout(function () {
                                        Fanex.ExtAcc.Customer.PTSetting.CustIdIndex = 0;
                                        clearTimeout(timeout);
                                        Fanex.ExtAcc.UnBlockUI();

                                        if (document.getElementById('customerListGrid')) {
                                            Fanex.ExtAcc.Form.Close(false);
                                            Fanex.ExtAcc.Customer.CallBackCustomerList();
                                        }
                                        else {
                                            $('#errMsg').css('display', 'block');
                                            $('#errMsg').addClass('successMsg').removeClass('errMsg');
                                            $('#errMsg').html('<font color="Blue">Updated successfully</font>');
                                        }
                                    }, 2000);
                                }
                                else {
                                    Fanex.ExtAcc.UnBlockUI();

                                    $('#errMsg').css('display', 'block');
                                    $('#errMsg').html(Fanex.ExtAcc.Customer.PTSetting.errorMessage);
                                }

                                Fanex.ExtAcc.Customer.PTSetting.errorMessage = '';
                                Fanex.ExtAcc.Customer.PTSetting.CustIdIndex = 0;
                            }
                        }
                    }
                    else {
                        if (!Fanex.ExtAcc.Customer.PTSetting.IsMultiple) {
                            Fanex.ExtAcc.Customer.PTSetting.CustIdIndex = 0;
                            Fanex.ExtAcc.UnBlockUI();

                            /*
                            var hasBetSettingError = false;
    
                            for (var i = 0; i < Fanex.ExtAcc.Customer.PTSetting.BetSettings.length; i++) {
                            if (Fanex.ExtAcc.Customer.PTSetting.errorMessage.indexOf(Fanex.ExtAcc.Customer.PTSetting.BetSettings[i]) != -1) {
                            hasBetSettingError = true;
                            break;
                            }
                            }
                            */
                            if (Fanex.ExtAcc.Customer.PTSetting.errorMessage == '') {
                                Fanex.ExtAcc.Form.Close(false);
                            }
                            else {
                                $('#errMsg').css('display', 'block');
                                $('#errMsg').html(Fanex.ExtAcc.Customer.PTSetting.errorMessage);
                            }

                            Fanex.ExtAcc.Customer.PTSetting.errorMessage = '';
                            Fanex.ExtAcc.Customer.PTSetting.CustIdIndex = 0;
                        }
                        else {
                            if (Fanex.ExtAcc.Customer.PTSetting.CustIdIndex < Fanex.ExtAcc.Customer.CustIds.split(',').length - 1) {
                                Fanex.ExtAcc.Customer.PTSetting.AgentUpdateMemberPT(null, data, Fanex.ExtAcc.Customer.PTSetting.CustIdIndex, customer);
                            }
                            else {
                                if (Fanex.ExtAcc.Customer.PTSetting.errorMessage.length == 0) {
                                    var timeout = setTimeout(function () {
                                        Fanex.ExtAcc.Customer.PTSetting.CustIdIndex = 0;
                                        clearTimeout(timeout);
                                        Fanex.ExtAcc.UnBlockUI();

                                        Fanex.ExtAcc.Form.Close(false);
                                    }, 2000);
                                }
                                else {
                                    Fanex.ExtAcc.UnBlockUI();

                                    $('#errMsg').css('display', 'block');
                                    $('#errMsg').html(Fanex.ExtAcc.Customer.PTSetting.errorMessage);
                                }

                                Fanex.ExtAcc.Customer.PTSetting.errorMessage = '';
                                Fanex.ExtAcc.Customer.PTSetting.CustIdIndex = 0;
                            }
                        }
                    }
                }
            });
    },

    SuperMasterUpdateMemberPT: function (custId, data, i, customer) {
        var urlPost = '';

        if (Fanex.ExtAcc.Customer.isSuperEditMember) {
            urlPost = 'SportsbookSetting/SuperEditPTMemberPost';
        }
        else {
            urlPost = 'SportsbookSetting/MasterEditPTMemberPost';
        }

        if (!i) {
            data.custId = custId;
        }
        else {
            customer.custId = parseInt(Fanex.ExtAcc.Customer.CustIds.split(',')[i]);
            data.txtCustomerObject = JSON.stringify(customer).escapeHtml();
            data.custId = parseInt(Fanex.ExtAcc.Customer.CustIds.split(',')[i]);
        }

        data.groupName = Fanex.ExtAcc.Customer.GroupName().groupName;

        $.ajax({
            url: resolveClientUrl(urlPost),
            data: data,
            type: 'post'
        })
            .success(function (result) {

                if (Fanex.ExtAcc.ShowSecurityCodePage(result, Fanex.ExtAcc.UnBlockUI)) {
                    return;
                }

                if (result.errorCode && (result.errorCode == '1' || result.errorCode == UMError)) {
                    $('#errMsg').show();
                    $('#errMsg').html(result.errorMessage.formatErrorMessage() + '<br />');

                    Fanex.ExtAcc.UnBlockUI();
                }
                else {
                    Fanex.ExtAcc.Customer.PTSetting.CustIdIndex++;

                    var codes = result.errorCode.split('\u0007');
                    var msgs = result.errorMessage.split('\u0007');
                    var success = true;
                    var error = '';

                    for (var i = 0; i < codes.length; i++) {
                        if (parseInt(codes[i]) > 0 && parseInt(codes[i]) != 1000) {
                            success = false;

                            error += msgs[i] + '<br/>';
                            Fanex.ExtAcc.Customer.PTSetting.errorMessage += msgs[i].formatErrorMessage() + '<br/>';
                        }
                    }

                    if (success) {
                        if (!Fanex.ExtAcc.Customer.PTSetting.IsMultiple) {
                            var timeout = setTimeout(function () {
                                $('#errMsg').css('display', 'block');
                                $('#errMsg').addClass('successMsg').removeClass('errMsg');
                                $('#errMsg').html('<font color="Blue">Updated successfully</font>');

                                Fanex.ExtAcc.Customer.PTSetting.CustIdIndex = 0;
                                clearTimeout(timeout);
                                Fanex.ExtAcc.UnBlockUI();

                                if (Fanex.ExtAcc.Customer.PTSetting.IsFullInfo && document.getElementById('customerListGrid')) {
                                    Fanex.ExtAcc.Customer.CallBackCustomerList();
                                }

                                Fanex.ExtAcc.Form.Close(false);
                            }, 2000);
                        }
                        else {
                            if (Fanex.ExtAcc.Customer.PTSetting.CustIdIndex < Fanex.ExtAcc.Customer.CustIds.split(',').length - 1) {
                                Fanex.ExtAcc.Customer.PTSetting.SuperMasterUpdateMemberPT(null, data, Fanex.ExtAcc.Customer.PTSetting.CustIdIndex, customer);
                            }
                            else {
                                if (Fanex.ExtAcc.Customer.PTSetting.errorMessage.length == 0) {
                                    var timeout = setTimeout(function () {
                                        $('#errMsg').css('display', 'block');
                                        $('#errMsg').addClass('successMsg').removeClass('errMsg');
                                        $('#errMsg').html('<font color="Blue">Updated successfully</font>');

                                        Fanex.ExtAcc.Customer.PTSetting.CustIdIndex = 0;
                                        clearTimeout(timeout);
                                        Fanex.ExtAcc.UnBlockUI();

                                        Fanex.ExtAcc.Form.Close(false);

                                        if (document.getElementById('customerListGrid'))
                                            Fanex.ExtAcc.Customer.CallBackCustomerList();
                                    }, 2000);
                                }
                                else {
                                    Fanex.ExtAcc.UnBlockUI();

                                    $('#errMsg').css('display', 'block');
                                    $('#errMsg').html(Fanex.ExtAcc.Customer.PTSetting.errorMessage);
                                }

                                Fanex.ExtAcc.Customer.PTSetting.errorMessage = '';
                                Fanex.ExtAcc.Customer.PTSetting.CustIdIndex = 0;
                            }
                        }
                    }
                    else {
                        if (!Fanex.ExtAcc.Customer.PTSetting.IsMultiple) {
                            Fanex.ExtAcc.Customer.PTSetting.CustIdIndex = 0;
                            Fanex.ExtAcc.UnBlockUI();

                            var hasBetSettingError = false;

                            for (var i = 0; i < Fanex.ExtAcc.Customer.PTSetting.BetSettings.length; i++) {
                                if (Fanex.ExtAcc.Customer.PTSetting.errorMessage.indexOf(Fanex.ExtAcc.Customer.PTSetting.BetSettings[i]) != -1) {
                                    hasBetSettingError = true;
                                    break;
                                }
                            }

                            if (hasBetSettingError) {
                                Fanex.ExtAcc.Form.Close(false);
                            }
                            else {
                                $('#errMsg').css('display', 'block');
                                $('#errMsg').html(Fanex.ExtAcc.Customer.PTSetting.errorMessage);
                            }
                        }
                        else {
                            if (Fanex.ExtAcc.Customer.PTSetting.CustIdIndex < Fanex.ExtAcc.Customer.CustIds.split(',').length - 1) {
                                Fanex.ExtAcc.Customer.PTSetting.SuperMasterUpdateMemberPT(null, data, Fanex.ExtAcc.Customer.PTSetting.CustIdIndex, customer);
                            }
                            else {
                                if (Fanex.ExtAcc.Customer.PTSetting.errorMessage.length == 0) {
                                    var timeout = setTimeout(function () {
                                        $('#errMsg').css('display', 'block');
                                        $('#errMsg').html('<font color="Blue">Updated successfully</font>');

                                        Fanex.ExtAcc.Customer.PTSetting.CustIdIndex = 0;
                                        clearTimeout(timeout);
                                        Fanex.ExtAcc.UnBlockUI();

                                        Fanex.ExtAcc.Form.Close(false);
                                    }, 2000);
                                }
                                else {
                                    Fanex.ExtAcc.UnBlockUI();

                                    $('#errMsg').css('display', 'block');
                                    $('#errMsg').html(Fanex.ExtAcc.Customer.PTSetting.errorMessage);
                                }

                                Fanex.ExtAcc.Customer.PTSetting.errorMessage = '';
                                Fanex.ExtAcc.Customer.PTSetting.CustIdIndex = 0;
                            }
                        }
                    }
                }
            });
    }
};

var isUpdate = false;
var isUpdatePositionTaking = false;

function getSuperMasterUpdateMemberParamaters(paramaters) {
    var paramaters = "";

    if (document.getElementById("status").checked) {
        paramaters += "1";
        if (!isUpdate) {
            isUpdate = true;
        }
    }
    else {
        paramaters += "0";
    }

    var itemsPt = Fanex.ExtAcc.Customer.PTSetting.ptGrid.items;
    var cloneFromSoccer = itemsPt[0].cloneButton;
    var cloneFromBasketball = itemsPt[1].cloneButton;
    if (document.getElementById("positionTaking0").checked && cloneFromSoccer.firstChild.checked) {
        isUpdatePositionTaking = true;
        for (var i = 0; i < itemsPt.length; i++) {
            paramaters += "1";
            if (!isUpdate) {
                isUpdate = true;
            }
        }
    }
    else if (document.getElementById("positionTaking1").checked && cloneFromBasketball.firstChild.checked) {
        document.getElementById("positionTaking0").checked ? paramaters += "1" : paramaters += "0";
        isUpdatePositionTaking = true;
        for (var i = 1; i < itemsPt.length; i++) {
            paramaters += "1";
            if (!isUpdate) {
                isUpdate = true;
            }
        }
    }
    else {
        for (var i = 0; i < itemsPt.length; i++) {
            if (document.getElementById("positionTaking" + i).checked) {
                paramaters += "1";
                if (!isUpdatePositionTaking) {
                    isUpdatePositionTaking = true;
                }

                if (!isUpdate) {
                    isUpdate = true;
                }
            }
            else {
                paramaters += "0";
            }
        }
    }

    return paramaters;
}

function disableChildren(element, cssApplied) { // Check dublicated disable
    if (typeof (element) != 'object') return false;
    var count = element.childNodes.length;
    if (typeof (count) == 'undefined') return false;
    for (var i = 1; i < count; i++) {
        if (element.childNodes[i].tagName == "INPUT" || element.childNodes[i].tagName == "SELECT") {
            var previousValue = element.childNodes[i].disabled;
            if (previousValue != true) {
                element.childNodes[i].disabled = true;
                element.childNodes[i].setAttribute("previousValue", previousValue);
            };
        } else {
            if (typeof (cssApplied) == 'string' && element.childNodes[i].tagName == "DIV") {
                var addedCss = " " + cssApplied;
                if (element.childNodes[i].className.indexOf(addedCss) == -1) { // Avoid dublicated adding
                    element.childNodes[i].setAttribute("previousValue", element.childNodes[i].className);
                    element.childNodes[i].className += " " + cssApplied;
                };
            };

            Fanex.PTEngine.DisableChildren(element.childNodes[i]);
        }
    }

    return true;
};

function reEnableChildren(element, isCopyAll) { // Check dublicated enable
    if (typeof (element) != 'object') return false;
    var count = element.childNodes.length;
    var j = 1;
    if (isCopyAll == 1) {
        count = 1;
        j = 0;
    }

    if (typeof (count) == 'undefined') return false;
    for (var i = j; i < count; i++) {
        if (element.childNodes[i].tagName == "INPUT" || element.childNodes[i].tagName == "SELECT") {
            var previousValue = element.childNodes[i].getAttribute("previousValue");
            if (previousValue != null && typeof (previousValue) != 'undefined') {
                element.childNodes[i].setAttribute("previousValue", element.childNodes[i].disabled); // Store previous value
                element.childNodes[i].disabled = (previousValue == "true");
            }
        } else {
            if (element.childNodes[i].tagName == "DIV") {
                var previousValue = element.childNodes[i].getAttribute("previousValue");
                if (previousValue != null && typeof (previousValue) != 'undefined') {
                    element.childNodes[i].setAttribute("previousValue", element.childNodes[i].className);
                    element.childNodes[i].className = previousValue;
                };
            }

            Fanex.PTEngine.ReEnableChildren(element.childNodes[i], 0);
        }
    }

    return true;
};

function initBettingLimit() {
    var items = Fanex.ExtAcc.Customer.PTSetting.bettingLimit.items;
    var c = items.length;

    for (var i = 0; i < c; i++) {
        var item = items[i];
        disableChildren(item.element, "BetTypeInactive");
    }

    var headerSoccer = items[0].header;
    var headerBasketBall = items[3].header;
    headerSoccer.onclick = function () {
        if (document.getElementById('bettingLimit0').checked) {
            if (items[0].active === false) { // Uncheck --> Checked
                items[0].active = true;
                headerSoccer.className = "BettingLimitHeaderCopiedActive";
                if (items[3].active === true) { // Untick basket ball
                    items[3].active = false;
                    headerBasketBall.className = "BettingLimitHeaderCopied";
                }

                for (var i = 1; i < c; i++) {
                    var item = items[i];
                    item.copiedItem = {
                        id: 0
                    }; // Copied from Soccer, using index

                    item.inactive = true;
                    var checkBox = 'bettingLimit' + i;
                    document.getElementById(checkBox).checked = false;
                    Fanex.PTEngine.DisableChildren(item.element, "BetTypeInactive");
                }
            } else {
                items[0].active = false;
                headerSoccer.className = "BettingLimitHeaderCopied";
                for (var i = 1; i < c; i++) {
                    var item = items[i];
                    item.copiedItem = null; // Remove copiedItem
                    if (item.inactive === false) break;
                    item.inactive = false;
                    reEnableChildren(item.element, 1);
                }
            }
        }
    };

    headerBasketBall.onclick = function () {
        if (document.getElementById('bettingLimit3').checked) {
            if (items[3].inactive == true) { // Re-enable current
                items[3].inactive = false;
                items[3].copiedItem = null; // Remove copiedItem
                Fanex.PTEngine.ReEnableChildren(items[3].element);
            }

            setOriginalStatusSabaSoccer();
            setOriginalStatusSoccerParlay();

            var c = items.length;
            if (items[3].active === false) { // Uncheck --> Checked
                items[3].active = true;
                headerBasketBall.className = "BettingLimitHeaderCopiedActive";
                if (items[0].active === true) { // Untick soccer
                    items[0].active = false;
                    headerSoccer.className = "BettingLimitHeaderCopied";
                }

                for (var i = 4; i < c; i++) {
                    var item = items[i];
                    item.copiedItem = {
                        id: 3
                    }; // Copied from Basketball, using index

                    item.inactive = true;
                    var checkBox = 'bettingLimit' + i;
                    document.getElementById(checkBox).checked = false;
                    Fanex.PTEngine.DisableChildren(item.element, "BetTypeInactive");
                }
            } else {
                items[3].active = false;
                headerBasketBall.className = "BettingLimitHeaderCopied";
                for (var i = 4; i < c; i++) {
                    var item = items[i];
                    item.copiedItem = null; // Remove copiedItem
                    if (item.inactive === false) break;
                    item.inactive = false;
                    var checkBox = 'bettingLimit' + i;
                    reEnableChildren(item.element, 1);
                }
            }
        }
    };  

    setOriginalStatusSabaSoccer = function () {
        items[1].active = false;
        items[1].copiedItem = null;
        $('bettingLimit1').checked = false;
        Fanex.PTEngine.RemoveClass(items[1].header, "BetTypeInactive");
        Fanex.PTEngine.RemoveDisableChildren(items[1].header);
    };

    setOriginalStatusSoccerParlay = function () {
        items[2].active = false;
        items[2].copiedItem = null;
        $('bettingLimit2').checked = false;
        Fanex.PTEngine.RemoveClass(items[2].header, "BetTypeInactive");
        Fanex.PTEngine.RemoveDisableChildren(items[2].header);
    };
};

function initPositionTaking() {
    var sportTypes = Fanex.ExtAcc.Customer.PTSetting.ptGrid.items;
    var cloneFromSoccer = sportTypes[0].cloneButton;
    var cloneFromBasketball = sportTypes[1].cloneButton;
    var editBasketball = sportTypes[1].editButton;
    var ignoreCopySports = [161];
    var copiedItemSoccer = {
        sportId: 1,
        id: 1,
        isLive: false
    };

    var copiedItemBasketball = {
        sportId: 2,
        id: 1,
        isLive: false
    };

    disablePositionTaking();

    cloneFromSoccer.onclick = function () {
        if (document.getElementById('positionTaking0').checked) {
            Validators.hide();
            var count = sportTypes.length;
            cloneFromSoccer.firstChild.checked = !cloneFromSoccer.firstChild.checked;
            cloneFromSoccer.parent.copiedItem = copiedItemSoccer;
            cloneFromSoccer.parent.activeCopiedTitle = "{0} {1}".Format(sportTypes[0].name, sportTypes[0].betTypeGroupSets[0].betTypeGroups[0].betTypes[0].name);
            cloneFromSoccer.className = cloneFromSoccer.firstChild.checked ? "PTSportCopiedActive" : "PTSportCopied";
            cloneFromSoccer.parent.items[0].Toggle(true); // Expand
            // Rollback Basketball
            if (cloneFromBasketball.firstChild.checked) {
                cloneFromBasketball.className = "PTSportCopied";
                cloneFromBasketball.firstChild.checked = false;
                cloneFromBasketball.title = Language.COPY_ALL_OTHER_POSITION_TAKING_FROM.Format(sportTypes[1].name, sportTypes[0].betTypeGroupSets[0].betTypeGroups[0].betTypes[0].name);
            }
            // Show / Hide edit button of Basketball
            if (cloneFromSoccer.firstChild.checked) {
                editBasketball.className = "PTSportEdit";
                editBasketball.title = Language.EDIT;
                cloneFromSoccer.title = String.Empty;
            } else {
                editBasketball.className = "PTSportCopiedInactive";
                editBasketball.title = String.Empty;
                cloneFromSoccer.title = Language.COPY_ALL_OTHER_POSITION_TAKING_FROM.Format(sportTypes[0].name, sportTypes[0].betTypeGroupSets[0].betTypeGroups[0].betTypes[0].name);
            }

            for (var i = 0; i < count; i++) {
                if (!ignoreCopySports.includes(sportTypes[i].id)) {
                    if (cloneFromSoccer.firstChild.checked) {
                        if (i > 0) {
                            var id = 'positionTaking' + i;
                            document.getElementById(id).checked = false;
                            document.getElementById(id).disabled = true;
                            //disableSportType(i, true);
                        }

                        sportTypes[i].Inactive(Fanex.copy(copiedItemSoccer));
                    }
                    else if (i > 0) {
                        var id = 'positionTaking' + i;
                        document.getElementById(id).disabled = false;
                        // disableSportType(i, true);
                    }

                    if (i > 1) {
                        sportTypes[i].cloneButton.className = cloneFromSoccer.firstChild.checked ? "PTSportEdit" : "PTSportCopiedInactive";
                        sportTypes[i].cloneButton.title = cloneFromSoccer.firstChild.checked ? Language.EDIT : String.Empty;
                    }
                }
            };

            if (!cloneFromSoccer.firstChild.checked) {
                sportTypes[0].Active();
                cloneFromSoccer.parent.copiedItem = null;
            }

            cloneFromSoccer.parent.ShowCloneHint();
        }
    };

    // Basketball
    cloneFromBasketball.onclick = function () {
        if (document.getElementById('positionTaking1').checked) {
            Validators.hide();
            var count = sportTypes.length;
            cloneFromBasketball.firstChild.checked = !cloneFromBasketball.firstChild.checked;
            cloneFromBasketball.parent.copiedItem = copiedItemBasketball;
            cloneFromBasketball.parent.activeCopiedTitle = "{0} {1}".Format(sportTypes[1].name, sportTypes[0].betTypeGroupSets[0].betTypeGroups[0].betTypes[0].name);
            cloneFromBasketball.className = cloneFromBasketball.firstChild.checked ? "PTSportCopiedActive" : "PTSportCopied";
            cloneFromBasketball.parent.items[1].Toggle(true); // Expand
            // Rollback Soccer
            if (cloneFromSoccer.firstChild.checked) {
                sportTypes[0].Active();
                cloneFromSoccer.className = "PTSportCopied";
                cloneFromSoccer.firstChild.checked = false;
                cloneFromSoccer.title = Language.COPY_ALL_OTHER_POSITION_TAKING_FROM.Format(sportTypes[0].name, sportTypes[0].betTypeGroupSets[0].betTypeGroups[0].betTypes[0].name);
            };
            // Hide edit button of Basketball
            if (cloneFromBasketball.firstChild.checked) {
                editBasketball.className = "PTSportCopiedInactive";
                cloneFromBasketball.title = String.Empty;
            } else {
                cloneFromBasketball.title = Language.COPY_ALL_OTHER_POSITION_TAKING_FROM.Format(sportTypes[1].name, sportTypes[0].betTypeGroupSets[0].betTypeGroups[0].betTypes[0].name);
            }

            for (var i = 1; i < count; i++) {
                if (!ignoreCopySports.includes(sportTypes[i].id)) {
                    if (cloneFromBasketball.firstChild.checked) {
                        // disableSportType(i, true);
                        sportTypes[i].Inactive(Fanex.copy(copiedItemBasketball));
                        if (i > 1) {
                            var id = 'positionTaking' + i;
                            document.getElementById(id).checked = false;
                            document.getElementById(id).disabled = true;
                        }
                    }
                    else if (i > 1) {
                        var id = 'positionTaking' + i;
                        document.getElementById(id).disabled = false;
                    }

                    if (i > 1) {
                        sportTypes[i].cloneButton.className = cloneFromBasketball.firstChild.checked ? "PTSportEdit" : "PTSportCopiedInactive";
                        sportTypes[i].cloneButton.title = cloneFromBasketball.firstChild.checked ? Language.EDIT : String.Empty;
                        // disableSportType(i, true);
                    }
                }
            };

            if (!cloneFromBasketball.firstChild.checked) {
                sportTypes[1].Active();
                cloneFromBasketball.parent.copiedItem = null;
            }

            cloneFromBasketball.parent.ShowCloneHint();
        }
    };
};

function disablePositionTaking() {
    if (Fanex.ExtAcc.Customer.PTSetting.ptGrid.items.length <= 0) return false;
    for (var i = 0; i < Fanex.ExtAcc.Customer.PTSetting.ptGrid.items.length; i++) {
        var betTypeGroupSets = Fanex.ExtAcc.Customer.PTSetting.ptGrid.items[i].items;
        for (var j = 0; j < betTypeGroupSets.length; j++) {
            var betTypeGroups = betTypeGroupSets[j].items;
            betTypeGroupSets[j].copiedItem = {};
            for (var k = 0; k < betTypeGroups.length; k++) {
                var betTypes = betTypeGroups[k].items;
                for (var l = 0; l < betTypes.length; l++) {
                    if (betTypes[l].actived == true) {
                        Fanex.PTEngine.DisableChildren(betTypes[l].element, "BetTypeInactive");
                        betTypes[l].actived = false;
                    }
                }
            }
        }
    }

    return true;
};

function disableSportType(id, isDisable) {
    if (typeof (id) == 'undefined') return false;
    if (Fanex.ExtAcc.Customer.PTSetting.ptGrid.items.length <= 0) return false;

    var betTypeGroupSets = Fanex.ExtAcc.Customer.PTSetting.ptGrid.items[id].items;
    for (var j = 0; j < betTypeGroupSets.length; j++) {
        var betTypeGroups = betTypeGroupSets[j].items;
        isDisable ? betTypeGroupSets[j].copiedItem = {} : betTypeGroupSets[j].copiedItem = null;
        for (var k = 0; k < betTypeGroups.length; k++) {
            var betTypes = betTypeGroups[k].items;
            if (isDisable) {
                betTypeGroups[k].copiedItem = null;
            }

            for (var l = 0; l < betTypes.length; l++) {
                var idCopied = "c$" + betTypeGroups[k].betTypes[0].id + "$" + betTypeGroups[k].betTypes[0].sportId + "$" + (betTypeGroups[k].isLive ? "0" : "1");
                if (isDisable) {
                    if (betTypes[l].actived == true) {
                        Fanex.PTEngine.DisableChildren(betTypes[l].element, "BetTypeInactive");
                        betTypes[l].actived = false;

                        if (document.getElementById(idCopied)) {
                            document.getElementById(idCopied).label.className = "BetTypeHeader2 BetTypeInactive";
                            document.getElementById(idCopied).checked = false;
                            betTypeGroups[k].copiedItem = null;
                            betTypeGroups[k].copiedFromFirst = false;
                        }
                    }
                }
                else {
                    if (betTypes[l].actived == false) {
                        Fanex.PTEngine.ReEnableChildren(betTypes[l].element);
                        betTypes[l].actived = true;
                        if (document.getElementById(idCopied)) {
                            document.getElementById(idCopied).label.className = "BetTypeHeader2";
                        }
                    }
                }
            }
        }
    }

    return true;
};

function getUpdateParamaters() {
    var paramaters = "";
    var items = Fanex.ExtAcc.Customer.PTSetting.bettingLimit.items;

    if (document.getElementById("status").checked) {
        paramaters += "1";
        if (!isUpdate) {
            isUpdate = true;
        }
    }
    else {
        paramaters += "0";
    }

    if (document.getElementById("commission").checked) {
        paramaters += "1";
        if (!isUpdate) {
            isUpdate = true;
        }
    }
    else {
        paramaters += "0";
    }

    if (document.getElementById("bettingLimit0").checked && items[0].active === true) {
        for (var i = 0; i < items.length; i++) {
            paramaters += "1";
            if (!isUpdate) {
                isUpdate = true;
            }
        }
    }
    else if (document.getElementById("bettingLimit3").checked && items[3].active === true) {
        document.getElementById("bettingLimit0").checked ? paramaters += "1" : paramaters += "0";
        document.getElementById("bettingLimit1").checked ? paramaters += "1" : paramaters += "0";
        document.getElementById("bettingLimit2").checked ? paramaters += "1" : paramaters += "0";
        for (var i = 3; i < items.length; i++) {
            paramaters += "1";
            if (!isUpdate) {
                isUpdate = true;
            }
        }
    }
    else {
        for (var i = 0; i < items.length; i++) {
            if (document.getElementById("bettingLimit" + i).checked) {
                paramaters += "1";
                if (!isUpdate) {
                    isUpdate = true;
                }
            }
            else {
                paramaters += "0";
            }
        }
    }

    var itemsPt = Fanex.ExtAcc.Customer.PTSetting.ptGrid.items;
    var cloneFromSoccer = itemsPt[0].cloneButton;
    var cloneFromBasketball = itemsPt[1].cloneButton;
    if (document.getElementById("positionTaking0").checked && cloneFromSoccer.firstChild.checked) {
        isUpdatePositionTaking = true;
        for (var i = 0; i < itemsPt.length; i++) {
            paramaters += "1";
            if (!isUpdate) {
                isUpdate = true;
            }
        }
    }
    else if (document.getElementById("positionTaking1").checked && cloneFromBasketball.firstChild.checked) {
        document.getElementById("positionTaking0").checked ? paramaters += "1" : paramaters += "0";
        isUpdatePositionTaking = true;
        for (var i = 1; i < itemsPt.length; i++) {
            paramaters += "1";
            if (!isUpdate) {
                isUpdate = true;
            }
        }
    }
    else {
        for (var i = 0; i < itemsPt.length; i++) {
            if (document.getElementById("positionTaking" + i).checked) {
                paramaters += "1";
                if (!isUpdatePositionTaking) {
                    isUpdatePositionTaking = true;
                }

                if (!isUpdate) {
                    isUpdate = true;
                }
            }
            else {
                paramaters += "0";
            }
        }
    }

    return paramaters;
};

function saveCommissionSettings(comm) {
    if (Fanex.ExtAcc.Customer.CurrentLevel == 1) {
        if (document.getElementById("discountAsian"))
            comm.playerDiscount = parseFloat($("#discountAsian").val());

        if (document.getElementById("discount1x2"))
            comm.playerDiscount1x2 = parseFloat($("#discount1x2").val());

        if (document.getElementById("discountCS"))
            comm.playerDiscountCs = parseFloat($("#discountCS").val());
        if (comm.isMyanmarOdds)
            comm.playerDiscountMyanmarOdds = parseFloat($("#discountMyanmarOdds").val());
    }
    else {
        if (document.getElementById("discountAsian"))
            comm.discount = parseFloat($("#discountAsian").val());

        if (document.getElementById("discount1x2"))
            comm.discount1x2 = parseFloat($("#discount1x2").val());

        if (document.getElementById("discountCS"))
            comm.discountCs = parseFloat($("#discountCS").val());
        if (comm.isMyanmarOdds)
            comm.discountMyanmarOdds = parseFloat($("#discountMyanmarOdds").val());
    }

    if (document.getElementById("groupA"))
        comm.groupA = parseFloat($("#groupA").val());

    if (document.getElementById("groupB"))
        comm.groupB = parseFloat($("#groupB").val());

    if (document.getElementById("groupC"))
        comm.groupC = parseFloat($("#groupC").val());

    if (document.getElementById("groupD"))
        comm.groupD = parseFloat($("#groupD").val());
}

function extendPTEngine() {
    Validators.utils.findPos = function (obj) {
        var curleft = curtop = 0;
        if (obj.offsetParent) {
            do {
                curleft += obj.offsetLeft;
                curtop += obj.offsetTop;
            } while (obj = obj.offsetParent);
        }

        // windowPopup is id of popup form and need to re-define if your popup id is different from windowPopup
        if (document.getElementById('windowPopup')) {
            var parent = document.getElementById('windowPopup');

            curleft -= parent.scrollLeft;
            curtop -= parent.scrollTop;
        } else if (document.getElementById('contentSplitter')) {
            var parent = document.getElementById('contentSplitter');
            curleft -= parent.scrollLeft;
            curtop -= parent.scrollTop;
        }

        return [curleft, curtop];
    }
}

$(document).ready(function () {
    Fanex.ExtAcc.Customer.PTSetting.Init();
});