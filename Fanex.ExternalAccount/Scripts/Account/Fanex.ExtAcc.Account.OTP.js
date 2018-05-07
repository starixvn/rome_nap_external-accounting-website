$(document).ready(function () {
    var username = $("#_username").val();
    if (localStorage.getItem(username + "_pt") != null) {
        $("#_pt").val(localStorage.getItem(username + "_pt"));
        $("#otp-form").submit();
    }
    else {
        $("#otp-form").removeClass("hidden");
    }
});