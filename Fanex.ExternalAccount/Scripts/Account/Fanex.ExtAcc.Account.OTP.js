$(document).ready(function () {
    var username = $("#_username").val();

    if ($("#_md").val() == localStorage.getItem(username + "_md")
        && $("#_requiredInputOTP").val() == "false"
        && localStorage.getItem(username + "_pt") != null
        && localStorage.getItem(username + "_pt") != ""
        && localStorage.getItem(username + "_cd") != null
        && localStorage.getItem(username + "_cd") != "") {
        $("#_pt").val(localStorage.getItem(username + "_pt"));
        $("#_cd").val(localStorage.getItem(username + "_cd"));
        $("#_md").val(localStorage.getItem(username + "_md"));
        $("#otp-form").submit();
    }
    else {
        $("#otp-form").removeClass("hidden");
    }
});