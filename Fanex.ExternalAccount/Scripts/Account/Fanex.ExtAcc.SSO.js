if (typeof Fanex === 'undefined') {
    Fanex = {};
}

if (typeof Fanex.ExtAcc === 'undefined') {
    Fanex.ExtAcc = {};
}

Fanex.ExtAcc.SSO = {
    imageAddress: "/assets/styles/images/sample_test_network_speed_1.jpg",
    imageDownloadSize: 4995374, // bytes
    domains: [],
    maxSpeedsLength: 5,
    addedErrorTime: 1000 * 60 * 5, // 5 minutes

    Init: function () {
        Fanex.ExtAcc.SSO.InitDropdownList();

        for (var i = 0, max = Fanex.ExtAcc.SSO.domains.length; i < max; i++) {
            Fanex.ExtAcc.SSO.MeasureConnectionSpeed(Fanex.ExtAcc.SSO.domains[i]);
        }
    },

    InitDropdownList: function () {
        $("#ddl-switchLine")
            .select2({
                minimumResultsForSearch: -1,
                templateResult: function (line) {
                    if (!line.id) {
                        return null;
                    }

                    var $origOption = $(line.element);
                    return $('<span>' + line.text + '<span class="' + $origOption.data("classname") + '"></span></span>');
                }
            })
            .find("option").each(function () {
                Fanex.ExtAcc.SSO.domains.push({
                    id: this.value,
                    speeds: [],
                    index: 0,
                    average: 0
                });
            });
    },

    MeasureConnectionSpeed: function (domain) {
        var startTime;
        var download = new Image();
        download.onload = function () {
            var endTime = (new Date()).getTime();
            Fanex.ExtAcc.SSO.OnMeasureConnectionSpeedSuccess(domain, startTime, endTime);
        };

        download.onerror = function () {
            var endTime = startTime + Fanex.ExtAcc.SSO.addedErrorTime;
            Fanex.ExtAcc.SSO.OnMeasureConnectionSpeedSuccess(domain, startTime, endTime);
        };

        startTime = (new Date()).getTime();
        var cacheBuster = "?v=" + startTime;
        download.src = domain.id + Fanex.ExtAcc.SSO.imageAddress + cacheBuster;
    },

    OnMeasureConnectionSpeedSuccess: function (domain, startTime, endTime) {
        var duration = (endTime - startTime) / 1000;
        var bitsLoaded = Fanex.ExtAcc.SSO.imageDownloadSize * 8;
        //var speedBps = (bitsLoaded / duration).toFixed(2);
        //var speedKbps = (speedBps / 1024).toFixed(2);
        //var speedMbps = (speedKbps / 1024).toFixed(2);
        var speedBps = bitsLoaded / duration;
        var speedKbps = speedBps / 1024;
        var speedMbps = speedKbps / 1024;

        Fanex.ExtAcc.SSO.UpdateConnectionSpeed(domain, speedMbps);
    },

    UpdateConnectionSpeed: function (domain, value) {
        if (domain.index >= Fanex.ExtAcc.SSO.maxSpeedsLength) {
            domain.index = 0;
        }

        domain.speeds[domain.index++] = value;
        domain.average = Fanex.ExtAcc.SSO.CalculateAverage(domain.speeds);

        console.dir(domain);
        setTimeout(function () {
            Fanex.ExtAcc.SSO.MeasureConnectionSpeed(domain);
        }, 1000 * 5)
    },

    CalculateAverage: function (values) {
        var total = 0;
        var max = values.length;

        for (var i = 0; i < max; i++) {
            total += values[i];
        }

        return total / max;
    }
}

$(function () {
    Fanex.ExtAcc.SSO.Init();
});