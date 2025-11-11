(function ($) {
    $.fn.inputFilter = function (inputFilter) {
        return this.on("input keydown keyup mousedown mouseup select contextmenu drop", function () {
            if (inputFilter(this.value)) {
                this.oldValue = this.value;
                this.oldSelectionStart = this.selectionStart;
                this.oldSelectionEnd = this.selectionEnd;
            }
            else if (this.hasOwnProperty("oldValue")) {
                this.value = this.oldValue;
                this.setSelectionRange(this.oldSelectionStart, this.oldSelectionEnd);
            }
            else {
                this.value = "";
            }
        });
    };
}(jQuery));
var AddNewBonus = /** @class */ (function () {
    function AddNewBonus($, callback) {
        this.resources = {
            popupTitle: "Add New Bonus",
            usernamesRequiredError: "Member Username(s) is required.",
            usernamesNotAllowBackslashError: "You are not allowed to input backslash '\\' into the Username field.",
            usernamesTooManyError: "The function can only add new up to {0} usernames per request.",
            usernamesDuplicateError: "Duplicated usernames: {0}.",
            amountRequiredError: "Amount is required.",
            amountInvalidError: "Amount must be a positive number."
        };
        this.maxUsernamesLength = 100;
        this.$ = $;
        this.callback = callback;
    }
    AddNewBonus.prototype.RegisterAddNewBonus = function (button, popupAddNewBonus) {
        var _this = this;
        button.click(function () {
            Fanex.ExtAcc.Form.ConfirmBox(_this.resources.popupTitle, popupAddNewBonus.html(), 500, "auto", false, function () { return _this.addNewBonusPost(); }, null, "addnew-bonus-modal", true, null, true);
            _this.cacheElements();
            _this.bindEvents();
        });
    };
    AddNewBonus.prototype.cacheElements = function () {
        this.msgAddNewBonus = this.$("#div-msgAddNewBonus");
        this.formAddNewBonus = this.$("#form-addNewBonus");
        this.txtMembers = this.formAddNewBonus.find("#txt-MemberUsernames");
        this.txtAmount = this.formAddNewBonus.find("#txt-amount");
        this.groupName = this.$("#selectedGroupName").html();
        this.buttonSubmit = this.$("#addnew-bonus-modal").next(".ui-dialog-buttonpane").find(".btn.-submit");
    };
    AddNewBonus.prototype.bindEvents = function () {
        var _this = this;
        this.formAddNewBonus.unbind("submit").bind("submit", function (e) { e.preventDefault(); });
        this.txtMembers
            .unbind("change")
            .bind("change", function () {
            var membersValue = _this.$.trim(_this.txtMembers.val());
            if (membersValue.length > 0) {
                if (membersValue.indexOf("\n") >= 0) {
                    membersValue = membersValue.replace(/\n/g, " ");
                }
                if (membersValue.indexOf("\r") >= 0) {
                    membersValue = membersValue.replace(/\r/g, " ");
                }
                if (membersValue.indexOf("\t") >= 0) {
                    membersValue = membersValue.replace(/\t/g, " ");
                }
                membersValue = membersValue.replace(/\s/g, ",");
                var usernamesArray = membersValue.split(",").filter(function (item) { return item.length > 0; });
                _this.txtMembers.val(usernamesArray.join(","));
            }
        });
        this.txtAmount.inputFilter(function (value) {
            return /^-?\d*[.]?\d{0,2}$/.test(value);
        });
    };
    AddNewBonus.prototype.addNewBonusPost = function () {
        var _this = this;
        if (this.isValidMembers() && this.isValidAmount()) {
            Fanex.ExtAcc.BlockUI();
            this.buttonSubmit.prop("disabled", true);
            var formData = this.formAddNewBonus.serializeArray();
            formData.push({ name: "groupName", value: this.$("#selectedGroupName").html() });
            this.$.post(resolveClientUrl("BonusDetail/AddNew"), formData, function (data) {
                var isSuccess = data.isSuccess || data.IsSuccess;
                if (!isSuccess) {
                    _this.showError(data.errorMessage || data.ErrorMessage);
                    _this.buttonSubmit.prop("disabled", false);
                }
                else {
                    _this.showSuccess(data.ErrorMessage);
                    setTimeout(function () {
                        _this.callback();
                        Fanex.ExtAcc.Form.Close(null, null, "addnew-bonus-modal");
                    }, 1000 * 3);
                }
            }).always(Fanex.ExtAcc.UnBlockUI);
        }
    };
    AddNewBonus.prototype.isValidMembers = function () {
        var membersValue = this.$.trim(this.txtMembers.val());
        if (membersValue.length === 0) {
            this.txtMembers.focus();
            this.showError(this.resources.usernamesRequiredError);
            return false;
        }
        // Check usernames contains '\'.
        if (membersValue.indexOf("\\") >= 0) {
            this.txtMembers.focus();
            this.showError(this.resources.usernamesNotAllowBackslashError);
            return false;
        }
        var membersArray = membersValue.split(",");
        if (membersArray.length > this.maxUsernamesLength) {
            this.txtMembers.focus();
            this.showError(this.resources.usernamesTooManyError.replace("{0}", this.maxUsernamesLength.toString()));
            return false;
        }
        // Check duplicate.
        var duplicatedMembers = membersArray.filter(function (item, index) { return membersArray.indexOf(item) !== index; });
        if (duplicatedMembers.length > 0) {
            this.txtMembers.focus();
            this.showError(this.resources.usernamesDuplicateError.replace("{0}", duplicatedMembers.join(", ")));
            return false;
        }
        this.hideMessage();
        return true;
    };
    AddNewBonus.prototype.isValidAmount = function () {
        var amountValue = this.$.trim(this.txtAmount.val());
        if (amountValue.length === 0) {
            this.txtAmount.focus();
            this.showError(this.resources.amountRequiredError);
            return false;
        }
        amountValue = amountValue.replace(/\,/g, "");
        if (!this.isNumeric(amountValue)) {
            this.txtAmount.focus();
            this.showError(this.resources.amountInvalidError);
            return false;
        }
        var number = parseFloat(amountValue);
        if (number <= 0) {
            this.txtAmount.focus();
            this.showError(this.resources.amountInvalidError);
            return false;
        }
        this.hideMessage();
        return true;
    };
    AddNewBonus.prototype.isNumeric = function (text) {
        return text.match(/^-?(\d+|\d+\.\d*|\d*\.\d+)$/);
    };
    AddNewBonus.prototype.showError = function (error) {
        this.msgAddNewBonus.html(error).removeClass().addClass("errMsg");
    };
    AddNewBonus.prototype.showSuccess = function (message) {
        this.msgAddNewBonus.html(message).removeClass().addClass("successMsg");
    };
    AddNewBonus.prototype.hideMessage = function () {
        this.msgAddNewBonus.removeClass().addClass("hide").html("");
    };
    return AddNewBonus;
}());
//# sourceMappingURL=Fanex.ExtAcc.AddNewBonus.js.map