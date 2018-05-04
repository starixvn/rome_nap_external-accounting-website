$(document).ready(function () {
    if ( localStorage.getItem("_pt") != null && localStorage.getItem("_pt") != "") {
        $("#_pt").val(localStorage.getItem("_pt"));
        $("#_cd").val(localStorage.getItem("_cd"));
        $("#otp-form").submit();
    }
});