(function ($) {
    var buttonSubmit,
        spanProtectedStatus;

    function changeAllProtectedStatus(changeStatusFunc) {
        var disableSubmit = true;
        $.each(spanProtectedStatus, function () {
            var $this = $(this);
            changeStatusFunc($this);
            if ($this.attr("data-origin") !== $this.attr("data-new")) {
                disableSubmit = false;
            }
        });

        disableSubmitButton(disableSubmit);
    }

    function changeProtectedStatus() {
        var $this = $(this);
        var isProtected = $this.hasClass("suspend-switch");

        if (isProtected) {
            changeToUnprotected($this);
        } else {
            changeToProtected($this);
        }

        var disableSubmit = !isFormDataChanged();
        disableSubmitButton(disableSubmit);
    }

    function changeToProtected(element) {
        element
            .removeClass("unsuspend-switch")
            .addClass("suspend-switch")
            .attr("data-new", 1)
            .text("Protected");
    }

    function changeToUnprotected(element) {
        element
            .removeClass("suspend-switch")
            .addClass("unsuspend-switch")
            .attr("data-new", 0)
            .text("Unprotected");
    }

    function disableSubmitButton(disabled) {
        buttonSubmit.prop("disabled", disabled)
        if (disabled) {
            buttonSubmit.addClass("ui-state-disabled");
        } else {
            buttonSubmit.removeClass("ui-state-disabled");
        }
    }

    function isFormDataChanged() {
        for (var i = 0, max = spanProtectedStatus.length; i < max; i++) {
            var span = $(spanProtectedStatus[i]);
            if (span.attr("data-origin") !== span.attr("data-new")) {
                return true;
            }
        }

        return false;
    }

    function getFormData() {
        var protectedStatus = [];
        $.each(spanProtectedStatus, function () {
            var $this = $(this);
            if ($this.attr("data-origin") !== $this.attr("data-new")) {
                protectedStatus.push({
                    custId: $this.data("custid"),
                    username: $this.data("username"),
                    roleId: $this.data("roleid"),
                    isProtectedMode: $this.hasClass("suspend-switch")
                });
            }
        });

        return {
            groupName: parent.Fanex.ExtAcc.Customer.GroupName().groupName,
            protectedStatus: protectedStatus
        };
    }

    function editSuccess(data) {
        var isSuccess = true;
        var messages = [];
        if (Array.isArray(data)) {
            $.each(data, function (index, value) {
                if (!value.IsSuccess && isSuccess) {
                    isSuccess = false;
                }

                messages.push('<div class="');
                messages.push(value.IsSuccess ? 'successMsg' : 'errMsg');
                messages.push('">');
                messages.push(value.ErrorMessage);
                messages.push('</div>');
            })
        } else {
            isSuccess = false;
            messages.push('<div class="errMsg">');
            messages.push(data.errorMessage || data.ErrorMessage);
            messages.push('</div>');
        }

        $("#msg-container")
            .html(messages.join(""))
            .removeClass("ui-helper-hidden");

        if (isSuccess) {
            setTimeout(function () {
                parent.Fanex.ExtAcc.Form.Close();
            }, 3000);
        }
    }

    function cacheElements() {
        spanProtectedStatus = $('.protected-status');
        buttonSubmit = $("#btn-submit");
    }

    function bindEvents() {
        $.ajaxSetup({
            beforeSend: function (xhr) {
                var antiXSRFToken = document.getElementsByName("__RequestVerificationToken");
                if (antiXSRFToken.length) {
                    xhr.setRequestHeader("__RequestVerificationToken", antiXSRFToken[0].value);
                }
            }
        });

        $("#protected-all").click(function () {
            changeAllProtectedStatus(changeToProtected);
        });

        $("#unprotected-all").click(function () {
            changeAllProtectedStatus(changeToUnprotected);
        });

        spanProtectedStatus.click(changeProtectedStatus);

        $("#btn-reset").click(function () {
            parent.$("#targetProtectedStatusForm").submit();
        });

        buttonSubmit.click(function () {
            var confirmPopupId = "popupConfirmEditMultipleProtectedStatus";
            parent.Fanex.ExtAcc.Form.ConfirmBox(
                "Edit Multiple Protection Status",
                "Are you sure you want to change the Protection Status for selected account(s)?\nProtection Status should just be turned on in emergency situations.",
                480,
                "auto",
                false,
                function () {
                    parent.Fanex.ExtAcc.BlockUI();
                    $.post(parent.resolveClientUrl("ProtectedStatus/EditMultiple"), getFormData(), editSuccess)
                        .always(function () {
                            parent.Fanex.ExtAcc.Form.Close(null, null, confirmPopupId);
                            parent.Fanex.ExtAcc.UnBlockUI();
                        });
                },
                null,
                confirmPopupId);
        });
    }

    $(function () {
        cacheElements();
        bindEvents();
    })
})(jQuery);