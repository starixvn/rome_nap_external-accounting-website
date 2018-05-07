$(document).ready(function () {
    var username = $("#_username").val();
    if (localStorage.getItem(username + "_pt") != null && localStorage.getItem(username + "_pt") != "") {
        $("#_pt").val(localStorage.getItem(username + "_pt"));
        $("#_cd").val(localStorage.getItem(username + "_cd"));
        $("#otp-form").submit();
    }
});