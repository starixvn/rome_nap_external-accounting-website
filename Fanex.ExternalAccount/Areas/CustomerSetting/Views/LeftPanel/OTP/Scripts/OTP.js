customerOTP = {
    initialize: function () {
        $("#btnSubmit").click(function () {
            customerOTP.confirmRemoveOTP();
        });
    },
    confirmRemoveOTP: function () {
        msg = $("#removingOTPConfirm").val();
        title = "Remove OTP";
        parent.Fanex.ExtAcc.Form.ConfirmBox(title, msg, 450, 130, false, function () {
            customerOTP.removeOTP();
            parent.Fanex.ExtAcc.Form.Close();
        });
    },
    removeOTP: function () {
        try { Fanex.CustomerSetting.Core.BlockUI(); } catch (e) { console.log(e); }
        var $errorMessage = $("#errMsg");
        $.ajax({
            type: "POST",
            url: parent.resolveClientUrl('CustomerSetting/LeftPanel/UpdateCustomer/RemoveOTP'),
            data: { custId: $("#customerId").val(), groupName: parent.$('#selectedGroupName').html() },
            success: function (result) {

                if (parent.Fanex.ExtAcc.ShowSecurityCodePage(result, Fanex.CustomerSetting.Core.UnBlockUI)) {
                    return;
                }

                if (result.isSuccess) {
                    $errorMessage.attr('class', 'successMsg');
                    $errorMessage.empty().html('<font color="Blue">' + $("#removeOTPsuccessfully").val() + '</font>');
                    $errorMessage.show();
                }
                else {
                    $errorMessage.attr('class', 'errMsg');
                    $errorMessage.empty().html(result.errorMessage.formatErrorMessage());
                    $errorMessage.show();
                }
            },
            error: function () {
                Fanex.CustomerSetting.Core.UnBlockUI();
            },
            complete: function () {
                try { Fanex.CustomerSetting.Core.UnBlockUI(); } catch (e) { }
            }
        });
    }
};

$(function () {
    customerOTP.initialize();
});
