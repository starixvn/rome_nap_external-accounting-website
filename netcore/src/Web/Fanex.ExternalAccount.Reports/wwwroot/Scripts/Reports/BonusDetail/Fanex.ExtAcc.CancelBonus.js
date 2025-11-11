var CancelBonus = /** @class */ (function () {
    function CancelBonus($, callback) {
        this.resources = {
            popupTitle: "Cancel Bonus",
            confirmMessage: '<div id="div-msgCancelPending" class="hide"></div>Are you sure you want to cancel this Bonus (Ref. No.: {0})?'
        };
        this.$ = $;
        this.callback = callback;
    }
    CancelBonus.prototype.RegisterCancelPending = function (elements) {
        var self = this;
        elements
            .unbind("click")
            .bind("click", function () {
            var $this = self.$(this);
            var custId = $this.data("custid");
            var transId = $this.data("transid");
            Fanex.ExtAcc.Form.ConfirmBox(self.resources.popupTitle, self.resources.confirmMessage.replace("{0}", transId), 450, "auto", false, function () { return self.cancelPendingPost(custId, transId); }, null, "cancel-bonus-modal");
            self.buttonSubmit = self.$("#cancel-bonus-modal").next(".ui-dialog-buttonpane").find(".btn.-submit");
        });
    };
    CancelBonus.prototype.cancelPendingPost = function (custId, transId) {
        var _this = this;
        Fanex.ExtAcc.BlockUI();
        this.buttonSubmit.prop("disabled", true);
        this.$.post(resolveClientUrl("BonusDetail/CancelPending"), { custId: custId, transId: transId, groupName: this.$("#selectedGroupName").html() }, function (data) {
            var isSuccess = data.isSuccess || data.IsSuccess;
            if (!isSuccess) {
                _this.showError(data.errorMessage || data.ErrorMessage);
                _this.buttonSubmit.prop("disabled", false);
                _this.callback();
            }
            else {
                _this.showSuccess(data.ErrorMessage);
                setTimeout(function () {
                    _this.callback();
                    Fanex.ExtAcc.Form.Close(null, null, "cancel-bonus-modal");
                }, 1000 * 3);
            }
        })
            .always(Fanex.ExtAcc.UnBlockUI);
    };
    CancelBonus.prototype.showError = function (error) {
        this.$("#div-msgCancelPending").html(error).removeClass().addClass("errMsg");
    };
    CancelBonus.prototype.showSuccess = function (message) {
        this.$("#div-msgCancelPending").html(message).removeClass().addClass("successMsg");
    };
    CancelBonus.prototype.hideMessage = function () {
        this.$("#div-msgCancelPending").removeClass().addClass("hide").html("");
    };
    return CancelBonus;
}());
//# sourceMappingURL=Fanex.ExtAcc.CancelBonus.js.map