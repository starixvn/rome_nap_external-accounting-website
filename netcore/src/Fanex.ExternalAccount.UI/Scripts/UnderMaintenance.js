var reLoginId = null;

$(document).ready(function () {
    ReCheckinOffline(); // call immediately after page load complete
    reLoginId = setInterval('ReCheckinOffline();', 45000); //call kickout every 10s
});

function ReCheckinOffline() {
    $.ajax({
        url: resolveClientUrl('Shared/ReCheckInOffline'),
        type: 'GET',
        dataType: 'html',
        cache: false,
        success: function (data) {
            if (data == 3) {
                clearInterval(reLoginId);
                navigateToUrl('Account/Login');
            }
        }
    });
}