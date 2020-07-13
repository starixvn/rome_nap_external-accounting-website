$(document).ready(function () {
    $('#verySecurityCodeForm').submit(function (event) {
        event.preventDefault();

        var code = $('input#code').val();
        var returnUrl = $('input#returnUrl').val();

        $.post(resolveClientUrl('SecurityCode/Verify'), { code })
            .done(function (message) {
                if (message === 'valid') {
                    $('#securityCodePageContainer').html('');
                } else {
                    $('input#code').val('');
                    $("label#codeError").removeClass('hidden');
                }
            })
            .fail(function (error) {
                console.log(error);
            });
    });
});