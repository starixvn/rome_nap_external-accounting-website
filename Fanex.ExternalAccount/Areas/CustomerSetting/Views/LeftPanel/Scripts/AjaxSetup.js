$(document).ready(function () {
    $.ajaxSetup({
        beforeSend: function (xhr) {
            var antiXSRFToken = document.getElementsByName("__RequestVerificationToken");
            if (antiXSRFToken.length > 0) {
                xhr.setRequestHeader("__RequestVerificationToken", antiXSRFToken[0].value);
            }
        },
        error: function () {
            parent.Fanex.ExtAcc.UnBlockUI();
        },
        statusCode: {
            500: function () {
                parent.Fanex.ExtAcc.Form.Close();
                parent.Fanex.ExtAcc.LoadMainFrame(resolveClientUrl('Shared/Error'));
            },
            401: function (error, callback) {
                if (JSON && error.responseText) {
                    var errorResult = JSON.parse(error.responseText);
                    if (errorResult.code === 999999) {
                        top.location.href = parent.resolveClientUrl('Shared/Offline');
                    }
                    if (errorResult.code == 401) {
                        top.location.href = parent.resolveClientUrl('Account/Login');
                    }
                }
            },
            503: function (error, callback) {
                if (JSON && error.responseText) {
                    var errorResult = JSON.parse(error.responseText);
                    if (errorResult.code === 999999) {
                        top.location.href = parent.resolveClientUrl('Shared/Offline');
                    }
                }
            }
        }
    });
});