$(document).ready(function () {
    var username = $("#_username").val();
    var reset = $("#_reset").val() == "true";

    if (!reset && localStorage.getItem(username + "_pt") != null
        && localStorage.getItem(username + "_pt") != ""
        && localStorage.getItem(username + "_cd") != null
        && localStorage.getItem(username + "_cd") != "") {
        $("#_pt").val(localStorage.getItem(username + "_pt"));
        $("#_cd").val(localStorage.getItem(username + "_cd"));
        $("#otp-form").submit();
    }
    else {
        $("#otp-form").removeClass("hidden");
    }
});