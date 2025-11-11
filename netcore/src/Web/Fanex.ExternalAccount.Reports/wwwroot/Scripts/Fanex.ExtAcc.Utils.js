function disablePulldownAndReleaseRefresh() {
    var maybePreventPullToRefresh = false;
    var lastTouchY = 0;
    var touchstartHandler = function (e) {
        if (e.touches.length != 1) return;
        lastTouchY = e.touches[0].clientY;
        // Pull-to-refresh will only trigger if the scroll begins when the
        // document's Y offset is zero.
        maybePreventPullToRefresh =
            window.pageYOffset == 0;
    }

    var touchmoveHandler = function (e) {
        var touchY = e.touches[0].clientY;
        var touchYDelta = touchY - lastTouchY;
        lastTouchY = touchY;

        if (maybePreventPullToRefresh) {
            // To suppress pull-to-refresh it is sufficient to preventDefault the
            // first overscrolling touchmove.
            maybePreventPullToRefresh = false;
            if (touchYDelta > 0) {
                e.stopPropagation();
                return;
            }
        }
    }

    document.addEventListener('touchstart', touchstartHandler, { passive: false });
    document.addEventListener('touchmove', touchmoveHandler, { passive: false });
}

function exportExcel(url, currentWindow) {
    if (top.isMobile.phone || top.isMobile.tablet) {
        window.open(url);
    } else {
        currentWindow.location.href = url;
    }
}

function serialize(obj) {
    var str = [];
    for (var p in obj)
        if (obj.hasOwnProperty(p)) {
            str.push(encodeURIComponent(p) + "=" + encodeURIComponent(obj[p]));
        }
    return str.join("&");
}
