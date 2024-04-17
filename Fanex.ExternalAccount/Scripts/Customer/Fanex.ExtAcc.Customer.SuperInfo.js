var SuperInfo = /** @class */ (function () {
    function SuperInfo($, topWindow) {
        this.groupId = "";
        this.groupName = "";
        this.scode = "";
        this.$ = $;
        this.topWindow = topWindow;
    }
    SuperInfo.prototype.Init = function (scode) {
        this.groupId = Fanex.ExtAcc.Customer.GetGroupId() || "";
        this.groupName = this.$("#selectedGroupName").html();
        this.superInfoList = this.$("#superInfoList");
        this.selectedSupers = this.$("#selectedSupers");
        this.btnSelectSupers = this.$("#btnSelectSupers");
        this.btnResetSelect = this.$("#btnResetSelect");
        this.updateSuperInfoContent = this.$("#updateSuperInfoContent");
        this.firstName = this.$("#SuperFirstName");
        this.lastName = this.$("#SuperLastName");
        this.phone = this.$("#SuperPhone");
        this.mobilePhone = this.$("#SuperMobilePhone");
        this.password = this.$("#SuperPassword");
        this.confirmPassword = this.$("#SuperConfirmPwd");
        this.secCode = this.$("#SuperSecCode");
        this.confirmSecCode = this.$("#SuperConfirmSecCode");
        this.forceChange = this.$("#SuperForceChange");
        this.btnSubmitUpdate = this.$("#btnSubmitUpdate");
        this.scode = scode;
        this.BindingEvents();
    };
    SuperInfo.prototype.BindingEvents = function () {
        var _this = this;
        var $this = this;
        this.superInfoList.blur(function () { return _this.ReplaceChars(); });
        this.btnSelectSupers.click(function () { return _this.CheckValidSuperList(); });
        this.btnResetSelect.click(function () { return _this.ResetSuperList(); });
        this.btnSubmitUpdate.click(function () { return _this.SubmitUpdate(); });
        this.$("#ckbPersonalInfo").change(function () { return _this.ShowHideTab("#updateInfoTab", _this.$("#ckbPersonalInfo").prop('checked') == true); });
        this.$("#ckbResetPwd").change(function () { return _this.ShowHideTab("#updatePwdTab", _this.$("#ckbResetPwd").prop('checked') == true); });
        this.$("#ckbResetSecCode").change(function () { return _this.ShowHideTab("#updateSecCodeTab", _this.$("#ckbResetSecCode").prop('checked') == true); });
        this.$("#ckbSuperFirstName").change(function () { return _this.EnableDisableEditField("#SuperFirstName", _this.$("#ckbSuperFirstName").prop('checked') == true); });
        this.$("#ckbSuperLastName").change(function () { return _this.EnableDisableEditField("#SuperLastName", _this.$("#ckbSuperLastName").prop('checked') == true); });
        this.$("#ckbSuperPhone").change(function () { return _this.EnableDisableEditField("#SuperPhone", _this.$("#ckbSuperPhone").prop('checked') == true); });
        this.$("#ckbSuperMobilePhone").change(function () { return _this.EnableDisableEditField("#SuperMobilePhone", _this.$("#ckbSuperMobilePhone").prop('checked') == true); });
        $("#notesPwd").tooltip();
        $("#notesSecCode").tooltip();
        $('[data-toggle="tooltip"]').tooltip();
        $('.ic-pwd-eye').click(function () { $this.HideUnHidePwd($(this)); });
    };
    SuperInfo.prototype.HideUnHidePwd = function (iConSpan) {
        var targetId = iConSpan.data("id");
        var currentType = $("#" + targetId).prop("type");
        if (currentType === "text") {
            $("#" + targetId).prop("type", "password");
            iConSpan.removeClass("ic-pwd-eye-visible-off").addClass("ic-pwd-eye-visible");
        }
        else {
            $("#" + targetId).prop("type", "text");
            iConSpan.removeClass("ic-pwd-eye-visible").addClass("ic-pwd-eye-visible-off");
        }
    };
    SuperInfo.prototype.ReplaceChars = function () {
        this.superInfoList.val(this.superInfoList.val()
            .replace(/\s\s/g, ' ')
            .replace(/\s/g, ',')
            .replace(/\t/g, ',')
            .replace(/;/g, ',')
            .replace(/\\n|\\r\\n|\\r/g, ',')
            .replaceAll(',,', ','));
        if (this.superInfoList.val().startsWith(',')) {
            this.superInfoList.val(this.superInfoList.val().substr(1));
        }
        if (this.superInfoList.val().endsWith(',')) {
            this.superInfoList.val(this.superInfoList.val().substr(0, this.superInfoList.val().length - 1));
        }
    };
    SuperInfo.prototype.ValidateSuperList = function (superList) {
        if (superList === "") {
            this.$("#errMsg").empty().html("Please input username of supers.").show();
            return false;
        }
        if (this.ContainsHTML(superList)) {
            this.$("#errMsg").empty().html("Invalid parameter value.").show();
            return false;
        }
        var usernameList = superList.split(",");
        if (usernameList.length > 50) {
            this.$("#errMsg").empty().html("The function can only edit up to 50 accounts per request.").show();
            return false;
        }
        var duplicateUsernames = [];
        for (var i = 0; i < usernameList.length; i++) {
            if (duplicateUsernames.indexOf(usernameList[i]) < 0 && i < usernameList.findLastIndex(function (un) { return un.toLowerCase() === usernameList[i].toLowerCase(); })) {
                duplicateUsernames.push(usernameList[i]);
            }
        }
        if (duplicateUsernames.length > 0) {
            this.$("#errMsg").empty().html("Duplicate super account(s): " + duplicateUsernames.join(', ')).show();
            return false;
        }
        return true;
    };
    SuperInfo.prototype.CheckValidSuperList = function () {
        var _this = this;
        this.selectedSuperIdWithUsername = null;
        var superList = this.superInfoList.val();
        this.$("#errMsg").empty().hide();
        this.selectedSupers.empty().hide();
        if (!this.ValidateSuperList(superList)) {
            return;
        }
        Fanex.ExtAcc.BlockUI();
        var groupName = Fanex.ExtAcc.Customer.GroupName().groupName;
        var params = "destusers=" + superList +
            "&groupname=" + this.groupName;
        var url = resolveClientUrl("SuperInfo/CheckMultipleSupers?");
        this.$.ajax({
            url: url + params,
            type: "post",
            dataType: "json",
            cache: false,
            success: function (data) {
                if (data.errorCode !== '0') {
                    _this.$("#errMsg").empty().html(data.errorMessage).show();
                }
                else {
                    _this.superInfoList.addClass("-disabled").attr("disabled", true);
                    _this.btnSelectSupers.addClass("-disabled").attr("disabled", true);
                    _this.btnResetSelect.removeClass("-disabled").attr("disabled", false);
                    _this.selectedSupers.empty().html(superList.split(',').join(', ')).show();
                    _this.updateSuperInfoContent.show();
                    _this.selectedSuperIdWithUsername = data.superIds;
                }
                Fanex.ExtAcc.UnBlockUI();
            }
        });
    };
    SuperInfo.prototype.ResetSuperList = function () {
        this.btnSelectSupers.removeClass("-disabled").attr("disabled", false);
        this.btnResetSelect.addClass("-disabled").attr("disabled", true);
        this.$("#errMsg").empty().removeClass("succMsg").addClass("errMsg").hide();
        this.superInfoList.removeClass("-disabled").attr("disabled", false).val("").focus();
        this.selectedSupers.empty().hide();
        this.selectedSuperIdWithUsername = null;
        this.updateSuperInfoContent.hide();
        $("#SuperFirstName").val("");
        $("#SuperLastName").val("");
        $("#SuperPhone").val("");
        $("#SuperMobilePhone").val("");
        $("#SuperPassword").val("");
        $("#SuperConfirmPwd").val("");
        $("#SuperSecCode").val("");
        $("#SuperConfirmSecCode").val("");
        this.$("#ckbPersonalInfo").prop("checked", true).trigger("change");
        this.$("#ckbResetPwd").prop("checked", true).trigger("change");
        this.$("#ckbResetSecCode").prop("checked", true).trigger("change");
        this.$("#ckbSuperFirstName").prop("checked", true).trigger("change");
        this.$("#ckbSuperLastName").prop("checked", true).trigger("change");
        this.$("#ckbSuperPhone").prop("checked", true).trigger("change");
        this.$("#ckbSuperMobilePhone").prop("checked", false).trigger("change");
        this.$("#SuperForceChange").prop("checked", true).trigger("change");
        this.$(".ic-pwd-eye").each(function () {
            var targetId = $(this).data("id");
            $("#" + targetId).prop("type", "password");
            $(this).removeClass("ic-pwd-eye-visible-off").addClass("ic-pwd-eye-visible");
        });
    };
    SuperInfo.prototype.EnableDisableBtnSubmit = function () {
        if ((this.$("#ckbPersonalInfo").prop('checked') && (this.$("#ckbSuperFirstName").prop('checked') || this.$("#ckbSuperLastName").prop('checked') || this.$("#ckbSuperPhone").prop('checked') || this.$("#ckbSuperMobilePhone").prop('checked'))) ||
            this.$("#ckbResetPwd").prop('checked') ||
            this.$("#ckbResetSecCode").prop('checked')) {
            this.btnSubmitUpdate.removeClass("-disabled").attr("disabled", false);
        }
        else {
            this.btnSubmitUpdate.addClass("-disabled").attr("disabled", true);
        }
    };
    SuperInfo.prototype.ShowHideTab = function (tabId, isShow) {
        this.EnableDisableBtnSubmit();
        if (isShow) {
            this.$(tabId).removeClass("hidden");
        }
        else {
            this.$(tabId).addClass("hidden");
        }
    };
    SuperInfo.prototype.EnableDisableEditField = function (fieldId, isEnable) {
        this.EnableDisableBtnSubmit();
        if (isEnable) {
            this.$(fieldId).removeClass("-disabled").prop("disabled", false);
        }
        else {
            this.$(fieldId).val("").addClass("-disabled").prop("disabled", true);
        }
    };
    SuperInfo.prototype.ValidateBeforUpdate = function () {
        var errorMessage = "";
        var errorField;
        if (this.$("#ckbPersonalInfo").prop('checked') == true) {
            if (this.$("#ckbSuperFirstName").prop('checked') && this.ContainsHTML($("#SuperFirstName").val())) {
                errorField = $('#SuperFirstName');
                errorMessage = 'Invalid parameter value.';
            }
            else if (this.$("#ckbSuperLastName").prop('checked') && this.ContainsHTML($("#SuperLastName").val())) {
                errorField = $('#SuperLastName');
                errorMessage = 'Invalid parameter value.';
            }
            else if (this.$("#ckbSuperPhone").prop('checked') && this.ContainsHTML($("#SuperPhone").val())) {
                errorField = $('#SuperPhone');
                errorMessage = 'Invalid parameter value.';
            }
            else if (this.$("#ckbSuperMobilePhone").prop('checked') && this.ContainsHTML($("#SuperMobilePhone").val())) {
                errorField = $('#SuperMobilePhone');
                errorMessage = 'Invalid parameter value.';
            }
            if (errorMessage !== "") {
                $("#errMsg").html(errorMessage).show();
                errorField.addClass("invalid-field").focus();
                return false;
            }
        }
        if (this.$("#ckbResetPwd").prop('checked') == true) {
            if (!Validators.validateInput($('#SuperPassword').val())) {
                errorField = $('#SuperPassword');
                errorMessage = 'Password is required';
            }
            else if (!this.ValidatePwdAgent($('#SuperPassword').val())) {
                errorField = $('#SuperPassword');
                errorMessage = 'Your password does not meet the minimum security requirement';
            }
            else if (!Validators.validateInput($('#SuperConfirmPwd').val())) {
                errorField = $('#SuperConfirmPwd');
                errorMessage = 'Confirm password is required';
            }
            else if ($('#SuperConfirmPwd').val() !== $('#SuperPassword').val()) {
                errorField = $('#SuperConfirmPwd');
                errorMessage = 'Confirm password is not matched';
            }
            if (errorMessage !== "") {
                $("#errMsg").html(errorMessage).show();
                errorField.addClass("invalid-field").focus();
                return false;
            }
        }
        if (this.$("#ckbResetSecCode").prop('checked') == true) {
            if (!Validators.validateInput($('#SuperSecCode').val())) {
                errorMessage = "Sorry, security code can't be empty";
                errorField = $('#SuperSecCode');
            }
            else if (!this.IsSecurityCode($('#SuperSecCode').val())) {
                errorMessage = "Sorry, security code is incorrect format";
                errorField = $('#SuperSecCode');
            }
            else if (!Validators.validateInput($('#SuperConfirmSecCode').val())) {
                errorMessage = "Sorry, confirm security code can't be empty";
                errorField = $('#SuperConfirmSecCode');
            }
            else if ($('#SuperSecCode').val() != $('#SuperConfirmSecCode').val()) {
                errorMessage = "Sorry, confirm security code doesn't match with security code";
                errorField = $('#SuperConfirmSecCode');
            }
            if (errorMessage !== "") {
                $("#errMsg").html(errorMessage).show();
                errorField.addClass("invalid-field").focus();
                return false;
            }
        }
        return true;
    };
    SuperInfo.prototype.SubmitUpdate = function () {
        var _this = this;
        this.$("#errMsg").empty().removeClass("succMsg").addClass("errMsg").hide();
        if (!this.ValidateBeforUpdate()) {
            return;
        }
        var requestData = {
            FirstName: $("#SuperFirstName").val(),
            IsChangeFirstName: this.$("#ckbSuperFirstName").prop('checked'),
            LastName: $("#SuperLastName").val(),
            IsChangeLastName: this.$("#ckbSuperLastName").prop('checked'),
            Phone: $("#SuperPhone").val(),
            IsChangePhone: this.$("#ckbSuperPhone").prop('checked'),
            MobilePhone: $("#SuperMobilePhone").val(),
            IsChangeMobilePhone: this.$("#ckbSuperMobilePhone").prop('checked'),
            Password: $('#SuperPassword').val().encrypt(this.scode),
            SecurityCode: $("#SuperSecCode").val(),
            IsForceChange: this.$("#SuperForceChange").prop('checked'),
            IsUpdateInfo: this.$("#ckbPersonalInfo").prop('checked'),
            IsUpdatePassword: this.$("#ckbResetPwd").prop('checked'),
            IsUpdateSecurityCode: this.$("#ckbResetSecCode").prop('checked'),
            SuperIdNameList: this.selectedSuperIdWithUsername
        };
        Fanex.ExtAcc.BlockUI();
        var url = resolveClientUrl("SuperInfo/UpdateMultipleSupers");
        this.$.ajax({
            url: url,
            type: "post",
            dataType: "json",
            data: requestData,
            cache: false,
            success: function (result) {
                if (result.errorMessage) {
                    _this.$("#errMsg").empty().html(result.errorMessage).show();
                }
                else if (_this.$.isArray(result) && result.length > 0) {
                    _this.$("#errMsg").empty().html(_this.BuildErrorUpdate(result)).show();
                }
                else {
                    var message = _this.selectedSuperIdWithUsername.split(',').length === 1 ? "This account has been successfully updated." : "All accounts have been successfully updated.";
                    _this.$("#errMsg").empty().html(message).removeClass("errMsg").addClass("succMsg").show();
                    setTimeout(function () {
                        _this.$("#errMsg").empty().removeClass("succMsg").addClass("errMsg").hide();
                    }, 1000 * 3);
                }
                Fanex.ExtAcc.UnBlockUI();
            }
        });
    };
    SuperInfo.prototype.ValidatePwdAgent = function (str) {
        var regex1 = /^(?=.*\d).{8,100}$/;
        var regex2 = /^(?=.*[a-z]).{8,100}$/;
        var regex3 = /^(?=.*[A-Z]).{8,100}$/;
        var regex4 = /^(?=.*[\[\]\^\$\.\|\?\*\+\(\)\\~`\!@#%&\-_+={}'""<>:;,]).{8,100}$/;
        var whiteSpace = /^\S*$/;
        var case1 = str.search(regex1) != -1 ? 1 : 0;
        var case2 = str.search(regex2) != -1 ? 1 : 0;
        var case3 = str.search(regex3) != -1 ? 1 : 0;
        var case4 = str.search(regex4) != -1 ? 1 : 0;
        var case5 = str.search(whiteSpace) != -1;
        return (case1 + case2 + case3 + case4 >= 3) && case5;
    };
    SuperInfo.prototype.IsSecurityCode = function (secCode) {
        var secCodeLength = secCode.length;
        if (secCodeLength != 6) { // check length
            return false;
        }
        var regex = /^([0-9])(?!\1+$)[0-9]+$/; // check digits and all of them are not the same
        var result = secCode.search(regex) != -1 ? true : false;
        if (result) { // check 'Forward Run' and 'Backward Run'
            var isForwardRun = true;
            var isBackwardRun = true;
            var forward = 0;
            var backward = 0;
            var forward = parseInt(secCode.charAt(0));
            var backward = parseInt(secCode.charAt(0));
            for (var i = 1; i < secCodeLength; i++) {
                var current = parseInt(secCode.charAt(i));
                if (++forward != current) {
                    isForwardRun = false;
                }
                if (--backward != current) {
                    isBackwardRun = false;
                }
            }
            result = !(isForwardRun || isBackwardRun);
        }
        return result;
    };
    SuperInfo.prototype.BuildErrorUpdate = function (errorList) {
        var updateInfoError = "";
        var updatePwdError = "";
        var updateCodeError = "";
        for (var i = 0; i < errorList.length; i++) {
            var updateError = errorList[i];
            if (updateError.errCode === 1) {
                updateInfoError += "    " + updateError.resultData + ": " + updateError.errMsg + " </br>";
            }
            if (updateError.errCode === 2) {
                updatePwdError += updateError.resultData + ": " + updateError.errMsg + " </br>";
            }
            if (updateError.errCode === 3) {
                updateCodeError += updateError.resultData + ": " + updateError.errMsg + " </br>";
            }
        }
        var errorMessage = "";
        if (updateInfoError !== "") {
            errorMessage += "Update Personal Info:</br>" + updateInfoError;
        }
        if (updatePwdError !== "") {
            errorMessage += "Reset Password:</br>" + updatePwdError;
        }
        if (updatePwdError !== "") {
            errorMessage += "Reset Security Code:</br>" + updatePwdError;
        }
        return errorMessage;
    };
    SuperInfo.prototype.ContainsHTML = function (str) {
        return str.match(/^<(?:\"[^\"]*\"['\"]*|'[^']*'['\"]*|[^'\">])+>/);
    };
    return SuperInfo;
}());
$(document).ready(function () {
    var superInfoController = new SuperInfo($, window.top);
    superInfoController.Init($("#SCode").val());
});
//# sourceMappingURL=Fanex.ExtAcc.Customer.SuperInfo.js.map