if (typeof Fanex === 'undefined') {
    Fanex = {};
}

if (typeof Fanex.ExtAcc === 'undefined') {
    Fanex.ExtAcc = {};
}

Fanex.ExtAcc.SSO = {
    imageAddress: "/assets/styles/images/sample_test_network_speed_1.jpg",
    imageDownloadSize: 4995374, // bytes
    lines: [],
    maxSpeedsLength: 5,
    addedErrorTime: 1000 * 60 * 5, // 5 minutes

    Init: function () {
        Fanex.ExtAcc.SSO.InitDropdownList();

        for (var i = 0, max = Fanex.ExtAcc.SSO.lines.length; i < max; i++) {
            Fanex.ExtAcc.SSO.MeasureConnectionSpeed(Fanex.ExtAcc.SSO.lines[i]);
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
                Fanex.ExtAcc.SSO.lines.push({
                    id: this.id,
                    url: this.value,
                    speeds: [],
                    index: 0,
                    average: 0
                });
            });
    },

    MeasureConnectionSpeed: function (line) {
        var startTime;
        var download = new Image();
        download.onload = function () {
            var endTime = (new Date()).getTime();
            Fanex.ExtAcc.SSO.OnMeasureConnectionSpeedSuccess(line, startTime, endTime);
        };

        download.onerror = function () {
            var endTime = startTime + Fanex.ExtAcc.SSO.addedErrorTime;
            Fanex.ExtAcc.SSO.OnMeasureConnectionSpeedSuccess(line, startTime, endTime);
        };

        startTime = (new Date()).getTime();
        var cacheBuster = "?v=" + startTime;
        download.src = line.url + Fanex.ExtAcc.SSO.imageAddress + cacheBuster;
    },

    OnMeasureConnectionSpeedSuccess: function (line, startTime, endTime) {
        var duration = (endTime - startTime) / 1000;
        var bitsLoaded = Fanex.ExtAcc.SSO.imageDownloadSize * 8;
        //var speedBps = (bitsLoaded / duration).toFixed(2);
        //var speedKbps = (speedBps / 1024).toFixed(2);
        //var speedMbps = (speedKbps / 1024).toFixed(2);
        var speedBps = bitsLoaded / duration;
        var speedKbps = speedBps / 1024;
        var speedMbps = speedKbps / 1024;

        Fanex.ExtAcc.SSO.UpdateConnectionSpeed(line, speedMbps);
    },

    UpdateConnectionSpeed: function (line, value) {
        if (line.index >= Fanex.ExtAcc.SSO.maxSpeedsLength) {
            line.index = 0;
        }

        line.speeds[line.index++] = value;
        line.average = Fanex.ExtAcc.SSO.CalculateSpeedAverage(line.speeds);
        $("#" + line.id).attr("data-speed", line.average);

        console.dir(line);
        setTimeout(function () {
            Fanex.ExtAcc.SSO.MeasureConnectionSpeed(line);
        }, 1000 * 5);
    },

    CalculateSpeedAverage: function (values) {
        var total = 0;
        var max = values.length;

        for (var i = 0; i < max; i++) {
            total += values[i];
        }

        return Math.round(total / max);
    }
}

$(function () {
    Fanex.ExtAcc.SSO.Init();
});