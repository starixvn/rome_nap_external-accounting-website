/// <reference path="Languages/en.js" />
if (typeof (Fanex) == 'undefined') { // Require Fanex namespace
    Fanex = {};
}
if (typeof (Fanex.PTRacing) == 'undefined') { // Require Fanex.PTRacing namespace
    alert('Require Fanex.PTRacing namespace');
}
if (typeof (Validators) == 'undefined') { // Require Validators library
    alert('Require validators library');
}

Fanex.PTRacing.BettingCommSetting = function (source) {
    if (typeof (source) == 'object') {
        this.items = source;
    }
    this.isCopyAll = false;
    this.context = Fanex.copy(Fanex.PTRacing.CURRENT_CONTEXT);
    var validateExpr = /^(\$|)([0-9]\d{0,2}(\,\d{3})*|([0-9]\d*))(\.?\d*)?$/;
    this.ValidateMessage = function (elementInput, errorMessage, xpos, pointerx) {
        var isXPos = (typeof (xpos) != "undefined" && xpos != null);
        var isPointerXPos = (typeof (pointerx) != "undefined" && pointerx != null);
        if (isXPos && pointerx) {
            Validators.show(elementInput, errorMessage, xpos, pointerx);
        }
        else {
            Validators.show(elementInput, errorMessage);
        }
        Validators.animate(elementInput);
    };
    this.Validate = function () {
        var c = this.items.length;
        for (var i = 0; i < c; i++) {
            var bettypeGroups = this.items[i].betTypeGroups;
            var totalGroups = bettypeGroups.length;
            for (var j = 0; j < totalGroups; j++) {
                var item = bettypeGroups[j].bettingCommSetting;
                if (bettypeGroups[j].isHide) {
                    continue;
                }

                // Validate each sport type      
                if ((this.isCopyAll && j == 0 && i == 0) | this.isCopyAll == false) {
                    if (validateExpr.test(item.minBetElement.value) == false) {
                        this.ValidateMessage(item.minBetElement, Language.INCORRECT_MIN_BET);
                        return false;
                    }
                    if (validateExpr.test(item.maxBetElement.value) == false) {
                        this.ValidateMessage(item.maxBetElement, Language.INCORRECT_MAX_BET);
                        return false;
                    }
                    if (validateExpr.test(item.maxPerMatchElement.value) == false) {
                        this.ValidateMessage(item.maxPerMatchElement, Language.INCORRECT_MAX_PER_MATCH);
                        return false;
                    }
                }

                var minBet = parseFloat(item.minBetElement.value, true);
                var maxBet = parseFloat(item.maxBetElement.value, true);
                var maxPerMatch = parseFloat(item.maxPerMatchElement.value, true);
                var minBetMin = item.minBetMin;
                var maxBetMax = item.maxBetMax;
                var maxPerMatchMax = item.maxPerMatchMax;

                if (this.isCopyAll) {
                    minBet = parseFloat(this.items[0].betTypeGroups[0].bettingCommSetting.minBetElement.value, true);
                    maxBet = parseFloat(this.items[0].betTypeGroups[0].bettingCommSetting.maxBetElement.value, true);
                    maxPerMatch = parseFloat(this.items[0].betTypeGroups[0].bettingCommSetting.maxPerMatchElement.value, true);
                }

                if (minBet == maxBet && maxBet == maxPerMatch && maxPerMatch == 0) {
                    continue;
                }

                // Check min bet < max bet < max per match
                if (minBet > maxBet) {
                    this.ValidateMessage(item.minBetElement, Language.MIN_BET_LOWER_THAN_MIN_MAX_BET.Format(item.name));
                    return false;
                }
                if (minBet > maxPerMatch) {
                    this.ValidateMessage(item.minBetElement, Language.MIN_BET_LOWER_THAN_MIN_MAX_PER_MATCH_BET.Format(item.name));
                    return false;
                }
                if (maxBet > maxPerMatch) {
                    this.ValidateMessage(item.maxBetElement, Language.MAX_BET_LOWER_THAN_MIN_MAX_PER_MATCH_BET.Format(item.name));
                    return false;
                }

                // Check minbet < minbetmax
                if (typeof (item.minBetMin) != 'undefined' && minBetMin > 0 && minBet < minBetMin) {
                    this.ValidateMessage(item.minBetElement, Language.MIN_BET_GREATER_THAN_MIN_MIN_BET.Format(''.FormatNumber(minBetMin)));
                    return false;
                }
                // Check maxbet < maxbetmin
                if (typeof (item.maxBetMax) != 'undefined' && maxBet > maxBetMax && maxBetMax != 0) {
                    this.ValidateMessage(item.maxBetElement, Language.MAX_BET_LOWER_THAN_MAX_MAX_BET.Format(''.FormatNumber(item.maxBetMax)));
                    return false;
                }
                // Check maxpermatch_min < maxpermatch < maxpermatch_max
                if (typeof (item.maxPerMatchMax) != 'undefined' && maxPerMatch > maxPerMatchMax && maxPerMatchMax != 0) {
                    this.ValidateMessage(item.maxPerMatchElement, Language.MAX_PER_MATCH_LOWER_THAN_MAX_PER_MATCH_MAX_BET.Format(''.FormatNumber(item.maxPerMatchMax)), -130, 160);
                    return false;
                }
            }

        }
        // Success
        return true;
    }
    this.Render = function (owner) {
        var divHeight = 24;
        var targetRoleId = this.context.targetRoleId;
        var roleId = this.context.roleId;
        if (targetRoleId == 1 && (roleId == 4 | roleId == 3)) {
            return this.RenderCrossLevel(owner);
        }

        var box = document.createElement("DIV");
        // Create MinBet, MaxBet, MaxPerMatch header
        var emptyBox = document.createElement("DIV");
        emptyBox.innerHTML = "&nbsp;";
        emptyBox.className = "bettingBox-header-sport";
        box.appendChild(emptyBox); //pool

        var emptyBox2 = document.createElement("DIV");
        emptyBox2.innerHTML = "&nbsp;";
        emptyBox2.className = "bettingBox-header-tote";
        box.appendChild(emptyBox2); //tote

        /*bet setting*/
        var betSettingHeaderBox = document.createElement("DIV");
        if (this.context.formId == 4) {
            betSettingHeaderBox.className = "headerBetSettingCrossLevelBox";
        }
        else {
            betSettingHeaderBox.className = "headerBetSettingBox";
        }
        var betSettingMergeTitleBox = document.createElement("DIV");
        betSettingMergeTitleBox.id = "betsettingheader";
        if (this.context.formId == 1) {
            betSettingMergeTitleBox.className = "bettingBox-header-bsbox";
        }
        else {
            betSettingMergeTitleBox.className = "bettingBox-header-bsbox2";
        }
        betSettingMergeTitleBox.innerHTML = Language.BETTING_LIMIT;
        betSettingHeaderBox.appendChild(betSettingMergeTitleBox);

        var betSettingTitleBox = document.createElement("DIV");
        var minBetHeader = document.createElement("DIV");
        minBetHeader.innerHTML = Language.MIN_BET;
        if (this.context.formId == 1) {
            minBetHeader.className = "bettingBox-header-minbet";
        }
        else {
            minBetHeader.className = "bettingBox-header-minbet2";
        }
        betSettingTitleBox.appendChild(minBetHeader);

        var maxBetHeader = document.createElement("DIV");
        maxBetHeader.innerHTML = Language.MAX_BET;
        if (this.context.formId == 1) {
            maxBetHeader.className = "bettingBox-header-maxbet";
        }
        else {
            maxBetHeader.className = "bettingBox-header-maxbet2";
        }
        betSettingTitleBox.appendChild(maxBetHeader);

        var maxPerMatchHeader = document.createElement("DIV");
        maxPerMatchHeader.innerHTML = Language.MAX_PER_MATCH;
        if (this.context.formId == 1) {
            maxPerMatchHeader.className = "bettingBox-header-maxpermatch";
        }
        else {
            maxPerMatchHeader.className = "bettingBox-header-maxpermatch2";
        }
        betSettingTitleBox.appendChild(maxPerMatchHeader);
        betSettingHeaderBox.appendChild(betSettingTitleBox);

        box.appendChild(betSettingHeaderBox);
        /*end bet setting*/


        /*comm*/
        /*region edit comm button*/
        var divCommButtonBox = document.createElement("DIV");
        divCommButtonBox.className = 'divCommButtonBox';
        var buttonEditComm = document.createElement("INPUT");
        buttonEditComm.type = "button";
        buttonEditComm.title = Language.EDIT;
        buttonEditComm.id = "btnEditComm";
        buttonEditComm.className = "bettingCommSportEdit hidden";
        buttonEditComm.onclick = function () {
            if (Fanex.hasClass(this, 'bettingCommSportEdit')) {
                Fanex.removeClass(this, 'bettingCommSportEdit');
                Fanex.addClass(this, 'bettingCommSportActive');
                this.setAttribute('copyComm', 0);
            }
            else {
                Fanex.removeClass(this, 'bettingCommSportActive');
                Fanex.addClass(this, 'bettingCommSportEdit');
                this.setAttribute('copyComm', 1);
            }
            Fanex.PTRacing.ToggleBetCommSetting(grid);
        };
        /*endregion edit comm button*/

        if (targetRoleId == 1) {
            var commHeaderBox = document.createElement("DIV");
            commHeaderBox.className = "bettingBox-header-commbox2";
            var commHeaderMergeBox = document.createElement("DIV");
            var divCommHeaderTitle = document.createElement("DIV");
            divCommHeaderTitle.className = 'float-left divCommButtonTile text-align-right';
            divCommHeaderTitle.innerHTML = Language.COMMISSION;
            divCommHeaderTitle.style.width = '100px';
            commHeaderMergeBox.appendChild(divCommHeaderTitle);
            commHeaderMergeBox.id = "commheader";
            commHeaderMergeBox.className = "bettingBox-header-comm2";
            divCommButtonBox.appendChild(buttonEditComm);
            commHeaderMergeBox.appendChild(divCommButtonBox);
            divCommButtonBox.style.top = '0px';
            commHeaderBox.appendChild(commHeaderMergeBox);

            var commHeaderTitleBox = document.createElement("DIV");
            var agentCommHeader = document.createElement("DIV");
            agentCommHeader.innerHTML = Language.AGENT;
            agentCommHeader.className = "bettingBox-header-roletitle";
            commHeaderTitleBox.appendChild(agentCommHeader);
            var memberCommHeader = document.createElement("DIV");
            memberCommHeader.innerHTML = Language.MEMBER;
            memberCommHeader.className = "bettingBox-header-roletitle";
            commHeaderTitleBox.appendChild(memberCommHeader);
            commHeaderBox.appendChild(commHeaderTitleBox);

            box.appendChild(commHeaderBox);
        }
        else {
            var commHeader = document.createElement("DIV");
            var divCommHeaderTitle = document.createElement("DIV");
            divCommHeaderTitle.className = 'float-left divCommButtonTile';
            divCommHeaderTitle.innerHTML = Language.COMMISSION;
            commHeader.appendChild(divCommHeaderTitle);
            commHeader.id = "commheader";
            if (targetRoleId == 2 && roleId == 4) {
                commHeader.className = "bettingBox-header-comm4";
            }
            else {
                commHeader.className = "bettingBox-header-comm";
            }
            divCommButtonBox.appendChild(buttonEditComm);
            commHeader.appendChild(divCommButtonBox);

            box.appendChild(commHeader);
        }
        /*end comm*/

        /*satus*/
        /*region edit status button*/
        var divEditStatusButtonBox = document.createElement("DIV");
        divEditStatusButtonBox.className = 'divBettingCommSportCopy';

        var buttonEditStatus = document.createElement("INPUT");
        buttonEditStatus.type = "button";
        buttonEditStatus.title = Language.EDIT;
        buttonEditStatus.id = "btnEditStatus";
        buttonEditStatus.className = "bettingCommSportEdit hidden";
        buttonEditStatus.onclick = function () {
            if (Fanex.hasClass(this, 'bettingCommSportEdit')) {
                Fanex.removeClass(this, 'bettingCommSportEdit');
                Fanex.addClass(this, 'bettingCommSportActive');
                document.getElementById(this.id).setAttribute('copyStatus', 0);
            }
            else {
                Fanex.removeClass(this, 'bettingCommSportActive');
                Fanex.addClass(this, 'bettingCommSportEdit');
                document.getElementById(this.id).setAttribute('copyStatus', 1);
            }
            Fanex.PTRacing.ToggleBetCommSetting(grid);
        };
        /*endregion edit status button*/
        if (targetRoleId > 1) {
            var statusHeaderBox = document.createElement("DIV");
            statusHeaderBox.className = "headerStatusBox";
            var statusHeaderMergeBox = document.createElement("DIV");
            var statusHeaderMergeTitleBox = document.createElement("DIV");
            statusHeaderMergeTitleBox.innerHTML = Language.STATUS;
            statusHeaderMergeTitleBox.className = 'float-left text-align-right';
            statusHeaderMergeTitleBox.style.width = '88px';
            statusHeaderMergeBox.id = "statusheader";
            statusHeaderMergeBox.className = "bettingBox-header-status";
            statusHeaderMergeBox.appendChild(statusHeaderMergeTitleBox);
            divEditStatusButtonBox.appendChild(buttonEditStatus);
            statusHeaderMergeBox.appendChild(divEditStatusButtonBox);
            statusHeaderBox.appendChild(statusHeaderMergeBox);

            var statusHeaderTitleBox = document.createElement("DIV");
            var enableHeader = document.createElement("DIV");
            enableHeader.innerHTML = this.getRoleNameById(targetRoleId);
            enableHeader.className = "bettingBox-header-roletitle";
            statusHeaderTitleBox.appendChild(enableHeader);
            var enableDownlinesHeader = document.createElement("DIV");
            enableDownlinesHeader.innerHTML = Language.DOWNLINES;
            enableDownlinesHeader.className = "bettingBox-header-downlines";
            statusHeaderTitleBox.appendChild(enableDownlinesHeader);

            statusHeaderBox.appendChild(statusHeaderTitleBox);
            box.appendChild(statusHeaderBox);
        }
        else {
            var statusHeaderBox = document.createElement("DIV");
            statusHeaderBox.className = "headerStatusBoxForMember";
            var statusHeaderMergeBox = document.createElement("DIV");
            var statusHeaderMergeTitleBox = document.createElement("DIV");
            statusHeaderMergeTitleBox.innerHTML = Language.STATUS;
            statusHeaderMergeTitleBox.className = 'float-left text-align-right';
            statusHeaderMergeTitleBox.style.width = '58px';
            statusHeaderMergeBox.appendChild(statusHeaderMergeTitleBox);
            statusHeaderMergeBox.id = "statusheader";
            statusHeaderMergeBox.className = "bettingBox-header-status2";
            divEditStatusButtonBox.appendChild(buttonEditStatus);
            divEditStatusButtonBox.style.top = '12px';
            statusHeaderMergeBox.appendChild(divEditStatusButtonBox);
            statusHeaderBox.appendChild(statusHeaderMergeBox);

            box.appendChild(statusHeaderBox);
        }
        /*end status*/

        var clear = document.createElement("DIV");
        clear.style.clear = "both";
        box.appendChild(clear);

        var c = this.items.length;
        for (var i = 0; i < c; i++) {
            var sportInfo = this.items[i];
            var bettypeGroups = sportInfo.betTypeGroups;
            var totalBettypeGroups = bettypeGroups.length;

            var sportBox = document.createElement("DIV");
            sportBox.className = "bettingBox-sportbox";
            var sportHeader = document.createElement("DIV");

            sportBox.appendChild(sportHeader);

            var bettypeGroupBox = document.createElement("DIV");
            var ieMode = this.checkIEModeVersion();
            bettypeGroupBox.className = (ieMode && ieMode < 7) ? 'float-left' : '';
            for (var j = 0; j < totalBettypeGroups; j++) {
                var bettypeGroup = bettypeGroups[j];
                if (bettypeGroup.isHide) {
                    totalBettypeGroups--;
                    continue;
                }
                var item = bettypeGroup.bettingCommSetting;
                item.name = sportInfo.name + ' ' + bettypeGroup.name;
                var box2 = document.createElement("DIV");
                box2.className = "bettingBox-row float-left";
                if (typeof (item.minBet) == 'undefined') { // For Add new case
                    item.minBet = 0; //item.minBetMax;
                }
                if (typeof (item.maxBet) == 'undefined') { // For Add new case
                    item.maxBet = 0; // item.maxBetMin;
                }
                if (typeof (item.maxPerMatch) == 'undefined') { // For Add new case
                    item.maxPerMatch = 0; // item.maxPerMatchMax;
                }

                var bettingCommCopySettingClass = " bettingCommCopySetting ";
                var betSettingCopyStatus = "betSettingCopyStatus ";
                var betSettingCopyComm = "betSettingCopyComm ";

                var header = document.createElement("DIV");
                var divHeader = document.createElement("DIV");
                divHeader.className = 'float-left';
                divHeader.innerHTML = bettypeGroup.name;
                header.className = "bettingBox-tote";
                header.id = "divBettingComm_BettypeGroup_" + sportInfo.id + "_" + bettypeGroup.id;

                header.appendChild(divHeader);

                if (sportInfo.id == Fanex.PTRacing.HR_UKIRISHTOTE_WIN.sportId && Fanex.PTRacing.HR_UKIRISHTOTE_WIN.toteId == bettypeGroup.id) {
                    if (this.context.formId != 4) {
                        var divBettingCommSportCopy = document.createElement("DIV");
                        divBettingCommSportCopy.className = 'divBettingCommSportCopy';

                        var buttonCopyBettingComm = document.createElement("INPUT");
                        buttonCopyBettingComm.type = "button";
                        buttonCopyBettingComm.title = Language.COPY_ALL_BETTING_LIMIT_FROM.Format(Language.HORSE_RACING_UI_IRISH_TOTE);
                        buttonCopyBettingComm.id = "btnCopyBettingComm";
                        buttonCopyBettingComm.className = "bettingCommSportCopied";
                        var grid = this;
                        buttonCopyBettingComm.onclick = function () {
                            grid.isCopyAll = !grid.isCopyAll;
                            var editStatusButton = document.getElementById('btnEditStatus');
                            editStatusButton.setAttribute('copyStatus', "1");

                            var editCommButton = document.getElementById('btnEditComm');
                            editCommButton.setAttribute('copyComm', "1");

                            Fanex.removeClass(editStatusButton, 'bettingCommSportActive');
                            Fanex.removeClass(editCommButton, 'bettingCommSportActive');
                            Fanex.addClass(editStatusButton, 'bettingCommSportEdit');
                            Fanex.addClass(editCommButton, 'bettingCommSportEdit');

                            if (Fanex.hasClass(this, 'bettingCommSportCopied')) {
                                Fanex.removeClass(this, 'bettingCommSportCopied');
                                Fanex.addClass(this, 'bettingCommSportCopiedActive');
                                Fanex.removeClass(editStatusButton, 'hidden');
                                Fanex.removeClass(editCommButton, 'hidden');
                                document.getElementById(this.id).setAttribute('copyBettingComm', 1);
                            }
                            else {
                                Fanex.removeClass(this, 'bettingCommSportCopiedActive');
                                Fanex.addClass(this, 'bettingCommSportCopied');
                                Fanex.addClass(editStatusButton, 'hidden');
                                Fanex.addClass(editCommButton, 'hidden');
                                document.getElementById(this.id).setAttribute('copyBettingComm', 0);
                            }
                            Fanex.PTRacing.ToggleBetCommSetting(grid);
                        };

                        divBettingCommSportCopy.appendChild(buttonCopyBettingComm);
                        header.appendChild(divBettingCommSportCopy);
                    }

                    bettingCommCopySettingClass = "";
                    betSettingCopyStatus = "";
                    betSettingCopyComm = "";
                }
                box2.appendChild(header);

                // min bet
                // Show label instead to TextBox
                var minBetBox = document.createElement("DIV");
                if (this.context.formId == 4) {
                    minBetBox.innerHTML = ''.FormatNumber(item.minBet);
                    minBetBox.className = "bettingBox-minbet2";
                }
                else {
                    var minBet = document.createElement("INPUT");
                    minBet.type = "text";
                    minBet.id = 'minbet_' + sportInfo.id + "_" + bettypeGroup.id;
                    minBet.maxLength = 14;
                    minBet.onkeyup = function (event) { Fanex.FormatNumber(event, true); };
                    minBet.className = "inputNumberWidth90" + bettingCommCopySettingClass;
                    minBet.value = ''.FormatNumber(item.minBet);
                    minBetBox.appendChild(minBet);

                    // Limit box
                    var minBetMax = document.createElement("DIV");
                    minBetMax.innerHTML = item.minBetMin == 0 ? '' : '>='.FormatNumber(item.minBetMin);
                    minBetBox.appendChild(minBetMax);

                    item.minBetElement = minBet;
                    minBetBox.className = "bettingBox-minbet";
                }
                box2.appendChild(minBetBox);

                // max bet
                // Show label instead to TextBox
                var maxBetBox = document.createElement("DIV");
                if (this.context.formId == 4) {
                    maxBetBox.innerHTML = ''.FormatNumber(item.maxBet);
                    maxBetBox.className = "bettingBox-maxbet2";
                }
                else {
                    var maxBet = document.createElement("INPUT");
                    maxBet.type = "text";
                    maxBet.id = 'maxbet_' + sportInfo.id + "_" + bettypeGroup.id;
                    maxBet.maxLength = 14;
                    maxBet.onkeyup = function (event) { Fanex.FormatNumber(event, true); };
                    maxBet.className = "inputNumberWidth90" + bettingCommCopySettingClass;
                    maxBet.value = ''.FormatNumber(item.maxBet);
                    maxBetBox.appendChild(maxBet);
                    maxBetBox.className = "bettingBox-maxbet";
                    // Limit box
                    if (targetRoleId < 4) {
                        var maxBetMin = document.createElement("DIV");
                        maxBetMin.innerHTML = item.maxBetMax == 0 ? '' : '<='.FormatNumber(item.maxBetMax);
                        maxBetBox.appendChild(maxBetMin);
                    }
                    item.maxBetElement = maxBet;
                }
                box2.appendChild(maxBetBox);

                // Max per match
                // Show label instead to TextBox
                var maxPerMatchBox = document.createElement("DIV");
                if (this.context.formId == 4) {
                    maxPerMatchBox.innerHTML = ''.FormatNumber(item.maxPerMatch);
                    maxPerMatchBox.className = "bettingBox-maxpermatch2";
                }
                else {
                    var maxPerMatch = document.createElement("INPUT");
                    maxPerMatch.type = "text";
                    maxPerMatch.id = 'maxpermatch_' + sportInfo.id + "_" + bettypeGroup.id;
                    maxPerMatch.maxLength = 14;
                    maxPerMatch.onkeyup = function (event) { Fanex.FormatNumber(event, true); };
                    maxPerMatch.className = "inputNumberWidth90" + bettingCommCopySettingClass;
                    maxPerMatch.value = ''.FormatNumber(item.maxPerMatch);
                    maxPerMatchBox.appendChild(maxPerMatch);
                    maxPerMatchBox.className = "bettingBox-maxpermatch";
                    // Limit box
                    if (targetRoleId < 4) {
                        var maxPerMatchMax = document.createElement("DIV");
                        maxPerMatchMax.innerHTML = item.maxPerMatchMax == 0 ? '' : '<='.FormatNumber(item.maxPerMatchMax);
                        maxPerMatchBox.appendChild(maxPerMatchMax);
                    }
                    item.maxPerMatchElement = maxPerMatch;
                }

                box2.appendChild(maxPerMatchBox);

                // commision
                if (this.context.formId == 4) {
                    var agentCommBox = document.createElement("DIV");
                    agentCommBox.innerHTML = item.discount;
                    agentCommBox.className = "bettingBox-comm2";
                    box2.appendChild(agentCommBox);
                }
                else {
                    // Show label instead to TextBox
                    if (targetRoleId == 1) {
                        var agentCommBox = document.createElement("DIV");
                        agentCommBox.innerHTML = item.discountMax;
                        agentCommBox.className = "bettingBox-comm2";
                        box2.appendChild(agentCommBox);
                    }

                    var commMax = item.discountMax;
                    var commSelected = item.discount;
                    var commDropdown = Fanex.PTRacing.PT_RenderSelect(0, commMax, Fanex.PTRacing.COMMISSION_STEP, commSelected, false);
                    commDropdown.select.id = 'comm_' + sportInfo.id + '_' + bettypeGroup.id;
                    commDropdown.select.className = "betSettingCommDropdown " + bettingCommCopySettingClass + betSettingCopyComm;
                    if (targetRoleId == 1) {
                        commDropdown.className = "bettingBox-comm2";
                    }
                    else {
                        commDropdown.className = "bettingBox-comm";
                    }

                    box2.appendChild(commDropdown);
                    item.discountElement = commDropdown.select;
                }

                // enable
                // Show label instead to TextBox
                var enableBox = document.createElement("DIV");
                var enableCheckbox = document.createElement("INPUT");
                enableCheckbox.type = "checkbox";
                enableCheckbox.id = 'enable_' + sportInfo.id + '_' + bettypeGroup.id;
                enableCheckbox.className = bettingCommCopySettingClass + betSettingCopyStatus;
                enableCheckbox.setAttribute('oldStatus', !(item.disable))
                if (this.context.formId == 4 || (targetRoleId != 4 && item.disableUplines)) {
                    enableCheckbox.disabled = "disabled";
                }
                if (targetRoleId > 1) {
                    if (this.context.formId != 4) {
                        enableCheckbox.onclick = function () {
                            var currentStatus = this.checked;
                            var oldStatus = this.getAttribute("oldStatus") == "true" ? true : false;
                            var downline = document.getElementById('downlines_' + this.id);
                            var downlineOldStatus = downline.getAttribute('oldStatus') == "true" ? true : false;

                            if (currentStatus) {
                                if (downlineOldStatus) {
                                    downline.setAttribute('disabled', 'disabled');
                                    downline.checked = true;
                                }
                                else {
                                    downline.removeAttribute('disabled');
                                    downline.checked = false;
                                }
                            }
                            else {
                                downline.setAttribute('disabled', 'disabled');
                                downline.checked = false;
                            }
                        }
                    }
                    enableBox.appendChild(enableCheckbox);

                    enableBox.className = "bettingBox-status";
                    box2.appendChild(enableBox);
                    item.enableElement = enableCheckbox;

                    var enableDownlinesBox = document.createElement("DIV");
                    var enableDownlinesCheckbox = document.createElement("INPUT");
                    enableDownlinesCheckbox.type = "checkbox";
                    enableDownlinesCheckbox.id = 'downlines_enable_' + sportInfo.id + '_' + bettypeGroup.id;
                    enableDownlinesCheckbox.disabled = (item.disable || !item.disableDownlines) ? "disabled" : "";
                    enableDownlinesCheckbox.setAttribute('oldStatus', !(item.disableDownlines))
                    enableDownlinesCheckbox.className = bettingCommCopySettingClass + betSettingCopyStatus;
                    enableDownlinesBox.appendChild(enableDownlinesCheckbox);
                    enableDownlinesBox.className = "bettingBox-downlines";
                    box2.appendChild(enableDownlinesBox);
                    enableDownlinesCheckbox.checked = !(item.disableDownlines);
                    item.enableDownlinesElement = enableDownlinesCheckbox;
                    if (this.context.formId == 4) {
                        enableDownlinesBox.disabled = "disabled";
                    }
                }
                else {
                    enableBox.appendChild(enableCheckbox);
                    enableBox.className = "bettingBox-status2";
                    box2.appendChild(enableBox);
                    item.enableElement = enableCheckbox;
                }
                enableCheckbox.checked = !(item.disable);
                item.element = box2;
                bettypeGroupBox.appendChild(box2);
            }

            var sportBoxHeight = ((totalBettypeGroups * divHeight) + totalBettypeGroups - 1) + 'px';
            sportHeader.innerHTML = sportInfo.name;
            sportHeader.title = sportInfo.name;
            sportHeader.className = "bettingBox-sport";
            sportHeader.style.height = sportBoxHeight;
            sportHeader.style.lineHeight = sportBoxHeight;

            sportBox.appendChild(bettypeGroupBox);
            var clear = document.createElement("DIV");
            clear.style.clear = "both";
            sportBox.appendChild(clear);

            box.appendChild(sportBox);

        }

        box.id = "divBettingCommSettingMainBox";
        if (this.context.formId == 4) {
            box.className = "bettingBox5";
        }
        else {
            if (targetRoleId == 1) {
                box.className = "bettingBox4";
            }
            else {
                box.className = "bettingBox";
            }
        }
        if (typeof (owner) == 'object') {

            owner.appendChild(box);
        }
        return box;
    },
    this.RenderCrossLevel = function (owner) {
        var targetRoleId = this.context.targetRoleId;
        var roleId = this.context.roleId;
        var divHeight = 24;

        var box = document.createElement("DIV");
        // Create MinBet, MaxBet, MaxPerMatch header
        var emptyBox = document.createElement("DIV");
        emptyBox.innerHTML = "&nbsp;";
        emptyBox.className = "bettingBox-header-sport";
        box.appendChild(emptyBox); //pool

        var emptyBox2 = document.createElement("DIV");
        emptyBox2.innerHTML = "&nbsp;";
        emptyBox2.className = "bettingBox-header-tote";
        box.appendChild(emptyBox2); //tote

        /*bet setting*/
        var betSettingHeaderBox = document.createElement("DIV");
        betSettingHeaderBox.className = "headerBetSettingCrossLevelBox";

        var betSettingMergeTitleBox = document.createElement("DIV");
        betSettingMergeTitleBox.id = "betsettingheader";
        betSettingMergeTitleBox.className = "bettingBox-header-bsbox2";
        betSettingMergeTitleBox.innerHTML = Language.BETTING_LIMIT;
        betSettingHeaderBox.appendChild(betSettingMergeTitleBox);

        var betSettingTitleBox = document.createElement("DIV");

        var minBetHeader = document.createElement("DIV");
        minBetHeader.innerHTML = Language.MIN_BET;
        minBetHeader.className = "bettingBox-header-minbet2";
        betSettingTitleBox.appendChild(minBetHeader);

        var maxBetHeader = document.createElement("DIV");
        maxBetHeader.innerHTML = Language.MAX_BET;
        maxBetHeader.className = "bettingBox-header-maxbet2";
        betSettingTitleBox.appendChild(maxBetHeader);

        var maxPerMatchHeader = document.createElement("DIV");
        maxPerMatchHeader.innerHTML = Language.MAX_PER_MATCH;
        maxPerMatchHeader.className = "bettingBox-header-maxpermatch2";
        betSettingTitleBox.appendChild(maxPerMatchHeader);

        betSettingHeaderBox.appendChild(betSettingTitleBox);

        box.appendChild(betSettingHeaderBox);
        /*end bet setting*/


        /*comm*/
        var commHeaderBox = document.createElement("DIV");
        commHeaderBox.className = "bettingBox-header-commbox";

        var commMergeTitleBox = document.createElement("DIV");
        commMergeTitleBox.id = "commheader";
        commMergeTitleBox.className = "bettingBox-header-comm3";
        commMergeTitleBox.innerHTML = Language.COMMISSION;
        commHeaderBox.appendChild(commMergeTitleBox);

        var commTitleBox = document.createElement("DIV");
        if (roleId == 4) {
            box.className = "bettingBox2";
            var masterCommHeader = document.createElement("DIV");
            masterCommHeader.innerHTML = Language.MASTER;
            masterCommHeader.className = "bettingBox-header-roletitle";
            commTitleBox.appendChild(masterCommHeader);
        }
        else {
            box.className = "bettingBox3";
            commHeaderBox.className = "bettingBox-header-commbox2";
            commMergeTitleBox.className = "bettingBox-header-comm2";
        }

        var agentCommHeader = document.createElement("DIV");
        agentCommHeader.innerHTML = Language.AGENT;
        agentCommHeader.className = "bettingBox-header-roletitle";
        commTitleBox.appendChild(agentCommHeader);

        var memberCommHeader = document.createElement("DIV");
        memberCommHeader.innerHTML = Language.MEMBER;
        memberCommHeader.className = "bettingBox-header-roletitle";
        commTitleBox.appendChild(memberCommHeader);

        commHeaderBox.appendChild(commTitleBox);

        box.appendChild(commHeaderBox);
        /*end comm*/

        /*status*/
        var statusHeaderBox = document.createElement("DIV");
        statusHeaderBox.className = "headerStatusBoxForMember";
        var statusHeaderMergeBox = document.createElement("DIV");
        statusHeaderMergeBox.innerHTML = Language.STATUS;
        statusHeaderMergeBox.id = "statusheader";
        statusHeaderMergeBox.className = "bettingBox-header-status2";
        statusHeaderBox.appendChild(statusHeaderMergeBox);
        box.appendChild(statusHeaderBox);
        /*status*/

        var clear = document.createElement("DIV");
        clear.style.clear = "both";
        box.appendChild(clear);

        var c = this.items.length;

        for (var i = 0; i < c; i++) {
            var sportInfo = this.items[i];
            var bettypeGroups = sportInfo.betTypeGroups;
            var totalBettypeGroups = bettypeGroups.length;

            var sportBox = document.createElement("DIV");
            sportBox.className = "bettingBox-sportbox";

            var sportHeader = document.createElement("DIV");
            var sportBoxHeight = ((totalBettypeGroups * divHeight) + totalBettypeGroups - 1) + 'px';
            sportHeader.innerHTML = sportInfo.name;
            sportHeader.title = sportInfo.title;
            sportHeader.className = "bettingBox-sport";
            sportHeader.style.height = sportBoxHeight;
            sportHeader.style.lineHeight = sportBoxHeight;
            sportBox.appendChild(sportHeader);

            var bettypeGroupBox = document.createElement("DIV");
            var ieMode = this.checkIEModeVersion();
            bettypeGroupBox.className = (ieMode && ieMode < 7) ? 'float-left' : '';
            for (var j = 0; j < totalBettypeGroups; j++) {
                var bettypeGroup = bettypeGroups[j];
                var item = bettypeGroup.bettingCommSetting;

                var box2 = document.createElement("DIV");
                box2.className = "bettingBox-row float-left";
                if (typeof (item.minBet) == 'undefined') { // For Add new case
                    item.minBet = 0; //item.minBetMax;
                }
                if (typeof (item.maxBet) == 'undefined') { // For Add new case
                    item.maxBet = 0; // item.maxBetMin;
                }
                if (typeof (item.maxPerMatch) == 'undefined') { // For Add new case
                    item.maxPerMatch = 0; // item.maxPerMatchMax;
                }

                var bettingCommCopySettingClass = " bettingCommCopySetting ";
                var betSettingCopyStatus = "betSettingCopyStatus ";
                var betSettingCopyComm = "betSettingCopyComm ";

                var header = document.createElement("DIV");
                var divHeader = document.createElement("DIV");
                divHeader.innerHTML = bettypeGroup.name;
                header.title = bettypeGroup.title;
                divHeader.className = 'float-left';
                header.className = "bettingBox-tote";
                header.id = "divBettingComm_BettypeGroup_" + sportInfo.id + "_" + bettypeGroup.id;
                header.appendChild(divHeader);
                box2.appendChild(header);

                // min bet                
                var minBetBox = document.createElement("DIV");
                minBetBox.className = "bettingBox-minbet2";
                minBetBox.innerHTML = ''.FormatNumber(item.minBet);
                box2.appendChild(minBetBox);

                // max bet
                var maxBetBox = document.createElement("DIV");
                maxBetBox.className = "bettingBox-maxbet2";
                maxBetBox.innerHTML = ''.FormatNumber(item.maxBet);
                box2.appendChild(maxBetBox);


                // Max per match
                var maxPermatchBox = document.createElement("DIV");
                maxPermatchBox.className = "bettingBox-maxpermatch2";
                maxPermatchBox.innerHTML = ''.FormatNumber(item.maxPerMatch);
                box2.appendChild(maxPermatchBox);


                // commision
                if (roleId == 4) {
                    var masterCommBox = document.createElement("DIV");
                    masterCommBox.innerHTML = item.masterDiscount;
                    masterCommBox.className = "bettingBox-comm2";
                    box2.appendChild(masterCommBox);
                }

                var agentCommBox = document.createElement("DIV");
                agentCommBox.innerHTML = item.agentDiscount;
                agentCommBox.className = "bettingBox-comm2";
                box2.appendChild(agentCommBox);

                var memberCommBox = document.createElement("DIV");
                memberCommBox.innerHTML = item.discount;
                memberCommBox.className = "bettingBox-comm2";
                box2.appendChild(memberCommBox);

                item.element = box2;
                bettypeGroupBox.appendChild(box2);


                //status
                var enableBox = document.createElement("DIV");
                var enableCheckbox = document.createElement("INPUT");
                enableCheckbox.type = "checkbox";
                enableCheckbox.id = 'enable_' + sportInfo.id + '_' + bettypeGroup.id;

                enableCheckbox.disabled = "disabled";
                enableCheckbox.className = bettingCommCopySettingClass + betSettingCopyStatus;
                enableCheckbox.setAttribute('oldStatus', !(item.disable))

                enableBox.appendChild(enableCheckbox);
                enableBox.className = "bettingBox-status2";
                box2.appendChild(enableBox);
                enableCheckbox.checked = !(item.disable);

            }

            sportBox.appendChild(bettypeGroupBox);
            var clear = document.createElement("DIV");
            clear.style.clear = "both";
            sportBox.appendChild(clear);

            box.appendChild(sportBox);

        }

        box.id = "divBettingCommSettingMainBox";
        if (typeof (owner) == 'object') {
            owner.appendChild(box);
        }
        return box;
    }
    this.getRoleNameById = function (roleId) {
        var roleName = Language.MEMBER;
        switch (roleId) {
            case 4:
                roleName = Language.SUPER;
                break;
            case 3:
                roleName = Language.MASTER;
                break;
            case 2:
                roleName = Language.AGENT;
                break;
            case 1:
                roleName = Language.MEMBER;
                break;
            default:
                break;
        }

        return roleName;
    }
    this.checkIEModeVersion = function () {
        engine = null;
        if (window.navigator.appName == "Microsoft Internet Explorer") {
            // This is an IE browser. What mode is the engine in?
            if (document.documentMode) // IE8 or later
                engine = document.documentMode;
            else // IE 5-7
            {
                engine = 5; // Assume quirks mode unless proven otherwise
                if (document.compatMode) {
                    if (document.compatMode == "CSS1Compat")
                        engine = 7; // standards mode
                }
                // There is no test for IE6 standards mode because that mode  
                // was replaced by IE7 standards mode; there is no emulation.
            }
            // the engine variable now contains the document compatibility mode.
        }

        return engine;

    }
};
