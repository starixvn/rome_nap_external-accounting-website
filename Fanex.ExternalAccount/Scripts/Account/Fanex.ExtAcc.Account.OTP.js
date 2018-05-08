$(document).ready(function () {
    var username = $("#_username").val();
    if (localStorage.getItem(username + "_pt") != null
        && localStorage.getItem(username + "_pt") != ""
        && localStorage.getItem(username + "_cd") != null
        && localStorage.getItem(username + "_cd") != "") {
        $("#_pt").val(localStorage.getItem(username + "_pt"));
        $("#_cd").val(localStorage.getItem(username + "_cd"));
        $("#otp-form").submit();
    }
    else {
        var failNumerous = parseInt(localStorage.getItem(username + "_revevify"));
        if (failNumerous  == 4)
        {
            window.location.href = $("#_loginUrl").val();
        }

        localStorage.setItem(username + "_revevify", $("#_reverify").val() == "true" ? failNumerous + 1 : 0);
        $("#otp-form").removeClass("hidden");
    }
});