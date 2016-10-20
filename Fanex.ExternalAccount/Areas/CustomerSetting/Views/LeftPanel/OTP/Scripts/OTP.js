customerOTP = {
    initialize: function () {
        $("#btnSubmit").click(function () {
            customerOTP.removeOTP();
        });
    },
    removeOTP: function () {
        try { Fanex.CustomerSetting.Core.BlockUI(); } catch (e) { }
        var $errorMessage = $("#errMsg");
        $.ajax({
            type: "POST",
            url: parent.resolveClientUrl('CustomerSetting/LeftPanel/SearchCustomer/RemoveOTP'),
            data: { custId: $("#customerId").val(), groupName: parent.$('#selectedGroupName').html() },
            success: function (result) {
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
