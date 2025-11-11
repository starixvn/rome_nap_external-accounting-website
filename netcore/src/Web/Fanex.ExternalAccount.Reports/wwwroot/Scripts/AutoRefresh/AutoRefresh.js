var AutoRefresh = {
    refreshTime: 0,
    currentTime: 0,
    refreshAction: null,
    isInit: false,
    noRefresh: 'No refresh',
    init: function (refreshAction) {
        AutoRefresh.refreshAction = refreshAction;

        $(".div-down").click(function () { AutoRefresh.openOptions(); });
        $(".div-refresh").click(function () { AutoRefresh.doRefresh(); });
        $(".count-down-item").click(function () {
            var time = $(this).attr('time');
            AutoRefresh.saveOption(parseInt(time, 0));
        });

        var defaultTime = 15;
        AutoRefresh.saveOption(defaultTime);
        AutoRefresh.runCountDown();
        AutoRefresh.isInit = true;

        var $this = $(this);
        $(document).delegate('html', 'click', function (e) {
            var $target = $(e.target);

            if (!$target.closest('.div-count-down').length && !$(".panel-options").hasClass('hide')) {
                AutoRefresh.openOptions();
            }
        });
    },
    openOptions: function () {
        var isHide = $(".panel-options").hasClass('hide');
        if (isHide) {
            $(".panel-options").removeClass('hide');
        } else {
            $(".panel-options").addClass('hide');
        }
    },
    saveOption: function (time) {

        $(".panel-options").addClass('hide');
        $(".count-down-item").removeClass('menuitemcheck');
        $(".count-down-item[time='" + time + "']").addClass('menuitemcheck');

        var txt = time || AutoRefresh.noRefresh;
        $(".div-refresh").html(txt);

        AutoRefresh.refreshTime = time;
        AutoRefresh.currentTime = time;

        if (time === 0) {
            AutoRefresh.stop();
        } else {
            AutoRefresh.runCountDown();
        }
    },
    runCountDown: function () {
        $(".div-refresh").html(AutoRefresh.currentTime);

        AutoRefresh.currentTime--;

        if (window.top.countDownTimeout != null) {
            clearTimeout(window.top.countDownTimeout);
        }

        window.top.countDownTimeout = setTimeout(AutoRefresh.runCountDown, 1000);

        if (AutoRefresh.currentTime == -1) {
            AutoRefresh.doRefresh();
        }
    },
    doRefresh: function () {
        if (window.top.countDownTimeout != null) {
            clearTimeout(window.top.countDownTimeout);
        }

        $(".div-refresh").html("<span class='miniloading'>Loading</span>");

        if (AutoRefresh.refreshAction) {
            AutoRefresh.refreshAction(AutoRefresh.resetTime);
        } else {
            AutoRefresh.resetTime();
        }
    },
    resetTime: function () {
        if (AutoRefresh.refreshTime) {
            AutoRefresh.currentTime = AutoRefresh.refreshTime;
            AutoRefresh.runCountDown();
        } else {
            $(".div-refresh").html(AutoRefresh.noRefresh);
        }
    },
    stop: function () {
        if (window.top.countDownTimeout != null) {
            clearTimeout(window.top.countDownTimeout);
        }
    }
}; 